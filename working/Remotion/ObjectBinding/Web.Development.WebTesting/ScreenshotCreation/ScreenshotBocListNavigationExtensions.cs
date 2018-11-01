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
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation.BocList;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation
{
  /// <summary>
  /// Provides fluent extension methods for controlling a <see cref="BocListControlObjectBase{TRowControlObject,TCellControlObject}"/> navigation.
  /// </summary>
  public static class ScreenshotBocListNavigationExtensions
  {
    /// <summary>
    /// Returns the text information field of the navigator.
    /// </summary>
    /// <exception cref="MissingHtmlException">Can not find the information field of the navigator.</exception>
    public static FluentScreenshotElement<ElementScope> GetPageInformationText<TList, TRow, TCell> (
        this IFluentScreenshotElementWithCovariance<ScreenshotBocListNavigator<TList, TRow, TCell>> fluentNavigation)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      var element = fluentNavigation.Target.Element.FindCss ("span", Options.NoWait);
      element.EnsureExistence();

      return element.ForElementScopeScreenshot();
    }

    /// <summary>
    /// Returns the page number input field of the navigator.
    /// </summary>
    /// <exception cref="MissingHtmlException">Can not find the input field of the navigator.</exception>
    public static FluentScreenshotElement<ElementScope> GetPageNumberInput<TList, TRow, TCell> (
        this IFluentScreenshotElementWithCovariance<ScreenshotBocListNavigator<TList, TRow, TCell>> fluentNavigation)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      var element = fluentNavigation.Target.Element.FindCss ("span > input", Options.NoWait);
      element.EnsureExistence();

      return element.ForElementScopeScreenshot();
    }

    /// <summary>
    /// Returns the first-page button of the navigator.
    /// </summary>
    /// <exception cref="MissingHtmlException">Can not find the first-page button of the navigator.</exception>
    public static FluentScreenshotElement<ElementScope> GetFirstPageButton<TList, TRow, TCell> (
        this IFluentScreenshotElementWithCovariance<ScreenshotBocListNavigator<TList, TRow, TCell>> fluentNavigation)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      var scope = fluentNavigation.Target.List.Scope;
      var element = scope.FindChild ("Navigation_First", Options.NoWait);
      element.EnsureExistence();

      return element.ForElementScopeScreenshot();
    }

    /// <summary>
    /// Returns the previous-page button of the navigator.
    /// </summary>
    /// <exception cref="MissingHtmlException">Can not find the previous-page button of the navigator.</exception>
    public static FluentScreenshotElement<ElementScope> GetPreviousPageButton<TList, TRow, TCell> (
        this IFluentScreenshotElementWithCovariance<ScreenshotBocListNavigator<TList, TRow, TCell>> fluentNavigation)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      var scope = fluentNavigation.Target.List.Scope;
      var element = scope.FindChild ("Navigation_Previous", Options.NoWait);
      element.EnsureExistence();

      return element.ForElementScopeScreenshot();
    }

    /// <summary>
    /// Returns the next-page button of the navigator.
    /// </summary>
    /// <exception cref="MissingHtmlException">Can not find the next-page button of the navigator.</exception>
    public static FluentScreenshotElement<ElementScope> GetNextPageButton<TList, TRow, TCell> (
        this IFluentScreenshotElementWithCovariance<ScreenshotBocListNavigator<TList, TRow, TCell>> fluentNavigation)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      var scope = fluentNavigation.Target.List.Scope;
      var element = scope.FindChild ("Navigation_Next", Options.NoWait);
      element.EnsureExistence();

      return element.ForElementScopeScreenshot();
    }

    /// <summary>
    /// Returns the last-page button of the navigator.
    /// </summary>
    /// <exception cref="MissingHtmlException">Can not find the last-page button of the navigator.</exception>
    public static FluentScreenshotElement<ElementScope> GetLastPageButton<TList, TRow, TCell> (
        this IFluentScreenshotElementWithCovariance<ScreenshotBocListNavigator<TList, TRow, TCell>> fluentNavigation)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      var scope = fluentNavigation.Target.List.Scope;
      var element = scope.FindChild ("Navigation_Last", Options.NoWait);
      element.EnsureExistence();

      return element.ForElementScopeScreenshot();
    }
  }
}