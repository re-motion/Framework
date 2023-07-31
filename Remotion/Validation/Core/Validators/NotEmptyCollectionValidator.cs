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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using Remotion.FunctionalProgramming;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Results;

namespace Remotion.Validation.Validators
{
  /// <summary>
  /// Checks that a collection contains at least one element.
  /// </summary>
  /// <remarks>
  /// The following collection interfaces and their implementations are supported:
  /// <list type="bullet">
  ///   <item><description><see cref="ICollection"/></description></item>
  ///   <item><description><see cref="ICollection{T}"/></description></item>
  ///   <item><description><see cref="IReadOnlyCollection{T}"/></description></item>
  /// </list>
  /// </remarks>
  public class NotEmptyCollectionValidator : IRequiredValidator
  {
    public string ErrorMessage { get; }
    public ValidationMessage ValidationMessage { get; }

    public NotEmptyCollectionValidator ([NotNull] ValidationMessage validationMessage)
    {
      ArgumentUtility.CheckNotNull("validationMessage", validationMessage);

      ErrorMessage = "The value must not be empty.";
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

      if (propertyValue == null)
        return true;

      if (propertyValue is ICollection collection)
        return collection.Count > 0;

      if (propertyValue is IReadOnlyCollection<object> readOnlyCollection)
        return readOnlyCollection.Count > 0;

      if (propertyValue.GetType().CanAscribeTo(typeof(ICollection<>)))
        return ((IEnumerable<object>)propertyValue).Any();

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
