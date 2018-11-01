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
  /// Provides fluent extension methods for controlling a <see cref="BocListControlObjectBase{TRowControlObject,TCellControlObject}"/>.
  /// </summary>
  public static class ScreenshotBocListExtensions
  {
    /// <summary>
    /// Starts the fluent API for controlling the menu-block of the <see cref="BocListControlObjectBase{TRowControlObject,TCellControlObject}"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">The menu-block could not be found.</exception>
    public static FluentScreenshotElement<ScreenshotBocListMenuBlock<TList, TRow, TCell>> GetMenuBlock<TList, TRow, TCell> (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocList<TList, TRow, TCell>> fluentList)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentList", fluentList);

      var result = fluentList.Target.List.Scope.FindCss (".bocListMenuBlock", Options.NoWait);
      if (!result.Exists (Options.NoWait))
        throw new InvalidOperationException ("Can not find a menu-block for this BocList.");

      return
          SelfResolvableFluentScreenshot.Create (new ScreenshotBocListMenuBlock<TList, TRow, TCell> (fluentList, result.ForElementScopeScreenshot()));
    }

    /// <summary>
    /// Starts the fluent API for controlling the navigator of the <see cref="BocListControlObjectBase{TRowControlObject,TCellControlObject}"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">The navigator could not be found.</exception>
    public static FluentScreenshotElement<ScreenshotBocListNavigator<TList, TRow, TCell>> GetNavigator<TList, TRow, TCell> (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocList<TList, TRow, TCell>> fluentList)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentList", fluentList);

      var result = fluentList.Target.List.Scope.FindCss (".bocListNavigator", Options.NoWait);
      if (!result.Exists (Options.NoWait))
        throw new InvalidOperationException ("Can not find a navigator for this BocList.");

      return
          SelfResolvableFluentScreenshot.Create (new ScreenshotBocListNavigator<TList, TRow, TCell> (fluentList, result.ForElementScopeScreenshot()));
    }

    /// <summary>
    /// Starts the fluent API for controlling the table-container of the <see cref="BocListControlObjectBase{TRowControlObject,TCellControlObject}"/>.
    /// </summary>
    public static FluentScreenshotElement<ScreenshotBocListTableContainer<TList, TRow, TCell>> GetTableContainer<TList, TRow, TCell> (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocList<TList, TRow, TCell>> fluentList)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentList", fluentList);

      var result = fluentList.Target.List.Scope.FindCss (".bocListTableContainer", Options.NoWait);
      result.EnsureExistence();

      return
          SelfResolvableFluentScreenshot.Create (
              new ScreenshotBocListTableContainer<TList, TRow, TCell> (fluentList, result.ForElementScopeScreenshot()));
    }
  }
}