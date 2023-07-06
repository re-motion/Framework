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
using System.Collections;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Validators
{
  [TestFixture]
  public class NotEqualValidatorTest : ValidatorTestBase
  {
    [Test]
    public void Validate_WithValueTypeAndValueTypeNotEqualsComparisonValue_NoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(2);
      var validator = new NotEqualValidator(1, new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithValueTypeAndValueTypeEqualsComparisonValue_ReturnsSingleValidationFailure ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(1);
      var validator = new NotEqualValidator(1, new InvariantValidationMessage("Custom validation message: '{0}'."));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      Assert.That(validationFailures.Length, Is.EqualTo(1));
      Assert.That(validationFailures[0].ValidatedObject, Is.EqualTo(propertyValidatorContext.Instance));
      Assert.That(validationFailures[0].ValidatedProperties.Select(vp => vp.Property), Is.EqualTo(new [] { propertyValidatorContext.Property }));
      Assert.That(validationFailures[0].ValidatedProperties.Select(vp => vp.ValidatedPropertyValue), Is.EqualTo(new [] { propertyValidatorContext.PropertyValue }));
      Assert.That(validationFailures[0].ErrorMessage, Is.EqualTo("The value must not be equal to '1'."));
      Assert.That(validationFailures[0].LocalizedValidationMessage, Is.EqualTo("Custom validation message: '1'."));
    }

    [Test]
    public void Validate_WithValueTypeAndValueTypeDifferentTypeThanComparisonValue_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(1);
      var validator = new NotEqualValidator(1f, new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithPropertyValueNull_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(null);
      var validator = new NotEqualValidator(1f, new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Ctor_WithComparisonValueNull_ThrowsArgumentNullException ()
    {
      using (CultureScope.CreateInvariantCultureScope())
      {
        Assert.That(
            () => new NotEqualValidator(null, new InvariantValidationMessage("Fake Message")),
            Throws.InstanceOf<ArgumentNullException>()
                .With.ArgumentExceptionMessageEqualTo($"Value cannot be null.", "comparisonValue"));
      }
    }

    [Test]
    public void Validate_WithReferenceTypeAndReferenceTypeNotEqualsComparisonReference_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(new object());
      var validator = new NotEqualValidator(new object(), new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithReferenceTypeAndReferenceTypeEqualsComparisonReference_ReturnsSingleValidationFailure ()
    {
      var instanceToValidate = new object();
      var propertyValidatorContext = CreatePropertyValidatorContext(instanceToValidate);
      var validator = new NotEqualValidator(instanceToValidate, new InvariantValidationMessage("Custom validation message: '{0}'."));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      Assert.That(validationFailures.Length, Is.EqualTo(1));
      Assert.That(validationFailures[0].ValidatedObject, Is.EqualTo(propertyValidatorContext.Instance));
      Assert.That(validationFailures[0].ValidatedProperties.Select(vp => vp.Property), Is.EqualTo(new [] { propertyValidatorContext.Property }));
      Assert.That(validationFailures[0].ValidatedProperties.Select(vp => vp.ValidatedPropertyValue), Is.EqualTo(new [] { propertyValidatorContext.PropertyValue }));
      Assert.That(validationFailures[0].ErrorMessage, Is.EqualTo("The value must not be equal to 'System.Object'."));
      Assert.That(validationFailures[0].LocalizedValidationMessage, Is.EqualTo("Custom validation message: 'System.Object'."));
    }

    [Test]
    public void Validate_WithCustomComparer_UsesComparer ()
    {
      var equalityComparerMock = new Mock<IEqualityComparer>();
      equalityComparerMock
          .Setup(_ => _.Equals("comparison value", "property value"))
          .Returns(true)
          .Verifiable();
      var propertyValidatorContext = CreatePropertyValidatorContext("property value");
      var validator = new NotEqualValidator("comparison value", new InvariantValidationMessage("Fake Message"), equalityComparerMock.Object);

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      equalityComparerMock.Verify();
      Assert.That(validationFailures.Length, Is.EqualTo(1));
      Assert.That(validationFailures[0].ErrorMessage, Is.EqualTo("The value must not be equal to 'comparison value'."));
    }
  }
}
