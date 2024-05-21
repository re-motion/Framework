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
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class OneToManyRelationChangeWithDomainObjectCollectionTest : RelationChangeBaseTest
  {
    private Customer _oldCustomer;
    private Customer _newCustomer;
    private Order _order1;
    private Order _order2;

    public override void SetUp ()
    {
      base.SetUp();

      _oldCustomer = DomainObjectIDs.Customer1.GetObject<Customer>();
      _newCustomer = DomainObjectIDs.Customer2.GetObject<Customer>();
      _order1 = DomainObjectIDs.Order1.GetObject<Order>();
      _order2 = DomainObjectIDs.Order2.GetObject<Order>();
    }

    [Test]
    public void ChangeEvents ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] {
        _oldCustomer, _newCustomer, _order1};

      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] {
        _oldCustomer.Orders, _newCustomer.Orders};

      SequenceEventReceiver eventReceiver = new SequenceEventReceiver(domainObjectEventSources, collectionEventSources);

      _newCustomer.Orders.Add(_order1);

      ChangeState[] expectedChangeStates =
          new ChangeState[]
          {
              new RelationChangeState(
                  _order1,
                  "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer",
                  _oldCustomer,
                  _newCustomer,
                  "1. Changing event of order from old to new customer"),
              new CollectionChangeState(_newCustomer.Orders, _order1, "2. Adding event of new customer's order collection"),
              new RelationChangeState(_newCustomer, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", null, _order1, "3. Changing event of new customer"),
              new CollectionChangeState(_oldCustomer.Orders, _order1, "4. Removing of orders of old customer"),
              new RelationChangeState(_oldCustomer, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", _order1, null, "5. Changing event of old customer"),
              new RelationChangeState(_oldCustomer, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", null, null, "6. Changed event of old customer"),
              new CollectionChangeState(_oldCustomer.Orders, _order1, "7. Removed event of old customer's order collection"),
              new RelationChangeState(_newCustomer, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", null, null, "8. Changed event of new customer"),
              new CollectionChangeState(_newCustomer.Orders, _order1, "9. Added event of new customer's order collection"),
              new RelationChangeState(
                  _order1,
                  "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer",
                  null,
                  null,
                  "10. Changed event of order from old to new customer"),
          };

      eventReceiver.Check(expectedChangeStates);

      Assert.That(_order1.State.IsChanged, Is.True);
      Assert.That(_oldCustomer.State.IsChanged, Is.True);
      Assert.That(_newCustomer.State.IsChanged, Is.True);

      Assert.That(_order1.Customer, Is.SameAs(_newCustomer));
      Assert.That(_oldCustomer.Orders[_order1.ID], Is.Null);
      Assert.That(_newCustomer.Orders[_order1.ID], Is.SameAs(_order1));

      Assert.That(_order1.InternalDataContainer.State.IsChanged, Is.True);
      Assert.That(_oldCustomer.InternalDataContainer.State.IsUnchanged, Is.True);
      Assert.That(_newCustomer.InternalDataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void OrderCancelsChangeEvent ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] {_oldCustomer, _newCustomer, _order1};
      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] {_oldCustomer.Orders, _newCustomer.Orders};
      SequenceEventReceiver eventReceiver = new SequenceEventReceiver(domainObjectEventSources, collectionEventSources, 1);

      try
      {
        _newCustomer.Orders.Add(_order1);
        Assert.Fail("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedChangeStates =
            new ChangeState[]
            {
                new RelationChangeState(
                    _order1,
                    "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer",
                    _oldCustomer,
                    _newCustomer,
                    "1. Changing event of order from old to new customer")
            };

        eventReceiver.Check(expectedChangeStates);

        Assert.That(_order1.State.IsUnchanged, Is.True);
        Assert.That(_oldCustomer.State.IsUnchanged, Is.True);
        Assert.That(_newCustomer.State.IsUnchanged, Is.True);

        Assert.That(_order1.Customer, Is.SameAs(_oldCustomer));
        Assert.That(_oldCustomer.Orders[_order1.ID], Is.SameAs(_order1));
        Assert.That(_newCustomer.Orders[_order1.ID], Is.Null);
      }
    }

    [Test]
    public void NewCustomerCollectionCancelsChangeEvent ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] {
        _oldCustomer, _newCustomer, _order1};

      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] {
        _oldCustomer.Orders, _newCustomer.Orders};

      SequenceEventReceiver eventReceiver = new SequenceEventReceiver(domainObjectEventSources, collectionEventSources, 2);

      try
      {
        _newCustomer.Orders.Add(_order1);
        Assert.Fail("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedChangeStates =
            new ChangeState[]
            {
                new RelationChangeState(
                    _order1,
                    "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer",
                    _oldCustomer,
                    _newCustomer,
                    "1. Changing event of order from old to new customer"),
                new CollectionChangeState(_newCustomer.Orders, _order1, "2. Adding event of new customer's order collection")
            };

        eventReceiver.Check(expectedChangeStates);

        Assert.That(_order1.State.IsUnchanged, Is.True);
        Assert.That(_oldCustomer.State.IsUnchanged, Is.True);
        Assert.That(_newCustomer.State.IsUnchanged, Is.True);

        Assert.That(_order1.Customer, Is.SameAs(_oldCustomer));
        Assert.That(_oldCustomer.Orders[_order1.ID], Is.SameAs(_order1));
        Assert.That(_newCustomer.Orders[_order1.ID], Is.Null);
      }
    }

    [Test]
    public void NewCustomerCancelsChangeEvent ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] {
        _oldCustomer, _newCustomer, _order1};

      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] {
        _oldCustomer.Orders, _newCustomer.Orders};

      SequenceEventReceiver eventReceiver = new SequenceEventReceiver(domainObjectEventSources, collectionEventSources, 3);

      try
      {
        _newCustomer.Orders.Add(_order1);
        Assert.Fail("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedChangeStates =
            new ChangeState[]
            {
                new RelationChangeState(
                    _order1,
                    "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer",
                    _oldCustomer,
                    _newCustomer,
                    "1. Changing event of order from old to new customer"),
                new CollectionChangeState(_newCustomer.Orders, _order1, "2. Adding event of new customer's order collection"),
                new RelationChangeState(_newCustomer, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", null, _order1, "3. Changing event of new customer")
            };

        eventReceiver.Check(expectedChangeStates);

        Assert.That(_order1.State.IsUnchanged, Is.True);
        Assert.That(_oldCustomer.State.IsUnchanged, Is.True);
        Assert.That(_newCustomer.State.IsUnchanged, Is.True);

        Assert.That(_order1.Customer, Is.SameAs(_oldCustomer));
        Assert.That(_oldCustomer.Orders[_order1.ID], Is.SameAs(_order1));
        Assert.That(_newCustomer.Orders[_order1.ID], Is.Null);
      }
    }

    [Test]
    public void OldCustomerCollectionCancelsChangeEvent ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] {
        _oldCustomer, _newCustomer, _order1};

      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] {
        _oldCustomer.Orders, _newCustomer.Orders};

      SequenceEventReceiver eventReceiver = new SequenceEventReceiver(domainObjectEventSources, collectionEventSources, 4);

      try
      {
        _newCustomer.Orders.Add(_order1);
        Assert.Fail("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedChangeStates =
            new ChangeState[]
            {
                new RelationChangeState(
                    _order1,
                    "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer",
                    _oldCustomer,
                    _newCustomer,
                    "1. Changing event of order from old to new customer"),
                new CollectionChangeState(_newCustomer.Orders, _order1, "2. Adding event of new customer's order collection"),
                new RelationChangeState(_newCustomer, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", null, _order1, "3. Changing event of new customer"),
                new CollectionChangeState(_oldCustomer.Orders, _order1, "4. Removing of orders of old customer")
            };

        eventReceiver.Check(expectedChangeStates);

        Assert.That(_order1.State.IsUnchanged, Is.True);
        Assert.That(_oldCustomer.State.IsUnchanged, Is.True);
        Assert.That(_newCustomer.State.IsUnchanged, Is.True);

        Assert.That(_order1.Customer, Is.SameAs(_oldCustomer));
        Assert.That(_oldCustomer.Orders[_order1.ID], Is.SameAs(_order1));
        Assert.That(_newCustomer.Orders[_order1.ID], Is.Null);
      }
    }

    [Test]
    public void OldCustomerCancelsChangeEvent ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] {
        _oldCustomer, _newCustomer, _order1};

      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] {
        _oldCustomer.Orders, _newCustomer.Orders};

      SequenceEventReceiver eventReceiver = new SequenceEventReceiver(domainObjectEventSources, collectionEventSources, 5);

      try
      {
        _newCustomer.Orders.Add(_order1);
        Assert.Fail("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedChangeStates =
            new ChangeState[]
            {
                new RelationChangeState(
                    _order1,
                    "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer",
                    _oldCustomer,
                    _newCustomer,
                    "1. Changing event of order from old to new customer"),
                new CollectionChangeState(_newCustomer.Orders, _order1, "2. Adding event of new customer's order collection"),
                new RelationChangeState(_newCustomer, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", null, _order1, "3. Changing event of new customer"),
                new CollectionChangeState(_oldCustomer.Orders, _order1, "4. Removing of orders of old customer"),
                new RelationChangeState(_oldCustomer, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", _order1, null, "5. Changing event of old customer")
            };

        eventReceiver.Check(expectedChangeStates);

        Assert.That(_order1.State.IsUnchanged, Is.True);
        Assert.That(_oldCustomer.State.IsUnchanged, Is.True);
        Assert.That(_newCustomer.State.IsUnchanged, Is.True);

        Assert.That(_order1.Customer, Is.SameAs(_oldCustomer));
        Assert.That(_oldCustomer.Orders[_order1.ID], Is.SameAs(_order1));
        Assert.That(_newCustomer.Orders[_order1.ID], Is.Null);
      }
    }

    [Test]
    public void StateTracking ()
    {
      _newCustomer.Orders.Add(_order1);

      Assert.That(_order1.State.IsChanged, Is.True);
      Assert.That(_oldCustomer.State.IsChanged, Is.True);
      Assert.That(_newCustomer.State.IsChanged, Is.True);
    }

    [Test]
    public void ChangeWithInheritance ()
    {
      IndustrialSector industrialSector = DomainObjectIDs.IndustrialSector1.GetObject<IndustrialSector>();
      Partner partner = DomainObjectIDs.Partner2.GetObject<Partner>();

      Assert.That(industrialSector.Companies[partner.ID], Is.Null);
      Assert.That(ReferenceEquals(industrialSector, partner.IndustrialSector), Is.False);

      industrialSector.Companies.Add(partner);

      Assert.That(industrialSector.Companies[partner.ID], Is.Not.Null);
      Assert.That(partner.IndustrialSector, Is.SameAs(industrialSector));
    }

    [Test]
    public void SetNewCustomerThroughOrder ()
    {
      _order1.Customer = _newCustomer;

      Assert.That(_order1.State.IsChanged, Is.True);
      Assert.That(_oldCustomer.State.IsChanged, Is.True);
      Assert.That(_newCustomer.State.IsChanged, Is.True);

      Assert.That(_order1.Customer, Is.SameAs(_newCustomer));
      Assert.That(_oldCustomer.Orders[_order1.ID], Is.Null);
      Assert.That(_newCustomer.Orders[_order1.ID], Is.SameAs(_order1));

      Assert.That(_order1.InternalDataContainer.State.IsChanged, Is.True);
      Assert.That(_newCustomer.InternalDataContainer.State.IsUnchanged, Is.True);
      Assert.That(_oldCustomer.InternalDataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void ChangeRelationBackToOriginalValue ()
    {
      _order1.Customer = _newCustomer;
      Assert.That(_order1.State.IsChanged, Is.True);
      Assert.That(_oldCustomer.State.IsChanged, Is.True);
      Assert.That(_newCustomer.State.IsChanged, Is.True);

      _order1.Customer = _oldCustomer;
      Assert.That(_order1.State.IsUnchanged, Is.True);
      Assert.That(_oldCustomer.State.IsUnchanged, Is.True);
      Assert.That(_newCustomer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void SetOriginalValue ()
    {
      _order1.Customer = _order1.Customer;
      Assert.That(_order1.State.IsUnchanged, Is.True);
      Assert.That(_order1.Customer.State.IsUnchanged, Is.True);

      Assert.That(_order1.InternalDataContainer.State.IsUnchanged, Is.True);
      Assert.That(_order1.Customer.InternalDataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void HasBeenTouched_FromOneProperty ()
    {
      CheckTouching(delegate { _order1.Customer = _newCustomer; }, _order1, "Customer",
        RelationEndPointID.Create(_order1.ID, typeof(Order).FullName + ".Customer"),
        RelationEndPointID.Create(_newCustomer.ID, typeof(Customer).FullName + ".Orders"),
        RelationEndPointID.Create(_oldCustomer.ID, typeof(Customer).FullName + ".Orders"));
    }

    [Test]
    public void HasBeenTouched_FromManyPropertyAdd ()
    {
      CheckTouching(delegate { _newCustomer.Orders.Add(_order1); }, _order1, "Customer",
          RelationEndPointID.Create(_order1.ID, typeof(Order).FullName + ".Customer"),
          RelationEndPointID.Create(_newCustomer.ID, typeof(Customer).FullName + ".Orders"),
          RelationEndPointID.Create(_oldCustomer.ID, typeof(Customer).FullName + ".Orders"));
    }

    [Test]
    public void HasBeenTouched_FromManyPropertyRemove ()
    {
      CheckTouching(delegate { _oldCustomer.Orders.Remove(_order1); }, _order1, "Customer",
          RelationEndPointID.Create(_order1.ID, typeof(Order).FullName + ".Customer"),
          RelationEndPointID.Create(_oldCustomer.ID, typeof(Customer).FullName + ".Orders"));
    }

    [Test]
    public void HasBeenTouched_FromManyPropertyReplaceWithNull ()
    {
      CheckTouching(delegate { _oldCustomer.Orders[_oldCustomer.Orders.IndexOf(_order1)] = null; }, _order1, "Customer",
          RelationEndPointID.Create(_order1.ID, typeof(Order).FullName + ".Customer"),
          RelationEndPointID.Create(_oldCustomer.ID, typeof(Customer).FullName + ".Orders"));
    }

    [Test]
    public void HasBeenTouched_FromManyPropertyReplaceWithNew ()
    {
      Order newOrder = Order.NewObject();

      Assert.IsFalse(newOrder.InternalDataContainer.HasValueBeenTouched(GetPropertyDefinition(typeof(Order), "Customer")), "newOrder ObjectID touched");

      CheckTouching(delegate { _oldCustomer.Orders[_oldCustomer.Orders.IndexOf(_order1)] = newOrder; }, _order1, "Customer",
        RelationEndPointID.Create(_order1.ID, typeof(Order).FullName + ".Customer"),
        RelationEndPointID.Create(newOrder.ID, typeof(Order).FullName + ".Customer"),
        RelationEndPointID.Create(_oldCustomer.ID, typeof(Customer).FullName + ".Orders"));

      Assert.That(newOrder.InternalDataContainer.HasValueBeenTouched(GetPropertyDefinition(typeof(Order), "Customer")), Is.True, "newOrder ObjectID touched");
    }

    [Test]
    public void HasBeenTouched_FromOneProperty_OriginalValue ()
    {
      CheckTouching(delegate { _order1.Customer = _order1.Customer; }, _order1, "Customer",
          RelationEndPointID.Create(_order1.ID, typeof(Order).FullName + ".Customer"),
          RelationEndPointID.Create(_oldCustomer.ID, typeof(Customer).FullName + ".Orders"));
    }

    [Test]
    public void HasBeenTouched_FromManyPropertyReplace_OriginalValue ()
    {
      CheckTouching(delegate { _oldCustomer.Orders[_oldCustomer.Orders.IndexOf(_order1)] = _order1; }, _order1, "Customer",
          RelationEndPointID.Create(_order1.ID, typeof(Order).FullName + ".Customer"),
          RelationEndPointID.Create(_oldCustomer.ID, typeof(Customer).FullName + ".Orders"),
          RelationEndPointID.Create(_oldCustomer.ID, typeof(Customer).FullName + ".Orders"));
    }

    [Test]
    public void GetOriginalRelatedObject ()
    {
      _order1.Customer = _newCustomer;

      Assert.That(_order1.Customer, Is.SameAs(_newCustomer));
      Assert.That(_order1.GetOriginalRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"), Is.SameAs(_oldCustomer));
    }

    [Test]
    public void GetOriginalRelatedObjects ()
    {
      Assert.That(_newCustomer.Orders[_order1.ID], Is.Null);

      _newCustomer.Orders.Add(_order1);

      DomainObjectCollection oldOrders = _newCustomer.GetOriginalRelatedObjectsAsDomainObjectCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");
      Assert.That(_newCustomer.Orders[_order1.ID], Is.SameAs(_order1));
      Assert.That(oldOrders[_order1.ID], Is.Null);
    }

    [Test]
    public void GetOriginalRelatedObjectsWithLazyLoad ()
    {
      Employee supervisor = DomainObjectIDs.Employee1.GetObject<Employee>();
      DomainObjectCollection subordinates =
          supervisor.GetOriginalRelatedObjectsAsDomainObjectCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates");

      Assert.That(subordinates.Count, Is.EqualTo(2));
    }

    [Test]
    public void CheckRequiredItemTypeForExisting ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      DomainObjectCollection orderItems = order.OrderItems;
      Assert.That(
          () => orderItems.Add(Customer.NewObject()),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Values of type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer' cannot be added to this collection. "
                  + "Values must be of type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem' "
                  + "or derived from 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem'.", "domainObject"));
    }

    [Test]
    public void CheckRequiredItemTypeForNew ()
    {
      Order order = Order.NewObject();
      DomainObjectCollection orderItems = order.OrderItems;
      Assert.That(
          () => orderItems.Add(Customer.NewObject()),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Values of type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer' cannot be added to this collection. "
                  + "Values must be of type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem' "
                  + "or derived from 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem'.", "domainObject"));
    }

    [Test]
    public void SetRelatedObjectWithInvalidObjectClass ()
    {
      Assert.That(
          () => _order1.SetRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", DomainObjectIDs.Company1.GetObject<Company>()),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "DomainObject 'Company|c4954da8-8870-45c1-b7a3-c7e5e6ad641a|System.Guid' cannot be assigned "
                  + "to property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' "
                  + "of DomainObject 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid', because it is not compatible with the type of the property.",
                  "newRelatedObject"));
    }

    [Test]
    public void Clear_Events ()
    {
      Assert.That(_oldCustomer.Orders, Is.EqualTo(new[] { _order1, _order2 }));

      var eventReceiver = new SequenceEventReceiver(
          new DomainObject[] { _oldCustomer, _order1, _order2 },
          new[] { _oldCustomer.Orders });

      _oldCustomer.Orders.Clear();

      var expectedStates = new ChangeState[]
      {
        new RelationChangeState(_order2, typeof(Order).FullName + ".Customer", _oldCustomer, null, "1. Setting _order2.Customer to null"),
        new CollectionChangeState(_oldCustomer.Orders, _order2, "2. Removing _order2 from _oldCustomer.Orders"),
        new RelationChangeState(_oldCustomer, typeof(Customer).FullName + ".Orders", _order2, null, "3. Removing _order2 from _oldCustomer"),

        new RelationChangeState(_order1, typeof(Order).FullName + ".Customer", _oldCustomer, null, "4. Setting _order1.Customer to null"),
        new CollectionChangeState(_oldCustomer.Orders, _order1, "5. Removing _order1 from _oldCustomer.Orders"),
        new RelationChangeState(_oldCustomer, typeof(Customer).FullName + ".Orders", _order1, null, "6. Removing _order1 from _oldCustomer"),

        new RelationChangeState(_oldCustomer, typeof(Customer).FullName + ".Orders", null, null, "7. Removed _order1 from _oldCustomer"),
        new CollectionChangeState(_oldCustomer.Orders, _order1, "8. Removed _order1 from _oldCustomer.Orders"),
        new RelationChangeState(_order1, typeof(Order).FullName + ".Customer", null, null, "9. Setting _order1.Customer to null"),

        new RelationChangeState(_oldCustomer, typeof(Customer).FullName + ".Orders", null, null, "10. Removed _order2 from _oldCustomer"),
        new CollectionChangeState(_oldCustomer.Orders, _order2, "11. Removed _order2 from _oldCustomer.Orders"),
        new RelationChangeState(_order2, typeof(Order).FullName + ".Customer", null, null, "12. Set _order2.Customer to null"),
      };

      eventReceiver.Check(expectedStates);

      Assert.That(_oldCustomer.Orders, Is.Empty);
      Assert.That(_order2.Customer, Is.Null);
      Assert.That(_order1.Customer, Is.Null);
      Assert.That(DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint(_oldCustomer.Orders).HasBeenTouched, Is.True);
    }

    [Test]
    public void Clear_CancelAtSecondObject ()
    {
      Assert.That(_oldCustomer.Orders, Is.EqualTo(new[] { _order1, _order2 }));

      var eventReceiver = new SequenceEventReceiver(
          new DomainObject[] { _oldCustomer, _order1, _order2 },
          new[] { _oldCustomer.Orders });

      eventReceiver.CancelEventNumber = 6;

      try
      {
        _oldCustomer.Orders.Clear();
        Assert.Fail("Expected cancellation");
      }
      catch (EventReceiverCancelException)
      {
        // ok
      }

      var expectedStates = new ChangeState[]
      {
        new RelationChangeState(_order2, typeof(Order).FullName + ".Customer", _oldCustomer, null, "1. Setting _order2.Customer to null"),
        new CollectionChangeState(_oldCustomer.Orders, _order2, "2. Removing _order2 from _oldCustomer.Orders"),
        new RelationChangeState(_oldCustomer, typeof(Customer).FullName + ".Orders", _order2, null, "3. Removing _order2 from _oldCustomer"),

        new RelationChangeState(_order1, typeof(Order).FullName + ".Customer", _oldCustomer, null, "4. Setting _order1.Customer to null"),
        new CollectionChangeState(_oldCustomer.Orders, _order1, "5. Removing _order1 from _oldCustomer.Orders"),
        new RelationChangeState(_oldCustomer, typeof(Customer).FullName + ".Orders", _order1, null, "6. Removing _order1 from _oldCustomer"),
      };

      eventReceiver.Check(expectedStates);

      Assert.That(_oldCustomer.Orders, Is.EqualTo(new[] { _order1, _order2 }));
      Assert.That(_order2.Customer, Is.SameAs(_oldCustomer));
      Assert.That(_order1.Customer, Is.SameAs(_oldCustomer));
      Assert.That(DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint(_oldCustomer.Orders).HasBeenTouched, Is.False);
    }
  }
}
