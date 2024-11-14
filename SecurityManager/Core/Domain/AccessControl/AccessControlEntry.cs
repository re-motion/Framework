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
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.Globalization;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure.Metadata;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [MultiLingualResources("Remotion.SecurityManager.Globalization.Domain.AccessControl.AccessControlEntry")]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class AccessControlEntry : AccessControlObject
  {
    public static AccessControlEntry NewObject ()
    {
      return NewObject<AccessControlEntry>();
    }

    private SecurityTokenMatcher _matcher;
    private DomainObjectDeleteHandler? _deleteHandler;

    protected AccessControlEntry ()
    {
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      TenantCondition = TenantCondition.None;
      GroupCondition = GroupCondition.None;
      UserCondition = UserCondition.None;
      // ReSharper restore DoNotCallOverridableMethodsInConstructor

      _matcher = new SecurityTokenMatcher(this);
    }

    // methods and properties

    protected override void OnLoaded (LoadMode loadMode)
    {
      base.OnLoaded(loadMode);

      if (loadMode == LoadMode.WholeDomainObjectInitialized)
        _matcher = new SecurityTokenMatcher(this);
    }

    public abstract int Index { get; set; }

    [DisableEnumValues(typeof(AccessControlEntryPropertiesEnumerationValueFilter))]
    public abstract TenantCondition TenantCondition { get; set; }

    public abstract TenantHierarchyCondition TenantHierarchyCondition { get; set; }

    [DisableEnumValues(typeof(AccessControlEntryPropertiesEnumerationValueFilter))]
    public abstract GroupCondition GroupCondition { get; set; }

    public abstract GroupHierarchyCondition GroupHierarchyCondition { get; set; }

    [DisableEnumValues(typeof(AccessControlEntryPropertiesEnumerationValueFilter))]
    public abstract UserCondition UserCondition { get; set; }

    [SearchAvailableObjectsServiceType(typeof(TenantPropertyTypeSearchService))]
    public abstract Tenant? SpecificTenant { get; set; }

    [SearchAvailableObjectsServiceType(typeof(GroupPropertyTypeSearchService))]
    public abstract Group? SpecificGroup { get; set; }

    [SearchAvailableObjectsServiceType(typeof(GroupTypePropertyTypeSearchService))]
    public abstract GroupType? SpecificGroupType { get; set; }

    [SearchAvailableObjectsServiceType(typeof(PositionPropertyTypeSearchService))]
    public abstract Position? SpecificPosition { get; set; }

    [SearchAvailableObjectsServiceType(typeof(UserPropertyTypeSearchService))]
    public abstract User? SpecificUser { get; set; }

    [SearchAvailableObjectsServiceType(typeof(AbstractRoleDefinitionPropertyTypeSearchService))]
    public abstract AbstractRoleDefinition? SpecificAbstractRole { get; set; }

    [DBBidirectionalRelation("AccessControlEntries")]
    public abstract AccessControlList? AccessControlList { get; set; }

    [StorageClassNone]
    public SecurableClassDefinition? Class
    {
      get
      {
        if (AccessControlList == null)
          return null;
        return AccessControlList.Class;
      }
    }

    [DBBidirectionalRelation("AccessControlEntry")]
    protected abstract ObjectList<Permission> PermissionsInternal { get; }

    public ReadOnlyCollection<Permission> GetPermissions ()
    {
      if (Class == null)
        throw new InvalidOperationException("Cannot get permissions if no securable class definition is assigned to this access control entry.");

      if (Class.AccessTypes.Count != PermissionsInternal.Count)
      {
        throw new InvalidOperationException(
            string.Format("Inconsistent access type definition found for securable class definition '{0}'.", Class.Name));
      }

      var permissions = (from accessType in Class.AccessTypes
                         // LiNQ join preserves order of OUTER
                         join permission in PermissionsInternal on accessType equals permission.AccessType
                         select permission).ToList().AsReadOnly();

      if (Class.AccessTypes.Count != permissions.Count)
      {
        throw new InvalidOperationException(
            string.Format("Inconsistent access type definition found for securable class definition '{0}'.", Class.Name));
      }

      return permissions;
    }

    public AccessTypeDefinition[] GetAllowedAccessTypes ()
    {
      return PermissionsInternal.Where(p => (p.Allowed.HasValue && p.Allowed.Value)).Select(p => p.AccessType).ToArray();
    }

    public AccessTypeDefinition[] GetDeniedAccessTypes ()
    {
      return PermissionsInternal.Where(p => (p.Allowed.HasValue && !p.Allowed.Value)).Select(p => p.AccessType).ToArray();
    }

    public void AddAccessType (AccessTypeDefinition accessType)
    {
      ArgumentUtility.CheckNotNull("accessType", accessType);

      if (FindPermission(accessType) != null)
      {
        throw new ArgumentException(
            string.Format("The access type '{0}' has already been added to this access control entry.", accessType.Name), "accessType");
      }

      var permission = Permission.NewObject();
      permission.AccessType = accessType;
      permission.Allowed = null;
      PermissionsInternal.Add(permission);
    }

    public void RemoveAccessType (AccessTypeDefinition accessType)
    {
      ArgumentUtility.CheckNotNull("accessType", accessType);

      var permission = FindPermission(accessType);
      if (permission == null)
      {
        throw new ArgumentException(
            string.Format("The access type '{0}' is not associated with the access control entry.", accessType.Name), "accessType");
      }

      permission.Delete();
    }

    public void AllowAccess (AccessTypeDefinition accessType)
    {
      ArgumentUtility.CheckNotNull("accessType", accessType);

      var permission = GetPermission(accessType);
      permission.Allowed = true;
    }

    public void DenyAccess (AccessTypeDefinition accessType)
    {
      ArgumentUtility.CheckNotNull("accessType", accessType);

      var permission = GetPermission(accessType);
      permission.Allowed = false;
    }

    public void RemoveAccess (AccessTypeDefinition accessType)
    {
      ArgumentUtility.CheckNotNull("accessType", accessType);

      var permission = GetPermission(accessType);
      permission.Allowed = null;
    }

    public bool MatchesToken (SecurityToken token)
    {
      ArgumentUtility.CheckNotNull("token", token);

      return _matcher.MatchesToken(token);
    }

    private Permission GetPermission (AccessTypeDefinition accessType)
    {
      var permission = FindPermission(accessType);
      if (permission == null)
      {
        throw new ArgumentException(
            string.Format("The access type '{0}' is not assigned to this access control entry.", accessType.Name), "accessType");
      }

      return permission;
    }

    private Permission? FindPermission (AccessTypeDefinition accessType)
    {
      return PermissionsInternal.Where(p => p.AccessType.ID == accessType.ID).SingleOrDefault();
    }

    //TODO: Rewrite with test

    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting(args);

      _deleteHandler = new DomainObjectDeleteHandler(PermissionsInternal);
    }

    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted(args);

      _deleteHandler?.Delete();
    }

    public AccessControlEntryValidationResult Validate ()
    {
      var result = new AccessControlEntryValidationResult();

      if (!State.IsDeleted)
      {
        if (TenantCondition == TenantCondition.OwningTenant && TenantHierarchyCondition == TenantHierarchyCondition.Undefined)
          result.SetError(AccessControlEntryValidationError.IsTenantHierarchyConditionMissing);

        if (TenantCondition == TenantCondition.OwningTenant && TenantHierarchyCondition == TenantHierarchyCondition.Parent)
          result.SetError(AccessControlEntryValidationError.IsTenantHierarchyConditionOnlyParent);

        if (TenantCondition == TenantCondition.SpecificTenant && SpecificTenant == null)
          result.SetError(AccessControlEntryValidationError.IsSpecificTenantMissing);

        if (TenantCondition == TenantCondition.SpecificTenant && TenantHierarchyCondition == TenantHierarchyCondition.Undefined)
          result.SetError(AccessControlEntryValidationError.IsTenantHierarchyConditionMissing);

        if (TenantCondition == TenantCondition.SpecificTenant && TenantHierarchyCondition == TenantHierarchyCondition.Parent)
          result.SetError(AccessControlEntryValidationError.IsTenantHierarchyConditionOnlyParent);

        if (GroupCondition == GroupCondition.AnyGroupWithSpecificGroupType && SpecificGroupType == null)
          result.SetError(AccessControlEntryValidationError.IsSpecificGroupTypeMissing);

        if (GroupCondition == GroupCondition.BranchOfOwningGroup && SpecificGroupType == null)
          result.SetError(AccessControlEntryValidationError.IsSpecificGroupTypeMissing);

        if (GroupCondition == GroupCondition.OwningGroup && GroupHierarchyCondition == GroupHierarchyCondition.Undefined)
          result.SetError(AccessControlEntryValidationError.IsGroupHierarchyConditionMissing);

        if (GroupCondition == GroupCondition.OwningGroup && GroupHierarchyCondition == GroupHierarchyCondition.Parent)
          result.SetError(AccessControlEntryValidationError.IsGroupHierarchyConditionOnlyParent);

        if (GroupCondition == GroupCondition.OwningGroup && GroupHierarchyCondition == GroupHierarchyCondition.Children)
          result.SetError(AccessControlEntryValidationError.IsGroupHierarchyConditionOnlyChildren);

        if (GroupCondition == GroupCondition.SpecificGroup && SpecificGroup == null)
          result.SetError(AccessControlEntryValidationError.IsSpecificGroupMissing);

        if (GroupCondition == GroupCondition.SpecificGroup && GroupHierarchyCondition == GroupHierarchyCondition.Undefined)
          result.SetError(AccessControlEntryValidationError.IsGroupHierarchyConditionMissing);

        if (GroupCondition == GroupCondition.SpecificGroup && GroupHierarchyCondition == GroupHierarchyCondition.Parent)
          result.SetError(AccessControlEntryValidationError.IsGroupHierarchyConditionOnlyParent);

        if (GroupCondition == GroupCondition.SpecificGroup && GroupHierarchyCondition == GroupHierarchyCondition.Children)
          result.SetError(AccessControlEntryValidationError.IsGroupHierarchyConditionOnlyChildren);

        if (UserCondition == UserCondition.SpecificUser && SpecificUser == null)
          result.SetError(AccessControlEntryValidationError.IsSpecificUserMissing);

        if (UserCondition == UserCondition.SpecificPosition && SpecificPosition == null)
          result.SetError(AccessControlEntryValidationError.IsSpecificPositionMissing);
      }

      return result;
    }

    protected override void OnCommitting (DomainObjectCommittingEventArgs args)
    {
      AccessControlEntryValidationResult result = Validate();
      if (!result.IsValid)
        throw new ConstraintViolationException(result.GetErrorMessage());

      if (!State.IsDeleted)
      {
        if (TenantCondition != TenantCondition.SpecificTenant)
          SpecificTenant = null;

        if (TenantCondition != TenantCondition.OwningTenant && TenantCondition != TenantCondition.SpecificTenant)
          TenantHierarchyCondition = TenantHierarchyCondition.Undefined;

        if (GroupCondition != GroupCondition.SpecificGroup)
          SpecificGroup = null;

        if (GroupCondition != GroupCondition.AnyGroupWithSpecificGroupType && GroupCondition != GroupCondition.BranchOfOwningGroup)
          SpecificGroupType = null;

        if (GroupCondition != GroupCondition.OwningGroup && GroupCondition != GroupCondition.SpecificGroup)
          GroupHierarchyCondition = GroupHierarchyCondition.Undefined;

        if (UserCondition != UserCondition.SpecificUser)
          SpecificUser = null;

        if (UserCondition != UserCondition.SpecificPosition)
          SpecificPosition = null;
      }

      base.OnCommitting(args);

      if (Class != null)
        Class.RegisterForCommit();
    }
  }
}
