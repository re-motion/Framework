using System;
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// Implements the default behavior for <see cref="PropertyDefinition.DefaultValue"/>, which sets all reference-<see cref="Type"/> properties to
  /// <see langword="null"/>, even if they are declared with <see cref="NullablePropertyAttribute.IsNullable"/> set to <see langword="false"/>.
  /// </summary>     
  public class PropertyDefaultValueProvider : IPropertyDefaultValueProvider
  {
    public object? GetDefaultValue (IPropertyInformation propertyInfo, bool isNullable)
    {
      if (isNullable)
        return null;

      var propertyType = propertyInfo.PropertyType;
      if (propertyType.IsEnum)
      {
        var firstValueOrNull = EnumUtility.GetEnumMetadata(propertyType).OrderedValues.FirstOrDefault();
        if (firstValueOrNull != null)
          return firstValueOrNull;
      }

      return propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null;
    }
  }
}
