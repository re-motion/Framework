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
  /// The <see cref="PropertyValueTooLongException"/> is thrown when a domain object property's value exceeds it's maximum length.
  /// </summary>
  [Serializable]
  public class PropertyValueTooLongException : DomainObjectValidationException
  {
    private readonly DomainObject? _domainObject;
    private readonly string _propertyName;
    private readonly int _maxLength;

    public PropertyValueTooLongException (DomainObject? domainObject, string propertyName, int maxLength, string message)
        : this(domainObject, propertyName, maxLength, message, null)
    {
    }

    public PropertyValueTooLongException (DomainObject? domainObject, string propertyName, int maxLength, string message, Exception? inner)
        : base(message, inner)
    {
      ArgumentUtility.CheckNotNullOrEmpty("propertyName", propertyName);

      _domainObject = domainObject;
      _propertyName = propertyName;
      _maxLength = maxLength;
    }

#if NET8_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    protected PropertyValueTooLongException (SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
      _domainObject = (DomainObject?)info.GetValue("_domainObject", typeof(DomainObject));
      _propertyName = info.GetString("_propertyName")!;
      _maxLength = info.GetInt32("_maxLength");
    }

#if NET8_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    public override void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);

      info.AddValue("_domainObject", _domainObject);
      info.AddValue("_propertyName", _propertyName);
      info.AddValue("_maxLength", _maxLength);
    }

    public DomainObject? DomainObject
    {
      get { return _domainObject; }
    }

    public string PropertyName
    {
      get { return _propertyName; }
    }

    public int MaxLength
    {
      get { return _maxLength; }
    }

    public override DomainObject[] AffectedObjects
    {
      get { return _domainObject == null ? new DomainObject[0] : new[] { _domainObject }; }
    }
  }
}
