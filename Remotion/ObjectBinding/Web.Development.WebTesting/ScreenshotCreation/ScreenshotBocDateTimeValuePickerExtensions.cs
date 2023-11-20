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
using Coypu;
using JetBrains.Annotations;
using OpenQA.Selenium;
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
    public static FluentScreenshotElement<ElementScope> GetElement (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocDateTimeValuePicker> fluentDatePicker)
    {
      ArgumentUtility.CheckNotNull("fluentDatePicker", fluentDatePicker);

      if (!IsVisible(fluentDatePicker))
        throw new InvalidOperationException("The date-picker is not visible.");

      var dateTimeValue = fluentDatePicker.Target.DateTimeValue;
      var id = string.Join("_", dateTimeValue.Scope.Id, "DatePicker");
      var picker = dateTimeValue.Context.RootScope.FindId(id, Options.NoWait);
      picker.EnsureExistence();
      var frame = picker.FindCss("iframe", Options.NoWait);
      frame.EnsureExistence();
      var calendar = frame.FindId("Calendar", Options.NoWait);

      return calendar.ForElementScopeScreenshot();
    }

    /// <summary>
    /// Returns the navigation bar of the date-picker.
    /// </summary>
    public static FluentScreenshotElement<ElementScope> GetNavigationBar (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocDateTimeValuePicker> fluentDatePicker)
    {
      ArgumentUtility.CheckNotNull("fluentDatePicker", fluentDatePicker);

      var root = fluentDatePicker.GetElement();

      var element = ((IFluentScreenshotElementWithCovariance<ElementScope>)root).Target.FindCss("table");
      element.EnsureExistence();

      return FluentUtility.CloneWith(root, element);
    }

    /// <summary>
    /// Returns the next-month button of the date-picker.
    /// </summary>
    public static FluentScreenshotElement<ElementScope> GetNextMonthButton (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocDateTimeValuePicker> fluentDatePicker)
    {
      ArgumentUtility.CheckNotNull("fluentDatePicker", fluentDatePicker);

      var headerBar = fluentDatePicker.GetNavigationBar();

      var element = ((IFluentScreenshotElementWithCovariance<ElementScope>)headerBar).GetTarget().FindCss("tbody > tr > td:nth-child(3)", Options.NoWait);
      element.EnsureExistence();

      return FluentUtility.CloneWith(headerBar, element);
    }

    /// <summary>
    /// Returns the previous-month button of the date-picker.
    /// </summary>
    public static FluentScreenshotElement<ElementScope> GetPreviousMonthButton (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocDateTimeValuePicker> fluentDatePicker)
    {
      ArgumentUtility.CheckNotNull("fluentDatePicker", fluentDatePicker);

      var headerBar = fluentDatePicker.GetNavigationBar();

      var element = ((IFluentScreenshotElementWithCovariance<ElementScope>)headerBar).GetTarget().FindCss("tbody > tr > td:nth-child(1)", Options.NoWait);
      element.EnsureExistence();

      return FluentUtility.CloneWith(headerBar, element);
    }

    /// <summary>
    /// Returns the title element of the date-picker.
    /// </summary>
    public static FluentScreenshotElement<ElementScope> GetTitle (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocDateTimeValuePicker> fluentDatePicker)
    {
      ArgumentUtility.CheckNotNull("fluentDatePicker", fluentDatePicker);

      var headerBar = fluentDatePicker.GetNavigationBar();

      var element = ((IFluentScreenshotElementWithCovariance<ElementScope>)headerBar).GetTarget().FindCss("tbody > tr > td:nth-child(2)", Options.NoWait);
      element.EnsureExistence();

      return FluentUtility.CloneWith(headerBar, element);
    }

    /// <summary>
    /// Returns the currently selected day of the date-picker.
    /// </summary>
    public static FluentScreenshotElement<ElementScope> GetSelectedDay (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocDateTimeValuePicker> fluentDatePicker)
    {
      ArgumentUtility.CheckNotNull("fluentDatePicker", fluentDatePicker);

      var root = fluentDatePicker.GetElement();

      var date = fluentDatePicker.Target.DateTimeValue.GetDateTime();
      var x = GetDayOfTheWeekIndex(date);
      var y = (GetDayOfTheWeekIndex(new DateTime(date.Year, date.Month, 1)) + date.Day - 1) / 7;

      var element = ((IFluentScreenshotElementWithCovariance<ElementScope>)root).Target.FindCss(string.Format("tbody > tr:nth-child({0}) > td:nth-child({1})", y + 3, x + 1));
      element.EnsureExistence();

      return FluentUtility.CloneWith(root, element);
    }

    /// <summary>
    /// Returns the header row indicating the weekdays of the date-picker.
    /// </summary>
    public static FluentScreenshotElement<ElementScope> GetWeekdayRow (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocDateTimeValuePicker> fluentDatePicker)
    {
      ArgumentUtility.CheckNotNull("fluentDatePicker", fluentDatePicker);

      var root = fluentDatePicker.GetElement();

      var element = ((IFluentScreenshotElementWithCovariance<ElementScope>)root).Target.FindCss("tbody > tr:nth-child(2)");
      element.EnsureExistence();

      return FluentUtility.CloneWith(root, element);
    }

    /// <summary>
    /// Opens the date-picker.
    /// </summary>
    /// <param name="fluentDatePicker">The date-picker to be opened.</param>
    /// <param name="timeout">A maximum timeout until the date-picker must be opened. Default is 3000 ms.</param>
    public static void Open (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocDateTimeValuePicker> fluentDatePicker,
        int timeout = 3000)
    {
      ArgumentUtility.CheckNotNull("fluentDatePicker", fluentDatePicker);

      var element = fluentDatePicker.Target.FluentDateTimeValue.GetDatePickerIcon().GetTarget();
      element.Click();

      WaitUntilVisible(fluentDatePicker, TimeSpan.FromMilliseconds(timeout));
    }

    private static int GetDayOfTheWeekIndex (DateTime date)
    {
      if (date.DayOfWeek == DayOfWeek.Sunday)
        return 6;
      return (int)date.DayOfWeek - 1;
    }

    private static bool IsVisible (IFluentScreenshotElementWithCovariance<ScreenshotBocDateTimeValuePicker> fluentDatePicker)
    {
      var dateTimeValue = fluentDatePicker.Target.DateTimeValue;
      var id = GetDatePickerID(dateTimeValue);
      var result = dateTimeValue.Context.RootScope.FindId(id, Options.NoWait);

      return result.ExistsWorkaround();
    }

    private static void WaitUntilVisible (IFluentScreenshotElementWithCovariance<ScreenshotBocDateTimeValuePicker> fluentDatePicker, TimeSpan timeout)
    {
      var dateTimeValue = fluentDatePicker.Target.DateTimeValue;
      var datePickerID = GetDatePickerID(dateTimeValue);
      var seleniumDriver = (IWebDriver)dateTimeValue.Context.Browser.Driver.Native;
      var iframe = seleniumDriver.FindElement(By.Id(datePickerID)).FindElement(By.TagName("iframe"));
      iframe.WaitUntilFrameIsVisible("#Calendar", timeout);
    }

    private static string GetDatePickerID (BocDateTimeValueControlObject dateTimeValue)
    {
      return string.Join("_", dateTimeValue.Scope.Id, "DatePicker");
    }
  }
}
