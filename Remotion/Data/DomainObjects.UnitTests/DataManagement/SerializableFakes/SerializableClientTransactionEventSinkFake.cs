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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes
{
  [Serializable]
  public class SerializableClientTransactionEventSinkFake : IClientTransactionEventSink
  {
    public void RaiseRelationChangingEvent (
        DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {

    }

    public void RaiseRelationChangedEvent (
        DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {

    }

    public void RaiseObjectDeletingEvent (DomainObject domainObject)
    {

    }

    public void RaiseObjectDeletedEvent (DomainObject domainObject)
    {

    }

    public void RaiseObjectsUnloadingEvent (IReadOnlyList<DomainObject> unloadedDomainObjects)
    {

    }

    public void RaiseObjectsUnloadedEvent (IReadOnlyList<DomainObject> unloadedDomainObjects)
    {

    }

    public void RaiseRelationEndPointBecomingIncompleteEvent (RelationEndPointID endPointID)
    {

    }

    public void RaiseRelationEndPointMapRegisteringEvent (IRelationEndPoint endPoint)
    {

    }

    public void RaiseRelationEndPointMapUnregisteringEvent (RelationEndPointID endPointID)
    {

    }

    public void RaiseVirtualRelationEndPointStateUpdatedEvent (RelationEndPointID endPointID, bool? newEndPointChangeState)
    {

    }

    public void RaisePropertyValueReadingEvent (DomainObject domainObject, PropertyDefinition propertyDefinition, ValueAccess valueAccess)
    {

    }

    public void RaisePropertyValueReadEvent (DomainObject domainObject, PropertyDefinition propertyDefinition, object value, ValueAccess valueAccess)
    {

    }

    public void RaisePropertyValueChangingEvent (DomainObject domainObject, PropertyDefinition propertyDefinition, object oldValue, object newValue)
    {

    }

    public void RaisePropertyValueChangedEvent (DomainObject domainObject, PropertyDefinition propertyDefinition, object oldValue, object newValue)
    {

    }

    public void RaiseDataContainerStateUpdatedEvent (DataContainer container, DataContainerState newDataContainerState)
    {

    }

    public void RaiseDataContainerMapRegisteringEvent (DataContainer container)
    {

    }

    public void RaiseDataContainerMapUnregisteringEvent (DataContainer container)
    {

    }

    public void RaiseSubTransactionCreatingEvent ()
    {

    }

    public void RaiseSubTransactionInitializeEvent (ClientTransaction subTransaction)
    {

    }

    public void RaiseSubTransactionCreatedEvent (ClientTransaction subTransaction)
    {

    }

    public void RaiseObjectMarkedInvalidEvent (DomainObject domainObject)
    {

    }

    public void RaiseObjectMarkedNotInvalidEvent (DomainObject domainObject)
    {

    }

    public void RaiseNewObjectCreatingEvent (Type type)
    {

    }

    public void RaiseObjectsLoadingEvent (IReadOnlyList<ObjectID> objectIDs)
    {

    }

    public void RaiseObjectsLoadedEvent (IReadOnlyList<DomainObject> domainObjects)
    {

    }

    public void RaiseObjectsNotFoundEvent (IReadOnlyList<ObjectID> objectIDs)
    {

    }

    public void RaiseTransactionCommittingEvent (IReadOnlyList<DomainObject> domainObjects, ICommittingEventRegistrar eventRegistrar)
    {

    }

    public void RaiseTransactionCommitValidateEvent (IReadOnlyList<PersistableData> committedData)
    {

    }

    public void RaiseTransactionCommittedEvent (IReadOnlyList<DomainObject> domainObjects)
    {

    }

    public void RaiseTransactionRollingBackEvent (IReadOnlyList<DomainObject> domainObjects)
    {

    }

    public void RaiseTransactionRolledBackEvent (IReadOnlyList<DomainObject> domainObjects)
    {

    }

    public QueryResult<T> RaiseFilterQueryResultEvent<T> (QueryResult<T> queryResult) where T : DomainObject
    {
      return null;
    }

    public IEnumerable<T> RaiseFilterCustomQueryResultEvent<T> (IQuery query, IEnumerable<T> results)
    {
      return null;
    }

    public void RaiseTransactionInitializeEvent ()
    {

    }

    public void RaiseTransactionDiscardEvent ()
    {

    }

    public void RaiseRelationReadingEvent (DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, ValueAccess valueAccess)
    {
    }


    public void RaiseRelationReadEvent (
        DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, DomainObject relatedObject, ValueAccess valueAccess)
    {

    }

    public void RaiseRelationReadEvent (
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        IReadOnlyCollectionData<IDomainObject> relatedObjects,
        ValueAccess valueAccess)
    {

    }
  }
}
