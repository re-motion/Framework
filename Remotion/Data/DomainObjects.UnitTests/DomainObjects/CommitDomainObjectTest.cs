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

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class CommitDomainObjectTest : ClientTransactionBaseTest
  {
    [Test]
    public void CommitOneToManyRelation ()
    {
      Customer customer1 = DomainObjectIDs.Customer1.GetObject<Customer>();
      Customer customer2 = DomainObjectIDs.Customer2.GetObject<Customer>();
      Order order = customer1.Orders[DomainObjectIDs.Order1];

      customer2.Orders.Add(order);

      Assert.That(customer1.State.IsChanged, Is.True);
      Assert.That(customer2.State.IsChanged, Is.True);
      Assert.That(order.State.IsChanged, Is.True);

      TestableClientTransaction.Commit();

      Assert.That(customer1.State.IsUnchanged, Is.True);
      Assert.That(customer2.State.IsUnchanged, Is.True);
      Assert.That(order.State.IsUnchanged, Is.True);
    }

    [Test]
    public void CommitOneToOneRelation ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      OrderTicket oldOrderTicket = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket>();
      OrderTicket newOrderTicket = DomainObjectIDs.OrderTicket2.GetObject<OrderTicket>();

      object orderTimestamp = order.InternalDataContainer.Timestamp;
      object oldOrderTicketTimestamp = oldOrderTicket.InternalDataContainer.Timestamp;
      object newOrderTicketTimestamp = newOrderTicket.InternalDataContainer.Timestamp;

      oldOrderTicket.Order = newOrderTicket.Order;
      order.OrderTicket = newOrderTicket;

      TestableClientTransaction.Commit();

      Assert.That(order.InternalDataContainer.Timestamp, Is.EqualTo(orderTimestamp));
      Assert.That(oldOrderTicketTimestamp.Equals(oldOrderTicket.InternalDataContainer.Timestamp), Is.False);
      Assert.That(newOrderTicketTimestamp.Equals(newOrderTicket.InternalDataContainer.Timestamp), Is.False);
    }

    [Test]
    public void CommitHierarchy ()
    {
      Employee supervisor1 = DomainObjectIDs.Employee1.GetObject<Employee>();
      Employee supervisor2 = DomainObjectIDs.Employee2.GetObject<Employee>();
      Employee subordinate = (Employee)supervisor1.Subordinates[DomainObjectIDs.Employee4];

      subordinate.Supervisor = supervisor2;

      TestableClientTransaction.Commit();
      ReInitializeTransaction();

      supervisor1 = DomainObjectIDs.Employee1.GetObject<Employee>();
      supervisor2 = DomainObjectIDs.Employee2.GetObject<Employee>();

      Assert.That(supervisor1.Subordinates[DomainObjectIDs.Employee4], Is.Null);
      Assert.That(supervisor2.Subordinates[DomainObjectIDs.Employee4], Is.Not.Null);
    }

    [Test]
    public void CommitPolymorphicRelation ()
    {
      Ceo companyCeo = DomainObjectIDs.Ceo1.GetObject<Ceo>();
      Ceo distributorCeo = DomainObjectIDs.Ceo10.GetObject<Ceo>();
      Company company = companyCeo.Company;
      Distributor distributor = DomainObjectIDs.Distributor1.GetObject<Distributor>();

      distributor.Ceo = companyCeo;
      company.Ceo = distributorCeo;

      TestableClientTransaction.Commit();
      ReInitializeTransaction();

      companyCeo = DomainObjectIDs.Ceo1.GetObject<Ceo>();
      distributorCeo = DomainObjectIDs.Ceo10.GetObject<Ceo>();
      company = DomainObjectIDs.Company1.GetObject<Company>();
      distributor = DomainObjectIDs.Distributor1.GetObject<Distributor>();

      Assert.That(distributor.Ceo, Is.SameAs(companyCeo));
      Assert.That(companyCeo.Company, Is.SameAs(distributor));
      Assert.That(company.Ceo, Is.SameAs(distributorCeo));
      Assert.That(distributorCeo.Company, Is.SameAs(company));
    }

    [Test]
    public void CommitPropertyChange ()
    {
      Customer customer = DomainObjectIDs.Customer1.GetObject<Customer>();
      customer.Name = "Arthur Dent";

      TestableClientTransaction.Commit();
      ReInitializeTransaction();

      customer = DomainObjectIDs.Customer1.GetObject<Customer>();
      Assert.That(customer.Name, Is.EqualTo("Arthur Dent"));
    }

    [Test]
    public void OriginalDomainObjectCollection_IsNotSameAfterCommit ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      DomainObjectCollection originalOrderItems = order.GetOriginalRelatedObjectsAsDomainObjectCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");
      var orderItem = OrderItem.NewObject(order);
      orderItem.Product = "MyProduct";

      TestableClientTransaction.Commit();

      Assert.That(
          order.GetOriginalRelatedObjectsAsDomainObjectCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"),
          Is.Not.SameAs(originalOrderItems));
      Assert.That(
          order.GetOriginalRelatedObjectsAsDomainObjectCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"),
          Is.EqualTo(order.OrderItems));
      Assert.That(
          order.GetOriginalRelatedObjectsAsDomainObjectCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems").IsReadOnly,
          Is.True);
    }

    [Test]
    public void OriginalVirtualCollection_IsNotSameAfterCommit ()
    {
      Product product = DomainObjectIDs.Product1.GetObject<Product>();
      var originalProductReviews = product.GetOriginalRelatedObjectsAsVirtualCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews");

      var productReview = ProductReview.NewObject();
      productReview.Product = product;
      productReview.Reviewer = DomainObjectIDs.Person3.GetObject<Person>();
      productReview.Comment = "Test";
      productReview.CreatedAt = DateTime.Now;

      TestableClientTransaction.Commit();

      Assert.That(
          product.GetOriginalRelatedObjectsAsVirtualCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews"),
          Is.Not.SameAs(originalProductReviews));
      Assert.That(
          product.GetOriginalRelatedObjectsAsVirtualCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews"),
          Is.EqualTo(product.Reviews));
    }
  }
}
