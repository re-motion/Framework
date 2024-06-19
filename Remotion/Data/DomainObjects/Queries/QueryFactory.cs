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
using Remotion.Collections;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Linq;
using Remotion.Linq.EagerFetching;
using Remotion.Linq.Parsing.Structure;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries
{
  /// <summary>
  /// Provides a central entry point to get instances of <see cref="IQuery"/> and <see cref="DomainObjectQueryable{T}"/> query objects. Use this 
  /// factory to create LINQ queries or to read queries from the <see cref="QueryConfiguration"/>.
  /// </summary>
  public static class QueryFactory
  {
    // Use DoubleCheckedLockingContainers to ensure that the registries are created as lazily as possible in order to allow users to register
    // customizers via IoC
    private static readonly DoubleCheckedLockingContainer<ILinqProviderComponentFactory> s_linqProviderComponentFactory =
        new DoubleCheckedLockingContainer<ILinqProviderComponentFactory>(() => SafeServiceLocator.Current.GetInstance<ILinqProviderComponentFactory>());
    private static readonly DoubleCheckedLockingContainer<IQueryParser> s_queryParser =
        new DoubleCheckedLockingContainer<IQueryParser>(() => s_linqProviderComponentFactory.Value.CreateQueryParser());
    private static readonly IReadOnlyDictionary<string, object> s_emptyMetadata = new Dictionary<string, object>().AsReadOnly<string, object>();

    /// <summary>
    /// Creates a <see cref="DomainObjectQueryable{T}"/> used as the entry point to a LINQ query with the default implementation of the SQL
    /// generation.
    /// </summary>
    /// <typeparam name="T">The <see cref="DomainObject"/> type to be queried.</typeparam>
    /// <returns>A <see cref="DomainObjectQueryable{T}"/> object as an entry point to a LINQ query.</returns>
    /// <remarks>
    /// Calls <see cref="CreateLinqQuery{T}()"/> with a default id set to '&lt;dynamic query&gt;' and
    /// an empty metadata dictionary.
    /// </remarks>
    /// <example>
    /// The following example used <see cref="CreateLinqQuery{T}()"/> to retrieve
    /// an entry point for a query that selects a number of <c>Order</c> objects, filters them by <c>OrderNumber</c>, and orders them by name of
    /// customer (which includes an implicit join between <c>Order</c> and <c>Customer</c> objects).
    /// <code>
    /// var query =
    ///     from o in QueryFactory.CreateLinqQuery&lt;Order&gt; ()
    ///     where o.OrderNumber &lt;= 4
    ///     orderby o.Customer.Name
    ///     select o;
    /// var result = query.ToArray();
    /// </code>
    /// </example>
    public static IQueryable<T> CreateLinqQuery<T> ()
        where T: DomainObject
    {
      return CreateLinqQuery<T>("<dynamic query>", s_emptyMetadata);
    }

    /// <summary>
    /// Creates a <see cref="DomainObjectQueryable{T}"/> used as the entry point to a LINQ query with the default implementation of the SQL 
    /// generation.
    /// </summary>
    /// <typeparam name="T">The <see cref="DomainObject"/> type to be queried.</typeparam>
    /// <returns>A <see cref="DomainObjectQueryable{T}"/> object as an entry point to a LINQ query.</returns>
    /// <param name="id">The id that is configured for the create <see cref="IQueryable{T}"/>.</param>
    /// <param name="metadata">The metadata can be used to e.g. provide diagnostic information or query hints.</param>
    /// <example>
    /// The following example used <see cref="CreateLinqQuery{T}(string, IReadOnlyDictionary{string,object})"/> to retrieve
    /// an entry point for a query that selects a number of <c>Order</c> objects, filters them by <c>OrderNumber</c>, and orders them by name of
    /// customer (which includes an implicit join between <c>Order</c> and <c>Customer</c> objects).
    /// <code>
    /// var query =
    ///     from o in QueryFactory.CreateLinqQuery&lt;Order&gt; ("id", metadataDictionary)
    ///     where o.OrderNumber &lt;= 4
    ///     orderby o.Customer.Name
    ///     select o;
    ///  var result = query.ToArray();
    /// </code>
    /// </example>
    public static IQueryable<T> CreateLinqQuery<T> (string id, IReadOnlyDictionary<string, object> metadata)
        where T: DomainObject
    {
      ArgumentUtility.CheckNotNullOrEmpty("id", id);
      ArgumentUtility.CheckNotNull("metadata", metadata);

      var startingClassDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(T));
      var providerDefinition = startingClassDefinition.StorageEntityDefinition.StorageProviderDefinition;

      var executor = s_linqProviderComponentFactory.Value.CreateQueryExecutor(providerDefinition, id, metadata);
      return CreateLinqQuery<T>(s_queryParser.Value, executor);
    }

    /// <summary>
    /// Creates a <see cref="DomainObjectQueryable{T}"/> used as the entry point to a LINQ query 
    /// with user defined query analysis or SQL generation.
    /// </summary>
    /// <typeparam name="T">The <see cref="DomainObject"/> type to be queried.</typeparam>
    /// <param name="queryParser">The <see cref="IQueryParser"/> used to parse queries. Specify an instance of <see cref="QueryParser"/>
    ///   for default behavior. See <see cref="QueryParser.CreateDefault"/></param>
    /// <param name="executor">The <see cref="IQueryExecutor"/> that is used for the query. Specify an instance of 
    ///   <see cref="DomainObjectQueryExecutor"/> for default behavior.</param>
    /// <returns>A <see cref="DomainObjectQueryable{T}"/> object as an entry point to a LINQ query.</returns>
    public static IQueryable<T> CreateLinqQuery<T> (IQueryParser queryParser, IQueryExecutor executor)
        where T: DomainObject
    {
      ArgumentUtility.CheckNotNull("executor", executor);
      ArgumentUtility.CheckNotNull("queryParser", queryParser);

      return s_linqProviderComponentFactory.Value.CreateQueryable<T>(queryParser, executor);
    }

    /// <summary>
    /// Creates a new query object from a given <paramref name="queryDefinition"/>.
    /// </summary>
    /// <param name="queryDefinition">The query definition to construct a query from.</param>
    /// <returns>An implementation of <see cref="IQuery"/> corresponding to <paramref name="queryDefinition"/>.</returns>
    public static IQuery CreateQuery (QueryDefinition queryDefinition)
    {
      ArgumentUtility.CheckNotNull("queryDefinition", queryDefinition);
      return CreateQuery(queryDefinition, new QueryParameterCollection());
    }

    /// <summary>
    /// Creates a new query object from a given <paramref name="queryDefinition"/>.
    /// </summary>
    /// <param name="queryDefinition">The query definition to construct a query from.</param>
    /// <param name="queryParameterCollection">The parameter collection to use for the query.</param>
    /// <returns>An implementation of <see cref="IQuery"/> corresponding to <paramref name="queryDefinition"/>.</returns>
    public static IQuery CreateQuery (QueryDefinition queryDefinition, QueryParameterCollection queryParameterCollection)
    {
      ArgumentUtility.CheckNotNull("queryDefinition", queryDefinition);
      ArgumentUtility.CheckNotNull("queryParameterCollection", queryParameterCollection);

      return new Query(queryDefinition, queryParameterCollection);
    }

    /// <summary>
    /// Creates a new query object from a given LINQ query.
    /// </summary>
    /// <param name="id">The ID to assign to the query.</param>
    /// <param name="queryable">The queryable constituting the LINQ query. This must be obtained by forming a LINQ query starting with an instance of 
    /// <see cref="DomainObjectQueryable{T}"/>. Use <see cref="CreateLinqQuery{T}()"/> to create such a query source.</param>
    /// <returns>An implementation of <see cref="IQuery"/> holding the parsed LINQ query data.</returns>
    /// <remarks>
    /// <para>
    /// Note that parts of the <paramref name="queryable"/> might not be represented in the returned <see cref="IQuery"/>. For example,
    /// query methods such as <see cref="Queryable.SingleOrDefault{TSource}(System.Linq.IQueryable{TSource})"/> 
    /// need some code to be executed in memory in order to throw the right exception if too many items are returned by the query, or to return a 
    /// default value if no item is returned. The <see cref="IQuery"/> will not know about those in-memory parts.
    /// </para>
    /// <para>
    /// In addition, scalar queries cannot be created using this method because they cannot be represented as a <see cref="IQueryable"/>.
    /// (The methods that generate scalar queries, such as <see cref="Queryable.Count{TSource}(System.Linq.IQueryable{TSource})"/>, alsways
    /// execute the query immediately instead of returning an <see cref="IQueryable"/>.)
    /// </para>
    /// </remarks>
    public static IQuery CreateQuery<T> (string id, IQueryable queryable)
    {
      ArgumentUtility.CheckNotNull("queryable", queryable);
      ArgumentUtility.CheckNotNullOrEmpty("id", id);

      var provider = queryable.Provider as QueryProviderBase;
      var queryExecutor = provider != null ? provider.Executor as DomainObjectQueryExecutor : null;

      if (provider == null || queryExecutor == null)
      {
        string message = string.Format(
            "The given queryable must stem from an instance of DomainObjectQueryable. Instead, it is of type '{0}',"
            + " with a query provider of type '{1}'. Be sure to use QueryFactory.CreateLinqQuery to create the queryable instance, and only use "
            + "standard query methods on it.",
            queryable.GetType().Name,
            queryable.Provider.GetType().Name);
        throw new ArgumentException(message, "queryable");
      }

      var expression = queryable.Expression;
      var queryModel = provider.GenerateQueryModel(expression);
      var fetchQueryModelBuilders = FetchFilteringQueryModelVisitor.RemoveFetchRequestsFromQueryModel(queryModel);

      return queryExecutor.QueryGenerator.CreateSequenceQuery<T>(
          id,
          queryExecutor.StorageProviderDefinition,
          queryModel,
          fetchQueryModelBuilders,
          queryExecutor.Metadata);
    }


    /// <summary>
    /// Creates a new query object, loading its data from the <see cref="QueryConfiguration"/>.
    /// </summary>
    /// <param name="id">The id of the query to load.</param>
    /// <returns>An implementation of <see cref="IQuery"/> corresponding to the <see cref="QueryDefinition"/> with the given <paramref name="id"/>
    /// held by the current <see cref="QueryConfiguration"/>.</returns>
    public static IQuery CreateQueryFromConfiguration (string id)
    {
      ArgumentUtility.CheckNotNullOrEmpty("id", id);
      return CreateQueryFromConfiguration(id, new QueryParameterCollection());
    }

    /// <summary>
    /// Creates a new query object, loading its data from the <see cref="QueryConfiguration"/>.
    /// </summary>
    /// <param name="id">The id of the query to load.</param>
    /// <param name="queryParameterCollection">The parameter collection to use for the query.</param>
    /// <returns>An implementation of <see cref="IQuery"/> corresponding to the <see cref="QueryDefinition"/> with the given <paramref name="id"/>
    /// held by the current <see cref="QueryConfiguration"/>.</returns>
    public static IQuery CreateQueryFromConfiguration (string id, QueryParameterCollection queryParameterCollection)
    {
      ArgumentUtility.CheckNotNullOrEmpty("id", id);
      var queryDefinition = DomainObjectsConfiguration.Current.Query.QueryDefinitions.GetMandatory(id);
      return new Query(queryDefinition, queryParameterCollection);
    }

    /// <summary>
    /// Creates a new scalar query with the given statement, parameters, and metadata.
    /// Note that creating queries with a hard-coded SQL statement is not very flexible and not portable at all.
    /// Therefore, the <see cref="CreateLinqQuery{T}()"/> and <see cref="CreateQueryFromConfiguration(string)"/>
    /// methods should usually be preferred to this method.
    /// </summary>
    /// <param name="id">A string identifying the query.</param>
    /// <param name="statement">The scalar query statement.</param>
    /// <param name="storageProviderDefinition">The <see cref="StorageProviderDefinition"/> used to execute the query.</param>
    /// <param name="metaData">The optional metadata can be used to e.g. provide diagnostic information or query hints.</param>
    /// <param name="queryParameterCollection">The parameter collection to be used for the query.</param>
    /// <returns>An implementation of <see cref="IQuery"/> with the given statement, parameters, and metadata.</returns>
    public static IQuery CreateScalarQuery (
        string id, StorageProviderDefinition storageProviderDefinition, string statement, QueryParameterCollection queryParameterCollection, IReadOnlyDictionary<string, object>? metaData = null)
    {
      ArgumentUtility.CheckNotNull("id", id);
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("statement", statement);
      ArgumentUtility.CheckNotNull("queryParameterCollection", queryParameterCollection);

      var definition = new QueryDefinition(id, storageProviderDefinition, statement, QueryType.ScalarReadOnly, metaData: metaData);
      return new Query(definition, queryParameterCollection);
    }

    /// <summary>
    /// Creates a new collection query with the given statement, parameters, and metadata.
    /// Note that creating queries with a hard-coded SQL statement is not very flexible and not portable at all.
    /// Therefore, the <see cref="CreateLinqQuery{T}()"/> and <see cref="CreateQueryFromConfiguration(string)"/>
    /// methods should usually be preferred to this method.
    /// </summary>
    /// <param name="id">A string identifying the query.</param>
    /// <param name="storageProviderDefinition">The <see cref="StorageProviderDefinition"/> of the storage provider used to execute the query.</param>
    /// <param name="statement">The collection query statement.</param>
    /// <param name="queryParameterCollection">The parameter collection to be used for the query.</param>
    /// <param name="collectionType">The collection type to be returned from the query. Pass <see cref="DomainObjectCollection"/> if you don't care
    /// about the collection type. The type passed here is used by <see cref="QueryResult{T}.ToCustomCollection"/>.</param>
    /// <param name="metaData">The optional metadata can be used to e.g. provide diagnostic information or query hints.</param>
    /// <returns>An implementation of <see cref="IQuery"/> with the given statement, parameters, and metadata.</returns>
    public static IQuery CreateCollectionQuery (
        string id,
        StorageProviderDefinition storageProviderDefinition,
        string statement,
        QueryParameterCollection queryParameterCollection,
        Type collectionType,
        IReadOnlyDictionary<string, object>? metaData = null)
    {
      ArgumentUtility.CheckNotNull("id", id);
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("statement", statement);
      ArgumentUtility.CheckNotNull("queryParameterCollection", queryParameterCollection);
      ArgumentUtility.CheckNotNull("collectionType", collectionType);

      var definition = new QueryDefinition(id, storageProviderDefinition, statement, QueryType.CollectionReadOnly, collectionType, metaData);
      return new Query(definition, queryParameterCollection);
    }

    /// <summary>
    /// Creates a new custom collection query with the given statement, parameters, and metadata.
    /// Note that creating queries with a hard-coded SQL statement is not very flexible and not portable at all.
    /// Therefore, the <see cref="CreateLinqQuery{T}()"/> and <see cref="CreateQueryFromConfiguration(string)"/>
    /// methods should usually be preferred to this method.
    /// </summary>
    /// <param name="id">A string identifying the query.</param>
    /// <param name="storageProviderDefinition">The <see cref="StorageProviderDefinition"/> of the storage provider used to execute the query.</param>
    /// <param name="statement">The custom query statement.</param>
    /// <param name="queryParameterCollection">The parameter collection to be used for the query.</param>
    /// <param name="metaData">The optional metadata can be used to e.g. provide diagnostic information or query hints.</param>
    /// <returns>An implementation of <see cref="IQuery"/> with the given statement, parameters, and metadata.</returns>
    public static IQuery CreateCustomQuery (
        string id,
        StorageProviderDefinition storageProviderDefinition,
        string statement,
        QueryParameterCollection queryParameterCollection,
        IReadOnlyDictionary<string, object>? metaData = null)
    {
      var definition = new QueryDefinition(id, storageProviderDefinition, statement, QueryType.CustomReadOnly, metaData: metaData);
      return new Query(definition, queryParameterCollection);
    }
  }
}
