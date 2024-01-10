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

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public partial class BrowserWindowTest
  {
    [Test]
    public void Test_ContentPosition ()
    {
      var home = Start();
      var window = home.Context.Window;

      var offset = BrowserHelper.GetBrowserContentOffset(window);

      var position = new Point(50, 50);
      BrowserHelper.MoveBrowserWindowTo(window, position);

      Assert.That(
          BrowserHelper.GetBrowserContentBounds(window).Location,
          Is.EqualTo(position + offset),
          "Content position does not match the expected position.");
    }

    [Test]
    public void Test_ContentSize ()
    {
      var home = Start();
      var window = home.Context.Window;

      var size = new Size(600, 600);
      BrowserHelper.ResizeBrowserContentTo(window, size);

      Assert.That(
          BrowserHelper.GetBrowserContentBounds(window).Size,
          Is.EqualTo(size),
          "Browser content size does not match the expected size.");
    }
  }
}
