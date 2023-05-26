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
using System.Linq;
using JetBrains.Annotations;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Results;
using Remotion.Validation.Validators;

namespace Remotion.Validation.Rules
{
  public class ObjectValidationRule<TValidatedType> : IObjectValidationRule
  {
    [CanBeNull]
    public Func<TValidatedType, bool>? Condition { get; }

    [NotNull]
    public IReadOnlyCollection<IObjectValidator> Validators { get; }

    public ObjectValidationRule (
        [CanBeNull] Func<TValidatedType, bool>? condition,
        [NotNull] IReadOnlyCollection<IObjectValidator> validators)
    {
      ArgumentUtility.CheckNotNull("validators", validators);

      Condition = condition;
      Validators = validators;
    }

    public IEnumerable<ValidationFailure> Validate (ValidationContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      var instanceToValidate = (TValidatedType?)context.InstanceToValidate;
      if (instanceToValidate == null)
        return Enumerable.Empty<ValidationFailure>();

      if (Condition != null && !Condition(instanceToValidate))
        return Enumerable.Empty<ValidationFailure>();

      var objectValidatorContext = new ObjectValidatorContext(context, instanceToValidate);

      return Validators.SelectMany(validator => validator.Validate(objectValidatorContext));
    }

    public bool IsActive (ValidationContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      if (Condition == null)
        return true;

      var instanceToValidate = (TValidatedType?)context.InstanceToValidate;
      if (instanceToValidate == null)
        return false;

      return Condition(instanceToValidate);
    }

    public override string ToString ()
    {
      return $"ObjectValidationRule ({typeof(TValidatedType).GetFullNameSafe()})";
    }
  }
}
