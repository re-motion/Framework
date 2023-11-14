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
  /// Provides <see cref="WebString"/> conversion from and to <see cref="String"/> and <see cref="InstanceDescriptor"/> instances.
  /// </summary>
  public class WebStringConverter : TypeConverter
  {
    private const string c_textPrefix = "(text)";
    private const string c_htmlPrefix = "(html)";

    private static readonly MethodInfo s_createFromTextMethodInfo = MemberInfoFromExpressionUtility.GetMethod(() => WebString.CreateFromText(null));
    private static readonly MethodInfo s_createFromHtmlMethodInfo = MemberInfoFromExpressionUtility.GetMethod(() => WebString.CreateFromHtml(null));

    /// <inheritdoc />
    public override bool CanConvertFrom (ITypeDescriptorContext? context, Type sourceType)
    {
      ArgumentUtility.CheckNotNull("sourceType", sourceType);

      return sourceType == typeof(string)
             || base.CanConvertFrom(context, sourceType);
    }

    /// <inheritdoc />
    public override bool CanConvertTo (ITypeDescriptorContext? context, Type? destinationType)
    {
      return destinationType == typeof(string)
             || destinationType == typeof(InstanceDescriptor)
             || base.CanConvertTo(context, destinationType);
    }

    /// <inheritdoc />
    public override object? ConvertFrom (ITypeDescriptorContext? context, CultureInfo? culture, object? value)
    {
      if (value == null)
        return null;

      if (value is string @string)
        return ConvertFromString(@string);

      return base.ConvertFrom(context, culture, value);
    }

    /// <inheritdoc />
    public override object? ConvertTo (ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
      ArgumentUtility.CheckNotNull("destinationType", destinationType);

      if (value == null)
        return null;

      var webString = (WebString)value;

      if (destinationType == typeof(string))
        return ConvertToString(in webString);

      if (destinationType == typeof(InstanceDescriptor))
        return ConvertToInstanceDescriptor(in webString);

      return base.ConvertTo(context, culture, value, destinationType);
    }

    private new object ConvertFromString (string input)
    {
      if (input.StartsWith(c_textPrefix))
        return WebString.CreateFromText(input.Substring(c_textPrefix.Length));

      if (input.StartsWith(c_htmlPrefix))
        return WebString.CreateFromHtml(input.Substring(c_htmlPrefix.Length));

      return WebString.CreateFromText(input);
    }

    private object ConvertToString (in WebString input)
    {
      var rawString = input.GetValue();

      if (input.Type == WebStringType.Encoded)
        return c_htmlPrefix + rawString;

      if (input.Type == WebStringType.PlainText)
      {
        if (rawString.StartsWith(c_htmlPrefix) || rawString.StartsWith(c_textPrefix))
          return c_textPrefix + rawString;

        return rawString;
      }

      throw new InvalidOperationException($"The conversion of a value with type {input.Type} is not supported.");
    }

    private object ConvertToInstanceDescriptor (in WebString input)
    {
      var rawString = input.GetValue();

      if (input.Type == WebStringType.PlainText)
        return new InstanceDescriptor(s_createFromTextMethodInfo, new[] { rawString });

      if (input.Type == WebStringType.Encoded)
        return new InstanceDescriptor(s_createFromHtmlMethodInfo, new[] { rawString });

      throw new InvalidOperationException($"The conversion of a value with type {input.Type} is not supported.");
    }
  }
}
