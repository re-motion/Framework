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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Validation
{
  /// <summary>
  /// The <see cref="PropertyValueTooLongException"/> is thrown when a domain object property's value exceeds it's maximum length.
  /// </summary>
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
