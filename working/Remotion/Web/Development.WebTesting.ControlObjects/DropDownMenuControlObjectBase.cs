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
using Remotion.Web.Development.WebTesting.WebTestActions;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Base class for all control objects representing a <see cref="T:Remotion.Web.UI.Controls.DropDownMenu"/>.
  /// </summary>
  public abstract class DropDownMenuControlObjectBase
      : WebFormsControlObjectWithDiagnosticMetadata, IControlObjectWithSelectableItems, IFluentControlObjectWithSelectableItems
  {
    protected DropDownMenuControlObjectBase ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Template method for derived classes to open the drop down menu (i.e. make it visible on the page).
    /// </summary>
    protected abstract void OpenDropDownMenu ();

    /// <inheritdoc/>
    public IReadOnlyList<ItemDefinition> GetItemDefinitions ()
    {
      var dropDownMenuScope = GetDropDownMenuScope();

      return RetryUntilTimeout.Run (
          () => dropDownMenuScope.FindAllCss ("li.DropDownMenuItem, li.DropDownMenuItemDisabled")
              .Select (
                  (itemScope, i) =>
                      new ItemDefinition (
                          itemScope[DiagnosticMetadataAttributes.ItemID],
                          i + 1,
                          itemScope.Text.Trim(),
                          !itemScope["class"].Contains ("DropDownMenuItemDisabled")))
              .ToList());
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

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithItemID (string itemID, IWebTestActionOptions actionOptions)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindTagWithAttribute ("li.DropDownMenuItem", DiagnosticMetadataAttributes.ItemID, itemID);
      return ClickItem (scope, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithIndex (int index, IWebTestActionOptions actionOptions)
    {
      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindXPath (string.Format ("li[{0}]", index));
      return ClickItem (scope, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithHtmlID (string htmlID, IWebTestActionOptions actionOptions)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindId (htmlID);
      return ClickItem (scope, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithDisplayText (string displayText, IWebTestActionOptions actionOptions)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("displayText", displayText);

      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindTagWithAttribute ("li.DropDownMenuItem", DiagnosticMetadataAttributes.Content, displayText);
      return ClickItem (scope, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithDisplayTextContains (
        string containsDisplayText,
        IWebTestActionOptions actionOptions)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("containsDisplayText", containsDisplayText);

      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindTagWithAttributeUsingOperator (
          "li.DropDownMenuItem",
          CssComparisonOperator.SubstringMatch,
          DiagnosticMetadataAttributes.Content,
          containsDisplayText);
      return ClickItem (scope, actionOptions);
    }

    private ElementScope GetDropDownMenuScope ()
    {
      OpenDropDownMenu();

      var dropDownMenuOptionsScope = Context.RootScope.FindCss ("ul.DropDownMenuOptions");
      return dropDownMenuOptionsScope;
    }

    private UnspecifiedPageObject ClickItem (ElementScope item, IWebTestActionOptions actionOptions)
    {
      var actualActionOptions = MergeWithDefaultActionOptions (item, actionOptions);

      var anchorScope = item.FindLink();
      new ClickAction (this, anchorScope).Execute (actualActionOptions);
      return UnspecifiedPage();
    }
  }
}