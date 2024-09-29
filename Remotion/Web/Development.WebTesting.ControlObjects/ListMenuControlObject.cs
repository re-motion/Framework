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
  /// Control object for <see cref="T:Remotion.Web.UI.Controls.ListMenu"/>.
  /// </summary>
  public class ListMenuControlObject
      : WebFormsControlObjectWithDiagnosticMetadata, IControlObjectWithSelectableItems, IFluentControlObjectWithSelectableItems, ISupportsDisabledState
  {
    private const string c_headingFragmentID = "Heading";

    public ListMenuControlObject ([NotNull] ControlObjectContext context)
        : base(context)
    {
    }

    /// <inheritdoc/>
    public IReadOnlyList<ItemDefinition> GetItemDefinitions ()
    {
      return
          RetryUntilTimeout.Run(
              Logger,
              () =>
                  Scope.FindAllCss("span.listMenuItem, span.listMenuItemDisabled")
                      .Select(
                          (itemScope, i) =>
                              new ItemDefinition(
                                  itemScope[DiagnosticMetadataAttributes.ItemID],
                                  i + 1,
                                  itemScope.Text.Trim(),
                                  FindItemCommand(itemScope).IsDisabled(),
                                  itemScope.FindCss("a")?["accesskey"] ?? string.Empty))
                      .ToList());
    }

    /// <summary>
    /// Gets the hidden heading of the list menu.
    /// </summary>
    public string? Heading
    {
      get
      {
        var hasHeading = Scope.FindCss(":scope > table")["aria-labelledby"] != null;
        if (!hasHeading)
          return null;

        return Scope.FindChild(c_headingFragmentID).Text.Trim();
      }
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
      var itemScope = Scope.FindTagWithAttribute(
          "td.listMenuRow > span",
          DiagnosticMetadataAttributes.ItemID,
          itemID);
      var itemCommand = FindItemCommand(itemScope);

      return itemCommand;
    }

    private CommandControlObject GetItemCommandByIndex (int oneBasedIndex)
    {
      var itemScope = Scope.FindChild((oneBasedIndex - 1).ToString());
      var itemCommand = FindItemCommand(itemScope);

      return itemCommand;
    }

    private CommandControlObject GetItemCommandByHtmlID (string htmlID)
    {
      var itemScope = Scope.FindId(htmlID);
      var itemCommand = FindItemCommand(itemScope);

      return itemCommand;
    }

    private CommandControlObject GetItemCommandByDisplayText (string displayText)
    {
      var itemScope = Scope.FindTagWithAttribute(
          "td.listMenuRow > span",
          DiagnosticMetadataAttributes.Content,
          displayText);

      return FindItemCommand(itemScope);
    }

    private CommandControlObject GetItemCommandByDisplayTextContains (string containsDisplayText)
    {
      var itemScope = Scope.FindTagWithAttributeUsingOperator(
          "td.listMenuRow > span",
          CssComparisonOperator.SubstringMatch,
          DiagnosticMetadataAttributes.Content,
          containsDisplayText);

      return FindItemCommand(itemScope);
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
