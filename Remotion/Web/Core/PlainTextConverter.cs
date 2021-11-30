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
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Web
{
  /// <summary>
  /// Provides <see cref="PlainTextString"/> conversion from and to <see cref="String"/> and <see cref="InstanceDescriptor"/> instances.
  /// </summary>
  public class PlainTextStringConverter : TypeConverter
  {
    private static readonly MethodInfo s_createFromTextMethodInfo = MemberInfoFromExpressionUtility.GetMethod(() => PlainTextString.CreateFromText(null));

    /// <inheritdoc />
    public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof(string)
             || base.CanConvertFrom(context, sourceType);
    }

    /// <inheritdoc />
    public override bool CanConvertTo (ITypeDescriptorContext context, Type destinationType)
    {
      return destinationType == typeof(string)
             || destinationType == typeof(InstanceDescriptor)
             || base.CanConvertTo(context, destinationType);
    }

    /// <inheritdoc />
    public override object? ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object? value)
    {
      if (value == null)
        return null;

      if (value is string @string)
        return PlainTextString.CreateFromText(@string);

      return base.ConvertFrom(context, culture, value);
    }

    /// <inheritdoc />
    public override object? ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object? value, Type destinationType)
    {
      if (value == null)
        return null;

      var plainTextString = (PlainTextString)value;

      if (destinationType == typeof(string))
        return plainTextString.GetValue();

      if (destinationType == typeof(InstanceDescriptor))
        return ConvertToInstanceDescriptor(in plainTextString);

      return base.ConvertTo(context, culture, value, destinationType);
    }

    private object ConvertToInstanceDescriptor (in PlainTextString input)
    {
      var rawString = input.GetValue();
      return new InstanceDescriptor(s_createFromTextMethodInfo, new[] { rawString });
    }
  }
}
