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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  [TestFixture]
  public class EndPointDelegatingCollectionDataTest : ClientTransactionBaseTest
  {
    private Order _owningOrder;
    private RelationEndPointID _endPointID;

    private ICollectionEndPoint _collectionEndPointMock;
    private IVirtualEndPointProvider _virtualEndPointProviderStub;

    private IDomainObjectCollectionData _endPointDataStub;
    private ReadOnlyCollectionDataDecorator _endPointDataDecorator;
    private IDataManagementCommand _nestedCommandMock;
    private ExpandedCommand _expandedCommandFake;
    private IDataManagementCommand _commandStub;

    private EndPointDelegatingCollectionData _delegatingData;

    private OrderItem _orderItem1;
    private OrderItem _orderItem2;

    public override void SetUp ()
    {
      base.SetUp();

      _owningOrder = DomainObjectIDs.Order1.GetObject<Order> ();
      _endPointID = RelationEndPointID.Resolve (_owningOrder, o => o.OrderItems);

      _collectionEndPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint>();
      StubCollectionEndPoint (_collectionEndPointMock, TestableClientTransaction, _owningOrder);
      _virtualEndPointProviderStub = MockRepository.GenerateStub<IVirtualEndPointProvider> ();
      _virtualEndPointProviderStub
          .Stub (stub => stub.GetOrCreateVirtualEndPoint (_endPointID))
          .Return (_collectionEndPointMock);

      _endPointDataStub = MockRepository.GenerateStub<IDomainObjectCollectionData>();
      _endPointDataDecorator = new ReadOnlyCollectionDataDecorator (_endPointDataStub);

      _commandStub = MockRepository.GenerateStub<IDataManagementCommand>();
      _nestedCommandMock = MockRepository.GenerateMock<IDataManagementCommand> ();
      _nestedCommandMock.Stub (stub => stub.GetAllExceptions ()).Return (new Exception[0]);
      _expandedCommandFake = new ExpandedCommand (_nestedCommandMock);

      _delegatingData = new EndPointDelegatingCollectionData (_endPointID, _virtualEndPointProviderStub);

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
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "Associated end-point must be a CollectionEndPoint.\r\nParameter name: endPointID")]
    public void Initialization_ChecksEndPointIDCardinality ()
    {
      new EndPointDelegatingCollectionData (RelationEndPointID.Resolve (_owningOrder, o => o.Customer), _virtualEndPointProviderStub);
    }

    [Test]
    public void Count ()
    {
      _endPointDataStub.Stub (stub => stub.Count).Return (42);
      _collectionEndPointMock.Expect (mock => mock.GetData()).Return (_endPointDataDecorator);
      _collectionEndPointMock.Replay();

      Assert.That (_delegatingData.Count, Is.EqualTo (42));

      _collectionEndPointMock.VerifyAllExpectations();
    }

    [Test]
    public void RequiredItemType ()
    {
      Assert.That (_delegatingData.RequiredItemType, Is.Null);
    }

    [Test]
    public void IsReadOnly ()
    {
      Assert.That (_delegatingData.IsReadOnly, Is.False);
    }

    [Test]
    public void GetAssociatedEndPoint ()
    {
      Assert.That (_delegatingData.GetAssociatedEndPoint(), Is.SameAs (_collectionEndPointMock));
    }

    [Test]
    public void IsDataComplete ()
    {
      _collectionEndPointMock.Stub (stub => stub.IsDataComplete).Return (true).Repeat.Once();
      Assert.That (_delegatingData.IsDataComplete, Is.True);

      _collectionEndPointMock.Stub (stub => stub.IsDataComplete).Return (false).Repeat.Once ();
      Assert.That (_delegatingData.IsDataComplete, Is.False);
    }

    [Test]
    public void EnsureDataComplete ()
    {
      _collectionEndPointMock.Expect (mock => mock.EnsureDataComplete ());
      _collectionEndPointMock.Replay();

      _delegatingData.EnsureDataComplete ();

      _collectionEndPointMock.VerifyAllExpectations();
    }

    [Test]
    public void ContainsObjectID ()
    {
      _endPointDataStub.Stub (stub => stub.ContainsObjectID (_orderItem1.ID)).Return (true);
      _collectionEndPointMock.Expect (mock => mock.GetData ()).Return (_endPointDataDecorator);
      _collectionEndPointMock.Replay ();

      Assert.That (_delegatingData.ContainsObjectID (_orderItem1.ID), Is.True);

      _collectionEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetObject_Index ()
    {
      _endPointDataStub.Stub (stub => stub.GetObject (1)).Return (_orderItem1);
      _collectionEndPointMock.Expect (mock => mock.GetData ()).Return (_endPointDataDecorator);
      _collectionEndPointMock.Replay ();

      Assert.That (_delegatingData.GetObject (1), Is.SameAs(_orderItem1));

      _collectionEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetObject_ID ()
    {
      _endPointDataStub.Stub (stub => stub.GetObject (_orderItem1.ID)).Return (_orderItem1);
      _collectionEndPointMock.Expect (mock => mock.GetData ()).Return (_endPointDataDecorator);
      _collectionEndPointMock.Replay ();

      Assert.That (_delegatingData.GetObject (_orderItem1.ID), Is.SameAs (_orderItem1));

      _collectionEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void IndexOf ()
    {
      _endPointDataStub.Stub (stub => stub.IndexOf(_orderItem1.ID)).Return (3);
      _collectionEndPointMock.Expect (mock => mock.GetData ()).Return (_endPointDataDecorator);
      _collectionEndPointMock.Replay ();

      Assert.That (_delegatingData.IndexOf(_orderItem1.ID), Is.EqualTo(3));

      _collectionEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetEnumerator ()
    {
      var fakeEnumerator = MockRepository.GenerateStub<IEnumerator<DomainObject>>();
      _endPointDataStub.Stub (stub => stub.GetEnumerator ()).Return (fakeEnumerator);
      _collectionEndPointMock.Expect (mock => mock.GetData ()).Return (_endPointDataDecorator);
      _collectionEndPointMock.Replay ();

      Assert.That (_delegatingData.GetEnumerator(), Is.SameAs(fakeEnumerator));

      _collectionEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void Clear ()
    {
      var mockRepository = _collectionEndPointMock.GetMockRepository ();

      var removeCommandStub1 = mockRepository.Stub<IDataManagementCommand> ();
      var removeCommandStub2 = mockRepository.Stub<IDataManagementCommand> ();

      var nestedCommandMock1 = mockRepository.StrictMock<IDataManagementCommand> ();
      var nestedCommandMock2 = mockRepository.StrictMock<IDataManagementCommand> ();

      nestedCommandMock1.Stub (stub => stub.GetAllExceptions ()).Return (new Exception[0]);
      nestedCommandMock2.Stub (stub => stub.GetAllExceptions ()).Return (new Exception[0]);

      nestedCommandMock1.Replay ();
      nestedCommandMock2.Replay ();

      removeCommandStub1.Stub (stub => stub.ExpandToAllRelatedObjects ()).Return (new ExpandedCommand (nestedCommandMock1));
      removeCommandStub2.Stub (stub => stub.ExpandToAllRelatedObjects ()).Return (new ExpandedCommand (nestedCommandMock2));

      nestedCommandMock1.BackToRecord();
      nestedCommandMock2.BackToRecord ();

      _endPointDataStub.Stub (stub => stub.Count).Return (2);
      _endPointDataStub.Stub (stub => stub.GetObject (1)).Return (_orderItem2);
      _endPointDataStub.Stub (stub => stub.GetObject (0)).Return (_orderItem1);

      _collectionEndPointMock.Expect (mock => mock.GetData ()).Return (_endPointDataDecorator).Repeat.Any();
      _collectionEndPointMock.Expect (mock => mock.CreateRemoveCommand (_orderItem1)).Return (removeCommandStub1);
      _collectionEndPointMock.Expect (mock => mock.CreateRemoveCommand (_orderItem2)).Return (removeCommandStub2);

      using (mockRepository.Ordered ())
      {
        nestedCommandMock2.Expect (mock => mock.Begin ()).Message ("nestedCommandMock2.Begin");
        nestedCommandMock1.Expect (mock => mock.Begin ()).Message ("nestedCommandMock1.Begin");
        nestedCommandMock2.Expect (mock => mock.Perform ()).Message ("nestedCommandMock2.Perform");
        nestedCommandMock1.Expect (mock => mock.Perform ()).Message ("nestedCommandMock1.Perform");
        _collectionEndPointMock.Expect (mock => mock.Touch ()).Message ("endPoint.Touch");
        nestedCommandMock1.Expect (mock => mock.End ()).Message ("nestedCommandMock1.End");
        nestedCommandMock2.Expect (mock => mock.End ()).Message ("nestedCommandMock2.End");
      }

      mockRepository.ReplayAll ();

      _delegatingData.Clear();

      mockRepository.VerifyAll ();
    }

    [Test]
    public void Clear_WithoutItems ()
    {
      _endPointDataStub.Stub (stub => stub.Count).Return (0);
      _collectionEndPointMock.Expect (mock => mock.GetData ()).Return (_endPointDataDecorator);
      _collectionEndPointMock.Expect (mock => mock.Touch ());
      _collectionEndPointMock.Replay();

      _delegatingData.Clear();

      _collectionEndPointMock.VerifyAllExpectations();
    }

    [Test]
    public void Insert ()
    {
      _collectionEndPointMock.Expect (mock => mock.CreateInsertCommand (_orderItem1, 17)).Return (_commandStub);
      _collectionEndPointMock.Expect (mock => mock.Touch ());
      _collectionEndPointMock.Replay ();
      _commandStub.Stub (stub => stub.ExpandToAllRelatedObjects()).Return (_expandedCommandFake);

      _delegatingData.Insert (17, _orderItem1);

      _collectionEndPointMock.VerifyAllExpectations ();
      DataManagementCommandTestHelper.AssertNotifyAndPerformWasCalled (_nestedCommandMock);
    }

    [Test]
    public void Insert_ChecksErrorConditions ()
    {
      CheckClientTransactionDiffersException ((data, relatedObjectInOtherTransaction) => data.Insert (17, relatedObjectInOtherTransaction));
      CheckObjectDeletedException ((data, deletedRelatedObject) => data.Insert (17, deletedRelatedObject));
      CheckOwningObjectDeletedException ((data, relatedObject) => data.Insert (17, relatedObject));
    }

    [Test]
    public void Remove ()
    {
      _endPointDataStub.Stub (stub => stub.ContainsObjectID (_orderItem1.ID)).Return (true);
      _collectionEndPointMock.Expect (mock => mock.GetData ()).Return (_endPointDataDecorator);
      _collectionEndPointMock.Expect (mock => mock.CreateRemoveCommand (_orderItem1)).Return (_commandStub);
      _collectionEndPointMock.Expect (mock => mock.Touch ());
      _collectionEndPointMock.Replay ();

      _commandStub.Stub (stub => stub.ExpandToAllRelatedObjects()).Return (_expandedCommandFake);

      var result = _delegatingData.Remove (_orderItem1);

      _collectionEndPointMock.VerifyAllExpectations ();
      DataManagementCommandTestHelper.AssertNotifyAndPerformWasCalled (_nestedCommandMock);

      Assert.That (result, Is.True);
    }

    [Test]
    public void Remove_ObjectNotContained ()
    {
      _endPointDataStub.Stub (stub => stub.ContainsObjectID(_orderItem1.ID)).Return (false);
      _collectionEndPointMock.Expect (mock => mock.GetData ()).Return (_endPointDataDecorator);
      _collectionEndPointMock.Expect(mock => mock.Touch ());
      _collectionEndPointMock.Replay ();

      bool result = _delegatingData.Remove (_orderItem1);

      _collectionEndPointMock.AssertWasNotCalled (mock => mock.CreateRemoveCommand (Arg<DomainObject>.Is.Anything));
      _collectionEndPointMock.VerifyAllExpectations();
      Assert.That (result, Is.False);
    }

    [Test]
    public void Remove_ChecksErrorConditions ()
    {
      CheckClientTransactionDiffersException ((data, relatedObjectInOtherTransaction) => data.Remove (relatedObjectInOtherTransaction));
      CheckObjectDeletedException ((data, deletedRelatedObject) => data.Remove (deletedRelatedObject));
      CheckOwningObjectDeletedException ((data, relatedObject) => data.Remove (relatedObject));
    }

    [Test]
    public void Remove_ID ()
    {
      _endPointDataStub.Stub (stub => stub.GetObject (_orderItem1.ID)).Return (_orderItem1);
      _collectionEndPointMock.Expect (mock => mock.GetData ()).Return (_endPointDataDecorator);
      _collectionEndPointMock.Expect (mock => mock.CreateRemoveCommand (_orderItem1)).Return (_commandStub);
      _collectionEndPointMock.Expect (mock => mock.Touch());
      _collectionEndPointMock.Replay();
      _commandStub.Stub (stub => stub.ExpandToAllRelatedObjects()).Return (_expandedCommandFake);

      var result = _delegatingData.Remove (_orderItem1.ID);

      _collectionEndPointMock.VerifyAllExpectations ();
      DataManagementCommandTestHelper.AssertNotifyAndPerformWasCalled (_nestedCommandMock);

      Assert.That (result, Is.True);
    }

    [Test]
    public void Remove_ID_ObjectNotContained ()
    {
      _endPointDataStub.Stub (stub => stub.GetObject (_orderItem1.ID)).Return (null);
      _collectionEndPointMock.Expect (mock => mock.GetData ()).Return (_endPointDataDecorator);
      _collectionEndPointMock.Expect (mock => mock.Touch ());
      _collectionEndPointMock.Replay ();

      var result = _delegatingData.Remove (_orderItem1.ID);

      _collectionEndPointMock.AssertWasNotCalled (mock => mock.CreateRemoveCommand (Arg<DomainObject>.Is.Anything));
      _collectionEndPointMock.VerifyAllExpectations();

      Assert.That (result, Is.False);
    }

    [Test]
    public void Remove_ID_ChecksErrorConditions ()
    {
      CheckOwningObjectDeletedException ((data, relatedObject) => data.Remove (relatedObject.ID));
    }

    [Test]
    public void Replace ()
    {
      _collectionEndPointMock.Expect (mock => mock.CreateReplaceCommand (17, _orderItem1)).Return (_commandStub);
      _collectionEndPointMock.Expect (mock => mock.Touch ());
      _collectionEndPointMock.Replay ();
      _commandStub.Stub (stub => stub.ExpandToAllRelatedObjects()).Return (_expandedCommandFake);

      _delegatingData.Replace (17, _orderItem1);

      _collectionEndPointMock.VerifyAllExpectations ();
      DataManagementCommandTestHelper.AssertNotifyAndPerformWasCalled (_nestedCommandMock);
    }

    [Test]
    public void Replace_ChecksErrorConditions ()
    {
      CheckClientTransactionDiffersException ((data, relatedObjectInOtherTransaction) => data.Replace (17, relatedObjectInOtherTransaction));
      CheckObjectDeletedException ((data, deletedRelatedObject) => data.Replace (17, deletedRelatedObject));
      CheckOwningObjectDeletedException ((data, relatedObject) => data.Replace (17, relatedObject));
    }

    [Test]
    public void Sort ()
    {
      Comparison<DomainObject> comparison = (one, two) => 0;

      _collectionEndPointMock.Expect (mock => mock.SortCurrentData (comparison));
    }

    [Test]
    public void Serializable ()
    {
      var data = new EndPointDelegatingCollectionData (_endPointID, new SerializableRelationEndPointProviderFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize (data);

      Assert.That (deserializedInstance.AssociatedEndPointID, Is.EqualTo (_endPointID));
      Assert.That (deserializedInstance.VirtualEndPointProvider, Is.Not.Null);
    }

    private ICollectionEndPoint CreateCollectionEndPointStub (ClientTransaction clientTransaction, Order owningOrder)
    {
      var endPointStub = MockRepository.GenerateStub<ICollectionEndPoint>();
      StubCollectionEndPoint (endPointStub, clientTransaction, owningOrder);
      return endPointStub;
    }

    private void StubCollectionEndPoint (ICollectionEndPoint endPointStub, ClientTransaction clientTransaction, Order owningOrder)
    {
      endPointStub.Stub (stub => stub.ClientTransaction).Return (clientTransaction);
      var relationEndPointDefinition = owningOrder.ID.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");
      endPointStub.Stub (mock => mock.ObjectID).Return (owningOrder.ID);
      endPointStub.Stub (mock => mock.Definition).Return (relationEndPointDefinition);
      endPointStub.Stub (mock => mock.GetDomainObject ()).Return (owningOrder);
      endPointStub.Stub (mock => mock.GetDomainObjectReference ()).Return (owningOrder);
    }

    private void CheckClientTransactionDiffersException (Action<EndPointDelegatingCollectionData, DomainObject> action)
    {
      var orderItemInOtherTransaction = DomainObjectMother.CreateObjectInTransaction<OrderItem> (ClientTransaction.CreateRootTransaction());
      try
      {
        action (_delegatingData, orderItemInOtherTransaction);
        Assert.Fail ("Expected ClientTransactionsDifferException");
      }
      catch (ClientTransactionsDifferException)
      {
        // ok
      }
    }

    private void CheckObjectDeletedException (Action<EndPointDelegatingCollectionData, DomainObject> action)
    {
      OrderItem deletedObject;
      using (_delegatingData.GetAssociatedEndPoint().ClientTransaction.EnterNonDiscardingScope())
      {
        deletedObject = DomainObjectIDs.OrderItem5.GetObject<OrderItem>();
        deletedObject.Delete();
      }

      try
      {
        action (_delegatingData, deletedObject);
        Assert.Fail ("Expected ObjectDeletedException");
      }
      catch (ObjectDeletedException)
      {
        // ok
      }
    }

    private void CheckOwningObjectDeletedException (Action<EndPointDelegatingCollectionData, DomainObject> action)
    {
      Order deletedOwningObject;
      using (_delegatingData.GetAssociatedEndPoint().ClientTransaction.EnterNonDiscardingScope())
      {
        deletedOwningObject = DomainObjectIDs.Order5.GetObject<Order> ();
      }

      var endPointStub = CreateCollectionEndPointStub (TestableClientTransaction, deletedOwningObject);
      var virtualEndPointProviderStub = MockRepository.GenerateStub<IVirtualEndPointProvider> ();
      virtualEndPointProviderStub.Stub (stub => stub.GetOrCreateVirtualEndPoint (_endPointID)).Return (endPointStub);
      var data = new EndPointDelegatingCollectionData (_endPointID, virtualEndPointProviderStub);

      using (_delegatingData.GetAssociatedEndPoint().ClientTransaction.EnterNonDiscardingScope())
      {
        deletedOwningObject.Delete();
      }

      try
      {
        action (data, _orderItem1);
        Assert.Fail ("Expected ObjectDeletedException");
      }
      catch (ObjectDeletedException)
      {
        // ok
      }
    }
  }
}