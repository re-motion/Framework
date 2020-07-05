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
using JetBrains.Annotations;
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
  public class VirtualCollectionEndPointLoadState : IFlattenedSerializable
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (VirtualCollectionEndPointLoadState));


    [CanBeNull]
    public IVirtualCollectionEndPointDataManager DataManager { get; internal set; }
    public IRelationEndPointProvider EndPointProvider { get; }
    public IClientTransactionEventSink TransactionEventSink { get; }
    private readonly ILazyLoader _lazyLoader;
    private readonly IVirtualCollectionEndPointDataManagerFactory _dataManagerFactory;

    private readonly Dictionary<ObjectID, IRealObjectEndPoint> _unsynchronizedOppositeEndPoints;
    private readonly Dictionary<ObjectID, IRealObjectEndPoint> _originalOppositeEndPointsForIncompleteData;
    private readonly HashSet<ObjectID> _addedDomainObjects;
    private readonly HashSet<ObjectID> _removedDomainObjects;

    public VirtualCollectionEndPointLoadState (
        ILazyLoader lazyLoader,
        IVirtualCollectionEndPointDataManagerFactory dataManagerFactory,
        IRelationEndPointProvider endPointProvider,
        IClientTransactionEventSink transactionEventSink)
    {
      ArgumentUtility.CheckNotNull ("dataManagerFactory", dataManagerFactory);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull ("transactionEventSink", transactionEventSink);

      _lazyLoader = lazyLoader;
      _dataManagerFactory = dataManagerFactory;
      EndPointProvider = endPointProvider;
      TransactionEventSink = transactionEventSink;

      _unsynchronizedOppositeEndPoints = new Dictionary<ObjectID, IRealObjectEndPoint>();
      _originalOppositeEndPointsForIncompleteData = new Dictionary<ObjectID, IRealObjectEndPoint>();
      _addedDomainObjects = new HashSet<ObjectID>();
      _removedDomainObjects = new HashSet<ObjectID>();
    }

    public IReadOnlyCollection<IRealObjectEndPoint> UnsynchronizedOppositeEndPoints
    {
      get { return _unsynchronizedOppositeEndPoints.Values; }
    }

    public ReadOnlyVirtualCollectionDataDecorator GetData (IVirtualCollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);

      EnsureDataComplete (collectionEndPoint);
      Assertion.IsNotNull (DataManager);

      return new ReadOnlyVirtualCollectionDataDecorator (DataManager.CollectionData);
    }

    public ReadOnlyVirtualCollectionDataDecorator GetOriginalData (IVirtualCollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);

      EnsureDataComplete (collectionEndPoint);
      Assertion.IsNotNull (DataManager);

      return DataManager.OriginalCollectionData;
    }

    public bool? HasChangedFast ()
    {
      if (DataManager == null)
        return false;
      return DataManager.HasDataChangedFast();
    }

    public bool IsDataComplete ()
    {
      return DataManager != null;
    }

    private void EnsureDataComplete (IVirtualCollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);

      if (DataManager == null)
      {
        _lazyLoader.LoadLazyCollectionEndPoint (collectionEndPoint.ID);
        Assertion.IsNotNull (DataManager, "LazyLoad did not complete the collection endpoint");
      }
    }

    public void MarkDataComplete (
        IVirtualCollectionEndPoint collectionEndPoint,
        IEnumerable<DomainObject> items,
        Action<IVirtualCollectionEndPointDataManager> stateSetter)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("items", items);
      ArgumentUtility.CheckNotNull ("stateSetter", stateSetter);

      if (DataManager != null)
        throw new InvalidOperationException ("The data is already complete.");

      if (s_log.IsInfoEnabled())
        s_log.InfoFormat ("Virtual end-point '{0}' is transitioned to complete state.", collectionEndPoint.ID);

      var dataManager = _dataManagerFactory.CreateEndPointDataManager (collectionEndPoint.ID);

      foreach (var item in items)
      {
        IRealObjectEndPoint oppositeEndPoint;
        if (_originalOppositeEndPointsForIncompleteData.TryGetValue (item.ID, out oppositeEndPoint))
        {
          dataManager.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
          oppositeEndPoint.MarkSynchronized();
          _originalOppositeEndPointsForIncompleteData.Remove (item.ID);
        }
        else
        {
          // Virtual end-point contains an item without an opposite end-point. The virtual end-point is out-of-sync. Note that this can temporarily 
          // occur during eager fetching because the end-point contents are set before the related objects' DataContainers are registered.
          // Apart from that case, this indicates that foreign keys in the database have changed between loading the foreign key side and the virtual
          // side of a bidirectional relation.
          dataManager.RegisterOriginalItemWithoutEndPoint (item);
        }
      }

      var originalOppositeEndPoints = _originalOppositeEndPointsForIncompleteData.Values.ToArray();

      DataManager = dataManager;
      _originalOppositeEndPointsForIncompleteData.Clear();
      Assertion.IsTrue (_unsynchronizedOppositeEndPoints.Count == 0);

      foreach (var oppositeEndPointWithoutItem in originalOppositeEndPoints)
        collectionEndPoint.RegisterOriginalOppositeEndPoint (oppositeEndPointWithoutItem);

      var eventRaiser = collectionEndPoint.GetCollectionEventRaiser();
      eventRaiser.WithinReplaceData();

    }

    public bool CanDataBeMarkedIncomplete (IVirtualCollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      //return !HasChanged(); //TODO: RM-7294: constrained no longer required
      return true;
    }

    public void MarkDataIncomplete (IVirtualCollectionEndPoint collectionEndPoint, Action stateSetter)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("stateSetter", stateSetter);

      if (DataManager == null)
        return;

      //TODO: RM-7294: constrained no longer required
      //if (HasChanged())
      //{
      //  var message = string.Format ("Cannot mark virtual end-point '{0}' incomplete because it has been changed.", collectionEndPoint.ID);
      //  throw new InvalidOperationException (message);
      //}

      TransactionEventSink.RaiseRelationEndPointBecomingIncompleteEvent (collectionEndPoint.ID);

      Assertion.DebugIsNotNull (DataManager, "_dataManager has already been checked.");
      var allOppositeEndPoints = _unsynchronizedOppositeEndPoints.Values.Concat (DataManager.OriginalOppositeEndPoints).ToArray();

      DataManager = null;
      _unsynchronizedOppositeEndPoints.Clear();
      Assertion.IsTrue (_originalOppositeEndPointsForIncompleteData.Count == 0);

      foreach (var oppositeEndPoint in allOppositeEndPoints)
        collectionEndPoint.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
    }

    public void SortCurrentData (IVirtualCollectionEndPoint collectionEndPoint, Comparison<DomainObject> comparison)
    {
      throw new NotSupportedException ("RM-7294: API is obsolete. DomainObjectCollection implemented it for Ordered Collections");
    }

    public void Synchronize (IVirtualCollectionEndPoint collectionEndPoint)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat ("End-point '{0}' is being synchronized.", collectionEndPoint.ID);

      EnsureDataComplete (collectionEndPoint);
      Assertion.DebugIsNotNull (DataManager, "EnsureDataComplete sets _dataManager.");

      foreach (var item in DataManager.OriginalItemsWithoutEndPoints)
        DataManager.UnregisterOriginalItemWithoutEndPoint (item);

      RaiseReplaceDataEvent (collectionEndPoint);
    }

    public void SynchronizeOppositeEndPoint (IVirtualCollectionEndPoint collectionEndPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      Assertion.IsNotNull (DataManager, "Cannot synchronize an opposite end-point with a virtual end-point in incomplete state.");

      if (s_log.IsDebugEnabled())
        s_log.DebugFormat ("ObjectEndPoint '{0}' is being marked as synchronized.", oppositeEndPoint.ID);

      if (!_unsynchronizedOppositeEndPoints.Remove (oppositeEndPoint.ObjectID))
      {
        var message = string.Format (
            "Cannot synchronize opposite end-point '{0}' - the end-point is not in the list of unsynchronized end-points.",
            oppositeEndPoint.ID);
        throw new InvalidOperationException (message);
      }

      DataManager.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
      oppositeEndPoint.MarkSynchronized();

      RaiseReplaceDataEvent (collectionEndPoint);
    }

    public bool HasChanged ()
    {
      if (DataManager == null)
        return false;
      return DataManager.HasDataChanged();
    }

    public void Commit (IVirtualCollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);

      if (DataManager != null)
        DataManager.Commit();
    }

    public void Rollback (IVirtualCollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);

      if (DataManager != null)
        DataManager.Rollback();

      RaiseReplaceDataEvent (collectionEndPoint);
    }

    public IDataManagementCommand CreateRemoveCommand (IVirtualCollectionEndPoint collectionEndPoint, DomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("removedRelatedObject", removedRelatedObject);

      //TODO RM-7294: Remove
      //CheckRemovedObject (removedRelatedObject);


      IVirtualCollectionData virtualCollectionData;
      if (DataManager == null)
      {
        virtualCollectionData = new IncompleteEndPointModificationVirtualCollectionData (collectionEndPoint.ID);
      }
      else
      {
        virtualCollectionData = DataManager.CollectionData;
      }

      return new VirtualCollectionEndPointRemoveCommand (
          collectionEndPoint,
          removedRelatedObject,
          virtualCollectionData,
          EndPointProvider,
          TransactionEventSink);
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
      //      _dataManager.EndPointID.ObjectID,
      //      _dataManager.EndPointID.Definition.PropertyName,
      //      _dataManager.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName,
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
      //      _dataManager.EndPointID.ObjectID,
      //      _dataManager.EndPointID.Definition.PropertyName,
      //      _dataManager.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName,
      //      _dataManager.OriginalItemsWithoutEndPoints.First().ID);
      //  throw new InvalidOperationException (message);
      //}

      IVirtualCollectionData virtualCollectionData;
      if (DataManager == null)
      {
        virtualCollectionData = new IncompleteEndPointModificationVirtualCollectionData (collectionEndPoint.ID);
      }
      else
      {
        virtualCollectionData = DataManager.CollectionData;
      }

      return new VirtualCollectionEndPointDeleteCommand (collectionEndPoint, virtualCollectionData, TransactionEventSink);
    }

    public IDataManagementCommand CreateAddCommand (IVirtualCollectionEndPoint collectionEndPoint, DomainObject addedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("addedRelatedObject", addedRelatedObject);

      //TODO RM-7294: Remove
      //CheckAddedObject (addedRelatedObject);

      IVirtualCollectionData virtualCollectionData;
      if (DataManager == null)
      {
        virtualCollectionData = new IncompleteEndPointModificationVirtualCollectionData (collectionEndPoint.ID);
      }
      else
      {
        virtualCollectionData = DataManager.CollectionData;
      }

      return new VirtualCollectionEndPointAddCommand (
          collectionEndPoint,
          addedRelatedObject,
          virtualCollectionData,
          EndPointProvider,
          TransactionEventSink);
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

    private void RaiseReplaceDataEvent (IVirtualCollectionEndPoint collectionEndPoint)
    {
      var eventRaiser = collectionEndPoint.GetCollectionEventRaiser();
      eventRaiser.WithinReplaceData();
    }

    public bool CanEndPointBeCollected (IVirtualCollectionEndPoint collectionEndPoint)
    {
      if (DataManager != null)
      {
        return false;
      }
      else
      {
        return _originalOppositeEndPointsForIncompleteData.Count == 0;
      }
    }

    public void RegisterOriginalOppositeEndPoint (IVirtualCollectionEndPoint collectionEndPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (DataManager != null)
      {
        if (DataManager.ContainsOriginalObjectID (oppositeEndPoint.ObjectID))
        {
          // RealObjectEndPoint is registered for an already loaded virtual end-point. The query result contained the item, so the ObjectEndPoint is 
          // marked as synchronzed.

          DataManager.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
          oppositeEndPoint.MarkSynchronized();
        }
        else
        {
          // ObjectEndPoint is registered for an already loaded virtual end-point. The query result did not contain the item, so the ObjectEndPoint is 
          // out-of-sync.

          _unsynchronizedOppositeEndPoints.Add (oppositeEndPoint.ObjectID, oppositeEndPoint);
          oppositeEndPoint.MarkUnsynchronized();
        }
      }
      else
      {
        _originalOppositeEndPointsForIncompleteData.Add (oppositeEndPoint.ObjectID, oppositeEndPoint);
        oppositeEndPoint.ResetSyncState();
      }
    }

    public void UnregisterOriginalOppositeEndPoint (IVirtualCollectionEndPoint collectionEndPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (DataManager != null)
      {
        if (_unsynchronizedOppositeEndPoints.ContainsKey (oppositeEndPoint.ObjectID))
        {
          if (s_log.IsDebugEnabled())
          {
            s_log.DebugFormat (
                "Unsynchronized ObjectEndPoint '{0}' is unregistered from virtual end-point '{1}'.",
                oppositeEndPoint.ID,
                collectionEndPoint.ID);
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
                collectionEndPoint.ID);
          }

          collectionEndPoint.MarkDataIncomplete();
          collectionEndPoint.UnregisterOriginalOppositeEndPoint (oppositeEndPoint);
        }
      }
      else
      {
        if (!_originalOppositeEndPointsForIncompleteData.ContainsKey (oppositeEndPoint.ObjectID))
          throw new InvalidOperationException ("The opposite end-point has not been registered.");

        _originalOppositeEndPointsForIncompleteData.Remove (oppositeEndPoint.ObjectID);
      }
    }

    public void RegisterCurrentOppositeEndPoint (IVirtualCollectionEndPoint collectionEndPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      EnsureDataComplete (collectionEndPoint);
      Assertion.DebugIsNotNull (DataManager, "EnsureDataComplete sets _dataManager.");
      DataManager.RegisterCurrentOppositeEndPoint (oppositeEndPoint);
    }

    public void UnregisterCurrentOppositeEndPoint (IVirtualCollectionEndPoint collectionEndPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      EnsureDataComplete (collectionEndPoint);
      Assertion.DebugIsNotNull (DataManager, "EnsureDataComplete sets _dataManager.");
      DataManager.UnregisterCurrentOppositeEndPoint (oppositeEndPoint);
    }

    public bool? IsSynchronized (IVirtualCollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);

      if (DataManager == null)
        return null;

      return !DataManager.OriginalItemsWithoutEndPoints.Any();
    }

    private bool ContainsUnsynchronizedOppositeEndPoint (ObjectID objectID)
    {
      return _unsynchronizedOppositeEndPoints.ContainsKey (objectID);
    }

    #region Serialization

    public VirtualCollectionEndPointLoadState (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      _lazyLoader = info.GetValue<ILazyLoader>();
      _dataManagerFactory = info.GetValueForHandle<IVirtualCollectionEndPointDataManagerFactory>();
      DataManager = info.GetValueForHandle<IVirtualCollectionEndPointDataManager>();
      EndPointProvider = info.GetValueForHandle<IRelationEndPointProvider>();
      TransactionEventSink = info.GetValueForHandle<IClientTransactionEventSink>();

      var unsynchronizedOppositeEndPoints = new List<IRealObjectEndPoint>();
      info.FillCollection (unsynchronizedOppositeEndPoints);
      _unsynchronizedOppositeEndPoints = unsynchronizedOppositeEndPoints.ToDictionary (ep => ep.ObjectID);

      var realObjectEndPoints = new List<IRealObjectEndPoint>();
      info.FillCollection (realObjectEndPoints);
      _originalOppositeEndPointsForIncompleteData = realObjectEndPoints.ToDictionary (ep => ep.ObjectID);
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      info.AddValue (_lazyLoader);
      info.AddHandle (_dataManagerFactory);
      info.AddHandle (DataManager);
      info.AddHandle (EndPointProvider);
      info.AddHandle (TransactionEventSink);
      info.AddCollection (_unsynchronizedOppositeEndPoints.Values);
      info.AddCollection (_originalOppositeEndPointsForIncompleteData.Values);
    }

    #endregion
  }
}