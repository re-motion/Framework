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

namespace Remotion.Data.DomainObjects.Validation
{
  public class MandatoryRelationNotSetException : DomainObjectValidationException
  {
    private readonly DomainObject? _domainObject;
    private readonly string? _propertyName;

    public MandatoryRelationNotSetException (DomainObject? domainObject, string? propertyName, string message)
        : this(domainObject, propertyName, message, null)
    {
    }

    public MandatoryRelationNotSetException (DomainObject? domainObject, string? propertyName, string message, Exception? inner)
        : base(message, inner)
    {
      _domainObject = domainObject;
      _propertyName = propertyName;
    }

    public DomainObject? DomainObject
    {
      get { return _domainObject; }
    }

    public string? PropertyName
    {
      get { return _propertyName; }
    }

    public override DomainObject[] AffectedObjects
    {
      get { return _domainObject == null ? new DomainObject[0] : new[] { _domainObject }; }
    }
  }
}
