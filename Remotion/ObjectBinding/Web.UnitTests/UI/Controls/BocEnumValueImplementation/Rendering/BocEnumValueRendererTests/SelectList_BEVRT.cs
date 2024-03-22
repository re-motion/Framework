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
using Moq;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.FunctionalProgramming;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.BusinessObjectPropertyConstraints;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UnitTests.Domain;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Web;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.Utilities;
using AttributeCollection = System.Web.UI.AttributeCollection;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocEnumValueImplementation.Rendering.BocEnumValueRendererTests
{
  [TestFixture]
  public class SelectList_BEVRT : RendererTestBase
  {
    private enum NullOption
    {
      WithoutNullValue,
      HasNullValueWithScreenReaderOnlyText,
      HasNullValueWithVisibleText
    }

    private const string c_clientID = "MyEnumValue";
    private const string c_valueName = "ListControlClientID";
    private const string c_labelID = "Label";
    private static readonly PlainTextString s_validationErrors = PlainTextString.CreateFromText("ValidationError");
    private Mock<IBocEnumValue> _enumValue;
    private readonly Unit _width = Unit.Point(173);
    private readonly Unit _height = Unit.Point(17);
    private IEnumerationValueInfo[] _enumerationInfos;
    private IInternalControlMemberCaller _internalControlMemberCaller;

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      _enumValue = new Mock<IBocEnumValue>();
      _enumValue.SetupProperty(_ => _.CssClass);
      var businessObjectProvider = BindableObjectProvider.GetProvider(typeof(BindableObjectProviderAttribute));
      var propertyInfo = PropertyInfoAdapter.Create(typeof(TypeWithEnum).GetProperty("EnumValue"));
      IBusinessObjectEnumerationProperty property =
          new EnumerationProperty(
              new PropertyBase.Parameters(
                  (BindableObjectProvider)businessObjectProvider,
                  propertyInfo,
                  typeof(TestEnum),
                  new Lazy<Type>(() => typeof(TestEnum)),
                  null,
                  false,
                  true,
                  false,
                  new BindableObjectDefaultValueStrategy(),
                  new Mock<IBindablePropertyReadAccessStrategy>().Object,
                  new Mock<IBindablePropertyWriteAccessStrategy>().Object,
                  SafeServiceLocator.Current.GetInstance<BindableObjectGlobalizationService>(),
                  new Mock<IBusinessObjectPropertyConstraintProvider>().Object));

      _enumValue.Object.Property = property;
      _enumValue.Setup(stub => stub.ClientID).Returns(c_clientID);
      _enumValue.Setup(stub => stub.ControlType).Returns("BocEnumValue");
      _enumValue.Setup(mock => mock.GetLabelIDs()).Returns(EnumerableUtility.Singleton(c_labelID));
      _enumValue.Setup(mock => mock.GetValidationErrors()).Returns(EnumerableUtility.Singleton(s_validationErrors));

      var pageStub = new Mock<IPage>();
      pageStub.Setup(stub => stub.WrappedInstance).Returns(new PageMock());
      _enumValue.Setup(stub => stub.Page).Returns(pageStub.Object);

      var values = new List<EnumerationValueInfo>(3);
      foreach (TestEnum value in Enum.GetValues(typeof(TestEnum)))
        values.Add(new EnumerationValueInfo(value, value.ToString(), value.ToString(), true));
      _enumerationInfos = values.ToArray();
      _enumValue.Setup(mock => mock.GetEnabledValues()).Returns(_enumerationInfos);

      _enumValue.Setup(mock => mock.GetNullItemText()).Returns(PlainTextString.CreateFromText("null-text"));
      _enumValue.Setup(mock => mock.NullIdentifier).Returns("null-id");
      _enumValue.Setup(mock => mock.GetValueName()).Returns(c_valueName);

      StateBag stateBag = new StateBag();
      _enumValue.Setup(mock => mock.Attributes).Returns(new AttributeCollection(stateBag));
      _enumValue.Setup(mock => mock.Style).Returns(_enumValue.Object.Attributes.CssStyle);
      _enumValue.Setup(mock => mock.LabelStyle).Returns(new Style(stateBag));
      _enumValue.Setup(mock => mock.ListControlStyle).Returns(new ListControlStyle { ControlType = ListControlType.DropDownList });
      _enumValue.Setup(mock => mock.ControlStyle).Returns(new Style(stateBag));
      _enumValue.Setup(mock => mock.Enabled).Returns(true);

      _internalControlMemberCaller = SafeServiceLocator.Current.GetInstance<IInternalControlMemberCaller>();
    }

    [Test]
    public void Render_NullValueSelected_IsNotRequired ()
    {
      _enumValue.Setup(mock => mock.IsRequired).Returns(false);

      AssertOptionList(NullOption.HasNullValueWithScreenReaderOnlyText, null, false, false);
    }

    [Test]
    public void Render_NullValueSelected_IsRequired ()
    {
      _enumValue.Setup(mock => mock.IsRequired).Returns(true);

      AssertOptionList(NullOption.HasNullValueWithScreenReaderOnlyText, null, false, false);
    }

    [Test]
    public void Render_NamedValueSelected ()
    {
      _enumValue.Setup(mock => mock.IsRequired).Returns(true);
      _enumValue.SetupProperty(_ => _.Value);
      _enumValue.Object.Value = TestEnum.First;

      AssertOptionList(NullOption.WithoutNullValue, TestEnum.First, false, false);
    }

    [Test]
    public void Render_NamedValueSelected_AutoPostback ()
    {
      _enumValue.Setup(mock => mock.IsRequired).Returns(true);
      _enumValue.Object.ListControlStyle.AutoPostBack = true;
      _enumValue.SetupProperty(_ => _.Value);
      _enumValue.Object.Value = TestEnum.First;

      AssertOptionList(NullOption.WithoutNullValue, TestEnum.First, false, true);
    }

    [Test]
    public void Render_NamedValueSelected_WithNullOption ()
    {
      _enumValue.Setup(mock => mock.IsRequired).Returns(false);
      _enumValue.SetupProperty(_ => _.Value);
      _enumValue.Object.Value = TestEnum.First;

      AssertOptionList(NullOption.HasNullValueWithScreenReaderOnlyText, TestEnum.First, false, false);
    }

    [Test]
    public void Render_NamedValueSelected_WithCssClass ()
    {
      _enumValue.Object.CssClass = "CssClass";
      _enumValue.Setup(mock => mock.IsRequired).Returns(true);
      _enumValue.SetupProperty(_ => _.Value);
      _enumValue.Object.Value = TestEnum.First;

      AssertOptionList(NullOption.WithoutNullValue, TestEnum.First, false, false);
    }

    [Test]
    public void Render_NullOption_WithVisibleText ()
    {
      _enumValue.Setup(mock => mock.IsRequired).Returns(false);
      _enumValue.Object.ListControlStyle.DropDownListNullValueTextVisible = true;
      _enumValue.SetupProperty(_ => _.Value);
      _enumValue.Object.Value = TestEnum.First;

      AssertOptionList(NullOption.HasNullValueWithVisibleText, TestEnum.First, false, false);
    }

    [Test]
    public void Render_NamedValueSelected_WithCssClassInAttributes ()
    {
      _enumValue.Object.Attributes["class"] = "CssClass";
      _enumValue.Setup(mock => mock.IsRequired).Returns(true);
      _enumValue.SetupProperty(_ => _.Value);
      _enumValue.Object.Value = TestEnum.First;

      AssertOptionList(NullOption.WithoutNullValue, TestEnum.First, false, false);
    }

    [Test]
    public void Render_NamedValueSelected_WithStyle ()
    {
      _enumValue.SetupProperty(_ => _.Height);
      _enumValue.SetupProperty(_ => _.Width);
      _enumValue.Object.Height = _height;
      _enumValue.Object.Width = _width;
      _enumValue.Object.ControlStyle.Height = _height;
      _enumValue.Object.ControlStyle.Width = _width;
      _enumValue.Setup(mock => mock.IsRequired).Returns(true);
      _enumValue.SetupProperty(_ => _.Value);
      _enumValue.Object.Value = TestEnum.First;

      AssertOptionList(NullOption.WithoutNullValue, TestEnum.First, true, false);
    }

    [Test]
    public void Render_NamedValueSelected_WithStyleInAttributes ()
    {
      _enumValue.Object.Style[HtmlTextWriterStyle.Height] = _height.ToString();
      _enumValue.Object.Style[HtmlTextWriterStyle.Width] = _width.ToString();
      _enumValue.Setup(mock => mock.IsRequired).Returns(true);
      _enumValue.SetupProperty(_ => _.Value);
      _enumValue.Object.Value = TestEnum.First;

      AssertOptionList(NullOption.WithoutNullValue, TestEnum.First, true, false);
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes ()
    {
      _enumValue.Object.ListControlStyle.AutoPostBack = true;

      var resourceUrlFactory = new FakeResourceUrlFactory();
      var renderer = new BocEnumValueRenderer(
          resourceUrlFactory,
          GlobalizationService,
          RenderingFeatures.WithDiagnosticMetadata,
          _internalControlMemberCaller,
          new StubLabelReferenceRenderer(),
          new StubValidationErrorRenderer());
      renderer.Render(new BocEnumValueRenderingContext(HttpContext, Html.Writer, _enumValue.Object));

      var document = Html.GetResultDocument();
      var outerSpan = Html.GetAssertedChildElement(document, "span", 0);
      Html.AssertAttribute(outerSpan, DiagnosticMetadataAttributes.ControlType, "BocEnumValue");
      Html.AssertAttribute(outerSpan, DiagnosticMetadataAttributes.TriggersPostBack, "true");
      Html.AssertAttribute(outerSpan, DiagnosticMetadataAttributesForObjectBinding.BocEnumValueStyle, "DropDownList");
      Html.AssertAttribute(outerSpan, DiagnosticMetadataAttributesForObjectBinding.NullIdentifier, "null-id");
    }

    private XmlNode GetAssertedSpan (XmlDocument document, bool withStyle, BocEnumValueRenderer renderer)
    {
      var div = Html.GetAssertedChildElement(document, "span", 0);
      string cssClass = _enumValue.Object.CssClass;
      if (string.IsNullOrEmpty(cssClass))
        cssClass = _enumValue.Object.Attributes["class"];
      if (string.IsNullOrEmpty(cssClass))
        cssClass = "bocEnumValue dropDownList";

      Html.AssertAttribute(div, "id", "MyEnumValue");
      Html.AssertAttribute(div, "class", cssClass, HtmlHelperBase.AttributeValueCompareMode.Contains);

      if (withStyle)
      {
        Html.AssertStyleAttribute(div, "height", _height.ToString());
        Html.AssertStyleAttribute(div, "width", _width.ToString());
      }

      return div;
    }

    private void AssertOptionList (NullOption nullOption, TestEnum? selectedValue, bool withStyle, bool autoPostBack)
    {
      var renderer = new BocEnumValueRenderer(
          new FakeResourceUrlFactory(),
          GlobalizationService,
          RenderingFeatures.Default,
          _internalControlMemberCaller,
          new StubLabelReferenceRenderer(),
          new StubValidationErrorRenderer());
      renderer.Render(new BocEnumValueRenderingContext(HttpContext, Html.Writer, _enumValue.Object));

      var document = Html.GetResultDocument();
      var div = GetAssertedSpan(document, false, renderer);

      var select = Html.GetAssertedChildElement(div, "select", 0);
      Html.AssertAttribute(select, "id", c_valueName);
      Html.AssertAttribute(select, "name", c_valueName);
      Html.AssertAttribute(select, StubLabelReferenceRenderer.LabelReferenceAttribute, c_labelID);
      Html.AssertAttribute(select, StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute, "");
      Html.AssertAttribute(select, StubValidationErrorRenderer.ValidationErrorsIDAttribute, c_clientID + "_ValidationErrors");
      Html.AssertAttribute(select, StubValidationErrorRenderer.ValidationErrorsAttribute, s_validationErrors);

      if (withStyle)
        Html.AssertStyleAttribute(select, "height", "100%");

      if (nullOption == NullOption.HasNullValueWithScreenReaderOnlyText)
        AssertNullOptionWithScreenReaderOnlyText(select, !selectedValue.HasValue);
      else if (nullOption == NullOption.HasNullValueWithVisibleText)
        AssertNullOptionWithVisibleText(select, !selectedValue.HasValue);

      if (autoPostBack)
        Html.AssertAttribute(select, "onchange", string.Format("javascript:__doPostBack('{0}','')", c_valueName));

      int index = nullOption == NullOption.WithoutNullValue ? 0 : 1;
      foreach (TestEnum value in Enum.GetValues(typeof(TestEnum)))
      {
        AssertOption(select, value.ToString(), value.ToString(), index, selectedValue == value);
        ++index;
      }

      var validationErrors = Html.GetAssertedChildElement(div, "fake", 1);
      Html.AssertAttribute(validationErrors, StubValidationErrorRenderer.ValidationErrorsIDAttribute, c_clientID + "_ValidationErrors");
      Html.AssertAttribute(validationErrors, StubValidationErrorRenderer.ValidationErrorsAttribute, s_validationErrors);
    }

    private XmlNode AssertOption (XmlNode select, string value, string text, int index, bool isSelected)
    {
      var option = Html.GetAssertedChildElement(select, "option", index);
      Html.AssertAttribute(option, "value", value);

      if (!string.IsNullOrEmpty(text))
        Html.AssertTextNode(option, text, 0);

      if (isSelected)
        Html.AssertAttribute(option, "selected", "selected");

      return option;
    }

    private void AssertNullOptionWithScreenReaderOnlyText (XmlNode select, bool isSelected)
    {
      var option = AssertOption(select, _enumValue.Object.NullIdentifier, "", 0, isSelected);
      Html.AssertAttribute(option, HtmlTextWriterAttribute2.Label, ((char)160).ToString());
      Html.AssertAttribute(option, HtmlTextWriterAttribute2.AriaLabel, "null-text");
    }

    private void AssertNullOptionWithVisibleText (XmlNode select, bool isSelected)
    {
      AssertOption(select, _enumValue.Object.NullIdentifier, "null-text", 0, isSelected);
    }
  }
}
