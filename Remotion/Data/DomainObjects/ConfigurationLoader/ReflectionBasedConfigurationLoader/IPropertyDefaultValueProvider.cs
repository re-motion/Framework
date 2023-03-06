using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// Provides the default value returned by <see cref="PropertyDefinition"/>.<see cref="PropertyDefinition.DefaultValue"/>.
  /// </summary>
  /// <seealso cref="PropertyDefaultValueProvider" />
  /// <seealso cref="LegacyPropertyDefaultValueProvider" />
  public interface IPropertyDefaultValueProvider
  {
    /// <summary>
    /// Gets the default value for the property represented by teh given <paramref name="propertyInfo"/>.
    /// </summary>
    object? GetDefaultValue (IPropertyInformation propertyInfo, bool isNullable);
  }
}
