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
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  [Serializable]
  public class InvalidatedTransactionListener : IClientTransactionListener
  {
    private Exception CreateException ()
    {
      return new InvalidOperationException("The transaction can no longer be used because it has been discarded.");
    }

    public void TransactionInitialize (ClientTransaction clientTransaction)
    {
      throw CreateException();
    }

    public void TransactionDiscard (ClientTransaction clientTransaction)
    {
      throw CreateException();
    }

    public void SubTransactionCreating (ClientTransaction clientTransaction)
    {
      throw CreateException();
    }

    public void SubTransactionInitialize (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      throw CreateException();
    }

    public void SubTransactionCreated (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      throw CreateException();
    }

    public void NewObjectCreating (ClientTransaction clientTransaction, Type type)
    {
      throw CreateException();
    }

    public void ObjectsLoading (ClientTransaction clientTransaction, IReadOnlyList<ObjectID> objectIDs)
    {
      throw CreateException();
    }

    public void ObjectsUnloaded (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> unloadedDomainObjects)
    {
      throw CreateException();
    }

    public void ObjectsLoaded (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> domainObjects)
    {
      throw CreateException();
    }

    public void ObjectsNotFound (ClientTransaction clientTransaction, IReadOnlyList<ObjectID> objectIDs)
    {
      throw CreateException();
    }

    public void ObjectsUnloading (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> unloadedDomainObjects)
    {
      throw CreateException();
    }

    public void ObjectDeleting (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      throw CreateException();
    }

    public void ObjectDeleted (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      throw CreateException();
    }

    public void PropertyValueReading (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, ValueAccess valueAccess)
    {
      throw CreateException();
    }

    public void PropertyValueRead (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, object? value, ValueAccess valueAccess)
    {
      throw CreateException();
    }

    public void PropertyValueChanging (ClientTransaction clientTransaction, DomainObject? domainObject, PropertyDefinition propertyDefinition, object? oldValue, object? newValue)
    {
      throw CreateException();
    }

    public void PropertyValueChanged (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, object? oldValue, object? newValue)
    {
      throw CreateException();
    }

    public void RelationReading (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, ValueAccess valueAccess)
    {
      throw CreateException();
    }

    public void RelationRead (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject? relatedObject,
        ValueAccess valueAccess)
    {
      throw CreateException();
    }

    public void RelationRead (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        IReadOnlyCollectionData<IDomainObject> relatedObjects,
        ValueAccess valueAccess)
    {
      throw CreateException();
    }

    public void RelationChanging (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject? oldRelatedObject,
        DomainObject? newRelatedObject)
    {
      throw CreateException();
    }

    public void RelationChanged (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject? oldRelatedObject,
        DomainObject? newRelatedObject)
    {
      throw CreateException();
    }

    public QueryResult<T> FilterQueryResult<T> (ClientTransaction clientTransaction, QueryResult<T> queryResult)
        where T : DomainObject
    {
      throw CreateException();
    }

    public IEnumerable<T> FilterCustomQueryResult<T> (ClientTransaction clientTransaction, IQuery query, IEnumerable<T> results)
    {
      throw CreateException();
    }

    public void TransactionCommitting (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> domainObjects, ICommittingEventRegistrar eventRegistrar)
    {
      throw CreateException();
    }

    public void TransactionCommitValidate (ClientTransaction clientTransaction, IReadOnlyList<PersistableData> committedData)
    {
      throw CreateException();
    }

    public void TransactionCommitted (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> domainObjects)
    {
      throw CreateException();
    }

    public void TransactionRollingBack (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> domainObjects)
    {
      throw CreateException();
    }

    public void TransactionRolledBack (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> domainObjects)
    {
      throw CreateException();
    }

    public void RelationEndPointMapRegistering (ClientTransaction clientTransaction, IRelationEndPoint endPoint)
    {
      throw CreateException();
    }

    public void RelationEndPointMapUnregistering (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      throw CreateException();
    }

    public void RelationEndPointBecomingIncomplete (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      throw CreateException();
    }

    public void ObjectMarkedInvalid (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      throw CreateException();
    }

    public void ObjectMarkedNotInvalid (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      throw CreateException();
    }

    public void DataContainerMapRegistering (ClientTransaction clientTransaction, DataContainer container)
    {
      throw CreateException();
    }

    public void DataContainerMapUnregistering (ClientTransaction clientTransaction, DataContainer container)
    {
      throw CreateException();
    }

    public void DataContainerStateUpdated (ClientTransaction clientTransaction, DataContainer container, DataContainerState newDataContainerState)
    {
      throw CreateException();
    }

    public void VirtualRelationEndPointStateUpdated (ClientTransaction clientTransaction, RelationEndPointID endPointID, bool? newEndPointChangeState)
    {
      throw CreateException();
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
