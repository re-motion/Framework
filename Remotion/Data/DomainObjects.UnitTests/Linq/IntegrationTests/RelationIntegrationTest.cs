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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Linq.IntegrationTests
{
  [TestFixture]
  public class RelationIntegrationTest : IntegrationTestBase
  {
    [Test]
    public void Query_WithImplicitJoin_SingleValue ()
    {
      var query =
          from ot in QueryFactory.CreateLinqQuery<OrderTicket>()
          where ot.Order.OrderNumber == 1
          select ot.Order;
      CheckQueryResult(query, DomainObjectIDs.Order1);
    }

    [Test]
    public void Query_WithImplicitJoin_DomainObjectCollection ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<Order>()
          from oi in o.OrderItems
          where oi.Position == 1
          select oi;

      CheckQueryResult(
          query,
          DomainObjectIDs.OrderItem1,
          DomainObjectIDs.OrderItem3,
          DomainObjectIDs.OrderItem4,
          DomainObjectIDs.OrderItem5,
          DomainObjectIDs.OrderItem6);
    }

    [Test]
    public void Query_WithImplicitJoin_VirtualCollection ()
    {
      var query =
          from p in QueryFactory.CreateLinqQuery<Product>()
          from r in p.Reviews
          where r.CreatedAt > new DateTime(2006, 01, 01)
          select r;

      CheckQueryResult(
          query,
          DomainObjectIDs.ProductReview2,
          DomainObjectIDs.ProductReview3,
          DomainObjectIDs.ProductReview4);
    }

    [Test]
    public void Query_WithExplicitJoin ()
    {
      var query =
          from c in QueryFactory.CreateLinqQuery<Order>()
          join k in QueryFactory.CreateLinqQuery<Customer>() on c.Customer equals k
          where c.OrderNumber == 5
          select c;
      CheckQueryResult(query, DomainObjectIDs.Order5);
    }

    [Test]
    public void Query_WithDomainObjectCollection_ContainsObject ()
    {
      OrderItem item = DomainObjectIDs.OrderItem1.GetObject<OrderItem>();
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where o.OrderItems.ContainsObject(item)
          select o;

      CheckQueryResult(orders, DomainObjectIDs.Order1);
    }

    [Test]
    [Ignore("TODO: RM-7294: Implement Contains for ObjectID")]
    public void Query_WithVirtualCollection_ContainsObjectID ()
    {
      ProductReview review = DomainObjectIDs.ProductReview1.GetObject<ProductReview>();
      var products =
          from p in QueryFactory.CreateLinqQuery<Product>()
          where p.Reviews.Contains(review.ID)
          select p;

      CheckQueryResult(products, DomainObjectIDs.Product1);
    }

    [Test]
    public void Query_WithVirtualCollection_ICollectionContainsObject ()
    {
      ProductReview review = DomainObjectIDs.ProductReview1.GetObject<ProductReview>();
      var products =
          from p in QueryFactory.CreateLinqQuery<Product>()
          where p.Reviews.Contains(review)
          select p;

      CheckQueryResult(products, DomainObjectIDs.Product1);
    }

    [Test]
    public void Query_WithVirtualCollection_EnumerableContainsObject ()
    {
      ProductReview review = DomainObjectIDs.ProductReview1.GetObject<ProductReview>();
      var products =
          from p in QueryFactory.CreateLinqQuery<Product>()
          where p.Reviews.Contains<ProductReview>(review)
          select p;

      CheckQueryResult(products, DomainObjectIDs.Product1);
    }

    [Test]
    public void Query_WithDomainObjectCollection_Count ()
    {
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where o.OrderItems.Count == 2
          select o;

      CheckQueryResult(orders, DomainObjectIDs.Order1);
    }

    [Test]
    public void Query_WithVirtualCollection_Count ()
    {
      var products =
          from p in QueryFactory.CreateLinqQuery<Product>()
          where p.Reviews.Count == 3
          select p;

      CheckQueryResult(products, DomainObjectIDs.Product1);
    }

    [Test]
    public void Query_WithWhereOnForeignKey_RealSide ()
    {
      ObjectID id = DomainObjectIDs.Order1;
      var query = from oi in QueryFactory.CreateLinqQuery<OrderItem>()
                  where oi.Order.ID == id
                  select oi;
      CheckQueryResult(query, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
    }

    [Test]
    public void Query_WithWhereOnForeignKey_VirtualSide ()
    {
      ObjectID id = DomainObjectIDs.Computer1;
      var query = from e in QueryFactory.CreateLinqQuery<Employee>()
                  where e.Computer.ID == id
                  select e;
      CheckQueryResult(query, DomainObjectIDs.Employee3);
    }
  }
}
