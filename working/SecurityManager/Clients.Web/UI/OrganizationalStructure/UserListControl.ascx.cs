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
using System.Collections;
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Security;
using Remotion.SecurityManager.Clients.Web.Classes.OrganizationalStructure;
using Remotion.SecurityManager.Clients.Web.WxeFunctions;
using Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Remotion.SecurityManager.Configuration;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;

namespace Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  public partial class UserListControl : BaseListControl<User>
  {
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure.OrganizationalStructureResources")]
    public enum ResourceIdentifier
    {
      UserListLabelText,
      NewUserButtonText,
    }

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (!IsPostBack)
      {
        UserList.SetSortingOrder (
            new BocListSortingOrderEntry ((IBocSortableColumnDefinition) UserList.FixedColumns[0], SortingDirection.Ascending));
      }
      UserList.LoadUnboundValue (GetValues(), IsPostBack);

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration();
      Type userType = SecurityManagerConfiguration.Current.OrganizationalStructureFactory.GetUserType();
      NewUserButton.Visible = securityClient.HasConstructorAccess (userType);
    }

    protected override void OnPreRender (EventArgs e)
    {
      var resourceManager = GetResourceManager (typeof (ResourceIdentifier));
      UserListLabel.Text = resourceManager.GetString (ResourceIdentifier.UserListLabelText);
      NewUserButton.Text = resourceManager.GetString (ResourceIdentifier.NewUserButtonText);

      base.OnPreRender (e);

      ResetListOnTenantChange (UserList);
    }

    protected void UserList_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
    {
      HandleEditItemClick (UserList, e);
    }

    protected void NewUserButton_Click (object sender, EventArgs e)
    {
      HandleNewButtonClick (UserList);
    }

    protected override IList GetValues ()
    {
      return User.FindByTenant (CurrentFunction.TenantHandle).ToArray();
    }

    protected override FormFunction<User> CreateEditFunction (ITransactionMode transactionMode, IDomainObjectHandle<User> editedObject)
    {
      ArgumentUtility.CheckNotNull ("transactionMode", transactionMode);

      return new EditUserFormFunction (transactionMode, editedObject);
    }
  }
}