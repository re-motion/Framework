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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Represents the state of a <see cref="VirtualCollectionEndPoint"/> where all of its data is available (ie., the end-point has been (lazily) loaded).
  /// </summary>
  public class CompleteVirtualCollectionEndPointLoadState
      : CompleteVirtualEndPointLoadStateBase<IVirtualCollectionEndPoint, ReadOnlyVirtualCollectionDataDecorator, IVirtualCollectionEndPointDataManager>,
        IVirtualCollectionEndPointLoadState
  {
    public CompleteVirtualCollectionEndPointLoadState (
        IVirtualCollectionEndPointDataManager dataManager,
        IRelationEndPointProvider endPointProvider,
        IClientTransactionEventSink transactionEventSink)
        : base (dataManager, endPointProvider, transactionEventSink)
    {
    }

    public override ReadOnlyVirtualCollectionDataDecorator GetData (IVirtualCollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      return new ReadOnlyVirtualCollectionDataDecorator (DataManager.CollectionData);
    }

    public override ReadOnlyVirtualCollectionDataDecorator GetOriginalData (IVirtualCollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      return DataManager.OriginalCollectionData;
    }

    public override void SetDataFromSubTransaction (
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

    public new void MarkDataComplete (IVirtualCollectionEndPoint collectionEndPoint, IEnumerable<DomainObject> items, Action<IVirtualCollectionEndPointDataManager> stateSetter)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("items", items);
      ArgumentUtility.CheckNotNull ("stateSetter", stateSetter);

      base.MarkDataComplete (collectionEndPoint, items, stateSetter);
    }

    public void SortCurrentData (IVirtualCollectionEndPoint collectionEndPoint, Comparison<DomainObject> comparison)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("comparison", comparison);

      DataManager.SortCurrentData (comparison);

      RaiseReplaceDataEvent (collectionEndPoint);
    }

    public override void Synchronize (IVirtualCollectionEndPoint endPoint)
    {
      base.Synchronize (endPoint);

      RaiseReplaceDataEvent (endPoint);
    }

    public override void SynchronizeOppositeEndPoint (IVirtualCollectionEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      base.SynchronizeOppositeEndPoint (endPoint, oppositeEndPoint);

      RaiseReplaceDataEvent (endPoint);
    }

    public override void Rollback (IVirtualCollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      
      base.Rollback (endPoint);

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

    protected override IEnumerable<IRealObjectEndPoint> GetOriginalOppositeEndPoints ()
    {
      return DataManager.OriginalOppositeEndPoints;
    }

    protected override IEnumerable<DomainObject> GetOriginalItemsWithoutEndPoints ()
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


    #region Serialization

    public CompleteVirtualCollectionEndPointLoadState (FlattenedDeserializationInfo info)
        : base(info)
    {
    }

    #endregion
  }
}