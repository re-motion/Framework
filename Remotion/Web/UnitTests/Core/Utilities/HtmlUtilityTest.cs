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
#pragma warning disable REMOTION0001
using System;
using NUnit.Framework;
using Remotion.Web.Utilities;

namespace Remotion.Web.UnitTests.Core.Utilities
{
  [TestFixture]
  public class HtmlUtilityTest
  {
    [Test]
    public void HtmlEncode_DoubleQuote ()
    {
      Assert.That(HtmlUtility.HtmlEncode("a\"b"), Is.EqualTo("a&quot;b"));
    }

    [Test]
    public void HtmlEncode_SingleQuote ()
    {
      Assert.That(HtmlUtility.HtmlEncode("a'b"), Is.EqualTo("a&#39;b"));
    }

    [Test]
    public void HtmlEncode_NewLine ()
    {
      Assert.That(HtmlUtility.HtmlEncode("a\nb"), Is.EqualTo("a<br />b"));
    }

    [Test]
    public void HtmlEncode_LineFeed ()
    {
      Assert.That(HtmlUtility.HtmlEncode("a\rb"), Is.EqualTo("a<br />b"));
    }

    [Test]
    public void HtmlEncode_LineFeedNewLine ()
    {
      Assert.That(HtmlUtility.HtmlEncode("a\r\nb"), Is.EqualTo("a<br />b"));
    }

    [Test]
    public void ExtractPlainText ()
    {
      Assert.That(
          HtmlUtility.ExtractPlainText(WebString.CreateFromText("SimpleString")),
          Is.EqualTo(PlainTextString.CreateFromText("SimpleString")));
    }

    [Test]
    public void ExtractPlainText_Empty ()
    {
      Assert.That(
          HtmlUtility.ExtractPlainText(WebString.CreateFromText("")),
          Is.EqualTo(PlainTextString.CreateFromText("")));
    }

    [Test]
    public void ExtractPlainText_OpenedAndClosedTagRemoval ()
    {
      Assert.That(
          HtmlUtility.ExtractPlainText(WebString.CreateFromHtml("<span>SimpleS<i>tr<b>i</b>n</i></span>g")),
          Is.EqualTo(PlainTextString.CreateFromText("SimpleString")));
    }

    [Test]
    public void ExtractPlainText_SelfClosingTagRemoval ()
    {
      Assert.That(
          HtmlUtility.ExtractPlainText(WebString.CreateFromHtml("Simple<br/>Stri<img src=\"WithAttributes.html\"/>ng")),
          Is.EqualTo(PlainTextString.CreateFromText("SimpleString")));
    }

    [Test]
    public void ExtractPlainText_WithEncodedWebString_StripsHtmlTags ()
    {
      Assert.That(
          HtmlUtility.ExtractPlainText(WebString.CreateFromHtml("<span>SimpleS<i>tr<b>i</b>n</i></span>g")),
          Is.EqualTo(PlainTextString.CreateFromText("SimpleString")));
    }

    [Test]
    public void ExtractPlainText_WithPlainTextWebString_DoesNotStripsHtmlTags ()
    {
      Assert.That(
          HtmlUtility.ExtractPlainText(WebString.CreateFromText("<span>SimpleS<i>tr<b>i</b>n</i></span>g")),
          Is.EqualTo(PlainTextString.CreateFromText("<span>SimpleS<i>tr<b>i</b>n</i></span>g")));
    }

    [Test]
    public void ExtractPlainText_WithUmlautInPlainTextWebString ()
    {
      Assert.That(
          HtmlUtility.ExtractPlainText(WebString.CreateFromText("Text-Umlaut ö")),
          Is.EqualTo(PlainTextString.CreateFromText("Text-Umlaut ö")));
    }

    [Test]
    public void ExtractPlainText_WithUmlautInEncodedWebString ()
    {
      Assert.That(
          HtmlUtility.ExtractPlainText(WebString.CreateFromHtml("Html-Umlaut ö")),
          Is.EqualTo(PlainTextString.CreateFromText("Html-Umlaut ö")));
    }

    [Test]
    public void ExtractPlainText_WithEncodedUmlautInEncodedWebString ()
    {
      Assert.That(
          HtmlUtility.ExtractPlainText(WebString.CreateFromHtml("Html-Encoded-Umlaut &#246;")),
          Is.EqualTo(PlainTextString.CreateFromText("Html-Encoded-Umlaut ö")));
    }
  }
}
