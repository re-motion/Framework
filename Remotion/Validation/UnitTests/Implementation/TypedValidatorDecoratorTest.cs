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
using Moq;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Validation.Implementation;
using Remotion.Validation.Results;
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.TestDomain;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class TypedValidatorDecoratorTest
  {
    private Mock<IValidationRule> _validationRuleStub1;
    private Mock<IValidationRule> _validationRuleStub2;
    private Validator _validator;
    private TypedValidatorDecorator<Customer> _validatorDecorator;
    private ValidationFailure _validationFailure;
    private Customer _validatedObject;

    [SetUp]
    public void SetUp ()
    {
      _validatedObject = new Customer();

      _validationRuleStub1 = new Mock<IValidationRule>();
      _validationRuleStub2 = new Mock<IValidationRule>();

      _validationFailure = ValidationFailure.CreateObjectValidationFailure(
          _validatedObject,
          "Error",
          "ValidationMessage");

      _validator = new Validator(new[] { _validationRuleStub1.Object, _validationRuleStub2.Object }, typeof(Customer));
      _validatorDecorator = new TypedValidatorDecorator<Customer>(_validator);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_validatorDecorator.Validator, Is.SameAs(_validator));
    }

    [Test]
    public void Initialization_InvalidType ()
    {
      Assert.That(
          () => new TypedValidatorDecorator<Address>(_validator),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The validated type 'Address' is not supported by the passed validator.", "validator"));
    }

    [Test]
    public void Validate ()
    {
      var customer = _validatedObject;

      _validationRuleStub1
          .Setup(stub => stub.Validate(It.IsNotNull<ValidationContext>()))
          .Returns(new[] { _validationFailure });
      _validationRuleStub2
          .Setup(stub => stub.Validate(It.IsNotNull<ValidationContext>()))
          .Returns(new ValidationFailure[0]);

      var result = _validatorDecorator.Validate(customer);

      Assert.That(result.Errors, Is.EquivalentTo(new[] { _validationFailure }));
    }

    [Test]
    public void Validate_InvalidInstance ()
    {
      Assert.That(
          () => ((IValidator)_validatorDecorator).Validate("Invalid"),
          Throws.InvalidOperationException.And.Message.EqualTo(
              "Cannot validate instances of type 'String'. This validator can only validate instances of type 'Customer'."));
    }

    [Test]
    public void CreateDescriptor ()
    {
      var result = _validatorDecorator.CreateDescriptor();

      Assert.That(result.ValidationRules, Is.EquivalentTo(new[] { _validationRuleStub1.Object, _validationRuleStub2.Object }));
    }

    [Test]
    public void CanValidateInstancesOfType_Customer_True ()
    {
      Assert.That(_validatorDecorator.CanValidateInstancesOfType(typeof(Customer)), Is.True);
    }

    [Test]
    public void CanValidateInstancesOfType_NoCustomer_False ()
    {
      Assert.That(_validatorDecorator.CanValidateInstancesOfType(typeof(Address)), Is.False);
    }
  }
}
