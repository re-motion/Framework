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
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public abstract class BocTreeViewMenuItemProvider : WebTreeViewMenuItemProvider
  {
    private BocTreeView _ownerControl;

    public BocTreeViewMenuItemProvider ()
    {
    }

    public override void OnMenuItemEventCommandClick (WebMenuItem menuItem, WebTreeNode node)
    {
      if (menuItem != null && menuItem.Command != null)
      {
        if (menuItem is BocMenuItem)
          ((BocMenuItemCommand) menuItem.Command).OnClick ((BocMenuItem) menuItem);
        else
          base.OnMenuItemEventCommandClick (menuItem, node);
      }
    }

    public override void OnMenuItemWxeFunctionCommandClick (WebMenuItem menuItem, WebTreeNode node)
    {
      if (menuItem != null && menuItem.Command != null)
      {
        if (menuItem is BocMenuItem)
        {
          BocMenuItemCommand command = (BocMenuItemCommand) menuItem.Command;
          IBusinessObject businessObject = null;
          if (node is BusinessObjectTreeNode)
            businessObject = ((BusinessObjectTreeNode) node).BusinessObject;

          Page page = node.TreeView.Page;
          if (page is IWxePage)
            command.ExecuteWxeFunction ((IWxePage) page, businessObject);
          //else
          //  command.ExecuteWxeFunction (Page, businessObject);
        }
        else
        {
          base.OnMenuItemWxeFunctionCommandClick (menuItem, node);
        }
      }
    }

    public BocTreeView OwnerControl
    {
      get { return _ownerControl; }
      set { _ownerControl = value; }
    }
  }
}
