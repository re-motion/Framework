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
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.TableInheritance;

namespace Remotion.Data.DomainObjects.UnitTests.Linq.IntegrationTests
{
  [TestFixture]
  public class MultiFromIntegrationTest : IntegrationTestBase
  {
    [Test]
    public void QueryWithInto ()
    {
      var orders = from c in QueryFactory.CreateLinqQuery<Customer> () 
                   where c.ID == DomainObjectIDs.Customer1 
                   select c.Orders 
                   into x 
                       from o in x 
                       from oi in o.OrderItems
                       select oi;

      CheckQueryResult (orders, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2, DomainObjectIDs.OrderItem6);
    }

    [Test]
    public void Query_WithSeveralFroms ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<Order>()
          from c in QueryFactory.CreateLinqQuery<OrderTicket>()
          where c.Order == o
          where o.OrderNumber == 1
          select c;

      CheckQueryResult (query, DomainObjectIDs.OrderTicket1);
    }

    [Test]
    public void QueryWithMemberFromClause ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<Order> ()
          from oi in o.OrderItems
          where o.OrderNumber == 1
          select oi;
      CheckQueryResult (query, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
    }

    [Test]
    public void TableInheritance_MemberJoinViaBaseClass ()
    {
      var query = from c in QueryFactory.CreateLinqQuery<TIClient> ()
                  from domainBase in c.AssignedObjects
                  where domainBase.CreatedAt == new DateTime (2006, 01, 03)
                  select domainBase;
      
      var domainObjectIDs = new TableInheritanceDomainObjectIDs (Configuration);
      CheckQueryResult (query, domainObjectIDs.Person);
    }
  }
}
