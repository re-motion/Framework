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
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Results;

namespace Remotion.Validation.Validators
{
  public class ScalePrecisionValidator : IPropertyValidator
  {
    /// <summary> Gets the number of digits ro the right of the decimal point of the decimal value. </summary>
    public int Scale { get; }

    /// <summary> Gets the total number of digits allowed for the decimal value. </summary>
    public int Precision { get; }

    /// <summary> Gets a flag that indicates if <see cref="Scale"/> and <see cref="Precision"/> should consider trailing zeros to the of the decimal point. </summary>
    /// <remarks> .NET <see cref="Decimal"/> supports trailing zeros when handling the scale of the value. </remarks>
    public bool IgnoreTrailingZeros { get; }

    public string ErrorMessage { get; }
    public ValidationMessage ValidationMessage { get; }

    public ScalePrecisionValidator (int scale, int precision, bool ignoreTrailingZeros, [NotNull] ValidationMessage validationMessage)
    {
      ArgumentUtility.CheckNotNull ("validationMessage", validationMessage);

      // TODO RM-5906: Replace with Decimal-Validator that will validate a maximum number of integer digits and decimal digits

      if (scale < 0)
        throw new ArgumentOutOfRangeException ("scale", scale, $"Scale must not be negative.");

      if (precision < 1)
        throw new ArgumentOutOfRangeException ("precision", precision, $"Precision must not be zero or negative.");

      if (precision < scale)
        throw new ArgumentOutOfRangeException ("precision", precision, $"Precision must not be less than scale.");

      Scale = scale;
      Precision = precision;
      IgnoreTrailingZeros = ignoreTrailingZeros;
      ErrorMessage = ignoreTrailingZeros
          ? $"The value must not have more than {precision} digits in total, with allowance for {scale} decimals."
          : $"The value must not have more than {precision} digits in total, with allowance for {scale} decimals including trailing zeros.";
      ValidationMessage = validationMessage;
    }

    public IEnumerable<PropertyValidationFailure> Validate (PropertyValidatorContext context)
    {
      if (IsValid (context))
        return Enumerable.Empty<PropertyValidationFailure>();

      return EnumerableUtility.Singleton (CreateValidationError (context));
    }

    private bool IsValid (PropertyValidatorContext context)
    {
      if (!(context.PropertyValue is decimal propertyValue))
        return true;

      var decimalAsString = propertyValue.ToString (CultureInfo.InvariantCulture);
      var splitDecimalString = decimalAsString.Split ('.');

      var characteristic = splitDecimalString[0];
      var mantissa = splitDecimalString.Length == 1 ? "" : splitDecimalString[1];

      var scale = IgnoreTrailingZeros ? mantissa.Trim ('0').Length : mantissa.Length;
      var precision = characteristic.Length + scale;

      return scale <= Scale && precision <= Precision;
    }

    private PropertyValidationFailure CreateValidationError (PropertyValidatorContext context)
    {
      string localizedValidationMessage = ValidationMessage.Format (
          CultureInfo.CurrentUICulture,
          (IFormatProvider) CultureInfo.CurrentCulture,
          Precision,
          Scale);

      return new PropertyValidationFailure (
          context.Instance,
          context.Property,
          context.PropertyValue,
          errorMessage: ErrorMessage,
          localizedValidationMessage: localizedValidationMessage);
    }
  }
}