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
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UI.Controls
{
  public abstract class WebTreeViewMenuItemProvider
  {
    protected WebTreeViewMenuItemProvider ()
    {
    }

    public abstract WebMenuItem[] InitalizeMenuItems (WebTreeNode node);

    public virtual void PreRenderMenuItems (WebTreeNode node, WebMenuItemCollection menuItems)
    {
    }

    public virtual void OnMenuItemEventCommandClick (WebMenuItem menuItem, WebTreeNode node)
    {
      if (menuItem != null && menuItem.Command != null)
        menuItem.Command.OnClick();
    }

    public virtual void OnMenuItemWxeFunctionCommandClick (WebMenuItem menuItem, WebTreeNode node)
    {
      if (menuItem != null && menuItem.Command != null)
      {
        Command command = menuItem.Command;
        Page page = node.TreeView.Page;
        if (page is IWxePage)
          command.ExecuteWxeFunction ((IWxePage) page, null);
        //else
        //  command.ExecuteWxeFunction (Page, null, new NameValueCollection (0));
      }
    }
  }
}
