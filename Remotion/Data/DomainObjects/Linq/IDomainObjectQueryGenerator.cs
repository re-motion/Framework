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
using Remotion.Data.DomainObjects.Linq.ExecutableQueries;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Linq;
using Remotion.Linq.EagerFetching;

namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// Defines an interface for objects generating <see cref="IQuery"/> objects from LINQ queries (parsed by re-linq into <see cref="QueryModel"/> 
  /// instances).
  /// </summary>
  public interface IDomainObjectQueryGenerator
  {
    /// <summary>
    /// Creates an <see cref="IExecutableQuery{T}"/> object for a given <see cref="ClassDefinition"/> based on the given <see cref="QueryModel"/>.
    /// </summary>
    /// <param name="id">The identifier for the resulting query.</param>
    /// <param name="storageProviderDefinition">The <see cref="IReadOnlyStorageProvider"/> for the query.</param>
    /// <param name="queryModel">The <see cref="QueryModel"/> describing the query.</param>
    /// <param name="metadata">The metadata for the query. This parameter can be used to e.g. provide diagnostic information or query hints to the system.</param>
    /// <returns>
    /// An <see cref="IExecutableQuery{T}"/> object corresponding to the given <paramref name="queryModel"/> that returns a scalar value when it is executed.
    /// </returns>
    IExecutableQuery<T> CreateScalarQuery<T> (string id, StorageProviderDefinition storageProviderDefinition, QueryModel queryModel, IReadOnlyDictionary<string, object> metadata);

    /// <summary>
    /// Creates an <see cref="IExecutableQuery{T}"/> collection for a given <see cref="ClassDefinition"/> based on the given <see cref="QueryModel"/>.
    /// </summary>
    /// <param name="id">The identifier for the resulting query.</param>
    /// <param name="storageProviderDefinition">The <see cref="IReadOnlyStorageProvider"/> for the query.</param>
    /// <param name="queryModel">The <see cref="QueryModel"/> describing the query.</param>
    /// <param name="fetchQueryModelBuilders">
    /// A number of <see cref="FetchQueryModelBuilder"/> instances for the fetch requests to be executed together with the query.</param>
    /// <param name="metadata">The metadata for the query. This parameter can be used to e.g. provide diagnostic information or query hints to the system.</param>
    /// <returns>
    /// An <see cref="IExecutableQuery{T}"/> collection corresponding to the given <paramref name="queryModel"/>.
    /// </returns>
    IExecutableQuery<IEnumerable<T>> CreateSequenceQuery<T> (
        string id,
        StorageProviderDefinition storageProviderDefinition,
        QueryModel queryModel,
        IEnumerable<FetchQueryModelBuilder> fetchQueryModelBuilders,
        IReadOnlyDictionary<string, object> metadata);


  }
}
