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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Represents the state of a <see cref="VirtualCollectionEndPoint"/> where all of its data is available (ie., the end-point has been (lazily) loaded).
  /// </summary>
  public class CompleteVirtualCollectionEndPointLoadState : IVirtualCollectionEndPointLoadState
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (CompleteVirtualCollectionEndPointLoadState));

    private readonly IVirtualCollectionEndPointDataManager _dataManager;
    private readonly IRelationEndPointProvider _endPointProvider;
    private readonly IClientTransactionEventSink _transactionEventSink;

    private readonly Dictionary<ObjectID, IRealObjectEndPoint> _unsynchronizedOppositeEndPoints;


    public CompleteVirtualCollectionEndPointLoadState (
        IVirtualCollectionEndPointDataManager dataManager,
        IRelationEndPointProvider endPointProvider,
        IClientTransactionEventSink transactionEventSink)
    {
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull ("transactionEventSink", transactionEventSink);

      _dataManager = dataManager;
      _endPointProvider = endPointProvider;
      _transactionEventSink = transactionEventSink;

      _unsynchronizedOppositeEndPoints = new Dictionary<ObjectID, IRealObjectEndPoint> ();
    }

    public IVirtualCollectionEndPointDataManager DataManager
    {
      get { return _dataManager; }
    }

    public IRelationEndPointProvider EndPointProvider
    {
      get { return _endPointProvider; }
    }

    public IClientTransactionEventSink TransactionEventSink
    {
      get { return _transactionEventSink; }
    }

    public ICollection<IRealObjectEndPoint> UnsynchronizedOppositeEndPoints
    {
      get { return _unsynchronizedOppositeEndPoints.Values; }
    }

    public ReadOnlyVirtualCollectionDataDecorator GetData (IVirtualCollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      return new ReadOnlyVirtualCollectionDataDecorator (DataManager.CollectionData);
    }

    public ReadOnlyVirtualCollectionDataDecorator GetOriginalData (IVirtualCollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      return DataManager.OriginalCollectionData;
    }

    public void SetDataFromSubTransaction (
        IVirtualCollectionEndPoint collectionEndPoint,
        IVirtualEndPointLoadState<IVirtualCollectionEndPoint, ReadOnlyVirtualCollectionDataDecorator, IVirtualCollectionEndPointDataManager> sourceLoadState)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      var sourceCompleteLoadState =
          ArgumentUtility.CheckNotNullAndType<CompleteVirtualCollectionEndPointLoadState> ("sourceLoadState", sourceLoadState);

      DataManager.SetDataFromSubTransaction (sourceCompleteLoadState.DataManager, EndPointProvider);

      RaiseReplaceDataEvent (collectionEndPoint);
    }

    public bool? HasChangedFast ()
    {
      return DataManager.HasDataChangedFast();
    }

    public bool IsDataComplete ()
    {
      return true;
    }

    public void EnsureDataComplete (IVirtualCollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      // Data is already complete
    }

    public void MarkDataComplete (IVirtualCollectionEndPoint collectionEndPoint, IEnumerable<DomainObject> items, Action<IVirtualCollectionEndPointDataManager> stateSetter)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("items", items);
      ArgumentUtility.CheckNotNull ("stateSetter", stateSetter);

      throw new InvalidOperationException ("The data is already complete.");
    }

    public bool CanDataBeMarkedIncomplete (IVirtualCollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      return !HasChanged();
    }

    public virtual void MarkDataIncomplete (IVirtualCollectionEndPoint endPoint, Action stateSetter)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("stateSetter", stateSetter);

      if (HasChanged ())
      {
        var message = string.Format ("Cannot mark virtual end-point '{0}' incomplete because it has been changed.", endPoint.ID);
        throw new InvalidOperationException (message);
      }

      _transactionEventSink.RaiseRelationEndPointBecomingIncompleteEvent (endPoint.ID);

      stateSetter ();

      var allOppositeEndPoints = UnsynchronizedOppositeEndPoints.Concat (GetOriginalOppositeEndPoints());
      foreach (var oppositeEndPoint in allOppositeEndPoints)
        endPoint.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
    }

    public void SortCurrentData (IVirtualCollectionEndPoint collectionEndPoint, Comparison<DomainObject> comparison)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("comparison", comparison);

      DataManager.SortCurrentData (comparison);

      RaiseReplaceDataEvent (collectionEndPoint);
    }

    public void Synchronize (IVirtualCollectionEndPoint endPoint)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat ("End-point '{0}' is being synchronized.", endPoint.ID);

      foreach (var item in GetOriginalItemsWithoutEndPoints ())
        DataManager.UnregisterOriginalItemWithoutEndPoint (item);

      RaiseReplaceDataEvent (endPoint);
    }

    public void SynchronizeOppositeEndPoint (IVirtualCollectionEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat ("ObjectEndPoint '{0}' is being marked as synchronized.", oppositeEndPoint.ID);

      if (!_unsynchronizedOppositeEndPoints.Remove (oppositeEndPoint.ObjectID))
      {
        var message = string.Format (
            "Cannot synchronize opposite end-point '{0}' - the end-point is not in the list of unsynchronized end-points.",
            oppositeEndPoint.ID);
        throw new InvalidOperationException (message);
      }

      _dataManager.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
      oppositeEndPoint.MarkSynchronized ();

      RaiseReplaceDataEvent (endPoint);
    }

    public bool HasChanged ()
    {
      return _dataManager.HasDataChanged ();
    }

    public void Commit (IVirtualCollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      _dataManager.Commit ();
    }

    public void Rollback (IVirtualCollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      _dataManager.Rollback();

      RaiseReplaceDataEvent (endPoint);
    }

    public IDataManagementCommand CreateRemoveCommand (IVirtualCollectionEndPoint collectionEndPoint, DomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("removedRelatedObject", removedRelatedObject);

      //TODO RM-7294: Remove
      //CheckRemovedObject (removedRelatedObject);

      return new VirtualCollectionEndPointRemoveCommand (
          collectionEndPoint, removedRelatedObject, DataManager.CollectionData, EndPointProvider, TransactionEventSink);
    }

    public IDataManagementCommand CreateDeleteCommand (IVirtualCollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);

      //TODO RM-7294: Remove
      //if (UnsynchronizedOppositeEndPoints.Count != 0)
      //{
      //  var message = string.Format (
      //      "The domain object '{0}' cannot be deleted because the opposite object property '{2}' of domain object '{3}' is out of sync with the "
      //      + "collection property '{1}'. To make this change, synchronize the two properties by calling the "
      //      + "'BidirectionalRelationSyncService.Synchronize' method on the '{2}' property.",
      //      DataManager.EndPointID.ObjectID,
      //      DataManager.EndPointID.Definition.PropertyName,
      //      DataManager.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName,
      //      UnsynchronizedOppositeEndPoints.First().ObjectID);
      //  throw new InvalidOperationException (message);
      //}

      //TODO RM-7294: Remove
      //if (!IsSynchronized (collectionEndPoint))
      //{
      //  var message = string.Format (
      //      "The domain object '{0}' cannot be deleted because its collection property '{1}' is out of sync with "
      //      + "the opposite object property '{2}' of domain object '{3}'. To make this change, synchronize the two properties by calling the "
      //      + "'BidirectionalRelationSyncService.Synchronize' method on the '{1}' property.",
      //      DataManager.EndPointID.ObjectID,
      //      DataManager.EndPointID.Definition.PropertyName,
      //      DataManager.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName,
      //      DataManager.OriginalItemsWithoutEndPoints.First().ID);
      //  throw new InvalidOperationException (message);
      //}

      return new VirtualCollectionEndPointDeleteCommand (collectionEndPoint, DataManager.CollectionData, TransactionEventSink);
    }

    public IDataManagementCommand CreateAddCommand (IVirtualCollectionEndPoint collectionEndPoint, DomainObject addedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("addedRelatedObject", addedRelatedObject);

      //TODO RM-7294: Remove
      //CheckAddedObject (addedRelatedObject);

      return new VirtualCollectionEndPointAddCommand (
          collectionEndPoint,
          addedRelatedObject,
          DataManager.CollectionData,
          EndPointProvider,
          TransactionEventSink);
    }

    protected IEnumerable<IRealObjectEndPoint> GetOriginalOppositeEndPoints ()
    {
      return DataManager.OriginalOppositeEndPoints;
    }

    protected IEnumerable<DomainObject> GetOriginalItemsWithoutEndPoints ()
    {
      return DataManager.OriginalItemsWithoutEndPoints;
    }

    private void CheckAddedObject (DomainObject domainObject)
    {
      //TODO RM-7294: Remove
      if (ContainsUnsynchronizedOppositeEndPoint (domainObject.ID))
      {
        var message = string.Format (
            "The domain object with ID '{0}' cannot be added to collection property '{1}' of object '{2}' because its object property "
            + "'{3}' is out of sync with the collection property. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{3}' property.",
            domainObject.ID,
            DataManager.EndPointID.Definition.PropertyName,
            DataManager.EndPointID.ObjectID,
            DataManager.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName);
        throw new InvalidOperationException (message);
      }

      //TODO RM-7294: Remove
      if (DataManager.ContainsOriginalItemWithoutEndPoint (domainObject))
      {
        var message = string.Format (
            "The domain object with ID '{0}' cannot be added to collection property '{1}' of object '{2}' because the property is "
            + "out of sync with the opposite object property '{3}'. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{1}' property.",
            domainObject.ID,
            DataManager.EndPointID.Definition.PropertyName,
            DataManager.EndPointID.ObjectID,
            DataManager.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName);
        throw new InvalidOperationException (message);
      }
    }

    private void CheckRemovedObject (DomainObject domainObject)
    {
      //TODO RM-7294: Remove
      if (ContainsUnsynchronizedOppositeEndPoint (domainObject.ID))
      {
        var message = string.Format (
            "The domain object with ID '{0}' cannot be replaced or removed from collection property '{1}' of object '{2}' because its object property "
            + "'{3}' is out of sync with the collection property. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{3}' property.",
            domainObject.ID,
            DataManager.EndPointID.Definition.PropertyName,
            DataManager.EndPointID.ObjectID,
            DataManager.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName);
        throw new InvalidOperationException (message);
      }

      //TODO RM-7294: Remove
      if (DataManager.ContainsOriginalItemWithoutEndPoint (domainObject))
      {
        var message = string.Format (
            "The domain object with ID '{0}' cannot be replaced or removed from collection property '{1}' of object '{2}' because the property is "
            + "out of sync with the opposite object property '{3}'. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{1}' property.",
            domainObject.ID,
            DataManager.EndPointID.Definition.PropertyName,
            DataManager.EndPointID.ObjectID,
            DataManager.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName);
        throw new InvalidOperationException (message);
      }
    }

    private void RaiseReplaceDataEvent (IVirtualCollectionEndPoint endPoint)
    {
      var eventRaiser = endPoint.GetCollectionEventRaiser ();
      eventRaiser.WithinReplaceData ();
    }

    public bool CanEndPointBeCollected (IVirtualCollectionEndPoint endPoint)
    {
      return false;
    }

    public void RegisterOriginalOppositeEndPoint (IVirtualCollectionEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (_dataManager.ContainsOriginalObjectID (oppositeEndPoint.ObjectID))
      {
        // RealObjectEndPoint is registered for an already loaded virtual end-point. The query result contained the item, so the ObjectEndPoint is 
        // marked as synchronzed.

        _dataManager.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
        oppositeEndPoint.MarkSynchronized ();
      }
      else
      {
        // ObjectEndPoint is registered for an already loaded virtual end-point. The query result did not contain the item, so the ObjectEndPoint is 
        // out-of-sync.

        _unsynchronizedOppositeEndPoints.Add (oppositeEndPoint.ObjectID, oppositeEndPoint);
        oppositeEndPoint.MarkUnsynchronized ();
      }
    }

    public void UnregisterOriginalOppositeEndPoint (IVirtualCollectionEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (_unsynchronizedOppositeEndPoints.ContainsKey (oppositeEndPoint.ObjectID))
      {
        if (s_log.IsDebugEnabled())
        {
          s_log.DebugFormat (
              "Unsynchronized ObjectEndPoint '{0}' is unregistered from virtual end-point '{1}'.",
              oppositeEndPoint.ID,
              endPoint.ID);
        }

        _unsynchronizedOppositeEndPoints.Remove (oppositeEndPoint.ObjectID);
      }
      else
      {
        if (s_log.IsInfoEnabled())
        {
          s_log.InfoFormat (
              "ObjectEndPoint '{0}' is unregistered from virtual end-point '{1}'. The virtual end-point is transitioned to incomplete state.",
              oppositeEndPoint.ID,
              endPoint.ID);
        }

        endPoint.MarkDataIncomplete ();
        endPoint.UnregisterOriginalOppositeEndPoint (oppositeEndPoint);
      }
    }

    public void RegisterCurrentOppositeEndPoint (IVirtualCollectionEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _dataManager.RegisterCurrentOppositeEndPoint (oppositeEndPoint);
    }

    public void UnregisterCurrentOppositeEndPoint (IVirtualCollectionEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _dataManager.UnregisterCurrentOppositeEndPoint (oppositeEndPoint);
    }

    public bool? IsSynchronized (IVirtualCollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      return !GetOriginalItemsWithoutEndPoints().Any();
    }

    private bool ContainsUnsynchronizedOppositeEndPoint (ObjectID objectID)
    {
      return _unsynchronizedOppositeEndPoints.ContainsKey (objectID);
    }

    #region Serialization

    public CompleteVirtualCollectionEndPointLoadState (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      _dataManager = info.GetValueForHandle<IVirtualCollectionEndPointDataManager> ();
      _endPointProvider = info.GetValueForHandle<IRelationEndPointProvider> ();
      _transactionEventSink = info.GetValueForHandle<IClientTransactionEventSink> ();
      var unsynchronizedOppositeEndPoints = new List<IRealObjectEndPoint> ();
      info.FillCollection (unsynchronizedOppositeEndPoints);
      _unsynchronizedOppositeEndPoints = unsynchronizedOppositeEndPoints.ToDictionary (ep => ep.ObjectID);
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      info.AddHandle (_dataManager);
      info.AddHandle (_endPointProvider);
      info.AddHandle (_transactionEventSink);
      info.AddCollection (_unsynchronizedOppositeEndPoints.Values);
    }
    
    #endregion
  }
}