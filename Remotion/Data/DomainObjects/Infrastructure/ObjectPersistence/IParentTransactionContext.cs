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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Provides access to the parent transaction for <see cref="SubPersistenceStrategy"/>.
  /// </summary>
  public interface IParentTransactionContext
  {
    ObjectID CreateNewObjectID (ClassDefinition classDefinition);

    IDomainObject GetObject (ObjectID objectID);
    IDomainObject[] GetObjects (IEnumerable<ObjectID> objectIDs);
    IDomainObject? TryGetObject (ObjectID objectID);
    IDomainObject?[] TryGetObjects (IEnumerable<ObjectID> objectIDs);

    IDomainObject? ResolveRelatedObject (RelationEndPointID relationEndPointID);
    IEnumerable<IDomainObject> ResolveRelatedObjects (RelationEndPointID relationEndPointID);

    QueryResult<DomainObject> ExecuteCollectionQuery (IQuery query);
    IEnumerable<IQueryResultRow> ExecuteCustomQuery (IQuery query);
    object? ExecuteScalarQuery (IQuery query);

    DataContainer? GetDataContainerWithoutLoading (ObjectID objectID);
    DataContainer? GetDataContainerWithLazyLoad (ObjectID objectID, bool throwOnNotFound);
    IRelationEndPoint? GetRelationEndPointWithoutLoading (RelationEndPointID relationEndPointID);

    bool IsInvalid (ObjectID objectID);

    IUnlockedParentTransactionContext UnlockParentTransaction ();
  }
}
