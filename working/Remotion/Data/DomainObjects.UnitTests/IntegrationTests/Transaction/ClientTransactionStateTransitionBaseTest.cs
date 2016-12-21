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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  public abstract class ClientTransactionStateTransitionBaseTest : ClientTransactionBaseTest
  {
    public Order GetInvalid ()
    {
      Order invalid = Order.NewObject ();
      invalid.Delete ();
      Assert.That (invalid.State, Is.EqualTo (StateType.Invalid));
      return invalid;
    }

    public Location GetUnidirectionalWithDeletedNew ()
    {
      Location unidirectionalWithDeletedNew = DomainObjectIDs.Location3.GetObject<Location>();
      unidirectionalWithDeletedNew.Client = Client.NewObject();
      unidirectionalWithDeletedNew.Client.Delete ();
      return unidirectionalWithDeletedNew;
    }

    public Location GetUnidirectionalWithDeleted ()
    {
      Location unidirectionalWithDeleted = DomainObjectIDs.Location1.GetObject<Location>();
      unidirectionalWithDeleted.Client.Delete ();
      return unidirectionalWithDeleted;
    }

    public Order GetDeleted ()
    {
      Order deleted = DomainObjectIDs.Order5.GetObject<Order> ();
      FullyDeleteOrder (deleted);
      return deleted;
    }

    public ClassWithAllDataTypes GetNewChanged ()
    {
      ClassWithAllDataTypes newChanged = ClassWithAllDataTypes.NewObject ();
      newChanged.Int32Property = 13;
      return newChanged;
    }

    public ClassWithAllDataTypes GetNewUnchanged ()
    {
      return ClassWithAllDataTypes.NewObject ();
    }

    public Employee GetChangedThroughRelatedObjectVirtualSide ()
    {
      Employee changedThroughRelatedObjectVirtualSide = DomainObjectIDs.Employee3.GetObject<Employee> ();
      changedThroughRelatedObjectVirtualSide.Computer = DomainObjectIDs.Computer3.GetObject<Computer> ();
      return changedThroughRelatedObjectVirtualSide;
    }

    public Computer GetChangedThroughRelatedObjectRealSide ()
    {
      Computer changedThroughRelatedObjectRealSide = DomainObjectIDs.Computer1.GetObject<Computer> ();
      changedThroughRelatedObjectRealSide.Employee = DomainObjectIDs.Employee1.GetObject<Employee> ();
      return changedThroughRelatedObjectRealSide;
    }

    public Order GetChangedThroughRelatedObjects ()
    {
      Order changedThroughRelatedObjects = DomainObjectIDs.Order4.GetObject<Order> ();
      changedThroughRelatedObjects.OrderItems.Clear ();
      return changedThroughRelatedObjects;
    }

    public Order GetChangedThroughPropertyValue ()
    {
      Order changedThroughPropertyValue = DomainObjectIDs.Order3.GetObject<Order> ();
      changedThroughPropertyValue.OrderNumber = 74;
      return changedThroughPropertyValue;
    }

    public Order GetUnchanged ()
    {
      return DomainObjectIDs.Order1.GetObject<Order> ();
    }

    [Test]
    public void CheckInitialStates ()
    {
      Order unchanged = GetUnchanged();
      Order changedThroughPropertyValue = GetChangedThroughPropertyValue();
      Order changedThroughRelatedObjects = GetChangedThroughRelatedObjects();
      Computer changedThroughRelatedObjectRealSide = GetChangedThroughRelatedObjectRealSide();
      Employee changedThroughRelatedObjectVirtualSide = GetChangedThroughRelatedObjectVirtualSide();
      ClassWithAllDataTypes newUnchanged = GetNewUnchanged();
      ClassWithAllDataTypes newChanged = GetNewChanged ();
      Order deleted = GetDeleted();
      Location unidirectionalWithDeleted = GetUnidirectionalWithDeleted ();
      Location unidirectionalWithDeletedNew = GetUnidirectionalWithDeletedNew ();
      Order invalid = GetInvalid();

      Assert.That (unchanged.State, Is.EqualTo (StateType.Unchanged));

      Assert.That (changedThroughPropertyValue.State, Is.EqualTo (StateType.Changed));
      Assert.That (changedThroughPropertyValue.Properties[typeof (Order) + ".OrderNumber"].GetOriginalValue<int>(), Is.Not.EqualTo (changedThroughPropertyValue.OrderNumber));

      Assert.That (changedThroughRelatedObjects.State, Is.EqualTo (StateType.Changed));
      Assert.That (changedThroughRelatedObjects.Properties[typeof (Order) + ".OrderItems"].GetOriginalValue<ObjectList<OrderItem>> ().Count, Is.Not.EqualTo (changedThroughRelatedObjects.OrderItems.Count));

      Assert.That (changedThroughRelatedObjectRealSide.State, Is.EqualTo (StateType.Changed));
      Assert.That (changedThroughRelatedObjectRealSide.Properties[typeof (Computer) + ".Employee"].GetOriginalValue<Employee> (), Is.Not.EqualTo (changedThroughRelatedObjectRealSide.Employee));

      Assert.That (changedThroughRelatedObjectVirtualSide.State, Is.EqualTo (StateType.Changed));
      Assert.That (changedThroughRelatedObjectVirtualSide.Properties[typeof (Employee) + ".Computer"].GetOriginalValue<Computer> (), Is.Not.EqualTo (changedThroughRelatedObjectVirtualSide.Computer));

      Assert.That (newUnchanged.State, Is.EqualTo (StateType.New));
      Assert.That (newChanged.State, Is.EqualTo (StateType.New));

      Assert.That (deleted.State, Is.EqualTo (StateType.Deleted));

      Assert.That (unidirectionalWithDeleted.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (unidirectionalWithDeletedNew.State, Is.EqualTo (StateType.Changed));

      Assert.That (invalid.State, Is.EqualTo (StateType.Invalid));
    }

    protected void FullyDeleteOrder (Order order)
    {
      for (int i = order.OrderItems.Count - 1; i >= 0; --i)
        order.OrderItems[i].Delete ();
      order.OrderTicket.Delete ();
      order.Delete ();
    }
  }
}
