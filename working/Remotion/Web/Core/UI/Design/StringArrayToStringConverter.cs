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

namespace Remotion.Web.UI.Design
{

public class StringArrayConverter : TypeConverter
{
  /// <summary> Test: Can convert from the <paramref name="sourceType"/> to a <see cref="String"/> array? </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="sourceType"> 
  ///   The <see cref="Type"/> of the value to be converted into an <see cref="String"/>.
  /// </param>
  /// <returns> <see langword="true"/> if the conversion is supported. </returns>
  public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
  {
    if (sourceType == typeof (string))
      return true;
    return false;  
  }

  /// <summary> Test: Can convert a <see cref="String"/> array to the <paramref name="destinationType"/>? </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="destinationType"> 
  ///   The <see cref="Type"/> of the value to be converted into a <see cref="String"/> Array.
  /// </param>
  /// <returns> <see langword="true"/> if the conversion is supported. </returns>
  public override bool CanConvertTo (ITypeDescriptorContext context, Type destinationType)
  {
    if (destinationType == typeof (string))
      return true;
    return false;  
  }

  /// <summary> Converts <paramref name="value"/> into a <see cref="String"/> array. </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value">  The source value. </param>
  /// <returns>
  ///    A <see cref="String"/> array or <see langword="null"/> if the conversion failed. 
  /// </returns>
  public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
  {
    if (value is string)
      return ((string)value).Split (new char[] {','});
    return null;
  }

  /// <summary>
  ///   Convertes a <see cref="String"/> array into the <paramref name="destinationType"/>.
  /// </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value"> The <see cref="String"/> array to be converted. </param>
  /// <param name="destinationType"> The destination <see cref="Type"/>. </param>
  /// <returns> An <see cref="Object"/> that represents the converted value. </returns>
  public override object ConvertTo (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
  {
    if (value is string[])
      return string.Join (",", (string[]) value);
    return string.Empty;
  }
}
}
