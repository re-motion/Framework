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
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.UI.Controls.ListMenuImplementation.Rendering
{
  [TestFixture]
  public class ListMenuRendererTest : RendererTestBase
  {
    private IListMenu _control;
    private IClientScriptManager _clientScriptManagerMock;
    private HttpContextBase _httpContextStub;
    private HtmlHelper _htmlHelper;
    private ServiceLocatorScope _serviceLocatorScope;

    [SetUp]
    public void SetUp ()
    {
      _htmlHelper = new HtmlHelper();
      _httpContextStub = MockRepository.GenerateStub<HttpContextBase>();

      _control = MockRepository.GenerateStub<IListMenu>();
      _control.Stub (stub => stub.UniqueID).Return ("MyListMenu");
      _control.Stub (stub => stub.ClientID).Return ("MyListMenu");
      _control.Stub (stub => stub.ControlType).Return ("ListMenu");
      _control.Stub (stub => stub.MenuItems).Return (new WebMenuItemCollection (_control));

      _control.Stub (stub => stub.Enabled).Return (true);
      _control.Stub (stub => stub.HasClientScript).Return (true);
      _control.Stub (stub => stub.ResolveClientUrl (null)).IgnoreArguments().Do ((Func<string, string>) (url => url.TrimStart ('~')));
      _control.Stub (stub => stub.GetUpdateScriptReference ("null")).Return ("Update();");

      var pageStub = MockRepository.GenerateStub<IPage>();

      _clientScriptManagerMock = MockRepository.GenerateMock<IClientScriptManager>();
      pageStub.Stub (page => page.ClientScript).Return (_clientScriptManagerMock);

      _control.Stub (stub => stub.Page).Return (pageStub);

      PopulateMenu();

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle<IRenderingFeatures> (() => RenderingFeatures.WithDiagnosticMetadata);
      _serviceLocatorScope = new ServiceLocatorScope (serviceLocator);
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
      SetUpGetPostBackLinkExpectations (false);

      string script = "ListMenu_AddMenuInfo (document.getElementById ('{0}'), \r\n\tnew ListMenu_MenuInfo ('{0}', new Array (\r\n" +
                      "\t\t{1},\r\n\t\t{2},\r\n\t\t{3},\r\n\t\t{4} ) ) );\r\n" +
                      "Update();";

      script = string.Format (script, _control.ClientID, GetItemScript (0), GetItemScript (1), GetItemScript (2), GetItemScript (4));

      _clientScriptManagerMock.Expect (
          mock => mock.RegisterStartupScriptBlock (_control, typeof (ListMenuRenderer), _control.UniqueID + "_MenuItems", script));

      var renderer = new ListMenuRenderer (new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.Default);
      renderer.Render (new ListMenuRenderingContext (_httpContextStub, _htmlHelper.Writer, _control));
      _clientScriptManagerMock.VerifyAllExpectations();
    }


    [Test]
    public void RenderWithLineBreaksAll ()
    {
      SetUpGetPostBackLinkExpectations (true);
      _control.Stub (stub => stub.LineBreaks).Return (ListMenuLineBreaks.All);

      XmlNode table = GetAssertedTable();

      for (int itemIndex = 0; itemIndex < 5; itemIndex++)
      {
        if (itemIndex == 3)
          continue;

        var tr = table.GetAssertedChildElement ("tr", itemIndex < 3 ? itemIndex : itemIndex - 1);
        tr.AssertChildElementCount (1);
        var td = GetAssertedCell (tr, 0, 1);
        AssertMenuItem (td, itemIndex, 0);
      }
    }

    [Test]
    public void RenderWithLineBreaksGroup ()
    {
      SetUpGetPostBackLinkExpectations (true);
      _control.Stub (stub => stub.LineBreaks).Return (ListMenuLineBreaks.BetweenGroups);

      var table = GetAssertedTable();
      var tr1 = table.GetAssertedChildElement ("tr", 0);
      tr1.AssertChildElementCount (1);
      var td1 = GetAssertedCell (tr1, 0, 2);

      AssertMenuItem (td1, 0, 0);
      AssertMenuItem (td1, 1, 1);

      var tr2 = table.GetAssertedChildElement ("tr", 1);
      tr2.AssertChildElementCount (1);

      var td2 = GetAssertedCell (tr2, 0, 2);

      AssertMenuItem (td2, 2, 0);
      AssertMenuItem (td2, 4, 1);
    }

    [Test]
    public void RenderWithLineBreaksNone ()
    {
      SetUpGetPostBackLinkExpectations (true);
      _control.Stub (stub => stub.LineBreaks).Return (ListMenuLineBreaks.None);

      var table = GetAssertedTable();
      var tr = table.GetAssertedChildElement ("tr", 0);
      tr.AssertChildElementCount (1);

      var td = GetAssertedCell (tr, 0, 4);
      for (int iColumn = 0; iColumn < 4; iColumn++)
        AssertMenuItem (td, iColumn < 3 ? iColumn : iColumn + 1, iColumn);
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes ()
    {
      SetUpGetPostBackLinkExpectations (true);

      var table = GetAssertedTable();
      table.AssertAttributeValueEquals (DiagnosticMetadataAttributes.ControlType, "ListMenu");
    }

    private XmlNode GetAssertedTable ()
    {
      var renderer = new ListMenuRenderer (new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.WithDiagnosticMetadata);
      renderer.Render (new ListMenuRenderingContext (_httpContextStub, _htmlHelper.Writer, _control));

      var document = _htmlHelper.GetResultDocument();

      var table = _htmlHelper.GetAssertedChildElement (document, "table", 0);
      table.AssertAttributeValueEquals ("id", _control.ClientID);
      table.AssertAttributeValueEquals ("class", _control.CssClass);
      table.AssertAttributeValueEquals ("cellspacing", "0");
      table.AssertAttributeValueEquals ("cellpadding", "0");
      table.AssertAttributeValueEquals ("border", "0");
      return table;
    }

    private void AssertMenuItem (XmlNode parentCell, int itemIndex, int nodeIndex)
    {
      var item = (WebMenuItem) _control.MenuItems[itemIndex];

      switch (item.Style)
      {
        case WebMenuItemStyle.IconAndText:
          AssertIconAndText (itemIndex, parentCell, item, nodeIndex);
          break;
        case WebMenuItemStyle.Text:
          AssertText (itemIndex, parentCell, item, nodeIndex);
          break;
        case WebMenuItemStyle.Icon:
          AssertIcon (itemIndex, parentCell, item, nodeIndex);
          break;
      }
    }

    private XmlNode GetAssertedCell (XmlNode parentRow, int cellIndex, int itemCount)
    {
      var td = parentRow.GetAssertedChildElement ("td", cellIndex);
      td.AssertAttributeValueEquals ("class", "listMenuRow");
      td.AssertStyleAttribute ("width", "100%");
      td.AssertChildElementCount (itemCount);
      return td;
    }

    private void AssertIcon (int itemIndex, XmlNode parent, WebMenuItem item, int nodeIndex)
    {
      XmlNode a = GetAssertedItemLink (parent, itemIndex, nodeIndex, item.ItemID, item.Text);
      AssertIcon (a);
    }

    private void AssertText (int itemIndex, XmlNode parent, WebMenuItem item, int nodeIndex)
    {
      XmlNode a = GetAssertedItemLink (parent, itemIndex, nodeIndex, item.ItemID, item.Text);
      a.AssertTextNode (item.Text, 0);
    }

    private void AssertIconAndText (int itemIndex, XmlNode td, WebMenuItem item, int nodeIndex)
    {
      XmlNode a = GetAssertedItemLink (td, itemIndex, nodeIndex, item.ItemID, item.Text);
      AssertIcon (a);

      a.AssertTextNode (HtmlHelper.WhiteSpace + item.Text, 1);
    }

    private void AssertIcon (XmlNode parent)
    {
      var img = parent.GetAssertedChildElement ("img", 0);
      img.AssertAttributeValueContains ("src", "/Images/ClassicBlue/NullIcon.gif");
    }

    private XmlNode GetAssertedItemLink (XmlNode td, int itemIndex, int nodeIndex, string itemID, string text)
    {
      var span = td.GetAssertedChildElement ("span", nodeIndex);
      span.AssertAttributeValueEquals ("id", _control.ClientID + "_" + itemIndex);
      span.AssertAttributeValueEquals (DiagnosticMetadataAttributes.ItemID, itemID);
      span.AssertAttributeValueEquals (DiagnosticMetadataAttributes.Content, text);
      span.AssertChildElementCount (1);

      return span.GetAssertedChildElement ("a", 0);
    }

    private void PopulateMenu ()
    {
      AddMenuItem ("item 1", "category 1", "Event", WebMenuItemStyle.IconAndText, RequiredSelection.Any, CommandType.Event);
      AddMenuItem ("item 2", "category 1", "WxeFunction", WebMenuItemStyle.Text, RequiredSelection.OneOrMore, CommandType.WxeFunction);
      AddMenuItem ("item 3", "category 2", "Href", WebMenuItemStyle.Icon, RequiredSelection.ExactlyOne, CommandType.Href);
      ((WebMenuItem) _control.MenuItems[2]).Command.HrefCommand.Href = "/LinkedPage.html";
      ((WebMenuItem) _control.MenuItems[2]).Command.HrefCommand.Target = "_blank";
      AddMenuItem ("invisible item", "category 2", "Href", WebMenuItemStyle.IconAndText, RequiredSelection.ExactlyOne, CommandType.Href);
      ((WebMenuItem) _control.MenuItems[3]).IsVisible = false;
      AddMenuItem ("disabled item", "category 2", "Href", WebMenuItemStyle.IconAndText, RequiredSelection.ExactlyOne, CommandType.Href);
      ((WebMenuItem) _control.MenuItems[4]).IsDisabled = true;
    }

    private void SetUpGetPostBackLinkExpectations (bool withHrefItem)
    {
      _clientScriptManagerMock.Expect (mock => mock.GetPostBackClientHyperlink (_control, "0")).Return ("PostBackLink: 0");
      _clientScriptManagerMock.Expect (mock => mock.GetPostBackClientHyperlink (_control, "1")).Return ("PostBackLink: 1");
      if (withHrefItem)
        _clientScriptManagerMock.Expect (mock => mock.GetPostBackClientHyperlink (_control, "2")).Return ("PostBackLink: 2");
    }

    private void AddMenuItem (
        string itemID,
        string category,
        string text,
        WebMenuItemStyle style,
        RequiredSelection selection,
        CommandType commandType)
    {
      WebMenuItem item = new WebMenuItem (
          itemID,
          category,
          text,
          new IconInfo ("~/Images/ClassicBlue/NullIcon.gif"),
          new IconInfo ("~/Images/ClassicBlue/NullIcon.gif"),
          style,
          selection,
          false,
          new Command (commandType));

      _control.MenuItems.Add (item);
    }

    private string GetItemScript (int itemIndex)
    {
      const string itemTemplate = "new ListMenuItemInfo ('{0}', '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8})";
      var menuItem = (WebMenuItem) _control.MenuItems[itemIndex];

      string href;
      string target = "null";

      if (menuItem.Command.Type == CommandType.Href)
      {
        href = menuItem.Command.HrefCommand.FormatHref (itemIndex.ToString(), menuItem.ItemID);
        href = "'" + href + "'";
        target = "'" + menuItem.Command.HrefCommand.Target + "'";
      }
      else
      {
        string argument = itemIndex.ToString();
        href = _control.Page.ClientScript.GetPostBackClientHyperlink (_control, argument) + ";";
        href = ScriptUtility.EscapeClientScript (href);
        href = "'" + href + "'";
      }

      return string.Format (
          itemTemplate,
          _control.ClientID + "_" + itemIndex,
          menuItem.Category,
          menuItem.Style != WebMenuItemStyle.Icon ? "'" + menuItem.Text + "'" : "null",
          menuItem.Style != WebMenuItemStyle.Text ? "'" + menuItem.Icon.Url.TrimStart ('~') + "'" : "null",
          menuItem.Style != WebMenuItemStyle.Text ? "'" + menuItem.DisabledIcon.Url.TrimStart ('~') + "'" : "null",
          (int) menuItem.RequiredSelection,
          (itemIndex == 4) ? "true" : "false",
          href,
          target);
    }
  }
}