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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Represents a collection-valued relation end-point in the <see cref="RelationEndPointManager"/>.
  /// </summary>
  public class VirtualCollectionEndPoint : RelationEndPoint, IVirtualCollectionEndPoint
  {
    [Serializable]
    public class EndPointLoader : IncompleteVirtualCollectionEndPointLoadState.IEndPointLoader
    {
      private readonly ILazyLoader _lazyLoader;

      public EndPointLoader (ILazyLoader lazyLoader)
      {
        ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader);
        _lazyLoader = lazyLoader;
      }

      public ILazyLoader LazyLoader
      {
        get { return _lazyLoader; }
      }

      public IVirtualCollectionEndPointLoadState LoadEndPointAndGetNewState (IVirtualCollectionEndPoint endPoint)
      {
        var collectionEndPoint = ArgumentUtility.CheckNotNullAndType<VirtualCollectionEndPoint> ("endPoint", endPoint);
        _lazyLoader.LoadLazyCollectionEndPoint (endPoint.ID);
        return collectionEndPoint._loadState;
      }

      #region Serialization
      public EndPointLoader (FlattenedDeserializationInfo info)
      {
        ArgumentUtility.CheckNotNull ("info", info);

        _lazyLoader = info.GetValueForHandle<ILazyLoader>();
      }

      void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
      {
        ArgumentUtility.CheckNotNull ("info", info);

        info.AddHandle (_lazyLoader);
      }
      #endregion
    }

    private readonly IVirtualCollectionEndPointCollectionManager _collectionManager;
    private readonly ILazyLoader _lazyLoader;
    private readonly IRelationEndPointProvider _endPointProvider;
    private readonly IClientTransactionEventSink _transactionEventSink;
    private readonly IVirtualCollectionEndPointDataManagerFactory _dataManagerFactory;

    private IVirtualCollectionEndPointLoadState _loadState; // keeps track of whether this end-point has been completely loaded or not

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

      SetIncompleteLoadState ();
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

    public IObjectList Collection
    {
      get { return _collectionManager.GetCurrentCollectionReference (); }
    }

    public IDomainObjectCollectionEventRaiser GetCollectionEventRaiser ()
    {
      return (IDomainObjectCollectionEventRaiser) Collection; // TODO RM-7294 invalid cast
    }

    public IObjectList GetCollectionWithOriginalData ()
    {
      return CreateCollection ((IVirtualCollectionData)_loadState.GetOriginalData (this)); // TODO RM-7294 invalid cast
    }

    public ReadOnlyDomainObjectCollectionDataDecorator GetData ()
    {
      return _loadState.GetData (this);
    }

    public ReadOnlyDomainObjectCollectionDataDecorator GetOriginalData ()
    {
      return _loadState.GetOriginalData (this);
    }

    public override bool IsDataComplete
    {
      get { return _loadState.IsDataComplete(); }
    }

    public bool CanBeCollected
    {
      get { return !_collectionManager.HasCollectionReferenceChanged() && _loadState.CanEndPointBeCollected (this); }
    }

    public bool CanBeMarkedIncomplete
    {
      get { return !_collectionManager.HasCollectionReferenceChanged() && _loadState.CanDataBeMarkedIncomplete (this); }
    }

    public override bool HasChanged
    {
      get { return _collectionManager.HasCollectionReferenceChanged () || _loadState.HasChanged (); }
    }

    public bool? HasChangedFast
    {
      get
      {
        if (_collectionManager.HasCollectionReferenceChanged ())
          return true;
        return _loadState.HasChangedFast ();
      }
    }

    public override bool HasBeenTouched
    {
      get { return _hasBeenTouched; }
    }

    public override void EnsureDataComplete ()
    {
      _loadState.EnsureDataComplete(this);
    }

    public void MarkDataComplete (DomainObject[] items)
    {
      ArgumentUtility.CheckNotNull ("items", items);
      _loadState.MarkDataComplete (this, items, SetCompleteLoadState);
    }

    public void MarkDataIncomplete ()
    {
      _loadState.MarkDataIncomplete (this, SetIncompleteLoadState);
    }

    public override void Touch ()
    {
      _hasBeenTouched = true;
    }

    public override void Commit ()
    {
      if (HasChanged)
      {
        _collectionManager.CommitCollectionReference ();
        _loadState.Commit (this);
      }

      _hasBeenTouched = false;
    }

    public override void Rollback ()
    {
      if (HasChanged)
      {
        _collectionManager.RollbackCollectionReference ();
        _loadState.Rollback (this);
      }

      _hasBeenTouched = false;
    }

    public override void ValidateMandatory ()
    {
      // In order to perform the mandatory check, we need to load data. It's up to the caller to decide whether an incomplete end-point should be 
      // checked. (DataManager will not check incomplete end-points, as it also ignores not-yet-loaded end-points.)

      if (GetData ().Count == 0)
      {
        var objectReference = GetDomainObjectReference ();
        var message = String.Format (
            "Mandatory relation property '{0}' of domain object '{1}' contains no items.",
            Definition.PropertyName,
            ObjectID);
        throw new MandatoryRelationNotSetException (objectReference, Definition.PropertyName, message);
      }
    }

    public void SortCurrentData (Comparison<DomainObject> comparison)
    {
      ArgumentUtility.CheckNotNull ("comparison", comparison);

      _loadState.SortCurrentData (this, comparison);
      Touch();
    }

    public void RegisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _loadState.RegisterOriginalOppositeEndPoint (this, oppositeEndPoint);
    }

    public void UnregisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _loadState.UnregisterOriginalOppositeEndPoint (this, oppositeEndPoint);
    }

    public void RegisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _loadState.RegisterCurrentOppositeEndPoint (this, oppositeEndPoint);
    }

    public void UnregisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _loadState.UnregisterCurrentOppositeEndPoint (this, oppositeEndPoint);
    }

    public override bool? IsSynchronized
    {
      get { return _loadState.IsSynchronized (this); }
    }

    public override void Synchronize ()
    {
      _loadState.Synchronize (this);
    }

    public void SynchronizeOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _loadState.SynchronizeOppositeEndPoint (this, oppositeEndPoint);
    }

    public override IDataManagementCommand CreateRemoveCommand (DomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("removedRelatedObject", removedRelatedObject);

      var command = _loadState.CreateRemoveCommand (this, removedRelatedObject);
      return command;
    }

    public override IDataManagementCommand CreateDeleteCommand ()
    {
      var command = _loadState.CreateDeleteCommand (this);
      return command;
    }

    public virtual IDataManagementCommand CreateAddCommand (DomainObject addedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("addedRelatedObject", addedRelatedObject);
      var command = _loadState.CreateAddCommand (this, addedRelatedObject);
      return command;
    }

    public override IEnumerable<RelationEndPointID> GetOppositeRelationEndPointIDs ()
    {
      var oppositeEndPointDefinition = Definition.GetOppositeEndPointDefinition ();

      Assertion.IsFalse (oppositeEndPointDefinition.IsAnonymous);

      return from oppositeDomainObject in _loadState.GetData (this)
             select RelationEndPointID.Create (oppositeDomainObject.ID, oppositeEndPointDefinition);
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

      _loadState.SetDataFromSubTransaction (this, sourceCollectionEndPoint._loadState);

      if (sourceCollectionEndPoint.HasBeenTouched || HasChanged)
        Touch ();
    }

    private void SetCompleteLoadState (IVirtualCollectionEndPointDataManager dataManager)
    {
      _loadState = new CompleteVirtualCollectionEndPointLoadState (dataManager, _endPointProvider, _transactionEventSink);
    }

    private void SetIncompleteLoadState ()
    {
      var loader = new EndPointLoader (_lazyLoader);
      _loadState = new IncompleteVirtualCollectionEndPointLoadState (loader, _dataManagerFactory);
    }

    private IObjectList CreateCollection (IVirtualCollectionData dataStrategy)
    {
      return ObjectListFactory.Create (dataStrategy);
    }

    #region Serialization

    protected VirtualCollectionEndPoint (FlattenedDeserializationInfo info)
        : base (info)
    {
      _collectionManager = info.GetValueForHandle<IVirtualCollectionEndPointCollectionManager>();
      _lazyLoader = info.GetValueForHandle<ILazyLoader>();
      _endPointProvider = info.GetValueForHandle<IRelationEndPointProvider> ();
      _transactionEventSink = info.GetValueForHandle<IClientTransactionEventSink> ();
      _dataManagerFactory = info.GetValueForHandle<IVirtualCollectionEndPointDataManagerFactory> ();

      _loadState = info.GetValue<IVirtualCollectionEndPointLoadState>();
      _hasBeenTouched = info.GetBoolValue ();
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle (_collectionManager);
      info.AddHandle (_lazyLoader);
      info.AddHandle (_endPointProvider);
      info.AddHandle (_transactionEventSink);
      info.AddHandle (_dataManagerFactory);

      info.AddValue (_loadState);
      info.AddBoolValue (_hasBeenTouched);
    }

    #endregion
  }
}