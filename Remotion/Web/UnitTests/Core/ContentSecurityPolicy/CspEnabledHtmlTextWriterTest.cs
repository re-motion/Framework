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
using System.IO;
using System.Web.UI;
using Moq;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Web.UnitTests.Core.UI.Controls;
using Remotion.Web.ContentSecurityPolicy;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

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
    private Mock<IRenderingFeatures> _renderingFeaturesStub;
    private Mock<IFallbackNavigationUrlProvider> _fallbackNavigationUrlProviderStub;

    [SetUp]
    public void SetUp ()
    {
       _pageStub = new Mock<ISmartPage>(MockBehavior.Strict);
      _clientScriptStub = new Mock<ISmartPageClientScriptManager>();
      _randomNumberGeneratorStub = new Mock<INonceGenerator>(MockBehavior.Strict);
      _renderingFeaturesStub = new Mock<IRenderingFeatures>(MockBehavior.Strict);
      _renderingFeaturesStub.Setup(_ => _.EnableDiagnosticMetadata).Returns(false);

      _fallbackNavigationUrlProviderStub = new Mock<IFallbackNavigationUrlProvider>(MockBehavior.Strict);

      _pageStub
          .Setup(s => s.ClientScript)
          .Returns(_clientScriptStub.Object);

      _htmlHelper = new HtmlHelper();
      _writer = new CspEnabledHtmlTextWriter(
              _pageStub.Object,
              _htmlHelper.Writer,
              _randomNumberGeneratorStub.Object,
              "TEST-NONCE",
              _renderingFeaturesStub.Object,
              _fallbackNavigationUrlProviderStub.Object);
    }

    [Test]
    public void CloneWithTextWriter ()
    {
      var stringWriter = new StringWriter();
      var clonedWriter = _writer.CloneWithTextWriter(stringWriter);

      _writer.Write("original");
      clonedWriter.Write("cloned");

      Assert.That(clonedWriter, Is.Not.SameAs(_writer));
      Assert.That(stringWriter.ToString(), Is.EqualTo("cloned"));
    }

    [Test]
    public void RegisterSupportedEvent_WithEventNameWithoutOnPrefix_Throws ()
    {
      Assert.That(
          () => CspEnabledHtmlTextWriter.RegisterSupportedEvent("bla"),
    Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("The specified event name must start with 'on'.", "eventName"));
    }

    [Test]
    public void RegisterSupportedEvent_NewEventRegistration ()
    {
      _randomNumberGeneratorStub.Setup(_ => _.GenerateAlphaNumericNonce()).Returns("eventTargetID");

      _writer.RenderBeginTag("root");

      _writer.AddAttribute("onNewAttribute", "value");
      _writer.RenderBeginTag("div");
      _writer.RenderEndTag();

      CspEnabledHtmlTextWriter.RegisterSupportedEvent("onNewAttribute");

      _writer.AddAttribute("onNewAttribute", "value");
      _writer.RenderBeginTag("div");
      _writer.RenderEndTag();

      _writer.RenderEndTag();

      Assert.That(
          _htmlHelper.GetDocumentText().Replace("\t", "  "),
          Is.EqualTo(
              """
              <root>
                <div onNewAttribute="value">

                </div><div data-inline-event-target="eventTargetID">

                </div>
              </root>
              """.ReplaceLineEndings()));
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

    private static readonly IReadOnlyCollection<Action<CspEnabledHtmlTextWriter>> s_addAttributeEveryOverloadWithSupportedEventTypeScriptIsRegisteredTestCases =
    [
        writer => { writer.AddAttribute("onclick", "console.info('test');"); },
        writer => { writer.AddAttribute("onclick", "console.info('test');", true); },
        writer => { writer.AddAttribute("onclick", "console.info(&#39;test&#39;);", false); },
        writer => { writer.AddAttribute(HtmlTextWriterAttribute.Onclick, "console.info('test');"); },
        writer => { writer.AddAttribute(HtmlTextWriterAttribute.Onclick, "console.info('test');", true); },
        writer => { writer.AddAttribute(HtmlTextWriterAttribute.Onclick, "console.info(&#39;test&#39;);", false); }
    ];

    [Test]
    [TestCaseSource(nameof(s_addAttributeEveryOverloadWithSupportedEventTypeScriptIsRegisteredTestCases))]
    public void AddAttribute_EveryOverload_WithSupportedEventType_ScriptIsRegistered (Action<CspEnabledHtmlTextWriter> addAttribute)
    {
      _randomNumberGeneratorStub
          .Setup(m => m.GenerateAlphaNumericNonce())
          .Returns("eventTargetID")
          .Verifiable();

      addAttribute(_writer);
      _writer.RenderBeginTag(HtmlTextWriterTag.Button);
      _writer.RenderEndTag();

      var document = _htmlHelper.GetResultDocument();
      var element = _htmlHelper.GetAssertedChildElement(document, "button", 0);
      _htmlHelper.AssertAttribute(element, "data-inline-event-target", "eventTargetID");

      _clientScriptStub.Verify(
          m => m.RegisterStartupScriptBlock(
              _pageStub.Object,
              typeof(CspEnabledHtmlTextWriter),
              $"eventTargetID-onclick",
              $";(function() {{ const target = document.querySelector('[data-inline-event-target=\"eventTargetID\"]'); if (target) {{ target.onclick = function (event){{console.info('test');}}; }} }})();"),
          Times.Once);
      _clientScriptStub.VerifyNoOtherCalls();

      _randomNumberGeneratorStub.Verify(m => m.GenerateAlphaNumericNonce(), Times.Once());
    }

    private static readonly IReadOnlyCollection<Action<CspEnabledHtmlTextWriter>> s_addAttributeEveryOverloadWithAttributeOtherThanEventTypeScriptIsRegisteredTestCases =
    [
        writer => { writer.AddAttribute("style", "color:white;"); },
        writer => { writer.AddAttribute("style", "color:white;", true); },
        writer => { writer.AddAttribute("style", "color:white;", false); },
        writer => { writer.AddAttribute(HtmlTextWriterAttribute.Style, "color:white;"); },
        writer => { writer.AddAttribute(HtmlTextWriterAttribute.Style, "color:white;", true); },
        writer => { writer.AddAttribute(HtmlTextWriterAttribute.Style, "color:white;", false); }
    ];

    [Test]
    [TestCaseSource(nameof(s_addAttributeEveryOverloadWithAttributeOtherThanEventTypeScriptIsRegisteredTestCases))]
    public void AddAttribute_EveryOverload_WithAttributeOtherThanEventType_ScriptIsNotRegistered (Action<CspEnabledHtmlTextWriter> addAttribute)
    {
      addAttribute(_writer);
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
      _clientScriptStub.VerifyNoOtherCalls();
    }

    [TestCase("onclick", "onclick")]
    [TestCase("ONCLICK", "onclick")]
    [TestCase("onchange", "onchange")]
    [TestCase("onmouseover", "onmouseover")]
    [TestCase("onmouseout", "onmouseout")]
    [TestCase("onkeyup", "onkeyup")]
    [TestCase("onkeydown", "onkeydown")]
    [TestCase("onkeypress", "onkeypress")]
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
                      $"eventTargetID-{value}",
                      $";(function() {{ const target = document.querySelector('[data-inline-event-target=\"eventTargetID\"]'); if (target) {{ target.{value} = function (event){{console.info('test');}}; }} }})();"),
              Times.Once);
      _clientScriptStub.VerifyNoOtherCalls();

      _randomNumberGeneratorStub.Verify(m => m.GenerateAlphaNumericNonce(), Times.Once());
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
      _clientScriptStub.VerifyNoOtherCalls();
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
      _clientScriptStub.VerifyNoOtherCalls();
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
              "eventTargetID-onclick",
              ";(function() { const target = document.querySelector('[data-inline-event-target=\"eventTargetID\"]'); if (target) { target.onclick = function (event){console.info('test1');}; } })();"),
          Times.Once);

      _clientScriptStub.Verify(
          m => m.RegisterStartupScriptBlock(
              _pageStub.Object,
              typeof(CspEnabledHtmlTextWriter),
              "eventTargetID-onchange",
              ";(function() { const target = document.querySelector('[data-inline-event-target=\"eventTargetID\"]'); if (target) { target.onchange = function (event){console.info('test2');}; } })();"),
          Times.Once);

      _clientScriptStub.VerifyNoOtherCalls();

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
              "eventTargetID1-onclick",
              It.IsAny<string>()),
          Times.Once);
      _clientScriptStub.Verify(
          m => m.RegisterStartupScriptBlock(
              _pageStub.Object,
              typeof(CspEnabledHtmlTextWriter),
              "eventTargetID1-onclick",
              ";(function() { const target = document.querySelector('[data-inline-event-target=\"eventTargetID1\"]'); if (target) { target.onclick = function (event){console.info('test1');}; } })();"),
          Times.Once);

      _clientScriptStub.Verify(
          m => m.RegisterStartupScriptBlock(
              _pageStub.Object,
              typeof(CspEnabledHtmlTextWriter),
              "eventTargetID2-onchange",
              It.IsAny<string>()),
          Times.Once);
      _clientScriptStub.Verify(
          m => m.RegisterStartupScriptBlock(
              _pageStub.Object,
              typeof(CspEnabledHtmlTextWriter),
              "eventTargetID2-onchange",
              ";(function() { const target = document.querySelector('[data-inline-event-target=\"eventTargetID2\"]'); if (target) { target.onchange = function (event){console.info('test2');}; } })();"),
          Times.Once);

      _clientScriptStub.VerifyNoOtherCalls();
    }

    [Test]
    public void AddAttribute_WithNotSupportedEventType_RendersNonTransformedAttribute ()
    {
      _writer.AddAttribute("onbla", "console.info('test');");
      _writer.RenderBeginTag("root");
      _writer.RenderEndTag();

      var document = _htmlHelper.GetResultDocument();
      _htmlHelper.AssertChildElementCount(document, 1);

      var root = _htmlHelper.GetAssertedChildElement(document, "root", 0);
      _htmlHelper.AssertChildElementCount(root, 0);
      _htmlHelper.AssertAttribute(root, "onbla", "console.info('test');");
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
              "eventTargetID-onclick",
              $";(function() {{ const target = document.querySelector('[data-inline-event-target=\"eventTargetID\"]'); if (target) {{ target.onclick = function (event){{console.info('test');}}; }} }})();"),
          Times.Once);

      _clientScriptStub.VerifyNoOtherCalls();

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
              "eventTargetID-onclick",
              $";(function() {{ const target = document.querySelector('[data-inline-event-target=\"eventTargetID\"]'); if (target) {{ target.onclick = function (event){{console.info('test');}}; }} }})();"),
          Times.Once);

      _clientScriptStub.VerifyNoOtherCalls();

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
              "eventTargetID-onclick",
              $";(function() {{ const target = document.querySelector('[data-inline-event-target=\"eventTargetID\"]'); if (target) {{ target.onclick = function (event){{console.info('test');}}; }} }})();"),
          Times.Once);

      _clientScriptStub.VerifyNoOtherCalls();

      _randomNumberGeneratorStub.Verify(m => m.GenerateAlphaNumericNonce(), Times.Once());
    }

    [Test]
    public void AddAttribute_HrefAttributeWithLink_RendersNormally ()
    {
      _writer.AddAttribute("href", "/my/url");
      _writer.RenderBeginTag("div");

      Assert.That(
          _htmlHelper.GetDocumentText(),
          Is.EqualTo($"<div href=\"/my/url\">{Environment.NewLine}"));
    }

    [Test]
    public void AddAttribute_HrefAttributeWithJavaScriptLink ()
    {
      _randomNumberGeneratorStub.Setup(_ => _.GenerateAlphaNumericNonce()).Returns("eventTargetID");
      _renderingFeaturesStub.Setup(_ => _.EnableDiagnosticMetadata).Returns(true);
      _fallbackNavigationUrlProviderStub.Setup(_ => _.GetURL()).Returns("/defaultUrl");

      _writer.AddAttribute("href", " javascript: test");
      _writer.RenderBeginTag("div");

      _clientScriptStub.Verify(
          m => m.RegisterStartupScriptBlock(
              _pageStub.Object,
              typeof(CspEnabledHtmlTextWriter),
              "eventTargetID-href",
              """
                document.querySelector('[data-inline-event-target="eventTargetID"]')?.addEventListener('click', function (event){let __defaultPrevented = event.defaultPrevented;

                event.preventDefault();
                event.preventDefault = () => {
                  __defaultPrevented = true;
                };

                setTimeout(() => {
                  if (!__defaultPrevented) {
                    test
                  }
                }, 0);});
                """),
          Times.Once);

      _clientScriptStub.VerifyNoOtherCalls();

      Assert.That(
          _htmlHelper.GetDocumentText(),
          Is.EqualTo($"<div href=\"/defaultUrl\" data-inline-event-target=\"eventTargetID\" data-event-content-href=\" javascript: test\">{Environment.NewLine}"));
    }

    [TestCase("javascript: console.info('test');", "console.info('test');")]
    [TestCase("javascript:console.info('test');", "console.info('test');")]
    [TestCase("  javascript:  console.info('test');", "console.info('test');")]
    [TestCase("JavaScript: console.info('test');", "console.info('test');")]
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
              "eventTargetID-onclick",
              $";(function() {{ const target = document.querySelector('[data-inline-event-target=\"eventTargetID\"]'); if (target) {{ target.onclick = function (event){{{expected}}}; }} }})();"),
          Times.Once);

      _clientScriptStub.VerifyNoOtherCalls();

      _randomNumberGeneratorStub.Verify(m => m.GenerateAlphaNumericNonce(), Times.Once());
    }

    [Test]
    public void WriteBeginTag_ScriptTag ()
    {
      _writer.WriteBeginTag("script");

      Assert.That(
          _htmlHelper.GetDocumentText(),
          Is.EqualTo("<script nonce=\"TEST-NONCE\""));
    }

    [Test]
    public void WriteBeginTag_NonScriptTag ()
    {
      _writer.WriteBeginTag("div");

      Assert.That(
          _htmlHelper.GetDocumentText(),
          Is.EqualTo("<div"));
    }

    [Test]
    public void WriteFullBeginTag_ScriptTag ()
    {
      _writer.WriteFullBeginTag("script");

      Assert.That(
          _htmlHelper.GetDocumentText(),
          Is.EqualTo("<script nonce=\"TEST-NONCE\">"));
    }

    [Test]
    public void WriteFullBeginTag_NonScriptTag ()
    {
      _writer.WriteFullBeginTag("div");

      Assert.That(
          _htmlHelper.GetDocumentText(),
          Is.EqualTo("<div>"));
    }

    [Test]
    public void WriteAttribute_NonEventAttribute ()
    {
      _writer.WriteBeginTag("div");
      _writer.WriteAttribute("id", "test");

      Assert.That(
          _htmlHelper.GetDocumentText(),
          Is.EqualTo("<div id=\"test\""));
    }

    [Test]
    public void WriteAttribute_EventAttribute ()
    {
      _randomNumberGeneratorStub.Setup(_ => _.GenerateAlphaNumericNonce()).Returns("eventTargetID");

      _writer.WriteBeginTag("div");
      _writer.WriteAttribute("onclick", "test");

      _clientScriptStub.Verify(
          m => m.RegisterStartupScriptBlock(
              _pageStub.Object,
              typeof(CspEnabledHtmlTextWriter),
              "eventTargetID-onclick",
              $";(function() {{ const target = document.querySelector('[data-inline-event-target=\"eventTargetID\"]'); if (target) {{ target.onclick = function (event){{test}}; }} }})();"),
          Times.Once);

      _clientScriptStub.VerifyNoOtherCalls();

      Assert.That(
          _htmlHelper.GetDocumentText(),
          Is.EqualTo("<div data-inline-event-target=\"eventTargetID\""));
    }

    [Test]
    public void WriteAttribute_EventAttributeWithDiagnosticMetadata ()
    {
      _randomNumberGeneratorStub.Setup(_ => _.GenerateAlphaNumericNonce()).Returns("eventTargetID");
      _renderingFeaturesStub.Setup(_ => _.EnableDiagnosticMetadata).Returns(true);

      _writer.WriteBeginTag("div");
      _writer.WriteAttribute("onclick", " javascript:test");

      _clientScriptStub.Verify(
          m => m.RegisterStartupScriptBlock(
              _pageStub.Object,
              typeof(CspEnabledHtmlTextWriter),
              "eventTargetID-onclick",
              $";(function() {{ const target = document.querySelector('[data-inline-event-target=\"eventTargetID\"]'); if (target) {{ target.onclick = function (event){{test}}; }} }})();"),
          Times.Once);

      _clientScriptStub.VerifyNoOtherCalls();

      Assert.That(
          _htmlHelper.GetDocumentText(),
          Is.EqualTo("<div data-inline-event-target=\"eventTargetID\" data-event-content-onclick=\" javascript:test\""));
    }

    [Test]
    public void WriteAttribute_MultipleEventAttributes ()
    {
      _randomNumberGeneratorStub.Setup(_ => _.GenerateAlphaNumericNonce()).Returns("eventTargetID");

      _writer.WriteBeginTag("div");
      _writer.WriteAttribute("onclick", "test");
      _writer.WriteAttribute("onload", "test2");

      _clientScriptStub.Verify(
          m => m.RegisterStartupScriptBlock(
              _pageStub.Object,
              typeof(CspEnabledHtmlTextWriter),
              "eventTargetID-onclick",
              $";(function() {{ const target = document.querySelector('[data-inline-event-target=\"eventTargetID\"]'); if (target) {{ target.onclick = function (event){{test}}; }} }})();"),
          Times.Once);

      _clientScriptStub.Verify(
          m => m.RegisterStartupScriptBlock(
              _pageStub.Object,
              typeof(CspEnabledHtmlTextWriter),
              "eventTargetID-onload",
              $";(function() {{ const target = document.querySelector('[data-inline-event-target=\"eventTargetID\"]'); if (target) {{ target.onload = function (event){{test2}}; }} }})();"),
          Times.Once);

      _clientScriptStub.VerifyNoOtherCalls();

      Assert.That(
          _htmlHelper.GetDocumentText(),
          Is.EqualTo("<div data-inline-event-target=\"eventTargetID\""));
    }

    [Test]
    public void WriteAttribute_MultipleEventAttributesWithDiagnosticMetadata ()
    {
      _randomNumberGeneratorStub.Setup(_ => _.GenerateAlphaNumericNonce()).Returns("eventTargetID");
      _renderingFeaturesStub.Setup(_ => _.EnableDiagnosticMetadata).Returns(true);

      _writer.WriteBeginTag("div");
      _writer.WriteAttribute("onclick", "test");
      _writer.WriteAttribute("onload", "test2");

      _clientScriptStub.Verify(
          m => m.RegisterStartupScriptBlock(
              _pageStub.Object,
              typeof(CspEnabledHtmlTextWriter),
              "eventTargetID-onclick",
              $";(function() {{ const target = document.querySelector('[data-inline-event-target=\"eventTargetID\"]'); if (target) {{ target.onclick = function (event){{test}}; }} }})();"),
          Times.Once);

      _clientScriptStub.Verify(
          m => m.RegisterStartupScriptBlock(
              _pageStub.Object,
              typeof(CspEnabledHtmlTextWriter),
              "eventTargetID-onload",
              $";(function() {{ const target = document.querySelector('[data-inline-event-target=\"eventTargetID\"]'); if (target) {{ target.onload = function (event){{test2}}; }} }})();"),
          Times.Once);

      _clientScriptStub.VerifyNoOtherCalls();

      Assert.That(
          _htmlHelper.GetDocumentText(),
          Is.EqualTo("<div data-inline-event-target=\"eventTargetID\" data-event-content-onclick=\"test\" data-event-content-onload=\"test2\""));
    }

    [Test]
    public void WriteAttribute_EventTargetIdResetsWithWriteBeginTag ()
    {
      _randomNumberGeneratorStub.SetupSequence(_ => _.GenerateAlphaNumericNonce())
          .Returns("eventTargetID1")
          .Returns("eventTargetID2");

      _writer.WriteBeginTag("div");
      _writer.WriteAttribute("onclick", "test");
      _writer.Write("/>");

      _writer.WriteBeginTag("div");
      _writer.WriteAttribute("onclick", "test");
      _writer.Write("/>");

      Assert.That(
          _htmlHelper.GetDocumentText(),
          Is.EqualTo("<div data-inline-event-target=\"eventTargetID1\"/><div data-inline-event-target=\"eventTargetID2\"/>"));
    }

    [Test]
    public void WriteAttribute_EventTargetIdResetsWithWriteFullBeginTag ()
    {
      _randomNumberGeneratorStub.SetupSequence(_ => _.GenerateAlphaNumericNonce())
          .Returns("eventTargetID1")
          .Returns("eventTargetID2");

      _writer.WriteBeginTag("div");
      _writer.WriteAttribute("onclick", "test");
      _writer.Write("/>");

      _writer.WriteFullBeginTag("div");
      _writer.WriteAttribute("onclick", "test");

      Assert.That(
          _htmlHelper.GetDocumentText(),
          Is.EqualTo("<div data-inline-event-target=\"eventTargetID1\"/><div> data-inline-event-target=\"eventTargetID2\""));
    }

    [Test]
    public void WriteAttribute_HrefAttributeWithLink_RendersNormally ()
    {
      _writer.WriteBeginTag("div");
      _writer.WriteAttribute("href", "/my/url");
      _writer.Write("/>");

      Assert.That(
          _htmlHelper.GetDocumentText(),
          Is.EqualTo("<div href=\"/my/url\"/>"));
    }

    [Test]
    public void WriteAttribute_HrefAttributeWithJavaScriptLink ()
    {
      _randomNumberGeneratorStub.Setup(_ => _.GenerateAlphaNumericNonce()).Returns("eventTargetID");
      _renderingFeaturesStub.Setup(_ => _.EnableDiagnosticMetadata).Returns(true);
      _fallbackNavigationUrlProviderStub.Setup(_ => _.GetURL()).Returns("/defaultUrl");

      _writer.WriteBeginTag("div");
      _writer.WriteAttribute("href", " javascript: test");
      _writer.Write("/>");

      _clientScriptStub.Verify(
          m => m.RegisterStartupScriptBlock(
              _pageStub.Object,
              typeof(CspEnabledHtmlTextWriter),
              "eventTargetID-href",
              """
                document.querySelector('[data-inline-event-target="eventTargetID"]')?.addEventListener('click', function (event){let __defaultPrevented = event.defaultPrevented;

                event.preventDefault();
                event.preventDefault = () => {
                  __defaultPrevented = true;
                };

                setTimeout(() => {
                  if (!__defaultPrevented) {
                    test
                  }
                }, 0);});
                """),
          Times.Once);

      _clientScriptStub.VerifyNoOtherCalls();

      Assert.That(
          _htmlHelper.GetDocumentText(),
          Is.EqualTo("<div href=\"/defaultUrl\" data-inline-event-target=\"eventTargetID\" data-event-content-href=\" javascript: test\"/>"));
    }
  }
}
