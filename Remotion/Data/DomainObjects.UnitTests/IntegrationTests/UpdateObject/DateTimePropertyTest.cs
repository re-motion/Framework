// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.UpdateObject;

[TestFixture]
public class DateTimePropertyTest : ClientTransactionBaseTest
{
  [Test]
  public void Update ()
  {
    const int milliseconds = 4;
    const int millisecondsAfterwards = 3;
    var order = DomainObjectIDs.Order1.GetObject<Order>();
    Assert.That(order.DeliveryDate, Is.EqualTo(new DateTime(2005, 1, 1)));

    order.DeliveryDate = new DateTime(1800, 3, 4, 1, 3, 6, milliseconds);
    TestableClientTransaction.Commit();

    UnloadService.UnloadAll(TestableClientTransaction);

    order.EnsureDataAvailable();

    Assert.That(order.DeliveryDate, Is.EqualTo(new DateTime(1800, 3, 4, 1, 3, 6, millisecondsAfterwards)));
  }

  [Test]
  public void Update_Nullable ()
  {
    var customer = DomainObjectIDs.Customer1.GetObject<Customer>();
    Assert.That(customer.CustomerSince, Is.EqualTo(new DateTime(2000, 1, 1)));

    customer.CustomerSince = null;
    TestableClientTransaction.Commit();

    UnloadService.UnloadAll(TestableClientTransaction);

    customer.EnsureDataAvailable();

    Assert.That(customer.CustomerSince, Is.EqualTo(null));
  }
}
