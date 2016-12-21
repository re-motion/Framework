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
using System.ComponentModel;
using System.Globalization;

namespace Remotion.Utilities
{
  /// <summary> Specialization of <see cref="TypeConverter"/> for conversions from and to <see cref="Enum"/> types. </summary>
  public class AdvancedEnumConverter: EnumConverter
  {
    private readonly Type _enumType;
    private readonly Type _underlyingType;
    private readonly bool _isNullable;

    public AdvancedEnumConverter (Type enumType)
        : base (Nullable.GetUnderlyingType (ArgumentUtility.CheckNotNull ("enumType", enumType)) ?? enumType)
    {
      _enumType = enumType;
      _underlyingType = Enum.GetUnderlyingType (UnderlyingEnumType);
      _isNullable = UnderlyingEnumType != EnumType;
    }

    /// <summary>
    /// Gets the <see cref="Type"/> this converter is associated with, as it was passed to this <see cref="AdvancedEnumConverter"/>'s constructor.
    /// This can be an <see cref="Enum"/> type or a nullable <see cref="Enum"/> type.
    /// </summary>
    /// <returns>
    /// The <see cref="Type"/> this converter is associated with, as it was passed to this <see cref="AdvancedEnumConverter"/>'s constructor.
    /// </returns>
    public new Type EnumType
    {
      get { return _enumType; }
    }

    /// <summary>
    /// Gets the <see cref="Enum"/> <see cref="Type"/> this converter is associated with. If a nullable type was passed to this 
    /// <see cref="AdvancedEnumConverter"/>'s constructor, this property returns the actual <see cref="Enum"/> type underlying the nullable type.
    /// Otherwise, it returns the same as <see cref="EnumType"/>.
    /// </summary>
    /// <returns>
    /// The <see cref="Enum"/> <see cref="Type"/> this converter is associated with.
    /// </returns>
    public Type UnderlyingEnumType
    {
      get { return base.EnumType; }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is associated with a nullable <see cref="Enum"/>.
    /// </summary>
    /// <returns>
    /// 	<see langword="true"/> if <see cref="EnumType"/> is nullable; otherwise, <see langword="false" />.
    /// </returns>
    public bool IsNullable
    {
      get { return _isNullable; }
    }

    /// <summary> Test: Can convert from <paramref name="sourceType"/> to <see cref="String"/>? </summary>
    /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
    /// <param name="sourceType"> The <see cref="Type"/> of the value to be converted into an <see cref="Enum"/> type. </param>
    /// <returns> <see langword="true"/> if the conversion is supported. </returns>
    public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
    {
      ArgumentUtility.CheckNotNull ("sourceType", sourceType);

      if (sourceType == _underlyingType)
        return true;
  
      if (_isNullable && Nullable.GetUnderlyingType (sourceType) == _underlyingType)
        return true;

      return base.CanConvertFrom (context, sourceType);
    }

    /// <summary> Test: Can convert from <see cref="String"/> to <paramref name="destinationType"/>? </summary>
    /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
    /// <param name="destinationType"> The <see cref="Type"/> to convert an <see cref="Enum"/> value to. </param>
    /// <returns> <see langword="true"/> if the conversion is supported. </returns>
    public override bool CanConvertTo (ITypeDescriptorContext context, Type destinationType)
    {
      ArgumentUtility.CheckNotNull ("destinationType", destinationType);

      if (!_isNullable && destinationType == _underlyingType)
        return true;

      if (Nullable.GetUnderlyingType (destinationType) == _underlyingType)
        return true;

      return base.CanConvertFrom (context, destinationType);
    }

    /// <summary> Converts <paramref name="value"/> into an <see cref="Enum"/> value. </summary>
    /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
    /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
    /// <param name="value"> The source value. </param>
    /// <returns> An <see cref="Enum"/> value.  </returns>
    /// <exception cref="NotSupportedException"> The conversion could not be performed. </exception>
    public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      // ReSharper disable ConditionIsAlwaysTrueOrFalse
      if (_isNullable && (value == null || (value is string) && string.IsNullOrEmpty ((string) value)))
        return null;

      if (!(value is string))
      {
        if (value != null && _underlyingType == value.GetType())
        {
          if (!EnumUtility.IsValidEnumValue(UnderlyingEnumType, value))
            throw new ArgumentOutOfRangeException (string.Format ("The value {0} is not supported for enumeration '{1}'.", value, UnderlyingEnumType.FullName), (Exception) null);

          return Enum.ToObject (UnderlyingEnumType, value);
        }
      }

      return base.ConvertFrom (context, culture, value);
      // ReSharper restore ConditionIsAlwaysTrueOrFalse
    }

    /// <summary> Convertes an <see cref="Enum"/> value into the <paramref name="destinationType"/>. </summary>
    /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
    /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
    /// <param name="value"> The <see cref="Enum"/> value to be converted. </param>
    /// <param name="destinationType"> The destination <see cref="Type"/>. Must not be <see langword="null"/>. </param>
    /// <returns> An <see cref="Object"/> that represents the converted value. </returns>
    /// <exception cref="NotSupportedException"> The conversion could not be performed. </exception>
    public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      ArgumentUtility.CheckNotNull ("destinationType", destinationType);

      // ReSharper bug: value can be null
      // ReSharper disable ConditionIsAlwaysTrueOrFalse
      // ReSharper disable HeuristicUnreachableCode
      if (_isNullable && value == null)
        return (destinationType == typeof (string)) ? string.Empty : null;
      // ReSharper restore ConditionIsAlwaysTrueOrFalse
      // ReSharper restore ConditionIsAlwaysTrueOrFalse
      
      bool isMatchingDestinationType = !_isNullable && destinationType == _underlyingType;
      bool isMatchingNullableDestinationType = Nullable.GetUnderlyingType (destinationType) == _underlyingType;
      
      if (value is Enum && (isMatchingDestinationType || isMatchingNullableDestinationType))
        return Convert.ChangeType (value, _underlyingType, culture);
      
      return base.ConvertTo (context, culture, value, destinationType);
    }
  }
}
