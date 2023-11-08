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
using System.Web.UI;
using System.Xml;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.Core.UI.Controls.WebButtonTests
{
  [TestFixture]
  public class EncodingTest : BaseTest
  {
    private WebButton _webButton;
    private HtmlHelper _html;

    protected override void SetUpPage ()
    {
      base.SetUpPage();
      _webButton = new WebButton();
      _webButton.ID = "WebButton";
      _html = new HtmlHelper();
    }

    public override void TearDown ()
    {
      base.TearDown();
      _html.Dispose();
    }

    [Test]
    public void Text_WithPlainText ()
    {
      _webButton.Text = WebString.CreateFromText("test");

      var result = RenderControl(_webButton);

      var button = _html.GetAssertedChildElement(result, "button", 0);
      _html.AssertAttribute(button, "value", "test");
      var buttonBody = _html.GetAssertedChildElement(button, "span", 0);
      var textSpan = _html.GetAssertedChildElement(buttonBody, "span", 0);
      _html.AssertTextNode(textSpan, "test", 0);
    }

    [Test]
    public void Text_WithHtml ()
    {
      _webButton.Text = WebString.CreateFromHtml("<b>test</b>");

      var result = RenderControl(_webButton);

      var button = _html.GetAssertedChildElement(result, "button", 0);
      _html.AssertAttribute(button, "value", "<b>test</b>");
      var buttonBody = _html.GetAssertedChildElement(button, "span", 0);
      var textSpan = _html.GetAssertedChildElement(buttonBody, "span", 0);
      var textB = _html.GetAssertedChildElement(textSpan, "b", 0);
      _html.AssertTextNode(textB, "test", 0);
    }

    [Test]
    public void Text_WithHtmlInPlaintext_Encoded ()
    {
      _webButton.Text = WebString.CreateFromText("<b>test</b>");

      var result = RenderControl(_webButton);

      var button = _html.GetAssertedChildElement(result, "button", 0);
      _html.AssertAttribute(button, "value", "<b>test</b>");
      var buttonBody = _html.GetAssertedChildElement(button, "span", 0);
      var textSpan = _html.GetAssertedChildElement(buttonBody, "span", 0);
      _html.AssertTextNode(textSpan, "<b>test</b>", 0);
    }

    private XmlDocument RenderControl (Control control)
    {
      var page = new Page();
      page.Controls.Add(control);

      var ci = new ControlInvoker(page);
      ci.InitRecursive();
      ci.LoadRecursive();
      ci.PreRenderRecursive();

      control.RenderControl(_html.Writer);

      return _html.GetResultDocument();
    }
  }
}
