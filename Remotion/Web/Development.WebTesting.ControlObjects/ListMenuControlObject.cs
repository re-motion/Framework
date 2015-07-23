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
      : WebFormsControlObjectWithDiagnosticMetadata, IControlObjectWithSelectableItems, IFluentControlObjectWithSelectableItems
  {
    public ListMenuControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <inheritdoc/>
    public IReadOnlyList<ItemDefinition> GetItemDefinitions ()
    {
      return
          RetryUntilTimeout.Run (
              () =>
                  Scope.FindAllCss ("span.listMenuItem, span.listMenuItemDisabled")
                      .Select (
                          (itemScope, i) =>
                              new ItemDefinition (
                                  itemScope[DiagnosticMetadataAttributes.ItemID],
                                  i + 1,
                                  itemScope.Text.Trim(),
                                  !itemScope["class"].Contains ("listMenuItemDisabled")))
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

      var itemScope = Scope.FindTagWithAttribute ("span.listMenuItem", DiagnosticMetadataAttributes.ItemID, itemID);
      return ClickItem (itemScope, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithIndex (int index, IWebTestActionOptions actionOptions)
    {
      var itemScope = Scope.FindChild ((index - 1).ToString());
      return ClickItem (itemScope, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithHtmlID (string htmlID, IWebTestActionOptions actionOptions)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      var itemScope = Scope.FindId (htmlID);
      return ClickItem (itemScope, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithDisplayText (string displayText, IWebTestActionOptions actionOptions)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("displayText", displayText);

      var itemScope = Scope.FindTagWithAttribute ("span.listMenuItem", DiagnosticMetadataAttributes.Content, displayText);
      return ClickItem (itemScope, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithDisplayTextContains (
        string containsDisplayText,
        IWebTestActionOptions actionOptions)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("containsDisplayText", containsDisplayText);

      var itemScope = Scope.FindTagWithAttributeUsingOperator (
          "span.listMenuItem",
          CssComparisonOperator.SubstringMatch,
          DiagnosticMetadataAttributes.Content,
          containsDisplayText);
      return ClickItem (itemScope, actionOptions);
    }

    private UnspecifiedPageObject ClickItem (ElementScope itemScope, IWebTestActionOptions actionOptions)
    {
      var itemCommandScope = itemScope.FindLink();
      var itemCommand = new CommandControlObject (Context.CloneForControl (itemCommandScope));
      return itemCommand.Click (actionOptions);
    }
  }
}