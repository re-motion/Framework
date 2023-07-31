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
using Remotion.FunctionalProgramming;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Results;

namespace Remotion.Validation.Validators
{
  /// <summary>
  ///   Checks that a <see cref="string"/> value is neither <see cref="string.Empty"/> nor contains only whitespace characters.
  /// </summary>
  /// <remarks>Note the definition of whitespace in <see cref="char.IsWhiteSpace(char)"/>, which includes tab, carriage return and line feed as well as non-breaking space,
  /// thin space and hair space, but does not include zero-width space.</remarks>
  /// <seealso cref="char.IsWhiteSpace(char)"/>
  public class NotEmptyOrWhitespaceValidator : IPropertyValidator
  {
    public string ErrorMessage { get; }
    public ValidationMessage ValidationMessage { get; }

    public NotEmptyOrWhitespaceValidator (ValidationMessage validationMessage)
    {
      ArgumentUtility.CheckNotNull("validationMessage", validationMessage);

      ErrorMessage = "The value must not be empty or contain only whitespace characters.";
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
      if (context.PropertyValue is string stringValue)
        return !string.IsNullOrWhiteSpace(stringValue);

      if (context.PropertyValue is string[] stringArray)
        return stringArray.Any(s => !string.IsNullOrWhiteSpace(s));

      return true;
    }

    private ValidationFailure CreateValidationError (PropertyValidatorContext context)
    {
      return ValidationFailure.CreatePropertyValidationFailure(
          context.Instance,
          context.Property,
          context.PropertyValue,
          errorMessage: ErrorMessage,
          localizedValidationMessage: ValidationMessage.Format(CultureInfo.CurrentUICulture, null, Array.Empty<object>()));
    }
  }
}
