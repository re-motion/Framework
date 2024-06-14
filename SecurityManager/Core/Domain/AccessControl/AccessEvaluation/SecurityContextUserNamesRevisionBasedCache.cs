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
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Logging;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation
{
  public class SecurityContextUserNamesRevisionBasedCache
      : SecurityContextRevisionBasedCacheBase<SecurityContextUserNamesRevisionBasedCache.Data, UserNamesRevisionKey, GuidRevisionValue>
  {
    public class Data : RevisionBasedData
    {
      public readonly IReadOnlyDictionary<string, IDomainObjectHandle<User>> Users;

      internal Data (
          GuidRevisionValue revision,
          IReadOnlyDictionary<string, IDomainObjectHandle<User>> users)
          : base(revision)
      {
        Users = users;
      }
    }

    private static readonly ILog s_log = LogManager.GetLogger(MethodInfo.GetCurrentMethod()!.DeclaringType!);

    public SecurityContextUserNamesRevisionBasedCache (IUserNamesRevisionProvider revisionProvider)
        : base(revisionProvider)
    {
    }

    public Data GetData ()
    {
      return GetCachedData(UserNamesRevisionKey.Global, Revision.Stale);
    }

    public Data GetDataWithRefresh ()
    {
      return GetCachedData(UserNamesRevisionKey.Global, Revision.Invalidate);
    }

    protected override Data LoadData (GuidRevisionValue revision)
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        s_log.Info("Reset SecurityContextUserRevisionBasedCache cache.");
        using (StopwatchScope.CreateScope(s_log, LogLevel.Info, "Refreshed data in SecurityContextUserRevisionBasedCache. Time taken: {elapsed:ms}ms"))
        {
          var users = LoadUsers();

          return new Data(revision, users);
        }
      }
    }

    private IReadOnlyDictionary<string, IDomainObjectHandle<User>> LoadUsers ()
    {
      var result = GetOrCreateQuery(
          MethodInfo.GetCurrentMethod()!,
          () => from u in QueryFactory.CreateLinqQuery<User>()
                select new { Key = u.UserName, Value = u.ID.GetHandle<User>() });

      using (CreateStopwatchScopeForQueryExecution("users"))
      {
        return result.ToDictionary(u => u.Key, u => u.Value).AsReadOnly();
      }
    }
  }
}
