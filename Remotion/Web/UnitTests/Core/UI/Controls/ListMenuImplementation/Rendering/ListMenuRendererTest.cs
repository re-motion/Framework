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
using System.Xml;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.ServiceLocation;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.ListMenuImplementation;
using Remotion.Web.UI.Controls.ListMenuImplementation.Rendering;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.Utilities;

namespace Remotion.Web.UnitTests.Core.UI.Controls.ListMenuImplementation.Rendering
{
  [TestFixture]
  public class ListMenuRendererTest : RendererTestBase
  {
    private Mock<IListMenu> _control;
    private Mock<IClientScriptManager> _clientScriptManagerMock;
    private Mock<HttpContextBase> _httpContextStub;
    private HtmlHelper _htmlHelper;
    private ServiceLocatorScope _serviceLocatorScope;

    [SetUp]
    public void SetUp ()
    {
      _htmlHelper = new HtmlHelper();
      _httpContextStub = new Mock<HttpContextBase>();

      _control = new Mock<IListMenu>();
      _control.Setup(stub => stub.UniqueID).Returns("MyListMenu");
      _control.Setup(stub => stub.ClientID).Returns("MyListMenu");
      _control.Setup(stub => stub.ControlType).Returns("ListMenu");
      _control.Setup(stub => stub.MenuItems).Returns(new WebMenuItemCollection(_control.Object));

      _control.Setup(stub => stub.Enabled).Returns(true);
      _control.Setup(stub => stub.HasClientScript).Returns(true);
      _control.Setup(stub => stub.ResolveClientUrl(It.IsAny<string>())).Returns((string relativeUrl) => relativeUrl.TrimStart('~'));
      _control.Setup(stub => stub.GetUpdateScriptReference("null")).Returns("Update();");

      var pageStub = new Mock<IPage>();

      _clientScriptManagerMock = new Mock<IClientScriptManager>();
      pageStub.Setup(page => page.ClientScript).Returns(_clientScriptManagerMock.Object);

      _control.Setup(stub => stub.Page).Returns(pageStub.Object);

      PopulateMenu();

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle<IRenderingFeatures>(() => RenderingFeatures.WithDiagnosticMetadata);
      _serviceLocatorScope = new ServiceLocatorScope(serviceLocator);
    }

    [TearDown]
    public void TearDown ()
    {
      if (_serviceLocatorScope != null)
        _serviceLocatorScope.Dispose();
    }

    [Test]
    public void Render_RegistersMenuItems ()
    {
      SetUpGetPostBackLinkExpectations(false);

      string script = "ListMenu.Initialize ('#{0}');\r\n" +
                      "ListMenu.AddMenuInfo ('#{0}', \r\n\tnew ListMenu_MenuInfo ('{0}', new Array (\r\n" +
                      "\t\t{1},\r\n\t\t{2},\r\n\t\t{3},\r\n\t\t{4} ) ) );\r\n" +
                      "Update();";

      script = string.Format(script, _control.Object.ClientID, GetItemScript(0), GetItemScript(1), GetItemScript(2), GetItemScript(4));

      _clientScriptManagerMock
          .Setup(mock => mock.RegisterStartupScriptBlock(_control.Object, typeof(ListMenuRenderer), _control.Object.UniqueID + "_MenuItems", script))
          .Verifiable();

      var renderer = new ListMenuRenderer(new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.Default, new FakeFallbackNavigationUrlProvider());
      renderer.Render(new ListMenuRenderingContext(_httpContextStub.Object, _htmlHelper.Writer, _control.Object));
      _clientScriptManagerMock.Verify();
    }


    [Test]
    public void RenderWithLineBreaksAll ()
    {
      SetUpGetPostBackLinkExpectations(true);
      _control.Setup(stub => stub.LineBreaks).Returns(ListMenuLineBreaks.All);

      XmlNode table = GetAssertedTable();

      var tbody = table.GetAssertedChildElement("tbody", 0);

      for (int itemIndex = 0; itemIndex < 5; itemIndex++)
      {
        if (itemIndex == 3)
          continue;

        var tr = tbody.GetAssertedChildElement("tr", itemIndex < 3 ? itemIndex : itemIndex - 1);
        tr.AssertChildElementCount(1);
        var td = GetAssertedCell(tr, 0, 1);
        AssertMenuItem(td, itemIndex, 0, itemIndex == 0 ? "0" : "-1");
      }
    }

    [Test]
    public void RenderWithLineBreaksGroup ()
    {
      SetUpGetPostBackLinkExpectations(true);
      _control.Setup(stub => stub.LineBreaks).Returns(ListMenuLineBreaks.BetweenGroups);

      var table = GetAssertedTable();
      var tbody = table.GetAssertedChildElement("tbody", 0);

      var tr1 = tbody.GetAssertedChildElement("tr", 0);
      tr1.AssertChildElementCount(1);
      var td1 = GetAssertedCell(tr1, 0, 2);

      AssertMenuItem(td1, 0, 0, "0");
      AssertMenuItem(td1, 1, 1, "-1");

      var tr2 = tbody.GetAssertedChildElement("tr", 1);
      tr2.AssertChildElementCount(1);

      var td2 = GetAssertedCell(tr2, 0, 2);

      AssertMenuItem(td2, 2, 0, "-1");
      AssertMenuItem(td2, 4, 1, "-1");
    }

    [Test]
    public void RenderWithLineBreaksNone ()
    {
      SetUpGetPostBackLinkExpectations(true);
      _control.Setup(stub => stub.LineBreaks).Returns(ListMenuLineBreaks.None);

      var table = GetAssertedTable();

      var tbody = table.GetAssertedChildElement("tbody", 0);

      var tr = tbody.GetAssertedChildElement("tr", 0);
      tr.AssertChildElementCount(1);

      var td = GetAssertedCell(tr, 0, 4);
      for (int iColumn = 0; iColumn < 4; iColumn++)
        AssertMenuItem(td, iColumn < 3 ? iColumn : iColumn + 1, iColumn, iColumn == 0 ? "0" : "-1");
    }

    [Test]
    public void Render_HasRoleNoneOnAllChildElementsBetweenRoleToolbarAndRoleButton ()
    {
      SetUpGetPostBackLinkExpectations(false);
      _control.Setup(stub => stub.LineBreaks).Returns(ListMenuLineBreaks.BetweenGroups);

      var table = GetAssertedTable();
      table.AssertAttributeValueEquals("role", "toolbar");

      var tbody = table.GetAssertedChildElement("tbody", 0);
      tbody.AssertAttributeValueEquals("role", "none");

      var tr = tbody.GetAssertedChildElement("tr", 0);
      tr.AssertAttributeValueEquals("role", "none");
      tr.AssertChildElementCount(1);

      var td = tr.GetAssertedChildElement("td", 0);
      td.AssertAttributeValueEquals("role", "none");

      var span = td.GetAssertedChildElement("span", 0);
      span.AssertAttributeValueEquals("role", "none");

      var a = span.GetAssertedChildElement("a", 0);
      a.AssertAttributeValueEquals("role", "button");
    }

    [Test]
    public void RenderWithEncodedWebString ()
    {
      SetUpGetPostBackLinkExpectations(true);
      _control.Setup(stub => stub.LineBreaks).Returns(ListMenuLineBreaks.None);

      _control.Object.MenuItems.Clear();
      var textItemContent = WebString.CreateFromText("this should be <b>text</b>");
      AddMenuItem("item text", "category 1", textItemContent, WebMenuItemStyle.Text, RequiredSelection.Any, CommandType.None);
      var htmlItemContent = WebString.CreateFromHtml("this should be <b>text</b>");
      AddMenuItem("item html", "category 1", htmlItemContent, WebMenuItemStyle.Text, RequiredSelection.Any, CommandType.None);

      var table = GetAssertedTable();
      var tbody = table.GetAssertedChildElement("tbody", 0);
      tbody.AssertChildElementCount(1);

      var tr = tbody.GetAssertedChildElement("tr", 0);
      tr.AssertChildElementCount(1);

      var td = GetAssertedCell(tr, 0, 2);
      for (int iColumn = 0; iColumn < 2; iColumn++)
      {
        XmlNode a = GetAssertedItemLink(td, iColumn, iColumn, iColumn == 0 ? "0" : "-1", isButtonRole: true);
        var span = a.GetAssertedChildElement("span", 0);
        if (iColumn == 0)
        {
          span.AssertTextNode(textItemContent.GetValue(), 0);
        }
        else
        {
          span.AssertChildElementCount(1);
          span.AssertTextNode("this should be", 0);
          var b = span.GetAssertedChildElement("b", 1);
          b.AssertTextNode("text", 0);
        }
      }
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes ()
    {
      SetUpGetPostBackLinkExpectations(true);

      var wrapper = GetAssertedWrapper();
      wrapper.AssertAttributeValueEquals(DiagnosticMetadataAttributes.ControlType, "ListMenu");
      wrapper.AssertAttributeValueEquals(DiagnosticMetadataAttributes.IsDisabled, (!_control.Object.Enabled).ToString().ToLower());
    }

    [Test]
    public void Render_WithNoHeading_NoHeaderAndNoLabelledBy ()
    {
      SetUpGetPostBackLinkExpectations(true);

      _control.Setup(e => e.Heading).Returns(WebString.Empty);
      var wrapper = GetAssertedWrapper();

      wrapper.AssertNoAttribute("aria-labelledby");
      wrapper.AssertChildElementCount(1);
    }

    [Test]
    public void Render_WithHeadingButNoHeadingLevel_RendersAndLinksSpan ()
    {
      SetUpGetPostBackLinkExpectations(true);

      _control.Setup(e => e.Heading).Returns(WebString.CreateFromText("My heading"));
      _control.Setup(e => e.HeadingLevel).Returns((HeadingLevel?)null);
      var wrapper = GetAssertedWrapper();

      wrapper.AssertAttributeValueEquals("aria-labelledby", "MyListMenu_Heading");

      var span = wrapper.GetAssertedChildElement("span", 0);
      span.AssertAttributeValueEquals("id", "MyListMenu_Heading");
      span.AssertNoAttribute("class");
      span.AssertAttributeValueEquals("hidden", "hidden");
      Assert.That(span.InnerText, Is.EqualTo("My heading"));
    }

    [Test]
    public void Render_WithHeadingAndHeadingLevel_RendersAndLinksHxElement ()
    {
      SetUpGetPostBackLinkExpectations(true);

      _control.Setup(e => e.Heading).Returns(WebString.CreateFromText("My heading"));
      _control.Setup(e => e.HeadingLevel).Returns(HeadingLevel.H2);
      var wrapper = GetAssertedWrapper();

      wrapper.AssertAttributeValueEquals("aria-labelledby", "MyListMenu_Heading");

      var h2 = wrapper.GetAssertedChildElement("h2", 0);
      h2.AssertAttributeValueEquals("id", "MyListMenu_Heading");
      h2.AssertAttributeValueEquals("class", "screenReaderText");
      h2.AssertNoAttribute("hidden");
      h2.AssertTextNode("My heading", 0);
    }

    private XmlNode GetAssertedWrapper ()
    {
      var renderer = new ListMenuRenderer(new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.WithDiagnosticMetadata, new FakeFallbackNavigationUrlProvider());
      renderer.Render(new ListMenuRenderingContext(_httpContextStub.Object, _htmlHelper.Writer, _control.Object));

      var document = _htmlHelper.GetResultDocument();

      var wrapper = _htmlHelper.GetAssertedChildElement(document, "div", 0);
      wrapper.AssertAttributeValueEquals("id", _control.Object.ClientID);
      wrapper.AssertAttributeValueEquals("class", _control.Object.CssClass);
      wrapper.AssertAttributeValueEquals("role", "region");
      return wrapper;
    }

    private XmlNode GetAssertedTable ()
    {
      var wrapper = GetAssertedWrapper();

      var table = _htmlHelper.GetAssertedChildElement(wrapper, "table", 0);
      table.AssertAttributeValueEquals("role", "toolbar");
      return table;
    }

    private void AssertMenuItem (XmlNode parentCell, int itemIndex, int nodeIndex, string tabIndex)
    {
      var item = (WebMenuItem)_control.Object.MenuItems[itemIndex];
      Assert.That(_control.Object.Enabled, Is.True, "ListMenu is disabled. Only enabled ListMenu are supported by the test.");
      var isButtonRole = item.Command.Type != CommandType.Href || item.IsDisabled;

      switch (item.Style)
      {
        case WebMenuItemStyle.IconAndText:
          AssertIconAndText(itemIndex, parentCell, item, nodeIndex, tabIndex, isButtonRole);
          break;
        case WebMenuItemStyle.Text:
          AssertText(itemIndex, parentCell, item, nodeIndex, tabIndex, isButtonRole);
          break;
        case WebMenuItemStyle.Icon:
          AssertIcon(itemIndex, parentCell, nodeIndex, tabIndex, isButtonRole);
          break;
      }
    }

    private XmlNode GetAssertedCell (XmlNode parentRow, int cellIndex, int itemCount)
    {
      var td = parentRow.GetAssertedChildElement("td", cellIndex);
      td.AssertAttributeValueEquals("class", "listMenuRow");
      td.AssertAttributeValueEquals("role", "none");
      td.AssertStyleAttribute("width", "100%");
      td.AssertChildElementCount(itemCount);
      return td;
    }

    private void AssertIcon (int itemIndex, XmlNode parent, int nodeIndex, string tabIndex, bool isButtonRole)
    {
      XmlNode a = GetAssertedItemLink(parent, itemIndex, nodeIndex, tabIndex, isButtonRole);
      AssertIcon(a);
    }

    private void AssertText (int itemIndex, XmlNode parent, WebMenuItem item, int nodeIndex, string tabIndex, bool isButtonRole)
    {
      XmlNode a = GetAssertedItemLink(parent, itemIndex, nodeIndex, tabIndex, isButtonRole);
      var span = a.GetAssertedChildElement("span", 0);
      span.AssertTextNode(item.Text.ToString(WebStringEncoding.HtmlWithTransformedLineBreaks), 0);
    }

    private void AssertIconAndText (int itemIndex, XmlNode td, WebMenuItem item, int nodeIndex, string tabIndex, bool isButtonRole)
    {
      XmlNode a = GetAssertedItemLink(td, itemIndex, nodeIndex, tabIndex, isButtonRole);
      AssertIcon(a);

      var span = a.GetAssertedChildElement("span", 1);
      span.AssertTextNode(item.Text.ToString(WebStringEncoding.HtmlWithTransformedLineBreaks), 0);
    }

    private void AssertIcon (XmlNode parent)
    {
      var img = parent.GetAssertedChildElement("img", 0);
      img.AssertAttributeValueContains("src", "/Images/ClassicBlue/NullIcon.gif");
    }

    private XmlNode GetAssertedItemLink (XmlNode td, int itemIndex, int nodeIndex, string tabIndex, bool isButtonRole)
    {
      var span = td.GetAssertedChildElement("span", nodeIndex);
      span.AssertAttributeValueEquals("id", _control.Object.ClientID + "_" + itemIndex);
      span.AssertChildElementCount(1);

      var anchor = span.GetAssertedChildElement("a", 0);
      if (isButtonRole)
        anchor.AssertAttributeValueEquals("role", "button");
      else
        anchor.AssertNoAttribute("role");
      anchor.AssertAttributeValueEquals("tabindex", tabIndex);
      return anchor;
    }

    private void PopulateMenu ()
    {
      AddMenuItem("item 1", "category 1", WebString.CreateFromText("Event"), WebMenuItemStyle.IconAndText, RequiredSelection.Any, CommandType.Event);
      AddMenuItem("item 2", "category 1", WebString.CreateFromText("WxeFunction"), WebMenuItemStyle.Text, RequiredSelection.OneOrMore, CommandType.WxeFunction);
      AddMenuItem("item 3", "category 2", WebString.CreateFromText("Href"), WebMenuItemStyle.Icon, RequiredSelection.ExactlyOne, CommandType.Href);
      ((WebMenuItem)_control.Object.MenuItems[2]).Command.HrefCommand.Href = "/LinkedPage.html";
      ((WebMenuItem)_control.Object.MenuItems[2]).Command.HrefCommand.Target = "_blank";
      AddMenuItem("invisible item", "category 2", WebString.CreateFromText("Href"), WebMenuItemStyle.IconAndText, RequiredSelection.ExactlyOne, CommandType.Href);
      ((WebMenuItem)_control.Object.MenuItems[3]).IsVisible = false;
      AddMenuItem("disabled item", "category 2", WebString.CreateFromText("Href"), WebMenuItemStyle.IconAndText, RequiredSelection.ExactlyOne, CommandType.Href);
      ((WebMenuItem)_control.Object.MenuItems[4]).IsDisabled = true;
    }

    private void SetUpGetPostBackLinkExpectations (bool withHrefItem)
    {
      _clientScriptManagerMock.Setup(mock => mock.GetPostBackEventReference(_control.Object, "0")).Returns("PostBackLink: 0").Verifiable();
      _clientScriptManagerMock.Setup(mock => mock.GetPostBackEventReference(_control.Object, "1")).Returns("PostBackLink: 1").Verifiable();
      if (withHrefItem)
        _clientScriptManagerMock.Setup(mock => mock.GetPostBackEventReference(_control.Object, "2")).Returns("PostBackLink: 2").Verifiable();
    }

    private void AddMenuItem (
        string itemID,
        string category,
        WebString text,
        WebMenuItemStyle style,
        RequiredSelection selection,
        CommandType commandType)
    {
      WebMenuItem item = new WebMenuItem(
          itemID,
          category,
          text,
          new IconInfo("~/Images/ClassicBlue/NullIcon.gif"),
          new IconInfo("~/Images/ClassicBlue/NullIcon.gif"),
          style,
          selection,
          false,
          new Command(commandType));

      _control.Object.MenuItems.Add(item);
    }

    private string GetItemScript (int itemIndex)
    {
      const string itemTemplate = "new ListMenuItemInfo ('{0}', '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11})";
      var menuItem = (WebMenuItem)_control.Object.MenuItems[itemIndex];
      const string diagnosticMetadata = "null";
      const string diagnosticMetadataForCommand = "null";

      string href;
      string target = "null";
      string onclick = "null";

      if (menuItem.Command.Type == CommandType.Href)
      {
        href = menuItem.Command.HrefCommand.FormatHref(itemIndex.ToString(), menuItem.ItemID);
        href = "'" + href + "'";
        if (!string.IsNullOrEmpty(menuItem.Command.HrefCommand.Target))
          target = "'" + menuItem.Command.HrefCommand.Target + "'";
      }
      else
      {
        href = "'fakeFallbackUrl'";
        onclick = "function() { PostBackLink: " + itemIndex.ToString() + "; return false; }";
      }

      return string.Format(
          itemTemplate,
          _control.Object.ClientID + "_" + itemIndex,
          menuItem.Category,
          menuItem.Style != WebMenuItemStyle.Icon ? "'" + menuItem.Text + "'" : "null",
          menuItem.Style != WebMenuItemStyle.Text ? "'" + menuItem.Icon.Url.TrimStart('~') + "'" : "null",
          menuItem.Style != WebMenuItemStyle.Text ? "'" + menuItem.DisabledIcon.Url.TrimStart('~') + "'" : "null",
          (int)menuItem.RequiredSelection,
          (itemIndex == 4) ? "true" : "false",
          href,
          target,
          onclick,
          diagnosticMetadata,
          diagnosticMetadataForCommand);
    }
  }
}
