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
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Provides access to the parent transaction for <see cref="SubPersistenceStrategy"/>.
  /// </summary>
  public class ParentTransactionContext : IParentTransactionContext
  {
    private readonly ClientTransaction _parentTransaction;
    private readonly IInvalidDomainObjectManager _parentInvalidDomainObjectManager;

    public ParentTransactionContext (ClientTransaction parentTransaction, IInvalidDomainObjectManager parentInvalidDomainObjectManager)
    {
      ArgumentUtility.CheckNotNull("parentTransaction", parentTransaction);
      ArgumentUtility.CheckNotNull("parentInvalidDomainObjectManager", parentInvalidDomainObjectManager);

      if (parentTransaction.IsWriteable)
      {
        throw new ArgumentException(
            "In order for the parent transaction access to work correctly, the parent transaction needs to be read-only. "
            + "Using ClientTransaction.CreateSubTransaction() to create a subtransaction automatically sets the parent transaction read-only.",
            "parentTransaction");
      }

      _parentTransaction = parentTransaction;
      _parentInvalidDomainObjectManager = parentInvalidDomainObjectManager;
    }

    public ClientTransaction ParentTransaction
    {
      get { return _parentTransaction; }
    }

    public IInvalidDomainObjectManager ParentInvalidDomainObjectManager
    {
      get { return _parentInvalidDomainObjectManager; }
    }

    public ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);
      return _parentTransaction.CreateNewObjectID(classDefinition);
    }

    public DomainObject GetObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);
      return _parentTransaction.GetObject(objectID, false);
    }

    public DomainObject[] GetObjects (IEnumerable<ObjectID> objectIDs)
    {
      ArgumentUtility.CheckNotNull("objectIDs", objectIDs);
      return _parentTransaction.GetObjects<DomainObject>(objectIDs);
    }

    public DomainObject? TryGetObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);
      return _parentTransaction.TryGetObject(objectID);
    }

    public DomainObject?[] TryGetObjects (IEnumerable<ObjectID> objectIDs)
    {
      ArgumentUtility.CheckNotNull("objectIDs", objectIDs);
      return _parentTransaction.TryGetObjects<DomainObject>(objectIDs);
    }

    public DomainObject? ResolveRelatedObject (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull("relationEndPointID", relationEndPointID);
      if (!relationEndPointID.Definition.IsVirtual || relationEndPointID.Definition.Cardinality != CardinalityType.One)
        throw new ArgumentException("EndPoint ID must denote a virtual relation end-point with cardinality one.", "relationEndPointID");

      var endPoint = (IVirtualObjectEndPoint)_parentTransaction.DataManager.GetRelationEndPointWithLazyLoad(relationEndPointID);
      return endPoint.GetData();
    }

    public IEnumerable<DomainObject> ResolveRelatedObjects (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull("relationEndPointID", relationEndPointID);
      if (!relationEndPointID.Definition.IsVirtual || relationEndPointID.Definition.Cardinality != CardinalityType.Many)
        throw new ArgumentException("EndPoint ID must denote a virtual relation end-point with cardinality many.", "relationEndPointID");

      var endPoint = (ICollectionEndPoint<ICollectionEndPointData>)_parentTransaction.DataManager.GetRelationEndPointWithLazyLoad(relationEndPointID);
      return endPoint.GetData();
    }

    public QueryResult<DomainObject> ExecuteCollectionQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull("query", query);
      return _parentTransaction.QueryManager.GetCollection(query);
    }

    public IEnumerable<IQueryResultRow> ExecuteCustomQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull("query", query);
      return _parentTransaction.QueryManager.GetCustom(query, qrr => qrr);
    }

    public object? ExecuteScalarQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull("query", query);
      return _parentTransaction.QueryManager.GetScalar(query);
    }

    public DataContainer? GetDataContainerWithoutLoading (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);
      return _parentTransaction.DataManager.GetDataContainerWithoutLoading(objectID);
    }

    public DataContainer? GetDataContainerWithLazyLoad (ObjectID objectID, bool throwOnNotFound)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);
      return _parentTransaction.DataManager.GetDataContainerWithLazyLoad(objectID, throwOnNotFound);
    }

    public IRelationEndPoint? GetRelationEndPointWithoutLoading (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull("relationEndPointID", relationEndPointID);
      return _parentTransaction.DataManager.GetRelationEndPointWithoutLoading(relationEndPointID);
    }

    public bool IsInvalid (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);
      return _parentInvalidDomainObjectManager.IsInvalid(objectID);
    }

    public IUnlockedParentTransactionContext UnlockParentTransaction ()
    {
      var scope = _parentTransaction.HierarchyManager.Unlock();
      return new UnlockedParentTransactionContext(_parentTransaction, _parentInvalidDomainObjectManager, scope);
    }
  }
}
