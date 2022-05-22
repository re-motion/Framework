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
  /// Manages the <see cref="IClientTransactionListener"/> and <see cref="IClientTransactionExtension"/> instances attached to a 
  /// <see cref="DomainObjects.ClientTransaction"/> and allows clients to raise events for the <see cref="ClientTransaction"/>. 
  /// The event notifications are forwarded to <see cref="IClientTransactionListener"/>, <see cref="IClientTransactionExtension"/>, 
  /// <see cref="ClientTransaction"/>, and <see cref="DomainObject"/> instances.
  /// </summary>
  [Serializable]
  public class ClientTransactionEventBroker : IClientTransactionEventBroker // todo R2I move event handling to IDomainObject?
  {
    private readonly ClientTransaction _clientTransaction;

    private readonly ClientTransactionExtensionCollection _extensionCollection;
    private readonly CompoundClientTransactionListener _listenerCollection;

    public ClientTransactionEventBroker (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);

      _clientTransaction = clientTransaction;
      _extensionCollection = new ClientTransactionExtensionCollection("root");
      _listenerCollection = new CompoundClientTransactionListener();
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public IEnumerable<IClientTransactionListener> Listeners
    {
      get { return _listenerCollection.Listeners; }
    }

    public ClientTransactionExtensionCollection Extensions
    {
      get { return _extensionCollection; }
    }

    public void AddListener (IClientTransactionListener listener)
    {
      ArgumentUtility.CheckNotNull("listener", listener);
      _listenerCollection.AddListener(listener);
    }

    public void RemoveListener (IClientTransactionListener listener)
    {
      ArgumentUtility.CheckNotNull("listener", listener);
      _listenerCollection.RemoveListener(listener);
    }

    public void RaiseTransactionInitializeEvent ()
    {
      _listenerCollection.TransactionInitialize(_clientTransaction);
      _extensionCollection.TransactionInitialize(_clientTransaction);
    }

    public void RaiseTransactionDiscardEvent ()
    {
      _listenerCollection.TransactionDiscard(_clientTransaction);
      _extensionCollection.TransactionDiscard(_clientTransaction);
    }

    public void RaiseSubTransactionCreatingEvent ()
    {
      _listenerCollection.SubTransactionCreating(_clientTransaction);
      _extensionCollection.SubTransactionCreating(_clientTransaction);
    }

    public void RaiseSubTransactionInitializeEvent (ClientTransaction subTransaction)
    {
      ArgumentUtility.CheckNotNull("subTransaction", subTransaction);

      _listenerCollection.SubTransactionInitialize(_clientTransaction, subTransaction);
      _extensionCollection.SubTransactionInitialize(_clientTransaction, subTransaction);
    }

    public void RaiseSubTransactionCreatedEvent (ClientTransaction subTransaction)
    {
      ArgumentUtility.CheckNotNull("subTransaction", subTransaction);

      using (EnterScopeOnDemand())
      {
        _clientTransaction.OnSubTransactionCreated(new SubTransactionCreatedEventArgs(subTransaction));
      }

      _extensionCollection.SubTransactionCreated(_clientTransaction, subTransaction);
      _listenerCollection.SubTransactionCreated(_clientTransaction, subTransaction);
    }

    public void RaiseNewObjectCreatingEvent (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      _listenerCollection.NewObjectCreating(_clientTransaction, type);
      _extensionCollection.NewObjectCreating(_clientTransaction, type);
    }

    public void RaiseObjectsLoadingEvent (IReadOnlyList<ObjectID> objectIDs)
    {
      ArgumentUtility.CheckNotNull("objectIDs", objectIDs);

      _listenerCollection.ObjectsLoading(_clientTransaction, objectIDs);
      _extensionCollection.ObjectsLoading(_clientTransaction, objectIDs);
    }

    public void RaiseObjectsLoadedEvent (IReadOnlyList<IDomainObject> domainObjects)
    {
      ArgumentUtility.CheckNotNull("domainObjects", domainObjects);

      using (EnterScopeOnDemand())
      {
        foreach (var domainObject in domainObjects.OfType<DomainObject>()) // todo R2I correct?
          domainObject.OnLoaded();

        _clientTransaction.OnLoaded(new ClientTransactionEventArgs(domainObjects));
      }

      _extensionCollection.ObjectsLoaded(_clientTransaction, domainObjects);
      _listenerCollection.ObjectsLoaded(_clientTransaction, domainObjects);
    }

    public void RaiseObjectsNotFoundEvent (IReadOnlyList<ObjectID> objectIDs)
    {
      _listenerCollection.ObjectsNotFound(_clientTransaction, objectIDs);
    }

    public void RaiseObjectsUnloadingEvent (IReadOnlyList<IDomainObject> unloadedDomainObjects)
    {
      ArgumentUtility.CheckNotNull("unloadedDomainObjects", unloadedDomainObjects);

      _listenerCollection.ObjectsUnloading(_clientTransaction, unloadedDomainObjects);
      _extensionCollection.ObjectsUnloading(_clientTransaction, unloadedDomainObjects);
      using (EnterScopeOnDemand())
      {
        // This is a for loop for symmetry with ObjectsUnloaded
        // ReSharper disable ForCanBeConvertedToForeach
        for (int i = 0; i < unloadedDomainObjects.Count; i++)
        // ReSharper restore ForCanBeConvertedToForeach
        {
          var domainObject = unloadedDomainObjects[i];
          if (domainObject is DomainObject concreteDomainObject)
            concreteDomainObject.OnUnloading();
        }
      }
    }

    public void RaiseObjectsUnloadedEvent (IReadOnlyList<IDomainObject> unloadedDomainObjects)
    {
      ArgumentUtility.CheckNotNull("unloadedDomainObjects", unloadedDomainObjects);

      using (EnterScopeOnDemand())
      {
        for (int i = unloadedDomainObjects.Count - 1; i >= 0; i--)
        {
          var domainObject = unloadedDomainObjects[i];
          if (domainObject is DomainObject concreteDomainObject)
            concreteDomainObject.OnUnloaded();
        }
      }
      _extensionCollection.ObjectsUnloaded(_clientTransaction, unloadedDomainObjects);
      _listenerCollection.ObjectsUnloaded(_clientTransaction, unloadedDomainObjects);
    }

    public void RaiseObjectDeletingEvent (IDomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      _listenerCollection.ObjectDeleting(_clientTransaction, domainObject);
      _extensionCollection.ObjectDeleting(_clientTransaction, domainObject);
      using (EnterScopeOnDemand())
      {
        if (domainObject is DomainObject concreteDomainObject)
          concreteDomainObject.OnDeleting(EventArgs.Empty);
      }
    }

    public void RaiseObjectDeletedEvent (IDomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      using (EnterScopeOnDemand())
      {
        if (domainObject is DomainObject concreteDomainObject)
          concreteDomainObject.OnDeleted(EventArgs.Empty);
      }
      _extensionCollection.ObjectDeleted(_clientTransaction, domainObject);
      _listenerCollection.ObjectDeleted(_clientTransaction, domainObject);
    }

    public void RaisePropertyValueReadingEvent (IDomainObject domainObject, PropertyDefinition propertyDefinition, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      ArgumentUtility.CheckNotNull("propertyDefinition", propertyDefinition);

      _listenerCollection.PropertyValueReading(_clientTransaction, domainObject, propertyDefinition, valueAccess);
      _extensionCollection.PropertyValueReading(_clientTransaction, domainObject, propertyDefinition, valueAccess);
    }

    public void RaisePropertyValueReadEvent (IDomainObject domainObject, PropertyDefinition propertyDefinition, object? value, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      ArgumentUtility.CheckNotNull("propertyDefinition", propertyDefinition);

      _extensionCollection.PropertyValueRead(_clientTransaction, domainObject, propertyDefinition, value, valueAccess);
      _listenerCollection.PropertyValueRead(_clientTransaction, domainObject, propertyDefinition, value, valueAccess);
    }

    public void RaisePropertyValueChangingEvent (IDomainObject domainObject, PropertyDefinition propertyDefinition, object? oldValue, object? newValue)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      ArgumentUtility.CheckNotNull("propertyDefinition", propertyDefinition);

      _listenerCollection.PropertyValueChanging(_clientTransaction, domainObject, propertyDefinition, oldValue, newValue);
      _extensionCollection.PropertyValueChanging(_clientTransaction, domainObject, propertyDefinition, oldValue, newValue);
      using (EnterScopeOnDemand())
      {
        if (domainObject is DomainObject concreteDomainObject)
          concreteDomainObject.OnPropertyChanging(new PropertyChangeEventArgs(propertyDefinition, oldValue, newValue));
      }
    }

    public void RaisePropertyValueChangedEvent (IDomainObject domainObject, PropertyDefinition propertyDefinition, object? oldValue, object? newValue)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      ArgumentUtility.CheckNotNull("propertyDefinition", propertyDefinition);

      using (EnterScopeOnDemand())
      {
        if (domainObject is DomainObject concreteDomainObject)
          concreteDomainObject.OnPropertyChanged(new PropertyChangeEventArgs(propertyDefinition, oldValue, newValue));
      }

      _extensionCollection.PropertyValueChanged(_clientTransaction, domainObject, propertyDefinition, oldValue, newValue);
      _listenerCollection.PropertyValueChanged(_clientTransaction, domainObject, propertyDefinition, oldValue, newValue);
    }

    public void RaiseRelationReadingEvent (IDomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      ArgumentUtility.CheckNotNull("relationEndPointDefinition", relationEndPointDefinition);

      _listenerCollection.RelationReading(_clientTransaction, domainObject, relationEndPointDefinition, valueAccess);
      _extensionCollection.RelationReading(_clientTransaction, domainObject, relationEndPointDefinition, valueAccess);
    }

    public void RaiseRelationReadEvent (IDomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, IDomainObject? relatedObject, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      ArgumentUtility.CheckNotNull("relationEndPointDefinition", relationEndPointDefinition);

      _extensionCollection.RelationRead(_clientTransaction, domainObject, relationEndPointDefinition, relatedObject, valueAccess);
      _listenerCollection.RelationRead(_clientTransaction, domainObject, relationEndPointDefinition, relatedObject, valueAccess);
    }

    public void RaiseRelationReadEvent (
        IDomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        IReadOnlyCollectionData<IDomainObject> relatedObjects,
        ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      ArgumentUtility.CheckNotNull("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNull("relatedObjects", relatedObjects);

      _extensionCollection.RelationRead(_clientTransaction, domainObject, relationEndPointDefinition, relatedObjects, valueAccess);
      _listenerCollection.RelationRead(_clientTransaction, domainObject, relationEndPointDefinition, relatedObjects, valueAccess);
    }

    public void RaiseRelationChangingEvent (
        IDomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        IDomainObject? oldRelatedObject,
        IDomainObject? newRelatedObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      ArgumentUtility.CheckNotNull("relationEndPointDefinition", relationEndPointDefinition);

      _listenerCollection.RelationChanging(_clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);
      _extensionCollection.RelationChanging(_clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);
      using (EnterScopeOnDemand())
      {
        if (domainObject is DomainObject concreteDomainObject)
          concreteDomainObject.OnRelationChanging(new RelationChangingEventArgs(relationEndPointDefinition, oldRelatedObject, newRelatedObject));
      }
    }

    public void RaiseRelationChangedEvent (
        IDomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        IDomainObject? oldRelatedObject,
        IDomainObject? newRelatedObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      ArgumentUtility.CheckNotNull("relationEndPointDefinition", relationEndPointDefinition);

      using (EnterScopeOnDemand())
      {
        if (domainObject is DomainObject concreteDomainObject)
          concreteDomainObject.OnRelationChanged(new RelationChangedEventArgs(relationEndPointDefinition, oldRelatedObject, newRelatedObject));
      }
      _extensionCollection.RelationChanged(_clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);
      _listenerCollection.RelationChanged(_clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);
    }

    public QueryResult<T> RaiseFilterQueryResultEvent<T> (QueryResult<T> queryResult) where T : DomainObject
    {
      ArgumentUtility.CheckNotNull("queryResult", queryResult);

      queryResult = _listenerCollection.FilterQueryResult(_clientTransaction, queryResult);
      queryResult = _extensionCollection.FilterQueryResult(_clientTransaction, queryResult);
      return queryResult;
    }

    public IEnumerable<T> RaiseFilterCustomQueryResultEvent<T> (IQuery query, IEnumerable<T> results)
    {
      return _listenerCollection.FilterCustomQueryResult(_clientTransaction, query, results);
    }

    public void RaiseTransactionCommittingEvent (IReadOnlyList<IDomainObject> domainObjects, ICommittingEventRegistrar eventRegistrar)
    {
      ArgumentUtility.CheckNotNull("domainObjects", domainObjects);
      ArgumentUtility.CheckNotNull("eventRegistrar", eventRegistrar);

      _listenerCollection.TransactionCommitting(_clientTransaction, domainObjects, eventRegistrar);
      _extensionCollection.Committing(_clientTransaction, domainObjects, eventRegistrar);
      using (EnterScopeOnDemand())
      {
        _clientTransaction.OnCommitting(new ClientTransactionCommittingEventArgs(domainObjects, eventRegistrar));
        // ReSharper disable ForCanBeConvertedToForeach
        for (int i = 0; i < domainObjects.Count; i++)
        {
          var domainObject = domainObjects[i];
          if (domainObject is DomainObject concreteDomainObject)
          {
            if (!concreteDomainObject.State.IsInvalid)
              concreteDomainObject.OnCommitting(new DomainObjectCommittingEventArgs(eventRegistrar));
          }
        }
        // ReSharper restore ForCanBeConvertedToForeach
      }
    }

    public void RaiseTransactionCommitValidateEvent (IReadOnlyList<PersistableData> committedData)
    {
      ArgumentUtility.CheckNotNull("committedData", committedData);

      _listenerCollection.TransactionCommitValidate(_clientTransaction, committedData);
      _extensionCollection.CommitValidate(_clientTransaction, committedData);
    }

    public void RaiseTransactionCommittedEvent (IReadOnlyList<IDomainObject> domainObjects)
    {
      ArgumentUtility.CheckNotNull("domainObjects", domainObjects);

      using (EnterScopeOnDemand())
      {
        for (int i = domainObjects.Count - 1; i >= 0; i--)
        {
          if (domainObjects[i] is DomainObject concreteDomainObject)
            concreteDomainObject.OnCommitted(EventArgs.Empty);
        }
        _clientTransaction.OnCommitted(new ClientTransactionEventArgs(domainObjects));
      }

      _extensionCollection.Committed(_clientTransaction, domainObjects);
      _listenerCollection.TransactionCommitted(_clientTransaction, domainObjects);
    }

    public void RaiseTransactionRollingBackEvent (IReadOnlyList<IDomainObject> domainObjects)
    {
      ArgumentUtility.CheckNotNull("domainObjects", domainObjects);

      _listenerCollection.TransactionRollingBack(_clientTransaction, domainObjects);
      _extensionCollection.RollingBack(_clientTransaction, domainObjects);

      using (EnterScopeOnDemand())
      {
        _clientTransaction.OnRollingBack(new ClientTransactionEventArgs(domainObjects));
        // ReSharper disable ForCanBeConvertedToForeach
        for (int i = 0; i < domainObjects.Count; i++)
        {
          var domainObject = domainObjects[i];
          if (domainObject is DomainObject concreteDomainObject)
          {
            if (!concreteDomainObject.State.IsInvalid)
              concreteDomainObject.OnRollingBack(EventArgs.Empty);
          }
        }
        // ReSharper restore ForCanBeConvertedToForeach
      }
    }

    public void RaiseTransactionRolledBackEvent (IReadOnlyList<IDomainObject> domainObjects)
    {
      ArgumentUtility.CheckNotNull("domainObjects", domainObjects);

      using (EnterScopeOnDemand())
      {
        for (int i = domainObjects.Count - 1; i >= 0; i--)
        {
          if (domainObjects[i] is DomainObject concreteDomainObject)
            concreteDomainObject.OnRolledBack(EventArgs.Empty);
        }
        _clientTransaction.OnRolledBack(new ClientTransactionEventArgs(domainObjects));
      }

      _extensionCollection.RolledBack(_clientTransaction, domainObjects);
      _listenerCollection.TransactionRolledBack(_clientTransaction, domainObjects);
    }

    public void RaiseRelationEndPointMapRegisteringEvent (IRelationEndPoint endPoint)
    {
      _listenerCollection.RelationEndPointMapRegistering(_clientTransaction, endPoint);
    }

    public void RaiseRelationEndPointMapUnregisteringEvent (RelationEndPointID endPointID)
    {
      _listenerCollection.RelationEndPointMapUnregistering(_clientTransaction, endPointID);
    }

    public void RaiseRelationEndPointBecomingIncompleteEvent (RelationEndPointID endPointID)
    {
      _listenerCollection.RelationEndPointBecomingIncomplete(_clientTransaction, endPointID);
    }

    public void RaiseObjectMarkedInvalidEvent (IDomainObject domainObject)
    {
      _listenerCollection.ObjectMarkedInvalid(_clientTransaction, domainObject);
    }

    public void RaiseObjectMarkedNotInvalidEvent (IDomainObject domainObject)
    {
      _listenerCollection.ObjectMarkedNotInvalid(_clientTransaction, domainObject);
    }

    public void RaiseDataContainerMapRegisteringEvent (DataContainer container)
    {
      _listenerCollection.DataContainerMapRegistering(_clientTransaction, container);
    }

    public void RaiseDataContainerMapUnregisteringEvent (DataContainer container)
    {
      _listenerCollection.DataContainerMapUnregistering(_clientTransaction, container);
    }

    public void RaiseDataContainerStateUpdatedEvent (DataContainer container, DataContainerState newDataContainerState)
    {
      _listenerCollection.DataContainerStateUpdated(_clientTransaction, container, newDataContainerState);
    }

    public void RaiseVirtualRelationEndPointStateUpdatedEvent (RelationEndPointID endPointID, bool? newEndPointChangeState)
    {
      _listenerCollection.VirtualRelationEndPointStateUpdated(_clientTransaction, endPointID, newEndPointChangeState);
    }

    private ClientTransactionScope? EnterScopeOnDemand ()
    {
      //if (_clientTransaction.ActiveTransaction != _clientTransaction)
      //  return _clientTransaction.EnterNonDiscardingScope();

      if (ClientTransaction.Current != _clientTransaction)
        return _clientTransaction.EnterNonDiscardingScope();

      return null;
    }
  }
}
