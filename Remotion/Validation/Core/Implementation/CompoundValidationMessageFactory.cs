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
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Combines one or more <see cref="IValidationMessageFactory"/>-instances.
  /// When calling the factory method, the first result that is not <see langword="null" /> will be used.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor (typeof (IValidationMessageFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]

  public class CompoundValidationMessageFactory : IValidationMessageFactory
  {
    public IReadOnlyCollection<IValidationMessageFactory> ValidationMessageFactories { get; }

    public CompoundValidationMessageFactory (IEnumerable<IValidationMessageFactory> validationMessageFactories)
    {
      ValidationMessageFactories = validationMessageFactories.ToList().AsReadOnly();
    }

    public ValidationMessage CreateValidationMessageForPropertyValidator (Type validatorType, IPropertyInformation validatedProperty)
    {
      ArgumentUtility.CheckNotNull ("validatorType", validatorType);
      ArgumentUtility.CheckNotNull ("validatedProperty", validatedProperty);

      return ValidationMessageFactories
          .Select (f => f.CreateValidationMessageForPropertyValidator (validatorType, validatedProperty))
          .FirstOrDefault (m => m != null);
    }
    public ValidationMessage CreateValidationMessageForObjectValidator (Type validatorType, ITypeInformation validatedType)
    {
      ArgumentUtility.CheckNotNull ("validatorType", validatorType);
      ArgumentUtility.CheckNotNull ("validatedType", validatedType);

      return ValidationMessageFactories
          .Select (f => f.CreateValidationMessageForObjectValidator (validatorType, validatedType))
          .FirstOrDefault (m => m != null);
    }
  }
}