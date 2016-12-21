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
  public class DeleteDomainObjectWithOneToOneRelationTest : ClientTransactionBaseTest
  {
    private OrderTicket _orderTicket;
    private Order _order;
    private SequenceEventReceiver _eventReceiver;

    public override void SetUp ()
    {
      base.SetUp ();

      _orderTicket = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket> ();
      _order = _orderTicket.Order;

      _eventReceiver = CreateEventReceiver ();
    }

    [Test]
    public void DeleteOrderTicketEvents ()
    {
      _orderTicket.Delete ();

      ChangeState[] expectedStates = new ChangeState[]
    {
      new ObjectDeletionState (_orderTicket, "1. Deleting event of orderTicket"),
      new RelationChangeState (_order, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", _orderTicket, null, "2. Relation changing event of order"),
      new RelationChangeState (_order, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", null, null, "3. Relation changed event of order"),
      new ObjectDeletionState (_orderTicket, "4. Deleted event of orderTicket")
    };

      _eventReceiver.Check (expectedStates);
    }

    [Test]
    public void DeleteComputerWithoutEmployeeEvents ()
    {
      Computer computer = DomainObjectIDs.Computer4.GetObject<Computer> ();

      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (
          new DomainObject[] { computer }, new DomainObjectCollection[0]);

      computer.Delete ();

      ChangeState[] expectedStates = new ChangeState[] 
    {
      new ObjectDeletionState (computer, "1. Deleting of computer"),
      new ObjectDeletionState (computer, "2. Deleted of computer")
    };

      eventReceiver.Check (expectedStates);
    }

    [Test]
    public void OrderTicketCancelsDeleteEvent ()
    {
      _eventReceiver.CancelEventNumber = 1;

      try
      {
        _orderTicket.Delete ();
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedStates = new ChangeState[] { new ObjectDeletionState (_orderTicket, "1. Deleting event of orderTicket") };

        _eventReceiver.Check (expectedStates);
      }
    }

    [Test]
    public void OrderCancelsRelationChangeEvent ()
    {
      _eventReceiver.CancelEventNumber = 2;

      try
      {
        _orderTicket.Delete ();
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedStates = new ChangeState[]
            {
              new ObjectDeletionState (_orderTicket, "1. Deleting event of orderTicket"),
              new RelationChangeState (_order, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", _orderTicket, null, "2. Relation changing event of order")
            };

        _eventReceiver.Check (expectedStates);
      }
    }

    [Test]
    public void NonVirtualProperty ()
    {
      _orderTicket.Delete ();

      Assert.That (_orderTicket.Order, Is.Null);
      Assert.That (_order.OrderTicket, Is.Null);
      Assert.That (_orderTicket.Properties[typeof (OrderTicket), "Order"].GetRelatedObjectID(), Is.Null);
      Assert.That (_order.State, Is.EqualTo (StateType.Changed));
      Assert.That (_order.InternalDataContainer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void VirtualProperty ()
    {
      _order.Delete ();

      Assert.That (_orderTicket.Order, Is.Null);
      Assert.That (_order.OrderTicket, Is.Null);
      Assert.That (_orderTicket.Properties[typeof (OrderTicket), "Order"].GetRelatedObjectID (), Is.Null);
      Assert.That (_orderTicket.InternalDataContainer.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void ChangeNonVirtualPropertyBeforeDeletion ()
    {
      _orderTicket.Order = null;
      _eventReceiver = CreateEventReceiver ();

      _orderTicket.Delete ();

      ChangeState[] expectedStates = new ChangeState[]
          {
            new ObjectDeletionState (_orderTicket, "1. Deleting event of orderTicket"),
            new ObjectDeletionState (_orderTicket, "2. Deleted event of orderTicket"),
          };

      _eventReceiver.Check (expectedStates);
    }

    [Test]
    public void ChangeVirtualPropertyBeforeDeletion ()
    {
      _order.OrderTicket = null;
      _eventReceiver = CreateEventReceiver ();

      _order.Delete ();

      ChangeState[] expectedStates = new ChangeState[]
          {
            new ObjectDeletionState (_order, "1. Deleting event of order"),
            new CollectionDeletionState (_order.OrderItems, "2. Deleting of _order.OrderItems"),
            new CollectionDeletionState (_order.OrderItems, "3. Deleted of _order.OrderItems"),
            new ObjectDeletionState (_order, "4. Deleted event of order"),
          };

      _eventReceiver.Check (expectedStates);
    }

    [Test]
    public void GetOriginalRelatedObjectFromDeletedObject ()
    {
      _orderTicket.Delete ();

      Order originalOrder = (Order) _orderTicket.GetOriginalRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order");

      Assert.That (originalOrder, Is.Not.Null);
      Assert.That (originalOrder.ID, Is.EqualTo (DomainObjectIDs.Order1));
    }

    [Test]
    public void GetOriginalRelatedObjectFromOppositeObject ()
    {
      Order oldRelatedOrder = _orderTicket.Order;
      _orderTicket.Delete ();

      OrderTicket deletedOrderTicket = (OrderTicket) oldRelatedOrder.GetOriginalRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");

      Assert.That (deletedOrderTicket, Is.Not.Null);
      Assert.That (deletedOrderTicket.ID, Is.EqualTo (_orderTicket.ID));
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void SetRelatedObjectOfDeletedObject ()
    {
      _orderTicket.Delete ();

      _orderTicket.Order = _order;
    }

    [Test]
    public void GetOriginalRelatedObjectForBothDeleted ()
    {
      OrderTicket orderTicket = _order.OrderTicket;
      _order.Delete ();
      orderTicket.Delete ();

      Assert.That (_order.GetOriginalRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"), Is.Not.Null);
      Assert.That (orderTicket.GetOriginalRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"), Is.Not.Null);
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void ReassignDeletedObject ()
    {
      _orderTicket.Delete ();

      _order.OrderTicket = _orderTicket;
    }

    private SequenceEventReceiver CreateEventReceiver ()
    {
      return new SequenceEventReceiver (
          new DomainObject[] { _orderTicket, _order },
          new DomainObjectCollection[] { _order.OrderItems });
    }
  }
}
