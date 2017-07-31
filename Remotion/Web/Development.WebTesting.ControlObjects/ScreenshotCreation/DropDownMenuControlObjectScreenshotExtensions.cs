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
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;

namespace Remotion.Web.Development.WebTesting.ControlObjects.ScreenshotCreation
{
  /// <summary>
  /// Screenshot extensions for <see cref="DropDownMenuControlObject"/>.
  /// </summary>
  public static class DropDownMenuControlObjectScreenshotExtensions
  {
    /// <summary>
    /// Returns the drop down menu.
    /// </summary>
    public static FluentScreenshotElement<ElementScope> GetMenu ([NotNull] this IFluentScreenshotElement<DropDownMenuControlObject> fluentDropDownMenu)
    {
      ArgumentUtility.CheckNotNull ("fluentDropDownMenu", fluentDropDownMenu);

      var menu = fluentDropDownMenu.Target.Context.RootScope.FindCss ("div.DropDownMenuOptions", Options.NoWait);
      if (!menu.Exists (Options.NoWait))
        throw new InvalidOperationException ("Could not find the drop-down-menu.");

      return menu.ForElementScopeScreenshot();
    }

    /// <summary>
    /// Opens the drop down menu.
    /// </summary>
    public static void OpenMenu ([NotNull] this IFluentScreenshotElement<DropDownMenuControlObject> fluentDropDownMenu)
    {
      ArgumentUtility.CheckNotNull ("fluentDropDownMenu", fluentDropDownMenu);

      if (!fluentDropDownMenu.Target.ExistsDropDownScope())
        fluentDropDownMenu.Target.Scope.FindCss ("a.DropDownMenuButton").Click();
    }

    /// <summary>
    /// Starts the fluent interface for selecting a drop down menu item.
    /// </summary>
    public static ScreenshotDropDownMenuSelector SelectItem ([NotNull] this IFluentScreenshotElement<DropDownMenuControlObject> fluentDropDownMenu)
    {
      ArgumentUtility.CheckNotNull ("fluentDropDownMenu", fluentDropDownMenu);

      return new ScreenshotDropDownMenuSelector (fluentDropDownMenu.Target);
    }

    private static bool ExistsDropDownScope (this DropDownMenuControlObject controlObject)
    {
      return controlObject.Context.RootScope.FindCss ("ul.DropDownMenuOptions", Options.NoWait).Exists (Options.NoWait);
    }
  }
}