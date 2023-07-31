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
using JetBrains.Annotations;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Validation.Results
{
  public class ValidationFailure
  {
    public static ValidationFailure CreatePropertyValidationFailure (
        [NotNull] object validatedObject,
        [NotNull] IPropertyInformation validatedProperty,
        [CanBeNull] object? validatedPropertyValue,
        [NotNull] string errorMessage,
        [NotNull] string localizedValidationMessage)
    {
      return new ValidationFailure(
          validatedObject,
          new[] { new ValidatedProperty(validatedObject, validatedProperty, validatedPropertyValue) },
          errorMessage,
          localizedValidationMessage);
    }

    public static ValidationFailure CreateObjectValidationFailure (
        [NotNull] object validatedObject,
        [NotNull] IReadOnlyList<ValidatedProperty> validatedProperties,
        [NotNull] string errorMessage,
        [NotNull] string localizedValidationMessage)
    {
      return new ValidationFailure(validatedObject, validatedProperties, errorMessage, localizedValidationMessage);
    }

    public static ValidationFailure CreateObjectValidationFailure (
        [NotNull] object validatedObject,
        [NotNull] string errorMessage,
        [NotNull] string localizedValidationMessage)
    {
      return new ValidationFailure(validatedObject, Array.Empty<ValidatedProperty>(), errorMessage, localizedValidationMessage);
    }

    [NotNull]
    public object ValidatedObject { get; }

    [NotNull]
    public IReadOnlyList<ValidatedProperty> ValidatedProperties { get; }

    /// <summary>The technical representation of this validation error. Intended for logging.</summary>
    [NotNull]
    public string ErrorMessage { get; }

    /// <summary>The localized representation of this validation error. Intended for display in the user interface.</summary>
    [NotNull]
    public string LocalizedValidationMessage { get; }

    protected ValidationFailure (
        [NotNull] object validatedObject,
        [NotNull] IReadOnlyList<ValidatedProperty> validatedProperties,
        [NotNull] string errorMessage,
        [NotNull] string localizedValidationMessage)
    {
      ArgumentUtility.CheckNotNull("validatedObject", validatedObject);
      ArgumentUtility.CheckNotNull("validatedProperties", validatedProperties);
      ArgumentUtility.CheckNotNullOrEmpty("errorMessage", errorMessage);
      ArgumentUtility.CheckNotNullOrEmpty("localizedValidationMessage", localizedValidationMessage);

      ValidatedObject = validatedObject;
      ValidatedProperties = validatedProperties;
      ErrorMessage = errorMessage;
      LocalizedValidationMessage = localizedValidationMessage;
    }
  }
}
