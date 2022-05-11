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
using System.Collections.ObjectModel;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Defines an interface allowing clients to raise events for the associated <see cref="ClientTransaction"/>.
  /// </summary>
  public interface IClientTransactionEventSink
  {
    // User event
    void RaiseTransactionInitializeEvent ();
    // User event
    void RaiseTransactionDiscardEvent ();

    // User event
    void RaiseSubTransactionCreatingEvent ();
    // User event
    void RaiseSubTransactionInitializeEvent (ClientTransaction subTransaction);
    // User event
    void RaiseSubTransactionCreatedEvent (ClientTransaction subTransaction);

    // User event
    void RaiseNewObjectCreatingEvent (Type type);

    // User event
    void RaiseObjectsLoadingEvent (IReadOnlyList<ObjectID> objectIDs);
    // User event
    void RaiseObjectsLoadedEvent (IReadOnlyList<DomainObject> domainObjects);
    // Infrastructure event
    void RaiseObjectsNotFoundEvent (IReadOnlyList<ObjectID> objectIDs);

    // User event
    void RaiseObjectsUnloadingEvent (IReadOnlyList<DomainObject> unloadedDomainObjects);
    // User event
    void RaiseObjectsUnloadedEvent (IReadOnlyList<DomainObject> unloadedDomainObjects);

    // User event
    void RaiseObjectDeletingEvent (DomainObject domainObject);
    // User event
    void RaiseObjectDeletedEvent (DomainObject domainObject);

    // User event
    void RaisePropertyValueReadingEvent (DomainObject domainObject, PropertyDefinition propertyDefinition, ValueAccess valueAccess);
    // User event
    void RaisePropertyValueReadEvent (DomainObject domainObject, PropertyDefinition propertyDefinition, object? value, ValueAccess valueAccess);
    // User event
    void RaisePropertyValueChangingEvent (DomainObject domainObject, PropertyDefinition propertyDefinition, object? oldValue, object? newValue);
    // User event
    void RaisePropertyValueChangedEvent (DomainObject domainObject, PropertyDefinition propertyDefinition, object? oldValue, object? newValue);

    // User event
    void RaiseRelationReadingEvent (
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        ValueAccess valueAccess);
    // User event
    void RaiseRelationReadEvent (
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject? relatedObject,
        ValueAccess valueAccess);
    // User event
    void RaiseRelationReadEvent (
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        IReadOnlyCollectionData<IDomainObject> relatedObjects,
        ValueAccess valueAccess);
    // User event
    void RaiseRelationChangingEvent (
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject? oldRelatedObject,
        DomainObject? newRelatedObject);
    // User event
    void RaiseRelationChangedEvent (
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject? oldRelatedObject,
        DomainObject? newRelatedObject);

    // User event
    QueryResult<T> RaiseFilterQueryResultEvent<T> (QueryResult<T> queryResult) where T : DomainObject;
    // Infrastructure event, may become user event if needed
    IEnumerable<T> RaiseFilterCustomQueryResultEvent<T> (IQuery query, IEnumerable<T> results);

    // User event
    void RaiseTransactionCommittingEvent (
        IReadOnlyList<DomainObject> domainObjects, ICommittingEventRegistrar eventRegistrar);
    // User event
    void RaiseTransactionCommitValidateEvent (IReadOnlyList<PersistableData> committedData);
    // User event
    void RaiseTransactionCommittedEvent (IReadOnlyList<DomainObject> domainObjects);

    // User event
    void RaiseTransactionRollingBackEvent (IReadOnlyList<DomainObject> domainObjects);
    // User event
    void RaiseTransactionRolledBackEvent (IReadOnlyList<DomainObject> domainObjects);

    // Infrastructure event
    void RaiseRelationEndPointMapRegisteringEvent (IRelationEndPoint endPoint);
    // Infrastructure event
    void RaiseRelationEndPointMapUnregisteringEvent (RelationEndPointID endPointID);
    // Infrastructure event
    void RaiseRelationEndPointBecomingIncompleteEvent (RelationEndPointID endPointID);

    // Infrastructure event
    void RaiseObjectMarkedInvalidEvent (DomainObject domainObject);
    // Infrastructure event
    void RaiseObjectMarkedNotInvalidEvent (DomainObject domainObject);

    // Infrastructure event
    void RaiseDataContainerMapRegisteringEvent (DataContainer container);
    // Infrastructure event
    void RaiseDataContainerMapUnregisteringEvent (DataContainer container);

    // Infrastructure event
    void RaiseDataContainerStateUpdatedEvent (DataContainer container, DataContainerState newDataContainerState);
    // Infrastructure event
    void RaiseVirtualRelationEndPointStateUpdatedEvent (RelationEndPointID endPointID, bool? newEndPointChangeState);
  }
}
