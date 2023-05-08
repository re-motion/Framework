﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Results;

namespace Remotion.Validation.Validators
{
  public class DecimalValidator : IPropertyValidator
  {
    private static readonly decimal[] s_compareValuesWithoutScale
        = new decimal[c_systemDecimalMaxPrecision]
          {
              1m,
              10m,
              100m,
              1000m,
              10000m,
              100000m,
              1000000m,
              10000000m,
              100000000m,
              1000000000m,
              10000000000m,
              100000000000m,
              1000000000000m,
              10000000000000m,
              100000000000000m,
              1000000000000000m,
              10000000000000000m,
              100000000000000000m,
              1000000000000000000m,
              10000000000000000000m,
              100000000000000000000m,
              1000000000000000000000m,
              10000000000000000000000m,
              100000000000000000000000m,
              1000000000000000000000000m,
              10000000000000000000000000m,
              100000000000000000000000000m,
              1000000000000000000000000000m,
              10000000000000000000000000000m,
          };

    private const int c_systemDecimalMaxPrecision = 29;

    /// <summary> Gets the maximum number of digits allowed for the integer part of the value. </summary>
    public int MaxIntegerPlaces { get; }

    /// <summary> Gets the maximum number of digits allowed for the decimal part of the value. </summary>
    public int MaxDecimalPlaces { get; }

    /// <summary> Gets a flag that indicates if <see cref="MaxDecimalPlaces"/> should consider trailing zeros. </summary>
    /// <remarks> .NET <see cref="Decimal"/> supports trailing zeros when handling the scale of the value. </remarks>
    public bool IgnoreTrailingZeros { get; }

    public string ErrorMessage { get; }
    public ValidationMessage ValidationMessage { get; }

    public DecimalValidator (int maxIntegerPlaces, int maxDecimalPlaces, bool ignoreTrailingZeros, [NotNull] ValidationMessage validationMessage)
    {
      ArgumentUtility.CheckNotNull("validationMessage", validationMessage);

      if (maxIntegerPlaces < 1)
        throw new ArgumentOutOfRangeException("maxIntegerPlaces", maxIntegerPlaces, "Value must not be zero or negative.");

      if (maxDecimalPlaces < 0)
        throw new ArgumentOutOfRangeException("maxDecimalPlaces", maxDecimalPlaces, "Value must not be negative.");

      if ((maxIntegerPlaces + maxDecimalPlaces) > c_systemDecimalMaxPrecision)
      {
        throw new ArgumentException(
            message:
            $"The sum of {nameof(maxIntegerPlaces)} ({maxIntegerPlaces}) and {nameof(maxDecimalPlaces)} ({maxDecimalPlaces}) must not be greater than {c_systemDecimalMaxPrecision}.");
      }

      MaxIntegerPlaces = maxIntegerPlaces;
      MaxDecimalPlaces = maxDecimalPlaces;
      IgnoreTrailingZeros = ignoreTrailingZeros;
      ErrorMessage = ignoreTrailingZeros
          ? $"The value must not have more than {maxIntegerPlaces} integer digits and {maxDecimalPlaces} decimal places."
          : $"The value must not have more than {maxIntegerPlaces} integer digits and {maxDecimalPlaces} decimal places including trailing zeros.";
      ValidationMessage = validationMessage;
    }

    public IEnumerable<PropertyValidationFailure> Validate (PropertyValidatorContext context)
    {
      if (IsValid(context))
        return Enumerable.Empty<PropertyValidationFailure>();

      return EnumerableUtility.Singleton(CreateValidationError(context));
    }

    private bool IsValid (PropertyValidatorContext context)
    {
      if (!(context.PropertyValue is decimal propertyValue))
        return true;

      decimal absolutePropertyValue = Math.Abs(propertyValue);
      bool hasValidIntegerPlaces;
      if (MaxIntegerPlaces == c_systemDecimalMaxPrecision)
        hasValidIntegerPlaces = true;
      else
        hasValidIntegerPlaces = Decimal.Floor(absolutePropertyValue) < s_compareValuesWithoutScale[MaxIntegerPlaces];

      decimal propertyValueForScale;
      if (IgnoreTrailingZeros)
        propertyValueForScale = propertyValue / 1.0m;
      else
        propertyValueForScale = propertyValue;
      int[] bits = Decimal.GetBits(propertyValueForScale);
      var scale = (bits[3] & 0x00FF0000) >> 16;
      bool hasValidDecimalPlaces = scale <= MaxDecimalPlaces;

      return hasValidIntegerPlaces && hasValidDecimalPlaces;
    }

    private PropertyValidationFailure CreateValidationError (PropertyValidatorContext context)
    {
      string localizedValidationMessage = ValidationMessage.Format(
          CultureInfo.CurrentUICulture,
          (IFormatProvider)CultureInfo.CurrentCulture,
          MaxIntegerPlaces,
          MaxDecimalPlaces);

      return new PropertyValidationFailure(
          context.Instance,
          context.Property,
          context.PropertyValue,
          errorMessage: ErrorMessage,
          localizedValidationMessage: localizedValidationMessage);
    }
  }
}
