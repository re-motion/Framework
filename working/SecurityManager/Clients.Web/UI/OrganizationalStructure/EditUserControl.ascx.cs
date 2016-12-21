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
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Classes.OrganizationalStructure;
using Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
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

    private BocAutoCompleteReferenceValue _owningGroupField;

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected new EditUserFormFunction CurrentFunction
    {
      get { return (EditUserFormFunction) base.CurrentFunction; }
    }

    protected override FormGridManager GetFormGridManager()
    {
      return FormGridManager;
    }

    public override IFocusableControl InitialFocusControl
    {
      get { return UserNameField; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      _owningGroupField = GetControl<BocAutoCompleteReferenceValue> ("OwningGroupField", "OwningGroup");

      if (string.IsNullOrEmpty (_owningGroupField.SearchServicePath))
        SecurityManagerSearchWebService.BindServiceToControl (_owningGroupField);

      var bocListInlineEditingConfigurator = ServiceLocator.GetInstance<BocListInlineEditingConfigurator>();

      SubstitutedByList.EditModeControlFactory = ServiceLocator.GetInstance<UserSubstitedByListEditableRowControlFactory>();
      bocListInlineEditingConfigurator.Configure (SubstitutedByList, Substitution.NewObject);

      RolesList.EditModeControlFactory = ServiceLocator.GetInstance<UserRolesListEditableRowControlFactory>();
      bocListInlineEditingConfigurator.Configure (RolesList, Role.NewObject);
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (!IsPostBack)
      {
        RolesList.SetSortingOrder (
            new BocListSortingOrderEntry ((IBocSortableColumnDefinition) RolesList.FixedColumns.Find ("Group"), SortingDirection.Ascending),
            new BocListSortingOrderEntry ((IBocSortableColumnDefinition) RolesList.FixedColumns.Find ("Position"), SortingDirection.Ascending));
      }

      if (RolesList.IsReadOnly)
        RolesList.Selection = RowSelection.Disabled;
    }

    protected override void OnPreRender (EventArgs e)
    {
      UserLabel.Text = GetResourceManager (typeof (ResourceIdentifier)).GetString (ResourceIdentifier.UserLabelText);

      base.OnPreRender (e);

      _owningGroupField.Args = CurrentFunction.TenantHandle.AsArgument();
    }
  }
}
