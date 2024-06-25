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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Represents a collection-valued relation end-point in the <see cref="RelationEndPointManager"/>.
  /// </summary>
  public class DomainObjectCollectionEndPoint : RelationEndPoint, IDomainObjectCollectionEndPoint
  {
    public class EndPointLoader : IncompleteDomainObjectCollectionEndPointLoadState.IEndPointLoader
    {
      private readonly ILazyLoader _lazyLoader;

      public EndPointLoader (ILazyLoader lazyLoader)
      {
        ArgumentUtility.CheckNotNull("lazyLoader", lazyLoader);
        _lazyLoader = lazyLoader;
      }

      public ILazyLoader LazyLoader
      {
        get { return _lazyLoader; }
      }

      public IDomainObjectCollectionEndPointLoadState LoadEndPointAndGetNewState (IDomainObjectCollectionEndPoint endPoint)
      {
        var collectionEndPoint = ArgumentUtility.CheckNotNullAndType<DomainObjectCollectionEndPoint>("endPoint", endPoint);
        _lazyLoader.LoadLazyCollectionEndPoint(endPoint.ID);
        return collectionEndPoint._loadState;
      }
    }

    private readonly IDomainObjectCollectionEndPointCollectionManager _collectionManager;
    private readonly ILazyLoader _lazyLoader;
    private readonly IRelationEndPointProvider _endPointProvider;
    private readonly IClientTransactionEventSink _transactionEventSink;
    private readonly IDomainObjectCollectionEndPointDataManagerFactory _dataManagerFactory;

    private IDomainObjectCollectionEndPointLoadState _loadState; // keeps track of whether this end-point has been completely loaded or not

    private bool _hasBeenTouched;

    public DomainObjectCollectionEndPoint (
        ClientTransaction clientTransaction,
        RelationEndPointID id,
        IDomainObjectCollectionEndPointCollectionManager collectionManager,
        ILazyLoader lazyLoader,
        IRelationEndPointProvider endPointProvider,
        IClientTransactionEventSink transactionEventSink,
        IDomainObjectCollectionEndPointDataManagerFactory dataManagerFactory)
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

      SetIncompleteLoadState();
    }

    public IDomainObjectCollectionEndPointCollectionManager CollectionManager
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

    public IDomainObjectCollectionEndPointDataManagerFactory DataManagerFactory
    {
      get { return _dataManagerFactory; }
    }

    public DomainObjectCollection Collection
    {
      get { return _collectionManager.GetCurrentCollectionReference(); }
    }

    public DomainObjectCollection OriginalCollection
    {
      get { return _collectionManager.GetOriginalCollectionReference(); }
    }

    public IDomainObjectCollectionEventRaiser GetCollectionEventRaiser ()
    {
      return Collection;
    }

    public DomainObjectCollection GetCollectionWithOriginalData ()
    {
      return CreateCollection(_loadState.GetOriginalData(this));
    }

    public ReadOnlyDomainObjectCollectionDataDecorator GetData ()
    {
      return _loadState.GetData(this);
    }

    public ReadOnlyDomainObjectCollectionDataDecorator GetOriginalData ()
    {
      return _loadState.GetOriginalData(this);
    }

    public override bool IsDataComplete
    {
      get { return _loadState.IsDataComplete(); }
    }

    public bool CanBeCollected
    {
      get { return !_collectionManager.HasCollectionReferenceChanged() && _loadState.CanEndPointBeCollected(this); }
    }

    public bool CanBeMarkedIncomplete
    {
      get { return !_collectionManager.HasCollectionReferenceChanged() && _loadState.CanDataBeMarkedIncomplete(this); }
    }

    public override bool HasChanged
    {
      get { return _collectionManager.HasCollectionReferenceChanged() || _loadState.HasChanged(); }
    }

    public bool? HasChangedFast
    {
      get
      {
        if (_collectionManager.HasCollectionReferenceChanged())
          return true;
        return _loadState.HasChangedFast();
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
      ArgumentUtility.CheckNotNull("items", items);
      _loadState.MarkDataComplete(this, items, SetCompleteLoadState);
    }

    public void MarkDataIncomplete ()
    {
      _loadState.MarkDataIncomplete(this, SetIncompleteLoadState);
    }

    public override void Touch ()
    {
      _hasBeenTouched = true;
    }

    public override void Commit ()
    {
      if (HasChanged)
      {
        _collectionManager.CommitCollectionReference();
        _loadState.Commit(this);
      }

      _hasBeenTouched = false;
    }

    public override void Rollback ()
    {
      if (HasChanged)
      {
        _collectionManager.RollbackCollectionReference();
        _loadState.Rollback(this);
      }

      _hasBeenTouched = false;
    }

    public override void ValidateMandatory ()
    {
      // In order to perform the mandatory check, we need to load data. It's up to the caller to decide whether an incomplete end-point should be 
      // checked. (DataManager will not check incomplete end-points, as it also ignores not-yet-loaded end-points.)

      if (GetData().Count == 0)
      {
        var objectReference = GetDomainObjectReference();
        var message = String.Format(
            "Mandatory relation property '{0}' of domain object '{1}' contains no items.",
            Definition.PropertyName,
            ObjectID);
        throw new MandatoryRelationNotSetException(objectReference, Definition.PropertyName, message);
      }
    }

    public void SortCurrentData (Comparison<DomainObject> comparison)
    {
      ArgumentUtility.CheckNotNull("comparison", comparison);

      _loadState.SortCurrentData(this, comparison);
      Touch();
    }

    public void RegisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);

      _loadState.RegisterOriginalOppositeEndPoint(this, oppositeEndPoint);
    }

    public void UnregisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);

      _loadState.UnregisterOriginalOppositeEndPoint(this, oppositeEndPoint);
    }

    public void RegisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);

      _loadState.RegisterCurrentOppositeEndPoint(this, oppositeEndPoint);
    }

    public void UnregisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);

      _loadState.UnregisterCurrentOppositeEndPoint(this, oppositeEndPoint);
    }

    public override bool? IsSynchronized
    {
      get { return _loadState.IsSynchronized(this); }
    }

    public override void Synchronize ()
    {
      _loadState.Synchronize(this);
    }

    public void SynchronizeOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);

      _loadState.SynchronizeOppositeEndPoint(this, oppositeEndPoint);
    }

    public IDataManagementCommand CreateSetCollectionCommand (DomainObjectCollection newCollection)
    {
      ArgumentUtility.CheckNotNull("newCollection", newCollection);

      var command = _loadState.CreateSetCollectionCommand(this, newCollection, _collectionManager);
      return command;
    }

    public override IDataManagementCommand CreateRemoveCommand (DomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull("removedRelatedObject", removedRelatedObject);

      var command = _loadState.CreateRemoveCommand(this, removedRelatedObject);
      return command;
    }

    public override IDataManagementCommand CreateDeleteCommand ()
    {
      var command = _loadState.CreateDeleteCommand(this);
      return command;
    }

    public virtual IDataManagementCommand CreateInsertCommand (DomainObject insertedRelatedObject, int index)
    {
      ArgumentUtility.CheckNotNull("insertedRelatedObject", insertedRelatedObject);
      var command = _loadState.CreateInsertCommand(this, insertedRelatedObject, index);
      return command;
    }

    public virtual IDataManagementCommand CreateAddCommand (DomainObject addedRelatedObject)
    {
      ArgumentUtility.CheckNotNull("addedRelatedObject", addedRelatedObject);
      var command = _loadState.CreateAddCommand(this, addedRelatedObject);
      return command;
    }

    public virtual IDataManagementCommand CreateReplaceCommand (int index, DomainObject replacementObject)
    {
      ArgumentUtility.CheckNotNull("replacementObject", replacementObject);
      var command = _loadState.CreateReplaceCommand(this, index, replacementObject);
      return command;
    }

    public override IEnumerable<RelationEndPointID> GetOppositeRelationEndPointIDs ()
    {
      var oppositeEndPointDefinition = Definition.GetOppositeEndPointDefinition();

      Assertion.IsFalse(oppositeEndPointDefinition.IsAnonymous);

      return from oppositeDomainObject in _loadState.GetData(this)
             select RelationEndPointID.Create(oppositeDomainObject.ID, oppositeEndPointDefinition);
    }

    public override void SetDataFromSubTransaction (IRelationEndPoint source)
    {
      var sourceCollectionEndPoint = ArgumentUtility.CheckNotNullAndType<DomainObjectCollectionEndPoint>("source", source);
      if (Definition != sourceCollectionEndPoint.Definition)
      {
        var message = string.Format(
            "Cannot set this end point's value from '{0}'; the end points do not have the same end point definition.",
            source.ID);
        throw new ArgumentException(message, "source");
      }

      _loadState.SetDataFromSubTransaction(this, sourceCollectionEndPoint._loadState);

      if (sourceCollectionEndPoint.HasBeenTouched || HasChanged)
        Touch();
    }

    [MemberNotNull(nameof(_loadState))]
    private void SetCompleteLoadState (IDomainObjectCollectionEndPointDataManager dataManager)
    {
      _loadState = new CompleteDomainObjectCollectionEndPointLoadState(dataManager, _endPointProvider, _transactionEventSink);
    }

    [MemberNotNull(nameof(_loadState))]
    private void SetIncompleteLoadState ()
    {
      var loader = new EndPointLoader(_lazyLoader);
      _loadState = new IncompleteDomainObjectCollectionEndPointLoadState(loader, _dataManagerFactory);
    }

    private DomainObjectCollection CreateCollection (IDomainObjectCollectionData dataStrategy)
    {
      Assertion.DebugAssert(!Definition.IsAnonymous, "!Definition.IsAnonymous");
      return DomainObjectCollectionFactory.Instance.CreateCollection(Definition.PropertyInfo.PropertyType, dataStrategy);
    }
  }
}
