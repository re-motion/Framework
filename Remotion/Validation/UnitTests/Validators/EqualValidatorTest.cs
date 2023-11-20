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
  public class EqualValidatorTest : ValidatorTestBase
  {
    [Test]
    public void Validate_WithValueTypeAndValueEqualsComparisonValue_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(1);
      var validator = new EqualValidator(1, new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithValueTypeAndValueNotEqualsComparisonValue_ReturnsSingleValidationFailure ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(2);
      var validator = new EqualValidator(1, validationMessage: new InvariantValidationMessage("Custom validation message: '{0}'."));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      Assert.That(validationFailures.Length, Is.EqualTo(1));
      Assert.That(validationFailures[0].ValidatedObject, Is.EqualTo(propertyValidatorContext.Instance));
      Assert.That(validationFailures[0].ValidatedProperties.Select(vp => vp.Property), Is.EqualTo(new [] { propertyValidatorContext.Property }));
      Assert.That(validationFailures[0].ValidatedProperties.Select(vp => vp.ValidatedPropertyValue), Is.EqualTo(new [] { propertyValidatorContext.PropertyValue }));
      Assert.That(validationFailures[0].ErrorMessage, Is.EqualTo("The value must be equal to '1'."));
      Assert.That(validationFailures[0].LocalizedValidationMessage, Is.EqualTo("Custom validation message: '1'."));
    }

    [Test]
    public void Validate_WithReferenceTypeAndReferenceEqualsComparisonValue_ReturnsNoValidationFailures ()
    {
      var instanceToValidate = new object();
      var propertyValidatorContext = CreatePropertyValidatorContext(instanceToValidate);
      var validator = new EqualValidator(instanceToValidate, new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Ctor_WithComparisonValueNull_ThrowsArgumentNullException ()
    {
      using (CultureScope.CreateInvariantCultureScope())
      {
        Assert.That(
            () => new EqualValidator(null, new InvariantValidationMessage("Fake Message")),
            Throws.InstanceOf<ArgumentNullException>()
                .With.ArgumentExceptionMessageEqualTo($"Value cannot be null.", "comparisonValue"));
      }
    }

    [Test]
    public void Validate_WithPropertyValueNull_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(null);
      var validator = new EqualValidator("some string", new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithPropertyValueDifferentTypeThanComparisonValue_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(1);
      var validator = new EqualValidator("some string", new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithReferenceTypeAndReferenceNotEqualsComparisonValue_ReturnsSingleValidationFailure ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(new object());
      var validator = new EqualValidator(new object(), new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      Assert.That(validationFailures.Length, Is.EqualTo(1));
      Assert.That(validationFailures[0].ErrorMessage, Is.EqualTo("The value must be equal to 'System.Object'."));
    }

    [Test]
    public void Validate_WithCustomComparer_UsesComparer ()
    {
      var equalityComparerMock = new Mock<IEqualityComparer>();
      equalityComparerMock
          .Setup(_ => _.Equals("comparisonValue", "propertyValue"))
          .Returns(true)
          .Verifiable();
      var propertyValidatorContext = CreatePropertyValidatorContext("propertyValue");
      var validator = new EqualValidator("comparisonValue", new InvariantValidationMessage("Fake Message"), equalityComparerMock.Object);

      var validationFailures = validator.Validate(propertyValidatorContext);

      equalityComparerMock.Verify();
      Assert.That(validationFailures, Is.Empty);
    }
  }
}
