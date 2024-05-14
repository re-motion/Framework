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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Linq;
using Remotion.Linq.EagerFetching;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// Provides an implementation of <see cref="IQueryExecutor"/> for <see cref="DomainObject"/> queries.
  /// </summary>
  public class DomainObjectQueryExecutor : IQueryExecutor
  {
    private readonly StorageProviderDefinition _storageProviderDefinition;
    private readonly IDomainObjectQueryGenerator _queryGenerator;
    private readonly string _id;
    private readonly IReadOnlyDictionary<string, object> _metadata;

    public DomainObjectQueryExecutor (StorageProviderDefinition storageProviderDefinition, IDomainObjectQueryGenerator queryGenerator, string id, IReadOnlyDictionary<string, object> metadata)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("queryGenerator", queryGenerator);
      ArgumentUtility.CheckNotNullOrEmpty("id", id);
      ArgumentUtility.CheckNotNull("metadata", metadata);

      _storageProviderDefinition = storageProviderDefinition;
      _queryGenerator = queryGenerator;
      _id = id;
      _metadata = metadata;
    }

    public StorageProviderDefinition StorageProviderDefinition
    {
      get { return _storageProviderDefinition; }
    }

    public IDomainObjectQueryGenerator QueryGenerator
    {
      get { return _queryGenerator; }
    }

    public string ID => _id;

    public IReadOnlyDictionary<string, object> Metadata => _metadata;

    /// <summary>
    /// Creates and executes a given <see cref="QueryModel"/> as an <see cref="IQuery"/> using the current <see cref="ClientTransaction"/>'s
    /// <see cref="ClientTransaction.QueryManager"/>. The query is executed as a scalar query.
    /// </summary>
    /// <param name="queryModel">The generated <see cref="QueryModel"/> of the LINQ query.</param>
    /// <returns>
    /// The result of the executed query, converted to <typeparam name="T"/>.
    /// </returns>
    [return: MaybeNull]
    public T ExecuteScalar<T> (QueryModel queryModel)
    {
      ArgumentUtility.CheckNotNull("queryModel", queryModel);

      if (ClientTransaction.Current == null)
        throw new InvalidOperationException("No ClientTransaction has been associated with the current thread.");

      var fetchQueryModelBuilders = RemoveTrailingFetchRequests(queryModel);
      if (fetchQueryModelBuilders.Any())
        throw new NotSupportedException("Scalar queries cannot perform eager fetching.");

      var query = _queryGenerator.CreateScalarQuery<T>(_id, _storageProviderDefinition, queryModel, _metadata);
      return query.Execute(ClientTransaction.Current.QueryManager);
    }

    /// <summary>
    /// Creates and executes a given <see cref="QueryModel"/> as an <see cref="IQuery"/> using the current <see cref="ClientTransaction"/>'s
    /// <see cref="ClientTransaction.QueryManager"/>. The query is executed as a collection query, and its result set is expected to contain only a 
    /// single element.
    /// </summary>
    /// <param name="queryModel">The generated <see cref="QueryModel"/> of the LINQ query.</param>
    /// <param name="returnDefaultWhenEmpty">If <see langword="true" />, the executor returns a default value when the query's result set is empty; 
    /// if <see langword="false" />, it throws an <see cref="InvalidOperationException"/> when its result set is empty.</param>
    /// <returns>
    /// The result of the executed query, converted to <typeparam name="T"/>.
    /// </returns>
    [return: MaybeNull]
    public T ExecuteSingle<T> (QueryModel queryModel, bool returnDefaultWhenEmpty)
    {
      ArgumentUtility.CheckNotNull("queryModel", queryModel);

      if (ClientTransaction.Current == null)
        throw new InvalidOperationException("No ClientTransaction has been associated with the current thread.");

      var sequence = ExecuteCollection<T>(queryModel);

      if (returnDefaultWhenEmpty)
        return sequence.SingleOrDefault();
      else
        return sequence.Single();
    }

    /// <summary>
    /// Creates and executes a given <see cref="QueryModel"/> as an <see cref="IQuery"/> using the current <see cref="ClientTransaction"/>'s
    /// <see cref="ClientTransaction.QueryManager"/>. The query is executed as a collection query.
    /// </summary>
    /// <param name="queryModel">The generated <see cref="QueryModel"/> of the LINQ query.</param>
    /// <returns>
    /// The result of the executed query as an <see cref="IEnumerable{T}"/>.
    /// </returns>
    public IEnumerable<T> ExecuteCollection<T> (QueryModel queryModel)
    {
      ArgumentUtility.CheckNotNull("queryModel", queryModel);

      if (ClientTransaction.Current == null)
        throw new InvalidOperationException("No ClientTransaction has been associated with the current thread.");

      var fetchQueryModelBuilders = RemoveTrailingFetchRequests(queryModel);

      var query = _queryGenerator.CreateSequenceQuery<T>(
          _id,
          _storageProviderDefinition,
          queryModel,
          fetchQueryModelBuilders,
          _metadata);
      return query.Execute(ClientTransaction.Current.QueryManager);
    }

    private ICollection<FetchQueryModelBuilder> RemoveTrailingFetchRequests (QueryModel queryModel)
    {
      var result = new List<FetchQueryModelBuilder>();
      for (int i = queryModel.ResultOperators.Count - 1; i >= 0 && queryModel.ResultOperators[i] is FetchRequestBase; --i)
      {
        result.Add(new FetchQueryModelBuilder(queryModel.ResultOperators[i] as FetchRequestBase, queryModel, i));
        queryModel.ResultOperators.RemoveAt(i);
      }
      return result;
    }
  }
}
