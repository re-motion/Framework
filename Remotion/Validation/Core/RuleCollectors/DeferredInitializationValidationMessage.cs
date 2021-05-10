using System;
using System.Globalization;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.RuleCollectors
{
  internal sealed class DeferredInitializationValidationMessage : ValidationMessage
  {
    private ValidationMessage? _validationMessage;

    public DeferredInitializationValidationMessage ()
    {
    }

    public void Initialize (ValidationMessage validationMessage)
    {
      if (_validationMessage != null)
        throw new InvalidOperationException ("Validation message has already been initialized.");

      _validationMessage = validationMessage;
    }

    public override string Format (CultureInfo culture, IFormatProvider? formatProvider, params object?[] parameters)
    {
      if (_validationMessage == null)
        throw new InvalidOperationException ("Validation message has not been initialized.");

      return _validationMessage.Format (culture, formatProvider, parameters);
    }

    public override string ToString ()
    {
      if (_validationMessage == null)
        return nameof (DeferredInitializationValidationMessage);

      return _validationMessage.ToString();
    }
  }
}