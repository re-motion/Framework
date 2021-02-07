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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Represents a collection-valued relation end-point in the <see cref="RelationEndPointManager"/>.
  /// </summary>
  public class VirtualCollectionEndPoint : RelationEndPoint, IVirtualCollectionEndPoint
  {
    //TODO RM-7294: Remove commented out code

    private static readonly ILog s_log = LogManager.GetLogger (typeof (VirtualCollectionEndPoint));

    private readonly IVirtualCollectionEndPointCollectionManager _collectionManager;
    private readonly ILazyLoader _lazyLoader;
    private readonly IRelationEndPointProvider _endPointProvider;
    private readonly IClientTransactionEventSink _transactionEventSink;
    private readonly IVirtualCollectionEndPointDataManagerFactory _dataManagerFactory;

    //private readonly Dictionary<ObjectID, IRealObjectEndPoint> _unsynchronizedOppositeEndPoints;
    //private readonly Dictionary<ObjectID, IRealObjectEndPoint> _originalOppositeEndPointsForIncompleteData;
    private readonly HashSet<ObjectID> _addedDomainObjects;
    private readonly HashSet<ObjectID> _removedDomainObjects;

    [CanBeNull]
    private IVirtualCollectionEndPointDataManager _dataManager;
    private bool _hasBeenTouched;

    public VirtualCollectionEndPoint (
        ClientTransaction clientTransaction,
        RelationEndPointID id,
        IVirtualCollectionEndPointCollectionManager collectionManager,
        ILazyLoader lazyLoader,
        IRelationEndPointProvider endPointProvider,
        IClientTransactionEventSink transactionEventSink,
        IVirtualCollectionEndPointDataManagerFactory dataManagerFactory)
        : base (ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction), ArgumentUtility.CheckNotNull ("id", id))
    {
      ArgumentUtility.CheckNotNull ("collectionManager", collectionManager);
      ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull ("transactionEventSink", transactionEventSink);
      ArgumentUtility.CheckNotNull ("dataManagerFactory", dataManagerFactory);

      if (id.Definition.Cardinality != CardinalityType.Many)
        throw new ArgumentException ("End point ID must refer to an end point with cardinality 'Many'.", "id");

      if (id.Definition.IsAnonymous)
        throw new ArgumentException ("End point ID must not refer to an anonymous end point.", "id");

      Assertion.IsTrue (ID.Definition.IsVirtual);

      _hasBeenTouched = false;
      _collectionManager = collectionManager;
      _lazyLoader = lazyLoader;
      _endPointProvider = endPointProvider;
      _transactionEventSink = transactionEventSink;
      _dataManagerFactory = dataManagerFactory;

      //_unsynchronizedOppositeEndPoints = new Dictionary<ObjectID, IRealObjectEndPoint>();
      //_originalOppositeEndPointsForIncompleteData = new Dictionary<ObjectID, IRealObjectEndPoint>();
      _addedDomainObjects = new HashSet<ObjectID>();
      _removedDomainObjects = new HashSet<ObjectID>();
    }

    public IVirtualCollectionEndPointCollectionManager CollectionManager
    {
      get { return _collectionManager; }
    }

    public ILazyLoader LazyLoader
    {
      get { return _lazyLoader; }
    }

    public IRelationEndPointProvider EndPointProvider
    {
      get { return _endPointProvider; }
    }

    public IClientTransactionEventSink TransactionEventSink
    {
      get { return _transactionEventSink; }
    }

    public IVirtualCollectionEndPointDataManagerFactory DataManagerFactory
    {
      get { return _dataManagerFactory; }
    }

    public IObjectList<IDomainObject> Collection
    {
      get { return _collectionManager.GetCurrentCollectionReference(); }
    }

    public IVirtualCollectionEventRaiser GetCollectionEventRaiser ()
    {
      return (IVirtualCollectionEventRaiser) Collection; // TODO RM-7294 invalid cast
    }

    public IObjectList<IDomainObject> GetCollectionWithOriginalData ()
    {
      return CreateCollection (GetOriginalData());
    }

    public ReadOnlyVirtualCollectionDataDecorator GetData ()
    {
      EnsureDataComplete ();
      Assertion.IsNotNull (_dataManager);

      return new ReadOnlyVirtualCollectionDataDecorator (_dataManager.CollectionData);
    }

    public ReadOnlyVirtualCollectionDataDecorator GetOriginalData ()
    {
      EnsureDataComplete ();
      Assertion.IsNotNull (_dataManager);

      return _dataManager.GetOriginalCollectionData();
    }

    public override bool IsDataComplete
    {
      get { return _dataManager != null; }
    }

    public bool CanBeCollected
    {
      get
      {
        if (_dataManager != null)
        {
          return true; //TODO: RM-7294: is this okay?
          //return false;
        }
        else
        {
          return true;
          //return _originalOppositeEndPointsForIncompleteData.Count == 0;
        }
      }
    }

    public bool CanBeMarkedIncomplete
    {
      get
      {
        //return !HasChanged(); //TODO: RM-7294: constrained no longer required
        return true;
      }
    }

    public override bool HasChanged
    {
      get
      {
        //TODO: RM-7294: merge ChangeTrackingVirtualCollectionDataDecorator into DataManager and make DataManager work for loaded and unloaded state
        return _addedDomainObjects.Count > 0 || _removedDomainObjects.Count > 0;
        //if (_dataManager == null)
        //  return false;
        //return _dataManager.HasDataChanged();
      }
    }

    public bool? HasChangedFast
    {
      get
      {
        //TODO: RM-7294: merge ChangeTrackingVirtualCollectionDataDecorator into DataManager and make DataManager work for loaded and unloaded state
        return _addedDomainObjects.Count > 0 || _removedDomainObjects.Count > 0;
        //if (_dataManager == null)
        //  return false;
        //return _dataManager.HasDataChangedFast();
      }
    }

    public override bool HasBeenTouched
    {
      get { return _hasBeenTouched; }
    }

    public override void EnsureDataComplete ()
    {
      if (_dataManager == null)
      {
        _lazyLoader.LoadLazyCollectionEndPoint (ID);
        Assertion.IsNotNull (_dataManager, "LazyLoad did not complete the collection endpoint");
      }
    }

    public void MarkDataComplete (DomainObject[] items)
    {
      ArgumentUtility.CheckNotNull ("items", items);

      if (_dataManager != null)
        throw new InvalidOperationException ("The data is already complete.");

      if (s_log.IsInfoEnabled())
        s_log.InfoFormat ("Virtual end-point '{0}' is transitioned to complete state.", ID);

      var dataManager = _dataManagerFactory.CreateEndPointDataManager (ID);

      //foreach (var item in items)
      //{
        //IRealObjectEndPoint oppositeEndPoint;
        //if (_originalOppositeEndPointsForIncompleteData.TryGetValue (item.ID, out oppositeEndPoint))
        //{
        //  dataManager.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
        //  oppositeEndPoint.MarkSynchronized();
        //  _originalOppositeEndPointsForIncompleteData.Remove (item.ID);
        //}
        //else
        //{
        //  // Virtual end-point contains an item without an opposite end-point. The virtual end-point is out-of-sync. Note that this can temporarily 
        //  // occur during eager fetching because the end-point contents are set before the related objects' DataContainers are registered.
        //  // Apart from that case, this indicates that foreign keys in the database have changed between loading the foreign key side and the virtual
        //  // side of a bidirectional relation.
        //  dataManager.RegisterOriginalItemWithoutEndPoint (item);
        //}
      //}

      //var originalOppositeEndPoints = _originalOppositeEndPointsForIncompleteData.Values.ToArray();

      _dataManager = dataManager;
      //_originalOppositeEndPointsForIncompleteData.Clear();
      //Assertion.IsTrue (_unsynchronizedOppositeEndPoints.Count == 0);

      //foreach (var oppositeEndPointWithoutItem in originalOppositeEndPoints)
      //  RegisterOriginalOppositeEndPoint (oppositeEndPointWithoutItem);

      RaiseReplaceDataEvent();
    }

    public void MarkDataIncomplete ()
    {
      if (_dataManager == null)
        return;

      //TODO: RM-7294: constrained no longer required
      //if (HasChanged())
      //{
      //  var message = string.Format ("Cannot mark virtual end-point '{0}' incomplete because it has been changed.", collectionEndPoint.ID);
      //  throw new InvalidOperationException (message);
      //}

      _transactionEventSink.RaiseRelationEndPointBecomingIncompleteEvent (ID);

      Assertion.DebugIsNotNull (_dataManager, "_dataManager has already been checked.");
      //var allOppositeEndPoints = _unsynchronizedOppositeEndPoints.Values.Concat (_dataManager.OriginalOppositeEndPoints).ToArray();

      _dataManager = null;
      //_unsynchronizedOppositeEndPoints.Clear();
      //Assertion.IsTrue (_originalOppositeEndPointsForIncompleteData.Count == 0);

      //foreach (var oppositeEndPoint in allOppositeEndPoints)
      //  RegisterOriginalOppositeEndPoint (oppositeEndPoint);
    }

    public override void Touch ()
    {
      _hasBeenTouched = true;
    }

    public override void Commit ()
    {
      if (HasChanged)
      {
        if (_dataManager != null)
          _dataManager.Commit();
      }

      _addedDomainObjects.Clear();
      _removedDomainObjects.Clear();
      _hasBeenTouched = false;
    }

    public override void Rollback ()
    {
      if (HasChanged)
      {
        if (_dataManager != null)
          _dataManager.Rollback();

        RaiseReplaceDataEvent();
      }

      _addedDomainObjects.Clear();
      _removedDomainObjects.Clear();
      _hasBeenTouched = false;
    }

    public override void ValidateMandatory ()
    {
      // In order to perform the mandatory check, we need to load data. It's up to the caller to decide whether an incomplete end-point should be 
      // checked. (DataManager will not check incomplete end-points, as it also ignores not-yet-loaded end-points.)

      if (GetData().Count == 0)
      {
        var objectReference = GetDomainObjectReference();
        var message = string.Format (
            "Mandatory relation property '{0}' of domain object '{1}' contains no items.",
            Definition.PropertyName,
            ObjectID);
        throw new MandatoryRelationNotSetException (objectReference, Definition.PropertyName, message);
      }
    }

    public void SortCurrentData (Comparison<DomainObject> comparison)
    {
      //TODO: RM-7294: API is obsolete. DomainObjectCollection implemented it for Ordered Collections

      ArgumentUtility.CheckNotNull ("comparison", comparison);

      throw new NotSupportedException ("RM-7294: API is obsolete. DomainObjectCollection implemented it for Ordered Collections");
      //Touch();
    }

    public void RegisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (_dataManager != null)
      {
        //TODO: RM-7294: do we need to reset the CachedDomainObjects?
        _dataManager.RegisterOriginalOppositeEndPoint (oppositeEndPoint); // performs cache reset
      }

      oppositeEndPoint.MarkSynchronized();

      //if (_dataManager != null)
      //{
      //  if (_dataManager.ContainsOriginalObjectID (oppositeEndPoint.ObjectID))
      //  {
      //    // RealObjectEndPoint is registered for an already loaded virtual end-point. The query result contained the item, so the ObjectEndPoint is 
      //    // marked as synchronzed.

      //    _dataManager.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
      //    oppositeEndPoint.MarkSynchronized();
      //  }
      //  else
      //  {
      //    // ObjectEndPoint is registered for an already loaded virtual end-point. The query result did not contain the item, so the ObjectEndPoint is 
      //    // out-of-sync.

      //    _unsynchronizedOppositeEndPoints.Add (oppositeEndPoint.ObjectID, oppositeEndPoint);
      //    oppositeEndPoint.MarkUnsynchronized();
      //  }
      //}
      //else
      //{
      //  _originalOppositeEndPointsForIncompleteData.Add (oppositeEndPoint.ObjectID, oppositeEndPoint);
      //  oppositeEndPoint.ResetSyncState();
      //}
    }

    public void UnregisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (s_log.IsInfoEnabled ())
      {
        s_log.InfoFormat (
            "RealObjectEndPoint '{0}' is unregistered from VirtualCollectionEndPoint '{1}'. The VirtualCollectionEndPoint is transitioned to incomplete state.",
            oppositeEndPoint.ID,
            ID);
      }
      MarkDataIncomplete ();

      //if (_dataManager != null)
      //{
      //  if (_unsynchronizedOppositeEndPoints.ContainsKey (oppositeEndPoint.ObjectID))
      //  {
      //    if (s_log.IsDebugEnabled())
      //    {
      //      s_log.DebugFormat (
      //          "Unsynchronized ObjectEndPoint '{0}' is unregistered from virtual end-point '{1}'.",
      //          oppositeEndPoint.ID,
      //          ID);
      //    }

      //    _unsynchronizedOppositeEndPoints.Remove (oppositeEndPoint.ObjectID);
      //  }
      //  else
      //  {
      //    if (s_log.IsInfoEnabled())
      //    {
      //      s_log.InfoFormat (
      //          "ObjectEndPoint '{0}' is unregistered from virtual end-point '{1}'. The virtual end-point is transitioned to incomplete state.",
      //          oppositeEndPoint.ID,
      //          ID);
      //    }

      //    MarkDataIncomplete();
      //    Assertion.DebugAssert (_dataManager == null, "MarkDataIncomplete clears _dataManager.");
      //    UnregisterOriginalOppositeEndPoint (oppositeEndPoint);
      //  }
      //}
      //else
      //{
      //  if (!_originalOppositeEndPointsForIncompleteData.ContainsKey (oppositeEndPoint.ObjectID))
      //    throw new InvalidOperationException ("The opposite end-point has not been registered.");

      //  _originalOppositeEndPointsForIncompleteData.Remove (oppositeEndPoint.ObjectID);
      //}
    }

    public void RegisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      EnsureDataComplete ();
      Assertion.DebugIsNotNull (_dataManager, "EnsureDataComplete sets _dataManager.");
      _dataManager.RegisterCurrentOppositeEndPoint (oppositeEndPoint);
    }

    public void UnregisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      EnsureDataComplete ();
      Assertion.DebugIsNotNull (_dataManager, "EnsureDataComplete sets _dataManager.");
      _dataManager.UnregisterCurrentOppositeEndPoint (oppositeEndPoint);
    }

    public override bool? IsSynchronized
    {
      get
      {
        return true;
        //if (_dataManager == null)
        //  return null;

        //return !_dataManager.OriginalItemsWithoutEndPoints.Any();
      }
    }

    public override void Synchronize ()
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat ("End-point '{0}' is being synchronized.", ID);

      EnsureDataComplete ();
      Assertion.DebugIsNotNull (_dataManager, "EnsureDataComplete sets _dataManager.");

      //TODO: RM-7294: do we need to reset the CachedDomainObjects?
      //foreach (var item in _dataManager.OriginalItemsWithoutEndPoints)
      //  _dataManager.UnregisterOriginalItemWithoutEndPoint (item);

      RaiseReplaceDataEvent();
    }

    public void SynchronizeOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      Assertion.IsNotNull (_dataManager, "Cannot synchronize an opposite end-point with a virtual end-point in incomplete state.");

      if (s_log.IsDebugEnabled())
        s_log.DebugFormat ("ObjectEndPoint '{0}' is being marked as synchronized.", oppositeEndPoint.ID);

      //if (!_unsynchronizedOppositeEndPoints.Remove (oppositeEndPoint.ObjectID))
      //{
      //  var message = string.Format (
      //      "Cannot synchronize opposite end-point '{0}' - the end-point is not in the list of unsynchronized end-points.",
      //      oppositeEndPoint.ID);
      //  throw new InvalidOperationException (message);
      //}

      _dataManager.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
      oppositeEndPoint.MarkSynchronized();

      RaiseReplaceDataEvent();
    }

    public override IDataManagementCommand CreateRemoveCommand (DomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("removedRelatedObject", removedRelatedObject);

      //TODO RM-7294: Remove
      //CheckRemovedObject (removedRelatedObject);


      IVirtualCollectionData virtualCollectionData;
      if (_dataManager == null)
      {
        virtualCollectionData = new IncompleteEndPointModificationVirtualCollectionData (ID);
      }
      else
      {
        virtualCollectionData = _dataManager.CollectionData;
      }

      //TODO: RM-7294: merge ChangeTrackingVirtualCollectionDataDecorator into DataManager and make DataManager work for loaded and unloaded state
      var changeTrackingVirtualCollectionData = new ChangeTrackingVirtualCollectionDataDecorator (
          virtualCollectionData,
          addedDomainObjects: _addedDomainObjects,
          removedDomainObjects: _removedDomainObjects);

      return new VirtualCollectionEndPointRemoveCommand (
          this,
          removedRelatedObject,
          changeTrackingVirtualCollectionData,
          _endPointProvider,
          _transactionEventSink);
    }

    public override IDataManagementCommand CreateDeleteCommand ()
    {
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
      if (_dataManager == null)
      {
        virtualCollectionData = new IncompleteEndPointModificationVirtualCollectionData (ID);
      }
      else
      {
        virtualCollectionData = _dataManager.CollectionData;
      }

      //TODO: RM-7294: merge ChangeTrackingVirtualCollectionDataDecorator into DataManager and make DataManager work for loaded and unloaded state
      var changeTrackingVirtualCollectionData = new ChangeTrackingVirtualCollectionDataDecorator (
          virtualCollectionData,
          addedDomainObjects: _addedDomainObjects,
          removedDomainObjects: _removedDomainObjects);

      return new VirtualCollectionEndPointDeleteCommand (this, changeTrackingVirtualCollectionData, _transactionEventSink);
    }

    public virtual IDataManagementCommand CreateAddCommand (DomainObject addedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("addedRelatedObject", addedRelatedObject);

      //TODO RM-7294: Remove
      //CheckAddedObject (addedRelatedObject);

      IVirtualCollectionData virtualCollectionData;
      if (_dataManager == null)
      {
        virtualCollectionData = new IncompleteEndPointModificationVirtualCollectionData (ID);
      }
      else
      {
        virtualCollectionData = _dataManager.CollectionData;
      }

      //TODO: RM-7294: merge ChangeTrackingVirtualCollectionDataDecorator into DataManager and make DataManager work for loaded and unloaded state
      var changeTrackingVirtualCollectionData = new ChangeTrackingVirtualCollectionDataDecorator (
          virtualCollectionData,
          addedDomainObjects: _addedDomainObjects,
          removedDomainObjects: _removedDomainObjects);

      return new VirtualCollectionEndPointAddCommand (
          this,
          addedRelatedObject,
          changeTrackingVirtualCollectionData,
          _endPointProvider,
          _transactionEventSink);
    }

    public override IEnumerable<RelationEndPointID> GetOppositeRelationEndPointIDs ()
    {
      var oppositeEndPointDefinition = Definition.GetOppositeEndPointDefinition();

      Assertion.IsFalse (oppositeEndPointDefinition.IsAnonymous);

      EnsureDataComplete ();
      Assertion.IsNotNull (_dataManager);

      return _dataManager.CollectionData
          .Select (oppositeDomainObject => RelationEndPointID.Create (oppositeDomainObject.ID, oppositeEndPointDefinition));
    }

    public override void SetDataFromSubTransaction (IRelationEndPoint source)
    {
      var sourceCollectionEndPoint = ArgumentUtility.CheckNotNullAndType<VirtualCollectionEndPoint> ("source", source);
      if (Definition != sourceCollectionEndPoint.Definition)
      {
        var message = string.Format (
            "Cannot set this end point's value from '{0}'; the end points do not have the same end point definition.",
            source.ID);
        throw new ArgumentException (message, "source");
      }

      var sourceDataManager = sourceCollectionEndPoint._dataManager;
      Assertion.IsNotNull (_dataManager, "Cannot commit data from a sub-transaction into a virtual collection end-point in incomplete state.");
      Assertion.IsNotNull (sourceDataManager, "Cannot commit incomplete data from a sub-transaction into a virtual collection end-point.");
      _dataManager.SetDataFromSubTransaction (sourceDataManager, _endPointProvider);
      RaiseReplaceDataEvent();

      if (sourceCollectionEndPoint.HasBeenTouched || HasChanged)
        Touch();
    }

    private IObjectList<IDomainObject> CreateCollection (IVirtualCollectionData dataStrategy)
    {
      return ObjectListFactory.Create (dataStrategy);
    }

    private void RaiseReplaceDataEvent ()
    {
      //TODO: RM-7294: unused on VirtualObjectList, can be removed

      var eventRaiser = GetCollectionEventRaiser();
      eventRaiser.WithinReplaceData();
    }

    #region Serialization

    protected VirtualCollectionEndPoint (FlattenedDeserializationInfo info)
        : base (info)
    {
      _collectionManager = info.GetValueForHandle<IVirtualCollectionEndPointCollectionManager>();
      _lazyLoader = info.GetValueForHandle<ILazyLoader>();
      _endPointProvider = info.GetValueForHandle<IRelationEndPointProvider>();
      _transactionEventSink = info.GetValueForHandle<IClientTransactionEventSink>();
      _dataManagerFactory = info.GetValueForHandle<IVirtualCollectionEndPointDataManagerFactory>();

      _dataManager = info.GetValueForHandle<IVirtualCollectionEndPointDataManager>();
      _hasBeenTouched = info.GetBoolValue();

      //var unsynchronizedOppositeEndPoints = new List<IRealObjectEndPoint>();
      //info.FillCollection (unsynchronizedOppositeEndPoints);
      //_unsynchronizedOppositeEndPoints = unsynchronizedOppositeEndPoints.ToDictionary (ep => ep.ObjectID);

      //var realObjectEndPoints = new List<IRealObjectEndPoint>();
      //info.FillCollection (realObjectEndPoints);
      //_originalOppositeEndPointsForIncompleteData = realObjectEndPoints.ToDictionary (ep => ep.ObjectID);

      _addedDomainObjects = new HashSet<ObjectID>();
      info.FillCollection (_addedDomainObjects);

      _removedDomainObjects = new HashSet<ObjectID>();
      info.FillCollection (_removedDomainObjects);
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle (_collectionManager);
      info.AddHandle (_lazyLoader);
      info.AddHandle (_endPointProvider);
      info.AddHandle (_transactionEventSink);
      info.AddHandle (_dataManagerFactory);

      info.AddHandle (_dataManager);
      info.AddBoolValue (_hasBeenTouched);

      //info.AddCollection (_unsynchronizedOppositeEndPoints.Values);
      //info.AddCollection (_originalOppositeEndPointsForIncompleteData.Values);
      info.AddCollection (_addedDomainObjects);
      info.AddCollection (_removedDomainObjects);
    }

    #endregion
  }
}