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
using System.Web.UI.WebControls;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace OBWTest.IndividualControlTests
{

  public partial class BocDropDownMenuUserControl : BaseUserControl
  {
    protected BindableObjectDataSourceControl CurrentObject;
    protected BocDropDownMenu PartnerField;
    protected BocDropDownMenu UnboundField;
    protected Label MenuItemClickEventArgsLabel;

    protected override void RegisterEventHandlers ()
    {
      base.RegisterEventHandlers();
      PartnerField.MenuItemClick += PartnerField_MenuItemClick;
      UnboundField.MenuItemClick += UnboundField_MenuItemClick;
    }

    override protected void OnInit (EventArgs e)
    {
      base.OnInit (e);
      InitializeMenuItems();
    }

    private void InitializeMenuItems ()
    {
      BocMenuItem menuItem = null;

      menuItem = new BocMenuItem();
      menuItem.ItemID = "Open";
      menuItem.Text = "Open";
      menuItem.Category = "Object";
      menuItem.RequiredSelection = RequiredSelection.OneOrMore;
      menuItem.Command.Type = CommandType.WxeFunction;
      menuItem.Command.WxeFunctionCommand.Parameters = "objects";
      menuItem.Command.WxeFunctionCommand.TypeName = "OBWTest.ViewPersonsWxeFunction,OBWTest";
      PartnerField.MenuItems.Add (menuItem);

      menuItem = new BocMenuItem();
      menuItem.ItemID = "Copy";
      menuItem.Text = "Copy";
      menuItem.Category = "Edit";
      menuItem.Icon.Url = "~/Images/CopyItem.gif";
      menuItem.RequiredSelection = RequiredSelection.OneOrMore;
      menuItem.Command.Type = CommandType.Event;
      PartnerField.MenuItems.Add (menuItem);

      menuItem = new BocMenuItem();
      menuItem.ItemID = "Cut";
      menuItem.Text = "Cut";
      menuItem.Category = "Edit";
      menuItem.RequiredSelection = RequiredSelection.OneOrMore;
      menuItem.Command.Type = CommandType.Event;
      PartnerField.MenuItems.Add (menuItem);

      menuItem = new BocMenuItem();
      menuItem.ItemID = "Paste";
      menuItem.Text = "Paste";
      menuItem.Category = "Edit";
      menuItem.Command.Type = CommandType.Event;
      PartnerField.MenuItems.Add (menuItem);

      menuItem = new BocMenuItem();
      menuItem.ItemID = "Duplicate";
      menuItem.Text = "Duplicate";
      menuItem.Category = "Edit";
      menuItem.Command.Type = CommandType.Event;
      PartnerField.MenuItems.Add (menuItem);

      menuItem = new BocMenuItem();
      menuItem.ItemID = "Delete";
      menuItem.Text = "Delete";
      menuItem.Category = "Edit";
      menuItem.Icon.Url = "~/Images/DeleteItem.gif";
      menuItem.DisabledIcon.Url = "~/Images/DeleteItemDisabled.gif";
      menuItem.RequiredSelection = RequiredSelection.OneOrMore;
      menuItem.Style = WebMenuItemStyle.Icon;
      menuItem.Command.Type = CommandType.Event;
      PartnerField.MenuItems.Add (menuItem);

      menuItem = new BocMenuItem();
      menuItem.Text = "Invisible Item";
      menuItem.IsVisible = false;
      PartnerField.MenuItems.Add (menuItem);

      PartnerField.MenuItems.Add (WebMenuItem.GetSeparator());

      menuItem = new BocMenuItem();
      menuItem.ItemID = "FilterByService";
      menuItem.Text = "Should be filtered";
      menuItem.IsVisible = true;
      PartnerField.MenuItems.Add (menuItem);

      menuItem = new BocMenuItem();
      menuItem.ItemID = "DisabledByService";
      menuItem.Text = "Should be disabled";
      menuItem.IsDisabled = false;
      PartnerField.MenuItems.Add (menuItem);

      menuItem = new BocMenuItem();
      menuItem.ItemID = "Command";
      menuItem.Text = "Command";
      menuItem.Command.Type = CommandType.Event;
      UnboundField.MenuItems.Add (menuItem);

      UnboundField.MenuItems.Add (WebMenuItem.GetSeparator());

      menuItem = new BocMenuItem();
      menuItem.ItemID = "FilterByService";
      menuItem.Text = "Should be filtered";
      menuItem.IsVisible = true;
      UnboundField.MenuItems.Add (menuItem);

      menuItem = new BocMenuItem();
      menuItem.ItemID = "DisabledByService";
      menuItem.Text = "Should be disabled";
      menuItem.IsDisabled = false;
      UnboundField.MenuItems.Add (menuItem);
    }

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    override protected void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      Person person = (Person) CurrentObject.BusinessObject;

      UnboundField.LoadUnboundValue ((IBusinessObject) person, IsPostBack);
    }

    override protected void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
    }

    private void PartnerField_MenuItemClick(object sender, WebMenuItemClickEventArgs e)
    {
      MenuItemClickEventArgsLabel.Text = "Partner-Menu: " + e.Item.ItemID;
    }

    private void UnboundField_MenuItemClick(object sender, WebMenuItemClickEventArgs e)
    {
      MenuItemClickEventArgsLabel.Text = "Unbound-Menu: " + e.Item.ItemID;
    }
  }
}