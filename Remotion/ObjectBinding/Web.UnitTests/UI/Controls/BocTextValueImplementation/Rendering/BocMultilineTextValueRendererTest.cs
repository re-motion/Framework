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
using System.Linq;
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
  public class BocMultilineTextValueRendererTest : BocTextValueRendererTestBase<IBocMultilineTextValue>
  {
    private const string c_textValueID = "MyTextValue_Boc_Textbox";
    private const string c_labelID = "Label";

    private static readonly PlainTextString c_validationErrors = PlainTextString.CreateFromText ("ValidationError");

    private BocMultilineTextValueRenderer _renderer;
    
    [SetUp]
    public void SetUp ()
    {
      Initialize();

      TextValue = new Mock<IBocMultilineTextValue>();
      TextValue.Setup (mock => mock.Text).Returns (
          BocTextValueRendererTestBase<IBocTextValue>.c_firstLineText + Environment.NewLine
          + BocTextValueRendererTestBase<IBocTextValue>.c_secondLineText);
      TextValue.Setup (mock => mock.Value).Returns (
          new[] { BocTextValueRendererTestBase<IBocTextValue>.c_firstLineText, BocTextValueRendererTestBase<IBocTextValue>.c_secondLineText });

      TextValue.Setup (stub => stub.ClientID).Returns ("MyTextValue");
      TextValue.Setup (stub => stub.ControlType).Returns ("BocMultilineTextValue");
      TextValue.Setup (stub => stub.GetValueName()).Returns (c_textValueID);
      TextValue.Setup (mock => mock.GetLabelIDs()).Returns (EnumerableUtility.Singleton (c_labelID));
      TextValue.Setup (mock => mock.GetValidationErrors()).Returns (EnumerableUtility.Singleton (c_validationErrors));
      TextValue.SetupProperty (_ => _.CssClass);

      var pageStub = new Mock<IPage>();
      pageStub.Setup (stub => stub.WrappedInstance).Returns (new PageMock());
      TextValue.Setup (stub => stub.Page).Returns (pageStub.Object);

      _renderer = new BocMultilineTextValueRenderer (
          new FakeResourceUrlFactory(),
          GlobalizationService,
          RenderingFeatures.Default,
          new StubLabelReferenceRenderer(),
          new StubValidationErrorRenderer());
    }

    [Test]
    public void RenderMultiLineEditableAutoPostback ()
    {
      RenderMultiLineEditable (false, false, false, false, true);
    }

    [Test]
    public void RenderMultiLineEditable ()
    {
      RenderMultiLineEditable (false, false, false, false, false);
    }

    [Test]
    public void RenderMultiLineEditableWithStyle ()
    {
      RenderMultiLineEditable (false, true, false, false, false);
    }

    [Test]
    public void RenderMultiLineEditableWithStyleInStandardProperties ()
    {
      RenderMultiLineEditable (false, true, false, true, false);
    }

    [Test]
    public void RenderMultiLineEditableWithStyleAndCssClass ()
    {
      RenderMultiLineEditable (false, true, true, false, false);
    }

    [Test]
    public void RenderMultiLineEditableWithStyleAndCssClassInStandardProperties ()
    {
      RenderMultiLineEditable (false, true, true, true, false);
    }

    [Test]
    public void RenderMultiLineReadonly ()
    {
      RenderMultiLineReadOnly (false, false, false);
    }

    [Test]
    public void RenderMultiLineReadonlyWithStyle ()
    {
      RenderMultiLineReadOnly (true, false, false);
    }

    [Test]
    public void RenderMultiLineReadonlyWithStyleInStandardProperties ()
    {
      RenderMultiLineReadOnly (true, false, true);
    }

    [Test]
    public void RenderMultiLineReadonlyWithStyleAndCssClass ()
    {
      RenderMultiLineReadOnly (true, true, false);
    }

    [Test]
    public void RenderMultiLineReadonlyWithStyleAndCssClassInStandardProperties ()
    {
      RenderMultiLineReadOnly (true, true, true);
    }

    [Test]
    public void RenderMultiLineDisabled ()
    {
      RenderMultiLineEditable (true, false, false, false, false);
    }

    [Test]
    public void RenderMultiLineDisabledWithStyle ()
    {
      RenderMultiLineEditable (true, true, false, false, false);
    }

    [Test]
    public void RenderMultiLineDisabledWithStyleInStandardProperties ()
    {
      RenderMultiLineEditable (true, true, false, true, false);
    }

    [Test]
    public void TestDiagnosticMetadataRenderingWithAutoPostBack ()
    {
      _renderer = new BocMultilineTextValueRenderer (
          new FakeResourceUrlFactory(),
          GlobalizationService,
          RenderingFeatures.WithDiagnosticMetadata,
          new StubLabelReferenceRenderer(),
          new StubValidationErrorRenderer());
      
      var span = RenderMultiLineEditable (false, false, false, false, true);
      Html.AssertAttribute (span, DiagnosticMetadataAttributes.ControlType, "BocMultilineTextValue");
      Html.AssertAttribute (span, DiagnosticMetadataAttributes.TriggersPostBack, "true");
    }

    [Test]
    public void TestDiagnosticMetadataRenderingWithoutAutoPostBack ()
    {
      _renderer = new BocMultilineTextValueRenderer (
          new FakeResourceUrlFactory(),
          GlobalizationService,
          RenderingFeatures.WithDiagnosticMetadata,
          new StubLabelReferenceRenderer(),
          new StubValidationErrorRenderer());
      
      var span = RenderMultiLineEditable (false, false, false, false, false);
      Html.AssertAttribute (span, DiagnosticMetadataAttributes.ControlType, "BocMultilineTextValue");
      Html.AssertAttribute (span, DiagnosticMetadataAttributes.TriggersPostBack, "false");
    }

    private XmlNode RenderMultiLineEditable (bool isDisabled, bool withStyle, bool withCssClass, bool inStandardProperties, bool autoPostBack)
    {
      SetStyle (withStyle, withCssClass, inStandardProperties, autoPostBack);

      TextValue.Setup (mock => mock.Enabled).Returns (!isDisabled);

      _renderer.Render (new BocMultilineTextValueRenderingContext (new Mock<HttpContextBase>().Object, Html.Writer, TextValue.Object));
      
      var document = Html.GetResultDocument();
      Html.AssertChildElementCount (document.DocumentElement, 1);

      var span = Html.GetAssertedChildElement (document, "span", 0);
      Html.AssertAttribute (span, "id", "MyTextValue");
      CheckCssClass (_renderer, span, withCssClass, inStandardProperties);
      if (isDisabled)
        Html.AssertAttribute (span, "class", _renderer.CssClassDisabled, HtmlHelper.AttributeValueCompareMode.Contains);

      Html.AssertChildElementCount (span, 1);
      var content = Html.GetAssertedChildElement (span, "span", 0);
      Html.AssertAttribute (content, "class", "content");
      Html.AssertChildElementCount (content, 2);

      var textarea = Html.GetAssertedChildElement (content, "textarea", 0);
      Html.AssertAttribute (textarea, "id", c_textValueID);
      Html.AssertAttribute (textarea, "name", c_textValueID);
      Html.AssertAttribute (textarea, StubLabelReferenceRenderer.LabelReferenceAttribute, c_labelID);
      Html.AssertAttribute (textarea, StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute, "");
      Html.AssertAttribute (textarea, StubValidationErrorRenderer.ValidationErrorsIDAttribute, "MyTextValue_ValidationErrors");
      Html.AssertAttribute (textarea, StubValidationErrorRenderer.ValidationErrorsAttribute, c_validationErrors);
      if (TextValue.Object.TextBoxStyle.AutoPostBack == true)
        Html.AssertAttribute (textarea, "onchange", string.Format("javascript:__doPostBack('{0}','')", c_textValueID));
      CheckTextAreaStyle (textarea, false, withStyle);
      Html.AssertTextNode (textarea, TextValue.Object.Text, 0);
      Html.AssertChildElementCount (textarea, 0);

      var validationErrors = Html.GetAssertedChildElement (content, "fake", 1);
      Html.AssertAttribute (validationErrors, StubValidationErrorRenderer.ValidationErrorsIDAttribute, "MyTextValue_ValidationErrors");
      Html.AssertAttribute (validationErrors, StubValidationErrorRenderer.ValidationErrorsAttribute, c_validationErrors);

      return span;
    }

    private void RenderMultiLineReadOnly (bool withStyle, bool withCssClass, bool inStandardProperties)
    {
      SetStyle (withStyle, withCssClass, inStandardProperties, false);

      TextValue.Setup (mock => mock.IsReadOnly).Returns (true);

      _renderer.Render (new BocMultilineTextValueRenderingContext (new Mock<HttpContextBase>().Object, Html.Writer, TextValue.Object));

      var document = Html.GetResultDocument();
      Html.AssertChildElementCount (document.DocumentElement, 1);

      var span = Html.GetAssertedChildElement (document, "span", 0);
      Html.AssertAttribute (span, "id", "MyTextValue");
      CheckCssClass (_renderer, span, withCssClass, inStandardProperties);
      Html.AssertAttribute (span, "class", _renderer.CssClassReadOnly, HtmlHelper.AttributeValueCompareMode.Contains);
      Html.AssertChildElementCount (span, 1);
      var content = Html.GetAssertedChildElement (span, "span", 0);
      Html.AssertAttribute (content, "class", "content");
      Html.AssertChildElementCount (content, 2);

      if (withStyle)
      {
        Html.AssertStyleAttribute (span, "height", Height.ToString());
      }

      var label = Html.GetAssertedChildElement (content, "span", 0);
      Html.AssertAttribute (label, "id", c_textValueID);
      Html.AssertAttribute (label, StubLabelReferenceRenderer.LabelReferenceAttribute, c_labelID);
      Html.AssertAttribute (label, StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute, c_textValueID);
      Html.AssertAttribute (label, StubValidationErrorRenderer.ValidationErrorsIDAttribute, "MyTextValue_ValidationErrors");
      Html.AssertAttribute (label, StubValidationErrorRenderer.ValidationErrorsAttribute, c_validationErrors);
      Html.AssertAttribute (label, "tabindex", "0");
      Html.AssertTextNode (label, BocTextValueRendererTestBase<IBocTextValue>.c_firstLineText, 0);
      Html.GetAssertedChildElement (label, "br", 1);
      Html.AssertTextNode (label, BocTextValueRendererTestBase<IBocTextValue>.c_secondLineText, 2);
      Html.AssertChildElementCount (label, 1);

      var validationErrors = Html.GetAssertedChildElement (content, "fake", 1);
      Html.AssertAttribute (validationErrors, StubValidationErrorRenderer.ValidationErrorsIDAttribute, "MyTextValue_ValidationErrors");
      Html.AssertAttribute (validationErrors, StubValidationErrorRenderer.ValidationErrorsAttribute, c_validationErrors);
    }

    private void CheckTextAreaStyle (XmlNode textarea, bool isDisabled, bool withStyle)
    {
      Html.AssertAttribute (textarea, "rows", "2"); //ASP.NET Default
      Html.AssertAttribute (textarea, "cols", "20"); //ASP.NET Default

      if (isDisabled)
      {
        Html.AssertAttribute (textarea, "readonly", "readonly");
        Html.AssertAttribute (textarea, "disabled", "disabled");
      }
    }

    protected override void SetStyle (bool withStyle, bool withCssClass, bool inStyleProperty, bool autoPostBack)
    {
      base.SetStyle (withStyle, withCssClass, inStyleProperty, autoPostBack);
      TextValue.Object.TextBoxStyle.TextMode = BocTextBoxMode.MultiLine;
    }
  }
}