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
using Remotion.Utilities;

namespace Remotion.Validation.Attributes.Validation
{
  /// <summary>
  /// Apply the <see cref="RemoveValidatorAttribute"/> to remove a validator introduced to the property via its base type. For instance, this can be used to 
  /// redefine the length contraint for a property in a derived type.
  /// </summary>
  [AttributeUsage (AttributeTargets.Property)]
  public class RemoveValidatorAttribute : Attribute
  {
    private readonly Type _validatorType;
    private readonly Type _collectorTypeToRemoveFrom;

    /// <summary>
    /// Instantiates a new <see cref="RemoveValidatorAttribute "/>.
    /// </summary>
    /// <param name="validatorType">The type of the validator to remove. Must not be <see langword="null" />.</param>
    public RemoveValidatorAttribute (Type validatorType) : this(validatorType, null) { }

    /// <summary>
    /// Instantiates a new <see cref="RemoveValidatorAttribute "/>.
    /// </summary>
    /// <param name="validatorType">The type of the validator to remove. Must not be <see langword="null" />.</param>
    /// <param name="collectorTypeToRemoveFrom">Constraints the removal to validators introduced by the specified <see cref="Type"/>.</param>/>
    public RemoveValidatorAttribute (Type validatorType, Type collectorTypeToRemoveFrom)
    {
      ArgumentUtility.CheckNotNull ("validatorType", validatorType);

      _validatorType = validatorType;
      _collectorTypeToRemoveFrom = collectorTypeToRemoveFrom;
    }

    public Type ValidatorType
    {
      get { return _validatorType; }
    }

    public Type CollectorTypeToRemoveFrom
    {
      get { return _collectorTypeToRemoveFrom; }
    }
    
  }
}