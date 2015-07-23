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
  /// Control object for form grids created with <see cref="T:Remotion.Web.UI.Controls.TabbedMenu"/>.
  /// </summary>
  [UsedImplicitly]
  public class TabbedMenuControlObject
      : WebFormsControlObjectWithDiagnosticMetadata, IControlObjectWithSelectableItems, IFluentControlObjectWithSelectableItems
  {
    public TabbedMenuControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <inheritdoc/>
    public IReadOnlyList<ItemDefinition> GetItemDefinitions ()
    {
      return GetMenuItemOrSubMenuItemDefinitions (GetMainMenuScope());
    }

    /// <summary>
    /// Returns the tabbed menu's status text.
    /// </summary>
    public string GetStatusText ()
    {
      return Scope.FindCss ("td.tabbedMenuStatusCell").Text.Trim();
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithSelectableItems SelectItem ()
    {
      return this;
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject SelectItem (string itemID, IWebTestActionOptions actionOptions = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return SelectItem().WithItemID (itemID, actionOptions);
    }

    /// <summary>
    /// Gives access to the sub menu.
    /// </summary>
    public IControlObjectWithSelectableItems SubMenu
    {
      get { return new SubMenuItems (Context); }
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithItemID (string itemID, IWebTestActionOptions actionOptions)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var menuItemScope = GetMainMenuScope().FindTagWithAttribute ("span", DiagnosticMetadataAttributes.ItemID, itemID);
      return SelectMenuOrSubMenuItem (Context, menuItemScope, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithIndex (int index, IWebTestActionOptions actionOptions)
    {
      var menuItemScope = GetMainMenuScope().FindXPath (string.Format ("(.//li/span/span[2])[{0}]", index));
      return SelectMenuOrSubMenuItem (Context, menuItemScope, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithHtmlID (string htmlID, IWebTestActionOptions actionOptions)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      var menuItemScope = Scope.FindId (htmlID);
      return SelectMenuOrSubMenuItem (Context, menuItemScope, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithDisplayText (string displayText, IWebTestActionOptions actionOptions)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("displayText", displayText);

      var menuItemScope = GetMainMenuScope().FindTagWithAttribute ("span", DiagnosticMetadataAttributes.Content, displayText);
      return SelectMenuOrSubMenuItem (Context, menuItemScope, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithDisplayTextContains (
        string containsDisplayText,
        IWebTestActionOptions actionOptions)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("containsDisplayText", containsDisplayText);

      var menuItemScope = GetMainMenuScope()
          .FindTagWithAttributeUsingOperator ("span", CssComparisonOperator.SubstringMatch, DiagnosticMetadataAttributes.Content, containsDisplayText);
      return SelectMenuOrSubMenuItem (Context, menuItemScope, actionOptions);
    }

    private static IReadOnlyList<ItemDefinition> GetMenuItemOrSubMenuItemDefinitions (ElementScope scope)
    {
      return
          RetryUntilTimeout.Run (
              () =>
                  scope.FindAllXPath (".//li/span/span[2]")
                      .Select (
                          (itemScope, i) =>
                              new ItemDefinition (
                                  itemScope[DiagnosticMetadataAttributes.ItemID],
                                  i + 1,
                                  itemScope.Text.Trim(),
                                  true /* currently we have no way to determine if a menu item is enabled or not */))
                      .ToList());
    }

    private static UnspecifiedPageObject SelectMenuOrSubMenuItem (
        ControlObjectContext context,
        ElementScope menuItemScope,
        IWebTestActionOptions actionOptions)
    {
      var menuItemCommandScope = menuItemScope.FindLink();
      var menuItemCommand = new CommandControlObject (context.CloneForControl (menuItemCommandScope));
      return menuItemCommand.Click (actionOptions);
    }

    private ElementScope GetMainMenuScope ()
    {
      return Scope.FindCss ("td.tabbedMainMenuCell");
    }

    private class SubMenuItems
        : WebFormsControlObjectWithDiagnosticMetadata, IControlObjectWithSelectableItems, IFluentControlObjectWithSelectableItems
    {
      public SubMenuItems ([NotNull] ControlObjectContext context)
          : base (context)
      {
      }

      public IReadOnlyList<ItemDefinition> GetItemDefinitions ()
      {
        return GetMenuItemOrSubMenuItemDefinitions (GetSubMenuScope());
      }

      public IFluentControlObjectWithSelectableItems SelectItem ()
      {
        return this;
      }

      public UnspecifiedPageObject SelectItem (string itemID, IWebTestActionOptions actionOptions = null)
      {
        ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

        return SelectItem().WithItemID (itemID, actionOptions);
      }

      UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithItemID (string itemID, IWebTestActionOptions actionOptions)
      {
        ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

        var menuItemScope = GetSubMenuScope().FindTagWithAttribute ("span", DiagnosticMetadataAttributes.ItemID, itemID);
        return SelectMenuOrSubMenuItem (Context, menuItemScope, actionOptions);
      }

      UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithIndex (int index, IWebTestActionOptions actionOptions)
      {
        var menuItemScope = GetSubMenuScope().FindXPath (string.Format ("(.//li/span/span[2])[{0}]", index));
        return SelectMenuOrSubMenuItem (Context, menuItemScope, actionOptions);
      }

      UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithHtmlID (string htmlID, IWebTestActionOptions actionOptions)
      {
        ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

        var menuItemScope = Scope.FindId (htmlID);
        return SelectMenuOrSubMenuItem (Context, menuItemScope, actionOptions);
      }

      UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithDisplayText (string displayText, IWebTestActionOptions actionOptions)
      {
        ArgumentUtility.CheckNotNullOrEmpty ("displayText", displayText);

        var menuItemScope = GetSubMenuScope().FindTagWithAttribute ("span", DiagnosticMetadataAttributes.Content, displayText);
        return SelectMenuOrSubMenuItem (Context, menuItemScope, actionOptions);
      }

      UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithDisplayTextContains (
          string containsDisplayText,
          IWebTestActionOptions actionOptions)
      {
        ArgumentUtility.CheckNotNullOrEmpty ("containsDisplayText", containsDisplayText);

        var menuItemScope = GetSubMenuScope()
            .FindTagWithAttributeUsingOperator (
                "span",
                CssComparisonOperator.SubstringMatch,
                DiagnosticMetadataAttributes.Content,
                containsDisplayText);
        return SelectMenuOrSubMenuItem (Context, menuItemScope, actionOptions);
      }

      private ElementScope GetSubMenuScope ()
      {
        return Scope.FindCss ("td.tabbedSubMenuCell");
      }
    }
  }
}