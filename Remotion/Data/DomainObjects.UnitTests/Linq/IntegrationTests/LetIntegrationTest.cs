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
  public class LetIntegrationTest : IntegrationTestBase
  {
    [Test]
    public void QueryWithLet_LetWithTable ()
    {
      var orders = from o in QueryFactory.CreateLinqQuery<Order>()
                   let x = o
                   select x;

      CheckQueryResult (orders, DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4, DomainObjectIDs.Order5,
                        DomainObjectIDs.InvalidOrder, DomainObjectIDs.Order2, DomainObjectIDs.OrderWithoutOrderItems);
    }

    [Test]
    public void QueryWithLet_LetWithColumn ()
    {
      var orders = from o in QueryFactory.CreateLinqQuery<Order>()
                   let y = o.OrderNumber
                   where y > 1 && y < 6
                   select o;

      CheckQueryResult (orders,
                        DomainObjectIDs.Order4, DomainObjectIDs.Order2, DomainObjectIDs.Order3, DomainObjectIDs.Order5);
    }

    [Test]
    public void QueryWithLet_LetWithColumn2 ()
    {
      var orders = from o in QueryFactory.CreateLinqQuery<Order>()
                   let x = o.Customer.Name
                   where x == "Kunde 1"
                   select o;
      CheckQueryResult (orders, DomainObjectIDs.Order1, DomainObjectIDs.Order2);
    }

    [Test]
    public void QueryWithSeveralJoinsAndLet ()
    {
      var ceos = from o in QueryFactory.CreateLinqQuery<Order>()
                 let x = o.Customer.Ceo
                 where x.Name == "Hugo Boss"
                 select x;

      CheckQueryResult (ceos, DomainObjectIDs.Ceo5);
    }

    [Test]
    public void QueryWithSeveralLets ()
    {
      var orders = from o in QueryFactory.CreateLinqQuery<Order>()
                   let x = o
                   let y = o.Customer
                   select x;

      CheckQueryResult (orders, DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4, DomainObjectIDs.Order5, 
                        DomainObjectIDs.InvalidOrder, DomainObjectIDs.Order2, DomainObjectIDs.OrderWithoutOrderItems);
    }

    [Test]
    public void QueryWithLet_AndMultipleFromClauses ()
    {
      var query =
          from ot in QueryFactory.CreateLinqQuery<OrderTicket> ()
          from o in QueryFactory.CreateLinqQuery<Order>()
          let x = ot.Order
          where ot.Order.OrderNumber == 1
          where o == ot.Order
          select x;
      CheckQueryResult (query, DomainObjectIDs.Order1);
    }

    [Test]
    public void QueryWithMemberFromClause_WithLet ()
    {
      var query =
          from ot in QueryFactory.CreateLinqQuery<OrderTicket> ()
          let x = ot.Order
          from oi in x.OrderItems
          where ot.Order.OrderNumber == 1
          select oi;
      CheckQueryResult (query, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
    }
  }
}
