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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Unload
{
  [TestFixture]
  public class UnloadVirtualEndPointTest : UnloadTestBase
  {
    [Test]
    public void UnloadVirtualEndPoint_Collection ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var orderItems = order.OrderItems;
      orderItems.EnsureDataComplete();
      var orderItem1 = DomainObjectIDs.OrderItem1.GetObject<OrderItem>();
      var orderItem2 = DomainObjectIDs.OrderItem2.GetObject<OrderItem>();

      Assert.That(orderItems.IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, orderItems.AssociatedEndPointID);

      CheckDataContainerExists(order, true);
      CheckDataContainerExists(orderItem1, true);
      CheckDataContainerExists(orderItem2, true);

      CheckEndPointExists(orderItem1, "Order", true);
      CheckEndPointExists(orderItem2, "Order", true);
      CheckVirtualEndPointExistsAndComplete(order, "OrderItems", true, false);

      Assert.That(order.State.IsUnchanged, Is.True);
      Assert.That(orderItem1.State.IsUnchanged, Is.True);
      Assert.That(orderItem2.State.IsUnchanged, Is.True);
    }

    [Test]
    public void UnloadVirtualEndPoint_Collection_AccessingEndPoint ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var orderItems = order.OrderItems;
      orderItems.EnsureDataComplete();
      var orderItem1 = DomainObjectIDs.OrderItem1.GetObject<OrderItem>();
      var orderItem2 = DomainObjectIDs.OrderItem2.GetObject<OrderItem>();

      Assert.That(orderItems.IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPoint(
          TestableClientTransaction,
          orderItems.AssociatedEndPointID);

      Assert.That(orderItems.IsDataComplete, Is.False);

      Dev.Null = order.OrderItems;

      Assert.That(orderItems.IsDataComplete, Is.False, "Reaccessing the end point does not load data");

      var orderItemArray = order.OrderItems.ToArray();

      Assert.That(orderItems.IsDataComplete, Is.True);
      Assert.That(orderItemArray, Is.EquivalentTo(new[] { orderItem1, orderItem2 }));
    }

    [Test]
    public void UnloadVirtualEndPoint_Collection_EnsureDataComplete ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var orderItems = order.OrderItems;
      orderItems.EnsureDataComplete();
      var orderItem1 = DomainObjectIDs.OrderItem1.GetObject<OrderItem>();
      var orderItem2 = DomainObjectIDs.OrderItem2.GetObject<OrderItem>();

      Assert.That(orderItems.IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPoint(
          TestableClientTransaction,
          orderItems.AssociatedEndPointID);

      Assert.That(orderItems.IsDataComplete, Is.False);

      orderItems.EnsureDataComplete();

      Assert.That(orderItems.IsDataComplete, Is.True);
      Assert.That(orderItems, Is.EquivalentTo(new[] { orderItem1, orderItem2 }));
    }

    [Test]
    public void UnloadVirtualEndPoint_Collection_Reload ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var orderItems = order.OrderItems;
      orderItems.EnsureDataComplete();
      var orderItem1 = DomainObjectIDs.OrderItem1.GetObject<OrderItem>();
      var orderItem2 = DomainObjectIDs.OrderItem2.GetObject<OrderItem>();

      ObjectID newOrderItemID;
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var orderInOtherTx = DomainObjectIDs.Order1.GetObject<Order>();
        var newOrderItem = OrderItem.NewObject();
        newOrderItem.Product = "Product";

        newOrderItemID = newOrderItem.ID;
        orderInOtherTx.OrderItems.Add(newOrderItem);
        ClientTransaction.Current.Commit();
      }

      Assert.That(orderItems, Is.EquivalentTo(new[] { orderItem1, orderItem2 }));

      UnloadService.UnloadVirtualEndPoint(
          TestableClientTransaction,
          orderItems.AssociatedEndPointID);

      Assert.That(orderItems, Is.EquivalentTo(new[] { orderItem1, orderItem2, newOrderItemID.GetObject<OrderItem>() }));
    }

    [Test]
    public void UnloadVirtualEndPoint_DomainObjectCollection_AlreadyUnloaded ()
    {
      var customer = DomainObjectIDs.Customer1.GetObject<Customer>();
      var endPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint(customer.Orders);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, endPoint.ID);
      ClientTransactionTestHelperWithMocks.EnsureTransactionThrowsOnEvents(TestableClientTransaction);
      Assert.That(endPoint.IsDataComplete, Is.False);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, endPoint.ID);
    }

    [Test]
    public void UnloadVirtualEndPoint_VirtualCollection_AlreadyUnloaded ()
    {
      var product = DomainObjectIDs.Product1.GetObject<Product>();
      var endPoint = VirtualCollectionDataTestHelper.GetAssociatedEndPoint(product.Reviews);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, endPoint.ID);
      ClientTransactionTestHelperWithMocks.EnsureTransactionThrowsOnEvents(TestableClientTransaction);
      Assert.That(endPoint.IsDataComplete, Is.False);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, endPoint.ID);
    }

    [Test]
    public void UnloadVirtualEndPoint_DomainObjectCollection_ChangedCollectionReference_Throws ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      order.OrderItems = new ObjectList<OrderItem>(order.OrderItems); // new collection reference, same contents
      var endPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint(order.OrderItems);

      Assert.That(endPoint.HasChanged, Is.True);

      Assert.That(
          () => UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, endPoint.ID),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "The end point with ID "
              + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems' has been "
              + "changed. Changed end points cannot be unloaded."));

      Assert.That(UnloadService.TryUnloadVirtualEndPoint(TestableClientTransaction, endPoint.ID), Is.False);

      CheckVirtualEndPointExistsAndComplete(order, "OrderItems", true, true);
    }

    [Test]
    public void UnloadVirtualEndPoint_Object ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var orderTicket = order.OrderTicket;

      CheckVirtualEndPointExistsAndComplete(order, "OrderTicket", true, true);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, RelationEndPointID.Resolve(order, o => o.OrderTicket));

      CheckDataContainerExists(order, true);
      CheckDataContainerExists(orderTicket, true);

      CheckEndPointExists(orderTicket, "Order", true);
      CheckVirtualEndPointExistsAndComplete(order, "OrderTicket", true, false);

      Assert.That(order.State.IsUnchanged, Is.True);
      Assert.That(orderTicket.State.IsUnchanged, Is.True);
    }

    [Test]
    public void UnloadVirtualEndPoint_Object_AccessingEndPoint ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      TestableClientTransaction.EnsureDataComplete(RelationEndPointID.Resolve(order, o => o.OrderTicket));

      CheckVirtualEndPointExistsAndComplete(order, "OrderTicket", true, true);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, RelationEndPointID.Resolve(order, o => o.OrderTicket));

      CheckVirtualEndPointExistsAndComplete(order, "OrderTicket", true, false);

      Dev.Null = order.OrderTicket;

      CheckVirtualEndPointExistsAndComplete(order, "OrderTicket", true, true);
    }

    [Test]
    public void UnloadVirtualEndPoint_Object_EnsureDataComplete ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var orderTicket = order.OrderTicket;

      CheckVirtualEndPointExistsAndComplete(order, "OrderTicket", true, true);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, RelationEndPointID.Resolve(order, o => o.OrderTicket));

      CheckVirtualEndPointExistsAndComplete(order, "OrderTicket", true, false);

      TestableClientTransaction.EnsureDataComplete(RelationEndPointID.Resolve(order, o => o.OrderTicket));

      CheckVirtualEndPointExistsAndComplete(order, "OrderTicket", true, true);
      Assert.That(order.OrderTicket, Is.SameAs(orderTicket));
    }

    [Test]
    public void UnloadVirtualEndPoint_Object_Reload ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var oldOrderTicket = order.OrderTicket;

      ObjectID newOrderTicketID;
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var orderInOtherTx = DomainObjectIDs.Order1.GetObject<Order>();
        orderInOtherTx.OrderTicket.Delete();

        orderInOtherTx.OrderTicket = OrderTicket.NewObject();
        orderInOtherTx.OrderTicket.FileName = @"C:\order.tkt";

        newOrderTicketID = orderInOtherTx.OrderTicket.ID;
        ClientTransaction.Current.Commit();
      }

      Assert.That(order.OrderTicket, Is.SameAs(oldOrderTicket));

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, RelationEndPointID.Resolve(order, o => o.OrderTicket));

      Assert.That(order.OrderTicket, Is.SameAs(newOrderTicketID.GetObject<OrderTicket>()));
    }

    [Test]
    public void UnloadVirtualEndPoint_Object_Null_Reload ()
    {
      var employee = DomainObjectIDs.Employee1.GetObject<Employee>();
      Dev.Null = employee.Computer;

      ObjectID newComputerID;
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var employeeInOtherTx = employee.ID.GetObject<Employee>();
        employeeInOtherTx.Computer = Computer.NewObject();
        employeeInOtherTx.Computer.SerialNumber = "12345";

        newComputerID = employeeInOtherTx.Computer.ID;
        ClientTransaction.Current.Commit();
      }

      Assert.That(employee.Computer, Is.Null);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, RelationEndPointID.Resolve(employee, e => e.Computer));

      Assert.That(employee.Computer.ID, Is.EqualTo(newComputerID));
      Assert.That(employee.Computer, Is.SameAs(newComputerID.GetObject<Computer>()));
    }

    [Test]
    public void UnloadVirtualEndPoint_Object_AlreadyUnloaded ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      TestableClientTransaction.EnsureDataComplete(RelationEndPointID.Resolve(order, o => o.OrderTicket));

      CheckVirtualEndPointExistsAndComplete(order, "OrderTicket", true, true);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, RelationEndPointID.Resolve(order, o => o.OrderTicket));

      CheckVirtualEndPointExistsAndComplete(order, "OrderTicket", true, false);
      ClientTransactionTestHelperWithMocks.EnsureTransactionThrowsOnEvents(TestableClientTransaction);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, RelationEndPointID.Resolve(order, o => o.OrderTicket));
    }

    [Test]
    public void UnloadVirtualEndPoint_EndPointsOfNewObject_CannotBeUnloaded ()
    {
      var order = Order.NewObject();
      var endPointID1 = RelationEndPointID.Resolve(order, o => o.OrderTicket);
      var endPointID2 = RelationEndPointID.Resolve(order, o => o.OrderItems);

      CheckEndPointExists(endPointID1, true);
      CheckEndPointExists(endPointID2, true);

      Assert.That(
          () => UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, endPointID1),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "Cannot unload the following relation end-points because they belong to new or deleted objects: " + endPointID1 + "."));
      Assert.That(
          () => UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, endPointID2),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "Cannot unload the following relation end-points because they belong to new or deleted objects: " + endPointID2 + "."));

      Assert.That(UnloadService.TryUnloadVirtualEndPoint(TestableClientTransaction, endPointID1), Is.False);
      Assert.That(UnloadService.TryUnloadVirtualEndPoint(TestableClientTransaction, endPointID2), Is.False);

      CheckEndPointExists(endPointID1, true);
      CheckEndPointExists(endPointID2, true);
    }

    [Test]
    public void UnloadVirtualEndPoint_EndPointsOfDeletedObject_CannotBeUnloaded ()
    {
      var customerWithoutOrders = DomainObjectIDs.Customer2.GetObject<Customer>();
      var employeeWithoutComputer = DomainObjectIDs.Employee1.GetObject<Employee>();
      var endPointID1 = RelationEndPointID.Resolve(customerWithoutOrders, o => o.Orders);
      var endPointID2 = RelationEndPointID.Resolve(employeeWithoutComputer, o => o.Computer);

      customerWithoutOrders.Delete();
      employeeWithoutComputer.Delete();

      CheckEndPointExists(endPointID1, true);
      CheckEndPointExists(endPointID2, true);

      Assert.That(
          () => UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, endPointID1),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "Cannot unload the following relation end-points because they belong to new or deleted objects: " + endPointID1 + "."));
      Assert.That(
          () => UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, endPointID2),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "Cannot unload the following relation end-points because they belong to new or deleted objects: " + endPointID2 + "."));

      Assert.That(UnloadService.TryUnloadVirtualEndPoint(TestableClientTransaction, endPointID1), Is.False);
      Assert.That(UnloadService.TryUnloadVirtualEndPoint(TestableClientTransaction, endPointID2), Is.False);

      CheckEndPointExists(endPointID1, true);
      CheckEndPointExists(endPointID2, true);
    }
  }
}
