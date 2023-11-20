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
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Results;

namespace Remotion.Validation.Validators
{
  public class InclusiveRangeValidator : IRangeValidator
  {
    public IComparable From { get; }
    public IComparable To { get; }

    [CanBeNull]
    public IComparer? Comparer { get; }
    public string ErrorMessage { get; }
    public ValidationMessage ValidationMessage { get; }

    public InclusiveRangeValidator (
        [NotNull] IComparable from,
        [NotNull] IComparable to,
        [NotNull] ValidationMessage validationMessage,
        [CanBeNull] IComparer? comparer = null)
    {
      ArgumentUtility.CheckNotNull("from", from);
      ArgumentUtility.CheckNotNull("to", to);
      ArgumentUtility.CheckNotNull("validationMessage", validationMessage);

      if (from.GetType() != to.GetType())
        throw new ArgumentException("'from' must have the same type as 'to'.", "to");

      if (to.CompareTo(from) < 0)
        throw new ArgumentOutOfRangeException("to", "'to' should be larger than 'from'.");

      To = to;
      From = from;
      Comparer = comparer;
      ErrorMessage = $"The value must be between '{from}' and '{to}'.";
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

      if (Comparer != null)
        return Comparer.Compare(propertyValue, From) >= 0 && Comparer.Compare(propertyValue, To) <= 0;

      if (propertyValue.GetType() != From.GetType())
        return true;

      return ((IComparable)propertyValue).CompareTo(From) >= 0 && ((IComparable)propertyValue).CompareTo(To) <= 0;
    }

    private ValidationFailure CreateValidationError (PropertyValidatorContext context)
    {
      string localizedValidationMessage = ValidationMessage.Format(
          CultureInfo.CurrentUICulture,
          (IFormatProvider)CultureInfo.CurrentCulture,
          From,
          To);

      return ValidationFailure.CreatePropertyValidationFailure(
          context.Instance,
          context.Property,
          context.PropertyValue,
          errorMessage: ErrorMessage,
          localizedValidationMessage: localizedValidationMessage);
    }
  }
}
