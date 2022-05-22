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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.RealObjectEndPoints
{
  /// <summary>
  /// Represents the state of an <see cref="IObjectEndPoint"/> that is synchronized with the opposite <see cref="IRelationEndPoint"/>.
  /// </summary>
  public class SynchronizedRealObjectEndPointSyncState : IRealObjectEndPointSyncState
  {
    private readonly IRelationEndPointProvider _endPointProvider;
    private readonly IClientTransactionEventSink _transactionEventSink;

    public SynchronizedRealObjectEndPointSyncState (IRelationEndPointProvider endPointProvider, IClientTransactionEventSink transactionEventSink)
    {
      ArgumentUtility.CheckNotNull("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull("transactionEventSink", transactionEventSink);

      _endPointProvider = endPointProvider;
      _transactionEventSink = transactionEventSink;
    }

    public IRelationEndPointProvider EndPointProvider
    {
      get { return _endPointProvider; }
    }

    public IClientTransactionEventSink TransactionEventSink
    {
      get { return _transactionEventSink; }
    }

    public bool? IsSynchronized (IRealObjectEndPoint endPoint)
    {
      return true;
    }

    public void Synchronize (IRealObjectEndPoint endPoint, IVirtualEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);

      // nothing to do here - the end-point is already syncrhonized
    }

    public IDataManagementCommand CreateDeleteCommand (IRealObjectEndPoint endPoint, Action oppositeObjectNullSetter)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);
      ArgumentUtility.CheckNotNull("oppositeObjectNullSetter", oppositeObjectNullSetter);

      var oppositeEndPointDefinition = endPoint.Definition.GetOppositeEndPointDefinition();

      var objectEndPointDeleteCommand = new ObjectEndPointDeleteCommand(endPoint, oppositeObjectNullSetter, _transactionEventSink);

      if (!oppositeEndPointDefinition.IsAnonymous && oppositeEndPointDefinition.IsVirtual)
      {
        var oldRelatedEndPoint = GetOppositeEndPoint(endPoint, endPoint.OppositeObjectID);
        var newRelatedEndPoint = GetOppositeEndPoint(endPoint, null);
        return new RealObjectEndPointRegistrationCommandDecorator(objectEndPointDeleteCommand, endPoint, oldRelatedEndPoint, newRelatedEndPoint);
      }
      else
      {
        return objectEndPointDeleteCommand;
      }
    }

    public IDataManagementCommand CreateSetCommand (IRealObjectEndPoint endPoint, IDomainObject? newRelatedObject, Action<IDomainObject?> oppositeObjectSetter)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);
      ArgumentUtility.CheckNotNull("oppositeObjectSetter", oppositeObjectSetter);

      var oppositeEndPointDefinition = endPoint.Definition.GetOppositeEndPointDefinition();

      var newRelatedObjectID = newRelatedObject.GetSafeID();
      if (endPoint.OppositeObjectID == newRelatedObjectID)
        return new ObjectEndPointSetSameCommand(endPoint, _transactionEventSink);
      else if (oppositeEndPointDefinition.IsAnonymous)
        return new ObjectEndPointSetUnidirectionalCommand(endPoint, newRelatedObject, oppositeObjectSetter, _transactionEventSink);
      else
      {
        var setCommand =
            oppositeEndPointDefinition.Cardinality == CardinalityType.One
                ? (IDataManagementCommand)new ObjectEndPointSetOneOneCommand(endPoint, newRelatedObject, oppositeObjectSetter, _transactionEventSink)
                : new ObjectEndPointSetOneManyCommand(endPoint, newRelatedObject, oppositeObjectSetter, _endPointProvider, _transactionEventSink);

        var oldRelatedEndPoint = GetOppositeEndPoint(endPoint, endPoint.OppositeObjectID);
        var newRelatedEndPoint = GetOppositeEndPoint(endPoint, newRelatedObjectID);
        return new RealObjectEndPointRegistrationCommandDecorator(setCommand, endPoint, oldRelatedEndPoint, newRelatedEndPoint);
      }
    }

    private IVirtualEndPoint GetOppositeEndPoint (IRealObjectEndPoint sourceEndPoint, ObjectID? oppositeObjectID)
    {
      var newOppositeID = RelationEndPointID.CreateOpposite(sourceEndPoint.Definition, oppositeObjectID);
      return (IVirtualEndPoint)_endPointProvider.GetRelationEndPointWithLazyLoad(newOppositeID);
    }

    #region Serialization

    public SynchronizedRealObjectEndPointSyncState (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull("info", info);

      _endPointProvider = info.GetValueForHandle<IRelationEndPointProvider>();
      _transactionEventSink = info.GetValueForHandle<IClientTransactionEventSink>();
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull("info", info);

      info.AddHandle(_endPointProvider);
      info.AddHandle(_transactionEventSink);
    }

    #endregion
  }
}
