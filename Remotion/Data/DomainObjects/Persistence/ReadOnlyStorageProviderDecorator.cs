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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence
{
  /// <summary>
  /// Decorator for <see cref="IReadOnlyStorageProvider"/> to ensure that <see cref="IQuery"/>s have the appropriate <see cref="QueryType"/> when
  /// being executed in the ReadOnly context.
  /// </summary>
  public class ReadOnlyStorageProviderDecorator : IReadOnlyStorageProvider
  {
    public IReadOnlyStorageProvider InnerStorageProvider { get; }

    public ReadOnlyStorageProviderDecorator (IReadOnlyStorageProvider innerStorageProvider)
    {
      ArgumentUtility.CheckNotNull("innerStorageProvider", innerStorageProvider);

      InnerStorageProvider = innerStorageProvider;
    }

    public void Dispose ()
    {
      InnerStorageProvider.Dispose();
    }

    public ObjectLookupResult<DataContainer> LoadDataContainer (ObjectID id)
    {
      ArgumentUtility.CheckNotNull("id", id);

      return InnerStorageProvider.LoadDataContainer(id);
    }

    public IEnumerable<ObjectLookupResult<DataContainer>> LoadDataContainers (IReadOnlyCollection<ObjectID> ids)
    {
      ArgumentUtility.CheckNotNull("ids", ids);

      return InnerStorageProvider.LoadDataContainers(ids);
    }

    public IEnumerable<DataContainer> LoadDataContainersByRelatedID (
        RelationEndPointDefinition relationEndPointDefinition,
        SortExpressionDefinition? sortExpressionDefinition,
        ObjectID relatedID)
    {
      ArgumentUtility.CheckNotNull("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNull("relatedID", relatedID);

      return InnerStorageProvider.LoadDataContainersByRelatedID(relationEndPointDefinition, sortExpressionDefinition, relatedID);
    }

    public IEnumerable<DataContainer?> ExecuteCollectionQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull("query", query);

      CheckQueryType(query);

      return InnerStorageProvider.ExecuteCollectionQuery(query);
    }

    public IEnumerable<IQueryResultRow> ExecuteCustomQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull("query", query);

      CheckQueryType(query);

      return InnerStorageProvider.ExecuteCustomQuery(query);
    }

    public object? ExecuteScalarQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull("query", query);

      CheckQueryType(query);

      return InnerStorageProvider.ExecuteScalarQuery(query);
    }

    public void BeginTransaction ()
    {
      InnerStorageProvider.BeginTransaction();
    }

    public void Commit ()
    {
      InnerStorageProvider.Commit();
    }

    public void Rollback ()
    {
      InnerStorageProvider.Rollback();
    }

    private void CheckQueryType (IQuery query)
    {
      if (query.QueryType is not (QueryType.CollectionReadOnly or QueryType.CustomReadOnly or QueryType.ScalarReadOnly))
        throw new ArgumentException($"Query '{query.ID}' has query type '{query.QueryType}' which cannot be used in a read-only query scenario.", nameof(query));
    }
  }
}
