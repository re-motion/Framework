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
using System.Linq;
using System.Reflection;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Logging;
using Remotion.Security;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation
{
  public class SecurityContextRevisionBasedCache
      : SecurityContextRevisionBasedCacheBase<SecurityContextRevisionBasedCache.Data, RevisionKey, GuidRevisionValue>
  {
    public class Data : RevisionBasedData
    {
      public readonly Dictionary<string, IDomainObjectHandle<Tenant>> Tenants;
      public readonly Dictionary<string, IDomainObjectHandle<Group>> Groups;
      public readonly Dictionary<string, IDomainObjectHandle<Position>> Positions;
      public readonly Dictionary<EnumWrapper, IDomainObjectHandle<AbstractRoleDefinition>> AbstractRoles;
      public readonly Dictionary<string, SecurableClassDefinitionData> Classes;
      public readonly Dictionary<IDomainObjectHandle<StatePropertyDefinition>, ReadOnlyCollectionDecorator<string>> StatePropertyValues;

      internal Data (
          GuidRevisionValue revision,
          Dictionary<string, IDomainObjectHandle<Tenant>> tenants,
          Dictionary<string, IDomainObjectHandle<Group>> groups,
          Dictionary<string, IDomainObjectHandle<Position>> positions,
          Dictionary<EnumWrapper, IDomainObjectHandle<AbstractRoleDefinition>> abstractRoles,
          Dictionary<string, SecurableClassDefinitionData> classes,
          Dictionary<IDomainObjectHandle<StatePropertyDefinition>, ReadOnlyCollectionDecorator<string>> statePropertyValues)
          : base (revision)
      {
        Tenants = tenants;
        Groups = groups;
        Positions = positions;
        AbstractRoles = abstractRoles;
        Classes = classes;
        StatePropertyValues = statePropertyValues;
      }
    }

    private static readonly ILog s_log = LogManager.GetLogger (MethodInfo.GetCurrentMethod().DeclaringType);
    private static readonly RevisionKey s_revisionKey = new RevisionKey();

    public SecurityContextRevisionBasedCache (IDomainRevisionProvider revisionProvider)
        : base (revisionProvider)
    {
    }

    public Data GetData ()
    {
      return GetCachedData (s_revisionKey, Revision.Stale);
    }

    public Data GetDataWithRefresh ()
    {
      return GetCachedData (s_revisionKey, Revision.Invalidate);
    }

    protected override Data LoadData (GuidRevisionValue revision)
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        s_log.Info ("Reset SecurityContextRevisionBasedCache cache.");
        using (StopwatchScope.CreateScope (s_log, LogLevel.Info, "Refreshed data in SecurityContextRevisionBasedCache. Time taken: {elapsed:ms}ms"))
        {
          var tenants = LoadTenants();
          var groups = LoadGroups();
          var positions = LoadPositions();
          var abstractRoles = LoadAbstractRoles();
          var classes = BuildClassCache (
              LoadSecurableClassDefinitions(),
              LoadStatelessAccessControlLists(),
              LoadStatefulAccessControlLists());
          var statePropertyValues = LoadStatePropertyValues();

          return new Data (revision, tenants, groups, positions, abstractRoles, classes, statePropertyValues);
        }
      }
    }

    private Dictionary<string, IDomainObjectHandle<Tenant>> LoadTenants ()
    {
      var result = GetOrCreateQuery (
          MethodInfo.GetCurrentMethod(),
          () => from t in QueryFactory.CreateLinqQuery<Tenant>()
                select new { Key = t.UniqueIdentifier, Value = t.ID.GetHandle<Tenant>() });

      using (CreateStopwatchScopeForQueryExecution ("tenants"))
      {
        return result.ToDictionary (t => t.Key, t => t.Value);
      }
    }

    private Dictionary<string, IDomainObjectHandle<Group>> LoadGroups ()
    {
      var result = GetOrCreateQuery (
          MethodInfo.GetCurrentMethod(),
          () => from g in QueryFactory.CreateLinqQuery<Group>()
                select new { Key = g.UniqueIdentifier, Value = g.ID.GetHandle<Group>() });

      using (CreateStopwatchScopeForQueryExecution ("groups"))
      {
        return result.ToDictionary (g => g.Key, g => g.Value);
      }
    }

    private Dictionary<string, IDomainObjectHandle<Position>> LoadPositions ()
    {
      var result = GetOrCreateQuery (
          MethodInfo.GetCurrentMethod(),
          () => from g in QueryFactory.CreateLinqQuery<Position>()
                select new { Key = g.UniqueIdentifier, Value = g.ID.GetHandle<Position>() });

      using (CreateStopwatchScopeForQueryExecution ("positions"))
      {
        return result.ToDictionary (g => g.Key, g => g.Value);
      }
    }

    private Dictionary<EnumWrapper, IDomainObjectHandle<AbstractRoleDefinition>> LoadAbstractRoles ()
    {
      var result = GetOrCreateQuery (
          MethodInfo.GetCurrentMethod(),
          () => from r in QueryFactory.CreateLinqQuery<AbstractRoleDefinition>()
                select new { Key = r.Name, Value = r.ID.GetHandle<AbstractRoleDefinition>() });

      using (CreateStopwatchScopeForQueryExecution ("abstract roles"))
      {
        return result.ToDictionary (r => EnumWrapper.Get (r.Key), r => r.Value);
      }
    }

    private Dictionary<ObjectID, Tuple<string, ObjectID>> LoadSecurableClassDefinitions ()
    {
      var result = GetOrCreateQuery (
          MethodInfo.GetCurrentMethod(),
          () => from @class in QueryFactory.CreateLinqQuery<SecurableClassDefinition>()
                select new
                       {
                           @class.ID,
                           @class.Name,
                           BaseClassID = @class.BaseClass.ID
                       });

      using (CreateStopwatchScopeForQueryExecution ("securable classes"))
      {
        return result.ToDictionary (c => c.ID, c => Tuple.Create (c.Name, c.BaseClassID));
      }
    }

    private ILookup<ObjectID, StatefulAccessControlListData> LoadStatefulAccessControlLists ()
    {
      var result = GetOrCreateQuery (
          MethodInfo.GetCurrentMethod(),
          () => from acl in QueryFactory.CreateLinqQuery<StatefulAccessControlList>()
                from sc in acl.GetStateCombinationsForQuery()
                from usage in sc.GetStateUsagesForQuery().DefaultIfEmpty()
                from propertyReference in acl.GetClassForQuery().GetStatePropertyReferencesForQuery().DefaultIfEmpty()
                select new
                       {
                           Class = acl.GetClassForQuery().ID,
                           StateCombination = sc.ID.GetHandle<StateCombination>(),
                           Acl = acl.ID.GetHandle<StatefulAccessControlList>(),
                           StatePropertyID = propertyReference.StateProperty.ID,
                           StatePropertyName = propertyReference.StateProperty.Name,
                           StateValue = usage.StateDefinition.Name
                       });

      using (CreateStopwatchScopeForQueryExecution ("stateful ACLs"))
      {
        return result.GroupBy (
            row => new { row.Class, row.Acl, row.StateCombination },
            row => row.StatePropertyID != null
                       ? new State (
                             row.StatePropertyID.GetHandle<StatePropertyDefinition>(),
                             row.StatePropertyName,
                             row.StateValue)
                       : null)
                     .ToLookup (g => g.Key.Class, g => new StatefulAccessControlListData (g.Key.Acl, g.Where (s => s != null)));
      }
    }

    private Dictionary<ObjectID, IDomainObjectHandle<StatelessAccessControlList>> LoadStatelessAccessControlLists ()
    {
      var result = GetOrCreateQuery (
          MethodInfo.GetCurrentMethod(),
          () => from acl in QueryFactory.CreateLinqQuery<StatelessAccessControlList>()
                select new { Class = acl.GetClassForQuery().ID, Acl = acl.ID.GetHandle<StatelessAccessControlList>() });

      using (CreateStopwatchScopeForQueryExecution ("stateless ACLs"))
      {
        return result.ToDictionary (o => o.Class, o => o.Acl);
      }
    }

    private Dictionary<string, SecurableClassDefinitionData> BuildClassCache (
        IDictionary<ObjectID, Tuple<string, ObjectID>> classes,
        IDictionary<ObjectID, IDomainObjectHandle<StatelessAccessControlList>> statelessAcls,
        ILookup<ObjectID, StatefulAccessControlListData> statefulAcls)
    {
      return classes.ToDictionary (
          c => c.Value.Item1,
          c => new SecurableClassDefinitionData (
                   c.Value.Item2 != null ? classes[c.Value.Item2].Item1 : null,
                   statelessAcls.GetValueOrDefault (c.Key),
                   statefulAcls[c.Key]));
    }

    private Dictionary<IDomainObjectHandle<StatePropertyDefinition>, ReadOnlyCollectionDecorator<string>> LoadStatePropertyValues ()
    {
      var result = GetOrCreateQuery (
          MethodInfo.GetCurrentMethod(),
          () => from s in QueryFactory.CreateLinqQuery<StateDefinition>()
                select
                    new
                    {
                        PropertyHandle = s.StateProperty.ID.GetHandle<StatePropertyDefinition>(),
                        PropertyValue = s.Name
                    });

      using (CreateStopwatchScopeForQueryExecution ("state properties"))
      {
        var lookUp = result.ToLookup (o => o.PropertyHandle, o => o.PropertyValue);
        return lookUp.ToDictionary (o => o.Key, o => o.ToArray().AsReadOnly());
      }
    }
  }
}