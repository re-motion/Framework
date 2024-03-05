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
using Remotion.Data.DomainObjects.Persistence;

namespace Remotion.Data.DomainObjects.Queries
{
  /// <summary>
  /// <see cref="IQueryManager"/> provides an interface for methods to execute queries within a <see cref="ClientTransaction"/>.
  /// </summary>
  public interface IQueryManager
  {
    /// <summary>
    /// Executes a given <see cref="IQuery"/> and returns the scalar value.
    /// </summary>
    /// <param name="query">The query to execute. Must not be <see langword="null"/>.</param>
    /// <returns>The scalar value that is returned by the query.</returns>
    /// <exception cref="System.ArgumentNullException">
    ///   <paramref name="query"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    ///   <paramref name="query"/> does not have a <see cref="Configuration.QueryType"/>
    ///   of <see cref="Configuration.QueryType.ScalarReadOnly"/> or <see cref="Configuration.QueryType.ScalarReadWrite"/>.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.Configuration.StorageProviderConfigurationException">
    ///   The <see cref="IQuery.StorageProviderDefinition"/> of <paramref name="query"/> could not be found.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.PersistenceException">
    ///   The <see cref="Remotion.Data.DomainObjects.Persistence.StorageProvider"/> for the given <see cref="IQuery"/> could not be instantiated.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.StorageProviderException">
    ///   An error occurred while executing the query.
    /// </exception>
    object? GetScalar (IQuery query);

    /// <summary>
    /// Executes a given <see cref="IQuery"/> and returns a collection of the <see cref="DomainObject"/>s returned by the query.
    /// </summary>
    /// <param name="query">The query to execute. Must not be <see langword="null"/>.</param>
    /// <returns>An <see cref="IQueryResult"/> containing the <see cref="DomainObject"/>s returned by the query.</returns>
    /// <exception cref="System.ArgumentNullException">
    ///   <paramref name="query"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    ///   <paramref name="query"/> does not have a <see cref="Configuration.QueryType"/> of
    ///   <see cref="Configuration.QueryType.CollectionReadOnly"/> or <see cref="Configuration.QueryType.CollectionReadWrite"/>.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.Configuration.StorageProviderConfigurationException">
    ///   The <see cref="IQuery.StorageProviderDefinition"/> of <paramref name="query"/> could not be found.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.PersistenceException">
    ///   The <see cref="Remotion.Data.DomainObjects.Persistence.StorageProvider"/> for the given <see cref="IQuery"/> could not be instantiated.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.StorageProviderException">
    ///   An error occurred while executing the query.
    /// </exception>
    /// <remarks>
    /// The <see cref="QueryResult{T}"/> can contain <see langword="null"/> values and deleted items. To check whether an item has been deleted,
    /// check the <see cref="DomainObjectState.IsDeleted"/> flag of its <see cref="DomainObject.State"/> property.
    /// </remarks>
    QueryResult<DomainObject> GetCollection (IQuery query);

    /// <summary>
    /// Executes a given <see cref="IQuery"/> and returns a collection of the <see cref="DomainObject"/>s returned by the query.
    /// </summary>
    /// <param name="query">The query to execute. Must not be <see langword="null"/>.</param>
    /// <typeparam name="T">The type of <see cref="DomainObjects"/> to be returned from the query.</typeparam>
    /// <returns>A <see cref="QueryResult{T}"/> containing the <see cref="DomainObject"/>s returned by the query.</returns>
    /// <exception cref="System.ArgumentNullException">
    ///   <paramref name="query"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidTypeException">
    ///   The objects returned by the <paramref name="query"/> do not match the expected type
    ///   <typeparamref name="T"/> or the configured collection type is not assignable to <see cref="ObjectList{T}"/> with the given <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    ///   <paramref name="query"/> does not have a <see cref="Configuration.QueryType"/> of
    ///   <see cref="Configuration.QueryType.CollectionReadOnly"/> or <see cref="Configuration.QueryType.CollectionReadWrite"/>.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.Configuration.StorageProviderConfigurationException">
    ///   The <see cref="IQuery.StorageProviderDefinition"/> of <paramref name="query"/> could not be found.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.PersistenceException">
    ///   The <see cref="Remotion.Data.DomainObjects.Persistence.StorageProvider"/> for the given <see cref="IQuery"/> could not be instantiated.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.StorageProviderException">
    ///   An error occurred while executing the query.
    /// </exception>
    /// <remarks>
    /// The <see cref="QueryResult{T}"/> can contain <see langword="null"/> values and deleted items. To check whether an item has been deleted,
    /// check the <see cref="DomainObjectState.IsDeleted"/> flag of its <see cref="DomainObject.State"/> property.
    /// </remarks>
    QueryResult<T> GetCollection<T> (IQuery query) where T : DomainObject;

    /// <summary>
    /// Executes a given <see cref="IQuery"/> and uses a delegate to convert the results into arbitrary objects. The result sequence may be lazy and 
    /// streamed, i.e., the query results may be gathered while the sequence is enumerated. The underlying <see cref="StorageProvider" /> may keep 
    /// resources (such as a database connection) open while the sequence is enumerated, see remarks. 
    /// </summary>
    /// <typeparam name="T">The type of values to be returned by the query.</typeparam>
    /// <param name="query">The query to execute. Must not be <see langword="null"/>.</param>
    /// <param name="rowReader">A delegate that is used to convert the query result, represented as an <see cref="IQueryResultRow"/>, 
    /// into the query result type, <typeparamref name="T"/>.</param>
    /// <returns>A collection containing the objects produced by the <paramref name="rowReader"/> delegate.</returns>
    /// <remarks>
    /// The underlying <see cref="StorageProvider" /> may implement this method in such a way that the query is only executed when the returned 
    /// <see cref="IEnumerable{T}"/> is enumerated. Resources, such as a database connection or transaction, may be kept open during that 
    /// enumeration until the <see cref="IEnumerator{T}"/> is disposed. Use "<see cref="Enumerable.ToArray{TSource}"/> or iterate immediately if you 
    /// require those resources to be freed as quickly as possible.
    /// </remarks>
    IEnumerable<T> GetCustom<T> (IQuery query, Func<IQueryResultRow, T> rowReader);
  }
}
