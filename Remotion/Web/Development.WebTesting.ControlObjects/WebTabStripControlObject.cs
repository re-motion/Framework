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
  /// Control object for form grids created with <see cref="T:Remotion.Web.UI.Controls.WebTabStrip"/>.
  /// </summary>
  public class WebTabStripControlObject : WebFormsControlObjectWithDiagnosticMetadata, IControlObjectWithTabs, IFluentControlObjectWithTabs
  {
    public WebTabStripControlObject ([NotNull] ControlObjectContext context)
        : base(context)
    {
    }

    /// <inheritdoc/>
    public WebTabStripTabDefinition GetSelectedTab ()
    {
      var tabScope = Scope.FindCss("span.tabStripTabSelected");
      return new WebTabStripTabDefinition(
          tabScope[DiagnosticMetadataAttributes.ItemID],
          -1,
          tabScope[DiagnosticMetadataAttributes.Content],
          tabScope[DiagnosticMetadataAttributes.IsDisabled] == "true",
          tabScope.FindCss("a")?["accesskey"] ?? string.Empty);
    }

    /// <inheritdoc/>
    public IReadOnlyList<WebTabStripTabDefinition> GetTabDefinitions ()
    {
      const string cssSelector = "span.tabStripTab, span.tabStripTabSelected";
      return RetryUntilTimeout.Run(
          Logger,
          () =>
              Scope.FindAllCss(cssSelector)
                  .Select(
                      (tabScope, i) =>
                          new WebTabStripTabDefinition(
                              tabScope[DiagnosticMetadataAttributes.ItemID],
                              i + 1,
                              tabScope[DiagnosticMetadataAttributes.Content],
                              tabScope[DiagnosticMetadataAttributes.IsDisabled] == "true",
                              tabScope.FindCss("a")?["accesskey"] ?? string.Empty))
                  .ToList());
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithTabs SwitchTo ()
    {
      return this;
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject SwitchTo (string itemID, IWebTestActionOptions? actionOptions = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty("itemID", itemID);

      var itemScope = Scope.FindTagWithAttribute("span.tabStripTab", DiagnosticMetadataAttributes.ItemID, itemID);
      var itemCommand = FindItemCommand(itemScope);

      if (itemCommand.IsDisabled())
        throw AssertionExceptionUtility.CreateCommandDisabledException(Driver, operationName: "SwitchTo(itemID)");

      return SwitchTo(itemCommand, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithTabs.WithItemID (string itemID, IWebTestActionOptions? actionOptions)
    {
      ArgumentUtility.CheckNotNullOrEmpty("itemID", itemID);

      var itemScope = Scope.FindTagWithAttribute("span.tabStripTab", DiagnosticMetadataAttributes.ItemID, itemID);
      var itemCommand = FindItemCommand(itemScope);

      if (itemCommand.IsDisabled())
        throw AssertionExceptionUtility.CreateCommandDisabledException(Driver, operationName: "SwitchTo.WithItemID");

      return SwitchTo(itemCommand, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithTabs.WithIndex (int oneBasedIndex, IWebTestActionOptions? actionOptions)
    {
      var xPathSelector = string.Format(
          "(.//span{0})[{1}]",
          DomSelectorUtility.CreateHasOneOfClassesCheckForXPath(new[] { "tabStripTab", "tabStripTabSelected" }),
          oneBasedIndex);
      var itemScope = Scope.FindXPath(xPathSelector);
      var itemCommand = FindItemCommand(itemScope);

      if (itemCommand.IsDisabled())
        throw AssertionExceptionUtility.CreateCommandDisabledException(Driver, operationName: "SwitchTo.WithIndex");

      return SwitchTo(itemCommand, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithTabs.WithHtmlID (string htmlID, IWebTestActionOptions? actionOptions)
    {
      ArgumentUtility.CheckNotNullOrEmpty("htmlID", htmlID);

      var itemScope = Scope.FindId(htmlID);
      var itemCommand = FindItemCommand(itemScope);

      if (itemCommand.IsDisabled())
        throw AssertionExceptionUtility.CreateCommandDisabledException(Driver, operationName: "SwitchTo.WithHtmlID");

      return SwitchTo(itemCommand, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithTabs.WithDisplayText (string displayText, IWebTestActionOptions? actionOptions)
    {
      ArgumentUtility.CheckNotNullOrEmpty("displayText", displayText);

      var itemScope = Scope.FindTagWithAttribute("span.tabStripTab", DiagnosticMetadataAttributes.Content, displayText);
      var itemCommand = FindItemCommand(itemScope);

      if (itemCommand.IsDisabled())
        throw AssertionExceptionUtility.CreateCommandDisabledException(Driver, operationName: "SwitchTo.WithDisplayText");

      return SwitchTo(itemCommand, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithTabs.WithDisplayTextContains (string containsDisplayText, IWebTestActionOptions? actionOptions)
    {
      ArgumentUtility.CheckNotNullOrEmpty("containsDisplayText", containsDisplayText);

      var itemScope = Scope.FindTagWithAttributeUsingOperator(
          "span.tabStripTab",
          CssComparisonOperator.SubstringMatch,
          DiagnosticMetadataAttributes.Content,
          containsDisplayText);
      var itemCommand = FindItemCommand(itemScope);

      if (itemCommand.IsDisabled())
        throw AssertionExceptionUtility.CreateCommandDisabledException(Driver, operationName: "SwitchTo.WithDisplayTextContains");

      return SwitchTo(itemCommand, actionOptions);
    }

    private UnspecifiedPageObject SwitchTo (CommandControlObject itemCommand, IWebTestActionOptions? actionOptions)
    {
      return itemCommand.Click(actionOptions);
    }

    private CommandControlObject FindItemCommand (ElementScope itemScope)
    {
      var itemCommandScope = itemScope.FindLink();

      return new CommandControlObject(Context.CloneForControl(itemCommandScope));
    }
  }
}
