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
using System.Linq;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using Remotion.Utilities;
using Remotion.Validation.Utilities;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Default implementation of the <see cref="IValidator"/> interface. Use <see cref="Create{T}"/> to create an instance of <see cref="IValidator{T}"/>.
  /// </summary>
  public sealed class Validator : IValidator
  {
    private readonly Type _validatedType;
    private readonly IReadOnlyCollection<IValidationRule> _validationRules;

    public Validator (IEnumerable<IValidationRule> validationRules, Type validatedType)
    {
      ArgumentUtility.CheckNotNull ("validationRules", validationRules);
      ArgumentUtility.CheckNotNull ("validatedType", validatedType);

      _validatedType = validatedType;
      _validationRules = validationRules.ToList().AsReadOnly();
    }

    public IReadOnlyCollection<IValidationRule> ValidationRules
    {
      get { return _validationRules; }
    }

    public IValidator<T> Create<T> ()
    {
      return new TypedValidatorDecorator<T> (this);
    }

    public ValidationResult Validate (object instance)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);

      return Validate (new ValidationContext (instance, new PropertyChain(), new DefaultValidatorSelector()));
    }

    public ValidationResult Validate (ValidationContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var failures = _validationRules.SelectMany (r => r.Validate (context)).ToList();
      foreach (var failure in failures)
        failure.SetValidatedInstance (context.InstanceToValidate);

      return new ValidationResult (failures);
    }

    public IValidatorDescriptor CreateDescriptor ()
    {
      var typeToInstantiate = typeof (ValidatorDescriptor<>).MakeGenericType (_validatedType);
      return (IValidatorDescriptor) Activator.CreateInstance (typeToInstantiate, _validationRules);
    }

    public bool CanValidateInstancesOfType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return _validatedType.IsAssignableFrom (type);
    }

    ValidationResult IValidator.Validate (object instance)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);

      if (!CanValidateInstancesOfType (instance.GetType()))
      {
        throw new InvalidOperationException (
            string.Format (
                "Cannot validate instances of type '{0}'. This validator can only validate instances of type '{1}'.",
                instance.GetType().Name,
                _validatedType.Name));
      }

      return Validate (instance);
    }

    ValidationResult IValidator.Validate (ValidationContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      return Validate (context);
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public IEnumerator<IValidationRule> GetEnumerator ()
    {
      return _validationRules.GetEnumerator();
    }
  }
}