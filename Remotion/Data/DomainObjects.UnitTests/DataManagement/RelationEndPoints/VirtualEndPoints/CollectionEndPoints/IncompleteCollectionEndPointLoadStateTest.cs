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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.Serialization;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  [TestFixture]
  public class IncompleteCollectionEndPointLoadStateTest : StandardMappingTest
  {
    private RelationEndPointID _endPointID;
    private ICollectionEndPoint _collectionEndPointMock;

    private IncompleteCollectionEndPointLoadState.IEndPointLoader _endPointLoaderMock;
    private ICollectionEndPointDataManagerFactory _dataManagerFactoryStub;

    private IncompleteCollectionEndPointLoadState _loadState;

    private Order _relatedObject;
    private IRealObjectEndPoint _relatedEndPointStub;

    private Order _relatedObject2;
    private IRealObjectEndPoint _relatedEndPointStub2;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _endPointID = RelationEndPointID.Create (DomainObjectIDs.Customer1, typeof (Customer), "Orders");
      _collectionEndPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
    
      _endPointLoaderMock = MockRepository.GenerateStrictMock<IncompleteCollectionEndPointLoadState.IEndPointLoader> ();
      _dataManagerFactoryStub = MockRepository.GenerateStub<ICollectionEndPointDataManagerFactory> ();
      
      var dataManagerStub = MockRepository.GenerateStub<ICollectionEndPointDataManager> ();
      dataManagerStub.Stub (stub => stub.HasDataChanged()).Return (false);

      _loadState = new IncompleteCollectionEndPointLoadState (_endPointLoaderMock, _dataManagerFactoryStub);

      _relatedObject = DomainObjectMother.CreateFakeObject<Order> ();
      _relatedEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _relatedEndPointStub.Stub (stub => stub.ObjectID).Return (_relatedObject.ID);

      _relatedObject2 = DomainObjectMother.CreateFakeObject<Order> ();
      _relatedEndPointStub2 = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _relatedEndPointStub2.Stub (stub => stub.ObjectID).Return (_relatedObject2.ID);
    }

    [Test]
    public void HasChangedFast ()
    {
      Assert.That (_loadState.HasChangedFast(), Is.False);
    }

    [Test]
    public void EnsureDataComplete ()
    {
      var newStateMock = MockRepository.GenerateStrictMock<ICollectionEndPointLoadState> ();

      _endPointLoaderMock
          .Expect (mock => mock.LoadEndPointAndGetNewState (_collectionEndPointMock))
          .Return (newStateMock);
      _endPointLoaderMock.Replay ();
      _collectionEndPointMock.Replay ();
      newStateMock.Replay();

      _loadState.EnsureDataComplete (_collectionEndPointMock);

      _endPointLoaderMock.VerifyAllExpectations ();
      _collectionEndPointMock.VerifyAllExpectations ();
      newStateMock.VerifyAllExpectations();
    }

    [Test]
    public void MarkDataComplete_CreatesNewDataManager ()
    {
      bool stateSetterCalled = false;

      _collectionEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _collectionEndPointMock.Stub (stub => stub.GetCollectionEventRaiser ()).Return (MockRepository.GenerateStub<IDomainObjectCollectionEventRaiser>());
      _collectionEndPointMock.Replay ();

      var newManagerMock = MockRepository.GenerateStrictMock<ICollectionEndPointDataManager> ();
      newManagerMock.Replay ();
      _dataManagerFactoryStub.Stub (stub => stub.CreateEndPointDataManager (_endPointID)).Return (newManagerMock);

      _loadState.MarkDataComplete (
          _collectionEndPointMock,
          new DomainObject[0],
          dataManager =>
          {
            stateSetterCalled = true;
            Assert.That (dataManager, Is.SameAs (newManagerMock));
          });

      Assert.That (stateSetterCalled, Is.True);
      newManagerMock.VerifyAllExpectations ();
    }

    [Test]
    public void MarkDataComplete_EndPointsWithoutItems_AreRegisteredAfterStateSetter ()
    {
      bool stateSetterCalled = false;

      _loadState.RegisterOriginalOppositeEndPoint (_collectionEndPointMock, _relatedEndPointStub);
      _loadState.RegisterOriginalOppositeEndPoint (_collectionEndPointMock, _relatedEndPointStub2);
      
      _collectionEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _collectionEndPointMock.Stub (stub => stub.GetCollectionEventRaiser ()).Return (MockRepository.GenerateStub<IDomainObjectCollectionEventRaiser> ());
      // ReSharper disable AccessToModifiedClosure
      _collectionEndPointMock
          .Expect (mock => mock.RegisterOriginalOppositeEndPoint (_relatedEndPointStub))
          .WhenCalled (mi => Assert.That (stateSetterCalled, Is.True));
      _collectionEndPointMock
          .Expect (mock => mock.RegisterOriginalOppositeEndPoint (_relatedEndPointStub2))
          .WhenCalled (mi => Assert.That (stateSetterCalled, Is.True));
      // ReSharper restore AccessToModifiedClosure
      _collectionEndPointMock.Replay ();

      var newManagerStub = MockRepository.GenerateStub<ICollectionEndPointDataManager> ();
      _dataManagerFactoryStub.Stub (stub => stub.CreateEndPointDataManager (_endPointID)).Return (newManagerStub);

      _loadState.MarkDataComplete (_collectionEndPointMock, new DomainObject[0], dataManager => stateSetterCalled = true);

      _collectionEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void MarkDataComplete_Items_AreRegisteredInOrder_WithOrWithoutEndPoints ()
    {
      var oppositeEndPointForItem1Mock = MockRepository.GenerateStrictMock<IRealObjectEndPoint> ();
      oppositeEndPointForItem1Mock.Stub (stub => stub.ObjectID).Return (_relatedObject.ID);
      oppositeEndPointForItem1Mock.Stub (stub => stub.ResetSyncState());
      oppositeEndPointForItem1Mock.Expect (mock => mock.MarkSynchronized ());
      oppositeEndPointForItem1Mock.Replay ();

      _loadState.RegisterOriginalOppositeEndPoint (_collectionEndPointMock, oppositeEndPointForItem1Mock);
      
      _collectionEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _collectionEndPointMock.Stub (stub => stub.GetCollectionEventRaiser ()).Return (MockRepository.GenerateStub<IDomainObjectCollectionEventRaiser> ());
      _collectionEndPointMock.Replay ();
      
      var newManagerMock = MockRepository.GenerateMock<ICollectionEndPointDataManager> ();
      using (newManagerMock.GetMockRepository ().Ordered ())
      {
        newManagerMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (oppositeEndPointForItem1Mock));
        newManagerMock.Expect (mock => mock.RegisterOriginalItemWithoutEndPoint (_relatedObject2));
      }
      newManagerMock.Replay ();

      _dataManagerFactoryStub.Stub (stub => stub.CreateEndPointDataManager (_endPointID)).Return (newManagerMock);

      _loadState.MarkDataComplete (_collectionEndPointMock, new DomainObject[] { _relatedObject, _relatedObject2 }, dataManager => { });

      newManagerMock.VerifyAllExpectations ();
      oppositeEndPointForItem1Mock.VerifyAllExpectations ();
      _collectionEndPointMock.AssertWasNotCalled (mock => mock.RegisterOriginalOppositeEndPoint (Arg<IRealObjectEndPoint>.Is.Anything));
    }

    [Test]
    public void MarkDataComplete_RaisesEvent ()
    {
      var counter = new OrderedExpectationCounter ();

      _loadState.RegisterOriginalOppositeEndPoint (_collectionEndPointMock, _relatedEndPointStub);

      var eventRaiserMock = MockRepository.GenerateStrictMock<IDomainObjectCollectionEventRaiser> ();

      _collectionEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _collectionEndPointMock.Stub (stub => stub.GetCollectionEventRaiser()).Return (eventRaiserMock);
      _collectionEndPointMock.Replay ();

      var newManagerMock = MockRepository.GenerateMock<ICollectionEndPointDataManager> ();
      newManagerMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (_relatedEndPointStub)).Ordered (counter);
      newManagerMock.Expect (mock => mock.RegisterOriginalItemWithoutEndPoint (_relatedObject2)).Ordered (counter);
      newManagerMock.Replay ();

      _dataManagerFactoryStub.Stub (stub => stub.CreateEndPointDataManager (_endPointID)).Return (newManagerMock);

      var expectedManagerPosition = counter.GetNextExpectedPosition ();
      Action<ICollectionEndPointDataManager> stateSetter = dataManager => counter.CheckPosition ("stateSetter", expectedManagerPosition);

      eventRaiserMock.Expect (mock => mock.WithinReplaceData()).Ordered(counter);
      eventRaiserMock.Replay();

      _loadState.MarkDataComplete (_collectionEndPointMock, new DomainObject[] { _relatedObject, _relatedObject2 }, stateSetter);

      newManagerMock.VerifyAllExpectations ();
      eventRaiserMock.VerifyAllExpectations();
    }

    [Test]
    public void SortCurrentData ()
    {
      Comparison<DomainObject> comparison = (one, two) => 0;
      CheckOperationDelegatesToCompleteState (
          s => s.SortCurrentData (_collectionEndPointMock, comparison));
    }

    [Test]
    public void CreateSetCollectionCommand ()
    {
      var domainObjectCollection = new DomainObjectCollection ();

      var fakeManager = MockRepository.GenerateStub<ICollectionEndPointCollectionManager>();
      CheckOperationDelegatesToCompleteState (
          s => s.CreateSetCollectionCommand (_collectionEndPointMock, domainObjectCollection, fakeManager),
          MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void CreateRemoveCommand ()
    {
      CheckOperationDelegatesToCompleteState (
          s => s.CreateRemoveCommand (_collectionEndPointMock, _relatedObject), 
          MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      CheckOperationDelegatesToCompleteState (
          s => s.CreateDeleteCommand (_collectionEndPointMock), 
          MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void CreateInsertCommand ()
    {
      CheckOperationDelegatesToCompleteState (
          s => s.CreateInsertCommand (_collectionEndPointMock, _relatedObject, 0), 
          MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void CreateAddCommand ()
    {
      CheckOperationDelegatesToCompleteState (
          s => s.CreateAddCommand (_collectionEndPointMock, _relatedObject), 
          MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void CreateReplaceCommand ()
    {
      CheckOperationDelegatesToCompleteState (
          s => s.CreateReplaceCommand (_collectionEndPointMock, 0, _relatedObject), 
          MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var endPointLoader = new SerializableVirtualEndPointLoaderFake<
          ICollectionEndPoint, 
          ReadOnlyCollectionDataDecorator, 
          ICollectionEndPointDataManager, 
          ICollectionEndPointLoadState> ();
      var dataManagerFactory = new SerializableCollectionEndPointDataManagerFactoryFake();

      var state = new IncompleteCollectionEndPointLoadState (endPointLoader, dataManagerFactory);

      var oppositeEndPoint = new SerializableRealObjectEndPointFake(null, _relatedObject);
      state.RegisterOriginalOppositeEndPoint (_collectionEndPointMock, oppositeEndPoint);

      var result = FlattenedSerializer.SerializeAndDeserialize (state);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.OriginalOppositeEndPoints, Is.Not.Null);
      Assert.That (result.OriginalOppositeEndPoints, Is.Not.Empty);
      Assert.That (result.EndPointLoader, Is.Not.Null);
      Assert.That (result.DataManagerFactory, Is.Not.Null);
    }
    
    private void CheckOperationDelegatesToCompleteState<T> (Func<ICollectionEndPointLoadState, T> operation, T fakeResult)
    {
      var newStateMock = MockRepository.GenerateStrictMock<ICollectionEndPointLoadState> ();
      _endPointLoaderMock
          .Expect (mock => mock.LoadEndPointAndGetNewState (_collectionEndPointMock))
          .Return (newStateMock);
      newStateMock
          .Expect (mock => operation (mock))
          .Return (fakeResult);
      
      _endPointLoaderMock.Replay ();
      newStateMock.Replay();

      var result = operation (_loadState);

      _endPointLoaderMock.VerifyAllExpectations ();
      newStateMock.Replay();
      Assert.That (result, Is.EqualTo (fakeResult));
    }

    private void CheckOperationDelegatesToCompleteState (
        Action<ICollectionEndPointLoadState> operation)
    {
      var newStateMock = MockRepository.GenerateStrictMock<ICollectionEndPointLoadState> ();
      _endPointLoaderMock
          .Expect (mock => mock.LoadEndPointAndGetNewState (_collectionEndPointMock))
          .Return (newStateMock);
      newStateMock.Expect (operation);

      _endPointLoaderMock.Replay ();
      newStateMock.Replay ();

      operation (_loadState);

      _endPointLoaderMock.VerifyAllExpectations ();
      newStateMock.Replay ();
    }
  }
}