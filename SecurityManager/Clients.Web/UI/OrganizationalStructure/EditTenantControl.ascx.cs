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
using System.Linq;
using System.Web.UI.WebControls;
using Remotion.FunctionalProgramming;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes.OrganizationalStructure;
using Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
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

    private BocAutoCompleteReferenceValue _parentField;

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected new EditTenantFormFunction CurrentFunction
    {
      get { return (EditTenantFormFunction) base.CurrentFunction; }
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
      base.OnInit (e);

      _parentField = GetControl<BocAutoCompleteReferenceValue> ("ParentField", "Parent");

      if (string.IsNullOrEmpty (_parentField.SearchServicePath))
        SecurityManagerSearchWebService.BindServiceToControl (_parentField);
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (ChildrenList.IsReadOnly)
        ChildrenList.Selection = RowSelection.Disabled;
    }

    protected override void OnPreRender (EventArgs e)
    {
      var resourceManager = GetResourceManager (typeof (ResourceIdentifier));
      
      ParentValidator.ErrorMessage = resourceManager.GetString (ResourceIdentifier.ParentValidatorErrorMessage);
      TenantLabel.Text = resourceManager.GetString (ResourceIdentifier.TenantLabelText);

      base.OnPreRender (e);
    }

    protected void ParentValidator_ServerValidate (object source, ServerValidateEventArgs args)
    {
      args.IsValid = IsParentHierarchyValid ((Tenant) _parentField.Value);
    }

    private bool IsParentHierarchyValid (Tenant group)
    {
      var groups = group.CreateSequence (g => g.Parent, g => g != null && g != CurrentFunction.CurrentObject && g.Parent != group).ToArray();
      if (groups.Length == 0)
        return false;
      if (groups.Last().Parent != null)
        return false;
      return true;
    }
  }
}