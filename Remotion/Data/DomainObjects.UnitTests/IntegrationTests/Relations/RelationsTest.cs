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
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Relations
{
  [TestFixture]
  public class RelationsTest : ClientTransactionBaseTest
  {
    [Test]
    public void OneToOneRelationChangeTest ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      OrderTicket orderTicket = order.OrderTicket;

      var orderEventReceiver = new DomainObjectRelationCheckEventReceiver (order);
      var orderTicketEventReceiver = new DomainObjectRelationCheckEventReceiver (orderTicket);

      orderTicket.Order = null;

      Assert.That (orderEventReceiver.HasRelationChangingEventBeenCalled, Is.True);
      Assert.That (orderTicketEventReceiver.HasRelationChangingEventBeenCalled, Is.True);
      Assert.That (orderEventReceiver.GetChangingRelatedDomainObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"), Is.SameAs (orderTicket));
      Assert.That (orderTicketEventReceiver.GetChangingRelatedDomainObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"), Is.SameAs (order));

      Assert.That (orderEventReceiver.HasRelationChangedEventBeenCalled, Is.True);
      Assert.That (orderTicketEventReceiver.HasRelationChangedEventBeenCalled, Is.True);
      Assert.That (orderEventReceiver.GetChangedRelatedDomainObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"), Is.SameAs (null));
      Assert.That (orderTicketEventReceiver.GetChangedRelatedDomainObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"), Is.SameAs (null));
    }
  }
}