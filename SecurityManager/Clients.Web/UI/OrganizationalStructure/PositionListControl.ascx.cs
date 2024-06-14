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
  public partial class PositionListControl : BaseListControl<Position>
  {
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure.OrganizationalStructureResources")]
    public enum ResourceIdentifier
    {
      PositionListLabelText,
      NewPositionButtonText,
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
        PositionList.SetSortingOrder(
            new BocListSortingOrderEntry((IBocSortableColumnDefinition)PositionList.FixedColumns[0], SortingDirection.Ascending));
      }
      PositionList.LoadUnboundValue(GetValues(), false);

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration();
      Type positionType = SafeServiceLocator.Current.GetInstance<IOrganizationalStructureFactory>().GetPositionType();
      NewPositionButton.Visible = securityClient.HasConstructorAccess(positionType);
    }

    protected override void OnPreRender (EventArgs e)
    {
      var resourceManager = GetResourceManager(typeof(ResourceIdentifier));
      PositionListLabel.Text = resourceManager.GetText(ResourceIdentifier.PositionListLabelText);
      NewPositionButton.Text = resourceManager.GetText(ResourceIdentifier.NewPositionButtonText);

      base.OnPreRender(e);

      ResetListOnTenantChange(PositionList);
    }

    protected void PositionList_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
    {
      HandleEditItemClick(PositionList, e);
    }

    protected void NewPositionButton_Click (object sender, EventArgs e)
    {
      HandleNewButtonClick(PositionList);
    }

    protected override IReadOnlyList<Position> GetValues ()
    {
      return Position.FindAll().ToArray();
    }

    protected override FormFunction<Position> CreateEditFunction (ITransactionMode transactionMode, IDomainObjectHandle<Position>? editedObject)
    {
      ArgumentUtility.CheckNotNull("transactionMode", transactionMode);

      return new EditPositionFormFunction(transactionMode, editedObject);
    }
  }
}
