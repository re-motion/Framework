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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Coypu;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.PageObjects;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Resolvers;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebDriver;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ScrollTests : IntegrationTest
  {
    [Test]
    public void PositioningTest ()
    {
      RunScrollTest(
          "outerA",
          "blockA",
          new WebPadding(1),
          (b, e) => { b.Crop(e, ElementScopeResolver.Instance, new ScreenshotCropping(new WebPadding(1))); });
    }

    [Test]
    public void PositioningTestWithBigBlock ()
    {
      RunScrollTest(
          "outerB",
          "blockB",
          new WebPadding(1),
          (b, e) => { b.Crop(e, ElementScopeResolver.Instance, new ScreenshotCropping(new WebPadding(1))); });
    }

    [Test]
    public void PaddingTest ()
    {
      RunScrollTest(
          "outerC",
          "blockC",
          new WebPadding(30),
          (b, e) => { b.Crop(e, ElementScopeResolver.Instance, new ScreenshotCropping(new WebPadding(1))); });
    }

    private void RunScrollTest (
        string boxId,
        string blockId,
        WebPadding padding,
        ScreenshotTestingDelegate<ElementScope> test,
        [CallerMemberName] string methodName = null)
    {
      var home = Start();

      // TODO RM-8379: In Edge and Chrome, the web test is too fast, resulting in screenshotting an empty page.
      if (Helper.BrowserConfiguration.IsChromium())
        Thread.Sleep(TimeSpan.FromMilliseconds(1000));

      var box = home.Scope.FindId(boxId);
      var block = home.Scope.FindId(blockId);

      var failed = new List<KeyValuePair<string, Exception>>();

      var count = 0;
      foreach (var contentAlignment in Enum.GetValues(typeof(ContentAlignment)).Cast<ContentAlignment>())
      {
        count++;

        box.ScrollToElement(block, contentAlignment, padding);

        var contentAlignmentText = contentAlignment.ToString();
        var testName = string.Join("_", methodName, contentAlignmentText);

        try
        {
          ScreenshotTesting.RunTest<ElementScope, ScrollTests>(Helper, test, ScreenshotTestingType.Both, testName, box, 0, 0d);
        }
        catch (Exception ex)
        {
          failed.Add(new KeyValuePair<string, Exception>(contentAlignmentText, ex));
        }
      }

      if (failed.Count > 0)
      {
        var stringBuilder = new StringBuilder();

        var failRate = failed.Count / (float)count;
        stringBuilder.AppendLine(string.Format("{0}/{1} '{2}' tests failed (fail rate: {3:P}):", failed.Count, count, methodName, failRate));
        stringBuilder.AppendLine();

        foreach (var fail in failed)
        {
          stringBuilder.AppendLine(string.Format(" # Sub test '{0}' failed:", fail.Key));
          stringBuilder.AppendLine(fail.Value.ToString());
          stringBuilder.AppendLine();
        }

        Assert.Fail(stringBuilder.ToString());
      }
    }

    private HtmlPageObject Start ()
    {
      return Start<HtmlPageObject>("ScrollTest.aspx");
    }
  }
}
