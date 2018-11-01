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
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class ReplaceInOneToManyRelationTest : ClientTransactionBaseTest
  {
    private Customer _customer;
    private Customer _oldCustomerOfNewOrder;
    private Order _oldOrder;
    private Order _newOrder;

    public override void SetUp ()
    {
      base.SetUp ();

      _customer = DomainObjectIDs.Customer1.GetObject<Customer> ();
      _oldCustomerOfNewOrder = DomainObjectIDs.Customer3.GetObject<Customer> ();
      _oldOrder = DomainObjectIDs.Order1.GetObject<Order> ();
      _newOrder = DomainObjectIDs.Order3.GetObject<Order> ();
    }

    [Test]
    public void Events ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] { _customer, _oldCustomerOfNewOrder, _oldOrder, _newOrder };
      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] { _customer.Orders, _oldCustomerOfNewOrder.Orders };
      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (domainObjectEventSources, collectionEventSources);

      int replaceIndex = _customer.Orders.IndexOf (_oldOrder);
      _customer.Orders[replaceIndex] = _newOrder;

      ChangeState[] expectedChangeStates = new ChangeState[]
    {
      new RelationChangeState (_oldOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", _customer, null, "1. Changing event of old order from old customer to null"),
      new RelationChangeState (_newOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", _oldCustomerOfNewOrder, _customer, "2. Changing event of new order from null to new customer"),
      new CollectionChangeState (_customer.Orders, _oldOrder, "3. Removing event of old order from customer.Orders"),
      new CollectionChangeState (_customer.Orders, _newOrder, "4. Adding event of new order to customer.Orders"),
      new RelationChangeState (_customer, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", _oldOrder, _newOrder, "5. Changing event of customer"),

      new CollectionChangeState (_oldCustomerOfNewOrder.Orders, _newOrder, "6. Removing event of new order from oldCustomerOfNewOrder.Orders"),
      new RelationChangeState (_oldCustomerOfNewOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", _newOrder, null, "7. Changing event of oldCustomerOfNewOrder"),

      new RelationChangeState (_oldCustomerOfNewOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", null, null, "8. Changed event of oldCustomerOfNewOrder"),
      new CollectionChangeState (_oldCustomerOfNewOrder.Orders, _newOrder, "9. Removed event of new order from oldCustomerOfNewOrder.Orders"),
      
      new RelationChangeState (_customer, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", null, null, "10. Changed event of customer"),
      new CollectionChangeState (_customer.Orders, _newOrder, "11. Added event of new order to orders"),
      new CollectionChangeState (_customer.Orders, _oldOrder, "12. Removed event of old order from orders"),
      new RelationChangeState (_newOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", null, null, "13. Changed event of new order from null to new customer"),
      new RelationChangeState (_oldOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", null, null, "14. Changed event of old order from old customer to null"),

    };

      eventReceiver.Check (expectedChangeStates);

      Assert.That (_customer.State, Is.EqualTo (StateType.Changed));
      Assert.That (_oldCustomerOfNewOrder.State, Is.EqualTo (StateType.Changed));
      Assert.That (_oldOrder.State, Is.EqualTo (StateType.Changed));
      Assert.That (_newOrder.State, Is.EqualTo (StateType.Changed));

      Assert.That (_customer.Orders[replaceIndex], Is.SameAs (_newOrder));
      Assert.That (_newOrder.Customer, Is.SameAs (_customer));

      Assert.That (_customer.Orders.ContainsObject (_oldOrder), Is.False);
      Assert.That (_oldOrder.Customer, Is.Null);

      Assert.That (_oldCustomerOfNewOrder.Orders.ContainsObject (_newOrder), Is.False);
    }

    [Test]
    public void EventsWithoutOldCustomerOfNewOrder ()
    {
      Order newOrder = Order.NewObject ();
      DomainObject[] domainObjectEventSources = new DomainObject[] { _customer, _oldCustomerOfNewOrder, _oldOrder, newOrder };
      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] { _customer.Orders, _oldCustomerOfNewOrder.Orders };
      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (domainObjectEventSources, collectionEventSources);

      int replaceIndex = _customer.Orders.IndexOf (_oldOrder);
      _customer.Orders[replaceIndex] = newOrder;

      ChangeState[] expectedChangeStates = new ChangeState[]
    {
      new RelationChangeState (_oldOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", _customer, null, "1. Changing event of old order from old customer to null"),
      new RelationChangeState (newOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", null, _customer, "2. Changing event of new order from null to new customer"),
      new CollectionChangeState (_customer.Orders, _oldOrder, "3. Removing event of old order from customer.Orders"),
      new CollectionChangeState (_customer.Orders, newOrder, "4. Adding event of new order to customer.Orders"),
      new RelationChangeState (_customer, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", _oldOrder, newOrder, "5. Changing event of customer"),

      new RelationChangeState (_customer, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", null, null, "6. Changed event of customer"),
      new CollectionChangeState (_customer.Orders, newOrder, "7. Added event of new order to orders"),
      new CollectionChangeState (_customer.Orders, _oldOrder, "8. Removed event of old order from orders"),
      new RelationChangeState (newOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", null, null, "8. Changed event of new order from null to new customer"),
      new RelationChangeState (_oldOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", null, null, "10. Changed event of old order from old customer to null"),
    };

      eventReceiver.Check (expectedChangeStates);

      Assert.That (_customer.State, Is.EqualTo (StateType.Changed));
      Assert.That (_oldCustomerOfNewOrder.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (_oldOrder.State, Is.EqualTo (StateType.Changed));
      Assert.That (newOrder.State, Is.EqualTo (StateType.New));
      Assert.That (newOrder.Properties[typeof (Order), "Customer"].GetRelatedObjectID (), Is.EqualTo (_customer.ID));

      Assert.That (_customer.Orders[replaceIndex], Is.SameAs (newOrder));
      Assert.That (newOrder.Customer, Is.SameAs (_customer));

      Assert.That (_customer.Orders.ContainsObject (_oldOrder), Is.False);
      Assert.That (_oldOrder.Customer, Is.Null);
    }

    [Test]
    public void OldOrderCancelsReplace ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] { _customer, _oldCustomerOfNewOrder, _oldOrder, _newOrder };
      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] { _customer.Orders, _oldCustomerOfNewOrder.Orders };

      SequenceEventReceiver eventReceiver =
          new SequenceEventReceiver (domainObjectEventSources, collectionEventSources, 1);

      int replaceIndex = _customer.Orders.IndexOf (_oldOrder);

      try
      {
        _customer.Orders[replaceIndex] = _newOrder;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedChangeStates = new ChangeState[]
            { new RelationChangeState (_oldOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", _customer, null, "1. Changing event of old order from old customer to null")};

        eventReceiver.Check (expectedChangeStates);

        Assert.That (_customer.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_oldCustomerOfNewOrder.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_oldOrder.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_newOrder.State, Is.EqualTo (StateType.Unchanged));

        Assert.That (_customer.Orders[replaceIndex], Is.SameAs (_oldOrder));
        Assert.That (_oldOrder.Customer, Is.SameAs (_customer));

        Assert.That (_oldCustomerOfNewOrder.Orders.ContainsObject (_newOrder), Is.True);
        Assert.That (_newOrder.Customer, Is.SameAs (_oldCustomerOfNewOrder));
      }
    }

    [Test]
    public void NewOrderCancelsReplace ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] { _customer, _oldCustomerOfNewOrder, _oldOrder, _newOrder };
      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] { _customer.Orders, _oldCustomerOfNewOrder.Orders };

      SequenceEventReceiver eventReceiver =
          new SequenceEventReceiver (domainObjectEventSources, collectionEventSources, 2);

      int replaceIndex = _customer.Orders.IndexOf (_oldOrder);

      try
      {
        _customer.Orders[replaceIndex] = _newOrder;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedChangeStates = new ChangeState[]
            {
              new RelationChangeState (_oldOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", _customer, null, "1. Changing event of old order from old customer to null"),
              new RelationChangeState (_newOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", _oldCustomerOfNewOrder, _customer, "2. Changing event of new order from null to new customer")
            };

        eventReceiver.Check (expectedChangeStates);

        Assert.That (_customer.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_oldCustomerOfNewOrder.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_oldOrder.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_newOrder.State, Is.EqualTo (StateType.Unchanged));

        Assert.That (_customer.Orders[replaceIndex], Is.SameAs (_oldOrder));
        Assert.That (_oldOrder.Customer, Is.SameAs (_customer));

        Assert.That (_oldCustomerOfNewOrder.Orders.ContainsObject (_newOrder), Is.True);
        Assert.That (_newOrder.Customer, Is.SameAs (_oldCustomerOfNewOrder));
      }
    }

    [Test]
    public void NewOrderCollectionCancelsRemove ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] { _customer, _oldCustomerOfNewOrder, _oldOrder, _newOrder };
      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] { _customer.Orders, _oldCustomerOfNewOrder.Orders };

      SequenceEventReceiver eventReceiver =
          new SequenceEventReceiver (domainObjectEventSources, collectionEventSources, 3);

      int replaceIndex = _customer.Orders.IndexOf (_oldOrder);

      try
      {
        _customer.Orders[replaceIndex] = _newOrder;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedChangeStates = new ChangeState[]
            {
              new RelationChangeState (_oldOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", _customer, null, "1. Changing event of old order from old customer to null"),
              new RelationChangeState (_newOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", _oldCustomerOfNewOrder, _customer, "2. Changing event of new order from null to new customer"),
              new CollectionChangeState (_customer.Orders, _oldOrder, "3. Removing event of old order from customer.Orders")
            };

        eventReceiver.Check (expectedChangeStates);

        Assert.That (_customer.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_oldCustomerOfNewOrder.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_oldOrder.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_newOrder.State, Is.EqualTo (StateType.Unchanged));

        Assert.That (_customer.Orders[replaceIndex], Is.SameAs (_oldOrder));
        Assert.That (_oldOrder.Customer, Is.SameAs (_customer));

        Assert.That (_oldCustomerOfNewOrder.Orders.ContainsObject (_newOrder), Is.True);
        Assert.That (_newOrder.Customer, Is.SameAs (_oldCustomerOfNewOrder));
      }
    }

    [Test]
    public void NewOrderCollectionCancelsAdd ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] { _customer, _oldCustomerOfNewOrder, _oldOrder, _newOrder };
      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] { _customer.Orders, _oldCustomerOfNewOrder.Orders };

      SequenceEventReceiver eventReceiver =
          new SequenceEventReceiver (domainObjectEventSources, collectionEventSources, 4);

      int replaceIndex = _customer.Orders.IndexOf (_oldOrder);

      try
      {
        _customer.Orders[replaceIndex] = _newOrder;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedChangeStates = new ChangeState[]
            {
              new RelationChangeState (_oldOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", _customer, null, "1. Changing event of old order from old customer to null"),
              new RelationChangeState (_newOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", _oldCustomerOfNewOrder, _customer, "2. Changing event of new order from null to new customer"),
              new CollectionChangeState (_customer.Orders, _oldOrder, "3. Removing event of old order from customer.Orders"),
              new CollectionChangeState (_customer.Orders, _newOrder, "4. Adding event of new order to customer.Orders")
            };

        eventReceiver.Check (expectedChangeStates);

        Assert.That (_customer.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_oldCustomerOfNewOrder.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_oldOrder.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_newOrder.State, Is.EqualTo (StateType.Unchanged));

        Assert.That (_customer.Orders[replaceIndex], Is.SameAs (_oldOrder));
        Assert.That (_oldOrder.Customer, Is.SameAs (_customer));

        Assert.That (_oldCustomerOfNewOrder.Orders.ContainsObject (_newOrder), Is.True);
        Assert.That (_newOrder.Customer, Is.SameAs (_oldCustomerOfNewOrder));
      }
    }

    [Test]
    public void NewCustomerCancelsReplace ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] { _customer, _oldCustomerOfNewOrder, _oldOrder, _newOrder };
      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] { _customer.Orders, _oldCustomerOfNewOrder.Orders };

      SequenceEventReceiver eventReceiver =
          new SequenceEventReceiver (domainObjectEventSources, collectionEventSources, 5);

      int replaceIndex = _customer.Orders.IndexOf (_oldOrder);

      try
      {
        _customer.Orders[replaceIndex] = _newOrder;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedChangeStates = new ChangeState[]
            {
              new RelationChangeState (_oldOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", _customer, null, "1. Changing event of old order from old customer to null"),
              new RelationChangeState (_newOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", _oldCustomerOfNewOrder, _customer, "2. Changing event of new order from null to new customer"),
              new CollectionChangeState (_customer.Orders, _oldOrder, "3. Removing event of old order from customer.Orders"),
              new CollectionChangeState (_customer.Orders, _newOrder, "4. Adding event of new order to customer.Orders"),
              new RelationChangeState (_customer, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", _oldOrder, _newOrder, "5. Changing event of customer")
            };

        eventReceiver.Check (expectedChangeStates);

        Assert.That (_customer.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_oldCustomerOfNewOrder.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_oldOrder.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_newOrder.State, Is.EqualTo (StateType.Unchanged));

        Assert.That (_customer.Orders[replaceIndex], Is.SameAs (_oldOrder));
        Assert.That (_oldOrder.Customer, Is.SameAs (_customer));

        Assert.That (_oldCustomerOfNewOrder.Orders.ContainsObject (_newOrder), Is.True);
        Assert.That (_newOrder.Customer, Is.SameAs (_oldCustomerOfNewOrder));
      }
    }

    [Test]
    public void OldOrderCollectionCancelsRemove ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] { _customer, _oldCustomerOfNewOrder, _oldOrder, _newOrder };
      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] { _customer.Orders, _oldCustomerOfNewOrder.Orders };

      SequenceEventReceiver eventReceiver =
          new SequenceEventReceiver (domainObjectEventSources, collectionEventSources, 6);

      int replaceIndex = _customer.Orders.IndexOf (_oldOrder);

      try
      {
        _customer.Orders[replaceIndex] = _newOrder;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedChangeStates = new ChangeState[]
            {
              new RelationChangeState (_oldOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", _customer, null, "1. Changing event of old order from old customer to null"),
              new RelationChangeState (_newOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", _oldCustomerOfNewOrder, _customer, "2. Changing event of new order from null to new customer"),
              new CollectionChangeState (_customer.Orders, _oldOrder, "3. Removing event of old order from customer.Orders"),
              new CollectionChangeState (_customer.Orders, _newOrder, "4. Adding event of new order to customer.Orders"),
              new RelationChangeState (_customer, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", _oldOrder, _newOrder, "5. Changing event of customer"),
              new CollectionChangeState (_oldCustomerOfNewOrder.Orders, _newOrder, "6. Removing event of new order from oldCustomerOfNewOrder.Orders")
            };

        eventReceiver.Check (expectedChangeStates);

        Assert.That (_customer.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_oldCustomerOfNewOrder.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_oldOrder.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_newOrder.State, Is.EqualTo (StateType.Unchanged));

        Assert.That (_customer.Orders[replaceIndex], Is.SameAs (_oldOrder));
        Assert.That (_oldOrder.Customer, Is.SameAs (_customer));

        Assert.That (_oldCustomerOfNewOrder.Orders.ContainsObject (_newOrder), Is.True);
        Assert.That (_newOrder.Customer, Is.SameAs (_oldCustomerOfNewOrder));
      }
    }

    [Test]
    public void OldCustomerCancelsRemove ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] { _customer, _oldCustomerOfNewOrder, _oldOrder, _newOrder };
      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] { _customer.Orders, _oldCustomerOfNewOrder.Orders };

      SequenceEventReceiver eventReceiver =
          new SequenceEventReceiver (domainObjectEventSources, collectionEventSources, 7);

      int replaceIndex = _customer.Orders.IndexOf (_oldOrder);

      try
      {
        _customer.Orders[replaceIndex] = _newOrder;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedChangeStates = new ChangeState[]
            {
              new RelationChangeState (_oldOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", _customer, null, "1. Changing event of old order from old customer to null"),
              new RelationChangeState (_newOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", _oldCustomerOfNewOrder, _customer, "2. Changing event of new order from null to new customer"),
              new CollectionChangeState (_customer.Orders, _oldOrder, "3. Removing event of old order from customer.Orders"),
              new CollectionChangeState (_customer.Orders, _newOrder, "4. Adding event of new order to customer.Orders"),
              new RelationChangeState (_customer, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", _oldOrder, _newOrder, "5. Changing event of customer"),
              new CollectionChangeState (_oldCustomerOfNewOrder.Orders, _newOrder, "6. Removing event of new order from oldCustomerOfNewOrder.Orders"),
              new RelationChangeState (_oldCustomerOfNewOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", _newOrder, null, "7. Changing event of oldCustomerOfNewOrder")
            };

        eventReceiver.Check (expectedChangeStates);

        Assert.That (_customer.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_oldCustomerOfNewOrder.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_oldOrder.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_newOrder.State, Is.EqualTo (StateType.Unchanged));

        Assert.That (_customer.Orders[replaceIndex], Is.SameAs (_oldOrder));
        Assert.That (_oldOrder.Customer, Is.SameAs (_customer));

        Assert.That (_oldCustomerOfNewOrder.Orders.ContainsObject (_newOrder), Is.True);
        Assert.That (_newOrder.Customer, Is.SameAs (_oldCustomerOfNewOrder));
      }
    }

    [Test]
    public void ReplaceWithSameObject ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] { _customer, _oldCustomerOfNewOrder, _oldOrder, _newOrder };
      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] { _customer.Orders, _oldCustomerOfNewOrder.Orders };

      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (domainObjectEventSources, collectionEventSources);

      int replaceIndex = _customer.Orders.IndexOf (_oldOrder);
      _customer.Orders[replaceIndex] = _oldOrder;

      ChangeState[] expectedChangeStates = new ChangeState[0];
      eventReceiver.Check (expectedChangeStates);
    }

    [Test]
    public void ReplaceWithObjectAlreadyInCollection ()
    {
      try
      {
        _customer.Orders[0] = _customer.Orders[1];
        Assert.Fail ("Expected test to raise exception.");
      }
      catch (InvalidOperationException e)
      {
        string expectedMessage = string.Format ("The collection already contains an object with ID '{0}'.", _customer.Orders[1].ID);
        Assert.That (e.Message, Is.EqualTo (expectedMessage));
      }
    }

    [Test]
    public void ChangeEventsWithOldRelatedObjectNotLoaded ()
    {
      Order newOrder = DomainObjectIDs.Order4.GetObject<Order> ();

      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (
          new DomainObject[] { _oldOrder, newOrder, _customer },
          new DomainObjectCollection[] { _customer.Orders });

      int replaceIndex = _customer.Orders.IndexOf (_oldOrder);
      _customer.Orders[replaceIndex] = newOrder;

      Assert.That (newOrder.Customer, Is.SameAs (_customer));
      Assert.That (_customer.Orders.ContainsObject (newOrder), Is.True);

      Customer oldCustomerOfNewOrder = DomainObjectIDs.Customer4.GetObject<Customer> ();

      Assert.That (oldCustomerOfNewOrder.Orders.ContainsObject (newOrder), Is.False);

      ChangeState[] expectedStates = new ChangeState[]
    {
      new RelationChangeState (_oldOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", _customer, null, "1. Changing event of old order from new customer to null"),
      new RelationChangeState (newOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", oldCustomerOfNewOrder, _customer, "2. Changing event of new order from old to new customer"),
      new CollectionChangeState (_customer.Orders, _oldOrder, "3. Removing event of new customer's order collection"),
      new CollectionChangeState (_customer.Orders, newOrder, "4. Adding event of new customer's order collection"),
      new RelationChangeState (_customer, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", _oldOrder, newOrder, "5. Changing event of new customer from old order to new order"),

      new RelationChangeState (_customer, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", null, null, "6. Changed event of new customer from old order to new order"),
      new CollectionChangeState (_customer.Orders, newOrder, "7. Added event of new customer's order collection"),
      new CollectionChangeState (_customer.Orders, _oldOrder, "8. Removed event of new customer's order collection"),
      new RelationChangeState (newOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", null, null, "9. Changed event of new order from old to new customer"),
      new RelationChangeState (_oldOrder, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", null, null, "10. Changed event of old order from new customer to null"),
    };

      eventReceiver.Check (expectedStates);

    }
  }
}
