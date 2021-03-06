﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Reflection;
using Remotion.Validation.Implementation;
using Remotion.Validation.Results;
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class CompoundValidatorTest
  {
    private IValidator<Customer> _validatorStub1;
    private IValidator<Customer> _validatorStub2;
    private CompoundValidator _compoundValidator;
    private ValidationFailure _validationFailure1;
    private ValidationFailure _validationFailure2;
    private ValidationFailure _validationFailure3;
    private ValidationResult _validationResult1;
    private ValidationResult _validationResult2;
    private IValidationRule _validationRuleStub1;
    private IValidationRule _validationRuleStub2;
    private Customer _validatedObject;

    [SetUp]
    public void SetUp ()
    {
      _validatedObject = new Customer();
      var propertyStub1 = MockRepository.GenerateStub<IPropertyInformation>();
      propertyStub1.Stub (_ => _.Name).Return ("PropertyStub1");
      var propertyStub2 = MockRepository.GenerateStub<IPropertyInformation>();
      propertyStub2.Stub (_ => _.Name).Return ("PropertyStub3");
      var propertyStub3 = MockRepository.GenerateStub<IPropertyInformation>();
      propertyStub3.Stub (_ => _.Name).Return ("PropertyStub3");

      _validationRuleStub1 = MockRepository.GenerateStub<IValidationRule>();
      _validationRuleStub2 = MockRepository.GenerateStub<IValidationRule>();

      _validatorStub1 = MockRepository.GenerateStub<IValidator<Customer>>();
      _validatorStub2 = MockRepository.GenerateStub<IValidator<Customer>>();

      _compoundValidator = new CompoundValidator (new[] { _validatorStub1, _validatorStub2 }, typeof (Customer));

      _validationFailure1 = new ObjectValidationFailure (_validatedObject, "Error1", "ValidationMessage1");
      _validationFailure2 = new PropertyValidationFailure (_validatedObject, propertyStub2, "value2", "Error2", "ValidationMessage2");
      _validationFailure3 = new PropertyValidationFailure (_validatedObject, propertyStub3, null, "Error3", "ValidationMessage3");

      _validationResult1 = new ValidationResult (new[] { _validationFailure1, _validationFailure2 });
      _validationResult2 = new ValidationResult (new[] { _validationFailure3 });
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_compoundValidator.Validators, Is.EquivalentTo (new[] { _validatorStub1, _validatorStub2 }));
    }

    [Test]
    public void Validate ()
    {
      _validatorStub1.Stub (stub => stub.Validate (Arg<ValidationContext>.Is.NotNull)).Repeat.Once().Return (_validationResult1);
      _validatorStub2.Stub (stub => stub.Validate (Arg<ValidationContext>.Is.NotNull)).Repeat.Once().Return (_validationResult2);

      var result = _compoundValidator.Validate (_validatedObject);

      Assert.That (result.Errors, Is.EquivalentTo (new[] { _validationFailure1, _validationFailure2, _validationFailure3 }));
    }

    [Test]
    public void Validate_InvalidInstance ()
    {
      Assert.That (
          () => ((IValidator) _compoundValidator).Validate ("Invalid"),
          Throws.InvalidOperationException.And.Message.EqualTo (
              "Cannot validate instances of type 'String'. This validator can only validate instances of type 'Customer'."));
    }

    [Test]
    public void CreateDescriptor ()
    {
      var validator1 = new Validator (new[] { _validationRuleStub1 }, typeof (Customer));
      var validator2 = new Validator (new[] { _validationRuleStub2 }, typeof (Customer));
      var compositeValidator = new CompoundValidator (new[] { validator1, validator2 }, typeof (Customer));

      var result = compositeValidator.CreateDescriptor();

      Assert.That (result.ValidationRules, Is.EquivalentTo (new[] { _validationRuleStub1, _validationRuleStub2 }));
    }

    [Test]
    public void CanValidateInstancesOfType_Customer_True ()
    {
      _validatorStub1.Stub (stub => stub.CanValidateInstancesOfType (typeof (Customer))).Return (true);
      _validatorStub2.Stub (stub => stub.CanValidateInstancesOfType (typeof (Customer))).Return (true);

      Assert.That (_compoundValidator.CanValidateInstancesOfType (typeof (Customer)), Is.True);
    }

    [Test]
    public void CanValidateInstancesOfType_NoCustomer_False ()
    {
      Assert.That (_compoundValidator.CanValidateInstancesOfType (typeof (Address)), Is.False);
    }
  }
}