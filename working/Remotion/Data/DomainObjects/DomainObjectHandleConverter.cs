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

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Converts <see cref="IDomainObjectHandle{T}"/> instances from and to <see cref="string"/>.
  /// </summary>
  public class DomainObjectHandleConverter : TypeConverter
  {
    public override bool CanConvertFrom (ITypeDescriptorContext context, System.Type sourceType)
    {
      return sourceType == typeof (string);
    }

    public override bool CanConvertTo (ITypeDescriptorContext context, System.Type destinationType)
    {
      return destinationType == typeof (string);
    }

    public override object ConvertFrom (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    {
      if (value == null)
        return null;

      var stringValue = value as string;
      if (stringValue == null)
        throw new NotSupportedException (string.Format ("This TypeConverter cannot convert from values of type '{0}'.", value.GetType()));

      try
      {
        return ObjectID.Parse (stringValue).GetHandle<IDomainObject>();
      }
      catch (FormatException ex)
      {
        throw new NotSupportedException ("The given string is not a valid ObjectID string.", ex);
      }
    }

    public override object ConvertTo (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    {
      if (destinationType != typeof (string))
        throw new NotSupportedException (string.Format ("This TypeConverter cannot convert to values of type '{0}'.", destinationType));

      if (value == null)
        return null;

      var domainObjectHandle = value as IDomainObjectHandle<IDomainObject>;
      if (domainObjectHandle == null)
      {
        var message = string.Format ("This TypeConverter can only convert values of type '{0}'.", typeof (IDomainObjectHandle<IDomainObject>));
        throw new NotSupportedException (message);
      }
      
      return domainObjectHandle.ObjectID.ToString();
    }
  }
}