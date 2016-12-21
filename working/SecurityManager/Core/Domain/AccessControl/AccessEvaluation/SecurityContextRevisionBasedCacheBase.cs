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
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Linq.ExecutableQueries;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Linq.EagerFetching;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation
{
  public abstract class SecurityContextRevisionBasedCacheBase<TData, TRevisionKey, TRevisionValue>
      : RepositoryBase<TData, TRevisionKey, TRevisionValue>
      where TData : RepositoryBase<TData, TRevisionKey, TRevisionValue>.RevisionBasedData
      where TRevisionKey : IRevisionKey
      where TRevisionValue : IRevisionValue
  {
    private static readonly ILog s_log = LogManager.GetLogger (MethodInfo.GetCurrentMethod().DeclaringType);
    private static readonly ICache<string, IQuery> s_queryCache = CacheFactory.CreateWithLocking<string, IQuery>();

    protected SecurityContextRevisionBasedCacheBase (IRevisionProvider<TRevisionKey, TRevisionValue> revisionProvider)
        : base (revisionProvider)
    {
    }

    private StopwatchScope CreateStopwatchScopeForQueryParsing (string queryName)
    {
      return StopwatchScope.CreateScope (
          s_log,
          LogLevel.Debug,
          "Parsed query for " + GetType().Name + "." + queryName + "(). Time taken: {elapsed:ms}ms");
    }

    protected StopwatchScope CreateStopwatchScopeForQueryExecution (string queryName)
    {
      return StopwatchScope.CreateScope (
          s_log,
          LogLevel.Debug,
          "Fetched " + queryName + " into " + GetType().Name + ". Time taken: {elapsed:ms}ms");
    }

    protected IEnumerable<T> GetOrCreateQuery<T> (MethodBase caller, Func<IQueryable<T>> queryCreator)
    {
      var executableQuery =
          (IExecutableQuery<IEnumerable<T>>) s_queryCache.GetOrCreateValue (caller.Name, key => CreateExecutableQuery (key, queryCreator));

      return executableQuery.Execute (ClientTransaction.Current.QueryManager);
    }

    private IExecutableQuery<IEnumerable<T>> CreateExecutableQuery<T> (string key, Func<IQueryable<T>> queryCreator)
    {
      // Note: Parsing the query takes about 1/3 of the total query time when connected to a local database instance.
      // Unfortunately, the first query also causes the initialization of various caches in re-store, 
      // an operation that cannot be easily excluded from the meassured parsing time. Therefor, the cache mainly helps to alleviate any concerns 
      // about the cost associated with this part of the cache initialization.
      using (CreateStopwatchScopeForQueryParsing (key))
      {
        var queryable = (DomainObjectQueryable<T>) queryCreator();
        var queryExecutor = queryable.GetExecutor();
        var queryModel = queryable.Provider.GenerateQueryModel (queryable.Expression);
        return queryExecutor.QueryGenerator.CreateSequenceQuery<T> (
            "<dynamic query>",
            queryExecutor.StorageProviderDefinition,
            queryModel,
            Enumerable.Empty<FetchQueryModelBuilder>());
      }
    }
  }
}