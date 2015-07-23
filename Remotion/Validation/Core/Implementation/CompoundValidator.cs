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
using System.Reflection;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using Remotion.Utilities;
using Remotion.Utilities.ReSharperAnnotations;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Aggregates a set of <see cref="IValidator"/> instances. When performing a validation all validators are executed.
  /// </summary>
  public sealed class CompoundValidator : IValidator
  {
    private static readonly MethodInfo s_CreateDescriptorMethod =
        typeof (CompoundValidator).GetMethod (
            "CreateDescriptor",
            BindingFlags.Instance | BindingFlags.NonPublic);

    private readonly IReadOnlyCollection<IValidator> _validators;
    private readonly Type _typeToValidate;
    private readonly MethodInfo _createDescriptorMethod;

    public CompoundValidator (IEnumerable<IValidator> validators, Type typeToValidate)
    {
      ArgumentUtility.CheckNotNull ("validators", validators);
      ArgumentUtility.CheckNotNull ("typeToValidate", typeToValidate);

      _validators = validators.ToList().AsReadOnly();
      _typeToValidate = typeToValidate;
      _createDescriptorMethod = s_CreateDescriptorMethod.MakeGenericMethod (_typeToValidate);
    }

    public IReadOnlyCollection<IValidator> Validators
    {
      get { return _validators; }
    }

    public ValidationResult Validate (object instance)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);

      return Validate (new ValidationContext (instance, new PropertyChain(), new DefaultValidatorSelector()));
    }

    public ValidationResult Validate (ValidationContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var failures = _validators.SelectMany (v => v.Validate (context).Errors);
      return new ValidationResult (failures);
    }

    public IValidatorDescriptor CreateDescriptor ()
    {
      return (IValidatorDescriptor) _createDescriptorMethod.Invoke (this, null);
    }

    [ReflectionAPI]
    private IValidatorDescriptor CreateDescriptor<T> ()
    {
      var validationRules = GetAllValidationRules();
      return new ValidatorDescriptor<T> (validationRules);
    }

    public bool CanValidateInstancesOfType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return _validators.All (v => v.CanValidateInstancesOfType (type));
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
                _typeToValidate.Name));
      }

      return Validate (instance);
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public IEnumerator<IValidationRule> GetEnumerator ()
    {
      return GetAllValidationRules().GetEnumerator();
    }

    private IEnumerable<IValidationRule> GetAllValidationRules ()
    {
      return _validators.SelectMany (valiator => valiator);
    }
  }
}