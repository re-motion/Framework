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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.Queries.EagerFetching;

namespace Remotion.Data.DomainObjects.Queries
{
  /// <summary>
  /// Represents an executable query.
  /// </summary>
  public interface IQuery
  {
    /// <summary>
    /// Gets a unique identifier for the query.
    /// </summary>
    string ID { get; }

    /// <summary>
    /// Gets metadata associated with the query. If provided, this information can be used for diagnostic and query-hint purposes.
    /// </summary>
    IReadOnlyDictionary<string, object> Metadata { get; }

    /// <summary>
    /// Gets the statement of the query.
    /// </summary>
    /// <remarks>The statement must be understood by the <see cref="Remotion.Data.DomainObjects.Persistence.IReadOnlyStorageProvider"/> or
    /// <see cref="Remotion.Data.DomainObjects.Persistence.IStorageProvider"/> responsible for executing the query.</remarks>
    string Statement { get; }

    /// <summary>
    /// Gets the <see cref="Persistence.Configuration.StorageProviderDefinition"/> of the associated <see cref="Configuration.QueryDefinition"/>.
    /// </summary>
    StorageProviderDefinition StorageProviderDefinition { get; }

    /// <summary>
    /// Gets the type of the collection if the query returns a collection of <see cref="Remotion.Data.DomainObjects.DomainObject"/>s.
    /// </summary> 
    Type? CollectionType { get; }

    /// <summary>
    /// Gets the <see cref="Configuration.QueryType"/> of the query.
    /// </summary>
    QueryType QueryType { get; }

    /// <summary>
    /// Gets the <see cref="QueryParameter"/>s that are used to execute the query.
    /// </summary>
    QueryParameterCollection Parameters { get; }

    /// <summary>
    /// Gets the eager fetch queries associated with this <see cref="IQuery"/> instance.
    /// </summary>
    /// <value>The eager fetch queries associated with this <see cref="IQuery"/> instance by <see cref="RelationEndPointDefinition"/>.</value>
    /// <remarks>
    /// <para>
    /// An eager fetch query is a query that can be used to obtain all related objects for all query result objects for a certain collection relation 
    /// end point. For example, if this <see cref="IQuery"/> returns a set of <c>Order</c> objects order1, order2, and order3, the fetch query for 
    /// <c>Order.OrderItems</c> would return all order items for order1, order2, and order3.
    /// </para>
    /// <para>
    /// For efficiency, the fetch queries should ensure they have the required <c>WHERE</c> conditions so that they only return objects related to the 
    /// original query result. Usually, this means duplicating the <c>WHERE</c> conditions of the original <see cref="IQuery"/> in the fetch queries.
    /// </para>
    /// <para>
    /// It is the responsibility of the supplier of a fetch query to ensure that it will actually return all related objects for the respective
    /// relation property of the objects returned by this <see cref="IQuery"/>. If the fetch query returns an incomplete (or empty) result set,
    /// the relation properties will be filled with incomplete (or empty) collections.
    /// </para>
    /// <para>
    /// On execution of this <see cref="IQuery"/>, the <see cref="IQueryManager"/> will automatically execute the fetch queries, collate their results, 
    /// and register them with the <see cref="ClientTransaction"/> executing the query. Later access to the respective relation properties will not 
    /// result in further database lookups for the objects returned by this <see cref="IQuery"/>.
    /// </para>
    /// <para>
    /// If a fetch query returns related objects for a relation property of an object which has already been loaded (e.g. by accessing the property
    /// before the fetch), those returned objects are ignored.
    /// </para>
    /// </remarks>
    EagerFetchQueryCollection EagerFetchQueries { get; }
  }
}
