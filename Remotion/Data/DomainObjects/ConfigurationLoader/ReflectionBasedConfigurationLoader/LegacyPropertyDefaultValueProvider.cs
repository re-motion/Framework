using System;
using System.Linq;
#if NETFRAMEWORK
using System.Runtime.Serialization;
#else
using System.Runtime.CompilerServices;
#endif
using Remotion.Data.DomainObjects.Mapping;
using Remotion.ExtensibleEnums;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// Implements the legacy behavior for <see cref="PropertyDefinition.DefaultValue"/>, which sets non-nullable reference-<see cref="Type"/> properties to
  /// non-<see langword="null"/> values. This affects all <see cref="Array"/>, <see cref="string"/>, and <see cref="ExtensibleEnum{T}"/> properties.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  public class LegacyPropertyDefaultValueProvider : IPropertyDefaultValueProvider
  {
    public object? GetDefaultValue (IPropertyInformation propertyInfo, bool isNullable)
    {
      if (isNullable)
        return null;

      var propertyType = propertyInfo.PropertyType;
      if (propertyType.IsArray)
      {
        var elementType = propertyType.GetElementType();
        Assertion.DebugIsNotNull(elementType, "elementType != null when _propertyType.IsArray");
        return Array.CreateInstance(elementType, 0);
      }

      if (propertyType == typeof(string))
        return string.Empty;

      if (propertyType.IsEnum)
      {
        var firstValueOrNull = EnumUtility.GetEnumMetadata(propertyType).OrderedValues.FirstOrDefault();
        if (firstValueOrNull == null)
          firstValueOrNull = Activator.CreateInstance(propertyType);

        return firstValueOrNull;
      }

      if (ExtensibleEnumUtility.IsExtensibleEnumType(propertyType))
      {
        var firstValueOrNull = ExtensibleEnumUtility.GetDefinition(propertyType).GetValueInfos().FirstOrDefault()?.Value;
        return firstValueOrNull ?? CreateUninitializedExtensibleEnumValue(propertyType);
      }

      return propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null;
    }

    private IExtensibleEnum CreateUninitializedExtensibleEnumValue (Type extensibleEnumType)
    {
#if NETFRAMEWORK
      var uninitializedObject = FormatterServices.GetSafeUninitializedObject(extensibleEnumType);
#else
      var uninitializedObject = RuntimeHelpers.GetUninitializedObject(extensibleEnumType);
#endif
      return (IExtensibleEnum)uninitializedObject;
    }
  }
}
