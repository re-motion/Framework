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
using System.Collections;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting.NUnit;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Validators
{
  [TestFixture]
  public class ExclusiveRangeValidatorTest : ValidatorTestBase
  {
    [Test]
    public void Validate_WithPropertyValueExclusivelyBetweenBoundaries_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(3);
      var validator = new ExclusiveRangeValidator(2, 4, new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithPropertyValueNull_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(null);
      var validator = new ExclusiveRangeValidator(2, 4, new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithPropertyValueDifferentTypeThanFrom_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(1m);
      var validator = new ExclusiveRangeValidator(2f, 4f, new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Ctor_WithFromDifferentTypeThanTo_ThrowsArgumentException ()
    {
      using (CultureScope.CreateInvariantCultureScope())
      {
        Assert.That(
            () => new ExclusiveRangeValidator(2m, 4f, new InvariantValidationMessage("Fake Message")),
            Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("'from' must have the same type as 'to'.", "to"));
      }
    }

    [Test]
    public void Ctor_WithFromGreaterThanTo_ThrowsArgumentOutOfRangeException ()
    {
      using (CultureScope.CreateInvariantCultureScope())
      {
        Assert.That(
            () => new ExclusiveRangeValidator(2, 1, new InvariantValidationMessage("Fake Message")),
            Throws.InstanceOf<ArgumentOutOfRangeException>()
                .With.ArgumentExceptionMessageEqualTo("'to' should be larger than 'from'.", "to"));
      }
    }

    [Test]
    public void Validate_WithPropertyValueEqualsUpperBoundary_ReturnsSingleValidationFailure ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(3);
      var validator = new ExclusiveRangeValidator(2, 3, new InvariantValidationMessage("Custom validation message: '{0}', '{1}'."));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      Assert.That(validationFailures.Length, Is.EqualTo(1));
      //TODO RM-5906: Assert ValidatedObject, ValidatedProperty, ValidatedValue
      Assert.That(validationFailures[0].ErrorMessage, Is.EqualTo("The value must be between 2 and 3 (exclusive)."));
      Assert.That(validationFailures[0].LocalizedValidationMessage, Is.EqualTo("Custom validation message: '2', '3'."));
    }

    [Test]
    public void Validate_WithPropertyValueEqualsLowerBoundary_ReturnsSingleValidationFailure ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(2);
      var validator = new ExclusiveRangeValidator(2, 3, new InvariantValidationMessage("Custom validation message: '{0}', '{1}'."));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      Assert.That(validationFailures.Length, Is.EqualTo(1));
      //TODO RM-5906: Assert ValidatedObject, ValidatedProperty, ValidatedValue
      Assert.That(validationFailures[0].ErrorMessage, Is.EqualTo("The value must be between 2 and 3 (exclusive)."));
      Assert.That(validationFailures[0].LocalizedValidationMessage, Is.EqualTo("Custom validation message: '2', '3'."));
    }

    [Test]
    public void Validate_WithIComparable_CallsCompareTo ()
    {
      var propertyValueMock = new Mock<IComparable>();
      var fromStub = new Mock<IComparable>();
      var toStub = new Mock<IComparable>();
      propertyValueMock.Setup(_ => _.CompareTo(fromStub.Object)).Returns(1).Verifiable();
      propertyValueMock.Setup(_ => _.CompareTo(toStub.Object)).Returns(-1).Verifiable();
      var propertyValidatorContext = CreatePropertyValidatorContext(propertyValueMock.Object);
      var validator = new ExclusiveRangeValidator(fromStub.Object, toStub.Object, new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      propertyValueMock.Verify();
      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithCustomComparer_UsesComparer ()
    {
      var comparerMock = new Mock<IComparer>();
      comparerMock.Setup(_ => _.Compare(10, 1)).Returns(1).Verifiable();
      comparerMock.Setup(_ => _.Compare(10, 3)).Returns(-1).Verifiable();
      var propertyValidatorContext = CreatePropertyValidatorContext(10);
      var validator = new ExclusiveRangeValidator(1, 3, new InvariantValidationMessage("Fake Message"), comparerMock.Object);

      var validationFailures = validator.Validate(propertyValidatorContext);

      comparerMock.Verify();
      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithPropertyValueDifferentTypeThanComparisonValue_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(new object());
      var validator = new ExclusiveRangeValidator(1, 3, new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }
  }
}
