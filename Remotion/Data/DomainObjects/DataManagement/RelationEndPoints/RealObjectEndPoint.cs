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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.RealObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Represents an <see cref="ObjectEndPoint"/> that holds the foreign key in a relation. The foreign key is actually held by a 
  /// <see cref="PropertyValue"/> object, this end point implementation just delegates to the <see cref="PropertyValue"/>.
  /// </summary>
  public class RealObjectEndPoint : ObjectEndPoint, IRealObjectEndPoint
  {
    private readonly DataContainer _foreignKeyDataContainer;
    private readonly IRelationEndPointProvider _endPointProvider;
    private readonly IClientTransactionEventSink _transactionEventSink;
    private readonly PropertyDefinition _propertyDefinition;

    private IRealObjectEndPointSyncState _syncState; // keeps track of whether this end-point is synchronised with the opposite end point

    public RealObjectEndPoint (
        ClientTransaction clientTransaction,
        RelationEndPointID id,
        DataContainer foreignKeyDataContainer,
        IRelationEndPointProvider endPointProvider,
        IClientTransactionEventSink transactionEventSink)
      : base(
          ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction),
          ArgumentUtility.CheckNotNull("id", id))
    {
      ArgumentUtility.CheckNotNull("foreignKeyDataContainer", foreignKeyDataContainer);
      ArgumentUtility.CheckNotNull("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull("transactionEventSink", transactionEventSink);

      if (id.ObjectID == null)
        throw new ArgumentException("End point ID must have a non-null ObjectID.", "id");

      if (id.Definition.IsVirtual)
        throw new ArgumentException("End point ID must refer to a non-virtual end point.", "id");

      if (foreignKeyDataContainer.ID != id.ObjectID)
        throw new ArgumentException("The foreign key data container must be from the same object as the end point definition.", "foreignKeyDataContainer");

      var propertyDefinition = GetPropertyDefinition();

      _foreignKeyDataContainer = foreignKeyDataContainer;
      _endPointProvider = endPointProvider;
      _transactionEventSink = transactionEventSink;

      _propertyDefinition = propertyDefinition;
      _syncState = new UnknownRealObjectEndPointSyncState(_endPointProvider);
    }

    public DataContainer ForeignKeyDataContainer
    {
      get { return _foreignKeyDataContainer; }
    }

    public IRelationEndPointProvider EndPointProvider
    {
      get { return _endPointProvider; }
    }

    public IClientTransactionEventSink TransactionEventSink
    {
      get { return _transactionEventSink; }
    }

    public PropertyDefinition PropertyDefinition
    {
      get { return _propertyDefinition; }
    }

    public override ObjectID? OppositeObjectID
    {
      get { return (ObjectID?)ForeignKeyDataContainer.GetValueWithoutEvents(PropertyDefinition, ValueAccess.Current); }
    }

    public override ObjectID? OriginalOppositeObjectID
    {
      get { return (ObjectID?)ForeignKeyDataContainer.GetValueWithoutEvents(PropertyDefinition, ValueAccess.Original); }
    }

    public override bool HasChanged
    {
      get { return ForeignKeyDataContainer.HasValueChanged(PropertyDefinition); }
    }

    public override bool HasBeenTouched
    {
      get { return ForeignKeyDataContainer.HasValueBeenTouched(PropertyDefinition); }
    }

    public override bool IsDataComplete
    {
      get { return true; }
    }

    public override bool? IsSynchronized
    {
      get { return _syncState.IsSynchronized(this); }
    }

    public override IDomainObject? GetOppositeObject ()
    {
      if (OppositeObjectID == null)
        return null;

      return ClientTransaction.GetObjectReference(OppositeObjectID);
    }

    public override IDomainObject? GetOriginalOppositeObject ()
    {
      if (OriginalOppositeObjectID == null)
        return null;

      return ClientTransaction.GetObjectReference(OriginalOppositeObjectID);
    }

    public override void EnsureDataComplete ()
    {
      Assertion.IsTrue(IsDataComplete);
    }

    public override void Synchronize ()
    {
      var oppositeID = RelationEndPointID.CreateOpposite(Definition, OppositeObjectID);
      var oppositeEndPoint = (IVirtualEndPoint)_endPointProvider.GetRelationEndPointWithLazyLoad(oppositeID);
      _syncState.Synchronize(this, oppositeEndPoint);
    }

    public void MarkSynchronized ()
    {
      _syncState = new SynchronizedRealObjectEndPointSyncState(_endPointProvider, _transactionEventSink);
    }

    public void MarkUnsynchronized ()
    {
      _syncState = new UnsynchronizedRealObjectEndPointSyncState();
    }

    public void ResetSyncState ()
    {
      _syncState = new UnknownRealObjectEndPointSyncState(_endPointProvider);
    }

    public override IDataManagementCommand CreateDeleteCommand ()
    {
      return _syncState.CreateDeleteCommand(this, () => SetOppositeObjectID(null));
    }

    public override IDataManagementCommand CreateSetCommand (IDomainObject? newRelatedObject)
    {
      return _syncState.CreateSetCommand(this, newRelatedObject, domainObject => SetOppositeObjectID(domainObject.GetSafeID()));
    }

    public override void Touch ()
    {
      ForeignKeyDataContainer.TouchValue(PropertyDefinition);
      Assertion.IsTrue(HasBeenTouched);
    }

    public override void Commit ()
    {
      ForeignKeyDataContainer.CommitValue(PropertyDefinition);
      Assertion.IsFalse(HasBeenTouched);
      Assertion.IsFalse(HasChanged);
    }

    public override void Rollback ()
    {
      ForeignKeyDataContainer.RollbackValue(PropertyDefinition);
      Assertion.IsFalse(HasBeenTouched);
      Assertion.IsFalse(HasChanged);
    }

    protected override void SetOppositeObjectDataFromSubTransaction (IObjectEndPoint sourceObjectEndPoint)
    {
      var sourceAsRealObjectEndPoint = ArgumentUtility.CheckNotNullAndType<RealObjectEndPoint>("sourceObjectEndPoint", sourceObjectEndPoint);
      ForeignKeyDataContainer.SetPropertyValueFromSubTransaction(PropertyDefinition, sourceAsRealObjectEndPoint.ForeignKeyDataContainer);
    }

    private void SetOppositeObjectID (ObjectID? value)
    {
      ForeignKeyDataContainer.SetValue(_propertyDefinition, value); // TODO 4608: This is with events, which is a little inconsistent to OppositeObjectID
    }

    private PropertyDefinition GetPropertyDefinition ()
    {
      return ((RelationEndPointDefinition)ID.Definition).PropertyDefinition;
    }


    #region Serialization
    protected RealObjectEndPoint (FlattenedDeserializationInfo info)
      : base(info)
    {
      _foreignKeyDataContainer = info.GetValueForHandle<DataContainer>();
      _propertyDefinition = GetPropertyDefinition();
      _endPointProvider = info.GetValueForHandle<IRelationEndPointProvider>();
      _transactionEventSink = info.GetValueForHandle<IClientTransactionEventSink>();
      _syncState = info.GetValueForHandle<IRealObjectEndPointSyncState>();
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      base.SerializeIntoFlatStructure(info);
      info.AddHandle(_foreignKeyDataContainer);
      info.AddHandle(_endPointProvider);
      info.AddHandle(_transactionEventSink);
      info.AddHandle(_syncState);
    }
    #endregion

  }
}
