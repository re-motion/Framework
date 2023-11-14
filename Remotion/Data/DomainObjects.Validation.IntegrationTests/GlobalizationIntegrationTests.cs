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
using Remotion.Data.DomainObjects.Validation.IntegrationTests.Testdomain;

namespace Remotion.Data.DomainObjects.Validation.IntegrationTests
{
  [TestFixture]
  public class GlobalizationIntegrationTests : IntegrationTestBase
  {
    public override void SetUp ()
    {
      base.SetUp();

      ShowLogOutput = false;
    }

    [Test]
    public void BuildCustomerValidator_ValidationFailuresAreLocalizedThroughMixinResource ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var customer = Customer.NewObject();
        ((ICustomerIntroduced)customer).Address = Address.NewObject();
        ((ICustomerIntroduced)customer).Title = "Chef1";

        var validator = ValidationProvider.GetValidator(typeof(Customer));

        var result1 = validator.Validate(customer);
        Assert.That(result1.IsValid, Is.False);
        Assert.That(
            result1.Errors.SelectMany(e => e.ValidatedProperties.Select(vp => $"{vp.Property.Name}: {e.ErrorMessage}")),
            Is.EquivalentTo(new[] { "Title: The value must not be equal to 'Chef1'." }));
      }
    }

    [Test]
    public void BuildOrderValidator_ValidationFailuresAreLocalizedThroughDomainObjectResource ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var order = Order.NewObject();
        order.Number = "er";
        order.OrderItems.Add(OrderItem.NewObject());

        var validator = ValidationProvider.GetValidator(typeof(Order));

        var result1 = validator.Validate(order);
        Assert.That(result1.IsValid, Is.False);
        Assert.That(
            result1.Errors.SelectMany(e => e.ValidatedProperties.Select(vp => $"{vp.Property.Name}: {e.ErrorMessage}")),
            Is.EquivalentTo(new[] { "Number: The value must have between 3 and 8 characters." }));
      }
    }
  }
}
