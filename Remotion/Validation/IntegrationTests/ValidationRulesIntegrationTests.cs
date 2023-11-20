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
using Remotion.Validation.Implementation;
using Remotion.Validation.IntegrationTests.TestDomain.ComponentA;
using Remotion.Validation.IntegrationTests.TestDomain.ComponentB;
using Remotion.Validation.Results;

namespace Remotion.Validation.IntegrationTests
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
    public void BuildSpecialCustomer_RemoveLastNameHardConstraint_ThrowsException ()
    {
      Assert.That(
          () => ValidationBuilder.BuildValidator<SpecialCustomer2>(),
          Throws.TypeOf<ValidationConfigurationException>().And.Message.EqualTo(
              "Attempted to remove non-removable validator(s) 'LengthValidator' on property "
              + "'Remotion.Validation.IntegrationTests.TestDomain.ComponentA.Person.LastName'."));
    }

    [Test]
    public void BuildSpecialCustomer_RemoveObjectValidatorHardConstraint_ThrowsException ()
    {
      Assert.That(
          () => ValidationBuilder.BuildValidator<SpecialCustomer3>(),
          Throws.TypeOf<ValidationConfigurationException>().And.Message.EqualTo(
              "Attempted to remove non-removable validator(s) 'FakeCustomerValidator' on type "
              + "'Remotion.Validation.IntegrationTests.TestDomain.ComponentA.Customer'."));
    }

    [Test]
    public void BuildAddressValidator_WithConditionApplied ()
    {
      var address1 = new Address { Country = "Deutschland", PostalCode = "DE - 432134" };
      var address2 = new Address { Country = "Deutschland", PostalCode = "AT - 1220" };
      var address3 = new Address { Street = "Maria Hilferstrasse 145", City = "Wien", PostalCode = "1090" };
      var address4 = new Address { Street = "Maria Hilferstrasse 145", City = "Salzburg", PostalCode = "1090" };
      var address5 = new Address { Country = "Brunei", PostalCode = "0000" };
      var address6 = new Address { Country = "Tschiputi" };

      var validator = ValidationBuilder.BuildValidator<Address>();

      var result1 = validator.Validate(address1);
      Assert.That(result1.IsValid, Is.True);

      var result2 = validator.Validate(address2);
      Assert.That(result2.IsValid, Is.False);
      Assert.That(result2.Errors.Count, Is.EqualTo(1));
      Assert.That(result2.Errors.First().ValidatedProperties.Select(vp => vp.Property.Name), Is.EqualTo(new [] { "PostalCode" }));
      Assert.That(result2.Errors.First().ErrorMessage, Is.EqualTo("The value must be in the correct format (^DE)."));

      var result3 = validator.Validate(address3);
      Assert.That(result3.IsValid, Is.True);

      var result4 = validator.Validate(address4);
      Assert.That(result4.IsValid, Is.False);
      Assert.That(result4.Errors.Count, Is.EqualTo(1));
      Assert.That(result4.Errors.First().ValidatedProperties.Select(vp => vp.Property.Name), Is.EqualTo(new [] { "City" }));
      Assert.That(result4.Errors.First().ErrorMessage, Is.EqualTo("The value must be in the correct format (Wien)."));

      var result5 = validator.Validate(address5);
      Assert.That(result5.IsValid, Is.True);

      var result6 = validator.Validate(address6);
      Assert.That(result6.IsValid, Is.False);
      Assert.That(result6.Errors.Count, Is.EqualTo(1));
      Assert.That(result6.Errors.First().ValidatedProperties.Select(vp => vp.Property.Name), Is.EqualTo(new [] { "PostalCode" }));
      Assert.That(result6.Errors.First().ErrorMessage, Is.EqualTo("The value must not be null."));
    }

    [Test]
    public void BuildSpecialAddressValidator ()
    {
      // SpecialAddress defines Street property new so there should be no validation on Street length
      // SpecialAddress overrides PostalCode so all validators of Address.PostalCode and SpecialAddress.PostalCode should be applied
      var address1 = new SpecialAddress { City = "12345678901", Street = "123456789012345678901234567890", PostalCode = "1234" };
      var address2 = new SpecialAddress { Street = "1234567890123456789012345", PostalCode = "1337", SpecialAddressIntroducedProperty = "Value" };
      var address3 = new SpecialAddress { Street = "1234567890123456789012345", SpecialAddressIntroducedProperty = "Value" };

      var validator = ValidationBuilder.BuildValidator<SpecialAddress>();

      var result1 = validator.Validate(address1);
      Assert.That(result1.IsValid, Is.False);
      Assert.That(result1.Errors.Count, Is.EqualTo(2));
      Assert.That(
          result1.Errors.SelectMany(e => e.ValidatedProperties.Select(vp => $"{vp.Property.Name}: {e.ErrorMessage}")),
          Is.EqualTo(new [] { "SpecialAddressIntroducedProperty: The value must not be null.", "PostalCode: The value must be in the correct format (1337)." }));

      var result2 = validator.Validate(address2);
      Assert.That(result2.IsValid, Is.True);

      var result3 = validator.Validate(address3);
      Assert.That(result3.IsValid, Is.False);
      Assert.That(result3.Errors.Count, Is.EqualTo(1));
      Assert.That(result3.Errors.First().ValidatedProperties.Select(vp => vp.Property.Name), Is.EqualTo(new [] { "PostalCode" }));
      Assert.That(result3.Errors.First().ErrorMessage, Is.EqualTo("The value must not be null."));
    }

    [Test]
    public void BuildEmployeeValidator_ConditionalMessage ()
    {
      var employee = new Employee { FirstName = "FirstName", LastName = "LastName" };

      var validator = ValidationBuilder.BuildValidator<Employee>();

      var result = validator.Validate(employee);

      Assert.That(result.IsValid, Is.False);
      Assert.That(result.Errors.Count, Is.EqualTo(1));
      Assert.That(result.Errors.First().ValidatedProperties.Select(vp => vp.Property.Name), Is.EqualTo(new [] { "Salary" }));
      Assert.That(result.Errors.First().ErrorMessage, Is.EqualTo("The value must not be equal to '0'."));
      Assert.That(result.Errors.First().LocalizedValidationMessage, Is.EqualTo("Conditional Message Text: Kein Gehalt definiert"));
    }

    [Test]
    public void BuildEmployeeValidator_WithExplicitPropertyGetter ()
    {
      var cvLines = new []
                    {
                        "Manager at some firm.",
                        "Intern at some other firm."
                    };
      var cv = string.Join(Environment.NewLine, cvLines);
      var employee = new Employee { FirstName = "FirstName", LastName = "LastName", Salary = 9000, CV = cv, CVLines = cvLines };

      var validator = ValidationBuilder.BuildValidator<Employee>();

      var result = validator.Validate(employee);

      Assert.That(result.IsValid, Is.False);
      Assert.That(result.Errors.Count, Is.EqualTo(1));
      Assert.That(result.Errors.First().ValidatedProperties.Select(vp => vp.Property.Name), Is.EqualTo(new [] { "CV" }));
      Assert.That(result.Errors.First().ValidatedProperties.Select(vp => vp.ValidatedPropertyValue), Is.EqualTo(new [] { cvLines }));
      Assert.That(result.Errors.First().LocalizedValidationMessage, Is.EqualTo("A line exceeds the maximum length of 25 characters."));
    }

    [Test]
    public void BuildSpecialPersonValidator_ConstraintRemovedByPredicate ()
    {
      var person1 = new SpecialPerson1 { FirstName = "Name", LastName = null };
      var person2 = new SpecialPerson1 { FirstName = "Name", LastName = "test" };
      var person3 = new SpecialPerson1 { FirstName = "Name", LastName = "Test" };

      var validator = ValidationBuilder.BuildValidator<SpecialPerson1>();

      var result1 = validator.Validate(person1);
      Assert.That(result1.IsValid, Is.False);
      Assert.That(result1.Errors.First().ValidatedProperties.Select(vp => vp.Property.Name), Is.EqualTo(new [] { "LastName" }));

      var result2 = validator.Validate(person2);
      Assert.That(result2.IsValid, Is.False);
      Assert.That(result2.Errors.First().ValidatedProperties.Select(vp => vp.Property.Name), Is.EqualTo(new [] { "LastName" }));

      var result3 = validator.Validate(person3);
      Assert.That(result3.IsValid, Is.True);
    }

    [Ignore("RM-5906: Obsolete due to default value removal on NotEmptyValidator.")]
    [Test]
    public void BuildOrderItemValidator_SetValueTypeToDefaulValue_ValidationFails ()
    {
      var orderItem = new OrderItem();
      orderItem.Quantity = 0;

      var validator = ValidationBuilder.BuildValidator<OrderItem>();

      var result = validator.Validate(orderItem);

      Assert.That(result.IsValid, Is.False);
      Assert.That(result.Errors.Count, Is.EqualTo(1));
      Assert.That(result.Errors.First().ErrorMessage, Is.EqualTo("Enter a value."));
    }

    [Test]
    public void BuildPersonValidator_ObjectValidatorApplied ()
    {
      var person1 = new Person { FirstName = null, LastName = "LastName" };
      var person2 = new Person { FirstName = "FirstName", LastName = null };
      var person3 = new Person { FirstName = "FirstName", LastName = "LastName" };

      var validator = ValidationBuilder.BuildValidator<Person>();

      var result1 = validator.Validate(person1);

      Assert.That(result1.IsValid, Is.False);
      Assert.That(result1.Errors.Count(e => e.ValidatedProperties.Count == 0), Is.EqualTo(1));
      Assert.That(
          result1.Errors.First(e => e.ValidatedProperties.Count == 0).ErrorMessage,
          Is.EqualTo("Person must have 'FirstName' and 'LastName' properties set."));
      Assert.That(
          result1.Errors.First(e => e.ValidatedProperties.Count == 0).LocalizedValidationMessage,
          Is.EqualTo("Localized validation message for 'RealPersonValidator': 'FirstName' or 'LastName' property is null or empty."));
      Assert.That(
          result1.Errors.First(e => e.ValidatedProperties.Count == 0).ValidatedProperties,
          Is.Empty);

      var result2 = validator.Validate(person2);

      Assert.That(result2.IsValid, Is.False);
      Assert.That(result2.Errors.Count(e => e.ValidatedProperties.Count == 0), Is.EqualTo(1));
      Assert.That(
          result2.Errors.First(e => e.ValidatedProperties.Count == 0).ErrorMessage,
          Is.EqualTo("Person must have 'FirstName' and 'LastName' properties set."));
      Assert.That(
          result2.Errors.First(e => e.ValidatedProperties.Count == 0).LocalizedValidationMessage,
          Is.EqualTo("Localized validation message for 'RealPersonValidator': 'FirstName' or 'LastName' property is null or empty."));
      Assert.That(
          result2.Errors.First(e => e.ValidatedProperties.Count == 0).ValidatedProperties,
          Is.Empty);

      var result3 = validator.Validate(person3);

      Assert.That(result3.IsValid, Is.True);
    }

    [Test]
    public void BuildEmployeeValidator_ObjectValidatorApplied ()
    {
      var employee1 = new Employee { FirstName = null, LastName = "LastName" };
      var employee2 = new Employee { FirstName = "FirstName", LastName = null };
      var employee3 = new Employee { FirstName = "FirstName", LastName = "LastName" };

      var validator = ValidationBuilder.BuildValidator<Employee>();

      var result1 = validator.Validate(employee1);

      Assert.That(result1.IsValid, Is.False);
      Assert.That(result1.Errors.Count(e => e.ValidatedProperties.Count == 0), Is.EqualTo(1));
      Assert.That(
          result1.Errors.First(e => e.ValidatedProperties.Count == 0).ErrorMessage,
          Is.EqualTo("Person must have 'FirstName' and 'LastName' properties set."));
      Assert.That(
          result1.Errors.First(e => e.ValidatedProperties.Count == 0).LocalizedValidationMessage,
          Is.EqualTo("Localized validation message for 'RealPersonValidator': 'FirstName' or 'LastName' property is null or empty."));
      Assert.That(
          result1.Errors.First(e => e.ValidatedProperties.Count == 0).ValidatedProperties,
          Is.Empty);

      var result2 = validator.Validate(employee2);

      Assert.That(result2.IsValid, Is.False);
      Assert.That(result2.Errors.Count(e => e.ValidatedProperties.Count == 0), Is.EqualTo(1));
      Assert.That(
          result2.Errors.First(e => e.ValidatedProperties.Count == 0).ErrorMessage,
          Is.EqualTo("Person must have 'FirstName' and 'LastName' properties set."));
      Assert.That(
          result2.Errors.First(e => e.ValidatedProperties.Count == 0).LocalizedValidationMessage,
          Is.EqualTo("Localized validation message for 'RealPersonValidator': 'FirstName' or 'LastName' property is null or empty."));
      Assert.That(
          result2.Errors.First(e => e.ValidatedProperties.Count == 0).ValidatedProperties,
          Is.Empty);

      var result3 = validator.Validate(employee3);
      Assert.That(result3.Errors.Where(e => e.ValidatedProperties.Count == 0), Is.Empty);
    }

    [Test]
    public void BuildCustomerValidator_ObjectValidatorRemoved ()
    {
      var customer1 = new Customer { FirstName = null, LastName = "LastName" };
      var customer2 = new Customer { FirstName = "FirstName", LastName = null };
      var customer3 = new Customer { FirstName = "FirstName", LastName = "LastName" };

      var validator = ValidationBuilder.BuildValidator<Customer>();

      var result1 = validator.Validate(customer1);
      Assert.That(result1.Errors.Where(e => e.ValidatedProperties.Count == 0), Is.Empty);

      var result2 = validator.Validate(customer2);
      Assert.That(result2.Errors.Where(e => e.ValidatedProperties.Count == 0), Is.Empty);

      var result3 = validator.Validate(customer3);
      Assert.That(result3.IsValid, Is.True);
    }

    [Test]
    public void BuildSpecialPerson1Validator_ObjectValidatorRemoved ()
    {
      var person1 = new SpecialPerson1 { FirstName = null, LastName = "LastName" };
      var person2 = new SpecialPerson1 { FirstName = "FirstName", LastName = null };
      var person3 = new SpecialPerson1 { FirstName = "FirstName", LastName = "LastName" };

      var validator = ValidationBuilder.BuildValidator<SpecialPerson1>();

      var result1 = validator.Validate(person1);
      Assert.That(result1.Errors.Where(e => e.ValidatedProperties.Count == 0), Is.Empty);

      var result2 = validator.Validate(person2);
      Assert.That(result2.Errors.Where(e => e.ValidatedProperties.Count == 0), Is.Empty);

      var result3 = validator.Validate(person3);
      Assert.That(result3.Errors.Where(e => e.ValidatedProperties.Count == 0), Is.Empty);
    }
  }
}
