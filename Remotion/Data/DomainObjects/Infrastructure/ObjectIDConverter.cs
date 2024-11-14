// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.ComponentModel;
using System.Globalization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure;

/// <summary> Specialization of <see cref="TypeConverter"/> for conversions from and to <see cref="ObjectID"/>. </summary>
public class ObjectIDConverter : TypeConverter
{
  public override bool CanConvertFrom (ITypeDescriptorContext? context, Type sourceType)
  {
    ArgumentUtility.CheckNotNull(nameof(sourceType), sourceType);

    return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
  }

  public override bool CanConvertTo (ITypeDescriptorContext? context, Type? destinationType)
  {
    if (destinationType == null)
      return false;

    return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
  }

  public override object? ConvertFrom (ITypeDescriptorContext? context, CultureInfo? culture, object? value)
  {
    if (value == null)
      return null;

    if (value is string valueString)
    {
      return ObjectID.Parse(valueString);
    }

    return base.ConvertFrom(context, culture, value);
  }

  public override object? ConvertTo (ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    ArgumentUtility.CheckNotNull(nameof(destinationType), destinationType);

    if (value == null)
      return null;

    var objectID = (ObjectID)value;

    if (destinationType == typeof(string))
      return objectID.ToString();

    return base.ConvertTo(context, culture, value, destinationType);
  }
}
