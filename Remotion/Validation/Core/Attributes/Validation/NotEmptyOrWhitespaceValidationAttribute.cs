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
  /// Apply the <see cref="NotEmptyOrWhitespaceValidationAttribute"/> to introduce a <see cref="NotEmptyOrWhitespaceValidator"/> to a <see cref="String"/> property.
  /// </summary>
  public class NotEmptyOrWhitespaceValidationAttribute : AddingValidationAttributeBase
  {
    protected override IEnumerable<IPropertyValidator> GetValidators (IPropertyInformation property, IValidationMessageFactory validationMessageFactory)
    {
      ArgumentUtility.CheckNotNull("property", property);
      ArgumentUtility.CheckNotNull("validationMessageFactory", validationMessageFactory);

      NotEmptyOrWhitespaceValidator validator;
      if (string.IsNullOrEmpty(ErrorMessage))
      {
        validator = PropertyValidatorFactory.Create(
            property,
            parameters => new NotEmptyOrWhitespaceValidator(parameters.ValidationMessage),
            validationMessageFactory);
      }
      else
      {
        validator = new NotEmptyOrWhitespaceValidator(new InvariantValidationMessage(ErrorMessage));
      }

      return EnumerableUtility.Singleton(validator);
    }
  }
}
