using System;
using System.Linq;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Validators
{
  [TestFixture]
  public class DecimalValidatorTest : ValidatorTestBase
  {
    [Test]
    public void Validate_WithNegativeMaxDecimalPlaces_ThrowsArgumentOutOfRangeException ()
    {
      using (CultureScope.CreateInvariantCultureScope())
      {
        Assert.That(
            () => new DecimalValidator(
                maxIntegerPlaces: 1,
                maxDecimalPlaces: -1,
                ignoreTrailingZeros: false,
                validationMessage: new InvariantValidationMessage("Fake Message")),
            Throws.InstanceOf<ArgumentOutOfRangeException>()
                .With.ArgumentOutOfRangeExceptionMessageEqualTo("Value must not be negative.", "maxDecimalPlaces", -1));
      }
    }

    [Test]
    public void Validate_WithZeroMaxIntegerPlaces_ThrowsArgumentOutOfRangeException ()
    {
      using (CultureScope.CreateInvariantCultureScope())
      {
        Assert.That(
            () => new DecimalValidator(
                maxIntegerPlaces: 0,
                maxDecimalPlaces: 0,
                ignoreTrailingZeros: false,
                validationMessage: new InvariantValidationMessage("Fake Message")),
            Throws.InstanceOf<ArgumentOutOfRangeException>()
                .With.ArgumentOutOfRangeExceptionMessageEqualTo("Value must not be zero or negative.", "maxIntegerPlaces", 0));
      }
    }

    [Test]
    public void Validate_WithNegativeMaxIntegerPlaces_ThrowsArgumentOutOfRangeException ()
    {
      using (CultureScope.CreateInvariantCultureScope())
      {
        Assert.That(
            () => new DecimalValidator(
                maxIntegerPlaces: -1,
                maxDecimalPlaces: 0,
                ignoreTrailingZeros: false,
                validationMessage: new InvariantValidationMessage("Fake Message")),
            Throws.InstanceOf<ArgumentOutOfRangeException>()
                .With.ArgumentOutOfRangeExceptionMessageEqualTo("Value must not be zero or negative.", "maxIntegerPlaces", -1));
      }
    }

    [Test]
    public void Validate_WithSumOfMaxIntegerPlacesAndMaxDecimalPlacesExceedingDecimalPrecision_ThrowsArgumentException ()
    {
      using (CultureScope.CreateInvariantCultureScope())
      {
        Assert.That(
            () => new DecimalValidator(
                maxIntegerPlaces: 20,
                maxDecimalPlaces: 10,
                ignoreTrailingZeros: false,
                validationMessage: new InvariantValidationMessage("Fake Message")),
            Throws.InstanceOf<ArgumentException>()
                .With.Message.EqualTo("The sum of maxIntegerPlaces (20) and maxDecimalPlaces (10) must not be greater than 29."));
      }
    }

    [Test]
    public void Validate_WithValidPropertyValueNull_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(null);
      var validator = new DecimalValidator(
          maxIntegerPlaces:2,
          maxDecimalPlaces: 2,
          ignoreTrailingZeros: false,
          validationMessage: new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithValidPropertyValue_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(1234.56m);
      var validator = new DecimalValidator(
          maxIntegerPlaces: 4,
          maxDecimalPlaces: 2,
          ignoreTrailingZeros: false,
          validationMessage: new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithValidPropertyValueAndTrailingZerosIgnored_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(10.1230m);
      var validator = new DecimalValidator(
          maxIntegerPlaces: 12,
          maxDecimalPlaces: 3,
          ignoreTrailingZeros: true,
          validationMessage: new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithPropertyValueDecimalPlacesGreaterThanValidatorValue_ReturnsSingleValidationFailure ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(1234.567m);
      var validator = new DecimalValidator(
          maxIntegerPlaces: 4,
          maxDecimalPlaces: 2,
          ignoreTrailingZeros: true,
          validationMessage: new InvariantValidationMessage("Custom validation message: '{0}', '{1}'."));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      Assert.That(validationFailures.Length, Is.EqualTo(1));
      Assert.That(validationFailures[0].ValidatedObject, Is.EqualTo(propertyValidatorContext.Instance));
      Assert.That(validationFailures[0].ValidatedProperties.Select(vp => vp.Property), Is.EqualTo(new [] { propertyValidatorContext.Property }));
      Assert.That(validationFailures[0].ValidatedProperties.Select(vp => vp.ValidatedPropertyValue), Is.EqualTo(new [] { propertyValidatorContext.PropertyValue }));
      Assert.That(
          validationFailures[0].ErrorMessage,
          Is.EqualTo("The value must not have more than 4 integer digits and 2 decimal places."));
      Assert.That(validationFailures[0].LocalizedValidationMessage, Is.EqualTo("Custom validation message: '4', '2'."));
    }

    [Test]
    public void Validate_WithPropertyValueIntegerPlacesGreaterThanValidatorValue_ReturnsSingleValidationFailure ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(123.4567m);
      var validator = new DecimalValidator(
          maxIntegerPlaces: 2,
          maxDecimalPlaces: 4,
          ignoreTrailingZeros: true,
          validationMessage: new InvariantValidationMessage("Custom validation message: '{0}', '{1}'."));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      Assert.That(validationFailures.Length, Is.EqualTo(1));
      Assert.That(validationFailures[0].ValidatedObject, Is.EqualTo(propertyValidatorContext.Instance));
      Assert.That(validationFailures[0].ValidatedProperties.Select(vp => vp.Property), Is.EqualTo(new [] { propertyValidatorContext.Property }));
      Assert.That(validationFailures[0].ValidatedProperties.Select(vp => vp.ValidatedPropertyValue), Is.EqualTo(new [] { propertyValidatorContext.PropertyValue }));
      Assert.That(
          validationFailures[0].ErrorMessage,
          Is.EqualTo("The value must not have more than 2 integer digits and 4 decimal places."));
      Assert.That(validationFailures[0].LocalizedValidationMessage, Is.EqualTo("Custom validation message: '2', '4'."));
    }

    [Test]
    public void Validate_WithPropertyValueDecimalPlacesGreaterThanValidatorValueAndTrailingZerosRelevant_ReturnsSingleValidationFailure ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(123.10m);
      var validator = new DecimalValidator(
          maxIntegerPlaces: 3,
          maxDecimalPlaces: 1,
          ignoreTrailingZeros: false,
          validationMessage: new InvariantValidationMessage("Custom validation message: '{0}', '{1}'."));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      Assert.That(validationFailures.Length, Is.EqualTo(1));
      Assert.That(validationFailures[0].ValidatedObject, Is.EqualTo(propertyValidatorContext.Instance));
      Assert.That(validationFailures[0].ValidatedProperties.Select(vp => vp.Property), Is.EqualTo(new [] { propertyValidatorContext.Property }));
      Assert.That(validationFailures[0].ValidatedProperties.Select(vp => vp.ValidatedPropertyValue), Is.EqualTo(new [] { propertyValidatorContext.PropertyValue }));
      Assert.That(
          validationFailures[0].ErrorMessage,
          Is.EqualTo("The value must not have more than 3 integer digits and 1 decimal places including trailing zeros."));
      Assert.That(validationFailures[0].LocalizedValidationMessage, Is.EqualTo("Custom validation message: '3', '1'."));
    }

    [Test]
    public void Validate_WithPropertyValueIntegerPlacesLessThanValidatorValue_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(123.4d);
      var validator = new DecimalValidator(
          maxIntegerPlaces: 3,
          maxDecimalPlaces: 1,
          ignoreTrailingZeros: false,
          validationMessage: new InvariantValidationMessage("Custom validation message: '{0}', '{1}'."));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithPropertyValueDecimalPlacesLessThanValidatorValue_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(0.123m);
      var validator = new DecimalValidator(
          maxIntegerPlaces: 1,
          maxDecimalPlaces: 3,
          ignoreTrailingZeros: false,
          validationMessage: new InvariantValidationMessage("Custom validation message: '{0}', '{1}'."));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    [TestCaseSource(typeof(DecimalValidatorTest), nameof(TestCaseSource_Validate_WithPropertyValueIntegerPlacesValidAndDecimalPlacesZero))]
    public void Validate_WithPropertyValueIntegerPlacesValidAndDecimalPlacesZero (int integerPlaces, decimal value)
    {
      Assert.That(Math.Abs(value).ToString().Length, Is.EqualTo(integerPlaces), "Test specification mismatch");

      var propertyValidatorContext = CreatePropertyValidatorContext(value);
      var validator = new DecimalValidator(
          maxIntegerPlaces: integerPlaces,
          maxDecimalPlaces: 0,
          ignoreTrailingZeros: false,
          validationMessage: new InvariantValidationMessage("Custom validation message: '{0}', '{1}'."));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    private static TestCaseData[] TestCaseSource_Validate_WithPropertyValueIntegerPlacesValidAndDecimalPlacesZero ()
    {
      return new[]
             {
                 new TestCaseData(1,  0m),
                 new TestCaseData(1,  1m),
                 new TestCaseData(2,  10m),
                 new TestCaseData(3,  100m),
                 new TestCaseData(4,  1000m),
                 new TestCaseData(5,  10000m),
                 new TestCaseData(6,  100000m),
                 new TestCaseData(7,  1000000m),
                 new TestCaseData(8,  10000000m),
                 new TestCaseData(9,  100000000m),
                 new TestCaseData(10, 1000000000m),
                 new TestCaseData(11, 10000000000m),
                 new TestCaseData(12, 100000000000m),
                 new TestCaseData(13, 1000000000000m),
                 new TestCaseData(14, 10000000000000m),
                 new TestCaseData(15, 100000000000000m),
                 new TestCaseData(16, 1000000000000000m),
                 new TestCaseData(17, 10000000000000000m),
                 new TestCaseData(18, 100000000000000000m),
                 new TestCaseData(19, 1000000000000000000m),
                 new TestCaseData(20, 10000000000000000000m),
                 new TestCaseData(21, 100000000000000000000m),
                 new TestCaseData(22, 1000000000000000000000m),
                 new TestCaseData(23, 10000000000000000000000m),
                 new TestCaseData(24, 100000000000000000000000m),
                 new TestCaseData(25, 1000000000000000000000000m),
                 new TestCaseData(26, 10000000000000000000000000m),
                 new TestCaseData(27, 100000000000000000000000000m),
                 new TestCaseData(28, 1000000000000000000000000000m),
                 new TestCaseData(29, 10000000000000000000000000000m),
                 new TestCaseData(1,  9m),
                 new TestCaseData(2,  99m),
                 new TestCaseData(3,  999m),
                 new TestCaseData(4,  9999m),
                 new TestCaseData(5,  99999m),
                 new TestCaseData(6,  999999m),
                 new TestCaseData(7,  9999999m),
                 new TestCaseData(8,  99999999m),
                 new TestCaseData(9,  999999999m),
                 new TestCaseData(10, 9999999999m),
                 new TestCaseData(11, 99999999999m),
                 new TestCaseData(12, 999999999999m),
                 new TestCaseData(13, 9999999999999m),
                 new TestCaseData(14, 99999999999999m),
                 new TestCaseData(15, 999999999999999m),
                 new TestCaseData(16, 9999999999999999m),
                 new TestCaseData(17, 99999999999999999m),
                 new TestCaseData(18, 999999999999999999m),
                 new TestCaseData(19, 9999999999999999999m),
                 new TestCaseData(20, 99999999999999999999m),
                 new TestCaseData(21, 999999999999999999999m),
                 new TestCaseData(22, 9999999999999999999999m),
                 new TestCaseData(23, 99999999999999999999999m),
                 new TestCaseData(24, 999999999999999999999999m),
                 new TestCaseData(25, 9999999999999999999999999m),
                 new TestCaseData(26, 99999999999999999999999999m),
                 new TestCaseData(27, 999999999999999999999999999m),
                 new TestCaseData(28, 9999999999999999999999999999m),
                 new TestCaseData(1,  -1m),
                 new TestCaseData(2,  -10m),
                 new TestCaseData(3,  -100m),
                 new TestCaseData(4,  -1000m),
                 new TestCaseData(5,  -10000m),
                 new TestCaseData(6,  -100000m),
                 new TestCaseData(7,  -1000000m),
                 new TestCaseData(8,  -10000000m),
                 new TestCaseData(9,  -100000000m),
                 new TestCaseData(10, -1000000000m),
                 new TestCaseData(11, -10000000000m),
                 new TestCaseData(12, -100000000000m),
                 new TestCaseData(13, -1000000000000m),
                 new TestCaseData(14, -10000000000000m),
                 new TestCaseData(15, -100000000000000m),
                 new TestCaseData(16, -1000000000000000m),
                 new TestCaseData(17, -10000000000000000m),
                 new TestCaseData(18, -100000000000000000m),
                 new TestCaseData(19, -1000000000000000000m),
                 new TestCaseData(20, -10000000000000000000m),
                 new TestCaseData(21, -100000000000000000000m),
                 new TestCaseData(22, -1000000000000000000000m),
                 new TestCaseData(23, -10000000000000000000000m),
                 new TestCaseData(24, -100000000000000000000000m),
                 new TestCaseData(25, -1000000000000000000000000m),
                 new TestCaseData(26, -10000000000000000000000000m),
                 new TestCaseData(27, -100000000000000000000000000m),
                 new TestCaseData(28, -1000000000000000000000000000m),
                 new TestCaseData(29, -10000009000000000000000000000m),
                 new TestCaseData(1,  -9m),
                 new TestCaseData(2,  -99m),
                 new TestCaseData(3,  -999m),
                 new TestCaseData(4,  -9999m),
                 new TestCaseData(5,  -99999m),
                 new TestCaseData(6,  -999999m),
                 new TestCaseData(7,  -9999999m),
                 new TestCaseData(8,  -99999999m),
                 new TestCaseData(9,  -999999999m),
                 new TestCaseData(10, -9999999999m),
                 new TestCaseData(11, -99999999999m),
                 new TestCaseData(12, -999999999999m),
                 new TestCaseData(13, -9999999999999m),
                 new TestCaseData(14, -99999999999999m),
                 new TestCaseData(15, -999999999999999m),
                 new TestCaseData(16, -9999999999999999m),
                 new TestCaseData(17, -99999999999999999m),
                 new TestCaseData(18, -999999999999999999m),
                 new TestCaseData(19, -9999999999999999999m),
                 new TestCaseData(20, -99999999999999999999m),
                 new TestCaseData(21, -999999999999999999999m),
                 new TestCaseData(22, -9999999999999999999999m),
                 new TestCaseData(23, -99999999999999999999999m),
                 new TestCaseData(24, -999999999999999999999999m),
                 new TestCaseData(25, -9999999999999999999999999m),
                 new TestCaseData(26, -99999999999999999999999999m),
                 new TestCaseData(27, -999999999999999999999999999m),
                 new TestCaseData(28, -9999999999999999999999999999m),
             };
    }

    [Test]
    [TestCaseSource(typeof(DecimalValidatorTest), nameof(TestCaseSource_Validate_WithPropertyValueIntegerPlacesValidAndDecimalPlacesValid))]
    public void Validate_WithPropertyValueIntegerPlacesValidAndDecimalPlacesValid (int integerPlaces, int decimalPlaces, decimal value)
    {
      Assert.That(Math.Abs(value).ToString().Length, Is.EqualTo(integerPlaces + 1 + decimalPlaces), "Test specification mismatch");

      var propertyValidatorContext = CreatePropertyValidatorContext(value);
      var validator = new DecimalValidator(
          maxIntegerPlaces: integerPlaces,
          maxDecimalPlaces: decimalPlaces,
          ignoreTrailingZeros: false,
          validationMessage: new InvariantValidationMessage("Custom validation message: '{0}', '{1}'."));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    private static TestCaseData[] TestCaseSource_Validate_WithPropertyValueIntegerPlacesValidAndDecimalPlacesValid ()
    {
      return new[]
             {
                 new TestCaseData(1, 1, 0.9m),
                 new TestCaseData(1, 1, 9.0m),
                 new TestCaseData(1, 1, 9.9m),
                 new TestCaseData(4, 3, 9999.999m),
                 new TestCaseData(3, 2, -999.99m),
             };
    }

    [Test]
    [TestCaseSource(typeof(DecimalValidatorTest), nameof(TestCaseSource_Validate_WithPropertyValueIntegerPlacesInvalidAndDecimalPlacesValid))]
    public void Validate_WithPropertyValueIntegerPlacesInvalidAndDecimalPlacesValid (int integerPlaces, int decimalPlaces, decimal value)
    {
      Assert.That(Math.Abs(value).ToString().Length, Is.EqualTo(integerPlaces + 1 + decimalPlaces + 1), "Test specification mismatch");

      var propertyValidatorContext = CreatePropertyValidatorContext(value);
      var validator = new DecimalValidator(
          maxIntegerPlaces: integerPlaces,
          maxDecimalPlaces: decimalPlaces,
          ignoreTrailingZeros: false,
          validationMessage: new InvariantValidationMessage("Custom validation message: '{0}', '{1}'."));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      Assert.That(validationFailures.Length, Is.EqualTo(1));
      Assert.That(validationFailures[0].LocalizedValidationMessage, Is.EqualTo($"Custom validation message: '{integerPlaces}', '{decimalPlaces}'."));
    }

    private static TestCaseData[] TestCaseSource_Validate_WithPropertyValueIntegerPlacesInvalidAndDecimalPlacesValid ()
    {
      return new[]
             {
                 new TestCaseData(1, 1, 10.0m),
                 new TestCaseData(1, 1, 10.9m),
                 new TestCaseData(4, 3, 10000.999m),
                 new TestCaseData(3, 2, -1000.99m),
             };
    }

    [Test]
    [TestCaseSource(typeof(DecimalValidatorTest), nameof(TestCaseSource_Validate_WithPropertyValueDecimalPlacesValidAndIntegerPlacesZero))]
    public void Validate_WithPropertyValueDecimalPlacesValidAndIntegerPlacesZero (int decimalPlaces, decimal value)
    {
      Assert.That(Math.Abs(value).ToString().Length, Is.EqualTo(decimalPlaces + 2), "Test specification mismatch");

      var propertyValidatorContext = CreatePropertyValidatorContext(value);
      var validator = new DecimalValidator(
          maxIntegerPlaces: 1,
          maxDecimalPlaces: decimalPlaces,
          ignoreTrailingZeros: false,
          validationMessage: new InvariantValidationMessage("Custom validation message: '{0}', '{1}'."));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    private static TestCaseData[] TestCaseSource_Validate_WithPropertyValueDecimalPlacesValidAndIntegerPlacesZero ()
    {
      return new[]
             {
                 new TestCaseData(1,  0.0m),
                 new TestCaseData(1,  0.1m),
                 new TestCaseData(2,  0.01m),
                 new TestCaseData(3,  0.001m),
                 new TestCaseData(4,  0.0001m),
                 new TestCaseData(5,  0.00001m),
                 new TestCaseData(6,  0.000001m),
                 new TestCaseData(7,  0.0000001m),
                 new TestCaseData(8,  0.00000001m),
                 new TestCaseData(9,  0.000000001m),
                 new TestCaseData(10, 0.0000000001m),
                 new TestCaseData(11, 0.00000000001m),
                 new TestCaseData(12, 0.000000000001m),
                 new TestCaseData(13, 0.0000000000001m),
                 new TestCaseData(14, 0.00000000000001m),
                 new TestCaseData(15, 0.000000000000001m),
                 new TestCaseData(16, 0.0000000000000001m),
                 new TestCaseData(17, 0.00000000000000001m),
                 new TestCaseData(18, 0.000000000000000001m),
                 new TestCaseData(19, 0.0000000000000000001m),
                 new TestCaseData(20, 0.00000000000000000001m),
                 new TestCaseData(21, 0.000000000000000000001m),
                 new TestCaseData(22, 0.0000000000000000000001m),
                 new TestCaseData(23, 0.00000000000000000000001m),
                 new TestCaseData(24, 0.000000000000000000000001m),
                 new TestCaseData(25, 0.0000000000000000000000001m),
                 new TestCaseData(26, 0.00000000000000000000000001m),
                 new TestCaseData(27, 0.000000000000000000000000001m),
                 new TestCaseData(28, 0.0000000000000000000000000001m),
                 new TestCaseData(1,  -0.1m),
                 new TestCaseData(2,  -0.01m),
                 new TestCaseData(3,  -0.001m),
                 new TestCaseData(4,  -0.0001m),
                 new TestCaseData(5,  -0.00001m),
                 new TestCaseData(6,  -0.000001m),
                 new TestCaseData(7,  -0.0000001m),
                 new TestCaseData(8,  -0.00000001m),
                 new TestCaseData(9,  -0.000000001m),
                 new TestCaseData(10, -0.0000000001m),
                 new TestCaseData(11, -0.00000000001m),
                 new TestCaseData(12, -0.000000000001m),
                 new TestCaseData(13, -0.0000000000001m),
                 new TestCaseData(14, -0.00000000000001m),
                 new TestCaseData(15, -0.000000000000001m),
                 new TestCaseData(16, -0.0000000000000001m),
                 new TestCaseData(17, -0.00000000000000001m),
                 new TestCaseData(18, -0.000000000000000001m),
                 new TestCaseData(19, -0.0000000000000000001m),
                 new TestCaseData(20, -0.00000000000000000001m),
                 new TestCaseData(21, -0.000000000000000000001m),
                 new TestCaseData(22, -0.0000000000000000000001m),
                 new TestCaseData(23, -0.00000000000000000000001m),
                 new TestCaseData(24, -0.000000000000000000000001m),
                 new TestCaseData(25, -0.0000000000000000000000001m),
                 new TestCaseData(26, -0.00000000000000000000000001m),
                 new TestCaseData(27, -0.000000000000000000000000001m),
                 new TestCaseData(28, -0.0000000000000000000000000001m),
             };
    }

    [Test]
    [TestCaseSource(typeof(DecimalValidatorTest), nameof(TestCaseSource_Validate_WithPropertyValueDecimalPlacesValidAndIntegerPlacesZeroAndIgnoresTrailingZeroes))]
    public void Validate_WithPropertyValueDecimalPlacesValidAndIntegerPlacesZeroAndIgnoresTrailingZeroes (int decimalPlaces, decimal value)
    {
      Assert.That(Math.Abs(value).ToString().Length, Is.EqualTo(decimalPlaces + 3), "Test specification mismatch");

      var propertyValidatorContext = CreatePropertyValidatorContext(value);
      var validator = new DecimalValidator(
          maxIntegerPlaces: 1,
          maxDecimalPlaces: decimalPlaces,
          ignoreTrailingZeros: true,
          validationMessage: new InvariantValidationMessage("Custom validation message: '{0}', '{1}'."));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    private static TestCaseData[] TestCaseSource_Validate_WithPropertyValueDecimalPlacesValidAndIntegerPlacesZeroAndIgnoresTrailingZeroes ()
    {
      return new[]
             {
                 new TestCaseData(1,  0.00m),
                 new TestCaseData(1,  0.10m),
                 new TestCaseData(2,  0.010m),
                 new TestCaseData(3,  0.0010m),
                 new TestCaseData(4,  0.00010m),
                 new TestCaseData(5,  0.000010m),
                 new TestCaseData(6,  0.0000010m),
                 new TestCaseData(7,  0.00000010m),
                 new TestCaseData(8,  0.000000010m),
                 new TestCaseData(9,  0.0000000010m),
                 new TestCaseData(10, 0.00000000010m),
                 new TestCaseData(11, 0.000000000010m),
                 new TestCaseData(12, 0.0000000000010m),
                 new TestCaseData(13, 0.00000000000010m),
                 new TestCaseData(14, 0.000000000000010m),
                 new TestCaseData(15, 0.0000000000000010m),
                 new TestCaseData(16, 0.00000000000000010m),
                 new TestCaseData(17, 0.000000000000000010m),
                 new TestCaseData(18, 0.0000000000000000010m),
                 new TestCaseData(19, 0.00000000000000000010m),
                 new TestCaseData(20, 0.000000000000000000010m),
                 new TestCaseData(21, 0.0000000000000000000010m),
                 new TestCaseData(22, 0.00000000000000000000010m),
                 new TestCaseData(23, 0.000000000000000000000010m),
                 new TestCaseData(24, 0.0000000000000000000000010m),
                 new TestCaseData(25, 0.00000000000000000000000010m),
                 new TestCaseData(26, 0.000000000000000000000000010m),
                 new TestCaseData(27, 0.0000000000000000000000000010m),
                 new TestCaseData(1,  -0.10m),
                 new TestCaseData(2,  -0.010m),
                 new TestCaseData(3,  -0.0010m),
                 new TestCaseData(4,  -0.00010m),
                 new TestCaseData(5,  -0.000010m),
                 new TestCaseData(6,  -0.0000010m),
                 new TestCaseData(7,  -0.00000010m),
                 new TestCaseData(8,  -0.000000010m),
                 new TestCaseData(9,  -0.0000000010m),
                 new TestCaseData(10, -0.00000000010m),
                 new TestCaseData(11, -0.000000000010m),
                 new TestCaseData(12, -0.0000000000010m),
                 new TestCaseData(13, -0.00000000000010m),
                 new TestCaseData(14, -0.000000000000010m),
                 new TestCaseData(15, -0.0000000000000010m),
                 new TestCaseData(16, -0.00000000000000010m),
                 new TestCaseData(17, -0.000000000000000010m),
                 new TestCaseData(18, -0.0000000000000000010m),
                 new TestCaseData(19, -0.00000000000000000010m),
                 new TestCaseData(20, -0.000000000000000000010m),
                 new TestCaseData(21, -0.0000000000000000000010m),
                 new TestCaseData(22, -0.00000000000000000000010m),
                 new TestCaseData(23, -0.000000000000000000000010m),
                 new TestCaseData(24, -0.0000000000000000000000010m),
                 new TestCaseData(25, -0.00000000000000000000000010m),
                 new TestCaseData(26, -0.000000000000000000000000010m),
                 new TestCaseData(27, -0.0000000000000000000000000010m),
             };
    }

    [Test]
    [TestCaseSource(typeof(DecimalValidatorTest), nameof(TestCaseSource_Validate_WithPropertyValueMaxIntegerPlacesNotValidAndScaleZero))]
    public void Validate_WithPropertyValueMaxIntegerPlacesNotValidAndScaleZero (int integerPlaces, decimal value)
    {
      Assert.That(Math.Abs(value).ToString().Length, Is.EqualTo(integerPlaces + 1), "Test specification mismatch");

      var propertyValidatorContext = CreatePropertyValidatorContext(value);
      var validator = new DecimalValidator(
          maxIntegerPlaces: integerPlaces,
          maxDecimalPlaces: 0,
          ignoreTrailingZeros: false,
          validationMessage: new InvariantValidationMessage("Custom validation message: '{0}', '{1}'."));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      Assert.That(validationFailures.Length, Is.EqualTo(1));
      Assert.That(validationFailures[0].LocalizedValidationMessage, Is.EqualTo($"Custom validation message: '{integerPlaces}', '0'."));
    }

    private static TestCaseData[] TestCaseSource_Validate_WithPropertyValueMaxIntegerPlacesNotValidAndScaleZero ()
    {
      return new[]
             {
                 new TestCaseData(1,  10m),
                 new TestCaseData(2,  100m),
                 new TestCaseData(3,  1000m),
                 new TestCaseData(4,  10000m),
                 new TestCaseData(5,  100000m),
                 new TestCaseData(6,  1000000m),
                 new TestCaseData(7,  10000000m),
                 new TestCaseData(8,  100000000m),
                 new TestCaseData(9,  1000000000m),
                 new TestCaseData(10, 10000000000m),
                 new TestCaseData(11, 100000000000m),
                 new TestCaseData(12, 1000000000000m),
                 new TestCaseData(13, 10000000000000m),
                 new TestCaseData(14, 100000000000000m),
                 new TestCaseData(15, 1000000000000000m),
                 new TestCaseData(16, 10000000000000000m),
                 new TestCaseData(17, 100000000000000000m),
                 new TestCaseData(18, 1000000000000000000m),
                 new TestCaseData(19, 10000000000000000000m),
                 new TestCaseData(20, 100000000000000000000m),
                 new TestCaseData(21, 1000000000000000000000m),
                 new TestCaseData(22, 10000000000000000000000m),
                 new TestCaseData(23, 100000000000000000000000m),
                 new TestCaseData(24, 1000000000000000000000000m),
                 new TestCaseData(25, 10000000000000000000000000m),
                 new TestCaseData(26, 100000000000000000000000000m),
                 new TestCaseData(27, 1000000000000000000000000000m),
                 new TestCaseData(28, 10000000000000000000000000000m),
                 new TestCaseData(1,  -10m),
                 new TestCaseData(2,  -100m),
                 new TestCaseData(3,  -1000m),
                 new TestCaseData(4,  -10000m),
                 new TestCaseData(5,  -100000m),
                 new TestCaseData(6,  -1000000m),
                 new TestCaseData(7,  -10000000m),
                 new TestCaseData(8,  -100000000m),
                 new TestCaseData(9,  -1000000000m),
                 new TestCaseData(10, -10000000000m),
                 new TestCaseData(11, -100000000000m),
                 new TestCaseData(12, -1000000000000m),
                 new TestCaseData(13, -10000000000000m),
                 new TestCaseData(14, -100000000000000m),
                 new TestCaseData(15, -1000000000000000m),
                 new TestCaseData(16, -10000000000000000m),
                 new TestCaseData(17, -100000000000000000m),
                 new TestCaseData(18, -1000000000000000000m),
                 new TestCaseData(19, -10000000000000000000m),
                 new TestCaseData(20, -100000000000000000000m),
                 new TestCaseData(21, -1000000000000000000000m),
                 new TestCaseData(22, -10000000000000000000000m),
                 new TestCaseData(23, -100000000000000000000000m),
                 new TestCaseData(24, -1000000000000000000000000m),
                 new TestCaseData(25, -10000000000000000000000000m),
                 new TestCaseData(26, -100000000000000000000000000m),
                 new TestCaseData(27, -1000000000000000000000000000m),
                 new TestCaseData(28, -10000000000000000000000000000m),
             };
    }

  }
}
