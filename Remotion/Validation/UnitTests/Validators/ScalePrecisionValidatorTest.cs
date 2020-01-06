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
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Validators
{
  [TestFixture]
  public class ScalePrecisionValidatorTest : ValidatorTestBase
  {
    [Test]
    public void Validate_WithNegativeScale_ThrowsArgumentOutOfRangeException ()
    {
      using (CultureScope.CreateInvariantCultureScope())
      {
        Assert.That (
            () => new ScalePrecisionValidator (-1, 2, new InvariantValidationMessage ("Fake Message")),
            Throws.InstanceOf<ArgumentOutOfRangeException>()
                .With.Message.EqualTo ($"Scale must not be negative.\r\nParameter name: scale\r\nActual value was -1."));
      }
    }

    [Test]
    public void Validate_WithNegativePrecision_ThrowsArgumentOutOfRangeException ()
    {
      using (CultureScope.CreateInvariantCultureScope())
      {
        Assert.That (
            () => new ScalePrecisionValidator (2, 0, new InvariantValidationMessage ("Fake Message")),
            Throws.InstanceOf<ArgumentOutOfRangeException>()
                .With.Message.EqualTo ($"Precision must not be zero or negative.\r\nParameter name: precision\r\nActual value was 0."));
      }
    }

    [Test]
    public void Validate_WithPrecisionLessThanScale_ThrowsArgumentOutOfRangeException ()
    {
      using (CultureScope.CreateInvariantCultureScope())
      {
        Assert.That (
            () => new ScalePrecisionValidator (3, 2, new InvariantValidationMessage ("Fake Message")),
            Throws.InstanceOf<ArgumentOutOfRangeException>()
                .With.Message.EqualTo ($"Scale must be greater than precision.\r\nParameter name: scale\r\nActual value was 3."));
      }
    }

    [Test]
    public void Validate_WithValidPropertyValueNull_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext (null);
      var validator = new ScalePrecisionValidator (2, 2, new InvariantValidationMessage ("Fake Message"));

      var validationFailures = validator.Validate (propertyValidatorContext);

      Assert.That (validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithValidPropertyValueAndPrecisionMatchingScale_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext (10m);
      var validator = new ScalePrecisionValidator (2, 2, new InvariantValidationMessage ("Fake Message"));

      var validationFailures = validator.Validate (propertyValidatorContext);

      Assert.That (validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithValidPropertyValue_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext (10.12m);
      var validator = new ScalePrecisionValidator (2, 4, new InvariantValidationMessage ("Fake Message"));

      var validationFailures = validator.Validate (propertyValidatorContext);

      Assert.That (validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithPropertyValueScaleGreaterThanValidatorValue_ReturnsSingleValidationFailure ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext (10.123m);
      var validator = new ScalePrecisionValidator (
          2,
          5,
          new InvariantValidationMessage ("Custom validation message: '{0}', '{1}'."));
          

      var validationFailures = validator.Validate (propertyValidatorContext).ToArray();

      Assert.That (validationFailures.Length, Is.EqualTo (1));
      Assert.That (
          validationFailures[0].ErrorMessage,
          Is.EqualTo ("The value must not have more than 5 digits in total, with allowance for 2 decimals."));
      Assert.That (validationFailures[0].LocalizedValidationMessage, Is.EqualTo ("Custom validation message: '5', '2'."));
    }

    [Test]
    public void Validate_WithPropertyValuePrecisionGreaterThanValidatorValue_ReturnsSingleValidationFailure ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext (10.123m);
      var validator = new ScalePrecisionValidator (
          3,
          4,
          new InvariantValidationMessage ("Custom validation message: '{0}', '{1}'."));

      var validationFailures = validator.Validate (propertyValidatorContext).ToArray();

      Assert.That (validationFailures.Length, Is.EqualTo (1));
      Assert.That (
          validationFailures[0].ErrorMessage,
          Is.EqualTo ("The value must not have more than 4 digits in total, with allowance for 3 decimals."));
      Assert.That (validationFailures[0].LocalizedValidationMessage, Is.EqualTo ("Custom validation message: '4', '3'."));
    }

    [Test]
    public void Validate_WithPropertyValuePrecisionGreaterThanValidatorValueAndNonDecimal_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext (10.123d);
      var validator = new ScalePrecisionValidator (3, 4, new InvariantValidationMessage ("Fake Message"));

      var validationFailures = validator.Validate (propertyValidatorContext);

      Assert.That (validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithPropertyValuePrecisionLessThanValidatorValue_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext (10.123m);
      var validator = new ScalePrecisionValidator (3, 6, new InvariantValidationMessage ("Fake Message"));

      var validationFailures = validator.Validate (propertyValidatorContext);

      Assert.That (validationFailures, Is.Empty);
    }
  }
}