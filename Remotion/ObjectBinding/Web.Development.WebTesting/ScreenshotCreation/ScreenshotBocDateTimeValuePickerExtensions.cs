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
using System.Diagnostics;
using System.Threading;
using Coypu;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation
{
  /// <summary>
  /// Provides fluent extension methods for controlling a <see cref="BocDateTimeValueControlObject"/> date-picker.
  /// </summary>
  public static class ScreenshotBocDateTimeValuePickerExtensions
  {
    public static IFluentScreenshotElement<ElementScope> GetElement (
        [NotNull] this IFluentScreenshotElement<ScreenshotBocDateTimeValuePicker> fluentDatePicker)
    {
      ArgumentUtility.CheckNotNull ("fluentDatePicker", fluentDatePicker);

      if (!IsVisible (fluentDatePicker))
        throw new InvalidOperationException ("The date-picker is not visible.");

      var dateTimeValue = fluentDatePicker.Target.DateTimeValue;
      var id = string.Join ("_", dateTimeValue.Scope.Id, "DatePicker");
      var picker = dateTimeValue.Context.RootScope.FindId (id, Options.NoWait);
      picker.EnsureExistence();
      var frame = picker.FindCss ("iframe", Options.NoWait);
      frame.EnsureExistence();
      var calendar = frame.FindId ("Calendar", Options.NoWait);

      return calendar.ForElementScopeScreenshot();
    }

    /// <summary>
    /// Returns the navigation bar of the date-picker.
    /// </summary>
    public static FluentScreenshotElement<ElementScope> GetNavigationBar (
        [NotNull] this IFluentScreenshotElement<ScreenshotBocDateTimeValuePicker> fluentDatePicker)
    {
      ArgumentUtility.CheckNotNull ("fluentDatePicker", fluentDatePicker);

      var root = fluentDatePicker.GetElement();

      var element = root.Target.FindCss ("table");
      element.EnsureExistence();

      return FluentUtility.CloneWith (root, element);
    }

    /// <summary>
    /// Returns the next-month button of the date-picker.
    /// </summary>
    public static FluentScreenshotElement<ElementScope> GetNextMonthButton (
        [NotNull] this IFluentScreenshotElement<ScreenshotBocDateTimeValuePicker> fluentDatePicker)
    {
      ArgumentUtility.CheckNotNull ("fluentDatePicker", fluentDatePicker);

      var headerBar = fluentDatePicker.GetNavigationBar();

      var element = headerBar.GetTarget().FindCss ("tbody > tr > td:nth-child(3)", Options.NoWait);
      element.EnsureExistence();

      return FluentUtility.CloneWith (headerBar, element);
    }

    /// <summary>
    /// Returns the previous-month button of the date-picker.
    /// </summary>
    public static FluentScreenshotElement<ElementScope> GetPreviousMonthButton (
        [NotNull] this IFluentScreenshotElement<ScreenshotBocDateTimeValuePicker> fluentDatePicker)
    {
      ArgumentUtility.CheckNotNull ("fluentDatePicker", fluentDatePicker);

      var headerBar = fluentDatePicker.GetNavigationBar();

      var element = headerBar.GetTarget().FindCss ("tbody > tr > td:nth-child(1)", Options.NoWait);
      element.EnsureExistence();

      return FluentUtility.CloneWith (headerBar, element);
    }

    /// <summary>
    /// Returns the title element of the date-picker.
    /// </summary>
    public static FluentScreenshotElement<ElementScope> GetTitle (
        [NotNull] this IFluentScreenshotElement<ScreenshotBocDateTimeValuePicker> fluentDatePicker)
    {
      ArgumentUtility.CheckNotNull ("fluentDatePicker", fluentDatePicker);

      var headerBar = fluentDatePicker.GetNavigationBar();

      var element = headerBar.GetTarget().FindCss ("tbody > tr > td:nth-child(2)", Options.NoWait);
      element.EnsureExistence();

      return FluentUtility.CloneWith (headerBar, element);
    }

    /// <summary>
    /// Returns the currently selected day of the date-picker.
    /// </summary>
    public static FluentScreenshotElement<ElementScope> GetSelectedDay (
        [NotNull] this IFluentScreenshotElement<ScreenshotBocDateTimeValuePicker> fluentDatePicker)
    {
      ArgumentUtility.CheckNotNull ("fluentDatePicker", fluentDatePicker);

      var root = fluentDatePicker.GetElement();

      var date = fluentDatePicker.Target.DateTimeValue.GetDateTime();
      var x = GetDayOfTheWeekIndex (date);
      var y = (GetDayOfTheWeekIndex (new DateTime (date.Year, date.Month, 1)) + date.Day - 1) / 7;

      var element = root.Target.FindCss (string.Format ("tbody > tr:nth-child({0}) > td:nth-child({1})", y + 3, x + 1));
      element.EnsureExistence();

      return FluentUtility.CloneWith (root, element);
    }

    /// <summary>
    /// Returns the header row indicating the weekdays of the date-picker.
    /// </summary>
    public static FluentScreenshotElement<ElementScope> GetWeekdayRow (
        [NotNull] this IFluentScreenshotElement<ScreenshotBocDateTimeValuePicker> fluentDatePicker)
    {
      ArgumentUtility.CheckNotNull ("fluentDatePicker", fluentDatePicker);

      var root = fluentDatePicker.GetElement();

      var element = root.Target.FindCss ("tbody > tr:nth-child(2)");
      element.EnsureExistence();

      return FluentUtility.CloneWith (root, element);
    }

    /// <summary>
    /// Opens the date-picker.
    /// </summary>
    public static void Open ([NotNull] this IFluentScreenshotElement<ScreenshotBocDateTimeValuePicker> fluentDatePicker, int timeout = 3000)
    {
      ArgumentUtility.CheckNotNull ("fluentDatePicker", fluentDatePicker);

      var element = fluentDatePicker.Target.FluentDateTimeValue.GetDatePickerIcon().GetTarget();
      element.Click();

      WaitUntilVisible (fluentDatePicker, timeout);
    }

    private static int GetDayOfTheWeekIndex (DateTime date)
    {
      if (date.DayOfWeek == DayOfWeek.Sunday)
        return 6;
      return (int) date.DayOfWeek - 1;
    }

    private static bool IsVisible (IFluentScreenshotElement<ScreenshotBocDateTimeValuePicker> fluentDatePicker)
    {
      var dateTimeValue = fluentDatePicker.Target.DateTimeValue;
      var id = string.Join ("_", dateTimeValue.Scope.Id, "DatePicker");
      var result = dateTimeValue.Context.RootScope.FindId (id, Options.NoWait);

      Thread.Sleep (TimeSpan.FromSeconds (2)); //Workaround. See RM-6944.
      
      return result.Exists (Options.NoWait);
    }

    private static void WaitUntilVisible (IFluentScreenshotElement<ScreenshotBocDateTimeValuePicker> fluentDatePicker, int timeout)
    {
      var watch = new Stopwatch();
      watch.Start();

      do
      {
        if (IsVisible (fluentDatePicker))
          return;

        if (watch.ElapsedMilliseconds >= timeout)
          throw new TimeoutException ("Could not wait for the timeout in the specified amount of time.");

        Thread.Sleep (50);
      } while (true);
    }
  }
}