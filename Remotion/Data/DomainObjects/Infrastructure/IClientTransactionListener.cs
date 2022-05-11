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
  /// <summary>
  /// Defines an interface for objects listening for events occuring in the scope of a ClientTransaction.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This is similar to <see cref="IClientTransactionExtension"/>, but where <see cref="IClientTransactionExtension"/> is for the public,
  /// <see cref="IClientTransactionListener"/> is for internal usage (and therefore provides more events).
  /// </para>
  /// <para>
  /// The <see cref="ClientTransaction.Current"/> property is not guaranteed to be set to the affected <see cref="ClientTransaction"/> when 
  /// a notification method is executed. Implementations that require access to the calling transaction must have the transaction passed to them via
  /// the constructors.
  /// </para>
  /// </remarks>
  public interface IClientTransactionListener : INullObject
  {
    void TransactionInitialize (ClientTransaction clientTransaction);
    void TransactionDiscard (ClientTransaction clientTransaction);

    void SubTransactionCreating (ClientTransaction clientTransaction);
    void SubTransactionInitialize (ClientTransaction clientTransaction, ClientTransaction subTransaction);
    void SubTransactionCreated (ClientTransaction clientTransaction, ClientTransaction subTransaction);

    /// <summary>
    /// Indicates a new <see cref="DomainObject"/> instance is being created. This event is called while the <see cref="DomainObject"/> base 
    /// constructor is executing before the subclass constructors have run and before the object has got its <see cref="ObjectID"/> or 
    /// <see cref="DataContainer"/>. If this method throws an exception, the object construction will be canceled and no side effects will remain.
    /// </summary>
    void NewObjectCreating (ClientTransaction clientTransaction, Type type);

    void ObjectsLoading (ClientTransaction clientTransaction, IReadOnlyList<ObjectID> objectIDs);
    void ObjectsLoaded (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> domainObjects);
    void ObjectsNotFound (ClientTransaction clientTransaction, IReadOnlyList<ObjectID> objectIDs);

    void ObjectsUnloading (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> unloadedDomainObjects);
    void ObjectsUnloaded (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> unloadedDomainObjects);

    void ObjectDeleting (ClientTransaction clientTransaction, DomainObject domainObject);
    void ObjectDeleted (ClientTransaction clientTransaction, DomainObject domainObject);

    void PropertyValueReading (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, ValueAccess valueAccess);
    void PropertyValueRead (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, object? value, ValueAccess valueAccess);
    void PropertyValueChanging (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, object? oldValue, object? newValue);
    void PropertyValueChanged (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, object? oldValue, object? newValue);

    void RelationReading (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, ValueAccess valueAccess);

    /// <summary>
    /// Indicates that a scalar-value relation has been read.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/> providing the scope to the operation.</param>
    /// <param name="domainObject">The domain object owning the relation that has been read.</param>
    /// <param name="relationEndPointDefinition">The relation endpoint definition of the relation that has been read.</param>
    /// <param name="relatedObject">The related object that is returned to the reader.</param>
    /// <param name="valueAccess">An indicator whether the current or original values have been read.</param>
    void RelationRead (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject? relatedObject,
        ValueAccess valueAccess);

    /// <summary>
    /// Indicates that a collection-value relation has been read.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/> providing the scope to the operation.</param>
    /// <param name="domainObject">The domain object owning the relation that has been read.</param>
    /// <param name="relationEndPointDefinition">The relation endpoint definition of the relation that has been read.</param>
    /// <param name="relatedObjects">
    ///   A read-only wrapper of the related object data that is returned to the reader. Implementors should check the 
    ///   <see cref="IReadOnlyCollectionData{T}.IsDataComplete"/> property before accessing the collection data in order to avoid reloading 
    ///   an unloaded collection end-point.
    /// </param>
    /// <param name="valueAccess">An indicator whether the current or original values have been read.</param>
    void RelationRead (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        IReadOnlyCollectionData<IDomainObject> relatedObjects,
        ValueAccess valueAccess);

    /// <summary>
    /// Indicates that a relation is about to change. 
    /// This method might be invoked more than once for a given relation change operation. For example, when a whole related object collection is 
    /// replaced in one go, the method is invoked once for each old object that is not in the new collection and once for each new object not in the 
    /// old collection.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/> providing the scope to the operation.</param>
    /// <param name="domainObject">The domain object holding the relation being changed.</param>
    /// <param name="relationEndPointDefinition">The relation endpoint definition of the relation that changes.</param>
    /// <param name="oldRelatedObject">The related object that is removed from the relation, or <see langword="null" /> if a new item is added without 
    ///   replacing an old one.</param>
    /// <param name="newRelatedObject">The related object that is added to the relation, or <see langword="null" /> if an old item is removed without 
    ///   being replaced by a new one.</param>
    void RelationChanging (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject? oldRelatedObject,
        DomainObject? newRelatedObject);

    /// <summary>
    /// Indicates that a relation has been changed. 
    /// This method might be invoked more than once for a given relation change operation. For example, when a whole related object collection is 
    /// replaced in one go, the method is invoked once for each old object that is not in the new collection and once for each new object not in the 
    /// old collection.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/> providing the scope to the operation.</param>
    /// <param name="domainObject">The domain object holding the relation being changed.</param>
    /// <param name="relationEndPointDefinition">The relation endpoint defintition of the relation that changes.</param>
    /// <param name="oldRelatedObject">The related object that is removed from the relation, or <see langword="null" /> if a new item is added without 
    ///   replacing an old one.</param>
    /// <param name="newRelatedObject">The related object that is added to the relation, or <see langword="null" /> if an old item is removed without 
    ///   being replaced by a new one.</param>
    void RelationChanged (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject? oldRelatedObject,
        DomainObject? newRelatedObject);

    QueryResult<T> FilterQueryResult<T> (ClientTransaction clientTransaction, QueryResult<T> queryResult) where T : DomainObject;
    IEnumerable<T> FilterCustomQueryResult<T> (ClientTransaction clientTransaction, IQuery query, IEnumerable<T> results);

    void TransactionCommitting (
        ClientTransaction clientTransaction, IReadOnlyList<DomainObject> domainObjects, ICommittingEventRegistrar eventRegistrar);
    void TransactionCommitValidate (ClientTransaction clientTransaction, IReadOnlyList<PersistableData> committedData);
    void TransactionCommitted (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> domainObjects);
    void TransactionRollingBack (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> domainObjects);
    void TransactionRolledBack (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> domainObjects);

    void RelationEndPointMapRegistering (ClientTransaction clientTransaction, IRelationEndPoint endPoint);
    void RelationEndPointMapUnregistering (ClientTransaction clientTransaction, RelationEndPointID endPointID);
    void RelationEndPointBecomingIncomplete (ClientTransaction clientTransaction, RelationEndPointID endPointID);

    void ObjectMarkedInvalid (ClientTransaction clientTransaction, DomainObject domainObject);
    void ObjectMarkedNotInvalid (ClientTransaction clientTransaction, DomainObject domainObject);

    void DataContainerMapRegistering (ClientTransaction clientTransaction, DataContainer container);
    void DataContainerMapUnregistering (ClientTransaction clientTransaction, DataContainer container);

    void DataContainerStateUpdated (ClientTransaction clientTransaction, DataContainer container, DataContainerState newDataContainerState);
    void VirtualRelationEndPointStateUpdated (ClientTransaction clientTransaction, RelationEndPointID endPointID, bool? newEndPointChangeState);

  }
}
