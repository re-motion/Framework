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
    [LinqPropertyRedirection (typeof (StatePropertyDefinition), "DefinedStatesInternal")]
    public static ObjectList<StateDefinition> GetDefinedStatesForQuery (this StatePropertyDefinition statePropertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("statePropertyDefinition", statePropertyDefinition);

      return new ObjectList<StateDefinition> (statePropertyDefinition.DefinedStates);
    }

    [LinqPropertyRedirection (typeof (StatePropertyDefinition), "StatePropertyReferences")]
    public static ObjectList<StatePropertyReference> GetStatePropertyReferencesForQuery (this StatePropertyDefinition statePropertyDefinition)
    {
      throw new NotSupportedException ("GetStatePropertyReferences() is only supported for building LiNQ query expressions.");
    }

    [LinqPropertyRedirection (typeof (SecurableClassDefinition), "StatePropertyReferences")]
    public static ObjectList<StatePropertyReference> GetStatePropertyReferencesForQuery (this SecurableClassDefinition securableClassDefinition)
    {
      throw new NotSupportedException ("GetStatePropertyReferences() is only supported for building LiNQ query expressions.");
    }

    [LinqPropertyRedirection (typeof (SecurableClassDefinition), "AccessTypeReferences")]
    public static ObjectList<AccessTypeReference> GetAccessTypeReferencesForQuery (this SecurableClassDefinition securableClassDefinition)
    {
      throw new NotSupportedException ("GetAccessTypeReferences() is only supported for building LiNQ query expressions.");
    }

    public static IQueryable<SecurableClassDefinition> FetchDetails (this IQueryable<SecurableClassDefinition> query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      return query.FetchAccessTypes()
                  .FetchStateProperties()
                  .FetchStatelessAccessControlList()
                  .FetchStatefulAcessControlLists();
    }

    private static IQueryable<SecurableClassDefinition> FetchAccessTypes (this IQueryable<SecurableClassDefinition> query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      return query.FetchMany (@class => @class.GetAccessTypeReferencesForQuery()).ThenFetchOne (r => r.AccessType);
    }

    private static IQueryable<SecurableClassDefinition> FetchStateProperties (this IQueryable<SecurableClassDefinition> query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      return query.FetchMany (@class => @class.GetStatePropertyReferencesForQuery())
                  .ThenFetchOne (r => r.StateProperty)
                  .ThenFetchMany (p => p.GetDefinedStatesForQuery());
    }

    private static IQueryable<SecurableClassDefinition> FetchStatelessAccessControlList (this IQueryable<SecurableClassDefinition> query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      return query.FetchOne (cd => cd.StatelessAccessControlList)
                  .ThenFetchMany (acl => acl.AccessControlEntries)
                  .ThenFetchMany (ace => ace.GetPermissionsForQuery());
    }

    private static IQueryable<SecurableClassDefinition> FetchStatefulAcessControlLists (this IQueryable<SecurableClassDefinition> query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      return query.FetchMany (cd => cd.StatefulAccessControlLists)
                  .ThenFetchMany (acl => acl.AccessControlEntries)
                  .ThenFetchMany (ace => ace.GetPermissionsForQuery())
                  .FetchMany (cd => cd.StatefulAccessControlLists)
                  .ThenFetchMany (acl => acl.GetStateCombinationsForQuery())
                  .ThenFetchMany (sc => sc.GetStateUsagesForQuery());
    }
  }
}