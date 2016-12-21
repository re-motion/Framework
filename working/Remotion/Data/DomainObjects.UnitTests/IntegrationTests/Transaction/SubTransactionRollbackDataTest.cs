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
  [TestFixture]
  public class SubTransactionRollbackDataTest : ClientTransactionBaseTest
  {
    [Test]
    public void RollbackResetsPropertyValuesToThoseOfParentTransaction ()
    {
      Order loadedOrder = DomainObjectIDs.Order1.GetObject<Order> ();
      Order newOrder = Order.NewObject ();

      loadedOrder.OrderNumber = 5;
      newOrder.OrderNumber = 7;

      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        loadedOrder.OrderNumber = 13;
        newOrder.OrderNumber = 47;

        ClientTransactionScope.CurrentTransaction.Rollback ();

        Assert.That (loadedOrder.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (newOrder.State, Is.EqualTo (StateType.Unchanged));

        Assert.That (loadedOrder.OrderNumber, Is.EqualTo (5));
        Assert.That (newOrder.OrderNumber, Is.EqualTo (7));
      }

      Assert.That (loadedOrder.OrderNumber, Is.EqualTo (5));
      Assert.That (newOrder.OrderNumber, Is.EqualTo (7));
    }

    [Test]
    public void RollbackResetsRelatedObjectsToThoseOfParentTransaction ()
    {
      Order newOrder = Order.NewObject ();
      OrderItem orderItem = OrderItem.NewObject ();
      newOrder.OrderItems.Add (orderItem);

      Assert.That (newOrder.OrderItems.Count, Is.EqualTo (1));
      Assert.That (newOrder.OrderItems.ContainsObject (orderItem), Is.True);

      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        newOrder.OrderItems.Clear ();
        newOrder.OrderItems.Add (OrderItem.NewObject ());
        newOrder.OrderItems.Add (OrderItem.NewObject ());

        Assert.That (newOrder.OrderItems.Count, Is.EqualTo (2));
        Assert.That (newOrder.OrderItems.ContainsObject (orderItem), Is.False);

        ClientTransactionScope.CurrentTransaction.Rollback ();

        Assert.That (newOrder.State, Is.EqualTo (StateType.Unchanged));

        Assert.That (newOrder.OrderItems.Count, Is.EqualTo (1));
        Assert.That (newOrder.OrderItems.ContainsObject (orderItem), Is.True);
      }

      Assert.That (newOrder.OrderItems.Count, Is.EqualTo (1));
      Assert.That (newOrder.OrderItems.ContainsObject (orderItem), Is.True);
    }

    [Test]
    public void RollbackResetsRelatedObjectToThatOfParentTransaction ()
    {
      Computer computer = DomainObjectIDs.Computer1.GetObject<Computer> ();
      Employee employee = computer.Employee;
      Location location = Location.NewObject ();
      Client client = Client.NewObject ();
      location.Client = client;

      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        computer.Employee = Employee.NewObject ();
        location.Client = null;
        Assert.That (employee.Computer, Is.Null);

        ClientTransactionScope.CurrentTransaction.Rollback ();

        Assert.That (computer.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (employee.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (location.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (client.State, Is.EqualTo (StateType.NotLoadedYet));

        Assert.That (computer.Employee, Is.SameAs (employee));
        Assert.That (employee.Computer, Is.SameAs (computer));
        Assert.That (location.Client, Is.SameAs (client));
      }

      Assert.That (computer.Employee, Is.SameAs (employee));
      Assert.That (employee.Computer, Is.SameAs (computer));
      Assert.That (location.Client, Is.SameAs (client));
    }

    [Test]
    public void SubCommitDoesNotRollbackParent ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      order.OrderNumber = 5;
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        order.OrderNumber = 3;
        ClientTransactionScope.CurrentTransaction.Rollback ();
      }
      Assert.That (order.OrderNumber, Is.EqualTo (5));
      TestableClientTransaction.Rollback ();
      Assert.That (order.OrderNumber, Is.EqualTo (1));
    }
  }
}
