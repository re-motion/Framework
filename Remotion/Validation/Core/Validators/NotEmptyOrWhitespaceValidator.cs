using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Results;

namespace Remotion.Validation.Validators
{
  public class NotEmptyOrWhitespaceValidator : IPropertyValidator
  {
    public string ErrorMessage { get; }
    public ValidationMessage ValidationMessage { get; }

    public NotEmptyOrWhitespaceValidator (ValidationMessage validationMessage)
    {
      ArgumentUtility.CheckNotNull("validationMessage", validationMessage);

      ErrorMessage = "The value must not be empty and cannot consist entirely of whitespace characters.";
      ValidationMessage = validationMessage;
    }

    public IEnumerable<PropertyValidationFailure> Validate (PropertyValidatorContext context)
    {
      if (IsValid(context))
        return Enumerable.Empty<PropertyValidationFailure>();

      return EnumerableUtility.Singleton(CreateValidationError(context));
    }

    private bool IsValid (PropertyValidatorContext context)
    {
      if (context.PropertyValue is string stringValue)
        return !string.IsNullOrWhiteSpace(stringValue);

      if (context.PropertyValue is string[] stringArray)
        return stringArray.Any(s => !string.IsNullOrWhiteSpace(s));

      return true;
    }

    private PropertyValidationFailure CreateValidationError (PropertyValidatorContext context)
    {
      return new PropertyValidationFailure(
          context.Instance,
          context.Property,
          context.PropertyValue,
          errorMessage: ErrorMessage,
          localizedValidationMessage: ValidationMessage.Format(CultureInfo.CurrentUICulture, null, Array.Empty<object>()));
    }
  }
}
