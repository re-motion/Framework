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

/// <summary> Specialization of <see cref="TypeConverter"/> for conversions from and to <see cref="String"/>. </summary>
/// <remarks>
///   <para>
///     Conversions from <see cref="Single"/> and <see cref="Double"/> are done using "R" as format string. 
///   </para><para>
///     Conversion is possible under the following conditions:
///   </para>
///   <list type="bullet">
///     <item>
///       The <see cref="Type"/> is <see cref="Guid"/> or <see cref="DBNull"/>
///     </item>
///     <item>
///       The <see cref="Type"/> is an array of scalar values.
///     </item>
///     <item>
///       A <see cref="Type"/> implements either a 
///       public <see langword="static"/> &lt;DestinationType&gt; Parse (string) or a 
///       public <see langword="static"/> &lt;DestinationType&gt; Parse (string, IFormatProvider) method.
///     </item>
///   </list>
/// </remarks>
public class BidirectionalStringConverter: TypeConverter
{
  /// <summary> Test: Can convert from <paramref name="sourceType"/> to <see cref="String"/>? </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="sourceType"> 
  ///   The <see cref="Type"/> of the value to be converted into a <see cref="String"/>.
  /// </param>
  /// <returns> <see langword="true"/> if the conversion is supported. </returns>
  /// <remarks>
  /// In theory, every object can be converted to a string because every object has a <see cref="object.ToString"/> method. The 
  /// <see cref="BidirectionalStringConverter"/>, however, only supports objects for which round-tripping is supported. This method therefore only
  /// returns <see langword="true"/> for types whose values can be converted both into and back from a string.
  /// </remarks>
  public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
  {
    if (sourceType == null)
      return false;
    // check whether we can _parse_ the source type; only then, we can perform round-tripping
    return StringUtility.CanParse (sourceType);
  }

  /// <summary> Test: Can convert from <see cref="String"/> to <paramref name="destinationType"/>? </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="destinationType"> The <see cref="Type"/>  to convert a <see cref="String"/> value to. </param>
  /// <returns> <see langword="true"/> if the conversion is supported. </returns>
  public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
  {
    if (destinationType == null)
      return false;
    return StringUtility.CanParse (destinationType);
  }

  /// <summary> Converts <paramref name="value"/> into a <see cref="String"/>. </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value">  The source value. </param>
  /// <returns> A <see cref="String"/>.  </returns>
  /// <exception cref="NotSupportedException"> The conversion could not be performed. </exception>
  /// <remarks>
  ///   Conversions from <see cref="Single"/> and <see cref="Double"/> are done using "R" as format string. 
  /// </remarks>
  public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
  {
    if (value == null)
      return string.Empty;
    if (CanConvertFrom (context, value.GetType()))
      return StringUtility.Format (value, culture);
    throw new NotSupportedException (string.Format ("Cannot convert from '{0}' to String.", value.GetType()));
  }

  /// <summary> Convertes a <see cref="String"/> into the <paramref name="destinationType"/>. </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value"> The <see cref="String"/> to be converted. </param>
  /// <param name="destinationType"> The destination <see cref="Type"/>. Must not be <see langword="null"/>. </param>
  /// <returns> An <see cref="Object"/> that represents the converted value. </returns>
  /// <exception cref="NotSupportedException"> The conversion could not be performed. </exception>
  public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
  {
    ArgumentUtility.CheckNotNull ("destinationType", destinationType);

    if (value == null)
      value = string.Empty;

    if (value is string && CanConvertTo (context, destinationType))
      return StringUtility.Parse (destinationType, (string) value, culture);
    return base.ConvertTo (context, culture, value, destinationType);
  }

  /// <summary>
  ///   Returns whether the collection of standard values returned by 
  ///   <see cref="TypeConverter.GetStandardValues()"/> is an exclusive list.
  /// </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <returns> <see langword="false"/>. </returns>
  public override bool GetStandardValuesExclusive (ITypeDescriptorContext context)
  {
    return false;
  }

  /// <summary> Returns whether this object supports a standard set of values that can be picked from a list. </summary>
  /// <param name="context"> An ITypeDescriptorContext that provides a format context. </param>
  /// <returns> <see langword="false"/>. </returns>
  public override bool GetStandardValuesSupported (ITypeDescriptorContext context)
  {
    return false;
  }
}

}
