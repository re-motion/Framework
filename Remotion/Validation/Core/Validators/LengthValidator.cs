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
  public class LengthValidator : ILengthValidator
  {
    public int Min { get; }

    public int? Max { get; }
    public string ErrorMessage { get; }
    public ValidationMessage ValidationMessage { get; }

    public LengthValidator (int min, int max, [NotNull] ValidationMessage validationMessage)
        : this(min, max, $"The value must have between {min} and {max} characters.", validationMessage)
    {
    }

    protected LengthValidator (int min, int? max, [NotNull] string errorMessage, [NotNull] ValidationMessage validationMessage)
    {
      ArgumentUtility.CheckNotNullOrEmpty("errorMessage", errorMessage);
      ArgumentUtility.CheckNotNull("validationMessage", validationMessage);
      if (min < 0)
        throw new ArgumentOutOfRangeException("min", "Value cannot be less than zero.");
      if (max != null && max < min)
        throw new ArgumentOutOfRangeException("max", "Max should be larger than min.");

      Max = max;
      Min = min;
      ErrorMessage = errorMessage;
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
      var propertyValue = context.PropertyValue;
      if (propertyValue == null)
        return true;

      if (!(propertyValue is string stringValue))
        return true;

      return stringValue.Length >= Min && (Max == null || !(stringValue.Length > Max));
    }

    private PropertyValidationFailure CreateValidationError (PropertyValidatorContext context)
    {
      string localizedValidationMessage = ValidationMessage.Format(
          CultureInfo.CurrentUICulture,
          (IFormatProvider) CultureInfo.CurrentCulture,
          Min,
          Max);

      return new PropertyValidationFailure(
          context.Instance,
          context.Property,
          context.PropertyValue,
          errorMessage: ErrorMessage,
          localizedValidationMessage: localizedValidationMessage);
    }
  }
}