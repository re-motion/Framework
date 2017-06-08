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
using Coypu;
using JetBrains.Annotations;
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
  public class BocListControlObject : BocListControlObject<BocListRowControlObject>
  {
    public BocListControlObject ([NotNull] ControlObjectContext context)
        : base(context)
    {
    }
  }

  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/>.
  /// </summary>
  public class BocListControlObject<TRowControlObject>
      : BocListControlObjectBase<TRowControlObject, BocListCellControlObject>,
          IControlObjectWithRowsWhereColumnContains<TRowControlObject>,
          IFluentControlObjectWithRowsWhereColumnContains<TRowControlObject>,
          IControlObjectWithCellsInRowsWhereColumnContains<BocListCellControlObject>,
          IFluentControlObjectWithCellsInRowsWhereColumnContains<BocListCellControlObject>
      where TRowControlObject : BocListRowControlObject
  {
    public BocListControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <inheritdoc/>
    public override int GetCurrentPage ()
    {
      var currentPageTextInputScope = GetCurrentPageTextInputScope();
      return int.Parse (currentPageTextInputScope.Value);
    }

    /// <inheritdoc/>
    public override int GetNumberOfPages ()
    {
      var navigatorDivScope = Scope.FindCss (".bocListNavigator");
      return int.Parse (navigatorDivScope[DiagnosticMetadataAttributesForObjectBinding.BocListNumberOfPages]);
    }

    /// <inheritdoc/>
    public override void GoToSpecificPage (int oneBasedPageNumber)
    {
      var currentPageTextInputScope = GetCurrentPageTextInputScope();
      new FillWithAction (this, currentPageTextInputScope, Keys.Backspace + oneBasedPageNumber, FinishInput.WithTab).Execute (
          Opt.ContinueWhen (((IWebFormsPageObject) Context.PageObject).PostBackCompletionDetectionStrategy));
    }

    /// <inheritdoc/>
    public override void GoToFirstPage ()
    {
      var firstPageLinkScope = Scope.FindChild ("Navigation_First");
      new ClickAction (this, firstPageLinkScope).Execute (
          Opt.ContinueWhen (((IWebFormsPageObject) Context.PageObject).PostBackCompletionDetectionStrategy));
    }

    /// <inheritdoc/>
    public override void GoToPreviousPage ()
    {
      var previousPageLinkScope = Scope.FindChild ("Navigation_Previous");
      new ClickAction (this, previousPageLinkScope).Execute (
          Opt.ContinueWhen (((IWebFormsPageObject) Context.PageObject).PostBackCompletionDetectionStrategy));
    }

    /// <inheritdoc/>
    public override void GoToNextPage ()
    {
      var nextPageLinkScope = Scope.FindChild ("Navigation_Next");
      new ClickAction (this, nextPageLinkScope).Execute (
          Opt.ContinueWhen (((IWebFormsPageObject) Context.PageObject).PostBackCompletionDetectionStrategy));
    }

    /// <inheritdoc/>
    public override void GoToLastPage ()
    {
      var lastPageLinkScope = Scope.FindChild ("Navigation_Last");
      new ClickAction (this, lastPageLinkScope).Execute (
          Opt.ContinueWhen (((IWebFormsPageObject) Context.PageObject).PostBackCompletionDetectionStrategy));
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithRowsWhereColumnContains<TRowControlObject> GetRowWhere ()
    {
      return this;
    }

    /// <inheritdoc/>
    public TRowControlObject GetRowWhere (string columnItemID, string containsCellText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnItemID", columnItemID);
      ArgumentUtility.CheckNotNull ("containsCellText", containsCellText);

      return GetRowWhere().ColumnWithItemIDContains (columnItemID, containsCellText);
    }

    /// <inheritdoc/>
    TRowControlObject IFluentControlObjectWithRowsWhereColumnContains<TRowControlObject>.ColumnWithItemIDContains (
        string itemID,
        string containsCellText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);
      ArgumentUtility.CheckNotNull ("containsCellText", containsCellText);

      var cell = GetCellWhere (itemID, containsCellText);
      return GetRowFromCell (cell);
    }

    /// <inheritdoc/>
    TRowControlObject IFluentControlObjectWithRowsWhereColumnContains<TRowControlObject>.ColumnWithIndexContains (
        int oneBasedIndex,
        string containsCellText)
    {
      ArgumentUtility.CheckNotNull ("containsCellText", containsCellText);

      var cell = GetCellWhere().ColumnWithIndexContains (oneBasedIndex, containsCellText);
      return GetRowFromCell (cell);
    }

    /// <inheritdoc/>
    TRowControlObject IFluentControlObjectWithRowsWhereColumnContains<TRowControlObject>.ColumnWithTitleContainsExactly (
        string title,
        string containsCellText)
    {
      ArgumentUtility.CheckNotNull ("title", title);
      ArgumentUtility.CheckNotNull ("containsCellText", containsCellText);

      var cell = GetCellWhere().ColumnWithTitleContainsExactly (title, containsCellText);
      return GetRowFromCell (cell);
    }

    /// <inheritdoc/>
    TRowControlObject IFluentControlObjectWithRowsWhereColumnContains<TRowControlObject>.ColumnWithTitleContains (
        string title,
        string containsCellText)
    {
      ArgumentUtility.CheckNotNull ("title", title);
      ArgumentUtility.CheckNotNull ("containsCellText", containsCellText);

      var cell = GetCellWhere().ColumnWithTitleContains (title, containsCellText);
      return GetRowFromCell (cell);
    }

    private TRowControlObject GetRowFromCell (BocListCellControlObject cell)
    {
      var rowScope = cell.Scope.FindXPath ("..");
      return CreateRowControlObject (GetHtmlID(), rowScope, Accessor);
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithCellsInRowsWhereColumnContains<BocListCellControlObject> GetCellWhere ()
    {
      return this;
    }

    /// <inheritdoc/>
    public BocListCellControlObject GetCellWhere (string columnItemID, string containsCellText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnItemID", columnItemID);
      ArgumentUtility.CheckNotNull ("containsCellText", containsCellText);

      return GetCellWhere().ColumnWithItemIDContains (columnItemID, containsCellText);
    }

    /// <inheritdoc/>
    BocListCellControlObject IFluentControlObjectWithCellsInRowsWhereColumnContains<BocListCellControlObject>.ColumnWithItemIDContains (
        string itemID,
        string containsCellText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);
      ArgumentUtility.CheckNotNull ("containsCellText", containsCellText);

      var column = GetColumnByItemID (itemID);
      return GetCellWhereColumnContainsExactly (column, containsCellText);
    }

    /// <inheritdoc/>
    BocListCellControlObject IFluentControlObjectWithCellsInRowsWhereColumnContains<BocListCellControlObject>.ColumnWithIndexContains (
        int oneBasedIndex,
        string containsCellText)
    {
      ArgumentUtility.CheckNotNull ("containsCellText", containsCellText);

      var column = GetColumnByIndex (oneBasedIndex);
      return GetCellWhereColumnContainsExactly (column, containsCellText);
    }

    /// <inheritdoc/>
    BocListCellControlObject IFluentControlObjectWithCellsInRowsWhereColumnContains<BocListCellControlObject>.ColumnWithTitleContainsExactly (
        string title,
        string cellText)
    {
      ArgumentUtility.CheckNotNull ("title", title);
      ArgumentUtility.CheckNotNull ("cellText", cellText);

      var column = GetColumnByTitle (title);
      return GetCellWhereColumnContainsExactly (column, cellText);
    }

    /// <inheritdoc/>
    BocListCellControlObject IFluentControlObjectWithCellsInRowsWhereColumnContains<BocListCellControlObject>.ColumnWithTitleContains (
        string title,
        string containsCellText)
    {
      ArgumentUtility.CheckNotNull ("title", title);
      ArgumentUtility.CheckNotNull ("containsCellText", containsCellText);

      var column = GetColumnByTitle (title);
      return GetCellWhereColumnContains (column, containsCellText);
    }

    private BocListCellControlObject GetCellWhereColumnContainsExactly (
        BocListColumnDefinition<TRowControlObject, BocListCellControlObject> column,
        string containsCellText)
    {
      if (column.HasDiagnosticMetadata)
      {
        var cssSelector = string.Format (
            ".bocListTable .bocListTableBody .bocListDataRow .bocListDataCell[{0}='{1}'] span[{2}='{3}']",
            DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex,
            column.Index,
            DiagnosticMetadataAttributesForObjectBinding.BocListCellContents,
            containsCellText);
        var cellScope = Scope.FindCss (cssSelector).FindXPath ("../..");
        return CreateCellControlObject (GetHtmlID(), cellScope);
      }
      else
      {
        var xPathSelector = string.Format (
            ".//tbody{0}/tr/td[.//*='{1}']",
            XPathUtils.CreateHasClassCheck ("bocListTableBody"),
            containsCellText);
        var cellScope = Scope.FindXPath (xPathSelector);
        return CreateCellControlObject (GetHtmlID(), cellScope);
      }
    }

    private BocListCellControlObject GetCellWhereColumnContains (
        BocListColumnDefinition<TRowControlObject, BocListCellControlObject> column,
        string containsCellText)
    {
      if (column.HasDiagnosticMetadata)
      {
        var cssSelector = string.Format (
            ".bocListTable .bocListTableBody .bocListDataRow .bocListDataCell[{0}='{1}'] span[{2}*='{3}']",
            DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex,
            column.Index,
            DiagnosticMetadataAttributesForObjectBinding.BocListCellContents,
            containsCellText);
        var cellScope = Scope.FindCss (cssSelector).FindXPath ("../..");
        return CreateCellControlObject (GetHtmlID(), cellScope);
      }
      else
      {
        var xPathSelector = string.Format (
            ".//tbody{0}/tr/td[contains(.//*,'{1}')]",
            XPathUtils.CreateHasClassCheck ("bocListTableBody"),
            containsCellText);
        var cellScope = Scope.FindXPath (xPathSelector);
        return CreateCellControlObject (GetHtmlID(), cellScope);
      }
    }

    /// <summary>
    /// Clicks on the column header given by <paramref name="columnItemID"/> in order to sort the column.
    /// </summary>
    public void ClickOnSortColumn ([NotNull] string columnItemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnItemID", columnItemID);

      var column = GetColumnByItemID (columnItemID);
      ClickOnSortColumn (column.Index);
    }

    /// <summary>
    /// Clicks on the column header given by <paramref name="oneBasedColumnIndex"/> in order to sort the column.
    /// </summary>
    public void ClickOnSortColumn (int oneBasedColumnIndex)
    {
      var sortColumnClickScope = Scope.FindTagWithAttribute (
          HasFakeTableHead ? ".bocListFakeTableHead th" : ".bocListTableContainer th",
          DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex,
          oneBasedColumnIndex.ToString());

      var sortColumnLinkScope = sortColumnClickScope.FindLink();
      // Note: explicit hovering is required: Selenium does not correctly bring the fake table head into view.
      if (HasFakeTableHead)
        sortColumnLinkScope.Hover();

      new ClickAction (this, sortColumnLinkScope).Execute (
          Opt.ContinueWhen (((IWebFormsPageObject) Context.PageObject).PostBackCompletionDetectionStrategy));
    }

    /// <summary>
    /// Clicks on the column header given by <paramref name="columnTitle"/> in order to sort the column.
    /// </summary>
    public void ClickOnSortColumnByTitle ([NotNull] string columnTitle)
    {
      ArgumentUtility.CheckNotNull ("columnTitle", columnTitle);

      var column = GetColumnByTitle (columnTitle);
      ClickOnSortColumn (column.Index);
    }

    /// <summary>
    /// Changes the list's view to the view given by <paramref name="itemID"/>.
    /// </summary>
    public void ChangeViewTo ([NotNull] string itemID, [CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      ChangeViewTo (scope => scope.SelectOptionByDMA (DiagnosticMetadataAttributes.ItemID, itemID), actionOptions);
    }

    /// <summary>
    /// Changes the list's view to the view given by <paramref name="oneBasedIndex"/>.
    /// </summary>
    public void ChangeViewTo (int oneBasedIndex, [CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      ChangeViewTo (scope => scope.SelectOptionByIndex (oneBasedIndex), actionOptions);
    }

    /// <summary>
    /// Changes the list's view to the view given by <paramref name="label"/>.
    /// </summary>
    public void ChangeViewToByLabel ([NotNull] string label, [CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      ArgumentUtility.CheckNotNull ("label", label);

      ChangeViewTo (scope => scope.SelectOption (label), actionOptions);
    }

    private void ChangeViewTo ([NotNull] Action<ElementScope> selectAction, [CanBeNull] IWebTestActionOptions actionOptions)
    {
      ArgumentUtility.CheckNotNull ("selectAction", selectAction);

      var actualActionOptions = MergeWithDefaultActionOptions (Scope, actionOptions);
      var availableViewsScope = GetAvailableViewsScope();
      new CustomAction (this, availableViewsScope, "Select", selectAction).Execute (actualActionOptions);
    }

    /// <inheritdoc/>
    protected override TRowControlObject CreateRowControlObject (
        string id,
        ElementScope rowScope,
        IBocListRowControlObjectHostAccessor accessor)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("rowScope", rowScope);
      ArgumentUtility.CheckNotNull ("accessor", accessor);

      return (TRowControlObject) Activator.CreateInstance (typeof (TRowControlObject), accessor, Context.CloneForControl (rowScope));
    }

    /// <inheritdoc/>
    protected override BocListCellControlObject CreateCellControlObject (string id, ElementScope cellScope)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("cellScope", cellScope);

      return new BocListCellControlObject (Context.CloneForControl (cellScope));
    }

    /// <summary>
    /// Allows derived classes to override the scope in which the available views select box resides.
    /// </summary>
    protected virtual ElementScope GetAvailableViewsScope ()
    {
      return Scope.FindChild ("Boc_AvailableViewsList");
    }

    private ElementScope GetCurrentPageTextInputScope ()
    {
      return Scope.FindIdEndingWith ("Boc_CurrentPage_TextBox");
    }
  }
}