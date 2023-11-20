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
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Validators
{
  [TestFixture]
  public class LessThanOrEqualValidatorTest : ValidatorTestBase
  {
    [Test]
    public void Validate_WithPropertyValueEqualsComparisonValue_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(3);
      var validator = new LessThanOrEqualValidator(3, new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithPropertyValueLessThanComparisonValue_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(2);
      var validator = new LessThanOrEqualValidator(3, new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithPropertyValueGreaterThanComparisonValue_ReturnsSingleValidationFailure ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(4);
      var validator = new LessThanOrEqualValidator(3, new InvariantValidationMessage("Custom validation message: '{0}'."));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      Assert.That(validationFailures.Length, Is.EqualTo(1));
      Assert.That(validationFailures[0].ValidatedObject, Is.EqualTo(propertyValidatorContext.Instance));
      Assert.That(validationFailures[0].ValidatedProperties.Select(vp => vp.Property), Is.EqualTo(new [] { propertyValidatorContext.Property }));
      Assert.That(validationFailures[0].ValidatedProperties.Select(vp => vp.ValidatedPropertyValue), Is.EqualTo(new [] { propertyValidatorContext.PropertyValue }));
      Assert.That(validationFailures[0].ErrorMessage, Is.EqualTo("The value must be less than or equal to '3'."));
      Assert.That(validationFailures[0].LocalizedValidationMessage, Is.EqualTo("Custom validation message: '3'."));
    }

    [Test]
    public void Validate_WithIComparable_CallsCompareTo ()
    {
      var comparisonValueMock = new Mock<IComparable>(MockBehavior.Strict);
      var propertyValueMock = new Mock<IComparable>(MockBehavior.Strict);
      propertyValueMock
          .Setup(_ => _.CompareTo(comparisonValueMock.Object))
          .Returns(0)
          .Verifiable();

      var propertyValidatorContext = CreatePropertyValidatorContext(propertyValueMock.Object);
      var validator = new LessThanOrEqualValidator(comparisonValueMock.Object, new InvariantValidationMessage("Custom validation message: '{0}'."));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      propertyValueMock.Verify();
      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithPropertyValueDifferentTypeThanComparisonValue_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(new object());
      var validator = new LessThanOrEqualValidator(3, new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithPropertyValueNull_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(null);
      var validator = new LessThanOrEqualValidator(3, new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithCustomComparer_UsesComparer ()
    {
      var comparerMock = new Mock<IComparer>();
      comparerMock
          .Setup(_ => _.Compare("propertyValue", "comparisonValue"))
          .Returns(0)
          .Verifiable();
      var propertyValidatorContext = CreatePropertyValidatorContext("propertyValue");
      var validator = new LessThanOrEqualValidator("comparisonValue", new InvariantValidationMessage("Fake Message"), comparerMock.Object);

      var validationFailures = validator.Validate(propertyValidatorContext);

      comparerMock.Verify();
      Assert.That(validationFailures, Is.Empty);
    }
  }
}
