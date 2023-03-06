using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// Provides the <see cref="PropertyDefinition.DefaultValue"/> for a <see cref="PropertyDefinition"/>.
  /// </summary>
  public interface IPropertyDefaultValueProvider
  {
    /// <summary>
    /// Gets the default value for the property represented by teh given <paramref name="propertyInfo"/>.
    /// </summary>
    object? GetDefaultValue (IPropertyInformation propertyInfo, bool isNullable);
  }
}
