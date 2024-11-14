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
using Microsoft.Extensions.Logging;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.FunctionalProgramming;
using Remotion.Logging;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation
{
  public sealed class CachedUser : RepositoryBase<CachedUser.Data, UserRevisionKey, GuidRevisionValue>
  {
    //RM-5640: Rewrite with tests

    public sealed class Data : RevisionBasedData
    {
      public readonly User User;

      internal Data (GuidRevisionValue revision, User user)
          : base(revision)
      {
        User = user;
      }
    }

    private const string c_userNameParameter = "<userName>";

    private static readonly ILogger s_logger = LazyLoggerFactory.CreateLogger<CachedUser>();

    // Note: Parsing the query takes about 1/6 of the total query time when connected to a local database instance.
    // Unfortunately, the first query also causes the initialization of various caches in re-store,
    // an operation that cannot be easily excluded from the meassured parsing time. Therefor, the cache mainly helps to alleviate any concerns
    // about the cost associated with this part of the cache initialization.
    private static readonly QueryCache s_queryCache = new QueryCache();

    private readonly string _userName;

    public CachedUser (IRevisionProvider<UserRevisionKey, GuidRevisionValue> revisionProvider, string userName)
        : base(revisionProvider)
    {
      ArgumentUtility.CheckNotNull("userName", userName);

      _userName = userName;
    }

    public User GetValue ()
    {
      return GetCachedData(new UserRevisionKey(_userName)).User;
    }

    protected override Data LoadData (GuidRevisionValue revision)
    {
      s_logger.LogInformation("Reset CachedUser for user '{0}'.", _userName);
      return new Data(revision, GetUser(_userName));
    }

    private User GetUser (string userName)
    {
      using (StopwatchScope.CreateScope(
          s_logger,
          LogLevel.Information,
          "Refreshed data in CachedUser for user '" + userName + "'. Time taken: {elapsed:ms}ms"))
      {
        var clientTransaction = ClientTransaction.CreateRootTransaction();
        return LoadUser(clientTransaction, userName);
      }
    }

    private User LoadUser (ClientTransaction clientTransaction, string userName)
    {
      using (StopwatchScope.CreateScope(
          s_logger,
          LogLevel.Debug,
          "Fetched data into CachedUser for user '" + userName + "'. Time taken: {elapsed:ms}ms"))
      {
        var queryTemplate = s_queryCache.GetQuery<User>(
            MethodBase.GetCurrentMethod()!.Name,
            users => users.Where(u => u.UserName == c_userNameParameter).Select(u => u)
                          .FetchMany(u => u.Roles)
                          .FetchMany(User.SelectSubstitutions()).ThenFetchOne(s => s.SubstitutedRole));

        var query = queryTemplate.CreateCopyFromTemplate(new Dictionary<object, object?> { { c_userNameParameter, userName } });
        return clientTransaction.QueryManager.GetCollection<User>(query)
                                .AsEnumerable()
                                .Where(user => user != null)
                                .Select(user => user!)
                                .Single(() => CreateAccessControlException("The user '{0}' could not be found.", userName));
      }
    }

    private AccessControlException CreateAccessControlException (string message, params object[] args)
    {
      return new AccessControlException(string.Format(message, args));
    }
  }
}
