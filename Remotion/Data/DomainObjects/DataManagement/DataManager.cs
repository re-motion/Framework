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
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Manages the data (<see cref="DataContainer"/> instances, <see cref="IRelationEndPoint"/> instances, and invalid objects) for a 
  /// <see cref="ClientTransaction"/>.
  /// </summary>
  [Serializable]
  public class DataManager : ISerializable, IDeserializationCallback, IDataManager
  {
    private ClientTransaction _clientTransaction;
    private IClientTransactionEventSink _transactionEventSink;
    private IDataContainerEventListener _dataContainerEventListener;
    private IInvalidDomainObjectManager _invalidDomainObjectManager;
    private IObjectLoader _objectLoader;
    private IRelationEndPointManager _relationEndPointManager;

    private DataContainerMap _dataContainerMap;
    private DomainObjectStateCache _domainObjectStateCache;

    private object[]? _deserializedData; // only used for deserialization

    public DataManager (
        ClientTransaction clientTransaction,
        IClientTransactionEventSink transactionEventSink,
        IDataContainerEventListener dataContainerEventListener,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        IObjectLoader objectLoader,
        IRelationEndPointManager relationEndPointManager)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("transactionEventSink", transactionEventSink);
      ArgumentUtility.CheckNotNull("dataContainerEventListener", dataContainerEventListener);
      ArgumentUtility.CheckNotNull("invalidDomainObjectManager", invalidDomainObjectManager);
      ArgumentUtility.CheckNotNull("objectLoader", objectLoader);
      ArgumentUtility.CheckNotNull("relationEndPointManager", relationEndPointManager);

      _clientTransaction = clientTransaction;
      _transactionEventSink = transactionEventSink;
      _dataContainerEventListener = dataContainerEventListener;
      _invalidDomainObjectManager = invalidDomainObjectManager;
      _objectLoader = objectLoader;
      _relationEndPointManager = relationEndPointManager;

      _dataContainerMap = new DataContainerMap(_transactionEventSink);
      _domainObjectStateCache = new DomainObjectStateCache(clientTransaction);
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public IClientTransactionEventSink TransactionEventSink
    {
      get { return _transactionEventSink; }
    }

    public IDataContainerEventListener DataContainerEventListener
    {
      get { return _dataContainerEventListener; }
    }

    public IDataContainerMapReadOnlyView DataContainers
    {
      get { return _dataContainerMap; }
    }

    public IRelationEndPointMapReadOnlyView RelationEndPoints
    {
      get { return _relationEndPointManager.RelationEndPoints; }
    }

    public IEnumerable<PersistableData> GetLoadedDataByObjectState (Predicate<DomainObjectState> predicate)
    {
      ArgumentUtility.CheckNotNull("predicate", predicate);

      var matchingObjects = from dataContainer in DataContainers
          let domainObject = dataContainer.DomainObject
          let state = domainObject.TransactionContext[_clientTransaction].State
          where predicate(state)
          let associatedEndPointSequence =
              dataContainer.AssociatedRelationEndPointIDs.Select(GetRelationEndPointWithoutLoading).Where(ep => ep != null)
          select new PersistableData(domainObject, state, dataContainer, associatedEndPointSequence);
      return matchingObjects;
    }

    public void RegisterDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull("dataContainer", dataContainer);

      if (!dataContainer.HasDomainObject)
        throw new InvalidOperationException("The DomainObject of a DataContainer must be set before it can be registered with a transaction.");

      if (_dataContainerMap[dataContainer.ID] != null)
        throw new InvalidOperationException(string.Format("A DataContainer with ID '{0}' already exists in this transaction.", dataContainer.ID));

      dataContainer.SetClientTransaction(_clientTransaction);
      dataContainer.SetEventListener(_dataContainerEventListener);

      _dataContainerMap.Register(dataContainer);
      _relationEndPointManager.RegisterEndPointsForDataContainer(dataContainer);
    }

    public void Discard (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull("dataContainer", dataContainer);

      var unregisterEndPointsCommand = _relationEndPointManager.CreateUnregisterCommandForDataContainer(dataContainer);
      var unregisterDataContainerCommand = CreateUnregisterDataContainerCommand(dataContainer.ID);
      var compositeCommand = new CompositeCommand(unregisterEndPointsCommand, unregisterDataContainerCommand);

      try
      {
        compositeCommand.NotifyAndPerform();
      }
      catch (Exception ex)
      {
        var message = string.Format("Cannot discard data for object '{0}': {1}", dataContainer.ID, ex.Message);
        throw new InvalidOperationException(message, ex);
      }

      dataContainer.Discard();

      var domainObject = dataContainer.DomainObject;
      _invalidDomainObjectManager.MarkInvalid(domainObject);
    }

    public void MarkInvalid (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      // This uses IsEnlisted rather than a RootTransaction check because the DomainObject reference is used inside the ClientTransaction, and we
      // explicitly want to allow only objects enlisted in the transaction.
      if (!_clientTransaction.IsEnlisted(domainObject))
      {
        throw CreateClientTransactionsDifferException(
            "Cannot mark DomainObject '{0}' invalid, because it belongs to a different ClientTransaction.",
            domainObject.ID);
      }

      if (DataContainers[domainObject.ID] != null)
      {
        var message = string.Format("Cannot mark DomainObject '{0}' invalid because there is data registered for the object.", domainObject.ID);
        throw new InvalidOperationException(message);
      }

      _invalidDomainObjectManager.MarkInvalid(domainObject);
    }

    public void MarkNotInvalid (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);

      if (!_invalidDomainObjectManager.MarkNotInvalid(objectID))
      {
        var message = string.Format("Cannot clear the invalid state from object '{0}' - it wasn't marked invalid in the first place.", objectID);
        throw new InvalidOperationException(message);
      }
    }

    public void Commit ()
    {
      var deletedDataContainers = _dataContainerMap.Where(dc => dc.State.IsDeleted).ToList();

      _relationEndPointManager.CommitAllEndPoints();

      foreach (var deletedDataContainer in deletedDataContainers)
        Discard(deletedDataContainer);

      _dataContainerMap.CommitAllDataContainers();
    }

    public void Rollback ()
    {
      var newDataContainers = _dataContainerMap.Where(dc => dc.State.IsNew).ToList();

      // roll back end point state before discarding data containers because Discard checks that no dangling end points are created
      _relationEndPointManager.RollbackAllEndPoints();

      // discard new data containers before rolling back data container state - new data containers cannot be rolled back
      foreach (var newDataContainer in newDataContainers)
        Discard(newDataContainer);

      _dataContainerMap.RollbackAllDataContainers();
    }

    public DataContainer? GetDataContainerWithoutLoading (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);

      if (_invalidDomainObjectManager.IsInvalid(objectID))
        throw new ObjectInvalidException(objectID);

      return DataContainers[objectID];
    }

    public DomainObjectState GetState (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);

      return _domainObjectStateCache.GetState(objectID);
    }

    public DataContainer? GetDataContainerWithLazyLoad (ObjectID objectID, bool throwOnNotFound)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);

      // GetDataContainerWithoutLoading guards against invalid IDs.
      var dataContainer = GetDataContainerWithoutLoading(objectID);
      if (dataContainer != null)
        return dataContainer;

      _objectLoader.LoadObject(objectID, throwOnNotFound);

      // Since LoadObjects might have marked IDs as invalid, we need to use DataContainers[...] instead of GetDataContainerWithoutLoading here.
      return DataContainers[objectID];
    }

    public IEnumerable<DataContainer?> GetDataContainersWithLazyLoad (IEnumerable<ObjectID> objectIDs, bool throwOnNotFound)
    {
      ArgumentUtility.CheckNotNull("objectIDs", objectIDs);

      // GetDataContainerWithoutLoading below guards against invalid IDs.

      var objectIDsAsCollection = objectIDs.ConvertToCollection();
      // Note that the empty list check is just an "optimization": IObjectLoader works well with empty ObjectID lists, but it seems waste to go 
      // through that whole call chain even if no IDs are to be loaded.
      var idsToBeLoaded = objectIDsAsCollection.Where(id => GetDataContainerWithoutLoading(id) == null).ConvertToCollection();
      if (idsToBeLoaded.Any())
        _objectLoader.LoadObjects(idsToBeLoaded, throwOnNotFound);

      // Since LoadObjects might have marked IDs as invalid, we need to use DataContainers[...] instead of GetDataContainerWithoutLoading here.
      return objectIDsAsCollection.Select(id => DataContainers[id]);
    }

    public void LoadLazyCollectionEndPoint (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull("endPointID", endPointID);

      var collectionEndPoint = GetRelationEndPointWithoutLoading(endPointID) as ICollectionEndPoint<ICollectionEndPointData>;

      if (collectionEndPoint == null)
        throw new ArgumentException("The given ID does not identify an ICollectionEndPoint managed by this DataManager.", "endPointID");

      if (collectionEndPoint.IsDataComplete)
        throw new InvalidOperationException("The given end-point cannot be loaded, its data is already complete.");

      var loadedData = _objectLoader.GetOrLoadRelatedObjects(endPointID);
      var domainObjects = loadedData.Select(
          data =>
          {
            Assertion.IsFalse(data.IsNull, "ILoadedObjectData.ObjectID: {0}", data.ObjectID);

            var domainObjectReference = data.GetDomainObjectReference();
            Assertion.DebugIsNotNull(domainObjectReference, "data.GetDomainObjectReference() != null");

            return domainObjectReference;
          }).ToArray();
      collectionEndPoint.MarkDataComplete(domainObjects);
    }

    public void LoadLazyVirtualObjectEndPoint (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull("endPointID", endPointID);

      var virtualObjectEndPoint = GetRelationEndPointWithoutLoading(endPointID) as IVirtualObjectEndPoint;

      if (virtualObjectEndPoint == null)
        throw new ArgumentException("The given ID does not identify an IVirtualObjectEndPoint managed by this DataManager.", "endPointID");

      if (virtualObjectEndPoint.IsDataComplete)
        throw new InvalidOperationException("The given end-point cannot be loaded, its data is already complete.");

      var loadedObjectData = _objectLoader.GetOrLoadRelatedObject(endPointID);
      var domainObject = loadedObjectData.GetDomainObjectReference();

      // Since RelationEndPointManager.RegisterEndPoint contains a query optimization for 1:1 relations, it is possible that
      // loading the related object has already marked the end-point complete. In that case, we won't call it again (to avoid an exception).
      if (!virtualObjectEndPoint.IsDataComplete)
        virtualObjectEndPoint.MarkDataComplete(domainObject);
    }

    public DataContainer LoadLazyDataContainer (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);

      if (_dataContainerMap[objectID] != null)
        throw new InvalidOperationException("The given DataContainer cannot be loaded, its data is already available.");

      return GetDataContainerWithLazyLoad(objectID, throwOnNotFound: true)!;
    }

    public IRelationEndPoint GetRelationEndPointWithLazyLoad (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull("endPointID", endPointID);
      return _relationEndPointManager.GetRelationEndPointWithLazyLoad(endPointID);
    }

    public IRelationEndPoint? GetRelationEndPointWithoutLoading (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull("endPointID", endPointID);
      return _relationEndPointManager.GetRelationEndPointWithoutLoading(endPointID);
    }

    public IVirtualEndPoint GetOrCreateVirtualEndPoint (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull("endPointID", endPointID);
      return _relationEndPointManager.GetOrCreateVirtualEndPoint(endPointID);
    }

    public IDataManagementCommand CreateDeleteCommand (DomainObject deletedObject)
    {
      ArgumentUtility.CheckNotNull("deletedObject", deletedObject);

      // This uses IsEnlisted rather than a RootTransaction check because the DomainObject reference is used inside the ClientTransaction, and we
      // explicitly want to allow only objects enlisted in the transaction.
      if (!_clientTransaction.IsEnlisted(deletedObject))
      {
        throw CreateClientTransactionsDifferException(
            "Cannot delete DomainObject '{0}', because it belongs to a different ClientTransaction.",
            deletedObject.ID);
      }

      DomainObjectCheckUtility.EnsureNotInvalid(deletedObject, ClientTransaction);

      if (deletedObject.TransactionContext[_clientTransaction].State.IsDeleted)
        return new NopCommand();

      return new DeleteCommand(_clientTransaction, deletedObject, _transactionEventSink);
    }

    public IDataManagementCommand CreateUnloadCommand (params ObjectID[] objectIDs)
    {
      ArgumentUtility.CheckNotNull("objectIDs", objectIDs);

      var domainObjects = new List<DomainObject>();
      var problematicDataContainers = new List<KeyValuePair<ObjectID, DataContainerState>>();
      var commands = new List<IDataManagementCommand>();
      var dataContainerStateDiscarded = new DataContainerState.Builder().SetDiscarded().Value;

      foreach (var objectID in objectIDs)
      {
        if (_invalidDomainObjectManager.IsInvalid(objectID))
        {
          problematicDataContainers.Add(new KeyValuePair<ObjectID, DataContainerState>(objectID, dataContainerStateDiscarded));
        }
        else
        {
          var dataContainer = GetDataContainerWithoutLoading(objectID);
          if (dataContainer != null)
          {
            domainObjects.Add(dataContainer.DomainObject);

            if (dataContainer.State.IsUnchanged)
            {
              commands.Add(CreateUnregisterDataContainerCommand(objectID));
              commands.Add(_relationEndPointManager.CreateUnregisterCommandForDataContainer(dataContainer));
            }
            else
            {
              problematicDataContainers.Add(new KeyValuePair<ObjectID, DataContainerState>(dataContainer.ID, dataContainer.State));
            }
          }
        }
      }

      if (problematicDataContainers.Count != 0)
      {
        var itemList = string.Join(", ", problematicDataContainers.Select(dc => string.Format("'{0}' ({1})", dc.Key, dc.Value)));
        var message = string.Format(
            "The state of the following DataContainers prohibits that they be unloaded; only unchanged DataContainers can be unloaded: {0}.",
            itemList);
        return new ExceptionCommand(new InvalidOperationException(message));
      }

      if (domainObjects.Count == 0)
      {
        Assertion.IsTrue(commands.Count == 0);
        return new NopCommand();
      }
      else
      {
        var compositeCommand = new CompositeCommand(commands);
        return new UnloadCommand(domainObjects, compositeCommand, _transactionEventSink);
      }
    }

    public IDataManagementCommand CreateUnloadVirtualEndPointsCommand (params RelationEndPointID[] endPointIDs)
    {
      ArgumentUtility.CheckNotNull("endPointIDs", endPointIDs);

      var endPointsOfNewOrDeletedObjects = endPointIDs
          .Where(endPointID => endPointID.ObjectID != null)
          .Where(
              endPointID =>
              {
                Assertion.DebugIsNotNull(endPointID.ObjectID, "endPointID.ObjectID != null");
                var owningDataContainer = GetDataContainerWithoutLoading(endPointID.ObjectID);
                return owningDataContainer != null && (owningDataContainer.State.IsDeleted || owningDataContainer.State.IsNew);
              })
          .ToList();
      if (endPointsOfNewOrDeletedObjects.Count > 0)
      {
        var message = "Cannot unload the following relation end-points because they belong to new or deleted objects: "
            + string.Join(", ", endPointsOfNewOrDeletedObjects) + ".";
        var exception = new InvalidOperationException(message);
        return new ExceptionCommand(exception);
      }

      return _relationEndPointManager.CreateUnloadVirtualEndPointsCommand(endPointIDs);
    }

    public IDataManagementCommand CreateUnloadAllCommand ()
    {
      return new UnloadAllCommand(_relationEndPointManager, _dataContainerMap, _invalidDomainObjectManager, _transactionEventSink);
    }

    public IDataManagementCommand CreateUnloadFilteredDomainObjectsCommand (Predicate<DomainObject> domainObjectFilter)
    {
      ArgumentUtility.CheckNotNull("domainObjectFilter", domainObjectFilter);

      return new UnloadFilteredDomainObjectsCommand(
          _dataContainerMap,
          _invalidDomainObjectManager,
          //TODO RM-8240: refactor UnloadFilteredDomainObjectsCommand to move relation endpoint unloading to separate command created by RelationEndPointManager
          (RelationEndPointMap)_relationEndPointManager.RelationEndPoints,
          _transactionEventSink,
          domainObjectFilter);
    }

    private ClientTransactionsDifferException CreateClientTransactionsDifferException (string message, params object?[] args)
    {
      return new ClientTransactionsDifferException(String.Format(message, args));
    }

    private UnregisterDataContainerCommand CreateUnregisterDataContainerCommand (ObjectID objectID)
    {
      return new UnregisterDataContainerCommand(objectID, _dataContainerMap);
    }

    #region Serialization

    protected DataManager (SerializationInfo info, StreamingContext context)
    {
      _deserializedData = (object[])info.GetValue("doInfo.GetData", typeof(object[]))!;
      _clientTransaction = null!;
      _transactionEventSink = null!;
      _dataContainerEventListener = null!;
      _dataContainerMap = null!;
      _relationEndPointManager = null!;
      _domainObjectStateCache = null!;
      _invalidDomainObjectManager = null!;
      _objectLoader = null!;
    }

    void IDeserializationCallback.OnDeserialization (object? sender)
    {
      Assertion.IsNotNull(_deserializedData, "_deserializedData != null");
      var doInfo = new FlattenedDeserializationInfo(_deserializedData);
      _clientTransaction = doInfo.GetValueForHandle<ClientTransaction>();
      _transactionEventSink = doInfo.GetValueForHandle<IClientTransactionEventSink>();
      _dataContainerEventListener = doInfo.GetValueForHandle<IDataContainerEventListener>();
      _dataContainerMap = doInfo.GetValue<DataContainerMap>();
      _relationEndPointManager = doInfo.GetValueForHandle<RelationEndPointManager>();
      _domainObjectStateCache = doInfo.GetValue<DomainObjectStateCache>();
      _invalidDomainObjectManager = doInfo.GetValue<IInvalidDomainObjectManager>();
      _objectLoader = doInfo.GetValueForHandle<IObjectLoader>();

      _deserializedData = null;
      doInfo.SignalDeserializationFinished();
    }

#if NET8_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
    {
      var doInfo = new FlattenedSerializationInfo();
      doInfo.AddHandle(_clientTransaction);
      doInfo.AddHandle(_transactionEventSink);
      doInfo.AddHandle(_dataContainerEventListener);
      doInfo.AddValue(_dataContainerMap);
      doInfo.AddHandle(_relationEndPointManager);
      doInfo.AddValue(_domainObjectStateCache);
      doInfo.AddValue(_invalidDomainObjectManager);
      doInfo.AddHandle(_objectLoader);

      info.AddValue("doInfo.GetData", doInfo.GetData());
    }

    #endregion
  }
}
