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
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Rendering;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocTextValueImplementation.Rendering
{
  [TestFixture]
  public class BocMultilineTextValueRendererTest : BocTextValueRendererTestBase<IBocMultilineTextValue>
  {
    private const string c_textValueID = "MyTextValue_Boc_Textbox";
    private BocMultilineTextValueRenderer _renderer;
    
    [SetUp]
    public void SetUp ()
    {
      Initialize();

      TextValue = MockRepository.GenerateMock<IBocMultilineTextValue>();
      TextValue.Stub (mock => mock.Text).Return (
          BocTextValueRendererTestBase<IBocTextValue>.c_firstLineText + Environment.NewLine
          + BocTextValueRendererTestBase<IBocTextValue>.c_secondLineText);
      TextValue.Stub (mock => mock.Value).Return (
          new[] { BocTextValueRendererTestBase<IBocTextValue>.c_firstLineText, BocTextValueRendererTestBase<IBocTextValue>.c_secondLineText });

      TextValue.Stub (stub => stub.ClientID).Return ("MyTextValue");
      TextValue.Stub (stub => stub.ControlType).Return ("BocMultilineTextValue");
      TextValue.Stub (stub => stub.GetValueName()).Return (c_textValueID);

      TextValue.Stub (mock => mock.CssClass).PropertyBehavior();

      var pageStub = MockRepository.GenerateStub<IPage>();
      pageStub.Stub (stub => stub.WrappedInstance).Return (new PageMock());
      TextValue.Stub (stub => stub.Page).Return (pageStub);

      _renderer = new BocMultilineTextValueRenderer (new FakeResourceUrlFactory (), GlobalizationService, RenderingFeatures.Default);
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
      _renderer = new BocMultilineTextValueRenderer (new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.WithDiagnosticMetadata);
      
      var span = RenderMultiLineEditable (false, false, false, false, true);
      Html.AssertAttribute (span, DiagnosticMetadataAttributes.ControlType, "BocMultilineTextValue");
      Html.AssertAttribute (span, DiagnosticMetadataAttributes.TriggersPostBack, "true");
    }

    [Test]
    public void TestDiagnosticMetadataRenderingWithoutAutoPostBack ()
    {
      _renderer = new BocMultilineTextValueRenderer (new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.WithDiagnosticMetadata);
      
      var span = RenderMultiLineEditable (false, false, false, false, false);
      Html.AssertAttribute (span, DiagnosticMetadataAttributes.ControlType, "BocMultilineTextValue");
      Html.AssertAttribute (span, DiagnosticMetadataAttributes.TriggersPostBack, "false");
    }

    private XmlNode RenderMultiLineEditable (bool isDisabled, bool withStyle, bool withCssClass, bool inStandardProperties, bool autoPostBack)
    {
      SetStyle (withStyle, withCssClass, inStandardProperties, autoPostBack);

      TextValue.Stub (mock => mock.Enabled).Return (!isDisabled);

      _renderer.Render (new BocMultilineTextValueRenderingContext (MockRepository.GenerateMock<HttpContextBase> (), Html.Writer, TextValue));
      
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
      Html.AssertChildElementCount (content, 1);

      var textarea = Html.GetAssertedChildElement (content, "textarea", 0);
      Html.AssertAttribute (textarea, "id", c_textValueID);
      Html.AssertAttribute (textarea, "name", c_textValueID);
      if (TextValue.TextBoxStyle.AutoPostBack == true)
        Html.AssertAttribute (textarea, "onchange", string.Format("javascript:__doPostBack('{0}','')", c_textValueID));
      CheckTextAreaStyle (textarea, false, withStyle);
      Html.AssertTextNode (textarea, TextValue.Text, 0);
      Html.AssertChildElementCount (textarea, 0);

      return span;
    }

    private void RenderMultiLineReadOnly (bool withStyle, bool withCssClass, bool inStandardProperties)
    {
      SetStyle (withStyle, withCssClass, inStandardProperties, false);

      TextValue.Stub (mock => mock.IsReadOnly).Return (true);

      _renderer.Render (new BocMultilineTextValueRenderingContext (MockRepository.GenerateMock<HttpContextBase> (), Html.Writer, TextValue));

      var document = Html.GetResultDocument();
      Html.AssertChildElementCount (document.DocumentElement, 1);

      var span = Html.GetAssertedChildElement (document, "span", 0);
      Html.AssertAttribute (span, "id", "MyTextValue");
      CheckCssClass (_renderer, span, withCssClass, inStandardProperties);
      Html.AssertAttribute (span, "class", _renderer.CssClassReadOnly, HtmlHelper.AttributeValueCompareMode.Contains);
      Html.AssertChildElementCount (span, 1);
      var content = Html.GetAssertedChildElement (span, "span", 0);
      Html.AssertAttribute (content, "class", "content");
      Html.AssertChildElementCount (content, 1);

      if (withStyle)
      {
        Html.AssertStyleAttribute (span, "height", Height.ToString());
      }

      var label = Html.GetAssertedChildElement (content, "span", 0);
      Html.AssertAttribute (label, "id", c_textValueID);
      Html.AssertTextNode (label, BocTextValueRendererTestBase<IBocTextValue>.c_firstLineText, 0);
      Html.GetAssertedChildElement (label, "br", 1);
      Html.AssertTextNode (label, BocTextValueRendererTestBase<IBocTextValue>.c_secondLineText, 2);
      Html.AssertChildElementCount (label, 1);
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
      TextValue.TextBoxStyle.TextMode = BocTextBoxMode.MultiLine;
    }
  }
}