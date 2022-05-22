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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Represents the state of a <see cref="DomainObjectCollectionEndPoint"/> where all of its data is available (ie., the end-point has been (lazily) loaded).
  /// </summary>
  public class CompleteDomainObjectCollectionEndPointLoadState
      : CompleteVirtualEndPointLoadStateBase<IDomainObjectCollectionEndPoint, ReadOnlyDomainObjectCollectionDataDecorator, IDomainObjectCollectionEndPointDataManager>,
        IDomainObjectCollectionEndPointLoadState
  {
    public CompleteDomainObjectCollectionEndPointLoadState (
        IDomainObjectCollectionEndPointDataManager dataManager,
        IRelationEndPointProvider endPointProvider,
        IClientTransactionEventSink transactionEventSink)
        : base(dataManager, endPointProvider, transactionEventSink)
    {
    }

    public override ReadOnlyDomainObjectCollectionDataDecorator GetData (IDomainObjectCollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull("collectionEndPoint", collectionEndPoint);
      return new ReadOnlyDomainObjectCollectionDataDecorator(DataManager.CollectionData);
    }

    public override ReadOnlyDomainObjectCollectionDataDecorator GetOriginalData (IDomainObjectCollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull("collectionEndPoint", collectionEndPoint);
      return DataManager.OriginalCollectionData;
    }

    public override void SetDataFromSubTransaction (
        IDomainObjectCollectionEndPoint collectionEndPoint,
        IVirtualEndPointLoadState<IDomainObjectCollectionEndPoint, ReadOnlyDomainObjectCollectionDataDecorator, IDomainObjectCollectionEndPointDataManager> sourceLoadState)
    {
      ArgumentUtility.CheckNotNull("collectionEndPoint", collectionEndPoint);
      var sourceCompleteLoadState = ArgumentUtility.CheckNotNullAndType<CompleteDomainObjectCollectionEndPointLoadState>("sourceLoadState", sourceLoadState);

      DataManager.SetDataFromSubTransaction(sourceCompleteLoadState.DataManager, EndPointProvider);

      RaiseReplaceDataEvent(collectionEndPoint);
    }

    public bool? HasChangedFast ()
    {
      return DataManager.HasDataChangedFast();
    }

    public new void MarkDataComplete (
        IDomainObjectCollectionEndPoint collectionEndPoint,
        IEnumerable<IDomainObject> items,
        Action<IDomainObjectCollectionEndPointDataManager> stateSetter)
    {
      ArgumentUtility.CheckNotNull("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull("items", items);
      ArgumentUtility.CheckNotNull("stateSetter", stateSetter);

      base.MarkDataComplete(collectionEndPoint, items, stateSetter);
    }

    public void SortCurrentData (IDomainObjectCollectionEndPoint collectionEndPoint, Comparison<IDomainObject> comparison)
    {
      ArgumentUtility.CheckNotNull("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull("comparison", comparison);

      DataManager.SortCurrentData(comparison);

      RaiseReplaceDataEvent(collectionEndPoint);
    }

    public override void Synchronize (IDomainObjectCollectionEndPoint endPoint)
    {
      base.Synchronize(endPoint);

      RaiseReplaceDataEvent(endPoint);
    }

    public override void SynchronizeOppositeEndPoint (IDomainObjectCollectionEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      base.SynchronizeOppositeEndPoint(endPoint, oppositeEndPoint);

      RaiseReplaceDataEvent(endPoint);
    }

    public override void Rollback (IDomainObjectCollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);

      base.Rollback(endPoint);

      RaiseReplaceDataEvent(endPoint);
    }

    public IDataManagementCommand CreateSetCollectionCommand (
        IDomainObjectCollectionEndPoint collectionEndPoint,
        DomainObjectCollection newCollection,
        IDomainObjectCollectionEndPointCollectionManager collectionEndPointCollectionManager)
    {
      ArgumentUtility.CheckNotNull("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull("newCollection", newCollection);
      ArgumentUtility.CheckNotNull("collectionEndPointCollectionManager", collectionEndPointCollectionManager);

      if (UnsynchronizedOppositeEndPoints.Count != 0)
      {
        var message = string.Format(
            "The collection of relation property '{0}' of domain object '{1}' cannot be replaced because the opposite object property '{2}' of domain "
            + "object '{3}' is out of sync. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{2}' property.",
            DataManager.EndPointID.Definition.PropertyName,
            DataManager.EndPointID.ObjectID,
            DataManager.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName,
            UnsynchronizedOppositeEndPoints.First().ObjectID);
        throw new InvalidOperationException(message);
      }

      if (!IsSynchronized(collectionEndPoint))
      {
        var message = string.Format(
            "The collection of relation property '{0}' of domain object '{1}' cannot be replaced because the relation property is out of sync with "
            + "the opposite object property '{2}' of domain object '{3}'. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{0}' property.",
            DataManager.EndPointID.Definition.PropertyName,
            DataManager.EndPointID.ObjectID,
            DataManager.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName,
            DataManager.OriginalItemsWithoutEndPoints.First().ID);
        throw new InvalidOperationException(message);
      }

      return new DomainObjectCollectionEndPointSetCollectionCommand(
          collectionEndPoint, newCollection, DataManager.CollectionData, collectionEndPointCollectionManager, TransactionEventSink);
    }

    public IDataManagementCommand CreateRemoveCommand (IDomainObjectCollectionEndPoint collectionEndPoint, IDomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull("removedRelatedObject", removedRelatedObject);

      CheckRemovedObject(removedRelatedObject);
      return new DomainObjectCollectionEndPointRemoveCommand(
          collectionEndPoint, removedRelatedObject, DataManager.CollectionData, EndPointProvider, TransactionEventSink);
    }

    public IDataManagementCommand CreateDeleteCommand (IDomainObjectCollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull("collectionEndPoint", collectionEndPoint);

      if (UnsynchronizedOppositeEndPoints.Count != 0)
      {
        var message = string.Format(
            "The domain object '{0}' cannot be deleted because the opposite object property '{2}' of domain object '{3}' is out of sync with the "
            + "collection property '{1}'. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{2}' property.",
            DataManager.EndPointID.ObjectID,
            DataManager.EndPointID.Definition.PropertyName,
            DataManager.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName,
            UnsynchronizedOppositeEndPoints.First().ObjectID);
        throw new InvalidOperationException(message);
      }

      if (!IsSynchronized(collectionEndPoint))
      {
        var message = string.Format(
            "The domain object '{0}' cannot be deleted because its collection property '{1}' is out of sync with "
            + "the opposite object property '{2}' of domain object '{3}'. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{1}' property.",
            DataManager.EndPointID.ObjectID,
            DataManager.EndPointID.Definition.PropertyName,
            DataManager.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName,
            DataManager.OriginalItemsWithoutEndPoints.First().ID);
        throw new InvalidOperationException(message);
      }

      return new DomainObjectCollectionEndPointDeleteCommand(collectionEndPoint, DataManager.CollectionData, TransactionEventSink);
    }

    public IDataManagementCommand CreateInsertCommand (IDomainObjectCollectionEndPoint collectionEndPoint, IDomainObject insertedRelatedObject, int index)
    {
      ArgumentUtility.CheckNotNull("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull("insertedRelatedObject", insertedRelatedObject);

      CheckAddedObject(insertedRelatedObject);
      return new DomainObjectCollectionEndPointInsertCommand(
          collectionEndPoint,
          index,
          insertedRelatedObject,
          DataManager.CollectionData,
          EndPointProvider,
          TransactionEventSink);
    }

    public IDataManagementCommand CreateAddCommand (IDomainObjectCollectionEndPoint collectionEndPoint, IDomainObject addedRelatedObject)
    {
      ArgumentUtility.CheckNotNull("collectionEndPoint", collectionEndPoint);

      return CreateInsertCommand(collectionEndPoint, addedRelatedObject, DataManager.CollectionData.Count);
    }

    public IDataManagementCommand CreateReplaceCommand (IDomainObjectCollectionEndPoint collectionEndPoint, int index, IDomainObject replacementObject)
    {
      ArgumentUtility.CheckNotNull("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull("replacementObject", replacementObject);

      CheckAddedObject(replacementObject);
      CheckRemovedObject(DataManager.CollectionData.GetObject(index));

      var replacedObject = DataManager.CollectionData.GetObject(index);
      if (replacedObject == replacementObject)
        return new DomainObjectCollectionEndPointReplaceSameCommand(collectionEndPoint, replacedObject, TransactionEventSink);
      else
        return new DomainObjectCollectionEndPointReplaceCommand(
            collectionEndPoint, replacedObject, index, replacementObject, DataManager.CollectionData, TransactionEventSink);
    }

    protected override IEnumerable<IRealObjectEndPoint> GetOriginalOppositeEndPoints ()
    {
      return DataManager.OriginalOppositeEndPoints;
    }

    protected override IEnumerable<IDomainObject> GetOriginalItemsWithoutEndPoints ()
    {
      return DataManager.OriginalItemsWithoutEndPoints;
    }

    private void CheckAddedObject (IDomainObject domainObject)
    {
      if (ContainsUnsynchronizedOppositeEndPoint(domainObject.ID))
      {
        var message = string.Format(
            "The domain object with ID '{0}' cannot be added to collection property '{1}' of object '{2}' because its object property "
            + "'{3}' is out of sync with the collection property. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{3}' property.",
            domainObject.ID,
            DataManager.EndPointID.Definition.PropertyName,
            DataManager.EndPointID.ObjectID,
            DataManager.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName);
        throw new InvalidOperationException(message);
      }

      if (DataManager.ContainsOriginalItemWithoutEndPoint(domainObject))
      {
        var message = string.Format(
            "The domain object with ID '{0}' cannot be added to collection property '{1}' of object '{2}' because the property is "
            + "out of sync with the opposite object property '{3}'. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{1}' property.",
            domainObject.ID,
            DataManager.EndPointID.Definition.PropertyName,
            DataManager.EndPointID.ObjectID,
            DataManager.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName);
        throw new InvalidOperationException(message);
      }
    }

    private void CheckRemovedObject (IDomainObject domainObject)
    {
      if (ContainsUnsynchronizedOppositeEndPoint(domainObject.ID))
      {
        var message = string.Format(
            "The domain object with ID '{0}' cannot be replaced or removed from collection property '{1}' of object '{2}' because its object property "
            + "'{3}' is out of sync with the collection property. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{3}' property.",
            domainObject.ID,
            DataManager.EndPointID.Definition.PropertyName,
            DataManager.EndPointID.ObjectID,
            DataManager.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName);
        throw new InvalidOperationException(message);
      }

      if (DataManager.ContainsOriginalItemWithoutEndPoint(domainObject))
      {
        var message = string.Format(
            "The domain object with ID '{0}' cannot be replaced or removed from collection property '{1}' of object '{2}' because the property is "
            + "out of sync with the opposite object property '{3}'. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{1}' property.",
            domainObject.ID,
            DataManager.EndPointID.Definition.PropertyName,
            DataManager.EndPointID.ObjectID,
            DataManager.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName);
        throw new InvalidOperationException(message);
      }
    }

    private void RaiseReplaceDataEvent (IDomainObjectCollectionEndPoint endPoint)
    {
      var eventRaiser = endPoint.GetCollectionEventRaiser();
      eventRaiser.WithinReplaceData();
    }


    #region Serialization

    public CompleteDomainObjectCollectionEndPointLoadState (FlattenedDeserializationInfo info)
        : base(info)
    {
    }

    #endregion
  }
}
