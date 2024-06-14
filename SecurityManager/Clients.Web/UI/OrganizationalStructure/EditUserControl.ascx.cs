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
using System.Diagnostics.CodeAnalysis;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Classes.OrganizationalStructure;
using Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.Globalization;
using Remotion.Web.UI.Controls;

namespace Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  public partial class EditUserControl : BaseEditControl<EditUserControl>
  {
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure.OrganizationalStructureResources")]
    public enum ResourceIdentifier
    {
      UserLabelText,
    }

    /// <remarks>Initialized during <see cref="OnInit"/>.</remarks>
    private BocAutoCompleteReferenceValue _owningGroupField = default!;

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected new EditUserFormFunction CurrentFunction
    {
      get { return (EditUserFormFunction)base.CurrentFunction; }
    }

    protected override FormGridManager GetFormGridManager ()
    {
      return FormGridManager;
    }

    public override IFocusableControl InitialFocusControl
    {
      get { return UserNameField; }
    }

    [MemberNotNull(nameof(_owningGroupField))]
    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);

      _owningGroupField = GetControl<BocAutoCompleteReferenceValue>("OwningGroupField", "OwningGroup");

      if (string.IsNullOrEmpty(_owningGroupField.ControlServicePath))
        SecurityManagerAutoCompleteReferenceValueWebService.BindServiceToControl(_owningGroupField);

      var bocListInlineEditingConfigurator = ServiceLocator.GetInstance<BocListInlineEditingConfigurator>();

      SubstitutedByList.EditModeControlFactory = ServiceLocator.GetInstance<UserSubstitedByListEditableRowControlFactory>();
      bocListInlineEditingConfigurator.Configure(SubstitutedByList, Substitution.NewObject);

      RolesList.EditModeControlFactory = ServiceLocator.GetInstance<UserRolesListEditableRowControlFactory>();
      bocListInlineEditingConfigurator.Configure(RolesList, Role.NewObject);
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad(e);

      if (!IsPostBack)
      {
        RolesList.SetSortingOrder(
            new BocListSortingOrderEntry((IBocSortableColumnDefinition)RolesList.FixedColumns.FindMandatory("Group"), SortingDirection.Ascending),
            new BocListSortingOrderEntry((IBocSortableColumnDefinition)RolesList.FixedColumns.FindMandatory("Position"), SortingDirection.Ascending));
      }

      if (RolesList.IsReadOnly)
        RolesList.Selection = RowSelection.Disabled;
    }

    protected override void OnPreRender (EventArgs e)
    {
      UserLabel.Text = GetResourceManager(typeof(ResourceIdentifier)).GetText(ResourceIdentifier.UserLabelText);

      base.OnPreRender(e);

      _owningGroupField.ControlServiceArguments = CurrentFunction.TenantHandle.AsArgument();
    }
  }
}
