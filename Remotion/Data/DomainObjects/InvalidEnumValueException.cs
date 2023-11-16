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
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// The exception that is thrown when a <see cref="PropertyValue"/> is set with an enum value that does not match the property's type.
  /// </summary>
  [Serializable]
  public class InvalidEnumValueException : DomainObjectException
  {
    private readonly string _propertyName;
    private readonly Type _underlyingPropertyType;
    private readonly object _invalidValue;

    public InvalidEnumValueException (string message, string propertyName, Type propertyType, object invalidValue)
        : base(message)
    {
      ArgumentUtility.CheckNotNullOrEmpty("message", message);
      ArgumentUtility.CheckNotNullOrEmpty("propertyName", propertyName);
      ArgumentUtility.CheckNotNull("propertyType", propertyType);

      _propertyName = propertyName;
      _underlyingPropertyType = propertyType;
      _invalidValue = invalidValue;
    }

#if NET8_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    protected InvalidEnumValueException (SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
      _propertyName = info.GetString("PropertyName")!;
      _underlyingPropertyType = (Type)info.GetValue("UnderlyingPropertyType", typeof(Type))!;
      _invalidValue = info.GetValue("InvalidValue", typeof(object))!;
    }

    public string PropertyName
    {
      get { return _propertyName; }
    }

    public Type UnderlyingPropertyType
    {
      get { return _underlyingPropertyType; }
    }

    public object InvalidValue
    {
      get { return _invalidValue; }
    }

#if NET8_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    public override void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);

      info.AddValue("PropertyName", _propertyName);
      info.AddValue("UnderlyingPropertyType", _underlyingPropertyType);
      info.AddValue("InvalidValue", _invalidValue);
    }
  }
}
