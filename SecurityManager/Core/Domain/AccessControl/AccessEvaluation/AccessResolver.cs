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
  [ImplementationFor (typeof (IAccessResolver), Lifetime = LifetimeKind.Singleton)]
  public class AccessResolver : IAccessResolver
  {
    private static readonly ILog s_log = LogManager.GetLogger (MethodInfo.GetCurrentMethod().DeclaringType);
    private static readonly QueryCache s_queryCache = new QueryCache();

    public AccessType[] GetAccessTypes (IDomainObjectHandle<AccessControlList> aclHandle, SecurityToken token)
    {
      ArgumentUtility.CheckNotNull ("aclHandle", aclHandle);
      ArgumentUtility.CheckNotNull ("token", token);

      using (SecurityFreeSection.Activate())
      {
        using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
        {
          using (StopwatchScope.CreateScope (
              s_log,
              LogLevel.Info,
              string.Format (
                  "Evaluated access types of ACL '{0}' for principal '{1}'. Time taken: {{elapsed:ms}}ms",
                  aclHandle.ObjectID,
                  token.Principal.User != null ? token.Principal.User.ObjectID.ToString() : "<unknown>")))
          {
            LoadAccessTypeDefinitions ();
            var acl = LoadAccessControlList (aclHandle);

            var accessInformation = acl.GetAccessTypes (token);
            return Array.ConvertAll (accessInformation.AllowedAccessTypes, ConvertToAccessType);
          }
        }
      }
    }

    private static readonly Guid s_aclParameter = Guid.Empty;

    private AccessControlList LoadAccessControlList (IDomainObjectHandle<AccessControlList> aclHandle)
    {
      using (StopwatchScope.CreateScope (
          s_log,
          LogLevel.Debug,
          "Fetched ACL '" + aclHandle.ObjectID + "' for AccessResolver. Time taken: {elapsed:ms}ms"))
      {
        var queryTemplate = s_queryCache.GetQuery<AccessControlList> (
            MethodInfo.GetCurrentMethod().Name,
            acls => acls.Where (o => s_aclParameter.Equals (o.ID.Value))
                        .Select (o => o)
                        .FetchMany (o => o.AccessControlEntries)
                        .ThenFetchMany (ace => ace.GetPermissionsForQuery()));

        var query = queryTemplate.CreateCopyFromTemplate (new Dictionary<object, object> { { s_aclParameter, aclHandle.ObjectID.Value } });
        return ClientTransaction.Current.QueryManager.GetCollection<AccessControlList> (query)
                                .AsEnumerable()
                                .Single (() => new ObjectsNotFoundException (EnumerableUtility.Singleton (aclHandle.ObjectID)));
      }
    }

    private void LoadAccessTypeDefinitions ()
    {
      using (StopwatchScope.CreateScope (s_log, LogLevel.Debug, "Fetched access types for AccessResolver. Time taken: {elapsed:ms}ms"))
      {
        s_queryCache.ExecuteCollectionQuery<AccessTypeDefinition> (
            ClientTransaction.Current,
            MethodInfo.GetCurrentMethod().Name,
            accessTypes => accessTypes);
      }
    }

    private AccessType ConvertToAccessType (AccessTypeDefinition accessTypeDefinition)
    {
      return AccessType.Get (EnumWrapper.Get (accessTypeDefinition.Name));
    }
  }
}