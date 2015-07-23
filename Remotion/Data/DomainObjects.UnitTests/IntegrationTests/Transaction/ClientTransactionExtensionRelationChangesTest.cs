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
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Is = Rhino.Mocks.Constraints.Is;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class ClientTransactionExtensionRelationChangesTest : ClientTransactionBaseTest
  {
    private Order _order1;
    private OrderTicket _orderTicket1;
    private Location _location1;
    private Client _client1;

    private DomainObjectMockEventReceiver _order1EventReceiver;
    private DomainObjectMockEventReceiver _orderTicket1EventReceiver;
    private DomainObjectMockEventReceiver _location1EventReceiver;
    private MockRepository _mockRepository;
    private IClientTransactionExtension _extension;

    public override void SetUp ()
    {
      base.SetUp();

      _order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      _orderTicket1 = _order1.OrderTicket;
      _location1 = DomainObjectIDs.Location1.GetObject<Location>();
      _client1 = _location1.Client;

      _mockRepository = new MockRepository();

      _extension = _mockRepository.StrictMock<IClientTransactionExtension>();
      _order1EventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_order1);
      _orderTicket1EventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_orderTicket1);
      _location1EventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_location1);
      _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_client1); // no events must be signalled for _client1

      _extension.Stub (stub => stub.Key).Return ("Name");
      _extension.Replay();
      ClientTransactionScope.CurrentTransaction.Extensions.Add ( _extension);
      _extension.BackToRecord();
    }

    public override void TearDown ()
    {
      ClientTransactionScope.CurrentTransaction.Extensions.Remove ("Name");

      base.TearDown ();
    }

    [Test]
    public void OneToOneRelationFromVirtualEndPointWithSameObject ()
    {
      // no calls on the extension are expected

      _mockRepository.ReplayAll();

      _order1.OrderTicket = _orderTicket1;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void OneToOneRelationFromVirtualEndPointWithNewNull ()
    {
      using (_mockRepository.Ordered())
      {
        _extension.RelationChanging (
            TestableClientTransaction,
            _order1,
            GetEndPointDefinition (typeof (Order), "OrderTicket"),
            _orderTicket1,
            null);
        _order1EventReceiver.RelationChanging (GetEndPointDefinition (typeof (Order), "OrderTicket"), _orderTicket1, null);

        _extension.RelationChanging (
            TestableClientTransaction, _orderTicket1, GetEndPointDefinition (typeof (OrderTicket), "Order"), _order1, null);
        _orderTicket1EventReceiver.RelationChanging (GetEndPointDefinition (typeof (OrderTicket), "Order"), _order1, null);

        _orderTicket1EventReceiver.RelationChanged (GetEndPointDefinition (typeof (OrderTicket), "Order"), _order1, null);
        _extension.RelationChanged (TestableClientTransaction, _orderTicket1, GetEndPointDefinition (typeof (OrderTicket), "Order"), _order1, null);

        _order1EventReceiver.RelationChanged (GetEndPointDefinition (typeof (Order), "OrderTicket"), _orderTicket1, null);
        _extension.RelationChanged (TestableClientTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderTicket"), _orderTicket1, null);
      }

      _mockRepository.ReplayAll();

      _order1.OrderTicket = null;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void OneToOneRelationFromVirtualEndPointWithOldNull ()
    {
      Order order = Order.NewObject();
      OrderTicket orderTicket = OrderTicket.NewObject();

      _mockRepository.BackToRecord (_extension);

      var orderEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (order);
      var orderTicketEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (orderTicket);

      using (_mockRepository.Ordered())
      {
        _extension.RelationChanging (
            TestableClientTransaction, order, GetEndPointDefinition (typeof (Order), "OrderTicket"), null, orderTicket);
        orderEventReceiver.RelationChanging (GetEndPointDefinition (typeof (Order), "OrderTicket"), null, orderTicket);

        _extension.RelationChanging (
            TestableClientTransaction, orderTicket, GetEndPointDefinition (typeof (OrderTicket), "Order"), null, order);
        orderTicketEventReceiver.RelationChanging (GetEndPointDefinition (typeof (OrderTicket), "Order"), null, order);

        orderTicketEventReceiver.RelationChanged (GetEndPointDefinition (typeof (OrderTicket), "Order"), null, order);
        _extension.RelationChanged (TestableClientTransaction, orderTicket, GetEndPointDefinition (typeof (OrderTicket), "Order"), null, order);

        orderEventReceiver.RelationChanged (GetEndPointDefinition (typeof (Order), "OrderTicket"), null, orderTicket);
        _extension.RelationChanged (TestableClientTransaction, order, GetEndPointDefinition (typeof (Order), "OrderTicket"), null, orderTicket);
      }

      _mockRepository.ReplayAll();

      order.OrderTicket = orderTicket;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void OneToOneRelationFromVirtualEndPointWithBothOldRelatedObjects ()
    {
      OrderTicket orderTicket3 = DomainObjectIDs.OrderTicket3.GetObject<OrderTicket> ();
      Order oldOrderOfOrderTicket3 = orderTicket3.Order;
      oldOrderOfOrderTicket3.EnsureDataAvailable ();

      var orderTicket3EventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (orderTicket3);
      var oldOrderOfOrderTicket3EventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (oldOrderOfOrderTicket3);
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.RelationChanging (
            TestableClientTransaction, orderTicket3, GetEndPointDefinition (typeof (OrderTicket), "Order"), oldOrderOfOrderTicket3, _order1);
        orderTicket3EventReceiver.RelationChanging (GetEndPointDefinition (typeof (OrderTicket), "Order"), oldOrderOfOrderTicket3, _order1);

        _extension.RelationChanging (
            TestableClientTransaction, oldOrderOfOrderTicket3, GetEndPointDefinition (typeof (Order), "OrderTicket"), orderTicket3, null);
        oldOrderOfOrderTicket3EventReceiver.RelationChanging (GetEndPointDefinition (typeof (Order), "OrderTicket"), orderTicket3, null);

        _extension.RelationChanging (
            TestableClientTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderTicket"), _orderTicket1, orderTicket3);
        _order1EventReceiver.RelationChanging (GetEndPointDefinition (typeof (Order), "OrderTicket"), _orderTicket1, orderTicket3);

        _extension.RelationChanging (
            TestableClientTransaction, _orderTicket1, GetEndPointDefinition (typeof (OrderTicket), "Order"), _order1, null);
        _orderTicket1EventReceiver.RelationChanging (GetEndPointDefinition (typeof (OrderTicket), "Order"), _order1, null);

        _orderTicket1EventReceiver.RelationChanged (GetEndPointDefinition (typeof (OrderTicket), "Order"), _order1, null);
        _extension.RelationChanged (TestableClientTransaction, _orderTicket1, GetEndPointDefinition (typeof (OrderTicket), "Order"), _order1, null);

        _order1EventReceiver.RelationChanged (GetEndPointDefinition (typeof (Order), "OrderTicket"), _orderTicket1, orderTicket3);
        _extension.RelationChanged (TestableClientTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderTicket"), _orderTicket1, orderTicket3);

        oldOrderOfOrderTicket3EventReceiver.RelationChanged (GetEndPointDefinition (typeof (Order), "OrderTicket"), orderTicket3, null);
        _extension.RelationChanged (
            TestableClientTransaction, oldOrderOfOrderTicket3, GetEndPointDefinition (typeof (Order), "OrderTicket"), orderTicket3, null);

        orderTicket3EventReceiver.RelationChanged (GetEndPointDefinition (typeof (OrderTicket), "Order"), oldOrderOfOrderTicket3, _order1);
        _extension.RelationChanged (TestableClientTransaction, orderTicket3, GetEndPointDefinition (typeof (OrderTicket), "Order"), oldOrderOfOrderTicket3, _order1);
      }

      _mockRepository.ReplayAll();

      orderTicket3.Order = _order1;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void OneToOneRelationFromEndPointWithSameObject ()
    {
      // no calls on the extension are expected

      _mockRepository.ReplayAll();

      _orderTicket1.Order = _order1;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void OneToOneRelationFromEndPointWithBothOldRelatedObjects ()
    {
      OrderTicket orderTicket3 = DomainObjectIDs.OrderTicket3.GetObject<OrderTicket> ();
      Order oldOrderOfOrderTicket3 = orderTicket3.Order;
      oldOrderOfOrderTicket3.EnsureDataAvailable();

      var orderTicket3EventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (orderTicket3);
      var oldOrderOfOrderTicket3EventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (oldOrderOfOrderTicket3);
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.RelationChanging (
            TestableClientTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderTicket"), _orderTicket1, orderTicket3);
        _order1EventReceiver.RelationChanging (GetEndPointDefinition (typeof (Order), "OrderTicket"), _orderTicket1, orderTicket3);

        _extension.RelationChanging (
            TestableClientTransaction, _orderTicket1, GetEndPointDefinition (typeof (OrderTicket), "Order"), _order1, null);
        _orderTicket1EventReceiver.RelationChanging (GetEndPointDefinition (typeof (OrderTicket), "Order"), _order1, null);

        _extension.RelationChanging (
            TestableClientTransaction, orderTicket3, GetEndPointDefinition (typeof (OrderTicket), "Order"), oldOrderOfOrderTicket3, _order1);
        orderTicket3EventReceiver.RelationChanging (GetEndPointDefinition (typeof (OrderTicket), "Order"), oldOrderOfOrderTicket3, _order1);

        _extension.RelationChanging (
            TestableClientTransaction, oldOrderOfOrderTicket3, GetEndPointDefinition (typeof (Order), "OrderTicket"), orderTicket3, null);
        oldOrderOfOrderTicket3EventReceiver.RelationChanging (GetEndPointDefinition (typeof (Order), "OrderTicket"), orderTicket3, null);

        
        oldOrderOfOrderTicket3EventReceiver.RelationChanged (GetEndPointDefinition (typeof (Order), "OrderTicket"), orderTicket3, null);
        _extension.RelationChanged (
            TestableClientTransaction, oldOrderOfOrderTicket3, GetEndPointDefinition (typeof (Order), "OrderTicket"), orderTicket3, null);
        
        orderTicket3EventReceiver.RelationChanged (GetEndPointDefinition (typeof (OrderTicket), "Order"), oldOrderOfOrderTicket3, _order1);
        _extension.RelationChanged (TestableClientTransaction, orderTicket3, GetEndPointDefinition (typeof (OrderTicket), "Order"), oldOrderOfOrderTicket3, _order1);
        
        _orderTicket1EventReceiver.RelationChanged (GetEndPointDefinition (typeof (OrderTicket), "Order"), _order1, null);
        _extension.RelationChanged (TestableClientTransaction, _orderTicket1, GetEndPointDefinition (typeof (OrderTicket), "Order"), _order1, null);

        _order1EventReceiver.RelationChanged (GetEndPointDefinition (typeof (Order), "OrderTicket"), _orderTicket1, orderTicket3);
        _extension.RelationChanged (TestableClientTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderTicket"), _orderTicket1, orderTicket3);
      }

      _mockRepository.ReplayAll();

      _order1.OrderTicket = orderTicket3;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void UnidirectionalRelationWithSameObject ()
    {
      // no calls on the extension are expected

      _mockRepository.ReplayAll();

      _location1.Client = _client1;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void UnidirectionalRelationWithNewNull ()
    {
      using (_mockRepository.Ordered())
      {
        _extension.RelationChanging (TestableClientTransaction, _location1, GetEndPointDefinition (typeof (Location), "Client"), _client1, null);
        _location1EventReceiver.RelationChanging (GetEndPointDefinition (typeof (Location), "Client"), _client1, null);

        _location1EventReceiver.RelationChanged (GetEndPointDefinition (typeof (Location), "Client"), _client1, null);
        _extension.RelationChanged (TestableClientTransaction, _location1, GetEndPointDefinition (typeof (Location), "Client"), _client1, null);
      }

      _mockRepository.ReplayAll();

      _location1.Client = null;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void UnidirectionalRelationWithOldNull ()
    {
      Location newLocation = Location.NewObject();

      var newLocationEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (newLocation);

      _mockRepository.BackToRecord (_extension);
      using (_mockRepository.Ordered())
      {
        _extension.RelationChanging (TestableClientTransaction, newLocation, GetEndPointDefinition (typeof (Location), "Client"), null, _client1);
        newLocationEventReceiver.RelationChanging (GetEndPointDefinition (typeof (Location), "Client"), null, _client1);

        newLocationEventReceiver.RelationChanged (GetEndPointDefinition (typeof (Location), "Client"), null, _client1);
        _extension.RelationChanged (TestableClientTransaction, newLocation, GetEndPointDefinition (typeof (Location), "Client"), null, _client1);
      }

      _mockRepository.ReplayAll();

      newLocation.Client = _client1;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void UnidirectionalRelationWithOldRelatedObject ()
    {
      Client newClient = Client.NewObject();
      _mockRepository.StrictMock<DomainObjectMockEventReceiver> (newClient); // no events for newClient

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.RelationChanging (
            TestableClientTransaction, _location1, GetEndPointDefinition (typeof (Location), "Client"), _client1, newClient);
        _location1EventReceiver.RelationChanging (GetEndPointDefinition (typeof (Location), "Client"), _client1, newClient);

        _location1EventReceiver.RelationChanged (GetEndPointDefinition (typeof (Location), "Client"), _client1, newClient);
        _extension.RelationChanged (TestableClientTransaction, _location1, GetEndPointDefinition (typeof (Location), "Client"), _client1, newClient);
      }

      _mockRepository.ReplayAll();

      _location1.Client = newClient;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void RemoveFromOneToManyRelation ()
    {
      DomainObjectCollection preloadedOrderItems = _order1.OrderItems;

      Assert.Greater (preloadedOrderItems.Count, 0);
      var orderItem = (OrderItem) preloadedOrderItems[0];

      _mockRepository.BackToRecord (_extension);
      var orderItemEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (orderItem);

      using (_mockRepository.Ordered())
      {
        _extension.RelationReading (
            TestableClientTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderItems"), ValueAccess.Current);
        _extension.RelationRead (null, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Current);
        LastCall.Constraints (
            Is.Same (TestableClientTransaction),
            Is.Same (_order1),
            Is.Equal (GetEndPointDefinition (typeof (Order), "OrderItems")),
            Property.Value ("Count", preloadedOrderItems.Count) & new ContainsConstraint (preloadedOrderItems),
            Is.Equal (ValueAccess.Current));

        _extension.RelationChanging (
            TestableClientTransaction, orderItem, GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);
        orderItemEventReceiver.RelationChanging (GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);

        _extension.RelationChanging (
            TestableClientTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderItems"), orderItem, null);
        _order1EventReceiver.RelationChanging (GetEndPointDefinition (typeof (Order), "OrderItems"), orderItem, null);

        _order1EventReceiver.RelationChanged (GetEndPointDefinition (typeof (Order), "OrderItems"), orderItem, null);
        _extension.RelationChanged (TestableClientTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderItems"), orderItem, null);

        orderItemEventReceiver.RelationChanged (GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);
        _extension.RelationChanged (TestableClientTransaction, orderItem, GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);
      }

      _mockRepository.ReplayAll();

      _order1.OrderItems.Remove (orderItem);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void AddToOneToManyRelation ()
    {
      DomainObjectCollection preloadedOrderItems = _order1.OrderItems;
      preloadedOrderItems.EnsureDataComplete();
      OrderItem orderItem = OrderItem.NewObject();

      _mockRepository.BackToRecord (_extension);
      var orderItemEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (orderItem);

      using (_mockRepository.Ordered())
      {
        _extension.RelationReading (
            TestableClientTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderItems"), ValueAccess.Current);
        _extension.RelationRead (null, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Current);
        LastCall.Constraints (
            Is.Same (TestableClientTransaction),
            Is.Same (_order1),
            Is.Equal (GetEndPointDefinition (typeof (Order), "OrderItems")),
            Property.Value ("Count", preloadedOrderItems.Count) & new ContainsConstraint (preloadedOrderItems),
            Is.Equal (ValueAccess.Current));

        _extension.RelationChanging (TestableClientTransaction, orderItem, GetEndPointDefinition (typeof (OrderItem), "Order"), null, _order1);
        orderItemEventReceiver.RelationChanging (GetEndPointDefinition (typeof (OrderItem), "Order"), null, _order1);

        _extension.RelationChanging (TestableClientTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderItems"), null, orderItem);
        _order1EventReceiver.RelationChanging (GetEndPointDefinition (typeof (Order), "OrderItems"), null, orderItem);

        _order1EventReceiver.RelationChanged (GetEndPointDefinition (typeof (Order), "OrderItems"), null, orderItem);
        _extension.RelationChanged (TestableClientTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderItems"), null, orderItem);

        orderItemEventReceiver.RelationChanged (GetEndPointDefinition (typeof (OrderItem), "Order"), null, _order1);
        _extension.RelationChanged (TestableClientTransaction, orderItem, GetEndPointDefinition (typeof (OrderItem), "Order"), null, _order1);
      }

      _mockRepository.ReplayAll();

      _order1.OrderItems.Add (orderItem);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void AddToOneToManyRelationWithOldRelatedObject ()
    {
      DomainObjectCollection preloadedOrderItemsOfOrder1 = _order1.OrderItems;
      preloadedOrderItemsOfOrder1.EnsureDataComplete();

      OrderItem newOrderItem = DomainObjectIDs.OrderItem3.GetObject<OrderItem>();
      Order oldOrderOfNewOrderItem = newOrderItem.Order;
      oldOrderOfNewOrderItem.EnsureDataAvailable();

      _mockRepository.BackToRecord (_extension);
      var newOrderItemEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (newOrderItem);
      var oldOrderOfNewOrderItemEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (oldOrderOfNewOrderItem);

      using (_mockRepository.Ordered())
      {
        _extension.RelationReading (
            TestableClientTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderItems"), ValueAccess.Current);
        _extension.RelationRead (null, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Current);
        LastCall.Constraints (
            Is.Same (TestableClientTransaction),
            Is.Same (_order1),
            Is.Equal (GetEndPointDefinition (typeof (Order), "OrderItems")),
            Property.Value ("Count", preloadedOrderItemsOfOrder1.Count) & new ContainsConstraint (preloadedOrderItemsOfOrder1),
            Is.Equal (ValueAccess.Current));

        _extension.RelationChanging (
            TestableClientTransaction, newOrderItem, GetEndPointDefinition (typeof (OrderItem), "Order"), oldOrderOfNewOrderItem, _order1);
        newOrderItemEventReceiver.RelationChanging (GetEndPointDefinition (typeof (OrderItem), "Order"), oldOrderOfNewOrderItem, _order1);

        _extension.RelationChanging (
            TestableClientTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderItems"), null, newOrderItem);
        _order1EventReceiver.RelationChanging (GetEndPointDefinition (typeof (Order), "OrderItems"), null, newOrderItem);

        _extension.RelationChanging (
            TestableClientTransaction, oldOrderOfNewOrderItem, GetEndPointDefinition (typeof (Order), "OrderItems"), newOrderItem, null);
        oldOrderOfNewOrderItemEventReceiver.RelationChanging (GetEndPointDefinition (typeof (Order), "OrderItems"), newOrderItem, null);

       
        oldOrderOfNewOrderItemEventReceiver.RelationChanged (GetEndPointDefinition (typeof (Order), "OrderItems"), newOrderItem, null);
        _extension.RelationChanged (
            TestableClientTransaction, oldOrderOfNewOrderItem, GetEndPointDefinition (typeof (Order), "OrderItems"), newOrderItem, null);

        _order1EventReceiver.RelationChanged (GetEndPointDefinition (typeof (Order), "OrderItems"), null, newOrderItem);
        _extension.RelationChanged (TestableClientTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderItems"), null, newOrderItem);

        newOrderItemEventReceiver.RelationChanged (GetEndPointDefinition (typeof (OrderItem), "Order"), oldOrderOfNewOrderItem, _order1);
        _extension.RelationChanged (TestableClientTransaction, newOrderItem, GetEndPointDefinition (typeof (OrderItem), "Order"), oldOrderOfNewOrderItem, _order1);
      }

      _mockRepository.ReplayAll();

      _order1.OrderItems.Add (newOrderItem);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ReplaceInOneToManyRelationWithSameObject ()
    {
      DomainObjectCollection orderItems = _order1.OrderItems;
      OrderItem oldOrderItem = _order1.OrderItems[0];

      _mockRepository.BackToRecord (_extension);

      // no calls on the extension are expected

      _mockRepository.ReplayAll();

      orderItems[0] = oldOrderItem;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ReplaceInOneToManyRelation ()
    {
      Assert.Greater (_order1.OrderItems.Count, 0);
      OrderItem oldOrderItem = _order1.OrderItems[0];

      DomainObjectCollection preloadedOrderItems = _order1.OrderItems;
      OrderItem newOrderItem = OrderItem.NewObject();

      _mockRepository.BackToRecord (_extension);
      var oldOrderItemEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (oldOrderItem);
      var newOrderItemEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (newOrderItem);

      using (_mockRepository.Ordered())
      {
        _extension.RelationReading (
            TestableClientTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderItems"), ValueAccess.Current);
        _extension.RelationRead (null, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Current);
        LastCall.Constraints (
            Is.Same (TestableClientTransaction),
            Is.Same (_order1),
            Is.Equal (GetEndPointDefinition (typeof (Order), "OrderItems")),
            Property.Value ("Count", preloadedOrderItems.Count) & new ContainsConstraint (preloadedOrderItems),
            Is.Equal (ValueAccess.Current));

        _extension.RelationChanging (
            TestableClientTransaction, oldOrderItem,GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);
        oldOrderItemEventReceiver.RelationChanging (GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);

        _extension.RelationChanging (
            TestableClientTransaction, newOrderItem, GetEndPointDefinition (typeof (OrderItem), "Order"), null, _order1);
        newOrderItemEventReceiver.RelationChanging (GetEndPointDefinition (typeof (OrderItem), "Order"), null, _order1);

        _extension.RelationChanging (
            TestableClientTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderItems"), oldOrderItem, newOrderItem);
        _order1EventReceiver.RelationChanging (GetEndPointDefinition (typeof (Order), "OrderItems"), oldOrderItem, newOrderItem);
        
        _order1EventReceiver.RelationChanged (GetEndPointDefinition (typeof (Order), "OrderItems"), oldOrderItem, newOrderItem);
        _extension.RelationChanged (TestableClientTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderItems"), oldOrderItem, newOrderItem);

        newOrderItemEventReceiver.RelationChanged (GetEndPointDefinition (typeof (OrderItem), "Order"), null, _order1);
        _extension.RelationChanged (TestableClientTransaction, newOrderItem, GetEndPointDefinition (typeof (OrderItem), "Order"), null, _order1);

        oldOrderItemEventReceiver.RelationChanged (GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);
        _extension.RelationChanged (TestableClientTransaction, oldOrderItem, GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);
      }

      _mockRepository.ReplayAll();

      _order1.OrderItems[0] = newOrderItem;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ReplaceInOneToManyRelationWithOldRelatedObject ()
    {
      Assert.Greater (_order1.OrderItems.Count, 0);
      OrderItem oldOrderItem = _order1.OrderItems[0];

      DomainObjectCollection preloadedOrderItemsOfOrder1 = _order1.OrderItems;
      OrderItem newOrderItem = DomainObjectIDs.OrderItem3.GetObject<OrderItem>();
      Order oldOrderOfNewOrderItem = newOrderItem.Order;
      Dev.Null = oldOrderOfNewOrderItem.OrderItems; // preload

      _mockRepository.BackToRecord (_extension);
      var oldOrderItemEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (oldOrderItem);
      var newOrderItemEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (newOrderItem);
      var oldOrderOfNewOrderItemEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (oldOrderOfNewOrderItem);

      using (_mockRepository.Ordered())
      {
        _extension.RelationReading (
            TestableClientTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderItems"), ValueAccess.Current);
        _extension.RelationRead (null, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Current);
        LastCall.Constraints (
            Is.Same (TestableClientTransaction),
            Is.Same (_order1),
            Is.Equal (GetEndPointDefinition (typeof (Order), "OrderItems")),
            Property.Value ("Count", preloadedOrderItemsOfOrder1.Count) & new ContainsConstraint (preloadedOrderItemsOfOrder1),
            Is.Equal (ValueAccess.Current));

        _extension.RelationChanging (
            TestableClientTransaction, oldOrderItem, GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);
        oldOrderItemEventReceiver.RelationChanging (GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);

        _extension.RelationChanging (
            TestableClientTransaction, newOrderItem, GetEndPointDefinition (typeof (OrderItem), "Order"), oldOrderOfNewOrderItem, _order1);
        newOrderItemEventReceiver.RelationChanging (GetEndPointDefinition (typeof (OrderItem), "Order"), oldOrderOfNewOrderItem, _order1);

        _extension.RelationChanging (
            TestableClientTransaction, _order1,GetEndPointDefinition (typeof (Order), "OrderItems"), oldOrderItem, newOrderItem);
        _order1EventReceiver.RelationChanging (GetEndPointDefinition (typeof (Order), "OrderItems"), oldOrderItem, newOrderItem);

        _extension.RelationChanging (
            TestableClientTransaction, oldOrderOfNewOrderItem, GetEndPointDefinition (typeof (Order), "OrderItems"), newOrderItem, null);
        oldOrderOfNewOrderItemEventReceiver.RelationChanging (GetEndPointDefinition (typeof (Order), "OrderItems"), newOrderItem, null);

        oldOrderOfNewOrderItemEventReceiver.RelationChanged (GetEndPointDefinition (typeof (Order), "OrderItems"), newOrderItem, null);
        _extension.RelationChanged (TestableClientTransaction, oldOrderOfNewOrderItem, GetEndPointDefinition (typeof (Order), "OrderItems"), newOrderItem, null);

        _order1EventReceiver.RelationChanged (GetEndPointDefinition (typeof (Order), "OrderItems"), oldOrderItem, newOrderItem);
        _extension.RelationChanged (TestableClientTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderItems"), oldOrderItem, newOrderItem);

        newOrderItemEventReceiver.RelationChanged (GetEndPointDefinition (typeof (OrderItem), "Order"), oldOrderOfNewOrderItem, _order1);
        _extension.RelationChanged (TestableClientTransaction, newOrderItem, GetEndPointDefinition (typeof (OrderItem), "Order"), oldOrderOfNewOrderItem, _order1);

        oldOrderItemEventReceiver.RelationChanged (GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);
        _extension.RelationChanged (TestableClientTransaction, oldOrderItem, GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);
      }

      _mockRepository.ReplayAll();

      _order1.OrderItems[0] = newOrderItem;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ReplaceWholeCollectionInOneToManyRelation ()
    {
      var oldCollection = _order1.OrderItems;
      var removedOrderItem = oldCollection[0];
      var stayingOrderItem = oldCollection[1];
      var addedOrderItem = OrderItem.NewObject ();

      var newCollection = new ObjectList<OrderItem> { stayingOrderItem, addedOrderItem };

      _mockRepository.BackToRecord (_extension);
      var removedOrderItemEventReceiverMock = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (removedOrderItem);
      _mockRepository.StrictMock<DomainObjectMockEventReceiver> (stayingOrderItem);
      var addedOrderItemEventReceiverMock = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (addedOrderItem);

      using (_mockRepository.Ordered ())
      {
        _extension.Expect (mock => mock.RelationChanging (
            TestableClientTransaction, removedOrderItem, GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null));
        removedOrderItemEventReceiverMock.Expect (mock => mock.RelationChanging (GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null));

        _extension.Expect (mock => mock.RelationChanging (
            TestableClientTransaction, addedOrderItem, GetEndPointDefinition (typeof (OrderItem), "Order"), null, _order1));
        addedOrderItemEventReceiverMock.Expect (mock => mock.RelationChanging (GetEndPointDefinition (typeof (OrderItem), "Order"), null, _order1));

        _extension.Expect (mock => mock.RelationChanging (
            TestableClientTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderItems"), removedOrderItem, null));
        _order1EventReceiver.Expect (mock => mock.RelationChanging (GetEndPointDefinition (typeof (Order), "OrderItems"), removedOrderItem, null));

        _extension.Expect (mock => mock.RelationChanging (
            TestableClientTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderItems"), null, addedOrderItem));
        _order1EventReceiver.Expect (mock => mock.RelationChanging (GetEndPointDefinition (typeof (Order), "OrderItems"), null, addedOrderItem));

        _order1EventReceiver.Expect (mock => mock.RelationChanged (GetEndPointDefinition (typeof (Order), "OrderItems"), null, addedOrderItem));
        _extension.Expect (mock => mock.RelationChanged (TestableClientTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderItems"), null, addedOrderItem));

        _order1EventReceiver.Expect (mock => mock.RelationChanged (GetEndPointDefinition (typeof (Order), "OrderItems"), removedOrderItem, null));
        _extension.Expect (mock => mock.RelationChanged (TestableClientTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderItems"), removedOrderItem, null));

        addedOrderItemEventReceiverMock.Expect (mock => mock.RelationChanged (GetEndPointDefinition (typeof (OrderItem), "Order"), null, _order1));
        _extension.Expect (mock => mock.RelationChanged (TestableClientTransaction, addedOrderItem, GetEndPointDefinition (typeof (OrderItem), "Order"), null, _order1));

        removedOrderItemEventReceiverMock.Expect (mock => mock.RelationChanged (GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null));
        _extension.Expect (mock => mock.RelationChanged (TestableClientTransaction, removedOrderItem, GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null));

      }

      _mockRepository.ReplayAll ();

      _order1.OrderItems = newCollection;

      _mockRepository.VerifyAll ();
    }
  }
}
