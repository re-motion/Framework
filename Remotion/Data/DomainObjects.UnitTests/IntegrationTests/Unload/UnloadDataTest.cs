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
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Synchronization;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Unload
{
  [TestFixture]
  public class UnloadDataTest : UnloadTestBase
  {
    [Test]
    public void UnloadData_OrderTicket ()
    {
      var orderTicket1 = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket>();
      var order = orderTicket1.Order;
      order.EnsureDataAvailable();

      Assert.That(orderTicket1.State.IsUnchanged, Is.True);
      Assert.That(order.State.IsUnchanged, Is.True);

      UnloadService.UnloadData(TestableClientTransaction, orderTicket1.ID);

      CheckDataContainerExists(orderTicket1, false);
      CheckDataContainerExists(order, true);

      CheckEndPointExists(orderTicket1, "Order", false);
      CheckEndPointExists(order, "OrderTicket", false);

      Assert.That(orderTicket1.State.IsNotLoadedYet, Is.True);
      Assert.That(order.State.IsUnchanged, Is.True);
    }

    [Test]
    public void UnloadData_OrderTicket_ReloadData ()
    {
      var orderTicket1 = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket>();
      var order = orderTicket1.Order;
      order.EnsureDataAvailable();

      Assert.That(orderTicket1.State.IsUnchanged, Is.True);
      Assert.That(order.State.IsUnchanged, Is.True);

      UnloadService.UnloadData(TestableClientTransaction, orderTicket1.ID);

      CheckDataContainerExists(orderTicket1, false);
      CheckDataContainerExists(order, true);

      // Data reload

      CheckDataContainerExists(orderTicket1, false);

      Assert.That(orderTicket1.FileName, Is.EqualTo("C:\\order1.png"));

      CheckDataContainerExists(orderTicket1, true);
      CheckDataContainerExists(order, true);
    }

    [Test]
    public void UnloadData_OrderTicket_ReloadRelation_OneToOne_FromRealSide ()
    {
      var orderTicket1 = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket>();
      var order = orderTicket1.Order;
      order.EnsureDataAvailable();

      Assert.That(orderTicket1.State.IsUnchanged, Is.True);
      Assert.That(order.State.IsUnchanged, Is.True);

      UnloadService.UnloadData(TestableClientTransaction, orderTicket1.ID);

      CheckEndPointExists(orderTicket1, "Order", false);
      CheckEndPointExists(order, "OrderTicket", false);

      // 1:1 relation reload from real side

      Assert.That(orderTicket1.Order, Is.SameAs(order));

      CheckEndPointExists(orderTicket1, "Order", true);
      CheckVirtualEndPointExistsAndComplete(order, "OrderTicket", true, true);
    }

    [Test]
    public void UnloadData_OrderTicket_ReloadRelation_OneToOne_FromVirtualSide ()
    {
      var orderTicket1 = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket>();
      var order = orderTicket1.Order;
      order.EnsureDataAvailable();

      Assert.That(orderTicket1.State.IsUnchanged, Is.True);
      Assert.That(order.State.IsUnchanged, Is.True);

      UnloadService.UnloadData(TestableClientTransaction, orderTicket1.ID);

      CheckEndPointExists(orderTicket1, "Order", false);
      CheckEndPointExists(order, "OrderTicket", false);

      // 1:1 relation reload from virtual side

      Assert.That(order.OrderTicket, Is.SameAs(orderTicket1));

      CheckEndPointExists(orderTicket1, "Order", true);
      CheckVirtualEndPointExistsAndComplete(order, "OrderTicket", true, true);
    }

    [Test]
    public void UnloadData_Computer_WithUnsynchronizedOppositeEndPoint ()
    {
      var computer1 = DomainObjectIDs.Computer1.GetObject<Computer>();

      var employee = computer1.Employee;
      employee.EnsureDataAvailable();

      var unsynchronizedComputerID =
          RelationInconcsistenciesTestHelper.CreateAndInitializeObjectAndSetRelationInOtherTransaction<Computer, Employee>(employee.ID, (ot, o) =>
          {
            ot.SerialNumber = "12345";
            ot.Employee = o;
          });
      var unsynchronizedComputer = unsynchronizedComputerID.GetObject<Computer>();

      Assert.That(computer1.State.IsUnchanged, Is.True);
      Assert.That(unsynchronizedComputer.State.IsUnchanged, Is.True);
      Assert.That(employee.State.IsUnchanged, Is.True);

      CheckEndPointExists(computer1, "Employee", true);
      CheckEndPointExists(unsynchronizedComputer, "Employee", true);
      CheckVirtualEndPointExistsAndComplete(employee, "Computer", true, true);

      UnloadService.UnloadData(TestableClientTransaction, computer1.ID);

      CheckDataContainerExists(computer1, false);
      CheckDataContainerExists(unsynchronizedComputer, true);
      CheckDataContainerExists(employee, true);

      CheckEndPointExists(computer1, "Employee", false);
      CheckEndPointExists(unsynchronizedComputer, "Employee", true);
      CheckVirtualEndPointExistsAndComplete(employee, "Computer", true, false);

      Assert.That(computer1.State.IsNotLoadedYet, Is.True);
      Assert.That(unsynchronizedComputer.State.IsUnchanged, Is.True);
      Assert.That(employee.State.IsUnchanged, Is.True);
    }

    [Test]
    public void UnloadData_Order ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      var orderItems = order1.OrderItems;
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];
      var orderTicket = order1.OrderTicket;
      var customer = order1.Customer;
      var customerOrders = customer.Orders;
      customerOrders.EnsureDataComplete();

      customer.EnsureDataAvailable();

      Assert.That(order1.State.IsUnchanged, Is.True);
      Assert.That(orderItems.IsDataComplete, Is.True);
      Assert.That(orderItemA.State.IsUnchanged, Is.True);
      Assert.That(orderItemB.State.IsUnchanged, Is.True);
      Assert.That(orderTicket.State.IsUnchanged, Is.True);
      Assert.That(customer.State.IsUnchanged, Is.True);
      Assert.That(customerOrders.IsDataComplete, Is.True);

      UnloadService.UnloadData(TestableClientTransaction, order1.ID);

      CheckDataContainerExists(order1, false);
      CheckDataContainerExists(orderItemA, true);
      CheckDataContainerExists(orderItemB, true);
      CheckDataContainerExists(orderTicket, true);
      CheckDataContainerExists(customer, true);

      CheckEndPointExists(orderTicket, "Order", true);
      CheckEndPointExists(order1, "OrderTicket", true);
      CheckEndPointExists(orderItemA, "Order", true);
      CheckEndPointExists(orderItemB, "Order", true);
      CheckVirtualEndPointExistsAndComplete(order1, "OrderItems", true, true);
      CheckEndPointExists(order1, "Customer", false);
      CheckVirtualEndPointExistsAndComplete(customer, "Orders", true, false);

      Assert.That(order1.State.IsNotLoadedYet, Is.True);
      Assert.That(orderItems.IsDataComplete, Is.True);
      Assert.That(orderItemA.State.IsUnchanged, Is.True);
      Assert.That(orderItemB.State.IsUnchanged, Is.True);
      Assert.That(orderTicket.State.IsUnchanged, Is.True);
      Assert.That(customer.State.IsUnchanged, Is.True);
      Assert.That(customerOrders.IsDataComplete, Is.False);
    }

    [Test]
    public void UnloadData_Order_RelationAccess_OneToMany_FromRealSide ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];

      UnloadService.UnloadData(TestableClientTransaction, order1.ID);

      CheckDataContainerExists(order1, false);
      CheckDataContainerExists(orderItemA, true);
      CheckDataContainerExists(orderItemB, true);

      CheckEndPointExists(orderItemA, "Order", true);
      CheckEndPointExists(orderItemB, "Order", true);
      CheckVirtualEndPointExistsAndComplete(order1, "OrderItems", true, true);

      Assert.That(orderItemA.Order, Is.SameAs(order1));
      Assert.That(orderItemB.Order, Is.SameAs(order1));

      CheckDataContainerExists(order1, false); // Relation access does not reload object
      CheckDataContainerExists(orderItemA, true);
      CheckDataContainerExists(orderItemB, true);

      CheckEndPointExists(orderItemA, "Order", true);
      CheckEndPointExists(orderItemB, "Order", true);
      CheckVirtualEndPointExistsAndComplete(order1, "OrderItems", true, true);
    }

    [Test]
    public void UnloadData_Order_RelationAccess_OneToMany_FromVirtualSide ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];

      UnloadService.UnloadData(TestableClientTransaction, order1.ID);

      CheckDataContainerExists(order1, false);
      CheckDataContainerExists(orderItemA, true);
      CheckDataContainerExists(orderItemB, true);

      CheckEndPointExists(orderItemA, "Order", true);
      CheckEndPointExists(orderItemB, "Order", true);
      CheckVirtualEndPointExistsAndComplete(order1, "OrderItems", true, true);

      Assert.That(order1.OrderItems, Is.EqualTo(new[] { orderItemA, orderItemB }));

      CheckDataContainerExists(order1, true); // Relation access reloads object, although this is not really necessary
      CheckDataContainerExists(orderItemA, true);
      CheckDataContainerExists(orderItemB, true);

      CheckEndPointExists(orderItemA, "Order", true);
      CheckEndPointExists(orderItemB, "Order", true);
      CheckVirtualEndPointExistsAndComplete(order1, "OrderItems", true, true);
    }

    [Test]
    public void UnloadData_OrderItem ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      var orderItems = order1.OrderItems;
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];

      Assert.That(order1.State.IsUnchanged, Is.True);
      Assert.That(orderItems.IsDataComplete, Is.True);
      Assert.That(orderItemA.State.IsUnchanged, Is.True);
      Assert.That(orderItemB.State.IsUnchanged, Is.True);

      UnloadService.UnloadData(TestableClientTransaction, orderItemA.ID);

      CheckDataContainerExists(order1, true);
      CheckDataContainerExists(orderItemA, false);
      CheckDataContainerExists(orderItemB, true);

      CheckEndPointExists(orderItemA, "Order", false);
      CheckEndPointExists(orderItemB, "Order", true);
      CheckVirtualEndPointExistsAndComplete(order1, "OrderItems", true, false);

      Assert.That(order1.State.IsUnchanged, Is.True);
      Assert.That(orderItems.IsDataComplete, Is.False);
      Assert.That(orderItemA.State.IsNotLoadedYet, Is.True);
      Assert.That(orderItemB.State.IsUnchanged, Is.True);
    }

    [Test]
    public void UnloadData_OrderItem_AfterCommit ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      var orderItems = order1.OrderItems;

      var newOrderItem = OrderItem.NewObject();
      newOrderItem.Product = "Product";

      orderItems.Add(newOrderItem);

      TestableClientTransaction.Commit();

      Assert.That(newOrderItem.Order, Is.SameAs(order1));
      Assert.That(newOrderItem.State.IsUnchanged, Is.True);

      UnloadService.UnloadData(TestableClientTransaction, newOrderItem.ID);

      Assert.That(newOrderItem.State.IsNotLoadedYet, Is.True);
      Assert.That(orderItems.IsDataComplete, Is.False);
    }

    [Test]
    public void UnloadData_OrderItem_ReloadRelation_OneToMany_FromRealSide ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];

      UnloadService.UnloadData(TestableClientTransaction, orderItemA.ID);

      CheckDataContainerExists(order1, true);
      CheckDataContainerExists(orderItemA, false);
      CheckDataContainerExists(orderItemB, true);

      CheckEndPointExists(orderItemA, "Order", false);
      CheckEndPointExists(orderItemB, "Order", true);
      CheckVirtualEndPointExistsAndComplete(order1, "OrderItems", true, false);

      Assert.That(orderItemA.Order, Is.SameAs(order1));

      CheckDataContainerExists(order1, true);
      CheckDataContainerExists(orderItemA, true);
      CheckDataContainerExists(orderItemB, true);

      CheckEndPointExists(orderItemA, "Order", true);
      CheckEndPointExists(orderItemB, "Order", true);
      CheckVirtualEndPointExistsAndComplete(order1, "OrderItems", true, false);
    }

    [Test]
    public void UnloadData_OrderItem_ReloadRelation_OneToMany_FromVirtualSide ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];

      UnloadService.UnloadData(TestableClientTransaction, orderItemA.ID);

      CheckDataContainerExists(order1, true);
      CheckDataContainerExists(orderItemA, false);
      CheckDataContainerExists(orderItemB, true);

      CheckEndPointExists(orderItemA, "Order", false);
      CheckEndPointExists(orderItemB, "Order", true);
      CheckVirtualEndPointExistsAndComplete(order1, "OrderItems", true, false);

      Assert.That(order1.OrderItems, Is.EquivalentTo(new[] { orderItemA, orderItemB }));

      CheckDataContainerExists(order1, true);
      CheckDataContainerExists(orderItemA, true);
      CheckDataContainerExists(orderItemB, true);

      CheckEndPointExists(orderItemA, "Order", true);
      CheckEndPointExists(orderItemB, "Order", true);
      CheckVirtualEndPointExistsAndComplete(order1, "OrderItems", true, true);
    }

    [Test]
    public void UnloadData_ReloadChanges_PropertyValue ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var orderInOtherTx = order1.ID.GetObject<Order>();
        orderInOtherTx.OrderNumber = 4711;
        ClientTransaction.Current.Commit();
      }

      Assert.That(order1.OrderNumber, Is.EqualTo(1));

      UnloadService.UnloadData(TestableClientTransaction, order1.ID);

      Assert.That(order1.OrderNumber, Is.EqualTo(4711));
    }

    [Test]
    public void UnloadData_ReloadChanges_ForeignKey ()
    {
      var computer1 = DomainObjectIDs.Computer1.GetObject<Computer>();

      ObjectID newEmployeeID;
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var computerInOtherTx = computer1.ID.GetObject<Computer>();
        computerInOtherTx.Employee = Employee.NewObject();
        computerInOtherTx.Employee.Name = "Employee";

        newEmployeeID = computerInOtherTx.Employee.ID;
        ClientTransaction.Current.Commit();
      }

      Assert.That(computer1.Employee, Is.SameAs(DomainObjectIDs.Employee3.GetObject<Employee>()));

      UnloadService.UnloadData(TestableClientTransaction, computer1.ID);

      Assert.That(computer1.Employee, Is.SameAs(newEmployeeID.GetObject<Employee>()));
    }

    [Test]
    public void UnloadData_AlreadyUnloaded ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      UnloadService.UnloadData(TestableClientTransaction, order1.ID);
      Assert.That(order1.State.IsNotLoadedYet, Is.True);
      Assert.That(TestableClientTransaction.GetEnlistedDomainObject(DomainObjectIDs.Order1), Is.SameAs(order1));

      ClientTransactionTestHelperWithMocks.EnsureTransactionThrowsOnEvents(TestableClientTransaction);

      UnloadService.UnloadData(TestableClientTransaction, order1.ID);

      Assert.That(order1.State.IsNotLoadedYet, Is.True);
    }

    [Test]
    public void UnloadData_New ()
    {
      var orderNew = (Order)LifetimeService.NewObject(TestableClientTransaction, typeof(Order), ParamList.Empty);
      Assert.That(orderNew.State.IsNew, Is.True);
      Assert.That(TestableClientTransaction.GetEnlistedDomainObject(DomainObjectIDs.Order1), Is.Null);

      ClientTransactionTestHelperWithMocks.EnsureTransactionThrowsOnEvents(TestableClientTransaction);

      Assert.That(
          () => UnloadService.UnloadData(TestableClientTransaction, orderNew.ID),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "The state of the following DataContainers prohibits that they be unloaded; only unchanged DataContainers can be unloaded: "
              + string.Format("'Order|{0}|System.Guid' (DataContainerState (New, NewInHierarchy)).", orderNew.ID.Value)));

      Assert.That(orderNew.State.IsNew, Is.True);
    }

    [Test]
    public void UnloadData_NonLoadedObject ()
    {
      ClientTransactionTestHelperWithMocks.EnsureTransactionThrowsOnEvents(TestableClientTransaction);

      UnloadService.UnloadData(TestableClientTransaction, DomainObjectIDs.Order1);

      Assert.That(TestableClientTransaction.GetEnlistedDomainObject(DomainObjectIDs.Order1), Is.Null);
    }

    [Test]
    public void UnloadData_Changed ()
    {
      ++DomainObjectIDs.Order1.GetObject<Order>().OrderNumber;

      Assert.That(
          () => UnloadService.UnloadData(TestableClientTransaction, DomainObjectIDs.Order1),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "The state of the following DataContainers prohibits that they be unloaded; only unchanged DataContainers can be unloaded: "
              + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' (DataContainerState (Changed, PersistentDataChanged))."));

    }

    [Test]
    public void UnloadData_ChangedVirtualEndPoint ()
    {
      var newTicket = OrderTicket.NewObject();

      var domainObject = DomainObjectIDs.Order1.GetObject<Order>();
      domainObject.OrderTicket = newTicket;

      UnloadService.UnloadData(TestableClientTransaction, domainObject.ID);

      Assert.That(domainObject.State.IsNotLoadedYet, Is.True);
      Assert.That(domainObject.OrderTicket, Is.SameAs(newTicket));
    }

    [Test]
    public void UnloadData_ChangedVirtualNullEndPoint ()
    {
      var domainObject = DomainObjectIDs.Employee3.GetObject<Employee>();

      domainObject.Computer = null;

      UnloadService.UnloadData(TestableClientTransaction, domainObject.ID);

      Assert.That(domainObject.State.IsNotLoadedYet, Is.True);
      Assert.That(domainObject.Computer, Is.Null);
    }

    [Test]
    public void UnloadData_ChangedCollection ()
    {
      DomainObjectIDs.OrderItem1.GetObject<OrderItem>().Order.OrderItems.Add(OrderItem.NewObject());
      Assert.That(TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.OrderItem1].State.IsUnchanged, Is.True);
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "OrderItems");
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(endPointID).HasChanged, Is.True);
      Assert.That(
          () => UnloadService.UnloadData(TestableClientTransaction, DomainObjectIDs.OrderItem1),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The relations of object 'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid' cannot be unloaded.\r\n"
                  + "The opposite relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems' of relation end-point "
                  + "'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order' has "
                  + "changed. Non-virtual end-points that are part of changed relations cannot be unloaded."));
    }

    [Test]
    public void ReadingValueProperties_ReloadsObject ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      UnloadService.UnloadData(TestableClientTransaction, order1.ID);
      Assert.That(order1.State.IsNotLoadedYet, Is.True);

      var listenerMock = ClientTransactionTestHelperWithMocks.CreateAndAddListenerMock(TestableClientTransaction);

      Dev.Null = order1.OrderNumber;

      AssertObjectWasLoaded(listenerMock, order1);
      Assert.That(order1.State.IsUnchanged, Is.True);
    }

    [Test]
    public void WritingValueProperties_ReloadsObject ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      UnloadService.UnloadData(TestableClientTransaction, order1.ID);
      Assert.That(order1.State.IsNotLoadedYet, Is.True);

      var listenerMock = ClientTransactionTestHelperWithMocks.CreateAndAddListenerMock(TestableClientTransaction);

      order1.OrderNumber = 4711;

      AssertObjectWasLoaded(listenerMock, order1);
      Assert.That(order1.State.IsChanged, Is.True);
    }

    [Test]
    public void ReadingStateProperties_DoesNotReloadObject ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      UnloadService.UnloadData(TestableClientTransaction, order1.ID);
      Assert.That(order1.State.IsNotLoadedYet, Is.True);

      EnsureTransactionThrowsOnLoad();

      Dev.Null = order1.ID;
      Dev.Null = order1.State;
      Dev.Null = order1.RootTransaction;

      Assert.That(order1.State.IsNotLoadedYet, Is.True);
    }

    [Test]
    public void ReadingTimestamp_ReloadsObject ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      UnloadService.UnloadData(TestableClientTransaction, order1.ID);
      Assert.That(order1.State.IsNotLoadedYet, Is.True);

      var listenerMock = ClientTransactionTestHelperWithMocks.CreateAndAddListenerMock(TestableClientTransaction);

      Dev.Null = order1.Timestamp;

      AssertObjectWasLoaded(listenerMock, order1);
      Assert.That(order1.State.IsUnchanged, Is.True);
    }

    [Test]
    public void RegisterForCommit_ReloadsObject ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      UnloadService.UnloadData(TestableClientTransaction, order1.ID);
      Assert.That(order1.State.IsNotLoadedYet, Is.True);

      var listenerMock = ClientTransactionTestHelperWithMocks.CreateAndAddListenerMock(TestableClientTransaction);

      order1.RegisterForCommit();

      AssertObjectWasLoaded(listenerMock, order1);
      Assert.That(order1.State.IsChanged, Is.True);
    }

    [Test]
    public void EnsureDataAvailable_ReloadsObject ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      UnloadService.UnloadData(TestableClientTransaction, order1.ID);
      Assert.That(order1.State.IsNotLoadedYet, Is.True);

      var listenerMock = ClientTransactionTestHelperWithMocks.CreateAndAddListenerMock(TestableClientTransaction);

      order1.EnsureDataAvailable();

      AssertObjectWasLoaded(listenerMock, order1);
      Assert.That(order1.State.IsUnchanged, Is.True);
    }

    [Test]
    public void ReadingPropertyAccessor_DoesNotReloadObject ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      UnloadService.UnloadData(TestableClientTransaction, order1.ID);
      Assert.That(order1.State.IsNotLoadedYet, Is.True);

      EnsureTransactionThrowsOnLoad();

      order1.PreparePropertyAccess(typeof(Order).FullName + ".OrderNumber");
      Dev.Null = order1.CurrentProperty;
      order1.PropertyAccessFinished();

      Dev.Null = order1.Properties;

      Assert.That(order1.State.IsNotLoadedYet, Is.True);
    }

    [Test]
    public void ReadingTransactionContext_DoesNotReloadObject ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      UnloadService.UnloadData(TestableClientTransaction, order1.ID);
      Assert.That(order1.State.IsNotLoadedYet, Is.True);

      EnsureTransactionThrowsOnLoad();

      Dev.Null = order1.DefaultTransactionContext;
      Dev.Null = order1.TransactionContext;

      Assert.That(order1.State.IsNotLoadedYet, Is.True);
    }

    [Test]
    public void ReadingCollectionEndPoint_DoesNotReloadObject ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      var customer = order1.Customer;
      var customerOrders = customer.Orders;
      customerOrders.EnsureDataComplete();

      UnloadService.UnloadData(TestableClientTransaction, order1.ID);

      EnsureTransactionThrowsOnLoad();

      Assert.That(order1.State.IsNotLoadedYet, Is.True);
      Assert.That(customerOrders.IsDataComplete, Is.False);

      Assert.That(customer.Orders, Is.SameAs(customerOrders)); // does not reload the object or the relation

      Assert.That(order1.State.IsNotLoadedYet, Is.True);
      Assert.That(customerOrders.IsDataComplete, Is.False);
    }

    [Test]
    public void ChangingCollectionEndPoint_ReloadsCollectionAndObject ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      var customer = order1.Customer;
      var customerOrders = customer.Orders;

      UnloadService.UnloadData(TestableClientTransaction, order1.ID);

      Assert.That(order1.State.IsNotLoadedYet, Is.True);
      Assert.That(customerOrders.IsDataComplete, Is.False);

      var listenerMock = ClientTransactionTestHelperWithMocks.CreateAndAddListenerMock(TestableClientTransaction);

      customer.Orders.Add(Order.NewObject()); // reloads the relation contents and thus the object

      AssertObjectWasLoadedAmongOthers(listenerMock, order1);

      Assert.That(order1.State.IsUnchanged, Is.True);
      Assert.That(customerOrders.IsDataComplete, Is.True);
    }

    [Test]
    [Ignore("TODO 2264")]
    public void ReadingVirtualRelationEndPoints_DoesNotReloadObject ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      var orderItems = order1.OrderItems;
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];
      var orderTicket = order1.OrderTicket;

      UnloadService.UnloadData(TestableClientTransaction, order1.ID);

      EnsureTransactionThrowsOnLoad();

      Assert.That(order1.State.IsNotLoadedYet, Is.True);

      Assert.That(order1.OrderTicket, Is.SameAs(orderTicket)); // does not reload the object
      Assert.That(orderTicket.Order, Is.SameAs(order1)); // does not reload the object
      Assert.That(order1.OrderItems, Is.SameAs(orderItems)); // does not reload the object
      Assert.That(order1.OrderItems, Is.EquivalentTo(new[] { orderItemA, orderItemB })); // does not reload the object
      Assert.That(orderItemA.Order, Is.SameAs(order1)); // does not reload the object
      Assert.That(orderItemB.Order, Is.SameAs(order1)); // does not reload the object

      Assert.That(order1.State.IsNotLoadedYet, Is.True);
    }

    [Test]
    [Ignore("TODO 2264")]
    public void ReadingOriginalVirtualRelationEndPoints_DoesNotReloadObject ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];
      var orderTicket = order1.OrderTicket;

      UnloadService.UnloadData(TestableClientTransaction, order1.ID);

      EnsureTransactionThrowsOnLoad();

      Assert.That(order1.State.IsNotLoadedYet, Is.True);

      Assert.That(order1.Properties.Find("OrderTicket").GetOriginalValueWithoutTypeCheck(), Is.SameAs(orderTicket)); // does not reload the object
      Assert.That(orderTicket.Properties.Find("Order").GetOriginalValueWithoutTypeCheck(), Is.SameAs(order1)); // does not reload the object
      Assert.That(order1.Properties.Find("OrderItems").GetOriginalValueWithoutTypeCheck(), Is.EquivalentTo(new[] { orderItemA, orderItemB })); // does not reload the object
      Assert.That(orderItemA.Properties.Find("Order").GetOriginalValueWithoutTypeCheck(), Is.SameAs(order1)); // does not reload the object
      Assert.That(orderItemB.Properties.Find("Order").GetOriginalValueWithoutTypeCheck(), Is.SameAs(order1)); // does not reload the object

      Assert.That(order1.State.IsNotLoadedYet, Is.True);
    }

    [Test]
    [Ignore("TODO 2263")]
    public void ChangingVirtualRelationEndPoints_DoesNotReloadObject ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];
      var orderTicket = order1.OrderTicket;

      UnloadService.UnloadData(TestableClientTransaction, order1.ID);

      EnsureTransactionThrowsOnLoad();

      Assert.That(order1.State.IsNotLoadedYet, Is.True);

      order1.OrderTicket = OrderTicket.NewObject(); // does not reload the object
      Assert.That(orderTicket.Order, Is.Null);

      order1.OrderItems.Add(OrderItem.NewObject()); // does not reload the object
      order1.OrderItems = new ObjectList<OrderItem>(new[] { orderItemA }); // does not reload the object
      Assert.That(orderItemA.Order, Is.SameAs(order1));
      Assert.That(orderItemB.Order, Is.Null);

      Assert.That(order1.State.IsNotLoadedYet, Is.True);
    }

    [Test]
    [Ignore("TODO 2263")]
    public void ChangingRealRelationEndPoints_DoesNotReloadOppositeObjects ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      var orderItemA = order1.OrderItems[0];
      var orderTicket = order1.OrderTicket;

      UnloadService.UnloadData(TestableClientTransaction, order1.ID);

      EnsureTransactionThrowsOnLoad();

      orderTicket.Order = Order.NewObject();
      Assert.That(order1.State.IsNotLoadedYet, Is.True);

      orderItemA.Order = Order.NewObject();
      Assert.That(order1.State.IsNotLoadedYet, Is.True);
    }

    [Test]
    public void ReadingRealRelationEndPoints_ReloadsObject ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      var customer = order1.Customer;

      UnloadService.UnloadData(TestableClientTransaction, order1.ID);
      var listenerMock = ClientTransactionTestHelperWithMocks.CreateAndAddListenerMock(TestableClientTransaction);
      Assert.That(order1.State.IsNotLoadedYet, Is.True);

      Assert.That(order1.Customer, Is.SameAs(customer)); // reloads the object because the foreign key is stored in order1

      AssertObjectWasLoaded(listenerMock, order1);

      Assert.That(order1.State.IsUnchanged, Is.True);
    }

    [Test]
    public void ChangingRealRelationEndPoints_ReloadsObject ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();

      UnloadService.UnloadData(TestableClientTransaction, order1.ID);
      var listenerMock = ClientTransactionTestHelperWithMocks.CreateAndAddListenerMock(TestableClientTransaction);
      Assert.That(order1.State.IsNotLoadedYet, Is.True);

      order1.Customer = Customer.NewObject(); // reloads the object because the foreign key is stored in order1

      AssertObjectWasLoaded(listenerMock, order1);
      Assert.That(order1.State.IsChanged, Is.True);
    }

    [Test]
    public void ReadingOppositeCollectionEndPoints_ReloadsObject ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      var customer = order1.Customer;

      UnloadService.UnloadData(TestableClientTransaction, order1.ID);
      var listenerMock = ClientTransactionTestHelperWithMocks.CreateAndAddListenerMock(TestableClientTransaction);
      Assert.That(order1.State.IsNotLoadedYet, Is.True);

      Assert.That(customer.Orders, Has.Member(order1)); // enumerating reloads the relation contents because the foreign key is stored in order1

      AssertObjectWasLoadedAmongOthers(listenerMock, order1);
    }

    [Test]
    [Ignore("TODO 2263")]
    public void AddingToCollectionEndPoint_DoesntReloadOtherItems ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      var customer = order1.Customer;
      Console.WriteLine(customer.State);

      UnloadService.UnloadData(TestableClientTransaction, order1.ID);
      Assert.That(order1.State.IsNotLoadedYet, Is.True);

      Console.WriteLine(customer.State);
      EnsureTransactionThrowsOnLoad();

      customer.Orders.Add(Order.NewObject()); // does not reload order1 because that object's foreign key is not involved

      Assert.That(order1.State.IsNotLoadedYet, Is.True);
    }

    [Test]
    public void AddingToCollectionEndPoint_ReloadsObjectBeingAdded ()
    {
      var customer = DomainObjectIDs.Customer1.GetObject<Customer>();
      var order3 = DomainObjectIDs.Order3.GetObject<Order>();

      UnloadService.UnloadData(TestableClientTransaction, order3.ID);
      Assert.That(order3.State.IsNotLoadedYet, Is.True);

      var listenerMock = ClientTransactionTestHelperWithMocks.CreateAndAddListenerMock(TestableClientTransaction);

      customer.Orders.Add(order3); // reloads order3 because order3's foreign key is changed

      AssertObjectWasLoaded(listenerMock, order3);
      Assert.That(order3.State.IsChanged, Is.True);
    }

    [Test]
    public void Commit_DoesNotReloadObjectOrCollection ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      var customerOrders = order1.Customer.Orders;

      UnloadService.UnloadData(TestableClientTransaction, order1.ID);

      Assert.That(order1.State.IsNotLoadedYet, Is.True);
      Assert.That(customerOrders.IsDataComplete, Is.False);

      EnsureTransactionThrowsOnLoad();

      TestableClientTransaction.Commit();

      Assert.That(order1.State.IsNotLoadedYet, Is.True);
      Assert.That(customerOrders.IsDataComplete, Is.False);
    }

    [Test]
    public void Rollback_DoesNotReloadObjectOrCollection ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      var customerOrders = order1.Customer.Orders;

      UnloadService.UnloadData(TestableClientTransaction, order1.ID);

      Assert.That(order1.State.IsNotLoadedYet, Is.True);
      Assert.That(customerOrders.IsDataComplete, Is.False);

      EnsureTransactionThrowsOnLoad();

      TestableClientTransaction.Rollback();

      Assert.That(order1.State.IsNotLoadedYet, Is.True);
      Assert.That(customerOrders.IsDataComplete, Is.False);
    }
  }
}
