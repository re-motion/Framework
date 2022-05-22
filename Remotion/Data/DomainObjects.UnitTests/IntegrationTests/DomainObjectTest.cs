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

    public override void SetUp ()
    {
      base.SetUp();

      _orderDeliveryDateProperty = GetPropertyDefinition(typeof(Order), "DeliveryDate");
    }

    [Test]
    public void RelationEventTestWithMockObject ()
    {
      var orderCustomerRelationEndPointDefinition = GetEndPointDefinition(typeof(Order), "Customer");

      Customer newCustomer1 = Customer.NewObject();
      newCustomer1.Name = "NewCustomer1";

      Customer newCustomer2 = Customer.NewObject();
      newCustomer2.Name = "NewCustomer2";

      Official official2 = DomainObjectIDs.Official2.GetObject<Official>();

      Ceo newCeo1 = Ceo.NewObject();
      newCeo1.Name = "NewCeo1";

      Ceo newCeo2 = Ceo.NewObject();
      newCeo2.Name = "NewCeo2";

      Order newOrder1 = Order.NewObject();
      newOrder1.DeliveryDate = new DateTime(2006, 1, 1);

      Order newOrder2 = Order.NewObject();
      newOrder2.DeliveryDate = new DateTime(2006, 2, 2);

      OrderItem newOrderItem1 = OrderItem.NewObject();
      newOrderItem1.Product = "Product1";

      OrderItem newOrderItem2 = OrderItem.NewObject();
      newOrderItem2.Product = "Product2";

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

      var newCustomer1EventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, newCustomer1);
      var newCustomer2EventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, newCustomer2);
      var official2EventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, official2);
      var newCeo1EventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, newCeo1);
      var newCeo2EventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, newCeo2);
      var newOrder1EventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, newOrder1);
      var newOrder2EventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, newOrder2);
      var newOrderItem1EventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, newOrderItem1);
      var newOrderItem2EventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, newOrderItem2);

      var newCustomer1OrdersEventReceiver =
          DomainObjectCollectionMockEventReceiver.CreateMock(MockBehavior.Strict, newCustomer1Orders);
      var newCustomer2OrdersEventReceiver =
          DomainObjectCollectionMockEventReceiver.CreateMock(MockBehavior.Strict, newCustomer2Orders);
      var official2OrdersEventReceiver =
          DomainObjectCollectionMockEventReceiver.CreateMock(MockBehavior.Strict, official2Orders);
      var newOrder1OrderItemsEventReceiver =
          DomainObjectCollectionMockEventReceiver.CreateMock(MockBehavior.Strict, newOrder1OrderItems);
      var newOrder2OrderItemsEventReceiver =
          DomainObjectCollectionMockEventReceiver.CreateMock(MockBehavior.Strict, newOrder2OrderItems);

      var extension = new Mock<IClientTransactionExtension>(MockBehavior.Strict);

      var sequence1 = new VerifiableSequence();
      //1
      //newCeo1.Company = newCustomer1;
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newCeo1, GetEndPointDefinition(typeof(Ceo), "Company"), null, newCustomer1))
          .Verifiable();
      newCeo1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanging(newCeo1, GetEndPointDefinition(typeof(Ceo), "Company"), null, newCustomer1)
          .Verifiable();

      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newCustomer1, GetEndPointDefinition(typeof(Company), "Ceo"), null, newCeo1))
          .Verifiable();
      newCustomer1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanging(newCustomer1, GetEndPointDefinition(typeof(Company), "Ceo"), null, newCeo1)
          .Verifiable();

      newCustomer1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanged(newCustomer1, GetEndPointDefinition(typeof(Company), "Ceo"), null, newCeo1)
          .Verifiable();
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newCustomer1, GetEndPointDefinition(typeof(Company), "Ceo"), null, newCeo1))
          .Verifiable();

      newCeo1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanged(newCeo1, GetEndPointDefinition(typeof(Ceo), "Company"), null, newCustomer1)
          .Verifiable();
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newCeo1, GetEndPointDefinition(typeof(Ceo), "Company"), null, newCustomer1))
          .Verifiable();

      //2
      //newCeo2.Company = newCustomer1;
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newCeo2, GetEndPointDefinition(typeof(Ceo), "Company"), null, newCustomer1))
          .Verifiable();
      newCeo2EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanging(newCeo2, GetEndPointDefinition(typeof(Ceo), "Company"), null, newCustomer1)
          .Verifiable();

      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newCustomer1, GetEndPointDefinition(typeof(Company), "Ceo"), newCeo1, newCeo2))
          .Verifiable();
      newCustomer1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanging(newCustomer1, GetEndPointDefinition(typeof(Company), "Ceo"), newCeo1, newCeo2)
          .Verifiable();

      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newCeo1, GetEndPointDefinition(typeof(Ceo), "Company"), newCustomer1, null))
          .Verifiable();
      newCeo1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanging(newCeo1, GetEndPointDefinition(typeof(Ceo), "Company"), newCustomer1, null)
          .Verifiable();

      newCeo1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanged(newCeo1, GetEndPointDefinition(typeof(Ceo), "Company"), newCustomer1, null)
          .Verifiable();
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newCeo1, GetEndPointDefinition(typeof(Ceo), "Company"), newCustomer1, null))
          .Verifiable();

      newCustomer1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanged(newCustomer1, GetEndPointDefinition(typeof(Company), "Ceo"), newCeo1, newCeo2)
          .Verifiable();
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newCustomer1, GetEndPointDefinition(typeof(Company), "Ceo"), newCeo1, newCeo2))
          .Verifiable();

      newCeo2EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanged(newCeo2, GetEndPointDefinition(typeof(Ceo), "Company"), null, newCustomer1)
          .Verifiable();
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newCeo2, GetEndPointDefinition(typeof(Ceo), "Company"), null, newCustomer1))
          .Verifiable();

      //3
      //newCeo1.Company = newCustomer2;
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newCeo1, GetEndPointDefinition(typeof(Ceo), "Company"), null, newCustomer2))
        .Verifiable();
      newCeo1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanging(newCeo1, GetEndPointDefinition(typeof(Ceo), "Company"), null, newCustomer2)
          .Verifiable();

      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newCustomer2, GetEndPointDefinition(typeof(Company), "Ceo"), null, newCeo1))
          .Verifiable();
      newCustomer2EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanging(newCustomer2, GetEndPointDefinition(typeof(Company), "Ceo"), null, newCeo1)
          .Verifiable();

      newCustomer2EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanged(newCustomer2, GetEndPointDefinition(typeof(Company), "Ceo"), null, newCeo1)
          .Verifiable();

      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newCustomer2, GetEndPointDefinition(typeof(Company), "Ceo"), null, newCeo1))
          .Verifiable();
      newCeo1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanged(newCeo1, GetEndPointDefinition(typeof(Ceo), "Company"), null, newCustomer2)
          .Verifiable();

      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newCeo1, GetEndPointDefinition(typeof(Ceo), "Company"), null, newCustomer2))
          .Verifiable();

      //4
      //newCeo1.Company = null;
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newCeo1, GetEndPointDefinition(typeof(Ceo), "Company"), newCustomer2, null))
          .Verifiable();
      newCeo1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanging(newCeo1, GetEndPointDefinition(typeof(Ceo), "Company"), newCustomer2, null)
          .Verifiable();

      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newCustomer2, GetEndPointDefinition(typeof(Company), "Ceo"), newCeo1, null))
          .Verifiable();
      newCustomer2EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanging(newCustomer2, GetEndPointDefinition(typeof(Company), "Ceo"), newCeo1, null)
          .Verifiable();

      newCustomer2EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanged(newCustomer2, GetEndPointDefinition(typeof(Company), "Ceo"), newCeo1, null)
          .Verifiable();
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newCustomer2, GetEndPointDefinition(typeof(Company), "Ceo"), newCeo1, null))
          .Verifiable();

      newCeo1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanged(newCeo1, GetEndPointDefinition(typeof(Ceo), "Company"), newCustomer2, null)
          .Verifiable();
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newCeo1, GetEndPointDefinition(typeof(Ceo), "Company"), newCustomer2, null))
          .Verifiable();

      //5
      //newCustomer1.Orders.Add (newOrder1);
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationReading(TestableClientTransaction, newCustomer1, GetEndPointDefinition(typeof(Customer), "Orders"), ValueAccess.Current))
        .Verifiable();
      extension
          .InVerifiableSequence(sequence1)
          .Setup(
              mock => mock.RelationRead(
                  ClientTransactionScope.CurrentTransaction,
                  newCustomer1,
                  GetEndPointDefinition(typeof(Customer), "Orders"),
                  It.Is<IReadOnlyCollectionData<DomainObject>>(data => data.Count == 0),
                  ValueAccess.Current))
          .Verifiable();

      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newOrder1, orderCustomerRelationEndPointDefinition, null, newCustomer1))
          .Verifiable();
      newOrder1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanging(newOrder1, orderCustomerRelationEndPointDefinition, null, newCustomer1)
          .Verifiable();

      newCustomer1OrdersEventReceiver
          .InVerifiableSequence(sequence1)
          .SetupAdding(newCustomer1Orders,  newOrder1)
          .Verifiable();

      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newCustomer1, GetEndPointDefinition(typeof(Customer), "Orders"), null, newOrder1))
          .Verifiable();
      newCustomer1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanging(newCustomer1, GetEndPointDefinition(typeof(Customer), "Orders"), null, newOrder1)
          .Verifiable();

      newCustomer1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanged(newCustomer1, GetEndPointDefinition(typeof(Customer), "Orders"), null, newOrder1)
          .Verifiable();
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newCustomer1, GetEndPointDefinition(typeof(Customer), "Orders"), null, newOrder1))
          .Verifiable();

      newCustomer1OrdersEventReceiver
          .InVerifiableSequence(sequence1)
          .SetupAdded(newCustomer1Orders, newOrder1)
          .Verifiable();

      newOrder1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanged(newOrder1, orderCustomerRelationEndPointDefinition, null, newCustomer1)
          .Verifiable();
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newOrder1, orderCustomerRelationEndPointDefinition, null, newCustomer1))
          .Verifiable();

      //6
      //newCustomer1.Orders.Add (newOrder2);
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationReading(TestableClientTransaction, newCustomer1, GetEndPointDefinition(typeof(Customer), "Orders"), ValueAccess.Current))
        .Verifiable();
      extension
          .InVerifiableSequence(sequence1)
          .Setup(
          mock => mock.RelationRead(
              ClientTransactionScope.CurrentTransaction,
              newCustomer1,
              GetEndPointDefinition(typeof(Customer), "Orders"),
              It.Is<IReadOnlyCollectionData<DomainObject>>(data => data.Count == 1 && data.GetObject(newOrder1.ID) == newOrder1),
              ValueAccess.Current))
        .Verifiable();

      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newOrder2, orderCustomerRelationEndPointDefinition, null, newCustomer1))
          .Verifiable();
      newOrder2EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanging(newOrder2, orderCustomerRelationEndPointDefinition, null, newCustomer1)
          .Verifiable();

      newCustomer1OrdersEventReceiver
          .InVerifiableSequence(sequence1)
          .SetupAdding(newCustomer1Orders, newOrder2)
          .Verifiable();

      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newCustomer1, GetEndPointDefinition(typeof(Customer), "Orders"), null, newOrder2))
        .Verifiable();

      newCustomer1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanging(newCustomer1, GetEndPointDefinition(typeof(Customer), "Orders"), null, newOrder2)
          .Verifiable();

      newCustomer1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanged(newCustomer1, GetEndPointDefinition(typeof(Customer), "Orders"), null, newOrder2)
          .Verifiable();

      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newCustomer1, GetEndPointDefinition(typeof(Customer), "Orders"), null, newOrder2))
          .Verifiable();

      newCustomer1OrdersEventReceiver
          .InVerifiableSequence(sequence1)
          .SetupAdded(newCustomer1Orders, newOrder2)
          .Verifiable();

      newOrder2EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanged(newOrder2, orderCustomerRelationEndPointDefinition, null, newCustomer1)
        .Verifiable();
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newOrder2, orderCustomerRelationEndPointDefinition, null, newCustomer1))
          .Verifiable();

      //7
      //newCustomer1.Orders.Remove (newOrder2);
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationReading(TestableClientTransaction, newCustomer1, GetEndPointDefinition(typeof(Customer), "Orders"), ValueAccess.Current))
        .Verifiable();
      extension
          .InVerifiableSequence(sequence1)
          .Setup(
              mock => mock.RelationRead(
                  ClientTransactionScope.CurrentTransaction,
                  newCustomer1,
                  GetEndPointDefinition(typeof(Customer), "Orders"),
                  It.Is<IReadOnlyCollectionData<DomainObject>>(
                      data => data.Count == 2 && data.GetObject(newOrder1.ID) == newOrder1 && data.GetObject(newOrder2.ID) == newOrder2),
                  ValueAccess.Current))
        .Verifiable();

      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newOrder2, orderCustomerRelationEndPointDefinition, newCustomer1, null))
        .Verifiable();
      newOrder2EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanging(newOrder2, orderCustomerRelationEndPointDefinition, newCustomer1, null)
        .Verifiable();

      newCustomer1OrdersEventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRemoving(newCustomer1Orders, newOrder2)
        .Verifiable();

      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newCustomer1, GetEndPointDefinition(typeof(Customer), "Orders"), newOrder2, null))
        .Verifiable();
      newCustomer1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanging(newCustomer1, GetEndPointDefinition(typeof(Customer), "Orders"), newOrder2, null)
        .Verifiable();

      newCustomer1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanged(newCustomer1, GetEndPointDefinition(typeof(Customer), "Orders"), newOrder2, null)
        .Verifiable();
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newCustomer1, GetEndPointDefinition(typeof(Customer), "Orders"), newOrder2, null))
        .Verifiable();

      newCustomer1OrdersEventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRemoved(newCustomer1Orders, newOrder2)
        .Verifiable();

      newOrder2EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanged(newOrder2, orderCustomerRelationEndPointDefinition, newCustomer1, null)
        .Verifiable();

      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newOrder2, orderCustomerRelationEndPointDefinition, newCustomer1, null))
        .Verifiable();

      //8
      //newOrderItem1.Order = newOrder1;
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newOrderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), null, newOrder1))
        .Verifiable();

      newOrderItem1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanging(newOrderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), null, newOrder1)
          .Verifiable();

      newOrder1OrderItemsEventReceiver
          .InVerifiableSequence(sequence1)
          .SetupAdding(newOrder1OrderItems, newOrderItem1)
          .Verifiable();

      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newOrder1, GetEndPointDefinition(typeof(Order), "OrderItems"), null, newOrderItem1))
          .Verifiable();
      newOrder1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanging(newOrder1, GetEndPointDefinition(typeof(Order), "OrderItems"), null, newOrderItem1)
          .Verifiable();

      newOrder1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanged(newOrder1, GetEndPointDefinition(typeof(Order), "OrderItems"), null, newOrderItem1)
          .Verifiable();
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newOrder1, GetEndPointDefinition(typeof(Order), "OrderItems"), null, newOrderItem1))
          .Verifiable();

      newOrder1OrderItemsEventReceiver
          .InVerifiableSequence(sequence1)
          .SetupAdded(newOrder1OrderItems, newOrderItem1)
          .Verifiable();

      newOrderItem1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanged(newOrderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), null, newOrder1)
          .Verifiable();
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newOrderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), null, newOrder1))
          .Verifiable();

      //9
      //newOrderItem2.Order = newOrder1;
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newOrderItem2, GetEndPointDefinition(typeof(OrderItem), "Order"), null, newOrder1))
          .Verifiable();
      newOrderItem2EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanging(newOrderItem2, GetEndPointDefinition(typeof(OrderItem), "Order"), null, newOrder1)
          .Verifiable();

      newOrder1OrderItemsEventReceiver
          .InVerifiableSequence(sequence1)
          .SetupAdding(newOrder1OrderItems, newOrderItem2)
          .Verifiable();

      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newOrder1, GetEndPointDefinition(typeof(Order), "OrderItems"), null, newOrderItem2))
          .Verifiable();
      newOrder1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanging(newOrder1, GetEndPointDefinition(typeof(Order), "OrderItems"), null, newOrderItem2)
          .Verifiable();

      newOrder1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanged(newOrder1, GetEndPointDefinition(typeof(Order), "OrderItems"), null, newOrderItem2)
          .Verifiable();
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newOrder1, GetEndPointDefinition(typeof(Order), "OrderItems"), null, newOrderItem2))
          .Verifiable();

      newOrder1OrderItemsEventReceiver
          .InVerifiableSequence(sequence1)
          .SetupAdded(newOrder1OrderItems, newOrderItem2)
          .Verifiable();

      newOrderItem2EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanged(newOrderItem2, GetEndPointDefinition(typeof(OrderItem), "Order"), null, newOrder1)
          .Verifiable();
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newOrderItem2, GetEndPointDefinition(typeof(OrderItem), "Order"), null, newOrder1))
          .Verifiable();

      //10
      //newOrderItem1.Order = null;
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newOrderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), newOrder1, null))
          .Verifiable();
      newOrderItem1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanging(newOrderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), newOrder1, null)
          .Verifiable();

      newOrder1OrderItemsEventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRemoving(newOrder1OrderItems, newOrderItem1)
          .Verifiable();

      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newOrder1, GetEndPointDefinition(typeof(Order), "OrderItems"), newOrderItem1, null))
          .Verifiable();
      newOrder1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanging(newOrder1, GetEndPointDefinition(typeof(Order), "OrderItems"), newOrderItem1, null)
          .Verifiable();

      newOrder1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanged(newOrder1, GetEndPointDefinition(typeof(Order), "OrderItems"), newOrderItem1, null)
          .Verifiable();
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newOrder1, GetEndPointDefinition(typeof(Order), "OrderItems"), newOrderItem1, null))
          .Verifiable();

      newOrder1OrderItemsEventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRemoved(newOrder1OrderItems, newOrderItem1)
          .Verifiable();

      newOrderItem1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanged(newOrderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), newOrder1, null)
          .Verifiable();
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newOrderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), newOrder1, null))
          .Verifiable();

      //11
      //newOrderItem1.Order = newOrder2;
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newOrderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), null, newOrder2))
          .Verifiable();
      newOrderItem1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanging(newOrderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), null, newOrder2)
          .Verifiable();

      newOrder2OrderItemsEventReceiver
          .InVerifiableSequence(sequence1)
          .SetupAdding(newOrder2OrderItems, newOrderItem1)
          .Verifiable();

      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newOrder2, GetEndPointDefinition(typeof(Order), "OrderItems"), null, newOrderItem1))
          .Verifiable();
      newOrder2EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanging(newOrder2, GetEndPointDefinition(typeof(Order), "OrderItems"), null, newOrderItem1)
          .Verifiable();

      newOrder2EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanged(newOrder2, GetEndPointDefinition(typeof(Order), "OrderItems"), null, newOrderItem1)
          .Verifiable();
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newOrder2, GetEndPointDefinition(typeof(Order), "OrderItems"), null, newOrderItem1))
          .Verifiable();

      newOrder2OrderItemsEventReceiver
          .InVerifiableSequence(sequence1)
          .SetupAdded(newOrder2OrderItems, newOrderItem1)
          .Verifiable();

      newOrderItem1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanged(newOrderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), null, newOrder2)
          .Verifiable();
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newOrderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), null, newOrder2))
          .Verifiable();

      //12
      //newOrder1.Official = official2;
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newOrder1, GetEndPointDefinition(typeof(Order), "Official"), null, official2))
          .Verifiable();
      newOrder1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanging(newOrder1, GetEndPointDefinition(typeof(Order), "Official"), null, official2)
          .Verifiable();

      official2OrdersEventReceiver
          .InVerifiableSequence(sequence1)
          .SetupAdding(official2Orders, newOrder1)
          .Verifiable();

      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, official2, GetEndPointDefinition(typeof(Official), "Orders"), null, newOrder1))
          .Verifiable();
      official2EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanging(official2, GetEndPointDefinition(typeof(Official), "Orders"), null, newOrder1)
          .Verifiable();

      official2EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanged(official2, GetEndPointDefinition(typeof(Official), "Orders"), null, newOrder1)
          .Verifiable();
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, official2, GetEndPointDefinition(typeof(Official), "Orders"), null, newOrder1))
        .Verifiable();

      official2OrdersEventReceiver
          .InVerifiableSequence(sequence1)
          .SetupAdded(official2Orders, newOrder1)
          .Verifiable();

      newOrder1EventReceiver
          .InVerifiableSequence(sequence1)
          .SetupRelationChanged(newOrder1, GetEndPointDefinition(typeof(Order), "Official"), null, official2)
          .Verifiable();
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newOrder1, GetEndPointDefinition(typeof(Order), "Official"), null, official2))
          .Verifiable();

      //13
      //OrderTicket newOrderTicket1 = OrderTicket.NewObject (newOrder1);
      //newOrderTicket1.FileName = @"C:\orders\order1.tkt";

      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.NewObjectCreating(TestableClientTransaction, typeof(OrderTicket)))
          .Verifiable();

      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, It.IsNotNull<OrderTicket>(), GetEndPointDefinition(typeof(OrderTicket), "Order"), null, newOrder1))
          .Verifiable();

      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanging(TestableClientTransaction, newOrder1, GetEndPointDefinition(typeof(Order), "OrderTicket"), null, It.IsNotNull<OrderTicket>()))
          .Verifiable();
      newOrder1EventReceiver
          .InVerifiableSequence(sequence1)
          .Setup(
              _ => _.RelationChanging(
                  newOrder1,
                  It.Is<RelationChangingEventArgs>(
                      args =>
                          args.RelationEndPointDefinition == GetEndPointDefinition(typeof(Order), "OrderTicket")
                          && args.OldRelatedObject == null
                          && args.NewRelatedObject is OrderTicket)))
          .Verifiable();

      newOrder1EventReceiver
          .InVerifiableSequence(sequence1)
          .Setup(
              _ => _.RelationChanged(
                  newOrder1,
                  It.Is<RelationChangedEventArgs>(
                      args =>
                          args.RelationEndPointDefinition == GetEndPointDefinition(typeof(Order), "OrderTicket")
                          && args.OldRelatedObject == null
                          && args.NewRelatedObject is OrderTicket)))
          .Verifiable();
      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, newOrder1, GetEndPointDefinition(typeof(Order), "OrderTicket"), null, It.IsNotNull<OrderTicket>()))
          .Verifiable();

      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.RelationChanged(TestableClientTransaction, It.IsNotNull<OrderTicket>(), GetEndPointDefinition(typeof(OrderTicket), "Order"), null, newOrder1))
          .Verifiable();

      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.PropertyValueChanging(TestableClientTransaction, It.IsNotNull<OrderTicket>(), GetPropertyDefinition(typeof(OrderTicket), "FileName"), null, @"C:\orders\order1.tkt"))
          .Verifiable();

      extension
          .InVerifiableSequence(sequence1)
          .Setup(_ => _.PropertyValueChanged(TestableClientTransaction, It.IsNotNull<OrderTicket>(), GetPropertyDefinition(typeof(OrderTicket), "FileName"), null, @"C:\orders\order1.tkt"))
          .Verifiable();

      extension.Setup(stub => stub.Key).Returns("Extension");

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
        newOrderTicket1.FileName = @"C:\orders\order1.tkt";

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

        extension.Reset();
        newCustomer1EventReceiver.Reset();
        newCustomer2EventReceiver.Reset();
        official2EventReceiver.Reset();
        newCeo1EventReceiver.Reset();
        newCeo2EventReceiver.Reset();
        newOrder1EventReceiver.Reset();
        newOrder2EventReceiver.Reset();
        newOrderItem1EventReceiver.Reset();
        newOrderItem2EventReceiver.Reset();
        newCustomer1OrdersEventReceiver.Reset();
        newCustomer2OrdersEventReceiver.Reset();
        official2OrdersEventReceiver.Reset();
        newOrder1OrderItemsEventReceiver.Reset();
        newOrder2OrderItemsEventReceiver.Reset();

        var newOrderTicket1EventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, newOrderTicket1);

        var sequence2 = new VerifiableSequence();

        //14
        //newOrderTicket1.Order = newOrder2;
        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.RelationChanging(TestableClientTransaction, newOrderTicket1, GetEndPointDefinition(typeof(OrderTicket), "Order"), newOrder1, newOrder2))
            .Verifiable();
        newOrderTicket1EventReceiver
            .InVerifiableSequence(sequence2)
            .SetupRelationChanging(newOrderTicket1, GetEndPointDefinition(typeof(OrderTicket), "Order"), newOrder1, newOrder2)
            .Verifiable();

        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.RelationChanging(TestableClientTransaction, newOrder1, GetEndPointDefinition(typeof(Order), "OrderTicket"), newOrderTicket1, null))
            .Verifiable();
        newOrder1EventReceiver
            .InVerifiableSequence(sequence2)
            .SetupRelationChanging(newOrder1, GetEndPointDefinition(typeof(Order), "OrderTicket"), newOrderTicket1, null)
            .Verifiable();

        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.RelationChanging(TestableClientTransaction, newOrder2, GetEndPointDefinition(typeof(Order), "OrderTicket"), null, newOrderTicket1))
            .Verifiable();
        newOrder2EventReceiver
            .InVerifiableSequence(sequence2)
            .SetupRelationChanging(newOrder2, GetEndPointDefinition(typeof(Order), "OrderTicket"), null, newOrderTicket1)
            .Verifiable();

        newOrder2EventReceiver
            .InVerifiableSequence(sequence2)
            .SetupRelationChanged(newOrder2, GetEndPointDefinition(typeof(Order), "OrderTicket"), null, newOrderTicket1)
            .Verifiable();
        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.RelationChanged(TestableClientTransaction, newOrder2, GetEndPointDefinition(typeof(Order), "OrderTicket"), null, newOrderTicket1))
            .Verifiable();

        newOrder1EventReceiver
            .InVerifiableSequence(sequence2)
            .SetupRelationChanged(newOrder1, GetEndPointDefinition(typeof(Order), "OrderTicket"), newOrderTicket1, null)
            .Verifiable();
        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.RelationChanged(TestableClientTransaction, newOrder1, GetEndPointDefinition(typeof(Order), "OrderTicket"), newOrderTicket1, null))
            .Verifiable();

        newOrderTicket1EventReceiver
            .InVerifiableSequence(sequence2)
            .SetupRelationChanged(newOrderTicket1, GetEndPointDefinition(typeof(OrderTicket), "Order"), newOrder1, newOrder2)
            .Verifiable();
        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.RelationChanged(TestableClientTransaction, newOrderTicket1, GetEndPointDefinition(typeof(OrderTicket), "Order"), newOrder1, newOrder2))
            .Verifiable();

        //15a
        //newOrder2.Customer = newCustomer1;
        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.RelationChanging(TestableClientTransaction, newOrder2, orderCustomerRelationEndPointDefinition, null, newCustomer1))
            .Verifiable();
        newOrder2EventReceiver
            .InVerifiableSequence(sequence2)
            .SetupRelationChanging(newOrder2, orderCustomerRelationEndPointDefinition, null, newCustomer1)
            .Verifiable();

        newCustomer1OrdersEventReceiver
            .InVerifiableSequence(sequence2)
            .SetupAdding(newCustomer1Orders, newOrder2)
            .Verifiable();

        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.RelationChanging(TestableClientTransaction, newCustomer1, GetEndPointDefinition(typeof(Customer), "Orders"), null, newOrder2))
            .Verifiable();
        newCustomer1EventReceiver
            .InVerifiableSequence(sequence2)
            .SetupRelationChanging(newCustomer1, GetEndPointDefinition(typeof(Customer), "Orders"), null, newOrder2)
            .Verifiable();

        newCustomer1EventReceiver
            .InVerifiableSequence(sequence2)
            .SetupRelationChanged(newCustomer1, GetEndPointDefinition(typeof(Customer), "Orders"), null, newOrder2)
            .Verifiable();
        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.RelationChanged(TestableClientTransaction, newCustomer1, GetEndPointDefinition(typeof(Customer), "Orders"), null, newOrder2))
            .Verifiable();

        newCustomer1OrdersEventReceiver
            .InVerifiableSequence(sequence2)
            .SetupAdded(newCustomer1Orders, newOrder2)
            .Verifiable();

        newOrder2EventReceiver
            .InVerifiableSequence(sequence2)
            .SetupRelationChanged(newOrder2, orderCustomerRelationEndPointDefinition, null, newCustomer1)
            .Verifiable();
        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.RelationChanged(TestableClientTransaction, newOrder2, orderCustomerRelationEndPointDefinition, null, newCustomer1))
            .Verifiable();

        //15b
        //newOrder2.Customer = newCustomer2;
        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.RelationChanging(TestableClientTransaction, newOrder2, orderCustomerRelationEndPointDefinition, newCustomer1, newCustomer2))
            .Verifiable();
        newOrder2EventReceiver
            .InVerifiableSequence(sequence2)
            .SetupRelationChanging(newOrder2, orderCustomerRelationEndPointDefinition, newCustomer1, newCustomer2)
            .Verifiable();

        newCustomer2OrdersEventReceiver
            .InVerifiableSequence(sequence2)
            .SetupAdding(newCustomer2Orders, newOrder2)
            .Verifiable();

        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.RelationChanging(TestableClientTransaction, newCustomer2, GetEndPointDefinition(typeof(Customer), "Orders"), null, newOrder2))
            .Verifiable();
        newCustomer2EventReceiver
            .InVerifiableSequence(sequence2)
            .SetupRelationChanging(newCustomer2, GetEndPointDefinition(typeof(Customer), "Orders"), null, newOrder2)
            .Verifiable();

        newCustomer1OrdersEventReceiver
            .InVerifiableSequence(sequence2)
            .SetupRemoving(newCustomer1Orders, newOrder2)
            .Verifiable();

        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.RelationChanging(TestableClientTransaction, newCustomer1, GetEndPointDefinition(typeof(Customer), "Orders"), newOrder2, null))
            .Verifiable();
        newCustomer1EventReceiver
            .InVerifiableSequence(sequence2)
            .SetupRelationChanging(newCustomer1, GetEndPointDefinition(typeof(Customer), "Orders"), newOrder2, null)
            .Verifiable();

        newCustomer1EventReceiver
            .InVerifiableSequence(sequence2)
            .SetupRelationChanged(newCustomer1, GetEndPointDefinition(typeof(Customer), "Orders"), newOrder2, null)
            .Verifiable();
        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.RelationChanged(TestableClientTransaction, newCustomer1, GetEndPointDefinition(typeof(Customer), "Orders"), newOrder2, null))
            .Verifiable();

        newCustomer1OrdersEventReceiver
            .InVerifiableSequence(sequence2)
            .SetupRemoved(newCustomer1Orders, newOrder2)
            .Verifiable();

        newCustomer2EventReceiver
            .InVerifiableSequence(sequence2)
            .SetupRelationChanged(newCustomer2, GetEndPointDefinition(typeof(Customer), "Orders"), null, newOrder2)
            .Verifiable();
        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.RelationChanged(TestableClientTransaction, newCustomer2, GetEndPointDefinition(typeof(Customer), "Orders"), null, newOrder2))
            .Verifiable();

        newCustomer2OrdersEventReceiver
            .InVerifiableSequence(sequence2)
            .SetupAdded(newCustomer2Orders, newOrder2)
            .Verifiable();

        newOrder2EventReceiver
            .InVerifiableSequence(sequence2)
            .SetupRelationChanged(newOrder2, orderCustomerRelationEndPointDefinition, newCustomer1, newCustomer2)
            .Verifiable();
        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.RelationChanged(TestableClientTransaction, newOrder2, orderCustomerRelationEndPointDefinition, newCustomer1, newCustomer2))
            .Verifiable();

        //16
        //newOrder2.Delete ();
        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.ObjectDeleting(TestableClientTransaction, newOrder2))
            .Verifiable();

        newOrder2EventReceiver
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.Deleting(newOrder2, EventArgs.Empty))
            .Verifiable();

        newOrder2OrderItemsEventReceiver
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.Deleting(newOrder2OrderItems, EventArgs.Empty))
            .Verifiable();

        newCustomer2OrdersEventReceiver
            .SetupRemoving(newCustomer2Orders, newOrder2)
            .Verifiable();
        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.RelationChanging(TestableClientTransaction, newCustomer2, GetEndPointDefinition(typeof(Customer), "Orders"), newOrder2, null))
            .Verifiable();
        newCustomer2EventReceiver
            .SetupRelationChanging(newCustomer2, GetEndPointDefinition(typeof(Customer), "Orders"), newOrder2, null)
            .Verifiable();

        extension
            .Setup(_ => _.RelationChanging(TestableClientTransaction, newOrderTicket1, GetEndPointDefinition(typeof(OrderTicket), "Order"), newOrder2, null))
            .Verifiable();
        newOrderTicket1EventReceiver
            .SetupRelationChanging(newOrderTicket1, GetEndPointDefinition(typeof(OrderTicket), "Order"), newOrder2, null)
            .Verifiable();

        extension
            .Setup(_ => _.RelationChanging(TestableClientTransaction, newOrderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), newOrder2, null))
            .Verifiable();
        newOrderItem1EventReceiver
            .SetupRelationChanging(newOrderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), newOrder2, null)
            .Verifiable();

        newCustomer2EventReceiver
            .SetupRelationChanged(newCustomer2, GetEndPointDefinition(typeof(Customer), "Orders"), newOrder2, null)
            .Verifiable();
        extension
            .Setup(_ => _.RelationChanged(TestableClientTransaction, newCustomer2, GetEndPointDefinition(typeof(Customer), "Orders"), newOrder2, null))
            .Verifiable();
        newCustomer2OrdersEventReceiver
            .SetupRemoved(newCustomer2Orders, newOrder2)
            .Verifiable();

        newOrderTicket1EventReceiver
            .SetupRelationChanged(newOrderTicket1, GetEndPointDefinition(typeof(OrderTicket), "Order"), newOrder2, null)
            .Verifiable();
        extension
            .Setup(_ => _.RelationChanged(TestableClientTransaction, newOrderTicket1, GetEndPointDefinition(typeof(OrderTicket), "Order"), newOrder2, null))
            .Verifiable();

        newOrderItem1EventReceiver
            .SetupRelationChanged(newOrderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), newOrder2, null)
            .Verifiable();
        extension
            .Setup(_ => _.RelationChanged(TestableClientTransaction, newOrderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), newOrder2, null))
            .Verifiable();

        newOrder2OrderItemsEventReceiver
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.Deleted(newOrder2OrderItems, It.IsAny<EventArgs>()))
            .Verifiable();

        newOrder2EventReceiver
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.Deleted(newOrder2, It.IsNotNull<EventArgs>()))
            .Verifiable();

        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.ObjectDeleted(TestableClientTransaction, newOrder2))
            .Verifiable();

        //17
        //newOrderTicket1.Order = newOrder1;
        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.RelationChanging(TestableClientTransaction, newOrderTicket1, GetEndPointDefinition(typeof(OrderTicket), "Order"), null, newOrder1))
            .Verifiable();
        newOrderTicket1EventReceiver
            .InVerifiableSequence(sequence2)
            .SetupRelationChanging(newOrderTicket1, GetEndPointDefinition(typeof(OrderTicket), "Order"), null, newOrder1)
            .Verifiable();

        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.RelationChanging(TestableClientTransaction, newOrder1, GetEndPointDefinition(typeof(Order), "OrderTicket"), null, newOrderTicket1))
            .Verifiable();
        newOrder1EventReceiver
            .InVerifiableSequence(sequence2)
            .SetupRelationChanging(newOrder1, GetEndPointDefinition(typeof(Order), "OrderTicket"), null, newOrderTicket1)
            .Verifiable();

        newOrder1EventReceiver
            .InVerifiableSequence(sequence2)
            .SetupRelationChanged(newOrder1, GetEndPointDefinition(typeof(Order), "OrderTicket"), null, newOrderTicket1)
            .Verifiable();
        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.RelationChanged(TestableClientTransaction, newOrder1, GetEndPointDefinition(typeof(Order), "OrderTicket"), null, newOrderTicket1))
            .Verifiable();

        newOrderTicket1EventReceiver
            .InVerifiableSequence(sequence2)
            .SetupRelationChanged(newOrderTicket1, GetEndPointDefinition(typeof(OrderTicket), "Order"), null, newOrder1)
            .Verifiable();
        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.RelationChanged(TestableClientTransaction, newOrderTicket1, GetEndPointDefinition(typeof(OrderTicket), "Order"), null, newOrder1))
            .Verifiable();

        //cleanup for commit
        //18
        //newCustomer2.Delete ();
        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.ObjectDeleting(TestableClientTransaction, newCustomer2))
            .Verifiable();
        newCustomer2EventReceiver
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.Deleting(newCustomer2, It.IsNotNull<EventArgs>()))
            .Verifiable();
        newCustomer2OrdersEventReceiver
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.Deleting(newCustomer2Orders, It.IsAny<EventArgs>()))
            .Verifiable();

        newCustomer2OrdersEventReceiver
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.Deleted(newCustomer2Orders, It.IsAny<EventArgs>()))
            .Verifiable();
        newCustomer2EventReceiver
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.Deleted(newCustomer2, It.IsNotNull<EventArgs>()))
            .Verifiable();
        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.ObjectDeleted(TestableClientTransaction, newCustomer2))
            .Verifiable();

        //19
        //newCeo1.Delete ();
        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.ObjectDeleting(TestableClientTransaction, newCeo1))
            .Verifiable();
        newCeo1EventReceiver
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.Deleting(newCeo1, It.IsNotNull<EventArgs>()))
            .Verifiable();

        newCeo1EventReceiver
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.Deleted(newCeo1, It.IsNotNull<EventArgs>()))
            .Verifiable();
        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.ObjectDeleted(TestableClientTransaction, newCeo1))
            .Verifiable();

        //20
        //newOrderItem1.Delete ();
        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.ObjectDeleting(TestableClientTransaction, newOrderItem1))
            .Verifiable();

        newOrderItem1EventReceiver
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.Deleting(newOrderItem1, It.IsNotNull<EventArgs>()))
            .Verifiable();

        newOrderItem1EventReceiver
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.Deleted(newOrderItem1, It.IsNotNull<EventArgs>()))
            .Verifiable();

        extension
            .InVerifiableSequence(sequence2)
            .Setup(_ => _.ObjectDeleted(TestableClientTransaction, newOrderItem1))
            .Verifiable();

        //21
        //ClientTransactionScope.CurrentTransaction.Commit ();

        extension
            .InVerifiableSequence(sequence2)
            .Setup(
                _ => _.Committing(
                    TestableClientTransaction,
                    It.Is<ReadOnlyCollection<IDomainObject>>(c => c.SetEquals(new DomainObject[] { newCustomer1, official2, newCeo2, newOrder1, newOrderItem2, newOrderTicket1 })),
                    It.IsAny<ICommittingEventRegistrar>()))
            .Verifiable();

        newCustomer1EventReceiver
            .Setup(_ => _.Committing(newCustomer1, It.IsNotNull<DomainObjectCommittingEventArgs>()))
            .Verifiable();
        official2EventReceiver
            .Setup(_ => _.Committing(official2, It.IsNotNull<DomainObjectCommittingEventArgs>()))
            .Verifiable();
        newCeo2EventReceiver
            .Setup(_ => _.Committing(newCeo2, It.IsNotNull<DomainObjectCommittingEventArgs>()))
            .Verifiable();
        newOrder1EventReceiver
            .Setup(_ => _.Committing(newOrder1, It.IsNotNull<DomainObjectCommittingEventArgs>()))
            .Verifiable();
        newOrderItem2EventReceiver
            .Setup(_ => _.Committing(newOrderItem2, It.IsNotNull<DomainObjectCommittingEventArgs>()))
            .Verifiable();
        newOrderTicket1EventReceiver
            .Setup(_ => _.Committing(newOrderTicket1, It.IsNotNull<DomainObjectCommittingEventArgs>()))
            .Verifiable();

        extension
            .InVerifiableSequence(sequence2)
            .Setup(
                _ => _.CommitValidate(
                    TestableClientTransaction,
                    It.Is<ReadOnlyCollection<PersistableData>>(
                        collection => collection.Select(c => c.DomainObject)
                            .SetEquals(new DomainObject[] { newCustomer1, official2, newCeo2, newOrder1, newOrderItem2, newOrderTicket1 }))))
            .Verifiable();

        newCustomer1EventReceiver
            .Setup(_ => _.Committed(newCustomer1, It.IsNotNull<EventArgs>()))
            .Verifiable();
        official2EventReceiver
            .Setup(_ => _.Committed(official2, It.IsNotNull<EventArgs>()))
            .Verifiable();
        newCeo2EventReceiver
            .Setup(_ => _.Committed(newCeo2, It.IsNotNull<EventArgs>()))
            .Verifiable();
        newOrder1EventReceiver
            .Setup(_ => _.Committed(newOrder1, It.IsNotNull<EventArgs>()))
            .Verifiable();
        newOrderItem2EventReceiver
            .Setup(_ => _.Committed(newOrderItem2, It.IsNotNull<EventArgs>()))
            .Verifiable();
        newOrderTicket1EventReceiver
            .Setup(_ => _.Committed(newOrderTicket1, It.IsNotNull<EventArgs>()))
            .Verifiable();

        extension
            .InVerifiableSequence(sequence2)
            .Setup(
                _ => _.Committed(
                    TestableClientTransaction,
                    It.Is<ReadOnlyCollection<IDomainObject>>(c => c.SetEquals(new DomainObject[] { newCustomer1, official2, newCeo2, newOrder1, newOrderItem2, newOrderTicket1 }))))
            .Verifiable();

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
        TestableClientTransaction.Commit();

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
      industrialSector.Name = "Sector";

      Customer customer = Customer.NewObject();
      customer.Name = "Customer";
      customer.Ceo = Ceo.NewObject();
      customer.Ceo.Name = "CustomerCEO";

      industrialSector.Companies.Add(customer);

      Order order1 = Order.NewObject();
      var orderTicket1 = OrderTicket.NewObject(order1);
      orderTicket1.FileName = @"C:\temp\order.tkt";

      //getting an SQL Exception without this line
      order1.DeliveryDate = DateTime.Now;

      OrderItem orderItem = OrderItem.NewObject();
      orderItem.Product = "Product";

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
      newCustomer.Name = "Customer";
      newCustomer.Ceo = Ceo.NewObject();
      newCustomer.Ceo.Name = "CustomerCEO";

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
  }
}
