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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Remotion.Utilities;

namespace Remotion.ExtensibleEnums
{
  /// <summary>
  /// Provides support for converting extensible enums to and from instances of type <see cref="string"/>.
  /// </summary>
  public class ExtensibleEnumConverter : TypeConverter
  {
    private readonly Type _extensibleEnumType;
    private readonly IExtensibleEnumDefinition _definition;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtensibleEnumConverter"/> class.
    /// </summary>
    /// <param name="extensibleEnumType">The extensible enum type to be converted from and to.</param>
    public ExtensibleEnumConverter (Type extensibleEnumType)
    {
      ArgumentUtility.CheckNotNull ("extensibleEnumType", extensibleEnumType);

      _extensibleEnumType = extensibleEnumType;
      _definition = ExtensibleEnumUtility.GetDefinition (ExtensibleEnumType);
    }

    /// <summary>
    /// Gets the extensible enum type to be be converted from and to.
    /// </summary>
    /// <value>The extensible enum type.</value>
    public Type ExtensibleEnumType 
    { 
      get { return _extensibleEnumType; }
    }

    /// <summary>
    /// Returns whether this converter can convert an object of the given type to the <see cref="ExtensibleEnumType"/>, using the specified context. 
    /// This method returns <see langword="true" /> if <paramref name="sourceType"/> is <see cref="string"/>; <see langword="false" /> otherwise.
    /// </summary>
    /// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. Ignored by this 
    /// implementation.</param>
    /// <param name="sourceType">A <see cref="System.Type"/> that represents the type you want to convert from.</param>
    /// <returns>
    /// <see langword="true" /> if this converter can perform the conversion; otherwise, <see langword="false" />.
    /// </returns>
    public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
    {
      ArgumentUtility.CheckNotNull ("sourceType", sourceType);

      return sourceType == typeof (string);
    }

    /// <summary>
    /// Returns whether this converter can convert an object of the <see cref="ExtensibleEnumType"/> to the given destination type, using the 
    /// specified context. This
    /// method returns <see langword="true" /> if <paramref name="destinationType"/> is <see cref="string"/>; <see langword="false" /> otherwise.
    /// </summary>
    /// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. Ignored by this 
    /// implementation.</param>
    /// <param name="destinationType">A <see cref="System.Type"/> that represents the type you want to convert to.</param>
    /// <returns>
    /// <see langword="true" /> if this converter can perform the conversion; otherwise, <see langword="false" />.
    /// </returns>
    public override bool CanConvertTo (ITypeDescriptorContext context, Type destinationType)
    {
      ArgumentUtility.CheckNotNull ("destinationType", destinationType);

      return destinationType == typeof (string);
    }

    /// <summary>
    /// Converts the given object to the <see cref="ExtensibleEnumType"/>, using the specified context and culture information.
    /// </summary>
    /// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. Ignored by this
    /// implementation</param>
    /// <param name="culture">The <see cref="System.Globalization.CultureInfo"/> to use as the current culture. Ignored by this implementation</param>
    /// <param name="value">The <see cref="System.Object"/> to convert. Must be a <see cref="string"/> value.</param>
    /// <returns>
    /// An <see cref="System.Object"/> that represents the converted value.
    /// </returns>
    /// <exception cref="System.NotSupportedException">
    /// The conversion cannot be performed.
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// The value is of a convertible type, but the <see cref="ExtensibleEnumType"/> does not define a corresponding value.
    /// </exception>
    public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      if (value == null)
        return null;
      
      var stringValue = value as string;
      if (stringValue == null)
      {
        var message = string.Format ("Cannot convert value from type '{0}' to type '{1}'.", value.GetType(), ExtensibleEnumType);
        throw new NotSupportedException (message);
      }

      if (stringValue == string.Empty)
        return null;

      return _definition.GetValueInfoByID (stringValue).Value;
    }

    /// <summary>
    /// Converts the given extensible enum object to the given destination type, using the specified context and culture information.
    /// </summary>
    /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. Ignored
    /// by this implementation.</param>
    /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If <see langword="null" /> is passed, the current culture is 
    /// assumed. Ignored by this implementation.</param>
    /// <param name="value">The <see cref="T:System.Object"/> to convert. Must be an instance of <see cref="ExtensibleEnumType"/>.</param>
    /// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to. Must be
    /// <see cref="string"/>.</param>
    /// <returns>
    /// An <see cref="T:System.Object"/> that represents the converted value.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// The <paramref name="destinationType"/> parameter is null.
    /// </exception>
    /// <exception cref="T:System.NotSupportedException">
    /// The conversion cannot be performed.
    /// </exception>
    public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      ArgumentUtility.CheckNotNull ("destinationType", destinationType);

      if (destinationType != typeof (string))
      {
        var message = string.Format (
            "Cannot convert values to type '{0}'. This converter only supports converting to type '{1}'.",
            destinationType,
            typeof (string));
        throw new NotSupportedException (message);
      }

      if (value == null)
        return null;

      var enumValue = value as IExtensibleEnum;
      if (enumValue == null)
      {
        var message = string.Format (
            "Cannot convert values of type '{0}' to type '{1}'. This converter only supports values of type '{2}'.",
            value.GetType(),
            destinationType,
            ExtensibleEnumType);
        throw new NotSupportedException (message);
      }

      return enumValue.ID;
    }
  }
}