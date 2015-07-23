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
using FluentValidation;
using FluentValidation.Results;
using Remotion.Utilities;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Implements the <see cref="IValidator{T}"/> interface as a typed wrapper of the <see cref="IValidator"/> interface.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class TypedValidatorDecorator<T> : IValidator<T>
  {
    private readonly IValidator _validator;

    public TypedValidatorDecorator (IValidator validator)
    {
      ArgumentUtility.CheckNotNull ("validator", validator);
      if (!validator.CanValidateInstancesOfType (typeof (T)))
      {
        throw new ArgumentException (
            string.Format ("The validated type '{0}' is not supported by the passed validator.", typeof (T).Name),
            "validator");
      }

      _validator = validator;
    }

    public IValidator Validator
    {
      get { return _validator; }
    }

    public ValidationResult Validate (T instance)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);

      return _validator.Validate (instance);
    }

    public ValidationResult Validate (ValidationContext<T> context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      return _validator.Validate (context);
    }

    public IValidatorDescriptor CreateDescriptor ()
    {
      return _validator.CreateDescriptor();
    }

    public bool CanValidateInstancesOfType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return _validator.CanValidateInstancesOfType (type);
    }

    ValidationResult IValidator.Validate (object instance)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);

      return _validator.Validate (instance);
    }

    ValidationResult IValidator.Validate (ValidationContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      return _validator.Validate (context);
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public IEnumerator<IValidationRule> GetEnumerator ()
    {
      return _validator.GetEnumerator();
    }

    CascadeMode IValidator<T>.CascadeMode
    {
      get
      {
        throw new NotSupportedException (string.Format ("CascadeMode is not supported for a '{0}'", typeof (TypedValidatorDecorator<>).FullName));
      }
      set
      {
        throw new NotSupportedException (string.Format ("CascadeMode is not supported for a '{0}'", typeof (TypedValidatorDecorator<>).FullName));
      }
    }
  }
}