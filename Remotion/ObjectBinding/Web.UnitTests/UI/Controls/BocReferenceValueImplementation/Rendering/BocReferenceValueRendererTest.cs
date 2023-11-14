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
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.FunctionalProgramming;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UnitTests.Domain;
using Remotion.Web;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocReferenceValueImplementation.Rendering
{
  [TestFixture]
  public class BocReferenceValueRendererTest : RendererTestBase
  {
    private const string c_clientID = "MyReferenceValue";
    private const string c_contentID = "MyReferenceValue_Content";
    private const string c_valueName = "MyReferenceValue_SelectedValue";
    private const string c_readOnlyTextValueName = "MyReferenceValue_TextValue";
    private const string c_uniqueIdentifier = "uniqueidentifiert";
    private const string c_labelID = "Label";
    private static readonly PlainTextString s_validationErrors = PlainTextString.CreateFromText("ValidationError");

    private enum OptionMenuConfiguration
    {
      NoOptionsMenu,
      HasOptionsMenu
    }

    private IBusinessObjectProvider _provider;
    private BusinessObjectReferenceDataSource _dataSource;
    protected static readonly Unit Width = Unit.Pixel(250);
    protected static readonly Unit Height = Unit.Point(12);
    private IResourceUrlFactory _resourceUrlFactoryStub;

    public Mock<IClientScriptManager> ClientScriptManagerMock { get; set; }
    public Mock<IBocReferenceValue> Control { get; set; }
    public TypeWithReference BusinessObject { get; set; }
    public StubDropDownMenu OptionsMenu { get; set; }
    public StubDropDownList DropDownList { get; set; }

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      OptionsMenu = new StubDropDownMenu();
      DropDownList = new StubDropDownList();

      Control = new Mock<IBocReferenceValue>();
      Control.SetupProperty(_ => _.CssClass);
      Control.Setup(stub => stub.ClientID).Returns(c_clientID);
      Control.Setup(stub => stub.ControlType).Returns("BocReferenceValue");
      Control.Setup(mock => mock.GetLabelIDs()).Returns(EnumerableUtility.Singleton(c_labelID));
      Control.Setup(mock => mock.GetValidationErrors()).Returns(EnumerableUtility.Singleton(s_validationErrors));
      Control.Setup(stub => stub.BusinessObjectUniqueIdentifier).Returns(c_uniqueIdentifier);

      Control.Setup(stub => stub.OptionsMenu).Returns(OptionsMenu);

      var pageStub = new Mock<IPage>();
      pageStub.Setup(stub => stub.WrappedInstance).Returns(new PageMock());
      Control.Setup(stub => stub.Page).Returns(pageStub.Object);

      ClientScriptManagerMock = new Mock<IClientScriptManager>();
      pageStub.Setup(stub => stub.ClientScript).Returns(ClientScriptManagerMock.Object);

      BusinessObject = TypeWithReference.Create("MyBusinessObject");
      BusinessObject.ReferenceList = new[]
                                     {
                                         TypeWithReference.Create("ReferencedObject 0"),
                                         TypeWithReference.Create("ReferencedObject 1"),
                                         TypeWithReference.Create("ReferencedObject 2")
                                     };
      _dataSource = new BusinessObjectReferenceDataSource();
      _dataSource.BusinessObject = (IBusinessObject)BusinessObject;

      _provider = ((IBusinessObject)BusinessObject).BusinessObjectClass.BusinessObjectProvider;
      _provider.AddService<IBusinessObjectWebUIService>(new ReflectionBusinessObjectWebUIService());

      StateBag stateBag = new StateBag();
      Control.Setup(mock => mock.Attributes).Returns(new AttributeCollection(stateBag));
      Control.Setup(mock => mock.Style).Returns(Control.Object.Attributes.CssStyle);
      Control.Setup(mock => mock.LabelStyle).Returns(new Style(stateBag));
      Control.Setup(mock => mock.DropDownListStyle).Returns(new DropDownListStyle());
      Control.Setup(mock => mock.ControlStyle).Returns(new Style(stateBag));
      Control.Setup(stub => stub.GetValueName()).Returns(c_valueName);
      Control
          .Setup(stub => stub.PopulateDropDownList(It.IsNotNull<DropDownList>()))
          .Callback(
              (DropDownList dropDownList) =>
              {
                foreach (var item in BusinessObject.ReferenceList)
                  dropDownList.Items.Add(new ListItem(item.DisplayName, item.UniqueIdentifier));
              });

      Control.Setup(stub => stub.GetLabelText()).Returns("MyText");
      Control.Setup(stub => stub.ResolveClientUrl(It.IsAny<string>())).Returns((string url) => url.TrimStart('~'));
      Control.Setup(stub => stub.GetResourceManager()).Returns(NullResourceManager.Instance);
      Control.Setup(stub => stub.NullValueString).Returns("null-id");

      _resourceUrlFactoryStub = new FakeResourceUrlFactory();
    }

    [TearDown]
    public void TearDown ()
    {
      ClientScriptManagerMock.Verify();
    }

    [Test]
    public void RenderNullReferenceValue ()
    {
      Control.Setup(stub => stub.Enabled).Returns(true);
      XmlNode containerDiv = GetAssertedContainerSpan(false);
      AssertControl(containerDiv, OptionMenuConfiguration.NoOptionsMenu);
    }

    [Test]
    public void RenderNullReferenceValueWithOptionsMenu ()
    {
      Control.Setup(stub => stub.Enabled).Returns(true);
      Control.Setup(stub => stub.HasOptionsMenu).Returns(true);

      XmlNode containerDiv = GetAssertedContainerSpan(false);
      AssertControl(containerDiv, OptionMenuConfiguration.HasOptionsMenu);
    }

    [Test]
    public void RenderNullReferenceValueReadOnly ()
    {
      Control.Setup(stub => stub.Enabled).Returns(true);
      Control.Setup(stub => stub.IsIconEnabled()).Returns(true);
      Control.Setup(stub => stub.IsReadOnly).Returns(true);

      XmlNode span = GetAssertedContainerSpan(false);
      AssertReadOnlyContent(span);
    }

    [Test]
    public void RenderNullReferenceValueReadOnlyWithStyle ()
    {
      Control.Setup(stub => stub.Enabled).Returns(true);
      Control.Setup(stub => stub.IsIconEnabled()).Returns(true);
      Control.Setup(stub => stub.IsReadOnly).Returns(true);
      AddStyle();

      XmlNode span = GetAssertedContainerSpan(true);
      AssertReadOnlyContent(span);
    }

    [Test]
    public void RenderNullReferenceValueReadOnlyWithOptionsMenu ()
    {
      Control.Setup(stub => stub.Enabled).Returns(true);
      Control.Setup(stub => stub.IsIconEnabled()).Returns(true);
      Control.Setup(stub => stub.HasOptionsMenu).Returns(true);
      Control.Setup(stub => stub.IsReadOnly).Returns(true);

      XmlNode span = GetAssertedContainerSpan(false);
      AssertReadOnlyContent(span);

      Assert.That(OptionsMenu.Style["width"], Is.Null);
      Assert.That(OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderNullReferenceValueWithStyle ()
    {
      Control.Setup(stub => stub.Enabled).Returns(true);
      AddStyle();

      XmlNode span = GetAssertedContainerSpan(true);
      AssertControl(span, OptionMenuConfiguration.NoOptionsMenu);
    }

    [Test]
    public void RenderNullReferenceValueWithOptionsAndStyle ()
    {
      Control.Setup(stub => stub.Enabled).Returns(true);
      Control.Setup(stub => stub.HasOptionsMenu).Returns(true);
      AddStyle();

      XmlNode span = GetAssertedContainerSpan(true);
      AssertControl(span, OptionMenuConfiguration.HasOptionsMenu);

      Assert.That(OptionsMenu.Style["width"], Is.Null);
      Assert.That(OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderNullReferenceValueWithIcon ()
    {
      Control.Setup(stub => stub.Enabled).Returns(true);
      Control.Setup(stub => stub.IsIconEnabled()).Returns(true);
      Control
          .Setup(stub => stub.Property)
          .Returns((IBusinessObjectReferenceProperty)((IBusinessObject)BusinessObject).BusinessObjectClass.GetPropertyDefinition("ReferenceValue"));
      SetUpGetIconExpectations();

      XmlNode span = GetAssertedContainerSpan(false);
      AssertControl(span, OptionMenuConfiguration.NoOptionsMenu);
    }

    [Test]
    public void RenderReferenceValue ()
    {
      Control.Setup(stub => stub.Enabled).Returns(true);
      SetValue();
      XmlNode span = GetAssertedContainerSpan(false);
      AssertControl(span, OptionMenuConfiguration.NoOptionsMenu);
    }

    [Test]
    public void RenderReferenceValueAutoPostback ()
    {
      DropDownList.AutoPostBack = false;
      Control.Setup(stub => stub.Enabled).Returns(true);
      SetValue();

      Control.Setup(stub => stub.DropDownListStyle).Returns(new DropDownListStyle());
      Control.Object.DropDownListStyle.AutoPostBack = true;

      XmlNode span = GetAssertedContainerSpan(false);
      AssertControl(span, OptionMenuConfiguration.NoOptionsMenu);
      Assert.That(DropDownList.AutoPostBack, Is.True);
    }

    private void SetValue ()
    {
      BusinessObject.ReferenceValue = BusinessObject.ReferenceList[0];
      Control.Setup(stub => stub.Value).Returns((IBusinessObjectWithIdentity)BusinessObject.ReferenceValue);
    }

    [Test]
    public void RenderReferenceValueWithOptionsMenu ()
    {
      Control.Setup(stub => stub.Enabled).Returns(true);
      SetValue();
      Control.Setup(stub => stub.HasOptionsMenu).Returns(true);

      XmlNode span = GetAssertedContainerSpan(false);
      AssertControl(span, OptionMenuConfiguration.HasOptionsMenu);

      Assert.That(OptionsMenu.Style["width"], Is.Null);
      Assert.That(OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderReferenceValueReadOnly ()
    {
      Control.Setup(stub => stub.Enabled).Returns(true);
      SetValue();
      Control.Setup(stub => stub.IsIconEnabled()).Returns(true);
      Control.Setup(stub => stub.IsReadOnly).Returns(true);

      XmlNode span = GetAssertedContainerSpan(false);
      AssertReadOnlyContent(span);
    }

    [Test]
    public void RenderReferenceValueReadOnlyWithStyle ()
    {
      Control.Setup(stub => stub.Enabled).Returns(true);
      SetValue();
      Control.Setup(stub => stub.IsIconEnabled()).Returns(true);
      Control.Setup(stub => stub.IsReadOnly).Returns(true);
      AddStyle();

      XmlNode span = GetAssertedContainerSpan(true);
      AssertReadOnlyContent(span);
    }

    [Test]
    public void RenderReferenceValueReadOnlyWithOptionsMenu ()
    {
      Control.Setup(stub => stub.Enabled).Returns(true);
      SetValue();
      Control.Setup(stub => stub.IsIconEnabled()).Returns(true);
      Control.Setup(stub => stub.HasOptionsMenu).Returns(true);
      Control.Setup(stub => stub.ReserveOptionsMenuWidth).Returns(true);
      Control.Setup(stub => stub.IsReadOnly).Returns(true);

      XmlNode span = GetAssertedContainerSpan(false);
      AssertReadOnlyContent(span);

      Assert.That(OptionsMenu.Style["width"], Is.Null);
      Assert.That(OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderReferenceValueWithStyle ()
    {
      Control.Setup(stub => stub.Enabled).Returns(true);
      SetValue();
      AddStyle();

      XmlNode span = GetAssertedContainerSpan(true);
      AssertControl(span, OptionMenuConfiguration.NoOptionsMenu);
    }

    [Test]
    public void RenderReferenceValueWithOptionsAndStyle ()
    {
      Control.Setup(stub => stub.Enabled).Returns(true);
      SetValue();
      Control.Setup(stub => stub.HasOptionsMenu).Returns(true);
      AddStyle();

      XmlNode span = GetAssertedContainerSpan(true);
      AssertControl(span, OptionMenuConfiguration.HasOptionsMenu);

      Assert.That(OptionsMenu.Style["width"], Is.Null);
      Assert.That(OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderReferenceValueWithIcon ()
    {
      Control.Setup(stub => stub.Enabled).Returns(true);
      SetValue();
      Control.Setup(stub => stub.IsIconEnabled()).Returns(true);
      Control
          .Setup(stub => stub.Property)
          .Returns((IBusinessObjectReferenceProperty)((IBusinessObject)BusinessObject).BusinessObjectClass.GetPropertyDefinition("ReferenceValue"));
      SetUpGetIconExpectations();

      XmlNode span = GetAssertedContainerSpan(false);
      AssertControl(span, OptionMenuConfiguration.NoOptionsMenu);
    }

    [Test]
    public void RenderIDs ()
    {
      Control.Setup(stub => stub.Enabled).Returns(true);

      var renderer = new BocReferenceValueRenderer(
          _resourceUrlFactoryStub,
          GlobalizationService,
          RenderingFeatures.Default,
          new StubLabelReferenceRenderer(),
          new StubValidationErrorRenderer());
      renderer.Render(CreateRenderingContext());
      var document = Html.GetResultDocument();
      var span = document.GetAssertedChildElement("span", 0).GetAssertedChildElement("span", 0).GetAssertedChildElement("span", 1);
      var select = span.GetAssertedChildElement("select", 0);
      select.AssertAttributeValueEquals("id", c_valueName);
      select.AssertAttributeValueEquals("name", c_valueName);
      Html.AssertAttribute(select, StubLabelReferenceRenderer.LabelReferenceAttribute, c_labelID);
      Html.AssertAttribute(select, StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute, "");
      Html.AssertAttribute(select, StubValidationErrorRenderer.ValidationErrorsIDAttribute, c_clientID + "_ValidationErrors");
      Html.AssertAttribute(select, StubValidationErrorRenderer.ValidationErrorsAttribute, s_validationErrors);

      var validationErrors = span.GetAssertedChildElement("fake", 1);
      Html.AssertAttribute(validationErrors, StubValidationErrorRenderer.ValidationErrorsIDAttribute, c_clientID + "_ValidationErrors");
      Html.AssertAttribute(validationErrors, StubValidationErrorRenderer.ValidationErrorsAttribute, s_validationErrors);
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes ()
    {
      Control.Object.DropDownListStyle.AutoPostBack = true;

      var renderer = new BocReferenceValueRenderer(
          _resourceUrlFactoryStub,
          GlobalizationService,
          RenderingFeatures.WithDiagnosticMetadata,
          new StubLabelReferenceRenderer(),
          new StubValidationErrorRenderer());
      renderer.Render(CreateRenderingContext());

      var document = Html.GetResultDocument();
      var control = document.DocumentElement;
      control.AssertAttributeValueEquals(DiagnosticMetadataAttributes.ControlType, "BocReferenceValue");
      control.AssertAttributeValueEquals(DiagnosticMetadataAttributes.TriggersPostBack, "true");
      control.AssertAttributeValueEquals(DiagnosticMetadataAttributesForObjectBinding.NullIdentifier, "null-id");
    }

    private void AssertReadOnlyContent (XmlNode parent)
    {
      var hasOptionsMenu = Control.Object.HasOptionsMenu;
      var optionsMenuOffset = hasOptionsMenu ? 1 : 0;
      var hasOptionsMenuCssClass = hasOptionsMenu ? "hasOptionsMenu" : "withoutOptionsMenu";

      var span = parent.GetAssertedChildElement("span", 0);
      span.AssertAttributeValueEquals("class", "body");

      var hasIcon = AssertIcon(span);
      var iconOffset = hasIcon ? 1 : 0;
      var hasIconCssClass = hasIcon ? " hasIcon" : "";

      span.AssertChildElementCount(1 + iconOffset + optionsMenuOffset);

      var contentSpan = span.GetAssertedChildElement("span", 0 + iconOffset);
      contentSpan.AssertAttributeValueEquals("id",  c_contentID);
      contentSpan.AssertAttributeValueEquals("class", "content remotion-themed" + hasIconCssClass + " " + hasOptionsMenuCssClass);
      contentSpan.AssertChildElementCount(3);

      var input = contentSpan.GetAssertedChildElement("input", 0);
      input.AssertAttributeValueEquals("id", c_readOnlyTextValueName);
      input.AssertAttributeValueEquals("name", c_readOnlyTextValueName);
      input.AssertAttributeValueEquals("readonly", "readonly");
      input.AssertNoAttribute("disabled");
      input.AssertAttributeValueEquals("value", "MyText");
      input.AssertAttributeValueEquals("role", "combobox");
      input.AssertAttributeValueEquals("aria-roledescription", "read only combobox");
      input.AssertAttributeValueEquals("aria-expanded", "false");
      input.AssertAttributeValueEquals("aria-haspopup", "menu");
      input.AssertAttributeValueEquals(StubLabelReferenceRenderer.LabelReferenceAttribute, c_labelID);
      input.AssertAttributeValueEquals(StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute, "");
      input.AssertAttributeValueEquals(StubValidationErrorRenderer.ValidationErrorsIDAttribute, c_clientID + "_ValidationErrors");
      input.AssertAttributeValueEquals(StubValidationErrorRenderer.ValidationErrorsAttribute, s_validationErrors);
      input.AssertAttributeValueEquals("data-value", c_uniqueIdentifier);
      input.AssertAttributeValueEquals("class", CssClassDefinition.ScreenReaderText);

      var labelSpan = contentSpan.GetAssertedChildElement("span", 1);
      labelSpan.AssertNoAttribute("tabindex");
      labelSpan.AssertAttributeValueEquals("aria-hidden", "true");
      labelSpan.AssertTextNode("MyText", 0);

      var validationErrors = contentSpan.GetAssertedChildElement("fake", 2);
      validationErrors.AssertAttributeValueEquals(StubValidationErrorRenderer.ValidationErrorsIDAttribute, c_clientID + "_ValidationErrors");
      validationErrors.AssertAttributeValueEquals(StubValidationErrorRenderer.ValidationErrorsAttribute, s_validationErrors);

      if (hasOptionsMenu)
      {
        var wrapperSpan = span.GetAssertedChildElement("span", 1 + iconOffset);
        wrapperSpan.AssertAttributeValueEquals("class", "optionsMenu");
        wrapperSpan.AssertChildElementCount(0);
        wrapperSpan.AssertTextNode("DropDownMenu", 0);
      }
    }

    private void AssertControl (XmlNode containerDiv, OptionMenuConfiguration optionMenuConfiguration)
    {
      var contentDiv = containerDiv.GetAssertedChildElement("span", 0);
      contentDiv.AssertAttributeValueEquals("class", "body");

      var hasIcon = AssertIcon(contentDiv);
      var iconOffset = 1;
      var hasIconCssClass = hasIcon ? " hasIcon" : "";

      var contentSpan = contentDiv.GetAssertedChildElement("span", iconOffset);
      contentSpan.AssertAttributeValueEquals("id",  c_contentID);
      switch (optionMenuConfiguration)
      {
        case OptionMenuConfiguration.NoOptionsMenu:
          contentSpan.AssertAttributeValueEquals("class", "content remotion-themed" + hasIconCssClass + " withoutOptionsMenu");
          break;
        case OptionMenuConfiguration.HasOptionsMenu:
          contentSpan.AssertAttributeValueEquals("class", "content remotion-themed" + hasIconCssClass + " hasOptionsMenu");
          break;
      }

      contentSpan.AssertTextNode("DropDownList", 0);

      if (optionMenuConfiguration == OptionMenuConfiguration.HasOptionsMenu)
      {
        var optionsMenuDiv = contentDiv.GetAssertedChildElement("span", 1 + iconOffset);
        optionsMenuDiv.AssertAttributeValueEquals("class", "optionsMenu");
        optionsMenuDiv.AssertTextNode("DropDownMenu", 0);
      }
    }

    private XmlNode GetAssertedContainerSpan (bool withStyle)
    {
      var renderer = new TestableBocReferenceValueRenderer(
          _resourceUrlFactoryStub,
          GlobalizationService,
          RenderingFeatures.Default,
          new StubLabelReferenceRenderer(),
          new StubValidationErrorRenderer(),
          () => DropDownList);
      renderer.Render(CreateRenderingContext());

      var document = Html.GetResultDocument();
      var containerDiv = document.GetAssertedChildElement("span", 0);

      containerDiv.AssertAttributeValueEquals("id", c_clientID);
      containerDiv.AssertAttributeValueContains("class", "bocReferenceValue");
      if (Control.Object.IsReadOnly)
        containerDiv.AssertAttributeValueContains("class", "readOnly");
      if (!Control.Object.Enabled)
        containerDiv.AssertAttributeValueContains("class", "disabled");
      if (Control.Object.HasOptionsMenu && Control.Object.ReserveOptionsMenuWidth)
        containerDiv.AssertAttributeValueContains("class", "reserveOptionsMenuWidth");

      if (withStyle)
      {
        containerDiv.AssertStyleAttribute("width", Width.ToString());
        containerDiv.AssertStyleAttribute("height", Height.ToString());
      }

      return containerDiv;
    }

    protected void AddStyle ()
    {
      Control.SetupProperty(_ => _.Height);
      Control.SetupProperty(_ => _.Width);
      Control.Object.Height = Height;
      Control.Object.Width = Width;
      Control.Object.Style["height"] = Control.Object.Height.ToString();
      Control.Object.Style["width"] = Control.Object.Width.ToString();
    }

    protected void SetUpGetIconExpectations ()
    {
      Control.Setup(mock => mock.GetIcon()).Returns(new IconInfo("~/Images/NullIcon.gif")).Verifiable();
    }

    private bool AssertIcon (XmlNode parent)
    {
      var isIconEnabled = Control.Object.IsIconEnabled();

      if (!isIconEnabled)
        return false;

      if (Control.Object.IsReadOnly && Control.Object.GetIcon() == null)
        return false;

      var iconOffset = 0;
      var iconParent = parent.GetAssertedChildElement("span", iconOffset);

      iconParent.AssertAttributeValueEquals("class", "icon");
      iconParent.AssertChildElementCount(1);

      var icon = iconParent.GetAssertedChildElement("img", 0);
      icon.AssertAttributeValueEquals("src", "/Images/NullIcon.gif");

      return true;
    }

    private BocReferenceValueRenderingContext CreateRenderingContext ()
    {
      var businessObjectWebServiceContext = BusinessObjectWebServiceContext.Create(Control.Object.DataSource, Control.Object.Property, "Args");

      return new BocReferenceValueRenderingContext(HttpContext, Html.Writer, Control.Object, businessObjectWebServiceContext);
    }
  }
}
