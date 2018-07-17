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
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;

namespace Remotion.Web.Development.WebTesting.ControlObjects.ScreenshotCreation
{
  /// <summary>
  /// Screenshot extensions for <see cref="TabbedMenuControlObject"/>.
  /// </summary>
  public static class TabbedMenuControlObjectScreenshotExtensions
  {
    /// <summary>
    /// Starts the fluent interface for selecting an item from the tabbed menu.
    /// </summary>
    public static ScreenshotTabbedMenuSelector SelectItem ([NotNull] this IFluentScreenshotElementWithCovariance<TabbedMenuControlObject> fluentTabbedMenu)
    {
      ArgumentUtility.CheckNotNull ("fluentTabbedMenu", fluentTabbedMenu);

      return new ScreenshotTabbedMenuSelector (fluentTabbedMenu.Target.Scope.FindCss ("td.tabbedMainMenuCell"));
    }

    /// <summary>
    /// Starts the fluent interface for the tabbed sub menu.
    /// </summary>
    public static FluentScreenshotElement<ScreenshotTabbedSubMenu> GetSubMenu ([NotNull] this IFluentScreenshotElementWithCovariance<TabbedMenuControlObject> fluentTabbedMenu)
    {
      ArgumentUtility.CheckNotNull ("fluentTabbedMenu", fluentTabbedMenu);

      var target = fluentTabbedMenu.Target.Scope.FindCss ("td.tabbedSubMenuCell");

      return SelfResolvableFluentScreenshot.Create (new ScreenshotTabbedSubMenu (fluentTabbedMenu, target.ForElementScopeScreenshot()));
    }

    /// <summary>
    /// Starts the fluent interface for selecting an item from the tabbed sub menu.
    /// </summary>
    public static ScreenshotTabbedMenuSelector SelectItem ([NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotTabbedSubMenu> fluentTabbedSubMenu)
    {
      ArgumentUtility.CheckNotNull ("fluentTabbedSubMenu", fluentTabbedSubMenu);

      return new ScreenshotTabbedMenuSelector (fluentTabbedSubMenu.Target.Element);
    }
  }
}