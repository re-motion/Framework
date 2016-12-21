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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.Serialization;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints
{
  [TestFixture]
  public class CompleteVirtualEndPointLoadStateBaseTest : StandardMappingTest
  {
    private IVirtualEndPoint<object> _virtualEndPointMock;
    private IVirtualEndPointDataManager _dataManagerMock;
    private IRelationEndPointProvider _endPointProviderStub;
    private IClientTransactionEventSink _transactionEventSinkWithMock;

    private TestableCompleteVirtualEndPointLoadState _loadState;

    private IRelationEndPointDefinition _definition;
    private Order _relatedObject;
    private IRealObjectEndPoint _relatedEndPointStub;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _definition = Configuration.GetTypeDefinition (typeof (Customer)).GetRelationEndPointDefinition (typeof (Customer).FullName + ".Orders");

      _virtualEndPointMock = MockRepository.GenerateStrictMock<IVirtualEndPoint<object>> ();
      _dataManagerMock = MockRepository.GenerateStrictMock<IVirtualEndPointDataManager>();
      _dataManagerMock.Stub (stub => stub.EndPointID).Return (RelationEndPointID.Create (DomainObjectIDs.Customer1, _definition));
      _endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider>();
      _transactionEventSinkWithMock = MockRepository.GenerateStrictMock<IClientTransactionEventSink>();

      _loadState = new TestableCompleteVirtualEndPointLoadState (_dataManagerMock, _endPointProviderStub, _transactionEventSinkWithMock);

      _relatedObject = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);
      _relatedEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint>();
      _relatedEndPointStub.Stub (stub => stub.GetDomainObjectReference()).Return (_relatedObject);
      _relatedEndPointStub.Stub (stub => stub.ObjectID).Return (_relatedObject.ID);
    }

    [Test]
    public void IsDataComplete ()
    {
      Assert.That (_loadState.IsDataComplete(), Is.True);
    }

    [Test]
    public void EnsureDataComplete_DoesNothing ()
    {
      _virtualEndPointMock.Replay();
      _dataManagerMock.Replay();

      _loadState.EnsureDataComplete (_virtualEndPointMock);

      _virtualEndPointMock.VerifyAllExpectations();
      _dataManagerMock.VerifyAllExpectations();
    }

    [Test]
    public void CanDataBeMarkedIncomplete_True ()
    {
      _virtualEndPointMock.Replay();
      _dataManagerMock.Stub (stub => stub.HasDataChanged()).Return (false);
      _dataManagerMock.Replay();

      Assert.That (_loadState.CanDataBeMarkedIncomplete (_virtualEndPointMock), Is.True);
    }

    [Test]
    public void CanDataBeMarkedIncomplete_False ()
    {
      _virtualEndPointMock.Replay ();
      _dataManagerMock.Stub (stub => stub.HasDataChanged ()).Return (true);
      _dataManagerMock.Replay ();

      Assert.That (_loadState.CanDataBeMarkedIncomplete (_virtualEndPointMock), Is.False);
    }

    [Test]
    public void MarkDataIncomplete_RaisesEvent ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      _virtualEndPointMock
          .Stub (stub => stub.ID)
          .Return (endPointID);
      _virtualEndPointMock.Replay();

      _dataManagerMock.Stub (stub => stub.HasDataChanged ()).Return (false);
      _dataManagerMock.Replay ();

      _loadState.StubOriginalOppositeEndPoints (new IRealObjectEndPoint[0]);

      _transactionEventSinkWithMock.Expect (mock => mock.RaiseRelationEndPointBecomingIncompleteEvent ( endPointID));
      _transactionEventSinkWithMock.Replay();

      _loadState.MarkDataIncomplete (_virtualEndPointMock, () => { });

      _virtualEndPointMock.VerifyAllExpectations();
      _dataManagerMock.VerifyAllExpectations();
      _transactionEventSinkWithMock.VerifyAllExpectations();
    }

    [Test]
    public void MarkDataIncomplete_ExecutesStateSetter_AndSynchronizesOppositeEndPoints ()
    {
      // ReSharper disable AccessToModifiedClosure
      bool stateSetterCalled = false;

      var synchronizedOppositeEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _loadState.StubOriginalOppositeEndPoints (new[] { synchronizedOppositeEndPointStub });

      var unsynchronizedOppositeEndPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint> ();
      unsynchronizedOppositeEndPointMock.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      AddUnsynchronizedOppositeEndPoint (_loadState, unsynchronizedOppositeEndPointMock);
      unsynchronizedOppositeEndPointMock.Replay ();

      _virtualEndPointMock
          .Stub (stub => stub.ID)
          .Return (RelationEndPointID.Create (DomainObjectIDs.Customer1, _definition));
      _virtualEndPointMock
          .Expect (mock => mock.RegisterOriginalOppositeEndPoint (synchronizedOppositeEndPointStub))
          .WhenCalled (mi => Assert.That (stateSetterCalled, Is.True));
      _virtualEndPointMock
          .Expect (mock => mock.RegisterOriginalOppositeEndPoint (unsynchronizedOppositeEndPointMock))
          .WhenCalled (mi => Assert.That (stateSetterCalled, Is.True));
      _virtualEndPointMock.Replay();

      _dataManagerMock.Stub (stub => stub.HasDataChanged ()).Return (false);
      _dataManagerMock.Replay ();

      _transactionEventSinkWithMock.Stub (mock => mock.RaiseRelationEndPointBecomingIncompleteEvent ( Arg<RelationEndPointID>.Is.Anything));

      _loadState.MarkDataIncomplete (_virtualEndPointMock, () => stateSetterCalled = true);

      _virtualEndPointMock.VerifyAllExpectations();
      unsynchronizedOppositeEndPointMock.VerifyAllExpectations ();
      _dataManagerMock.VerifyAllExpectations();

      Assert.That (stateSetterCalled, Is.True);
      // ReSharper restore AccessToModifiedClosure
    }

    [Test]
    public void MarkDataIncomplete_Throws_WithChangedData ()
    {
      _virtualEndPointMock
          .Stub (stub => stub.ID)
          .Return (RelationEndPointID.Create (DomainObjectIDs.Customer1, _definition));
      _virtualEndPointMock.Replay ();

      _dataManagerMock.Stub (stub => stub.HasDataChanged ()).Return (true);
      _dataManagerMock.Replay ();

      _transactionEventSinkWithMock.Replay();

      Assert.That (
          () =>_loadState.MarkDataIncomplete (_virtualEndPointMock, () => Assert.Fail ("Must not be called.")),
          Throws.InvalidOperationException.With.Message.EqualTo (
          "Cannot mark virtual end-point "
          + "'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' incomplete "
          + "because it has been changed."));

      _transactionEventSinkWithMock.AssertWasNotCalled (mock => mock.RaiseRelationEndPointBecomingIncompleteEvent ( Arg<RelationEndPointID>.Is.Anything));
    }

    [Test]
    public void CanEndPointBeCollected ()
    {
      var result = _loadState.CanEndPointBeCollected (_virtualEndPointMock);
      Assert.That (result, Is.False);
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint_WithoutExistingItem ()
    {
      var endPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint>();
      endPointMock.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      endPointMock.Expect (mock => mock.MarkUnsynchronized());
      endPointMock.Replay();

      _dataManagerMock.Stub (stub => stub.ContainsOriginalObjectID (DomainObjectIDs.Order1)).Return (false);
      _dataManagerMock.Replay();

      _loadState.RegisterOriginalOppositeEndPoint (_virtualEndPointMock, endPointMock);

      _dataManagerMock.AssertWasNotCalled (mock => mock.RegisterOriginalOppositeEndPoint (endPointMock));
      endPointMock.VerifyAllExpectations();
      _dataManagerMock.VerifyAllExpectations();

      Assert.That (_loadState.UnsynchronizedOppositeEndPoints, Has.Member(endPointMock));
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint_WithExistingItem ()
    {
      var endPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint>();
      endPointMock.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      endPointMock.Expect (mock => mock.MarkSynchronized());
      endPointMock.Replay();

      _dataManagerMock.Stub (stub => stub.ContainsOriginalObjectID (DomainObjectIDs.Order1)).Return (true);
      _dataManagerMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (endPointMock));
      _dataManagerMock.Replay();

      _loadState.RegisterOriginalOppositeEndPoint (_virtualEndPointMock, endPointMock);

      endPointMock.VerifyAllExpectations();
      _dataManagerMock.VerifyAllExpectations();
      Assert.That (_loadState.UnsynchronizedOppositeEndPoints, Has.No.Member(endPointMock));
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint ()
    {
      using (_virtualEndPointMock.GetMockRepository().Ordered())
      {
        _virtualEndPointMock.Expect (mock => mock.MarkDataIncomplete());
        _virtualEndPointMock.Expect (mock => mock.UnregisterOriginalOppositeEndPoint (_relatedEndPointStub));
      }
      _virtualEndPointMock.Replay();

      _loadState.UnregisterOriginalOppositeEndPoint (_virtualEndPointMock, _relatedEndPointStub);

      _dataManagerMock.VerifyAllExpectations();
      _virtualEndPointMock.VerifyAllExpectations();
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint_InUnsyncedList ()
    {
      AddUnsynchronizedOppositeEndPoint (_loadState, _relatedEndPointStub);

      _virtualEndPointMock.Replay();
      _dataManagerMock.Replay();

      _loadState.UnregisterOriginalOppositeEndPoint (_virtualEndPointMock, _relatedEndPointStub);

      _dataManagerMock.VerifyAllExpectations();
      _virtualEndPointMock.AssertWasNotCalled (mock => mock.MarkDataIncomplete());
      _virtualEndPointMock.AssertWasNotCalled (mock => mock.UnregisterOriginalOppositeEndPoint (_relatedEndPointStub));
      Assert.That (_loadState.UnsynchronizedOppositeEndPoints, Has.No.Member(_relatedEndPointStub));
    }

    [Test]
    public void RegisterCurrentOppositeEndPoint ()
    {
      _relatedEndPointStub.Stub (stub => stub.IsSynchronized).Return (true);

      _dataManagerMock.Expect (mock => mock.RegisterCurrentOppositeEndPoint (_relatedEndPointStub));
      _dataManagerMock.Replay();

      _loadState.RegisterCurrentOppositeEndPoint (_virtualEndPointMock, _relatedEndPointStub);

      _dataManagerMock.VerifyAllExpectations();
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint ()
    {
      _dataManagerMock.Expect (mock => mock.UnregisterCurrentOppositeEndPoint (_relatedEndPointStub));
      _dataManagerMock.Replay();

      _loadState.UnregisterCurrentOppositeEndPoint (_virtualEndPointMock, _relatedEndPointStub);

      _dataManagerMock.VerifyAllExpectations();
    }

    [Test]
    public void IsSynchronized_True ()
    {
      _loadState.StubOriginalItemsWithoutEndPoints (new DomainObject[0]);

      var result = _loadState.IsSynchronized (_virtualEndPointMock);

      Assert.That (result, Is.True);
    }

    [Test]
    public void IsSynchronized_False ()
    {
      _loadState.StubOriginalItemsWithoutEndPoints (new DomainObject[] { _relatedObject });

      var result = _loadState.IsSynchronized (_virtualEndPointMock);

      Assert.That (result, Is.False);
    }

    [Test]
    public void Synchronize ()
    {
      _loadState.StubOriginalItemsWithoutEndPoints (new[] { _relatedObject });

      _dataManagerMock.Expect (mock => mock.UnregisterOriginalItemWithoutEndPoint (_relatedObject));
      _dataManagerMock.Replay ();

      _loadState.Synchronize (_virtualEndPointMock);

      _dataManagerMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetUnsynchronizedOppositeEndPoints_Empty ()
    {
      var result = _loadState.UnsynchronizedOppositeEndPoints;

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void SynchronizeOppositeEndPoint_InList ()
    {
      var endPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint>();
      endPointMock.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      endPointMock.Stub (mock => mock.MarkUnsynchronized());
      endPointMock.Expect (mock => mock.MarkSynchronized());
      endPointMock.Replay();

      _dataManagerMock.Stub (stub => stub.ContainsOriginalObjectID (DomainObjectIDs.Order1)).Return (false);
      _dataManagerMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (endPointMock));
      _dataManagerMock.Replay();

      _loadState.RegisterOriginalOppositeEndPoint (_virtualEndPointMock, endPointMock);
      Assert.That (_loadState.UnsynchronizedOppositeEndPoints, Has.Member(endPointMock));

      _loadState.SynchronizeOppositeEndPoint (_virtualEndPointMock, endPointMock);

      _dataManagerMock.VerifyAllExpectations();
      endPointMock.VerifyAllExpectations();
      Assert.That (_loadState.UnsynchronizedOppositeEndPoints, Has.No.Member(endPointMock));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot synchronize opposite end-point "
        + "'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order' - the "
        + "end-point is not in the list of unsynchronized end-points.")]
    public void SynchronizeOppositeEndPoint_NotInList ()
    {
      var endPointStub = MockRepository.GenerateStub<IRealObjectEndPoint>();
      endPointStub
          .Stub (stub => stub.ID)
          .Return (RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order"));
      endPointStub
          .Stub (stub => stub.ObjectID)
          .Return (DomainObjectIDs.OrderItem1);

      _loadState.SynchronizeOppositeEndPoint (_virtualEndPointMock, endPointStub);
    }
    
    [Test]
    public void HasChanged ()
    {
      _dataManagerMock.Expect (mock => mock.HasDataChanged()).Return (true);
      _dataManagerMock.Replay();

      var result = _loadState.HasChanged();

      _dataManagerMock.VerifyAllExpectations();
      Assert.That (result, Is.True);
    }

    [Test]
    public void Commit ()
    {
      _dataManagerMock.Expect (mock => mock.Commit());
      _dataManagerMock.Replay();

      _loadState.Commit (_virtualEndPointMock);

      _dataManagerMock.VerifyAllExpectations();
    }

    [Test]
    public void Rollback ()
    {
      _dataManagerMock.Expect (mock => mock.Rollback());
      _dataManagerMock.Replay();

      _loadState.Rollback (_virtualEndPointMock);

      _dataManagerMock.VerifyAllExpectations();
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var state = new TestableCompleteVirtualEndPointLoadState (
          new SerializableVirtualEndPointDataManagerFake(),
          new SerializableRelationEndPointProviderFake(),
          new SerializableClientTransactionEventSinkFake());

      var oppositeEndPoint = new SerializableRealObjectEndPointFake (null, _relatedObject);
      AddUnsynchronizedOppositeEndPoint (state, oppositeEndPoint);

      var result = FlattenedSerializer.SerializeAndDeserialize (state);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.DataManager, Is.Not.Null);
      Assert.That (result.EndPointProvider, Is.Not.Null);
      Assert.That (result.TransactionEventSink, Is.Not.Null);
      Assert.That (result.UnsynchronizedOppositeEndPoints.Count, Is.EqualTo (1));
    }

    private void AddUnsynchronizedOppositeEndPoint (
        CompleteVirtualEndPointLoadStateBase<IVirtualEndPoint<object>, object, IVirtualEndPointDataManager> loadState, IRealObjectEndPoint oppositeEndPoint)
    {
      var dictionary = (Dictionary<ObjectID, IRealObjectEndPoint>) PrivateInvoke.GetNonPublicField (loadState, "_unsynchronizedOppositeEndPoints");
      dictionary.Add (oppositeEndPoint.ObjectID, oppositeEndPoint);
    }
  }
}