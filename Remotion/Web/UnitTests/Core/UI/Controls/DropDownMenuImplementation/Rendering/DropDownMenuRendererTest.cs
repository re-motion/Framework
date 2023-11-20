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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Moq;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.Globalization;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.DropDownMenuImplementation;
using Remotion.Web.UI.Controls.DropDownMenuImplementation.Rendering;
using Remotion.Web.UI.Controls.Rendering;
using ButtonType = Remotion.Web.UI.Controls.ButtonType;

namespace Remotion.Web.UnitTests.Core.UI.Controls.DropDownMenuImplementation.Rendering
{
  [TestFixture]
  public class DropDownMenuRendererTest : RendererTestBase
  {
    private const string c_MenuTitle = "Menu&Title";
    private const string c_Icon_Url = "/Image/icon.gif";
    private const string c_IconAlternateText = "Icon_&AlternateText";
    private const string c_Icon_ToolTip = "Icon_ToolTip";

    private static readonly Unit s_iconWidth = Unit.Pixel(16);
    private static readonly Unit s_iconHeight = Unit.Pixel(12);
    private static readonly IconInfo s_titleIcon = new IconInfo(c_Icon_Url, c_IconAlternateText, c_Icon_ToolTip, s_iconWidth, s_iconHeight);

    private Mock<IDropDownMenu> _control;
    private Mock<HttpContextBase> _httpContextStub;
    private HtmlHelper _htmlHelper;
    private FakeResourceUrlFactory _resourceUrlFactory;

    [SetUp]
    public void SetUp ()
    {
      _htmlHelper = new HtmlHelper();
      _httpContextStub = new Mock<HttpContextBase>();

      _control = new Mock<IDropDownMenu>();
      _control.SetupProperty(_ => _.ID);
      _control.Object.ID = "DropDownMenu1";
      _control.Setup(stub => stub.Enabled).Returns(true);
      _control.Setup(stub => stub.UniqueID).Returns("DropDownMenu1");
      _control.Setup(stub => stub.ClientID).Returns("DropDownMenu1");
      _control.SetupProperty(stub => stub.CssClass);
      _control.Setup(stub => stub.ControlType).Returns("DropDownMenu");
      _control.Setup(stub => stub.MenuItems).Returns(new WebMenuItemCollection(_control.Object));
      _control.Setup(stub => stub.GetBindOpenEventScript(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns("OpenDropDownMenuEventReference");
      _control.Setup(stub => stub.ResolveClientUrl(It.IsAny<string>())).Returns((string relativeUrl) => (relativeUrl.TrimStart('~')));

      var pageStub = new Mock<IPage>();
      _control.Setup(stub => stub.Page).Returns(pageStub.Object);

      StateBag stateBag = new StateBag();
      _control.Setup(stub => stub.Attributes).Returns(new AttributeCollection(stateBag));
      _control.Setup(stub => stub.ControlStyle).Returns(new Style(stateBag));

      _control.Setup(stub => stub.GetResourceManager()).Returns(NullResourceManager.Instance);

      var scriptManagerMock = new Mock<IClientScriptManager>();
      Mock.Get(_control.Object.Page).Setup(stub => stub.ClientScript).Returns(scriptManagerMock.Object);
      _resourceUrlFactory = new FakeResourceUrlFactory();
    }

    [Test]
    public void RenderEmptyMenuWithoutTitle ()
    {
      _control.Setup(stub => stub.ShowTitle).Returns(true);
      XmlNode containerDiv = GetAssertedContainerSpan();
      AssertTitleSpan(containerDiv, false, false);
    }

    [Test]
    public void RenderEmptyMenuWithTitle ()
    {
      _control.Setup(stub=>stub.TitleText).Returns(WebString.CreateFromText(c_MenuTitle));
      _control.Setup(stub => stub.ShowTitle).Returns(true);

      XmlNode containerDiv = GetAssertedContainerSpan();
      AssertTitleSpan(containerDiv, true, false);
    }

    [Test]
    public void RenderEmptyMenuWithTitleAndIcon ()
    {
      _control.Setup(stub => stub.TitleText).Returns(WebString.CreateFromText(c_MenuTitle));
      _control.Setup(stub => stub.TitleIcon).Returns(s_titleIcon);
      _control.Setup(stub => stub.ShowTitle).Returns(true);

      XmlNode containerDiv = GetAssertedContainerSpan();
      AssertTitleSpan(containerDiv, true, true);
    }

    [Test]
    public void RenderPopulatedMenu ()
    {
      _control.Setup(stub => stub.TitleText).Returns(WebString.CreateFromText(c_MenuTitle));
      _control.Setup(stub => stub.ShowTitle).Returns(true);
      PopulateMenu();

      XmlNode containerDiv = GetAssertedContainerSpan();
      AssertTitleSpan(containerDiv, true, false);
    }

    [Test]
    public void RenderMenuWithHiddenTitleAndOnlyText ()
    {
      _control.Setup(stub => stub.TitleText).Returns(WebString.CreateFromText(c_MenuTitle));
      _control.Setup(stub => stub.ShowTitle).Returns(false);

      PopulateMenu();

      XmlNode containerDiv = GetAssertedContainerSpan();
      AssertTitleSpanWithHiddenTitle(containerDiv, true, false);
    }

    [Test]
    public void RenderMenuWithHiddenTitleAndOnlyIcon ()
    {
      _control.Setup(stub => stub.TitleText).Returns(WebString.Empty);
      _control.Setup(stub => stub.TitleIcon).Returns(s_titleIcon);
      _control.Setup(stub => stub.ShowTitle).Returns(false);

      PopulateMenu();

      XmlNode containerDiv = GetAssertedContainerSpan();
      AssertTitleSpanWithHiddenTitle(containerDiv, false, true);
    }

    [Test]
    public void RenderMenuWithHiddenTitleAndTextAndIcon ()
    {
      _control.Setup(stub => stub.TitleText).Returns(WebString.CreateFromText(c_MenuTitle));
      _control.Setup(stub => stub.TitleIcon).Returns(s_titleIcon);
      _control.Setup(stub => stub.ShowTitle).Returns(false);

      PopulateMenu();

      XmlNode containerDiv = GetAssertedContainerSpan();
      AssertTitleSpanWithHiddenTitle(containerDiv, true, false);
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes ()
    {
      _control.Setup(stub => stub.TitleText).Returns(WebString.CreateFromText(c_MenuTitle));
      _control.Setup(stub => stub.ShowTitle).Returns(true);
      PopulateMenu();

      var document = RenderDropDownMenu(RenderingFeatures.WithDiagnosticMetadata);

      var containerDiv = document.GetAssertedChildElement("span", 0);
      containerDiv.AssertAttributeValueEquals(DiagnosticMetadataAttributes.ControlType, "DropDownMenu");
      containerDiv.AssertAttributeValueEquals(DiagnosticMetadataAttributes.Content, c_MenuTitle);
      containerDiv.AssertAttributeValueEquals(DiagnosticMetadataAttributes.IsDisabled, (!_control.Object.Enabled).ToString().ToLower());
    }

    [Test]
    public void RenderWithPrimaryButtonType ()
    {
      _control.Setup(stub => stub.ButtonType).Returns(ButtonType.Primary);

      GetAssertedContainerSpan(ButtonType.Primary);
    }

    [Test]
    public void RenderWithSupplementalButtonType ()
    {
      _control.Setup(stub => stub.ButtonType).Returns(ButtonType.Supplemental);

      GetAssertedContainerSpan(ButtonType.Supplemental);
    }

    [Test]
    public void RenderWebStrings ()
    {
      _control.Setup(stub => stub.TitleText).Returns(WebString.CreateFromText("Multiline\nTitle"));

      var node = GetAssertedContainerSpan();

      var titleNode = node.GetAssertedElementByID("DropDownMenu1_DropDownMenuLabel");
      Assert.That(titleNode.InnerXml, Is.EqualTo("Multiline<br />Title"));
    }

    [Test]
    public void RenderCustomCssClass ()
    {
      _control.Setup(stub => stub.CssClass).Returns("myDropDownMenu");

      var document = RenderDropDownMenu(RenderingFeatures.Default);

      var containerDiv = document.GetAssertedChildElement("span", 0);
      containerDiv.AssertAttributeValueEquals("class", "myDropDownMenu");
    }

    [Test]
    public void RenderCustomCssClassWithPrimaryButtonType ()
    {
      _control.Setup(stub => stub.ButtonType).Returns(ButtonType.Primary);
      _control.Object.CssClass = "myDropDownMenu";

      var document = RenderDropDownMenu(RenderingFeatures.Default);

      var containerDiv = document.GetAssertedChildElement("span", 0);
      containerDiv.AssertAttributeValueEquals("class", "myDropDownMenu primary");
    }

    [Test]
    public void RenderCustomCssClassWithSupplementalButtonType ()
    {
      _control.Setup(stub => stub.ButtonType).Returns(ButtonType.Supplemental);
      _control.Object.CssClass = "myDropDownMenu";

      var document = RenderDropDownMenu(RenderingFeatures.Default);

      var containerDiv = document.GetAssertedChildElement("span", 0);
      containerDiv.AssertAttributeValueEquals("class", "myDropDownMenu supplemental");
    }

    [Test]
    public void RenderCustomClassAttribute ()
    {
      _control.Object.Attributes["class"] = "testClass";

      var document = RenderDropDownMenu(RenderingFeatures.Default);

      var containerDiv = document.GetAssertedChildElement("span", 0);
      containerDiv.AssertAttributeValueEquals("class", "testClass");
    }

    private void AssertTitleSpan (XmlNode containerDiv, bool withTitle, bool withIcon)
    {
      var hasTitle = withTitle || withIcon;
      var titleDiv = containerDiv.GetAssertedChildElement("span", 0);
      titleDiv.AssertAttributeValueEquals("class", "DropDownMenuSelect");
      titleDiv.AssertChildElementCount(2);

      AssertTitleAnchor(titleDiv, withTitle, withIcon);
      AssertDropDownButton(titleDiv, hasTitle);
    }

    private void AssertTitleSpanWithHiddenTitle (XmlNode containerDiv, bool withTitle, bool withIcon)
    {
      var titleDiv = containerDiv.GetAssertedChildElement("span", 0);
      titleDiv.AssertAttributeValueEquals("class", "DropDownMenuSelect");
      titleDiv.AssertChildElementCount(2);

      AssertHiddenTitle(titleDiv, withTitle, withIcon);
      AssertDropDownButton(titleDiv, false);
    }

    private void AssertDropDownButton (XmlNode titleDiv, bool hasTitle)
    {
      var buttonAnchor = titleDiv.GetAssertedChildElement("a", 1);
      if (hasTitle)
      {
        buttonAnchor.AssertNoAttribute("id");
        buttonAnchor.AssertNoAttribute("role");
        buttonAnchor.AssertAttributeValueEquals("aria-hidden", "true");
      }
      else
      {
        buttonAnchor.AssertAttributeValueEquals("id", _control.Object.ClientID + "_DropDownMenuButton");
        buttonAnchor.AssertAttributeValueEquals("role", "button");
        buttonAnchor.AssertAttributeValueEquals(StubLabelReferenceRenderer.LabelReferenceAttribute, _control.Object.ClientID + "_DropDownMenuLabel");
        buttonAnchor.AssertAttributeValueEquals(StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute, "");
        buttonAnchor.AssertNoAttribute("aria-hidden");
      }
      var image = buttonAnchor.GetAssertedChildElement("img", 0);
      image.AssertAttributeValueEquals("src", IconInfo.CreateSpacer(_resourceUrlFactory).Url);
      image.AssertAttributeValueEquals("alt", "");
    }

    private void AssertHiddenTitle (XmlNode titleDiv, bool withTitle, bool withIcon)
    {
      if (!(withTitle || withIcon))
        return;

      var titleBody = titleDiv.GetAssertedChildElement("span", 0);
      titleBody.AssertAttributeValueEquals("hidden", "hidden");
      if (withTitle)
      {
        titleBody.AssertTextNode(c_MenuTitle, 0);
      }
      else
      {
        titleBody.AssertTextNode(c_IconAlternateText, 0);
      }
    }

    private void AssertTitleAnchor (XmlNode titleDiv, bool withTitle, bool withIcon)
    {
      if (!(withTitle || withIcon))
        return;

      var titleAnchor = titleDiv.GetAssertedChildElement("a", 0);
      titleAnchor.AssertAttributeValueEquals("href", "fakeFallbackUrl");
      titleAnchor.AssertNoAttribute("onclick");
      titleAnchor.AssertAttributeValueEquals(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.Button);
      titleAnchor.AssertChildElementCount(withIcon ? 1 : 0);
      if (withTitle)
        titleAnchor.AssertTextNode(c_MenuTitle, withIcon ? 1 : 0);

      if (withIcon)
      {
        var icon = titleAnchor.GetAssertedChildElement("img", 0);
        icon.AssertAttributeValueEquals("src", c_Icon_Url);
        icon.AssertAttributeValueEquals("alt", c_IconAlternateText);
        icon.AssertStyleAttribute("width", s_iconWidth.ToString());
        icon.AssertStyleAttribute("height", s_iconHeight.ToString());
      }
    }

    private XmlNode GetAssertedContainerSpan (ButtonType buttonType = ButtonType.Standard)
    {
      var buttonClass = buttonType switch {
              ButtonType.Standard => "DropDownMenuContainer",
              ButtonType.Primary => "DropDownMenuContainer primary",
              ButtonType.Supplemental => "DropDownMenuContainer supplemental",
              _ => throw new ArgumentOutOfRangeException(nameof(buttonType), buttonType, null)
          };

      var document = RenderDropDownMenu(RenderingFeatures.Default);

      var containerDiv = document.GetAssertedChildElement("span", 0);
      containerDiv.AssertAttributeValueEquals("id", _control.Object.ClientID);
      containerDiv.AssertAttributeValueEquals("class", buttonClass);
      containerDiv.AssertChildElementCount(1);

      return containerDiv;
    }

    private XmlDocument RenderDropDownMenu (IRenderingFeatures renderingFeatures)
    {
      var renderer = new DropDownMenuRenderer(
          _resourceUrlFactory,
          GlobalizationService,
          renderingFeatures,
          new StubLabelReferenceRenderer(),
          new FakeFallbackNavigationUrlProvider());
      renderer.Render(new DropDownMenuRenderingContext(_httpContextStub.Object, _htmlHelper.Writer, _control.Object));

      return _htmlHelper.GetResultDocument();
    }

    private void PopulateMenu ()
    {
      AddItem(0, "Category1", CommandType.Event, false, true);
      AddItem(1, "Category1", CommandType.Href, false, true);
      AddItem(2, "Category2", CommandType.WxeFunction, false, true);
      AddItem(3, "Category2", CommandType.WxeFunction, true, true);
      AddItem(4, "Category2", CommandType.WxeFunction, false, false);
    }

    private void AddItem (int index, string category, CommandType commandType, bool isDisabled, bool isVisible)
    {
      string id = "item" + index;
      string text = "Item" + index;
      const string height = "16";
      const string width = "16";
      const string iconUrl = "~/Images/Icon.gif";
      const string disabledIconUrl = "~/Images/DisabledIcon.gif";
      const RequiredSelection requiredSelection = RequiredSelection.Any;

      Command command = new Command(commandType);
      if (commandType == CommandType.Href)
      {
        command.HrefCommand.Href = "~/Target.aspx?index={0}&itemID={1}";
        command.HrefCommand.Target = "_blank";
      }

      _control.Object.MenuItems.Add(
          new WebMenuItem(
              id,
              category,
              WebString.CreateFromText(text),
              new IconInfo(iconUrl, text, text, width, height),
              new IconInfo(disabledIconUrl, text, text, width, height),
              WebMenuItemStyle.IconAndText,
              requiredSelection,
              isDisabled,
              command) { IsVisible = isVisible });

      if (commandType == CommandType.Href)
      {
      }
      else
      {
        if (isVisible && _control.Object.Enabled)
        {
          Mock.Get(_control.Object.Page.ClientScript)
              .Setup(mock => mock.GetPostBackClientHyperlink(_control.Object, index.ToString()))
              .Returns("PostBackHyperLink:" + index)
              .Verifiable();
        }
      }
    }
  }
}
