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
using System.Web.UI;
using Moq;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Web.UnitTests.Core.UI.Controls;
using Remotion.Web.ContentSecurityPolicy;
using Remotion.Web.UI;

namespace Remotion.Web.UnitTests.Core.ContentSecurityPolicy
{
  [TestFixture]
  public class CspEnabledHtmlTextWriterTest
  {
    private HtmlHelper _htmlHelper;
    private CspEnabledHtmlTextWriter _writer;
    private Mock<ISmartPageClientScriptManager> _clientScriptStub;
    private Mock<ISmartPage> _pageStub;
    private Mock<INonceGenerator> _randomNumberGeneratorStub;

    [SetUp]
    public void SetUp ()
    {
       _pageStub = new Mock<ISmartPage>();
      _clientScriptStub = new Mock<ISmartPageClientScriptManager>();
      _randomNumberGeneratorStub = new Mock<INonceGenerator>();

      _pageStub
          .Setup(s => s.ClientScript)
          .Returns(_clientScriptStub.Object);

      _htmlHelper = new HtmlHelper();
      _writer = new CspEnabledHtmlTextWriter(_pageStub.Object, _htmlHelper.Writer, _randomNumberGeneratorStub.Object, "TEST-NONCE");
    }

    [Test]
    public void RenderBeginTag_WithScriptTag_NonceValueIsRendered ()
    {
      _writer.RenderBeginTag(HtmlTextWriterTag.Script);
      _writer.RenderEndTag();

      var document = _htmlHelper.GetResultDocument();
      var element = _htmlHelper.GetAssertedChildElement(document, "script", 0);
      _htmlHelper.AssertAttribute(element, "nonce", "TEST-NONCE");
    }

    [Test]
    public void RenderBeginTag_WithTagsOtherThanScript_NonceValueIsNotRendered ()
    {
      _writer.RenderBeginTag("root");

      _writer.RenderBeginTag(HtmlTextWriterTag.Button);
      _writer.RenderEndTag();

      _writer.RenderBeginTag(HtmlTextWriterTag.Div);
      _writer.RenderEndTag();

      _writer.RenderBeginTag(HtmlTextWriterTag.A);
      _writer.RenderEndTag();

      _writer.RenderEndTag();

      var document = _htmlHelper.GetResultDocument();
      var root = _htmlHelper.GetAssertedChildElement(document, "root", 0);
      _htmlHelper.AssertChildElementCount(root, 3);

      var element1 = _htmlHelper.GetAssertedChildElement(root, "button", 0);
      _htmlHelper.AssertNoAttribute(element1, "nonce");

      var element2 = _htmlHelper.GetAssertedChildElement(root, "div", 1);
      _htmlHelper.AssertNoAttribute(element2, "nonce");

      var element3 = _htmlHelper.GetAssertedChildElement(root, "a", 2);
      _htmlHelper.AssertNoAttribute(element3, "nonce");
    }

    [TestCase("onclick", "click")]
    [TestCase("ONCLICK", "click")]
    [TestCase("onchange", "change")]
    [TestCase("onmouseover", "mouseover")]
    [TestCase("onmouseout", "mouseout")]
    [TestCase("onkeyup", "keyup")]
    [TestCase("onkeydown", "keydown")]
    [TestCase("onkeypress", "keypress")]
    public void AddAttribute_WithSupportedEventType_ScriptIsRegistered (string eventType, string value)
    {
      _randomNumberGeneratorStub
          .Setup(m => m.GenerateAlphaNumericNonce())
          .Returns("eventTargetID")
          .Verifiable();

      _writer.AddAttribute(eventType, "console.info('test');");
      _writer.RenderBeginTag(HtmlTextWriterTag.Button);
      _writer.RenderEndTag();

      var document = _htmlHelper.GetResultDocument();
      var element = _htmlHelper.GetAssertedChildElement(document, "button", 0);
      _htmlHelper.AssertAttribute(element, "data-inline-event-target", "eventTargetID");

      _clientScriptStub.Verify(
              m => m.RegisterStartupScriptBlock(
                      _pageStub.Object,
                      typeof(CspEnabledHtmlTextWriter),
                      "eventTargetID",
                      $"document.querySelector('[data-inline-event-target=\"eventTargetID\"]').addEventListener('{value}', function (event){{console.info(&#39;test&#39;);}});"),
              Times.Once);

      _randomNumberGeneratorStub.Verify(m => m.GenerateAlphaNumericNonce(), Times.Once());
    }

    [Test]
    public void AddAttribute_WithAttributeOtherThanEventType_ScriptIsNotRegistered ()
    {
      _writer.AddAttribute("style", "color:white;");
      _writer.RenderBeginTag(HtmlTextWriterTag.Button);
      _writer.RenderEndTag();

      var document = _htmlHelper.GetResultDocument();
      var element = _htmlHelper.GetAssertedChildElement(document, "button", 0);
      _htmlHelper.AssertNoAttribute(element, "data-inline-event-target");

      _clientScriptStub.Verify(
              m => m.RegisterStartupScriptBlock(
                      _pageStub.Object,
                      typeof(CspEnabledHtmlTextWriter),
                      It.IsAny<string>(),
                      It.IsAny<string>()),
              Times.Never);
    }

    [Test]
    public void AddAttribute_WithNullValue_ScriptIsNotRegistered ()
    {
      _writer.AddAttribute("onclick", null);
      _writer.RenderBeginTag(HtmlTextWriterTag.Button);
      _writer.RenderEndTag();

      var document = _htmlHelper.GetDocumentText();
      Assert.That(document, Is.EqualTo("<button onclick></button>"));

      _clientScriptStub.Verify(
          m => m.RegisterStartupScriptBlock(
              _pageStub.Object,
              typeof(CspEnabledHtmlTextWriter),
              It.IsAny<string>(),
              It.IsAny<string>()),
          Times.Never);
    }

    [Test]
    public void AddAttribute_WithEmptyValue_ScriptIsNotRegistered ()
    {
      _writer.AddAttribute("onclick", "");
      _writer.RenderBeginTag(HtmlTextWriterTag.Button);
      _writer.RenderEndTag();

      var document = _htmlHelper.GetResultDocument();
      var element = _htmlHelper.GetAssertedChildElement(document, "button", 0);
      _htmlHelper.AssertNoAttribute(element, "data-inline-event-target");

      _clientScriptStub.Verify(
              m => m.RegisterStartupScriptBlock(
                      _pageStub.Object,
                      typeof(CspEnabledHtmlTextWriter),
                      It.IsAny<string>(),
                      It.IsAny<string>()),
              Times.Never);
    }

    [Test]
    public void AddAttribute_WithMoreThanOneAttribute_ScriptIsRegistered ()
    {
      _randomNumberGeneratorStub
          .Setup(m => m.GenerateAlphaNumericNonce())
          .Returns("eventTargetID")
          .Verifiable();

      _writer.AddAttribute("onclick", "console.info('test1');");
      _writer.AddAttribute("onchange", "console.info('test2');");
      _writer.RenderBeginTag(HtmlTextWriterTag.Button);
      _writer.RenderEndTag();

      var document = _htmlHelper.GetResultDocument();
      var element = _htmlHelper.GetAssertedChildElement(document, "button", 0);
      _htmlHelper.AssertAttribute(element, "data-inline-event-target", "eventTargetID");

      _clientScriptStub.Verify(
          m => m.RegisterStartupScriptBlock(
              _pageStub.Object,
              typeof(CspEnabledHtmlTextWriter),
              "eventTargetID",
              "document.querySelector('[data-inline-event-target=\"eventTargetID\"]').addEventListener('click', function (event){console.info(&#39;test1&#39;);});"),
          Times.Once);

      _clientScriptStub.Verify(
          m => m.RegisterStartupScriptBlock(
              _pageStub.Object,
              typeof(CspEnabledHtmlTextWriter),
              "eventTargetID",
              "document.querySelector('[data-inline-event-target=\"eventTargetID\"]').addEventListener('change', function (event){console.info(&#39;test2&#39;);});"),
          Times.Once);

      _randomNumberGeneratorStub.Verify(m => m.GenerateAlphaNumericNonce(), Times.Once());
    }

    [Test]
    public void AddAttribute_WithMoreThanOneTag_ScriptIsRegistered ()
    {
      _randomNumberGeneratorStub
          .SetupSequence(m => m.GenerateAlphaNumericNonce())
          .Returns("eventTargetID1")
          .Returns("eventTargetID2");

      _writer.RenderBeginTag("root");

      _writer.AddAttribute("onclick", "console.info('test1');");
      _writer.RenderBeginTag(HtmlTextWriterTag.Button);
      _writer.RenderEndTag();

      _writer.AddAttribute("onchange", "console.info('test2');");
      _writer.RenderBeginTag(HtmlTextWriterTag.Label);
      _writer.RenderEndTag();

      _writer.RenderEndTag();

      var document = _htmlHelper.GetResultDocument();
      var root = _htmlHelper.GetAssertedChildElement(document, "root", 0);
      _htmlHelper.AssertChildElementCount(root, 2);

      var element1 = _htmlHelper.GetAssertedChildElement(root, "button", 0);
      _htmlHelper.AssertAttribute(element1, "data-inline-event-target", "eventTargetID1");

      var element2 = _htmlHelper.GetAssertedChildElement(root, "label", 1);
      _htmlHelper.AssertAttribute(element2, "data-inline-event-target", "eventTargetID2");

      _clientScriptStub.Verify(
          m => m.RegisterStartupScriptBlock(
              _pageStub.Object,
              typeof(CspEnabledHtmlTextWriter),
              "eventTargetID1",
              It.IsAny<string>()),
          Times.Once);
      _clientScriptStub.Verify(
          m => m.RegisterStartupScriptBlock(
              _pageStub.Object,
              typeof(CspEnabledHtmlTextWriter),
              "eventTargetID1",
              "document.querySelector('[data-inline-event-target=\"eventTargetID1\"]').addEventListener('click', function (event){console.info(&#39;test1&#39;);});"),
          Times.Once);

      _clientScriptStub.Verify(
          m => m.RegisterStartupScriptBlock(
              _pageStub.Object,
              typeof(CspEnabledHtmlTextWriter),
              "eventTargetID2",
              It.IsAny<string>()),
          Times.Once);
      _clientScriptStub.Verify(
          m => m.RegisterStartupScriptBlock(
              _pageStub.Object,
              typeof(CspEnabledHtmlTextWriter),
              "eventTargetID2",
              "document.querySelector('[data-inline-event-target=\"eventTargetID2\"]').addEventListener('change', function (event){console.info(&#39;test2&#39;);});"),
          Times.Once);
    }

    [Test]
    public void AddAttribute_WithNotSupportedEventType_ThrowsArgumentException ()
    {
      Assert.That(
          () => _writer.AddAttribute("onload", "console.info('test');"),
          Throws.InstanceOf<ArgumentException>()
              .With.ArgumentExceptionMessageEqualTo(
                  "The name of attribute 'onload' indicates a script event but the event type is not supported.",
                  "name"));
    }

    [Test]
    public void AddAttribute_WithAlreadyRegisteredEventType_ThrowsInvalidOperationException ()
    {
      Assert.That(
          () =>
          {
            _writer.AddAttribute("onchange", "console.info('test1');");
            _writer.AddAttribute("onchange", "console.info('test2');");
          },
          Throws.InstanceOf<ArgumentException>()
              .With.Message.EqualTo("Event handler 'onchange' cannot be registered more than once."));
    }

    [Test]
    public void AddAttribute_WithEnumOverload_WithSupportedEventType_ScriptIsRegistered ()
    {
      _randomNumberGeneratorStub
          .Setup(m => m.GenerateAlphaNumericNonce())
          .Returns("eventTargetID")
          .Verifiable();

      _writer.AddAttribute(HtmlTextWriterAttribute.Onclick, "console.info('test');");
      _writer.RenderBeginTag(HtmlTextWriterTag.Button);
      _writer.RenderEndTag();

      var document = _htmlHelper.GetResultDocument();
      var element = _htmlHelper.GetAssertedChildElement(document, "button", 0);
      _htmlHelper.AssertAttribute(element, "data-inline-event-target", "eventTargetID");

      _clientScriptStub.Verify(
          m => m.RegisterStartupScriptBlock(
              _pageStub.Object,
              typeof(CspEnabledHtmlTextWriter),
              "eventTargetID",
              $"document.querySelector('[data-inline-event-target=\"eventTargetID\"]').addEventListener('click', function (event){{console.info('test');}});"),
          Times.Once);

      _randomNumberGeneratorStub.Verify(m => m.GenerateAlphaNumericNonce(), Times.Once());
    }

    [Test]
    public void AddAttribute_WithEnumOverloadAndEncodeFlag_WithSupportedEventType_ScriptIsRegistered ()
    {
      _randomNumberGeneratorStub
          .Setup(m => m.GenerateAlphaNumericNonce())
          .Returns("eventTargetID")
          .Verifiable();

      _writer.AddAttribute(HtmlTextWriterAttribute.Onclick, "console.info('test');", false);
      _writer.RenderBeginTag(HtmlTextWriterTag.Button);
      _writer.RenderEndTag();

      var document = _htmlHelper.GetResultDocument();
      var element = _htmlHelper.GetAssertedChildElement(document, "button", 0);
      _htmlHelper.AssertAttribute(element, "data-inline-event-target", "eventTargetID");

      _clientScriptStub.Verify(
          m => m.RegisterStartupScriptBlock(
              _pageStub.Object,
              typeof(CspEnabledHtmlTextWriter),
              "eventTargetID",
              $"document.querySelector('[data-inline-event-target=\"eventTargetID\"]').addEventListener('click', function (event){{console.info('test');}});"),
          Times.Once);

      _randomNumberGeneratorStub.Verify(m => m.GenerateAlphaNumericNonce(), Times.Once());
    }

    [Test]
    public void AddAttribute_WithEnumEncodeFlag_WithSupportedEventType_ScriptIsRegistered ()
    {
      _randomNumberGeneratorStub
          .Setup(m => m.GenerateAlphaNumericNonce())
          .Returns("eventTargetID")
          .Verifiable();

      _writer.AddAttribute("onclick", "console.info('test');", false);
      _writer.RenderBeginTag(HtmlTextWriterTag.Button);
      _writer.RenderEndTag();

      var document = _htmlHelper.GetResultDocument();
      var element = _htmlHelper.GetAssertedChildElement(document, "button", 0);
      _htmlHelper.AssertAttribute(element, "data-inline-event-target", "eventTargetID");

      _clientScriptStub.Verify(
          m => m.RegisterStartupScriptBlock(
              _pageStub.Object,
              typeof(CspEnabledHtmlTextWriter),
              "eventTargetID",
              $"document.querySelector('[data-inline-event-target=\"eventTargetID\"]').addEventListener('click', function (event){{console.info('test');}});"),
          Times.Once);

      _randomNumberGeneratorStub.Verify(m => m.GenerateAlphaNumericNonce(), Times.Once());
    }

    [TestCase("javascript: console.info('test');", "console.info(&#39;test&#39;);")]
    [TestCase("javascript:console.info('test');", "console.info(&#39;test&#39;);")]
    [TestCase("  javascript:  console.info('test');", "console.info(&#39;test&#39;);")]
    [TestCase("JavaScript: console.info('test');", "console.info(&#39;test&#39;);")]
    public void AddAttribute_WithJavascriptPrefix_RemovesPrefix (string actual, string expected)
    {
      _randomNumberGeneratorStub
          .Setup(m => m.GenerateAlphaNumericNonce())
          .Returns("eventTargetID")
          .Verifiable();

      _writer.AddAttribute("onclick", actual);
      _writer.RenderBeginTag(HtmlTextWriterTag.Button);
      _writer.RenderEndTag();

      var document = _htmlHelper.GetResultDocument();
      var element = _htmlHelper.GetAssertedChildElement(document, "button", 0);
      _htmlHelper.AssertAttribute(element, "data-inline-event-target", "eventTargetID");

      _clientScriptStub.Verify(
          m => m.RegisterStartupScriptBlock(
              _pageStub.Object,
              typeof(CspEnabledHtmlTextWriter),
              "eventTargetID",
              $"document.querySelector('[data-inline-event-target=\"eventTargetID\"]').addEventListener('click', function (event){{{expected}}});"),
          Times.Once);

      _randomNumberGeneratorStub.Verify(m => m.GenerateAlphaNumericNonce(), Times.Once());
    }
  }
}
