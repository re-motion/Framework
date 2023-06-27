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
  public abstract class AbstractValueComparisonValidator : IValueComparisonValidator
  {
    public IComparable ComparisonValue { get; }
    public Comparison Comparison { get; }
    public IComparer? Comparer { get; }
    public string ErrorMessage { get; }
    public ValidationMessage ValidationMessage { get; }

    protected AbstractValueComparisonValidator (
        [NotNull] IComparable comparisonValue,
        [CanBeNull] IComparer? comparer,
        Comparison comparison,
        [NotNull] string errorMessage,
        [NotNull] ValidationMessage validationMessage)
    {
      ArgumentUtility.CheckNotNull("comparisonValue", comparisonValue);
      ArgumentUtility.CheckNotNull("errorMessage", errorMessage);
      ArgumentUtility.CheckNotNull("validationMessage", validationMessage);

      Comparer = comparer;
      ComparisonValue = comparisonValue;
      Comparison = comparison;
      ErrorMessage = errorMessage;
      ValidationMessage = validationMessage;
    }

    object IValueComparisonValidator.ValueToCompare => ComparisonValue;

    public IEnumerable<PropertyValidationFailure> Validate (PropertyValidatorContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      if (IsValid(context))
        return Enumerable.Empty<PropertyValidationFailure>();

      return EnumerableUtility.Singleton(CreateValidationError(context));
    }

    private bool IsValid (PropertyValidatorContext context)
    {
      var propertyValue = context.PropertyValue;

      if (propertyValue == null)
        return true;

      if (Comparer != null)
        return IsComparisonResultValid(Comparer.Compare(propertyValue, ComparisonValue));

      if (propertyValue.GetType() != ComparisonValue.GetType())
        return true;

      return IsComparisonResultValid(((IComparable)propertyValue).CompareTo(ComparisonValue));
    }

    private bool IsComparisonResultValid (int comparisonResult)
    {
      switch (Comparison)
      {
        case Comparison.GreaterThanOrEqual:
          return comparisonResult >= 0;
        case Comparison.GreaterThan:
          return comparisonResult > 0;
        case Comparison.LessThan:
          return comparisonResult < 0;
        case Comparison.LessThanOrEqual:
          return comparisonResult <= 0;
        default:
          throw new InvalidOperationException($"Unknown comparison type '{Comparison}'.");
      }
    }

    private PropertyValidationFailure CreateValidationError (PropertyValidatorContext context)
    {
      string localizedValidationMessage = ValidationMessage.Format(
          CultureInfo.CurrentUICulture,
          (IFormatProvider)CultureInfo.CurrentCulture,
          ComparisonValue);

      return new PropertyValidationFailure(
          context.Instance,
          context.Property,
          context.PropertyValue,
          errorMessage: ErrorMessage,
          localizedValidationMessage: localizedValidationMessage);
    }
  }
}
