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

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class OneToOneRelationChangeTest : RelationChangeBaseTest
  {
    private Order _order;
    private OrderTicket _oldOrderTicket;
    private OrderTicket _newOrderTicket;
    private Order _oldOrderOfNewOrderTicket;

    private DomainObjectEventReceiver _orderEventReceiver;
    private DomainObjectEventReceiver _oldOrderTicketEventReceiver;
    private DomainObjectEventReceiver _newOrderTicketEventReceiver;
    private DomainObjectEventReceiver _oldOrderOfNewOrderTicketEventReceiver;

    public override void SetUp ()
    {
      base.SetUp ();

      _order = DomainObjectIDs.Order1.GetObject<Order> ();
      _oldOrderTicket = _order.OrderTicket;
      _newOrderTicket = DomainObjectIDs.OrderTicket2.GetObject<OrderTicket> ();
      _oldOrderOfNewOrderTicket = DomainObjectIDs.Order2.GetObject<Order> ();

      _orderEventReceiver = new DomainObjectEventReceiver (_order);
      _oldOrderTicketEventReceiver = new DomainObjectEventReceiver (_oldOrderTicket);
      _newOrderTicketEventReceiver = new DomainObjectEventReceiver (_newOrderTicket);
      _oldOrderOfNewOrderTicketEventReceiver = new DomainObjectEventReceiver (_oldOrderOfNewOrderTicket);
    }

    [Test]
    public void RelationChangeEvents ()
    {
      _orderEventReceiver.Cancel = false;
      _oldOrderTicketEventReceiver.Cancel = false;
      _newOrderTicketEventReceiver.Cancel = false;
      _oldOrderOfNewOrderTicketEventReceiver.Cancel = false;

      _order.OrderTicket = _newOrderTicket;

      Assert.That (_orderEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (true));
      Assert.That (_orderEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo (true));
      Assert.That (_orderEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));
      Assert.That (_orderEventReceiver.ChangedRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));
      Assert.That (_orderEventReceiver.ChangingOldRelatedObject, Is.SameAs (_oldOrderTicket));
      Assert.That (_orderEventReceiver.ChangingNewRelatedObject, Is.SameAs (_newOrderTicket));
      Assert.That (_orderEventReceiver.ChangedOldRelatedObject, Is.SameAs (_oldOrderTicket));
      Assert.That (_orderEventReceiver.ChangedNewRelatedObject, Is.SameAs (_newOrderTicket));

      Assert.That (_oldOrderTicketEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (true));
      Assert.That (_oldOrderTicketEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo (true));
      Assert.That (_oldOrderTicketEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"));
      Assert.That (_oldOrderTicketEventReceiver.ChangedRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"));
      Assert.That (_oldOrderTicketEventReceiver.ChangingOldRelatedObject, Is.SameAs (_order));
      Assert.That (_oldOrderTicketEventReceiver.ChangingNewRelatedObject, Is.SameAs (null));
      Assert.That (_oldOrderTicketEventReceiver.ChangedOldRelatedObject, Is.SameAs (_order));
      Assert.That (_oldOrderTicketEventReceiver.ChangedNewRelatedObject, Is.SameAs (null));

      Assert.That (_newOrderTicketEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (true));
      Assert.That (_newOrderTicketEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo (true));
      Assert.That (_newOrderTicketEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"));
      Assert.That (_newOrderTicketEventReceiver.ChangedRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"));
      Assert.That (_newOrderTicketEventReceiver.ChangingOldRelatedObject, Is.SameAs (_oldOrderOfNewOrderTicket));
      Assert.That (_newOrderTicketEventReceiver.ChangingNewRelatedObject, Is.SameAs (_order));
      Assert.That (_newOrderTicketEventReceiver.ChangedOldRelatedObject, Is.SameAs (_oldOrderOfNewOrderTicket));
      Assert.That (_newOrderTicketEventReceiver.ChangedNewRelatedObject, Is.SameAs (_order));

      Assert.That (_oldOrderOfNewOrderTicketEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (true));
      Assert.That (_oldOrderOfNewOrderTicketEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo (true));
      Assert.That (_oldOrderOfNewOrderTicketEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));
      Assert.That (_oldOrderOfNewOrderTicketEventReceiver.ChangedRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));
      Assert.That (_oldOrderOfNewOrderTicketEventReceiver.ChangingOldRelatedObject, Is.SameAs (_newOrderTicket));
      Assert.That (_oldOrderOfNewOrderTicketEventReceiver.ChangingNewRelatedObject, Is.Null);
      Assert.That (_oldOrderOfNewOrderTicketEventReceiver.ChangedOldRelatedObject, Is.SameAs (_newOrderTicket));
      Assert.That (_oldOrderOfNewOrderTicketEventReceiver.ChangedNewRelatedObject, Is.Null);

      Assert.That (_order.State, Is.EqualTo (StateType.Changed));
      Assert.That (_newOrderTicket.State, Is.EqualTo (StateType.Changed));
      Assert.That (_oldOrderTicket.State, Is.EqualTo (StateType.Changed));
      Assert.That (_oldOrderOfNewOrderTicket.State, Is.EqualTo (StateType.Changed));

      Assert.That (_order.InternalDataContainer.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (_newOrderTicket.InternalDataContainer.State, Is.EqualTo (StateType.Changed));
      Assert.That (_oldOrderTicket.InternalDataContainer.State, Is.EqualTo (StateType.Changed));
      Assert.That (_oldOrderOfNewOrderTicket.InternalDataContainer.State, Is.EqualTo (StateType.Unchanged));

      Assert.That (_order.OrderTicket, Is.SameAs (_newOrderTicket));
      Assert.That (_newOrderTicket.Order, Is.SameAs (_order));
      Assert.That (_oldOrderTicket.Order, Is.Null);
      Assert.That (_oldOrderOfNewOrderTicket.OrderTicket, Is.Null);
    }

    [Test]
    public void OrderCancelsRelationChangeEvent ()
    {
      _orderEventReceiver.Cancel = true;
      _oldOrderTicketEventReceiver.Cancel = false;
      _newOrderTicketEventReceiver.Cancel = false;
      _oldOrderOfNewOrderTicketEventReceiver.Cancel = false;

      try
      {
        _order.OrderTicket = _newOrderTicket;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.That (_orderEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (true));
        Assert.That (_orderEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo (false));
        Assert.That (_orderEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));
        Assert.That (_orderEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_orderEventReceiver.ChangingOldRelatedObject, Is.SameAs (_oldOrderTicket));
        Assert.That (_orderEventReceiver.ChangingNewRelatedObject, Is.SameAs (_newOrderTicket));

        Assert.That (_oldOrderTicketEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (false));
        Assert.That (_oldOrderTicketEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo (false));
        Assert.That (_oldOrderTicketEventReceiver.ChangingRelationPropertyName, Is.Null);
        Assert.That (_oldOrderTicketEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_oldOrderTicketEventReceiver.ChangingOldRelatedObject, Is.Null);
        Assert.That (_oldOrderTicketEventReceiver.ChangingNewRelatedObject, Is.Null);

        Assert.That (_newOrderTicketEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (false));
        Assert.That (_newOrderTicketEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo (false));
        Assert.That (_newOrderTicketEventReceiver.ChangingRelationPropertyName, Is.Null);
        Assert.That (_newOrderTicketEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_newOrderTicketEventReceiver.ChangingOldRelatedObject, Is.Null);
        Assert.That (_newOrderTicketEventReceiver.ChangingNewRelatedObject, Is.Null);

        Assert.That (_oldOrderOfNewOrderTicketEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (false));
        Assert.That (_oldOrderOfNewOrderTicketEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo (false));
        Assert.That (_oldOrderOfNewOrderTicketEventReceiver.ChangingRelationPropertyName, Is.Null);
        Assert.That (_oldOrderOfNewOrderTicketEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_oldOrderOfNewOrderTicketEventReceiver.ChangingOldRelatedObject, Is.Null);
        Assert.That (_oldOrderOfNewOrderTicketEventReceiver.ChangingNewRelatedObject, Is.Null);

        Assert.That (_order.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_newOrderTicket.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_oldOrderTicket.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_oldOrderOfNewOrderTicket.State, Is.EqualTo (StateType.Unchanged));

        Assert.That (_order.OrderTicket, Is.SameAs (_oldOrderTicket));
        Assert.That (_newOrderTicket.Order, Is.SameAs (_oldOrderOfNewOrderTicket));
        Assert.That (_oldOrderTicket.Order, Is.SameAs (_order));
        Assert.That (_oldOrderOfNewOrderTicket.OrderTicket, Is.SameAs (_newOrderTicket));
      }
    }

    [Test]
    public void OldRelatedObjectCancelsRelationChange ()
    {
      _orderEventReceiver.Cancel = false;
      _oldOrderTicketEventReceiver.Cancel = true;
      _newOrderTicketEventReceiver.Cancel = false;
      _oldOrderOfNewOrderTicketEventReceiver.Cancel = false;

      try
      {
        _order.OrderTicket = _newOrderTicket;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.That (_orderEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (true));
        Assert.That (_orderEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo (false));
        Assert.That (_orderEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));
        Assert.That (_orderEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_orderEventReceiver.ChangingOldRelatedObject, Is.SameAs (_oldOrderTicket));
        Assert.That (_orderEventReceiver.ChangingNewRelatedObject, Is.SameAs (_newOrderTicket));

        Assert.That (_oldOrderTicketEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (true));
        Assert.That (_oldOrderTicketEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo (false));
        Assert.That (_oldOrderTicketEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"));
        Assert.That (_oldOrderTicketEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_oldOrderTicketEventReceiver.ChangingOldRelatedObject, Is.SameAs (_order));
        Assert.That (_oldOrderTicketEventReceiver.ChangingNewRelatedObject, Is.SameAs (null));

        Assert.That (_newOrderTicketEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (false));
        Assert.That (_newOrderTicketEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo (false));
        Assert.That (_newOrderTicketEventReceiver.ChangingRelationPropertyName, Is.Null);
        Assert.That (_newOrderTicketEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_newOrderTicketEventReceiver.ChangingOldRelatedObject, Is.Null);
        Assert.That (_newOrderTicketEventReceiver.ChangingNewRelatedObject, Is.Null);

        Assert.That (_oldOrderOfNewOrderTicketEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (false));
        Assert.That (_oldOrderOfNewOrderTicketEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo (false));
        Assert.That (_oldOrderOfNewOrderTicketEventReceiver.ChangingRelationPropertyName, Is.Null);
        Assert.That (_oldOrderOfNewOrderTicketEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_oldOrderOfNewOrderTicketEventReceiver.ChangingOldRelatedObject, Is.Null);
        Assert.That (_oldOrderOfNewOrderTicketEventReceiver.ChangingNewRelatedObject, Is.Null);

        Assert.That (_order.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_newOrderTicket.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_oldOrderTicket.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_oldOrderOfNewOrderTicket.State, Is.EqualTo (StateType.Unchanged));

        Assert.That (_order.OrderTicket, Is.SameAs (_oldOrderTicket));
        Assert.That (_newOrderTicket.Order, Is.SameAs (_oldOrderOfNewOrderTicket));
        Assert.That (_oldOrderTicket.Order, Is.SameAs (_order));
        Assert.That (_oldOrderOfNewOrderTicket.OrderTicket, Is.SameAs (_newOrderTicket));
      }
    }

    [Test]
    public void NewRelatedObjectCancelsRelationChange ()
    {
      _orderEventReceiver.Cancel = false;
      _oldOrderTicketEventReceiver.Cancel = false;
      _newOrderTicketEventReceiver.Cancel = true;
      _oldOrderOfNewOrderTicketEventReceiver.Cancel = false;

      try
      {
        _order.OrderTicket = _newOrderTicket;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.That (_orderEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (true));
        Assert.That (_orderEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo (false));
        Assert.That (_orderEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));
        Assert.That (_orderEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_orderEventReceiver.ChangingOldRelatedObject, Is.SameAs (_oldOrderTicket));
        Assert.That (_orderEventReceiver.ChangingNewRelatedObject, Is.SameAs (_newOrderTicket));

        Assert.That (_oldOrderTicketEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (true));
        Assert.That (_oldOrderTicketEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo (false));
        Assert.That (_oldOrderTicketEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"));
        Assert.That (_oldOrderTicketEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_oldOrderTicketEventReceiver.ChangingOldRelatedObject, Is.SameAs (_order));
        Assert.That (_oldOrderTicketEventReceiver.ChangingNewRelatedObject, Is.SameAs (null));

        Assert.That (_newOrderTicketEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (true));
        Assert.That (_newOrderTicketEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo (false));
        Assert.That (_newOrderTicketEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"));
        Assert.That (_newOrderTicketEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_newOrderTicketEventReceiver.ChangingOldRelatedObject, Is.SameAs (_oldOrderOfNewOrderTicket));
        Assert.That (_newOrderTicketEventReceiver.ChangingNewRelatedObject, Is.SameAs (_order));

        Assert.That (_oldOrderOfNewOrderTicketEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (false));
        Assert.That (_oldOrderOfNewOrderTicketEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo (false));
        Assert.That (_oldOrderOfNewOrderTicketEventReceiver.ChangingRelationPropertyName, Is.Null);
        Assert.That (_oldOrderOfNewOrderTicketEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_oldOrderOfNewOrderTicketEventReceiver.ChangingOldRelatedObject, Is.Null);
        Assert.That (_oldOrderOfNewOrderTicketEventReceiver.ChangingNewRelatedObject, Is.Null);

        Assert.That (_order.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_newOrderTicket.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_oldOrderTicket.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_oldOrderOfNewOrderTicket.State, Is.EqualTo (StateType.Unchanged));

        Assert.That (_order.OrderTicket, Is.SameAs (_oldOrderTicket));
        Assert.That (_newOrderTicket.Order, Is.SameAs (_oldOrderOfNewOrderTicket));
        Assert.That (_oldOrderTicket.Order, Is.SameAs (_order));
        Assert.That (_oldOrderOfNewOrderTicket.OrderTicket, Is.SameAs (_newOrderTicket));
      }
    }

    [Test]
    public void OldRelatedObjectOfNewRelatedObjectCancelsRelationChange ()
    {
      _orderEventReceiver.Cancel = false;
      _oldOrderTicketEventReceiver.Cancel = false;
      _newOrderTicketEventReceiver.Cancel = false;
      _oldOrderOfNewOrderTicketEventReceiver.Cancel = true;

      try
      {
        _order.OrderTicket = _newOrderTicket;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.That (_orderEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (true));
        Assert.That (_orderEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo (false));
        Assert.That (_orderEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));
        Assert.That (_orderEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_orderEventReceiver.ChangingOldRelatedObject, Is.SameAs (_oldOrderTicket));
        Assert.That (_orderEventReceiver.ChangingNewRelatedObject, Is.SameAs (_newOrderTicket));

        Assert.That (_oldOrderTicketEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (true));
        Assert.That (_oldOrderTicketEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo (false));
        Assert.That (_oldOrderTicketEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"));
        Assert.That (_oldOrderTicketEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_oldOrderTicketEventReceiver.ChangingOldRelatedObject, Is.SameAs (_order));
        Assert.That (_oldOrderTicketEventReceiver.ChangingNewRelatedObject, Is.SameAs (null));

        Assert.That (_newOrderTicketEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (true));
        Assert.That (_newOrderTicketEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo (false));
        Assert.That (_newOrderTicketEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"));
        Assert.That (_newOrderTicketEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_newOrderTicketEventReceiver.ChangingOldRelatedObject, Is.SameAs (_oldOrderOfNewOrderTicket));
        Assert.That (_newOrderTicketEventReceiver.ChangingNewRelatedObject, Is.SameAs (_order));

        Assert.That (_oldOrderOfNewOrderTicketEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (true));
        Assert.That (_oldOrderOfNewOrderTicketEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo (false));
        Assert.That (_oldOrderOfNewOrderTicketEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));
        Assert.That (_oldOrderOfNewOrderTicketEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_oldOrderOfNewOrderTicketEventReceiver.ChangingOldRelatedObject, Is.SameAs (_newOrderTicket));
        Assert.That (_oldOrderOfNewOrderTicketEventReceiver.ChangingNewRelatedObject, Is.Null);

        Assert.That (_order.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_newOrderTicket.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_oldOrderTicket.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_oldOrderOfNewOrderTicket.State, Is.EqualTo (StateType.Unchanged));

        Assert.That (_order.OrderTicket, Is.SameAs (_oldOrderTicket));
        Assert.That (_newOrderTicket.Order, Is.SameAs (_oldOrderOfNewOrderTicket));
        Assert.That (_oldOrderTicket.Order, Is.SameAs (_order));
        Assert.That (_oldOrderOfNewOrderTicket.OrderTicket, Is.SameAs (_newOrderTicket));
      }
    }

    [Test]
    public void StateTracking ()
    {
      Assert.That (_order.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (_newOrderTicket.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (_oldOrderTicket.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (_oldOrderOfNewOrderTicket.State, Is.EqualTo (StateType.Unchanged));

      _order.OrderTicket = _newOrderTicket;

      Assert.That (_order.State, Is.EqualTo (StateType.Changed));
      Assert.That (_newOrderTicket.State, Is.EqualTo (StateType.Changed));
      Assert.That (_oldOrderTicket.State, Is.EqualTo (StateType.Changed));
      Assert.That (_oldOrderOfNewOrderTicket.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void OldObjectAndNewObjectAreSame ()
    {
      Assert.That (_order.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (_oldOrderTicket.State, Is.EqualTo (StateType.Unchanged));

      _order.OrderTicket = _oldOrderTicket;

      Assert.That (_order.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (_oldOrderTicket.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void ChangeRelationOverVirtualEndPoint ()
    {
      _order.OrderTicket = _newOrderTicket;

      Assert.That (_oldOrderTicket.Properties[typeof (OrderTicket), "Order"].GetRelatedObjectID(), Is.Null);
      Assert.That (_newOrderTicket.Properties[typeof (OrderTicket), "Order"].GetRelatedObjectID (), Is.EqualTo (_order.ID));

      Assert.That (_order.OrderTicket, Is.SameAs (_newOrderTicket));
      Assert.That (_newOrderTicket.Order, Is.SameAs (_order));
      Assert.That (_oldOrderTicket.Order, Is.Null);
      Assert.That (_oldOrderOfNewOrderTicket.OrderTicket, Is.Null);
    }

    [Test]
    public void ChangeRelation ()
    {
      _newOrderTicket.Order = _order;

      Assert.That (_oldOrderTicket.Properties[typeof (OrderTicket), "Order"].GetRelatedObjectID (), Is.Null);
      Assert.That (_newOrderTicket.Properties[typeof (OrderTicket), "Order"].GetRelatedObjectID (), Is.EqualTo (_order.ID));

      Assert.That (_newOrderTicket.Order, Is.SameAs (_order));
      Assert.That (_order.OrderTicket, Is.SameAs (_newOrderTicket));
      Assert.That (_oldOrderTicket.Order, Is.Null);
      Assert.That (_oldOrderOfNewOrderTicket.OrderTicket, Is.Null);
    }

    [Test]
    public void ChangeRelationWithInheritance ()
    {
      Person person = DomainObjectIDs.Person1.GetObject<Person>();
      Distributor distributor = DomainObjectIDs.Distributor1.GetObject<Distributor> ();

      person.AssociatedPartnerCompany = distributor;

      Assert.That (person.AssociatedPartnerCompany, Is.SameAs (distributor));
      Assert.That (distributor.ContactPerson, Is.SameAs (person));
    }

    [Test]
    public void ChangeRelationBackToOriginalValue ()
    {
      _order.OrderTicket = _newOrderTicket;
      Assert.That (_order.State, Is.EqualTo (StateType.Changed));

      _order.OrderTicket = _oldOrderTicket;
      Assert.That (_order.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void HasBeenTouched_VirtualSide ()
    {
      Order oldOrder = _newOrderTicket.Order;

      CheckTouching (delegate { _order.OrderTicket = _newOrderTicket; }, _newOrderTicket, "Order",
          RelationEndPointID.Create(_order.ID, typeof (Order).FullName + ".OrderTicket"),
          RelationEndPointID.Create(oldOrder.ID, typeof (Order).FullName + ".OrderTicket"),
          RelationEndPointID.Create(_oldOrderTicket.ID, typeof (OrderTicket).FullName + ".Order"),
          RelationEndPointID.Create(_newOrderTicket.ID, typeof (OrderTicket).FullName + ".Order"));
    }

    [Test]
    public void HasBeenTouched_RealSide ()
    {
      Order oldOrder = _newOrderTicket.Order;

      Assert.That (_oldOrderTicket.InternalDataContainer.HasValueBeenTouched (GetPropertyDefinition (typeof (OrderTicket), "Order")), Is.False);

      CheckTouching (delegate { _newOrderTicket.Order = _order; }, _newOrderTicket, "Order",
          RelationEndPointID.Create(_order.ID, typeof (Order).FullName + ".OrderTicket"),
          RelationEndPointID.Create(oldOrder.ID, typeof (Order).FullName + ".OrderTicket"),
          RelationEndPointID.Create(_oldOrderTicket.ID, typeof (OrderTicket).FullName + ".Order"),
          RelationEndPointID.Create(_newOrderTicket.ID, typeof (OrderTicket).FullName + ".Order"));

      Assert.That (_oldOrderTicket.InternalDataContainer.HasValueBeenTouched (GetPropertyDefinition (typeof (OrderTicket), "Order")), Is.True);
    }

    [Test]
    public void HasBeenTouched_VirtualSide_OriginalValue ()
    {
      CheckTouching (delegate { _order.OrderTicket = _order.OrderTicket; }, _order.OrderTicket, "Order",
          RelationEndPointID.Create(_order.ID, typeof (Order).FullName + ".OrderTicket"),
          RelationEndPointID.Create(_oldOrderTicket.ID, typeof (OrderTicket).FullName + ".Order"));
    }

    [Test]
    public void HasBeenTouched_RealSide_OriginalValue ()
    {
      CheckTouching (delegate { _oldOrderTicket.Order = _order; }, _oldOrderTicket, "Order",
          RelationEndPointID.Create(_order.ID, typeof (Order).FullName + ".OrderTicket"),
          RelationEndPointID.Create(_oldOrderTicket.ID, typeof (OrderTicket).FullName + ".Order"));
    }

    [Test]
    public void GetOriginalRelatedObject ()
    {
      Assert.That (_order.GetOriginalRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"), Is.SameAs (_oldOrderTicket));

      _order.OrderTicket = _newOrderTicket;

      Assert.That (_order.OrderTicket, Is.SameAs (_newOrderTicket));
      Assert.That (_order.GetOriginalRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"), Is.SameAs (_oldOrderTicket));
    }

    [Test]
    public void GetOriginalRelatedObjectWithLazyLoad ()
    {
      Order order = DomainObjectIDs.Order3.GetObject<Order> ();

      Assert.That (order.GetOriginalRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket").ID, Is.EqualTo (DomainObjectIDs.OrderTicket3));
    }

    [Test]
    public void GetNullOriginalRelatedObject ()
    {
      Computer computer = DomainObjectIDs.Computer4.GetObject<Computer> ();
      Assert.That (computer.GetOriginalRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"), Is.Null);
    }

    [Test]
    public void OldObjectAndNewObjectAreSameRelationInherited ()
    {
      Customer customer = DomainObjectIDs.Customer4.GetObject<Customer> ();

      Ceo ceo = customer.Ceo;

      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (
          new DomainObject[] { customer, ceo },
          new DomainObjectCollection[0]);

      Assert.That (customer.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (ceo.State, Is.EqualTo (StateType.Unchanged));

      customer.Ceo = ceo;

      Assert.That (customer.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (ceo.State, Is.EqualTo (StateType.Unchanged));

      ChangeState[] expectedStates = new ChangeState[0];

      eventReceiver.Check (expectedStates);
    }

    [Test]
    public void SetRelatedObjectWithInvalidObjectClassOnRelationEndPoint ()
    {
      var orderTicket = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket>();
      var newRelatedObject = DomainObjectIDs.Customer1.GetObject<Customer>();

      var expectedMessage = string.Format (
          "DomainObject '{0}' cannot be assigned to property '{1}' of DomainObject '{2}',"
          + " because it is not compatible with the type of the property.\r\nParameter name: newRelatedObject",
          DomainObjectIDs.Customer1,
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order",
          DomainObjectIDs.OrderTicket1);

      Assert.That (
          () => orderTicket.SetRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", newRelatedObject),
          Throws.ArgumentException.And.Message.EqualTo (expectedMessage));
    }

    [Test]
    public void SetRelatedObjectWithInvalidObjectClassOnVirtualRelationEndPoint ()
    {
      var newRelatedObject = Ceo.NewObject();

      var expectedMessage = string.Format (
          "DomainObject '{0}' cannot be assigned to property '{1}' of DomainObject '{2}',"
          + " because it is not compatible with the type of the property.\r\nParameter name: newRelatedObject",
          newRelatedObject,
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket",
          _order);

      Assert.That (
          () => _order.SetRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", newRelatedObject),
          Throws.ArgumentException.And.Message.EqualTo (expectedMessage));
    }
  }
}
