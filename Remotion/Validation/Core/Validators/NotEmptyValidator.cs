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
  public class NotEmptyValidator : INotEmptyValidator
  {
    public string ErrorMessage { get; }
    public ValidationMessage ValidationMessage { get; }

    public NotEmptyValidator ([NotNull] ValidationMessage validationMessage)
    {
      ArgumentUtility.CheckNotNull("validationMessage", validationMessage);

      ErrorMessage = "The value must not be empty.";
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

      return !IsEmptyString(propertyValue) && !IsEmptyCollection(propertyValue);
    }

    private bool IsEmptyCollection (object propertyValue)
    {
      if (propertyValue is ICollection collectionValue)
        return !collectionValue.Cast<object>().Any();

      return false;
    }

    private bool IsEmptyString (object value)
    {
      if (value is string stringValue)
        return stringValue.Length == 0;

      return false;
    }

    private PropertyValidationFailure CreateValidationError (PropertyValidatorContext context)
    {
      return new PropertyValidationFailure(
          context.Instance,
          context.Property,
          context.PropertyValue,
          errorMessage: ErrorMessage,
          localizedValidationMessage: ValidationMessage.Format(CultureInfo.CurrentUICulture, null, Array.Empty<object>()));
    }
  }
}
