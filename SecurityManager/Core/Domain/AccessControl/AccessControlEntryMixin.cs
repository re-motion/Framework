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
using System.Collections.Generic;
using JetBrains.Annotations;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  internal static class AccessControlDomainObjectExtensions
  {
     public static T GetOrganizationStructureDomainObjectReference<T> ([NotNull] this IDomainObjectHandle<T> handle, [NotNull] ClientTransaction clientTransaction)
        where T : class, ISecurityManagerOrganizationalStructureObject, ISupportsGetObject
    {
      ArgumentUtility.CheckNotNull ("handle", handle);
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      return (T) (object) LifetimeService.GetObjectReference (clientTransaction, handle.ObjectID);
    }
  }

  public interface IAccessControlEntryMixin
  {
    ISecurityManagerTenant SpecificTenant { get; set; }

    ISecurityManagerGroup SpecificGroup { get; set; }

    ISecurityManagerUser SpecificUser { get; set; }

    ISecurityManagerGroupType SpecificGroupType { get; set; }

    ISecurityManagerPosition SpecificPosition { get; set; }
  }

  public interface ISecurityManagerOrganizationalStructureObject : IDomainObject
  {
  }

  public interface ISecurityManagerTenant : ISecurityManagerOrganizationalStructureObject, ISupportsGetObject
  {
    IEnumerable<ISecurityManagerTenant> GetParents ();
    string DisplayName { get; }
  }

  public interface ISecurityManagerGroup : ISecurityManagerOrganizationalStructureObject, ISupportsGetObject
  {
    IEnumerable<ISecurityManagerGroup> GetParents ();
    ISecurityManagerGroupType GroupType { get; }
    string DisplayName { get; }
  }

  public interface ISecurityManagerUser : ISecurityManagerOrganizationalStructureObject, ISupportsGetObject
  {
    IEnumerable<ISecurityManagerSubstitution> GetActiveSubstitutions ();
    ISecurityManagerTenant Tenant { get; }
    IEnumerable<ISecurityManagerRole> Roles { get; }
    string UserName { get; }
    string DisplayName { get; }
    // for sorting
    string LastName { get; }
    // for sorting
    string FirstName { get; }
  }

  public interface ISecurityManagerRole : ISecurityManagerOrganizationalStructureObject
  {
    ISecurityManagerPosition Position { get; }
    ISecurityManagerGroup Group { get; }
    ISecurityManagerUser User { get; }
  }

  public interface ISecurityManagerSubstitution : ISecurityManagerOrganizationalStructureObject
  {
    ISecurityManagerUser SubstitutedUser { get; }
    ISecurityManagerRole SubstitutedRole { get; }
  }

  public interface ISecurityManagerGroupType : ISecurityManagerOrganizationalStructureObject, ISupportsGetObject
  {
  }

  public interface ISecurityManagerPosition : ISecurityManagerOrganizationalStructureObject, ISupportsGetObject
  {
    string DisplayName { get; }
  }

  public interface IAccessControlEntryMixin<TTenant, TGroup, TUser, TGroupType, TPosition>
    where TTenant : DomainObject, ISecurityManagerTenant
    where TGroup : DomainObject, ISecurityManagerGroup
    where TUser : DomainObject, ISecurityManagerUser
    where TGroupType : DomainObject, ISecurityManagerGroupType
    where TPosition : DomainObject, ISecurityManagerPosition
  {
    TTenant SpecificTenant { get; set; }

    TGroup SpecificGroup { get; set; }

    TUser SpecificUser { get; set; }

    TGroupType SpecificGroupType { get; set; }

    TPosition SpecificPosition { get; set; }
  }

  [AcceptsAlphabeticOrdering]
  public class AccessControlEntryMixin<TTenant, TGroup, TUser, TGroupType, TPosition>
      : DomainObjectMixin<AccessControlEntry>, IAccessControlEntryMixin<TTenant, TGroup, TUser, TGroupType, TPosition>, IAccessControlEntryMixin
    where TTenant : DomainObject, ISecurityManagerTenant
    where TGroup : DomainObject, ISecurityManagerGroup
    where TUser : DomainObject, ISecurityManagerUser
    where TGroupType : DomainObject, ISecurityManagerGroupType
    where TPosition : DomainObject, ISecurityManagerPosition
  {
    // ReSharper disable once StaticMemberInGenericType
    private static readonly Type s_domainObjectType = typeof (AccessControlEntryMixin<,,,,>);

    [SearchAvailableObjectsServiceType (typeof (TenantPropertyTypeSearchService))]
    public TTenant SpecificTenant
    {
      get { return Properties[s_domainObjectType, "SpecificTenant"].GetValue<TTenant>(); }
      set { Properties[s_domainObjectType, "SpecificTenant"].SetValue (value); }
    }

    [SearchAvailableObjectsServiceType (typeof (GroupPropertyTypeSearchService))]
    public TGroup SpecificGroup
    {
      get { return Properties[s_domainObjectType, "SpecificGroup"].GetValue<TGroup>(); }
      set { Properties[s_domainObjectType, "SpecificGroup"].SetValue (value); }
    }

    [SearchAvailableObjectsServiceType (typeof (UserPropertyTypeSearchService))]
    public TUser SpecificUser
    {
      get { return Properties[s_domainObjectType, "SpecificUser"].GetValue<TUser>(); }
      set { Properties[s_domainObjectType, "SpecificUser"].SetValue (value); }
    }

    [SearchAvailableObjectsServiceType (typeof (GroupTypePropertyTypeSearchService))]
    public TGroupType SpecificGroupType
    {
      get { return Properties[s_domainObjectType, "SpecificGroupType"].GetValue<TGroupType>(); }
      set { Properties[s_domainObjectType, "SpecificGroupType"].SetValue (value); }
    }

    [SearchAvailableObjectsServiceType (typeof (PositionPropertyTypeSearchService))]
    public TPosition SpecificPosition
    {
      get { return Properties[s_domainObjectType, "SpecificPosition"].GetValue<TPosition>(); }
      set { Properties[s_domainObjectType, "SpecificPosition"].SetValue (value); }
    }

    [ObjectBinding (Visible = false)]
    ISecurityManagerTenant IAccessControlEntryMixin.SpecificTenant
    {
      get { return SpecificTenant; }
      set { SpecificTenant = ArgumentUtility.CheckType<TTenant> ("SpecificTenant", value); }
    }

    [ObjectBinding (Visible = false)]
    ISecurityManagerGroup IAccessControlEntryMixin.SpecificGroup
    {
      get { return SpecificGroup; }
      set { SpecificGroup = ArgumentUtility.CheckType<TGroup> ("SpecificGroup", value); }
    }

    [ObjectBinding (Visible = false)]
    ISecurityManagerUser IAccessControlEntryMixin.SpecificUser
    {
      get { return SpecificUser; }
      set { SpecificUser = ArgumentUtility.CheckType<TUser> ("SpecificUser", value); }
    }

    [ObjectBinding (Visible = false)]
    ISecurityManagerGroupType IAccessControlEntryMixin.SpecificGroupType
    {
      get { return SpecificGroupType; }
      set { SpecificGroupType = ArgumentUtility.CheckType<TGroupType> ("SpecificGroupType", value); }
    }

    [ObjectBinding (Visible = false)]
    ISecurityManagerPosition IAccessControlEntryMixin.SpecificPosition
    {
      get { return SpecificPosition; }
      set { SpecificPosition = ArgumentUtility.CheckType<TPosition> ("SpecificPosition", value); }
    }
  }
}