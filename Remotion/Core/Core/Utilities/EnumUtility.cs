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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Remotion.Reflection;

namespace Remotion.Utilities
{
  /// <summary>
  /// This utility class provides methods for dealing with enumeration values.
  /// </summary>
  public static class EnumUtility
  {
    public class EnumMetadata
    {
      private readonly Dictionary<ulong, object> _enumValuesByNumericValue;
      private readonly ReadOnlyCollection<object> _enumValues;

      public readonly Type UnderlyingType;
      public readonly bool IsFlagsEnum;

      public EnumMetadata (Type enumType)
      {
        UnderlyingType = Enum.GetUnderlyingType(enumType);
        IsFlagsEnum = Attribute.IsDefined(enumType, typeof(FlagsAttribute), false);

        var enumValueArray = Enum.GetValues(enumType);

        var enumValues = new List<object>(enumValueArray.Length);
        _enumValuesByNumericValue = new Dictionary<ulong, object>();

        foreach (object value in enumValueArray)
        {
          enumValues.Add(value);
          _enumValuesByNumericValue[ToUInt64(value)] = value;
        }
        _enumValues = enumValues.AsReadOnly();
      }

      public ReadOnlyCollection<object> OrderedValues
      {
        get { return _enumValues; }
      }

      [CLSCompliant(false)]
      public IEnumerable<ulong> NumericValues
      {
        get { return _enumValuesByNumericValue.Keys; }
      }

      [CLSCompliant(false)]
      public object GetValueByNumericValue (ulong numericValue)
      {
        return _enumValuesByNumericValue[numericValue];
      }

      [CLSCompliant(false)]
      public bool ContainsNumericValue (ulong numericValue)
      {
        return _enumValuesByNumericValue.ContainsKey(numericValue);
      }
    }

    private static readonly ConcurrentDictionary<Type, EnumMetadata> s_cache = new ConcurrentDictionary<Type, EnumMetadata>();

    /// <summary>
    /// Checks whether the specified value is one of the values that the enumeration type defines.
    /// </summary>
    public static bool IsValidEnumValue (object enumValue)
    {
      ArgumentUtility.CheckNotNull("enumValue", enumValue);

      var enumType = enumValue.GetType();
      if (!enumType.IsEnum)
      {
        throw new ArgumentException(
            string.Format("Argument was of type '{0}' but only enum-types are supported with this overload.", enumType.GetFullNameSafe()), "enumValue");
      }

      var enumMetadata = GetEnumMetadata(enumType);
      return IsValidEnumValueInternal(enumMetadata, enumValue);
    }

    /// <summary>
    /// Checks whether the specified <paramref name="value"/> is one of the values that the <paramref name="enumType"/> defines.
    /// </summary>
    /// <remarks>This method corresponds to <see cref="Enum"/>.<see cref="Enum.IsDefined"/> but can also handls flag-enums.</remarks>
    /// <exception cref="ArgumentException">
    ///   <para><paramref name="enumType"/> is not an <see cref="Enum"/>.</para>
    ///   -or- 
    ///   <para>The type of <paramref name="value"/> is not an <paramref name="enumType"/>.</para>
    ///   -or-
    ///   <para>The type of <paramref name="value"/> is not an underlying type of <paramref name="enumType"/>.</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   value is not type <see cref="SByte"/>, <see cref="Int16"/>, <see cref="Int32"/>, <see cref="Int64"/>, 
    ///   <see cref="Byte"/>, <see cref="UInt16"/>, <see cref="UInt32"/>, <see cref="UInt64"/>.
    /// </exception>
    public static bool IsValidEnumValue (Type enumType, object value)
    {
      ArgumentUtility.CheckNotNull("enumType", enumType);
      if (!enumType.IsEnum)
      {
        throw new ArgumentException(
            string.Format("Argument was a type representing '{0}' but only enum-types are supported.", enumType.GetFullNameSafe()), "enumType");
      }

      var enumMetadata = GetEnumMetadata(enumType);

      var enumValueType = value.GetType();
      if (enumValueType.IsEnum)
      {
        if (enumType != enumValueType)
        {
          throw new ArgumentException(
              string.Format(
                  "Object must be the same type as the enum. The type passed in was '{0}'; the enum type was '{1}'.",
                  enumValueType,
                  enumType),
              "value");
        }
      }
      else
      {
        if (enumMetadata.UnderlyingType != enumValueType)
        {
          throw new ArgumentException(
              string.Format(
                  "Enum underlying type and the object must be same type. The type passed in was '{0}'; the enum underlying type was '{1}'.",
                  enumValueType,
                  enumMetadata.UnderlyingType),
              "value");
        }
      }

      return IsValidEnumValueInternal(enumMetadata, value);
    }

    public static bool IsFlagsEnumValue (object enumValue)
    {
      ArgumentUtility.CheckNotNull("enumValue", enumValue);

      return IsFlagsEnumType(enumValue.GetType());
    }

    public static bool IsFlagsEnumType (Type enumType)
    {
      ArgumentUtility.CheckNotNull("enumType", enumType);
      if (!enumType.IsEnum)
      {
        throw new ArgumentException(
            string.Format("Argument was a type representing '{0}' but only enum-types are supported.", enumType.GetFullNameSafe()), "enumType");
      }

      return GetEnumMetadata(enumType).IsFlagsEnum;
    }

    private static bool IsValidEnumValueInternal (EnumMetadata enumMetaData, object enumValue)
    {
      var numericEnumValue = ToUInt64(enumValue);
      if (enumMetaData.IsFlagsEnum)
      {
        if (numericEnumValue == 0L)
          return enumMetaData.ContainsNumericValue(numericEnumValue);

        var missingBits = ulong.MaxValue;
        foreach (var definedValue in enumMetaData.NumericValues)
        {
          if ((definedValue & numericEnumValue) == definedValue)
            missingBits &= ~definedValue;
        }
        return (numericEnumValue & missingBits) == 0;
      }
      else
      {
        return enumMetaData.ContainsNumericValue(numericEnumValue);
      }
    }

    public static EnumMetadata GetEnumMetadata (Type enumType)
    {
      // C# compiler 7.2 already provides caching for anonymous method.
      return s_cache.GetOrAdd(enumType, t => new EnumMetadata(t));
    }

    private static ulong ToUInt64 (object value)
    {
      switch (Convert.GetTypeCode(value))
      {
        case TypeCode.SByte:
        case TypeCode.Int16:
        case TypeCode.Int32:
        case TypeCode.Int64:
          return (ulong)Convert.ToInt64(value, CultureInfo.InvariantCulture);

        case TypeCode.Byte:
        case TypeCode.UInt16:
        case TypeCode.UInt32:
        case TypeCode.UInt64:
          return Convert.ToUInt64(value, CultureInfo.InvariantCulture);
      }
      //unreachable
      throw new InvalidOperationException("Unknown Enum Type");
    }
  }
}
