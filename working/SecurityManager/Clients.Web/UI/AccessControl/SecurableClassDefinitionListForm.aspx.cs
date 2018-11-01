// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Web.UI;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.WxeFunctions;
using Remotion.SecurityManager.Clients.Web.WxeFunctions.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.SecurityManager.Clients.Web.UI.AccessControl
{
  public partial class SecurableClassDefinitionListForm : BasePage
  {
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.SecurityManager.Clients.Web.Globalization.UI.AccessControl.SecurableClassDefinitionListFormResources")]
    public enum ResourceIdentifier
    {
      Title,
    }

    protected new SecurableClassDefinitionListFormFunction CurrentFunction
    {
      get { return (SecurableClassDefinitionListFormFunction) base.CurrentFunction; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      LoadTree (IsPostBack, false);
      if (!IsPostBack)
        ExpandTreeNodes (SecurableClassDefinitionTree.Nodes);
    }

    protected override void OnPreRenderComplete (EventArgs e)
    {
      var title = GlobalizationService.GetResourceManager (typeof (ResourceIdentifier)).GetString (ResourceIdentifier.Title);
      HtmlHeadAppender.Current.SetTitle (title);
      base.OnPreRenderComplete (e);
    }

    private void LoadTree (bool interim, bool refreshTreeNodes)
    {
      SecurableClassDefinitionTree.LoadUnboundValue (SecurableClassDefinition.FindAllBaseClasses (), interim);
      if (refreshTreeNodes)
        SecurableClassDefinitionTree.RefreshTreeNodes ();
    }

    private void ExpandTreeNodes (WebTreeNodeCollection webTreeNodeCollection)
    {
      foreach (WebTreeNode treeNode in webTreeNodeCollection)
      {
        treeNode.EvaluateExpand ();
        ExpandTreeNodes (treeNode.Children);
      }
    }

    protected void SecurableClassDefinitionTree_Click (object sender, BocTreeNodeClickEventArgs e)
    {
      if (!IsReturningPostBack)
      {
        var classDefinition = (SecurableClassDefinition) e.BusinessObjectTreeNode.BusinessObject;
        var function = new EditPermissionsFormFunction (WxeTransactionMode.CreateRootWithAutoCommit , classDefinition.GetHandle());
        var options = new WxeCallOptionsExternal (
            "_blank", "width=1000, height=700, resizable=yes, menubar=no, toolbar=no, location=no, status=no", true);
        try
        {
          ExecuteFunction (function, new WxeCallArguments ((Control) sender, options));
        }
        catch (WxeCallExternalException)
        {
        }
      }
      else
      {
        var classDefinition = ((EditPermissionsFormFunction) ReturningFunction).CurrentObjectHandle.GetObject();
        UnloadService.UnloadVirtualEndPoint (
            ClientTransaction.Current,
            RelationEndPointID.Resolve (classDefinition, c => c.StatelessAccessControlList));
        UnloadService.UnloadVirtualEndPoint (
            ClientTransaction.Current,
            RelationEndPointID.Resolve (classDefinition, c => c.StatefulAccessControlLists));

        LoadTree (false, true);
      }
    }
  }
}
