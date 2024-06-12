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
  public abstract class ClientTransactionListenerBase : IClientTransactionListener
  {
    public virtual void TransactionInitialize (ClientTransaction clientTransaction)
    {
    }

    public virtual void TransactionDiscard (ClientTransaction clientTransaction)
    {
    }

    public virtual void SubTransactionCreating (ClientTransaction clientTransaction)
    {
    }

    public virtual void SubTransactionInitialize (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
    }

    public virtual void SubTransactionCreated (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
    }

    public virtual void NewObjectCreating (ClientTransaction clientTransaction, Type type)
    {
    }

    public virtual void ObjectsLoading (ClientTransaction clientTransaction, IReadOnlyList<ObjectID> objectIDs)
    {
    }

    public virtual void ObjectsLoaded (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> domainObjects)
    {
    }

    public virtual void ObjectsNotFound (ClientTransaction clientTransaction, IReadOnlyList<ObjectID> objectIDs)
    {
    }

    public virtual void ObjectsUnloading (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> unloadedDomainObjects)
    {
    }

    public virtual void ObjectsUnloaded (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> unloadedDomainObjects)
    {
    }

    public virtual void ObjectDeleting (ClientTransaction clientTransaction, DomainObject domainObject)
    {
    }

    public virtual void ObjectDeleted (ClientTransaction clientTransaction, DomainObject domainObject)
    {
    }

    public virtual void PropertyValueReading (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, ValueAccess valueAccess)
    {
    }

    public virtual void PropertyValueRead (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        PropertyDefinition propertyDefinition,
        object? value,
        ValueAccess valueAccess)
    {
    }

    public virtual void PropertyValueChanging (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        PropertyDefinition propertyDefinition,
        object? oldValue,
        object? newValue)
    {
    }

    public virtual void PropertyValueChanged (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        PropertyDefinition propertyDefinition,
        object? oldValue,
        object? newValue)
    {
    }

    public virtual void RelationReading (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        ValueAccess valueAccess)
    {
    }

    public virtual void RelationRead (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject? relatedObject,
        ValueAccess valueAccess)
    {
    }

    public virtual void RelationRead (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        IReadOnlyCollectionData<DomainObject> relatedObjects,
        ValueAccess valueAccess)
    {
    }

    public virtual void RelationChanging (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject? oldRelatedObject,
        DomainObject? newRelatedObject)
    {
    }

    public virtual void RelationChanged (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject? oldRelatedObject,
        DomainObject? newRelatedObject)
    {
    }

    public virtual QueryResult<T> FilterQueryResult<T> (ClientTransaction clientTransaction, QueryResult<T> queryResult)
        where T : DomainObject
    {
      return queryResult;
    }

    public virtual IEnumerable<T> FilterCustomQueryResult<T> (ClientTransaction clientTransaction, IQuery query, IEnumerable<T> results)
    {
      return results;
    }

    public virtual void TransactionCommitting (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> domainObjects, ICommittingEventRegistrar eventRegistrar)
    {
    }

    public virtual void TransactionCommitValidate (ClientTransaction clientTransaction, IReadOnlyList<PersistableData> committedData)
    {
    }

    public virtual void TransactionCommitted (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> domainObjects)
    {
    }

    public virtual void TransactionRollingBack (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> domainObjects)
    {
    }

    public virtual void TransactionRolledBack (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> domainObjects)
    {
    }

    public virtual void RelationEndPointMapRegistering (ClientTransaction clientTransaction, IRelationEndPoint endPoint)
    {
    }

    public virtual void RelationEndPointMapUnregistering (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
    }

    public virtual void RelationEndPointBecomingIncomplete (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
    }

    public virtual void ObjectMarkedInvalid (ClientTransaction clientTransaction, DomainObject domainObject)
    {
    }

    public virtual void ObjectMarkedNotInvalid (ClientTransaction clientTransaction, DomainObject domainObject)
    {
    }

    public virtual void DataContainerMapRegistering (ClientTransaction clientTransaction, DataContainer container)
    {
    }

    public virtual void DataContainerMapUnregistering (ClientTransaction clientTransaction, DataContainer container)
    {
    }

    public virtual void DataContainerStateUpdated (ClientTransaction clientTransaction, DataContainer container, DataContainerState newDataContainerState)
    {
    }

    public virtual void VirtualRelationEndPointStateUpdated (ClientTransaction clientTransaction, RelationEndPointID endPointID, bool? newEndPointChangeState)
    {
    }

    public virtual bool IsNull
    {
      get { return false; }
    }
  }
}
