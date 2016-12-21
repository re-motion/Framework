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
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocEnumValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.Legacy.UnitTests.Domain;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation.Rendering;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Web;
using Remotion.Web.UI;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.Legacy.UnitTests.UI.Controls.BocEnumValueImplementation.Rendering
{
  [TestFixture]
  public class BocEnumValueQuirksModeRendererTest : RendererTestBase
  {
    private const string c_clientID = "MyEnumValue";
    private const string c_valueName = "ListControlClientID";
    private IBocEnumValue _enumValue;
    private readonly Unit _width = Unit.Point (173);
    private readonly Unit _height = Unit.Point (17);
    private IEnumerationValueInfo[] _enumerationInfos;
    private IResourceUrlFactory _resourceUrlFactory;

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

      _resourceUrlFactory = new FakeResourceUrlFactory();
    }

    [Test]
    public void RenderNullValue ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Stub (mock => mock.IsRequired).Return (true);

      AssertOptionList (true, null, false, false, false);
    }

    [Test]
    public void RenderFirstValue ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, false, false, false);
    }

    [Test]
    public void RenderFirstValueAutoPostback ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.ListControlStyle.AutoPostBack = true;
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, false, false, true);
    }

    [Test]
    public void RenderFirstValueWithNullOption ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Stub (mock => mock.IsRequired).Return (false);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (true, TestEnum.First, false, false, false);
    }

    [Test]
    public void RenderNullValueDisabled ()
    {
      _enumValue.Stub (mock => mock.IsRequired).Return (true);

      AssertOptionList (true, null, true, false, false);
    }

    [Test]
    public void RenderFirstValueDisabled ()
    {
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, true, false, false);
    }

    [Test]
    public void RenderFirstValueWithNullOptionDisabled ()
    {
      _enumValue.Stub (mock => mock.IsRequired).Return (false);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (true, TestEnum.First, true, false, false);
    }

    [Test]
    public void RenderNullValueReadOnly ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.Stub (mock => mock.IsReadOnly).Return (true);

      AssertLabel (null, false);
    }

    [Test]
    public void RenderFirstValueReadOnly ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.Stub (mock => mock.IsReadOnly).Return (true);
      _enumValue.Value = TestEnum.First;
      _enumValue.Stub (mock => mock.EnumerationValueInfo).Return (_enumerationInfos[0]);

      AssertLabel (TestEnum.First, false);
    }

    [Test]
    public void RenderFirstValueWithCssClass ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.CssClass = "CssClass";
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, false, false, false);
    }

    [Test]
    public void RenderFirstValueDisabledWithCssClass ()
    {
      _enumValue.CssClass = "CssClass";
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, true, false, false);
    }

    [Test]
    public void RenderFirstValueReadOnlyWithCssClass ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.CssClass = "CssClass";
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.Stub (mock => mock.IsReadOnly).Return (true);
      _enumValue.Value = TestEnum.First;
      _enumValue.Stub (mock => mock.EnumerationValueInfo).Return (_enumerationInfos[0]);

      AssertLabel (TestEnum.First, false);
    }

    [Test]
    public void RenderFirstValueWithCssClassInAttributes ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Attributes["class"] = "CssClass";
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, false, false, false);
    }

    [Test]
    public void RenderFirstValueDisabledWithCssClassInAttributes ()
    {
      _enumValue.Attributes["class"] = "CssClass";
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, true, false, false);
    }

    [Test]
    public void RenderFirstValueReadOnlyWithCssClassInAttributes ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Attributes["class"] = "CssClass";
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.Stub (mock => mock.IsReadOnly).Return (true);
      _enumValue.Value = TestEnum.First;
      _enumValue.Stub (mock => mock.EnumerationValueInfo).Return (_enumerationInfos[0]);

      AssertLabel (TestEnum.First, false);
    }

    [Test]
    public void RenderFirstValueWithStyle ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Height = _height;
      _enumValue.Width = _width;
      _enumValue.ControlStyle.Height = _height;
      _enumValue.ControlStyle.Width = _width;
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, false, true, false);
    }

    [Test]
    public void RenderFirstValueDisabledWithStyle ()
    {
      _enumValue.Height = _height;
      _enumValue.Width = _width;
      _enumValue.ControlStyle.Height = _height;
      _enumValue.ControlStyle.Width = _width;
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, true, true, false);
    }

    [Test]
    public void RenderFirstValueReadOnlyWithStyle ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Height = _height;
      _enumValue.Width = _width;
      _enumValue.ControlStyle.Height = _height;
      _enumValue.ControlStyle.Width = _width;
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.Stub (mock => mock.IsReadOnly).Return (true);
      _enumValue.Value = TestEnum.First;
      _enumValue.Stub (mock => mock.EnumerationValueInfo).Return (_enumerationInfos[0]);

      AssertLabel (TestEnum.First, true);
    }

    [Test]
    public void RenderFirstValueWithStyleInAttributes ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Style["height"] = _height.ToString();
      _enumValue.Style["width"] = _width.ToString();
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, false, true, false);
    }

    [Test]
    public void RenderFirstValueDisabledWithStyleInAttributes ()
    {
      _enumValue.Style["height"] = _height.ToString();
      _enumValue.Style["width"] = _width.ToString();
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, true, true, false);
    }

    [Test]
    public void RenderFirstValueReadOnlyWithStyleInAttributes ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Style["height"] = _height.ToString();
      _enumValue.Style["width"] = _width.ToString();
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.Stub (mock => mock.IsReadOnly).Return (true);
      _enumValue.Value = TestEnum.First;
      _enumValue.Stub (mock => mock.EnumerationValueInfo).Return (_enumerationInfos[0]);

      AssertLabel (TestEnum.First, false);
    }

    private void AssertLabel (TestEnum? value, bool withStyle)
    {
      var renderer = new BocEnumValueQuirksModeRenderer (_resourceUrlFactory);
      renderer.Render (new BocEnumValueRenderingContext(HttpContext, Html.Writer, _enumValue));

      var document = Html.GetResultDocument();
      XmlNode div = GetAssertedSpan (document, true, false, false, renderer);
      
      var span = Html.GetAssertedChildElement (div, "span", 0);
      Html.AssertAttribute (span, "id", c_valueName);
      
      if (withStyle)
      {
        Html.AssertStyleAttribute (span, "width", _width.ToString());
        Html.AssertStyleAttribute (span, "height", "100%");
      }

      Html.AssertTextNode (span, value.HasValue ? value.Value.ToString() : HtmlHelper.WhiteSpace, 0);
    }

    private XmlNode GetAssertedSpan (XmlDocument document, bool isReadOnly, bool isDisabled, bool withStyle, BocEnumValueQuirksModeRenderer renderer)
    {
      var div = Html.GetAssertedChildElement (document, "span", 0);
      string cssClass = _enumValue.CssClass;
      if (string.IsNullOrEmpty (cssClass))
        cssClass = _enumValue.Attributes["class"];
      if (string.IsNullOrEmpty (cssClass))
        cssClass = renderer.CssClassBase;

      Html.AssertAttribute (div, "id", "MyEnumValue");
      Html.AssertAttribute (div, "class", cssClass, HtmlHelperBase.AttributeValueCompareMode.Contains);
      if (isReadOnly)
        Html.AssertAttribute (div, "class", renderer.CssClassReadOnly, HtmlHelperBase.AttributeValueCompareMode.Contains);
      if (isDisabled)
        Html.AssertAttribute (div, "class", renderer.CssClassDisabled, HtmlHelperBase.AttributeValueCompareMode.Contains);

      if (withStyle)
      {
        Html.AssertStyleAttribute (div, "height", _height.ToString());
        Html.AssertStyleAttribute (div, "width", _width.ToString());
      }

      return div;
    }

    private void AssertOptionList (bool withNullValue, TestEnum? selectedValue, bool isDisabled, bool withStyle, bool autoPostBack)
    {
      var renderer = new BocEnumValueQuirksModeRenderer (_resourceUrlFactory);
      renderer.Render (new BocEnumValueRenderingContext(HttpContext, Html.Writer, _enumValue));

      var document = Html.GetResultDocument();
      var div = GetAssertedSpan (document, false, false, false, renderer);

      var select = Html.GetAssertedChildElement (div, "select", 0);
      Html.AssertAttribute (select, "id", c_valueName);
      Html.AssertAttribute (select, "name", c_valueName);

      if (withStyle)
      {
        Html.AssertStyleAttribute (select, "width", "100%");
        Html.AssertStyleAttribute (select, "height", "100%");
      }
      else
        Html.AssertStyleAttribute (select, "width", "150pt");

      if (isDisabled)
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