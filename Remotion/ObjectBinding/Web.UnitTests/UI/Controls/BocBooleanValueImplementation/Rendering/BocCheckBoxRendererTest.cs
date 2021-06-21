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
using Moq;
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

    private Mock<IBocCheckBox> _checkbox;
    private string _startupScript;
    private BocCheckBoxRenderer _renderer;

    [SetUp]
    public void SetUp ()
    {
      Initialize();
      _checkbox = new Mock<IBocCheckBox>();

      _checkbox.Setup (mock => mock.ClientID).Returns (c_clientID);
      _checkbox.Setup (mock => mock.ControlType).Returns ("BocCheckBox");
      _checkbox.Setup (mock => mock.GetValueName()).Returns (c_valueName);
      _checkbox.Setup (mock => mock.GetLabelIDs()).Returns (EnumerableUtility.Singleton (c_labelID));
      _checkbox.Setup (mock => mock.GetValidationErrors()).Returns (EnumerableUtility.Singleton (c_validationErrors));
      
      var clientScriptManagerMock = new Mock<IClientScriptManager>();
      _startupScript = string.Format (
          "BocCheckBox.InitializeGlobals ('{0}', '{1}');",
          _checkbox.Object.DefaultTrueDescription,
          _checkbox.Object.DefaultFalseDescription);
      clientScriptManagerMock.Setup (mock => mock.RegisterStartupScriptBlock (_checkbox.Object, typeof (BocCheckBoxRenderer), _startUpScriptKey, _startupScript)).Verifiable();
      clientScriptManagerMock.Setup (mock => mock.IsStartupScriptRegistered (It.IsNotNull<Type>(), It.IsNotNull<string>())).Returns (false);
      clientScriptManagerMock.Setup (mock => mock.GetPostBackEventReference (_checkbox.Object, string.Empty)).Returns (c_postbackEventReference);

      var pageStub = new Mock<IPage>();
      pageStub.Setup (stub => stub.ClientScript).Returns (clientScriptManagerMock.Object);

      _checkbox.SetupProperty (_ => _.Value);
      _checkbox.Setup (mock => mock.IsDesignMode).Returns (false);
      _checkbox.Setup (mock => mock.IsDescriptionEnabled).Returns (true);

      _checkbox.Setup (mock => mock.Page).Returns (pageStub.Object);
      _checkbox.Setup (mock => mock.TrueDescription).Returns (c_trueDescription);
      _checkbox.Setup (mock => mock.FalseDescription).Returns (c_falseDescription);

      _checkbox.SetupProperty (_ => _.CssClass);

      StateBag stateBag = new StateBag();
      _checkbox.Setup (mock => mock.Attributes).Returns (new AttributeCollection (stateBag));
      _checkbox.Setup (mock => mock.Style).Returns (_checkbox.Object.Attributes.CssStyle);
      _checkbox.Setup (mock => mock.LabelStyle).Returns (new Style (stateBag));
      _checkbox.Setup (mock => mock.ControlStyle).Returns (new Style (stateBag));
    }

    [Test]
    public void RenderTrue ()
    {
      _checkbox.Setup (mock => mock.Enabled).Returns (true);
      CheckRender (true, _checkbox.Object.TrueDescription);
    }

    [Test]
    public void RenderFalse ()
    {
      _checkbox.Setup (mock => mock.Enabled).Returns (true);
      CheckRender (false, _checkbox.Object.FalseDescription);
    }

    [Test]
    public void RenderTrueReadOnly ()
    {
      _checkbox.Setup (mock => mock.Enabled).Returns (true);
      _checkbox.Setup (mock => mock.IsRequired).Returns (true);
      _checkbox.Setup (mock => mock.IsReadOnly).Returns (true);
      CheckRender (true, _checkbox.Object.TrueDescription);
    }

    [Test]
    public void RenderFalseReadOnly ()
    {
      _checkbox.Setup (mock => mock.Enabled).Returns (true);
      _checkbox.Setup (mock => mock.IsRequired).Returns (true);
      _checkbox.Setup (mock => mock.IsReadOnly).Returns (true);
      CheckRender (false, _checkbox.Object.FalseDescription);
    }

    [Test]
    public void RenderTrueDisabled ()
    {
      CheckRender (true, _checkbox.Object.TrueDescription);
    }

    [Test]
    public void RenderFalseDisabled ()
    {
      CheckRender (false, _checkbox.Object.FalseDescription);
    }

    [Test]
    public void RenderTrueWithCssClass ()
    {
      _checkbox.Setup (mock => mock.Enabled).Returns (true);
      _checkbox.Object.CssClass = c_cssClass;
      CheckRender (true, _checkbox.Object.TrueDescription);
    }

    [Test]
    public void RenderFalseWithCssClass ()
    {
      _checkbox.Setup (mock => mock.Enabled).Returns (true);
      _checkbox.Object.CssClass = c_cssClass;
      CheckRender (false, _checkbox.Object.FalseDescription);
    }

    [Test]
    public void RenderTrueReadOnlyWithCssClass ()
    {
      _checkbox.Setup (mock => mock.Enabled).Returns (true);
      _checkbox.Setup (mock => mock.IsRequired).Returns (true);
      _checkbox.Object.CssClass = c_cssClass;
      CheckRender (true, _checkbox.Object.TrueDescription);
    }

    [Test]
    public void RenderFalseReadOnlyWithCssClass ()
    {
      _checkbox.Setup (mock => mock.IsRequired).Returns (true);
      _checkbox.Object.CssClass = c_cssClass;
      CheckRender (false, _checkbox.Object.FalseDescription);
    }

    [Test]
    public void RenderTrueDisabledWithCssClass ()
    {
      _checkbox.Object.CssClass = c_cssClass;
      CheckRender (true, _checkbox.Object.TrueDescription);
    }

    [Test]
    public void RenderFalseDisabledWithCssClass ()
    {
      _checkbox.Object.CssClass = c_cssClass;
      CheckRender (false, _checkbox.Object.FalseDescription);
    }

    [Test]
    public void RenderTrueWithCssClassInStandardProperties ()
    {
      _checkbox.Setup (mock => mock.Enabled).Returns (true);
      _checkbox.Object.Attributes["class"] = c_cssClass;
      CheckRender (true, _checkbox.Object.TrueDescription);
    }

    [Test]
    public void RenderFalseWithCssClassInStandardProperties ()
    {
      _checkbox.Setup (mock => mock.Enabled).Returns (true);
      _checkbox.Object.Attributes["class"] = c_cssClass;
      CheckRender (false, _checkbox.Object.FalseDescription);
    }

    [Test]
    public void RenderTrueReadOnlyWithCssClassInStandardProperties ()
    {
      _checkbox.Setup (mock => mock.Enabled).Returns (true);
      _checkbox.Setup (mock => mock.IsRequired).Returns (true);
      _checkbox.Object.Attributes["class"] = c_cssClass;
      CheckRender (true, _checkbox.Object.TrueDescription);
    }

    [Test]
    public void RenderFalseReadOnlyWithCssClassInStandardProperties ()
    {
      _checkbox.Setup (mock => mock.Enabled).Returns (true);
      _checkbox.Setup (mock => mock.IsRequired).Returns (true);
      _checkbox.Object.Attributes["class"] = c_cssClass;
      CheckRender (false, _checkbox.Object.FalseDescription);
    }

    [Test]
    public void RenderTrueDisabledWithCssClassInStandardProperties ()
    {
      _checkbox.Object.Attributes["class"] = c_cssClass;
      CheckRender (true, _checkbox.Object.TrueDescription);
    }

    [Test]
    public void RenderFalseDisabledWithCssClassInStandardProperties ()
    {
      _checkbox.Object.Attributes["class"] = c_cssClass;
      CheckRender (false, _checkbox.Object.FalseDescription);
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes ()
    {
      _checkbox.Setup (mock => mock.IsAutoPostBackEnabled).Returns (true);
      _checkbox.Object.Value = true;

      var resourceUrlFactory = new FakeResourceUrlFactory();
      _renderer = new BocCheckBoxRenderer (
          resourceUrlFactory,
          GlobalizationService,
          RenderingFeatures.WithDiagnosticMetadata,
          new StubLabelReferenceRenderer(),
          new StubValidationErrorRenderer());
      _renderer.Render (new BocCheckBoxRenderingContext(HttpContext, Html.Writer, _checkbox.Object));
      
      var document = Html.GetResultDocument();
      var outerSpan = Html.GetAssertedChildElement (document, "span", 0);
      Html.AssertAttribute (outerSpan, DiagnosticMetadataAttributes.ControlType, "BocCheckBox");
      Html.AssertAttribute (outerSpan, DiagnosticMetadataAttributes.TriggersPostBack, "true");
    }

    private void CheckRender (bool value, string spanText)
    {
      _checkbox.Object.Value = value;

      _renderer = new BocCheckBoxRenderer (
          new FakeResourceUrlFactory(),
          GlobalizationService,
          RenderingFeatures.Default,
          new StubLabelReferenceRenderer(),
          new StubValidationErrorRenderer());
      _renderer.Render (new BocCheckBoxRenderingContext(HttpContext, Html.Writer, _checkbox.Object));

      var document = Html.GetResultDocument();

      var outerSpan = Html.GetAssertedChildElement (document, "span", 0);
      CheckCssClass (outerSpan);

      if (_checkbox.Object.IsReadOnly)
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

      if (_checkbox.Object.IsDescriptionEnabled)
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

      if (_checkbox.Object.Enabled)
        Html.AssertNoAttribute (checkbox, "disabled");
      else
        Html.AssertAttribute (checkbox, "disabled", "disabled");

      Html.AssertAttribute (checkbox, StubLabelReferenceRenderer.LabelReferenceAttribute, c_labelID);
      Html.AssertAttribute (checkbox, StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute, "");

      if (_checkbox.Object.IsDescriptionEnabled)
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
      string cssClass = _checkbox.Object.CssClass;
      if (string.IsNullOrEmpty (cssClass))
        cssClass = _checkbox.Object.Attributes["class"];
      if (string.IsNullOrEmpty (cssClass))
        cssClass = _renderer.GetCssClassBase(_checkbox.Object);

      Html.AssertAttribute (outerSpan, "id", "MyCheckBox");
      Html.AssertAttribute (outerSpan, "class", cssClass, HtmlHelper.AttributeValueCompareMode.Contains);
      if (_checkbox.Object.IsReadOnly)
        Html.AssertAttribute (outerSpan, "class", _renderer.CssClassReadOnly, HtmlHelper.AttributeValueCompareMode.Contains);
      if (!_checkbox.Object.Enabled)
        Html.AssertAttribute (outerSpan, "class", _renderer.CssClassDisabled, HtmlHelper.AttributeValueCompareMode.Contains);
    }
  }
}