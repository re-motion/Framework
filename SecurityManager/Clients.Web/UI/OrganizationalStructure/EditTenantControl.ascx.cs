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
using System.Linq;
using System.Web.UI.WebControls;
using Remotion.FunctionalProgramming;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes.OrganizationalStructure;
using Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.Globalization;
using Remotion.Web.UI.Controls;

namespace Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  public partial class EditTenantControl : BaseEditControl<EditTenantControl>
  {
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure.OrganizationalStructureResources")]
    public enum ResourceIdentifier
    {
      ParentValidatorErrorMessage ,
      TenantLabelText,
    }

    /// <remarks>Initialized during <see cref="OnInit"/>.</remarks>
    private BocAutoCompleteReferenceValue _parentField = default!;

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected new EditTenantFormFunction CurrentFunction
    {
      get { return (EditTenantFormFunction)base.CurrentFunction; }
    }

    protected override FormGridManager GetFormGridManager ()
    {
      return FormGridManager;
    }

    public override IFocusableControl InitialFocusControl
    {
      get { return NameField; }
    }

    [MemberNotNull(nameof(_parentField))]
    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);

      _parentField = GetControl<BocAutoCompleteReferenceValue>("ParentField", "Parent");

      if (string.IsNullOrEmpty(_parentField.ControlServicePath))
        SecurityManagerAutoCompleteReferenceValueWebService.BindServiceToControl(_parentField);
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad(e);

      if (ChildrenList.IsReadOnly)
        ChildrenList.Selection = RowSelection.Disabled;
    }

    protected override void OnPreRender (EventArgs e)
    {
      var resourceManager = GetResourceManager(typeof(ResourceIdentifier));

      ParentValidator.ErrorMessage = resourceManager.GetString(ResourceIdentifier.ParentValidatorErrorMessage);
      TenantLabel.Text = resourceManager.GetText(ResourceIdentifier.TenantLabelText);

      base.OnPreRender(e);
    }

    protected void ParentValidator_ServerValidate (object source, ServerValidateEventArgs args)
    {
      args.IsValid = IsParentHierarchyValid((Tenant?)_parentField.Value);
    }

    private bool IsParentHierarchyValid (Tenant? tenant)
    {
      var tenants = tenant.CreateSequence(g => g.Parent, g => g != CurrentFunction.CurrentObject && g.Parent != tenant).ToArray();
      if (tenants.Length == 0)
        return false;
      if (tenants.Last().Parent != null)
        return false;
      return true;
    }
  }
}
