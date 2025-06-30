using System;
using System.Globalization;
using Remotion.Globalization;
using Remotion.Utilities;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Globalization
{
  public sealed class ResourceManagerBasedValidationMessage : ValidationMessage
  {
    public IResourceManager ResourceManager { get; }
    public Enum ResourceIdentifier { get; }

    public ResourceManagerBasedValidationMessage (IResourceManager resourceManager, Enum resourceIdentifier)
    {
      ArgumentNullException.ThrowIfNull(resourceIdentifier);
      ArgumentNullException.ThrowIfNull(resourceManager);

      ResourceManager = resourceManager;
      ResourceIdentifier = resourceIdentifier;
    }

    public override string Format (CultureInfo culture, IFormatProvider? formatProvider, params object?[] parameters)
    {
      ArgumentNullException.ThrowIfNull(culture);
      ArgumentNullException.ThrowIfNull(parameters);

      using (new CultureScope(CultureInfo.InvariantCulture, culture))
      {
        var validationMessage = ResourceManager.GetString(ResourceIdentifier);
        return string.Format(formatProvider, validationMessage, parameters);
      }
    }

    public override string ToString ()
    {
      return ResourceManager.GetString(ResourceIdentifier);
    }
  }
}
