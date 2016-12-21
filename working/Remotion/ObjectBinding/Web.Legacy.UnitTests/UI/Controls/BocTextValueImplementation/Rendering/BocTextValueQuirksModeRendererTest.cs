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
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocTextValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Rendering;
using Remotion.Web;
using Remotion.Web.UI;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.Legacy.UnitTests.UI.Controls.BocTextValueImplementation.Rendering
{
  [TestFixture]
  public class BocTextValueQuirksModeRendererTest : BocTextValueQuirksModeRendererTestBase<IBocTextValue>
  {
    private const string c_valueName = "MyTextValue_TextValue";
    private const string c_clientID = "MyTextValue";
    private IResourceUrlFactory _resourceUrlFactory;
    private BocTextValueQuirksModeRenderer _renderer;

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      _resourceUrlFactory = new FakeResourceUrlFactory();

      TextValue = MockRepository.GenerateMock<IBocTextValue>();
      _renderer = new BocTextValueQuirksModeRenderer (_resourceUrlFactory);

      TextValue.Stub (stub => stub.ClientID).Return (c_clientID);
      TextValue.Stub (stub => stub.GetValueName()).Return (c_valueName);
      TextValue.Stub (mock => mock.CssClass).PropertyBehavior();

      var pageStub = MockRepository.GenerateStub<IPage>();
      pageStub.Stub (stub => stub.WrappedInstance).Return (new PageMock());

      TextValue.Stub (stub => stub.Page).Return (pageStub);
    }

    [Test]
    public void RenderSingleLineEditabaleAutoPostback ()
    {
      RenderSingleLineEditable (false, false, false, true);
    }

    [Test]
    public void RenderSingleLineEditable ()
    {
      RenderSingleLineEditable (false, false, false, false);
    }

    [Test]
    public void RenderSingleLineDisabled ()
    {
      RenderSingleLineDisabled (false, false, false);
    }

    [Test]
    public void RenderSingleLineReadonly ()
    {
      RenderSingleLineReadonly (false, false, false);
    }

    [Test]
    public void RenderMultiLineReadonly ()
    {
      RenderMultiLineReadonly (false, false, false);
    }

    [Test]
    public void RenderSingleLineEditableWithStyle ()
    {
      RenderSingleLineEditable (true, false, false, false);
    }

    [Test]
    public void RenderSingleLineDisabledWithStyle ()
    {
      RenderSingleLineDisabled (true, false, false);
    }

    [Test]
    public void RenderSingleLineReadonlyWithStyle ()
    {
      RenderSingleLineReadonly (true, false, false);
    }

    [Test]
    public void RenderMultiLineReadonlyWithStyle ()
    {
      RenderMultiLineReadonly (true, false, false);
    }

    [Test]
    public void RenderSingleLineEditableWithStyleAndCssClass ()
    {
      RenderSingleLineEditable (true, true, false, false);
    }

    [Test]
    public void RenderSingleLineDisabledWithStyleAndCssClass ()
    {
      RenderSingleLineDisabled (true, true, false);
    }

    [Test]
    public void RenderSingleLineReadonlyWithStyleAndCssClass ()
    {
      RenderSingleLineReadonly (true, true, false);
    }

    [Test]
    public void RenderMultiLineReadonlyWithStyleAndCssClass ()
    {
      RenderMultiLineReadonly (true, true, false);
    }

    [Test]
    public void RenderSingleLineEditableWithStyleInStandardProperties ()
    {
      RenderSingleLineEditable (true, false, true, false);
    }

    [Test]
    public void RenderSingleLineDisabledWithStyleInStandardProperties ()
    {
      RenderSingleLineDisabled (true, false, true);
    }

    [Test]
    public void RenderSingleLineReadonlyWithStyleInStandardProperties ()
    {
      RenderSingleLineReadonly (true, false, true);
    }

    [Test]
    public void RenderMultiLineReadonlyWithStyleInStandardProperties ()
    {
      RenderMultiLineReadonly (true, false, true);
    }

    [Test]
    public void RenderSingleLineEditableWithStyleAndCssClassInStandardProperties ()
    {
      RenderSingleLineEditable (true, true, true, false);
    }

    [Test]
    public void RenderSingleLineDisabledWithStyleAndCssClassInStandardProperties ()
    {
      RenderSingleLineDisabled (true, true, true);
    }

    [Test]
    public void RenderSingleLineReadonlyWithStyleAndCssClassInStandardProperties ()
    {
      RenderSingleLineReadonly (true, true, true);
    }

    [Test]
    public void RenderMultiLineReadonlyWithStyleAndCssClassInStandardProperties ()
    {
      RenderMultiLineReadonly (true, true, true);
    }

    [Test]
    public void RenderPasswordMaskedEditable ()
    {
      RenderPasswordEditable (true, false);
    }

    [Test]
    public void RenderPasswordMaskedEditableWithAutoPostback ()
    {
      RenderPasswordEditable (true, true);
    }
    
    [Test]
    public void RenderPasswordNoRenderEditable ()
    {
      RenderPasswordEditable (false, false);
    }

    [Test]
    public void RenderPasswordMaskedReadonly ()
    {
      RenderPasswordReadonly (true);
    }
    
    [Test]
    public void RenderPasswordNoRenderReadonly ()
    {
      RenderPasswordReadonly (false);
    }

    private void RenderSingleLineEditable (bool withStyle, bool withCssClass, bool inStandardProperties, bool autoPostBack)
    {
      TextValue.Stub (mock => mock.Text).Return (BocTextValueQuirksModeRendererTestBase<IBocTextValue>.c_firstLineText);

      SetStyle (withStyle, withCssClass, inStandardProperties, autoPostBack);

      _renderer.Render (new BocTextValueRenderingContext (MockRepository.GenerateMock<HttpContextBase> (), Html.Writer, TextValue));
      
      var document = Html.GetResultDocument();
      Html.AssertChildElementCount (document.DocumentElement, 1);

      var span = Html.GetAssertedChildElement (document, "span", 0);
      Html.AssertAttribute (span, "id", c_clientID);
      CheckCssClass (_renderer, span, withCssClass, inStandardProperties);
      Html.AssertStyleAttribute (span, "width", "auto");
      Html.AssertChildElementCount (span, 1);

      var input = Html.GetAssertedChildElement (span, "input", 0);
      Html.AssertAttribute (input, "id", c_valueName);
      Html.AssertAttribute (input, "name", c_valueName);
      Html.AssertAttribute (input, "type", "text");
      Html.AssertAttribute (input, "value", BocTextValueQuirksModeRendererTestBase<IBocTextValue>.c_firstLineText);
      Assert.That (TextValue.TextBoxStyle.AutoPostBack, Is.EqualTo (autoPostBack));
      if (autoPostBack)
        Html.AssertAttribute (input, "onchange", string.Format ("javascript:__doPostBack('{0}','')", c_valueName));
      else
        Html.AssertNoAttribute (input, "onchange");

      CheckStyle (withStyle, span, input);
    }

    private void RenderSingleLineDisabled (bool withStyle, bool withCssClass, bool inStandardProperties)
    {
      TextValue.Stub (mock => mock.Text).Return (BocTextValueQuirksModeRendererTestBase<IBocTextValue>.c_firstLineText);

      SetStyle (withStyle, withCssClass, inStandardProperties, false);

      TextValue.Stub (mock => mock.Enabled).Return (false);
      _renderer.Render (new BocTextValueRenderingContext (MockRepository.GenerateMock<HttpContextBase> (), Html.Writer, TextValue));
      
      var document = Html.GetResultDocument();
      Html.AssertChildElementCount (document.DocumentElement, 1);

      var span = Html.GetAssertedChildElement (document, "span", 0);
      Html.AssertAttribute (span, "id", c_clientID);
      CheckCssClass (_renderer, span, withCssClass, inStandardProperties);
      Html.AssertAttribute (span, "class", _renderer.CssClassDisabled, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertStyleAttribute (span, "width", "auto");
      Html.AssertChildElementCount (span, 1);

      var input = Html.GetAssertedChildElement (span, "input", 0);
      Html.AssertAttribute (input, "disabled", "disabled");
      Html.AssertAttribute (input, "readonly", "readonly");
      Html.AssertAttribute (input, "value", BocTextValueQuirksModeRendererTestBase<IBocTextValue>.c_firstLineText);

      CheckStyle (withStyle, span, input);
    }

    private void RenderSingleLineReadonly (bool withStyle, bool withCssClass, bool inStandardProperties)
    {
      TextValue.Stub (mock => mock.Text).Return (BocTextValueQuirksModeRendererTestBase<IBocTextValue>.c_firstLineText);

      SetStyle (withStyle, withCssClass, inStandardProperties, false);

      TextValue.Stub (mock => mock.IsReadOnly).Return (true);
      _renderer.Render (new BocTextValueRenderingContext (MockRepository.GenerateMock<HttpContextBase> (), Html.Writer, TextValue));

      var document = Html.GetResultDocument();
      Html.AssertChildElementCount (document.DocumentElement, 1);

      var span = Html.GetAssertedChildElement (document, "span", 0);
      Html.AssertAttribute (span, "id", c_clientID);
      CheckCssClass (_renderer, span, withCssClass, inStandardProperties);
      Html.AssertAttribute (span, "class", _renderer.CssClassReadOnly, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertStyleAttribute (span, "width", "auto");
      Html.AssertChildElementCount (span, 1);

      var labelSpan = Html.GetAssertedChildElement (span, "span", 0);
      Html.AssertAttribute (labelSpan, "id", c_valueName);
      Html.AssertTextNode (labelSpan, BocTextValueQuirksModeRendererTestBase<IBocTextValue>.c_firstLineText, 0);

      CheckStyle (withStyle, span, labelSpan);
    }

    private void RenderMultiLineReadonly (bool withStyle, bool withCssClass, bool inStandardProperties)
    {
      TextValue.Stub (mock => mock.Text).Return (
          BocTextValueQuirksModeRendererTestBase<IBocTextValue>.c_firstLineText + Environment.NewLine
          + BocTextValueQuirksModeRendererTestBase<IBocTextValue>.c_secondLineText);
      TextValue.Stub (mock => mock.IsReadOnly).Return (true);

      SetStyle (withStyle, withCssClass, inStandardProperties, false);
      TextValue.TextBoxStyle.TextMode = BocTextBoxMode.MultiLine;

      _renderer.Render (new BocTextValueRenderingContext (MockRepository.GenerateMock<HttpContextBase> (), Html.Writer, TextValue));
      
      var document = Html.GetResultDocument();
      Html.AssertChildElementCount (document.DocumentElement, 1);

      var span = Html.GetAssertedChildElement (document, "span", 0);

      Html.AssertAttribute (span, "id", c_clientID);
      CheckCssClass (_renderer, span, withCssClass, inStandardProperties);
      Html.AssertAttribute (span, "class", _renderer.CssClassReadOnly, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertStyleAttribute (span, "width", "auto");
      Html.AssertChildElementCount (span, 1);

      var labelSpan = Html.GetAssertedChildElement (span, "span", 0);

      Html.AssertTextNode (labelSpan, BocTextValueQuirksModeRendererTestBase<IBocTextValue>.c_firstLineText, 0);
      Html.GetAssertedChildElement (labelSpan, "br", 1);
      Html.AssertTextNode (labelSpan, BocTextValueQuirksModeRendererTestBase<IBocTextValue>.c_secondLineText, 2);
      Html.AssertChildElementCount (labelSpan, 1);

      CheckStyle (withStyle, span, labelSpan);
    }

    private void RenderPasswordEditable (bool renderPassword, bool autoPostBack)
    {
      TextValue.Stub (mock => mock.Text).Return (BocTextValueQuirksModeRendererTestBase<IBocTextValue>.c_firstLineText);

      SetStyle (false, false, false, autoPostBack);
      TextValue.TextBoxStyle.TextMode = renderPassword ? BocTextBoxMode.PasswordRenderMasked : BocTextBoxMode.PasswordNoRender;

      _renderer.Render (new BocTextValueRenderingContext (MockRepository.GenerateMock<HttpContextBase>(), Html.Writer, TextValue));

      var document = Html.GetResultDocument();
      Html.AssertChildElementCount (document.DocumentElement, 1);

      var span = Html.GetAssertedChildElement (document, "span", 0);
      Html.AssertAttribute (span, "id", c_clientID);
      Html.AssertStyleAttribute (span, "width", "auto");
      Html.AssertChildElementCount (span, 1);

      var input = Html.GetAssertedChildElement (span, "input", 0);
      Html.AssertAttribute (input, "type", "password");
      if (renderPassword)
        Html.AssertAttribute (input, "value", BocTextValueQuirksModeRendererTestBase<IBocTextValue>.c_firstLineText);
      else
        Html.AssertNoAttribute (input, "value");
      Assert.That (TextValue.TextBoxStyle.AutoPostBack, Is.EqualTo (autoPostBack));
      if (autoPostBack)
        Html.AssertAttribute (input, "onchange", string.Format ("javascript:__doPostBack('{0}','')", c_valueName));
      else
        Html.AssertNoAttribute (input, "onchange");
    }
    
    private void RenderPasswordReadonly (bool renderPassword)
    {
      TextValue.Stub (mock => mock.Text).Return (BocTextValueQuirksModeRendererTestBase<IBocTextValue>.c_firstLineText);

      SetStyle (false, false, false, false);
      TextValue.TextBoxStyle.TextMode = renderPassword ? BocTextBoxMode.PasswordRenderMasked : BocTextBoxMode.PasswordNoRender;

      TextValue.Stub (mock => mock.IsReadOnly).Return (true);
      _renderer.Render (new BocTextValueRenderingContext (MockRepository.GenerateMock<HttpContextBase> (), Html.Writer, TextValue));
      
      var document = Html.GetResultDocument();
      Html.AssertChildElementCount (document.DocumentElement, 1);

      var span = Html.GetAssertedChildElement (document, "span", 0);

      Html.AssertAttribute (span, "id", c_clientID);

      Html.AssertAttribute (span, "class", _renderer.CssClassReadOnly, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertStyleAttribute (span, "width", "auto");
      Html.AssertChildElementCount (span, 1);

      var labelSpan = Html.GetAssertedChildElement (span, "span", 0);
      Html.AssertTextNode (labelSpan, new string ((char) 9679, 5), 0);
    }

    private void CheckStyle (bool withStyle, XmlNode span, XmlNode valueElement)
    {
      string height = withStyle ? Height.ToString() : null;
      string width = withStyle ? Width.ToString() : /* default constant in BocTextValueRendererBase */ "150pt";
      if (withStyle)
      {
        Html.AssertStyleAttribute (span, "height", height);

        if (height != null)
          Html.AssertStyleAttribute (valueElement, "height", "100%");
        Html.AssertStyleAttribute (valueElement, "width", width);
      }
    }
  }
}