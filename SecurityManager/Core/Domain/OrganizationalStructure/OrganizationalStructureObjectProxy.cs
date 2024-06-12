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
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  /// <summary>
  /// Base class for proxy objects used instead of the organizational structure domain objects.
  /// </summary>
  public abstract class OrganizationalStructureObjectProxy<T> : BindableObjectWithIdentityBase
      where T : BaseSecurityManagerObject
  {
    private readonly IDomainObjectHandle<T> _handle;
    private readonly string _uniqueIdentifier;
    private readonly string _displayName;

    protected OrganizationalStructureObjectProxy (IDomainObjectHandle<T> handle, string uniqueIdentifier, string displayName)
    {
      ArgumentUtility.CheckNotNull("handle", handle);
      ArgumentUtility.CheckNotNullOrEmpty("uniqueIdentifier", uniqueIdentifier);
      ArgumentUtility.CheckNotNullOrEmpty("displayName", displayName);

      _handle = handle;
      _uniqueIdentifier = uniqueIdentifier;
      _displayName = displayName;
    }

    public ObjectID ID
    {
      get { return _handle.ObjectID; }
    }

    public IDomainObjectHandle<T> Handle
    {
      get { return _handle; }
    }

    public override string UniqueIdentifier
    {
      get { return _uniqueIdentifier; }
    }

    public override string DisplayName
    {
      get { return _displayName; }
    }

    public override bool Equals (object? obj)
    {
      if (obj == null)
        return false;
      if (this.GetType() != obj.GetType())
        return false;
      return this._handle.Equals(((OrganizationalStructureObjectProxy<T>)obj)._handle);
    }

    public override int GetHashCode ()
    {
      return _handle.GetHashCode();
    }
  }
}
