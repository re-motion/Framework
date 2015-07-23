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
  public class DeleteDomainObjectWithManyToOneRelationTest : ClientTransactionBaseTest
  {
    private OrderItem _orderItem;
    private Order _order;
    private SequenceEventReceiver _eventReceiver;

    public override void SetUp ()
    {
      base.SetUp ();

      _orderItem = DomainObjectIDs.OrderItem1.GetObject<OrderItem>();
      _order = _orderItem.Order;

      _eventReceiver = CreateEventReceiver ();
    }

    [Test]
    public void DeleteOrderItemEvents ()
    {
      _orderItem.Delete ();

      ChangeState[] expectedStates = new ChangeState[]
    {
      new ObjectDeletionState (_orderItem, "1. Deleting event of orderItem"),
      new CollectionChangeState (_order.OrderItems, _orderItem, "2. Removing event of order.OrderItems"),
      new RelationChangeState (_order, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems", _orderItem, null, "3. Relation changing event of order"),
      new RelationChangeState (_order, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems", null, null, "4. Relation changed event of order"),
      new CollectionChangeState (_order.OrderItems, _orderItem, "5. Removed event of order.OrderItems"),
      new ObjectDeletionState (_orderItem, "6. Deleted event of orderItem"),
    };

      _eventReceiver.Check (expectedStates);
    }

    [Test]
    public void OrderItemCancelsDeleteEvent ()
    {
      _eventReceiver.CancelEventNumber = 1;

      try
      {
        _orderItem.Delete ();
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedStates = new ChangeState[] { new ObjectDeletionState (_orderItem, "1. Deleting event of orderItem") };

        _eventReceiver.Check (expectedStates);
      }
    }

    [Test]
    public void OrderItemCollectionCancelsRemoveEvent ()
    {
      _eventReceiver.CancelEventNumber = 2;

      try
      {
        _orderItem.Delete ();
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedStates = new ChangeState[]
            { 
              new ObjectDeletionState (_orderItem, "1. Deleting event of orderItem"),
              new CollectionChangeState (_order.OrderItems, _orderItem, "2. Removing event of order.OrderItems") 
            };

        _eventReceiver.Check (expectedStates);
      }
    }

    [Test]
    public void OrderCancelsRelationChangeEvent ()
    {
      _eventReceiver.CancelEventNumber = 3;

      try
      {
        _orderItem.Delete ();
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedStates = new ChangeState[]
            {
              new ObjectDeletionState (_orderItem, "1. Deleting event of orderItem"),
              new CollectionChangeState (_order.OrderItems, _orderItem, "2. Removing event of order.OrderItems"),
              new RelationChangeState (_order, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems", _orderItem, null, "3. Relation changing event of order")
            };

        _eventReceiver.Check (expectedStates);
      }
    }

    [Test]
    public void Relations ()
    {
      int numberOfOrderItemsBeforeDelete = _order.OrderItems.Count;
      _orderItem.Delete ();

      Assert.That (_orderItem.Order, Is.Null);
      Assert.That (_order.OrderItems.Count, Is.EqualTo (numberOfOrderItemsBeforeDelete - 1));
      Assert.That (_order.OrderItems.Contains (_orderItem.ID), Is.False);
      Assert.That (_orderItem.Properties[typeof (OrderItem), "Order"].GetRelatedObjectID (), Is.Null);
      Assert.That (_order.State, Is.EqualTo (StateType.Changed));
      Assert.That (_order.InternalDataContainer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void ChangePropertyBeforeDeletion ()
    {
      _orderItem.Order = null;
      _eventReceiver = CreateEventReceiver ();

      _orderItem.Delete ();

      ChangeState[] expectedStates = new ChangeState[]
          {
            new ObjectDeletionState (_orderItem, "1. Deleting event of orderItem"),
            new ObjectDeletionState (_orderItem, "2. Deleted event of orderItem"),
          };

      _eventReceiver.Check (expectedStates);
    }

    [Test]
    public void GetOriginalRelatedObjects ()
    {
      _orderItem.Delete ();

      DomainObjectCollection originalOrderItems = _order.GetOriginalRelatedObjects ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");

      Assert.That (originalOrderItems, Is.Not.Null);
      Assert.That (originalOrderItems.Count, Is.EqualTo (2));
      Assert.That (originalOrderItems[_orderItem.ID], Is.SameAs (_orderItem));
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void SetRelatedObjectOfDeletedObject ()
    {
      _orderItem.Delete ();

      _orderItem.Order = _order;
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void ReassignDeletedObject ()
    {
      _orderItem.Delete ();

      _order.OrderItems.Add (_orderItem);
    }

    [Test]
    public void DeleteOrderEvents ()
    {
      _order.Delete ();

      var expectedStates = new ChangeState[]
    {
      new ObjectDeletionState (_order, "1. Deleting event of order"),
      new CollectionDeletionState (_order.OrderItems, "2. Deleting event of order.OrderItems"),
      new RelationChangeState (_orderItem, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", _order, null, "3. Relation changing event of orderItem"),
      new RelationChangeState (_orderItem, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", null, null, "4. Relation changed event of orderItem"),
      new CollectionDeletionState (_order.OrderItems, "5. Deleted event of order.OrderItems"),
      new ObjectDeletionState (_order, "6. Deleted event of order"),
    };

      _eventReceiver.Check (expectedStates);
    }

    [Test]
    public void DeleteOrderEvents_CancelFromCollectionDeletingEvent ()
    {
      _eventReceiver.CancelEventNumber = 2;

      try
      {
        _order.Delete ();
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        var expectedStates = new ChangeState[]
            {
              new ObjectDeletionState (_order, "1. Deleting event of order"),
              new CollectionDeletionState (_order.OrderItems, "2. Deleting event of order.OrderItems"),
            };

        _eventReceiver.Check (expectedStates);
      }

      Assert.That (_order.State, Is.Not.EqualTo (StateType.Deleted));
      Assert.That (_order.OrderItems, Is.Not.Empty);
    }

    [Test]
    public void DeleteOrderEvents_CancelAfterCollectionDeletingEvent ()
    {
      _eventReceiver.CancelEventNumber = 3;

      try
      {
        _order.Delete ();
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        var expectedStates = new ChangeState[]
            {
              new ObjectDeletionState (_order, "1. Deleting event of order"),
              new CollectionDeletionState (_order.OrderItems, "2. Deleting event of order.OrderItems"),
              new RelationChangeState (_orderItem, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", _order, null, "3. Relation changing event of orderItem"),
            };

        _eventReceiver.Check (expectedStates);
      }

      Assert.That (_order.State, Is.Not.EqualTo (StateType.Deleted));
      Assert.That (_order.OrderItems, Is.Not.Empty);
    }

    private SequenceEventReceiver CreateEventReceiver ()
    {
      return new SequenceEventReceiver (
          new DomainObject[] { _orderItem, _order },
          new DomainObjectCollection[] { _order.OrderItems });
    }
  }
}
