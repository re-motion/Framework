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
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation
{
  public sealed class SecurityToken
  {
    public static SecurityToken Create (
        [NotNull] Principal principal,
        [CanBeNull] Tenant? owningTenant,
        [CanBeNull] Group? owningGroup,
        [CanBeNull] User? owningUser,
        [NotNull] IEnumerable<IDomainObjectHandle<AbstractRoleDefinition>> abstractRoles)
    {
      ArgumentUtility.CheckNotNull("principal", principal);
      ArgumentUtility.CheckNotNull("abstractRoles", abstractRoles);

      return new SecurityToken(
          principal,
          owningTenant.GetSafeHandle(),
          owningGroup.GetSafeHandle(),
          owningUser.GetSafeHandle(),
          abstractRoles);
    }

    private readonly Principal _principal;
    private readonly IDomainObjectHandle<Tenant>? _owningTenant;
    private readonly IDomainObjectHandle<Group>? _owningGroup;
    private readonly IDomainObjectHandle<User>? _owningUser;
    private readonly ReadOnlyCollection<IDomainObjectHandle<AbstractRoleDefinition>> _abstractRoles;

    public SecurityToken (
        [NotNull] Principal principal,
        [CanBeNull] IDomainObjectHandle<Tenant>? owningTenant,
        [CanBeNull] IDomainObjectHandle<Group>? owningGroup,
        [CanBeNull] IDomainObjectHandle<User>? owningUser,
        [NotNull] IEnumerable<IDomainObjectHandle<AbstractRoleDefinition>> abstractRoles)
    {
      ArgumentUtility.CheckNotNull("principal", principal);
      ArgumentUtility.CheckNotNull("abstractRoles", abstractRoles);

      _principal = principal;
      _owningTenant = owningTenant;
      _owningGroup = owningGroup;
      _owningUser = owningUser;
      _abstractRoles = abstractRoles.ToList().AsReadOnly();
    }

    [NotNull]
    public Principal Principal
    {
      get { return _principal; }
    }

    [CanBeNull]
    public IDomainObjectHandle<Tenant>? OwningTenant
    {
      get { return _owningTenant; }
    }

    [CanBeNull]
    public IDomainObjectHandle<Group>? OwningGroup
    {
      get { return _owningGroup; }
    }

    [CanBeNull]
    public IDomainObjectHandle<User>? OwningUser
    {
      get { return _owningUser; }
    }

    [NotNull]
    public ReadOnlyCollection<IDomainObjectHandle<AbstractRoleDefinition>> AbstractRoles
    {
      get { return _abstractRoles; }
    }
  }
}
