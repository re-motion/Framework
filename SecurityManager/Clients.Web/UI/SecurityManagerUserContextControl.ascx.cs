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
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using Microsoft.Practices.ServiceLocation;
using Remotion.Data.DomainObjects;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Reflection;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Compilation;
using Remotion.Web.UI.Controls;

namespace Remotion.SecurityManager.Clients.Web.UI
{
  [FileLevelControlBuilder (typeof (CodeProcessingUserControlBuilder))]
  public partial class SecurityManagerUserContextControl : UserControl
  {
    [ResourceIdentifiers]
    [MultiLingualResources ("Remotion.SecurityManager.Clients.Web.Globalization.UI.SecurityManagerUserContextControlResources")]
    public enum ResourceIdentifier
    {
      CurrentUserLabelText,
      CurrentSubstitutionLabelText,
      CurrentTenantLabelText,
      CurrentSubstitutionCommandTooltip,
      CurrentTenantCommandTooltip
    }

    private static readonly string s_isTenantSelectionEnabledKey = typeof (SecurityManagerUserContextControl).FullName + "_IsTenantSelectionEnabled";
    private static readonly string s_enableAbstractTenantsKey = typeof (SecurityManagerUserContextControl).FullName + "_EnableAbstractTenants";
    private static readonly string s_isSubstitutionSelectionEnabledKey = typeof (SecurityManagerUserContextControl).FullName + "_IsSubstitutionSelectionEnabled";

    private bool _isCurrentTenantFieldReadOnly = true;
    private bool _isCurrentSubstitutionFieldReadOnly = true;

    [DefaultValue (true)]
    public bool EnableAbstractTenants
    {
      get { return (bool?) ViewState[s_enableAbstractTenantsKey] ?? true; }
      set { ViewState[s_enableAbstractTenantsKey] = value; }
    }

    protected SecurityManagerHttpApplication ApplicationInstance
    {
      get { return (SecurityManagerHttpApplication) Context.ApplicationInstance; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (!IsPostBack)
      {
        var tenants = GetPossibleTenants();
        CurrentTenantField.SetBusinessObjectList (tenants);
        var currentTenant = SecurityManagerPrincipal.Current.Tenant;

        CurrentTenantField.LoadUnboundValue (currentTenant, false);

        bool isCurrentTenantTheOnlyTenantInTheCollection = tenants.Length == 1 && currentTenant != null && tenants[0].ID.Equals (currentTenant.ID);
        bool isCurrentTenantTheOnlyTenant = tenants.Length == 0 && currentTenant != null;
        bool hasExactlyOneTenant = isCurrentTenantTheOnlyTenantInTheCollection || isCurrentTenantTheOnlyTenant;
        IsTenantSelectionEnabled = !hasExactlyOneTenant;

        var substitutions = GetPossibleSubstitutions();
        CurrentSubstitutionField.SetBusinessObjectList (substitutions);
        var currentSubstitution = SecurityManagerPrincipal.Current.Substitution;

        CurrentSubstitutionField.LoadUnboundValue (currentSubstitution, false);
        IsSubstitutionSelectionEnabled = substitutions.Length > 0;
      }

#pragma warning disable 618
      if (!IsTenantSelectionEnabled)
        CurrentTenantField.Command.Type = CommandType.None;

      if (!IsSubstitutionSelectionEnabled)
        CurrentSubstitutionField.Command.Type = CommandType.None;
#pragma warning restore 618
    }

    private TenantProxy[] GetPossibleTenants ()
    {
      return SecurityManagerPrincipal.Current.GetTenants (EnableAbstractTenants).OrderBy (t => t.DisplayName).ToArray();
    }

    private SubstitutionProxy[] GetPossibleSubstitutions ()
    {
      return SecurityManagerPrincipal.Current.GetActiveSubstitutions().OrderBy (s => s.DisplayName).ToArray();
    }

    protected void CurrentTenantField_SelectionChanged (object sender, EventArgs e)
    {
      string tenantID = CurrentTenantField.BusinessObjectUniqueIdentifier;
      Assertion.IsNotNull (tenantID);
      var possibleTenants = GetPossibleTenants();
      CurrentTenantField.SetBusinessObjectList (possibleTenants);
      if (!possibleTenants.Where (s=>s.UniqueIdentifier == tenantID).Any())
      {
        CurrentTenantField.Value = null;
        _isCurrentTenantFieldReadOnly = false;
        return;
      }

      var oldSecurityManagerPrincipal = SecurityManagerPrincipal.Current;
      var newSecurityManagerPrincipal = ApplicationInstance.SecurityManagerPrincipalFactory.Create (
          ObjectID.Parse (tenantID).GetHandle<Tenant> (),
          oldSecurityManagerPrincipal.User.Handle,
          oldSecurityManagerPrincipal.Substitution != null ? oldSecurityManagerPrincipal.Substitution.Handle : null);
      ApplicationInstance.SetCurrentPrincipal (newSecurityManagerPrincipal);

      _isCurrentTenantFieldReadOnly = true;
      CurrentTenantField.IsDirty = false;
    }

    protected void CurrentSubstitutionField_SelectionChanged (object sender, EventArgs e)
    {
      string substitutionID = CurrentSubstitutionField.BusinessObjectUniqueIdentifier;
      var possibleSubstitutions = GetPossibleSubstitutions();
      CurrentSubstitutionField.SetBusinessObjectList (possibleSubstitutions);
      if (substitutionID != null && !possibleSubstitutions.Where (s=>s.UniqueIdentifier == substitutionID).Any())
      {
        CurrentSubstitutionField.Value = null;
        _isCurrentSubstitutionFieldReadOnly = false;
        return;
      }

      var oldSecurityManagerPrincipal = SecurityManagerPrincipal.Current;
      var newSecurityManagerPrincipal = ApplicationInstance.SecurityManagerPrincipalFactory.Create (
          oldSecurityManagerPrincipal.Tenant.Handle,
          oldSecurityManagerPrincipal.User.Handle,
          substitutionID != null ? ObjectID.Parse (substitutionID).GetHandle<Substitution>() : null);
      ApplicationInstance.SetCurrentPrincipal (newSecurityManagerPrincipal);

      _isCurrentSubstitutionFieldReadOnly = true;
      CurrentSubstitutionField.IsDirty = false;
    }

    protected void CurrentTenantField_CommandClick (object sender, BocCommandClickEventArgs e)
    {
      _isCurrentTenantFieldReadOnly = false;
      CurrentTenantField.SetBusinessObjectList (GetPossibleTenants());
      CurrentTenantField.LoadUnboundValue (SecurityManagerPrincipal.Current.Tenant, false);
    }

    protected void CurrentSubstitutionField_CommandClick (object sender, BocCommandClickEventArgs e)
    {
      _isCurrentSubstitutionFieldReadOnly = false;
      CurrentSubstitutionField.SetBusinessObjectList (GetPossibleSubstitutions());
      CurrentSubstitutionField.LoadUnboundValue (SecurityManagerPrincipal.Current.Substitution, false);
    }

    protected override void OnPreRender (EventArgs e)
    {
      var resourceManager = GetResourceManager (typeof (ResourceIdentifier));

      CurrentUserLabel.Text = resourceManager.GetString (ResourceIdentifier.CurrentUserLabelText);
      CurrentSubstitutionLabel.Text = resourceManager.GetString (ResourceIdentifier.CurrentSubstitutionLabelText);
      CurrentTenantLabel.Text = resourceManager.GetString (ResourceIdentifier.CurrentTenantLabelText);
#pragma warning disable CS0618 // Type or member is obsolete
      CurrentSubstitutionField.Command.ToolTip = resourceManager.GetString (ResourceIdentifier.CurrentSubstitutionCommandTooltip);
      CurrentTenantField.Command.ToolTip  = resourceManager.GetString (ResourceIdentifier.CurrentTenantCommandTooltip);
#pragma warning restore CS0618 // Type or member is obsolete

      base.OnPreRender (e);

      if (_isCurrentTenantFieldReadOnly && !SecurityManagerPrincipal.Current.IsNull)
        CurrentTenantField.ReadOnly = true;
      else
        CurrentTenantField.ReadOnly = false;

      if (_isCurrentSubstitutionFieldReadOnly && !SecurityManagerPrincipal.Current.IsNull)
        CurrentSubstitutionField.ReadOnly = true;
      else
        CurrentSubstitutionField.ReadOnly = false;
      //For now: the subsitution is permanenty editable.
      CurrentSubstitutionField.ReadOnly = false;

      CurrentUserField.LoadUnboundValue (SecurityManagerPrincipal.Current.User, false);

      CurrentUserLabel.Visible = CurrentUserField.Visible;
      CurrentSubstitutionLabel.Visible = CurrentSubstitutionField.Visible;
      CurrentTenantLabel.Visible = CurrentTenantField.Visible;
    }

    protected virtual IResourceManager GetResourceManager ()
    {
      Type type = this.GetType();

      return GlobalizationService.GetResourceManager (type);
    }

    protected IResourceManager GetResourceManager (Type resourceEnumType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("resourceEnumType", resourceEnumType, typeof (Enum));

      return ResourceManagerSet.Create (GlobalizationService.GetResourceManager (TypeAdapter.Create (resourceEnumType)), GetResourceManager());
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
      get { return (bool?) ViewState[s_isTenantSelectionEnabledKey] ?? true; }
      set { ViewState[s_isTenantSelectionEnabledKey] = value; }
    }

    private bool IsSubstitutionSelectionEnabled
    {
      get { return (bool?) ViewState[s_isSubstitutionSelectionEnabledKey] ?? true; }
      set { ViewState[s_isSubstitutionSelectionEnabledKey] = value; }
    }
  }
}