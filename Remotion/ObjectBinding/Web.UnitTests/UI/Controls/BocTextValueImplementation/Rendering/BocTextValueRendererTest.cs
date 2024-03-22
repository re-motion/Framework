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
using System.Web;
using System.Xml;
using Moq;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.FunctionalProgramming;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Rendering;
using Remotion.Web;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocTextValueImplementation.Rendering
{
  [TestFixture]
  public class BocTextValueRendererTest : BocTextValueRendererTestBase<IBocTextValue>
  {
    private const string c_valueName = "MyTextValue_Boc_Textbox";
    private const string c_clientID = "MyTextValue";
    private const string c_labelID = "Label";

    private static readonly PlainTextString s_validationErrors = PlainTextString.CreateFromText("ValidationError");

    private BocTextValueRenderer _renderer;

    [SetUp]
    public void SetUp ()
    {
      Initialize();
      TextValue = new Mock<IBocTextValue>();
      _renderer = new BocTextValueRenderer(
          new FakeResourceUrlFactory(),
          GlobalizationService,
          RenderingFeatures.Default,
          new StubLabelReferenceRenderer(),
          new StubValidationErrorRenderer());
      TextValue.Setup(stub => stub.ClientID).Returns(c_clientID);
      TextValue.Setup(stub => stub.ControlType).Returns("BocTextValue");
      TextValue.Setup(stub => stub.GetValueName()).Returns(c_valueName);
      TextValue.Setup(mock => mock.GetLabelIDs()).Returns(EnumerableUtility.Singleton(c_labelID));
      TextValue.SetupProperty(_ => _.CssClass);
      TextValue.Setup(mock => mock.GetValidationErrors()).Returns(EnumerableUtility.Singleton(s_validationErrors));

      var pageStub = new Mock<IPage>();
      pageStub.Setup(stub => stub.WrappedInstance).Returns(new PageMock());

      TextValue.Setup(stub => stub.Page).Returns(pageStub.Object);
    }

    [Test]
    public void RenderSingleLineEditabaleAutoPostback ()
    {
      RenderSingleLineEditable(false, false, false, true);
    }

    [Test]
    public void RenderSingleLineEditable ()
    {
      RenderSingleLineEditable(false, false, false, false);
    }

    [Test]
    public void RenderSingleLineDisabled ()
    {
      RenderSingleLineDisabled(false, false, false);
    }

    [Test]
    public void RenderSingleLineReadonly ()
    {
      RenderSingleLineReadonly(false, false, false);
    }

    [Test]
    public void RenderMultiLineReadonly ()
    {
      RenderMultiLineReadonly(false, false, false);
    }

    [Test]
    public void RenderSingleLineEditableWithStyle ()
    {
      RenderSingleLineEditable(true, false, false, false);
    }

    [Test]
    public void RenderSingleLineEditableWithStyleAndStandardPlaceholder ()
    {
      RenderSingleLineEditable(true, false, false, false, PlainTextString.CreateFromText("Great Placeholder"));
    }

    [Test]
    public void RenderSingleLineEditableWithStyleAndAutoCompleteEmpty ()
    {
      RenderSingleLineEditable(true, false, false, false, autoComplete: string.Empty);
    }

    [Test]
    public void RenderSingleLineEditableWithStyleAndEncodedPlaceholder ()
    {
      RenderSingleLineEditable(true, false, false, false, PlainTextString.CreateFromText("P. \"The Great\" Holder"));
    }

    [Test]
    public void RenderSingleLineEditableWithStyleAndEmptyPlaceholder ()
    {
      RenderSingleLineEditable(true, false, false, false, PlainTextString.Empty);
    }

    public void RenderSingleLineEditableWithStyleAndTypicalAutoComplete ()
    {
      RenderSingleLineEditable(true, false, false, false, autoComplete: "on");
    }

    [Test]
    public void RenderSingleLineDisabledWithStyle ()
    {
      RenderSingleLineDisabled(true, false, false);
    }

    [Test]
    public void RenderSingleLineReadonlyWithStyle ()
    {
      RenderSingleLineReadonly(true, false, false);
    }

    [Test]
    public void RenderMultiLineReadonlyWithStyle ()
    {
      RenderMultiLineReadonly(true, false, false);
    }

    [Test]
    public void RenderSingleLineEditableWithStyleAndCssClass ()
    {
      RenderSingleLineEditable(true, true, false, false);
    }

    [Test]
    public void RenderSingleLineDisabledWithStyleAndCssClass ()
    {
      RenderSingleLineDisabled(true, true, false);
    }

    [Test]
    public void RenderSingleLineReadonlyWithStyleAndCssClass ()
    {
      RenderSingleLineReadonly(true, true, false);
    }

    [Test]
    public void RenderMultiLineReadonlyWithStyleAndCssClass ()
    {
      RenderMultiLineReadonly(true, true, false);
    }

    [Test]
    public void RenderSingleLineEditableWithStyleInStandardProperties ()
    {
      RenderSingleLineEditable(true, false, true, false);
    }

    [Test]
    public void RenderSingleLineDisabledWithStyleInStandardProperties ()
    {
      RenderSingleLineDisabled(true, false, true);
    }

    [Test]
    public void RenderSingleLineReadonlyWithStyleInStandardProperties ()
    {
      RenderSingleLineReadonly(true, false, true);
    }

    [Test]
    public void RenderMultiLineReadonlyWithStyleInStandardProperties ()
    {
      RenderMultiLineReadonly(true, false, true);
    }

    [Test]
    public void RenderSingleLineEditableWithStyleAndCssClassInStandardProperties ()
    {
      RenderSingleLineEditable(true, true, true, false);
    }

    [Test]
    public void RenderSingleLineDisabledWithStyleAndCssClassInStandardProperties ()
    {
      RenderSingleLineDisabled(true, true, true);
    }

    [Test]
    public void RenderSingleLineReadonlyWithStyleAndCssClassInStandardProperties ()
    {
      RenderSingleLineReadonly(true, true, true);
    }

    [Test]
    public void RenderMultiLineReadonlyWithStyleAndCssClassInStandardProperties ()
    {
      RenderMultiLineReadonly(true, true, true);
    }

    [Test]
    public void RenderPasswordMaskedEditable ()
    {
      RenderPasswordEditable(true, false);
    }

    [Test]
    public void RenderPasswordMaskedEditableWithStandardPlaceholder ()
    {
      RenderPasswordEditable(true, false, PlainTextString.CreateFromText("Great Placeholder"));
    }

    [Test]
    public void RenderPasswordMaskedEditableWithEncodedPlaceholder ()
    {
      RenderPasswordEditable(true, false, PlainTextString.CreateFromText("P. \"The Great\" Holder"));
    }

    [Test]
    public void RenderPasswordMaskedEditableWithEmptyPlaceholder ()
    {
      RenderPasswordEditable(true, false, PlainTextString.Empty);
    }

    [Test]
    public void RenderPasswordMaskedEditableWithAutoPostback ()
    {
      RenderPasswordEditable(true, true);
    }

    [Test]
    public void RenderPasswordMaskedEditableWithAutoPostbackAndAutoCompleteEmpty ()
    {
      RenderPasswordEditable(true, true, autoComplete: string.Empty);
    }

    [Test]
    public void RenderPasswordMaskedEditableWithAutoPostbackAndTypicalAutoComplete ()
    {
      RenderPasswordEditable(true, true, autoComplete: "current-password");
    }

    [Test]
    public void RenderPasswordNoRenderEditable ()
    {
      RenderPasswordEditable(false, false);
    }

    [Test]
    public void RenderPasswordNoRenderEditableWithAutoCompleteEmpty ()
    {
      RenderPasswordEditable(false, false, autoComplete: string.Empty);
    }

    [Test]
    public void RenderPasswordNoRenderEditableWithTypicalAutoComplete ()
    {
      RenderPasswordEditable(false, false, autoComplete:"current-password");
    }

    [Test]
    public void RenderPasswordMaskedReadonly ()
    {
      RenderPasswordReadonly(true);
    }

    [Test]
    public void RenderPasswordNoRenderReadonly ()
    {
      RenderPasswordReadonly(false);
    }

    [Test]
    public void TestDiagnosticMetadataRenderingWithAutoPostBack ()
    {
      _renderer = new BocTextValueRenderer(
          new FakeResourceUrlFactory(),
          GlobalizationService,
          RenderingFeatures.WithDiagnosticMetadata,
          new StubLabelReferenceRenderer(),
          new StubValidationErrorRenderer());
      var span = RenderSingleLineEditable(true, true, true, true);
      Html.AssertAttribute(span, DiagnosticMetadataAttributes.ControlType, "BocTextValue");
      Html.AssertAttribute(span, DiagnosticMetadataAttributes.TriggersPostBack, "true");
    }

    [Test]
    public void TestDiagnosticMetadataRenderingWithoutAutoPostBack ()
    {
      _renderer = new BocTextValueRenderer(
          new FakeResourceUrlFactory(),
          GlobalizationService,
          RenderingFeatures.WithDiagnosticMetadata,
          new StubLabelReferenceRenderer(),
          new StubValidationErrorRenderer());
      var span = RenderSingleLineEditable(true, true, true, false);
      Html.AssertAttribute(span, DiagnosticMetadataAttributes.ControlType, "BocTextValue");
      Html.AssertAttribute(span, DiagnosticMetadataAttributes.TriggersPostBack, "false");
    }

    private XmlNode RenderSingleLineEditable (
        bool withStyle,
        bool withCssClass,
        bool inStandardProperties,
        bool autoPostBack,
        PlainTextString? placeholder = null,
        string autoComplete = null)
    {
      TextValue.Setup(mock => mock.Text).Returns(c_firstLineText);
      TextValue.Setup(mock => mock.Enabled).Returns(true);

      SetStyle(withStyle, withCssClass, inStandardProperties, autoPostBack);
      if (placeholder != null)
        TextValue.Object.TextBoxStyle.Placeholder = placeholder.Value;
      TextValue.Object.TextBoxStyle.AutoComplete = autoComplete;

      _renderer.Render(new BocTextValueRenderingContext(new Mock<HttpContextBase>().Object, Html.Writer, TextValue.Object));

      var document = Html.GetResultDocument();
      Html.AssertChildElementCount(document.DocumentElement, 1);

      var span = Html.GetAssertedChildElement(document, "span", 0);
      Html.AssertAttribute(span, "id", c_clientID);
      CheckCssClass(_renderer, span, withCssClass, inStandardProperties);
      Html.AssertChildElementCount(span, 1);
      var content = Html.GetAssertedChildElement(span, "span", 0);
      Html.AssertAttribute(content, "class", "content");
      Html.AssertChildElementCount(content, 2);

      var input = Html.GetAssertedChildElement(content, "input", 0);
      Html.AssertAttribute(input, "id", c_valueName);
      Html.AssertAttribute(input, "name", c_valueName);
      Html.AssertAttribute(input, StubLabelReferenceRenderer.LabelReferenceAttribute, c_labelID);
      Html.AssertAttribute(input, StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute, "");
      Html.AssertAttribute(input, StubValidationErrorRenderer.ValidationErrorsIDAttribute, c_clientID + "_ValidationErrors");
      Html.AssertAttribute(input, StubValidationErrorRenderer.ValidationErrorsAttribute, s_validationErrors);
      Html.AssertAttribute(input, "type", "text");
      Html.AssertAttribute(input, "value", c_firstLineText);
      Assert.That(TextValue.Object.TextBoxStyle.AutoPostBack, Is.EqualTo(autoPostBack));
      if (autoPostBack)
        Html.AssertAttribute(input, "onchange", string.Format("javascript:__doPostBack('{0}','')", c_valueName));
      else
        Html.AssertNoAttribute(input, "onchange");

      if (placeholder.HasValue && !placeholder.Value.IsEmpty)
        Html.AssertAttribute(input, "placeholder", placeholder);
      else
        Html.AssertNoAttribute(input, "placeholder");

      if (string.IsNullOrEmpty(autoComplete))
        Html.AssertNoAttribute(input, "autocomplete");
      else
        Html.AssertAttribute(input, "autocomplete", autoComplete);

      CheckStyle(withStyle, span, input);

      var validationErrors = Html.GetAssertedChildElement(content, "fake", 1);
      Html.AssertAttribute(validationErrors, StubValidationErrorRenderer.ValidationErrorsIDAttribute, c_clientID + "_ValidationErrors");
      Html.AssertAttribute(validationErrors, StubValidationErrorRenderer.ValidationErrorsAttribute, s_validationErrors);

      return span;
    }

    private void RenderSingleLineDisabled (bool withStyle, bool withCssClass, bool inStandardProperties)
    {
      TextValue.Setup(mock => mock.Text).Returns(c_firstLineText);
      TextValue.Setup(mock => mock.Enabled).Returns(false);

      SetStyle(withStyle, withCssClass, inStandardProperties, false);

      TextValue.Setup(mock => mock.Enabled).Returns(false);
      _renderer.Render(new BocTextValueRenderingContext(new Mock<HttpContextBase>().Object, Html.Writer, TextValue.Object));

      var document = Html.GetResultDocument();
      Html.AssertChildElementCount(document.DocumentElement, 1);

      var span = Html.GetAssertedChildElement(document, "span", 0);
      Html.AssertAttribute(span, "id", c_clientID);
      CheckCssClass(_renderer, span, withCssClass, inStandardProperties);
      Html.AssertAttribute(span, "class", _renderer.CssClassDisabled, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertChildElementCount(span, 1);
      var content = Html.GetAssertedChildElement(span, "span", 0);
      Html.AssertAttribute(content, "class", "content");
      Html.AssertChildElementCount(content, 2);

      var input = Html.GetAssertedChildElement(content, "input", 0);
      Html.AssertAttribute(input, "disabled", "disabled");
      Html.AssertAttribute(input, "readonly", "readonly");
      Html.AssertAttribute(input, "value", c_firstLineText);

      CheckStyle(withStyle, span, input);

      var validationErrors = Html.GetAssertedChildElement(content, "fake", 1);
      Html.AssertAttribute(validationErrors, StubValidationErrorRenderer.ValidationErrorsIDAttribute, c_clientID + "_ValidationErrors");
      Html.AssertAttribute(validationErrors, StubValidationErrorRenderer.ValidationErrorsAttribute, s_validationErrors);
    }

    private void RenderSingleLineReadonly (bool withStyle, bool withCssClass, bool inStandardProperties)
    {
      TextValue.Setup(mock => mock.Text).Returns(c_firstLineText);
      TextValue.Setup(mock => mock.Enabled).Returns(true);

      SetStyle(withStyle, withCssClass, inStandardProperties, false);

      TextValue.Setup(mock => mock.IsReadOnly).Returns(true);
      _renderer.Render(new BocTextValueRenderingContext(new Mock<HttpContextBase>().Object, Html.Writer, TextValue.Object));

      var document = Html.GetResultDocument();
      Html.AssertChildElementCount(document.DocumentElement, 1);

      var span = Html.GetAssertedChildElement(document, "span", 0);
      Html.AssertAttribute(span, "id", c_clientID);
      CheckCssClass(_renderer, span, withCssClass, inStandardProperties);
      Html.AssertAttribute(span, "class", _renderer.CssClassReadOnly, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertChildElementCount(span, 1);
      var content = Html.GetAssertedChildElement(span, "span", 0);
      Html.AssertAttribute(content, "class", "content");
      Html.AssertChildElementCount(content, 3);

      var input = Html.GetAssertedChildElement(content, "input", 0);
      Html.AssertAttribute(input, "id", c_valueName);
      Html.AssertAttribute(input, "name", c_valueName);
      Html.AssertAttribute(input, "readonly", "readonly");
      Html.AssertNoAttribute(input, "disabled");
      Html.AssertAttribute(input, "class", CssClassDefinition.ScreenReaderText);
      Html.AssertAttribute(input, "value", c_firstLineText);
      Html.AssertAttribute(input, StubLabelReferenceRenderer.LabelReferenceAttribute, c_labelID);
      Html.AssertAttribute(input, StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute, "");
      Html.AssertAttribute(input, StubValidationErrorRenderer.ValidationErrorsIDAttribute, c_clientID + "_ValidationErrors");
      Html.AssertAttribute(input, StubValidationErrorRenderer.ValidationErrorsAttribute, s_validationErrors);

      var labelSpan = Html.GetAssertedChildElement(content, "span", 1);
      Html.AssertNoAttribute(labelSpan, "tabindex");
      Html.AssertAttribute(labelSpan, "aria-hidden", "true");
      Html.AssertTextNode(labelSpan, c_firstLineText, 0);

      CheckStyle(withStyle, span, labelSpan);
    }

    private void RenderMultiLineReadonly (bool withStyle, bool withCssClass, bool inStandardProperties)
    {
      TextValue
          .Setup(mock => mock.Text)
          .Returns(c_firstLineText + Environment.NewLine + c_secondLineText);
      TextValue.Setup(mock => mock.IsReadOnly).Returns(true);
      TextValue.Setup(mock => mock.Enabled).Returns(true);

      SetStyle(withStyle, withCssClass, inStandardProperties, false);
      TextValue.Object.TextBoxStyle.TextMode = BocTextBoxMode.MultiLine;

      _renderer.Render(new BocTextValueRenderingContext(new Mock<HttpContextBase>().Object, Html.Writer, TextValue.Object));

      var document = Html.GetResultDocument();
      Html.AssertChildElementCount(document.DocumentElement, 1);

      var span = Html.GetAssertedChildElement(document, "span", 0);

      Html.AssertAttribute(span, "id", c_clientID);
      CheckCssClass(_renderer, span, withCssClass, inStandardProperties);
      Html.AssertAttribute(span, "class", _renderer.CssClassReadOnly, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertChildElementCount(span, 1);
      var content = Html.GetAssertedChildElement(span, "span", 0);
      Html.AssertAttribute(content, "class", "content");
      Html.AssertChildElementCount(content, 3);

      var textarea = Html.GetAssertedChildElement(content, "textarea", 0);
      Html.AssertAttribute(textarea, "id", c_valueName);
      Html.AssertAttribute(textarea, "name", c_valueName);
      Html.AssertAttribute(textarea, "readonly", "readonly");
      Html.AssertNoAttribute(textarea, "disabled");
      Html.AssertAttribute(textarea, "class", CssClassDefinition.ScreenReaderText);
      Html.AssertTextNode(textarea, TextValue.Object.Text, 0);
      Html.AssertAttribute(textarea, StubLabelReferenceRenderer.LabelReferenceAttribute, c_labelID);
      Html.AssertAttribute(textarea, StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute, "");
      Html.AssertAttribute(textarea, StubValidationErrorRenderer.ValidationErrorsIDAttribute, c_clientID + "_ValidationErrors");
      Html.AssertAttribute(textarea, StubValidationErrorRenderer.ValidationErrorsAttribute, s_validationErrors);

      var labelSpan = Html.GetAssertedChildElement(content, "span", 1);

      Html.AssertTextNode(labelSpan, c_firstLineText, 0);
      Html.GetAssertedChildElement(labelSpan, "br", 1);
      Html.AssertTextNode(labelSpan, c_secondLineText, 2);
      Html.AssertChildElementCount(labelSpan, 1);

      CheckStyle(withStyle, span, labelSpan);

      var validationErrors = Html.GetAssertedChildElement(content, "fake", 2);
      Html.AssertAttribute(validationErrors, StubValidationErrorRenderer.ValidationErrorsIDAttribute, c_clientID + "_ValidationErrors");
      Html.AssertAttribute(validationErrors, StubValidationErrorRenderer.ValidationErrorsAttribute, s_validationErrors);
    }

    private void RenderPasswordEditable (bool renderPassword, bool autoPostBack, PlainTextString? placeholder = null, string autoComplete = null)
    {
      TextValue.Setup(mock => mock.Text).Returns(c_firstLineText);
      TextValue.Setup(mock => mock.Enabled).Returns(true);

      SetStyle(false, false, false, autoPostBack);
      TextValue.Object.TextBoxStyle.AutoComplete = autoComplete;
      TextValue.Object.TextBoxStyle.TextMode = renderPassword ? BocTextBoxMode.PasswordRenderMasked : BocTextBoxMode.PasswordNoRender;
      if (placeholder.HasValue)
        TextValue.Object.TextBoxStyle.Placeholder = placeholder.Value;

      _renderer.Render(new BocTextValueRenderingContext(new Mock<HttpContextBase>().Object, Html.Writer, TextValue.Object));

      var document = Html.GetResultDocument();
      Html.AssertChildElementCount(document.DocumentElement, 1);

      var span = Html.GetAssertedChildElement(document, "span", 0);
      Html.AssertAttribute(span, "id", c_clientID);
      Html.AssertChildElementCount(span, 1);
      var content = Html.GetAssertedChildElement(span, "span", 0);
      Html.AssertAttribute(content, "class", "content");
      Html.AssertChildElementCount(content, 2);

      var input = Html.GetAssertedChildElement(content, "input", 0);
      Html.AssertAttribute(input, "type", "password");
      if (renderPassword)
        Html.AssertAttribute(input, "value", c_firstLineText);
      else
        Html.AssertNoAttribute(input, "value");

      if (placeholder.HasValue && !placeholder.Value.IsEmpty)
        Html.AssertAttribute(input, "placeholder", placeholder);
      else
        Html.AssertNoAttribute(input, "placeholder");

      if (string.IsNullOrEmpty(autoComplete))
        Html.AssertNoAttribute(input, "autocomplete");
      else
        Html.AssertAttribute(input, "autocomplete", autoComplete);

      Assert.That(TextValue.Object.TextBoxStyle.AutoPostBack, Is.EqualTo(autoPostBack));
      if (autoPostBack)
        Html.AssertAttribute(input, "onchange", string.Format("javascript:__doPostBack('{0}','')", c_valueName));
      else
        Html.AssertNoAttribute(input, "onchange");

      var validationErrors = Html.GetAssertedChildElement(content, "fake", 1);
      Html.AssertAttribute(validationErrors, StubValidationErrorRenderer.ValidationErrorsIDAttribute, c_clientID + "_ValidationErrors");
      Html.AssertAttribute(validationErrors, StubValidationErrorRenderer.ValidationErrorsAttribute, s_validationErrors);
    }

    private void RenderPasswordReadonly (bool renderPassword)
    {
      TextValue.Setup(mock => mock.Text).Returns(c_firstLineText);
      TextValue.Setup(mock => mock.Enabled).Returns(true);

      SetStyle(false, false, false, false);
      TextValue.Object.TextBoxStyle.TextMode = renderPassword ? BocTextBoxMode.PasswordRenderMasked : BocTextBoxMode.PasswordNoRender;

      TextValue.Setup(mock => mock.IsReadOnly).Returns(true);
      _renderer.Render(new BocTextValueRenderingContext(new Mock<HttpContextBase>().Object, Html.Writer, TextValue.Object));

      var document = Html.GetResultDocument();
      Html.AssertChildElementCount(document.DocumentElement, 1);

      var span = Html.GetAssertedChildElement(document, "span", 0);
      Html.AssertAttribute(span, "id", c_clientID);

      Html.AssertAttribute(span, "class", _renderer.CssClassReadOnly, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertChildElementCount(span, 1);
      var content = Html.GetAssertedChildElement(span, "span", 0);
      Html.AssertAttribute(content, "class", "content");
      Html.AssertChildElementCount(content, 3);

      var input = Html.GetAssertedChildElement(content, "input", 0);
      Html.AssertAttribute(input, "type", "password");
      if (renderPassword)
        Html.AssertAttribute(input, "value", c_firstLineText);
      else
        Html.AssertNoAttribute(input, "value");

      var labelSpan = Html.GetAssertedChildElement(content, "span", 1);
      Html.AssertTextNode(labelSpan, new string((char)9679, 5), 0);

      var validationErrors = Html.GetAssertedChildElement(content, "fake", 2);
      Html.AssertAttribute(validationErrors, StubValidationErrorRenderer.ValidationErrorsIDAttribute, c_clientID + "_ValidationErrors");
      Html.AssertAttribute(validationErrors, StubValidationErrorRenderer.ValidationErrorsAttribute, s_validationErrors);
    }

    private void CheckStyle (bool withStyle, XmlNode span, XmlNode valueElement)
    {
      string height = withStyle ? Height.ToString() : null;
      string width = withStyle ? Width.ToString() : null;
      if (withStyle)
      {
        Html.AssertStyleAttribute(span, "height", height);
        Html.AssertStyleAttribute(span, "width", width);

        if (height != null)
          Html.AssertStyleAttribute(valueElement, "height", "100%");
      }
    }
  }
}
