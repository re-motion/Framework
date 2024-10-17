// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.NewObject;

[TestFixture]
public class DateTimePropertyTest: ClientTransactionBaseTest
{
  [Test]
  public void NewObject ()
  {
    const int milliseconds = 5;
    const int millisecondsAfterwards = 7;

    var order = Order.NewObject();

    order.DeliveryDate = new DateTime(1800, 3, 4, 1, 3, 6, milliseconds);
    order.OrderTicket = OrderTicket.NewObject();
    order.OrderItems.Add(OrderItem.NewObject("Product"));
    order.Official = DomainObjectIDs.Official1.GetObject<Official>();
    order.Customer = DomainObjectIDs.Customer1.GetObject<Customer>();

    order.OrderTicket.FileName = "dummy";

    TestableClientTransaction.Commit();

    UnloadService.UnloadAll(TestableClientTransaction);

    order.EnsureDataAvailable();

    Assert.That(order.DeliveryDate, Is.EqualTo(new DateTime(1800, 3, 4, 1, 3, 6, millisecondsAfterwards)));
  }

  [Test]
  public void NewObject_Nullable ()
  {
    var customer = Customer.NewObject();

    customer.CustomerSince = null;
    customer.Ceo = Ceo.NewObject();
    customer.Name = "dummy";
    customer.Ceo.Name = "dummyCeo";

    TestableClientTransaction.Commit();

    UnloadService.UnloadAll(TestableClientTransaction);

    customer.EnsureDataAvailable();

    Assert.That(customer.CustomerSince, Is.EqualTo(null));
  }
}
