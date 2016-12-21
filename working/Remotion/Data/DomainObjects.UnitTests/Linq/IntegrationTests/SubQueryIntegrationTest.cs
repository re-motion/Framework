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
  public class SubQueryIntegrationTest : IntegrationTestBase
  {
    [Test]
    public void QueryWithSubQuery_InWhere ()
    {
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where (from c in QueryFactory.CreateLinqQuery<Customer>() select c).Contains (o.Customer)
          select o;

      CheckQueryResult (orders, DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4, DomainObjectIDs.Order5,
                        DomainObjectIDs.Order2, DomainObjectIDs.InvalidOrder, DomainObjectIDs.OrderWithoutOrderItems);
    }

    [Test]
    public void QueryWithSubQueryInWhere_AccessingOuterVariable_InMainFromClause ()
    {
      var orderItem2 = DomainObjectIDs.OrderItem2.GetObject<OrderItem>();
      var number = from o in QueryFactory.CreateLinqQuery<Order> ()
                   where (from oi in o.OrderItems select oi).Contains (orderItem2)
                   select o;
      CheckQueryResult (number, DomainObjectIDs.Order1);
    }

    [Test]
    public void QueryWithSubQueryAndJoinInWhere ()
    {
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where (from c in QueryFactory.CreateLinqQuery<OrderTicket>() select c.Order).Contains (o)
          select o;

      CheckQueryResult (orders, DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4, DomainObjectIDs.Order5,
                        DomainObjectIDs.Order2, DomainObjectIDs.OrderWithoutOrderItems);
    }

    [Test]
    public void QueryWithSubQueryAndJoinInWhere_WithOuterVariable ()
    {
      OrderItem myOrderItem = DomainObjectIDs.OrderItem1.GetObject<OrderItem>();
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where (from oi in QueryFactory.CreateLinqQuery<OrderItem>() where oi.Order == o select oi).Contains (myOrderItem)
          select o;

      CheckQueryResult (orders, DomainObjectIDs.Order1);
    }

    [Test]
    public void QueryWithSubQuery_InMainFrom ()
    {
      var orders = from c in (from ci in QueryFactory.CreateLinqQuery<Computer> () select ci) select c;

      CheckQueryResult (orders, DomainObjectIDs.Computer1, DomainObjectIDs.Computer2, DomainObjectIDs.Computer3, DomainObjectIDs.Computer4, 
                        DomainObjectIDs.Computer5 );
    }

    [Test]
    public void QueryWithSubQuery_WithResultOperator_InMainFrom ()
    {
      var orders = from c in (from ci in QueryFactory.CreateLinqQuery<Computer> () orderby ci.ID.Value select ci).Take (1) select c;
      CheckQueryResult (orders, DomainObjectIDs.Computer5);
    }

    [Test]
    public void QueryWithSubQuery_InAdditionalFrom ()
    {
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order> ()
          from oi in
            (from oi in QueryFactory.CreateLinqQuery<OrderItem> () where oi.Order == o select oi)
          select oi;

      CheckQueryResult (
          orders,
          DomainObjectIDs.OrderItem5,
          DomainObjectIDs.OrderItem4,
          DomainObjectIDs.OrderItem2,
          DomainObjectIDs.OrderItem1,
          DomainObjectIDs.OrderItem3,
          DomainObjectIDs.OrderItem6);
    }


    [Test]
    public void QueryWithSubQuery_InThirdFrom ()
    {
      var orders =
          (from o1 in QueryFactory.CreateLinqQuery<Order> ()
          from o2 in QueryFactory.CreateLinqQuery<Order> ()
          from oi in
            (from oi in QueryFactory.CreateLinqQuery<OrderItem> () where oi.Order == o1 || oi.Order == o2 select oi)
          select oi).Distinct();

      CheckQueryResult (orders, DomainObjectIDs.OrderItem5, DomainObjectIDs.OrderItem4, DomainObjectIDs.OrderItem2, DomainObjectIDs.OrderItem1,
                        DomainObjectIDs.OrderItem3, DomainObjectIDs.OrderItem6);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "Queries selecting collections are not supported because SQL is not well-suited to returning collections.", 
        MatchType = MessageMatch.Contains)]
    public void QueryWithSubQuery_InSelectClause ()
    {
      var orders = from o in QueryFactory.CreateLinqQuery<Order>()
                   select
                       (from c in QueryFactory.CreateLinqQuery<Computer>() select c);

      orders.ToArray();
    }

    [Test]
    public void SubQueryWithNonConstantFromExpression ()
    {
      var query = from o in QueryFactory.CreateLinqQuery<Order> ()
                  from oi in (from oi1 in o.OrderItems select oi1)
                  where o.OrderNumber == 1
                  select oi;

      CheckQueryResult (query, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
    }

    [Test]
    public void FirstOrDefault_WithEntity_InSelectAndWhere ()
    {
      var query = from o in QueryFactory.CreateLinqQuery<Order> ()
                  where o.OrderItems.FirstOrDefault () != null
                  select o.OrderItems.OrderBy (oi => oi.ID.Value).FirstOrDefault ();

      CheckQueryResult (
          query, 
          DomainObjectIDs.OrderItem2, 
          DomainObjectIDs.OrderItem3, 
          DomainObjectIDs.OrderItem4,
          DomainObjectIDs.OrderItem5,
          DomainObjectIDs.OrderItem6);
    }

    [Test]
    public void OrderingsInSubQuery_WithDistinct ()
    {
      var query = from o in (
                    from oi in QueryFactory.CreateLinqQuery<OrderItem>() 
                    where oi.Order != null 
                    orderby oi.Order.OrderNumber 
                    select oi.Order).Distinct () 
                  select o;
      CheckQueryResult (
          query, 
          DomainObjectIDs.Order1, 
          DomainObjectIDs.Order3, 
          DomainObjectIDs.Order4,
          DomainObjectIDs.Order5,
          DomainObjectIDs.Order2);
    }

    [Test]
    public void OrderingsInSubQuery_WithTake ()
    {
      var query = from o in (from o in QueryFactory.CreateLinqQuery<Order> () orderby o.OrderNumber select o).Take (2)
                  select o;
      CheckOrderedQueryResult (
          query,
          DomainObjectIDs.Order1,
          DomainObjectIDs.Order2);
  }

    [Test]
    public void OrderingsInSubQuery_WithoutTakeOrDistinct ()
    {
      var query = from c in QueryFactory.CreateLinqQuery<Customer>()
                  where c.ID == DomainObjectIDs.Customer1
                  from o in (from o in c.Orders orderby o.OrderNumber ascending select o)
                  select o;
      CheckOrderedQueryResult (
          query,
          DomainObjectIDs.Order1,
          DomainObjectIDs.Order2);
    }

    [Test]
    public void OrderingsInSubQuery_WithoutTakeOrDistinct_WithAccessToMemberOfSubQuery ()
    {
      var query = from c in QueryFactory.CreateLinqQuery<Customer> ()
                  where c.ID == DomainObjectIDs.Customer1
                  from o in (from o in c.Orders orderby o.OrderNumber ascending select o)
                  where o.OrderNumber < 2
                  select o;
      CheckOrderedQueryResult (
          query,
          DomainObjectIDs.Order1);
    }

    [Test]
    public void MemberAccess_OnSubQuery_WithEntities ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order> ()
                   where o.OrderNumber == 1
                   select (from oi in o.OrderItems orderby oi.ID.Value select oi).First ().Product).Single ();
      Assert.That (query, Is.EqualTo ("CPU Fan"));
    }

    [Test]
    public void MemberAccess_OnSubQuery_WithColumns ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>()
                   where o.OrderNumber == 1
                   select (from oi in o.OrderItems orderby oi.ID.Value select oi.Product).First ().Length).Single();
      Assert.That (query, Is.EqualTo (7));
    }
  }
}
