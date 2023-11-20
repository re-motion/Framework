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
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Remotion.Validation.Results;
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Rules
{
  [TestFixture]
  public class ObjectValidationRuleTest
  {
    [Test]
    public void Validate_ReturnsValidationFailures ()
    {
      var context = new ValidationContext(new object());

      var failure1 = ValidationFailure.CreateObjectValidationFailure(
          15,
          "15 is not a number",
          "Localized validation message: 15 is not a number.");

      var failure2 = ValidationFailure.CreateObjectValidationFailure(
          42,
          "42 is not a number",
          "Localized validation message: 42 is not a number.");

      var validator1Stub = new Mock<IObjectValidator>();
      validator1Stub
          .Setup(_ => _.Validate(It.IsAny<ObjectValidatorContext>()))
          .Returns(new[] { failure1 });

      var validator2Stub = new Mock<IObjectValidator>();
      validator2Stub
          .Setup(_ => _.Validate(It.IsAny<ObjectValidatorContext>()))
          .Returns(new[] { failure2 });

      var rule = new ObjectValidationRule<object>(null, new[] { validator1Stub.Object, validator2Stub.Object });

      var result = rule.Validate(context);
      Assert.That(result, Is.EquivalentTo(new[] { failure1, failure2 }));
    }

    [Test]
    public void Validate_UsesCorrectChildContextToBuildParentContext ()
    {
      var instance = new object();
      var validationContext = new ValidationContext(instance);

      var failure1 = ValidationFailure.CreateObjectValidationFailure(
          15,
          "15 is not a number",
          "Localized validation message: 15 is not a number.");

      var validator1Stub = new Mock<IObjectValidator>();
      validator1Stub
          .Setup(_ => _.Validate(It.Is<ObjectValidatorContext>(ctx => ctx.ParentContext == validationContext && ctx.Instance == instance)))
          .Returns(new[] { failure1 });

      var rule = new ObjectValidationRule<object>(null, new[] { validator1Stub.Object });

      var result = rule.Validate(validationContext);
      Assert.That(result, Is.EquivalentTo(new[] { failure1 }));
    }

    [Test]
    public void Validate_WithNoValidators_ReturnsEmptyResult ()
    {
      var context = new ValidationContext(new object());
      var rule = new ObjectValidationRule<object>(null, new List<IObjectValidator>());

      var result = rule.Validate(context);

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void Validate_WithNullAsInstanceToValidate_ReturnsEmptyResultAsValidatorsAreNotCalled ()
    {
      var context = new ValidationContext(null);

      var validatorStub = new Mock<IObjectValidator>();
      validatorStub
          .Setup(_ => _.Validate(It.IsAny<ObjectValidatorContext>()))
          .Returns(
              new[]
              {
                  ValidationFailure.CreateObjectValidationFailure(
                    15,
                    "15 is not a number",
                    "Localized validation message: 15 is not a number.")
              });

      var rule = new ObjectValidationRule<object>(null, new[] { validatorStub.Object });

      var result = rule.Validate(context);

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void Validate_WithUnfulfilledCondition_ReturnsEmptyResultAsValidatorsAreNotCalled ()
    {
      Func<object, bool> condition = _ => false;
      var context = new ValidationContext(new object());

      var validatorStub = new Mock<IObjectValidator>();
      validatorStub
          .Setup(_ => _.Validate(It.IsAny<ObjectValidatorContext>()))
          .Returns(
              new[]
              {
                  ValidationFailure.CreateObjectValidationFailure(
                    15,
                    "15 is not a number",
                    "Localized validation message: 15 is not a number.")
              });

      var rule = new ObjectValidationRule<object>(condition, new[] { validatorStub.Object });

      var result = rule.Validate(context);

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void IsActive_WithoutInstanceToValidate_ReturnsFalse ()
    {
      Func<object, bool> condition = _ => true;

      var context = new ValidationContext(null);
      var rule = new ObjectValidationRule<object>(condition, new List<IObjectValidator>());

      var result = rule.IsActive(context);

      Assert.That(result, Is.False);
    }

    [Test]
    public void IsActive_WithNoCondition_ReturnsTrue ()
    {
      var context = new ValidationContext(new object());
      var rule = new ObjectValidationRule<object>(null, new List<IObjectValidator>());

      var result = rule.IsActive(context);

      Assert.That(result, Is.True);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void IsActive_ReturnsValueFromCondition (bool expectedResult)
    {
      Func<object, bool> condtition = _ => expectedResult;

      var context = new ValidationContext(new object());
      var rule = new ObjectValidationRule<object>(condtition, new List<IObjectValidator>());

      var result = rule.IsActive(context);

      Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void ToString_ReturnsValidatorTypeAndFullNameOfValidatedType ()
    {
      var rule = new ObjectValidationRule<Person>(null, new List<IObjectValidator>());

      var result = rule.ToString();

      Assert.That(result, Is.EqualTo("ObjectValidationRule (Remotion.Validation.UnitTests.TestDomain.Person)"));
    }
  }
}
