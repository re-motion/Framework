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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using Coypu;
using Coypu.Drivers;
using NUnit.Framework;
using OpenQA.Selenium;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.PageObjects;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Resolvers;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class MouseTest : IntegrationTest
  {
    private const string c_clickDivID = "clickDiv";
    private const string c_focusDivID = "focusDiv";
    private const string c_hoverDivID = "hoverDiv";

    private const string c_getBackgroundColorJs =
        @"return window.getComputedStyle (arguments[0])['background-color'].match (/rgb\(\s*([0-9]+),\s*([0-9]+),\s*([0-9]+)\)/).slice (1,4);";

    [Test]
    public void CorrectDivColors ()
    {
      var home = Start();

      var clickElement = home.Scope.FindId(c_clickDivID);
      Assert.That(GetColor(clickElement).ToArgb(), Is.EqualTo(Color.Black.ToArgb()));

      var focusElement = home.Scope.FindId(c_focusDivID);
      Assert.That(GetColor(focusElement).ToArgb(), Is.EqualTo(Color.Black.ToArgb()));

      var hoverElement = home.Scope.FindId(c_hoverDivID);
      Assert.That(GetColor(hoverElement).ToArgb(), Is.EqualTo(Color.Black.ToArgb()));
    }

    [Test]
    public void MouseLeftClick ()
    {
      var home = Start();

      var clickElement = home.Scope.FindId(c_clickDivID);
      Helper.BrowserConfiguration.MouseHelper.Hover(clickElement);
      Helper.BrowserConfiguration.MouseHelper.LeftClick();

      Assert.That(GetColor(clickElement).ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
    }

    [Test]
    public void MouseDoubleLeftClick ()
    {
      var home = Start();

      var clickElement = home.Scope.FindId(c_clickDivID);
      Helper.BrowserConfiguration.MouseHelper.Hover(clickElement);
      Helper.BrowserConfiguration.MouseHelper.DoubleLeftClick();

      Assert.That(GetColor(clickElement).ToArgb(), Is.EqualTo(Color.Green.ToArgb()));
    }

    [Test]
    public void MouseRightClick ()
    {
      var home = Start();

      var clickElement = home.Scope.FindId(c_clickDivID);
      Helper.BrowserConfiguration.MouseHelper.Hover(clickElement);
      Helper.BrowserConfiguration.MouseHelper.RightClick();

      Assert.That(GetColor(clickElement).ToArgb(), Is.EqualTo(Color.Blue.ToArgb()));
    }

    [Test]
    public void Focus ()
    {
      var home = Start();

      var focusElement = home.Scope.FindId(c_focusDivID);

      Helper.BrowserConfiguration.MouseHelper.Hover(focusElement);
      Helper.BrowserConfiguration.MouseHelper.LeftClick();

      Assert.That(GetColor(focusElement).ToArgb(), Is.EqualTo(Color.Orange.ToArgb()));
    }

    [Test]
    public void Hover ()
    {
      var home = Start();

      var hoverElement = home.Scope.FindId(c_hoverDivID);
      Helper.BrowserConfiguration.MouseHelper.Hover(hoverElement);
      Thread.Sleep(250);

      Assert.That(GetColor(hoverElement).ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
    }

    [Explicit("Testing the tooltip is flaky as long as there is no good implementation of .ShowTooltip")]
    [Test]
    public void Tooltip ()
    {
      var home = Start();

      ScreenshotTestingDelegate<ElementScope> test =
          (b, e) => { b.Crop(e, ElementScopeResolver.Instance, new ScreenshotCropping(new WebPadding(6, 47, 6, 18))); };

      var tooltipElement = home.Scope.FindId("tooltipDiv");

      // Workaround in order to make the test work.
      // If the test is run behind all the other tests the tooltip will not show.
      // Mouse movement beforehand with a little sleep does the trick
      Helper.BrowserConfiguration.MouseHelper.Hover(tooltipElement);

#pragma warning disable 0618
      Helper.BrowserConfiguration.MouseHelper.ShowTooltip(tooltipElement);
#pragma warning restore 0618

      Helper.RunScreenshotTestExact<ElementScope, MouseTest>(tooltipElement, ScreenshotTestingType.Desktop, test);
    }

    private Color GetColor (ElementScope scope)
    {
      var driver = ((IWrapsDriver)scope.Native).WrappedDriver;
      var jsExecutor = (IJavaScriptExecutor)driver;

      var rawData = (IReadOnlyCollection<object>)jsExecutor.ExecuteScript(c_getBackgroundColorJs, (IWebElement)scope.Native);
      if (rawData.Count != 3)
        throw new InvalidOperationException("Javascript is invalid.");
      var rgb = rawData.Select(i => int.Parse((string)i)).ToArray();
      return Color.FromArgb(rgb[0], rgb[1], rgb[2]);
    }

    private HtmlPageObject Start ()
    {
      var htmlPageObject = Start<HtmlPageObject>("MouseTest.aspx");

      //RM-9307 We seem to run tests too fast for firefox to load properly, so we need to delay all firefox tests a bit.
      if (htmlPageObject.Scope.Browser == Browser.Firefox)
        Thread.Sleep(200);

      return htmlPageObject;
    }
  }
}
