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
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation
{
  /// <summary>
  /// Provides fluent extension methods for controlling a <see cref="BocDateTimeValueControlObject"/>.
  /// </summary>
  public static class ScreenshotBocDateTimeValueExtensions
  {
    /// <summary>
    /// Returns the selected <see cref="DateTime"/>.
    /// </summary>
    public static DateTime GetDateTime (
        [NotNull] this IFluentScreenshotElementWithCovariance<BocDateTimeValueControlObject> fluentDateTimeValue)
    {
      ArgumentUtility.CheckNotNull("fluentDateTimeValue", fluentDateTimeValue);

      return fluentDateTimeValue.Target.GetDateTime();
    }

    /// <summary>
    /// Returns the selected <see cref="DateTime"/> as <see cref="string"/>.
    /// </summary>
    public static string GetDateTimeAsString (
        [NotNull] this IFluentScreenshotElementWithCovariance<BocDateTimeValueControlObject> fluentDateTimeValue)
    {
      ArgumentUtility.CheckNotNull("fluentDateTimeValue", fluentDateTimeValue);

      return fluentDateTimeValue.Target.GetDateTimeAsString();
    }

    /// <summary>
    /// Returns the field for the selected date.
    /// </summary>
    public static FluentScreenshotElement<ElementScope> GetDateField (
        [NotNull] this IFluentScreenshotElementWithCovariance<BocDateTimeValueControlObject> fluentDateTimeValue)
    {
      ArgumentUtility.CheckNotNull("fluentDateTimeValue", fluentDateTimeValue);

      ElementScope target;
      if (fluentDateTimeValue.IsReadOnly())
        target = fluentDateTimeValue.Target.Scope.FindCss("span:nth-child(1)");
      else
        target = fluentDateTimeValue.Target.Scope.FindChild("DateValue", Options.NoWait);
      target.EnsureExistence();

      return target.ForElementScopeScreenshot();
    }

    /// <summary>
    /// Returns the date-picker indicator button.
    /// </summary>
    public static FluentScreenshotElement<ElementScope> GetDatePickerIcon (
        [NotNull] this IFluentScreenshotElementWithCovariance<BocDateTimeValueControlObject> fluentDateTimeValue)
    {
      ArgumentUtility.CheckNotNull("fluentDateTimeValue", fluentDateTimeValue);

      var target = fluentDateTimeValue.Target.Scope.FindChild("Boc_DatePicker", Options.NoWait);
      target.EnsureExistence();

      return target.ForElementScopeScreenshot();
    }

    /// <summary>
    /// Returns fluent API for controlling the date-picker.
    /// </summary>
    public static FluentScreenshotElement<ScreenshotBocDateTimeValuePicker> GetDatePicker (
        [NotNull] this IFluentScreenshotElementWithCovariance<BocDateTimeValueControlObject> fluentDateTimeValue)
    {
      ArgumentUtility.CheckNotNull("fluentDateTimeValue", fluentDateTimeValue);

      return SelfResolvableFluentScreenshot.Create(new ScreenshotBocDateTimeValuePicker(fluentDateTimeValue));
    }

    /// <summary>
    /// Returns the field for the selected time.
    /// </summary>
    public static FluentScreenshotElement<ElementScope> GetTimeField (
        [NotNull] this IFluentScreenshotElementWithCovariance<BocDateTimeValueControlObject> fluentDateTimeValue)
    {
      ArgumentUtility.CheckNotNull("fluentDateTimeValue", fluentDateTimeValue);

      ElementScope target;
      if (fluentDateTimeValue.IsReadOnly())
        target = fluentDateTimeValue.Target.Scope.FindCss("span:nth-child(2)");
      else
        target = fluentDateTimeValue.Target.Scope.FindChild("TimeValue", Options.NoWait);
      target.EnsureExistence();

      return target.ForElementScopeScreenshot();
    }

    /// <summary>
    /// Returns <see langword="true" /> if the <see cref="BocDateTimeValueControlObject"/> has a field for the selected time, otherwise <see langword="false" />.
    /// </summary>
    /// <param name="fluentDateTimeValue"></param>
    /// <returns></returns>
    public static bool HasTimeField ([NotNull] this IFluentScreenshotElementWithCovariance<BocDateTimeValueControlObject> fluentDateTimeValue)
    {
      ArgumentUtility.CheckNotNull("fluentDateTimeValue", fluentDateTimeValue);

      return fluentDateTimeValue.Target.HasTimeField();
    }

    /// <summary>
    /// Returns <see langword="true" /> if the <see cref="BocDateTimeValueControlObject"/> is read-only, otherwise <see langword="false" />.
    /// </summary>
    public static bool IsReadOnly ([NotNull] this IFluentScreenshotElementWithCovariance<BocDateTimeValueControlObject> fluentDateTimeValue)
    {
      ArgumentUtility.CheckNotNull("fluentDateTimeValue", fluentDateTimeValue);

      return fluentDateTimeValue.Target.IsReadOnly();
    }
  }
}