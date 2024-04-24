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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
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
    private static readonly ILogger s_logger = LazyLoggerFactory.CreateLogger<SecurityContextRevisionBasedCacheBase<TData, TRevisionKey, TRevisionValue>>();
    private static readonly ConcurrentDictionary<string, IQuery> s_queryCache = new ConcurrentDictionary<string, IQuery>();

    protected SecurityContextRevisionBasedCacheBase (IRevisionProvider<TRevisionKey, TRevisionValue> revisionProvider)
        : base(revisionProvider)
    {
    }

    private StopwatchScope CreateStopwatchScopeForQueryParsing (string queryName)
    {
      return StopwatchScope.CreateScope(
          s_logger,
          LogLevel.Debug,
          "Parsed query for " + GetType().Name + "." + queryName + "(). Time taken: {elapsed:ms}ms");
    }

    protected StopwatchScope CreateStopwatchScopeForQueryExecution (string queryName)
    {
      return StopwatchScope.CreateScope(
          s_logger,
          LogLevel.Debug,
          "Fetched " + queryName + " into " + GetType().Name + ". Time taken: {elapsed:ms}ms");
    }

    protected IEnumerable<T> GetOrCreateQuery<T> (MethodBase caller, Func<IQueryable<T>> queryCreator)
    {
      // C# compiler 7.2 does not provide caching for delegate but during query execution there is already a significant amount of GC pressure so the delegate creation does not matter
      var executableQuery = (IExecutableQuery<IEnumerable<T>>)s_queryCache.GetOrAdd(caller.Name, key => CreateExecutableQuery(key, queryCreator));

      Assertion.IsNotNull(ClientTransaction.Current, "ClientTransaction.Current != null");
      return executableQuery.Execute(ClientTransaction.Current.QueryManager);
    }

    private IExecutableQuery<IEnumerable<T>> CreateExecutableQuery<T> (string key, Func<IQueryable<T>> queryCreator)
    {
      // Note: Parsing the query takes about 1/3 of the total query time when connected to a local database instance.
      // Unfortunately, the first query also causes the initialization of various caches in re-store, 
      // an operation that cannot be easily excluded from the meassured parsing time. Therefor, the cache mainly helps to alleviate any concerns 
      // about the cost associated with this part of the cache initialization.
      using (CreateStopwatchScopeForQueryParsing(key))
      {
        var queryable = (DomainObjectQueryable<T>)queryCreator();
        var queryExecutor = queryable.GetExecutor();
        var queryModel = queryable.Provider.GenerateQueryModel(queryable.Expression);
        return queryExecutor.QueryGenerator.CreateSequenceQuery<T>(
            "<dynamic query>",
            queryExecutor.StorageProviderDefinition,
            queryModel,
            Enumerable.Empty<FetchQueryModelBuilder>(),
            queryExecutor.Metadata);
      }
    }
  }
}
