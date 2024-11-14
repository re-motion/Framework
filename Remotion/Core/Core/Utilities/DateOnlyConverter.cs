// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.ComponentModel;
using System.Globalization;

namespace Remotion.Utilities;

  /// <summary> Specialization of <see cref="TypeConverter"/> for conversions from and to <see cref="DateOnly"/> from <see cref="DateTime" />. </summary>
public class DateOnlyConverter : TypeConverter
{

  public override bool CanConvertFrom (ITypeDescriptorContext? context, Type sourceType)
  {
    ArgumentUtility.CheckNotNull("sourceType", sourceType);

    return IsDateTimeType(sourceType) || base.CanConvertFrom(context, sourceType);
  }

  public override bool CanConvertTo (ITypeDescriptorContext? context, Type? destinationType)
  {
    return IsDateTimeType(destinationType) || base.CanConvertTo(context, destinationType);
  }

  public override object? ConvertFrom (ITypeDescriptorContext? context, CultureInfo? culture, object? value)
  {
    if (value == null)
      return null;

    if (value is DateTime dateTime)
      return DateOnly.FromDateTime(dateTime);

    return base.ConvertFrom(context, culture, value);
  }

  public override object? ConvertTo (ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    ArgumentUtility.CheckNotNull("destinationType", destinationType);

    if (!IsDateTimeType(destinationType))
      throw new NotSupportedException($"Cannot convert value to type '{destinationType}'. This converter only supports converting to '{typeof(DateTime)}'.");

    if (value == null)
      return null;

    if (value is DateOnly dateOnlyForDateTime)
        return dateOnlyForDateTime.ToDateTime(TimeOnly.MinValue);

    return base.ConvertTo(context, culture, value, destinationType);
  }

  private bool IsDateTimeType (Type? type) => type == typeof(DateTime?) || type == typeof(DateTime);
}
