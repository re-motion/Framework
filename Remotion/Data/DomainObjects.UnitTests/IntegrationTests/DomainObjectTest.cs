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
using System.Collections.ObjectModel;
using System.Linq;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.Validation;
using Remotion.FunctionalProgramming;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests
{
  [TestFixture]
  public class DomainObjectTest : ClientTransactionBaseTest
  {
    private PropertyDefinition _orderDeliveryDateProperty;

    public override void OneTimeSetUp ()
    {
      base.OneTimeSetUp();
      SetDatabaseModifyable();
    }

    public override void SetUp ()
    {
      base.SetUp();

      _orderDeliveryDateProperty = GetPropertyDefinition(typeof(Order), "DeliveryDate");
    }

    [Test]
    public void RelationEventTestWithMockObject ()
    {
      Customer newCustomer1 = Customer.NewObject();
      newCustomer1.Name = "NewCustomer1";

      Customer newCustomer2 = Customer.NewObject();
      newCustomer2.Name = "NewCustomer2";

      Official official2 = DomainObjectIDs.Official2.GetObject<Official>();
      Ceo newCeo1 = Ceo.NewObject();
      Ceo newCeo2 = Ceo.NewObject();
      Order newOrder1 = Order.NewObject();
      newOrder1.DeliveryDate = new DateTime(2006, 1, 1);

      Order newOrder2 = Order.NewObject();
      newOrder2.DeliveryDate = new DateTime(2006, 2, 2);

      OrderItem newOrderItem1 = OrderItem.NewObject();
      OrderItem newOrderItem2 = OrderItem.NewObject();

      DomainObjectCollection newCustomer1Orders = newCustomer1.Orders;
      Assert.That(newCustomer1Orders.IsDataComplete, Is.True);
      DomainObjectCollection newCustomer2Orders = newCustomer2.Orders;
      Assert.That(newCustomer2Orders.IsDataComplete, Is.True);
      DomainObjectCollection official2Orders = official2.Orders;
      official2Orders.EnsureDataComplete();
      DomainObjectCollection newOrder1OrderItems = newOrder1.OrderItems;
      Assert.That(newOrder1OrderItems.IsDataComplete, Is.True);
      DomainObjectCollection newOrder2OrderItems = newOrder2.OrderItems;
      Assert.That(newOrder2OrderItems.IsDataComplete, Is.True);

      var newCustomer1EventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, newCustomer1);
      var newCustomer2EventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, newCustomer2);
      var official2EventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, official2);
      var newCeo1EventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, newCeo1);
      var newCeo2EventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, newCeo2);
      var newOrder1EventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, newOrder1);
      var newOrder2EventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, newOrder2);
      var newOrderItem1EventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, newOrderItem1);
      var newOrderItem2EventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, newOrderItem2);

      var newCustomer1OrdersEventReceiver =
          new Mock<DomainObjectCollectionMockEventReceiver> (MockBehavior.Strict, newCustomer1.Orders);
      var newCustomer2OrdersEventReceiver =
          new Mock<DomainObjectCollectionMockEventReceiver> (MockBehavior.Strict, newCustomer2.Orders);
      var official2OrdersEventReceiver =
          new Mock<DomainObjectCollectionMockEventReceiver> (MockBehavior.Strict, official2.Orders);
      var newOrder1OrderItemsEventReceiver =
          new Mock<DomainObjectCollectionMockEventReceiver> (MockBehavior.Strict, newOrder1.OrderItems);
      var newOrder2OrderItemsEventReceiver =
          new Mock<DomainObjectCollectionMockEventReceiver> (MockBehavior.Strict, newOrder2.OrderItems);

      var extension = new Mock<IClientTransactionExtension> (MockBehavior.Strict);

      var sequence1 = new MockSequence();

      extension.Object.RelationChanging(
            TestableClientTransaction, newCeo1, GetEndPointDefinition(typeof(Ceo), "Company"), null, newCustomer1);

      newCeo1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Ceo), "Company"), null, newCustomer1);

      extension.Object.RelationChanging(
            TestableClientTransaction,
            newCustomer1,
            GetEndPointDefinition(typeof(Company), "Ceo"),
            null,
            newCeo1);

      newCustomer1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Company), "Ceo"), null, newCeo1);

      newCustomer1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Company), "Ceo"), null, newCeo1);

      extension.Object.RelationChanged(TestableClientTransaction, newCustomer1, GetEndPointDefinition(typeof(Company), "Ceo"), null, newCeo1);

      newCeo1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Ceo), "Company"), null, newCustomer1);

      extension.Object.RelationChanged(TestableClientTransaction, newCeo1, GetEndPointDefinition(typeof(Ceo), "Company"), null, newCustomer1);

      extension.Object.RelationChanging(
            TestableClientTransaction, newCeo2, GetEndPointDefinition(typeof(Ceo), "Company"), null, newCustomer1);

      newCeo2EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Ceo), "Company"), null, newCustomer1);

      extension.Object.RelationChanging(
            TestableClientTransaction,
            newCustomer1,
            GetEndPointDefinition(typeof(Company), "Ceo"),
            newCeo1,
            newCeo2);

      newCustomer1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Company), "Ceo"), newCeo1, newCeo2);

      extension.Object.RelationChanging(
            TestableClientTransaction, newCeo1, GetEndPointDefinition(typeof(Ceo), "Company"), newCustomer1, null);

      newCeo1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Ceo), "Company"), newCustomer1, null);

      newCeo1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Ceo), "Company"), newCustomer1, null);

      extension.Object.RelationChanged(
            TestableClientTransaction,
            newCeo1,
            GetEndPointDefinition(typeof(Ceo), "Company"),
            newCustomer1,
            null);

      newCustomer1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Company), "Ceo"), newCeo1, newCeo2);

      extension.Object.RelationChanged(
            TestableClientTransaction,
            newCustomer1,
            GetEndPointDefinition(typeof(Company), "Ceo"),
            newCeo1,
            newCeo2);

      newCeo2EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Ceo), "Company"), null, newCustomer1);

      extension.Object.RelationChanged(
            TestableClientTransaction,
            newCeo2,
            GetEndPointDefinition(typeof(Ceo), "Company"),
             null,
            newCustomer1);

      extension.Object.RelationChanging(
            TestableClientTransaction, newCeo1, GetEndPointDefinition(typeof(Ceo), "Company"), null, newCustomer2);

      newCeo1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Ceo), "Company"), null, newCustomer2);

      extension.Object.RelationChanging(
            TestableClientTransaction,
            newCustomer2,
            GetEndPointDefinition(typeof(Company), "Ceo"),
            null,
            newCeo1);

      newCustomer2EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Company), "Ceo"), null, newCeo1);

      newCustomer2EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Company), "Ceo"), null, newCeo1);

      extension.Object.RelationChanged(
            TestableClientTransaction,
            newCustomer2,
            GetEndPointDefinition(typeof(Company), "Ceo"),
            null,
            newCeo1);

      newCeo1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Ceo), "Company"), null, newCustomer2);

      extension.Object.RelationChanged(
            TestableClientTransaction,
            newCeo1,
            GetEndPointDefinition(typeof(Ceo), "Company"),
            null,
            newCustomer2);

      extension.Object.RelationChanging(
            TestableClientTransaction, newCeo1, GetEndPointDefinition(typeof(Ceo), "Company"), newCustomer2, null);

      newCeo1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Ceo), "Company"), newCustomer2, null);

      extension.Object.RelationChanging(
            TestableClientTransaction,
            newCustomer2,
            GetEndPointDefinition(typeof(Company), "Ceo"),
            newCeo1,
            null);

      newCustomer2EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Company), "Ceo"), newCeo1, null);

      newCustomer2EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Company), "Ceo"), newCeo1, null);

      extension.Object.RelationChanged(
            TestableClientTransaction,
            newCustomer2,
            GetEndPointDefinition(typeof(Company), "Ceo"),
            newCeo1,
            null);

      newCeo1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Ceo), "Company"), newCustomer2, null);

      extension.Object.RelationChanged(
            TestableClientTransaction,
            newCeo1,
            GetEndPointDefinition(typeof(Ceo), "Company"),
            newCustomer2,
            null);

      extension.Object.RelationReading(
            TestableClientTransaction,
            newCustomer1,
            GetEndPointDefinition(typeof(Customer), "Orders"),
            ValueAccess.Current);
      extension.InSequence (sequence1).Setup (
            mock => mock.RelationRead (
                ClientTransactionScope.CurrentTransaction,
                newCustomer1,
                GetEndPointDefinition (typeof(Customer), "Orders"),
                It.Is<IReadOnlyCollectionData<DomainObject>> (data => data.Count == 0),
                ValueAccess.Current)).Verifiable();

      extension.Object.RelationChanging(
            TestableClientTransaction,
            newOrder1,
            GetEndPointDefinition(typeof(Order), "Customer"),
            null,
            newCustomer1);

      newOrder1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Order), "Customer"), null, newCustomer1);

      newCustomer1OrdersEventReceiver.Object.Adding(newCustomer1Orders, newOrder1);

      extension.Object.RelationChanging(
            TestableClientTransaction,
            newCustomer1,
            GetEndPointDefinition(typeof(Customer), "Orders"),
            null,
            newOrder1);

      newCustomer1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Customer), "Orders"), null, newOrder1);

      newCustomer1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Customer), "Orders"), null, newOrder1);

      extension.Object.RelationChanged(
            TestableClientTransaction,
            newCustomer1,
            GetEndPointDefinition(typeof(Customer), "Orders"),
            null,
            newOrder1);

      newCustomer1OrdersEventReceiver.Object.Added(newCustomer1Orders, newOrder1);

      newOrder1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Order), "Customer"), null, newCustomer1);

      extension.Object.RelationChanged(
            TestableClientTransaction,
            newOrder1,
            GetEndPointDefinition(typeof(Order), "Customer"),
            null,
            newCustomer1);

      extension.Object.RelationReading(
            TestableClientTransaction,
            newCustomer1,
            GetEndPointDefinition(typeof(Customer), "Orders"),
            ValueAccess.Current);
      extension.InSequence (sequence1).Setup (
            mock => mock.RelationRead (
                ClientTransactionScope.CurrentTransaction,
                newCustomer1,
                GetEndPointDefinition (typeof(Customer), "Orders"),
                It.Is<IReadOnlyCollectionData<DomainObject>> (data => data.Count == 1 && data.GetObject (newOrder1.ID) == newOrder1),
                ValueAccess.Current)).Verifiable();

      extension.Object.RelationChanging(
            TestableClientTransaction,
            newOrder2,
            GetEndPointDefinition(typeof(Order), "Customer"),
            null,
            newCustomer1);

      newOrder2EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Order), "Customer"), null, newCustomer1);

      newCustomer1OrdersEventReceiver.Object.Adding(newCustomer1Orders, newOrder2);

      extension.Object.RelationChanging(
            TestableClientTransaction,
            newCustomer1,
            GetEndPointDefinition(typeof(Customer), "Orders"),
            null,
            newOrder2);

      newCustomer1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Customer), "Orders"), null, newOrder2);

      newCustomer1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Customer), "Orders"), null, newOrder2);

      extension.Object.RelationChanged(
            TestableClientTransaction,
            newCustomer1,
            GetEndPointDefinition(typeof(Customer), "Orders"),
            null,
            newOrder2);

      newCustomer1OrdersEventReceiver.Object.Added(newCustomer1Orders, newOrder2);

      newOrder2EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Order), "Customer"), null, newCustomer1);

      extension.Object.RelationChanged(
            TestableClientTransaction,
            newOrder2,
            GetEndPointDefinition(typeof(Order), "Customer"),
            null,
            newCustomer1);

      extension.Object.RelationReading(
            TestableClientTransaction,
            newCustomer1,
            GetEndPointDefinition(typeof(Customer), "Orders"),
            ValueAccess.Current);
      extension.InSequence (sequence1).Setup (
            mock => mock.RelationRead (
                ClientTransactionScope.CurrentTransaction,
                newCustomer1,
                GetEndPointDefinition (typeof(Customer), "Orders"),
                It.Is<IReadOnlyCollectionData<DomainObject>> (                    data => data.Count == 2 && data.GetObject (newOrder1.ID) == newOrder1 && data.GetObject (newOrder2.ID) == newOrder2),
                ValueAccess.Current)).Verifiable();

      extension.Object.RelationChanging(
            TestableClientTransaction,
            newOrder2,
            GetEndPointDefinition(typeof(Order), "Customer"),
            newCustomer1,
            null);

      newOrder2EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Order), "Customer"), newCustomer1, null);

      newCustomer1OrdersEventReceiver.Object.Removing(newCustomer1Orders, newOrder2);

      extension.Object.RelationChanging(
            TestableClientTransaction,
            newCustomer1,
            GetEndPointDefinition(typeof(Customer), "Orders"),
            newOrder2,
            null);

      newCustomer1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Customer), "Orders"), newOrder2, null);

      newCustomer1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Customer), "Orders"), newOrder2, null);

      extension.Object.RelationChanged(
            TestableClientTransaction,
            newCustomer1,
            GetEndPointDefinition(typeof(Customer), "Orders"),
            newOrder2,
            null);

      newCustomer1OrdersEventReceiver.Object.Removed(newCustomer1Orders, newOrder2);

      newOrder2EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Order), "Customer"), newCustomer1, null);

      extension.Object.RelationChanged(
            TestableClientTransaction,
            newOrder2,
            GetEndPointDefinition(typeof(Order), "Customer"),
            newCustomer1,
            null);

      extension.Object.RelationChanging(
            TestableClientTransaction,
            newOrderItem1,
            GetEndPointDefinition(typeof(OrderItem), "Order"),
            null,
            newOrder1);

      newOrderItem1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderItem), "Order"), null, newOrder1);

      newOrder1OrderItemsEventReceiver.Object.Adding(newOrder1OrderItems, newOrderItem1);

      extension.Object.RelationChanging(
            TestableClientTransaction,
            newOrder1,
            GetEndPointDefinition(typeof(Order), "OrderItems"),
            null,
            newOrderItem1);

      newOrder1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Order), "OrderItems"), null, newOrderItem1);

      newOrder1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Order), "OrderItems"), null, newOrderItem1);

      extension.Object.RelationChanged(
            TestableClientTransaction,
            newOrder1,
            GetEndPointDefinition(typeof(Order), "OrderItems"),
            null,
            newOrderItem1);

      newOrder1OrderItemsEventReceiver.Object.Added(newOrder1OrderItems, newOrderItem1);

      newOrderItem1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderItem), "Order"), null, newOrder1);

      extension.Object.RelationChanged(
            TestableClientTransaction,
            newOrderItem1,
            GetEndPointDefinition(typeof(OrderItem), "Order"),
            null,
            newOrder1);

      extension.Object.RelationChanging(
            TestableClientTransaction,
            newOrderItem2,
            GetEndPointDefinition(typeof(OrderItem), "Order"),
            null,
            newOrder1);

      newOrderItem2EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderItem), "Order"), null, newOrder1);

      newOrder1OrderItemsEventReceiver.Object.Adding(newOrder1OrderItems, newOrderItem2);

      extension.Object.RelationChanging(
            TestableClientTransaction,
            newOrder1,
            GetEndPointDefinition(typeof(Order), "OrderItems"),
            null,
            newOrderItem2);

      newOrder1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Order), "OrderItems"), null, newOrderItem2);

      newOrder1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Order), "OrderItems"), null, newOrderItem2);

      extension.Object.RelationChanged(
            TestableClientTransaction,
            newOrder1,
            GetEndPointDefinition(typeof(Order), "OrderItems"),
            null,
            newOrderItem2);

      newOrder1OrderItemsEventReceiver.Object.Added(newOrder1OrderItems, newOrderItem2);

      newOrderItem2EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderItem), "Order"), null, newOrder1);

      extension.Object.RelationChanged(
            TestableClientTransaction,
            newOrderItem2,
            GetEndPointDefinition(typeof(OrderItem), "Order"),
            null,
            newOrder1);

      extension.Object.RelationChanging(
            TestableClientTransaction,
            newOrderItem1,
            GetEndPointDefinition(typeof(OrderItem), "Order"),
            newOrder1,
            null);

      newOrderItem1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderItem), "Order"), newOrder1, null);

      newOrder1OrderItemsEventReceiver.Object.Removing(newOrder1OrderItems, newOrderItem1);

      extension.Object.RelationChanging(
            TestableClientTransaction,
            newOrder1,
            GetEndPointDefinition(typeof(Order), "OrderItems"),
            newOrderItem1,
            null);

      newOrder1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Order), "OrderItems"), newOrderItem1, null);

      newOrder1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Order), "OrderItems"), newOrderItem1, null);

      extension.Object.RelationChanged(
            TestableClientTransaction,
            newOrder1,
            GetEndPointDefinition(typeof(Order), "OrderItems"),
            newOrderItem1,
            null);

      newOrder1OrderItemsEventReceiver.Object.Removed(newOrder1OrderItems, newOrderItem1);

      newOrderItem1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderItem), "Order"), newOrder1, null);

      extension.Object.RelationChanged(
            TestableClientTransaction,
            newOrderItem1,
            GetEndPointDefinition(typeof(OrderItem), "Order"),
            newOrder1,
            null);

      extension.Object.RelationChanging(
            TestableClientTransaction,
            newOrderItem1,
            GetEndPointDefinition(typeof(OrderItem), "Order"),
            null,
            newOrder2);

      newOrderItem1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderItem), "Order"), null, newOrder2);

      newOrder2OrderItemsEventReceiver.Object.Adding(newOrder2OrderItems, newOrderItem1);

      extension.Object.RelationChanging(
            TestableClientTransaction,
            newOrder2,
            GetEndPointDefinition(typeof(Order), "OrderItems"),
            null,
            newOrderItem1);

      newOrder2EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Order), "OrderItems"), null, newOrderItem1);

      newOrder2EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Order), "OrderItems"), null, newOrderItem1);

      extension.Object.RelationChanged(
            TestableClientTransaction,
            newOrder2,
            GetEndPointDefinition(typeof(Order), "OrderItems"),
            null,
            newOrderItem1);

      newOrder2OrderItemsEventReceiver.Object.Added(newOrder2OrderItems, newOrderItem1);

      newOrderItem1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderItem), "Order"), null, newOrder2);

      extension.Object.RelationChanged(
            TestableClientTransaction,
            newOrderItem1,
            GetEndPointDefinition(typeof(OrderItem), "Order"),
            null,
            newOrder2);

      extension.Object.RelationChanging(
            TestableClientTransaction,
            newOrder1,
            GetEndPointDefinition(typeof(Order), "Official"),
            null,
            official2);

      newOrder1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Order), "Official"), null, official2);

      official2OrdersEventReceiver.Object.Adding(official2Orders, newOrder1);

      extension.Object.RelationChanging(
            TestableClientTransaction,
            official2,
            GetEndPointDefinition(typeof(Official), "Orders"),
            null,
            newOrder1);

      official2EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Official), "Orders"), null, newOrder1);

      official2EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Official), "Orders"), null, newOrder1);

      extension.Object.RelationChanged(
            TestableClientTransaction,
            official2,
            GetEndPointDefinition(typeof(Official), "Orders"),
            null,
            newOrder1);

      official2OrdersEventReceiver.Object.Added(official2Orders, newOrder1);

      newOrder1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Order), "Official"), null, official2);

      extension.Object.RelationChanged(
            TestableClientTransaction,
            newOrder1,
            GetEndPointDefinition(typeof(Order), "Official"),
            null,
            official2);

      extension.Object.NewObjectCreating(TestableClientTransaction, typeof(OrderTicket));

      extension.Object.RelationChanging(
            ClientTransactionScope.CurrentTransaction,
            Arg<OrderTicket>.Is.TypeOf,
            GetEndPointDefinition(typeof(OrderTicket), "Order"),
            (DomainObject)null,
            newOrder1);

      extension.Object.RelationChanging(
            ClientTransactionScope.CurrentTransaction,
            newOrder1,
            GetEndPointDefinition(typeof(Order), "OrderTicket"),
            (DomainObject)null,
            Arg<OrderTicket>.Is.TypeOf);

      newOrder1EventReceiver.Object.RelationChanging(
            newOrder1,
            It.Is<RelationChangingEventArgs> (                args =>
                args.RelationEndPointDefinition == GetEndPointDefinition(typeof(Order), "OrderTicket")
                && args.OldRelatedObject == null
                && args.NewRelatedObject is OrderTicket));

      newOrder1EventReceiver.Object.RelationChanged(newOrder1,
            It.Is<RelationChangedEventArgs> (                args =>
                args.RelationEndPointDefinition == GetEndPointDefinition(typeof(Order), "OrderTicket")
                && args.OldRelatedObject == null
                && args.NewRelatedObject is OrderTicket));

      extension.Object.RelationChanged(
            ClientTransactionScope.CurrentTransaction,
            newOrder1,
            GetEndPointDefinition(typeof(Order), "OrderTicket"),
            (DomainObject)null,
            Arg<OrderTicket>.Is.TypeOf);

      extension.Object.RelationChanged(
            ClientTransactionScope.CurrentTransaction,
            Arg<OrderTicket>.Is.TypeOf,
            GetEndPointDefinition(typeof(OrderTicket), "Order"),
            (DomainObject)null,
            newOrder1);

      extension.Setup (stub => stub.Key).Returns ("Extension");

      ClientTransactionScope.CurrentTransaction.Extensions.Add(extension.Object);
      try
      {
        //1
        newCeo1.Company = newCustomer1;
        //2
        newCeo2.Company = newCustomer1;
        //3
        newCeo1.Company = newCustomer2;
        //4
        newCeo1.Company = null;
        //5
        newCustomer1.Orders.Add(newOrder1);
        //6
        newCustomer1.Orders.Add(newOrder2);
        //7
        newCustomer1.Orders.Remove(newOrder2);
        //8
        newOrderItem1.Order = newOrder1;
        //9
        newOrderItem2.Order = newOrder1;
        //10
        newOrderItem1.Order = null;
        //11
        newOrderItem1.Order = newOrder2;
        //12
        newOrder1.Official = official2;
        //13
        OrderTicket newOrderTicket1 = OrderTicket.NewObject(newOrder1);

        newCustomer1EventReceiver.Verify();
        newCustomer2EventReceiver.Verify();
        official2EventReceiver.Verify();
        newCeo1EventReceiver.Verify();
        newCeo2EventReceiver.Verify();
        newOrder1EventReceiver.Verify();
        newOrder2EventReceiver.Verify();
        newOrderItem1EventReceiver.Verify();
        newOrderItem2EventReceiver.Verify();
        newCustomer1OrdersEventReceiver.Verify();
        newCustomer2OrdersEventReceiver.Verify();
        official2OrdersEventReceiver.Verify();
        newOrder1OrderItemsEventReceiver.Verify();
        newOrder2OrderItemsEventReceiver.Verify();
        extension.Verify();
        newOrderTicket1EventReceiver.Object.Verify();

        BackToRecord(
            mockRepository,
            extension.Object,
            newCustomer1EventReceiver.Object,
            newCustomer2EventReceiver.Object,
            official2EventReceiver.Object,
            newCeo1EventReceiver.Object,
            newCeo2EventReceiver.Object,
            newOrder1EventReceiver.Object,
            newOrder2EventReceiver.Object,
            newOrderItem1EventReceiver.Object,
            newOrderItem2EventReceiver.Object,
            newCustomer1OrdersEventReceiver.Object,
            newCustomer2OrdersEventReceiver.Object,
            official2OrdersEventReceiver.Object,
            newOrder1OrderItemsEventReceiver.Object,
            newOrder2OrderItemsEventReceiver.Object);

        var newOrderTicket1EventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, newOrderTicket1);

        var sequence2 = new MockSequence();

        extension.Object.RelationChanging(
              TestableClientTransaction,
              newOrderTicket1,
              GetEndPointDefinition(typeof(OrderTicket), "Order"),
              newOrder1,
              newOrder2);

        newOrderTicket1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderTicket), "Order"), newOrder1, newOrder2);

        extension.Object.RelationChanging(
              TestableClientTransaction,
              newOrder1,
              GetEndPointDefinition(typeof(Order), "OrderTicket"),
              newOrderTicket1,
              null);

        newOrder1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Order), "OrderTicket"), newOrderTicket1, null);

        extension.Object.RelationChanging(
              TestableClientTransaction,
              newOrder2,
              GetEndPointDefinition(typeof(Order), "OrderTicket"),
              null,
              newOrderTicket1);

        newOrder2EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Order), "OrderTicket"), null, newOrderTicket1);

        newOrder2EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Order), "OrderTicket"), null, newOrderTicket1);

        extension.Object.RelationChanged(
              TestableClientTransaction,
              newOrder2,
              GetEndPointDefinition(typeof(Order), "OrderTicket"),
              null,
              newOrderTicket1);

        newOrder1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Order), "OrderTicket"), newOrderTicket1, null);

        extension.Object.RelationChanged(
              TestableClientTransaction,
              newOrder1,
              GetEndPointDefinition(typeof(Order), "OrderTicket"),
              newOrderTicket1,
              null);

        newOrderTicket1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderTicket), "Order"), newOrder1, newOrder2);

        extension.Object.RelationChanged(
              TestableClientTransaction,
              newOrderTicket1,
              GetEndPointDefinition(typeof(OrderTicket), "Order"),
              newOrder1,
              newOrder2);

        extension.Object.RelationChanging(
              TestableClientTransaction,
              newOrder2,
              GetEndPointDefinition(typeof(Order), "Customer"),
              null,
              newCustomer1);

        newOrder2EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Order), "Customer"), null, newCustomer1);

        newCustomer1OrdersEventReceiver.Object.Adding(newCustomer1Orders, newOrder2);

        extension.Object.RelationChanging(
              TestableClientTransaction,
              newCustomer1,
              GetEndPointDefinition(typeof(Customer), "Orders"),
              null,
              newOrder2);

        newCustomer1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Customer), "Orders"), null, newOrder2);

        newCustomer1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Customer), "Orders"), null, newOrder2);

        extension.Object.RelationChanged(
              TestableClientTransaction,
              newCustomer1,
              GetEndPointDefinition(typeof(Customer), "Orders"),
              null,
              newOrder2);

        newCustomer1OrdersEventReceiver.Object.Added(newCustomer1Orders, newOrder2);

        newOrder2EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Order), "Customer"), null, newCustomer1);

        extension.Object.RelationChanged(
              TestableClientTransaction,
              newOrder2,
              GetEndPointDefinition(typeof(Order), "Customer"),
              null,
              newCustomer1);

        extension.Object.RelationChanging(
              TestableClientTransaction,
              newOrder2,
              GetEndPointDefinition(typeof(Order), "Customer"),
              newCustomer1,
              newCustomer2);

        newOrder2EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Order), "Customer"), newCustomer1, newCustomer2);

        newCustomer2OrdersEventReceiver.Object.Adding(newCustomer2Orders, newOrder2);

        extension.Object.RelationChanging(
              TestableClientTransaction,
              newCustomer2,
              GetEndPointDefinition(typeof(Customer), "Orders"),
              null,
              newOrder2);

        newCustomer2EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Customer), "Orders"), null, newOrder2);

        newCustomer1OrdersEventReceiver.Object.Removing(newCustomer1Orders, newOrder2);

        extension.Object.RelationChanging(
              TestableClientTransaction,
              newCustomer1,
              GetEndPointDefinition(typeof(Customer), "Orders"),
              newOrder2,
              null);

        newCustomer1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Customer), "Orders"), newOrder2, null);

        newCustomer1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Customer), "Orders"), newOrder2, null);

        extension.Object.RelationChanged(
              TestableClientTransaction, newCustomer1, GetEndPointDefinition(typeof(Customer), "Orders"), newOrder2, null);

        newCustomer1OrdersEventReceiver.Object.Removed(newCustomer1Orders, newOrder2);

        newCustomer2EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Customer), "Orders"), null, newOrder2);

        extension.Object.RelationChanged(
              TestableClientTransaction, newCustomer2, GetEndPointDefinition(typeof(Customer), "Orders"), null, newOrder2);

        newCustomer2OrdersEventReceiver.Object.Added(newCustomer2Orders, newOrder2);

        newOrder2EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Order), "Customer"), newCustomer1, newCustomer2);

        extension.Object.RelationChanged(
              TestableClientTransaction, newOrder2, GetEndPointDefinition(typeof(Order), "Customer"), newCustomer1, newCustomer2);

        extension.Object.ObjectDeleting(TestableClientTransaction, newOrder2);

        newOrder2EventReceiver.Object.Deleting(newOrder2, It.IsNotNull<EventArgs>());

        newOrder2OrderItemsEventReceiver.Object.Deleting();

        using (mockRepository.Unordered())
          {
            newCustomer2OrdersEventReceiver.Object.Removing(newCustomer2Orders, newOrder2);
            extension.Object.RelationChanging(
                TestableClientTransaction,
                newCustomer2,
                GetEndPointDefinition(typeof(Customer), "Orders"),
                newOrder2,
                null);
            newCustomer2EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Customer), "Orders"), newOrder2, null);

            extension.Object.RelationChanging(
                TestableClientTransaction,
                newOrderTicket1,
                GetEndPointDefinition(typeof(OrderTicket), "Order"),
                newOrder2,
                null);
            newOrderTicket1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderTicket), "Order"), newOrder2, null);

            extension.Object.RelationChanging(
                TestableClientTransaction,
                newOrderItem1,
                GetEndPointDefinition(typeof(OrderItem), "Order"),
                newOrder2,
                null);
            newOrderItem1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderItem), "Order"), newOrder2, null);
          }

        using (mockRepository.Unordered())
          {
            newCustomer2EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Customer), "Orders"), newOrder2, null);
            extension.Object.RelationChanged(
                TestableClientTransaction, newCustomer2, GetEndPointDefinition(typeof(Customer), "Orders"), newOrder2, null);
            newCustomer2OrdersEventReceiver.Object.Removed(newCustomer2Orders, newOrder2);

            newOrderTicket1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderTicket), "Order"), newOrder2, null);
            extension.Object.RelationChanged(
                TestableClientTransaction,
                newOrderTicket1,
                GetEndPointDefinition(typeof(OrderTicket), "Order"), newOrder2, null);

            newOrderItem1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderItem), "Order"), newOrder2, null);
            extension.Object.RelationChanged(
                TestableClientTransaction, newOrderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), newOrder2, null);
          }

        newOrder2OrderItemsEventReceiver.Object.Deleted();

        newOrder2EventReceiver.Object.Deleted(newOrder2, It.IsNotNull<EventArgs>());

        extension.Object.ObjectDeleted(TestableClientTransaction, newOrder2);

        extension.Object.RelationChanging(
              TestableClientTransaction,
              newOrderTicket1,
              GetEndPointDefinition(typeof(OrderTicket), "Order"),
              null,
              newOrder1);

        newOrderTicket1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderTicket), "Order"), null, newOrder1);

        extension.Object.RelationChanging(
              TestableClientTransaction,
              newOrder1,
              GetEndPointDefinition(typeof(Order), "OrderTicket"),
              null,
              newOrderTicket1);

        newOrder1EventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Order), "OrderTicket"), null, newOrderTicket1);

        newOrder1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Order), "OrderTicket"), null, newOrderTicket1);

        extension.Object.RelationChanged(
              TestableClientTransaction, newOrder1, GetEndPointDefinition(typeof(Order), "OrderTicket"), null, newOrderTicket1);

        newOrderTicket1EventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderTicket), "Order"), null, newOrder1);

        extension.Object.RelationChanged(
              TestableClientTransaction,
              newOrderTicket1,
              GetEndPointDefinition(typeof(OrderTicket), "Order"), null, newOrder1);

        extension.Object.ObjectDeleting(TestableClientTransaction, newCustomer2);

        newCustomer2EventReceiver.Object.Deleting(newCustomer2, It.IsNotNull<EventArgs>());

        newCustomer2OrdersEventReceiver.Object.Deleting();

        newCustomer2OrdersEventReceiver.Object.Deleted();

        newCustomer2EventReceiver.Object.Deleted(newCustomer2, It.IsNotNull<EventArgs>());

        extension.Object.ObjectDeleted(TestableClientTransaction, newCustomer2);

        extension.Object.ObjectDeleting(TestableClientTransaction, newCeo1);

        newCeo1EventReceiver.Object.Deleting(newCeo1, It.IsNotNull<EventArgs>());

        newCeo1EventReceiver.Object.Deleted(newCeo1, It.IsNotNull<EventArgs>());

        extension.Object.ObjectDeleted(TestableClientTransaction, newCeo1);

        extension.Object.ObjectDeleting(TestableClientTransaction, newOrderItem1);

        newOrderItem1EventReceiver.Object.Deleting(newOrderItem1, It.IsNotNull<EventArgs>());

        newOrderItem1EventReceiver.Object.Deleted(newOrderItem1, It.IsNotNull<EventArgs>());

        extension.Object.ObjectDeleted(TestableClientTransaction, newOrderItem1);

        extension.Object.Committing(
              ClientTransactionScope.CurrentTransaction,
              It.Is<ReadOnlyCollection<DomainObject>> (                  c => c.SetEquals(new DomainObject[] { newCustomer1, official2, newCeo2, newOrder1, newOrderItem2, newOrderTicket1 })),
              It.IsAny<ICommittingEventRegistrar>());

        using (mockRepository.Unordered())
          {
            newCustomer1EventReceiver.Setup (_ => _.Committing (It.Is<object> (_ => object.ReferenceEquals (_, newCustomer1)), It.Is<DomainObjectCommittingEventArgs> (_ => _ != null))).Verifiable();
            official2EventReceiver.Setup (_ => _.Committing (It.Is<object> (_ => object.ReferenceEquals (_, official2)), It.Is<DomainObjectCommittingEventArgs> (_ => _ != null))).Verifiable();
            newCeo2EventReceiver.Setup (_ => _.Committing (It.Is<object> (_ => object.ReferenceEquals (_, newCeo2)), It.Is<DomainObjectCommittingEventArgs> (_ => _ != null))).Verifiable();
            newOrder1EventReceiver.Setup (_ => _.Committing (It.Is<object> (_ => object.ReferenceEquals (_, newOrder1)), It.Is<DomainObjectCommittingEventArgs> (_ => _ != null))).Verifiable();
            newOrderItem2EventReceiver.Setup (_ => _.Committing (It.Is<object> (_ => object.ReferenceEquals (_, newOrderItem2)), It.Is<DomainObjectCommittingEventArgs> (_ => _ != null))).Verifiable();
            newOrderTicket1EventReceiver.Setup (_ => _.Committing (It.Is<object> (_ => object.ReferenceEquals (_, newOrderTicket1)), It.Is<DomainObjectCommittingEventArgs> (_ => _ != null))).Verifiable();
          }

        extension.Object.CommitValidate(
              ClientTransactionScope.CurrentTransaction,
              It.Is<ReadOnlyCollection<PersistableData>> (                  collection => collection.Select(c => c.DomainObject)
                      .SetEquals(new DomainObject[] { newCustomer1, official2, newCeo2, newOrder1, newOrderItem2, newOrderTicket1 })));

        using (mockRepository.Unordered())
          {
            newCustomer1EventReceiver.Expect (_ => _.Committed(It.Is<object> (_ => object.ReferenceEquals (_, newCustomer1)),Arg<EventArgs>.Matches(_ => Mocks_Is.Anything())));
            official2EventReceiver.Setup (_ => _.Committed (It.Is<object> (_ => object.ReferenceEquals (_, official2)), It.Is<EventArgs> (_ => _ != null))).Verifiable();
            newCeo2EventReceiver.Setup (_ => _.Committed (It.Is<object> (_ => object.ReferenceEquals (_, newCeo2)), It.Is<EventArgs> (_ => _ != null))).Verifiable();
            newOrder1EventReceiver.Setup (_ => _.Committed (It.Is<object> (_ => object.ReferenceEquals (_, newOrder1)), It.Is<EventArgs> (_ => _ != null))).Verifiable();
            newOrderItem2EventReceiver.Setup (_ => _.Committed (It.Is<object> (_ => object.ReferenceEquals (_, newOrderItem2)), It.Is<EventArgs> (_ => _ != null))).Verifiable();
            newOrderTicket1EventReceiver.Setup (_ => _.Committed (It.Is<object> (_ => object.ReferenceEquals (_, newOrderTicket1)), It.Is<EventArgs> (_ => _ != null))).Verifiable();
          }

        extension.Object.Committed(
              ClientTransactionScope.CurrentTransaction,
              It.Is<ReadOnlyCollection<DomainObject>> (                  c => c.SetEquals(new DomainObject[] { newCustomer1, official2, newCeo2, newOrder1, newOrderItem2, newOrderTicket1 })));

        //14
        newOrderTicket1.Order = newOrder2;
        //15a
        newOrder2.Customer = newCustomer1;
        //15b
        newOrder2.Customer = newCustomer2;
        //16
        newOrder2.Delete();
        //17
        newOrderTicket1.Order = newOrder1;
        //cleanup for commit
        //18
        newCustomer2.Delete();
        //19
        newCeo1.Delete();
        //20
        newOrderItem1.Delete();

        //21
        ClientTransactionScope.CurrentTransaction.Commit();

        newCustomer1EventReceiver.Verify();
        newCustomer2EventReceiver.Verify();
        official2EventReceiver.Verify();
        newCeo1EventReceiver.Verify();
        newCeo2EventReceiver.Verify();
        newOrder1EventReceiver.Verify();
        newOrder2EventReceiver.Verify();
        newOrderItem1EventReceiver.Verify();
        newOrderItem2EventReceiver.Verify();
        newCustomer1OrdersEventReceiver.Verify();
        newCustomer2OrdersEventReceiver.Verify();
        official2OrdersEventReceiver.Verify();
        newOrder1OrderItemsEventReceiver.Verify();
        newOrder2OrderItemsEventReceiver.Verify();
        extension.Verify();
        newOrderTicket1EventReceiver.Verify();
      }
      finally
      {
        ClientTransactionScope.CurrentTransaction.Extensions.Remove("Extension");
      }
    }

    [Test]
    public void SetValuesAndAccessOriginalValuesTest ()
    {
      OrderItem orderItem = DomainObjectIDs.OrderItem1.GetObject<OrderItem>();

      DataContainer dataContainer = orderItem.InternalDataContainer;

      var propertyDefinition = GetPropertyDefinition(typeof(OrderItem), "Product");
      dataContainer.SetValue(propertyDefinition, "newProduct");

      Assert.That(dataContainer.GetValue(propertyDefinition, ValueAccess.Original), Is.Not.EqualTo("newProduct"));
      Assert.That(orderItem.Product, Is.EqualTo("newProduct"));

      TestableClientTransaction.Commit();
      orderItem.Product = "newProduct2";

      Assert.That(dataContainer.GetValue(propertyDefinition, ValueAccess.Original), Is.EqualTo("newProduct"));
      Assert.That(orderItem.Product, Is.EqualTo("newProduct2"));
    }

    [Test]
    public void NewCustomerAndCEOTest ()
    {
      IndustrialSector industrialSector = IndustrialSector.NewObject();
      Customer customer = Customer.NewObject();
      customer.Ceo = Ceo.NewObject();

      industrialSector.Companies.Add(customer);

      Order order1 = Order.NewObject();
      OrderTicket.NewObject(order1);

      //getting an SQL Exception without this line
      order1.DeliveryDate = DateTime.Now;

      OrderItem orderItem = OrderItem.NewObject();
      order1.OrderItems.Add(orderItem);
      order1.Official = DomainObjectIDs.Official2.GetObject<Official>();
      customer.Orders.Add(order1);

      Assert.That(() => TestableClientTransaction.Commit(), Throws.Nothing);

      customer.Delete();

      Assert.That(() => TestableClientTransaction.Commit(), Throws.TypeOf<MandatoryRelationNotSetException>());
    }

    [Test]
    public void InsertComputerAndEmployee ()
    {
      Computer computer = Computer.NewObject();
      computer.Employee = Employee.NewObject();
      computer.SerialNumber = "12345";
      computer.Employee.Name = "ABCDE";

      TestableClientTransaction.Commit();

      computer.Employee.Delete();
      computer.Delete();

      TestableClientTransaction.Commit();
    }

    [Test]
    public void PropertyEventsOfNewObjectPropertyChangeTest ()
    {
      Order newOrder = Order.NewObject();

      var eventReceiver = new DomainObjectEventReceiver(newOrder);
      CheckNoEvents(eventReceiver);

      newOrder.DeliveryDate = DateTime.Now;

      CheckEvents(eventReceiver, _orderDeliveryDateProperty);
    }

    [Test]
    public void PropertyEventsOfNewObjectRelationChangeTest ()
    {
      Order newOrder = Order.NewObject();

      var eventReceiver = new DomainObjectEventReceiver(newOrder);
      CheckNoEvents(eventReceiver);

      newOrder.Customer = null;

      CheckNoEvents(eventReceiver);
    }

    [Test]
    public void PropertyEventsOfExistingObjectPropertyChangeTest ()
    {
      Order order3 = DomainObjectIDs.Order3.GetObject<Order>();

      var eventReceiver = new DomainObjectEventReceiver(order3);
      CheckNoEvents(eventReceiver);

      order3.DeliveryDate = DateTime.Now;

      CheckEvents(eventReceiver, _orderDeliveryDateProperty);
    }

    [Test]
    public void PropertyEventsOfExistingObjectRelationChangeTest ()
    {
      Order order3 = DomainObjectIDs.Order3.GetObject<Order>();

      var eventReceiver = new DomainObjectEventReceiver(order3);
      CheckNoEvents(eventReceiver);

      order3.Customer = null;

      CheckNoEvents(eventReceiver);
    }

    [Test]
    public void SaveObjectWithNonMandatoryOneToManyRelation ()
    {
      Customer newCustomer = Customer.NewObject();
      newCustomer.Ceo = Ceo.NewObject();

      Customer existingCustomer = DomainObjectIDs.Customer3.GetObject<Customer>();
      Assert.That(existingCustomer.Orders.Count, Is.EqualTo(1));
      Assert.That(existingCustomer.Orders[0].OrderTicket, Is.Not.Null);
      Assert.That(existingCustomer.Orders[0].OrderItems.Count, Is.EqualTo(1));

      existingCustomer.Orders[0].OrderTicket.Delete();
      existingCustomer.Orders[0].OrderItems[0].Delete();
      existingCustomer.Orders[0].Delete();

      ClientTransactionScope.CurrentTransaction.Commit();
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        newCustomer = newCustomer.ID.GetObject<Customer>();
        existingCustomer = DomainObjectIDs.Customer3.GetObject<Customer>();

        Assert.That(newCustomer.Orders.Count, Is.EqualTo(0));
        Assert.That(existingCustomer.Orders.Count, Is.EqualTo(0));
      }
    }

    private void CheckNoEvents (DomainObjectEventReceiver eventReceiver)
    {
      Assert.That(eventReceiver.HasChangingEventBeenCalled, Is.False);
      Assert.That(eventReceiver.HasChangedEventBeenCalled, Is.False);
      Assert.That(eventReceiver.ChangingPropertyDefinition, Is.Null);
      Assert.That(eventReceiver.ChangedPropertyDefinition, Is.Null);
    }

    private void CheckEvents (DomainObjectEventReceiver eventReceiver, PropertyDefinition propertyDefinition)
    {
      Assert.That(eventReceiver.HasChangingEventBeenCalled, Is.True);
      Assert.That(eventReceiver.HasChangedEventBeenCalled, Is.True);
      Assert.That(eventReceiver.ChangingPropertyDefinition, Is.SameAs(propertyDefinition));
      Assert.That(eventReceiver.ChangedPropertyDefinition, Is.SameAs(propertyDefinition));
    }

    private void BackToRecord (MockRepository mockRepository, params object[] objects)
    {
      foreach (object obj in objects)
        mockRepository.BackToRecord(obj);
    }
  }
}
