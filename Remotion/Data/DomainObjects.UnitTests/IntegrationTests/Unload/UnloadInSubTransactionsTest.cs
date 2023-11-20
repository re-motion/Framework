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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Unload
{
  [TestFixture]
  public class UnloadInSubTransactionsTest : ClientTransactionBaseTest
  {
    private ClientTransaction _subTransaction;

    public override void SetUp ()
    {
      base.SetUp();

      _subTransaction = TestableClientTransaction.CreateSubTransaction();
      _subTransaction.EnterDiscardingScope();
    }

    public override void TearDown ()
    {
      ClientTransactionScope.ActiveScope.Leave();

      base.TearDown();
    }

    [Test]
    public void UnloadVirtualEndPoint_Collection ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var orderItems = order.OrderItems;
      orderItems.EnsureDataComplete();
      var orderItem1 = DomainObjectIDs.OrderItem1.GetObject<OrderItem>();
      var orderItem2 = DomainObjectIDs.OrderItem2.GetObject<OrderItem>();

      Assert.That(orderItems.IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPoint(_subTransaction, orderItems.AssociatedEndPointID);

      CheckDataContainerExists(_subTransaction, order, true);
      CheckDataContainerExists(_subTransaction, orderItem1, true);
      CheckDataContainerExists(_subTransaction, orderItem2, true);

      CheckEndPointExists(_subTransaction, orderItem1, "Order", true);
      CheckEndPointExists(_subTransaction, orderItem2, "Order", true);
      CheckVirtualEndPoint(_subTransaction, order, "OrderItems", false);

      CheckDataContainerExists(_subTransaction.ParentTransaction, order, true);
      CheckDataContainerExists(_subTransaction.ParentTransaction, orderItem1, true);
      CheckDataContainerExists(_subTransaction.ParentTransaction, orderItem2, true);

      CheckEndPointExists(_subTransaction.ParentTransaction, orderItem1, "Order", true);
      CheckEndPointExists(_subTransaction.ParentTransaction, orderItem2, "Order", true);
      CheckVirtualEndPoint(_subTransaction.ParentTransaction, order, "OrderItems", false);

      Assert.That(order.State.IsUnchanged, Is.True);
      Assert.That(orderItem1.State.IsUnchanged, Is.True);
      Assert.That(orderItem2.State.IsUnchanged, Is.True);

      Assert.That(order.TransactionContext[_subTransaction.ParentTransaction].State.IsUnchanged, Is.True);
      Assert.That(orderItem1.TransactionContext[_subTransaction.ParentTransaction].State.IsUnchanged, Is.True);
      Assert.That(orderItem2.TransactionContext[_subTransaction.ParentTransaction].State.IsUnchanged, Is.True);
    }

    [Test]
    public void UnloadVirtualEndPoint_Collection_EnsureDataComplete ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var orderItems = order.OrderItems;
      orderItems.EnsureDataComplete();

      Assert.That(orderItems.IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPoint(_subTransaction, orderItems.AssociatedEndPointID);

      CheckVirtualEndPoint(_subTransaction, order, "OrderItems", false);
      CheckVirtualEndPoint(_subTransaction.ParentTransaction, order, "OrderItems", false);

      orderItems.EnsureDataComplete();

      CheckVirtualEndPoint(_subTransaction, order, "OrderItems", true);
      CheckVirtualEndPoint(_subTransaction.ParentTransaction, order, "OrderItems", true);
    }

    [Test]
    public void UnloadVirtualEndPoint_Collection_ChangedInParent_ButNotInSubTx ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var orderItems = order.OrderItems;
      orderItems.EnsureDataComplete();

      Assert.That(orderItems.Count, Is.EqualTo(2));
      orderItems.Add(OrderItem.NewObject());
      Assert.That(orderItems.Count, Is.EqualTo(3));
      _subTransaction.Commit();

      Assert.That(order.State.IsUnchanged, Is.True);
      Assert.That(order.TransactionContext[_subTransaction.ParentTransaction].State.IsChanged, Is.True);

      try
      {
        UnloadService.UnloadVirtualEndPoint(_subTransaction, orderItems.AssociatedEndPointID);
        Assert.Fail("Expected InvalidOperationException");
      }
      catch (InvalidOperationException ex)
      {
        Assert.That(ex.Message, Is.EqualTo(
            "The end point with ID "
            + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems' "
            + "has been changed. Changed end points cannot be unloaded."));
      }

      CheckVirtualEndPoint(_subTransaction, order, "OrderItems", true);
      CheckVirtualEndPoint(_subTransaction.ParentTransaction, order, "OrderItems", true);

      Assert.That(orderItems.Count, Is.EqualTo(3));
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

      UnloadService.UnloadVirtualEndPoint(_subTransaction, orderItems.AssociatedEndPointID);

      Assert.That(orderItems, Is.EquivalentTo(new[] { orderItem1, orderItem2, newOrderItemID.GetObject<OrderItem>() }));
    }

    [Test]
    public void UnloadVirtualEndPoint_VirtualEndPoint ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var orderTicket = order.OrderTicket;

      CheckVirtualEndPoint(_subTransaction, order, "OrderTicket", true);

      UnloadService.UnloadVirtualEndPoint(_subTransaction, RelationEndPointID.Resolve(order, o => o.OrderTicket));

      CheckDataContainerExists(_subTransaction, order, true);
      CheckDataContainerExists(_subTransaction, orderTicket, true);

      CheckEndPointExists(_subTransaction, orderTicket, "Order", true);
      CheckVirtualEndPoint(_subTransaction, order, "OrderTicket", false);

      CheckDataContainerExists(_subTransaction.ParentTransaction, order, true);
      CheckDataContainerExists(_subTransaction.ParentTransaction, orderTicket, true);

      CheckEndPointExists(_subTransaction.ParentTransaction, orderTicket, "Order", true);
      CheckVirtualEndPoint(_subTransaction.ParentTransaction, order, "OrderTicket", false);

      Assert.That(order.State.IsUnchanged, Is.True);
      Assert.That(orderTicket.State.IsUnchanged, Is.True);

      Assert.That(order.TransactionContext[_subTransaction.ParentTransaction].State.IsUnchanged, Is.True);
      Assert.That(orderTicket.TransactionContext[_subTransaction.ParentTransaction].State.IsUnchanged, Is.True);
    }

    [Test]
    public void UnloadVirtualEndPoint_ObjectEndPoint_EnsureDataComplete ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      _subTransaction.EnsureDataComplete(RelationEndPointID.Resolve(order, o => o.OrderTicket));

      CheckVirtualEndPoint(_subTransaction, order, "OrderTicket", true);

      UnloadService.UnloadVirtualEndPoint(_subTransaction, RelationEndPointID.Resolve(order, o => o.OrderTicket));

      CheckVirtualEndPoint(_subTransaction, order, "OrderTicket", false);
      CheckVirtualEndPoint(_subTransaction.ParentTransaction, order, "OrderTicket", false);

      _subTransaction.EnsureDataComplete(RelationEndPointID.Resolve(order, o => o.OrderTicket));

      CheckVirtualEndPoint(_subTransaction, order, "OrderTicket", true);
      CheckVirtualEndPoint(_subTransaction.ParentTransaction, order, "OrderTicket", true);
    }

    [Test]
    public void UnloadVirtualEndPoint_ObjectEndPoint_ChangedInParent_ButNotInSubTx ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var oldOrderTicket = order.OrderTicket;

      oldOrderTicket.Delete();
      var newOrderTicket = OrderTicket.NewObject();
      order.OrderTicket = newOrderTicket;

      _subTransaction.Commit();

      Assert.That(order.State.IsUnchanged, Is.True);
      Assert.That(order.TransactionContext[_subTransaction.ParentTransaction].State.IsChanged, Is.True);

      try
      {
        UnloadService.UnloadVirtualEndPoint(_subTransaction, RelationEndPointID.Resolve(order, o => o.OrderTicket));
        Assert.Fail("Expected InvalidOperationException");
      }
      catch (InvalidOperationException ex)
      {
        Assert.That(ex.Message, Is.EqualTo(
            "The end point with ID "
            + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' "
            + "has been changed. Changed end points cannot be unloaded."));
      }

      CheckVirtualEndPoint(_subTransaction, order, "OrderTicket", true);
      CheckVirtualEndPoint(_subTransaction.ParentTransaction, order, "OrderTicket", true);

      Assert.That(order.OrderTicket, Is.SameAs(newOrderTicket));
    }

    [Test]
    public void UnloadVirtualEndPoint_ObjectEndPoint_Reload ()
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

      UnloadService.UnloadVirtualEndPoint(_subTransaction, RelationEndPointID.Resolve(order, o => o.OrderTicket));

      Assert.That(order.OrderTicket, Is.SameAs(newOrderTicketID.GetObject<OrderTicket>()));
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

      Assert.That(order1.State.IsUnchanged, Is.True);
      Assert.That(orderItems.IsDataComplete, Is.True);
      Assert.That(orderItemA.State.IsUnchanged, Is.True);
      Assert.That(orderItemB.State.IsUnchanged, Is.True);
      Assert.That(orderTicket.State.IsUnchanged, Is.True);
      Assert.That(customer.State.IsUnchanged, Is.True);
      Assert.That(customerOrders.IsDataComplete, Is.True);

      UnloadService.UnloadData(_subTransaction, order1.ID);

      CheckDataContainerExists(_subTransaction, order1, false);
      CheckDataContainerExists(_subTransaction, orderItemA, true);
      CheckDataContainerExists(_subTransaction, orderItemB, true);
      CheckDataContainerExists(_subTransaction, orderTicket, true);
      CheckDataContainerExists(_subTransaction, customer, true);

      CheckEndPointExists(_subTransaction, orderTicket, "Order", true);
      CheckEndPointExists(_subTransaction, order1, "OrderTicket", true);
      CheckEndPointExists(_subTransaction, orderItemA, "Order", true);
      CheckEndPointExists(_subTransaction, orderItemB, "Order", true);
      CheckVirtualEndPoint(_subTransaction, order1, "OrderItems", true);
      CheckEndPointExists(_subTransaction, order1, "Customer", false);
      CheckVirtualEndPoint(_subTransaction, customer, "Orders", false);

      CheckDataContainerExists(_subTransaction.ParentTransaction, order1, false);
      CheckDataContainerExists(_subTransaction.ParentTransaction, orderItemA, true);
      CheckDataContainerExists(_subTransaction.ParentTransaction, orderItemB, true);
      CheckDataContainerExists(_subTransaction.ParentTransaction, orderTicket, true);
      CheckDataContainerExists(_subTransaction.ParentTransaction, customer, true);

      CheckEndPointExists(_subTransaction.ParentTransaction, orderTicket, "Order", true);
      CheckEndPointExists(_subTransaction.ParentTransaction, order1, "OrderTicket", true);
      CheckEndPointExists(_subTransaction.ParentTransaction, orderItemA, "Order", true);
      CheckEndPointExists(_subTransaction.ParentTransaction, orderItemB, "Order", true);
      CheckVirtualEndPoint(_subTransaction.ParentTransaction, order1, "OrderItems", true);
      CheckEndPointExists(_subTransaction.ParentTransaction, order1, "Customer", false);
      CheckVirtualEndPoint(_subTransaction.ParentTransaction, customer, "Orders", false);

      Assert.That(order1.State.IsNotLoadedYet, Is.True);
      Assert.That(orderItems.IsDataComplete, Is.True);
      Assert.That(orderItemA.State.IsUnchanged, Is.True);
      Assert.That(orderItemB.State.IsUnchanged, Is.True);
      Assert.That(orderTicket.State.IsUnchanged, Is.True);
      Assert.That(customer.State.IsUnchanged, Is.True);
      Assert.That(customerOrders.IsDataComplete, Is.False);

      Assert.That(order1.TransactionContext[_subTransaction.ParentTransaction].State.IsNotLoadedYet, Is.True);
      Assert.That(orderItemA.TransactionContext[_subTransaction.ParentTransaction].State.IsUnchanged, Is.True);
      Assert.That(orderItemB.TransactionContext[_subTransaction.ParentTransaction].State.IsUnchanged, Is.True);
      Assert.That(orderTicket.TransactionContext[_subTransaction.ParentTransaction].State.IsUnchanged, Is.True);
      Assert.That(customer.TransactionContext[_subTransaction.ParentTransaction].State.IsUnchanged, Is.True);
    }

    [Test]
    public void UnloadData_Reload_PropertyValue ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var orderInOtherTx = order1.ID.GetObject<Order>();
        orderInOtherTx.OrderNumber = 4711;
        ClientTransaction.Current.Commit();
      }

      Assert.That(order1.OrderNumber, Is.EqualTo(1));

      UnloadService.UnloadData(_subTransaction, order1.ID);

      Assert.That(order1.OrderNumber, Is.EqualTo(4711));
    }

    [Test]
    public void UnloadData_Reload_ForeignKey ()
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

      UnloadService.UnloadData(_subTransaction, computer1.ID);

      Assert.That(computer1.Employee, Is.SameAs(newEmployeeID.GetObject<Employee>()));
    }

    [Test]
    public void UnloadData_Changed_InParentTransaction_ButNotInSubTransaction ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      order1.OrderNumber = 4711;
      _subTransaction.Commit();

      Assert.That(order1.State.IsUnchanged, Is.True);
      Assert.That(order1.TransactionContext[_subTransaction.ParentTransaction].State.IsChanged, Is.True);

      try
      {
        UnloadService.UnloadData(_subTransaction, DomainObjectIDs.Order1);
        Assert.Fail("Expected InvalidOperationException");
      }
      catch (InvalidOperationException ex)
      {
        Assert.That(ex.Message, Is.EqualTo(
            "The state of the following DataContainers prohibits that they be unloaded; only unchanged DataContainers can be unloaded: "
            + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' (DataContainerState (Changed, PersistentDataChanged))."));
      }

      Assert.That(order1.State.IsUnchanged, Is.True);
      Assert.That(order1.TransactionContext[_subTransaction.ParentTransaction].State.IsChanged, Is.True);

      Assert.That(order1.OrderNumber, Is.EqualTo(4711));
    }

    [Test]
    public void UnloadData_New_InSubTransaction_ButNotInParentTransaction ()
    {
      var orderNew = (Order)LifetimeService.NewObject(_subTransaction, typeof(Order), ParamList.Empty);
      orderNew.OrderNumber = 4711;

      Assert.That(orderNew.State.IsNew, Is.True);
      Assert.That(orderNew.TransactionContext[_subTransaction.ParentTransaction].State.IsInvalid, Is.True);

      try
      {
        UnloadService.UnloadData(_subTransaction, orderNew.ID);
        Assert.Fail("Expected InvalidOperationException");
      }
      catch (InvalidOperationException ex)
      {
        Assert.That(ex.Message, Is.EqualTo(
            "The state of the following DataContainers prohibits that they be unloaded; only unchanged DataContainers can be unloaded: "
            + string.Format("'Order|{0}|System.Guid' (DataContainerState (New, PersistentDataChanged, NewInHierarchy)).", orderNew.ID.Value)));
      }

      Assert.That(orderNew.State.IsNew, Is.True);
      Assert.That(orderNew.TransactionContext[_subTransaction.ParentTransaction].State.IsInvalid, Is.True);

      Assert.That(orderNew.OrderNumber, Is.EqualTo(4711));
    }

    [Test]
    public void UnloadCollectionEndPointAndData_ForCollection ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var orderItems = order.OrderItems;
      orderItems.EnsureDataComplete();
      var orderItem1 = DomainObjectIDs.OrderItem1.GetObject<OrderItem>();
      var orderItem2 = DomainObjectIDs.OrderItem2.GetObject<OrderItem>();

      Assert.That(orderItems.IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPointAndItemData(_subTransaction, orderItems.AssociatedEndPointID);

      CheckDataContainerExists(_subTransaction, order, true);
      CheckDataContainerExists(_subTransaction, orderItem1, false);
      CheckDataContainerExists(_subTransaction, orderItem2, false);

      CheckEndPointExists(_subTransaction, order, "OrderItems", false);
      CheckEndPointExists(_subTransaction, orderItem1, "Order", false);
      CheckEndPointExists(_subTransaction, orderItem2, "Order", false);

      CheckDataContainerExists(_subTransaction.ParentTransaction, order, true);
      CheckDataContainerExists(_subTransaction.ParentTransaction, orderItem1, false);
      CheckDataContainerExists(_subTransaction.ParentTransaction, orderItem2, false);

      CheckEndPointExists(_subTransaction.ParentTransaction, order, "OrderItems", false);
      CheckEndPointExists(_subTransaction.ParentTransaction, orderItem1, "Order", false);
      CheckEndPointExists(_subTransaction.ParentTransaction, orderItem2, "Order", false);

      Assert.That(order.State.IsUnchanged, Is.True);
      Assert.That(orderItem1.State.IsNotLoadedYet, Is.True);
      Assert.That(orderItem2.State.IsNotLoadedYet, Is.True);

      Assert.That(order.TransactionContext[_subTransaction.ParentTransaction].State.IsUnchanged, Is.True);
      Assert.That(orderItem1.TransactionContext[_subTransaction.ParentTransaction].State.IsNotLoadedYet, Is.True);
      Assert.That(orderItem2.TransactionContext[_subTransaction.ParentTransaction].State.IsNotLoadedYet, Is.True);
    }

    [Test]
    public void UnloadCollectionEndPointAndData_ForObject_WithReferencedObjectBeingLoadedViaGetObject ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var orderTicket = order.OrderTicket;

      UnloadService.UnloadVirtualEndPointAndItemData(ClientTransaction.Current, RelationEndPointID.Resolve(order, o => o.OrderTicket));

      CheckDataContainerExists(_subTransaction, order, true);
      CheckDataContainerExists(_subTransaction, orderTicket, false);

      CheckEndPointExists(_subTransaction, order, "OrderTicket", false);
      CheckEndPointExists(_subTransaction, orderTicket, "Order", false);

      CheckDataContainerExists(_subTransaction.ParentTransaction, order, true);
      CheckDataContainerExists(_subTransaction.ParentTransaction, orderTicket, false);

      CheckEndPointExists(_subTransaction.ParentTransaction, order, "OrderTicket", false);
      CheckEndPointExists(_subTransaction.ParentTransaction, orderTicket, "Order", false);

      Assert.That(order.State.IsUnchanged, Is.True);
      Assert.That(orderTicket.State.IsNotLoadedYet, Is.True);

      Assert.That(order.TransactionContext[_subTransaction.ParentTransaction].State.IsUnchanged, Is.True);
      Assert.That(orderTicket.TransactionContext[_subTransaction.ParentTransaction].State.IsNotLoadedYet, Is.True);

      // This check can likely be removed when the above assertions pass
      Assert.That(order.OrderTicket, Is.SameAs(orderTicket));
    }

    [Test]
    [Ignore("RM-7236")]
    public void UnloadCollectionEndPointAndData_ForObject_WithReferencingObjectBeingLoadedViaGetObject ()
    {
      // It is required to load the OrderTicket via GetObject and then the Order via OrderTicket.Order to trigger the error.
      var orderTicket = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket>();
      var order = orderTicket.Order;
      order.EnsureDataAvailable();

      UnloadService.UnloadVirtualEndPointAndItemData(ClientTransaction.Current, RelationEndPointID.Resolve(order, o => o.OrderTicket));

      CheckDataContainerExists(_subTransaction, order, true);
      CheckDataContainerExists(_subTransaction, orderTicket, false);

      CheckEndPointExists(_subTransaction, order, "OrderTicket", false);
      CheckEndPointExists(_subTransaction, orderTicket, "Order", false);

      CheckDataContainerExists(_subTransaction.ParentTransaction, order, true);
      CheckDataContainerExists(_subTransaction.ParentTransaction, orderTicket, false);

      CheckEndPointExists(_subTransaction.ParentTransaction, order, "OrderTicket", false);
      CheckEndPointExists(_subTransaction.ParentTransaction, orderTicket, "Order", false);

      Assert.That(order.State.IsUnchanged, Is.True);
      Assert.That(orderTicket.State.IsNotLoadedYet, Is.True);

      Assert.That(order.TransactionContext[_subTransaction.ParentTransaction].State.IsUnchanged, Is.True);
      Assert.That(orderTicket.TransactionContext[_subTransaction.ParentTransaction].State.IsNotLoadedYet, Is.True);

      // This check can likely be removed when the above assertions pass
      Assert.That(order.OrderTicket, Is.SameAs(orderTicket));
    }

    [Test]
    public void UnloadCollectionEndPointAndData_Reload ()
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
        var orderItem1InOtherTx = DomainObjectIDs.OrderItem1.GetObject<OrderItem>();
        var newOrderItem = OrderItem.NewObject();
        newOrderItem.Product = "Product";
        newOrderItemID = newOrderItem.ID;
        orderInOtherTx.OrderItems.Add(newOrderItem);
        orderInOtherTx.OrderItems.Remove(orderItem1InOtherTx);

        orderItem1InOtherTx.Order = DomainObjectIDs.Order3.GetObject<Order>();

        ClientTransaction.Current.Commit();
      }

      Assert.That(orderItems, Is.EquivalentTo(new[] { orderItem1, orderItem2 }));

      UnloadService.UnloadVirtualEndPointAndItemData(_subTransaction, orderItems.AssociatedEndPointID);

      Assert.That(orderItems, Is.EquivalentTo(new[] { orderItem2, newOrderItemID.GetObject<OrderItem>() }));
      Assert.That(orderItem1.Order, Is.SameAs(DomainObjectIDs.Order3.GetObject<Order>()));
    }

    [Test]
    public void Events ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      order1.OrderItems.EnsureDataComplete();
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];

      var endPointID = order1.OrderItems.AssociatedEndPointID;
      var oppositeEndPointIDA = RelationEndPointID.Create(orderItemA.ID, endPointID.Definition.GetOppositeEndPointDefinition());
      var oppositeEndPointIDB = RelationEndPointID.Create(orderItemB.ID, endPointID.Definition.GetOppositeEndPointDefinition());

      var rootTransaction = _subTransaction.ParentTransaction;

      var subListenerMock = new Mock<IClientTransactionListener>(MockBehavior.Strict);
      var rootListenerMock = new Mock<IClientTransactionListener>(MockBehavior.Strict);

      ClientTransactionTestHelper.AddListener(_subTransaction, subListenerMock.Object);
      ClientTransactionTestHelper.AddListener(rootTransaction, rootListenerMock.Object);

      try
      {
        var sequence = new VerifiableSequence();
        subListenerMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.ObjectsUnloading(_subTransaction, new[] { orderItemA, orderItemB }))
            .Verifiable();
        rootListenerMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.ObjectsUnloading(rootTransaction, new[] { orderItemA, orderItemB }))
            .Callback(
                (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> unloadedDomainObjects) =>
                {
                  Assert.That(orderItemA.TransactionContext[_subTransaction].State.IsUnchanged, Is.True);
                  Assert.That(orderItemB.TransactionContext[_subTransaction].State.IsUnchanged, Is.True);

                  Assert.That(orderItemA.TransactionContext[rootTransaction].State.IsUnchanged, Is.True);
                  Assert.That(orderItemB.TransactionContext[rootTransaction].State.IsUnchanged, Is.True);

                  Assert.That(rootTransaction.IsWriteable, Is.False);
                })
            .Verifiable();
        subListenerMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.RelationEndPointBecomingIncomplete(_subTransaction, endPointID))
            .Verifiable();
        subListenerMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.DataContainerMapUnregistering(_subTransaction, It.Is<DataContainer>(dc => dc.ID == orderItemA.ID)))
            .Verifiable();
        subListenerMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.RelationEndPointMapUnregistering(_subTransaction, oppositeEndPointIDA))
            .Verifiable();
        subListenerMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.DataContainerMapUnregistering(_subTransaction, It.Is<DataContainer>(dc => dc.ID == orderItemB.ID)))
            .Verifiable();
        subListenerMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.RelationEndPointMapUnregistering(_subTransaction, endPointID))
            .Verifiable();
        subListenerMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.RelationEndPointMapUnregistering(_subTransaction, oppositeEndPointIDB))
            .Verifiable();
        rootListenerMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.RelationEndPointBecomingIncomplete(rootTransaction, endPointID))
            .Verifiable();
        rootListenerMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.DataContainerMapUnregistering(rootTransaction, It.Is<DataContainer>(dc => dc.ID == orderItemA.ID)))
            .Verifiable();
        rootListenerMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.RelationEndPointMapUnregistering(rootTransaction, oppositeEndPointIDA))
            .Verifiable();
        rootListenerMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.DataContainerMapUnregistering(rootTransaction, It.Is<DataContainer>(dc => dc.ID == orderItemB.ID)))
            .Verifiable();
        rootListenerMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.RelationEndPointMapUnregistering(rootTransaction, endPointID))
            .Verifiable();
        rootListenerMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.RelationEndPointMapUnregistering(rootTransaction, oppositeEndPointIDB))
            .Verifiable();
        rootListenerMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.ObjectsUnloaded(rootTransaction, new[] { orderItemA, orderItemB }))
            .Callback(
                (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> unloadedDomainObjects) =>
                {
                  Assert.That(orderItemA.TransactionContext[rootTransaction].State.IsNotLoadedYet, Is.True);
                  Assert.That(orderItemB.TransactionContext[rootTransaction].State.IsNotLoadedYet, Is.True);

                  Assert.That(orderItemA.TransactionContext[_subTransaction].State.IsNotLoadedYet, Is.True);
                  Assert.That(orderItemB.TransactionContext[_subTransaction].State.IsNotLoadedYet, Is.True);

                  Assert.That(rootTransaction.IsWriteable, Is.False);
                })
            .Verifiable();
        subListenerMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.ObjectsUnloaded(_subTransaction, new[] { orderItemA, orderItemB }))
            .Verifiable();


        UnloadService.UnloadVirtualEndPointAndItemData(_subTransaction, endPointID);

        subListenerMock.Verify();
        rootListenerMock.Verify();
        sequence.Verify();
      }
      finally
      {
        ClientTransactionTestHelper.RemoveListener(rootTransaction, rootListenerMock.Object);
        ClientTransactionTestHelper.RemoveListener(_subTransaction, subListenerMock.Object);
      }
    }

    private void CheckDataContainerExists (ClientTransaction clientTransaction, DomainObject domainObject, bool dataContainerShouldExist)
    {
      var dataContainer = ClientTransactionTestHelper.GetDataManager(clientTransaction).DataContainers[domainObject.ID];
      if (dataContainerShouldExist)
        Assert.That(dataContainer, Is.Not.Null, "Data container '{0}' does not exist.", domainObject.ID);
      else
        Assert.That(dataContainer, Is.Null, "Data container '{0}' should not exist.", domainObject.ID);
    }

    private void CheckEndPointExists (ClientTransaction clientTransaction, DomainObject owningObject, string shortPropertyName, bool endPointShouldExist)
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(owningObject.ID, shortPropertyName);
      var endPoint = ClientTransactionTestHelper.GetDataManager(clientTransaction).GetRelationEndPointWithoutLoading(endPointID);
      if (endPointShouldExist)
        Assert.That(endPoint, Is.Not.Null, "End point '{0}' does not exist.", endPointID);
      else
        Assert.That(endPoint, Is.Null, "End point '{0}' should not exist.", endPointID);
    }

    private void CheckVirtualEndPoint (
        ClientTransaction clientTransaction,
        DomainObject owningObject,
        string shortPropertyName,
        bool shouldDataBeComplete)
    {
      CheckEndPointExists(clientTransaction, owningObject, shortPropertyName, true);

      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(owningObject.ID, shortPropertyName);
      var endPoint = ClientTransactionTestHelper.GetDataManager(clientTransaction).GetRelationEndPointWithoutLoading(endPointID);
      if (shouldDataBeComplete)
        Assert.That(endPoint.IsDataComplete, Is.True, "End point '{0}' does not have any data.", endPoint.ID);
      else
        Assert.That(endPoint.IsDataComplete, Is.False, "End point '{0}' should not have any data.", endPoint.ID);
    }
  }
}
