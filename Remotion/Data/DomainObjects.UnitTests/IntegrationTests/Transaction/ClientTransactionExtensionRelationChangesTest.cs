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
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
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
    private ObjectList<OrderItem> _order1OrderItems;

    private Mock<IDomainObjectMockEventReceiver> _order1EventReceiver;
    private Mock<IDomainObjectMockEventReceiver> _orderTicket1EventReceiver;
    private Mock<IDomainObjectMockEventReceiver> _location1EventReceiver;
    private Mock<IDomainObjectMockEventReceiver> _client1EventReceiver;

    public override void SetUp ()
    {
      base.SetUp();

      _order1 = DomainObjectIDs.Order1.GetObject<Order>();
      _orderTicket1 = _order1.OrderTicket;
      _location1 = DomainObjectIDs.Location1.GetObject<Location>();
      _client1 = _location1.Client;

      _order1OrderItems = _order1.OrderItems;

      _order1EventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, _order1);
      _orderTicket1EventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, _orderTicket1);
      _location1EventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, _location1);
      _client1EventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, _client1); // no events must be signalled for _client1
    }

    public override void TearDown ()
    {
      TestableClientTransaction.Extensions.Remove("Name");

      base.TearDown();
    }

    [Test]
    public void OneToOneRelationFromVirtualEndPointWithSameObject ()
    {
      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction);
      // no calls on the extension are expected

      _order1.OrderTicket = _orderTicket1;

      extensionMock.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
      _client1EventReceiver.Verify();
    }

    [Test]
    public void OneToOneRelationFromVirtualEndPointWithNewNull ()
    {
      var orderTicketRelationEndPointDefinition = GetEndPointDefinition(typeof(Order), "OrderTicket");
      var orderRelationEndPointDefinition = GetEndPointDefinition(typeof(OrderTicket), "Order");

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, _order1, orderTicketRelationEndPointDefinition, _orderTicket1, null))
          .Verifiable();
      _order1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(_order1, orderTicketRelationEndPointDefinition, _orderTicket1, null)
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, _orderTicket1, orderRelationEndPointDefinition, _order1, null))
          .Verifiable();
      _orderTicket1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(_orderTicket1, orderRelationEndPointDefinition, _order1, null)
          .Verifiable();

      _orderTicket1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(_orderTicket1, orderRelationEndPointDefinition, _order1, null)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, _orderTicket1, orderRelationEndPointDefinition, _order1, null))
          .Verifiable();

      _order1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(_order1, orderTicketRelationEndPointDefinition, _orderTicket1, null)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, _order1, orderTicketRelationEndPointDefinition, _orderTicket1, null))
          .Verifiable();

      _order1.OrderTicket = null;

      extensionMock.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
      _client1EventReceiver.Verify();
      sequence.Verify();
    }

    [Test]
    public void OneToOneRelationFromVirtualEndPointWithOldNull ()
    {
      Order order = Order.NewObject();
      OrderTicket orderTicket = OrderTicket.NewObject();

      var orderEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, order);
      var orderTicketEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, orderTicket);

      var orderTicketRelationEndPointDefinition = GetEndPointDefinition(typeof(Order), "OrderTicket");
      var orderRelationEndPointDefinition = GetEndPointDefinition(typeof(OrderTicket), "Order");

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, order, orderTicketRelationEndPointDefinition, null, orderTicket))
          .Verifiable();
      orderEventReceiver.InVerifiableSequence(sequence).SetupRelationChanging(order, orderTicketRelationEndPointDefinition, null, orderTicket).Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, orderTicket, orderRelationEndPointDefinition, null, order))
          .Verifiable();
      orderTicketEventReceiver.InVerifiableSequence(sequence).SetupRelationChanging(orderTicket, orderRelationEndPointDefinition, null, order).Verifiable();

      orderTicketEventReceiver.InVerifiableSequence(sequence).SetupRelationChanged(orderTicket, orderRelationEndPointDefinition, null, order).Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, orderTicket, orderRelationEndPointDefinition, null, order))
          .Verifiable();

      orderEventReceiver.InVerifiableSequence(sequence).SetupRelationChanged(order, orderTicketRelationEndPointDefinition, null, orderTicket).Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, order, orderTicketRelationEndPointDefinition, null, orderTicket))
          .Verifiable();

      order.OrderTicket = orderTicket;

      extensionMock.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
      _client1EventReceiver.Verify();
      orderEventReceiver.Verify();
      orderTicketEventReceiver.Verify();
      sequence.Verify();
    }

    [Test]
    public void OneToOneRelationFromVirtualEndPointWithBothOldRelatedObjects ()
    {
      OrderTicket orderTicket3 = DomainObjectIDs.OrderTicket3.GetObject<OrderTicket>();
      Order oldOrderOfOrderTicket3 = orderTicket3.Order;
      oldOrderOfOrderTicket3.EnsureDataAvailable();

      var orderTicket3EventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, orderTicket3);
      var oldOrderOfOrderTicket3EventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, oldOrderOfOrderTicket3);

      var orderRelationEndPointDefinition = GetEndPointDefinition(typeof(OrderTicket), "Order");
      var orderTicketRelationEndPointDefinition = GetEndPointDefinition(typeof(Order), "OrderTicket");

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, orderTicket3, orderRelationEndPointDefinition, oldOrderOfOrderTicket3, _order1))
          .Verifiable();

      orderTicket3EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(orderTicket3, orderRelationEndPointDefinition, oldOrderOfOrderTicket3, _order1)
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, oldOrderOfOrderTicket3, orderTicketRelationEndPointDefinition, orderTicket3, null))
          .Verifiable();

      oldOrderOfOrderTicket3EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(oldOrderOfOrderTicket3, orderTicketRelationEndPointDefinition, orderTicket3, null)
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, _order1, orderTicketRelationEndPointDefinition, _orderTicket1, orderTicket3))
          .Verifiable();

      _order1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(_order1, orderTicketRelationEndPointDefinition, _orderTicket1, orderTicket3)
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, _orderTicket1, orderRelationEndPointDefinition, _order1, null))
          .Verifiable();

      _orderTicket1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(_orderTicket1, orderRelationEndPointDefinition, _order1, null)
          .Verifiable();

      _orderTicket1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(_orderTicket1, orderRelationEndPointDefinition, _order1, null)
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, _orderTicket1, orderRelationEndPointDefinition, _order1, null))
          .Verifiable();

      _order1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(_order1, orderTicketRelationEndPointDefinition, _orderTicket1, orderTicket3)
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, _order1, orderTicketRelationEndPointDefinition, _orderTicket1, orderTicket3))
          .Verifiable();

      oldOrderOfOrderTicket3EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(oldOrderOfOrderTicket3, orderTicketRelationEndPointDefinition, orderTicket3, null)
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, oldOrderOfOrderTicket3, orderTicketRelationEndPointDefinition, orderTicket3, null))
          .Verifiable();

      orderTicket3EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(orderTicket3, orderRelationEndPointDefinition, oldOrderOfOrderTicket3, _order1)
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, orderTicket3, orderRelationEndPointDefinition, oldOrderOfOrderTicket3, _order1))
          .Verifiable();

      orderTicket3.Order = _order1;

      extensionMock.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
      _client1EventReceiver.Verify();
      orderTicket3EventReceiver.Verify();
      oldOrderOfOrderTicket3EventReceiver.Verify();
      sequence.Verify();
    }

    [Test]
    public void OneToOneRelationFromEndPointWithSameObject ()
    {
      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction);
      // no calls on the extension are expected

      _orderTicket1.Order = _order1;

      extensionMock.Verify();
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

      var orderTicket3EventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, orderTicket3);
      var oldOrderOfOrderTicket3EventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, oldOrderOfOrderTicket3);

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), _orderTicket1, orderTicket3))
          .Verifiable();
      _order1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(_order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), _orderTicket1, orderTicket3)
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, _orderTicket1, GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null))
          .Verifiable();
      _orderTicket1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(_orderTicket1, GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null)
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, orderTicket3, GetEndPointDefinition(typeof(OrderTicket), "Order"), oldOrderOfOrderTicket3, _order1))
          .Verifiable();
      orderTicket3EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(orderTicket3, GetEndPointDefinition(typeof(OrderTicket), "Order"), oldOrderOfOrderTicket3, _order1)
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, oldOrderOfOrderTicket3, GetEndPointDefinition(typeof(Order), "OrderTicket"), orderTicket3, null))
          .Verifiable();
      oldOrderOfOrderTicket3EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(oldOrderOfOrderTicket3, GetEndPointDefinition(typeof(Order), "OrderTicket"), orderTicket3, null)
          .Verifiable();

      oldOrderOfOrderTicket3EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(oldOrderOfOrderTicket3, GetEndPointDefinition(typeof(Order), "OrderTicket"), orderTicket3, null)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, oldOrderOfOrderTicket3, GetEndPointDefinition(typeof(Order), "OrderTicket"), orderTicket3, null))
          .Verifiable();

      orderTicket3EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(orderTicket3, GetEndPointDefinition(typeof(OrderTicket), "Order"), oldOrderOfOrderTicket3, _order1)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, orderTicket3, GetEndPointDefinition(typeof(OrderTicket), "Order"), oldOrderOfOrderTicket3, _order1))
          .Verifiable();

      _orderTicket1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(_orderTicket1, GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, _orderTicket1, GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null))
          .Verifiable();

      _order1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(_order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), _orderTicket1, orderTicket3)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), _orderTicket1, orderTicket3))
          .Verifiable();

      _order1.OrderTicket = orderTicket3;

      extensionMock.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
      _client1EventReceiver.Verify();
      orderTicket3EventReceiver.Verify();
      oldOrderOfOrderTicket3EventReceiver.Verify();
      sequence.Verify();
    }

    [Test]
    public void UnidirectionalRelationWithSameObject ()
    {
      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction);
      // no calls on the extension are expected

      _location1.Client = _client1;

      extensionMock.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
      _client1EventReceiver.Verify();
    }

    [Test]
    public void UnidirectionalRelationWithNewNull ()
    {
      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, _location1, GetEndPointDefinition(typeof(Location), "Client"), _client1, null))
          .Verifiable();
      _location1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(_location1, GetEndPointDefinition(typeof(Location), "Client"), _client1, null)
          .Verifiable();

      _location1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(_location1, GetEndPointDefinition(typeof(Location), "Client"), _client1, null)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, _location1, GetEndPointDefinition(typeof(Location), "Client"), _client1, null))
          .Verifiable();

      _location1.Client = null;

      extensionMock.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
      _client1EventReceiver.Verify();
      sequence.Verify();
    }

    [Test]
    public void UnidirectionalRelationWithOldNull ()
    {
      Location newLocation = Location.NewObject();

      var newLocationEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, newLocation);

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newLocation, GetEndPointDefinition(typeof(Location), "Client"), null, _client1))
          .Verifiable();
      newLocationEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(newLocation, GetEndPointDefinition(typeof(Location), "Client"), null, _client1)
          .Verifiable();

      newLocationEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(newLocation, GetEndPointDefinition(typeof(Location), "Client"), null, _client1)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newLocation, GetEndPointDefinition(typeof(Location), "Client"), null, _client1))
          .Verifiable();

      newLocation.Client = _client1;

      extensionMock.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
      _client1EventReceiver.Verify();
      newLocationEventReceiver.Verify();
      sequence.Verify();
    }

    [Test]
    public void UnidirectionalRelationWithOldRelatedObject ()
    {
      Client newClient = Client.NewObject();
      var newClientEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, newClient); // no events for newClient

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, _location1, GetEndPointDefinition(typeof(Location), "Client"), _client1, newClient))
          .Verifiable();
      _location1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(_location1, GetEndPointDefinition(typeof(Location), "Client"), _client1, newClient)
          .Verifiable();

      _location1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(_location1, GetEndPointDefinition(typeof(Location), "Client"), _client1, newClient)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, _location1, GetEndPointDefinition(typeof(Location), "Client"), _client1, newClient))
          .Verifiable();

      _location1.Client = newClient;

      extensionMock.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
      _client1EventReceiver.Verify();
      newClientEventReceiver.Verify();
      sequence.Verify();
    }

    [Test]
    public void RemoveFromOneToManyRelation ()
    {
      ObjectList<OrderItem> preloadedOrderItems = _order1OrderItems;

      Assert.Greater(preloadedOrderItems.Count, 0);
      var orderItem = (OrderItem)preloadedOrderItems[0];

      var orderItemEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, orderItem);

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, orderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null))
          .Verifiable();
      orderItemEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(orderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null)
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), orderItem, null))
          .Verifiable();
      _order1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(_order1, GetEndPointDefinition(typeof(Order), "OrderItems"), orderItem, null)
          .Verifiable();

      _order1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(_order1, GetEndPointDefinition(typeof(Order), "OrderItems"), orderItem, null)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), orderItem, null))
          .Verifiable();

      orderItemEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(orderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, orderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null))
          .Verifiable();

      _order1OrderItems.Remove(orderItem);

      extensionMock.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
      _client1EventReceiver.Verify();
      orderItemEventReceiver.Verify();
      sequence.Verify();
    }

    [Test]
    public void AddToOneToManyRelation ()
    {
      ObjectList<OrderItem> preloadedOrderItems = _order1OrderItems;
      preloadedOrderItems.EnsureDataComplete();
      OrderItem orderItem = OrderItem.NewObject();

      var orderItemEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, orderItem);

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, orderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), null, _order1))
          .Verifiable();
      orderItemEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(orderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), null, _order1)
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), null, orderItem))
          .Verifiable();
      _order1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(_order1, GetEndPointDefinition(typeof(Order), "OrderItems"), null, orderItem)
          .Verifiable();

      _order1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(_order1, GetEndPointDefinition(typeof(Order), "OrderItems"), null, orderItem)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), null, orderItem))
          .Verifiable();

      orderItemEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(orderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), null, _order1)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, orderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), null, _order1))
          .Verifiable();

      _order1OrderItems.Add(orderItem);

      extensionMock.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
      _client1EventReceiver.Verify();
      orderItemEventReceiver.Verify();
      sequence.Verify();
    }

    [Test]
    public void AddToOneToManyRelationWithOldRelatedObject ()
    {
      ObjectList<OrderItem> preloadedOrderItemsOfOrder1 = _order1OrderItems;
      preloadedOrderItemsOfOrder1.EnsureDataComplete();

      OrderItem newOrderItem = DomainObjectIDs.OrderItem3.GetObject<OrderItem>();
      Order oldOrderOfNewOrderItem = newOrderItem.Order;
      oldOrderOfNewOrderItem.EnsureDataAvailable();

      var newOrderItemEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, newOrderItem);
      var oldOrderOfNewOrderItemEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, oldOrderOfNewOrderItem);

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), oldOrderOfNewOrderItem, _order1))
          .Verifiable();
      newOrderItemEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(newOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), oldOrderOfNewOrderItem, _order1)
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), null, newOrderItem))
          .Verifiable();
      _order1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(_order1, GetEndPointDefinition(typeof(Order), "OrderItems"), null, newOrderItem)
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, oldOrderOfNewOrderItem, GetEndPointDefinition(typeof(Order), "OrderItems"), newOrderItem, null))
          .Verifiable();
      oldOrderOfNewOrderItemEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(oldOrderOfNewOrderItem, GetEndPointDefinition(typeof(Order), "OrderItems"), newOrderItem, null)
          .Verifiable();

      oldOrderOfNewOrderItemEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(oldOrderOfNewOrderItem, GetEndPointDefinition(typeof(Order), "OrderItems"), newOrderItem, null)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, oldOrderOfNewOrderItem, GetEndPointDefinition(typeof(Order), "OrderItems"), newOrderItem, null))
          .Verifiable();

      _order1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(_order1, GetEndPointDefinition(typeof(Order), "OrderItems"), null, newOrderItem)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), null, newOrderItem))
          .Verifiable();

      newOrderItemEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(newOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), oldOrderOfNewOrderItem, _order1)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), oldOrderOfNewOrderItem, _order1))
          .Verifiable();

      _order1OrderItems.Add(newOrderItem);

      extensionMock.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
      _client1EventReceiver.Verify();
      newOrderItemEventReceiver.Verify();
      oldOrderOfNewOrderItemEventReceiver.Verify();
      sequence.Verify();
    }

    [Test]
    public void ReplaceInOneToManyRelationWithSameObject ()
    {
      DomainObjectCollection orderItems = _order1OrderItems;
      OrderItem oldOrderItem = _order1OrderItems[0];

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction);
      // no calls on the extension are expected

      orderItems[0] = oldOrderItem;

      extensionMock.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
      _client1EventReceiver.Verify();
    }

    [Test]
    public void ReplaceInOneToManyRelation ()
    {
      Assert.Greater(_order1OrderItems.Count, 0);
      OrderItem oldOrderItem = _order1OrderItems[0];

      ObjectList<OrderItem> preloadedOrderItems = _order1OrderItems;
      OrderItem newOrderItem = OrderItem.NewObject();

      var oldOrderItemEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, oldOrderItem);
      var newOrderItemEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, newOrderItem);

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, oldOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null))
          .Verifiable();
      oldOrderItemEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(oldOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null)
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), null, _order1))
          .Verifiable();
      newOrderItemEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(newOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), null, _order1)
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), oldOrderItem, newOrderItem))
          .Verifiable();
      _order1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(_order1, GetEndPointDefinition(typeof(Order), "OrderItems"), oldOrderItem, newOrderItem)
          .Verifiable();

      _order1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(_order1, GetEndPointDefinition(typeof(Order), "OrderItems"), oldOrderItem, newOrderItem)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), oldOrderItem, newOrderItem))
          .Verifiable();

      newOrderItemEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(newOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), null, _order1)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), null, _order1))
          .Verifiable();

      oldOrderItemEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(oldOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, oldOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null))
          .Verifiable();

      _order1OrderItems[0] = newOrderItem;

      extensionMock.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
      _client1EventReceiver.Verify();
      oldOrderItemEventReceiver.Verify();
      newOrderItemEventReceiver.Verify();
      sequence.Verify();
    }

    [Test]
    public void ReplaceInOneToManyRelationWithOldRelatedObject ()
    {
      Assert.Greater(_order1OrderItems.Count, 0);
      OrderItem oldOrderItem = _order1OrderItems[0];

      ObjectList<OrderItem> preloadedOrderItemsOfOrder1 = _order1OrderItems;
      OrderItem newOrderItem = DomainObjectIDs.OrderItem3.GetObject<OrderItem>();
      Order oldOrderOfNewOrderItem = newOrderItem.Order;
      Dev.Null = oldOrderOfNewOrderItem.OrderItems; // preload

      var oldOrderItemEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, oldOrderItem);
      var newOrderItemEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, newOrderItem);
      var oldOrderOfNewOrderItemEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, oldOrderOfNewOrderItem);

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, oldOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null))
          .Verifiable();

      oldOrderItemEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(oldOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null)
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), oldOrderOfNewOrderItem, _order1))
          .Verifiable();
      newOrderItemEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(newOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), oldOrderOfNewOrderItem, _order1)
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), oldOrderItem, newOrderItem))
          .Verifiable();
      _order1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(_order1, GetEndPointDefinition(typeof(Order), "OrderItems"), oldOrderItem, newOrderItem)
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, oldOrderOfNewOrderItem, GetEndPointDefinition(typeof(Order), "OrderItems"), newOrderItem, null))
          .Verifiable();
      oldOrderOfNewOrderItemEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(oldOrderOfNewOrderItem, GetEndPointDefinition(typeof(Order), "OrderItems"), newOrderItem, null)
          .Verifiable();

      oldOrderOfNewOrderItemEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(oldOrderOfNewOrderItem, GetEndPointDefinition(typeof(Order), "OrderItems"), newOrderItem, null)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, oldOrderOfNewOrderItem, GetEndPointDefinition(typeof(Order), "OrderItems"), newOrderItem, null))
          .Verifiable();

      _order1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(_order1, GetEndPointDefinition(typeof(Order), "OrderItems"), oldOrderItem, newOrderItem)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), oldOrderItem, newOrderItem))
          .Verifiable();

      newOrderItemEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(newOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), oldOrderOfNewOrderItem, _order1)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), oldOrderOfNewOrderItem, _order1))
          .Verifiable();

      oldOrderItemEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(oldOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, oldOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null))
          .Verifiable();

      _order1OrderItems[0] = newOrderItem;

      extensionMock.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
      _client1EventReceiver.Verify();
      oldOrderItemEventReceiver.Verify();
      newOrderItemEventReceiver.Verify();
      oldOrderOfNewOrderItemEventReceiver.Verify();
      sequence.Verify();
    }

    [Test]
    public void ReplaceWholeCollectionInOneToManyRelation ()
    {
      var oldCollection = _order1OrderItems;
      var removedOrderItem = oldCollection[0];
      var stayingOrderItem = oldCollection[1];
      var addedOrderItem = OrderItem.NewObject();

      var newCollection = new ObjectList<OrderItem> { stayingOrderItem, addedOrderItem };

      var removedOrderItemEventReceiverMock = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, removedOrderItem);
      var stayingOrderItemEventReceiverMock = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, stayingOrderItem);
      var addedOrderItemEventReceiverMock = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, addedOrderItem);

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          //.InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationChanging(TestableClientTransaction, removedOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null))
          .Verifiable();
      removedOrderItemEventReceiverMock
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(removedOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null)
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationChanging(TestableClientTransaction, addedOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), null, _order1))
          .Verifiable();
      addedOrderItemEventReceiverMock
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(addedOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), null, _order1)
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationChanging(TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), removedOrderItem, null))
          .Verifiable();
      _order1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(_order1, GetEndPointDefinition(typeof(Order), "OrderItems"), removedOrderItem, null)
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationChanging(TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), null, addedOrderItem))
          .Verifiable();
      _order1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanging(_order1, GetEndPointDefinition(typeof(Order), "OrderItems"), null, addedOrderItem)
          .Verifiable();

      _order1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(_order1, GetEndPointDefinition(typeof(Order), "OrderItems"), null, addedOrderItem)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationChanged(TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), null, addedOrderItem))
          .Verifiable();

      _order1EventReceiver
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(_order1, GetEndPointDefinition(typeof(Order), "OrderItems"), removedOrderItem, null)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationChanged(TestableClientTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), removedOrderItem, null))
          .Verifiable();

      addedOrderItemEventReceiverMock
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(addedOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), null, _order1)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationChanged(TestableClientTransaction, addedOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), null, _order1))
          .Verifiable();

      removedOrderItemEventReceiverMock
          .InVerifiableSequence(sequence)
          .SetupRelationChanged(removedOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationChanged(TestableClientTransaction, removedOrderItem, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null))
          .Verifiable();

      _order1.OrderItems = newCollection;

      extensionMock.Verify();
      _order1EventReceiver.Verify();
      _orderTicket1EventReceiver.Verify();
      _location1EventReceiver.Verify();
      _client1EventReceiver.Verify();
      removedOrderItemEventReceiverMock.Verify();
      addedOrderItemEventReceiverMock.Verify();
      stayingOrderItemEventReceiverMock.Verify();
      sequence.Verify();
    }

    private static Mock<IClientTransactionExtension> AddExtensionToClientTransaction (TestableClientTransaction transaction)
    {
      var extensionMock = new Mock<IClientTransactionExtension>(MockBehavior.Strict);
      extensionMock.Setup(stub => stub.Key).Returns("TestExtension");
      transaction.Extensions.Add(extensionMock.Object);
      extensionMock.Reset();

      return extensionMock;
    }
  }
}
