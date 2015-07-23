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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Relations
{
  [TestFixture]
  public class LoadingRelationsWithInvalidMandatoryRelationsTest : ClientTransactionBaseTest
  {
    [Test]
    public void LoadingMandatoryCollectionEndPoint_WithNoRelatedObjects_Throws ()
    {
      var order = DomainObjectIDs.OrderWithoutOrderItems.GetObject<Order>();

      Assert.That (
          () => order.OrderItems.EnsureDataComplete (), 
          Throws.TypeOf<PersistenceException> ().With.Message.EqualTo (
              "Collection for mandatory relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems' "
              + "on object 'Order|f7607cbc-ab34-465c-b282-0531d51f3b04|System.Guid' contains no items."));
    }

    [Test]
    public void LoadingMandatoryVirtualObjectEndPoint_WithNoRelatedObject_Throws ()
    {
      var partner = DomainObjectIDs.PartnerWithoutCeo.GetObject<Partner> ();

      Assert.That (
          () => partner.Ceo,
          Throws.TypeOf<PersistenceException> ().With.Message.EqualTo (
              "Mandatory relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo' on object "
              + "'Partner|a65b123a-6e17-498e-a28e-946217c0ae30|System.Guid' contains no item."));
    }

    [Test]
    public void LoadingMandatoryRealObjectEndPoint_WithNullValue_DoesNotThrow ()
    {
      // Note: This test documents current behavior, not necessarily desired behavior.
      OrderItem orderItemWithoutOrder = null;
      Assert.That (() => orderItemWithoutOrder = DomainObjectIDs.OrderItemWithoutOrder.GetObject<OrderItem>(), Throws.Nothing);

      Assert.That (orderItemWithoutOrder.Order, Is.Null);
    }
  }
}