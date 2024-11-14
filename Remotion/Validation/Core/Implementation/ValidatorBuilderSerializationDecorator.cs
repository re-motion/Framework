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
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Enables <see cref="IValidatorBuilder"/> to be serialized/deserialized.
  /// </summary>
  [ImplementationFor(typeof(IValidatorBuilder), RegistrationType = RegistrationType.Decorator, Position = Int32.MinValue)]
  public class ValidatorBuilderSerializationDecorator
      : IValidatorBuilder
  {
    private readonly IValidatorBuilder _validatorBuilder;

    public ValidatorBuilderSerializationDecorator (IValidatorBuilder validatorBuilder)
    {
      ArgumentUtility.CheckNotNull("validatorBuilder", validatorBuilder);

      _validatorBuilder = validatorBuilder;
    }

    public IValidatorBuilder InnerValidatorBuilder
    {
      get { return _validatorBuilder; }
    }

    public IValidator BuildValidator (Type validatedType)
    {
      ArgumentUtility.CheckNotNull("validatedType", validatedType);

      return _validatorBuilder.BuildValidator(validatedType);
    }
  }
}
