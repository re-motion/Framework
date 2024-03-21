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
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.Persistence
{
  /// <summary>
  /// Provides an API for classes encapsulating the execution of database queries. Subclasses of <see cref="IReadOnlyStorageProvider"/>
  /// are used by <see cref="RootPersistenceStrategy"/> to load <see cref="DataContainer"/>
  /// instances and execute queries.
  /// </summary>
  /// <remarks>
  /// Implementers must ensure that calls to the storage provider do not modify the internal state of the calling
  /// <see cref="RootPersistenceStrategy"/>. They cannot use <see cref="ClientTransaction.Current"/> to
  /// determine the calling <see cref="RootPersistenceStrategy"/> as that property is not guaranteed to be
  /// set by the caller.
  /// </remarks>
  public interface IReadOnlyStorageProvider : IDisposable
  {
    ObjectLookupResult<DataContainer> LoadDataContainer (ObjectID id);

    IEnumerable<ObjectLookupResult<DataContainer>> LoadDataContainers (IReadOnlyCollection<ObjectID> ids);

    IEnumerable<DataContainer> LoadDataContainersByRelatedID (
        RelationEndPointDefinition relationEndPointDefinition,
        SortExpressionDefinition? sortExpressionDefinition,
        ObjectID relatedID);

    IEnumerable<DataContainer?> ExecuteCollectionQuery (IQuery query);

    IEnumerable<IQueryResultRow> ExecuteCustomQuery (IQuery query);

    object? ExecuteScalarQuery (IQuery query);

    void BeginTransaction ();

    void Commit ();

    void Rollback ();
  }
}
