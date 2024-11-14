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
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using Remotion.Data.DomainObjects;
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Compilation;
using Remotion.Web.Globalization;

namespace Remotion.SecurityManager.Clients.Web.UI
{
  [FileLevelControlBuilder(typeof(CodeProcessingUserControlBuilder))]
  public partial class SecurityManagerUserContextControl : UserControl
  {
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.SecurityManager.Clients.Web.Globalization.UI.SecurityManagerUserContextControlResources")]
    public enum ResourceIdentifier
    {
      CurrentUserLabelText,
      CurrentSubstitutionLabelText,
      CurrentTenantLabelText
    }

    private static readonly string s_isTenantSelectionEnabledKey = typeof(SecurityManagerUserContextControl).GetFullNameChecked() + "_IsTenantSelectionEnabled";
    private static readonly string s_enableAbstractTenantsKey = typeof(SecurityManagerUserContextControl).GetFullNameChecked() + "_EnableAbstractTenants";
    private static readonly string s_isSubstitutionSelectionEnabledKey = typeof(SecurityManagerUserContextControl).GetFullNameChecked() + "_IsSubstitutionSelectionEnabled";


    [DefaultValue(true)]
    public bool EnableAbstractTenants
    {
      get { return (bool?)ViewState[s_enableAbstractTenantsKey] ?? true; }
      set { ViewState[s_enableAbstractTenantsKey] = value; }
    }

    protected SecurityManagerHttpApplication ApplicationInstance
    {
      get { return (SecurityManagerHttpApplication)Context.ApplicationInstance; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad(e);

      if (!IsPostBack)
      {
        var tenants = GetPossibleTenants();
        CurrentTenantField.SetBusinessObjectList(tenants);
        var currentTenant = SecurityManagerPrincipal.Current.Tenant;

        CurrentTenantField.LoadUnboundValue(currentTenant, false);

        bool isCurrentTenantTheOnlyTenantInTheCollection = tenants.Length == 1 && currentTenant != null && tenants[0].ID.Equals(currentTenant.ID);
        bool isCurrentTenantTheOnlyTenant = tenants.Length == 0 && currentTenant != null;
        bool hasExactlyOneTenant = isCurrentTenantTheOnlyTenantInTheCollection || isCurrentTenantTheOnlyTenant;
        IsTenantSelectionEnabled = !hasExactlyOneTenant;

        var substitutions = GetPossibleSubstitutions();
        CurrentSubstitutionField.SetBusinessObjectList(substitutions);
        var currentSubstitution = SecurityManagerPrincipal.Current.Substitution;

        CurrentSubstitutionField.LoadUnboundValue(currentSubstitution, false);
        IsSubstitutionSelectionEnabled = substitutions.Length > 0;
      }
    }

    private TenantProxy[] GetPossibleTenants ()
    {
      return SecurityManagerPrincipal.Current.GetTenants(EnableAbstractTenants).OrderBy(t => t.DisplayName).ToArray();
    }

    private SubstitutionProxy[] GetPossibleSubstitutions ()
    {
      return SecurityManagerPrincipal.Current.GetActiveSubstitutions().OrderBy(s => s.DisplayName).ToArray();
    }

    protected void CurrentTenantField_SelectionChanged (object sender, EventArgs e)
    {
      string? tenantID = CurrentTenantField.BusinessObjectUniqueIdentifier;
      Assertion.IsNotNull(tenantID);
      var possibleTenants = GetPossibleTenants();
      CurrentTenantField.SetBusinessObjectList(possibleTenants);
      if (!possibleTenants.Where(s=>s.UniqueIdentifier == tenantID).Any())
      {
        CurrentTenantField.Value = null;
        return;
      }

      var oldSecurityManagerPrincipal = SecurityManagerPrincipal.Current;
      if (!oldSecurityManagerPrincipal.IsNull)
      {
        Assertion.IsNotNull(oldSecurityManagerPrincipal.User, "SecurityManagerPrincipal.User != null when SecurityManagerPrincipal.IsNull == false");
        var newSecurityManagerPrincipal = ApplicationInstance.SecurityManagerPrincipalFactory.Create(
            ObjectID.Parse(tenantID).GetHandle<Tenant>(),
            oldSecurityManagerPrincipal.User.Handle,
            oldSecurityManagerPrincipal.Substitution != null ? oldSecurityManagerPrincipal.Substitution.Handle : null);
        ApplicationInstance.SetCurrentPrincipal(newSecurityManagerPrincipal);
      }

      CurrentTenantField.IsDirty = false;
    }

    protected void CurrentSubstitutionField_SelectionChanged (object sender, EventArgs e)
    {
      string? substitutionID = CurrentSubstitutionField.BusinessObjectUniqueIdentifier;
      var possibleSubstitutions = GetPossibleSubstitutions();
      CurrentSubstitutionField.SetBusinessObjectList(possibleSubstitutions);
      if (substitutionID != null && !possibleSubstitutions.Where(s=>s.UniqueIdentifier == substitutionID).Any())
      {
        CurrentSubstitutionField.Value = null;
        return;
      }

      var oldSecurityManagerPrincipal = SecurityManagerPrincipal.Current;
      if (!oldSecurityManagerPrincipal.IsNull)
      {
        Assertion.IsNotNull(oldSecurityManagerPrincipal.Tenant, "SecurityManagerPrincipal.Tenant != null when SecurityManagerPrincipal.IsNull == false");
        Assertion.IsNotNull(oldSecurityManagerPrincipal.User, "SecurityManagerPrincipal.User != null when SecurityManagerPrincipal.IsNull == false");
        var newSecurityManagerPrincipal = ApplicationInstance.SecurityManagerPrincipalFactory.Create(
            oldSecurityManagerPrincipal.Tenant.Handle,
            oldSecurityManagerPrincipal.User.Handle,
            substitutionID != null ? ObjectID.Parse(substitutionID).GetHandle<Substitution>() : null);
        ApplicationInstance.SetCurrentPrincipal(newSecurityManagerPrincipal);
      }

      CurrentSubstitutionField.IsDirty = false;
    }

    protected override void OnPreRender (EventArgs e)
    {
      var resourceManager = GetResourceManager(typeof(ResourceIdentifier));

      CurrentUserLabel.Text = resourceManager.GetText(ResourceIdentifier.CurrentUserLabelText);
      CurrentSubstitutionLabel.Text = resourceManager.GetText(ResourceIdentifier.CurrentSubstitutionLabelText);
      CurrentTenantLabel.Text = resourceManager.GetText(ResourceIdentifier.CurrentTenantLabelText);

      base.OnPreRender(e);

      CurrentTenantField.ReadOnly = !IsTenantSelectionEnabled && !SecurityManagerPrincipal.Current.IsNull;
      CurrentSubstitutionField.ReadOnly = !IsSubstitutionSelectionEnabled;
      //For now: the substitution is permanently editable.
      CurrentSubstitutionField.ReadOnly = false;

      CurrentUserField.LoadUnboundValue(SecurityManagerPrincipal.Current.User, false);

      CurrentUserLabel.Visible = CurrentUserField.Visible;
      CurrentSubstitutionLabel.Visible = CurrentSubstitutionField.Visible;
      CurrentTenantLabel.Visible = CurrentTenantField.Visible;
    }

    protected virtual IResourceManager GetResourceManager ()
    {
      Type type = this.GetType();

      return GlobalizationService.GetResourceManager(type);
    }

    protected IResourceManager GetResourceManager (Type resourceEnumType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("resourceEnumType", resourceEnumType, typeof(Enum));

      return ResourceManagerSet.Create(GlobalizationService.GetResourceManager(TypeAdapter.Create(resourceEnumType)), GetResourceManager());
    }

    protected IServiceLocator ServiceLocator
    {
      get { return SafeServiceLocator.Current; }
    }

    protected IGlobalizationService GlobalizationService
    {
      get { return SafeServiceLocator.Current.GetInstance<IGlobalizationService>(); }
    }

    private bool IsTenantSelectionEnabled
    {
      get { return (bool?)ViewState[s_isTenantSelectionEnabledKey] ?? true; }
      set { ViewState[s_isTenantSelectionEnabledKey] = value; }
    }

    private bool IsSubstitutionSelectionEnabled
    {
      get { return (bool?)ViewState[s_isSubstitutionSelectionEnabledKey] ?? true; }
      set { ViewState[s_isSubstitutionSelectionEnabledKey] = value; }
    }
  }
}
