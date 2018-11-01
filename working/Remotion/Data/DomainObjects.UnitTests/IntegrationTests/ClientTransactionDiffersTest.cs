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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests
{
  [TestFixture]
  public class ClientTransactionDiffersTest : ClientTransactionBaseTest
  {
    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException),
       ExpectedMessage = "Cannot insert DomainObject '.*'  at position 2 into collection of property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems' of DomainObject 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid', because the objects do not belong to the same ClientTransaction.",
       MatchType = MessageMatch.Regex)]
    public void PerformCollectionAddWithOtherClientTransaction ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      var orderItem3 = DomainObjectMother.GetObjectInOtherTransaction<OrderItem> (DomainObjectIDs.OrderItem3);

      order1.OrderItems.Add (orderItem3);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException),
        ExpectedMessage = "Cannot insert DomainObject '.*' at position 0 into collection of property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems' of DomainObject 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid', because the objects do not belong to the same ClientTransaction.",
        MatchType = MessageMatch.Regex)]
    public void PerformCollectionInsertWithOtherClientTransaction ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      var orderItem3 = DomainObjectMother.GetObjectInOtherTransaction<OrderItem> (DomainObjectIDs.OrderItem3);

      order1.OrderItems.Insert (0, orderItem3);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The object to be removed has the same ID as an object in this collection, but is a different object reference.\r\n"
        + "Parameter name: domainObject",
        MatchType = MessageMatch.Regex)]
    public void PerformCollectionRemoveWithOtherClientTransaction ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      var orderItem1 = DomainObjectMother.GetObjectInOtherTransaction<OrderItem> (DomainObjectIDs.OrderItem1);

      var endPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (order1.OrderItems);
      endPoint.Collection.Remove (orderItem1);
    }

    [Test]
    public void PerformCollectionReplaceWithOtherClientTransaction ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      var orderItem3 = DomainObjectMother.GetObjectInOtherTransaction<OrderItem> (DomainObjectIDs.OrderItem3);

      int index = order1.OrderItems.IndexOf (DomainObjectIDs.OrderItem1);

      try
      {
        order1.OrderItems[index] = orderItem3;
        Assert.Fail ("This test expects a ClientTransactionsDifferException.");
      }
      catch (ClientTransactionsDifferException ex)
      {
        string expectedMessage =
            "Cannot put DomainObject 'OrderItem|0d7196a5-8161-4048-820d-b1bbdabe3293|System.Guid' into the collection of property "
            + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems' of DomainObject "
            + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'. The objects do not belong to the same ClientTransaction.";
        Assert.That (ex.Message, Is.EqualTo (expectedMessage));
      }
    }


  }
}