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
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// Provides support for converting instances of type <see cref="MethodInfoAdapter"/> to and from instances of type <see cref="MethodInfo"/>.
  /// </summary>
  public class MethodInfoAdapterConverter : TypeConverter
  {
    /// <summary>
    /// Returns whether this converter can convert an object of the given type to a <see cref="MethodInfoAdapter"/>, using the specified context. 
    /// This method returns <see langword="true" /> if <paramref name="sourceType"/> is <see cref="MethodInfo"/>; <see langword="false" /> otherwise.
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
      return sourceType == typeof (MethodInfo);
    }

    /// <summary>
    /// Returns whether this converter can convert a <see cref="MethodInfoAdapter"/> to an object of the given type, using the specified context. This
    /// method returns <see langword="true" /> if <paramref name="destinationType"/> is <see cref="MethodInfo"/>; <see langword="false" /> otherwise.
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
      return destinationType == typeof (MethodInfo);
    }

    /// <summary>
    /// Converts the given object to a <see cref="MethodInfoAdapter"/>, using the specified context and culture information.
    /// </summary>
    /// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. Ignored by this
    /// implementation</param>
    /// <param name="culture">The <see cref="System.Globalization.CultureInfo"/> to use as the current culture. Ignored by this implementation</param>
    /// <param name="value">The <see cref="System.Object"/> to convert. Must be a <see cref="MethodInfo"/> value.</param>
    /// <returns>
    /// An <see cref="System.Object"/> that represents the converted value.
    /// </returns>
    /// <exception cref="System.NotSupportedException">
    /// The conversion cannot be performed.
    /// </exception>
    public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      if (value == null)
        return null;

      var methodInfo = value as MethodInfo;
      if (methodInfo == null)
      {
        var message = string.Format ("Cannot convert value from type '{0}' to type '{1}'.", value.GetType (), typeof (MethodInfoAdapter));
        throw new NotSupportedException (message);
      }

      return MethodInfoAdapter.Create(methodInfo);
    }

    /// <summary>
    /// Converts the given value object to a <see cref="MethodInfo"/>, using the specified context and culture information.
    /// </summary>
    /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. Ignored
    /// by this implementation.</param>
    /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed. Ignored
    /// by this implementation.</param>
    /// <param name="value">The <see cref="T:System.Object"/> to convert. Must be an instance of <see cref="MethodInfoAdapter"/>.</param>
    /// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to. Must be
    /// <see cref="MethodInfo"/>.</param>
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

      if (destinationType != typeof (MethodInfo))
      {
        var message = string.Format (
            "Cannot convert values to type '{0}'. This converter only supports converting to type '{1}'.",
            destinationType,
            typeof (MethodInfo));
        throw new NotSupportedException (message);
      }

      if (value == null)
        return null;

      var methodInfoAdapter = value as MethodInfoAdapter;
      if (methodInfoAdapter == null)
      {
        var message = string.Format (
            "Cannot convert values of type '{0}' to type '{1}'. This converter only supports values of type '{2}'.",
            value.GetType (),
            destinationType,
            typeof (MethodInfoAdapter));
        throw new NotSupportedException (message);
      }

      return methodInfoAdapter.MethodInfo;
    }
  }
}