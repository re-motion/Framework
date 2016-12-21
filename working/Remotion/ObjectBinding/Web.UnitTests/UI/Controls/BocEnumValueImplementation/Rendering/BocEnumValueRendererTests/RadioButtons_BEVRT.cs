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
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UnitTests.Domain;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.Rendering;
using Rhino.Mocks;
using AttributeCollection = System.Web.UI.AttributeCollection;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocEnumValueImplementation.Rendering.BocEnumValueRendererTests
{
  [TestFixture]
  public class RadioButtons_BEVRT : RendererTestBase
  {
    private const string c_clientID = "MyEnumValue";
    private const string c_valueName = "ListControlClientID";
    private IBocEnumValue _enumValue;
    private readonly Unit _width = Unit.Point (173);
    private readonly Unit _height = Unit.Point (17);
    private IEnumerationValueInfo[] _enumerationInfos;

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      _enumValue = MockRepository.GenerateStub<IBocEnumValue>();
      var businessObjectProvider = BindableObjectProvider.GetProvider (typeof (BindableObjectProviderAttribute));
      var propertyInfo = PropertyInfoAdapter.Create(typeof (TypeWithEnum).GetProperty ("EnumValue"));
      IBusinessObjectEnumerationProperty property =
          new EnumerationProperty (
              new PropertyBase.Parameters (
                  (BindableObjectProvider) businessObjectProvider,
                  propertyInfo,
                  typeof (TestEnum),
                  new Lazy<Type> (() => typeof (TestEnum)),
                  null,
                  true,
                  false,
                  new BindableObjectDefaultValueStrategy(),
                  MockRepository.GenerateStub<IBindablePropertyReadAccessStrategy>(),
                  MockRepository.GenerateStub<IBindablePropertyWriteAccessStrategy>(),
                  SafeServiceLocator.Current.GetInstance<BindableObjectGlobalizationService>()));

      _enumValue.Property = property;
      _enumValue.Stub (stub => stub.ClientID).Return (c_clientID);
      _enumValue.Stub (stub => stub.ControlType).Return ("BocEnumValue");
      _enumValue.Stub (mock => mock.IsDesignMode).Return (false);

      var pageStub = MockRepository.GenerateStub<IPage>();
      pageStub.Stub (stub => stub.WrappedInstance).Return (new PageMock());
      _enumValue.Stub (stub => stub.Page).Return (pageStub);

      var values = new List<EnumerationValueInfo> (3);
      foreach (TestEnum value in Enum.GetValues (typeof (TestEnum)))
        values.Add (new EnumerationValueInfo (value, value.ToString(), value.ToString(), true));
      _enumerationInfos = values.ToArray();
      _enumValue.Stub (mock => mock.GetEnabledValues()).Return (_enumerationInfos);

      _enumValue.Stub (mock => mock.GetNullItemText()).Return ("null-text");
      _enumValue.Stub (mock => mock.NullIdentifier).Return ("null-id");
      _enumValue.Stub (mock => mock.GetValueName()).Return (c_valueName);

      StateBag stateBag = new StateBag();
      _enumValue.Stub (mock => mock.Attributes).Return (new AttributeCollection (stateBag));
      _enumValue.Stub (mock => mock.Style).Return (_enumValue.Attributes.CssStyle);
      _enumValue.Stub (mock => mock.LabelStyle).Return (new Style (stateBag));
      _enumValue.Stub (mock => mock.ListControlStyle)
          .Return (
              new ListControlStyle
              {
                  ControlType = ListControlType.RadioButtonList,
                  RadioButtonListNullValueVisible = false
              });
      _enumValue.Stub (mock => mock.ControlStyle).Return (new Style (stateBag));
      _enumValue.Stub (mock => mock.Enabled).Return (true);

      var httpContext = HttpContextHelper.CreateHttpContext ("GET", "Page.aspx", "");
      System.Web.HttpContext.Current = httpContext;
    }

    [TearDown]
    public void TearDown ()
    {
      System.Web.HttpContext.Current = null;
    }

    [Test]
    public void Render_NullValueSelected_IsNotRequired_WithRadioButtonListNullValueVisible_RendersNullValue ()
    {
      _enumValue.Stub (mock => mock.IsRequired).Return (false);
      _enumValue.ListControlStyle.RadioButtonListNullValueVisible = true;

      AssertRadioButtonList (true, null, false, false);
    }

    [Test]
    public void Render_NullValueSelected_IsNotRequired_WithoutRadioButtonListNullValueVisible_DoesNotRenderNullValue ()
    {
      _enumValue.Stub (mock => mock.IsRequired).Return (false);
      _enumValue.ListControlStyle.RadioButtonListNullValueVisible = false;

      AssertRadioButtonList (false, null, false, false);
    }

    [Test]
    public void Render_NullValueSelected_IsRequired_WithRadioButtonListNullValueVisible_DoesNotRenderNullValue ()
    {
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.ListControlStyle.RadioButtonListNullValueVisible = true;

      AssertRadioButtonList (false, null, false, false);
    }

    [Test]
    public void Render_NullValueSelected_IsRequired_WithoutRadioButtonListNullValueVisible_DoesNotRenderNullValue ()
    {
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.ListControlStyle.RadioButtonListNullValueVisible = false;

      AssertRadioButtonList (false, null, false, false);
    }

    [Test]
    public void Render_NullValueSelected_AutoPostback ()
    {
      _enumValue.Stub (mock => mock.IsRequired).Return (false);
      _enumValue.ListControlStyle.AutoPostBack = true;
      _enumValue.ListControlStyle.RadioButtonListNullValueVisible = true;

      AssertRadioButtonList (true, null, false, true);
    }

    [Test]
    public void Render_NamedValueSelected ()
    {
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.Value = TestEnum.First;

      AssertRadioButtonList (false, TestEnum.First, false, false);
    }

    [Test]
    public void Render_NamedValueSelected_AutoPostback ()
    {
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.ListControlStyle.AutoPostBack = true;
      _enumValue.Value = TestEnum.First;

      AssertRadioButtonList (false, TestEnum.First, false, true);
    }

    [Test]
    public void Render_NamedValueSelected_IsNotRequired_WithoutRadioButtonListNullValueVisible_DoesNotRenderNullValue ()
    {
      _enumValue.Stub (mock => mock.IsRequired).Return (false);
      _enumValue.ListControlStyle.RadioButtonListNullValueVisible = false;
      _enumValue.Value = TestEnum.First;

      AssertRadioButtonList (false, TestEnum.First, false, false);
    }

    [Test]
    public void Render_NamedValueSelected_IsNotRequired_WithRadioButtonListNullValueVisible_RendersNullValue ()
    {
      _enumValue.Stub (mock => mock.IsRequired).Return (false);
      _enumValue.ListControlStyle.RadioButtonListNullValueVisible = true;
      _enumValue.Value = TestEnum.First;

      AssertRadioButtonList (true, TestEnum.First, false, false);
    }

    [Test]
    public void Render_NamedValueSelected_IsRequired_WithoutRadioButtonListNullValueVisible_DoesNotRenderNullValue ()
    {
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.ListControlStyle.RadioButtonListNullValueVisible = false;
      _enumValue.Value = TestEnum.First;

      AssertRadioButtonList (false, TestEnum.First, false, false);
    }

    [Test]
    public void Render_NamedValueSelected_IsRequired_WithRadioButtonListNullValueVisible_RendersNullValue ()
    {
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.ListControlStyle.RadioButtonListNullValueVisible = true;
      _enumValue.Value = TestEnum.First;

      AssertRadioButtonList (false, TestEnum.First, false, false);
    }

    [Test]
    public void Render_NamedValueSelected_WithCssClass ()
    {
      _enumValue.CssClass = "CssClass";
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.Value = TestEnum.First;

      AssertRadioButtonList (false, TestEnum.First, false, false);
    }

    [Test]
    public void Render_NamedValueSelected_WithCssClassInAttributes ()
    {
      _enumValue.Attributes["class"] = "CssClass";
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.Value = TestEnum.First;

      AssertRadioButtonList (false, TestEnum.First, false, false);
    }

    [Test]
    public void Render_NamedValueSelected_WithStyle ()
    {
      _enumValue.Height = _height;
      _enumValue.Width = _width;
      _enumValue.ControlStyle.Height = _height;
      _enumValue.ControlStyle.Width = _width;
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.Value = TestEnum.First;

      AssertRadioButtonList (false, TestEnum.First, true, false);
    }

    [Test]
    public void Render_NamedValueSelected_WithStyleInAttributes ()
    {
      _enumValue.Style["height"] = _height.ToString();
      _enumValue.Style["width"] = _width.ToString();
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.Value = TestEnum.First;

      AssertRadioButtonList (false, TestEnum.First, true, false);
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes ()
    {
      _enumValue.ListControlStyle.AutoPostBack = true;
      
      var resourceUrlFactory = new FakeResourceUrlFactory();
      var renderer = new BocEnumValueRenderer (
          resourceUrlFactory,
          GlobalizationService,
          RenderingFeatures.WithDiagnosticMetadata);
      renderer.Render (new BocEnumValueRenderingContext(HttpContext, Html.Writer, _enumValue));
      
      var document = Html.GetResultDocument();
      var outerSpan = Html.GetAssertedChildElement (document, "div", 0);
      Html.AssertAttribute (outerSpan, DiagnosticMetadataAttributes.ControlType, "BocEnumValue");
      Html.AssertAttribute (outerSpan, DiagnosticMetadataAttributes.TriggersPostBack, "true");
      Html.AssertAttribute (outerSpan, DiagnosticMetadataAttributesForObjectBinding.BocEnumValueStyle, "RadioButtonList");
      Html.AssertAttribute (outerSpan, DiagnosticMetadataAttributesForObjectBinding.NullIdentifier, "null-id");
    }

    private XmlNode GetAssertedDiv (XmlDocument document, bool withStyle, BocEnumValueRenderer renderer)
    {
      var div = Html.GetAssertedChildElement (document, "div", 0);
      string cssClass = _enumValue.CssClass;
      if (string.IsNullOrEmpty (cssClass))
        cssClass = _enumValue.Attributes["class"];
      if (string.IsNullOrEmpty (cssClass))
        cssClass = renderer.GetCssClassBase(_enumValue);

      Html.AssertAttribute (div, "id", "MyEnumValue");
      Html.AssertAttribute (div, "class", cssClass, HtmlHelperBase.AttributeValueCompareMode.Contains);

      if (withStyle)
      {
        Html.AssertStyleAttribute (div, "height", _height.ToString());
        Html.AssertStyleAttribute (div, "width", _width.ToString());
      }

      return div;
    }

    private void AssertRadioButtonList (bool withNullValue, TestEnum? selectedValue, bool withStyle, bool autoPostBack)
    {
      var renderer = new BocEnumValueRenderer (new FakeResourceUrlFactory (), GlobalizationService, RenderingFeatures.Default);
      renderer.Render (new BocEnumValueRenderingContext (HttpContext, Html.Writer, _enumValue));

      var document = Html.GetResultDocument();
      var div = GetAssertedDiv (document, false, renderer);

      var select = Html.GetAssertedChildElement (div, "table", 0);
      Html.AssertAttribute (select, "id", c_valueName);

      if (withStyle)
        Html.AssertStyleAttribute (select, "height", "100%");

      if (withNullValue)
        AssertNullOption (select, !selectedValue.HasValue, autoPostBack);

      int index = withNullValue ? 1 : 0;
      foreach (TestEnum value in Enum.GetValues (typeof (TestEnum)))
      {
        AssertRadioButton (select, value.ToString(), value.ToString(), index, selectedValue == value, autoPostBack);
        ++index;
      }
    }

    private void AssertRadioButton (XmlNode table, string value, string text, int index, bool isSelected, bool autoPostBack)
    {
      var id = string.Format ("{0}_{1}", c_valueName, index);

      var tr = Html.GetAssertedChildElement (table, "tr", index);
      var td = Html.GetAssertedChildElement (tr, "td", 0);
      var input = Html.GetAssertedChildElement (td, "input", 0);
      Html.AssertAttribute (input, "type", "radio");
      Html.AssertAttribute (input, "value", value);
      Html.AssertAttribute (input, "id", id);
      Html.AssertAttribute (input, "name", c_valueName);

      var label = Html.GetAssertedChildElement (td, "label", 1);
      Html.AssertAttribute (label, "for", id);
      Html.AssertTextNode (label, text, 0);

      if (isSelected)
        Html.AssertAttribute (input, "checked", "checked");

      if (autoPostBack && !isSelected)
        Html.AssertAttribute (input, "onclick", string.Format ("javascript:__doPostBack('{0}','')", c_valueName + "$" + id));
    }

    private void AssertNullOption (XmlNode select, bool isSelected, bool autoPostBack)
    {
      AssertRadioButton (select, _enumValue.NullIdentifier, _enumValue.GetNullItemText(), 0, isSelected, autoPostBack);
    }
  }
}