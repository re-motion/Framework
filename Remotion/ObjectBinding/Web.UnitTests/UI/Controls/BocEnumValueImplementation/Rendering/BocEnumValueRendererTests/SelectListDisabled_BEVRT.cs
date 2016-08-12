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
  public class SelectList_BEVRT : RendererTestBase
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

      _enumValue.Stub (mock => mock.GetNullItemText()).Return ("null");
      _enumValue.Stub (mock => mock.GetValueName()).Return (c_valueName);

      StateBag stateBag = new StateBag();
      _enumValue.Stub (mock => mock.Attributes).Return (new AttributeCollection (stateBag));
      _enumValue.Stub (mock => mock.Style).Return (_enumValue.Attributes.CssStyle);
      _enumValue.Stub (mock => mock.LabelStyle).Return (new Style (stateBag));
      _enumValue.Stub (mock => mock.ListControlStyle).Return (new ListControlStyle());
      _enumValue.Stub (mock => mock.ControlStyle).Return (new Style (stateBag));
      _enumValue.Stub (mock => mock.Enabled).Return (false);
    }

    [Test]
    public void Render_NullValue ()
    {
      _enumValue.Stub (mock => mock.IsRequired).Return (true);

      AssertOptionList (true, null, false, false);
    }

    [Test]
    public void Render_NamedValue ()
    {
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, false, false);
    }

    [Test]
    public void Render_NamedValue_WithCssClass ()
    {
      _enumValue.CssClass = "CssClass";
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, false, false);
    }

    [Test]
    public void Render_NamedValue_WithCssClassInAttributes ()
    {
      _enumValue.Attributes["class"] = "CssClass";
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, false, false);
    }

    [Test]
    public void Render_NamedValue_WithStyle ()
    {
      _enumValue.Height = _height;
      _enumValue.Width = _width;
      _enumValue.ControlStyle.Height = _height;
      _enumValue.ControlStyle.Width = _width;
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, true, false);
    }

    [Test]
    public void Render_NamedValue_WithStyleInAttributes ()
    {
      _enumValue.Style["height"] = _height.ToString();
      _enumValue.Style["width"] = _width.ToString();
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, true, false);
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes ()
    {
      _enumValue.ListControlStyle.ControlType = ListControlType.ListBox;
      _enumValue.ListControlStyle.AutoPostBack = true;
      
      var resourceUrlFactory = new FakeResourceUrlFactory();
      var renderer = new BocEnumValueRenderer (
          resourceUrlFactory,
          GlobalizationService,
          RenderingFeatures.WithDiagnosticMetadata);
      renderer.Render (new BocEnumValueRenderingContext(HttpContext, Html.Writer, _enumValue));
      
      var document = Html.GetResultDocument();
      var outerSpan = Html.GetAssertedChildElement (document, "span", 0);
      Html.AssertAttribute (outerSpan, DiagnosticMetadataAttributes.ControlType, "BocEnumValue");
      Html.AssertAttribute (outerSpan, DiagnosticMetadataAttributes.TriggersPostBack, "true");
      Html.AssertAttribute (outerSpan, DiagnosticMetadataAttributesForObjectBinding.BocEnumValueStyle, "ListBox");
    }

    private XmlNode GetAssertedSpan (XmlDocument document, bool isReadOnly, bool withStyle, BocEnumValueRenderer renderer)
    {
      var div = Html.GetAssertedChildElement (document, "span", 0);
      string cssClass = _enumValue.CssClass;
      if (string.IsNullOrEmpty (cssClass))
        cssClass = _enumValue.Attributes["class"];
      if (string.IsNullOrEmpty (cssClass))
        cssClass = renderer.GetCssClassBase(_enumValue);

      Html.AssertAttribute (div, "id", "MyEnumValue");
      Html.AssertAttribute (div, "class", cssClass, HtmlHelperBase.AttributeValueCompareMode.Contains);
      if (isReadOnly)
        Html.AssertAttribute (div, "class", renderer.CssClassReadOnly, HtmlHelperBase.AttributeValueCompareMode.Contains);

      Html.AssertAttribute (div, "class", renderer.CssClassDisabled, HtmlHelperBase.AttributeValueCompareMode.Contains);

      if (withStyle)
      {
        Html.AssertStyleAttribute (div, "height", _height.ToString());
        Html.AssertStyleAttribute (div, "width", _width.ToString());
      }

      return div;
    }

    private void AssertOptionList (bool withNullValue, TestEnum? selectedValue, bool withStyle, bool autoPostBack)
    {
      var renderer = new BocEnumValueRenderer (new FakeResourceUrlFactory (), GlobalizationService, RenderingFeatures.Default);
      renderer.Render (new BocEnumValueRenderingContext (HttpContext, Html.Writer, _enumValue));

      var document = Html.GetResultDocument();
      var div = GetAssertedSpan (document, false, false, renderer);

      var select = Html.GetAssertedChildElement (div, "select", 0);
      Html.AssertAttribute (select, "id", c_valueName);
      Html.AssertAttribute (select, "name", c_valueName);

      if (withStyle)
        Html.AssertStyleAttribute (select, "height", "100%");

      Html.AssertAttribute (select, "disabled", "disabled");

      if (withNullValue)
        AssertNullOption (select, !selectedValue.HasValue);

      if (autoPostBack)
        Html.AssertAttribute (select, "onchange", string.Format ("javascript:__doPostBack('{0}','')", c_valueName));

      int index = withNullValue ? 1 : 0;
      foreach (TestEnum value in Enum.GetValues (typeof (TestEnum)))
      {
        AssertOption (select, value.ToString(), value.ToString(), index, selectedValue == value);
        ++index;
      }
    }

    private void AssertOption (XmlNode select, string value, string text, int index, bool isSelected)
    {
      var option = Html.GetAssertedChildElement (select, "option", index);
      Html.AssertAttribute (option, "value", value);

      if (!string.IsNullOrEmpty (text))
        Html.AssertTextNode (option, text, 0);

      if (isSelected)
        Html.AssertAttribute (option, "selected", "selected");
    }

    private void AssertNullOption (XmlNode select, bool isSelected)
    {
      AssertOption (select, _enumValue.GetNullItemText(), "", 0, isSelected);
    }
  }
}