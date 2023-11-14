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
  public class ValidationRulesIntegrationTests : IntegrationTestBase
  {
    public override void SetUp ()
    {
      base.SetUp();

      ShowLogOutput = false;
    }

    [Test]
    public void BuildValidator_MandatoryReStoreAttributeIsAppliedOnDomainObject ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var orderItem1 = OrderItem.NewObject();
        orderItem1.Order = null;
        orderItem1.ProductReference = ProductReference.NewObject();

        var orderItem2 = OrderItem.NewObject();
        orderItem2.Order = Order.NewObject();
        orderItem2.ProductReference = ProductReference.NewObject();

        var validator = ValidationProvider.GetValidator(typeof(OrderItem));

        var result1 = validator.Validate(orderItem1);
        Assert.That(result1.IsValid, Is.False);
        Assert.That(result1.Errors.Count, Is.EqualTo(1));
        Assert.That(result1.Errors.First().ValidatedProperties.Select(vp => vp.Property.Name), Is.EqualTo(new [] { "Order" }));
        Assert.That(result1.Errors.First().ErrorMessage, Is.EqualTo("The value must not be null."));

        var result2 = validator.Validate(orderItem2);
        Assert.That(result2.IsValid, Is.True);
      }
    }

    [Test]
    public void BuildValidator_MandatoryReStoreAttributeAppliedOnDomainObjectMixin ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var customer1 = Customer.NewObject();
        var customer2 = Customer.NewObject();
        ((ICustomerIntroduced)customer2).Address = Address.NewObject();

        var validator = ValidationProvider.GetValidator(typeof(Customer));

        var result1 = validator.Validate(customer1);
        Assert.That(result1.IsValid, Is.False);
        Assert.That(result1.Errors.Count, Is.EqualTo(1));
        Assert.That(result1.Errors.First().ValidatedProperties.Select(vp => vp.Property.Name), Is.EqualTo(new [] { "Address" }));
        Assert.That(result1.Errors.First().ErrorMessage, Is.EqualTo("The value must not be null."));

        var result2 = validator.Validate(customer2);
        Assert.That(result2.IsValid, Is.True);
      }
    }

    [Test]
    [Ignore("TODO RM-5906")]
    public void BuildValidator_ValidationAttributesAppliedOnDerivedDomainObject ()
    {
      // Test validation declared via validation attribute on property in derived type
      // Test validation declared via validation attribute on overridden property in derived type
      // Test validation declared via validation attribute on property in base type
    }

    [Test]
    [Ignore("TODO RM-5906")]
    public void BuildValidator_ValidationAttributesAppliedOnDerivedDomainObjectMixin ()
    {
      // Test validation declared via validation attribute on property in derived mixin type
      // Test validation declared via validation attribute on overridden property in derived mixin type
      // Test validation declared via validation attribute on property in base mixin type
    }

    [Test]
    public void BuildValidator_NotLoadedCollectionNotValidated_AndDataNotLoaded ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var order1 = Order.NewObject();
        order1.Number = "001";

        var order2 = Order.NewObject();
        order2.Number = "002";

        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          Assert.That(order1.OrderItems, Is.Empty);
          Assert.That(order1.OrderItems.IsDataComplete, Is.True);
          Assert.That(order2.OrderItems.IsDataComplete, Is.False);

          var validator = ValidationProvider.GetValidator(typeof(Order));

          var result1 = validator.Validate(order1);
          Assert.That(result1.IsValid, Is.False);
          Assert.That(result1.Errors.Count, Is.EqualTo(1));
          Assert.That(result1.Errors.First().ValidatedProperties.Select(vp => vp.Property.Name), Is.EqualTo(new [] { "OrderItems" }));
          Assert.That(result1.Errors.First().ErrorMessage, Is.EqualTo("The value must not be empty."));
          Assert.That(order1.OrderItems.IsDataComplete, Is.True);

          var result2 = validator.Validate(order2);
          Assert.That(result2.IsValid, Is.True);
          Assert.That(order2.OrderItems.IsDataComplete, Is.False);
        }
      }
    }

    [Test]
    public void BuildValidator_NotLoadedReferenceNotValidated_AndDataNotLoaded ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var product1 = Product.NewObject();
        var productReference1 = ProductReference.NewObject();
        productReference1.Product = product1;

        var product2 = Product.NewObject();
        var productReference2 = ProductReference.NewObject();
        productReference2.Product = product2;

        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          productReference1.EnsureDataAvailable();
          Assert.That(productReference1.OrderItem, Is.Null);
          Assert.That(product1.State.IsNotLoadedYet, Is.True);

          productReference2.EnsureDataAvailable();
          Assert.That(product2.State.IsNotLoadedYet, Is.True);

          var validator = ValidationProvider.GetValidator(typeof(ProductReference));

          var result1 = validator.Validate(productReference1);
          Assert.That(result1.IsValid, Is.False);
          Assert.That(result1.Errors.Count, Is.EqualTo(1));
          Assert.That(result1.Errors.First().ValidatedProperties.Select(vp => vp.Property.Name), Is.EqualTo(new [] { "OrderItem" }));
          Assert.That(result1.Errors.First().ErrorMessage, Is.EqualTo("The value must not be null."));
          Assert.That(product1.State.IsNotLoadedYet, Is.True);

          var result2 = validator.Validate(productReference2);
          Assert.That(result2.IsValid, Is.True);
          Assert.That(product2.State.IsNotLoadedYet, Is.True);
        }
      }
    }
  }
}
