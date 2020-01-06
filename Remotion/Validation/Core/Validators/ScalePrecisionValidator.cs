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
  /// <summary>
  /// Allows a decimal to be validated for scale and precision.
  /// Scale would be the number of digits to the right of the decimal point.
  /// Precision would be the number of digits.
  /// 
  /// It can be configured to use the effective scale and precision
  /// (i.e. ignore trailing zeros) if required.
  /// 
  /// 123.4500 has an scale of 4 and a precision of 7, but an effective scale
  /// and precision of 2 and 5 respectively.
  /// </summary>
  public class ScalePrecisionValidator : IPropertyValidator
  {
    public int Scale { get; }

    public int Precision { get; }

    public string ErrorMessage { get; }
    public ValidationMessage ValidationMessage { get; }

    public ScalePrecisionValidator (int scale, int precision, [NotNull] ValidationMessage validationMessage)
    {
      ArgumentUtility.CheckNotNull ("validationMessage", validationMessage);

      // TODO RM-5960: Replace with Decimal-Validator that will validate a maximum number of integer digits and decimal digits

      if (scale < 0)
        throw new ArgumentOutOfRangeException ("scale", scale, $"Scale must not be negative.");

      if (precision < 1)
        throw new ArgumentOutOfRangeException ("precision", precision, $"Precision must not be zero or negative.");

      if (precision < scale)
        throw new ArgumentOutOfRangeException ("scale", scale, $"Scale must be greater than precision.");

      Scale = scale;
      Precision = precision;
      ErrorMessage = $"The value must not have more than {precision} digits in total, with allowance for {scale} decimals.";
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

      var scale = mantissa.Length;
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