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
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation.Validators;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Implements <see cref="IValidatorFormatter"/> interface by calling <see cref="Object.ToString()"/> on the <see cref="IPropertyValidator"/> instance.
  /// </summary>
  [ImplementationFor(typeof(IValidatorFormatter), Lifetime = LifetimeKind.Singleton)]
  public class DefaultValidatorFormatter : IValidatorFormatter
  {
    public string Format (IPropertyValidator validator, Func<Type, string> typeNameFormatter)
    {
      ArgumentUtility.CheckNotNull("validator", validator);

      string? formattedValidator = validator.ToString();
      Assertion.IsNotNull(formattedValidator, "ToString() of validator type '{0}' returned null.", validator.GetType());
      return formattedValidator;
    }
  }
}
