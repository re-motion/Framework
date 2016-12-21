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

  public class TypeNameConverter : TypeConverter
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public TypeNameConverter ()
    {
    }

    // methods and properties

    public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof (string);
    }

    public override bool CanConvertTo (ITypeDescriptorContext context, Type destinationType)
    {
      return destinationType == typeof (string);
    }

    public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      if (value is string)
      {
        string stringValue = (string) value;
        if (stringValue.Length == 0)
          return null;
        else
          return TypeUtility.GetType (stringValue, true);
      }
      if (value == null)
        return null;
      return null;
    }

    public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      ArgumentUtility.CheckNotNull ("destinationType", destinationType);

      if (value == null)
        return string.Empty;

      if (value is Type && destinationType == typeof (string))
        return TypeUtility.GetPartialAssemblyQualifiedName ((Type) value);

      return base.ConvertTo (context, culture, value, destinationType);
    }
  }
}
