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
using System.Drawing;
using Coypu;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Annotations;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public partial class ScreenshotTest : IntegrationTest
  {
    /// <summary>
    /// Tests that the <see cref="ScreenshotTooltipAnnotation"/> is annotated correctly.
    /// </summary>
    [Category("Screenshot")]
    [Test]
    public void DrawCursorTooltip ()
    {
      ScreenshotTestingDelegate<ElementScope> test = (builder, target) =>
      {
        Helper.BrowserConfiguration.BrowserAnnotateHelper.DrawCursorTooltip(builder, Helper.MainBrowserSession, "title", GetTooltipStyleForCurrentBrowser());
        builder.Crop(target.ForElementScopeScreenshot(), new WebPadding(30, 60, 30, 60));
      };

      var home = Start();
      var mouseHelper = Helper.BrowserConfiguration.MouseHelper;
      var element = home.Scope.FindId("tooltipTarget");

      // ElementScope.Hover() cannot be used due to Chromedriver not moving the cursor to trigger the hover event.
      mouseHelper.Hover(element);

      // When drawing the Cursor tooltip, only ScreenshotTestingType.Desktop is supported.
      Helper.RunScreenshotTestExact<ElementScope, ScreenshotTest>(element, ScreenshotTestingType.Desktop, test);

      // Reset the cursor to the initial position (0, 0).
      mouseHelper.Hover(Point.Empty);
    }

    /// <summary>
    /// Tests that the <see cref="ScreenshotTooltipAnnotation"/> is annotated correctly.
    /// </summary>
    [Category("Screenshot")]
    [Test]
    public void DrawCursorTooltip_WithFollowingBoxAnnotation ()
    {
      ScreenshotTestingDelegate<ElementScope> test = (builder, target) =>
      {
        var annotationTarget = Helper.BrowserConfiguration.BrowserAnnotateHelper.DrawCursorTooltip(
            builder,
            Helper.MainBrowserSession,
            "title",
            GetTooltipStyleForCurrentBrowser());
        builder.AnnotateBox(annotationTarget, Pens.Red);
        builder.Crop(target.ForElementScopeScreenshot(), new WebPadding(30, 60, 30, 60));
      };

      var home = Start();
      var mouseHelper = Helper.BrowserConfiguration.MouseHelper;
      var element = home.Scope.FindId("tooltipTarget");

      // ElementScope.Hover() cannot be used due to Chromedriver not moving the mouse to trigger the hover event.
      mouseHelper.Hover(element);

      Helper.RunScreenshotTestExact<ElementScope, ScreenshotTest>(element, ScreenshotTestingType.Both, test);

      // Reset the cursor to the initial position (0, 0).
      mouseHelper.Hover(Point.Empty);
    }
  }
}
