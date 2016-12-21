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
  /// The <see cref="NullValueConverter"/> can convert "null" between nullable types.
  /// </summary>
  public class NullValueConverter : TypeConverter
  {
    public static readonly NullValueConverter Instance = new NullValueConverter();

    protected NullValueConverter ()
    {
    }

    public override bool CanConvertTo (ITypeDescriptorContext context, Type destinationType)
    {
      ArgumentUtility.CheckNotNull ("destinationType", destinationType);

      return NullableTypeUtility.IsNullableType (destinationType);
    }

    public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
    {
      ArgumentUtility.CheckNotNull ("sourceType", sourceType);

      return NullableTypeUtility.IsNullableType (sourceType);
    }

    public override bool IsValid (ITypeDescriptorContext context, object value)
    {
// ReSharper disable ConditionIsAlwaysTrueOrFalse
      return value == null;
// ReSharper restore ConditionIsAlwaysTrueOrFalse
    }

    public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
    {
// ReSharper disable HeuristicUnreachableCode
// ReSharper disable ConditionIsAlwaysTrueOrFalse
      if(value == null)
        return null;
// ReSharper restore HeuristicUnreachableCode
// ReSharper restore ConditionIsAlwaysTrueOrFalse

      throw new NotSupportedException (string.Format ("Value '{0}' cannot be converted to null.", value));
    }

    public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      if (value != null)
        throw new NotSupportedException (string.Format ("Value '{0}' is not supported by this converter.", value));

// ReSharper disable HeuristicUnreachableCode
      if (!CanConvertTo (context, destinationType))
        throw new NotSupportedException (string.Format ("Null value cannot be converted to type '{0}'.", destinationType));

      return null;
// ReSharper restore HeuristicUnreachableCode
    }
  }
}