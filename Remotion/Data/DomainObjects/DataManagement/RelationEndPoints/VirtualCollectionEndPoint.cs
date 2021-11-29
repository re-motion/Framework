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
    private static readonly ILog s_log = LogManager.GetLogger(typeof(VirtualCollectionEndPoint));

    private readonly IVirtualCollectionEndPointCollectionManager _collectionManager;
    private readonly ILazyLoader _lazyLoader;
    private readonly IRelationEndPointProvider _endPointProvider;
    private readonly IClientTransactionEventSink _transactionEventSink;
    private readonly IVirtualCollectionEndPointDataManagerFactory _dataManagerFactory;

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
        : base(ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction), ArgumentUtility.CheckNotNull("id", id))
    {
      ArgumentUtility.CheckNotNull("collectionManager", collectionManager);
      ArgumentUtility.CheckNotNull("lazyLoader", lazyLoader);
      ArgumentUtility.CheckNotNull("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull("transactionEventSink", transactionEventSink);
      ArgumentUtility.CheckNotNull("dataManagerFactory", dataManagerFactory);

      if (id.Definition.Cardinality != CardinalityType.Many)
        throw new ArgumentException("End point ID must refer to an end point with cardinality 'Many'.", "id");

      if (id.Definition.IsAnonymous)
        throw new ArgumentException("End point ID must not refer to an anonymous end point.", "id");

      Assertion.IsTrue(ID.Definition.IsVirtual);

      _hasBeenTouched = false;
      _collectionManager = collectionManager;
      _lazyLoader = lazyLoader;
      _endPointProvider = endPointProvider;
      _transactionEventSink = transactionEventSink;
      _dataManagerFactory = dataManagerFactory;

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

    public IObjectList<IDomainObject> GetCollectionWithOriginalData ()
    {
      return CreateCollection(GetOriginalData());
    }

    public ReadOnlyVirtualCollectionDataDecorator GetData ()
    {
      EnsureDataComplete();
      Assertion.IsNotNull(_dataManager);

      return new ReadOnlyVirtualCollectionDataDecorator(_dataManager.CollectionData);
    }

    public ReadOnlyVirtualCollectionDataDecorator GetOriginalData ()
    {
      EnsureDataComplete();
      Assertion.IsNotNull(_dataManager);

      return _dataManager.GetOriginalCollectionData();
    }

    public override bool IsDataComplete
    {
      get { return _dataManager != null; }
    }

    public bool CanBeCollected
    {
      get { return true; }
    }

    public bool CanBeMarkedIncomplete
    {
      get { return true; }
    }

    public override bool HasChanged
    {
      get
      {
        //TODO: RM-7294: merge ChangeTrackingVirtualCollectionDataDecorator into DataManager and make DataManager work for loaded and unloaded state
        return _addedDomainObjects.Count > 0 || _removedDomainObjects.Count > 0;
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
        _lazyLoader.LoadLazyCollectionEndPoint(ID);
        Assertion.IsNotNull(_dataManager, "LazyLoad did not complete the collection endpoint");
      }
    }

    public void MarkDataComplete (DomainObject[] items)
    {
      ArgumentUtility.CheckNotNull("items", items);

      if (_dataManager != null)
        throw new InvalidOperationException("The data is already complete.");

      if (s_log.IsInfoEnabled())
        s_log.InfoFormat("Virtual end-point '{0}' is transitioned to complete state.", ID);

      var dataManager = _dataManagerFactory.CreateEndPointDataManager(ID);

      _dataManager = dataManager;
    }

    public void MarkDataIncomplete ()
    {
      if (_dataManager == null)
        return;

      _transactionEventSink.RaiseRelationEndPointBecomingIncompleteEvent(ID);

      Assertion.DebugIsNotNull(_dataManager, "_dataManager has already been checked.");

      _dataManager = null;
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
        var message = string.Format(
            "Mandatory relation property '{0}' of domain object '{1}' contains no items.",
            Definition.PropertyName,
            ObjectID);
        throw new MandatoryRelationNotSetException(objectReference, Definition.PropertyName, message);
      }
    }

    public void RegisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);

      if (_dataManager != null)
      {
        _dataManager.RegisterOriginalOppositeEndPoint(oppositeEndPoint);
      }

      oppositeEndPoint.MarkSynchronized();
    }

    public void UnregisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);

      if (s_log.IsInfoEnabled())
      {
        s_log.InfoFormat(
            "RealObjectEndPoint '{0}' is unregistered from VirtualCollectionEndPoint '{1}'. The VirtualCollectionEndPoint is transitioned to incomplete state.",
            oppositeEndPoint.ID,
            ID);
      }
      MarkDataIncomplete();
    }

    public void RegisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);
    }

    public void UnregisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);
    }

    public override bool? IsSynchronized
    {
      get { return true; }
    }

    public override void Synchronize ()
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("End-point '{0}' is being synchronized.", ID);

      if (_dataManager != null)
      {
        //TODO: RM-7294: do we need to reset the CachedDomainObjects?
        //_dataManager.Synchronize()
      }
      else
      {
        EnsureDataComplete();
        Assertion.DebugIsNotNull(_dataManager, "EnsureDataComplete sets _dataManager.");
      }
    }

    public void SynchronizeOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);

      Assertion.IsNotNull(_dataManager, "Cannot synchronize an opposite end-point with a virtual end-point in incomplete state.");

      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("ObjectEndPoint '{0}' is being marked as synchronized.", oppositeEndPoint.ID);

      _dataManager.SynchronizeOppositeEndPoint(oppositeEndPoint);
      oppositeEndPoint.MarkSynchronized();
    }

    public override IDataManagementCommand CreateRemoveCommand (DomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull("removedRelatedObject", removedRelatedObject);

      IVirtualCollectionData virtualCollectionData;
      if (_dataManager == null)
      {
        virtualCollectionData = new IncompleteEndPointModificationVirtualCollectionData(ID);
      }
      else
      {
        virtualCollectionData = _dataManager.CollectionData;
      }

      //TODO: RM-7294: merge ChangeTrackingVirtualCollectionDataDecorator into DataManager and make DataManager work for loaded and unloaded state
      var changeTrackingVirtualCollectionData = new ChangeTrackingVirtualCollectionDataDecorator(
          virtualCollectionData,
          addedDomainObjects: _addedDomainObjects,
          removedDomainObjects: _removedDomainObjects);

      return new VirtualCollectionEndPointRemoveCommand(
          this,
          removedRelatedObject,
          changeTrackingVirtualCollectionData,
          _endPointProvider,
          _transactionEventSink);
    }

    public override IDataManagementCommand CreateDeleteCommand ()
    {
      EnsureDataComplete();
      Assertion.DebugIsNotNull(_dataManager, "EnsureDataComplete sets _dataManager.");

      var virtualCollectionData = _dataManager.CollectionData;

      //TODO: RM-7294: merge ChangeTrackingVirtualCollectionDataDecorator into DataManager and make DataManager work for loaded and unloaded state
      var changeTrackingVirtualCollectionData = new ChangeTrackingVirtualCollectionDataDecorator(
          virtualCollectionData,
          addedDomainObjects: _addedDomainObjects,
          removedDomainObjects: _removedDomainObjects);

      return new VirtualCollectionEndPointDeleteCommand(this, changeTrackingVirtualCollectionData, _transactionEventSink);
    }

    public virtual IDataManagementCommand CreateAddCommand (DomainObject addedRelatedObject)
    {
      ArgumentUtility.CheckNotNull("addedRelatedObject", addedRelatedObject);

      IVirtualCollectionData virtualCollectionData;
      if (_dataManager == null)
      {
        virtualCollectionData = new IncompleteEndPointModificationVirtualCollectionData(ID);
      }
      else
      {
        virtualCollectionData = _dataManager.CollectionData;
      }

      //TODO: RM-7294: merge ChangeTrackingVirtualCollectionDataDecorator into DataManager and make DataManager work for loaded and unloaded state
      var changeTrackingVirtualCollectionData = new ChangeTrackingVirtualCollectionDataDecorator(
          virtualCollectionData,
          addedDomainObjects: _addedDomainObjects,
          removedDomainObjects: _removedDomainObjects);

      return new VirtualCollectionEndPointAddCommand(
          this,
          addedRelatedObject,
          changeTrackingVirtualCollectionData,
          _endPointProvider,
          _transactionEventSink);
    }

    public override IEnumerable<RelationEndPointID> GetOppositeRelationEndPointIDs ()
    {
      var oppositeEndPointDefinition = Definition.GetOppositeEndPointDefinition();

      Assertion.IsFalse(oppositeEndPointDefinition.IsAnonymous);

      EnsureDataComplete();
      Assertion.IsNotNull(_dataManager);

      return _dataManager.CollectionData
          .Select(oppositeDomainObject => RelationEndPointID.Create(oppositeDomainObject.ID, oppositeEndPointDefinition));
    }

    public override void SetDataFromSubTransaction (IRelationEndPoint source)
    {
      var sourceCollectionEndPoint = ArgumentUtility.CheckNotNullAndType<VirtualCollectionEndPoint>("source", source);
      if (Definition != sourceCollectionEndPoint.Definition)
      {
        var message = string.Format(
            "Cannot set this end point's value from '{0}'; the end points do not have the same end point definition.",
            source.ID);
        throw new ArgumentException(message, "source");
      }

      if (_dataManager != null)
      {
        var sourceDataManager = sourceCollectionEndPoint._dataManager;
        if (sourceDataManager != null)
        {
          _dataManager.SetDataFromSubTransaction(sourceDataManager, _endPointProvider);
        }
        else
        {
          // TODO: RM-7294: Consider reworking IVirtualCollectionEndPointDataManager to replace named operations with a general purpose Reset-api.
          if (_dataManager.CollectionData is VirtualCollectionData virtualCollectionData)
          {
            virtualCollectionData.ResetCachedDomainObjects();
          }
          else
          {
            //TODO: RM-7294: Hack for supporting unloaded collections. Will be reworked with changes to IVirtualCollectionEndPointDataManager.
            throw new InvalidOperationException($"VirtualCollectionEndPoint can only handle collection data of type '{typeof(VirtualCollectionData)}' but collection data type was '{_dataManager.CollectionData.GetType()}'.");
          }
        }
      }

      // TODO: RM-7294: this.HasChanged can be removed, it's redundant since _dataManager.SetDataFromSubTransaction doesn't affect this.HasChanged.
      if (sourceCollectionEndPoint.HasBeenTouched || HasChanged)
        Touch();
    }

    private IObjectList<IDomainObject> CreateCollection (IVirtualCollectionData dataStrategy)
    {
      return ObjectListFactory.Create(dataStrategy);
    }

    #region Serialization

    protected VirtualCollectionEndPoint (FlattenedDeserializationInfo info)
        : base(info)
    {
      _collectionManager = info.GetValueForHandle<IVirtualCollectionEndPointCollectionManager>();
      _lazyLoader = info.GetValueForHandle<ILazyLoader>();
      _endPointProvider = info.GetValueForHandle<IRelationEndPointProvider>();
      _transactionEventSink = info.GetValueForHandle<IClientTransactionEventSink>();
      _dataManagerFactory = info.GetValueForHandle<IVirtualCollectionEndPointDataManagerFactory>();

      _dataManager = info.GetValueForHandle<IVirtualCollectionEndPointDataManager>();
      _hasBeenTouched = info.GetBoolValue();

      _addedDomainObjects = new HashSet<ObjectID>();
      info.FillCollection(_addedDomainObjects);

      _removedDomainObjects = new HashSet<ObjectID>();
      info.FillCollection(_removedDomainObjects);
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle(_collectionManager);
      info.AddHandle(_lazyLoader);
      info.AddHandle(_endPointProvider);
      info.AddHandle(_transactionEventSink);
      info.AddHandle(_dataManagerFactory);

      info.AddHandle(_dataManager);
      info.AddBoolValue(_hasBeenTouched);

      info.AddCollection(_addedDomainObjects);
      info.AddCollection(_removedDomainObjects);
    }

    #endregion
  }
}
