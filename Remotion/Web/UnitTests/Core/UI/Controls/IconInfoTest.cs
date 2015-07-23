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
using System.Web.UI.WebControls;
using NUnit.Framework;
using Remotion.Web.UI.Controls;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.UI.Controls
{
  [TestFixture]
  public class IconInfoTest
  {
    private HtmlHelper _html;
    private IControl _controlStub;

    [SetUp]
    public void SetUp ()
    {
      _html = new HtmlHelper();
      _controlStub = MockRepository.GenerateStub<IControl>();
      _controlStub.Stub (stub => stub.ResolveClientUrl(null)).IgnoreArguments ().Do ((Func<string, string>) (url => url.TrimStart ('~')));
    }

    [Test]
    public void Render_HeightInPixels ()
    {
      IconInfo iconInfo = new IconInfo ("/image.gif") { Height = Unit.Pixel (20) };

      iconInfo.Render (_html.Writer, _controlStub);

      var document = _html.GetResultDocument();
      var imgTag = _html.GetAssertedChildElement (document, "img", 0);

      _html.AssertStyleAttribute (imgTag, "height", "20px");
    }

    [Test]
    public void Render_WidthInPixels ()
    {
      IconInfo iconInfo = new IconInfo ("/image.gif") { Width = Unit.Pixel (10) };

      iconInfo.Render (_html.Writer, _controlStub);

      var document = _html.GetResultDocument ();
      var imgTag = _html.GetAssertedChildElement (document, "img", 0);

      _html.AssertStyleAttribute (imgTag, "width", "10px");
    }

    [Test]
    public void Render_HeightInPercent ()
    {
      IconInfo iconInfo = new IconInfo ("/image.gif") {  Height = Unit.Percentage (20) };

      iconInfo.Render (_html.Writer, _controlStub);

      var document = _html.GetResultDocument ();
      var imgTag = _html.GetAssertedChildElement (document, "img", 0);

      _html.AssertStyleAttribute (imgTag, "height", "20%");
    }


    [Test]
    public void Render_WidthInPercent ()
    {
      IconInfo iconInfo = new IconInfo ("/image.gif") { Width = Unit.Percentage (10) };

      iconInfo.Render (_html.Writer, _controlStub);

      var document = _html.GetResultDocument();
      var imgTag = _html.GetAssertedChildElement (document, "img", 0);

      _html.AssertStyleAttribute (imgTag, "width", "10%");
    }

    [Test]
    public void Render_HeightInPoint ()
    {
      IconInfo iconInfo = new IconInfo ("/image.gif") { Height = Unit.Point (20) };

      iconInfo.Render (_html.Writer, _controlStub);

      var document = _html.GetResultDocument();
      var imgTag = _html.GetAssertedChildElement (document, "img", 0);

      _html.AssertStyleAttribute (imgTag, "height", "20pt");
    }

    [Test]
    public void Render_WidthInEm ()
    {
      IconInfo iconInfo = new IconInfo ("/image.gif") { Width = Unit.Parse ("20em") };

      iconInfo.Render (_html.Writer, _controlStub);

      var document = _html.GetResultDocument ();
      var imgTag = _html.GetAssertedChildElement (document, "img", 0);

      _html.AssertStyleAttribute (imgTag, "width", "20em");
    }
  }
}