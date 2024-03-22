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
using Remotion.Mixins;
using Remotion.TypePipe;
using Remotion.Validation.Mixins.IntegrationTests.TestDomain.ComponentA;
using Remotion.Validation.Mixins.IntegrationTests.TestDomain.ComponentB;

namespace Remotion.Validation.Mixins.IntegrationTests
{
  [TestFixture]
  public class ErrorMessagesGlobalizationIntegrationTest : IntegrationTestBase
  {
    [Test]
    public void BuildCustomerValidator_CustomerMixinTargetValidator ()
    {
      var customer = ObjectFactory.Create<Customer>(ParamList.Empty);
      customer.FirstName = "something";
      customer.LastName = "Mayr";
      customer.UserName = "mm2";

      var validator = ValidationBuilder.BuildValidator<Customer>();

      var result = validator.Validate(customer);

      Assert.That(result.IsValid, Is.False);
      Assert.That(result.Errors.Count, Is.EqualTo(1));
      Assert.That(result.Errors.First().ValidatedProperties.Select(vp => vp.Property.Name), Is.EqualTo(new [] { "FirstName" }));
      Assert.That(result.Errors.First().LocalizedValidationMessage, Is.EqualTo("Enter a valid value."));
    }

    [Test]
    public void BuildSpecialCustomerValidator_InvalidCustomerLastName_LengthValidatorFailed ()
    {
      //HardConstraintLengthValidator defined in CustomerValidationCollector1 not removed by SpecialCustomerValidationCollector1!

      var specialCustomer = ObjectFactory.Create<SpecialCustomer1>(ParamList.Empty);
      specialCustomer.UserName = "Test123456";
      specialCustomer.LastName = "LastNameTooLong";
      var validator = ValidationBuilder.BuildValidator<SpecialCustomer1>();

      var result = validator.Validate(specialCustomer);

      Assert.That(result.IsValid, Is.False);
      Assert.That(result.Errors.First().ValidatedProperties.Select(vp => vp.Property.Name), Is.EqualTo(new [] { "LastName" }));
      Assert.That(result.Errors.First().LocalizedValidationMessage, Is.EqualTo("Enter at least 2 and no more than 8 characters."));
    }

    [Test]
    public void BuildCustomerValidator_InvalidCustomerUserName_EqualsValidatorFailed ()
    {
      var customer = ObjectFactory.Create<Customer>(ParamList.Empty);
      customer.UserName = "Test";
      customer.LastName = "Muster";

      var validator = ValidationBuilder.BuildValidator<Customer>();

      var result = validator.Validate(customer);

      Assert.That(result.IsValid, Is.False);
      Assert.That(
          result.Errors.SelectMany(e => e.ValidatedProperties.Select(vp => $"{vp.Property.Name}: {e.LocalizedValidationMessage}")),
          Is.EquivalentTo(new [] { "FirstName: Enter a value.", "UserName: Enter a valid value." }));
    }

    [Test]
    public void BuildCustomerValidator_CustomerMixinIntroducedValidator_AttributeBaseRuleNotRemoveByRuleWithRemoveFrom ()
    {
      var customer = ObjectFactory.Create<Customer>(ParamList.Empty);
      customer.FirstName = "Ralf";
      customer.LastName = "Mayr";
      customer.UserName = "mm2";
      ((ICustomerIntroduced)customer).Title = "Chef1";

      var validator = ValidationBuilder.BuildValidator<Customer>();

      var result = validator.Validate(customer);

      Assert.That(result.IsValid, Is.False);
      Assert.That(result.Errors.Count, Is.EqualTo(1));
      Assert.That(result.Errors.First().ValidatedProperties.Select(vp => vp.Property.Name), Is.EqualTo(new [] { "Title" }));
      Assert.That(result.Errors.First().LocalizedValidationMessage, Is.EqualTo("Enter a valid value."));
    }
  }
}
