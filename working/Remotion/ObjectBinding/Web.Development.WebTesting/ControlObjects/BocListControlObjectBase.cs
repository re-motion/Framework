﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using Coypu;
using JetBrains.Annotations;
using log4net;
using OpenQA.Selenium;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.Utilities;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.PageObjects;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebTestActions;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Common functionality of <see cref="BocListControlObject"/> and <see cref="BocListAsGridControlObject"/>.
  /// </summary>
  public abstract class BocListControlObjectBase<TRowControlObject, TCellControlObject>
      : BocControlObject,
          IDropDownMenuHost,
          IListMenuHost,
          IControlObjectWithRows<TRowControlObject>,
          IFluentControlObjectWithRows<TRowControlObject>,
          ISupportsValidationErrors
      where TRowControlObject : ControlObject
      where TCellControlObject : ControlObject
  {
    private class BocListRowControlObjectHostAccessor : IBocListRowControlObjectHostAccessor
    {
      private readonly BocListControlObjectBase<TRowControlObject, TCellControlObject> _bocList;

      public BocListRowControlObjectHostAccessor (BocListControlObjectBase<TRowControlObject, TCellControlObject> bocList)
      {
        _bocList = bocList;
      }

      public ElementScope ParentScope
      {
        get { return _bocList.Context.Scope; }
      }

      public int GetColumnIndexForItemID (string columnItemID)
      {
        ArgumentUtility.CheckNotNullOrEmpty ("columnItemID", columnItemID);

        return _bocList.GetColumnByItemID (columnItemID).Index;
      }

      public int GetColumnIndexForTitle (string columnTitle)
      {
        ArgumentUtility.CheckNotNullOrEmpty ("columnTitle", columnTitle);

        return _bocList.GetColumnByTitle (columnTitle).Index;
      }

      public int GetColumnIndexForTitleContains (string columnTitleContains)
      {
        ArgumentUtility.CheckNotNullOrEmpty ("columnTitleContains", columnTitleContains);

        return _bocList.GetColumnByTitleContains (columnTitleContains).Index;
      }

      public int GetZeroBasedAbsoluteRowIndexOfFirstRow ()
      {
        return int.Parse (_bocList.GetRow (1).Scope.FindCss ("input[type='checkbox'], input[type='radio']").Id.Split ('_').Last());
      }
    }

    private readonly ILog _log;
    private readonly IBocListRowControlObjectHostAccessor _accessor;
    private readonly bool _hasFakeTableHead;
    private readonly List<BocListColumnDefinition<TRowControlObject, TCellControlObject>> _columns;

    protected BocListControlObjectBase ([NotNull] ControlObjectContext context)
        : base (context)
    {
      _log = LogManager.GetLogger (GetType());
      _accessor = new BocListRowControlObjectHostAccessor (this);

      EnsureBocListHasBeenFullyInitialized();

      _hasFakeTableHead = Scope.FindCss ("div.bocListTableContainer")[DiagnosticMetadataAttributesForObjectBinding.BocListHasFakeTableHead] != null;
      _columns = RetryUntilTimeout.Run (
          () => Scope.FindAllCss (_hasFakeTableHead ? ".bocListFakeTableHead th" : ".bocListTableContainer th")
              .Select (
                  (s, i) =>
                      new BocListColumnDefinition<TRowControlObject, TCellControlObject> (
                          s[DiagnosticMetadataAttributes.ItemID],
                          i + 1,
                          s[DiagnosticMetadataAttributes.Content],
                          ColumnHasDiagnosticMetadata (s)))
              .ToList());
    }

    /// <summary>
    /// Returns a <see cref="IBocListRowControlObjectHostAccessor"/> for accessing the
    /// <see cref="BocListControlObjectBase{TRowControlObject,TCellControlObject}"/> from a row.
    /// </summary>
    protected IBocListRowControlObjectHostAccessor Accessor
    {
      get { return _accessor; }
    }

    /// <summary>
    /// Returns whether the list has a fake table head.
    /// </summary>
    protected bool HasFakeTableHead ()
    {
      return _hasFakeTableHead;
    }

    /// <summary>
    /// Factory method: implementations instantiate their default row representation control object.
    /// </summary>
    protected abstract TRowControlObject CreateRowControlObject (
        [NotNull] string id,
        [NotNull] ElementScope rowScope,
        [NotNull] IBocListRowControlObjectHostAccessor accessor);

    /// <summary>
    /// Factory method: implementations instantiate their default cell representation control object.
    /// </summary>
    protected abstract TCellControlObject CreateCellControlObject ([NotNull] string id, [NotNull] ElementScope cellScope);

    /// <inheritdoc/>
    public int GetCurrentPage ()
    {
      var navigatorDivScope = Scope.FindCss (".bocListNavigator");

      if (!HasNavigator())
        return 1;

      return int.Parse (navigatorDivScope[DiagnosticMetadataAttributesForObjectBinding.BocListCurrentPageNumber]);
    }

    /// <inheritdoc/>
    public int GetNumberOfPages ()
    {
      var navigatorDivScope = Scope.FindCss (".bocListNavigator");

      if (!HasNavigator())
        return 1;

      return int.Parse (navigatorDivScope[DiagnosticMetadataAttributesForObjectBinding.BocListNumberOfPages]);
    }

    /// <inheritdoc/>
    public void GoToSpecificPage (int oneBasedPageNumber)
    {
      EnsureNavigationPossible();

      var currentPageNumber = GetCurrentPage();
      if (currentPageNumber == oneBasedPageNumber)
        throw new MissingHtmlException (string.Format ("List is already on page '{0}'.", currentPageNumber));

      if (oneBasedPageNumber < 1 || oneBasedPageNumber > GetNumberOfPages())
        throw CreateMissingHtmlExceptionForIndexOutOfRange (oneBasedPageNumber);

      var currentPageTextInputScope = Scope.FindIdEndingWith ("Boc_CurrentPage_TextBox");
      new FillWithAction (this, currentPageTextInputScope, Keys.Backspace + oneBasedPageNumber, FinishInput.WithTab).Execute (
          Opt.ContinueWhen (((IWebFormsPageObject) Context.PageObject).PostBackCompletionDetectionStrategy));
    }

    /// <inheritdoc/>
    public void GoToFirstPage ()
    {
      EnsureNavigationPossible();

      if (GetCurrentPage() == 1)
        throw CreateMissingHtmlExceptionForUnableToNavigateToPage ("first", "first");

      var firstPageLinkScope = Scope.FindChild ("Navigation_First");
      new ClickAction (this, firstPageLinkScope).Execute (
          Opt.ContinueWhen (((IWebFormsPageObject) Context.PageObject).PostBackCompletionDetectionStrategy));
    }

    /// <inheritdoc/>
    public void GoToPreviousPage ()
    {
      EnsureNavigationPossible();

      if (GetCurrentPage() == 1)
        throw CreateMissingHtmlExceptionForUnableToNavigateToPage ("previous", "first");

      var previousPageLinkScope = Scope.FindChild ("Navigation_Previous");
      new ClickAction (this, previousPageLinkScope).Execute (
          Opt.ContinueWhen (((IWebFormsPageObject) Context.PageObject).PostBackCompletionDetectionStrategy));
    }

    /// <inheritdoc/>
    public void GoToNextPage ()
    {
      EnsureNavigationPossible();

      if (GetCurrentPage() == GetNumberOfPages())
        throw CreateMissingHtmlExceptionForUnableToNavigateToPage ("next", "last");

      var nextPageLinkScope = Scope.FindChild ("Navigation_Next");
      new ClickAction (this, nextPageLinkScope).Execute (
          Opt.ContinueWhen (((IWebFormsPageObject) Context.PageObject).PostBackCompletionDetectionStrategy));
    }

    /// <inheritdoc/>
    public void GoToLastPage ()
    {
      EnsureNavigationPossible();

      if (GetCurrentPage() == GetNumberOfPages())
        throw CreateMissingHtmlExceptionForUnableToNavigateToPage ("last", "last");

      var lastPageLinkScope = Scope.FindChild ("Navigation_Last");
      new ClickAction (this, lastPageLinkScope).Execute (
          Opt.ContinueWhen (((IWebFormsPageObject) Context.PageObject).PostBackCompletionDetectionStrategy));
    }

    /// <summary>
    /// Returns the list's columns.
    /// Warning: this method does not wait until "the element" is available but detects all available columns at the moment of calling.
    /// </summary>
    public IReadOnlyList<BocListColumnDefinition<TRowControlObject, TCellControlObject>> GetColumnDefinitions ()
    {
      return _columns;
    }

    /// <summary>
    /// Returns the list's rows.
    /// Warning: this method does not wait until "the element" is available but detects all available rows at the moment of calling.
    /// </summary>
    public IReadOnlyList<TRowControlObject> GetDisplayedRows ()
    {
      var cssSelector = ".bocListTable .bocListTableBody .bocListDataRow";
      return RetryUntilTimeout.Run (
          () => Scope.FindAllCss (cssSelector).Select (rowScope => CreateRowControlObject (GetHtmlID(), rowScope, _accessor)).ToList());
    }

    /// <summary>
    /// Returns whether the list is empty.
    /// </summary>
    public bool IsEmpty ()
    {
      return GetNumberOfRows() == 0;
    }

    /// <summary>
    /// Returns whether the list is currently in edit mode.
    /// </summary>
    public bool IsEditModeActive ()
    {
      return Scope[DiagnosticMetadataAttributesForObjectBinding.BocListIsEditModeActive] == "true";
    }

    /// <summary>
    /// Returns whether the list has a navigator.
    /// </summary>
    public bool HasNavigator ()
    {
      return Scope[DiagnosticMetadataAttributesForObjectBinding.BocListHasNavigationBlock] == "true";
    }

    /// <summary>
    /// Returns the list's empty message, call only if <see cref="IsEmpty"/> returns <see langword="true" />.
    /// </summary>
    /// <returns></returns>
    public string GetEmptyMessage ()
    {
      return Scope.FindCss (".bocListTable .bocListTableBody").Text.Trim();
    }

    /// <summary>
    /// Returns the number of rows in the list (on the current page).
    /// </summary>
    public int GetNumberOfRows ()
    {
      return RetryUntilTimeout.Run (() => Scope.FindAllCss (".bocListTable .bocListTableBody > tr.bocListDataRow").Count());
    }

    /// <summary>
    /// Selects all rows by checking the table's select all checkbox.
    /// </summary>
    public void SelectAll ()
    {
      var scope = GetSelectAllCheckboxScope();
      new CheckAction (this, scope).Execute (Opt.ContinueImmediately());
    }

    /// <summary>
    /// Deselect all rows by checking the table's select all checkbox.
    /// </summary>
    public void DeselectAll ()
    {
      var scope = GetSelectAllCheckboxScope();
      new UncheckAction (this, scope).Execute (Opt.ContinueImmediately());
    }

    private ElementScope GetSelectAllCheckboxScope ()
    {
      var selectAllCheckboxID = GetHtmlID() + "_AllRowsSelector";
      return Scope.FindCss (string.Format ("input[name='{0}']", selectAllCheckboxID));
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithRows<TRowControlObject> GetRow ()
    {
      return this;
    }

    /// <inheritdoc/>
    public TRowControlObject GetRow (string itemID)
    {
      return GetRow().WithItemID (itemID);
    }

    /// <inheritdoc/>
    public TRowControlObject GetRow (int oneBasedIndex)
    {
      return GetRow().WithIndex (oneBasedIndex);
    }

    /// <inheritdoc/>
    TRowControlObject IFluentControlObjectWithRows<TRowControlObject>.WithItemID (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var cssSelector = string.Format (
          ".bocListTable .bocListTableBody .bocListDataRow[{0}={1}]",
          DiagnosticMetadataAttributes.ItemID,
          DomSelectorUtility.CreateMatchValueForCssSelector (itemID));
      return GetRowByCssSelector (cssSelector);
    }

    /// <inheritdoc/>
    TRowControlObject IFluentControlObjectWithRows<TRowControlObject>.WithIndex (int oneBasedIndex)
    {
      var cssSelector = string.Format (
          ".bocListTable .bocListTableBody .bocListDataRow[{0}='{1}']",
          DiagnosticMetadataAttributesForObjectBinding.BocListRowIndex,
          oneBasedIndex);
      return GetRowByCssSelector (cssSelector);
    }

    private TRowControlObject GetRowByCssSelector (string cssSelector)
    {
      var rowScope = Scope.FindCss (cssSelector);
      return CreateRowControlObject (GetHtmlID(), rowScope, _accessor);
    }

    /// <inheritdoc/>
    public DropDownMenuControlObject GetDropDownMenu ()
    {
      var dropDownMenuScope = Scope.FindChild ("Boc_OptionsMenu");
      return new DropDownMenuControlObject (Context.CloneForControl (dropDownMenuScope));
    }

    /// <inheritdoc/>
    public ListMenuControlObject GetListMenu ()
    {
      var listMenuScope = Scope.FindChild ("Boc_ListMenu");
      return new ListMenuControlObject (Context.CloneForControl (listMenuScope));
    }

    /// <summary>
    /// Returns the column definition given by <paramref name="columnItemID"/>.
    /// </summary>
    protected BocListColumnDefinition<TRowControlObject, TCellControlObject> GetColumnByItemID ([NotNull] string columnItemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnItemID", columnItemID);

      return _columns.Single (cd => cd.ItemID == columnItemID);
    }

    /// <summary>
    /// Returns the column definition given by <paramref name="oneBasedIndex"/>.
    /// </summary>
    protected BocListColumnDefinition<TRowControlObject, TCellControlObject> GetColumnByIndex (int oneBasedIndex)
    {
      return _columns.Single (cd => cd.Index == oneBasedIndex);
    }

    /// <summary>
    /// Returns the column definition given by <paramref name="columnTitle"/>.
    /// </summary>
    protected BocListColumnDefinition<TRowControlObject, TCellControlObject> GetColumnByTitle ([NotNull] string columnTitle)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnTitle", columnTitle);

      return _columns.Single (cd => cd.Title == columnTitle);
    }

    /// <summary>
    /// Returns the column definition given by <paramref name="columnTitleContains"/>.
    /// </summary>
    protected BocListColumnDefinition<TRowControlObject, TCellControlObject> GetColumnByTitleContains ([NotNull] string columnTitleContains)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnTitleContains", columnTitleContains);

      return _columns.Where (cd => cd.Title != null).Single (cd => cd.Title.Contains (columnTitleContains));
    }

    public IReadOnlyList<string> GetValidationErrors ()
    {
      return GetValidationErrors (Scope.FindCss (".bocListTableBlock > .bocListTableContainer"));
    }

    protected override ElementScope GetLabeledElementScope ()
    {
      return Scope;
    }

    private bool ColumnHasDiagnosticMetadata (ElementScope scope)
    {
      if (scope[DiagnosticMetadataAttributesForObjectBinding.BocListColumnHasDiagnosticMetadata] == null)
        return false;

      return bool.Parse (scope[DiagnosticMetadataAttributesForObjectBinding.BocListColumnHasDiagnosticMetadata]);
    }

    private void EnsureBocListHasBeenFullyInitialized ()
    {
      var bocListIsInitialized = RetryUntilTimeout.Run (() => Scope[DiagnosticMetadataAttributesForObjectBinding.BocListIsInitialized] == "true");
      if (!bocListIsInitialized)
        _log.WarnFormat ("Client side initialization of BocList '{0}' never finished.", GetHtmlID());
    }

    private void EnsureNavigationPossible ()
    {
      if (IsEditModeActive())
        throw new MissingHtmlException ("Unable to change current page of the list. List is currently in edit mode.");

      if (!HasNavigator())
        throw new MissingHtmlException ("Unable to change current page of the list. List only has one page.");
    }

    private Exception CreateMissingHtmlExceptionForUnableToNavigateToPage (string pageWhichCantBeNavigatedTo, string currentPageAsString)
    {
      return new MissingHtmlException (string.Format ("Unable to change page number to the {0} page, as the list is already on the {1} page.", pageWhichCantBeNavigatedTo, currentPageAsString));
    }

    private Exception CreateMissingHtmlExceptionForIndexOutOfRange (int pageNumberToBeNavigated)
    {
      if (GetNumberOfPages() == 1)
      {
        return new MissingHtmlException (
            string.Format ("Unable to navigate to page number '{0}'. The list only has one page.", pageNumberToBeNavigated));
      }

      return new MissingHtmlException (
              string.Format (
                  "Unable to change page number to '{0}'. Page number must be between '1' and '{1}'.",
                  pageNumberToBeNavigated,
                  GetNumberOfPages()));
    }
  }
}