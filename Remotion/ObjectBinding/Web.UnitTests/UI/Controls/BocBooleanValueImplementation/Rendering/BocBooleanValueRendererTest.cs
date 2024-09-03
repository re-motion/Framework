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
using System.Web.UI.WebControls;
using System.Xml;
using Moq;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.FunctionalProgramming;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Factories;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocBooleanValueImplementation.Rendering
{
  [TestFixture]
  public class BocBooleanValueRendererTest : RendererTestBase
  {
    private const string c_trueDescription = "Wahr";
    private const string c_falseDescription = "Falsch";
    private const string c_nullDescription = "Unbestimmt";
    private const string c_cssClass = "someCssClass";
    private const string c_postbackEventReference = "postbackEventReference";
    private const string c_clientID = "MyBooleanValue";
    private const string c_keyValueName = "MyBooleanValue_Value";
    private const string c_displayValueName = "MyBooleanValue_DisplayValue";
    private const string c_labelID = "Label";

    private static readonly PlainTextString s_validationErrors = PlainTextString.CreateFromText("ValidationError");

    private string _startupScript;
    private string _keyDownScript;
    private Mock<IBocBooleanValue> _booleanValue;
    private BocBooleanValueRenderer _renderer;
    private BocBooleanValueResourceSet _resourceSet;
    private CultureScope _cultureScope;

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      _resourceSet = new BocBooleanValueResourceSet(
          "ResourceKey",
          "TrueIconUrl",
          "FalseIconUrl",
          "NullIconUrl",
          "TrueHoverIconUrl",
          "FalseHoverIconUrl",
          "NullHoverIconUrl",
          PlainTextString.CreateFromText("DefaultTrueDescription"),
          PlainTextString.CreateFromText("DefaultFalseDescription"),
          PlainTextString.CreateFromText("DefaultNullDescription"));

      _booleanValue = new Mock<IBocBooleanValue>();

      var clientScriptManagerMock = new Mock<IClientScriptManager>();

      _booleanValue.Setup(mock => mock.ClientID).Returns(c_clientID);
      _booleanValue.Setup(stub => stub.ControlType).Returns("BocBooleanValue");
      _booleanValue.Setup(mock => mock.GetValueName()).Returns(c_keyValueName);
      _booleanValue.Setup(mock => mock.GetDisplayValueName()).Returns(c_displayValueName);
      _booleanValue.Setup(mock => mock.GetLabelIDs()).Returns(EnumerableUtility.Singleton(c_labelID));
      _booleanValue.Setup(mock => mock.GetValidationErrors()).Returns(EnumerableUtility.Singleton(s_validationErrors));

      string startupScriptKey = typeof(BocBooleanValueRenderer).FullName + "_Startup_" + _resourceSet.ResourceKey;
      _startupScript = string.Format(
          "BocBooleanValue.InitializeGlobals ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');",
          _resourceSet.ResourceKey,
          "true",
          "false",
          "null",
          ScriptUtility.EscapeClientScript(_resourceSet.DefaultTrueDescription),
          ScriptUtility.EscapeClientScript(_resourceSet.DefaultFalseDescription),
          ScriptUtility.EscapeClientScript(_resourceSet.DefaultNullDescription),
          _resourceSet.TrueIconUrl,
          _resourceSet.FalseIconUrl,
          _resourceSet.NullIconUrl,
          _resourceSet.TrueHoverIconUrl,
          _resourceSet.FalseHoverIconUrl,
          _resourceSet.NullHoverIconUrl);
      clientScriptManagerMock.Setup(mock => mock.RegisterStartupScriptBlock(_booleanValue.Object, typeof(BocBooleanValueRenderer), startupScriptKey, _startupScript)).Verifiable();
      clientScriptManagerMock.Setup(mock => mock.IsStartupScriptRegistered(It.IsNotNull<Type>(), It.IsNotNull<string>())).Returns(false);
      clientScriptManagerMock.Setup(mock => mock.GetPostBackEventReference(_booleanValue.Object, string.Empty)).Returns(c_postbackEventReference);

      _keyDownScript = "BocBooleanValue.OnKeyDown (this);";

      var pageStub = new Mock<IPage>();
      pageStub.Setup(stub => stub.ClientScript).Returns(clientScriptManagerMock.Object);

      _booleanValue.SetupProperty(_ => _.Value);
      _booleanValue.Setup(mock => mock.ShowDescription).Returns(true);

      _booleanValue.Setup(mock => mock.Page).Returns(pageStub.Object);
      _booleanValue.Setup(mock => mock.TrueDescription).Returns(PlainTextString.CreateFromText(c_trueDescription));
      _booleanValue.Setup(mock => mock.FalseDescription).Returns(PlainTextString.CreateFromText(c_falseDescription));
      _booleanValue.Setup(mock => mock.NullDescription).Returns(PlainTextString.CreateFromText(c_nullDescription));

      _booleanValue.SetupProperty(_ => _.CssClass);

      StateBag stateBag = new StateBag();
      _booleanValue.Setup(mock => mock.Attributes).Returns(new AttributeCollection(stateBag));
      _booleanValue.Setup(mock => mock.Style).Returns(_booleanValue.Object.Attributes.CssStyle);
      _booleanValue.Setup(mock => mock.LabelStyle).Returns(new Style(stateBag));
      _booleanValue.Setup(mock => mock.ControlStyle).Returns(new Style(stateBag));

      _booleanValue.Setup(stub => stub.GetResourceManager()).Returns(NullResourceManager.Instance);

      _booleanValue.Setup(stub => stub.CreateResourceSet()).Returns(_resourceSet);

      _cultureScope = CultureScope.CreateInvariantCultureScope();
    }

    [TearDown]
    public void TearDown ()
    {
      _cultureScope.Dispose();
    }

    [Test]
    public void RenderTrue ()
    {
      _booleanValue.Setup(mock => mock.Enabled).Returns(true);
      _booleanValue.Object.Value = true;
      CheckRendering("true", true.ToString(), "TrueIconUrl", "TrueHoverIconUrl", _booleanValue.Object.TrueDescription);
    }

    [Test]
    public void RenderFalse ()
    {
      _booleanValue.Setup(mock => mock.Enabled).Returns(true);
      _booleanValue.Object.Value = false;
      CheckRendering("false", false.ToString(), "FalseIconUrl", "FalseHoverIconUrl", _booleanValue.Object.FalseDescription);
    }

    [Test]
    public void RenderNull ()
    {
      _booleanValue.Setup(mock => mock.Enabled).Returns(true);
      _booleanValue.Object.Value = null;
      CheckRendering("mixed", "null", "NullIconUrl", "NullHoverIconUrl", _booleanValue.Object.NullDescription);
    }

    [Test]
    public void RenderTrueRequired ()
    {
      _booleanValue.Setup(mock => mock.IsRequired).Returns(true);
      _booleanValue.Setup(mock => mock.Enabled).Returns(true);
      _booleanValue.Object.Value = true;
      CheckRendering("true", true.ToString(), "TrueIconUrl", "TrueHoverIconUrl", _booleanValue.Object.TrueDescription);
    }

    [Test]
    public void RenderFalseRequired ()
    {
      _booleanValue.Setup(mock => mock.IsRequired).Returns(true);
      _booleanValue.Setup(mock => mock.Enabled).Returns(true);
      _booleanValue.Object.Value = false;
      CheckRendering("false", false.ToString(), "FalseIconUrl", "FalseHoverIconUrl", _booleanValue.Object.FalseDescription);
    }

    [Test]
    public void RenderNullRequired ()
    {
      _booleanValue.Setup(mock => mock.IsRequired).Returns(true);
      _booleanValue.Setup(mock => mock.Enabled).Returns(true);
      _booleanValue.Object.Value = null;
      CheckRendering("mixed", "null", "NullIconUrl", "NullHoverIconUrl", _booleanValue.Object.NullDescription);
    }

    [Test]
    public void RenderTrueReadOnly ()
    {
      _booleanValue.Setup(mock => mock.Enabled).Returns(true);
      _booleanValue.Object.Value = true;
      _booleanValue.Setup(mock => mock.IsReadOnly).Returns(true);
      CheckRendering("true", true.ToString(), "TrueIconUrl", "TrueHoverIconUrl", _booleanValue.Object.TrueDescription);
    }

    [Test]
    public void RenderFalseReadOnly ()
    {
      _booleanValue.Setup(mock => mock.Enabled).Returns(true);
      _booleanValue.Object.Value = false;
      _booleanValue.Setup(mock => mock.IsReadOnly).Returns(true);
      CheckRendering("false", false.ToString(), "FalseIconUrl", "FalseHoverIconUrl", _booleanValue.Object.FalseDescription);
    }

    [Test]
    public void RenderNullReadOnly ()
    {
      _booleanValue.Setup(mock => mock.Enabled).Returns(true);
      _booleanValue.Object.Value = null;
      _booleanValue.Setup(mock => mock.IsReadOnly).Returns(true);
      CheckRendering("mixed", "null", "NullIconUrl", "NullHoverIconUrl", _booleanValue.Object.NullDescription);
    }

    [Test]
    public void RenderTrueDisabled ()
    {
      _booleanValue.Setup(mock => mock.Enabled).Returns(false);
      _booleanValue.Object.Value = true;
      CheckRendering("true", true.ToString(), "TrueIconUrl", "TrueHoverIconUrl", _booleanValue.Object.TrueDescription);
    }

    [Test]
    public void RenderFalseDisabled ()
    {
      _booleanValue.Setup(mock => mock.Enabled).Returns(false);
      _booleanValue.Object.Value = false;
      CheckRendering("false", false.ToString(), "FalseIconUrl", "FalseHoverIconUrl", _booleanValue.Object.FalseDescription);
    }

    [Test]
    public void RenderNullDisabled ()
    {
      _booleanValue.Setup(mock => mock.Enabled).Returns(false);
      _booleanValue.Object.Value = null;
      CheckRendering("mixed", "null", "NullIconUrl", "NullHoverIconUrl", _booleanValue.Object.NullDescription);
    }

    [Test]
    public void RenderTrueWithCssClass ()
    {
      _booleanValue.Setup(mock => mock.Enabled).Returns(true);
      _booleanValue.Object.Value = true;
      _booleanValue.Object.CssClass = c_cssClass;
      CheckRendering("true", true.ToString(), "TrueIconUrl", "TrueHoverIconUrl", _booleanValue.Object.TrueDescription);
    }

    [Test]
    public void RenderTrueDisabledWithCssClass ()
    {
      _booleanValue.Object.Value = true;
      _booleanValue.Object.CssClass = c_cssClass;
      CheckRendering("true", true.ToString(), "TrueIconUrl", "TrueHoverIconUrl", _booleanValue.Object.TrueDescription);
    }

    [Test]
    public void RenderTrueReadonlyWithCssClass ()
    {
      _booleanValue.Setup(mock => mock.Enabled).Returns(true);
      _booleanValue.Object.Value = true;
      _booleanValue.Setup(mock => mock.IsReadOnly).Returns(true);
      _booleanValue.Object.CssClass = c_cssClass;
      CheckRendering("true", true.ToString(), "TrueIconUrl", "TrueHoverIconUrl", _booleanValue.Object.TrueDescription);
    }

    [Test]
    public void RenderTrueWithCssClassInStandardProperty ()
    {
      _booleanValue.Setup(mock => mock.Enabled).Returns(true);
      _booleanValue.Object.Value = true;
      _booleanValue.Object.Attributes["class"] = c_cssClass;
      CheckRendering("true", true.ToString(), "TrueIconUrl", "TrueHoverIconUrl", _booleanValue.Object.TrueDescription);
    }

    [Test]
    public void RenderTrueDisabledWithCssClassInStandardProperty ()
    {
      _booleanValue.Object.Value = true;
      _booleanValue.Object.Attributes["class"] = c_cssClass;
      CheckRendering("true", true.ToString(), "TrueIconUrl", "TrueHoverIconUrl", _booleanValue.Object.TrueDescription);
    }

    [Test]
    public void RenderTrueReadonlyWithCssClassInStandardProperty ()
    {
      _booleanValue.Setup(mock => mock.Enabled).Returns(true);
      _booleanValue.Object.Value = true;
      _booleanValue.Setup(mock => mock.IsReadOnly).Returns(true);
      _booleanValue.Object.Attributes["class"] = c_cssClass;
      CheckRendering("true", true.ToString(), "TrueIconUrl", "TrueHoverIconUrl", _booleanValue.Object.TrueDescription);
    }

    [Test]
    public void RenderTrueWithAutoPostback ()
    {
      _booleanValue.Setup(mock => mock.Enabled).Returns(true);
      _booleanValue.Object.Value = true;
      _booleanValue.Setup(mock => mock.IsAutoPostBackEnabled).Returns(true);
      CheckRendering("true", true.ToString(), "TrueIconUrl", "TrueHoverIconUrl", _booleanValue.Object.TrueDescription);
    }

    [Test]
    public void RenderTrueDescriptionWebString ()
    {
      _booleanValue.Setup(mock => mock.Enabled).Returns(true);
      _booleanValue.Object.Value = true;
      _booleanValue.Setup(mock => mock.TrueDescription).Returns(PlainTextString.CreateFromText("Multiline\nTrue"));

      var document = RenderBocBooleanValue();

      var label = document.GetAssertedElementByID(c_clientID + "_Description");
      Assert.That(label.InnerXml, Is.EqualTo("Multiline<br />True"));
    }

    [Test]
    public void RenderFalseDescriptionWebString ()
    {
      _booleanValue.Setup(mock => mock.Enabled).Returns(true);
      _booleanValue.Object.Value = false;
      _booleanValue.Setup(mock => mock.FalseDescription).Returns(PlainTextString.CreateFromText("Multiline\nFalse"));

      var document = RenderBocBooleanValue();

      var label = document.GetAssertedElementByID(c_clientID + "_Description");
      Assert.That(label.InnerXml, Is.EqualTo("Multiline<br />False"));
    }

    [Test]
    public void RenderNullDescriptionWebString ()
    {
      _booleanValue.Setup(mock => mock.Enabled).Returns(true);
      _booleanValue.Object.Value = null;
      _booleanValue.Setup(mock => mock.NullDescription).Returns(PlainTextString.CreateFromText("Multiline\nNull"));

      var document = RenderBocBooleanValue();

      var label = document.GetAssertedElementByID(c_clientID + "_Description");
      Assert.That(label.InnerXml, Is.EqualTo("Multiline<br />Null"));
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes ()
    {
      _booleanValue.Setup(mock => mock.IsRequired).Returns(false);
      _booleanValue.Setup(mock => mock.IsAutoPostBackEnabled).Returns(true);
      _booleanValue.Object.Value = true;

      var document = RenderBocBooleanValue();
      var outerSpan = Html.GetAssertedChildElement(document, "span", 0);
      Html.AssertAttribute(outerSpan, DiagnosticMetadataAttributes.ControlType, "BocBooleanValue");
      Html.AssertAttribute(outerSpan, DiagnosticMetadataAttributes.TriggersPostBack, "true");
      Html.AssertAttribute(outerSpan, DiagnosticMetadataAttributesForObjectBinding.BocBooleanValueIsTriState, "true");
    }

    private XmlDocument RenderBocBooleanValue ()
    {
      var resourceUrlFactory = new FakeResourceUrlFactory();
      _renderer = new BocBooleanValueRenderer(
          resourceUrlFactory,
          GlobalizationService,
          RenderingFeatures.WithDiagnosticMetadata,
          new BocBooleanValueResourceSetFactory(resourceUrlFactory),
          new StubLabelReferenceRenderer(),
          new StubValidationErrorRenderer());
      _renderer.Render(new BocBooleanValueRenderingContext(HttpContext, Html.Writer, _booleanValue.Object));

      return Html.GetResultDocument();
    }

    private void CheckRendering (string checkedState, string value, string iconUrl, string iconHoverUrl, WebString description)
    {
      var resourceUrlFactory = new FakeResourceUrlFactory();
      _renderer = new BocBooleanValueRenderer(
          resourceUrlFactory,
          GlobalizationService,
          RenderingFeatures.Default,
          new BocBooleanValueResourceSetFactory(resourceUrlFactory),
          new StubLabelReferenceRenderer(),
          new StubValidationErrorRenderer());
      _renderer.Render(new BocBooleanValueRenderingContext(HttpContext, Html.Writer, _booleanValue.Object));
      var document = Html.GetResultDocument();
      var outerSpan = Html.GetAssertedChildElement(document, "span", 0);
      CheckOuterSpanAttributes(outerSpan);

      if (!_booleanValue.Object.IsReadOnly)
        CheckHiddenField(outerSpan, value);
      else
        CheckDataValueField(outerSpan, value);
      Html.AssertChildElementCount(outerSpan, 5);

      var checkbox = Html.GetAssertedChildElement(outerSpan, "span", 1);
      CheckCheckboxAttributes(checkbox, checkedState, _booleanValue.Object.IsReadOnly, _booleanValue.Object.IsRequired);

      var image = Html.GetAssertedChildElement(checkbox, "img", 0);
      checkImageAttributes(image, iconUrl, iconHoverUrl);

      var requiredLabel = Html.GetAssertedChildElement(outerSpan, "span", 2);
      Html.AssertAttribute(requiredLabel, "hidden", "hidden");
      if (!_booleanValue.Object.IsReadOnly && _booleanValue.Object.IsRequired)
      {
        Html.AssertAttribute(requiredLabel, "id", c_clientID + "_Required");
        Html.AssertTextNode(requiredLabel, "required", 0);
      }

      var label = Html.GetAssertedChildElement(outerSpan, "span", 3);
      Html.AssertChildElementCount(label, 0);
      Html.AssertAttribute(label, "id", c_clientID + "_Description");
      Html.AssertAttribute(label, "class", "description");
      Html.AssertTextNode(label, description.ToString(WebStringEncoding.HtmlWithTransformedLineBreaks), 0);
      var isDescriptionVisible = (_booleanValue.Object.IsReadOnly && _booleanValue.Object.Value.HasValue)
                                 || (!_booleanValue.Object.IsReadOnly && _booleanValue.Object.ShowDescription);
      if (isDescriptionVisible)
      {
        Html.AssertNoAttribute(label, "hidden");
        Html.AssertAttribute(label, "aria-hidden", "true");
      }
      else
      {
        Html.AssertAttribute(label, "hidden", "hidden");
        Html.AssertNoAttribute(label, "aria-hidden");
      }

      if (!_booleanValue.Object.ShowDescription && !_booleanValue.Object.IsReadOnly)
        Html.AssertAttribute(checkbox, "title", description.GetValue());
      else
        Html.AssertNoAttribute(checkbox, "title");

      if (!_booleanValue.Object.IsReadOnly && _booleanValue.Object.Enabled)
      {
        Html.AssertAttribute(outerSpan, "onclick", GetClickScript(_booleanValue.Object.IsRequired, _booleanValue.Object.IsAutoPostBackEnabled));
      }
      else
      {
        Html.AssertNoAttribute(outerSpan, "onclick");
      }

      AssertValidationErrors(Html.GetAssertedChildElement(outerSpan, "fake", 4));
    }

    private void CheckCssClass (XmlNode outerSpan)
    {
      string cssClass = _booleanValue.Object.CssClass;
      if (string.IsNullOrEmpty(cssClass))
        cssClass = _booleanValue.Object.Attributes["class"];
      if (string.IsNullOrEmpty(cssClass))
        cssClass = _renderer.GetCssClassBase(_booleanValue.Object);
      Html.AssertAttribute(outerSpan, "class", cssClass, HtmlHelper.AttributeValueCompareMode.Contains);
    }

    private void CheckOuterSpanAttributes (XmlNode outerSpan)
    {
      CheckCssClass(outerSpan);

      Html.AssertAttribute(outerSpan, "id", "MyBooleanValue");

      if (!_booleanValue.Object.Enabled)
        Html.AssertAttribute(outerSpan, "class", _renderer.CssClassDisabled, HtmlHelper.AttributeValueCompareMode.Contains);
      if (_booleanValue.Object.IsReadOnly)
        Html.AssertAttribute(outerSpan, "class", _renderer.CssClassReadOnly, HtmlHelper.AttributeValueCompareMode.Contains);
    }

    private void checkImageAttributes (XmlNode image, string iconUrl, string iconHoverUrl)
    {
      Html.AssertAttribute(image, "src", "/fake/Remotion.Development.Web/Image/Spacer.gif");
      Html.AssertStyleAttribute(image, "--standard-background-image", "url('" + iconUrl + "')");
      Html.AssertStyleAttribute(image, "--hover-background-image", "url('" + iconHoverUrl + "')");
      Html.AssertAttribute(image, "alt", "");
    }

    private void CheckCheckboxAttributes (XmlNode checkbox, string checkedState, bool isReadOnly, bool isRequired)
    {
      Html.AssertAttribute(checkbox, "id", c_displayValueName);
      Html.AssertAttribute(checkbox, StubLabelReferenceRenderer.LabelReferenceAttribute, c_labelID);
      if (isRequired && !isReadOnly)
        Html.AssertAttribute(checkbox, StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute, c_clientID + "_Required");
      else
        Html.AssertAttribute(checkbox, StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute, "");

      AssertValidationErrors(checkbox);

      Html.AssertAttribute(checkbox, "role", "checkbox");
      Html.AssertAttribute(checkbox, "aria-checked", checkedState);
      if (isReadOnly)
      {
        Html.AssertAttribute(checkbox, "class", "screenReaderText");
        Html.AssertAttribute(checkbox, "aria-readonly", "true");
        Html.AssertAttribute(checkbox, "aria-roledescription", "read only checkbox");
      }

      if (!isReadOnly && _booleanValue.Object.Enabled)
      {
        Html.AssertAttribute(checkbox, "onkeydown", _keyDownScript);
      }
      else
      {
        Html.AssertNoAttribute(checkbox, "onkeydown");
      }

      if (_booleanValue.Object.Enabled)
      {
        Html.AssertAttribute(checkbox, "tabindex", "0");
        Html.AssertNoAttribute(checkbox, "aria-disabled");
      }
      else
      {
        Html.AssertNoAttribute(checkbox, "tabindex");
        Html.AssertAttribute(checkbox, "aria-disabled", "true");
      }

      if (_booleanValue.Object.ShowDescription)
      {
        Html.AssertAttribute(checkbox, "aria-describedby", c_clientID + "_Description");
      }
      else
      {
        Html.AssertNoAttribute(checkbox, "aria-describedby");
      }
    }

    private void AssertValidationErrors (XmlNode node)
    {
      Html.AssertAttribute(node, StubValidationErrorRenderer.ValidationErrorsIDAttribute, c_clientID + "_ValidationErrors");
      Html.AssertAttribute(node, StubValidationErrorRenderer.ValidationErrorsAttribute, s_validationErrors);
    }

    private void CheckHiddenField (XmlNode outerSpan, string value)
    {
      var hiddenField = Html.GetAssertedChildElement(outerSpan, "input", 0);
      Html.AssertAttribute(hiddenField, "type", "text");
      Html.AssertAttribute(hiddenField, "id", c_keyValueName);
      Html.AssertAttribute(hiddenField, "name", c_keyValueName);
      Html.AssertAttribute(hiddenField, "value", value);
      Html.AssertAttribute(hiddenField, "hidden", "hidden");
    }

    private void CheckDataValueField (XmlNode outerSpan, string value)
    {
      var dataValueField = Html.GetAssertedChildElement(outerSpan, "span", 0);
      Html.AssertAttribute(dataValueField, "id", c_keyValueName);
      if(value!="null")
        Html.AssertAttribute(dataValueField, "data-value", value);
    }

    private string GetClickScript (bool isRequired, bool isAutoPostbackEnabled)
    {
      return "BocBooleanValue.SelectNextCheckboxValue ('ResourceKey', this.querySelector(':scope > span[role=checkbox]'), "
             + "this.querySelector(':scope > span[role=checkbox] > img'), this.querySelector(':scope > span.description'), "
             + "this.querySelector(':scope > input'), " + isRequired.ToString().ToLower() + ", "
             + "'" + c_trueDescription + "', '" + c_falseDescription + "', '" + c_nullDescription + "');"
             + (isAutoPostbackEnabled ? c_postbackEventReference + ";" : "")
             + "return false;";
    }
  }
}
