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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.Serialization;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints
{
  [TestFixture]
  public class IncompleteVirtualObjectEndPointLoadStateTest : StandardMappingTest
  {
    private RelationEndPointID _endPointID;
    private IVirtualObjectEndPoint _virtualObjectEndPointMock;

    private IncompleteVirtualEndPointLoadStateBase<IVirtualObjectEndPoint, DomainObject, IVirtualObjectEndPointDataManager, IVirtualObjectEndPointLoadState>.IEndPointLoader _endPointLoaderMock;
    private IVirtualObjectEndPointDataManagerFactory _dataManagerFactoryStub;

    private IncompleteVirtualObjectEndPointLoadState _loadState;

    private OrderTicket _relatedObject;
    private IRealObjectEndPoint _relatedEndPointStub;

    private OrderTicket _relatedObject2;
    private IRealObjectEndPoint _relatedEndPointStub2;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();

      _endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderTicket");
      _virtualObjectEndPointMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
    
      _endPointLoaderMock = MockRepository.GenerateStrictMock<IncompleteVirtualObjectEndPointLoadState.IEndPointLoader> ();
      _dataManagerFactoryStub = MockRepository.GenerateStub<IVirtualObjectEndPointDataManagerFactory> ();

      var dataManagerStub = MockRepository.GenerateStub<IVirtualObjectEndPointDataManager> ();
      dataManagerStub.Stub (stub => stub.HasDataChanged()).Return (false);
      _loadState = new IncompleteVirtualObjectEndPointLoadState (_endPointLoaderMock, _dataManagerFactoryStub);

      _relatedObject = DomainObjectMother.CreateFakeObject<OrderTicket> ();
      _relatedEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _relatedEndPointStub.Stub (stub => stub.ObjectID).Return (_relatedObject.ID);

      _relatedObject2 = DomainObjectMother.CreateFakeObject<OrderTicket> ();
      _relatedEndPointStub2 = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _relatedEndPointStub2.Stub (stub => stub.ObjectID).Return (_relatedObject2.ID);
    }

    [Test]
    public void EnsureDataComplete ()
    {
      var newStateMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPointLoadState> ();

      _endPointLoaderMock
          .Expect (mock => mock.LoadEndPointAndGetNewState (_virtualObjectEndPointMock))
          .Return (newStateMock);
      _endPointLoaderMock.Replay ();
      _virtualObjectEndPointMock.Replay ();
      newStateMock.Replay ();

      _loadState.EnsureDataComplete (_virtualObjectEndPointMock);

      _endPointLoaderMock.VerifyAllExpectations ();
      _virtualObjectEndPointMock.VerifyAllExpectations ();
      newStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void MarkDataComplete_CreatesNewDataManager ()
    {
      bool stateSetterCalled = false;

      _virtualObjectEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _virtualObjectEndPointMock.Replay ();

      var newManagerMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPointDataManager> ();
      newManagerMock.Replay ();
      _dataManagerFactoryStub.Stub (stub => stub.CreateEndPointDataManager (_endPointID)).Return (newManagerMock);

      _loadState.MarkDataComplete (
          _virtualObjectEndPointMock,
          null,
          dataManager =>
          {
            stateSetterCalled = true;
            Assert.That (dataManager, Is.SameAs (newManagerMock));
          });

      Assert.That (stateSetterCalled, Is.True);
      newManagerMock.VerifyAllExpectations ();
    }

    [Test]
    public void MarkDataComplete_EndPointsWithoutItem_IsRegisteredAfterStateSetter ()
    {
      bool stateSetterCalled = false;

      AddOriginalOppositeEndPoint (_loadState, _relatedEndPointStub);
      AddOriginalOppositeEndPoint (_loadState, _relatedEndPointStub2);

      _virtualObjectEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      // ReSharper disable AccessToModifiedClosure
      _virtualObjectEndPointMock
          .Expect (mock => mock.RegisterOriginalOppositeEndPoint (_relatedEndPointStub))
          .WhenCalled (mi => Assert.That (stateSetterCalled, Is.True));
      _virtualObjectEndPointMock
          .Expect (mock => mock.RegisterOriginalOppositeEndPoint (_relatedEndPointStub2))
          .WhenCalled (mi => Assert.That (stateSetterCalled, Is.True));
      // ReSharper restore AccessToModifiedClosure
      _virtualObjectEndPointMock.Replay ();

      var newManagerStub = MockRepository.GenerateStub<IVirtualObjectEndPointDataManager> ();
      _dataManagerFactoryStub.Stub (stub => stub.CreateEndPointDataManager (_endPointID)).Return (newManagerStub);

      _loadState.MarkDataComplete (_virtualObjectEndPointMock, null, dataManager => stateSetterCalled = true);

      _virtualObjectEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void MarkDataComplete_ItemWithoutEndPoint ()
    {
      var item = DomainObjectMother.CreateFakeObject<Order> ();

      _virtualObjectEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _virtualObjectEndPointMock.Replay ();

      var newManagerMock = MockRepository.GenerateMock<IVirtualObjectEndPointDataManager> ();
      using (newManagerMock.GetMockRepository ().Ordered ())
      {
        newManagerMock.Expect (mock => mock.RegisterOriginalItemWithoutEndPoint (item));
      }
      newManagerMock.Replay ();

      _dataManagerFactoryStub.Stub (stub => stub.CreateEndPointDataManager (_endPointID)).Return (newManagerMock);

      _loadState.MarkDataComplete (_virtualObjectEndPointMock, item, dataManager => { });

      newManagerMock.VerifyAllExpectations ();
      _virtualObjectEndPointMock.AssertWasNotCalled (mock => mock.RegisterOriginalOppositeEndPoint (Arg<IRealObjectEndPoint>.Is.Anything));
    }
    
    [Test]
    public void MarkDataComplete_ItemWithEndPoint ()
    {
      var item = DomainObjectMother.CreateFakeObject<Order> ();

      var oppositeEndPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint> ();
      oppositeEndPointMock.Stub (stub => stub.ObjectID).Return (item.ID);
      oppositeEndPointMock.Stub (stub => stub.ResetSyncState());
      oppositeEndPointMock.Expect (mock => mock.MarkSynchronized ());
      oppositeEndPointMock.Replay ();

      AddOriginalOppositeEndPoint (_loadState, oppositeEndPointMock);

      _virtualObjectEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _virtualObjectEndPointMock.Replay ();

      var newManagerMock = MockRepository.GenerateMock<IVirtualObjectEndPointDataManager> ();
      using (newManagerMock.GetMockRepository ().Ordered ())
      {
        newManagerMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (oppositeEndPointMock));
      }
      newManagerMock.Replay ();

      _dataManagerFactoryStub.Stub (stub => stub.CreateEndPointDataManager (_endPointID)).Return (newManagerMock);

      _loadState.MarkDataComplete (_virtualObjectEndPointMock, item, dataManager => { });

      newManagerMock.VerifyAllExpectations ();
      oppositeEndPointMock.VerifyAllExpectations ();
      _virtualObjectEndPointMock.AssertWasNotCalled (mock => mock.RegisterOriginalOppositeEndPoint (Arg<IRealObjectEndPoint>.Is.Anything));
    }

    [Test]
    public void CreateSetCommand ()
    {
      CheckOperationDelegatesToCompleteState (
          s => s.CreateSetCommand (_virtualObjectEndPointMock, _relatedObject),
          MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void CreateSetCommand_Null ()
    {
      CheckOperationDelegatesToCompleteState (
          s => s.CreateSetCommand (_virtualObjectEndPointMock, null),
          MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      CheckOperationDelegatesToCompleteState (
          s => s.CreateDeleteCommand (_virtualObjectEndPointMock),
          MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var lazyLoader = new SerializableVirtualEndPointLoaderFake<
          IVirtualObjectEndPoint, 
          DomainObject, 
          IVirtualObjectEndPointDataManager, 
          IVirtualObjectEndPointLoadState>();
      var dataManagerFactory = new SerializableVirtualObjectEndPointDataManagerFactoryFake ();

      var state = new IncompleteVirtualObjectEndPointLoadState (lazyLoader, dataManagerFactory);
      AddOriginalOppositeEndPoint (state, new SerializableRealObjectEndPointFake (null, _relatedObject));

      var result = FlattenedSerializer.SerializeAndDeserialize (state);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.OriginalOppositeEndPoints, Is.Not.Null);
      Assert.That (result.OriginalOppositeEndPoints, Is.Not.Empty);
      Assert.That (result.EndPointLoader, Is.Not.Null);
      Assert.That (result.DataManagerFactory, Is.Not.Null);
    }

    private void CheckOperationDelegatesToCompleteState<T> (Func<IVirtualObjectEndPointLoadState, T> operation, T fakeResult)
    {
      var newStateMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPointLoadState> ();
      _endPointLoaderMock
          .Expect (mock => mock.LoadEndPointAndGetNewState (_virtualObjectEndPointMock))
          .Return (newStateMock);
      newStateMock
          .Expect (mock => operation (mock))
          .Return (fakeResult);

      _endPointLoaderMock.Replay ();
      newStateMock.Replay ();

      var result = operation (_loadState);

      _endPointLoaderMock.VerifyAllExpectations ();
      newStateMock.Replay ();
      Assert.That (result, Is.EqualTo (fakeResult));
    }

    private void AddOriginalOppositeEndPoint (IncompleteVirtualObjectEndPointLoadState loadState, IRealObjectEndPoint oppositeEndPoint)
    {
      var dictionary = (Dictionary<ObjectID, IRealObjectEndPoint>) PrivateInvoke.GetNonPublicField (loadState, "_originalOppositeEndPoints");
      dictionary.Add (oppositeEndPoint.ObjectID, oppositeEndPoint);
    }
  }
}