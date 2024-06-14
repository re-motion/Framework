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
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Security;
using Remotion.SecurityManager.Clients.Web.Classes.OrganizationalStructure;
using Remotion.SecurityManager.Clients.Web.WxeFunctions;
using Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Globalization;

namespace Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  public partial class GroupListControl : BaseListControl<Group>
  {
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure.OrganizationalStructureResources")]
    public enum ResourceIdentifier
    {
      GroupListLabelText,
      NewGroupButtonText,
    }

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad(e);

      if (!IsPostBack)
      {
        GroupList.SetSortingOrder(
            new BocListSortingOrderEntry((IBocSortableColumnDefinition)GroupList.FixedColumns[0], SortingDirection.Ascending));
      }
      GroupList.LoadUnboundValue(GetValues(), IsPostBack);

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration();
      Type groupType = SafeServiceLocator.Current.GetInstance<IOrganizationalStructureFactory>().GetGroupType();
      NewGroupButton.Visible = securityClient.HasConstructorAccess(groupType);
    }

    protected override void OnPreRender (EventArgs e)
    {
      var resourceManager = GetResourceManager(typeof(ResourceIdentifier));
      GroupListLabel.Text = resourceManager.GetText(ResourceIdentifier.GroupListLabelText);
      NewGroupButton.Text = resourceManager.GetText(ResourceIdentifier.NewGroupButtonText);

      base.OnPreRender(e);

      ResetListOnTenantChange(GroupList);
    }

    protected void GroupList_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
    {
      HandleEditItemClick(GroupList, e);
    }

    protected void NewGroupButton_Click (object sender, EventArgs e)
    {
      HandleNewButtonClick(GroupList);
    }

    protected override IReadOnlyList<Group> GetValues ()
    {
      return Group.FindByTenant(CurrentFunction.TenantHandle).ToArray();
    }

    protected override FormFunction<Group> CreateEditFunction (ITransactionMode transactionMode, IDomainObjectHandle<Group>? editedObject)
    {
      ArgumentUtility.CheckNotNull("transactionMode", transactionMode);

      return new EditGroupFormFunction(transactionMode, editedObject);
    }
  }
}
