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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  [TestFixture]
  public class EndPointDelegatingDomainObjectCollectionDataTest : ClientTransactionBaseTest
  {
    private Order _owningOrder;
    private RelationEndPointID _endPointID;

    private Mock<IDomainObjectCollectionEndPoint> _collectionEndPointMock;
    private Mock<IVirtualEndPointProvider> _virtualEndPointProviderStub;

    private Mock<IDomainObjectCollectionData> _endPointDataStub;
    private ReadOnlyDomainObjectCollectionDataDecorator _endPointDataDecorator;
    private Mock<IDataManagementCommand> _nestedCommandMock;
    private ExpandedCommand _expandedCommandFake;
    private Mock<IDataManagementCommand> _commandStub;

    private EndPointDelegatingDomainObjectCollectionData _delegatingData;

    private OrderItem _orderItem1;
    private OrderItem _orderItem2;

    public override void SetUp ()
    {
      base.SetUp();

      _owningOrder = DomainObjectIDs.Order1.GetObject<Order>();
      _endPointID = RelationEndPointID.Resolve(_owningOrder, o => o.OrderItems);

      _collectionEndPointMock = new Mock<IDomainObjectCollectionEndPoint>(MockBehavior.Strict);
      StubCollectionEndPoint(_collectionEndPointMock, TestableClientTransaction, _owningOrder);
      _virtualEndPointProviderStub = new Mock<IVirtualEndPointProvider>();
      _virtualEndPointProviderStub
          .Setup(stub => stub.GetOrCreateVirtualEndPoint(_endPointID))
          .Returns(_collectionEndPointMock.Object);

      _endPointDataStub = new Mock<IDomainObjectCollectionData>();
      _endPointDataDecorator = new ReadOnlyDomainObjectCollectionDataDecorator(_endPointDataStub.Object);

      _commandStub = new Mock<IDataManagementCommand>();
      _nestedCommandMock = new Mock<IDataManagementCommand>();
      _nestedCommandMock.Setup(stub => stub.GetAllExceptions()).Returns(new Exception[0]);
      _expandedCommandFake = new ExpandedCommand(_nestedCommandMock.Object);

      _delegatingData = new EndPointDelegatingDomainObjectCollectionData(_endPointID, _virtualEndPointProviderStub.Object);

      _orderItem1 = DomainObjectIDs.OrderItem1.GetObject<OrderItem>();
      _orderItem2 = DomainObjectIDs.OrderItem2.GetObject<OrderItem>();

      ClientTransactionScope.EnterNullScope(); // no active transaction
    }

    public override void TearDown ()
    {
      ClientTransactionScope.ActiveScope.Leave();
      base.TearDown();
    }

    [Test]
    public void Initialization_ChecksEndPointIDCardinality ()
    {
      Assert.That(
          () => new EndPointDelegatingDomainObjectCollectionData(RelationEndPointID.Resolve(_owningOrder, o => o.Customer), _virtualEndPointProviderStub.Object),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Associated end-point must be a CollectionEndPoint.", "endPointID"));
    }

    [Test]
    public void Count ()
    {
      _endPointDataStub.Setup(stub => stub.Count).Returns(42);
      _collectionEndPointMock.Setup(mock => mock.GetData()).Returns(_endPointDataDecorator).Verifiable();

      Assert.That(_delegatingData.Count, Is.EqualTo(42));

      _collectionEndPointMock.Verify();
    }

    [Test]
    public void RequiredItemType ()
    {
      Assert.That(_delegatingData.RequiredItemType, Is.Null);
    }

    [Test]
    public void IsReadOnly ()
    {
      Assert.That(_delegatingData.IsReadOnly, Is.False);
    }

    [Test]
    public void GetAssociatedEndPoint ()
    {
      Assert.That(_delegatingData.GetAssociatedEndPoint(), Is.SameAs(_collectionEndPointMock.Object));
    }

    [Test]
    public void IsDataComplete ()
    {
      _collectionEndPointMock.Setup(stub => stub.IsDataComplete).Returns(true);
      Assert.That(_delegatingData.IsDataComplete, Is.True);

      _collectionEndPointMock.Setup(stub => stub.IsDataComplete).Returns(false);
      Assert.That(_delegatingData.IsDataComplete, Is.False);
    }

    [Test]
    public void EnsureDataComplete ()
    {
      _collectionEndPointMock.Setup(mock => mock.EnsureDataComplete()).Verifiable();

      _delegatingData.EnsureDataComplete();

      _collectionEndPointMock.Verify();
    }

    [Test]
    public void ContainsObjectID ()
    {
      _endPointDataStub.Setup(stub => stub.ContainsObjectID(_orderItem1.ID)).Returns(true);
      _collectionEndPointMock.Setup(mock => mock.GetData()).Returns(_endPointDataDecorator).Verifiable();

      Assert.That(_delegatingData.ContainsObjectID(_orderItem1.ID), Is.True);

      _collectionEndPointMock.Verify();
    }

    [Test]
    public void GetObject_Index ()
    {
      _endPointDataStub.Setup(stub => stub.GetObject(1)).Returns(_orderItem1);
      _collectionEndPointMock.Setup(mock => mock.GetData()).Returns(_endPointDataDecorator).Verifiable();

      Assert.That(_delegatingData.GetObject(1), Is.SameAs(_orderItem1));

      _collectionEndPointMock.Verify();
    }

    [Test]
    public void GetObject_ID ()
    {
      _endPointDataStub.Setup(stub => stub.GetObject(_orderItem1.ID)).Returns(_orderItem1);
      _collectionEndPointMock.Setup(mock => mock.GetData()).Returns(_endPointDataDecorator).Verifiable();

      Assert.That(_delegatingData.GetObject(_orderItem1.ID), Is.SameAs(_orderItem1));

      _collectionEndPointMock.Verify();
    }

    [Test]
    public void IndexOf ()
    {
      _endPointDataStub.Setup(stub => stub.IndexOf(_orderItem1.ID)).Returns(3);
      _collectionEndPointMock.Setup(mock => mock.GetData()).Returns(_endPointDataDecorator).Verifiable();

      Assert.That(_delegatingData.IndexOf(_orderItem1.ID), Is.EqualTo(3));

      _collectionEndPointMock.Verify();
    }

    [Test]
    public void GetEnumerator ()
    {
      var fakeEnumerator = new Mock<IEnumerator<DomainObject>>();
      _endPointDataStub.Setup(stub => stub.GetEnumerator()).Returns(fakeEnumerator.Object);
      _collectionEndPointMock.Setup(mock => mock.GetData()).Returns(_endPointDataDecorator).Verifiable();

      Assert.That(_delegatingData.GetEnumerator(), Is.SameAs(fakeEnumerator.Object));

      _collectionEndPointMock.Verify();
    }

    [Test]
    public void Clear ()
    {
      var removeCommandStub1 = new Mock<IDataManagementCommand>();
      var removeCommandStub2 = new Mock<IDataManagementCommand>();

      var nestedCommandMock1 = new Mock<IDataManagementCommand>(MockBehavior.Strict);
      var nestedCommandMock2 = new Mock<IDataManagementCommand>(MockBehavior.Strict);

      nestedCommandMock1.Setup(stub => stub.GetAllExceptions()).Returns(new Exception[0]);
      nestedCommandMock2.Setup(stub => stub.GetAllExceptions()).Returns(new Exception[0]);

      removeCommandStub1.Setup(stub => stub.ExpandToAllRelatedObjects()).Returns(new ExpandedCommand(nestedCommandMock1.Object));
      removeCommandStub2.Setup(stub => stub.ExpandToAllRelatedObjects()).Returns(new ExpandedCommand(nestedCommandMock2.Object));

      nestedCommandMock1.Reset();
      nestedCommandMock2.Reset();

      _endPointDataStub.Setup(stub => stub.Count).Returns(2);
      _endPointDataStub.Setup(stub => stub.GetObject(1)).Returns(_orderItem2);
      _endPointDataStub.Setup(stub => stub.GetObject(0)).Returns(_orderItem1);

      _collectionEndPointMock.Setup(mock => mock.GetData()).Returns(_endPointDataDecorator).Verifiable();
      _collectionEndPointMock.Setup(mock => mock.CreateRemoveCommand(_orderItem1)).Returns(removeCommandStub1.Object).Verifiable();
      _collectionEndPointMock.Setup(mock => mock.CreateRemoveCommand(_orderItem2)).Returns(removeCommandStub2.Object).Verifiable();

      var sequence = new VerifiableSequence();

      nestedCommandMock2.InVerifiableSequence(sequence).Setup(mock => mock.Begin()).Verifiable("nestedCommandMock2.Begin");

      nestedCommandMock1.InVerifiableSequence(sequence).Setup(mock => mock.Begin()).Verifiable("nestedCommandMock1.Begin");

      nestedCommandMock2.InVerifiableSequence(sequence).Setup(mock => mock.Perform()).Verifiable("nestedCommandMock2.Perform");

      nestedCommandMock1.InVerifiableSequence(sequence).Setup(mock => mock.Perform()).Verifiable("nestedCommandMock1.Perform");

      _collectionEndPointMock.InVerifiableSequence(sequence).Setup(mock => mock.Touch()).Verifiable("endPoint.Touch");

      nestedCommandMock1.InVerifiableSequence(sequence).Setup(mock => mock.End()).Verifiable("nestedCommandMock1.End");

      nestedCommandMock2.InVerifiableSequence(sequence).Setup(mock => mock.End()).Verifiable("nestedCommandMock2.End");

      _delegatingData.Clear();

      nestedCommandMock1.Verify();
      nestedCommandMock2.Verify();
      sequence.Verify();
    }

    [Test]
    public void Clear_WithoutItems ()
    {
      _endPointDataStub.Setup(stub => stub.Count).Returns(0);
      _collectionEndPointMock.Setup(mock => mock.GetData()).Returns(_endPointDataDecorator).Verifiable();
      _collectionEndPointMock.Setup(mock => mock.Touch()).Verifiable();

      _delegatingData.Clear();

      _collectionEndPointMock.Verify();
    }

    [Test]
    public void Insert ()
    {
      var sequence = new VerifiableSequence();
      _collectionEndPointMock.Setup(mock => mock.CreateInsertCommand(_orderItem1, 17)).Returns(_commandStub.Object).Verifiable();
      _collectionEndPointMock.Setup(mock => mock.Touch()).Verifiable();
      _commandStub.Setup(stub => stub.ExpandToAllRelatedObjects()).Returns(_expandedCommandFake);
      ExpectNotifyAndPerform(sequence, _nestedCommandMock);

      _delegatingData.Insert(17, _orderItem1);

      _collectionEndPointMock.Verify();
      _nestedCommandMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Insert_ChecksErrorConditions ()
    {
      CheckClientTransactionDiffersException((data, relatedObjectInOtherTransaction) => data.Insert(17, relatedObjectInOtherTransaction));
      CheckObjectDeletedException((data, deletedRelatedObject) => data.Insert(17, deletedRelatedObject));
      CheckOwningObjectDeletedException((data, relatedObject) => data.Insert(17, relatedObject));
    }

    [Test]
    public void Remove ()
    {
      var sequence = new VerifiableSequence();
      _endPointDataStub.Setup(stub => stub.ContainsObjectID(_orderItem1.ID)).Returns(true);
      _collectionEndPointMock.Setup(mock => mock.GetData()).Returns(_endPointDataDecorator).Verifiable();
      _collectionEndPointMock.Setup(mock => mock.CreateRemoveCommand(_orderItem1)).Returns(_commandStub.Object).Verifiable();
      _collectionEndPointMock.Setup(mock => mock.Touch()).Verifiable();

      _commandStub.Setup(stub => stub.ExpandToAllRelatedObjects()).Returns(_expandedCommandFake);
      ExpectNotifyAndPerform(sequence, _nestedCommandMock);

      var result = _delegatingData.Remove(_orderItem1);

      _collectionEndPointMock.Verify();
      _nestedCommandMock.Verify();
      sequence.Verify();

      Assert.That(result, Is.True);
    }

    [Test]
    public void Remove_ObjectNotContained ()
    {
      _endPointDataStub.Setup(stub => stub.ContainsObjectID(_orderItem1.ID)).Returns(false);
      _collectionEndPointMock.Setup(mock => mock.GetData()).Returns(_endPointDataDecorator).Verifiable();
      _collectionEndPointMock.Setup(mock => mock.Touch()).Verifiable();

      bool result = _delegatingData.Remove(_orderItem1);

      _collectionEndPointMock.Verify(mock => mock.CreateRemoveCommand(It.IsAny<DomainObject>()), Times.Never());
      _collectionEndPointMock.Verify();
      Assert.That(result, Is.False);
    }

    [Test]
    public void Remove_ChecksErrorConditions ()
    {
      CheckClientTransactionDiffersException((data, relatedObjectInOtherTransaction) => data.Remove(relatedObjectInOtherTransaction));
      CheckObjectDeletedException((data, deletedRelatedObject) => data.Remove(deletedRelatedObject));
      CheckOwningObjectDeletedException((data, relatedObject) => data.Remove(relatedObject));
    }

    [Test]
    public void Remove_ID ()
    {
      var sequence = new VerifiableSequence();
      _endPointDataStub.Setup(stub => stub.GetObject(_orderItem1.ID)).Returns(_orderItem1);
      _collectionEndPointMock.Setup(mock => mock.GetData()).Returns(_endPointDataDecorator).Verifiable();
      _collectionEndPointMock.Setup(mock => mock.CreateRemoveCommand(_orderItem1)).Returns(_commandStub.Object).Verifiable();
      _collectionEndPointMock.Setup(mock => mock.Touch()).Verifiable();

      _commandStub.Setup(stub => stub.ExpandToAllRelatedObjects()).Returns(_expandedCommandFake);
      ExpectNotifyAndPerform(sequence, _nestedCommandMock);

      var result = _delegatingData.Remove(_orderItem1.ID);

      _collectionEndPointMock.Verify();
      _nestedCommandMock.Verify();
      sequence.Verify();

      Assert.That(result, Is.True);
    }

    [Test]
    public void Remove_ID_ObjectNotContained ()
    {
      _endPointDataStub.Setup(stub => stub.GetObject(_orderItem1.ID)).Returns((DomainObject)null);
      _collectionEndPointMock.Setup(mock => mock.GetData()).Returns(_endPointDataDecorator).Verifiable();
      _collectionEndPointMock.Setup(mock => mock.Touch()).Verifiable();

      var result = _delegatingData.Remove(_orderItem1.ID);

      _collectionEndPointMock.Verify(mock => mock.CreateRemoveCommand(It.IsAny<DomainObject>()), Times.Never());
      _collectionEndPointMock.Verify();

      Assert.That(result, Is.False);
    }

    [Test]
    public void Remove_ID_ChecksErrorConditions ()
    {
      CheckOwningObjectDeletedException((data, relatedObject) => data.Remove(relatedObject.ID));
    }

    [Test]
    public void Replace ()
    {
      var sequence = new VerifiableSequence();
      _collectionEndPointMock.Setup(mock => mock.CreateReplaceCommand(17, _orderItem1)).Returns(_commandStub.Object).Verifiable();
      _collectionEndPointMock.Setup(mock => mock.Touch()).Verifiable();
      _commandStub.Setup(stub => stub.ExpandToAllRelatedObjects()).Returns(_expandedCommandFake);
      ExpectNotifyAndPerform(sequence, _nestedCommandMock);

      _delegatingData.Replace(17, _orderItem1);

      _collectionEndPointMock.Verify();
      _nestedCommandMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Replace_ChecksErrorConditions ()
    {
      CheckClientTransactionDiffersException((data, relatedObjectInOtherTransaction) => data.Replace(17, relatedObjectInOtherTransaction));
      CheckObjectDeletedException((data, deletedRelatedObject) => data.Replace(17, deletedRelatedObject));
      CheckOwningObjectDeletedException((data, relatedObject) => data.Replace(17, relatedObject));
    }

    [Test]
    public void Sort ()
    {
      Comparison<IDomainObject> comparison = (one, two) => 0;

      _collectionEndPointMock.Setup(mock => mock.SortCurrentData(comparison)).Verifiable();
    }

    [Test]
    public void Serializable ()
    {
      var data = new EndPointDelegatingDomainObjectCollectionData(_endPointID, new SerializableRelationEndPointProviderFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize(data);

      Assert.That(deserializedInstance.AssociatedEndPointID, Is.EqualTo(_endPointID));
      Assert.That(deserializedInstance.VirtualEndPointProvider, Is.Not.Null);
    }

    private IDomainObjectCollectionEndPoint CreateDomainObjectCollectionEndPointStub (ClientTransaction clientTransaction, Order owningOrder)
    {
      var endPointStub = new Mock<IDomainObjectCollectionEndPoint>();
      StubCollectionEndPoint(endPointStub, clientTransaction, owningOrder);
      return endPointStub.Object;
    }

    private void StubCollectionEndPoint (Mock<IDomainObjectCollectionEndPoint> endPointStub, ClientTransaction clientTransaction, Order owningOrder)
    {
      endPointStub.Setup(stub => stub.IsNull).Returns(false);
      endPointStub.Setup(stub => stub.ClientTransaction).Returns(clientTransaction);
      var relationEndPointDefinition = owningOrder.ID.ClassDefinition.GetMandatoryRelationEndPointDefinition(typeof(Order).FullName + ".OrderItems");
      endPointStub.Setup(mock => mock.ObjectID).Returns(owningOrder.ID);
      endPointStub.Setup(mock => mock.Definition).Returns(relationEndPointDefinition);
      endPointStub.Setup(mock => mock.GetDomainObject()).Returns(owningOrder);
      endPointStub.Setup(mock => mock.GetDomainObjectReference()).Returns(owningOrder);
    }

    private void CheckClientTransactionDiffersException (Action<EndPointDelegatingDomainObjectCollectionData, DomainObject> action)
    {
      var orderItemInOtherTransaction = DomainObjectMother.CreateObjectInTransaction<OrderItem>(ClientTransaction.CreateRootTransaction());
      try
      {
        action(_delegatingData, orderItemInOtherTransaction);
        Assert.Fail("Expected ClientTransactionsDifferException");
      }
      catch (ClientTransactionsDifferException)
      {
        // ok
      }
    }

    private void CheckObjectDeletedException (Action<EndPointDelegatingDomainObjectCollectionData, DomainObject> action)
    {
      OrderItem deletedObject;
      using (_delegatingData.GetAssociatedEndPoint().ClientTransaction.EnterNonDiscardingScope())
      {
        deletedObject = DomainObjectIDs.OrderItem5.GetObject<OrderItem>();
        deletedObject.Delete();
      }

      try
      {
        action(_delegatingData, deletedObject);
        Assert.Fail("Expected ObjectDeletedException");
      }
      catch (ObjectDeletedException)
      {
        // ok
      }
    }

    private void CheckOwningObjectDeletedException (Action<EndPointDelegatingDomainObjectCollectionData, DomainObject> action)
    {
      Order deletedOwningObject;
      using (_delegatingData.GetAssociatedEndPoint().ClientTransaction.EnterNonDiscardingScope())
      {
        deletedOwningObject = DomainObjectIDs.Order5.GetObject<Order>();
      }

      var endPointStub = CreateDomainObjectCollectionEndPointStub(TestableClientTransaction, deletedOwningObject);
      var virtualEndPointProviderStub = new Mock<IVirtualEndPointProvider>();
      virtualEndPointProviderStub.Setup(stub => stub.GetOrCreateVirtualEndPoint(_endPointID)).Returns(endPointStub);
      var data = new EndPointDelegatingDomainObjectCollectionData(_endPointID, virtualEndPointProviderStub.Object);

      using (_delegatingData.GetAssociatedEndPoint().ClientTransaction.EnterNonDiscardingScope())
      {
        deletedOwningObject.Delete();
      }

      try
      {
        action(data, _orderItem1);
        Assert.Fail("Expected ObjectDeletedException");
      }
      catch (ObjectDeletedException)
      {
        // ok
      }
    }

    private void ExpectNotifyAndPerform (VerifiableSequence sequence, Mock<IDataManagementCommand> commandMock)
    {
      commandMock.InVerifiableSequence(sequence).Setup(mock => mock.Begin()).Verifiable();
      commandMock.InVerifiableSequence(sequence).Setup(mock => mock.Perform()).Verifiable();
      commandMock.InVerifiableSequence(sequence).Setup(mock => mock.End()).Verifiable();
    }
  }
}
