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
  /// <summary> 
  ///   Provides functionality to get the <see cref="TypeConverter"/> for a <see cref="Type"/> and to convert a value
  ///   from a source <see cref="Type"/> into a destination <see cref="Type"/>.
  /// </summary>
  /// <remarks>
  ///   Conversion is possible under the following conditions:
  ///   <list type="bullet">
  ///     <item>
  ///       A type has a <see cref="TypeConverter"/> applied through the <see cref="TypeConverterAttribute"/> that
  ///       supports the conversion. 
  ///     </item>
  ///     <item>
  ///       For <see cref="Enum"/> types into the <see cref="String"/> value or the underlying numeric 
  ///       <see cref="Type"/>.
  ///     </item>
  ///     <item>
  ///       For types without a <see cref="TypeConverter"/>, the <see cref="TypeConversionProvider"/> tries to use the 
  ///       <see cref="BidirectionalStringConverter"/>. See the documentation of the string converter for details on the
  ///       supported types.
  ///     </item>
  ///   </list>
  /// </remarks>
  public interface ITypeConversionProvider
  {
    /// <summary> 
    ///   Gets the <see cref="TypeConverter"/> that is able to convert an instance of the <paramref name="sourceType"/> 
    ///   <see cref="Type"/> into an instance of the <paramref name="destinationType"/> <see cref="Type"/>.
    /// </summary>
    /// <param name="sourceType"> 
    ///   The source <see cref="Type"/> of the value. Must not be <see langword="null"/>. 
    /// </param>
    /// <param name="destinationType"> 
    ///   The destination <see cref="Type"/> of the value. Must not be <see langword="null"/>. 
    /// </param>
    /// <returns> 
    ///   A <see cref="TypeConverterResult"/> or or <see cref="TypeConverterResult.Empty"/>if no matching <see cref="TypeConverter"/> can be found.
    /// </returns>
    /// <remarks> 
    ///   You can identify whether you must use the <see cref="TypeConverter.ConvertTo(object,Type)"/> or the 
    ///   <see cref="TypeConverter.ConvertFrom(object)"/> method by testing the returned <see cref="TypeConverter"/>'s
    ///   <see cref="TypeConverter.CanConvertTo(Type)"/> and <see cref="TypeConverter.CanConvertFrom(Type)"/> methods.
    /// </remarks>
    TypeConverterResult GetTypeConverter (Type sourceType, Type destinationType);

    /// <summary> 
    ///   Gets the <see cref="TypeConverter"/> that is associated with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type"> 
    ///   The <see cref="Type"/> to get the <see cref="TypeConverter"/> for. Must not be <see langword="null"/>.
    /// </param>
    /// <returns>
    ///   A <see cref="TypeConverter"/> or <see langword="null"/> of no <see cref="TypeConverter"/> can be found.
    /// </returns>
    TypeConverter? GetTypeConverter (Type type);

    /// <summary> 
    ///   Test whether the <see cref="TypeConversionProvider"/> object can convert an object of <see cref="Type"/> 
    ///   <paramref name="sourceType"/> into an object of <see cref="Type"/> <paramref name="destinationType"/>
    ///   by using the <see cref="TypeConversionProvider.Convert(System.Type,System.Type,object)"/> method.
    /// </summary>
    /// <param name="sourceType"> 
    ///   The source <see cref="Type"/> of the value. Must not be <see langword="null"/>. 
    /// </param>
    /// <param name="destinationType"> 
    ///   The destination <see cref="Type"/> of the value. Must not be <see langword="null"/>. 
    /// </param>
    /// <returns> <see langword="true"/> if a conversion is possible. </returns>
    bool CanConvert (Type sourceType, Type destinationType);

    //TODO RM-7432: remove "must not be null" from "value" parameter 
    /// <summary> Convertes the <paramref name="value"/> into the <paramref name="destinationType"/>. </summary>
    /// <param name="sourceType"> 
    ///   The source <see cref="Type"/> of the <paramref name="value"/>. Must not be <see langword="null"/>. 
    /// </param>
    /// <param name="destinationType"> 
    ///   The destination <see cref="Type"/> of the <paramref name="value"/>. Must not be <see langword="null"/>. 
    /// </param>
    /// <param name="value"> The value to be converted. Must not be <see langword="null"/>. </param>
    /// <returns> An <see cref="Object"/> that represents the converted <paramref name="value"/>. </returns>
    object? Convert (Type sourceType, Type destinationType, object? value);

    /// <summary> Convertes the <paramref name="value"/> into the <paramref name="destinationType"/>. </summary>
    /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
    /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
    /// <param name="sourceType"> 
    ///   The source <see cref="Type"/> of the <paramref name="value"/>. Must not be <see langword="null"/>. 
    /// </param>
    /// <param name="destinationType"> 
    ///   The destination <see cref="Type"/> of the <paramref name="value"/>. Must not be <see langword="null"/>. 
    /// </param>
    /// <param name="value"> The <see cref="Object"/> to be converted.</param>
    /// <returns> An <see cref="Object"/> that represents the converted <paramref name="value"/>. </returns>
    object? Convert (ITypeDescriptorContext? context, CultureInfo? culture, Type sourceType, Type destinationType, object? value);
  }
}
