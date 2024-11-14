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
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  [TestFixture]
  public class IncompleteDomainObjectCollectionEndPointLoadStateTest : StandardMappingTest
  {
    private RelationEndPointID _endPointID;
    private Mock<IDomainObjectCollectionEndPoint> _collectionEndPointMock;

    private Mock<IncompleteDomainObjectCollectionEndPointLoadState.IEndPointLoader> _endPointLoaderMock;
    private Mock<IDomainObjectCollectionEndPointDataManagerFactory> _dataManagerFactoryStub;

    private IncompleteDomainObjectCollectionEndPointLoadState _loadState;

    private Order _relatedObject;
    private Mock<IRealObjectEndPoint> _relatedEndPointStub;

    private Order _relatedObject2;
    private Mock<IRealObjectEndPoint> _relatedEndPointStub2;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _endPointID = RelationEndPointID.Create(DomainObjectIDs.Customer1, typeof(Customer), "Orders");
      _collectionEndPointMock = new Mock<IDomainObjectCollectionEndPoint>(MockBehavior.Strict);

      _endPointLoaderMock = new Mock<IncompleteDomainObjectCollectionEndPointLoadState.IEndPointLoader>(MockBehavior.Strict);
      _dataManagerFactoryStub = new Mock<IDomainObjectCollectionEndPointDataManagerFactory>();

      var dataManagerStub = new Mock<IDomainObjectCollectionEndPointDataManager>();
      dataManagerStub.Setup(stub => stub.HasDataChanged()).Returns(false);

      _loadState = new IncompleteDomainObjectCollectionEndPointLoadState(_endPointLoaderMock.Object, _dataManagerFactoryStub.Object);

      _relatedObject = DomainObjectMother.CreateFakeObject<Order>();
      _relatedEndPointStub = new Mock<IRealObjectEndPoint>();
      _relatedEndPointStub.Setup(stub => stub.ObjectID).Returns(_relatedObject.ID);

      _relatedObject2 = DomainObjectMother.CreateFakeObject<Order>();
      _relatedEndPointStub2 = new Mock<IRealObjectEndPoint>();
      _relatedEndPointStub2.Setup(stub => stub.ObjectID).Returns(_relatedObject2.ID);
    }

    [Test]
    public void HasChangedFast ()
    {
      Assert.That(_loadState.HasChangedFast(), Is.False);
    }

    [Test]
    public void EnsureDataComplete ()
    {
      var newStateMock = new Mock<IDomainObjectCollectionEndPointLoadState>(MockBehavior.Strict);

      _endPointLoaderMock
          .Setup(mock => mock.LoadEndPointAndGetNewState(_collectionEndPointMock.Object))
          .Returns(newStateMock.Object)
          .Verifiable();

      _loadState.EnsureDataComplete(_collectionEndPointMock.Object);

      _endPointLoaderMock.Verify();
      _collectionEndPointMock.Verify();
      newStateMock.Verify();
    }

    [Test]
    public void MarkDataComplete_CreatesNewDataManager ()
    {
      bool stateSetterCalled = false;

      _collectionEndPointMock.Setup(stub => stub.ID).Returns(_endPointID);
      _collectionEndPointMock.Setup(stub => stub.GetCollectionEventRaiser()).Returns(new Mock<IDomainObjectCollectionEventRaiser>().Object);

      var newManagerMock = new Mock<IDomainObjectCollectionEndPointDataManager>(MockBehavior.Strict);
      _dataManagerFactoryStub.Setup(stub => stub.CreateEndPointDataManager(_endPointID)).Returns(newManagerMock.Object);

      _loadState.MarkDataComplete(
          _collectionEndPointMock.Object,
          new DomainObject[0],
          dataManager =>
          {
            stateSetterCalled = true;
            Assert.That(dataManager, Is.SameAs(newManagerMock.Object));
          });

      Assert.That(stateSetterCalled, Is.True);
      newManagerMock.Verify();
    }

    [Test]
    public void MarkDataComplete_EndPointsWithoutItems_AreRegisteredAfterStateSetter ()
    {
      bool stateSetterCalled = false;

      _loadState.RegisterOriginalOppositeEndPoint(_collectionEndPointMock.Object, _relatedEndPointStub.Object);
      _loadState.RegisterOriginalOppositeEndPoint(_collectionEndPointMock.Object, _relatedEndPointStub2.Object);

      _collectionEndPointMock.Setup(stub => stub.ID).Returns(_endPointID);
      _collectionEndPointMock.Setup(stub => stub.GetCollectionEventRaiser()).Returns(new Mock<IDomainObjectCollectionEventRaiser>().Object);
      // ReSharper disable AccessToModifiedClosure
      _collectionEndPointMock
          .Setup(mock => mock.RegisterOriginalOppositeEndPoint(_relatedEndPointStub.Object))
          .Callback((IRealObjectEndPoint oppositeEndPoint) => Assert.That(stateSetterCalled, Is.True))
          .Verifiable();
      _collectionEndPointMock
          .Setup(mock => mock.RegisterOriginalOppositeEndPoint(_relatedEndPointStub2.Object))
          .Callback((IRealObjectEndPoint oppositeEndPoint) => Assert.That(stateSetterCalled, Is.True))
          .Verifiable();
      // ReSharper restore AccessToModifiedClosure

      var newManagerStub = new Mock<IDomainObjectCollectionEndPointDataManager>();
      _dataManagerFactoryStub.Setup(stub => stub.CreateEndPointDataManager(_endPointID)).Returns(newManagerStub.Object);

      _loadState.MarkDataComplete(_collectionEndPointMock.Object, new DomainObject[0], dataManager => stateSetterCalled = true);

      _collectionEndPointMock.Verify();
    }

    [Test]
    public void MarkDataComplete_Items_AreRegisteredInOrder_WithOrWithoutEndPoints ()
    {
      var oppositeEndPointForItem1Mock = new Mock<IRealObjectEndPoint>(MockBehavior.Strict);
      oppositeEndPointForItem1Mock.Setup(stub => stub.IsNull).Returns(false);
      oppositeEndPointForItem1Mock.Setup(stub => stub.ObjectID).Returns(_relatedObject.ID);
      oppositeEndPointForItem1Mock.Setup(stub => stub.ResetSyncState());
      oppositeEndPointForItem1Mock.Setup(mock => mock.MarkSynchronized()).Verifiable();

      _loadState.RegisterOriginalOppositeEndPoint(_collectionEndPointMock.Object, oppositeEndPointForItem1Mock.Object);

      _collectionEndPointMock.Setup(stub => stub.ID).Returns(_endPointID);
      _collectionEndPointMock.Setup(stub => stub.GetCollectionEventRaiser()).Returns(new Mock<IDomainObjectCollectionEventRaiser>().Object);

      var newManagerMock = new Mock<IDomainObjectCollectionEndPointDataManager>();
      var sequence = new VerifiableSequence();
      newManagerMock.InVerifiableSequence(sequence).Setup(mock => mock.RegisterOriginalOppositeEndPoint(oppositeEndPointForItem1Mock.Object)).Verifiable();
      newManagerMock.InVerifiableSequence(sequence).Setup(mock => mock.RegisterOriginalItemWithoutEndPoint(_relatedObject2)).Verifiable();

      _dataManagerFactoryStub.Setup(stub => stub.CreateEndPointDataManager(_endPointID)).Returns(newManagerMock.Object);

      _loadState.MarkDataComplete(_collectionEndPointMock.Object, new DomainObject[] { _relatedObject, _relatedObject2 }, dataManager => { });

      newManagerMock.Verify();
      oppositeEndPointForItem1Mock.Verify();
      _collectionEndPointMock.Verify(mock => mock.RegisterOriginalOppositeEndPoint(It.IsAny<IRealObjectEndPoint>()), Times.Never());
      sequence.Verify();
    }

    [Test]
    public void MarkDataComplete_RaisesEvent ()
    {
      var sequence = new VerifiableSequence();

      _loadState.RegisterOriginalOppositeEndPoint(_collectionEndPointMock.Object, _relatedEndPointStub.Object);

      var eventRaiserMock = new Mock<IDomainObjectCollectionEventRaiser>(MockBehavior.Strict);

      _collectionEndPointMock.Setup(stub => stub.ID).Returns(_endPointID);
      _collectionEndPointMock.Setup(stub => stub.GetCollectionEventRaiser()).Returns(eventRaiserMock.Object);

      var newManagerMock = new Mock<IDomainObjectCollectionEndPointDataManager>();
      newManagerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RegisterOriginalOppositeEndPoint(_relatedEndPointStub.Object))
          .Verifiable();
      newManagerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RegisterOriginalItemWithoutEndPoint(_relatedObject2))
          .Verifiable();

      _dataManagerFactoryStub.Setup(stub => stub.CreateEndPointDataManager(_endPointID)).Returns(newManagerMock.Object);

      var setterActionMock = new Mock<ISetterAction>();
      setterActionMock.InVerifiableSequence(sequence).Setup(_ => _.Do()).Verifiable();

      Action<IDomainObjectCollectionEndPointDataManager> stateSetter = dataManager => setterActionMock.Object.Do();
      eventRaiserMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.WithinReplaceData())
          .Verifiable();

      _loadState.MarkDataComplete(_collectionEndPointMock.Object, new DomainObject[] { _relatedObject, _relatedObject2 }, stateSetter);

      newManagerMock.Verify();
      eventRaiserMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void SortCurrentData ()
    {
      Comparison<DomainObject> comparison = (one, two) => 0;
      CheckOperationDelegatesToCompleteState(
          s => s.SortCurrentData(_collectionEndPointMock.Object, comparison));
    }

    [Test]
    public void CreateSetCollectionCommand ()
    {
      var domainObjectCollection = new DomainObjectCollection();

      var fakeManager = new Mock<IDomainObjectCollectionEndPointCollectionManager>();
      CheckOperationDelegatesToCompleteState(
          s => s.CreateSetCollectionCommand(_collectionEndPointMock.Object, domainObjectCollection, fakeManager.Object),
          new Mock<IDataManagementCommand>().Object);
    }

    [Test]
    public void CreateRemoveCommand ()
    {
      CheckOperationDelegatesToCompleteState(
          s => s.CreateRemoveCommand(_collectionEndPointMock.Object, _relatedObject),
          new Mock<IDataManagementCommand>().Object);
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      CheckOperationDelegatesToCompleteState(
          s => s.CreateDeleteCommand(_collectionEndPointMock.Object),
          new Mock<IDataManagementCommand>().Object);
    }

    [Test]
    public void CreateInsertCommand ()
    {
      CheckOperationDelegatesToCompleteState(
          s => s.CreateInsertCommand(_collectionEndPointMock.Object, _relatedObject, 0),
          new Mock<IDataManagementCommand>().Object);
    }

    [Test]
    public void CreateAddCommand ()
    {
      CheckOperationDelegatesToCompleteState(
          s => s.CreateAddCommand(_collectionEndPointMock.Object, _relatedObject),
          new Mock<IDataManagementCommand>().Object);
    }

    [Test]
    public void CreateReplaceCommand ()
    {
      CheckOperationDelegatesToCompleteState(
          s => s.CreateReplaceCommand(_collectionEndPointMock.Object, 0, _relatedObject),
          new Mock<IDataManagementCommand>().Object);
    }

    private void CheckOperationDelegatesToCompleteState<T> (Expression<Func<IDomainObjectCollectionEndPointLoadState, T>> operation, T fakeResult)
    {
      var newStateMock = new Mock<IDomainObjectCollectionEndPointLoadState>(MockBehavior.Strict);
      _endPointLoaderMock
          .Setup(mock => mock.LoadEndPointAndGetNewState(_collectionEndPointMock.Object))
          .Returns(newStateMock.Object)
          .Verifiable();
      newStateMock
          .Setup(operation)
          .Returns(fakeResult)
          .Verifiable();

      var compiledOperation = operation.Compile();
      var result = compiledOperation(_loadState);

      _endPointLoaderMock.Verify();
      Assert.That(result, Is.EqualTo(fakeResult));
    }

    private void CheckOperationDelegatesToCompleteState (
        Expression<Action<IDomainObjectCollectionEndPointLoadState>> operation)
    {
      var newStateMock = new Mock<IDomainObjectCollectionEndPointLoadState>(MockBehavior.Strict);
      _endPointLoaderMock
          .Setup(mock => mock.LoadEndPointAndGetNewState(_collectionEndPointMock.Object))
          .Returns(newStateMock.Object)
          .Verifiable();
      newStateMock.Setup(operation).Verifiable();

      var compiledOperation = operation.Compile();
      compiledOperation(_loadState);

      _endPointLoaderMock.Verify();
    }

    public interface ISetterAction
    {
      void Do ();
    }
  }
}
