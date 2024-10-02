// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Utilities;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Base class for all control objects representing a <see cref="T:Remotion.Web.UI.Controls.DropDownMenu"/>.
  /// </summary>
  public abstract class DropDownMenuControlObjectBase
      : WebFormsControlObjectWithDiagnosticMetadata, IControlObjectWithSelectableItems, IFluentControlObjectWithSelectableItems, ISupportsDisabledState
  {
    protected DropDownMenuControlObjectBase ([NotNull] ControlObjectContext context)
        : base(context)
    {
    }

    protected abstract void PerformOpen (ElementScope menuButtonScope);

    /// <summary>
    /// Opens the menu (i.e. make it visible on the page).
    /// </summary>
    public void Open ()
    {
      var menuButtonScope = GetDropDownMenuButtonScope();
      Open(menuButtonScope);
    }

    /// <summary>
    /// Returns true if the drop down option menu is detected to be open.
    /// </summary>
    public bool IsOpen ()
    {
      var menuButtonScope = GetDropDownMenuButtonScope();
      return IsOpen(menuButtonScope);
    }

    /// <summary>
    /// Method to close the currently open DropDown via javascript.
    /// </summary>
    public void Close ()
    {
      if (IsOpen())
        Context.Browser.Driver.ExecuteScript("DropDownMenu_ClosePopUp(false)", Scope);
    }

    /// <inheritdoc/>
    public IReadOnlyList<ItemDefinition> GetItemDefinitions ()
    {
      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver);

      var dropDownMenuScope = GetDropDownMenuScope();

      var itemDefinitions = RetryUntilTimeout.Run(
          Logger,
          () => dropDownMenuScope.FindAllCss("li.DropDownMenuItem, li.DropDownMenuItemDisabled")
              .Select(
                  (itemScope, i) =>
                      new ItemDefinition(
                          itemScope[DiagnosticMetadataAttributes.ItemID],
                          i + 1,
                          itemScope.Text.Trim(),
                          FindItemCommand(itemScope).IsDisabled(),
                          itemScope.FindCss("a")?["accesskey"] ?? string.Empty))
              .ToList());

      Close();

      return itemDefinitions;

    }

    /// <inheritdoc />
    public bool IsDisabled ()
    {
      return Scope[DiagnosticMetadataAttributes.IsDisabled] == "true";
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithSelectableItems SelectItem ()
    {
      return this;
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject SelectItem (string itemID, IWebTestActionOptions? actionOptions = null)
    {
      const string operationName = "SelectItem(itemID)";

      ArgumentUtility.CheckNotNullOrEmpty("itemID", itemID);

      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver, operationName: operationName);

      var itemCommand = GetItemCommandByItemID(itemID);

      if (itemCommand.IsDisabled())
        throw AssertionExceptionUtility.CreateCommandDisabledException(Driver, operationName: operationName);

      return ClickItem(itemCommand, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithItemID (string itemID, IWebTestActionOptions? actionOptions)
    {
      const string operationName = "SelectItem.WithItemID";

      ArgumentUtility.CheckNotNullOrEmpty("itemID", itemID);

      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver, operationName: operationName);

      var itemCommand = GetItemCommandByItemID(itemID);

      if (itemCommand.IsDisabled())
        throw AssertionExceptionUtility.CreateCommandDisabledException(Driver, operationName: operationName);

      return ClickItem(itemCommand, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithIndex (int oneBasedIndex, IWebTestActionOptions? actionOptions)
    {
      const string operationName = "SelectItem.WithIndex";

      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver, operationName: operationName);

      var itemCommand = GetItemCommandByIndex(oneBasedIndex);

      if (itemCommand.IsDisabled())
        throw AssertionExceptionUtility.CreateCommandDisabledException(Driver, operationName: operationName);

      return ClickItem(itemCommand, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithHtmlID (string htmlID, IWebTestActionOptions? actionOptions)
    {
      const string operationName = "SelectItem.WithHtmlID";

      ArgumentUtility.CheckNotNullOrEmpty("htmlID", htmlID);

      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver, operationName: operationName);

      var itemCommand = GetItemCommandByHtmlID(htmlID);

      if (itemCommand.IsDisabled())
        throw AssertionExceptionUtility.CreateCommandDisabledException(Driver, operationName: operationName);

      return ClickItem(itemCommand, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithDisplayText (string displayText, IWebTestActionOptions? actionOptions)
    {
      const string operationName = "SelectItem.WithDisplayText";

      ArgumentUtility.CheckNotNullOrEmpty("displayText", displayText);

      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver, operationName: operationName);

      var itemCommand = GetItemCommandByDisplayText(displayText);

      if (itemCommand.IsDisabled())
        throw AssertionExceptionUtility.CreateCommandDisabledException(Driver, operationName: operationName);

      return ClickItem(itemCommand, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithDisplayTextContains (
        string containsDisplayText,
        IWebTestActionOptions? actionOptions)
    {
      const string operationName = "SelectItem.WithDisplayTextContains";

      ArgumentUtility.CheckNotNullOrEmpty("containsDisplayText", containsDisplayText);

      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver, operationName: operationName);

      var itemCommand = GetItemCommandByDisplayTextContains(containsDisplayText);

      if (itemCommand.IsDisabled())
        throw AssertionExceptionUtility.CreateCommandDisabledException(Driver, operationName: operationName);

      return ClickItem(itemCommand, actionOptions);
    }

    private CommandControlObject GetItemCommandByItemID (string itemID)
    {
      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindTagWithAttribute(
          "ul.DropDownMenuOptions > li",
          DiagnosticMetadataAttributes.ItemID,
          itemID);

      return FindItemCommand(scope);
    }

    private CommandControlObject GetItemCommandByIndex (int oneBasedIndex)
    {
      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindXPath(string.Format("li[{0}]", oneBasedIndex));

      return FindItemCommand(scope);
    }

    private CommandControlObject GetItemCommandByHtmlID (string htmlID)
    {
      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindId(htmlID);

      return FindItemCommand(scope);
    }

    private CommandControlObject GetItemCommandByDisplayText (string displayText)
    {
      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindTagWithAttribute(
          "ul.DropDownMenuOptions > li",
          DiagnosticMetadataAttributes.Content,
          displayText);

      return FindItemCommand(scope);
    }

    private CommandControlObject GetItemCommandByDisplayTextContains (string containsDisplayText)
    {
      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindTagWithAttributeUsingOperator(
          "ul.DropDownMenuOptions > li",
          CssComparisonOperator.SubstringMatch,
          DiagnosticMetadataAttributes.Content,
          containsDisplayText);

      return FindItemCommand(scope);
    }

    private void Open (ElementScope menuButtonScope)
    {
      if (!IsOpen(menuButtonScope))
      {
        PerformOpen(menuButtonScope);

        RetryUntilTimeout.Run(
            Logger,
            () =>
            {
              if (GetDropDownMenuPopupID(menuButtonScope) == null)
                throw AssertionExceptionUtility.CreateExpectationException(Driver, "Unable to open the menu.");
            });
      }
    }

    private bool IsOpen (ElementScope menuButtonScope)
    {
      return GetDropDownMenuPopupID(menuButtonScope) != null;
    }

    private ElementScope GetDropDownMenuButtonScope ()
    {
      return Scope.FindCss("a[aria-haspopup='menu']");
    }

    private string GetDropDownMenuPopupID (ElementScope scope)
    {
      return scope["aria-controls"];
    }

    private ElementScope GetDropDownMenuScope ()
    {
      var menuButtonScope = GetDropDownMenuButtonScope();

      Open(menuButtonScope);

      var dropDownMenuPopupID = GetDropDownMenuPopupID(menuButtonScope);
      var dropDownMenuOptionsScope = Context.RootScope.FindCss("#" + dropDownMenuPopupID + " > ul.DropDownMenuOptions");

      return dropDownMenuOptionsScope;
    }

    private UnspecifiedPageObject ClickItem (CommandControlObject itemCommand, IWebTestActionOptions? actionOptions)
    {
      try
      {
        ((IControlObjectNotifier)itemCommand).ActionExecute += OnActionExecute;
        return itemCommand.Click(actionOptions);
      }
      finally
      {
        ((IControlObjectNotifier)itemCommand).ActionExecute -= OnActionExecute;
      }
    }

    private CommandControlObject FindItemCommand (ElementScope itemScope)
    {
      var itemCommandScope = itemScope.FindLink();
      return new CommandControlObject(Context.CloneForControl(itemCommandScope));
    }
  }
}
