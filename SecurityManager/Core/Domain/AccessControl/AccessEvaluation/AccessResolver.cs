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
using System.Linq;
using System.Reflection;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.FunctionalProgramming;
using Remotion.Logging;
using Remotion.Security;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation
{
  [ImplementationFor(typeof(IAccessResolver), Lifetime = LifetimeKind.Singleton)]
  public class AccessResolver : IAccessResolver
  {
    private static readonly ILog s_log = LogManager.GetLogger(MethodInfo.GetCurrentMethod()!.DeclaringType!);
    private static readonly QueryCache s_queryCache = new QueryCache();

    public AccessType[] GetAccessTypes (IDomainObjectHandle<AccessControlList> aclHandle, SecurityToken token)
    {
      ArgumentUtility.CheckNotNull("aclHandle", aclHandle);
      ArgumentUtility.CheckNotNull("token", token);

      using (SecurityFreeSection.Activate())
      {
        var clientTransaction = ClientTransaction.CreateRootTransaction();
        using (clientTransaction.EnterDiscardingScope())
        {
          using (StopwatchScope.CreateScope(
              s_log,
              LogLevel.Info,
              string.Format(
                  "Evaluated access types of ACL '{0}' for principal '{1}'. Time taken: {{elapsed:ms}}ms",
                  aclHandle.ObjectID,
                  token.Principal.User != null ? token.Principal.User.ObjectID.ToString() : "<unknown>")))
          {
            Assertion.DebugAssert(ClientTransaction.Current == clientTransaction, "ClientTransaction.Current == clientTransaction");
            LoadAccessTypeDefinitions(clientTransaction);
            var acl = LoadAccessControlList(clientTransaction, aclHandle);

            var accessInformation = acl.GetAccessTypes(token);
            return Array.ConvertAll(accessInformation.AllowedAccessTypes, ConvertToAccessType);
          }
        }
      }
    }

    private static readonly Guid s_aclParameter = Guid.Empty;

    private AccessControlList LoadAccessControlList (ClientTransaction clientTransaction, IDomainObjectHandle<AccessControlList> aclHandle)
    {
      using (StopwatchScope.CreateScope(
          s_log,
          LogLevel.Debug,
          "Fetched ACL '" + aclHandle.ObjectID + "' for AccessResolver. Time taken: {elapsed:ms}ms"))
      {
        var queryTemplate = s_queryCache.GetQuery<AccessControlList>(
            MethodInfo.GetCurrentMethod()!.Name,
            acls => acls.Where(o => s_aclParameter.Equals(o.ID.Value))
                        .Select(o => o)
                        .FetchMany(o => o.AccessControlEntries)
                        .ThenFetchMany(ace => ace.GetPermissionsForQuery()));

        var query = queryTemplate.CreateCopyFromTemplate(new Dictionary<object, object?> { { s_aclParameter, aclHandle.ObjectID.Value } });
        return clientTransaction.QueryManager.GetCollection<AccessControlList>(query)
            .AsEnumerable()
            .Where(acl => acl != null)
            .Select(acl => acl!)
            .Single(() => new ObjectsNotFoundException(EnumerableUtility.Singleton(aclHandle.ObjectID)));
      }
    }

    private void LoadAccessTypeDefinitions (ClientTransaction clientTransaction)
    {
      using (StopwatchScope.CreateScope(s_log, LogLevel.Debug, "Fetched access types for AccessResolver. Time taken: {elapsed:ms}ms"))
      {
        s_queryCache.ExecuteCollectionQuery<AccessTypeDefinition>(
            clientTransaction,
            MethodInfo.GetCurrentMethod()!.Name,
            accessTypes => accessTypes);
      }
    }

    private AccessType ConvertToAccessType (AccessTypeDefinition accessTypeDefinition)
    {
      return AccessType.Get(EnumWrapper.Get(accessTypeDefinition.Name));
    }
  }
}
