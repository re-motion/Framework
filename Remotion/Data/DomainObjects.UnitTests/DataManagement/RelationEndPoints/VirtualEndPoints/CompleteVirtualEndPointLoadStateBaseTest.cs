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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints
{
  [TestFixture]
  public class CompleteVirtualEndPointLoadStateBaseTest : StandardMappingTest
  {
    private Mock<IVirtualEndPoint<object>> _virtualEndPointMock;
    private Mock<IVirtualEndPointDataManager> _dataManagerMock;
    private Mock<IRelationEndPointProvider> _endPointProviderStub;
    private Mock<IClientTransactionEventSink> _transactionEventSinkWithMock;

    private TestableCompleteVirtualEndPointLoadState _loadState;

    private IRelationEndPointDefinition _definition;
    private Order _relatedObject;
    private Mock<IRealObjectEndPoint> _relatedEndPointStub;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _definition = Configuration.GetTypeDefinition(typeof(Customer)).GetRelationEndPointDefinition(typeof(Customer).FullName + ".Orders");

      _virtualEndPointMock = new Mock<IVirtualEndPoint<object>>(MockBehavior.Strict);
      _dataManagerMock = new Mock<IVirtualEndPointDataManager>(MockBehavior.Strict);
      _dataManagerMock.Setup(stub => stub.EndPointID).Returns(RelationEndPointID.Create(DomainObjectIDs.Customer1, _definition));
      _endPointProviderStub = new Mock<IRelationEndPointProvider>();
      _transactionEventSinkWithMock = new Mock<IClientTransactionEventSink>(MockBehavior.Strict);

      _loadState = new TestableCompleteVirtualEndPointLoadState(_dataManagerMock.Object, _endPointProviderStub.Object, _transactionEventSinkWithMock.Object);

      _relatedObject = DomainObjectMother.CreateFakeObject<Order>(DomainObjectIDs.Order1);
      _relatedEndPointStub = new Mock<IRealObjectEndPoint>();
      _relatedEndPointStub.Setup(stub => stub.GetDomainObjectReference()).Returns(_relatedObject);
      _relatedEndPointStub.Setup(stub => stub.ObjectID).Returns(_relatedObject.ID);
    }

    [Test]
    public void IsDataComplete ()
    {
      Assert.That(_loadState.IsDataComplete(), Is.True);
    }

    [Test]
    public void EnsureDataComplete_DoesNothing ()
    {
      _loadState.EnsureDataComplete(_virtualEndPointMock.Object);

      _virtualEndPointMock.Verify();
      _dataManagerMock.Verify();
    }

    [Test]
    public void CanDataBeMarkedIncomplete_True ()
    {
      _dataManagerMock.Setup(stub => stub.HasDataChanged()).Returns(false);

      Assert.That(_loadState.CanDataBeMarkedIncomplete(_virtualEndPointMock.Object), Is.True);
    }

    [Test]
    public void CanDataBeMarkedIncomplete_False ()
    {
      _dataManagerMock.Setup(stub => stub.HasDataChanged()).Returns(true);

      Assert.That(_loadState.CanDataBeMarkedIncomplete(_virtualEndPointMock.Object), Is.False);
    }

    [Test]
    public void MarkDataIncomplete_RaisesEvent ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "OrderItems");
      _virtualEndPointMock
          .Setup(stub => stub.ID)
          .Returns(endPointID);

      _dataManagerMock.Setup(stub => stub.HasDataChanged()).Returns(false);

      _loadState.StubOriginalOppositeEndPoints(new IRealObjectEndPoint[0]);

      _transactionEventSinkWithMock.Setup(mock => mock.RaiseRelationEndPointBecomingIncompleteEvent(endPointID)).Verifiable();

      _loadState.MarkDataIncomplete(_virtualEndPointMock.Object, () => { });

      _virtualEndPointMock.Verify();
      _dataManagerMock.Verify();
      _transactionEventSinkWithMock.Verify();
    }

    [Test]
    public void MarkDataIncomplete_ExecutesStateSetter_AndSynchronizesOppositeEndPoints ()
    {
      // ReSharper disable AccessToModifiedClosure
      bool stateSetterCalled = false;

      var synchronizedOppositeEndPointStub = new Mock<IRealObjectEndPoint>();
      _loadState.StubOriginalOppositeEndPoints(new[] { synchronizedOppositeEndPointStub.Object });

      var unsynchronizedOppositeEndPointMock = new Mock<IRealObjectEndPoint>(MockBehavior.Strict);
      unsynchronizedOppositeEndPointMock.Setup(stub => stub.ObjectID).Returns(DomainObjectIDs.Order1);
      AddUnsynchronizedOppositeEndPoint(_loadState, unsynchronizedOppositeEndPointMock.Object);

      _virtualEndPointMock
          .Setup(stub => stub.ID)
          .Returns(RelationEndPointID.Create(DomainObjectIDs.Customer1, _definition));
      _virtualEndPointMock
          .Setup(mock => mock.RegisterOriginalOppositeEndPoint(synchronizedOppositeEndPointStub.Object))
          .Callback((IRealObjectEndPoint oppositeEndPoint) => Assert.That(stateSetterCalled, Is.True))
          .Verifiable();
      _virtualEndPointMock
          .Setup(mock => mock.RegisterOriginalOppositeEndPoint(unsynchronizedOppositeEndPointMock.Object))
          .Callback((IRealObjectEndPoint oppositeEndPoint) => Assert.That(stateSetterCalled, Is.True))
          .Verifiable();

      _dataManagerMock.Setup(stub => stub.HasDataChanged()).Returns(false);

      _transactionEventSinkWithMock.Setup(mock => mock.RaiseRelationEndPointBecomingIncompleteEvent(It.IsAny<RelationEndPointID>()));

      _loadState.MarkDataIncomplete(_virtualEndPointMock.Object, () => stateSetterCalled = true);

      _virtualEndPointMock.Verify();
      unsynchronizedOppositeEndPointMock.Verify();
      _dataManagerMock.Verify();

      Assert.That(stateSetterCalled, Is.True);
      // ReSharper restore AccessToModifiedClosure
    }

    [Test]
    public void MarkDataIncomplete_Throws_WithChangedData ()
    {
      _virtualEndPointMock
          .Setup(stub => stub.ID)
          .Returns(RelationEndPointID.Create(DomainObjectIDs.Customer1, _definition));

      _dataManagerMock.Setup(stub => stub.HasDataChanged()).Returns(true);

      Assert.That(
          () =>_loadState.MarkDataIncomplete(_virtualEndPointMock.Object, () => Assert.Fail("Must not be called.")),
          Throws.InvalidOperationException.With.Message.EqualTo(
          "Cannot mark virtual end-point "
          + "'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' incomplete "
          + "because it has been changed."));

      _transactionEventSinkWithMock.Verify(mock => mock.RaiseRelationEndPointBecomingIncompleteEvent(It.IsAny<RelationEndPointID>()), Times.Never());
    }

    [Test]
    public void CanEndPointBeCollected ()
    {
      var result = _loadState.CanEndPointBeCollected(_virtualEndPointMock.Object);
      Assert.That(result, Is.False);
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint_WithoutExistingItem ()
    {
      var endPointMock = new Mock<IRealObjectEndPoint>(MockBehavior.Strict);
      endPointMock.Setup(stub => stub.IsNull).Returns(false);
      endPointMock.Setup(stub => stub.ObjectID).Returns(DomainObjectIDs.Order1);
      endPointMock.Setup(mock => mock.MarkUnsynchronized()).Verifiable();

      _dataManagerMock.Setup(stub => stub.ContainsOriginalObjectID(DomainObjectIDs.Order1)).Returns(false);

      _loadState.RegisterOriginalOppositeEndPoint(_virtualEndPointMock.Object, endPointMock.Object);

      _dataManagerMock.Verify(mock => mock.RegisterOriginalOppositeEndPoint(endPointMock.Object), Times.Never());
      endPointMock.Verify();
      _dataManagerMock.Verify();

      Assert.That(_loadState.UnsynchronizedOppositeEndPoints, Has.Member(endPointMock.Object));
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint_WithExistingItem ()
    {
      var endPointMock = new Mock<IRealObjectEndPoint>(MockBehavior.Strict);
      endPointMock.Setup(stub => stub.IsNull).Returns(false);
      endPointMock.Setup(stub => stub.ObjectID).Returns(DomainObjectIDs.Order1);
      endPointMock.Setup(mock => mock.MarkSynchronized()).Verifiable();

      _dataManagerMock.Setup(stub => stub.ContainsOriginalObjectID(DomainObjectIDs.Order1)).Returns(true);
      _dataManagerMock.Setup(mock => mock.RegisterOriginalOppositeEndPoint(endPointMock.Object)).Verifiable();

      _loadState.RegisterOriginalOppositeEndPoint(_virtualEndPointMock.Object, endPointMock.Object);

      endPointMock.Verify();
      _dataManagerMock.Verify();
      Assert.That(_loadState.UnsynchronizedOppositeEndPoints, Has.No.Member(endPointMock.Object));
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint_WithNullOppositeEndPoint_Throws ()
    {
      var endPointMock = new Mock<IRealObjectEndPoint>();
      endPointMock.Setup(stub => stub.IsNull).Returns(true);

      Assert.That(
          () => _loadState.RegisterOriginalOppositeEndPoint(_virtualEndPointMock.Object, endPointMock.Object),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("End point must not be a null object.", "oppositeEndPoint"));
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint ()
    {
      var sequence = new VerifiableSequence();
      _virtualEndPointMock.InVerifiableSequence(sequence).Setup(mock => mock.MarkDataIncomplete()).Verifiable();
      _virtualEndPointMock.InVerifiableSequence(sequence).Setup(mock => mock.UnregisterOriginalOppositeEndPoint(_relatedEndPointStub.Object)).Verifiable();

      _loadState.UnregisterOriginalOppositeEndPoint(_virtualEndPointMock.Object, _relatedEndPointStub.Object);

      _dataManagerMock.Verify();
      _virtualEndPointMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint_InUnsyncedList ()
    {
      AddUnsynchronizedOppositeEndPoint(_loadState, _relatedEndPointStub.Object);

      _loadState.UnregisterOriginalOppositeEndPoint(_virtualEndPointMock.Object, _relatedEndPointStub.Object);

      _dataManagerMock.Verify();
      _virtualEndPointMock.Verify(mock => mock.MarkDataIncomplete(), Times.Never());
      _virtualEndPointMock.Verify(mock => mock.UnregisterOriginalOppositeEndPoint(_relatedEndPointStub.Object), Times.Never());
      Assert.That(_loadState.UnsynchronizedOppositeEndPoints, Has.No.Member(_relatedEndPointStub.Object));
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint_WithNullOppositeEndPoint_Throws ()
    {
      var endPointMock = new Mock<IRealObjectEndPoint>();
      endPointMock.Setup(stub => stub.IsNull).Returns(true);

      Assert.That(
          () => _loadState.UnregisterOriginalOppositeEndPoint(_virtualEndPointMock.Object, endPointMock.Object),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("End point must not be a null object.", "oppositeEndPoint"));
    }

    [Test]
    public void RegisterCurrentOppositeEndPoint ()
    {
      _relatedEndPointStub.Setup(stub => stub.IsSynchronized).Returns(true);

      _dataManagerMock.Setup(mock => mock.RegisterCurrentOppositeEndPoint(_relatedEndPointStub.Object)).Verifiable();

      _loadState.RegisterCurrentOppositeEndPoint(_virtualEndPointMock.Object, _relatedEndPointStub.Object);

      _dataManagerMock.Verify();
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint ()
    {
      _dataManagerMock.Setup(mock => mock.UnregisterCurrentOppositeEndPoint(_relatedEndPointStub.Object)).Verifiable();

      _loadState.UnregisterCurrentOppositeEndPoint(_virtualEndPointMock.Object, _relatedEndPointStub.Object);

      _dataManagerMock.Verify();
    }

    [Test]
    public void IsSynchronized_True ()
    {
      _loadState.StubOriginalItemsWithoutEndPoints(new DomainObject[0]);

      var result = _loadState.IsSynchronized(_virtualEndPointMock.Object);

      Assert.That(result, Is.True);
    }

    [Test]
    public void IsSynchronized_False ()
    {
      _loadState.StubOriginalItemsWithoutEndPoints(new DomainObject[] { _relatedObject });

      var result = _loadState.IsSynchronized(_virtualEndPointMock.Object);

      Assert.That(result, Is.False);
    }

    [Test]
    public void Synchronize ()
    {
      _loadState.StubOriginalItemsWithoutEndPoints(new[] { _relatedObject });

      _dataManagerMock.Setup(mock => mock.UnregisterOriginalItemWithoutEndPoint(_relatedObject)).Verifiable();

      _loadState.Synchronize(_virtualEndPointMock.Object);

      _dataManagerMock.Verify();
    }

    [Test]
    public void GetUnsynchronizedOppositeEndPoints_Empty ()
    {
      var result = _loadState.UnsynchronizedOppositeEndPoints;

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void SynchronizeOppositeEndPoint_InList ()
    {
      var endPointMock = new Mock<IRealObjectEndPoint>(MockBehavior.Strict);
      endPointMock.Setup(stub => stub.IsNull).Returns(false);
      endPointMock.Setup(stub => stub.ObjectID).Returns(DomainObjectIDs.Order1);
      endPointMock.Setup(mock => mock.MarkUnsynchronized());
      endPointMock.Setup(mock => mock.MarkSynchronized()).Verifiable();

      _dataManagerMock.Setup(stub => stub.ContainsOriginalObjectID(DomainObjectIDs.Order1)).Returns(false);
      _dataManagerMock.Setup(mock => mock.RegisterOriginalOppositeEndPoint(endPointMock.Object)).Verifiable();

      _loadState.RegisterOriginalOppositeEndPoint(_virtualEndPointMock.Object, endPointMock.Object);
      Assert.That(_loadState.UnsynchronizedOppositeEndPoints, Has.Member(endPointMock.Object));

      _loadState.SynchronizeOppositeEndPoint(_virtualEndPointMock.Object, endPointMock.Object);

      _dataManagerMock.Verify();
      endPointMock.Verify();
      Assert.That(_loadState.UnsynchronizedOppositeEndPoints, Has.No.Member(endPointMock.Object));
    }

    [Test]
    public void SynchronizeOppositeEndPoint_NotInList ()
    {
      var endPointStub = new Mock<IRealObjectEndPoint>();
      endPointStub
          .Setup(stub => stub.ID)
          .Returns(RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.OrderItem1, "Order"));
      endPointStub
          .Setup(stub => stub.ObjectID)
          .Returns(DomainObjectIDs.OrderItem1);
      Assert.That(
          () => _loadState.SynchronizeOppositeEndPoint(_virtualEndPointMock.Object, endPointStub.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Cannot synchronize opposite end-point "
                  + "'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order' - the "
                  + "end-point is not in the list of unsynchronized end-points."));
    }

    [Test]
    public void SynchronizeOppositeEndPoint_WithNullOppositeEndPoint_Throws ()
    {
      var endPointMock = new Mock<IRealObjectEndPoint>();
      endPointMock.Setup(stub => stub.IsNull).Returns(true);

      Assert.That(
          () => _loadState.SynchronizeOppositeEndPoint(_virtualEndPointMock.Object, endPointMock.Object),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("End point must not be a null object.", "oppositeEndPoint"));
    }

    [Test]
    public void HasChanged ()
    {
      _dataManagerMock.Setup(mock => mock.HasDataChanged()).Returns(true).Verifiable();

      var result = _loadState.HasChanged();

      _dataManagerMock.Verify();
      Assert.That(result, Is.True);
    }

    [Test]
    public void Commit ()
    {
      _dataManagerMock.Setup(mock => mock.Commit()).Verifiable();

      _loadState.Commit(_virtualEndPointMock.Object);

      _dataManagerMock.Verify();
    }

    [Test]
    public void Rollback ()
    {
      _dataManagerMock.Setup(mock => mock.Rollback()).Verifiable();

      _loadState.Rollback(_virtualEndPointMock.Object);

      _dataManagerMock.Verify();
    }

    private void AddUnsynchronizedOppositeEndPoint (
        CompleteVirtualEndPointLoadStateBase<IVirtualEndPoint<object>, object, IVirtualEndPointDataManager> loadState, IRealObjectEndPoint oppositeEndPoint)
    {
      var dictionary = (Dictionary<ObjectID, IRealObjectEndPoint>)PrivateInvoke.GetNonPublicField(loadState, "_unsynchronizedOppositeEndPoints");
      dictionary.Add(oppositeEndPoint.ObjectID, oppositeEndPoint);
    }
  }
}
