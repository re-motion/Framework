// // This file is part of the re-motion Core Framework (www.re-motion.org)
// // Copyright (c) rubicon IT GmbH, www.rubicon.eu
// //
// // The re-motion Core Framework is free software; you can redistribute it
// // and/or modify it under the terms of the GNU Lesser General Public License
// // as published by the Free Software Foundation; either version 2.1 of the
// // License, or (at your option) any later version.
// //
// // re-motion is distributed in the hope that it will be useful,
// // but WITHOUT ANY WARRANTY; without even the implied warranty of
// // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// // GNU Lesser General Public License for more details.
// //
// // You should have received a copy of the GNU Lesser General Public License
// // along with re-motion; if not, see http://www.gnu.org/licenses.
// //
using System.Web;
using System.Web.UI;
using Moq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Web;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocCellAttributeRenderingContextTest
  {
    private HtmlHelper _html;
    private StubColumnDefinition _columnDefinition;
    private Mock<HttpContextBase> _httpContextStub;
    private Mock<IBocList> _bocListStub;
    private Mock<IBocListColumnIndexProvider> _columnIndexProviderStub;
    private BocColumnRenderingContext<StubColumnDefinition> _renderingContext;
    private BocCellAttributeRenderingContext<StubColumnDefinition> _attributeRenderingContext;

    [SetUp]
    public void SetUp ()
    {
      _html = new HtmlHelper();
      _columnDefinition = new StubColumnDefinition();
      _httpContextStub = new Mock<HttpContextBase>();
      _bocListStub = new Mock<IBocList>();
      _columnIndexProviderStub = new Mock<IBocListColumnIndexProvider>();
      _renderingContext = new BocColumnRenderingContext<StubColumnDefinition>(
          new BocColumnRenderingContext(
              _httpContextStub.Object,
              _html.Writer,
              _bocListStub.Object,
              BusinessObjectWebServiceContext.Create(null, null, null),
              _columnDefinition,
              _columnIndexProviderStub.Object,
              17,
              23));
      _attributeRenderingContext = new BocCellAttributeRenderingContext<StubColumnDefinition>(_renderingContext);
    }

    [Test]
    public void AddAttributeToRender_WithStringNameAndStringValueAndDefaultEncodeTrue_EncodesAttributeValue ()
    {
      _attributeRenderingContext.AddAttributeToRender("abbr", "expected\" Value=\"test");
      _renderingContext.Writer.RenderBeginTag("stub");
      _renderingContext.Writer.RenderEndTag();

      var document = _html.GetResultDocument();
      var element = _html.GetAssertedChildElement(document, "stub", 0);
      _html.AssertAttribute(element, "abbr", "expected\" Value=\"test");
    }

    [Test]
    public void AddAttributeToRender_WithStringNameAndStringValueAndDefaultEncodeFalse_DoesNotEncodeAttributeValue ()
    {
      _attributeRenderingContext.AddAttributeToRender("align", "expected\" Value=\"test");
      _renderingContext.Writer.RenderBeginTag("stub");
      _renderingContext.Writer.RenderEndTag();

      var document = _html.GetResultDocument();
      var element = _html.GetAssertedChildElement(document, "stub", 0);
      _html.AssertAttribute(element, "align", "expected");
      _html.AssertAttribute(element, "Value", "test");
    }

    [Test]
    public void AddAttributeToRender_WithStringNameAndStringValueAndFEncodeTrue_EncodesAttributeValue ()
    {
      _attributeRenderingContext.AddAttributeToRender("expectedAttribute", "expected\" Value=\"test", true);
      _renderingContext.Writer.RenderBeginTag("stub");
      _renderingContext.Writer.RenderEndTag();

      var document = _html.GetResultDocument();
      var element = _html.GetAssertedChildElement(document, "stub", 0);
      _html.AssertAttribute(element, "expectedAttribute", "expected\" Value=\"test");
    }

    [Test]
    public void AddAttributeToRender_WithStringNameAndStringValueAndFEncodeFalse_DoesNotEncodeAttributeValue ()
    {
      _attributeRenderingContext.AddAttributeToRender("expectedAttribute", "expected\" Value=\"test", false);
      _renderingContext.Writer.RenderBeginTag("stub");
      _renderingContext.Writer.RenderEndTag();

      var document = _html.GetResultDocument();
      var element = _html.GetAssertedChildElement(document, "stub", 0);
      _html.AssertAttribute(element, "expectedAttribute", "expected");
      _html.AssertAttribute(element, "Value", "test");
    }

    [Test]
    public void AddAttributeToRender_WithStringNameAndPlainTextStringValueAndDefaultEncodeTrue_EncodesAttributeValue ()
    {
      _attributeRenderingContext.AddAttributeToRender("abbr",  PlainTextString.CreateFromText("expected\" Value=\"test"));
      _renderingContext.Writer.RenderBeginTag("stub");
      _renderingContext.Writer.RenderEndTag();

      var document = _html.GetResultDocument();
      var element = _html.GetAssertedChildElement(document, "stub", 0);
      _html.AssertAttribute(element, "abbr", "expected\" Value=\"test");
    }

    [Test]
    public void AddAttributeToRender_WithStringNameAndPlainTextStringValueAndDefaultEncodeFalse_EncodesAttributeValue ()
    {
      _attributeRenderingContext.AddAttributeToRender("align",  PlainTextString.CreateFromText("expected\" Value=\"test"));
      _renderingContext.Writer.RenderBeginTag("stub");
      _renderingContext.Writer.RenderEndTag();

      var document = _html.GetResultDocument();
      var element = _html.GetAssertedChildElement(document, "stub", 0);
      _html.AssertAttribute(element, "align", "expected\" Value=\"test");
    }

    [Test]
    public void AddAttributeToRender_WithHtmlTextWriterAttributeNameAndStringValueAndDefaultEncodeTrue_EncodesAttributeValue ()
    {
      _attributeRenderingContext.AddAttributeToRender(HtmlTextWriterAttribute.Abbr, "expected\" Value=\"test");
      _renderingContext.Writer.RenderBeginTag("stub");
      _renderingContext.Writer.RenderEndTag();

      var document = _html.GetResultDocument();
      var element = _html.GetAssertedChildElement(document, "stub", 0);
      _html.AssertAttribute(element, "abbr", "expected\" Value=\"test");
    }

    [Test]
    public void AddAttributeToRender_WithHtmlTextWriterAttributeNameAndStringValueAndDefaultEncodeFalse_DoesNotEncodeAttributeValue ()
    {
      _attributeRenderingContext.AddAttributeToRender(HtmlTextWriterAttribute.Align, "expected\" Value=\"test");
      _renderingContext.Writer.RenderBeginTag("stub");
      _renderingContext.Writer.RenderEndTag();

      var document = _html.GetResultDocument();
      var element = _html.GetAssertedChildElement(document, "stub", 0);
      _html.AssertAttribute(element, "align", "expected");
      _html.AssertAttribute(element, "Value", "test");
    }

    [Test]
    public void AddAttributeToRender_WithHtmlTextWriterAttributeNameAndStringValueAndFEncodeTrue_EncodesAttributeValue ()
    {
      _attributeRenderingContext.AddAttributeToRender(HtmlTextWriterAttribute.Align, "expected\" Value=\"test", true);
      _renderingContext.Writer.RenderBeginTag("stub");
      _renderingContext.Writer.RenderEndTag();

      var document = _html.GetResultDocument();
      var element = _html.GetAssertedChildElement(document, "stub", 0);
      _html.AssertAttribute(element, "align", "expected\" Value=\"test");
    }

    [Test]
    public void AddAttributeToRender_WithHtmlTextWriterAttributeNameAndStringValueAndFEncodeFalse_DoesNotEncodeAttributeValue ()
    {
      _attributeRenderingContext.AddAttributeToRender(HtmlTextWriterAttribute.Abbr, "expected\" Value=\"test", false);
      _renderingContext.Writer.RenderBeginTag("stub");
      _renderingContext.Writer.RenderEndTag();

      var document = _html.GetResultDocument();
      var element = _html.GetAssertedChildElement(document, "stub", 0);
      _html.AssertAttribute(element, "abbr", "expected");
      _html.AssertAttribute(element, "Value", "test");
    }

    [Test]
    public void AddAttributeToRender_WithHtmlTextWriterAttributeNameAndPlainTextStringValueAndDefaultEncodeTrue_EncodesAttributeValue ()
    {
      _attributeRenderingContext.AddAttributeToRender(HtmlTextWriterAttribute.Abbr, PlainTextString.CreateFromText("expected\" Value=\"test"));
      _renderingContext.Writer.RenderBeginTag("stub");
      _renderingContext.Writer.RenderEndTag();

      var document = _html.GetResultDocument();
      var element = _html.GetAssertedChildElement(document, "stub", 0);
      _html.AssertAttribute(element, "abbr", "expected\" Value=\"test");
    }

    [Test]
    public void AddAttributeToRender_WithHtmlTextWriterAttributeNameAndPlainTextStringValueAndDefaultEncodeFalse_EncodesAttributeValue ()
    {
      _attributeRenderingContext.AddAttributeToRender(HtmlTextWriterAttribute.Align, PlainTextString.CreateFromText("expected\" Value=\"test"));
      _renderingContext.Writer.RenderBeginTag("stub");
      _renderingContext.Writer.RenderEndTag();

      var document = _html.GetResultDocument();
      var element = _html.GetAssertedChildElement(document, "stub", 0);
      _html.AssertAttribute(element, "align", "expected\" Value=\"test");
    }

    [Test]
    public void AddStyleAttributeToRender_WithStringNameAndStringValue_AddsStyleAttribute ()
    {
      _attributeRenderingContext.AddStyleAttributeToRender("color", "red");
      _renderingContext.Writer.RenderBeginTag("stub");
      _renderingContext.Writer.RenderEndTag();

      var document = _html.GetResultDocument();
      var element = _html.GetAssertedChildElement(document, "stub", 0);
      _html.AssertStyleAttribute(element, "color", "red");
    }

    [Test]
    public void AddStyleAttributeToRender_WithHtmlTextWriterStyleNameAndStringValue_AddsStyleAttribute ()
    {
      _attributeRenderingContext.AddStyleAttributeToRender(HtmlTextWriterStyle.Color, "red");
      _renderingContext.Writer.RenderBeginTag("stub");
      _renderingContext.Writer.RenderEndTag();

      var document = _html.GetResultDocument();
      var element = _html.GetAssertedChildElement(document, "stub", 0);
      _html.AssertStyleAttribute(element, "color", "red");
    }
  }
}
