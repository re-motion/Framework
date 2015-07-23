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
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.Web.Infrastructure;
using Remotion.Web.Legacy.UI.Controls.Rendering;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.DropDownMenuImplementation;
using Remotion.Web.UI.Controls.DropDownMenuImplementation.Rendering;
using Rhino.Mocks;

namespace Remotion.Web.Legacy.UnitTests.UI.Controls
{
  [TestFixture]
  public class DropDownMenuQuirksModeRendererTest : RendererTestBase
  {
    private IDropDownMenu _control;
    private readonly List<string> _itemInfos = new List<string>();
    private HttpContextBase _httpContext;
    private HtmlHelper _htmlHelper;
    private IResourceUrlFactory _resourceUrlFactory;

    public override void SetUp ()
    {
      base.SetUp();

      _htmlHelper = new HtmlHelper ();
      _httpContext = MockRepository.GenerateStub<HttpContextBase> ();
     
      _control = MockRepository.GenerateStub<IDropDownMenu>();
      _control.ID = "DropDownMenu1";
      _control.Stub (stub => stub.UniqueID).Return ("DropDownMenu1");
      _control.Stub (stub => stub.ClientID).Return ("DropDownMenu1");
      _control.Stub (stub => stub.MenuItems).Return (new WebMenuItemCollection (_control));
      _control.Stub (stub => stub.GetBindOpenEventScript(null, null, true)).IgnoreArguments().Return ("OpenDropDownMenuEventReference");
      _control.Stub (stub => stub.MenuHeadClientID).Return ("DropDownMenu1_MenuDiv");
      _control.Stub (stub => stub.ResolveClientUrl (null)).IgnoreArguments ().Do ((Func<string, string>) (url => url.TrimStart ('~')));

      IPage pageStub = MockRepository.GenerateStub<IPage>();
      pageStub.Stub (stub => stub.Context).Return (MockRepository.GenerateStub<HttpContextBase>());
      _control.Stub (stub => stub.Page).Return (pageStub);

      StateBag stateBag = new StateBag ();
      _control.Stub (stub => stub.Attributes).Return (new AttributeCollection (stateBag));
      _control.Stub (stub => stub.ControlStyle).Return (new Style (stateBag));

      IClientScriptManager scriptManagerMock = MockRepository.GenerateMock<IClientScriptManager> ();
      _control.Page.Stub (stub => stub.ClientScript).Return (scriptManagerMock);

      _resourceUrlFactory = new FakeResourceUrlFactory();
    }

    [TearDown]
    public void TearDown ()
    {
      _itemInfos.Clear ();
    }

    [Test]
    public void RenderEmptyMenuWithoutTitle ()
    {
      _control.Stub (stub => stub.Enabled).Return (true);

      SetUpScriptExpectations();
      XmlNode outerDiv = GetAssertedOuterDiv();
      XmlNode clickDiv = GetAssertedClickDiv(outerDiv);
      AssertHeadDiv (clickDiv, false, false, false);
      _control.Page.ClientScript.VerifyAllExpectations ();
    }

    [Test]
    public void RenderPopulatedMenuWithoutTitle ()
    {
      _control.Stub (stub => stub.Enabled).Return (true);

      PopulateMenu ();
      
      SetUpScriptExpectations ();
      XmlNode outerDiv = GetAssertedOuterDiv ();
      XmlNode clickDiv = GetAssertedClickDiv (outerDiv);
      AssertHeadDiv (clickDiv, false, false, false);
      _control.Page.ClientScript.VerifyAllExpectations ();
    }

    [Test]
    public void RenderDisabledPopulatedMenuWithoutTitle ()
    {
      _control.Stub (stub => stub.Enabled).Return (true);
      PopulateMenu ();

      SetUpScriptExpectations ();
      XmlNode outerDiv = GetAssertedOuterDiv ();
      XmlNode clickDiv = GetAssertedClickDiv (outerDiv);
      AssertHeadDiv (clickDiv, false, false, false);
      _control.Page.ClientScript.VerifyAllExpectations ();
    }

    [Test]
    public void RenderEmptyMenuWithTitle ()
    {
      AddTitle(false);
      _control.Stub (stub => stub.Enabled).Return (true);
      
      SetUpScriptExpectations ();
      XmlNode outerDiv = GetAssertedOuterDiv ();
      XmlNode clickDiv = GetAssertedClickDiv (outerDiv);
      AssertHeadDiv (clickDiv, true, false, false);
      _control.Page.ClientScript.VerifyAllExpectations ();
    }

    [Test]
    public void RenderEmptyMenuWithTitleAndIcon ()
    {
      AddTitle (true);
      _control.Stub (stub => stub.Enabled).Return (true);

      SetUpScriptExpectations ();
      XmlNode outerDiv = GetAssertedOuterDiv ();
      XmlNode clickDiv = GetAssertedClickDiv (outerDiv);
      AssertHeadDiv (clickDiv, true, true, false);
      _control.Page.ClientScript.VerifyAllExpectations ();
    }

    [Test]
    public void RenderPopulatedMenuWithTitle ()
    {
      AddTitle (false);
      _control.Stub (stub => stub.Enabled).Return (true);

      PopulateMenu ();

      SetUpScriptExpectations ();
      XmlNode outerDiv = GetAssertedOuterDiv ();
      XmlNode clickDiv = GetAssertedClickDiv (outerDiv);
      AssertHeadDiv (clickDiv, true, false, false);
      _control.Page.ClientScript.VerifyAllExpectations ();
    }

    [Test]
    public void RenderPopulatedMenuWithTitleAndEmptyRenderMethod ()
    {
      AddTitle (false);
      _control.Stub (stub => stub.Enabled).Return (true);

      PopulateMenu ();
      _control.Stub (stub => stub.Enabled).Return (true);
      _control.Stub (stub => stub.RenderHeadTitleMethod).Return (writer => { });

      SetUpScriptExpectations ();
      XmlNode outerDiv = GetAssertedOuterDiv ();
      XmlNode clickDiv = GetAssertedClickDiv (outerDiv);
      AssertHeadDiv (clickDiv, true, false, true);
      _control.Page.ClientScript.VerifyAllExpectations ();
    }

    [Test]
    public void RenderDisabledPopulatedMenuWithTitle ()
    {
      AddTitle(false);

      PopulateMenu();

      SetUpScriptExpectations ();
      XmlNode outerDiv = GetAssertedOuterDiv ();
      XmlNode clickDiv = GetAssertedClickDiv (outerDiv);
      AssertHeadDiv (clickDiv, true, false, false);
      _control.Page.ClientScript.VerifyAllExpectations ();
    }

    [Test]
    public void RenderPopulatedGroupedMenuWithTitle ()
    {
      AddTitle (false);
      _control.Stub (stub => stub.Enabled).Return (true);

      PopulateMenu ();
      _control.Stub (stub => stub.Enabled).Return (true);
      _control.Stub (stub => stub.EnableGrouping).Return (true);

      SetUpScriptExpectations ();
      XmlNode outerDiv = GetAssertedOuterDiv ();
      XmlNode clickDiv = GetAssertedClickDiv (outerDiv);
      AssertHeadDiv (clickDiv, true, false, false);
      _control.Page.ClientScript.VerifyAllExpectations ();
    }

    [Test]
    public void RenderDisabledPopulatedGroupedMenuWithTitle ()
    {
      _control.Stub (stub => stub.EnableGrouping).Return (true);

      AddTitle (false);

      PopulateMenu ();

      SetUpScriptExpectations ();
      XmlNode outerDiv = GetAssertedOuterDiv ();
      XmlNode clickDiv = GetAssertedClickDiv (outerDiv);
      AssertHeadDiv (clickDiv, true, false, false);
      _control.Page.ClientScript.VerifyAllExpectations ();
    }

    private void PopulateMenu ()
    {
      AddItem (0, "Category1", CommandType.Event, false, true);
      AddItem (1, "Category1", CommandType.Href, false, true);
      AddItem (2, "Category2", CommandType.WxeFunction, false, true);
      AddItem (3, "Category2", CommandType.WxeFunction, true, true);
      AddItem (4, "Category2", CommandType.WxeFunction, false, false);
    }
    
    private void AddTitle (bool withIcon)
    {
      _control.Stub (stub => stub.TitleText).Return ("MenuTitle");
      if (withIcon)
        _control.Stub (stub => stub.TitleIcon).Return (
            new IconInfo ("~/Images/DropDownMenuTitle.gif", "Title", "Title", Unit.Pixel (16), Unit.Pixel (16)));
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

      Command command = new Command (commandType);
      if (commandType == CommandType.Href)
      {
        command.HrefCommand.Href = "~/Target.aspx?index={0}&itemID={1}";
        command.HrefCommand.Target = "_blank";
      }

      _control.MenuItems.Add (
          new WebMenuItem (
              id,
              category,
              text,
              new IconInfo (iconUrl, text, text, width, height),
              new IconInfo (disabledIconUrl, text, text, width, height),
              WebMenuItemStyle.IconAndText,
              requiredSelection,
              isDisabled,
              command) { IsVisible = isVisible });

      string link;
      if (commandType == CommandType.Href)
        link = string.Format ("/Target.aspx?index={0}&itemID={1}", null, null);
      else
      {
        if (isVisible && _control.Enabled)
        {
          _control.Page.ClientScript.Expect (
              mock => mock.GetPostBackClientHyperlink (Arg.Is<IControl> (_control), Arg.Text.Like (index.ToString())))
              .Return ("PostBackHyperLink:" + index);
        }
        link = "PostBackHyperLink:" + index;
      }

      if (isVisible)
      {
        _itemInfos.Add (
            string.Format (
                "\t\tnew DropDownMenu_ItemInfo ('{0}', '{1}', '{2}', '{3}', '{4}', {5}, {6}, '{7}', {8})",
                index,
                category,
                text,
                iconUrl.TrimStart ('~'),
                disabledIconUrl.TrimStart ('~'),
                (int) requiredSelection,
                isDisabled ? "true" : "false",
                link.TrimStart ('~'),
                (commandType == CommandType.Href) ? "'_blank'" : "null"));
      }
    }

    private void SetUpScriptExpectations ()
    {
      Type type = typeof (DropDownMenuQuirksModeRenderer);
      string initializationScriptKey = type.FullName + "_Startup";
      string styleSheetUrl = "/fake/Remotion.Web.Legacy/Html/DropDownMenu.css";
      string initializationScript = string.Format ("DropDownMenu_InitializeGlobals ('{0}');", styleSheetUrl);

      string menuInfoKey = _control.UniqueID;
      string menuInfoScript = "DropDownMenu_AddMenuInfo (" + Environment.NewLine +
                              "\t" + "new DropDownMenu_MenuInfo ('{0}', new Array (" + Environment.NewLine +
                              "{1} ) ) );";
      StringBuilder menuItems = new StringBuilder();
      foreach (string menuItem in _itemInfos)
      {
        menuItems.Append (menuItem);
        bool isLast = _itemInfos.IndexOf (menuItem) == _itemInfos.Count - 1;
        if (!isLast)
          menuItems.AppendLine (",");
      }
      menuInfoScript = string.Format (menuInfoScript, _control.ClientID, menuItems);

      var scriptManagerMock = _control.Page.ClientScript;
      scriptManagerMock.Stub (mock => mock.IsStartupScriptRegistered (Arg<Type>.Is.NotNull, Arg<string>.Is.NotNull)).Return (false);
      scriptManagerMock.Expect (mock => mock.RegisterStartupScriptBlock (_control, typeof (DropDownMenuQuirksModeRenderer), initializationScriptKey, initializationScript));
      if(_control.Enabled)
        scriptManagerMock.Expect (mock => mock.RegisterStartupScriptBlock (_control, typeof (DropDownMenuQuirksModeRenderer), menuInfoKey, menuInfoScript));
      _control.Page.ClientScript.Replay ();
    }

    private XmlNode GetAssertedOuterDiv ()
    {
      var renderer = new DropDownMenuQuirksModeRenderer (_resourceUrlFactory);
      renderer.Render (new DropDownMenuRenderingContext (_httpContext, _htmlHelper.Writer, _control));

      var document = _htmlHelper.GetResultDocument();
      document.AssertChildElementCount (1);

      var outerDiv = document.GetAssertedChildElement ("div", 0);
      outerDiv.AssertStyleAttribute ("display", "inline-block");
      outerDiv.AssertChildElementCount (1);
      return outerDiv;
    }

    private XmlNode GetAssertedClickDiv (XmlNode outerDiv)
    {
      var clickDiv = outerDiv.GetAssertedChildElement ("div", 0);
      clickDiv.AssertNoAttribute ("onclick");

      clickDiv.AssertAttributeValueEquals ("id", _control.ClientID + "_MenuDiv");
      clickDiv.AssertStyleAttribute ("position", "relative");
      clickDiv.AssertChildElementCount (1);
      return clickDiv;
    }

    private void AssertHeadDiv (XmlNode parent, bool hasTitle, bool withIcon, bool withEmptyRenderMethod)
    {
      var headDiv = parent.GetAssertedChildElement ("div", 0);
      headDiv.AssertAttributeValueEquals ("id", _control.ClientID + "_HeadDiv");
      headDiv.AssertAttributeValueEquals ("class", "dropDownMenuHead");
      headDiv.AssertAttributeValueEquals ("OnMouseOver", "DropDownMenu_OnHeadMouseOver (this)");
      headDiv.AssertAttributeValueEquals ("OnMouseOut", "DropDownMenu_OnHeadMouseOut (this)");
      headDiv.AssertStyleAttribute ("position", "relative");
      headDiv.AssertChildElementCount (1);

      AssertHeadTable(headDiv, hasTitle, withIcon, withEmptyRenderMethod);
    }

    private void AssertHeadTable (XmlNode parent, bool hasTitle, bool withIcon, bool withEmptyRenderMethod)
    {
      var table = parent.GetAssertedChildElement ("table", 0);
      table.AssertAttributeValueEquals ("cellspacing", "0");
      table.AssertAttributeValueEquals ("cellpadding", "0");
      table.AssertStyleAttribute ("display", "inline");
      table.AssertChildElementCount (1);

      var tr = table.GetAssertedChildElement ("tr", 0);
      if (!hasTitle)
      {
        tr.AssertChildElementCount (1);
        AssertHeadButtonCell (tr, 0);
      }
      else if (!withEmptyRenderMethod)
      {
        tr.AssertChildElementCount (3);
        AssertHeadTitleCell (tr, withIcon);
        AssertHeadSeparatorCell (tr, 1);
        AssertHeadButtonCell (tr, 2);
      }
      else
      {
        tr.AssertChildElementCount (2);
        AssertHeadSeparatorCell (tr, 0);
        AssertHeadButtonCell (tr, 1);
      }
    }

    private void AssertHeadSeparatorCell (XmlNode parent, int index)
    {
      var td = parent.GetAssertedChildElement ("td", index);
      td.AssertStyleAttribute ("width", "0%");
      td.AssertStyleAttribute ("padding-right", "0.3em");
      td.AssertChildElementCount (0);
    }

    private void AssertHeadTitleCell (XmlNode parent, bool withIcon)
    {
      var td = parent.GetAssertedChildElement ("td", 0);
      td.AssertAttributeValueEquals ("class", "dropDownMenuHeadTitle");
      td.AssertStyleAttribute ("width", "1%");
      td.AssertChildElementCount (1);

      XmlNode title;
      if (_control.Enabled)
        title = td.GetAssertedChildElement ("a", 0);
      else
        title = td.GetAssertedChildElement ("span", 0);

      if (!withIcon)
      {
        title.AssertTextNode ("MenuTitle", 0);
        title.AssertChildElementCount (0);
      }
      else
      {
        title.AssertTextNode ("MenuTitle", 1);
        title.AssertChildElementCount (1);

        var img = title.GetAssertedChildElement ("img", 0);
        img.AssertAttributeValueEquals ("src", _control.TitleIcon.Url);
        img.AssertAttributeValueEquals ("width", _control.TitleIcon.Width.ToString());
        img.AssertAttributeValueEquals ("height", _control.TitleIcon.Height.ToString ());
        img.AssertStyleAttribute ("vertical-align", "middle");
        img.AssertStyleAttribute ("border-style", "none");
        img.AssertStyleAttribute ("margin-right", "0.3em");
      }
    }


    private void AssertHeadButtonCell (XmlNode parent, int index)
    {
      var td = parent.GetAssertedChildElement ("td", index);
      td.AssertAttributeValueEquals ("class", "dropDownMenuHeadButton");
      td.AssertStyleAttribute ("width", "0%");
      td.AssertStyleAttribute ("text-align", "center");
      td.AssertChildElementCount (1);

      AssertHeadLink(td);
    }

    private void AssertHeadLink (XmlNode parent)
    {
      var link = parent.GetAssertedChildElement ("a", 0);
      link.AssertAttributeValueEquals ("href", "#");
      link.AssertStyleAttribute ("width", "1em");
      link.AssertChildElementCount (1);

      AssertImage(link);
    }

    private void AssertImage (XmlNode parent)
    {
      var img = parent.GetAssertedChildElement ("img", 0);
      img.AssertAttributeValueEquals ("src", "/fake/Remotion.Web.Legacy/Image/DropDownMenuArrow.gif");
      img.AssertAttributeValueEquals ("alt", "");
      img.AssertStyleAttribute ("vertical-align", "middle");
      img.AssertStyleAttribute ("border-style", "none");
      img.AssertChildElementCount (0);
    }
  }
}