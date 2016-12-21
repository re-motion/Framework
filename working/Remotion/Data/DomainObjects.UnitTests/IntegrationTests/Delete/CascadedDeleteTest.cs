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

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Delete
{
  [TestFixture]
  public class CascadedDeleteTest : ClientTransactionBaseTest
  {
    [Test]
    [Ignore ("TODO: Define what re-store should do here - actually, it's not allowed to modify the relations within the Deleting handler, but the exception is quite unclear.")]
    public void BidirectionalRelation_CascadeWithinDeleting ()
    {
      var order = Order.NewObject ();
      var orderTicket = OrderTicket.NewObject ();
      orderTicket.Order = order;

      order.Deleting += delegate { order.OrderTicket.Delete (); };

      order.Delete ();

      Assert.That (order.State, Is.EqualTo (StateType.Invalid));
      Assert.That (orderTicket.State, Is.EqualTo (StateType.Invalid));
      Assert.That (TestableClientTransaction.DataManager.DataContainers, Is.Empty);
      Assert.That (TestableClientTransaction.DataManager.RelationEndPoints, Is.Empty);
    }

    [Test]
    public void BidirectionalRelation_CascadeWithinDeleted ()
    {
      var order = Order.NewObject ();
      var orderTicket = OrderTicket.NewObject ();

      orderTicket.Order = order;

      OrderTicket objectToBeDeleted = null;
      order.Deleting += delegate { objectToBeDeleted = order.OrderTicket; };
      order.Deleted += delegate { objectToBeDeleted.Delete (); };

      order.Delete ();

      Assert.That (order.State, Is.EqualTo (StateType.Invalid));
      Assert.That (orderTicket.State, Is.EqualTo (StateType.Invalid));
      Assert.That (TestableClientTransaction.DataManager.DataContainers, Is.Empty);
      Assert.That (TestableClientTransaction.DataManager.RelationEndPoints, Is.Empty);
    }

    [Test]
    [Ignore ("TODO: Define what re-store should do here - actually, it's not allowed to modify the relations within the Deleting handler, but the exception is quite unclear.")]
    public void BidirectionalRelation_CascadeWithinDeleting_SubTransaction ()
    {
      var order = Order.NewObject ();
      var orderTicket = OrderTicket.NewObject ();
      orderTicket.Order = order;

      order.Deleting += delegate { order.OrderTicket.Delete (); };

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        order.Delete ();
        ClientTransaction.Current.Commit ();
      }

      Assert.That (order.State, Is.EqualTo (StateType.Invalid));
      Assert.That (orderTicket.State, Is.EqualTo (StateType.Invalid));
      Assert.That (TestableClientTransaction.DataManager.DataContainers, Is.Empty);
      Assert.That (TestableClientTransaction.DataManager.RelationEndPoints, Is.Empty);
    }

    [Test]
    public void BidirectionalRelation_CascadeWithinDeleted_SubTransaction ()
    {
      var order = Order.NewObject ();
      var orderTicket = OrderTicket.NewObject ();

      orderTicket.Order = order;

      OrderTicket objectToBeDeleted = null;
      order.Deleting += delegate { objectToBeDeleted = order.OrderTicket; };
      order.Deleted += delegate { objectToBeDeleted.Delete (); };

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        order.Delete ();
        ClientTransaction.Current.Commit ();
      }

      Assert.That (order.State, Is.EqualTo (StateType.Invalid));
      Assert.That (orderTicket.State, Is.EqualTo (StateType.Invalid));
      Assert.That (TestableClientTransaction.DataManager.DataContainers, Is.Empty);
      Assert.That (TestableClientTransaction.DataManager.RelationEndPoints, Is.Empty);
    }
  }
}