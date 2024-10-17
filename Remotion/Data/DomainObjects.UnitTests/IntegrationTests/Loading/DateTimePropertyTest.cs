// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Loading;

[TestFixture]
public class DateTimePropertyTest: ClientTransactionBaseTest
{
  [Test]
  public void LoadObject ()
  {
    var order = DomainObjectIDs.Order1.GetObject<Order>();
    Assert.That(order.DeliveryDate, Is.EqualTo(new DateTime(2005, 1, 1)));
  }

  [Test]
  public void LoadObject_Nullable ()
  {
    var customer = DomainObjectIDs.Customer4.GetObject<Customer>();
    Assert.That(customer.CustomerSince, Is.EqualTo(null));
  }
}
