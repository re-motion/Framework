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

namespace Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement
{
  /// <summary>
  /// An implementation of <see cref="IClientTransactionListener"/> which throws an exception if the <see cref="ClientTransaction"/> is about
  /// to be modified while not <see cref="ClientTransaction.IsWriteable"/>.
  /// </summary>
  [Serializable]
  public class ReadOnlyClientTransactionListener : IClientTransactionListener
  {
    public virtual void TransactionInitialize (ClientTransaction clientTransaction)
    {
      // not handled by this listener
    }

    public virtual void TransactionDiscard (ClientTransaction clientTransaction)
    {
      // allowed
    }

    public virtual void SubTransactionCreating (ClientTransaction clientTransaction)
    {
      EnsureWriteable(clientTransaction, "SubTransactionCreating");
    }

    public virtual void SubTransactionInitialize (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      // Handled by Begin event
    }

    public virtual void SubTransactionCreated (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      // Handled by Begin event
    }

    public virtual void NewObjectCreating (ClientTransaction clientTransaction, Type type)
    {
      EnsureWriteable(clientTransaction, "NewObjectCreating");
    }

    public virtual void ObjectsLoading (ClientTransaction clientTransaction, IReadOnlyList<ObjectID> objectIDs)
    {
      // Allowed - this should be safe since the subtransaction can't have data for this object
      Assertion.DebugAssert(
          clientTransaction.SubTransaction == null
          || objectIDs.All(id => clientTransaction.SubTransaction.DataManager.DataContainers[id] == null));
    }

    public virtual void ObjectsLoaded (ClientTransaction clientTransaction, IReadOnlyList<IDomainObject> domainObjects)
    {
      // Handled by Begin event
    }

    public virtual void ObjectsNotFound (ClientTransaction clientTransaction, IReadOnlyList<ObjectID> objectIDs)
    {
      // Handled by Begin event
    }

    public virtual void ObjectsUnloading (ClientTransaction clientTransaction, IReadOnlyList<IDomainObject> unloadedDomainObjects)
    {
      // Allowed for read-only transactions, as the end-user API always affects the whole hierarchy
      // (DataContainerUnregistering and RelationEndPointUnregistering assert on the actual modification, though)
    }

    public virtual void ObjectsUnloaded (ClientTransaction clientTransaction, IReadOnlyList<IDomainObject> unloadedDomainObjects)
    {
      // Handled by Begin event
    }

    public virtual void ObjectDeleting (ClientTransaction clientTransaction, IDomainObject domainObject)
    {
      EnsureWriteable(clientTransaction, "ObjectDeleting");
    }

    public virtual void ObjectDeleted (ClientTransaction clientTransaction, IDomainObject domainObject)
    {
      // Handled by Begin event
    }

    public virtual void PropertyValueReading (ClientTransaction clientTransaction, IDomainObject domainObject, PropertyDefinition propertyDefinition, ValueAccess valueAccess)
    {
      // Allowed
    }

    public virtual void PropertyValueRead (
        ClientTransaction clientTransaction,
        IDomainObject domainObject,
        PropertyDefinition propertyDefinition,
        object? value,
        ValueAccess valueAccess)
    {
      // Handled by Begin event
    }

    public virtual void PropertyValueChanging (
        ClientTransaction clientTransaction,
        IDomainObject domainObject,
        PropertyDefinition propertyDefinition,
        object? oldValue,
        object? newValue)
    {
      EnsureWriteable(clientTransaction, "PropertyValueChanging");
    }

    public virtual void PropertyValueChanged (
        ClientTransaction clientTransaction,
        IDomainObject domainObject,
        PropertyDefinition propertyDefinition,
        object? oldValue,
        object? newValue)
    {
      // Handled by Begin event
    }

    public virtual void RelationReading (
        ClientTransaction clientTransaction,
        IDomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        ValueAccess valueAccess)
    {
      // Allowed
    }

    public virtual void RelationRead (
        ClientTransaction clientTransaction,
        IDomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        IDomainObject? relatedObject,
        ValueAccess valueAccess)
    {
      // Handled by Begin event
    }

    public virtual void RelationRead (
        ClientTransaction clientTransaction,
        IDomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        IReadOnlyCollectionData<IDomainObject> relatedObjects,
        ValueAccess valueAccess)
    {
      // Handled by Begin event
    }

    public virtual void RelationChanging (
        ClientTransaction clientTransaction,
        IDomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        IDomainObject? oldRelatedObject,
        IDomainObject? newRelatedObject)
    {
      EnsureWriteable(clientTransaction, "RelationChanging");
    }

    public virtual void RelationChanged (
        ClientTransaction clientTransaction,
        IDomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        IDomainObject? oldRelatedObject,
        IDomainObject? newRelatedObject)
    {
      // Handled by Begin event
    }

    public QueryResult<T> FilterQueryResult<T> (ClientTransaction clientTransaction, QueryResult<T> queryResult)
        where T : DomainObject
    {
      // Allowed
      return queryResult;
    }

    public IEnumerable<T> FilterCustomQueryResult<T> (ClientTransaction clientTransaction, IQuery query, IEnumerable<T> results)
    {
      // Allowed
      return results;
    }

    public virtual void TransactionCommitting (ClientTransaction clientTransaction, IReadOnlyList<IDomainObject> domainObjects, ICommittingEventRegistrar eventRegistrar)
    {
      EnsureWriteable(clientTransaction, "TransactionCommitting");
    }

    public virtual void TransactionCommitValidate (ClientTransaction clientTransaction, IReadOnlyList<PersistableData> committedData)
    {
      // Handled by Begin event
    }

    public virtual void TransactionCommitted (ClientTransaction clientTransaction, IReadOnlyList<IDomainObject> domainObjects)
    {
      // Handled by Begin event
    }

    public virtual void TransactionRollingBack (ClientTransaction clientTransaction, IReadOnlyList<IDomainObject> domainObjects)
    {
      EnsureWriteable(clientTransaction, "TransactionRollingBack");
    }

    public virtual void TransactionRolledBack (ClientTransaction clientTransaction, IReadOnlyList<IDomainObject> domainObjects)
    {
      // Handled by Begin event
    }

    public virtual void RelationEndPointMapRegistering (ClientTransaction clientTransaction, IRelationEndPoint endPoint)
    {
      // Safe assuming the subtransaction does not have a complete end-point for the same ID (subtransaction needs to be loaded later)
      // (or when it has been unlocked - during subtx.Commit)
      Assertion.IsTrue(
          clientTransaction.IsWriteable
          || clientTransaction.SubTransaction == null
          || IsNullOrIncomplete(clientTransaction.SubTransaction.DataManager.RelationEndPoints[endPoint.ID]));
    }

    public virtual void RelationEndPointMapUnregistering (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      // Safe assuming the subtransaction does not have a complete end-point for the same ID (subtransaction needs to be unloaded first)
      // (or when it has been unlocked - during subtx.Commit)
      Assertion.IsTrue(
          clientTransaction.IsWriteable
          || clientTransaction.SubTransaction == null
          || IsNullOrIncomplete(clientTransaction.SubTransaction.DataManager.RelationEndPoints[endPointID]));
    }

    public virtual void RelationEndPointBecomingIncomplete (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      // Safe assuming the subtransaction does not have a complete end-point for the same ID (subtransaction needs to be unloaded first)
      Assertion.IsTrue(
          clientTransaction.SubTransaction == null
          || IsNullOrIncomplete(clientTransaction.SubTransaction.DataManager.RelationEndPoints[endPointID]));
    }

    public virtual void ObjectMarkedInvalid (ClientTransaction clientTransaction, IDomainObject domainObject)
    {
      EnsureWriteable(clientTransaction, "ObjectMarkedInvalid");
    }

    public virtual void ObjectMarkedNotInvalid (ClientTransaction clientTransaction, IDomainObject domainObject)
    {
      EnsureWriteable(clientTransaction, "ObjectMarkedNotInvalid");
    }

    public virtual void DataContainerMapRegistering (ClientTransaction clientTransaction, DataContainer container)
    {
      // Safe assuming the subtransaction cannot already have a DataContainer for the same object (subtransaction needs to be loaded later)
      // (or when it has been unlocked - during subtx.Commit)
      Assertion.IsTrue(
          clientTransaction.IsWriteable
          || clientTransaction.SubTransaction == null
          || clientTransaction.SubTransaction.DataManager.DataContainers[container.ID] == null);
    }

    public virtual void DataContainerMapUnregistering (ClientTransaction clientTransaction, DataContainer container)
    {
      // Safe assuming the subtransaction does not have a DataContainer for the same object (subtransaction needs to be unloaded first)
      // (or when it has been unlocked - during subtx.Commit)
      Assertion.IsTrue(
          clientTransaction.IsWriteable
          || clientTransaction.SubTransaction == null
          || clientTransaction.SubTransaction.DataManager.DataContainers[container.ID] == null);
    }

    public virtual void DataContainerStateUpdated (ClientTransaction clientTransaction, DataContainer container, DataContainerState newDataContainerState)
    {
      EnsureWriteable(clientTransaction, "DataContainerStateUpdated");
    }

    public virtual void VirtualRelationEndPointStateUpdated (ClientTransaction clientTransaction, RelationEndPointID endPointID, bool? newEndPointChangeState)
    {
      EnsureWriteable(clientTransaction, "VirtualRelationEndPointStateUpdated");
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }

    private void EnsureWriteable (ClientTransaction clientTransaction, string operation)
    {
      if (!clientTransaction.IsWriteable)
      {
        string message = string.Format(
            "The operation cannot be executed because the ClientTransaction is read-only, probably because it has an open subtransaction. "
            + "Offending transaction modification: {0}.",
            operation);
        throw new ClientTransactionReadOnlyException(message);
      }
    }

    private bool IsNullOrIncomplete (IRelationEndPoint? relationEndPoint)
    {
      return relationEndPoint == null || !relationEndPoint.IsDataComplete;
    }
  }
}
