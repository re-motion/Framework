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
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.PageObjects;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public partial class BrowserWindowTest : IntegrationTest
  {
    protected override WindowSize WindowSize
    {
      get { return new WindowSize(600, 600); }
    }

    public BrowserHelper BrowserHelper
    {
      get { return Helper.BrowserConfiguration.BrowserHelper; }
    }

    [Test]
    public void Test_WindowSize ()
    {
      var home = Start();
      var window = home.Context.Window;

      var size = new Size(600, 600);
      BrowserHelper.ResizeBrowserWindowTo(window, size);

      // RM-7465 On some setups Edge does not resize to the exact size given.
      var edgeSize = size + new Size(2, 0);
      Assert.That(BrowserHelper.GetWindowBounds(window).Size, Is.AnyOf(size, edgeSize), "Window size does not match the expected size.");
    }

    [Test]
    public void Test_WindowPosition ()
    {
      var home = Start();
      var window = home.Context.Window;

      var position = new Point(50, 50);
      BrowserHelper.MoveBrowserWindowTo(window, position);

      Assert.That(BrowserHelper.GetWindowBounds(window).Location, Is.EqualTo(position), "Window position does not match the expected position.");
    }

    private HtmlPageObject Start ()
    {
      return Start<HtmlPageObject>("Empty.aspx");
    }
  }
}
