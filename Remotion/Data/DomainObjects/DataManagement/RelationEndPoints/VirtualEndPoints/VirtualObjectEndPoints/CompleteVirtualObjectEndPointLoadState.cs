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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints
{
  /// <summary>
  /// Represents the state of a <see cref="VirtualObjectEndPoint"/> where all of its data is available (ie., the end-point has been (lazily) loaded).
  /// </summary>
  public class CompleteVirtualObjectEndPointLoadState
      : CompleteVirtualEndPointLoadStateBase<IVirtualObjectEndPoint, DomainObject?, IVirtualObjectEndPointDataManager>, IVirtualObjectEndPointLoadState
  {
    public CompleteVirtualObjectEndPointLoadState (
        IVirtualObjectEndPointDataManager dataManager,
        IRelationEndPointProvider endPointProvider,
        IClientTransactionEventSink transactionEventSink)
        : base(dataManager, endPointProvider, transactionEventSink)
    {
    }

    public override DomainObject? GetData (IVirtualObjectEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);

      return DataManager.CurrentOppositeObject;
    }

    public override DomainObject? GetOriginalData (IVirtualObjectEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);

      return DataManager.OriginalOppositeObject;
    }

    public override void SetDataFromSubTransaction (
        IVirtualObjectEndPoint endPoint,
        IVirtualEndPointLoadState<IVirtualObjectEndPoint, DomainObject?, IVirtualObjectEndPointDataManager> sourceLoadState)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);
      var sourceCompleteLoadState = ArgumentUtility.CheckNotNullAndType<CompleteVirtualObjectEndPointLoadState>("sourceLoadState", sourceLoadState);

      DataManager.SetDataFromSubTransaction(sourceCompleteLoadState.DataManager, EndPointProvider);
    }

    public void MarkDataComplete (IVirtualObjectEndPoint endPoint, DomainObject? item, Action<IVirtualObjectEndPointDataManager> stateSetter)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);
      ArgumentUtility.CheckNotNull("stateSetter", stateSetter);

      var items = item == null ? Array.Empty<DomainObject>() : EnumerableUtility.Singleton(item);
      MarkDataComplete(endPoint, items, stateSetter);
    }

    public override void SynchronizeOppositeEndPoint (IVirtualObjectEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);

      if (DataManager.OriginalOppositeEndPoint != null)
      {
        var message = string.Format(
            "The object end-point '{0}' cannot be synchronized with the virtual object end-point '{1}' because the virtual relation property already "
            + "refers to another object ('{2}'). To synchronize '{0}', use UnloadService to unload either object '{2}' or the virtual object "
            + "end-point '{1}'.",
            oppositeEndPoint.ID,
            DataManager.EndPointID,
            DataManager.OriginalOppositeEndPoint.ObjectID);
        throw new InvalidOperationException(message);
      }

      base.SynchronizeOppositeEndPoint(endPoint, oppositeEndPoint);
    }

    public IDataManagementCommand CreateSetCommand (IVirtualObjectEndPoint virtualObjectEndPoint, DomainObject? newRelatedObject)
    {
      ArgumentUtility.CheckNotNull("virtualObjectEndPoint", virtualObjectEndPoint);

      var oldRelatedObject = DataManager.CurrentOppositeObject;
      if (DataManager.OriginalItemWithoutEndPoint != null)
      {
        var message = string.Format(
            "The virtual property '{0}' of object '{1}' cannot be set because the property is "
            + "out of sync with the opposite object property '{2}'. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{0}' property.",
            DataManager.EndPointID.Definition.PropertyName,
            DataManager.EndPointID.ObjectID,
            DataManager.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName);
        throw new InvalidOperationException(message);
      }

      if (oldRelatedObject == newRelatedObject)
      {
        return new ObjectEndPointSetSameCommand(virtualObjectEndPoint, TransactionEventSink);
      }
      else
      {
        if (newRelatedObject != null)
          CheckAddedObject(newRelatedObject);

        return new ObjectEndPointSetOneOneCommand(
            virtualObjectEndPoint, newRelatedObject, domainObject => DataManager.CurrentOppositeObject = domainObject, TransactionEventSink);
      }
    }

    public IDataManagementCommand CreateDeleteCommand (IVirtualObjectEndPoint virtualObjectEndPoint)
    {
      ArgumentUtility.CheckNotNull("virtualObjectEndPoint", virtualObjectEndPoint);

      if (UnsynchronizedOppositeEndPoints.Count != 0)
      {
        var message = string.Format(
            "The domain object '{0}' cannot be deleted because the opposite object property '{2}' of domain object '{3}' is out of sync with the "
            + "virtual property '{1}'. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{2}' property.",
            DataManager.EndPointID.ObjectID,
            DataManager.EndPointID.Definition.PropertyName,
            DataManager.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName,
            UnsynchronizedOppositeEndPoints.First().ObjectID);
        throw new InvalidOperationException(message);
      }

      if (!IsSynchronized(virtualObjectEndPoint))
      {
        Assertion.DebugIsNotNull(
            DataManager.OriginalItemWithoutEndPoint,
            "DataManager.OriginalItemWithoutEndPoint != null when IsSynchronized (virtualObjectEndPoint) == false");

        var message = string.Format(
            "The domain object '{0}' cannot be deleted because its virtual property '{1}' is out of sync with "
            + "the opposite object property '{2}' of domain object '{3}'. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{1}' property.",
            DataManager.EndPointID.ObjectID,
            DataManager.EndPointID.Definition.PropertyName,
            DataManager.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName,
            DataManager.OriginalItemWithoutEndPoint.ID);
        throw new InvalidOperationException(message);
      }

      return new ObjectEndPointDeleteCommand(virtualObjectEndPoint, () => DataManager.CurrentOppositeObject = null, TransactionEventSink);
    }

    protected override IEnumerable<IRealObjectEndPoint> GetOriginalOppositeEndPoints ()
    {
      return DataManager.OriginalOppositeEndPoint == null ? new IRealObjectEndPoint[0] : new[] { DataManager.OriginalOppositeEndPoint };
    }

    protected override IEnumerable<DomainObject> GetOriginalItemsWithoutEndPoints ()
    {
      return DataManager.OriginalItemWithoutEndPoint == null ? new DomainObject[0] : new[] { DataManager.OriginalItemWithoutEndPoint };
    }

    private void CheckAddedObject (DomainObject domainObject)
    {
      if (ContainsUnsynchronizedOppositeEndPoint(domainObject.ID))
      {
        var message = string.Format(
            "The domain object with ID '{0}' cannot be set into the virtual property '{1}' of object '{2}' because its object property "
            + "'{3}' is out of sync with the virtual property. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{3}' property.",
            domainObject.ID,
            DataManager.EndPointID.Definition.PropertyName,
            DataManager.EndPointID.ObjectID,
            DataManager.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName);
        throw new InvalidOperationException(message);
      }
    }
  }
}
