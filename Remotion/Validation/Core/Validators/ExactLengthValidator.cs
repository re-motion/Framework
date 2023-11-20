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
  public class ExactLengthValidator : IMinimumLengthValidator, IMaximumLengthValidator
  {
    int IMinimumLengthValidator.Min => Length;

    int IMaximumLengthValidator.Max => Length;

    public int Length { get; }

    public string ErrorMessage { get; }

    public ValidationMessage ValidationMessage { get; }

    public ExactLengthValidator (int length, [NotNull] ValidationMessage validationMessage)
    {
      ArgumentUtility.CheckNotNull(nameof(validationMessage), validationMessage);

      if (length <= 0)
        throw new ArgumentOutOfRangeException("length", "Value must be be greater than zero.");

      Length = length;
      ErrorMessage = $"The value must have exactly {length} characters.";
      ValidationMessage = validationMessage;
    }

    public IEnumerable<ValidationFailure> Validate (PropertyValidatorContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      if (IsValid(context))
        return Enumerable.Empty<ValidationFailure>();

      return EnumerableUtility.Singleton(CreateValidationError(context));
    }

    private bool IsValid (PropertyValidatorContext context)
    {
      var propertyValue = context.PropertyValue;

      if (propertyValue is not string stringValue)
        return true;

      return stringValue.Length == Length;
    }

    private ValidationFailure CreateValidationError (PropertyValidatorContext context)
    {
      var localizedValidationMessage = ValidationMessage.Format(
          CultureInfo.CurrentUICulture,
          (IFormatProvider)CultureInfo.CurrentCulture,
          Length);

      return ValidationFailure.CreatePropertyValidationFailure(
          context.Instance,
          context.Property,
          context.PropertyValue,
          errorMessage: ErrorMessage,
          localizedValidationMessage: localizedValidationMessage);
    }
  }
}
