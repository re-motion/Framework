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
  public class GroupByIntegrationTest : IntegrationTestBase
  {
    [Test]
    public void GroupBy_WithAggregateFunction ()
    {
      var result = QueryFactory.CreateLinqQuery<Order>().GroupBy (o => o.ID.ClassID).Count();

      Assert.That (result, Is.EqualTo (1));
    }
    
    [Test]
    public void GroupBy_GroupingWithSeveralAggregateFunction ()
    {
      var result = from o in QueryFactory.CreateLinqQuery<Order>()
                   group o by o.ID.ClassID
                   into ordersByClassID
                   select
                       new
                       {
                           ClassID = ordersByClassID.Key,
                           Count = ordersByClassID.Count(),
                           Sum = ordersByClassID.Sum (o => o.OrderNumber),
                           Min = ordersByClassID.Min (o => o.OrderNumber)
                       };

      Assert.That (result.ToArray(), Is.EqualTo (new[] { new { ClassID = "Order", Count = 7, Sum = 120, Min = 1 } }));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "There was an error generating SQL for the query 'DomainObjectQueryable<Order> => GroupBy([o].OrderNumber, [o])'. This SQL generator does "
        + "not support queries returning groupings that result from a GroupBy operator because SQL is not suited to efficiently return LINQ "
        + "groupings. Use 'group into' and either return the items of the groupings by feeding them into an additional from clause, or perform an "
        + "aggregation on the groupings. \r\n\r\n"
        + "Eg., instead of: \r\n"
        + "'from c in Cooks group c.ID by c.Name', \r\n"
        + "write: \r\n"
        + "'from c in Cooks group c.ID by c.Name into groupedCooks \r\n"
        + " from c in groupedCooks select new { Key = groupedCooks.Key, Item = c }', \r\n"
        + "or: \r\n"
        + "'from c in Cooks group c.ID by c.Name into groupedCooks \r\n"
        + " select new { Key = groupedCooks.Key, Count = groupedCooks.Count() }'.")]
    public void GroupBy_TopLevel ()
    {
      var query = QueryFactory.CreateLinqQuery<Order>().GroupBy (o => o.OrderNumber);

      query.ToArray();
    }

    [Test]
    public void GroupBy_WithinSubqueryInFromClause ()
    {
      var query = from ordersByCustomer in QueryFactory.CreateLinqQuery<Order>().GroupBy (o => o.Customer)
                  where ordersByCustomer.Key.Name.StartsWith ("Kunde")
                  select new { ordersByCustomer.Key.Name, Count = ordersByCustomer.Count() };
      var result = query.ToArray();

      Assert.That (
          result,
          Is.EquivalentTo (
              new[]
              {
                  new { Name = "Kunde 1", Count = 2 },
                  new { Name = "Kunde 3", Count = 1 }, 
                  new { Name = "Kunde 4", Count = 2 }
              }));
    }

  }
}