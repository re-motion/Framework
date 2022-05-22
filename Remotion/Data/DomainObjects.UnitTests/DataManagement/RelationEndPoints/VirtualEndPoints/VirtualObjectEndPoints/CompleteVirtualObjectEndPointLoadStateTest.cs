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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.Serialization;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints
{
  [TestFixture]
  public class CompleteVirtualObjectEndPointLoadStateTest : StandardMappingTest
  {
    private Mock<IVirtualObjectEndPoint> _virtualObjectEndPointMock;
    private Mock<IVirtualObjectEndPointDataManager> _dataManagerMock;
    private Mock<IRelationEndPointProvider> _endPointProviderStub;
    private Mock<IClientTransactionEventSink> _transactionEventSinkStub;

    private CompleteVirtualObjectEndPointLoadState _loadState;

    private IRelationEndPointDefinition _definition;

    private OrderTicket _relatedObject;
    private Mock<IRealObjectEndPoint> _relatedEndPointStub;
    private OrderTicket _relatedObject2;
    private Mock<IRealObjectEndPoint> _relatedEndPointStub2;

    private Order _owningObject;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _definition = Configuration.GetTypeDefinition(typeof(Order)).GetRelationEndPointDefinition(typeof(Order).FullName + ".OrderTicket");

      _virtualObjectEndPointMock = new Mock<IVirtualObjectEndPoint>(MockBehavior.Strict);
      _dataManagerMock = new Mock<IVirtualObjectEndPointDataManager>(MockBehavior.Strict);
      _dataManagerMock.Setup(stub => stub.EndPointID).Returns(RelationEndPointID.Create(DomainObjectIDs.Order1, _definition));
      _endPointProviderStub = new Mock<IRelationEndPointProvider>();
      _transactionEventSinkStub = new Mock<IClientTransactionEventSink>();

      _loadState = new CompleteVirtualObjectEndPointLoadState(_dataManagerMock.Object, _endPointProviderStub.Object, _transactionEventSinkStub.Object);

      _relatedObject = DomainObjectMother.CreateFakeObject<OrderTicket>(DomainObjectIDs.OrderTicket1);

      _relatedEndPointStub = new Mock<IRealObjectEndPoint>();
      _relatedEndPointStub.Setup(stub => stub.GetDomainObjectReference()).Returns(_relatedObject);
      _relatedEndPointStub.Setup(stub => stub.ObjectID).Returns(_relatedObject.ID);

      _relatedObject2 = DomainObjectMother.CreateFakeObject<OrderTicket>(DomainObjectIDs.OrderTicket2);

      _relatedEndPointStub2 = new Mock<IRealObjectEndPoint>();
      _relatedEndPointStub2.Setup(stub => stub.ID).Returns(RelationEndPointID.Create(_relatedObject2.ID, typeof(OrderTicket), "Order"));
      _relatedEndPointStub2.Setup(stub => stub.GetDomainObjectReference()).Returns(_relatedObject2);
      _relatedEndPointStub2.Setup(stub => stub.ObjectID).Returns(_relatedObject2.ID);

      _owningObject = DomainObjectMother.CreateFakeObject<Order>();
    }

    [Test]
    public void GetData ()
    {
      _dataManagerMock.Setup(stub => stub.CurrentOppositeObject).Returns(_relatedObject);

      var result = _loadState.GetData(_virtualObjectEndPointMock.Object);

      Assert.That(result, Is.SameAs(_relatedObject));
    }

    [Test]
    public void GetOriginalData ()
    {
      _dataManagerMock.Setup(stub => stub.OriginalOppositeObject).Returns(_relatedObject);

      var result = _loadState.GetOriginalData(_virtualObjectEndPointMock.Object);

      Assert.That(result, Is.SameAs(_relatedObject));
    }

    [Test]
    public void SetDataFromSubTransaction ()
    {
      var sourceDataManager = new Mock<IVirtualObjectEndPointDataManager>();
      var sourceLoadState = new CompleteVirtualObjectEndPointLoadState(sourceDataManager.Object, _endPointProviderStub.Object, _transactionEventSinkStub.Object);
      _dataManagerMock.Setup(mock => mock.SetDataFromSubTransaction(sourceDataManager.Object, _endPointProviderStub.Object)).Verifiable();

      _loadState.SetDataFromSubTransaction(_virtualObjectEndPointMock.Object, sourceLoadState);

      _dataManagerMock.Verify();
    }

    [Test]
    public void MarkDataComplete_ThrowsException ()
    {
      _dataManagerMock.Setup(stub => stub.CurrentOppositeObject).Returns(_relatedObject);
      Assert.That(
          () => _loadState.MarkDataComplete(_virtualObjectEndPointMock.Object, _relatedObject, dataManager => Assert.Fail("Must not be called")),
          Throws.InvalidOperationException
              .With.Message.EqualTo("The data is already complete."));
    }

    [Test]
    public void SynchronizeOppositeEndPoint_NoExistingOppositeEndPoint ()
    {
      var endPointMock = new Mock<IRealObjectEndPoint>(MockBehavior.Strict);
      endPointMock.Setup(stub => stub.IsNull).Returns(false);
      endPointMock.Setup(stub => stub.ObjectID).Returns(DomainObjectIDs.Order1);
      endPointMock.Setup(mock => mock.MarkSynchronized()).Verifiable();
      AddUnsynchronizedOppositeEndPoint(_loadState, endPointMock.Object);

      _dataManagerMock.Setup(stub => stub.OriginalOppositeEndPoint).Returns((IRealObjectEndPoint)null);
      _dataManagerMock.Setup(stub => stub.ContainsOriginalObjectID(DomainObjectIDs.Order1)).Returns(false);
      _dataManagerMock.Setup(mock => mock.RegisterOriginalOppositeEndPoint(endPointMock.Object)).Verifiable();

      _loadState.SynchronizeOppositeEndPoint(_virtualObjectEndPointMock.Object, endPointMock.Object);

      _dataManagerMock.Verify();
      endPointMock.Verify();
      Assert.That(_loadState.UnsynchronizedOppositeEndPoints, Has.No.Member(endPointMock.Object));
    }

    [Test]
    public void SynchronizeOppositeEndPoint_WithExistingOppositeEndPoint ()
    {
      _dataManagerMock.Setup(stub => stub.OriginalOppositeEndPoint).Returns(_relatedEndPointStub.Object);
      Assert.That(
          () => _loadState.SynchronizeOppositeEndPoint(_virtualObjectEndPointMock.Object, _relatedEndPointStub2.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The object end-point "
                  + "'OrderTicket|0005bdf4-4ccc-4a41-b9b5-baab3eb95237|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order' cannot "
                  + "be synchronized with the virtual object end-point "
                  + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' because the "
                  + "virtual relation property already refers to another object ('OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid'). To synchronize "
                  + "'OrderTicket|0005bdf4-4ccc-4a41-b9b5-baab3eb95237|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order', use "
                  + "UnloadService to unload either object 'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid' or the virtual object end-point "
                  + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket'."));
    }

    [Test]
    public void CreateSetCommand_Same ()
    {
      _dataManagerMock.Setup(stub => stub.CurrentOppositeObject).Returns(_relatedObject);
      _dataManagerMock.Setup(stub => stub.OriginalItemWithoutEndPoint).Returns((DomainObject)null);

      _virtualObjectEndPointMock.Setup(mock => mock.GetDomainObject()).Returns(_owningObject);
      _virtualObjectEndPointMock.Setup(mock => mock.GetOppositeObject()).Returns(_relatedObject);
      _virtualObjectEndPointMock.Setup(mock => mock.IsNull).Returns(false);

      var command = (RelationEndPointModificationCommand)_loadState.CreateSetCommand(_virtualObjectEndPointMock.Object, _relatedObject);

      Assert.That(command, Is.TypeOf(typeof(ObjectEndPointSetSameCommand)));
      Assert.That(command.DomainObject, Is.SameAs(_owningObject));
      Assert.That(command.ModifiedEndPoint, Is.SameAs(_virtualObjectEndPointMock.Object));
      Assert.That(command.OldRelatedObject, Is.SameAs(_relatedObject));
      Assert.That(command.NewRelatedObject, Is.SameAs(_relatedObject));
      Assert.That(command.TransactionEventSink, Is.SameAs(_transactionEventSinkStub.Object));
    }

    [Test]
    public void CreateSetCommand_Same_WithItemWithoutEndPoint ()
    {
      _dataManagerMock.Setup(stub => stub.CurrentOppositeObject).Returns(_relatedObject);
      _dataManagerMock.Setup(stub => stub.OriginalItemWithoutEndPoint).Returns(_relatedObject);

      _virtualObjectEndPointMock.Setup(mock => mock.GetDomainObject()).Returns(_owningObject);
      _virtualObjectEndPointMock.Setup(mock => mock.GetOppositeObject()).Returns(_relatedObject);
      _virtualObjectEndPointMock.Setup(mock => mock.IsNull).Returns(false);

      Assert.That(
          () => (RelationEndPointModificationCommand)_loadState.CreateSetCommand(_virtualObjectEndPointMock.Object, _relatedObject),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The virtual property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' of object "
                  + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be set because the property is out of sync with the opposite object "
                  + "property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order'. To make this change, synchronize the two properties by "
                  + "calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' property."));
    }

    [Test]
    public void CreateSetCommand_Same_Null ()
    {
      _dataManagerMock.Setup(stub => stub.CurrentOppositeObject).Returns((DomainObject)null);
      _dataManagerMock.Setup(stub => stub.OriginalItemWithoutEndPoint).Returns((DomainObject)null);

      _virtualObjectEndPointMock.Setup(mock => mock.GetDomainObject()).Returns(_owningObject);
      _virtualObjectEndPointMock.Setup(mock => mock.GetOppositeObject()).Returns((DomainObject)null);
      _virtualObjectEndPointMock.Setup(mock => mock.IsNull).Returns(false);

      var command = (RelationEndPointModificationCommand)_loadState.CreateSetCommand(_virtualObjectEndPointMock.Object, null);

      Assert.That(command, Is.TypeOf(typeof(ObjectEndPointSetSameCommand)));
      Assert.That(command.DomainObject, Is.SameAs(_owningObject));
      Assert.That(command.ModifiedEndPoint, Is.SameAs(_virtualObjectEndPointMock.Object));
      Assert.That(command.OldRelatedObject, Is.Null);
      Assert.That(command.NewRelatedObject, Is.Null);
    }

    [Test]
    public void CreateSetCommand_NotSame ()
    {
      _dataManagerMock.Setup(stub => stub.CurrentOppositeObject).Returns(_relatedObject);
      _dataManagerMock.Setup(stub => stub.OriginalItemWithoutEndPoint).Returns((DomainObject)null);

      _virtualObjectEndPointMock.Setup(mock => mock.GetDomainObject()).Returns(_owningObject);
      _virtualObjectEndPointMock.Setup(mock => mock.GetOppositeObject()).Returns(_relatedObject);
      _virtualObjectEndPointMock.Setup(mock => mock.IsNull).Returns(false);
      _virtualObjectEndPointMock.Setup(mock => mock.Definition).Returns(_definition);

      var newRelatedObject = DomainObjectMother.CreateFakeObject<OrderTicket>();

      var command = (RelationEndPointModificationCommand)_loadState.CreateSetCommand(_virtualObjectEndPointMock.Object, newRelatedObject);

      Assert.That(command, Is.TypeOf(typeof(ObjectEndPointSetOneOneCommand)));
      Assert.That(command.ModifiedEndPoint, Is.SameAs(_virtualObjectEndPointMock.Object));
      Assert.That(command.NewRelatedObject, Is.SameAs(newRelatedObject));
      Assert.That(command.OldRelatedObject, Is.SameAs(_relatedObject));
      Assert.That(command.TransactionEventSink, Is.SameAs(_transactionEventSinkStub.Object));

      CheckOppositeObjectIDSetter((ObjectEndPointSetCommand)command);
    }

    [Test]
    public void CreateSetCommand_NotSame_WithItemWithoutEndPoint ()
    {
      _dataManagerMock.Setup(stub => stub.CurrentOppositeObject).Returns(_relatedObject);
      _dataManagerMock.Setup(stub => stub.OriginalItemWithoutEndPoint).Returns(_relatedObject);
      Assert.That(
          () => _loadState.CreateSetCommand(_virtualObjectEndPointMock.Object, _relatedObject2),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The virtual property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' of object "
                  + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be set because the property is out of sync with the opposite object "
                  + "property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order'. To make this change, synchronize the two properties by "
                  + "calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' property."));
    }

    [Test]
    public void CreateSetCommand_NotSame_InputIsInUnsynchronizedOppositeEndPoints ()
    {
      _dataManagerMock.Setup(stub => stub.CurrentOppositeObject).Returns(_relatedObject);
      _dataManagerMock.Setup(stub => stub.OriginalItemWithoutEndPoint).Returns((DomainObject)null);

      AddUnsynchronizedOppositeEndPoint(_loadState, _relatedEndPointStub2.Object);
      Assert.That(
          () => _loadState.CreateSetCommand(_virtualObjectEndPointMock.Object, _relatedObject2),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The domain object with ID 'OrderTicket|0005bdf4-4ccc-4a41-b9b5-baab3eb95237|System.Guid' cannot be set into the virtual property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' of object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' "
                  + "because its object property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order' is out of sync with the virtual property. "
                  + "To make this change, synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order' property."));
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      _dataManagerMock.Setup(stub => stub.OriginalItemWithoutEndPoint).Returns((DomainObject)null);

      _virtualObjectEndPointMock.Setup(mock => mock.GetDomainObject()).Returns(_owningObject);
      _virtualObjectEndPointMock.Setup(mock => mock.IsNull).Returns(false);

      var command = (RelationEndPointModificationCommand)_loadState.CreateDeleteCommand(_virtualObjectEndPointMock.Object);

      Assert.That(command, Is.TypeOf(typeof(ObjectEndPointDeleteCommand)));
      Assert.That(command.DomainObject, Is.SameAs(_owningObject));
      Assert.That(command.ModifiedEndPoint, Is.SameAs(_virtualObjectEndPointMock.Object));
      Assert.That(command.TransactionEventSink, Is.SameAs(_transactionEventSinkStub.Object));

      CheckOppositeObjectNullSetter((ObjectEndPointDeleteCommand)command);
    }

    [Test]
    public void CreateDeleteCommand_WithOriginalItemWithoutEndPoint ()
    {
      _dataManagerMock.Setup(stub => stub.OriginalItemWithoutEndPoint).Returns(_relatedObject);
      Assert.That(
          () => _loadState.CreateDeleteCommand(_virtualObjectEndPointMock.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The domain object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be deleted because its virtual property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' is out of sync with the opposite object property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order' of domain object "
                  + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid'. To make this change, synchronize the two properties by calling the "
                  + "'BidirectionalRelationSyncService.Synchronize' method on the 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' "
                  + "property."));
    }

    [Test]
    public void CreateDeleteCommand_WithUnsynchronizedOppositeEndPoints ()
    {
      AddUnsynchronizedOppositeEndPoint(_loadState, _relatedEndPointStub.Object);
      Assert.That(
          () => _loadState.CreateDeleteCommand(_virtualObjectEndPointMock.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The domain object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be deleted because the opposite object property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order' of domain object "
                  + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid' is out of sync with the virtual property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket'. To make this change, synchronize the two properties by calling the "
                  + "'BidirectionalRelationSyncService.Synchronize' method on the 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order' "
                  + "property."));
    }

    [Test]
    public void GetOriginalOppositeEndPoints ()
    {
      _dataManagerMock.Setup(mock => mock.OriginalOppositeEndPoint).Returns(_relatedEndPointStub.Object);

      var result = (IEnumerable<IRealObjectEndPoint>)PrivateInvoke.InvokeNonPublicMethod(_loadState, "GetOriginalOppositeEndPoints");

      Assert.That(result, Is.EqualTo(new[] { _relatedEndPointStub.Object }));
    }

    [Test]
    public void GetOriginalOppositeEndPoints_None ()
    {
      _dataManagerMock.Setup(mock => mock.OriginalOppositeEndPoint).Returns((IRealObjectEndPoint)null);

      var result = (IEnumerable<IRealObjectEndPoint>)PrivateInvoke.InvokeNonPublicMethod(_loadState, "GetOriginalOppositeEndPoints");

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetOriginalItemsWithoutEndPoints ()
    {
      _dataManagerMock.Setup(mock => mock.OriginalItemWithoutEndPoint).Returns(_relatedObject);

      var result = (IEnumerable<IDomainObject>)PrivateInvoke.InvokeNonPublicMethod(_loadState, "GetOriginalItemsWithoutEndPoints");

      Assert.That(result, Is.EqualTo(new[] { _relatedObject }));
    }

    [Test]
    public void GetOriginalItemsWithoutEndPoints_None ()
    {
      _dataManagerMock.Setup(mock => mock.OriginalItemWithoutEndPoint).Returns((IDomainObject)null);

      var result = (IEnumerable<IDomainObject>)PrivateInvoke.InvokeNonPublicMethod(_loadState, "GetOriginalItemsWithoutEndPoints");

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void FlattenedSerializable ()
    {
      Assert2.IgnoreIfFeatureSerializationIsDisabled();

      var state = new CompleteVirtualObjectEndPointLoadState(
          new SerializableVirtualObjectEndPointDataManagerFake(),
          new SerializableRelationEndPointProviderFake(),
          new SerializableClientTransactionEventSinkFake());

      var oppositeEndPoint = new SerializableRealObjectEndPointFake(null, _relatedObject);
      AddUnsynchronizedOppositeEndPoint(state, oppositeEndPoint);

      var result = FlattenedSerializer.SerializeAndDeserialize(state);

      Assert.That(result, Is.Not.Null);
      Assert.That(result.DataManager, Is.Not.Null);
      Assert.That(result.TransactionEventSink, Is.Not.Null);
      Assert.That(result.EndPointProvider, Is.Not.Null);
      Assert.That(result.UnsynchronizedOppositeEndPoints.Count, Is.EqualTo(1));
    }

    private void AddUnsynchronizedOppositeEndPoint (CompleteVirtualObjectEndPointLoadState loadState, IRealObjectEndPoint oppositeEndPoint)
    {
      var dictionary = (Dictionary<ObjectID, IRealObjectEndPoint>)PrivateInvoke.GetNonPublicField(loadState, "_unsynchronizedOppositeEndPoints");
      dictionary.Add(oppositeEndPoint.ObjectID, oppositeEndPoint);
    }

    private void CheckOppositeObjectIDSetter (ObjectEndPointSetCommand command)
    {
      var setter = (Action<DomainObject>)PrivateInvoke.GetNonPublicField(command, "_oppositeObjectSetter");

      var newRelatedObject = DomainObjectMother.CreateFakeObject<OrderTicket>();

      _dataManagerMock.Reset();
      _dataManagerMock.SetupSet(mock => mock.CurrentOppositeObject = newRelatedObject).Verifiable();

      setter(newRelatedObject);

      _dataManagerMock.Verify();
    }

    private void CheckOppositeObjectNullSetter (ObjectEndPointDeleteCommand command)
    {
      var setter = (Action)PrivateInvoke.GetNonPublicField(command, "_oppositeObjectNullSetter");

      _dataManagerMock.Reset();
      _dataManagerMock.SetupSet(mock => mock.CurrentOppositeObject = null).Verifiable();

      setter();

      _dataManagerMock.Verify();
    }
  }
}
