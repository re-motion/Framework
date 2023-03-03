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
using Remotion.FunctionalProgramming;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;

namespace Remotion.Validation.Attributes.Validation
{
  /// <summary>
  /// Apply the <see cref="NotEmptyValidationAttribute"/> to introduce a <see cref="NotEmptyBinaryValidator"/> or <see cref="NotEmptyCollectionValidator"/>,
  /// based on the property <see cref="Type"/>.
  /// </summary>
  /// <remarks>
  /// The mapping between property types and validator types is as follows:
  /// <list type="bullet">
  ///   <item>
  ///     <term>byte[]: </term>
  ///     <description><see cref="NotEmptyBinaryValidator"/></description>
  ///   </item>
  ///   <item>
  ///     <term><see cref="ICollection"/>: </term>
  ///     <description><see cref="NotEmptyCollectionValidator"/></description>
  ///   </item>
  ///   <item>
  ///     <term><see cref="ICollection{T}"/>: </term>
  ///     <description><see cref="NotEmptyCollectionValidator"/></description>
  ///   </item>
  ///   <item>
  ///     <term><see cref="IReadOnlyCollection{T}"/>: </term>
  ///     <description><see cref="NotEmptyCollectionValidator"/></description>
  ///   </item>
  /// </list>
  /// </remarks>
  public class NotEmptyValidationAttribute : AddingValidationAttributeBase
  {
    public NotEmptyValidationAttribute ()
    {
    }

    protected override IEnumerable<IPropertyValidator> GetValidators (IPropertyInformation property, IValidationMessageFactory validationMessageFactory)
    {
      ArgumentUtility.CheckNotNull("property", property);
      ArgumentUtility.CheckNotNull("validationMessageFactory", validationMessageFactory);

      Func<ValidationMessage, IPropertyValidator> validatorFactory;
      if (property.PropertyType == typeof(byte[]))
        validatorFactory = msg => new NotEmptyBinaryValidator(msg);
      else if(typeof(ICollection).IsAssignableFrom(property.PropertyType))
        validatorFactory = msg => new NotEmptyCollectionValidator(msg);
      else if (typeof(IReadOnlyCollection<object>).IsAssignableFrom(property.PropertyType))
        validatorFactory = msg => new NotEmptyCollectionValidator(msg);
      else if(property.PropertyType.CanAscribeTo(typeof(ICollection<>)))
        validatorFactory = msg => new NotEmptyCollectionValidator(msg);
      else
        throw new NotSupportedException($"{nameof(NotEmptyValidationAttribute)} is not supported for properties of type '{property.PropertyType.FullName}'.");

      IPropertyValidator propertyValidator;
      if (string.IsNullOrEmpty(ErrorMessage))
      {
        propertyValidator = PropertyValidatorFactory.Create(
            property,
            parameters => validatorFactory(parameters.ValidationMessage),
            validationMessageFactory);
      }
      else
      {
        propertyValidator = validatorFactory(new InvariantValidationMessage(ErrorMessage));
      }

      return EnumerableUtility.Singleton(propertyValidator);
    }
  }
}
