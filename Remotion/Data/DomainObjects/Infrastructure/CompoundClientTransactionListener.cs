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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Implements a collection of <see cref="IClientTransactionListener"/> objects.
  /// </summary>
  [Serializable]
  public class CompoundClientTransactionListener : IClientTransactionListener
  {
    private readonly List<IClientTransactionListener> _listeners = new List<IClientTransactionListener>();

    public IEnumerable<IClientTransactionListener> Listeners
    {
      get { return _listeners; }
    }

    public void AddListener (IClientTransactionListener listener)
    {
      ArgumentUtility.CheckNotNull("listener", listener);

      _listeners.Add(listener);
    }

    public void RemoveListener (IClientTransactionListener listener)
    {
      ArgumentUtility.CheckNotNull("listener", listener);

      _listeners.Remove(listener);
    }

    public virtual void TransactionInitialize (ClientTransaction clientTransaction)
    {
      foreach (var listener in _listeners)
        listener.TransactionInitialize(clientTransaction);
    }

    public virtual void TransactionDiscard (ClientTransaction clientTransaction)
    {
      foreach (var listener in _listeners)
        listener.TransactionDiscard(clientTransaction);
    }

    public virtual void SubTransactionCreating (ClientTransaction clientTransaction)
    {
      foreach (var listener in _listeners)
        listener.SubTransactionCreating(clientTransaction);
    }

    public virtual void SubTransactionInitialize (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      foreach (var listener in _listeners)
        listener.SubTransactionInitialize(clientTransaction, subTransaction);
    }

    public virtual void SubTransactionCreated (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      foreach (var listener in _listeners)
        listener.SubTransactionCreated(clientTransaction, subTransaction);
    }

    public virtual void NewObjectCreating (ClientTransaction clientTransaction, Type type)
    {
      foreach (var listener in _listeners)
        listener.NewObjectCreating(clientTransaction, type);
    }

    public virtual void ObjectsLoading (ClientTransaction clientTransaction, IReadOnlyList<ObjectID> objectIDs)
    {
      foreach (var listener in _listeners)
        listener.ObjectsLoading(clientTransaction, objectIDs);
    }

    public virtual void ObjectsUnloaded (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> unloadedDomainObjects)
    {
      foreach (var listener in _listeners)
        listener.ObjectsUnloaded(clientTransaction, unloadedDomainObjects);
    }

    public virtual void ObjectsLoaded (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> domainObjects)
    {
      foreach (var listener in _listeners)
        listener.ObjectsLoaded(clientTransaction, domainObjects);
    }

    public void ObjectsNotFound (ClientTransaction clientTransaction, IReadOnlyList<ObjectID> objectIDs)
    {
      foreach (var listener in _listeners)
        listener.ObjectsNotFound(clientTransaction, objectIDs);
    }

    public virtual void ObjectsUnloading (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> unloadedDomainObjects)
    {
      foreach (var listener in _listeners)
        listener.ObjectsUnloading(clientTransaction, unloadedDomainObjects);
    }

    public virtual void ObjectDeleting (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      foreach (var listener in _listeners)
        listener.ObjectDeleting(clientTransaction, domainObject);
    }

    public virtual void ObjectDeleted (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      foreach (var listener in _listeners)
        listener.ObjectDeleted(clientTransaction, domainObject);
    }

    public virtual void PropertyValueReading (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, ValueAccess valueAccess)
    {
      foreach (var listener in _listeners)
        listener.PropertyValueReading(clientTransaction, domainObject, propertyDefinition, valueAccess);
    }

    public virtual void PropertyValueRead (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        PropertyDefinition propertyDefinition,
        object? value,
        ValueAccess valueAccess)
    {
      foreach (var listener in _listeners)
        listener.PropertyValueRead(clientTransaction, domainObject, propertyDefinition, value, valueAccess);
    }

    public virtual void PropertyValueChanging (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        PropertyDefinition propertyDefinition,
        object? oldValue,
        object? newValue)
    {
      foreach (var listener in _listeners)
        listener.PropertyValueChanging(clientTransaction, domainObject, propertyDefinition, oldValue, newValue);
    }

    public virtual void PropertyValueChanged (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        PropertyDefinition propertyDefinition,
        object? oldValue,
        object? newValue)
    {
      foreach (var listener in _listeners)
        listener.PropertyValueChanged(clientTransaction, domainObject, propertyDefinition, oldValue, newValue);
    }

    public virtual void RelationReading (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        ValueAccess valueAccess)
    {
      foreach (var listener in _listeners)
        listener.RelationReading(clientTransaction, domainObject, relationEndPointDefinition, valueAccess);
    }

    public virtual void RelationRead (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject? relatedObject,
        ValueAccess valueAccess)
    {
      foreach (var listener in _listeners)
        listener.RelationRead(clientTransaction, domainObject, relationEndPointDefinition, relatedObject, valueAccess);
    }

    public virtual void RelationRead (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        IReadOnlyCollectionData<IDomainObject> relatedObjects,
        ValueAccess valueAccess)
    {
      foreach (var listener in _listeners)
        listener.RelationRead(clientTransaction, domainObject, relationEndPointDefinition, relatedObjects, valueAccess);
    }

    public virtual void RelationChanging (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject? oldRelatedObject,
        DomainObject? newRelatedObject)
    {
      foreach (var listener in _listeners)
        listener.RelationChanging(clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);
    }

    public virtual void RelationChanged (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject? oldRelatedObject,
        DomainObject? newRelatedObject)
    {
      foreach (var listener in _listeners)
        listener.RelationChanged(clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);
    }

    public virtual QueryResult<T> FilterQueryResult<T> (ClientTransaction clientTransaction, QueryResult<T> queryResult)
        where T : DomainObject
    {
      return _listeners.Aggregate(queryResult, (current, listener) => listener.FilterQueryResult(clientTransaction, current));
    }

    public virtual IEnumerable<T> FilterCustomQueryResult<T> (ClientTransaction clientTransaction, IQuery query, IEnumerable<T> results)
    {
      return _listeners.Aggregate(results, (current, listener) => listener.FilterCustomQueryResult(clientTransaction, query, current));
    }

    public virtual void TransactionCommitting (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> domainObjects, ICommittingEventRegistrar eventRegistrar)
    {
      foreach (var listener in _listeners)
        listener.TransactionCommitting(clientTransaction, domainObjects, eventRegistrar);
    }

    public virtual void TransactionCommitValidate (ClientTransaction clientTransaction, IReadOnlyList<PersistableData> committedData)
    {
      foreach (var listener in _listeners)
        listener.TransactionCommitValidate(clientTransaction, committedData);
    }

    public virtual void TransactionCommitted (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> domainObjects)
    {
      foreach (var listener in _listeners)
        listener.TransactionCommitted(clientTransaction, domainObjects);
    }

    public virtual void TransactionRollingBack (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> domainObjects)
    {
      foreach (var listener in _listeners)
        listener.TransactionRollingBack(clientTransaction, domainObjects);
    }

    public virtual void TransactionRolledBack (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> domainObjects)
    {
      foreach (var listener in _listeners)
        listener.TransactionRolledBack(clientTransaction, domainObjects);
    }

    public virtual void RelationEndPointMapRegistering (ClientTransaction clientTransaction, IRelationEndPoint endPoint)
    {
      foreach (var listener in _listeners)
        listener.RelationEndPointMapRegistering(clientTransaction, endPoint);
    }

    public virtual void RelationEndPointMapUnregistering (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      foreach (var listener in _listeners)
        listener.RelationEndPointMapUnregistering(clientTransaction, endPointID);
    }

    public virtual void RelationEndPointBecomingIncomplete (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      foreach (var listener in _listeners)
        listener.RelationEndPointBecomingIncomplete(clientTransaction, endPointID);
    }

    public virtual void ObjectMarkedInvalid (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      foreach (var listener in _listeners)
        listener.ObjectMarkedInvalid(clientTransaction, domainObject);
    }

    public virtual void ObjectMarkedNotInvalid (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      foreach (var listener in _listeners)
        listener.ObjectMarkedNotInvalid(clientTransaction, domainObject);
    }

    public virtual void DataContainerMapRegistering (ClientTransaction clientTransaction, DataContainer container)
    {
      foreach (var listener in _listeners)
        listener.DataContainerMapRegistering(clientTransaction, container);
    }

    public virtual void DataContainerMapUnregistering (ClientTransaction clientTransaction, DataContainer container)
    {
      foreach (var listener in _listeners)
        listener.DataContainerMapUnregistering(clientTransaction, container);
    }

    public virtual void DataContainerStateUpdated (ClientTransaction clientTransaction, DataContainer container, DataContainerState newDataContainerState)
    {
      foreach (var listener in _listeners)
        listener.DataContainerStateUpdated(clientTransaction, container, newDataContainerState);
    }

    public virtual void VirtualRelationEndPointStateUpdated (ClientTransaction clientTransaction, RelationEndPointID endPointID, bool? newEndPointChangeState)
    {
      foreach (var listener in _listeners)
        listener.VirtualRelationEndPointStateUpdated(clientTransaction, endPointID, newEndPointChangeState);
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
