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
  public partial class TenantListControl : BaseListControl<Tenant>
  {
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure.OrganizationalStructureResources")]
    public enum ResourceIdentifier
    {
      TenantListLabelText,
      NewTenantButtonText,
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
        TenantList.SetSortingOrder(
            new BocListSortingOrderEntry((IBocSortableColumnDefinition)TenantList.FixedColumns[0], SortingDirection.Ascending));
      }
      TenantList.LoadUnboundValue(GetValues(), IsPostBack);

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration();
      Type tenantType = SafeServiceLocator.Current.GetInstance<IOrganizationalStructureFactory>().GetTenantType();
      NewTenantButton.Visible = securityClient.HasConstructorAccess(tenantType);
    }

    protected override void OnPreRender (EventArgs e)
    {
      var resourceManager = GetResourceManager(typeof(ResourceIdentifier));
      TenantListLabel.Text = resourceManager.GetText(ResourceIdentifier.TenantListLabelText);
      NewTenantButton.Text = resourceManager.GetText(ResourceIdentifier.NewTenantButtonText);

      base.OnPreRender(e);

      ResetListOnTenantChange(TenantList);
    }

    protected void TenantList_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
    {
      HandleEditItemClick(TenantList, e);
    }

    protected void NewTenantButton_Click (object sender, EventArgs e)
    {
      HandleNewButtonClick(TenantList);
    }

    protected override IReadOnlyList<Tenant> GetValues ()
    {
      return Tenant.FindAll().ToArray();
    }

    protected override FormFunction<Tenant> CreateEditFunction (ITransactionMode transactionMode, IDomainObjectHandle<Tenant>? editedObject)
    {
      ArgumentUtility.CheckNotNull("transactionMode", transactionMode);

      return new EditTenantFormFunction(transactionMode, editedObject);
    }
  }
}
