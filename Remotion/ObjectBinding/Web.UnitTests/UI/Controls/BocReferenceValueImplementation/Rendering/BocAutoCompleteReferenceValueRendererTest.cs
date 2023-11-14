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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using Moq;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.FunctionalProgramming;
using Remotion.Globalization;
using Remotion.ObjectBinding.BindableObject;
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
  public class BocAutoCompleteReferenceValueRendererTest : RendererTestBase
  {
    private const string c_clientID = "MyReferenceValue";
    private const string c_contentID = "MyReferenceValue_Content";
    private const string c_textValueName = "MyReferenceValue_SelectedTextValue";
    private const string c_keyValueName = "MyReferenceValue_SelectedKeyValue";
    private const string c_readOnlyTextValueName = "MyReferenceValue_TextValue";
    private const string c_uniqueidentifier = "uniqueidentifier";
    private const string c_labelID = "Label";

    private static readonly PlainTextString s_validationErrors = PlainTextString.CreateFromText("ValidationError");

    private enum OptionMenuConfiguration
    {
      NoOptionsMenu,
      HasOptionsMenu
    }

    private enum AutoPostBack
    {
      Disabled,
      Enabled
    }

    private static readonly Unit s_width = Unit.Pixel(250);
    private static readonly Unit s_height = Unit.Point(12);

    private BusinessObjectReferenceDataSource _dataSource;
    private IBusinessObjectProvider _provider;
    private IResourceUrlFactory _resourceUrlFactory;
    private Mock<IBocAutoCompleteReferenceValue> Control { get; set; }
    private DropDownMenu OptionsMenu { get; set; }
    private Mock<IClientScriptManager> ClientScriptManagerMock { get; set; }
    private TypeWithReference BusinessObject { get; set; }
    private IBusinessObjectDataSource DataSource { get; set; }
    private StubTextBox TextBox { get; set; }


    [SetUp]
    public void SetUp ()
    {
      Initialize();

      OptionsMenu = new StubDropDownMenu();
      TextBox = new StubTextBox();

      Control = new Mock<IBocAutoCompleteReferenceValue>();
      Control.SetupProperty(_ => _.CssClass);
      Control.Setup(stub => stub.ClientID).Returns(c_clientID);
      Control.Setup(stub => stub.ControlType).Returns("BocAutoCompleteReferenceValue");
      Control.Setup(stub => stub.GetTextValueName()).Returns(c_textValueName);
      Control.Setup(stub => stub.GetKeyValueName()).Returns(c_keyValueName);
      Control.Setup(mock => mock.GetLabelIDs()).Returns(EnumerableUtility.Singleton(c_labelID));
      Control.Setup(mock => mock.GetValidationErrors()).Returns(EnumerableUtility.Singleton(s_validationErrors));
      Control.Setup(stub => stub.BusinessObjectUniqueIdentifier).Returns(c_uniqueidentifier);
      Control.Setup(stub=>stub.ControlServicePath).Returns("~/ControlService.asmx");

      Control.Setup(stub => stub.OptionsMenu).Returns(OptionsMenu);

      var pageStub = new Mock<IPage>();
      pageStub.Setup(stub => stub.Form).Returns(new HtmlForm());
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

      DataSource = new BindableObjectDataSource { Type = typeof(TypeWithReference) };
      DataSource.Register(Control.Object);

      StateBag stateBag = new StateBag();
      Control.Setup(mock => mock.Attributes).Returns(new AttributeCollection(stateBag));
      Control.Setup(mock => mock.Style).Returns(Control.Object.Attributes.CssStyle);
      Control.Setup(mock => mock.CommonStyle).Returns(new Style(stateBag));
      Control.Setup(mock => mock.LabelStyle).Returns(new Style(stateBag));
      Control.Setup(mock => mock.TextBoxStyle).Returns(new SingleRowTextBoxStyle());
      Control.Setup(mock => mock.ControlStyle).Returns(new Style(stateBag));

      Control.Setup(stub => stub.GetLabelText()).Returns("MyText");
      Control.Setup(stub => stub.ResolveClientUrl(It.IsAny<string>())).Returns((string url) => url.TrimStart('~'));
      Control.Setup(stub => stub.GetResourceManager()).Returns(NullResourceManager.Instance);

      _resourceUrlFactory = new FakeResourceUrlFactory();
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
      AssertControl(containerDiv, OptionMenuConfiguration.NoOptionsMenu, AutoPostBack.Disabled);
    }

    [Test]
    public void RenderNullReferenceValueWithOptionsMenu ()
    {
      Control.Setup(stub => stub.Enabled).Returns(true);
      Control.Setup(stub => stub.HasOptionsMenu).Returns(true);

      XmlNode containerDiv = GetAssertedContainerSpan(false);
      AssertControl(containerDiv, OptionMenuConfiguration.HasOptionsMenu, AutoPostBack.Disabled);
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
      AssertControl(span, OptionMenuConfiguration.NoOptionsMenu, AutoPostBack.Disabled);
    }

    [Test]
    public void RenderNullReferenceValueWithOptionsAndStyle ()
    {
      Control.Setup(stub => stub.Enabled).Returns(true);
      Control.Setup(stub => stub.HasOptionsMenu).Returns(true);
      AddStyle();

      XmlNode span = GetAssertedContainerSpan(true);
      AssertControl(span, OptionMenuConfiguration.HasOptionsMenu, AutoPostBack.Disabled);

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
      AssertControl(span, OptionMenuConfiguration.NoOptionsMenu, AutoPostBack.Disabled);
    }

    [Test]
    public void RenderReferenceValue ()
    {
      Control.Setup(stub => stub.Enabled).Returns(true);
      SetValue();
      XmlNode span = GetAssertedContainerSpan(false);
      AssertControl(span, OptionMenuConfiguration.NoOptionsMenu, AutoPostBack.Disabled);
    }

    [Test]
    public void RenderReferenceValueAutoPostback ()
    {
      TextBox.AutoPostBack = false;
      Control.Setup(stub => stub.Enabled).Returns(true);
      SetUpClientScriptExpectations();
      SetValue();

      Control.Setup(stub => stub.TextBoxStyle).Returns(new SingleRowTextBoxStyle());
      Control.Object.TextBoxStyle.AutoPostBack = true;

      XmlNode span = GetAssertedContainerSpan(false);
      AssertControl(span, OptionMenuConfiguration.NoOptionsMenu, AutoPostBack.Enabled);
      Assert.That(TextBox.AutoPostBack, Is.True);
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
      Control.Setup(stub => stub.ReserveOptionsMenuWidth).Returns(true);

      XmlNode span = GetAssertedContainerSpan(false);
      AssertControl(span, OptionMenuConfiguration.HasOptionsMenu, AutoPostBack.Disabled);

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
      AssertControl(span, OptionMenuConfiguration.NoOptionsMenu, AutoPostBack.Disabled);
    }

    [Test]
    public void RenderReferenceValueWithOptionsAndStyle ()
    {
      Control.Setup(stub => stub.Enabled).Returns(true);
      SetValue();
      Control.Setup(stub => stub.HasOptionsMenu).Returns(true);
      AddStyle();

      XmlNode span = GetAssertedContainerSpan(true);
      AssertControl(span, OptionMenuConfiguration.HasOptionsMenu, AutoPostBack.Disabled);

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
      AssertControl(span, OptionMenuConfiguration.NoOptionsMenu, AutoPostBack.Disabled);

      var input = span.GetAssertedChildElement("span", 0).GetAssertedChildElement("span", 1).GetAssertedChildElement("input", 2);
      input.AssertAttributeValueEquals("id", c_keyValueName);
      input.AssertAttributeValueEquals("name", c_keyValueName);
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes ()
    {
      var renderer = new TestableBocAutoCompleteReferenceValueRenderer(
          _resourceUrlFactory,
          GlobalizationService,
          RenderingFeatures.WithDiagnosticMetadata,
          new StubLabelReferenceRenderer(),
          new StubValidationErrorRenderer(),
          () => TextBox);

      TextBox.AutoPostBack = false;
      Control.Setup(stub => stub.Enabled).Returns(true);
      SetUpClientScriptExpectations();
      SetValue();

      Control.Setup(stub => stub.TextBoxStyle).Returns(new SingleRowTextBoxStyle());
      Control.Object.TextBoxStyle.AutoPostBack = true;

      Html.Writer.AddAttribute(HtmlTextWriterAttribute.Class, "body");
      Html.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
      renderer.Render(CreateRenderingContext());
      Html.Writer.RenderEndTag();

      var document = Html.GetResultDocument();
      var control = document.DocumentElement.GetAssertedChildElement("span", 0);
      control.AssertAttributeValueEquals(DiagnosticMetadataAttributes.ControlType, "BocAutoCompleteReferenceValue");
      control.AssertAttributeValueEquals(DiagnosticMetadataAttributes.TriggersPostBack, "true");
    }

    protected void AddStyle ()
    {
      Control.SetupProperty(_ => _.Height);
      Control.SetupProperty(_ => _.Width);
      Control.Object.Height = s_height;
      Control.Object.Width = s_width;
      Control.Object.Style["height"] = Control.Object.Height.ToString();
      Control.Object.Style["width"] = Control.Object.Width.ToString();
    }

    protected void SetUpGetIconExpectations ()
    {
      Control.Setup(mock => mock.GetIcon()).Returns(new IconInfo("~/Images/NullIcon.gif")).Verifiable();
    }

    protected void SetUpClientScriptExpectations ()
    {
      ClientScriptManagerMock
          .Setup(mock => mock.GetPostBackEventReference(It.Is<PostBackOptions>(options => options.TargetControl == TextBox && options.AutoPostBack), true))
          .Returns("PostBackEventReference")
          .Verifiable();
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
      input.AssertAttributeValueEquals("aria-haspopup", "listbox");
      input.AssertAttributeValueEquals(StubLabelReferenceRenderer.LabelReferenceAttribute, c_labelID);
      input.AssertAttributeValueEquals(StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute, "");
      input.AssertAttributeValueEquals(StubValidationErrorRenderer.ValidationErrorsIDAttribute, c_clientID + "_ValidationErrors");
      input.AssertAttributeValueEquals(StubValidationErrorRenderer.ValidationErrorsAttribute, s_validationErrors);
      input.AssertAttributeValueEquals("data-value", c_uniqueidentifier);
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

    private void AssertDropDownListSpan (XmlNode contentSpan, AutoPostBack autoPostBack)
    {
      var inputSpan = contentSpan.GetAssertedChildElement("span", 0);
      inputSpan.AssertAttributeValueEquals("role", "combobox");
      inputSpan.AssertAttributeValueEquals(StubLabelReferenceRenderer.LabelReferenceAttribute, c_labelID);
      inputSpan.AssertAttributeValueEquals(StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute, "");

      inputSpan.AssertAttributeValueEquals("aria-expanded", "false");
      inputSpan.AssertAttributeValueEquals("aria-haspopup", "listbox");
      inputSpan.AssertChildElementCount(1);
      var inputField = inputSpan.GetAssertedChildElement("input", 0);
      inputField.AssertAttributeValueEquals("type", "stub");
      inputField.AssertAttributeValueEquals(StubValidationErrorRenderer.ValidationErrorsIDAttribute, Control.Object.ClientID + "_ValidationErrors");
      inputField.AssertAttributeValueEquals(StubValidationErrorRenderer.ValidationErrorsAttribute, s_validationErrors);

      int hiddenFieldIndex = 1;
      if (Control.Object.Enabled)
      {
        var dropDownButton = contentSpan.GetAssertedChildElement("span", 1);
        dropDownButton.AssertAttributeValueEquals("class", "bocAutoCompleteReferenceValueButton");
        dropDownButton.AssertChildElementCount(1);

        var dropDownSpacer = dropDownButton.GetAssertedChildElement("img", 0);
        dropDownSpacer.AssertAttributeValueEquals("src", IconInfo.CreateSpacer(_resourceUrlFactory).Url);
        dropDownSpacer.AssertChildElementCount(0);

        hiddenFieldIndex++;
      }

      var hiddenField = contentSpan.GetAssertedChildElement("input", hiddenFieldIndex);
      hiddenField.AssertAttributeValueEquals("id", c_keyValueName);
      hiddenField.AssertAttributeValueEquals("name", c_keyValueName);
      hiddenField.AssertAttributeValueEquals("type", "hidden");
      if (autoPostBack == AutoPostBack.Enabled)
        hiddenField.AssertAttributeValueEquals("onchange", "PostBackEventReference");
      else
        hiddenField.AssertNoAttribute("onchange");
      hiddenField.AssertChildElementCount(0);
    }

    private void AssertControl (XmlNode containerDiv, OptionMenuConfiguration optionMenuConfiguration, AutoPostBack autoPostBack)
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

      AssertDropDownListSpan(contentSpan, autoPostBack);

      var validationErrors = contentSpan.GetAssertedChildElement("fake", 3);
      validationErrors.AssertAttributeValueEquals(StubValidationErrorRenderer.ValidationErrorsIDAttribute, Control.Object.ClientID + "_ValidationErrors");
      validationErrors.AssertAttributeValueEquals(StubValidationErrorRenderer.ValidationErrorsAttribute, s_validationErrors);

      if (optionMenuConfiguration == OptionMenuConfiguration.HasOptionsMenu)
      {
        var optionsMenuDiv = contentDiv.GetAssertedChildElement("span", 1 + iconOffset);
        optionsMenuDiv.AssertAttributeValueEquals("class", "optionsMenu");
        optionsMenuDiv.AssertTextNode("DropDownMenu", 0);
      }
    }

    private XmlNode GetAssertedContainerSpan (bool withStyle)
    {
      var renderer = new TestableBocAutoCompleteReferenceValueRenderer(
          _resourceUrlFactory,
          GlobalizationService,
          RenderingFeatures.Default,
          new StubLabelReferenceRenderer(),
          new StubValidationErrorRenderer(),
          () => TextBox);
      renderer.Render(CreateRenderingContext());

      var document = Html.GetResultDocument();
      var containerDiv = document.GetAssertedChildElement("span", 0);

      containerDiv.AssertAttributeValueEquals("id", "MyReferenceValue");
      containerDiv.AssertAttributeValueContains("class", "bocAutoCompleteReferenceValue");
      if (Control.Object.IsReadOnly)
        containerDiv.AssertAttributeValueContains("class", "readOnly");
      if (!Control.Object.Enabled)
        containerDiv.AssertAttributeValueContains("class", "disabled");
      if (Control.Object.HasOptionsMenu && Control.Object.ReserveOptionsMenuWidth)
        containerDiv.AssertAttributeValueContains("class", "reserveOptionsMenuWidth");

      // containerDiv.AssertChildElementCount (1);

      if (withStyle)
      {
        containerDiv.AssertStyleAttribute("width", s_width.ToString());
        containerDiv.AssertStyleAttribute("height", s_height.ToString());
      }

      return containerDiv;
    }

    private BocAutoCompleteReferenceValueRenderingContext CreateRenderingContext ()
    {
      return new BocAutoCompleteReferenceValueRenderingContext(
          HttpContext,
          Html.Writer,
          Control.Object,
          BusinessObjectWebServiceContext.Create(Control.Object.DataSource, Control.Object.Property, "Args"));
    }
  }
}
