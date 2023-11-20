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
using Remotion.Reflection;
using Remotion.Validation.Results;
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Rules
{
  [TestFixture]
  public class PropertyValidationRuleTest
  {
    [Test]
    public void Validate_WithNullAsInstanceToValidate_SkipsValidation ()
    {
      var validatorMock = new Mock<IPropertyValidator>();
      validatorMock
          .Setup(_ => _.Validate(It.IsAny<PropertyValidatorContext>()))
          .Throws(new AssertionException("Should not be called."));

      var propertyInformationMock = new Mock<IPropertyInformation>();

      var rule = new PropertyValidationRule<object, int>(propertyInformationMock.Object, obj => null, null, new[] { validatorMock.Object });

      var context = new ValidationContext(null);
      var result = rule.Validate(context);

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void Validate_WithConditionIsFalse_SkipsValidation ()
    {
      var validatorMock = new Mock<IPropertyValidator>();

      validatorMock
          .Setup(_ => _.Validate(It.IsAny<PropertyValidatorContext>()))
          .Throws(new AssertionException("Should not be called."));

      var propertyInformationMock = new Mock<IPropertyInformation>();

      var rule = new PropertyValidationRule<object, int>(propertyInformationMock.Object, obj => null, _ => false, new[] { validatorMock.Object });

      var context = new ValidationContext(new Person());
      var result = rule.Validate(context);

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void Validate_WithConditionIsTrue_ReturnsValidationFailures ()
    {
      var propertyInformationMock = new Mock<IPropertyInformation>();

      var propertyFailure = ValidationFailure.CreatePropertyValidationFailure(
          new Person(),
          propertyInformationMock.Object,
          "propval",
          "err",
          "validmessage");

      var validatorMock = new Mock<IPropertyValidator>();

      PropertyValidatorContext actualPropertyValidatorContext = null;
      validatorMock
          .Setup(_ => _.Validate(It.IsAny<PropertyValidatorContext>()))
          .Callback((PropertyValidatorContext ctx) => { actualPropertyValidatorContext = ctx; })
          .Returns(new[] { propertyFailure });


      object actualInstanceToValidateFromPropertyFunc = null;
      Func<object, object> propertyFunc = obj =>
      {
        actualInstanceToValidateFromPropertyFunc = obj;
        return "test";
      };

      object actualInstanceToValidateFromConditionFunc = null;
      Func<object, bool> conditionFunc = obj =>
      {
        actualInstanceToValidateFromConditionFunc = obj;
        return true;
      };

      var rule = new PropertyValidationRule<object, int>(propertyInformationMock.Object, propertyFunc, conditionFunc, new[] { validatorMock.Object });

      var instanceToValidate = new Person();
      var result = rule.Validate(new ValidationContext(instanceToValidate ));
      Assert.That(result, Is.EqualTo(new[] { propertyFailure }));
      Assert.That(actualPropertyValidatorContext.Instance, Is.SameAs(instanceToValidate));
      Assert.That(actualInstanceToValidateFromConditionFunc, Is.SameAs(instanceToValidate));
      Assert.That(actualInstanceToValidateFromPropertyFunc, Is.SameAs(instanceToValidate));
      Assert.That(actualPropertyValidatorContext.PropertyValue, Is.EqualTo("test"));
    }

    [Test]
    public void Validate_WithoutCondition_ReturnsValidationResults ()
    {
      var propertyInformationMock = new Mock<IPropertyInformation>();

      var propertyFailure = ValidationFailure.CreatePropertyValidationFailure(
          new Person(),
          propertyInformationMock.Object,
          "propval",
          "err",
          "validmessage");

      var validatorMock = new Mock<IPropertyValidator>();

      PropertyValidatorContext actualPropertyValidatorContext = null;
      validatorMock
          .Setup(_ => _.Validate(It.IsAny<PropertyValidatorContext>()))
          .Callback((PropertyValidatorContext ctx) => { actualPropertyValidatorContext = ctx; })
          .Returns(new[] { propertyFailure });

      object actualInstanceToValidateFromPropertyFunc = null;
      Func<object, object> propertyFunc = obj =>
      {
        actualInstanceToValidateFromPropertyFunc = obj;
        return "test";
      };

      var rule = new PropertyValidationRule<object, int>(propertyInformationMock.Object, propertyFunc, null, new[] { validatorMock.Object });

      var instanceToValidate = new Person();
      var result = rule.Validate(new ValidationContext(instanceToValidate ));
      Assert.That(result, Is.EqualTo(new[] { propertyFailure }));
      Assert.That(actualPropertyValidatorContext.Instance, Is.SameAs(instanceToValidate));
      Assert.That(actualInstanceToValidateFromPropertyFunc, Is.SameAs(instanceToValidate));
      Assert.That(actualPropertyValidatorContext.PropertyValue, Is.EqualTo("test"));
    }

    [Test]
    public void IsActive_WithoutCondition_WithoutInstanceToValidate_ReturnsTrue ()
    {
      var propertyInformationMock = new Mock<IPropertyInformation>();

      var validatorMock = new Mock<IPropertyValidator>();

      var rule = new PropertyValidationRule<object, int>(propertyInformationMock.Object, _ => 42, null, new[] { validatorMock.Object });

      var context = new ValidationContext(null);
      var result = rule.IsActive(context);

      Assert.That(result, Is.True);
    }

    [Test]
    public void IsActive_WithCondition_WithoutInstanceToValidate_ReturnsFalse ()
    {
      var propertyInformationMock = new Mock<IPropertyInformation>();

      var rule = new PropertyValidationRule<object, int>(propertyInformationMock.Object, _ => 42, _ => false, new IPropertyValidator[0]);

      var context = new ValidationContext(null);
      var result = rule.IsActive(context);

      Assert.That(result, Is.False);
    }


    [Test]
    public void IsActive_ReturnsTrueFromCondition ()
    {
      var propertyInformationMock = new Mock<IPropertyInformation>();

      var rule = new PropertyValidationRule<object, int>(propertyInformationMock.Object, _ => _, _ => true, new IPropertyValidator[0]);

      var context = new ValidationContext(new Person());
      var result = rule.IsActive(context);

      Assert.That(result, Is.True);
    }

    [Test]
    public void IsActive_ReturnsFalseFromCondition ()
    {
      var propertyInformationMock = new Mock<IPropertyInformation>();

      var rule = new PropertyValidationRule<object, int>(propertyInformationMock.Object, _ => _, _ => false, new IPropertyValidator[0]);

      var context = new ValidationContext(new Person());
      var result = rule.IsActive(context);

      Assert.That(result, Is.False);
    }

    [Test]
    public void ToString_Overridden ()
    {
      var declaringTypeMock = new Mock<ITypeInformation>();
      declaringTypeMock
          .Setup(d => d.FullName)
          .Returns("Remotion.Validation.IntegrationTests.TestDomain.ComponentA.Address");

      var propertyInformationMock = new Mock<IPropertyInformation>();
      propertyInformationMock
          .Setup(s => s.Name)
          .Returns("PostalCode");

      propertyInformationMock
          .Setup(p => p.DeclaringType)
          .Returns(declaringTypeMock.Object);

      PropertyValidationRule<Customer, string> validationRule =
          new PropertyValidationRule<Customer, string>(propertyInformationMock.Object, _ => null, null, new IPropertyValidator[0]);
      Assert.That(
          validationRule.ToString(),
          Is.EqualTo("PropertyValidationRule (Remotion.Validation.IntegrationTests.TestDomain.ComponentA.Address#PostalCode)"));
    }
  }
}
