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
  public class ExplicitJoinsIntegrationTest : IntegrationTestBase
  {
    [Test]
    public void ExplicitJoin ()
    {
      CheckQueryResult (
          from c in QueryFactory.CreateLinqQuery<Order>()
          join k in QueryFactory.CreateLinqQuery<Customer>() on c.Customer equals k
          where c.OrderNumber == 5
          select c , DomainObjectIDs.Order5);
    }

    [Test]
    public void ExplicitJoinWithInto_Once ()
    {
      CheckQueryResult (
          from o in QueryFactory.CreateLinqQuery<Order> ()
          join i in QueryFactory.CreateLinqQuery<OrderItem> () on o equals i.Order into goi
          from oi in goi
          select oi,
          DomainObjectIDs.OrderItem1,
          DomainObjectIDs.OrderItem2,
          DomainObjectIDs.OrderItem3,
          DomainObjectIDs.OrderItem4,
          DomainObjectIDs.OrderItem5,
          DomainObjectIDs.OrderItem6);
    }

    [Test]
    public void ExplicitJoinWithInto_Twice() 
    {
      CheckQueryResult (
          from o in QueryFactory.CreateLinqQuery<Order> ()
          join i in QueryFactory.CreateLinqQuery<OrderItem> () on o equals i.Order into goi
          join c in QueryFactory.CreateLinqQuery<Customer> () on o.Customer equals c into goc
          from oi in goi
          from oc in goc
          select oi,
          DomainObjectIDs.OrderItem1,
          DomainObjectIDs.OrderItem2,
          DomainObjectIDs.OrderItem3,
          DomainObjectIDs.OrderItem4,
          DomainObjectIDs.OrderItem5,
          DomainObjectIDs.OrderItem6);
    }

    [Test]
    public void ExplicitJoinWithInto_InSubstatement_Once ()
    {
      CheckQueryResult (
          from o in QueryFactory.CreateLinqQuery<Order> ()
          where o.OrderNumber ==
            (from so in QueryFactory.CreateLinqQuery<Order> ()
             join si in QueryFactory.CreateLinqQuery<OrderItem> () on so equals si.Order into goi
             from oi in goi
             select oi.Order.OrderNumber).First ()
          select o,
          DomainObjectIDs.Order5);
    }

    [Test]
    public void ExplicitJoinWithInto_InSubstatement_Twice ()
    {
      CheckQueryResult (
          from o in QueryFactory.CreateLinqQuery<Order> ()
          where o.OrderNumber ==
            (from so in QueryFactory.CreateLinqQuery<Order> ()
             join si in QueryFactory.CreateLinqQuery<OrderItem> () on so equals si.Order into goi
             join si in QueryFactory.CreateLinqQuery<Customer>() on so.Customer equals si into goc
             from oi in goi
             from oc in goc
             select oi.Order.OrderNumber).First ()
          select o,
          DomainObjectIDs.Order5);
    }

    [Test]
    public void ExplicitJoinWithInto_InTwoSubstatements ()
    {
      CheckQueryResult (
          from o in QueryFactory.CreateLinqQuery<Order> ()
          where o.OrderNumber ==
            (from so in QueryFactory.CreateLinqQuery<Order> ()
             join si in QueryFactory.CreateLinqQuery<OrderItem> () on so equals si.Order into goi
             from oi in goi
             select oi.Order.OrderNumber).First ()
            && o.Customer.Name ==
              (from so in QueryFactory.CreateLinqQuery<Order> ()
               join sc in QueryFactory.CreateLinqQuery<Customer> () on so.Customer equals sc into goc
               from oc in goc
               select oc.Name).First ()
          select o);
    }

    [Test]
    public void ExplicitJoinWithInto_InSameStatementAndInSubstatement ()
    {
      CheckQueryResult (
        from o in QueryFactory.CreateLinqQuery<Order> ()
        join i in QueryFactory.CreateLinqQuery<OrderItem> () on o equals i.Order into goi
        from oi in goi
        where oi.Product ==
        (from so in QueryFactory.CreateLinqQuery<Order> ()
         join si in QueryFactory.CreateLinqQuery<OrderItem> () on so equals si.Order into gia
         from ia in gia
         select ia.Product).First ()
        select oi,
        DomainObjectIDs.OrderItem5);
    }

    [Test]
    public void ExplicitJoinWithInto_WithOrderBy ()
    {
      CheckQueryResult (
          from o in QueryFactory.CreateLinqQuery<Order> ()
          join i in QueryFactory.CreateLinqQuery<OrderItem> ().OrderBy(oi => oi.Product) on o equals i.Order into goi
          from oi in goi
          select oi,
          DomainObjectIDs.OrderItem5,
          DomainObjectIDs.OrderItem2,
          DomainObjectIDs.OrderItem3,
          DomainObjectIDs.OrderItem4,
          DomainObjectIDs.OrderItem1,
          DomainObjectIDs.OrderItem6);
    }
    
  }
}