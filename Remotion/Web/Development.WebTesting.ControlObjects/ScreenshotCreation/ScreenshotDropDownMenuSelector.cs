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
using Remotion.Utilities;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent.Selectors;

namespace Remotion.Web.Development.WebTesting.ControlObjects.ScreenshotCreation
{
  /// <summary>
  /// Selector for selecting a <see cref="DropDownMenuControlObject"/> item.
  /// </summary>
  public class ScreenshotDropDownMenuSelector
      : IFluentHtmlIDSelector<ElementScope>,
          IFluentItemIDSelector<ElementScope>,
          IFluentIndexSelector<ElementScope>,
          IFluentDisplayTextSelector<ElementScope>,
          IFluentDisplayTextContainsSelector<ElementScope>
  {
    private readonly DropDownMenuControlObject _dropDownMenu;

    public ScreenshotDropDownMenuSelector ([NotNull] DropDownMenuControlObject dropDownMenu)
    {
      ArgumentUtility.CheckNotNull("dropDownMenu", dropDownMenu);

      _dropDownMenu = dropDownMenu;
    }

    /// <inheritdoc />
    public FluentScreenshotElement<ElementScope> WithHtmlID (string htmlID)
    {
      ArgumentUtility.CheckNotNull("htmlID", htmlID);

      var scope = GetDropDownScope();
      var item = scope.FindId(htmlID);

      return item.ForElementScopeScreenshot();
    }

    /// <inheritdoc />
    public FluentScreenshotElement<ElementScope> WithItemID (string itemID)
    {
      ArgumentUtility.CheckNotNull("itemID", itemID);

      var scope = GetDropDownScope();
      var item = scope.FindTagWithAttribute("li.DropDownMenuItem", DiagnosticMetadataAttributes.ItemID, itemID);

      return item.ForElementScopeScreenshot();
    }

    /// <inheritdoc />
    public FluentScreenshotElement<ElementScope> WithIndex (int oneBasedIndex)
    {
      var scope = GetDropDownScope();
      var item = scope.FindXPath(string.Format("li[{0}]", oneBasedIndex));

      return item.ForElementScopeScreenshot();
    }

    /// <inheritdoc />
    public FluentScreenshotElement<ElementScope> WithDisplayText (string displayText)
    {
      ArgumentUtility.CheckNotNull("displayText", displayText);

      var scope = GetDropDownScope();
      var item = scope.FindTagWithAttribute("li.DropDownMenuItem", DiagnosticMetadataAttributes.Content, displayText);

      return item.ForElementScopeScreenshot();
    }

    /// <inheritdoc />
    public FluentScreenshotElement<ElementScope> WithDisplayTextContains (string displayText)
    {
      ArgumentUtility.CheckNotNull("displayText", displayText);

      var scope = GetDropDownScope();
      var item = scope.FindTagWithAttributeUsingOperator(
          "li.DropDownMenuItem",
          CssComparisonOperator.SubstringMatch,
          DiagnosticMetadataAttributes.Content,
          displayText);

      return FluentUtility.CreateFluentElementScope(item);
    }

    private ElementScope GetDropDownScope ()
    {
      var context = _dropDownMenu.Context.RootScope.FindCss("ul.DropDownMenuOptions", Options.NoWait);
      context.EnsureExistence();

      return context;
    }
  }
}
