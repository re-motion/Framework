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
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation.BocList;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation
{
  /// <summary>
  /// Provides fluent extension methods for navigating a <see cref="BocListControlObjectBase{TRowControlObject,TCellControlObject}"/> menu-block.
  /// </summary>
  public static class ScreenshotBocListMenuBlockExtensions
  {
    /// <summary>
    /// Starts the fluent API for controlling the DropDownMenu in the menu-block.
    /// </summary>
    public static FluentScreenshotElement<DropDownMenuControlObject> GetDropDownMenu<TList, TRow, TCell> (
        [NotNull] this IFluentScreenshotElement<ScreenshotBocListMenuBlock<TList, TRow, TCell>> listMenu)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      ArgumentUtility.CheckNotNull ("listMenu", listMenu);

      return listMenu.Target.List.GetDropDownMenu().ForScreenshot();
    }

    /// <summary>
    /// Starts the fluent API for controlling the ListMenu in the menu-block.
    /// </summary>
    public static FluentScreenshotElement<ListMenuControlObject> GetListMenu<TList, TRow, TCell> (
        [NotNull] this IFluentScreenshotElement<ScreenshotBocListMenuBlock<TList, TRow, TCell>> listMenu)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      ArgumentUtility.CheckNotNull ("listMenu", listMenu);

      return listMenu.Target.List.GetListMenu().ForScreenshot();
    }

    /// <summary>
    /// Starts the fluent API for controlling the ViewsMenu in the menu-block.
    /// </summary>
    /// <exception cref="MissingHtmlException">Can not find the view menu of the navigator.</exception>
    public static FluentScreenshotElement<ScreenshotBocListDropDown<TList, TRow, TCell>> GetViewsMenu<TList, TRow, TCell> (
        [NotNull] this IFluentScreenshotElement<ScreenshotBocListMenuBlock<TList, TRow, TCell>> listMenu)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      ArgumentUtility.CheckNotNull ("listMenu", listMenu);

      var result = listMenu.Target.List.Scope.FindChild ("Boc_AvailableViewsList", Options.NoWait);
      result.EnsureExistence();

      return
          SelfResolvableFluentScreenshot.Create (
              new ScreenshotBocListDropDown<TList, TRow, TCell> (listMenu.Target.FluentList, result.ForElementScopeScreenshot()));
    }
  }
}