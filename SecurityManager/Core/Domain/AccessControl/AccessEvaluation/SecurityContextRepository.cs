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
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation
{
  /// <summary>
  /// Cache-based implementation of the <see cref="ISecurityContextRepository"/> interface.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  [ImplementationFor(typeof(ISecurityContextRepository), Lifetime = LifetimeKind.Singleton)]
  public sealed class SecurityContextRepository : ISecurityContextRepository
  {
    //TODO RM-5640: test

    private readonly SecurityContextRevisionBasedCache _cache;
    private readonly SecurityContextUserNamesRevisionBasedCache _userNamesCache;

    public SecurityContextRepository (IDomainRevisionProvider revisionProvider, IUserNamesRevisionProvider userRevisionProvider)
    {
      ArgumentUtility.CheckNotNull("revisionProvider", revisionProvider);
      ArgumentUtility.CheckNotNull("userRevisionProvider", userRevisionProvider);

      _cache = new SecurityContextRevisionBasedCache(revisionProvider);
      _userNamesCache = new SecurityContextUserNamesRevisionBasedCache(userRevisionProvider);
    }

    public IDomainObjectHandle<Tenant> GetTenant (string uniqueIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty("uniqueIdentifier", uniqueIdentifier);

      var cachedData = _cache.GetData();
      var tenant = cachedData.Tenants.GetValueOrDefault(uniqueIdentifier);
      if (tenant == null)
      {
        cachedData = _cache.GetDataWithRefresh();
        tenant = cachedData.Tenants.GetValueOrDefault(uniqueIdentifier);
        if (tenant == null)
          throw CreateAccessControlException("The tenant '{0}' could not be found.", uniqueIdentifier);
      }
      return tenant;
    }

    public IDomainObjectHandle<Group> GetGroup (string uniqueIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty("uniqueIdentifier", uniqueIdentifier);

      var cachedData = _cache.GetData();
      var group = cachedData.Groups.GetValueOrDefault(uniqueIdentifier);
      if (group == null)
      {
        cachedData = _cache.GetDataWithRefresh();
        group = cachedData.Groups.GetValueOrDefault(uniqueIdentifier);
        if (group == null)
          throw CreateAccessControlException("The group '{0}' could not be found.", uniqueIdentifier);
      }
      return group;
    }

    public IDomainObjectHandle<User> GetUser (string userName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("userName", userName);

      var cachedData = _userNamesCache.GetData();
      var user = cachedData.Users.GetValueOrDefault(userName);
      if (user == null)
      {
        cachedData = _userNamesCache.GetDataWithRefresh();
        user = cachedData.Users.GetValueOrDefault(userName);
        if (user == null)
          throw CreateAccessControlException("The user '{0}' could not be found.", userName);
      }
      return user;
    }

    public IDomainObjectHandle<Position> GetPosition (string uniqueIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty("uniqueIdentifier", uniqueIdentifier);

      var cachedData = _cache.GetData();
      var position = cachedData.Positions.GetValueOrDefault(uniqueIdentifier);
      if (position == null)
      {
        cachedData = _cache.GetDataWithRefresh();
        position = cachedData.Positions.GetValueOrDefault(uniqueIdentifier);
        if (position == null)
          throw CreateAccessControlException("The position '{0}' could not be found.", uniqueIdentifier);
      }
      return position;
    }

    public IDomainObjectHandle<AbstractRoleDefinition> GetAbstractRole (EnumWrapper name)
    {
      ArgumentUtility.CheckNotNull("name", name);

      var cachedData = _cache.GetData();
      var abstractRole = cachedData.AbstractRoles.GetValueOrDefault(name);
      if (abstractRole == null)
      {
        cachedData = _cache.GetDataWithRefresh();
        abstractRole = cachedData.AbstractRoles.GetValueOrDefault(name);
        if (abstractRole == null)
          throw CreateAccessControlException("The abstract role '{0}' could not be found.", name);
      }
      return abstractRole;
    }

    public SecurableClassDefinitionData GetClass (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty("name", name);

      var cachedData = _cache.GetData();
      var @class = cachedData.Classes.GetValueOrDefault(name);
      if (@class == null)
      {
        cachedData = _cache.GetDataWithRefresh();
        @class = cachedData.Classes.GetValueOrDefault(name);
        if (@class == null)
          throw CreateAccessControlException("The securable class '{0}' could not be found.", name);
      }
      return @class;
    }

    public IReadOnlyCollection<string> GetStatePropertyValues (IDomainObjectHandle<StatePropertyDefinition> stateProperty)
    {
      ArgumentUtility.CheckNotNull("stateProperty", stateProperty);

      var cachedData = _cache.GetData();
      var values = cachedData.StatePropertyValues.GetValueOrDefault(stateProperty);
      if (values == null)
      {
        cachedData = _cache.GetDataWithRefresh();
        values = cachedData.StatePropertyValues.GetValueOrDefault(stateProperty);
        if (values == null)
          throw CreateAccessControlException("The state property with ID '{0}' could not be found.", stateProperty);
      }
      return values;
    }

    private AccessControlException CreateAccessControlException (string message, params object[] args)
    {
      return new AccessControlException(string.Format(message, args));
    }
  }
}
