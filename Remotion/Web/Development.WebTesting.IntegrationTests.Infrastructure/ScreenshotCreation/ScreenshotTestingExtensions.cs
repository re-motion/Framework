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
using System.Runtime.CompilerServices;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.ScreenshotCreation
{
  /// <summary>
  /// Extension methods which make using <see cref="ScreenshotTesting"/> easier.
  /// </summary>
  public static class ScreenshotTestingExtensions
  {
    /// <summary>
    /// Provides a simpler interface for <see cref="ScreenshotTesting.RunTest{TValue,TTarget}"/>.
    /// </summary>
    public static void RunScreenshotTest<TTarget> (
        this WebTestHelper helper,
        ControlObject value,
        ScreenshotTestingType type,
        ScreenshotTestingDelegate<ControlObject> test,
        int? maxAllowedPixelVariance = null,
        double? unequalPixelThreshold = null,
        [CallerMemberName] string memberName = null)
    {
      ScreenshotTesting.RunTest<ControlObject, TTarget> (helper, test, type, memberName, value, maxAllowedPixelVariance, unequalPixelThreshold);
    }

    /// <summary>
    /// Provides a simpler interface for <see cref="ScreenshotTesting.RunTest{TValue,TTarget}"/>.
    /// </summary>
    public static void RunScreenshotTest<TControl, TTarget> (
        this WebTestHelper helper,
        TControl value,
        ScreenshotTestingType type,
        ScreenshotTestingDelegate<TControl> test,
        int? maxAllowedPixelVariance = null,
        double? unequalPixelThreshold = null,
        [CallerMemberName] string memberName = null)
    {
      ScreenshotTesting.RunTest<TControl, TTarget> (helper, test, type, memberName, value, maxAllowedPixelVariance, unequalPixelThreshold);
    }

    /// <summary>
    /// Provides a simpler interface for <see cref="ScreenshotTesting.RunTest{TValue,TTarget}"/>. Uses exact image comparison.
    /// </summary>
    public static void RunScreenshotTestExact<TTarget> (
        this WebTestHelper helper,
        ControlObject value,
        ScreenshotTestingType type,
        ScreenshotTestingDelegate<ControlObject> test,
        [CallerMemberName] string memberName = null)
    {
      ScreenshotTesting.RunTest<ControlObject, TTarget> (helper, test, type, memberName, value, 0, 0d);
    }

    /// <summary>
    /// Provides a simpler interface for <see cref="ScreenshotTesting.RunTest{TValue,TTarget}"/>. Uses exact image comparison.
    /// </summary>
    public static void RunScreenshotTestExact<TControl, TTarget> (
        this WebTestHelper helper,
        TControl value,
        ScreenshotTestingType type,
        ScreenshotTestingDelegate<TControl> test,
        [CallerMemberName] string memberName = null)
    {
      ScreenshotTesting.RunTest<TControl, TTarget> (helper, test, type, memberName, value, 0, 0d);
    }
  }
}