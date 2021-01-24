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
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.Utilities;
using Rhino.Mocks;

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
    private const string _dummyScript = "return false;";
    private const string c_clientID = "MyBooleanValue";
    private const string c_keyValueName = "MyBooleanValue_Value";
    private const string c_displayValueName = "MyBooleanValue_DisplayValue";
    private const string c_labelID = "Label";
    private const string c_validationErrors = "ValidationError";

    private string _startupScript;
    private string _keyDownScript;
    private IBocBooleanValue _booleanValue;
    private BocBooleanValueRenderer _renderer;
    private BocBooleanValueResourceSet _resourceSet;
    private CultureScope _cultureScope;

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      _resourceSet = new BocBooleanValueResourceSet (
          "ResourceKey",
          "TrueIconUrl",
          "FalseIconUrl",
          "NullIconUrl",
          "DefaultTrueDescription",
          "DefaultFalseDescription",
          "DefaultNullDescription"
          );

      _booleanValue = MockRepository.GenerateMock<IBocBooleanValue>();

      var clientScriptManagerMock = MockRepository.GenerateMock<IClientScriptManager>();

      _booleanValue.Stub (mock => mock.ClientID).Return (c_clientID);
      _booleanValue.Stub (stub => stub.ControlType).Return ("BocBooleanValue");
      _booleanValue.Stub (mock => mock.GetValueName ()).Return (c_keyValueName);
      _booleanValue.Stub (mock => mock.GetDisplayValueName()).Return (c_displayValueName);
      _booleanValue.Stub (mock => mock.GetLabelIDs()).Return (EnumerableUtility.Singleton (c_labelID));
      _booleanValue.Stub (mock => mock.GetValidationErrors()).Return (EnumerableUtility.Singleton (c_validationErrors));
      
      string startupScriptKey = typeof (BocBooleanValueRenderer).FullName + "_Startup_" + _resourceSet.ResourceKey;
      _startupScript = string.Format (
          "BocBooleanValue_InitializeGlobals ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');",
          _resourceSet.ResourceKey,
          "true",
          "false",
          "null",
          ScriptUtility.EscapeClientScript (_resourceSet.DefaultTrueDescription),
          ScriptUtility.EscapeClientScript (_resourceSet.DefaultFalseDescription),
          ScriptUtility.EscapeClientScript (_resourceSet.DefaultNullDescription),
          _resourceSet.TrueIconUrl,
          _resourceSet.FalseIconUrl,
          _resourceSet.NullIconUrl);
      clientScriptManagerMock.Expect (mock => mock.RegisterStartupScriptBlock (_booleanValue, typeof (BocBooleanValueRenderer), startupScriptKey, _startupScript));
      clientScriptManagerMock.Stub (mock => mock.IsStartupScriptRegistered (Arg<Type>.Is.NotNull, Arg<string>.Is.NotNull)).Return (false);
      clientScriptManagerMock.Stub (mock => mock.GetPostBackEventReference (_booleanValue, string.Empty)).Return (c_postbackEventReference);


      _keyDownScript = "BocBooleanValue_OnKeyDown (this);";

      var pageStub = MockRepository.GenerateStub<IPage>();
      pageStub.Stub (stub => stub.ClientScript).Return (clientScriptManagerMock);

      _booleanValue.Stub (mock => mock.Value).PropertyBehavior();
      _booleanValue.Stub (mock => mock.IsDesignMode).Return (false);
      _booleanValue.Stub (mock => mock.ShowDescription).Return (true);

      _booleanValue.Stub (mock => mock.Page).Return (pageStub);
      _booleanValue.Stub (mock => mock.TrueDescription).Return (c_trueDescription);
      _booleanValue.Stub (mock => mock.FalseDescription).Return (c_falseDescription);
      _booleanValue.Stub (mock => mock.NullDescription).Return (c_nullDescription);

      _booleanValue.Stub (mock => mock.CssClass).PropertyBehavior();

      StateBag stateBag = new StateBag();
      _booleanValue.Stub (mock => mock.Attributes).Return (new AttributeCollection (stateBag));
      _booleanValue.Stub (mock => mock.Style).Return (_booleanValue.Attributes.CssStyle);
      _booleanValue.Stub (mock => mock.LabelStyle).Return (new Style (stateBag));
      _booleanValue.Stub (mock => mock.ControlStyle).Return (new Style (stateBag));

      _booleanValue.Stub (stub => stub.GetResourceManager()).Return (NullResourceManager.Instance);

      _booleanValue.Stub (stub => stub.CreateResourceSet ()).Return (_resourceSet);

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
      _booleanValue.Stub (mock => mock.Enabled).Return (true);
      _booleanValue.Value = true;
      CheckRendering ("true", true.ToString(), "TrueIconUrl", _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderFalse ()
    {
      _booleanValue.Stub (mock => mock.Enabled).Return (true);
      _booleanValue.Value = false;
      CheckRendering ("false", false.ToString(), "FalseIconUrl", _booleanValue.FalseDescription);
    }

    [Test]
    public void RenderNull ()
    {
      _booleanValue.Stub (mock => mock.Enabled).Return (true);
      _booleanValue.Value = null;
      CheckRendering ("mixed", "null", "NullIconUrl", _booleanValue.NullDescription);
    }

    [Test]
    public void RenderTrueRequired ()
    {
      _booleanValue.Stub (mock => mock.IsRequired).Return (true);
      _booleanValue.Stub (mock => mock.Enabled).Return (true);
      _booleanValue.Value = true;
      CheckRendering ("true", true.ToString(), "TrueIconUrl", _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderFalseRequired ()
    {
      _booleanValue.Stub (mock => mock.IsRequired).Return (true);
      _booleanValue.Stub (mock => mock.Enabled).Return (true);
      _booleanValue.Value = false;
      CheckRendering ("false", false.ToString(), "FalseIconUrl", _booleanValue.FalseDescription);
    }

    [Test]
    public void RenderNullRequired ()
    {
      _booleanValue.Stub (mock => mock.IsRequired).Return (true);
      _booleanValue.Stub (mock => mock.Enabled).Return (true);
      _booleanValue.Value = null;
      CheckRendering ("mixed", "null", "NullIconUrl", _booleanValue.NullDescription);
    }

    [Test]
    public void RenderTrueReadOnly ()
    {
      _booleanValue.Stub (mock => mock.Enabled).Return (true);
      _booleanValue.Value = true;
      _booleanValue.Stub (mock => mock.IsReadOnly).Return (true);
      CheckRendering ("true", true.ToString(), "TrueIconUrl", _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderFalseReadOnly ()
    {
      _booleanValue.Stub (mock => mock.Enabled).Return (true);
      _booleanValue.Value = false;
      _booleanValue.Stub (mock => mock.IsReadOnly).Return (true);
      CheckRendering ("false", false.ToString(), "FalseIconUrl", _booleanValue.FalseDescription);
    }

    [Test]
    public void RenderNullReadOnly ()
    {
      _booleanValue.Stub (mock => mock.Enabled).Return (true);
      _booleanValue.Value = null;
      _booleanValue.Stub (mock => mock.IsReadOnly).Return (true);
      CheckRendering ("mixed", "null", "NullIconUrl", _booleanValue.NullDescription);
    }

    [Test]
    public void RenderTrueDisabled ()
    {
      _booleanValue.Value = true;
      CheckRendering ("true", true.ToString(), "TrueIconUrl", _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderFalseDisabled ()
    {
      _booleanValue.Value = false;
      CheckRendering ("false", false.ToString(), "FalseIconUrl", _booleanValue.FalseDescription);
    }

    [Test]
    public void RenderNullDisabled ()
    {
      _booleanValue.Value = null;
      CheckRendering ("mixed", "null", "NullIconUrl", _booleanValue.NullDescription);
    }

    [Test]
    public void RenderTrueWithCssClass ()
    {
      _booleanValue.Stub (mock => mock.Enabled).Return (true);
      _booleanValue.Value = true;
      _booleanValue.CssClass = c_cssClass;
      CheckRendering ("true", true.ToString(), "TrueIconUrl", _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderTrueDisabledWithCssClass ()
    {
      _booleanValue.Value = true;
      _booleanValue.CssClass = c_cssClass;
      CheckRendering ("true", true.ToString(), "TrueIconUrl", _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderTrueReadonlyWithCssClass ()
    {
      _booleanValue.Stub (mock => mock.Enabled).Return (true);
      _booleanValue.Value = true;
      _booleanValue.Stub (mock => mock.IsReadOnly).Return (true);
      _booleanValue.CssClass = c_cssClass;
      CheckRendering ("true", true.ToString(), "TrueIconUrl", _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderTrueWithCssClassInStandardProperty ()
    {
      _booleanValue.Stub (mock => mock.Enabled).Return (true);
      _booleanValue.Value = true;
      _booleanValue.Attributes["class"] = c_cssClass;
      CheckRendering ("true", true.ToString(), "TrueIconUrl", _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderTrueDisabledWithCssClassInStandardProperty ()
    {
      _booleanValue.Value = true;
      _booleanValue.Attributes["class"] = c_cssClass;
      CheckRendering ("true", true.ToString(), "TrueIconUrl", _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderTrueReadonlyWithCssClassInStandardProperty ()
    {
      _booleanValue.Stub (mock => mock.Enabled).Return (true);
      _booleanValue.Value = true;
      _booleanValue.Stub (mock => mock.IsReadOnly).Return (true);
      _booleanValue.Attributes["class"] = c_cssClass;
      CheckRendering ("true", true.ToString(), "TrueIconUrl", _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderTrueWithAutoPostback ()
    {
      _booleanValue.Stub (mock => mock.Enabled).Return (true);
      _booleanValue.Value = true;
      _booleanValue.Stub (mock => mock.IsAutoPostBackEnabled).Return (true);
      CheckRendering ("true", true.ToString(), "TrueIconUrl", _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes ()
    {
      _booleanValue.Stub (mock => mock.IsRequired).Return (false);
      _booleanValue.Stub (mock => mock.IsAutoPostBackEnabled).Return(true);
      _booleanValue.Value = true;

      var resourceUrlFactory = new FakeResourceUrlFactory();
      _renderer = new BocBooleanValueRenderer (
          resourceUrlFactory,
          GlobalizationService,
          RenderingFeatures.WithDiagnosticMetadata,
          new BocBooleanValueResourceSetFactory (resourceUrlFactory),
          new StubLabelReferenceRenderer(),
          new StubValidationErrorRenderer());
      _renderer.Render (new BocBooleanValueRenderingContext(HttpContext, Html.Writer, _booleanValue));
      
      var document = Html.GetResultDocument();
      var outerSpan = Html.GetAssertedChildElement (document, "span", 0);
      Html.AssertAttribute (outerSpan, DiagnosticMetadataAttributes.ControlType, "BocBooleanValue");
      Html.AssertAttribute (outerSpan, DiagnosticMetadataAttributes.TriggersPostBack, "true");
      Html.AssertAttribute (outerSpan, DiagnosticMetadataAttributesForObjectBinding.BocBooleanValueIsTriState, "true");
    }

    private void CheckRendering (string checkedState, string value, string iconUrl, string description)
    {
      var resourceUrlFactory = new FakeResourceUrlFactory();
      _renderer = new BocBooleanValueRenderer (
          resourceUrlFactory,
          GlobalizationService,
          RenderingFeatures.Default,
          new BocBooleanValueResourceSetFactory (resourceUrlFactory),
          new StubLabelReferenceRenderer(),
          new StubValidationErrorRenderer());
      _renderer.Render (new BocBooleanValueRenderingContext(HttpContext, Html.Writer, _booleanValue));
      var document = Html.GetResultDocument();
      var outerSpan = Html.GetAssertedChildElement (document, "span", 0);
      CheckOuterSpanAttributes (outerSpan);

      if (!_booleanValue.IsReadOnly)
        CheckHiddenField (outerSpan, value);
      else
        CheckDataValueField (outerSpan, value);
      Html.AssertChildElementCount (outerSpan, 5);

      var link = Html.GetAssertedChildElement (outerSpan, "a", 1);
      CheckLinkAttributes (link, checkedState, null, _booleanValue.IsReadOnly, _booleanValue.IsRequired);

      var image = Html.GetAssertedChildElement (link, "img", 0);
      checkImageAttributes (image, iconUrl);

      var requiredLabel = Html.GetAssertedChildElement (outerSpan, "span", 2);
      Html.AssertAttribute (requiredLabel, "hidden", "hidden");
      if (!_booleanValue.IsReadOnly && _booleanValue.IsRequired)
      {
        Html.AssertAttribute (requiredLabel, "id", c_clientID + "_Required");
        Html.AssertTextNode (requiredLabel, "required", 0);
      }

      var label = Html.GetAssertedChildElement (outerSpan, "span", 3);
      Html.AssertChildElementCount (label, 0);
      Html.AssertAttribute (label, "id", c_clientID + "_Description");
      Html.AssertTextNode (label, description, 0);
      if (_booleanValue.ShowDescription)
        Html.AssertNoAttribute (label, "hidden");
      else
        Html.AssertAttribute (label, "hidden", "hidden");

      if (!_booleanValue.IsReadOnly)
      {
        var clickScript = _booleanValue.Enabled ? GetClickScript (_booleanValue.IsRequired, _booleanValue.IsAutoPostBackEnabled) : _dummyScript;
        Html.AssertAttribute (label, "onclick", clickScript);
      }

      AssertValidationErrors (Html.GetAssertedChildElement (outerSpan, "fake", 4));
    }

    private void CheckCssClass (XmlNode outerSpan)
    {
      string cssClass = _booleanValue.CssClass;
      if (string.IsNullOrEmpty (cssClass))
        cssClass = _booleanValue.Attributes["class"];
      if (string.IsNullOrEmpty (cssClass))
        cssClass = _renderer.GetCssClassBase(_booleanValue);
      Html.AssertAttribute (outerSpan, "class", cssClass, HtmlHelper.AttributeValueCompareMode.Contains);
    }

    private void CheckOuterSpanAttributes (XmlNode outerSpan)
    {
      CheckCssClass (outerSpan);

      Html.AssertAttribute (outerSpan, "id", "MyBooleanValue");

      if (!_booleanValue.Enabled)
        Html.AssertAttribute (outerSpan, "class", _renderer.CssClassDisabled, HtmlHelper.AttributeValueCompareMode.Contains);
      if (_booleanValue.IsReadOnly)
        Html.AssertAttribute (outerSpan, "class", _renderer.CssClassReadOnly, HtmlHelper.AttributeValueCompareMode.Contains);
    }

    private void checkImageAttributes (XmlNode image, string iconUrl)
    {
      Html.AssertAttribute (image, "src", iconUrl);
      Html.AssertAttribute (image, "alt", "");
    }

    private void CheckLinkAttributes (XmlNode link, string checkedState, string description, bool isReadOnly, bool isRequired)
    {
      Html.AssertAttribute (link, "id", c_displayValueName);
      Html.AssertAttribute (link, StubLabelReferenceRenderer.LabelReferenceAttribute, c_labelID);
      if (isRequired && !isReadOnly)
        Html.AssertAttribute (link, StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute, c_clientID + "_Required");
      else
        Html.AssertAttribute (link, StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute, "");
      Html.AssertAttribute (link, "aria-describedby", c_clientID + "_Description");

      AssertValidationErrors (link);

      Html.AssertAttribute (link, "role", "checkbox");
      Html.AssertAttribute (link, "aria-checked", checkedState);
      if (isReadOnly)
        Html.AssertAttribute (link, "aria-readonly", "true");
      Html.AssertAttribute (link, "href", "#");
      if (!isReadOnly)
      {
        Html.AssertAttribute (link, "onclick", _booleanValue.Enabled ? GetClickScript(isRequired, _booleanValue.IsAutoPostBackEnabled) : _dummyScript);
        Html.AssertAttribute (link, "onkeydown", _keyDownScript);
      }
      if (description == null)
        Html.AssertNoAttribute (link, "title");
      else
        Html.AssertAttribute (link, "title", description);
    }

    private void AssertValidationErrors (XmlNode node)
    {
      Html.AssertAttribute (node, StubValidationErrorRenderer.ValidationErrorsIDAttribute, c_clientID + "_ValidationErrors");
      Html.AssertAttribute (node, StubValidationErrorRenderer.ValidationErrorsAttribute, c_validationErrors);
    }

    private void CheckHiddenField (XmlNode outerSpan, string value)
    {
      var hiddenField = Html.GetAssertedChildElement (outerSpan, "input", 0);
      Html.AssertAttribute (hiddenField, "type", "hidden");
      Html.AssertAttribute (hiddenField, "id", c_keyValueName);
      Html.AssertAttribute (hiddenField, "name", c_keyValueName);
      Html.AssertAttribute (hiddenField, "value", value);
    }

    private void CheckDataValueField (XmlNode outerSpan, string value)
    {
      var dataValueField = Html.GetAssertedChildElement (outerSpan, "span", 0);
      Html.AssertAttribute (dataValueField, "id", c_keyValueName);
      if(value!="null")
        Html.AssertAttribute (dataValueField, "data-value", value);
    }

    private string GetClickScript (bool isRequired, bool isAutoPostbackEnabled)
    {
      return "BocBooleanValue_SelectNextCheckboxValue ('ResourceKey', $(this).parent().children('a')[0], "
             + "$(this).parent().children('a').children('img').first()[0], $(this).parent().children('span').first()[0], "
             + "$(this).parent().children('input').first()[0], " + isRequired.ToString().ToLower() + ", "
             + "'" + c_trueDescription + "', '" + c_falseDescription + "', '" + c_nullDescription + "');"
             + (isAutoPostbackEnabled ? c_postbackEventReference + ";" : "")
             + "return false;";
    }
  }
}