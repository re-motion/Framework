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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.Serialization;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints
{
  [TestFixture]
  public class IncompleteVirtualEndPointLoadStateBaseTest : StandardMappingTest
  {
    private IVirtualEndPoint<object> _virtualEndPointMock;
    private TestableIncompleteVirtualEndPointLoadState.IEndPointLoader _endPointLoaderMock;

    private TestableIncompleteVirtualEndPointLoadState _loadState;

    private IRealObjectEndPoint _relatedEndPointStub1;
    private IRealObjectEndPoint _relatedEndPointStub2;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _virtualEndPointMock = MockRepository.GenerateStrictMock<IVirtualEndPoint<object>>();
      _endPointLoaderMock = MockRepository.GenerateStrictMock<TestableIncompleteVirtualEndPointLoadState.IEndPointLoader> ();

      _loadState = new TestableIncompleteVirtualEndPointLoadState (_endPointLoaderMock);

      _relatedEndPointStub1 = MockRepository.GenerateStub<IRealObjectEndPoint>();
      _relatedEndPointStub1.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      _relatedEndPointStub1.Stub (stub => stub.ID).Return (RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "Customer"));

      _relatedEndPointStub2 = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _relatedEndPointStub2.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order3);
      _relatedEndPointStub2.Stub (stub => stub.ID).Return (RelationEndPointID.Create (DomainObjectIDs.Order3, typeof (Order), "Customer"));
    }

    [Test]
    public void IsDataComplete ()
    {
      Assert.That (_loadState.IsDataComplete (), Is.False);
    }

    [Test]
    public void CanDataBeMarkedIncomplete ()
    {
      _virtualEndPointMock.Replay ();

      Assert.That (_loadState.CanDataBeMarkedIncomplete (_virtualEndPointMock), Is.True);
    }

    [Test]
    public void MarkDataIncomplete_DoesNothing ()
    {
      _virtualEndPointMock.Replay();

      _loadState.MarkDataIncomplete (_virtualEndPointMock, () => Assert.Fail ("Must not be called."));
    }

    [Test]
    public void CanEndPointBeCollected_False ()
    {
      _loadState.RegisterOriginalOppositeEndPoint (_virtualEndPointMock, _relatedEndPointStub1);
      
      var result = _loadState.CanEndPointBeCollected (_virtualEndPointMock);

      Assert.That (result, Is.False);
    }

    [Test]
    public void CanEndPointBeCollected_True ()
    {
      var result = _loadState.CanEndPointBeCollected (_virtualEndPointMock);
      Assert.That (result, Is.True);
    }

    [Test]
    public void GetData ()
    {
      CheckOperationDelegatesToCompleteState (s => s.GetData (_virtualEndPointMock), new object());
    }

    [Test]
    public void GetOriginalData ()
    {
      CheckOperationDelegatesToCompleteState (s => s.GetOriginalData (_virtualEndPointMock), new object());
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint ()
    {
      Assert.That (_loadState.OriginalOppositeEndPoints.Count, Is.EqualTo (0));

      var endPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint> ();
      endPointMock.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      endPointMock.Expect (mock => mock.ResetSyncState());
      endPointMock.Replay();
      
      _loadState.RegisterOriginalOppositeEndPoint (_virtualEndPointMock, endPointMock);

      Assert.That (_loadState.OriginalOppositeEndPoints.ToArray(), Is.EqualTo (new[] { endPointMock }));
      endPointMock.VerifyAllExpectations();
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint_RegisteredInDataManager ()
    {
      Assert.That (_loadState.OriginalOppositeEndPoints.Count, Is.EqualTo (0));
      var endPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint> ();
      endPointMock.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      endPointMock.Expect (mock => mock.ResetSyncState ());
      endPointMock.Replay ();

      _loadState.RegisterOriginalOppositeEndPoint (_virtualEndPointMock, endPointMock);
      Assert.That (_loadState.OriginalOppositeEndPoints.ToArray (), Is.EqualTo (new[] { endPointMock }));
      
      _loadState.UnregisterOriginalOppositeEndPoint (_virtualEndPointMock, endPointMock);

      Assert.That (_loadState.OriginalOppositeEndPoints.Count, Is.EqualTo (0));
      endPointMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "The opposite end-point has not been registered.")]
    public void UnregisterOriginalOppositeEndPoint_ThrowsIfNotRegistered ()
    {
      Assert.That (_loadState.OriginalOppositeEndPoints.Count, Is.EqualTo (0));
      var endPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint> ();
      endPointMock.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);

      _loadState.UnregisterOriginalOppositeEndPoint (_virtualEndPointMock, endPointMock);
    }

    [Test]
    public void RegisterCurrentOppositeEndPoint ()
    {
      CheckOperationDelegatesToCompleteState (s => s.RegisterCurrentOppositeEndPoint (_virtualEndPointMock, _relatedEndPointStub1));
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint ()
    {
      CheckOperationDelegatesToCompleteState (s => s.UnregisterCurrentOppositeEndPoint (_virtualEndPointMock, _relatedEndPointStub1));
    }

    [Test]
    public void IsSynchronized ()
    {
      Assert.That (_loadState.IsSynchronized (_virtualEndPointMock), Is.Null);
    }

    [Test]
    public void Synchronize ()
    {
      CheckOperationDelegatesToCompleteState (s => s.Synchronize (_virtualEndPointMock));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot synchronize an opposite end-point with a virtual end-point in incomplete state.")]
    public void SynchronizeOppositeEndPoint ()
    {
      _loadState.SynchronizeOppositeEndPoint (_virtualEndPointMock, _relatedEndPointStub1);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "Cannot comit data from a sub-transaction into a virtual end-point in incomplete state.")]
    public void SetDataFromSubTransaction ()
    {
      _loadState.SetDataFromSubTransaction (
          _virtualEndPointMock,
          MockRepository.GenerateStub<IVirtualEndPointLoadState<IVirtualEndPoint<object>, object, IVirtualEndPointDataManager>> ());
    }

    [Test]
    public void HasChanged ()
    {
      var result = _loadState.HasChanged ();

      Assert.That (result, Is.False);
    }

    [Test]
    public void Commit ()
    {
      _loadState.Commit (_virtualEndPointMock);
    }

    [Test]
    public void Rollback ()
    {
      _loadState.Rollback (_virtualEndPointMock);
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var endPointLoader =
          new SerializableVirtualEndPointLoaderFake<
              IVirtualEndPoint<object>, 
              object, 
              IVirtualEndPointDataManager,
              IVirtualEndPointLoadState<IVirtualEndPoint<object>, object, IVirtualEndPointDataManager>>();

      var state = new TestableIncompleteVirtualEndPointLoadState (endPointLoader);

      var oppositeEndPoint = new SerializableRealObjectEndPointFake (
          null,
          DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket1));
      state.RegisterOriginalOppositeEndPoint (_virtualEndPointMock, oppositeEndPoint);
      
      var result = FlattenedSerializer.SerializeAndDeserialize (state);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.OriginalOppositeEndPoints, Is.Not.Null);
      Assert.That (result.OriginalOppositeEndPoints, Is.Not.Empty);
      Assert.That (result.EndPointLoader, Is.Not.Null);
    }

    private void CheckOperationDelegatesToCompleteState (
        Action<IVirtualEndPointLoadState<IVirtualEndPoint<object>, object, IVirtualEndPointDataManager>> operation)
    {
      var newStateMock = MockRepository.GenerateStrictMock<IVirtualEndPointLoadState<IVirtualEndPoint<object>, object, IVirtualEndPointDataManager>> ();
      _endPointLoaderMock.Expect (mock => mock.LoadEndPointAndGetNewState (_virtualEndPointMock)).Return (newStateMock);
      _endPointLoaderMock.Replay ();

      newStateMock.Expect (operation);
      newStateMock.Replay ();

      operation (_loadState);

      newStateMock.VerifyAllExpectations ();
    }

    private void CheckOperationDelegatesToCompleteState<T> (
        Func<IVirtualEndPointLoadState<IVirtualEndPoint<object>, object, IVirtualEndPointDataManager>, T> operation, 
        T fakeResult)
    {
      var newStateMock = MockRepository.GenerateStrictMock<IVirtualEndPointLoadState<IVirtualEndPoint<object>, object, IVirtualEndPointDataManager>>();
      _endPointLoaderMock.Expect (mock => mock.LoadEndPointAndGetNewState (_virtualEndPointMock)).Return (newStateMock);
      _endPointLoaderMock.Replay();

      newStateMock.Expect (mock => operation (mock)).Return (fakeResult);
      newStateMock.Replay ();

      var result = operation (_loadState);

      newStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (fakeResult));
    }
  }
}