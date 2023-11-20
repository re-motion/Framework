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
using Remotion.FunctionalProgramming;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Results;
using Remotion.Validation.Validators;

namespace Remotion.Validation.Attributes.Validation
{
  /// <summary>
  /// Apply the <see cref="LengthValidationAttribute"/> to introduce a <see cref="LengthValidator"/> constraint for a string property.
  /// </summary>
  public class LengthValidationAttribute : AddingValidationAttributeBase
  {
    private class SkippedValidator : IPropertyValidator
    {
      public IEnumerable<ValidationFailure> Validate (PropertyValidatorContext context) => Enumerable.Empty<ValidationFailure>();
    }

    /// <summary>
    /// Gets or sets the minimum number of characters required.
    /// </summary>
    public int MinLength { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of characters allowed.
    /// </summary>
    public int MaxLength { get; set; }

    /// <summary>
    /// Instantiates a new <see cref="LengthValidationAttribute"/>.
    /// </summary>
    public LengthValidationAttribute ()
    {
    }

    protected override IEnumerable<IPropertyValidator> GetValidators (IPropertyInformation property, IValidationMessageFactory validationMessageFactory)
    {
      ArgumentUtility.CheckNotNull("property", property);
      ArgumentUtility.CheckNotNull("validationMessageFactory", validationMessageFactory);

      IPropertyValidator validator;
      if (string.IsNullOrEmpty(ErrorMessage))
      {
        validator = PropertyValidatorFactory.Create(
            property,
            parameters => CreateValidator(parameters.ValidationMessage),
            validationMessageFactory);
      }
      else
      {
        validator = CreateValidator(new InvariantValidationMessage(ErrorMessage));
      }

      if (validator is SkippedValidator)
        return Enumerable.Empty<IPropertyValidator>();

      return EnumerableUtility.Singleton(validator);
    }

    private IPropertyValidator CreateValidator (ValidationMessage message)
    {
      if (MinLength == 0 && MaxLength == 0)
        return new SkippedValidator();
      else if (MinLength == 0 && MaxLength > 0)
        return new MaximumLengthValidator(MaxLength, message);
      else if (MaxLength == 0 & MinLength > 0)
        return new MinimumLengthValidator(MinLength, message);
      else if (MinLength == MaxLength)
        return new ExactLengthValidator(MinLength, message);
      else
        return new LengthValidator(MinLength, MaxLength, message);
    }
  }
}
