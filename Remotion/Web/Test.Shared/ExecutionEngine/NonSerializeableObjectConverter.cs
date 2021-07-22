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
using Remotion.Utilities;

namespace Remotion.Web.Test.ExecutionEngine
{
  public class NonSerializeableObjectConverter : TypeConverter
  {
    public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
    {
      if (sourceType == typeof (string))
        return true;
      return base.CanConvertFrom (context, sourceType);
    }

    public override bool CanConvertTo (ITypeDescriptorContext context, Type destinationType)
    {
      if (destinationType == typeof (string))
        return true;
      return base.CanConvertTo (context, destinationType);
    }

    public override object ConvertFrom (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    {
      if (value is string)
      {
        return new NonSerializeableObject ((string) value);
      }
      return base.ConvertFrom (context, culture, value);
    }

    public override object ConvertTo (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    {
      NonSerializeableObject obj = ArgumentUtility.CheckNotNullAndType<NonSerializeableObject> ("value", value);
      if (destinationType == typeof (string))
      {
        return obj.Value;
      }
      return base.ConvertTo (context, culture, value, destinationType);
    }

  }
}
