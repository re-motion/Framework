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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.Serialization;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints
{
  [TestFixture]
  public class CompleteVirtualObjectEndPointLoadStateTest : StandardMappingTest
  {
    private IVirtualObjectEndPoint _virtualObjectEndPointMock;
    private IVirtualObjectEndPointDataManager _dataManagerMock;
    private IRelationEndPointProvider _endPointProviderStub;
    private IClientTransactionEventSink _transactionEventSinkStub;

    private CompleteVirtualObjectEndPointLoadState _loadState;

    private IRelationEndPointDefinition _definition;

    private OrderTicket _relatedObject;
    private IRealObjectEndPoint _relatedEndPointStub;
    private OrderTicket _relatedObject2;
    private IRealObjectEndPoint _relatedEndPointStub2;

    private Order _owningObject;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();

      _definition = Configuration.GetTypeDefinition (typeof (Order)).GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");

      _virtualObjectEndPointMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      _dataManagerMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPointDataManager> ();
      _dataManagerMock.Stub (stub => stub.EndPointID).Return (RelationEndPointID.Create (DomainObjectIDs.Order1, _definition));
      _endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider> ();
      _transactionEventSinkStub = MockRepository.GenerateStub<IClientTransactionEventSink>();

      _loadState = new CompleteVirtualObjectEndPointLoadState (_dataManagerMock, _endPointProviderStub, _transactionEventSinkStub);

      _relatedObject = DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket1);

      _relatedEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _relatedEndPointStub.Stub (stub => stub.GetDomainObjectReference ()).Return (_relatedObject);
      _relatedEndPointStub.Stub (stub => stub.ObjectID).Return (_relatedObject.ID);

      _relatedObject2 = DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket2);

      _relatedEndPointStub2 = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _relatedEndPointStub2.Stub (stub => stub.ID).Return (RelationEndPointID.Create (_relatedObject2.ID, typeof (OrderTicket), "Order"));
      _relatedEndPointStub2.Stub (stub => stub.GetDomainObjectReference()).Return (_relatedObject2);
      _relatedEndPointStub2.Stub (stub => stub.ObjectID).Return (_relatedObject2.ID);

      _owningObject = DomainObjectMother.CreateFakeObject<Order> ();
    }

    [Test]
    public void GetData ()
    {
      _dataManagerMock.Stub (stub => stub.CurrentOppositeObject).Return (_relatedObject);
      _dataManagerMock.Replay();

      var result = _loadState.GetData (_virtualObjectEndPointMock);

      Assert.That (result, Is.SameAs (_relatedObject));
    }

    [Test]
    public void GetOriginalData ()
    {
      _dataManagerMock.Stub (stub => stub.OriginalOppositeObject).Return (_relatedObject);
      _dataManagerMock.Replay ();

      var result = _loadState.GetOriginalData (_virtualObjectEndPointMock);

      Assert.That (result, Is.SameAs (_relatedObject));
    }

    [Test]
    public void SetDataFromSubTransaction ()
    {
      var sourceDataManager = MockRepository.GenerateStub<IVirtualObjectEndPointDataManager>();
      var sourceLoadState = new CompleteVirtualObjectEndPointLoadState (sourceDataManager, _endPointProviderStub, _transactionEventSinkStub);
      _dataManagerMock.Expect (mock => mock.SetDataFromSubTransaction (sourceDataManager, _endPointProviderStub));
      _dataManagerMock.Replay();

      _loadState.SetDataFromSubTransaction (_virtualObjectEndPointMock, sourceLoadState);

      _dataManagerMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The data is already complete.")]
    public void MarkDataComplete_ThrowsException ()
    {
      _dataManagerMock.Stub (stub => stub.CurrentOppositeObject).Return (_relatedObject);
      _dataManagerMock.Replay ();

      _loadState.MarkDataComplete (_virtualObjectEndPointMock, _relatedObject, dataManager => Assert.Fail ("Must not be called"));
    }

    [Test]
    public void SynchronizeOppositeEndPoint_NoExistingOppositeEndPoint ()
    {
      var endPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint> ();
      endPointMock.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      endPointMock.Expect (mock => mock.MarkSynchronized ());
      endPointMock.Replay ();
      AddUnsynchronizedOppositeEndPoint (_loadState, endPointMock);

      _dataManagerMock.Stub (stub => stub.OriginalOppositeEndPoint).Return (null);
      _dataManagerMock.Stub (stub => stub.ContainsOriginalObjectID (DomainObjectIDs.Order1)).Return (false);
      _dataManagerMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (endPointMock));
      _dataManagerMock.Replay ();

      _loadState.SynchronizeOppositeEndPoint (_virtualObjectEndPointMock, endPointMock);

      _dataManagerMock.VerifyAllExpectations ();
      endPointMock.VerifyAllExpectations ();
      Assert.That (_loadState.UnsynchronizedOppositeEndPoints, Has.No.Member (endPointMock));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The object end-point "
        + "'OrderTicket|0005bdf4-4ccc-4a41-b9b5-baab3eb95237|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order' cannot "
        + "be synchronized with the virtual object end-point "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' because the "
        + "virtual relation property already refers to another object ('OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid'). To synchronize "
        + "'OrderTicket|0005bdf4-4ccc-4a41-b9b5-baab3eb95237|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order', use "
        + "UnloadService to unload either object 'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid' or the virtual object end-point "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket'.")]
    public void SynchronizeOppositeEndPoint_WithExistingOppositeEndPoint ()
    {
      _dataManagerMock.Stub (stub => stub.OriginalOppositeEndPoint).Return (_relatedEndPointStub);

      _loadState.SynchronizeOppositeEndPoint (_virtualObjectEndPointMock, _relatedEndPointStub2);
    }

    [Test]
    public void CreateSetCommand_Same ()
    {
      _dataManagerMock.Stub (stub => stub.CurrentOppositeObject).Return (_relatedObject);
      _dataManagerMock.Stub (stub => stub.OriginalItemWithoutEndPoint).Return (null);
      _dataManagerMock.Replay();
      
      _virtualObjectEndPointMock.Stub (mock => mock.GetDomainObject ()).Return (_owningObject);
      _virtualObjectEndPointMock.Stub (mock => mock.GetOppositeObject ()).Return (_relatedObject);
      _virtualObjectEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _virtualObjectEndPointMock.Replay ();

      var command = (RelationEndPointModificationCommand) _loadState.CreateSetCommand (_virtualObjectEndPointMock, _relatedObject);

      Assert.That (command, Is.TypeOf (typeof (ObjectEndPointSetSameCommand)));
      Assert.That (command.DomainObject, Is.SameAs (_owningObject));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_virtualObjectEndPointMock));
      Assert.That (command.OldRelatedObject, Is.SameAs (_relatedObject));
      Assert.That (command.NewRelatedObject, Is.SameAs (_relatedObject));
      Assert.That (command.TransactionEventSink, Is.SameAs (_transactionEventSinkStub));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
    "The virtual property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' of object "
    + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be set because the property is out of sync with the opposite object "
    + "property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order'. To make this change, synchronize the two properties by "
    + "calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
    + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' property.")]
    public void CreateSetCommand_Same_WithItemWithoutEndPoint ()
    {
      _dataManagerMock.Stub (stub => stub.CurrentOppositeObject).Return (_relatedObject);
      _dataManagerMock.Stub (stub => stub.OriginalItemWithoutEndPoint).Return (_relatedObject);
      _dataManagerMock.Replay ();

      _virtualObjectEndPointMock.Stub (mock => mock.GetDomainObject ()).Return (_owningObject);
      _virtualObjectEndPointMock.Stub (mock => mock.GetOppositeObject ()).Return (_relatedObject);
      _virtualObjectEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _virtualObjectEndPointMock.Replay ();

      var command = (RelationEndPointModificationCommand) _loadState.CreateSetCommand (_virtualObjectEndPointMock, _relatedObject);

      Assert.That (command, Is.TypeOf (typeof (ObjectEndPointSetSameCommand)));
      Assert.That (command.DomainObject, Is.SameAs (_owningObject));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_virtualObjectEndPointMock));
      Assert.That (command.OldRelatedObject, Is.SameAs (_relatedObject));
      Assert.That (command.NewRelatedObject, Is.SameAs (_relatedObject));
      CheckOppositeObjectIDSetter ((ObjectEndPointSetCommand) command);
    }

    [Test]
    public void CreateSetCommand_Same_Null ()
    {
      _dataManagerMock.Stub (stub => stub.CurrentOppositeObject).Return (null);
      _dataManagerMock.Stub (stub => stub.OriginalItemWithoutEndPoint).Return (null);
      _dataManagerMock.Replay ();

      _virtualObjectEndPointMock.Stub (mock => mock.GetDomainObject ()).Return (_owningObject);
      _virtualObjectEndPointMock.Stub (mock => mock.GetOppositeObject ()).Return (null);
      _virtualObjectEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _virtualObjectEndPointMock.Replay ();

      var command = (RelationEndPointModificationCommand) _loadState.CreateSetCommand (_virtualObjectEndPointMock, null);

      Assert.That (command, Is.TypeOf (typeof (ObjectEndPointSetSameCommand)));
      Assert.That (command.DomainObject, Is.SameAs (_owningObject));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_virtualObjectEndPointMock));
      Assert.That (command.OldRelatedObject, Is.Null);
      Assert.That (command.NewRelatedObject, Is.Null);
    }

    [Test]
    public void CreateSetCommand_NotSame ()
    {
      _dataManagerMock.Stub (stub => stub.CurrentOppositeObject).Return (_relatedObject);
      _dataManagerMock.Stub (stub => stub.OriginalItemWithoutEndPoint).Return (null);
      _dataManagerMock.Replay ();

      _virtualObjectEndPointMock.Stub (mock => mock.GetDomainObject ()).Return (_owningObject);
      _virtualObjectEndPointMock.Stub (mock => mock.GetOppositeObject ()).Return (_relatedObject);
      _virtualObjectEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _virtualObjectEndPointMock.Stub (mock => mock.Definition).Return (_definition);
      _virtualObjectEndPointMock.Replay ();

      var newRelatedObject = DomainObjectMother.CreateFakeObject<OrderTicket> ();

      var command = (RelationEndPointModificationCommand) _loadState.CreateSetCommand (_virtualObjectEndPointMock, newRelatedObject);

      Assert.That (command, Is.TypeOf (typeof (ObjectEndPointSetOneOneCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_virtualObjectEndPointMock));
      Assert.That (command.NewRelatedObject, Is.SameAs (newRelatedObject));
      Assert.That (command.OldRelatedObject, Is.SameAs (_relatedObject));
      Assert.That (command.TransactionEventSink, Is.SameAs (_transactionEventSinkStub));

      CheckOppositeObjectIDSetter ((ObjectEndPointSetCommand) command);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The virtual property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' of object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be set because the property is out of sync with the opposite object "
        + "property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order'. To make this change, synchronize the two properties by "
        + "calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' property.")]
    public void CreateSetCommand_NotSame_WithItemWithoutEndPoint ()
    {
      _dataManagerMock.Stub (stub => stub.CurrentOppositeObject).Return (_relatedObject);
      _dataManagerMock.Stub (stub => stub.OriginalItemWithoutEndPoint).Return (_relatedObject);
      _dataManagerMock.Replay ();

      _loadState.CreateSetCommand (_virtualObjectEndPointMock, _relatedObject2);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The domain object with ID 'OrderTicket|0005bdf4-4ccc-4a41-b9b5-baab3eb95237|System.Guid' cannot be set into the virtual property "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' of object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' "
        + "because its object property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order' is out of sync with the virtual property. "
        + "To make this change, synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order' property.")]
    public void CreateSetCommand_NotSame_InputIsInUnsynchronizedOppositeEndPoints ()
    {
      _dataManagerMock.Stub (stub => stub.CurrentOppositeObject).Return (_relatedObject);
      _dataManagerMock.Stub (stub => stub.OriginalItemWithoutEndPoint).Return (null);
      _dataManagerMock.Replay ();

      AddUnsynchronizedOppositeEndPoint (_loadState, _relatedEndPointStub2);

      _loadState.CreateSetCommand (_virtualObjectEndPointMock, _relatedObject2);
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      _dataManagerMock.Stub (stub => stub.OriginalItemWithoutEndPoint).Return (null);
      _dataManagerMock.Replay();

      _virtualObjectEndPointMock.Stub (mock => mock.GetDomainObject ()).Return (_owningObject);
      _virtualObjectEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _virtualObjectEndPointMock.Replay ();

      var command = (RelationEndPointModificationCommand) _loadState.CreateDeleteCommand (_virtualObjectEndPointMock);

      Assert.That (command, Is.TypeOf (typeof (ObjectEndPointDeleteCommand)));
      Assert.That (command.DomainObject, Is.SameAs (_owningObject));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_virtualObjectEndPointMock));
      Assert.That (command.TransactionEventSink, Is.SameAs (_transactionEventSinkStub));

      CheckOppositeObjectNullSetter ((ObjectEndPointDeleteCommand) command);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The domain object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be deleted because its virtual property "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' is out of sync with the opposite object property "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order' of domain object "
        + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid'. To make this change, synchronize the two properties by calling the "
        + "'BidirectionalRelationSyncService.Synchronize' method on the 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' "
        + "property.")]
    public void CreateDeleteCommand_WithOriginalItemWithoutEndPoint ()
    {
      _dataManagerMock.Stub (stub => stub.OriginalItemWithoutEndPoint).Return (_relatedObject);
      _dataManagerMock.Replay();

      _loadState.CreateDeleteCommand (_virtualObjectEndPointMock);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The domain object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be deleted because the opposite object property "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order' of domain object "
        + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid' is out of sync with the virtual property "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket'. To make this change, synchronize the two properties by calling the "
        + "'BidirectionalRelationSyncService.Synchronize' method on the 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order' "
        + "property.")]
    public void CreateDeleteCommand_WithUnsynchronizedOppositeEndPoints ()
    {
      AddUnsynchronizedOppositeEndPoint (_loadState, _relatedEndPointStub);

      _loadState.CreateDeleteCommand (_virtualObjectEndPointMock);
    }

    [Test]
    public void GetOriginalOppositeEndPoints ()
    {
      _dataManagerMock.Stub (mock => mock.OriginalOppositeEndPoint).Return (_relatedEndPointStub);
      _dataManagerMock.Replay ();

      var result = (IEnumerable<IRealObjectEndPoint>) PrivateInvoke.InvokeNonPublicMethod (_loadState, "GetOriginalOppositeEndPoints");

      Assert.That (result, Is.EqualTo (new[] { _relatedEndPointStub }));
    }

    [Test]
    public void GetOriginalOppositeEndPoints_None ()
    {
      _dataManagerMock.Stub (mock => mock.OriginalOppositeEndPoint).Return (null);
      _dataManagerMock.Replay ();

      var result = (IEnumerable<IRealObjectEndPoint>) PrivateInvoke.InvokeNonPublicMethod (_loadState, "GetOriginalOppositeEndPoints");

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void GetOriginalItemsWithoutEndPoints ()
    {
      _dataManagerMock.Stub (mock => mock.OriginalItemWithoutEndPoint).Return (_relatedObject);
      _dataManagerMock.Replay ();

      var result = (IEnumerable<DomainObject>) PrivateInvoke.InvokeNonPublicMethod (_loadState, "GetOriginalItemsWithoutEndPoints");

      Assert.That (result, Is.EqualTo (new[] { _relatedObject }));
    }

    [Test]
    public void GetOriginalItemsWithoutEndPoints_None ()
    {
      _dataManagerMock.Stub (mock => mock.OriginalItemWithoutEndPoint).Return (null);
      _dataManagerMock.Replay ();

      var result = (IEnumerable<DomainObject>) PrivateInvoke.InvokeNonPublicMethod (_loadState, "GetOriginalItemsWithoutEndPoints");

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var state = new CompleteVirtualObjectEndPointLoadState (
          new SerializableVirtualObjectEndPointDataManagerFake(),
          new SerializableRelationEndPointProviderFake(),
          new SerializableClientTransactionEventSinkFake());

      var oppositeEndPoint = new SerializableRealObjectEndPointFake (null, _relatedObject);
      AddUnsynchronizedOppositeEndPoint (state, oppositeEndPoint);

      var result = FlattenedSerializer.SerializeAndDeserialize (state);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.DataManager, Is.Not.Null);
      Assert.That (result.TransactionEventSink, Is.Not.Null);
      Assert.That (result.EndPointProvider, Is.Not.Null);
      Assert.That (result.UnsynchronizedOppositeEndPoints.Count, Is.EqualTo (1));
    }

    private void AddUnsynchronizedOppositeEndPoint (CompleteVirtualObjectEndPointLoadState loadState, IRealObjectEndPoint oppositeEndPoint)
    {
      var dictionary = (Dictionary<ObjectID, IRealObjectEndPoint>) PrivateInvoke.GetNonPublicField (loadState, "_unsynchronizedOppositeEndPoints");
      dictionary.Add (oppositeEndPoint.ObjectID, oppositeEndPoint);
    }

    private void CheckOppositeObjectIDSetter (ObjectEndPointSetCommand command)
    {
      var setter = (Action<DomainObject>) PrivateInvoke.GetNonPublicField (command, "_oppositeObjectSetter");

      var newRelatedObject = DomainObjectMother.CreateFakeObject<OrderTicket> ();

      _dataManagerMock.BackToRecord ();
      _dataManagerMock.Expect (mock => mock.CurrentOppositeObject = newRelatedObject);
      _dataManagerMock.Replay ();

      setter (newRelatedObject);

      _dataManagerMock.VerifyAllExpectations ();
    }

    private void CheckOppositeObjectNullSetter (ObjectEndPointDeleteCommand command)
    {
      var setter = (Action) PrivateInvoke.GetNonPublicField (command, "_oppositeObjectNullSetter");

      _dataManagerMock.BackToRecord ();
      _dataManagerMock.Expect (mock => mock.CurrentOppositeObject = null);
      _dataManagerMock.Replay ();

      setter ();

      _dataManagerMock.VerifyAllExpectations ();
    }
  }
}