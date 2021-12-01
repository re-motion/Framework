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
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class ClientTransactionExtensionRelationChangesTest : ClientTransactionBaseTest
  {
    private Order _order1;
    private OrderTicket _orderTicket1;
    private Location _location1;
    private Client _client1;

    private Mock<DomainObjectMockEventReceiver> _order1EventReceiver;
    private Mock<DomainObjectMockEventReceiver> _orderTicket1EventReceiver;
    private Mock<DomainObjectMockEventReceiver> _location1EventReceiver;
    private Mock<IClientTransactionExtension> _extension;

    public override void SetUp ()
    {
      base.SetUp();

      _order1 = DomainObjectIDs.Order1.GetObject<Order>();
      _orderTicket1 = _order1.OrderTicket;
      _location1 = DomainObjectIDs.Location1.GetObject<Location>();
      _client1 = _location1.Client;

      _extension = new Mock<IClientTransactionExtension> (MockBehavior.Strict);
      _order1EventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, _order1);
      _orderTicket1EventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, _orderTicket1);
      _location1EventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, _location1);
      new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, _client1).Object; // no events must be signalled for _client1

      _extension.Setup (stub => stub.Key).Returns ("Name");
      ClientTransactionScope.CurrentTransaction.Extensions.Add( _extension.Object);
      _extension.BackToRecord();
    }

    public override void TearDown ()
    {
      ClientTransactionScope.CurrentTransaction.Extensions.Remove("Name");

      base.TearDown();
    }

    [Test]
    public void OneToOneRelationFromVirtualEndPointWithSameObject ()
    {
      _order1.OrderTicket = _orderTicket1;

      _extension.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
    }

    [Test]
    public void OneToOneRelationFromVirtualEndPointWithNewNull ()
    {
      var sequence = new MockSequence();
      _extension.Object.RelationChanging(
            TestableClientTransaction,
            _order1,
            GetEndPointDefinition(typeof(Order), "OrderTicket"),
            _orderTicket1,
            null);
      _order1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Order), "OrderTicket"), _orderTicket1, null);
      _extension.Object.RelationChanging(
            TestableClientTransaction, _orderTicket1, GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null);
      _orderTicket1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null);
      _orderTicket1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null);
      _extension.Object.RelationChanged(TestableClientTransaction, _orderTicket1, GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null);
      _order1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Order), "OrderTicket"), _orderTicket1, null);
      _extension.Object.RelationChanged(TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), _orderTicket1, null);

      _order1.OrderTicket = null;

      _extension.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
    }

    [Test]
    public void OneToOneRelationFromVirtualEndPointWithOldNull ()
    {
      Order order = Order.NewObject();
      OrderTicket orderTicket = OrderTicket.NewObject();

      _mockRepository.BackToRecord(_extension.Object);

      var orderEventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, order);
      var orderTicketEventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, orderTicket);

      var sequence = new MockSequence();

      _extension.Object.RelationChanging(
            TestableClientTransaction, order, GetEndPointDefinition(typeof(Order), "OrderTicket"), null, orderTicket);

      orderEventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Order), "OrderTicket"), null, orderTicket);

      _extension.Object.RelationChanging(
            TestableClientTransaction, orderTicket, GetEndPointDefinition(typeof(OrderTicket), "Order"), null, order);

      orderTicketEventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderTicket), "Order"), null, order);

      orderTicketEventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderTicket), "Order"), null, order);

      _extension.Object.RelationChanged(TestableClientTransaction, orderTicket, GetEndPointDefinition(typeof(OrderTicket), "Order"), null, order);

      orderEventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Order), "OrderTicket"), null, orderTicket);

      _extension.Object.RelationChanged(TestableClientTransaction, order, GetEndPointDefinition(typeof(Order), "OrderTicket"), null, orderTicket);

      order.OrderTicket = orderTicket;

      _extension.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
      orderEventReceiver.Verify();
      orderTicketEventReceiver.Verify();
    }

    [Test]
    public void OneToOneRelationFromVirtualEndPointWithBothOldRelatedObjects ()
    {
      OrderTicket orderTicket3 = DomainObjectIDs.OrderTicket3.GetObject<OrderTicket>();
      Order oldOrderOfOrderTicket3 = orderTicket3.Order;
      oldOrderOfOrderTicket3.EnsureDataAvailable();

      var orderTicket3EventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, orderTicket3);
      var oldOrderOfOrderTicket3EventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, oldOrderOfOrderTicket3);
      _mockRepository.BackToRecord(_extension.Object);

      var sequence = new MockSequence();

      _extension.Object.RelationChanging(
            TestableClientTransaction, orderTicket3, GetEndPointDefinition(typeof(OrderTicket), "Order"), oldOrderOfOrderTicket3, _order1);

      orderTicket3EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderTicket), "Order"), oldOrderOfOrderTicket3, _order1);

      _extension.Object.RelationChanging(
            TestableClientTransaction, oldOrderOfOrderTicket3, GetEndPointDefinition(typeof(Order), "OrderTicket"), orderTicket3, null);

      oldOrderOfOrderTicket3EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Order), "OrderTicket"), orderTicket3, null);

      _extension.Object.RelationChanging(
            TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), _orderTicket1, orderTicket3);

      _order1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Order), "OrderTicket"), _orderTicket1, orderTicket3);

      _extension.Object.RelationChanging(
            TestableClientTransaction, _orderTicket1, GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null);

      _orderTicket1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null);

      _orderTicket1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null);

      _extension.Object.RelationChanged(TestableClientTransaction, _orderTicket1, GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null);

      _order1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Order), "OrderTicket"), _orderTicket1, orderTicket3);

      _extension.Object.RelationChanged(TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), _orderTicket1, orderTicket3);

      oldOrderOfOrderTicket3EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Order), "OrderTicket"), orderTicket3, null);

      _extension.Object.RelationChanged(
            TestableClientTransaction, oldOrderOfOrderTicket3, GetEndPointDefinition(typeof(Order), "OrderTicket"), orderTicket3, null);

      orderTicket3EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderTicket), "Order"), oldOrderOfOrderTicket3, _order1);

      _extension.Object.RelationChanged(TestableClientTransaction, orderTicket3, GetEndPointDefinition(typeof(OrderTicket), "Order"), oldOrderOfOrderTicket3, _order1);

      _mockRepository.ReplayAll();

      orderTicket3.Order = _order1;

      _extension.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
      orderTicket3EventReceiver.Verify();
      oldOrderOfOrderTicket3EventReceiver.Verify();
    }

    [Test]
    public void OneToOneRelationFromEndPointWithSameObject ()
    {
      _orderTicket1.Order = _order1;

      _extension.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
    }

    [Test]
    public void OneToOneRelationFromEndPointWithBothOldRelatedObjects ()
    {
      OrderTicket orderTicket3 = DomainObjectIDs.OrderTicket3.GetObject<OrderTicket>();
      Order oldOrderOfOrderTicket3 = orderTicket3.Order;
      oldOrderOfOrderTicket3.EnsureDataAvailable();

      var orderTicket3EventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, orderTicket3);
      var oldOrderOfOrderTicket3EventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, oldOrderOfOrderTicket3);
      _mockRepository.BackToRecord(_extension.Object);

      using (_mockRepository.Ordered())
      {
        _extension.Object.RelationChanging(
            TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), _orderTicket1, orderTicket3);
        _order1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Order), "OrderTicket"), _orderTicket1, orderTicket3);

        _extension.Object.RelationChanging(
            TestableClientTransaction, _orderTicket1, GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null);
        _orderTicket1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null);

        _extension.Object.RelationChanging(
            TestableClientTransaction, orderTicket3, GetEndPointDefinition(typeof(OrderTicket), "Order"), oldOrderOfOrderTicket3, _order1);
        orderTicket3EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderTicket), "Order"), oldOrderOfOrderTicket3, _order1);

        _extension.Object.RelationChanging(
            TestableClientTransaction, oldOrderOfOrderTicket3, GetEndPointDefinition(typeof(Order), "OrderTicket"), orderTicket3, null);
        oldOrderOfOrderTicket3EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Order), "OrderTicket"), orderTicket3, null);

        oldOrderOfOrderTicket3EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Order), "OrderTicket"), orderTicket3, null);
        _extension.Object.RelationChanged(
            TestableClientTransaction, oldOrderOfOrderTicket3, GetEndPointDefinition(typeof(Order), "OrderTicket"), orderTicket3, null);

        orderTicket3EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderTicket), "Order"), oldOrderOfOrderTicket3, _order1);
        _extension.Object.RelationChanged(TestableClientTransaction, orderTicket3, GetEndPointDefinition(typeof(OrderTicket), "Order"), oldOrderOfOrderTicket3, _order1);

        _orderTicket1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null);
        _extension.Object.RelationChanged(TestableClientTransaction, _orderTicket1, GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null);

        _order1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Order), "OrderTicket"), _orderTicket1, orderTicket3);
        _extension.Object.RelationChanged(TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), _orderTicket1, orderTicket3);
      }

      _order1.OrderTicket = orderTicket3;

      _extension.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
      orderTicket3EventReceiver.Verify();
      oldOrderOfOrderTicket3EventReceiver.Verify();
    }

    [Test]
    public void UnidirectionalRelationWithSameObject ()
    {
      _location1.Client = _client1;

      _extension.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
    }

    [Test]
    public void UnidirectionalRelationWithNewNull ()
    {
      var sequence = new MockSequence();
      _extension.Object.RelationChanging(TestableClientTransaction, _location1, GetEndPointDefinition(typeof(Location), "Client"), _client1, null);
      _location1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Location), "Client"), _client1, null);
      _location1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Location), "Client"), _client1, null);
      _extension.Object.RelationChanged(TestableClientTransaction, _location1, GetEndPointDefinition(typeof(Location), "Client"), _client1, null);

      _location1.Client = null;

      _extension.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
    }

    [Test]
    public void UnidirectionalRelationWithOldNull ()
    {
      Location newLocation = Location.NewObject();

      var newLocationEventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, newLocation);

      _mockRepository.BackToRecord(_extension.Object);
      var sequence = new MockSequence();
      _extension.Object.RelationChanging(TestableClientTransaction, newLocation, GetEndPointDefinition(typeof(Location), "Client"), null, _client1);
      newLocationEventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Location), "Client"), null, _client1);
      newLocationEventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Location), "Client"), null, _client1);
      _extension.Object.RelationChanged(TestableClientTransaction, newLocation, GetEndPointDefinition(typeof(Location), "Client"), null, _client1);

      newLocation.Client = _client1;

      _extension.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
      newLocationEventReceiver.Verify();
    }

    [Test]
    public void UnidirectionalRelationWithOldRelatedObject ()
    {
      Client newClient = Client.NewObject();
      new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, newClient).Object; // no events for newClient

      _mockRepository.BackToRecord(_extension.Object);

      var sequence = new MockSequence();

      _extension.Object.RelationChanging(
            TestableClientTransaction, _location1, GetEndPointDefinition(typeof(Location), "Client"), _client1, newClient);

      _location1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Location), "Client"), _client1, newClient);

      _location1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Location), "Client"), _client1, newClient);

      _extension.Object.RelationChanged(TestableClientTransaction, _location1, GetEndPointDefinition(typeof(Location), "Client"), _client1, newClient);

      _location1.Client = newClient;

      _extension.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
    }

    [Test]
    public void RemoveFromOneToManyRelation ()
    {
      DomainObjectCollection preloadedOrderItems = _order1.OrderItems;

      Assert.Greater(preloadedOrderItems.Count, 0);
      var orderItem = (OrderItem)preloadedOrderItems[0];

      _mockRepository.BackToRecord(_extension.Object);
      var orderItemEventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, orderItem);

      var sequence = new MockSequence();

      _extension.Object.RelationReading(
            TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), ValueAccess.Current);

      _extension.Expect (_ => _.RelationRead(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, TestableClientTransaction)),It.Is<DomainObject> (_ => object.ReferenceEquals (_, _order1)),It.Is<IRelationEndPointDefinition> (_ => object.Equals (_, GetEndPointDefinition(typeof(Order), "OrderItems"))),Arg<IReadOnlyCollectionData<DomainObject>>.Matches(_ => _ != null && object.Equals (_.Count, preloadedOrderItems.Count) &&new ContainsConstraint(preloadedOrderItems) ),It.Is<ValueAccess> (_ => object.Equals (_, ValueAccess.Current))));

      _extension.Object.RelationChanging(
            TestableClientTransaction, orderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);

      orderItemEventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);

      _extension.Object.RelationChanging(
            TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), orderItem, null);

      _order1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Order), "OrderItems"), orderItem, null);

      _order1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Order), "OrderItems"), orderItem, null);

      _extension.Object.RelationChanged(TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), orderItem, null);

      orderItemEventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);

      _extension.Object.RelationChanged(TestableClientTransaction, orderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);

      _order1.OrderItems.Remove(orderItem);

      _extension.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
      orderItemEventReceiver.Verify();
    }

    [Test]
    public void AddToOneToManyRelation ()
    {
      DomainObjectCollection preloadedOrderItems = _order1.OrderItems;
      preloadedOrderItems.EnsureDataComplete();
      OrderItem orderItem = OrderItem.NewObject();

      _mockRepository.BackToRecord(_extension.Object);
      var orderItemEventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, orderItem);

      var sequence = new MockSequence();

      _extension.Object.RelationReading(
            TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), ValueAccess.Current);

      _extension.Expect (_ => _.RelationRead(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, TestableClientTransaction)),It.Is<DomainObject> (_ => object.ReferenceEquals (_, _order1)),It.Is<IRelationEndPointDefinition> (_ => object.Equals (_, GetEndPointDefinition(typeof(Order), "OrderItems"))),Arg<IReadOnlyCollectionData<DomainObject>>.Matches(_ => _ != null && object.Equals (_.Count, preloadedOrderItems.Count) &&new ContainsConstraint(preloadedOrderItems) ),It.Is<ValueAccess> (_ => object.Equals (_, ValueAccess.Current))));

      _extension.Object.RelationChanging(TestableClientTransaction, orderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), null, _order1);

      orderItemEventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderItem), "Order"), null, _order1);

      _extension.Object.RelationChanging(TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), null, orderItem);

      _order1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Order), "OrderItems"), null, orderItem);

      _order1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Order), "OrderItems"), null, orderItem);

      _extension.Object.RelationChanged(TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), null, orderItem);

      orderItemEventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderItem), "Order"), null, _order1);

      _extension.Object.RelationChanged(TestableClientTransaction, orderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), null, _order1);

      _order1.OrderItems.Add(orderItem);

      _extension.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
      orderItemEventReceiver.Verify();
    }

    [Test]
    public void AddToOneToManyRelationWithOldRelatedObject ()
    {
      DomainObjectCollection preloadedOrderItemsOfOrder1 = _order1.OrderItems;
      preloadedOrderItemsOfOrder1.EnsureDataComplete();

      OrderItem newOrderItem = DomainObjectIDs.OrderItem3.GetObject<OrderItem>();
      Order oldOrderOfNewOrderItem = newOrderItem.Order;
      oldOrderOfNewOrderItem.EnsureDataAvailable();

      _mockRepository.BackToRecord(_extension.Object);
      var newOrderItemEventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, newOrderItem);
      var oldOrderOfNewOrderItemEventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, oldOrderOfNewOrderItem);

      var sequence = new MockSequence();

      _extension.Object.RelationReading(
            TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), ValueAccess.Current);

      _extension.Expect (_ => _.RelationRead(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, TestableClientTransaction)),It.Is<DomainObject> (_ => object.ReferenceEquals (_, _order1)),It.Is<IRelationEndPointDefinition> (_ => object.Equals (_, GetEndPointDefinition(typeof(Order), "OrderItems"))),Arg<IReadOnlyCollectionData<DomainObject>>.Matches(_ => _ != null && object.Equals (_.Count, preloadedOrderItemsOfOrder1.Count) &&new ContainsConstraint(preloadedOrderItemsOfOrder1) ),It.Is<ValueAccess> (_ => object.Equals (_, ValueAccess.Current))));

      _extension.Object.RelationChanging(
            TestableClientTransaction, newOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), oldOrderOfNewOrderItem, _order1);

      newOrderItemEventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderItem), "Order"), oldOrderOfNewOrderItem, _order1);

      _extension.Object.RelationChanging(
            TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), null, newOrderItem);

      _order1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Order), "OrderItems"), null, newOrderItem);

      _extension.Object.RelationChanging(
            TestableClientTransaction, oldOrderOfNewOrderItem, GetEndPointDefinition(typeof(Order), "OrderItems"), newOrderItem, null);

      oldOrderOfNewOrderItemEventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Order), "OrderItems"), newOrderItem, null);

      oldOrderOfNewOrderItemEventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Order), "OrderItems"), newOrderItem, null);

      _extension.Object.RelationChanged(
            TestableClientTransaction, oldOrderOfNewOrderItem, GetEndPointDefinition(typeof(Order), "OrderItems"), newOrderItem, null);

      _order1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Order), "OrderItems"), null, newOrderItem);

      _extension.Object.RelationChanged(TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), null, newOrderItem);

      newOrderItemEventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderItem), "Order"), oldOrderOfNewOrderItem, _order1);

      _extension.Object.RelationChanged(TestableClientTransaction, newOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), oldOrderOfNewOrderItem, _order1);

      _order1.OrderItems.Add(newOrderItem);

      _extension.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
      newOrderItemEventReceiver.Verify();
      oldOrderOfNewOrderItemEventReceiver.Verify();
    }

    [Test]
    public void ReplaceInOneToManyRelationWithSameObject ()
    {
      DomainObjectCollection orderItems = _order1.OrderItems;
      OrderItem oldOrderItem = _order1.OrderItems[0];

      _mockRepository.BackToRecord(_extension.Object);

      orderItems[0] = oldOrderItem;

      _extension.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
    }

    [Test]
    public void ReplaceInOneToManyRelation ()
    {
      Assert.Greater(_order1.OrderItems.Count, 0);
      OrderItem oldOrderItem = _order1.OrderItems[0];

      DomainObjectCollection preloadedOrderItems = _order1.OrderItems;
      OrderItem newOrderItem = OrderItem.NewObject();

      _mockRepository.BackToRecord(_extension.Object);
      var oldOrderItemEventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, oldOrderItem);
      var newOrderItemEventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, newOrderItem);

      using (_mockRepository.Ordered())
      {
        _extension.Object.RelationReading(
            TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), ValueAccess.Current);
        _extension.Expect (_ => _.RelationRead(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, TestableClientTransaction)),It.Is<DomainObject> (_ => object.ReferenceEquals (_, _order1)),It.Is<IRelationEndPointDefinition> (_ => object.Equals (_, GetEndPointDefinition(typeof(Order), "OrderItems"))),Arg<IReadOnlyCollectionData<DomainObject>>.Matches(_ => _ != null && object.Equals (_.Count, preloadedOrderItems.Count) &&new ContainsConstraint(preloadedOrderItems) ),It.Is<ValueAccess> (_ => object.Equals (_, ValueAccess.Current))));

        _extension.Object.RelationChanging(
            TestableClientTransaction, oldOrderItem,GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);
        oldOrderItemEventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);

        _extension.Object.RelationChanging(
            TestableClientTransaction, newOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), null, _order1);
        newOrderItemEventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderItem), "Order"), null, _order1);

        _extension.Object.RelationChanging(
            TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), oldOrderItem, newOrderItem);
        _order1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Order), "OrderItems"), oldOrderItem, newOrderItem);

        _order1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Order), "OrderItems"), oldOrderItem, newOrderItem);
        _extension.Object.RelationChanged(TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), oldOrderItem, newOrderItem);

        newOrderItemEventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderItem), "Order"), null, _order1);
        _extension.Object.RelationChanged(TestableClientTransaction, newOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), null, _order1);

        oldOrderItemEventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);
        _extension.Object.RelationChanged(TestableClientTransaction, oldOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);
      }

      _order1.OrderItems[0] = newOrderItem;

      _extension.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
      oldOrderItemEventReceiver.Verify();
      newOrderItemEventReceiver.Verify();
    }

    [Test]
    public void ReplaceInOneToManyRelationWithOldRelatedObject ()
    {
      Assert.Greater(_order1.OrderItems.Count, 0);
      OrderItem oldOrderItem = _order1.OrderItems[0];

      DomainObjectCollection preloadedOrderItemsOfOrder1 = _order1.OrderItems;
      OrderItem newOrderItem = DomainObjectIDs.OrderItem3.GetObject<OrderItem>();
      Order oldOrderOfNewOrderItem = newOrderItem.Order;
      Dev.Null = oldOrderOfNewOrderItem.OrderItems; // preload

      _mockRepository.BackToRecord(_extension.Object);
      var oldOrderItemEventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, oldOrderItem);
      var newOrderItemEventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, newOrderItem);
      var oldOrderOfNewOrderItemEventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, oldOrderOfNewOrderItem);

      var sequence = new MockSequence();

      _extension.Object.RelationReading(
            TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), ValueAccess.Current);

      _extension.Expect (_ => _.RelationRead(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, TestableClientTransaction)),It.Is<DomainObject> (_ => object.ReferenceEquals (_, _order1)),It.Is<IRelationEndPointDefinition> (_ => object.Equals (_, GetEndPointDefinition(typeof(Order), "OrderItems"))),Arg<IReadOnlyCollectionData<DomainObject>>.Matches(_ => _ != null && object.Equals (_.Count, preloadedOrderItemsOfOrder1.Count) &&new ContainsConstraint(preloadedOrderItemsOfOrder1) ),It.Is<ValueAccess> (_ => object.Equals (_, ValueAccess.Current))));

      _extension.Object.RelationChanging(
            TestableClientTransaction, oldOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);

      oldOrderItemEventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);

      _extension.Object.RelationChanging(
            TestableClientTransaction, newOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), oldOrderOfNewOrderItem, _order1);

      newOrderItemEventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderItem), "Order"), oldOrderOfNewOrderItem, _order1);

      _extension.Object.RelationChanging(
            TestableClientTransaction, _order1,GetEndPointDefinition(typeof(Order), "OrderItems"), oldOrderItem, newOrderItem);

      _order1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Order), "OrderItems"), oldOrderItem, newOrderItem);

      _extension.Object.RelationChanging(
            TestableClientTransaction, oldOrderOfNewOrderItem, GetEndPointDefinition(typeof(Order), "OrderItems"), newOrderItem, null);

      oldOrderOfNewOrderItemEventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Order), "OrderItems"), newOrderItem, null);

      oldOrderOfNewOrderItemEventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Order), "OrderItems"), newOrderItem, null);

      _extension.Object.RelationChanged(TestableClientTransaction, oldOrderOfNewOrderItem, GetEndPointDefinition(typeof(Order), "OrderItems"), newOrderItem, null);

      _order1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Order), "OrderItems"), oldOrderItem, newOrderItem);

      _extension.Object.RelationChanged(TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), oldOrderItem, newOrderItem);

      newOrderItemEventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderItem), "Order"), oldOrderOfNewOrderItem, _order1);

      _extension.Object.RelationChanged(TestableClientTransaction, newOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), oldOrderOfNewOrderItem, _order1);

      oldOrderItemEventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);

      _extension.Object.RelationChanged(TestableClientTransaction, oldOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);

      _order1.OrderItems[0] = newOrderItem;

      _extension.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
      oldOrderItemEventReceiver.Verify();
      newOrderItemEventReceiver.Verify();
      oldOrderOfNewOrderItemEventReceiver.Verify();
    }

    [Test]
    public void ReplaceWholeCollectionInOneToManyRelation ()
    {
      var oldCollection = _order1.OrderItems;
      var removedOrderItem = oldCollection[0];
      var stayingOrderItem = oldCollection[1];
      var addedOrderItem = OrderItem.NewObject();

      var newCollection = new ObjectList<OrderItem> { stayingOrderItem, addedOrderItem };

      _mockRepository.BackToRecord(_extension.Object);
      var removedOrderItemEventReceiverMock = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, removedOrderItem);
      new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, stayingOrderItem).Object;
      var addedOrderItemEventReceiverMock = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, addedOrderItem);

      var sequence = new MockSequence();
      _extension.InSequence (sequence).Setup (mock => mock.RelationChanging (
            TestableClientTransaction, removedOrderItem, GetEndPointDefinition (typeof(OrderItem), "Order"), _order1, null)).Verifiable();
      removedOrderItemEventReceiverMock.InSequence (sequence).Setup (mock => mock.RelationChanging (GetEndPointDefinition (typeof(OrderItem), "Order"), _order1, null)).Verifiable();
      _extension.InSequence (sequence).Setup (mock => mock.RelationChanging (
            TestableClientTransaction, addedOrderItem, GetEndPointDefinition (typeof(OrderItem), "Order"), null, _order1)).Verifiable();
      addedOrderItemEventReceiverMock.InSequence (sequence).Setup (mock => mock.RelationChanging (GetEndPointDefinition (typeof(OrderItem), "Order"), null, _order1)).Verifiable();
      _extension.InSequence (sequence).Setup (mock => mock.RelationChanging (
            TestableClientTransaction, _order1, GetEndPointDefinition (typeof(Order), "OrderItems"), removedOrderItem, null)).Verifiable();
      _order1EventReceiver.InSequence (sequence).Setup (mock => mock.RelationChanging (GetEndPointDefinition (typeof(Order), "OrderItems"), removedOrderItem, null)).Verifiable();
      _extension.InSequence (sequence).Setup (mock => mock.RelationChanging (
            TestableClientTransaction, _order1, GetEndPointDefinition (typeof(Order), "OrderItems"), null, addedOrderItem)).Verifiable();
      _order1EventReceiver.InSequence (sequence).Setup (mock => mock.RelationChanging (GetEndPointDefinition (typeof(Order), "OrderItems"), null, addedOrderItem)).Verifiable();
      _order1EventReceiver.InSequence (sequence).Setup (mock => mock.RelationChanged (GetEndPointDefinition (typeof(Order), "OrderItems"), null, addedOrderItem)).Verifiable();
      _extension.InSequence (sequence).Setup (mock => mock.RelationChanged (TestableClientTransaction, _order1, GetEndPointDefinition (typeof(Order), "OrderItems"), null, addedOrderItem)).Verifiable();
      _order1EventReceiver.InSequence (sequence).Setup (mock => mock.RelationChanged (GetEndPointDefinition (typeof(Order), "OrderItems"), removedOrderItem, null)).Verifiable();
      _extension.InSequence (sequence).Setup (mock => mock.RelationChanged (TestableClientTransaction, _order1, GetEndPointDefinition (typeof(Order), "OrderItems"), removedOrderItem, null)).Verifiable();
      addedOrderItemEventReceiverMock.InSequence (sequence).Setup (mock => mock.RelationChanged (GetEndPointDefinition (typeof(OrderItem), "Order"), null, _order1)).Verifiable();
      _extension.InSequence (sequence).Setup (mock => mock.RelationChanged (TestableClientTransaction, addedOrderItem, GetEndPointDefinition (typeof(OrderItem), "Order"), null, _order1)).Verifiable();
      removedOrderItemEventReceiverMock.InSequence (sequence).Setup (mock => mock.RelationChanged (GetEndPointDefinition (typeof(OrderItem), "Order"), _order1, null)).Verifiable();
      _extension.InSequence (sequence).Setup (mock => mock.RelationChanged (TestableClientTransaction, removedOrderItem, GetEndPointDefinition (typeof(OrderItem), "Order"), _order1, null)).Verifiable();

      _order1.OrderItems = newCollection;

      _extension.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
      removedOrderItemEventReceiverMock.Verify();
      addedOrderItemEventReceiverMock.Verify();
    }
  }
}
