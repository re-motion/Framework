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
  public class ResultOperatorIntegrationTest : IntegrationTestBase
  {
    [Test]
    public void Query_WithDistinct ()
    {
      var ceos =
          (from o in QueryFactory.CreateLinqQuery<Order>()
           where o.Customer.Ceo != null
           select o.Customer.Ceo).Distinct();

      CheckQueryResult (ceos, DomainObjectIDs.Ceo12, DomainObjectIDs.Ceo5, DomainObjectIDs.Ceo3);
    }

    [Test]
    public void Query_WithDomainObjectCollectionContainsObject ()
    {
      OrderItem item = DomainObjectIDs.OrderItem1.GetObject<OrderItem>();
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where o.OrderItems.ContainsObject (item)
          select o;

      CheckQueryResult (orders, DomainObjectIDs.Order1);
    }

    [Test]
    public void Query_WithDomainObjectCollectionCount ()
    {
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order> ()
          where o.OrderItems.Count == 2
          select o;

      CheckQueryResult (orders, DomainObjectIDs.Order1);
    }

    [Test]
    public void Query_WithCastOnResultSet ()
    {
      var query =
          (from o in QueryFactory.CreateLinqQuery<Order>()
           where o.OrderNumber == 1
           select o).Cast<TestDomainBase>();

      CheckQueryResult (query, DomainObjectIDs.Order1);
    }

    [Test]
    public void Query_WithCastInSubQuery ()
    {
      var query = from c in
                      (from o in QueryFactory.CreateLinqQuery<Order>()
                       where o.OrderNumber == 1
                       select o).Cast<TestDomainBase>()
                  where c.ID == DomainObjectIDs.Order1
                  select c;
      CheckQueryResult (query, DomainObjectIDs.Order1);
    }

    [Test]
    public void QueryWithFirst ()
    {
      var queryResult = (from o in QueryFactory.CreateLinqQuery<Order>()
                         orderby o.OrderNumber
                         select o).First();
      Assert.That (queryResult, Is.EqualTo (DomainObjectIDs.Order1.GetObject<Order> ()));
    }

    [Test]
    public void QueryWithFirst_AndInterface ()
    {
      var queryResult = (from o in QueryFactory.CreateLinqQuery<Order> ()
                         orderby o.OrderNumber
                         select (IOrder) o).First ();
      Assert.That (queryResult, Is.EqualTo (DomainObjectIDs.Order1.GetObject<Order> ()));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Sequence contains no elements")]
    public void QueryWithFirst_Throws_WhenNoItems ()
    {
      (from o in QueryFactory.CreateLinqQuery<Order>()
       where false
       select o).First();
    }

    [Test]
    public void QueryWithFirstOrDefault ()
    {
      var queryResult = (from o in QueryFactory.CreateLinqQuery<Order>()
                         orderby o.OrderNumber
                         select o).FirstOrDefault();
      Assert.That (queryResult, Is.EqualTo (DomainObjectIDs.Order1.GetObject<Order> ()));
    }

    [Test]
    public void QueryWithFirstOrDefault_ReturnsNull_WhenNoItems ()
    {
      var queryResult = (from o in QueryFactory.CreateLinqQuery<Order>()
                         where false
                         select o).FirstOrDefault();
      Assert.That (queryResult, Is.Null);
    }

    [Test]
    public void QueryWithSingle ()
    {
      var queryResult = (from o in QueryFactory.CreateLinqQuery<Order>()
                         where o.OrderNumber == 1
                         select o).Single();
      Assert.That (queryResult, Is.EqualTo (DomainObjectIDs.Order1.GetObject<Order> ()));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Sequence contains more than one element")]
    public void QueryWithSingle_ThrowsException_WhenMoreThanOneElement ()
    {
      (from o in QueryFactory.CreateLinqQuery<Order>()
       select o).Single();
    }

    [Test]
    public void QueryWithSingleOrDefault_ReturnsSingleItem ()
    {
      var queryResult = (from o in QueryFactory.CreateLinqQuery<Order>()
                         where o.OrderNumber == 1
                         select o).SingleOrDefault();
      Assert.That (queryResult, Is.EqualTo (DomainObjectIDs.Order1.GetObject<Order> ()));
    }

    [Test]
    public void QueryWithSingleOrDefault_ReturnsNull_WhenNoItem ()
    {
      var queryResult = (from o in QueryFactory.CreateLinqQuery<Order>()
                         where o.OrderNumber == 99999
                         select o).SingleOrDefault();
      Assert.That (queryResult, Is.EqualTo (null));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Sequence contains more than one element")]
    public void QueryWithSingleOrDefault_ThrowsException_WhenMoreThanOneElement ()
    {
      (from o in QueryFactory.CreateLinqQuery<Order>()
       select o).SingleOrDefault();
    }

    [Test]
    public void QueryWithCount ()
    {
      var number = (from o in QueryFactory.CreateLinqQuery<Order>()
                    select o).Count();
      Assert.That (7, Is.EqualTo (number));
    }

    [Test]
    public void QueryWithCount_InSubquery ()
    {
      var number = (from o in QueryFactory.CreateLinqQuery<Order>()
                    where (from oi in QueryFactory.CreateLinqQuery<OrderItem>() where oi.Order == o select oi).Count() == 2
                    select o);
      CheckQueryResult (number, DomainObjectIDs.Order1);
    }

    [Test]
    public void QueryDistinctTest ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>()
                   from oi in o.OrderItems
                   where o.OrderNumber == 1
                   select o).Distinct();
      CheckQueryResult (query, DomainObjectIDs.Order1);
    }

    [Test]
    public void QueryWithConvertToString ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<OrderItem>()
          where Convert.ToString (o.Position).Contains ("2")
          select o;

      CheckQueryResult (query, DomainObjectIDs.OrderItem2);
    }

    [Test]
    public void QueryWithArithmeticOperations ()
    {
      var query = from oi in QueryFactory.CreateLinqQuery<OrderItem>()
                  where (oi.Position + oi.Position) == 4
                  select oi;
      CheckQueryResult (query, DomainObjectIDs.OrderItem2);
    }

    [Test]
    public void QueryWithSubString ()
    {
      var query = from c in QueryFactory.CreateLinqQuery<Customer>()
                  where c.Name.Substring (1, 3).Contains ("und")
                  select c;
      CheckQueryResult (query, DomainObjectIDs.Customer1, DomainObjectIDs.Customer2, DomainObjectIDs.Customer3, DomainObjectIDs.Customer4);
    }

    [Test]
    public void QueryWithTake ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>() orderby o.OrderNumber select o).Take (3);
      CheckQueryResult (query, DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order2);
    }

    [Test]
    public void QueryWithTake_SubQueryAsArgument ()
    {
      var query = from o in QueryFactory.CreateLinqQuery<Order> () 
                  from oi in o.OrderItems.Take (o.OrderItems.Count() / 2)
                  where o.OrderNumber == 1
                  select oi;
      CheckQueryResult (query, DomainObjectIDs.OrderItem2);
    }

    [Test]
    public void QueryWithContainsInWhere_OnCollection ()
    {
      var possibleItems = new[] { DomainObjectIDs.Order1.Value, DomainObjectIDs.Order3.Value };
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where possibleItems.Contains (o.ID.Value)
          select o;

      CheckQueryResult (orders, DomainObjectIDs.Order1, DomainObjectIDs.Order3);
    }

    [Test]
    public void QueryWithContainsInWhere_OnEmptyCollection ()
    {
      var possibleItems = new object[0];
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order> ()
          where possibleItems.Contains (o.ID.Value)
          select o;

      CheckQueryResult (orders);
    }


    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
      "There was an error preparing or resolving query "
      + "'from Order o in DomainObjectQueryable<Order> where {value(Remotion.Data.DomainObjects.ObjectID[]) => Contains([o].ID)} select [o]' for "
      + "SQL generation. The SQL 'IN' operator (originally probably a call to a 'Contains' method) requires a single value, so the following "
      + "expression cannot be translated to SQL: "
      + "'new ObjectID(ClassID = [t0].[ClassID] AS ClassID, Value = Convert([t0].[ID] AS Value)) "
      + "IN (Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid,Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid)'.")]
    public void QueryWithContainsInWhere_OnCollection_WithObjectIDs ()
    {
      var possibleItems = new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 };
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order> ()
          where possibleItems.Contains (o.ID)
          select o;

      CheckQueryResult (orders, DomainObjectIDs.Order1, DomainObjectIDs.Order3);
    }

    [Test]
    public void Query_WithSupportForObjectList ()
    {
      var orders =
          (from o in QueryFactory.CreateLinqQuery<Order>()
           from oi in QueryFactory.CreateLinqQuery<OrderItem>()
           where oi.Order == o
           select o).Distinct();

      CheckQueryResult (orders, DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4, DomainObjectIDs.Order5, DomainObjectIDs.Order2);
    }

    [Test]
    public void Query_WithOfType_SelectingBaseType ()
    {
      var query = QueryFactory.CreateLinqQuery<Customer>().OfType<Company>();

      CheckQueryResult (
          query,
          DomainObjectIDs.Customer1,
          DomainObjectIDs.Customer2,
          DomainObjectIDs.Customer3,
          DomainObjectIDs.Customer4,
          DomainObjectIDs.Customer5);
    }

    [Test]
    public void Query_WithOfType_SameType ()
    {
      var query = QueryFactory.CreateLinqQuery<Customer>().OfType<Customer>();

      CheckQueryResult (
          query,
          DomainObjectIDs.Customer1,
          DomainObjectIDs.Customer2,
          DomainObjectIDs.Customer3,
          DomainObjectIDs.Customer4,
          DomainObjectIDs.Customer5);
    }

    [Test]
    public void Query_WithOfType_DerivedType ()
    {
      var partnerIDs = new[]
                       {
                           (Guid) DomainObjectIDs.Partner1.Value,
                           (Guid) DomainObjectIDs.Distributor1.Value,
                           (Guid) DomainObjectIDs.Supplier1.Value,
                           (Guid) DomainObjectIDs.Company1.Value,
                           (Guid) DomainObjectIDs.Customer1.Value
                       };
      var query = QueryFactory.CreateLinqQuery<Company>().OfType<Partner>().Where (p => partnerIDs.Contains ((Guid) p.ID.Value));

      CheckQueryResult (
          query,
          DomainObjectIDs.Partner1,
          DomainObjectIDs.Distributor1,
          DomainObjectIDs.Supplier1);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void Query_WithOfType_UnrelatedType ()
    {
      var query = QueryFactory.CreateLinqQuery<Company>().OfType<Order>();

      CheckQueryResult (query);
    }

    [Test]
    public void QueryWithAny_WithoutPredicate ()
    {
      var query = QueryFactory.CreateLinqQuery<Computer>().Any();

      Assert.That (query, Is.True);
    }

    [Test]
    public void QueryWithAny_WithPredicate ()
    {
      var query = QueryFactory.CreateLinqQuery<Computer>().Any (c => c.SerialNumber == "123456");

      Assert.That (query, Is.False);
    }

    [Test]
    public void QueryWithAny_InSubquery ()
    {
      var query = from o in QueryFactory.CreateLinqQuery<Order>()
                  where !o.OrderItems.Any()
                  select o;

      CheckQueryResult (query, DomainObjectIDs.OrderWithoutOrderItems, DomainObjectIDs.InvalidOrder);
    }

    [Test]
    public void QueryWithAll ()
    {
      var result1 = QueryFactory.CreateLinqQuery<Computer>().All (c => c.SerialNumber == "123456");
      Assert.That (result1, Is.False);

      var result2 = QueryFactory.CreateLinqQuery<Computer> ().All (c => c.SerialNumber != string.Empty);
      Assert.That (result2, Is.True);
    }

    [Test]
    public void QueryWithAll_AfterIncompatibleResultOperator ()
    {
      var query = QueryFactory.CreateLinqQuery<Computer>().Take (10).Take (20).All (c => c.SerialNumber == "123456");

      Assert.That (query, Is.False);
    }

    [Test]
    public void QueryWithOrderBy_BeforeDistinct ()
    {
      var result = QueryFactory.CreateLinqQuery<Computer> ().OrderBy (c => c.SerialNumber).Distinct ().Count ();

      Assert.That (result, Is.EqualTo (5));
    }

    [Test]
    public void QueryWithOrderBy_BeforeCount ()
    {
      var result = QueryFactory.CreateLinqQuery<Computer> ().OrderBy (c => c.SerialNumber).Count();

      Assert.That (result, Is.EqualTo (5));
    }

    [Test]
    public void QueryWithOrderBy_BeforeCount_DueToIncompatibleResultOperators ()
    {
      var result = QueryFactory.CreateLinqQuery<Computer> ().OrderBy (c => c.SerialNumber).Take (10).Count ();

      Assert.That (result, Is.EqualTo (5));
    }

    [Test]
    public void QueryWithAll_InSubquery ()
    {
      var query1 = from o in QueryFactory.CreateLinqQuery<Order>()
                  where o.OrderItems.All (oi => oi.Position == 1)
                  select o;

      CheckQueryResult (
          query1,
          DomainObjectIDs.Order3,
          DomainObjectIDs.Order4,
          DomainObjectIDs.Order5,
          DomainObjectIDs.Order2,
          DomainObjectIDs.InvalidOrder,
          DomainObjectIDs.OrderWithoutOrderItems);

      // ReSharper disable UseMethodAny.0
      var query2 = from c in QueryFactory.CreateLinqQuery<Customer>()
                   where c.Orders.All (o => o.OrderItems.Count() > 0)
                   select c;
      // ReSharper restore UseMethodAny.0

      CheckQueryResult (
          query2,
          DomainObjectIDs.Customer1,
          DomainObjectIDs.Customer2,
          DomainObjectIDs.Customer3,
          DomainObjectIDs.Customer4);
    }

    [Test]
    public void DefaultIsEmpty_WithoutJoin ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order> ()
                  where o.ID == DomainObjectIDs.Order1
                  select o).DefaultIfEmpty();

      CheckQueryResult (query, DomainObjectIDs.Order1);
    }

    [Test]
    public void DefaultIsEmpty_WithoutJoin_EmptyResult ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order> ()
                   where o.OrderNumber == -1
                   select o).DefaultIfEmpty ();

      CheckQueryResult (query, null);
    }

    [Test]
    public void DefaultIsEmpty_WithJoin ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>()
                   join c in QueryFactory.CreateLinqQuery<Customer>() on o.Customer equals c into goc
                   from oc in goc.DefaultIfEmpty()
                   where o.OrderNumber == 5
                   select oc);

      CheckQueryResult (query, DomainObjectIDs.Customer4);
    }

    [Test]
    public void Max_OnTopLevel ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order> () select o.OrderNumber).Max();

      Assert.That(query, Is.EqualTo(99));
    }

    [Test]
    public void Max_InSubquery ()
    {
      var query =
          (from o in QueryFactory.CreateLinqQuery<Order>()
           where (from s2 in QueryFactory.CreateLinqQuery<Order>() select s2.OrderNumber).Max() == o.OrderNumber
           select o);

      CheckQueryResult (query, DomainObjectIDs.OrderWithoutOrderItems);
    }

    [Test]
    public void Max_WithStrings ()
    {
      var result = QueryFactory.CreateLinqQuery<Customer> ().Max (c => c.Name);

      Assert.That (result, Is.EqualTo ("Kunde 4"));
    }

    [Test]
    public void Max_WithDateTimes ()
    {
      var result = QueryFactory.CreateLinqQuery<Order> ().Max (o => o.DeliveryDate);

      Assert.That (result, Is.EqualTo (new DateTime (2013, 3, 7)));
    }

    [Test]
    public void Max_WithNullableInt ()
    {
      int? result = QueryFactory.CreateLinqQuery<Order> ().Max (o => (int?) o.OrderNumber);

      Assert.That (result, Is.EqualTo (99));
    }

    [Test]
    public void Min_OnTopLevel ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order> () select o.OrderNumber).Min ();

      Assert.That (query, Is.EqualTo (1));
    }

    [Test]
    public void Min_InSubquery ()
    {
      var query =
          (from o in QueryFactory.CreateLinqQuery<Order> ()
           where (from s2 in QueryFactory.CreateLinqQuery<Order> () select s2.OrderNumber).Min () == o.OrderNumber
           select o);

      CheckQueryResult (query, DomainObjectIDs.Order1);
    }

    [Test]
    public void Average_OnTopLevel_WithIntProperty ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>() where o.OrderNumber <= 6 select o).Average (o => o.OrderNumber);

      Assert.That (query, Is.EqualTo (3.5));
    }

    [Test]
    public void Average_InSubquery_WithIntProperty ()
    {
      // ReSharper disable CompareOfFloatsByEqualityOperator
      var query =
          from c in QueryFactory.CreateLinqQuery<Customer>()
          where c.Orders.Average (o => o.OrderNumber) == 1.5
          select c;
      // ReSharper restore CompareOfFloatsByEqualityOperator

      CheckQueryResult (query, DomainObjectIDs.Customer1);
    }

    [Test]
    public void Sum_OnTopLevel ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order> () select o).Sum(o => o.OrderNumber);

      Assert.That (query, Is.EqualTo (120));
    }

    [Test]
    public void Sum_WithEmptyResultSet_AndAggregatedValueIsNotNullableProperty_ThrowsNotSupportedException ()
    {
      Assert.That (
          () => (from o in QueryFactory.CreateLinqQuery<Order>() where o.OrderNumber == -1 select o).Sum (o => o.OrderNumber),
          Throws.Exception.TypeOf<NotSupportedException>().And.Message.EqualTo ("Null cannot be converted to type 'System.Int32'."));
    }

    [Test]
    public void Sum_WithEmptyResultSet_AndAggregatedValueIsNotNullablePropertyButCastToNullable_ReturnsNull()
    {
      Assert.That (
          () => (from o in QueryFactory.CreateLinqQuery<Order>() where o.OrderNumber == -1 select o).Sum (o => (int?) o.OrderNumber),
          Is.Null);
    }

    [Test]
    public void Sum_InSubquery ()
    {
      var query =
          (from o in QueryFactory.CreateLinqQuery<Order> ()
           where (from s2 in QueryFactory.CreateLinqQuery<Order> () select s2.OrderNumber).Sum () == 120
           select o);

      CheckQueryResult (query, DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4, DomainObjectIDs.Order5,
        DomainObjectIDs.Order2, DomainObjectIDs.InvalidOrder, DomainObjectIDs.OrderWithoutOrderItems);
    }

    [Test]
    public void Skip_WithEntity ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>() orderby o.OrderNumber select o).Skip (6);

      CheckQueryResult (query, DomainObjectIDs.OrderWithoutOrderItems);
    }

    [Test]
    public void Skip_WithEntity_WithoutExplicitOrdering ()
    {
      Assert.That ((from o in QueryFactory.CreateLinqQuery<Order>() select o).Skip (6).Count(), Is.EqualTo (1));
    }

    [Test]
    public void TakeAfterSkip ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>() orderby o.OrderNumber select o).Skip (3).Take (2);

      CheckQueryResult (query, DomainObjectIDs.Order4, DomainObjectIDs.Order5);
    }

    [Test]
    public void QueryWithCastToInterface_ThrowsNoException ()
    {
      (from o in QueryFactory.CreateLinqQuery<Order> () select o).Cast<IOrder>();
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "This SQL generator does not support queries returning groupings that result from a GroupBy operator because SQL is not suited to " 
        + "efficiently return LINQ groupings.", MatchType = MessageMatch.Contains)]
    public void GroupBy_AtTopLevel ()
    {
      var query = from oi in QueryFactory.CreateLinqQuery<OrderItem> ()
                  where oi.Order.OrderNumber == 1 || oi.Order.OrderNumber == 3
                  orderby oi.Order.OrderNumber
                  group oi.Product by oi.Order;

      query.ToArray ();
    }

    [Test]
    public void GroupBy_NonEntityKey ()
    {
      var query = from o in QueryFactory.CreateLinqQuery<Order> ()
                  group o by o.Customer.ID into ordersByCustomer
                  from c in QueryFactory.CreateLinqQuery<Customer>()
                  where c.ID == ordersByCustomer.Key
                  select c;

      CheckQueryResult (query, DomainObjectIDs.Customer1, DomainObjectIDs.Customer3, DomainObjectIDs.Customer4, DomainObjectIDs.Customer5);
    }

    
    [Test]
    public void GroupBy_EntityKey ()
    {
      var query1 = from o in QueryFactory.CreateLinqQuery<Order> ()
                   where o.ID != DomainObjectIDs.InvalidOrder
                   group o by o.Customer into ordersByCustomer
                   select ordersByCustomer.Key;

      CheckQueryResult (query1, DomainObjectIDs.Customer1, DomainObjectIDs.Customer3, DomainObjectIDs.Customer4, DomainObjectIDs.Customer5);

      var query2 =
          from o in QueryFactory.CreateLinqQuery<Order> ()
          group o by o.OrderTicket into ordersByOrderTicket
          where ordersByOrderTicket.Key != null
          select ordersByOrderTicket.Key.FileName;

      Assert.That (query2.Count (), Is.EqualTo (6));

      var query3 = from r in QueryFactory.CreateLinqQuery<Order> ()
                   from c in r.OrderItems
                   group c.ID by r
                     into cooksByRestaurant
                     from cook in cooksByRestaurant
                     select new { cooksByRestaurant.Key.DeliveryDate, CookID = cook };
      Assert.That (query3.Count (), Is.GreaterThan (0));
    }


    [Test]
    public void GroupBy_AccessKey_Nesting ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<Order> ()
          from x in
            (
              from oi in o.OrderItems
              group oi by oi.Product into orderItemsByProduct
              select new { OrderID = o.ID, OrderItems = orderItemsByProduct }
              )
          let product = x.OrderItems.Key
          where product == "Mainboard"
          select o;

      CheckQueryResult (query, DomainObjectIDs.Order1);
    }

    [Test]
    public void GroupBy_UseGroupInFromExpression ()
    {
      var query = from o in QueryFactory.CreateLinqQuery<Order> ()
                  group o.ID by o.OrderNumber into orderByOrderNo
                  from id in orderByOrderNo
                  select new { orderByOrderNo.Key, OrderID = id };
      Assert.That (query.Count(), Is.EqualTo (7));

      var query2 =
          from o in QueryFactory.CreateLinqQuery<Order>()
          group o by o.OrderNumber
          into orderByOrderNo
          from o in orderByOrderNo
          where o != null
          select new { orderByOrderNo.Key, Order = o };
      Assert.That (query2.Count (), Is.EqualTo (7));
      
      var query3 =
          from o in QueryFactory.CreateLinqQuery<Order>()
          group o.OrderNumber by o.OrderNumber into orderByOrderNo
          from o in
            (
              from so in orderByOrderNo
              select so).Distinct ()
          select new { orderByOrderNo.Key, Order = o };
      Assert.That (query3.Count (), Is.EqualTo (7));
    }

    [Test]
    public void GroupBy_ResultSelector ()
    {
      var query = QueryFactory.CreateLinqQuery<Order>()
                              .Where (o => o.ID != DomainObjectIDs.InvalidOrder)
                              .GroupBy (o => o.Customer.ID, (key, group) => key);

      Assert.That (query.Count (), Is.EqualTo (4));
    }

    [Test]
    public void GroupBy_WithSubqueryKey ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order> ()
                   group o by QueryFactory.CreateLinqQuery<OrderItem> ().Where (oi => oi.Order == o).Select(oi => oi.Product).Count()).Select (g => g.Key);
      Assert.That (query.Count(), Is.EqualTo (3));
    }

    [Test]
    public void GroupBy_WithConstantKey ()
    {
      var query = QueryFactory.CreateLinqQuery<Order>().GroupBy (o => 0).Select (c => c.Key);

      Assert.That (query.Count (), Is.EqualTo (1));
    }
    
  }
}