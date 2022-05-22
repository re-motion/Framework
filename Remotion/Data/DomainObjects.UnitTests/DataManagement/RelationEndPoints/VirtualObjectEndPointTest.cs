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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.Serialization;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.UnitTesting;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class VirtualObjectEndPointTest : ClientTransactionBaseTest
  {
    private RelationEndPointID _endPointID;

    private Mock<ILazyLoader> _lazyLoaderMock;
    private Mock<IRelationEndPointProvider> _endPointProviderStub;
    private Mock<IClientTransactionEventSink> _transactionEventSinkStub;
    private IVirtualObjectEndPointDataManagerFactory _dataManagerFactory;
    private Mock<IVirtualObjectEndPointLoadState> _loadStateMock;

    private VirtualObjectEndPoint _endPoint;

    private Mock<IRealObjectEndPoint> _oppositeEndPointStub;
    private OrderTicket _oppositeObject;

    public override void SetUp ()
    {
      base.SetUp();

      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "OrderTicket");

      _lazyLoaderMock = new Mock<ILazyLoader>(MockBehavior.Strict);
      _endPointProviderStub = new Mock<IRelationEndPointProvider>();
      _transactionEventSinkStub = new Mock<IClientTransactionEventSink>();
      _dataManagerFactory = new VirtualObjectEndPointDataManagerFactory();
      _loadStateMock = new Mock<IVirtualObjectEndPointLoadState>(MockBehavior.Strict);

      _endPoint = new VirtualObjectEndPoint(
          ClientTransaction.Current,
          _endPointID,
          _lazyLoaderMock.Object,
          _endPointProviderStub.Object,
          _transactionEventSinkStub.Object,
          _dataManagerFactory);
      PrivateInvoke.SetNonPublicField(_endPoint, "_loadState", _loadStateMock.Object);

      _oppositeEndPointStub = new Mock<IRealObjectEndPoint>();
      _oppositeObject = DomainObjectMother.CreateFakeObject<OrderTicket>();
    }

    [Test]
    public void Initialization ()
    {
      var endPoint = new VirtualObjectEndPoint(
          ClientTransaction.Current,
          _endPointID,
          _lazyLoaderMock.Object,
          _endPointProviderStub.Object,
          _transactionEventSinkStub.Object,
          _dataManagerFactory);

      Assert.That(endPoint.ID, Is.EqualTo(_endPointID));
      Assert.That(endPoint.ClientTransaction, Is.SameAs(TestableClientTransaction));
      Assert.That(endPoint.LazyLoader, Is.SameAs(_lazyLoaderMock.Object));
      Assert.That(endPoint.EndPointProvider, Is.SameAs(_endPointProviderStub.Object));
      Assert.That(endPoint.DataManagerFactory, Is.SameAs(_dataManagerFactory));
      Assert.That(endPoint.HasBeenTouched, Is.False);
      Assert.That(endPoint.IsDataComplete, Is.False);

      var loadState = VirtualObjectEndPointTestHelper.GetLoadState(endPoint);
      Assert.That(loadState, Is.TypeOf(typeof(IncompleteVirtualObjectEndPointLoadState)));
      Assert.That(((IncompleteVirtualObjectEndPointLoadState)loadState).DataManagerFactory, Is.SameAs(_dataManagerFactory));
      Assert.That(
          ((IncompleteVirtualObjectEndPointLoadState)loadState).EndPointLoader,
          Is.TypeOf<VirtualObjectEndPoint.EndPointLoader>().With.Property<VirtualObjectEndPoint.EndPointLoader>(l => l.LazyLoader).SameAs(_lazyLoaderMock.Object));
    }

    [Test]
    public void Initialization_NonVirtualDefinition ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.OrderTicket1, "Order");
      Assert.That(
          () => new VirtualObjectEndPoint(
              TestableClientTransaction,
              id,
              _lazyLoaderMock.Object,
              _endPointProviderStub.Object,
              _transactionEventSinkStub.Object,
              _dataManagerFactory),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("End point ID must refer to a virtual end point.", "id"));
    }

    [Test]
    public void OppositeObjectID ()
    {
      _loadStateMock.Setup(mock => mock.GetData(_endPoint)).Returns(_oppositeObject).Verifiable();

      var result = _endPoint.OppositeObjectID;
      _loadStateMock.Verify();

      Assert.That(result, Is.EqualTo(_oppositeObject.ID));
    }

    [Test]
    public void GetData ()
    {
      _loadStateMock.Setup(mock => mock.GetData(_endPoint)).Returns(_oppositeObject).Verifiable();

      var result = ((IVirtualObjectEndPoint)_endPoint).GetData();
      Assert.That(result, Is.SameAs(_oppositeObject));
    }

    [Test]
    public void OriginalOppositeObjectID ()
    {
      _loadStateMock.Setup(mock => mock.GetOriginalData(_endPoint)).Returns(_oppositeObject).Verifiable();

      var result = _endPoint.OriginalOppositeObjectID;
      _loadStateMock.Verify();

      Assert.That(result, Is.EqualTo(_oppositeObject.ID));
    }

    [Test]
    public void GetOriginalData ()
    {
      _loadStateMock.Setup(mock => mock.GetOriginalData(_endPoint)).Returns(_oppositeObject).Verifiable();

      var result = ((IVirtualObjectEndPoint)_endPoint).GetOriginalData();
      Assert.That(result, Is.SameAs(_oppositeObject));
    }

    [Test]
    public void HasChanged ()
    {
      _loadStateMock.Setup(mock => mock.HasChanged()).Returns(true).Verifiable();

      var result = _endPoint.HasChanged;

      _loadStateMock.Verify();
      Assert.That(result, Is.True);
    }

    [Test]
    public void IsDataComplete ()
    {
      _loadStateMock.Setup(mock => mock.IsDataComplete()).Returns(true).Verifiable();

      var result = _endPoint.IsDataComplete;

      _loadStateMock.Verify();
      Assert.That(result, Is.True);
    }

    [Test]
    public void IsSynchronized ()
    {
      _loadStateMock.Setup(mock => mock.IsSynchronized(_endPoint)).Returns(true).Verifiable();

      var result = _endPoint.IsSynchronized;

      _loadStateMock.Verify();
      Assert.That(result, Is.True);
    }

    [Test]
    public void CanBeCollected_WithCanEndPointBeCollectedIsTrue_ReturnsTrue ()
    {
      _loadStateMock.Setup(mock => mock.CanEndPointBeCollected(_endPoint)).Returns(true).Verifiable();

      var result = _endPoint.CanBeCollected;

      _loadStateMock.Verify();
      Assert.That(result, Is.True);
    }

    [Test]
    public void CanBeCollected_WithCanEndPointBeCollectedIsFalse_ReturnsFalse ()
    {
      _loadStateMock.Setup(mock => mock.CanEndPointBeCollected(_endPoint)).Returns(false).Verifiable();

      var result = _endPoint.CanBeCollected;

      _loadStateMock.Verify();
      Assert.That(result, Is.False);
    }

    [Test]
    public void CanBeMarkedIncomplete_WithCanDataBeMarkedIncompleteIsTrue_ReturnsTrue ()
    {
      _loadStateMock.Setup(mock => mock.CanDataBeMarkedIncomplete(_endPoint)).Returns(true).Verifiable();

      var result = _endPoint.CanBeMarkedIncomplete;

      _loadStateMock.Verify();
      Assert.That(result, Is.True);
    }

    [Test]
    public void CanBeMarkedIncomplete_WithCanDataBeMarkedIncompleteIsFalse_ReturnsFalse ()
    {
      _loadStateMock.Setup(mock => mock.CanDataBeMarkedIncomplete(_endPoint)).Returns(false).Verifiable();

      var result = _endPoint.CanBeMarkedIncomplete;

      _loadStateMock.Verify();
      Assert.That(result, Is.False);
    }

    [Test]
    public void GetOppositeObject ()
    {
      _loadStateMock.Setup(mock => mock.GetData(_endPoint)).Returns(_oppositeObject).Verifiable();

      var result = _endPoint.GetOppositeObject();

      _loadStateMock.Verify();
      Assert.That(result, Is.SameAs(_oppositeObject));
    }

    [Test]
    public void GetOriginalOppositeObject ()
    {
      _loadStateMock.Setup(mock => mock.GetOriginalData(_endPoint)).Returns(_oppositeObject).Verifiable();

      var result = _endPoint.GetOriginalOppositeObject();

      _loadStateMock.Verify();
      Assert.That(result, Is.SameAs(_oppositeObject));
    }

    [Test]
    public void EnsureDataComplete ()
    {
      _loadStateMock.Setup(mock => mock.EnsureDataComplete(_endPoint)).Verifiable();

      _endPoint.EnsureDataComplete();

      _loadStateMock.Verify();
    }

    [Test]
    public void Synchronize ()
    {
      _loadStateMock.Setup(mock => mock.Synchronize(_endPoint)).Verifiable();
      _loadStateMock.Setup(mock => mock.HasChanged()).Returns(true);

      _endPoint.Synchronize();

      _loadStateMock.Verify();
    }

    [Test]
    public void SynchronizeOppositeEndPoint ()
    {
      _loadStateMock.Setup(mock => mock.SynchronizeOppositeEndPoint(_endPoint, _oppositeEndPointStub.Object)).Verifiable();
      _loadStateMock.Setup(mock => mock.HasChanged()).Returns(true);

      _endPoint.SynchronizeOppositeEndPoint(_oppositeEndPointStub.Object);

      _loadStateMock.Verify();
    }

    [Test]
    public void MarkDataComplete ()
    {
      Action<IVirtualObjectEndPointDataManager> actualStateSetter = null;

      _loadStateMock
          .Setup(mock => mock.MarkDataComplete(_endPoint, _oppositeObject, It.IsAny<Action<IVirtualObjectEndPointDataManager>>()))
          .Callback((IVirtualObjectEndPoint _, IDomainObject _, Action<IVirtualObjectEndPointDataManager> stateSetter) => { actualStateSetter =stateSetter; })
          .Verifiable();

      _endPoint.MarkDataComplete(_oppositeObject);

      _loadStateMock.Verify();

      Assert.That(VirtualObjectEndPointTestHelper.GetLoadState(_endPoint), Is.SameAs(_loadStateMock.Object));

      var dataManagerStub = new Mock<IVirtualObjectEndPointDataManager>();
      actualStateSetter(dataManagerStub.Object);

      var newLoadState = VirtualObjectEndPointTestHelper.GetLoadState(_endPoint);
      Assert.That(newLoadState, Is.Not.SameAs(_loadStateMock.Object));
      Assert.That(newLoadState, Is.TypeOf(typeof(CompleteVirtualObjectEndPointLoadState)));

      Assert.That(((CompleteVirtualObjectEndPointLoadState)newLoadState).DataManager, Is.SameAs(dataManagerStub.Object));
      Assert.That(((CompleteVirtualObjectEndPointLoadState)newLoadState).TransactionEventSink, Is.SameAs(_transactionEventSinkStub.Object));
      Assert.That(((CompleteVirtualObjectEndPointLoadState)newLoadState).EndPointProvider, Is.SameAs(_endPointProviderStub.Object));
    }

    [Test]
    public void MarkDataComplete_Null ()
    {
      _loadStateMock
          .Setup(
              mock => mock.MarkDataComplete(
                  _endPoint,
                  (DomainObject)null,
                  It.IsAny<Action<IVirtualObjectEndPointDataManager>>()))
          .Verifiable();

      _endPoint.MarkDataComplete(null);

      _loadStateMock.Verify();
    }

    [Test]
    public void MarkDataIncomplete ()
    {
      Action actualStateSetter = null;

      _loadStateMock
          .Setup(mock => mock.MarkDataIncomplete(_endPoint, It.IsAny<Action>()))
          .Callback((IVirtualObjectEndPoint _, Action stateSetter) => { actualStateSetter = stateSetter; })
          .Verifiable();

      _endPoint.MarkDataIncomplete();

      _loadStateMock.Verify();

      Assert.That(VirtualObjectEndPointTestHelper.GetLoadState(_endPoint), Is.SameAs(_loadStateMock.Object));

      actualStateSetter();

      var newLoadState = VirtualObjectEndPointTestHelper.GetLoadState(_endPoint);
      Assert.That(newLoadState, Is.Not.SameAs(_loadStateMock.Object));
      Assert.That(newLoadState, Is.TypeOf(typeof(IncompleteVirtualObjectEndPointLoadState)));

      Assert.That(((IncompleteVirtualObjectEndPointLoadState)newLoadState).DataManagerFactory, Is.SameAs(_dataManagerFactory));
      Assert.That(
        ((IncompleteVirtualObjectEndPointLoadState)newLoadState).EndPointLoader,
        Is.TypeOf<VirtualObjectEndPoint.EndPointLoader>()
          .With.Property<VirtualObjectEndPoint.EndPointLoader>(l => l.LazyLoader).SameAs(_lazyLoaderMock.Object));
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint ()
    {
      _loadStateMock.Setup(mock => mock.RegisterOriginalOppositeEndPoint(_endPoint, _oppositeEndPointStub.Object)).Verifiable();

      _endPoint.RegisterOriginalOppositeEndPoint(_oppositeEndPointStub.Object);

      _loadStateMock.Verify();
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint ()
    {
      _loadStateMock.Setup(mock => mock.UnregisterOriginalOppositeEndPoint(_endPoint, _oppositeEndPointStub.Object)).Verifiable();

      _endPoint.UnregisterOriginalOppositeEndPoint(_oppositeEndPointStub.Object);

      _loadStateMock.Verify();
    }

    [Test]
    public void RegisterCurrentOppositeEndPoint ()
    {
      _loadStateMock.Setup(mock => mock.RegisterCurrentOppositeEndPoint(_endPoint, _oppositeEndPointStub.Object)).Verifiable();

      _endPoint.RegisterCurrentOppositeEndPoint(_oppositeEndPointStub.Object);

      _loadStateMock.Verify();
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint ()
    {
      _loadStateMock.Setup(mock => mock.UnregisterCurrentOppositeEndPoint(_endPoint, _oppositeEndPointStub.Object)).Verifiable();

      _endPoint.UnregisterCurrentOppositeEndPoint(_oppositeEndPointStub.Object);

      _loadStateMock.Verify();
    }

    [Test]
    public void CreateSetCommand ()
    {
      var fakeCommand = new Mock<IDataManagementCommand>();
      _loadStateMock.Setup(mock => mock.CreateSetCommand(_endPoint, _oppositeObject)).Returns(fakeCommand.Object).Verifiable();

      var result = _endPoint.CreateSetCommand(_oppositeObject);

      _loadStateMock.Verify();

      Assert.That(result, Is.SameAs(fakeCommand.Object));
    }

    [Test]
    public void CreateSetCommand_Null ()
    {
      var fakeCommand = new Mock<IDataManagementCommand>();
      _loadStateMock.Setup(mock => mock.CreateSetCommand(_endPoint, null)).Returns(fakeCommand.Object).Verifiable();

      var result = _endPoint.CreateSetCommand(null);

      _loadStateMock.Verify();
      Assert.That(result, Is.SameAs(fakeCommand.Object));
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      var fakeCommand = new Mock<IDataManagementCommand>();
      _loadStateMock.Setup(mock => mock.CreateDeleteCommand(_endPoint)).Returns(fakeCommand.Object).Verifiable();

      var result = _endPoint.CreateDeleteCommand();

      _loadStateMock.Verify();
      Assert.That(result, Is.SameAs(fakeCommand.Object));
    }

    [Test]
    public void Touch ()
    {
      Assert.That(_endPoint.HasBeenTouched, Is.False);

      _endPoint.Touch();

      Assert.That(_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void Commit_Changed ()
    {
      _endPoint.Touch();
      Assert.That(_endPoint.HasBeenTouched, Is.True);

      _loadStateMock.Setup(mock => mock.HasChanged()).Returns(true);
      _loadStateMock.Setup(mock => mock.Commit(_endPoint)).Verifiable();

      _endPoint.Commit();

      _loadStateMock.Verify();
      Assert.That(_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void Commit_TouchedUnchanged ()
    {
      _endPoint.Touch();
      Assert.That(_endPoint.HasBeenTouched, Is.True);

      _loadStateMock.Setup(mock => mock.HasChanged()).Returns(false);

      _endPoint.Commit();

      _loadStateMock.Verify(mock => mock.Commit(_endPoint), Times.Never());
      Assert.That(_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void Rollback_Changed ()
    {
      _endPoint.Touch();
      Assert.That(_endPoint.HasBeenTouched, Is.True);

      _loadStateMock.Setup(mock => mock.HasChanged()).Returns(true);
      _loadStateMock.Setup(mock => mock.Rollback(_endPoint)).Verifiable();

      _endPoint.Rollback();

      _loadStateMock.Verify();
      Assert.That(_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void Rollback_TouchedUnchanged ()
    {
      _endPoint.Touch();
      Assert.That(_endPoint.HasBeenTouched, Is.True);

      _loadStateMock.Setup(mock => mock.HasChanged()).Returns(false);

      _endPoint.Rollback();

      _loadStateMock.Verify(mock => mock.Rollback(_endPoint), Times.Never());
      Assert.That(_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void SetOppositeObjectDataFromSubTransaction ()
    {
      var source = RelationEndPointObjectMother.CreateVirtualObjectEndPoint(_endPointID, TestableClientTransaction);

      _loadStateMock.Setup(mock => mock.SetDataFromSubTransaction(_endPoint, VirtualObjectEndPointTestHelper.GetLoadState(source))).Verifiable();
      _loadStateMock.Setup(mock => mock.HasChanged()).Returns(true);

      PrivateInvoke.InvokeNonPublicMethod(_endPoint, "SetOppositeObjectDataFromSubTransaction", source);

      _loadStateMock.Verify();
    }

    [Test]
    public void EndPointLoader_LoadEndPointAndGetNewState ()
    {
      var endPointLoader = new VirtualObjectEndPoint.EndPointLoader(_lazyLoaderMock.Object);
      var loadStateFake = new Mock<IVirtualObjectEndPointLoadState>();
      _lazyLoaderMock
          .Setup(mock => mock.LoadLazyVirtualObjectEndPoint(_endPointID))
          .Callback((RelationEndPointID endPointID) => VirtualObjectEndPointTestHelper.SetLoadState(_endPoint, loadStateFake.Object))
          .Verifiable();

      var result = endPointLoader.LoadEndPointAndGetNewState(_endPoint);

      _lazyLoaderMock.Verify();
      Assert.That(result, Is.SameAs(loadStateFake.Object));
    }

    [Test]
    public void EndPointLoader_Serializable ()
    {
      var endPointLoader = new VirtualObjectEndPoint.EndPointLoader(new SerializableLazyLoaderFake());

      var deserializedInstance = FlattenedSerializer.SerializeAndDeserialize(endPointLoader);

      Assert.That(deserializedInstance.LazyLoader, Is.Not.Null);
    }
  }
}
