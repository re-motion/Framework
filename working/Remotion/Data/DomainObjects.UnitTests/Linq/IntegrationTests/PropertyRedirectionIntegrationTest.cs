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
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Linq.IntegrationTests
{
  [TestFixture]
  public class PropertyRedirectionIntegrationTest : IntegrationTestBase
  {
    [Test]
    public void RedirectedValueProperty ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<Order> ()
          where o.RedirectedOrderNumber == 1
          select o;
      CheckQueryResult (query, DomainObjectIDs.Order1);
    }

    [Test]
    public void RedirectedRelationProperty_CardinalityOne ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<Order> ()
          where o.OrderNumber == 1
          select o.RedirectedOrderTicket;
      CheckQueryResult (query, DomainObjectIDs.OrderTicket1);
    }

    [Test]
    public void RedirectedRelationProperty_CardinalityMany ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<Order> ()
          from oi in o.RedirectedOrderItems
          where o.OrderNumber == 1
          select oi;
      CheckQueryResult (query, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
    }

    [Test]
    public void RedirectedRedirectedProperty ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<Order> ()
          where o.RedirectedRedirectedOrderNumber == 1
          select o;
      CheckQueryResult (query, DomainObjectIDs.Order1);
    }

    [Test]
    public void PropertyRedirectedToInterface_ImplementedByMixin ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<TargetClassForPersistentMixin> ()
          where o.RedirectedPersistentProperty == 1
          select o;
      CheckQueryResult (query);
    }

    [Test]
    public void PropertyRedirectedToInterface_ImplementedByMixinAndTargetClass ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<TargetClassWithSameInterfaceAsPersistentMixin>()
          where o.PersistentPropertyRedirectedToMixin == 1
          select o;
      CheckQueryResult (query);
    }

    [Test]
    public void RedirectedProperty_ViaExtensionMethod ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<Order> ()
          where o.GetRedirectedOrderNumber() == 1
          select o;
      CheckQueryResult (query, DomainObjectIDs.Order1);
    }

    [Test]
    public void RedirectedRedirectedProperty_ViaExtensionMethod ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<Order> ()
          where o.GetRedirectedRedirectedOrderNumber() == 1
          select o;
      CheckQueryResult (query, DomainObjectIDs.Order1);
    }
  }
}
