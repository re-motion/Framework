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
  public class EqualValidator : IValueComparisonValidator
  {
    public object ComparisonValue { get; }
    public IEqualityComparer? Comparer { get; }
    public string ErrorMessage { get; }
    public ValidationMessage ValidationMessage { get; }

    public EqualValidator (
        [NotNull] object comparisonValue,
        [NotNull] ValidationMessage validationMessage,
        [CanBeNull] IEqualityComparer? comparer = null)
    {
      ArgumentUtility.CheckNotNull("comparisonValue", comparisonValue);
      ArgumentUtility.CheckNotNull("validationMessage", validationMessage);

      ComparisonValue = comparisonValue;
      Comparer = comparer;
      ErrorMessage = $"The value must be equal to '{comparisonValue}'.";
      ValidationMessage = validationMessage;

      //TODO RM-5906: Refactor remove duplication with NotEqualsValidator.
    }

    object IValueComparisonValidator.ValueToCompare => ComparisonValue;

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

      if (Comparer != null)
        return Comparer.Equals(ComparisonValue, propertyValue);

      if (ComparisonValue.GetType() != propertyValue.GetType())
        return true;

      return ComparisonValue.Equals(propertyValue);
    }

    private PropertyValidationFailure CreateValidationError (PropertyValidatorContext context)
    {
      string localizedValidationMessage = ValidationMessage.Format(
          CultureInfo.CurrentUICulture,
          (IFormatProvider) CultureInfo.CurrentCulture,
          ComparisonValue);

      return new PropertyValidationFailure(
          context.Instance,
          context.Property,
          context.PropertyValue,
          ErrorMessage,
          localizedValidationMessage);
    }
  }
}
