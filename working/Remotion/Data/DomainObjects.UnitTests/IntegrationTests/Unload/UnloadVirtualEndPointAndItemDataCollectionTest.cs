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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Unload
{
  [TestFixture]
  public class UnloadVirtualEndPointAndItemDataCollectionTest : UnloadTestBase
  {
    [Test]
    public void UnloadVirtualEndPointAndItemData_Collection ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order> ();
      var orderItems = order.OrderItems;
      orderItems.EnsureDataComplete ();
      var orderItem1 = DomainObjectIDs.OrderItem1.GetObject<OrderItem> ();
      var orderItem2 = DomainObjectIDs.OrderItem2.GetObject<OrderItem>();

      Assert.That (orderItems.IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPointAndItemData (TestableClientTransaction, orderItems.AssociatedEndPointID);

      CheckDataContainerExists (order, true);
      CheckDataContainerExists (orderItem1, false);
      CheckDataContainerExists (orderItem2, false);

      CheckEndPointExists (order, "OrderItems", false);
      CheckEndPointExists (orderItem1, "Order", false);
      CheckEndPointExists (orderItem2, "Order", false);

      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItem1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItem2.State, Is.EqualTo (StateType.NotLoadedYet));

      Assert.That (orderItems.IsDataComplete, Is.False);
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_Object ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order> ();
      var orderTicket = order.OrderTicket;

      UnloadService.UnloadVirtualEndPointAndItemData (TestableClientTransaction, RelationEndPointID.Resolve (order, o => o.OrderTicket));

      CheckDataContainerExists (order, true);
      CheckDataContainerExists (orderTicket, false);

      CheckEndPointExists (order, "OrderTicket", false);

      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderTicket.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_Object_Null ()
    {
      var employee = DomainObjectIDs.Employee1.GetObject<Employee> ();
      Assert.That (employee.Computer, Is.Null);

      CheckDataContainerExists (employee, true);
      CheckEndPointExists (employee, "Computer", true);

      UnloadService.UnloadVirtualEndPointAndItemData (TestableClientTransaction, RelationEndPointID.Resolve (employee, e => e.Computer));

      CheckDataContainerExists (employee, true);
      CheckEndPointExists (employee, "Computer", false);

      Assert.That (employee.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_Collection_EnsureDataAvailable_AndComplete ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order> ();
      var orderItems = order.OrderItems;
      orderItems.EnsureDataComplete ();
      var orderItem1 = DomainObjectIDs.OrderItem1.GetObject<OrderItem> ();
      var orderItem2 = DomainObjectIDs.OrderItem2.GetObject<OrderItem>();

      Assert.That (orderItems.IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPointAndItemData (TestableClientTransaction, orderItems.AssociatedEndPointID);

      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItem1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItem2.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItems.IsDataComplete, Is.False);

      orderItem1.EnsureDataAvailable ();

      Assert.That (orderItem1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItem2.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItems.IsDataComplete, Is.False);

      orderItems.EnsureDataComplete ();

      Assert.That (orderItem1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItem2.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItems.IsDataComplete, Is.True);
      Assert.That (orderItems, Is.EquivalentTo (new[] { orderItem1, orderItem2 }));
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_Collection_Reload ()
    {
      SetDatabaseModifyable ();

      var order = DomainObjectIDs.Order1.GetObject<Order> ();
      var orderItems = order.OrderItems;
      orderItems.EnsureDataComplete ();
      var orderItem1 = DomainObjectIDs.OrderItem1.GetObject<OrderItem> ();
      var orderItem2 = DomainObjectIDs.OrderItem2.GetObject<OrderItem>();

      ObjectID newOrderItemID;
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var orderInOtherTx = DomainObjectIDs.Order1.GetObject<Order> ();
        var orderItem1InOtherTx = DomainObjectIDs.OrderItem1.GetObject<OrderItem>();
        var newOrderItem = OrderItem.NewObject ();
        newOrderItemID = newOrderItem.ID;
        orderInOtherTx.OrderItems.Add (newOrderItem);
        orderInOtherTx.OrderItems.Remove (orderItem1InOtherTx);

        orderItem1InOtherTx.Order = DomainObjectIDs.Order3.GetObject<Order> ();

        ClientTransaction.Current.Commit ();
      }

      Assert.That (orderItems, Is.EquivalentTo (new[] { orderItem1, orderItem2 }));

      UnloadService.UnloadVirtualEndPointAndItemData (TestableClientTransaction, orderItems.AssociatedEndPointID);

      Assert.That (orderItems, Is.EquivalentTo (new[] { orderItem2, newOrderItemID.GetObject<OrderItem>() }));
      Assert.That (orderItem1.Order, Is.SameAs (DomainObjectIDs.Order3.GetObject<Order> ()));
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_Object_Reload ()
    {
      SetDatabaseModifyable ();

      var employee = DomainObjectIDs.Employee3.GetObject<Employee> ();
      var computer = employee.Computer;

      ObjectID newComputerID;
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var employeeInOtherTx = employee.ID.GetObject<Employee> ();
        var newComputer = Computer.NewObject ();
        newComputerID = newComputer.ID;
        employeeInOtherTx.Computer = newComputer;

        ClientTransaction.Current.Commit ();
      }

      Assert.That (employee.Computer, Is.SameAs (computer));

      UnloadService.UnloadVirtualEndPointAndItemData (TestableClientTransaction, RelationEndPointID.Resolve (employee, e => e.Computer));

      Assert.That (employee.Computer, Is.Not.SameAs (computer));
      Assert.That (employee.Computer.ID, Is.EqualTo (newComputerID));
      Assert.That (computer.Employee, Is.Null);
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_Collection_Events ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];

      var listenerMock = ClientTransactionTestHelperWithMocks.CreateAndAddListenerMock (TestableClientTransaction);
      using (listenerMock.GetMockRepository ().Ordered ())
      {
        listenerMock
            .Expect (mock => mock.ObjectsUnloading (
                Arg.Is (TestableClientTransaction), 
                Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { orderItemA, orderItemB })))
            .WhenCalled (
            mi =>
            {
              Assert.That (orderItemA.OnUnloadingCalled, Is.False, "items unloaded after this method is called");
              Assert.That (orderItemB.OnUnloadingCalled, Is.False, "items unloaded after this method is called");
              Assert.That (orderItemA.OnUnloadedCalled, Is.False, "items unloaded after this method is called");
              Assert.That (orderItemB.OnUnloadedCalled, Is.False, "items unloaded after this method is called");

              Assert.That (orderItemA.State, Is.EqualTo (StateType.Unchanged));
              Assert.That (orderItemB.State, Is.EqualTo (StateType.Unchanged));
            });
        listenerMock
            .Expect (mock => mock.ObjectsUnloaded (
                Arg.Is (TestableClientTransaction), 
                Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { orderItemA, orderItemB })))
            .WhenCalled (
            mi =>
            {
              Assert.That (orderItemA.OnUnloadingCalled, Is.True, "items unloaded before this method is called");
              Assert.That (orderItemB.OnUnloadingCalled, Is.True, "items unloaded before this method is called");
              Assert.That (orderItemA.OnUnloadedCalled, Is.True, "items unloaded before this method is called");
              Assert.That (orderItemB.OnUnloadedCalled, Is.True, "items unloaded before this method is called");

              Assert.That (orderItemA.State, Is.EqualTo (StateType.NotLoadedYet));
              Assert.That (orderItemB.State, Is.EqualTo (StateType.NotLoadedYet));
            });
      }

      listenerMock.Replay ();

      try
      {
        UnloadService.UnloadVirtualEndPointAndItemData (TestableClientTransaction, order1.OrderItems.AssociatedEndPointID);
        listenerMock.VerifyAllExpectations ();
      }
      finally
      {
        listenerMock.BackToRecord (); // For Discarding
      }

      Assert.That (orderItemA.UnloadingState, Is.EqualTo (StateType.Unchanged), "OnUnloading before state change");
      Assert.That (orderItemB.UnloadingState, Is.EqualTo (StateType.Unchanged), "OnUnloading before state change");
      Assert.That (orderItemA.OnUnloadingDateTime, Is.LessThan (orderItemB.OnUnloadingDateTime), "orderItemA.OnUnloading before orderItemB.OnUnloading");

      Assert.That (orderItemA.UnloadedState, Is.EqualTo (StateType.NotLoadedYet), "OnUnloaded after state change");
      Assert.That (orderItemB.UnloadedState, Is.EqualTo (StateType.NotLoadedYet), "OnUnloaded after state change");
      Assert.That (orderItemA.OnUnloadedDateTime, Is.GreaterThan (orderItemB.OnUnloadedDateTime), "orderItemA.OnUnloaded after orderItemB.OnUnloaded");
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_Object_Events ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      var orderTicket = order1.OrderTicket;

      var listenerMock = ClientTransactionTestHelperWithMocks.CreateAndAddListenerMock (TestableClientTransaction);
      using (listenerMock.GetMockRepository ().Ordered ())
      {
        listenerMock
            .Expect (mock => mock.ObjectsUnloading (
                Arg.Is (TestableClientTransaction),
                Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { orderTicket })))
            .WhenCalled (
            mi =>
            {
              Assert.That (orderTicket.OnUnloadingCalled, Is.False, "items unloaded after this method is called");
              Assert.That (orderTicket.OnUnloadedCalled, Is.False, "items unloaded after this method is called");

              Assert.That (orderTicket.State, Is.EqualTo (StateType.Unchanged));
            });
        listenerMock
            .Expect (mock => mock.ObjectsUnloaded (
                Arg.Is (TestableClientTransaction),
                Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { orderTicket })))
            .WhenCalled (
            mi =>
            {
              Assert.That (orderTicket.OnUnloadingCalled, Is.True, "items unloaded before this method is called");
              Assert.That (orderTicket.OnUnloadedCalled, Is.True, "items unloaded before this method is called");

              Assert.That (orderTicket.State, Is.EqualTo (StateType.NotLoadedYet));
            });
      }

      listenerMock.Replay ();
      try
      {
        UnloadService.UnloadVirtualEndPointAndItemData (TestableClientTransaction, RelationEndPointID.Resolve (order1, o => o.OrderTicket));

        listenerMock.VerifyAllExpectations ();
      }
      finally
      {
        listenerMock.BackToRecord (); // For Discarding
      }

      Assert.That (orderTicket.UnloadingState, Is.EqualTo (StateType.Unchanged), "OnUnloading before state change");
      Assert.That (orderTicket.UnloadedState, Is.EqualTo (StateType.NotLoadedYet), "OnUnloaded after state change");
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_IsAtomicWithinTransaction_WhenSingleCollectionItemIsChanged ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      var endPointID = RelationEndPointID.Resolve (order1, o => o.OrderItems);
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];
      
      // Change a single OrderItem - this must cause nothing to be unloaded
      orderItemB.Product = "Changed";

      Assert.That (orderItemA.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItemB.State, Is.EqualTo (StateType.Changed));
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (endPointID).HasChanged, Is.False);
      Assert.That (order1.OrderItems.IsDataComplete, Is.True);

      CheckDataContainerExists (orderItemA, true);
      CheckDataContainerExists (orderItemB, true);
      CheckVirtualEndPointExistsAndComplete (endPointID, true, true);
      
      Assert.That (() => UnloadService.UnloadVirtualEndPointAndItemData (TestableClientTransaction, endPointID), Throws.InvalidOperationException);

      CheckDataContainerExists (orderItemA, true);
      CheckDataContainerExists (orderItemB, true);
      CheckVirtualEndPointExistsAndComplete (endPointID, true, true);

      Assert.That (UnloadService.TryUnloadVirtualEndPointAndItemData (TestableClientTransaction, endPointID), Is.False);

      CheckDataContainerExists (orderItemA, true);
      CheckDataContainerExists (orderItemB, true);
      CheckVirtualEndPointExistsAndComplete (endPointID, true, true);

      Assert.That (orderItemA.State, Is.Not.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItemB.State, Is.Not.EqualTo (StateType.NotLoadedYet));
      Assert.That (order1.OrderItems.IsDataComplete, Is.True);
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_IsAtomicWithinTransaction_WhenCollectionIsChanged ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      var endPointID = RelationEndPointID.Resolve (order1, o => o.OrderItems);
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];

      // Change the collection, but not the items; we need to test this within a subtransaction because this is the only way to get the collection to
      // change without changing items (or the collection reference, which doesn't influence unloadability).
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope())
      {
        order1.OrderItems.Clear();
        order1.OrderItems.Add (orderItemB);
        order1.OrderItems.Add (orderItemA);

        Assert.That (orderItemA.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (orderItemB.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (DataManagementService.GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (endPointID).HasChanged, Is.True);
        Assert.That (order1.OrderItems.IsDataComplete, Is.True);

        CheckDataContainerExists (orderItemA, true);
        CheckDataContainerExists (orderItemB, true);
        CheckVirtualEndPointExistsAndComplete (endPointID, true, true);

        Assert.That (() => UnloadService.UnloadVirtualEndPointAndItemData (ClientTransaction.Current, endPointID), Throws.InvalidOperationException);

        CheckDataContainerExists (orderItemA, true);
        CheckDataContainerExists (orderItemB, true);
        CheckVirtualEndPointExistsAndComplete (endPointID, true, true);

        Assert.That (UnloadService.TryUnloadVirtualEndPointAndItemData (ClientTransaction.Current, endPointID), Is.False);

        CheckDataContainerExists (orderItemA, true);
        CheckDataContainerExists (orderItemB, true);
        CheckVirtualEndPointExistsAndComplete (endPointID, true, true);

        Assert.That (orderItemA.State, Is.Not.EqualTo (StateType.NotLoadedYet));
        Assert.That (orderItemB.State, Is.Not.EqualTo (StateType.NotLoadedYet));
        Assert.That (order1.OrderItems.IsDataComplete, Is.True);
      }
    }
  }
}