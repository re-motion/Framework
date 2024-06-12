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
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints
{
  [TestFixture]
  public class IncompleteVirtualObjectEndPointLoadStateTest : StandardMappingTest
  {
    private RelationEndPointID _endPointID;
    private Mock<IVirtualObjectEndPoint> _virtualObjectEndPointMock;

    private Mock<IncompleteVirtualEndPointLoadStateBase<IVirtualObjectEndPoint, DomainObject, IVirtualObjectEndPointDataManager,
        IVirtualObjectEndPointLoadState>.IEndPointLoader> _endPointLoaderMock;
    private Mock<IVirtualObjectEndPointDataManagerFactory> _dataManagerFactoryStub;

    private IncompleteVirtualObjectEndPointLoadState _loadState;

    private OrderTicket _relatedObject;
    private Mock<IRealObjectEndPoint> _relatedEndPointStub;

    private OrderTicket _relatedObject2;
    private Mock<IRealObjectEndPoint> _relatedEndPointStub2;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderTicket");
      _virtualObjectEndPointMock = new Mock<IVirtualObjectEndPoint>(MockBehavior.Strict);

      _endPointLoaderMock = new Mock<IncompleteVirtualObjectEndPointLoadState.IEndPointLoader>(MockBehavior.Strict);
      _dataManagerFactoryStub = new Mock<IVirtualObjectEndPointDataManagerFactory>();

      var dataManagerStub = new Mock<IVirtualObjectEndPointDataManager>();
      dataManagerStub.Setup(stub => stub.HasDataChanged()).Returns(false);
      _loadState = new IncompleteVirtualObjectEndPointLoadState(_endPointLoaderMock.Object, _dataManagerFactoryStub.Object);

      _relatedObject = DomainObjectMother.CreateFakeObject<OrderTicket>();
      _relatedEndPointStub = new Mock<IRealObjectEndPoint>();
      _relatedEndPointStub.Setup(stub => stub.ObjectID).Returns(_relatedObject.ID);

      _relatedObject2 = DomainObjectMother.CreateFakeObject<OrderTicket>();
      _relatedEndPointStub2 = new Mock<IRealObjectEndPoint>();
      _relatedEndPointStub2.Setup(stub => stub.ObjectID).Returns(_relatedObject2.ID);
    }

    [Test]
    public void EnsureDataComplete ()
    {
      var newStateMock = new Mock<IVirtualObjectEndPointLoadState>(MockBehavior.Strict);

      _endPointLoaderMock
          .Setup(mock => mock.LoadEndPointAndGetNewState(_virtualObjectEndPointMock.Object))
          .Returns(newStateMock.Object)
          .Verifiable();

      _loadState.EnsureDataComplete(_virtualObjectEndPointMock.Object);

      _endPointLoaderMock.Verify();
      _virtualObjectEndPointMock.Verify();
      newStateMock.Verify();
    }

    [Test]
    public void MarkDataComplete_CreatesNewDataManager ()
    {
      bool stateSetterCalled = false;

      _virtualObjectEndPointMock.Setup(stub => stub.ID).Returns(_endPointID);

      var newManagerMock = new Mock<IVirtualObjectEndPointDataManager>(MockBehavior.Strict);
      _dataManagerFactoryStub.Setup(stub => stub.CreateEndPointDataManager(_endPointID)).Returns(newManagerMock.Object);

      _loadState.MarkDataComplete(
          _virtualObjectEndPointMock.Object,
          null,
          dataManager =>
          {
            stateSetterCalled = true;
            Assert.That(dataManager, Is.SameAs(newManagerMock.Object));
          });

      Assert.That(stateSetterCalled, Is.True);
      newManagerMock.Verify();
    }

    [Test]
    public void MarkDataComplete_EndPointsWithoutItem_IsRegisteredAfterStateSetter ()
    {
      bool stateSetterCalled = false;

      AddOriginalOppositeEndPoint(_loadState, _relatedEndPointStub.Object);
      AddOriginalOppositeEndPoint(_loadState, _relatedEndPointStub2.Object);

      _virtualObjectEndPointMock.Setup(stub => stub.ID).Returns(_endPointID);
      // ReSharper disable AccessToModifiedClosure
      _virtualObjectEndPointMock
          .Setup(mock => mock.RegisterOriginalOppositeEndPoint(_relatedEndPointStub.Object))
          .Callback((IRealObjectEndPoint oppositeEndPoint) => Assert.That(stateSetterCalled, Is.True))
          .Verifiable();
      _virtualObjectEndPointMock
          .Setup(mock => mock.RegisterOriginalOppositeEndPoint(_relatedEndPointStub2.Object))
          .Callback((IRealObjectEndPoint oppositeEndPoint) => Assert.That(stateSetterCalled, Is.True))
          .Verifiable();

      var newManagerStub = new Mock<IVirtualObjectEndPointDataManager>();
      _dataManagerFactoryStub.Setup(stub => stub.CreateEndPointDataManager(_endPointID)).Returns(newManagerStub.Object);

      _loadState.MarkDataComplete(_virtualObjectEndPointMock.Object, null, dataManager => stateSetterCalled = true);

      _virtualObjectEndPointMock.Verify();
    }

    [Test]
    public void MarkDataComplete_ItemWithoutEndPoint ()
    {
      var item = DomainObjectMother.CreateFakeObject<Order>();

      _virtualObjectEndPointMock.Setup(stub => stub.ID).Returns(_endPointID);

      var newManagerMock = new Mock<IVirtualObjectEndPointDataManager>();
      newManagerMock.Setup(mock => mock.RegisterOriginalItemWithoutEndPoint(item)).Verifiable();

      _dataManagerFactoryStub.Setup(stub => stub.CreateEndPointDataManager(_endPointID)).Returns(newManagerMock.Object);

      _loadState.MarkDataComplete(_virtualObjectEndPointMock.Object, item, dataManager => { });

      newManagerMock.Verify();
      _virtualObjectEndPointMock.Verify(mock => mock.RegisterOriginalOppositeEndPoint(It.IsAny<IRealObjectEndPoint>()), Times.Never());
    }

    [Test]
    public void MarkDataComplete_ItemWithEndPoint ()
    {
      var item = DomainObjectMother.CreateFakeObject<Order>();

      var oppositeEndPointMock = new Mock<IRealObjectEndPoint>(MockBehavior.Strict);
      oppositeEndPointMock.Setup(stub => stub.ObjectID).Returns(item.ID);
      oppositeEndPointMock.Setup(stub => stub.ResetSyncState());
      oppositeEndPointMock.Setup(mock => mock.MarkSynchronized()).Verifiable();

      AddOriginalOppositeEndPoint(_loadState, oppositeEndPointMock.Object);

      _virtualObjectEndPointMock.Setup(stub => stub.ID).Returns(_endPointID);

      var newManagerMock = new Mock<IVirtualObjectEndPointDataManager>();
      newManagerMock.Setup(mock => mock.RegisterOriginalOppositeEndPoint(oppositeEndPointMock.Object)).Verifiable();

      _dataManagerFactoryStub.Setup(stub => stub.CreateEndPointDataManager(_endPointID)).Returns(newManagerMock.Object);

      _loadState.MarkDataComplete(_virtualObjectEndPointMock.Object, item, dataManager => { });

      newManagerMock.Verify();
      oppositeEndPointMock.Verify();
      _virtualObjectEndPointMock.Verify(mock => mock.RegisterOriginalOppositeEndPoint(It.IsAny<IRealObjectEndPoint>()), Times.Never());
    }

    [Test]
    public void CreateSetCommand ()
    {
      CheckOperationDelegatesToCompleteState(
          s => s.CreateSetCommand(_virtualObjectEndPointMock.Object, _relatedObject),
          new Mock<IDataManagementCommand>().Object);
    }

    [Test]
    public void CreateSetCommand_Null ()
    {
      CheckOperationDelegatesToCompleteState(
          s => s.CreateSetCommand(_virtualObjectEndPointMock.Object, null),
          new Mock<IDataManagementCommand>().Object);
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      CheckOperationDelegatesToCompleteState(
          s => s.CreateDeleteCommand(_virtualObjectEndPointMock.Object),
          new Mock<IDataManagementCommand>().Object);
    }

    private void CheckOperationDelegatesToCompleteState<T> (Expression<Func<IVirtualObjectEndPointLoadState, T>> operation, T fakeResult)
    {
      var newStateMock = new Mock<IVirtualObjectEndPointLoadState>(MockBehavior.Strict);
      _endPointLoaderMock
          .Setup(mock => mock.LoadEndPointAndGetNewState(_virtualObjectEndPointMock.Object))
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

    private void AddOriginalOppositeEndPoint (IncompleteVirtualObjectEndPointLoadState loadState, IRealObjectEndPoint oppositeEndPoint)
    {
      var dictionary = (Dictionary<ObjectID, IRealObjectEndPoint>)PrivateInvoke.GetNonPublicField(loadState, "_originalOppositeEndPoints");
      dictionary.Add(oppositeEndPoint.ObjectID, oppositeEndPoint);
    }
  }
}
