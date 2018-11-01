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
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.EagerFetching
{
  [TestFixture]
  public class EagerFetchingWithInvalidMandatoryRelationsTest : ClientTransactionBaseTest
  {
    [Test]
    public void FetchingMandatoryCollectionEndPoint_WithNoRelatedObjects_Throws ()
    {
      var query = QueryFactory.CreateLinqQuery<Order>().Where (o => o.ID == DomainObjectIDs.OrderWithoutOrderItems).FetchMany (o => o.OrderItems);

      Assert.That (
          () => query.ToArray(),
          Throws.TypeOf<UnexpectedQueryResultException> ().With.Message.EqualTo (
              "Eager fetching encountered an unexpected query result: The fetched mandatory collection property "
              + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems' "
              + "on object 'Order|f7607cbc-ab34-465c-b282-0531d51f3b04|System.Guid' contains no items."));
    }

    [Test]
    public void FetchingMandatoryVirtualObjectEndPoint_WithNoRelatedObject_Throws ()
    {
      var query = QueryFactory.CreateLinqQuery<Partner> ().Where (o => o.ID == DomainObjectIDs.PartnerWithoutCeo).FetchOne (p => p.Ceo);

      Assert.That (
          () => query.ToArray(),
          Throws.TypeOf<UnexpectedQueryResultException> ().With.Message.EqualTo (
              "Eager fetching encountered an unexpected query result: The fetched mandatory relation property "
              + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo' on object "
              + "'Partner|a65b123a-6e17-498e-a28e-946217c0ae30|System.Guid' contains no related object."));
    }
    
    [Test]
    public void FetchingMandatoryRealObjectEndPoint_WithNullValue_DoesNotThrow ()
    {
      var query = QueryFactory.CreateLinqQuery<OrderItem> ().Where (o => o.ID == DomainObjectIDs.OrderItemWithoutOrder).FetchOne (oi => oi.Order);

      // Note: This test documents current behavior, not necessarily desired behavior.
      OrderItem orderItemWithoutOrder = null;
      Assert.That (() => orderItemWithoutOrder = query.ToArray().Single(), Throws.Nothing);

      Assert.That (orderItemWithoutOrder.Order, Is.Null);
    }
  }
}