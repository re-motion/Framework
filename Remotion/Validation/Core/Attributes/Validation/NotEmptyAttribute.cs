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
using Remotion.FunctionalProgramming;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;

namespace Remotion.Validation.Attributes.Validation
{
  /// <summary>
  /// Apply the <see cref="NotEmptyAttribute"/> to introduce a <see cref="NotEmptyValidator"/> constraint for a property.
  /// </summary>
  public class NotEmptyAttribute : AddingValidationAttributeBase
  {
    public NotEmptyAttribute ()
    {
    }

    protected override IEnumerable<IPropertyValidator> GetValidators (IPropertyInformation property, IValidationMessageFactory validationMessageFactory)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      ArgumentUtility.CheckNotNull ("validationMessageFactory", validationMessageFactory);

      NotEmptyValidator validator;
      if (string.IsNullOrEmpty (ErrorMessage))
      {
        var validatorType = typeof (NotEmptyValidator);
        var validationMessage = validationMessageFactory.CreateValidationMessageForPropertyValidator (validatorType, property);
        if (validationMessage == null)
        {
          throw new InvalidOperationException (
              $"The {nameof (IValidationMessageFactory)} did not return a result for {validatorType.Name} applied to property '{property.Name}' on type '{property.GetOriginalDeclaringType().FullName}'.");
        }
        validator = new NotEmptyValidator (validationMessage);
      }
      else
      {
        validator = new NotEmptyValidator (new InvariantValidationMessage (ErrorMessage));
      }

      return EnumerableUtility.Singleton (validator);
    }
  }
}

