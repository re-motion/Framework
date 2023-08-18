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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Validation
{
  /// <summary>
  /// The <see cref="PropertyValueNotSetException"/> is thrown when a domain object property is set to <see langword="null" /> 
  /// but the property's value is required.
  /// </summary>
  [Serializable]
  public class PropertyValueNotSetException : DomainObjectValidationException
  {
    private readonly DomainObject? _domainObject;
    private readonly string _propertyName;

    public PropertyValueNotSetException (DomainObject? domainObject, string propertyName, string message)
        : this(domainObject, propertyName, message, null)
    {
    }

    public PropertyValueNotSetException (DomainObject? domainObject, string propertyName, string message, Exception? inner)
        : base(message, inner)
    {
      ArgumentUtility.CheckNotNullOrEmpty("propertyName", propertyName);

      _domainObject = domainObject;
      _propertyName = propertyName;
    }

#if NET8_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    protected PropertyValueNotSetException (SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
      _domainObject = (DomainObject?)info.GetValue("_domainObject", typeof(DomainObject));
      _propertyName = info.GetString("_propertyName")!;
    }

#if NET8_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    public override void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);

      info.AddValue("_domainObject", _domainObject);
      info.AddValue("_propertyName", _propertyName);
    }

    public DomainObject? DomainObject
    {
      get { return _domainObject; }
    }

    public string PropertyName
    {
      get { return _propertyName; }
    }

    public override DomainObject[] AffectedObjects
    {
      get { return _domainObject == null ? new DomainObject[0] : new[] { _domainObject }; }
    }
  }
}
