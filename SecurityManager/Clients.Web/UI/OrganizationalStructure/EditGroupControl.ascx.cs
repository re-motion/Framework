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
using System.Linq;
using System.Web.UI.WebControls;
using Remotion.FunctionalProgramming;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Classes.OrganizationalStructure;
using Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;
using Remotion.Web.Globalization;
using Remotion.Web.UI.Controls;

namespace Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  public partial class EditGroupControl : BaseEditControl<EditGroupControl>
  {
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure.OrganizationalStructureResources")]
    public enum ResourceIdentifier
    {
      GroupLabelText,
      ParentValidatorErrorMessage
    }

    private BocAutoCompleteReferenceValue _parentField = default!;
    private BocAutoCompleteReferenceValue _groupTypeField = default!;

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected new EditGroupFormFunction CurrentFunction
    {
      get { return (EditGroupFormFunction)base.CurrentFunction; }
    }

    protected override FormGridManager GetFormGridManager ()
    {
      return FormGridManager;
    }

    public override IFocusableControl InitialFocusControl
    {
      get { return NameField; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);

      _parentField = GetControl<BocAutoCompleteReferenceValue>("ParentField", "Parent");
      _groupTypeField = GetControl<BocAutoCompleteReferenceValue>("GroupTypeField", "GroupType");
      _groupTypeField.SelectionChanged += GroupTypeField_SelectionChanged;
      _groupTypeField.TextBoxStyle.AutoPostBack = true;

      var bocListInlineEditingConfigurator = ServiceLocator.GetInstance<BocListInlineEditingConfigurator>();

      RolesList.EditModeControlFactory = ServiceLocator.GetInstance<GroupRolesListEditableRowControlFactory>();
      bocListInlineEditingConfigurator.Configure(RolesList, Role.NewObject);

      if (string.IsNullOrEmpty(_parentField.ControlServicePath))
        SecurityManagerAutoCompleteReferenceValueWebService.BindServiceToControl(_parentField);

      if (string.IsNullOrEmpty(_groupTypeField.ControlServicePath))
        SecurityManagerAutoCompleteReferenceValueWebService.BindServiceToControl(_groupTypeField);
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad(e);

      if (!IsPostBack)
      {
        RolesList.SetSortingOrder(
            new BocListSortingOrderEntry((IBocSortableColumnDefinition)RolesList.FixedColumns.FindMandatory("User"), SortingDirection.Ascending),
            new BocListSortingOrderEntry((IBocSortableColumnDefinition)RolesList.FixedColumns.FindMandatory("Position"), SortingDirection.Ascending));
      }

      if (ChildrenList.IsReadOnly)
        ChildrenList.Selection = RowSelection.Disabled;

      if (RolesList.IsReadOnly)
        RolesList.Selection = RowSelection.Disabled;
    }

    protected override void OnPreRender (EventArgs e)
    {
      var resourceManager = GetResourceManager(typeof(ResourceIdentifier));

      GroupLabel.Text = resourceManager.GetText(ResourceIdentifier.GroupLabelText);
      ParentValidator.ErrorMessage = resourceManager.GetString(ResourceIdentifier.ParentValidatorErrorMessage);

      base.OnPreRender(e);

      _parentField.ControlServiceArguments = CurrentFunction.TenantHandle.AsArgument();
    }

    protected void ParentValidator_ServerValidate (object source, ServerValidateEventArgs args)
    {
      args.IsValid = IsParentHierarchyValid((Group?)_parentField.Value);
    }

    private void GroupTypeField_SelectionChanged (object? sender, EventArgs e)
    {
      var referenceValue = ArgumentUtility.CheckNotNullAndType<BocAutoCompleteReferenceValue>("sender", sender!);

      referenceValue.SaveValue(false);
      referenceValue.IsDirty = true;
    }

    private bool IsParentHierarchyValid (Group? group)
    {
      var groups = group.CreateSequence(g => g.Parent, g => g != CurrentFunction.CurrentObject && g.Parent != group).ToArray();
      if (groups.Length == 0)
        return false;
      if (groups.Last().Parent != null)
        return false;
      return true;
    }
  }
}
