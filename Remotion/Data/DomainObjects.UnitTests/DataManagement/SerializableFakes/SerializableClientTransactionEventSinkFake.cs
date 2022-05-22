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
        IDomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        IDomainObject oldRelatedObject,
        IDomainObject newRelatedObject)
    {

    }

    public void RaiseRelationChangedEvent (
        IDomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        IDomainObject oldRelatedObject,
        IDomainObject newRelatedObject)
    {

    }

    public void RaiseObjectDeletingEvent (IDomainObject domainObject)
    {

    }

    public void RaiseObjectDeletedEvent (IDomainObject domainObject)
    {

    }

    public void RaiseObjectsUnloadingEvent (IReadOnlyList<IDomainObject> unloadedDomainObjects)
    {

    }

    public void RaiseObjectsUnloadedEvent (IReadOnlyList<IDomainObject> unloadedDomainObjects)
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

    public void RaisePropertyValueReadingEvent (IDomainObject domainObject, PropertyDefinition propertyDefinition, ValueAccess valueAccess)
    {

    }

    public void RaisePropertyValueReadEvent (IDomainObject domainObject, PropertyDefinition propertyDefinition, object value, ValueAccess valueAccess)
    {

    }

    public void RaisePropertyValueChangingEvent (IDomainObject domainObject, PropertyDefinition propertyDefinition, object oldValue, object newValue)
    {

    }

    public void RaisePropertyValueChangedEvent (IDomainObject domainObject, PropertyDefinition propertyDefinition, object oldValue, object newValue)
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

    public void RaiseObjectMarkedInvalidEvent (IDomainObject domainObject)
    {

    }

    public void RaiseObjectMarkedNotInvalidEvent (IDomainObject domainObject)
    {

    }

    public void RaiseNewObjectCreatingEvent (Type type)
    {

    }

    public void RaiseObjectsLoadingEvent (IReadOnlyList<ObjectID> objectIDs)
    {

    }

    public void RaiseObjectsLoadedEvent (IReadOnlyList<IDomainObject> domainObjects)
    {

    }

    public void RaiseObjectsNotFoundEvent (IReadOnlyList<ObjectID> objectIDs)
    {

    }

    public void RaiseTransactionCommittingEvent (IReadOnlyList<IDomainObject> domainObjects, ICommittingEventRegistrar eventRegistrar)
    {

    }

    public void RaiseTransactionCommitValidateEvent (IReadOnlyList<PersistableData> committedData)
    {

    }

    public void RaiseTransactionCommittedEvent (IReadOnlyList<IDomainObject> domainObjects)
    {

    }

    public void RaiseTransactionRollingBackEvent (IReadOnlyList<IDomainObject> domainObjects)
    {

    }

    public void RaiseTransactionRolledBackEvent (IReadOnlyList<IDomainObject> domainObjects)
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

    public void RaiseRelationReadingEvent (IDomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, ValueAccess valueAccess)
    {
    }


    public void RaiseRelationReadEvent (IDomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, IDomainObject relatedObject, ValueAccess valueAccess)
    {

    }

    public void RaiseRelationReadEvent (
        IDomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        IReadOnlyCollectionData<IDomainObject> relatedObjects,
        ValueAccess valueAccess)
    {

    }
  }
}
