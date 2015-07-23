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
using Remotion.Data.DomainObjects.Queries;
using Remotion.Logging;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation
{
  public class SecurityContextUserRevisionBasedCache
      : SecurityContextRevisionBasedCacheBase<SecurityContextUserRevisionBasedCache.Data, UserRevisionKey, GuidRevisionValue>
  {
    public class Data : RevisionBasedData
    {
      public readonly Dictionary<string, IDomainObjectHandle<User>> Users;

      internal Data (
          GuidRevisionValue revision,
          Dictionary<string, IDomainObjectHandle<User>> users)
          : base (revision)
      {
        Users = users;
      }
    }

    private static readonly ILog s_log = LogManager.GetLogger (MethodInfo.GetCurrentMethod().DeclaringType);

    public SecurityContextUserRevisionBasedCache (IUserRevisionProvider revisionProvider)
        : base (revisionProvider)
    {
    }

    public Data GetData ()
    {
      return GetCachedData (UserRevisionKey.Global, Revision.Stale);
    }

    public Data GetDataWithRefresh ()
    {
      return GetCachedData (UserRevisionKey.Global, Revision.Invalidate);
    }

    protected override Data LoadData (GuidRevisionValue revision)
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        s_log.Info ("Reset SecurityContextUserRevisionBasedCache cache.");
        using (StopwatchScope.CreateScope (s_log, LogLevel.Info, "Refreshed data in SecurityContextUserRevisionBasedCache. Time taken: {elapsed:ms}ms"))
        {
          var users = LoadUsers();

          return new Data (revision, users);
        }
      }
    }

    private Dictionary<string, IDomainObjectHandle<User>> LoadUsers ()
    {
      var result = GetOrCreateQuery (
          MethodInfo.GetCurrentMethod(),
          () => from u in QueryFactory.CreateLinqQuery<User>()
                select new { Key = u.UserName, Value = u.ID.GetHandle<User>() });

      using (CreateStopwatchScopeForQueryExecution ("users"))
      {
        return result.ToDictionary (u => u.Key, u => u.Value);
      }
    }
  }
}