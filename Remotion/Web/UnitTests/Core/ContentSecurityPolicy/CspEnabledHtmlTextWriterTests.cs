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
#if !NETFRAMEWORK
using System;
using System.Web.UI;
using Moq;
using NUnit.Framework;
using Remotion.Web.UnitTests.Core.UI.Controls;
using Remotion.Web.ContentSecurityPolicy;
using Remotion.Web.UI;

namespace Remotion.Web.UnitTests.Core.ContentSecurityPolicy
{
  [TestFixture]
  public class CspEnabledHtmlTextWriterTests
  {
    private HtmlHelper _htmlHelper;
    private CspEnabledHtmlTextWriter _writer;
    private Mock<ISmartPageClientScriptManager> _clientScriptMock;
    private Mock<ISmartPage> _pageMock;
    private Mock<INonceGenerator> _randomNumberGeneratorMock;

    [SetUp]
    public void SetUp ()
    {
       _pageMock = new Mock<ISmartPage>();
      _clientScriptMock = new Mock<ISmartPageClientScriptManager>();
      _randomNumberGeneratorMock = new Mock<INonceGenerator>();

      _pageMock
          .Setup(s => s.ClientScript)
          .Returns(_clientScriptMock.Object);

      _htmlHelper = new HtmlHelper();
      _writer = new CspEnabledHtmlTextWriter(_pageMock.Object, _htmlHelper.Writer, "TEST-NONCE", _randomNumberGeneratorMock.Object);
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
    public void RenderBeginTag_WithTagOtherThanScript_NonceValueIsNotRendered ()
    {
      _writer.RenderBeginTag(HtmlTextWriterTag.Button);
      _writer.RenderEndTag();

      var document = _htmlHelper.GetResultDocument();
      var element = _htmlHelper.GetAssertedChildElement(document, "button", 0);
      _htmlHelper.AssertNoAttribute(element, "nonce");
    }

    [TestCase("onclick")]
    [TestCase("onchange")]
    [TestCase("onmouseover")]
    [TestCase("onmouseout")]
    [TestCase("onkeyup")]
    [TestCase("onkeydown")]
    [TestCase("onkeypress")]
    public void AddAttribute_WithSupportedEventType_NonceValueIsRendered (string eventType)
    {
      _randomNumberGeneratorMock
          .Setup(m => m.GenerateAlphaNumericNonce())
          .Returns("eventTargetID")
          .Verifiable();

      _clientScriptMock
          .Setup(m => m.RegisterStartupScriptBlock(_pageMock.Object, typeof(CspEnabledHtmlTextWriter), It.IsAny<string>(), It.IsAny<string>()))
          .Verifiable();


      _writer.AddAttribute(eventType, "console.info('test');");
      _writer.RenderBeginTag(HtmlTextWriterTag.Button);
      _writer.RenderEndTag();

      var document = _htmlHelper.GetResultDocument();
      var element = _htmlHelper.GetAssertedChildElement(document, "button", 0);
      _htmlHelper.AssertAttribute(element, "data-inline-event-target", "eventTargetID");


      _clientScriptMock.Verify();
      _randomNumberGeneratorMock.Verify(m => m.GenerateAlphaNumericNonce(), Times.Once());
    }

    [Test]
    public void AddAttribute_WithAttributeOtherThanEventType_NonceValueIsNotRendered ()
    {
      _writer.AddAttribute("style", "color:white;");
      _writer.RenderBeginTag(HtmlTextWriterTag.Button);
      _writer.RenderEndTag();

      var document = _htmlHelper.GetResultDocument();
      var element = _htmlHelper.GetAssertedChildElement(document, "button", 0);
      _htmlHelper.AssertNoAttribute(element, "data-inline-event-target");

      _clientScriptMock.Verify();
    }

    [Test]
    public void AddAttribute_WithMoreThanOneAttribute_NonceValueIsRendered ()
    {
      _randomNumberGeneratorMock
          .Setup(m => m.GenerateAlphaNumericNonce())
          .Returns("eventTargetID")
          .Verifiable();

      _clientScriptMock
          .Setup(m => m.RegisterStartupScriptBlock(_pageMock.Object, typeof(CspEnabledHtmlTextWriter), It.IsAny<string>(), It.IsAny<string>()))
          .Verifiable();

      _writer.AddAttribute("onclick", "console.info('test1');");
      _writer.AddAttribute("onchange", "console.info('test2');");
      _writer.RenderBeginTag(HtmlTextWriterTag.Button);
      _writer.RenderEndTag();

      var document = _htmlHelper.GetResultDocument();
      var element = _htmlHelper.GetAssertedChildElement(document, "button", 0);
      _htmlHelper.AssertAttribute(element, "data-inline-event-target", "eventTargetID");

      _clientScriptMock.Verify();
      _randomNumberGeneratorMock.Verify(m => m.GenerateAlphaNumericNonce(), Times.Once());
    }

    [Test]
    public void AddAttribute_WithMoreThanOneTag_NonceValueIsRendered ()
    {
      _randomNumberGeneratorMock
          .SetupSequence(m => m.GenerateAlphaNumericNonce())
          .Returns("eventTargetID1")
          .Returns("eventTargetID2");

      _clientScriptMock
          .Setup(m => m.RegisterStartupScriptBlock(_pageMock.Object, typeof(CspEnabledHtmlTextWriter), It.IsAny<string>(), It.IsAny<string>()))
          .Verifiable();

      _writer.RenderBeginTag("root");

      _writer.AddAttribute("onchange", "console.info('test1');");
      _writer.RenderBeginTag(HtmlTextWriterTag.Label);
      _writer.RenderEndTag();

      _writer.AddAttribute("onclick", "console.info('test2');");
      _writer.RenderBeginTag(HtmlTextWriterTag.Button);
      _writer.RenderEndTag();

      _writer.RenderEndTag();

      var document = _htmlHelper.GetResultDocument();
      var root = _htmlHelper.GetAssertedChildElement(document, "root", 0);
      _htmlHelper.AssertChildElementCount(root, 2);

      var element1 = _htmlHelper.GetAssertedChildElement(root, "label", 0);
      _htmlHelper.AssertAttribute(element1, "data-inline-event-target", "eventTargetID1");

      var element2 = _htmlHelper.GetAssertedChildElement(root, "button", 1);
      _htmlHelper.AssertAttribute(element2, "data-inline-event-target", "eventTargetID2");

      _clientScriptMock.Verify();
    }

    [Test]
    public void AddAttribute_WithNotSupportedEventType_ThrowsNotSupportedException ()
    {
      Assert.That(
          () => _writer.AddAttribute("onload", "console.info('test');"),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo("onload is not supported."));
    }

    [Test]
    public void AddAttribute_WithAlreadyRegisteredEventType_ThrowsNotSupportedException ()
    {
      Assert.That(
          () =>
          {
            _writer.AddAttribute("onchange", "console.info('test1');");
            _writer.AddAttribute("onchange", "console.info('test2');");
          },
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo("onchange cannot be added more than once."));
    }
  }
}
#endif
