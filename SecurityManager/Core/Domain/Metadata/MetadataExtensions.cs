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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.Metadata
{
  /// <summary>
  /// Defines query extensions for types declared in the <c>Remotion.SecurityManager.Domain.Metadata</c> namespace.
  /// </summary>
  public static class MetadataExtensions
  {
    [LinqPropertyRedirection(typeof(StatePropertyDefinition), "DefinedStatesInternal")]
    public static ObjectList<StateDefinition> GetDefinedStatesForQuery (this StatePropertyDefinition statePropertyDefinition)
    {
      ArgumentUtility.CheckNotNull("statePropertyDefinition", statePropertyDefinition);

      return new ObjectList<StateDefinition>(statePropertyDefinition.DefinedStates);
    }

    [LinqPropertyRedirection(typeof(StatePropertyDefinition), "StatePropertyReferences")]
    public static ObjectList<StatePropertyReference> GetStatePropertyReferencesForQuery (this StatePropertyDefinition statePropertyDefinition)
    {
      throw new NotSupportedException("GetStatePropertyReferences() is only supported for building LiNQ query expressions.");
    }

    [LinqPropertyRedirection(typeof(SecurableClassDefinition), "StatePropertyReferences")]
    public static ObjectList<StatePropertyReference> GetStatePropertyReferencesForQuery (this SecurableClassDefinition securableClassDefinition)
    {
      throw new NotSupportedException("GetStatePropertyReferences() is only supported for building LiNQ query expressions.");
    }

    [LinqPropertyRedirection(typeof(SecurableClassDefinition), "AccessTypeReferences")]
    public static ObjectList<AccessTypeReference> GetAccessTypeReferencesForQuery (this SecurableClassDefinition securableClassDefinition)
    {
      throw new NotSupportedException("GetAccessTypeReferences() is only supported for building LiNQ query expressions.");
    }

    public static IQueryable<SecurableClassDefinition> FetchDetails (this IQueryable<SecurableClassDefinition> query)
    {
      ArgumentUtility.CheckNotNull("query", query);

      return query.FetchAccessTypes()
                  .FetchStateProperties()
                  .FetchStatelessAccessControlList()
                  .FetchStatefulAcessControlLists();
    }

    private static IQueryable<SecurableClassDefinition> FetchAccessTypes (this IQueryable<SecurableClassDefinition> query)
    {
      ArgumentUtility.CheckNotNull("query", query);

      return query.FetchMany(@class => @class.GetAccessTypeReferencesForQuery()).ThenFetchOne(r => r.AccessType);
    }

    private static IQueryable<SecurableClassDefinition> FetchStateProperties (this IQueryable<SecurableClassDefinition> query)
    {
      ArgumentUtility.CheckNotNull("query", query);

      return query.FetchMany(@class => @class.GetStatePropertyReferencesForQuery())
                  .ThenFetchOne(r => r.StateProperty)
                  .ThenFetchMany(p => p.GetDefinedStatesForQuery());
    }

    private static IQueryable<SecurableClassDefinition> FetchStatelessAccessControlList (this IQueryable<SecurableClassDefinition> query)
    {
      ArgumentUtility.CheckNotNull("query", query);

      return query.FetchOne(cd => cd.StatelessAccessControlList)
                  .ThenFetchMany(acl => acl!.AccessControlEntries)
                  .ThenFetchMany(ace => ace.GetPermissionsForQuery());
    }

    private static IQueryable<SecurableClassDefinition> FetchStatefulAcessControlLists (this IQueryable<SecurableClassDefinition> query)
    {
      ArgumentUtility.CheckNotNull("query", query);

      return query.FetchMany(cd => cd.StatefulAccessControlLists)
                  .ThenFetchMany(acl => acl.AccessControlEntries)
                  .ThenFetchMany(ace => ace.GetPermissionsForQuery())
                  .FetchMany(cd => cd.StatefulAccessControlLists)
                  .ThenFetchMany(acl => acl.GetStateCombinationsForQuery())
                  .ThenFetchMany(sc => sc.GetStateUsagesForQuery());
    }
  }
}
