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

      _clientScriptManagerMock.Setup(
          mock => mock.RegisterStartupScriptBlock(_control.Object, typeof (ListMenuRenderer), _control.Object.UniqueID + "_MenuItems", script)).Verifiable();

      var renderer = new ListMenuRenderer(new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.Default);
      renderer.Render(new ListMenuRenderingContext(_httpContextStub.Object, _htmlHelper.Writer, _control.Object));
      _clientScriptManagerMock.Verify();
    }


    [Test]
    public void RenderWithLineBreaksAll ()
    {
      SetUpGetPostBackLinkExpectations(true);
      _control.Setup(stub => stub.LineBreaks).Returns(ListMenuLineBreaks.All);

      XmlNode table = GetAssertedTable();

      for (int itemIndex = 0; itemIndex < 5; itemIndex++)
      {
        if (itemIndex == 3)
          continue;

        var tr = table.GetAssertedChildElement("tr", itemIndex < 3 ? itemIndex : itemIndex - 1);
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
      var tr1 = table.GetAssertedChildElement("tr", 0);
      tr1.AssertChildElementCount(1);
      var td1 = GetAssertedCell(tr1, 0, 2);

      AssertMenuItem(td1, 0, 0, "0");
      AssertMenuItem(td1, 1, 1, "-1");

      var tr2 = table.GetAssertedChildElement("tr", 1);
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
      var tr = table.GetAssertedChildElement("tr", 0);
      tr.AssertChildElementCount(1);

      var td = GetAssertedCell(tr, 0, 4);
      for (int iColumn = 0; iColumn < 4; iColumn++)
        AssertMenuItem(td, iColumn < 3 ? iColumn : iColumn + 1, iColumn, iColumn == 0 ? "0" : "-1");
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes ()
    {
      SetUpGetPostBackLinkExpectations(true);

      var table = GetAssertedTable();
      table.AssertAttributeValueEquals(DiagnosticMetadataAttributes.ControlType, "ListMenu");
      table.AssertAttributeValueEquals(DiagnosticMetadataAttributes.IsDisabled, (!_control.Object.Enabled).ToString().ToLower());
    }

    private XmlNode GetAssertedTable ()
    {
      var renderer = new ListMenuRenderer(new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.WithDiagnosticMetadata);
      renderer.Render(new ListMenuRenderingContext(_httpContextStub.Object, _htmlHelper.Writer, _control.Object));

      var document = _htmlHelper.GetResultDocument();

      var table = _htmlHelper.GetAssertedChildElement(document, "table", 0);
      table.AssertAttributeValueEquals("id", _control.Object.ClientID);
      table.AssertAttributeValueEquals("class", _control.Object.CssClass);
      table.AssertAttributeValueEquals("role", "menu");
      return table;
    }

    private void AssertMenuItem (XmlNode parentCell, int itemIndex, int nodeIndex, string tabIndex)
    {
      var item = (WebMenuItem) _control.Object.MenuItems[itemIndex];

      switch (item.Style)
      {
        case WebMenuItemStyle.IconAndText:
          AssertIconAndText(itemIndex, parentCell, item, nodeIndex, tabIndex);
          break;
        case WebMenuItemStyle.Text:
          AssertText(itemIndex, parentCell, item, nodeIndex, tabIndex);
          break;
        case WebMenuItemStyle.Icon:
          AssertIcon(itemIndex, parentCell, nodeIndex, tabIndex);
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

    private void AssertIcon (int itemIndex, XmlNode parent, int nodeIndex, string tabIndex)
    {
      XmlNode a = GetAssertedItemLink(parent, itemIndex, nodeIndex, tabIndex);
      AssertIcon(a);
    }

    private void AssertText (int itemIndex, XmlNode parent, WebMenuItem item, int nodeIndex, string tabIndex)
    {
      XmlNode a = GetAssertedItemLink(parent, itemIndex, nodeIndex, tabIndex);
      var span = a.GetAssertedChildElement("span", 0);
      span.AssertTextNode(item.Text, 0);
    }

    private void AssertIconAndText (int itemIndex, XmlNode td, WebMenuItem item, int nodeIndex, string tabIndex)
    {
      XmlNode a = GetAssertedItemLink(td, itemIndex, nodeIndex, tabIndex);
      AssertIcon(a);

      var span = a.GetAssertedChildElement("span", 1);
      span.AssertTextNode(item.Text, 0);
    }

    private void AssertIcon (XmlNode parent)
    {
      var img = parent.GetAssertedChildElement("img", 0);
      img.AssertAttributeValueContains("src", "/Images/ClassicBlue/NullIcon.gif");
    }

    private XmlNode GetAssertedItemLink (XmlNode td, int itemIndex, int nodeIndex, string tabIndex)
    {
      var span = td.GetAssertedChildElement("span", nodeIndex);
      span.AssertAttributeValueEquals("id", _control.Object.ClientID + "_" + itemIndex);
      span.AssertChildElementCount(1);

      var anchor = span.GetAssertedChildElement("a", 0);
      anchor.AssertAttributeValueEquals("role", "menuitem");
      anchor.AssertAttributeValueEquals("tabindex", tabIndex);
      return anchor;
    }

    private void PopulateMenu ()
    {
      AddMenuItem("item 1", "category 1", "Event", WebMenuItemStyle.IconAndText, RequiredSelection.Any, CommandType.Event);
      AddMenuItem("item 2", "category 1", "WxeFunction", WebMenuItemStyle.Text, RequiredSelection.OneOrMore, CommandType.WxeFunction);
      AddMenuItem("item 3", "category 2", "Href", WebMenuItemStyle.Icon, RequiredSelection.ExactlyOne, CommandType.Href);
      ((WebMenuItem) _control.Object.MenuItems[2]).Command.HrefCommand.Href = "/LinkedPage.html";
      ((WebMenuItem) _control.Object.MenuItems[2]).Command.HrefCommand.Target = "_blank";
      AddMenuItem("invisible item", "category 2", "Href", WebMenuItemStyle.IconAndText, RequiredSelection.ExactlyOne, CommandType.Href);
      ((WebMenuItem) _control.Object.MenuItems[3]).IsVisible = false;
      AddMenuItem("disabled item", "category 2", "Href", WebMenuItemStyle.IconAndText, RequiredSelection.ExactlyOne, CommandType.Href);
      ((WebMenuItem) _control.Object.MenuItems[4]).IsDisabled = true;
    }

    private void SetUpGetPostBackLinkExpectations (bool withHrefItem)
    {
      _clientScriptManagerMock.Setup(mock => mock.GetPostBackClientHyperlink(_control.Object, "0")).Returns("PostBackLink: 0").Verifiable();
      _clientScriptManagerMock.Setup(mock => mock.GetPostBackClientHyperlink(_control.Object, "1")).Returns("PostBackLink: 1").Verifiable();
      if (withHrefItem)
        _clientScriptManagerMock.Setup(mock => mock.GetPostBackClientHyperlink(_control.Object, "2")).Returns("PostBackLink: 2").Verifiable();
    }

    private void AddMenuItem (
        string itemID,
        string category,
        string text,
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
      const string itemTemplate = "new ListMenuItemInfo ('{0}', '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10})";
      var menuItem = (WebMenuItem) _control.Object.MenuItems[itemIndex];
      const string diagnosticMetadata = "null";
      const string diagnosticMetadataForCommand = "null";

      string href;
      string target = "null";

      if (menuItem.Command.Type == CommandType.Href)
      {
        href = menuItem.Command.HrefCommand.FormatHref(itemIndex.ToString(), menuItem.ItemID);
        href = "'" + href + "'";
        target = "'" + menuItem.Command.HrefCommand.Target + "'";
      }
      else
      {
        string argument = itemIndex.ToString();
        href = _control.Object.Page.ClientScript.GetPostBackClientHyperlink(_control.Object, argument);
        href = ScriptUtility.EscapeClientScript(href);
        href = "'" + href + "'";
      }

      return string.Format(
          itemTemplate,
          _control.Object.ClientID + "_" + itemIndex,
          menuItem.Category,
          menuItem.Style != WebMenuItemStyle.Icon ? "'" + menuItem.Text + "'" : "null",
          menuItem.Style != WebMenuItemStyle.Text ? "'" + menuItem.Icon.Url.TrimStart('~') + "'" : "null",
          menuItem.Style != WebMenuItemStyle.Text ? "'" + menuItem.DisabledIcon.Url.TrimStart('~') + "'" : "null",
          (int) menuItem.RequiredSelection,
          (itemIndex == 4) ? "true" : "false",
          href,
          target,
          diagnosticMetadata,
          diagnosticMetadataForCommand);
    }
  }
}