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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Sample
{
  public class TestBocTreeViewMenuItemProvider : BocTreeViewMenuItemProvider
  {
    public TestBocTreeViewMenuItemProvider ()
    {
    }

    public override WebMenuItem[] InitalizeMenuItems (WebTreeNode node)
    {
      ArgumentUtility.CheckNotNull("node", node);

      WebMenuItem eventMenuItem = new WebMenuItem();
      eventMenuItem.Text = WebString.CreateFromText("Event");
      eventMenuItem.Command.Type = CommandType.Event;

      WebMenuItem wxeMenuItem = new WebMenuItem();
      wxeMenuItem.Text = WebString.CreateFromText("WXE");
      wxeMenuItem.Command.Type = CommandType.WxeFunction;
      wxeMenuItem.Command.WxeFunctionCommand.TypeName = TypeUtility.GetPartialAssemblyQualifiedName(typeof(TestWxeFunction));

      WebMenuItem filteredByServiceMenuItem = new WebMenuItem();
      filteredByServiceMenuItem.ItemID = "FilterByService";
      filteredByServiceMenuItem.Text = WebString.CreateFromText("Should be filtered");
      filteredByServiceMenuItem.IsVisible = true;

      WebMenuItem disabledByServiceMenuItem = new WebMenuItem();
      disabledByServiceMenuItem.ItemID = "DisabledByService";
      disabledByServiceMenuItem.Text = WebString.CreateFromText("Should be disabled");
      disabledByServiceMenuItem.IsDisabled = false;

      WebMenuItem[] menuItems = new WebMenuItem[]
                                {
                                    eventMenuItem,
                                    wxeMenuItem,
                                    WebMenuItem.GetSeparator(),
                                    filteredByServiceMenuItem,
                                    disabledByServiceMenuItem
                                };
      return menuItems;
    }

    public override void PreRenderMenuItems (WebTreeNode node, WebMenuItemCollection menuItems)
    {
      base.PreRenderMenuItems(node, menuItems);
    }

    public override void OnMenuItemEventCommandClick (WebMenuItem menuItem, WebTreeNode node)
    {
      base.OnMenuItemEventCommandClick(menuItem, node);
    }

    public override void OnMenuItemWxeFunctionCommandClick (WebMenuItem menuItem, WebTreeNode node)
    {
      base.OnMenuItemWxeFunctionCommandClick(menuItem, node);
    }
  }
}
