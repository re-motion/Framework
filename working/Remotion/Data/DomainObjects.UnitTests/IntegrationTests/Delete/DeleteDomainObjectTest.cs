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

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Delete
{
  [TestFixture]
  public class DeleteDomainObjectTest : ClientTransactionBaseTest
  {
    Order _order;
    OrderTicket _orderTicket;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetDatabaseModifyable ();
    }

    public override void SetUp ()
    {
      base.SetUp ();

      _order = DomainObjectIDs.Order3.GetObject<Order> ();
      _orderTicket = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket> ();
    }

    [Test]
    public void Delete ()
    {
      _orderTicket.Delete ();

      Assert.That (_orderTicket.State, Is.EqualTo (StateType.Deleted));
      Assert.That (_orderTicket.InternalDataContainer.State, Is.EqualTo (StateType.Deleted));
    }

    [Test]
    public void DeleteTwice ()
    {
      _orderTicket.Delete ();

      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (_orderTicket);
      _orderTicket.Delete ();

      Assert.That (eventReceiver.Count, Is.EqualTo (0));
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void GetObject ()
    {
      _orderTicket.Delete ();

      _orderTicket.ID.GetObject<OrderTicket> ();
    }

    [Test]
    public void GetObjectAndIncludeDeleted ()
    {
      _orderTicket.Delete ();

      Assert.That (_orderTicket.ID.GetObject<OrderTicket> (includeDeleted: true), Is.Not.Null);
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void ModifyDeletedObject ()
    {
      var dataContainer = _order.InternalDataContainer;

      _order.Delete ();

      SetPropertyValue (dataContainer, typeof (Order), "OrderNumber", 10);
    }

    [Test]
    public void AccessDeletedObject ()
    {
      _order.Delete ();

      Assert.That (_order.ID, Is.EqualTo (DomainObjectIDs.Order3));
      Assert.That (_order.OrderNumber, Is.EqualTo (3));
      Assert.That (_order.DeliveryDate, Is.EqualTo (new DateTime (2005, 3, 1)));
      Assert.That (_order.InternalDataContainer.Timestamp, Is.Not.Null);
      Assert.That (GetPropertyValue (_order.InternalDataContainer, typeof (Order), "OrderNumber"), Is.Not.Null);
    }

    [Test]
    public void CascadedDelete ()
    {
      Employee supervisor = DomainObjectIDs.Employee1.GetObject<Employee> ();
      supervisor.DeleteWithSubordinates ();

      DomainObject deletedSubordinate4 = DomainObjectIDs.Employee4.GetObject<Employee> (includeDeleted: true);
      DomainObject deletedSubordinate5 = DomainObjectIDs.Employee5.GetObject<Employee> (includeDeleted: true);

      Assert.That (supervisor.State, Is.EqualTo (StateType.Deleted));
      Assert.That (deletedSubordinate4.State, Is.EqualTo (StateType.Deleted));
      Assert.That (deletedSubordinate5.State, Is.EqualTo (StateType.Deleted));

      TestableClientTransaction.Commit ();
      ReInitializeTransaction ();

      CheckIfObjectIsDeleted (DomainObjectIDs.Employee1);
      CheckIfObjectIsDeleted (DomainObjectIDs.Employee4);
      CheckIfObjectIsDeleted (DomainObjectIDs.Employee5);
    }

    [Test]
    public void CascadedDeleteForNewObjects ()
    {
      Order newOrder = Order.NewObject ();
      OrderTicket newOrderTicket = OrderTicket.NewObject (newOrder);
      Assert.That (newOrder.OrderTicket, Is.SameAs (newOrderTicket));
      OrderItem newOrderItem = OrderItem.NewObject (newOrder);
      Assert.That (newOrder.OrderItems, Has.Member (newOrderItem));

      newOrder.Deleted += delegate
      {
        newOrderTicket.Delete ();
        newOrderItem.Delete ();
      };

      newOrder.Delete ();

      //Expectation: no exception

      Assert.That (newOrder.State, Is.EqualTo (StateType.Invalid));
      Assert.That (newOrderTicket.State, Is.EqualTo (StateType.Invalid));
      Assert.That (newOrderItem.State, Is.EqualTo (StateType.Invalid));
    }
  }
}
