// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
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
  [Serializable]
  public abstract class OrganizationalStructureObjectProxy<T> : BindableObjectWithIdentityBase
      where T : BaseSecurityManagerObject
  {
    private readonly IDomainObjectHandle<T> _handle;
    private readonly string _uniqueIdentifier;
    private readonly string _displayName;

    protected OrganizationalStructureObjectProxy (IDomainObjectHandle<T> handle, string uniqueIdentifier, string displayName)
    {
      ArgumentUtility.CheckNotNull ("handle", handle);
      ArgumentUtility.CheckNotNullOrEmpty ("uniqueIdentifier", uniqueIdentifier);
      ArgumentUtility.CheckNotNullOrEmpty ("displayName", displayName);

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

    public override bool Equals (object obj)
    {
      if (obj == null)
        return false;
      if (this.GetType() != obj.GetType())
        return false;
      return this._handle.Equals (((OrganizationalStructureObjectProxy<T>) obj)._handle);
    }

    public override int GetHashCode ()
    {
      return _handle.GetHashCode();
    }
  }
}