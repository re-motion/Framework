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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.FunctionalProgramming;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Rendering;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocBooleanValueImplementation.Rendering
{
  [TestFixture]
  public class BocCheckBoxRendererTest : RendererTestBase
  {
    private const string c_postbackEventReference = "postbackEventReference";
    private const string c_trueDescription = "Wahr";
    private const string c_falseDescription = "Falsch";
    private const string c_cssClass = "someCssClass";
    private const string c_clientID = "MyCheckBox";
    private const string c_valueName = "MyCheckBox_Value";
    private const string c_labelID = "Label";
    private const string c_validationErrors = "ValidationError";

    private readonly string _startUpScriptKey = typeof (BocCheckBox).FullName + "_Startup";

    private IBocCheckBox _checkbox;
    private string _startupScript;
    private BocCheckBoxRenderer _renderer;

    [SetUp]
    public void SetUp ()
    {
      Initialize();
      _checkbox = MockRepository.GenerateMock<IBocCheckBox>();

      _checkbox.Stub (mock => mock.ClientID).Return (c_clientID);
      _checkbox.Stub (mock => mock.ControlType).Return ("BocCheckBox");
      _checkbox.Stub (mock => mock.GetValueName()).Return (c_valueName);
      _checkbox.Stub (mock => mock.GetLabelIDs()).Return (EnumerableUtility.Singleton (c_labelID));
      _checkbox.Stub (mock => mock.GetValidationErrors()).Return (EnumerableUtility.Singleton (c_validationErrors));
      
      var clientScriptManagerMock = MockRepository.GenerateMock<IClientScriptManager>();
      _startupScript = string.Format (
          "BocCheckBox_InitializeGlobals ('{0}', '{1}');",
          _checkbox.DefaultTrueDescription,
          _checkbox.DefaultFalseDescription);
      clientScriptManagerMock.Expect (mock => mock.RegisterStartupScriptBlock (_checkbox, typeof (BocCheckBoxRenderer), _startUpScriptKey, _startupScript));
      clientScriptManagerMock.Stub (mock => mock.IsStartupScriptRegistered (Arg<Type>.Is.NotNull, Arg<string>.Is.NotNull)).Return (false);
      clientScriptManagerMock.Stub (mock => mock.GetPostBackEventReference (_checkbox, string.Empty)).Return (c_postbackEventReference);

      var pageStub = MockRepository.GenerateStub<IPage>();
      pageStub.Stub (stub => stub.ClientScript).Return (clientScriptManagerMock);

      _checkbox.Stub (mock => mock.Value).PropertyBehavior();
      _checkbox.Stub (mock => mock.IsDesignMode).Return (false);
      _checkbox.Stub (mock => mock.IsDescriptionEnabled).Return (true);

      _checkbox.Stub (mock => mock.Page).Return (pageStub);
      _checkbox.Stub (mock => mock.TrueDescription).Return (c_trueDescription);
      _checkbox.Stub (mock => mock.FalseDescription).Return (c_falseDescription);

      _checkbox.Stub (mock => mock.CssClass).PropertyBehavior();

      StateBag stateBag = new StateBag();
      _checkbox.Stub (mock => mock.Attributes).Return (new AttributeCollection (stateBag));
      _checkbox.Stub (mock => mock.Style).Return (_checkbox.Attributes.CssStyle);
      _checkbox.Stub (mock => mock.LabelStyle).Return (new Style (stateBag));
      _checkbox.Stub (mock => mock.ControlStyle).Return (new Style (stateBag));
    }

    [Test]
    public void RenderTrue ()
    {
      _checkbox.Stub (mock => mock.Enabled).Return (true);
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalse ()
    {
      _checkbox.Stub (mock => mock.Enabled).Return (true);
      CheckRender (false, _checkbox.FalseDescription);
    }

    [Test]
    public void RenderTrueReadOnly ()
    {
      _checkbox.Stub (mock => mock.Enabled).Return (true);
      _checkbox.Stub (mock => mock.IsRequired).Return (true);
      _checkbox.Stub (mock => mock.IsReadOnly).Return (true);
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalseReadOnly ()
    {
      _checkbox.Stub (mock => mock.Enabled).Return (true);
      _checkbox.Stub (mock => mock.IsRequired).Return (true);
      _checkbox.Stub (mock => mock.IsReadOnly).Return (true);
      CheckRender (false, _checkbox.FalseDescription);
    }

    [Test]
    public void RenderTrueDisabled ()
    {
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalseDisabled ()
    {
      CheckRender (false, _checkbox.FalseDescription);
    }

    [Test]
    public void RenderTrueWithCssClass ()
    {
      _checkbox.Stub (mock => mock.Enabled).Return (true);
      _checkbox.CssClass = c_cssClass;
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalseWithCssClass ()
    {
      _checkbox.Stub (mock => mock.Enabled).Return (true);
      _checkbox.CssClass = c_cssClass;
      CheckRender (false, _checkbox.FalseDescription);
    }

    [Test]
    public void RenderTrueReadOnlyWithCssClass ()
    {
      _checkbox.Stub (mock => mock.Enabled).Return (true);
      _checkbox.Stub (mock => mock.IsRequired).Return (true);
      _checkbox.CssClass = c_cssClass;
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalseReadOnlyWithCssClass ()
    {
      _checkbox.Stub (mock => mock.IsRequired).Return (true);
      _checkbox.CssClass = c_cssClass;
      CheckRender (false, _checkbox.FalseDescription);
    }

    [Test]
    public void RenderTrueDisabledWithCssClass ()
    {
      _checkbox.CssClass = c_cssClass;
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalseDisabledWithCssClass ()
    {
      _checkbox.CssClass = c_cssClass;
      CheckRender (false, _checkbox.FalseDescription);
    }

    [Test]
    public void RenderTrueWithCssClassInStandardProperties ()
    {
      _checkbox.Stub (mock => mock.Enabled).Return (true);
      _checkbox.Attributes["class"] = c_cssClass;
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalseWithCssClassInStandardProperties ()
    {
      _checkbox.Stub (mock => mock.Enabled).Return (true);
      _checkbox.Attributes["class"] = c_cssClass;
      CheckRender (false, _checkbox.FalseDescription);
    }

    [Test]
    public void RenderTrueReadOnlyWithCssClassInStandardProperties ()
    {
      _checkbox.Stub (mock => mock.Enabled).Return (true);
      _checkbox.Stub (mock => mock.IsRequired).Return (true);
      _checkbox.Attributes["class"] = c_cssClass;
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalseReadOnlyWithCssClassInStandardProperties ()
    {
      _checkbox.Stub (mock => mock.Enabled).Return (true);
      _checkbox.Stub (mock => mock.IsRequired).Return (true);
      _checkbox.Attributes["class"] = c_cssClass;
      CheckRender (false, _checkbox.FalseDescription);
    }

    [Test]
    public void RenderTrueDisabledWithCssClassInStandardProperties ()
    {
      _checkbox.Attributes["class"] = c_cssClass;
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalseDisabledWithCssClassInStandardProperties ()
    {
      _checkbox.Attributes["class"] = c_cssClass;
      CheckRender (false, _checkbox.FalseDescription);
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes ()
    {
      _checkbox.Stub (mock => mock.IsAutoPostBackEnabled).Return(true);
      _checkbox.Value = true;

      var resourceUrlFactory = new FakeResourceUrlFactory();
      _renderer = new BocCheckBoxRenderer (
          resourceUrlFactory,
          GlobalizationService,
          RenderingFeatures.WithDiagnosticMetadata,
          new StubLabelReferenceRenderer(),
          new StubValidationErrorRenderer());
      _renderer.Render (new BocCheckBoxRenderingContext(HttpContext, Html.Writer, _checkbox));
      
      var document = Html.GetResultDocument();
      var outerSpan = Html.GetAssertedChildElement (document, "span", 0);
      Html.AssertAttribute (outerSpan, DiagnosticMetadataAttributes.ControlType, "BocCheckBox");
      Html.AssertAttribute (outerSpan, DiagnosticMetadataAttributes.TriggersPostBack, "true");
    }

    private void CheckRender (bool value, string spanText)
    {
      _checkbox.Value = value;

      _renderer = new BocCheckBoxRenderer (
          new FakeResourceUrlFactory(),
          GlobalizationService,
          RenderingFeatures.Default,
          new StubLabelReferenceRenderer(),
          new StubValidationErrorRenderer());
      _renderer.Render (new BocCheckBoxRenderingContext(HttpContext, Html.Writer, _checkbox));

      var document = Html.GetResultDocument();

      var outerSpan = Html.GetAssertedChildElement (document, "span", 0);
      CheckCssClass (outerSpan);

      if (_checkbox.IsReadOnly)
      {
        var valueSpan = outerSpan.GetAssertedChildElement ("span", 0);
        valueSpan.AssertAttributeValueEquals ("id", c_valueName);
        valueSpan.AssertAttributeValueEquals ("data-value", value.ToString());
        valueSpan.AssertAttributeValueEquals ("tabindex", "0");
        valueSpan.AssertAttributeValueEquals ("role", "checkbox");
        valueSpan.AssertAttributeValueEquals ("aria-readonly", "true");
        Html.AssertAttribute (valueSpan, StubLabelReferenceRenderer.LabelReferenceAttribute, c_labelID);
        Html.AssertAttribute (valueSpan, StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute, "");
        Html.AssertAttribute (valueSpan, StubValidationErrorRenderer.ValidationErrorsIDAttribute, c_clientID + "_ValidationErrors");
        Html.AssertAttribute (valueSpan, StubValidationErrorRenderer.ValidationErrorsAttribute, c_validationErrors);

        CheckImage (value, valueSpan, spanText);

        AssertValidationErrors (outerSpan);
      }
      else
      {
        CheckInput (value, outerSpan);

        AssertValidationErrors (outerSpan);
      }

      if (_checkbox.IsDescriptionEnabled)
      {
        var label = Html.GetAssertedChildElement (outerSpan, "span", 1);
        Html.AssertAttribute (label, "id", c_clientID + "_Description");
        Html.AssertTextNode (label, spanText, 0);
      }
      else
      {
        Html.AssertChildElementCount (outerSpan, 1);
      }
    }

    private void AssertValidationErrors (XmlNode node)
    {
      var validationErrorsSpan = node.GetAssertedChildElement ("fake", 2);

      Html.AssertAttribute (validationErrorsSpan, StubValidationErrorRenderer.ValidationErrorsIDAttribute, c_clientID + "_ValidationErrors");
      Html.AssertAttribute (validationErrorsSpan, StubValidationErrorRenderer.ValidationErrorsAttribute, c_validationErrors);
      
    }

    private void CheckInput (bool value, XmlNode outerSpan)
    {
      var checkbox = Html.GetAssertedChildElement (outerSpan, "input", 0);
      Html.AssertAttribute (checkbox, "type", "checkbox");
      Html.AssertAttribute (checkbox, "id", c_valueName);
      Html.AssertAttribute (checkbox, "name", c_valueName);
      if (value)
        Html.AssertAttribute (checkbox, "checked", "checked");
      else
        Html.AssertNoAttribute (checkbox, "checked");

      if (_checkbox.Enabled)
        Html.AssertNoAttribute (checkbox, "disabled");
      else
        Html.AssertAttribute (checkbox, "disabled", "disabled");

      Html.AssertAttribute (checkbox, StubLabelReferenceRenderer.LabelReferenceAttribute, c_labelID);
      Html.AssertAttribute (checkbox, StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute, "");

      if (_checkbox.IsDescriptionEnabled)
        Html.AssertAttribute (checkbox, "aria-describedby", c_clientID + "_Description");

      Html.AssertAttribute (checkbox, StubValidationErrorRenderer.ValidationErrorsIDAttribute, c_clientID + "_ValidationErrors");
      Html.AssertAttribute (checkbox, StubValidationErrorRenderer.ValidationErrorsAttribute, c_validationErrors);
    }

    private void CheckImage (bool value, XmlNode outerSpan, string altText)
    {
      var image = Html.GetAssertedChildElement (outerSpan, "img", 0);
      Html.AssertNoAttribute (image, "id");
      Html.AssertAttribute (image, "src", string.Format ("/CheckBox{0}.gif", value), HtmlHelper.AttributeValueCompareMode.Contains);
      Html.AssertAttribute (image, "alt", altText);
    }

    private void CheckCssClass (XmlNode outerSpan)
    {
      string cssClass = _checkbox.CssClass;
      if (string.IsNullOrEmpty (cssClass))
        cssClass = _checkbox.Attributes["class"];
      if (string.IsNullOrEmpty (cssClass))
        cssClass = _renderer.GetCssClassBase(_checkbox);

      Html.AssertAttribute (outerSpan, "id", "MyCheckBox");
      Html.AssertAttribute (outerSpan, "class", cssClass, HtmlHelper.AttributeValueCompareMode.Contains);
      if (_checkbox.IsReadOnly)
        Html.AssertAttribute (outerSpan, "class", _renderer.CssClassReadOnly, HtmlHelper.AttributeValueCompareMode.Contains);
      if (!_checkbox.Enabled)
        Html.AssertAttribute (outerSpan, "class", _renderer.CssClassDisabled, HtmlHelper.AttributeValueCompareMode.Contains);
    }
  }
}