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
  /// Proxy for the <see cref="Role"/> domain object class.
  /// </summary>
  /// <remarks>
  /// Used when a threadsafe representation of the domain object is required.
  /// </remarks>
  /// <threadsafety static="true" instance="true"/>
  public sealed class RoleProxy : OrganizationalStructureObjectProxy<Role>
  {
    public static RoleProxy Create (Role role)
    {
      ArgumentUtility.CheckNotNull("role", role);

      return new RoleProxy(
          role.GetHandle(),
          ((IBusinessObjectWithIdentity)role).UniqueIdentifier,
          ((IBusinessObjectWithIdentity)role).DisplayName);
    }

    private RoleProxy (IDomainObjectHandle<Role> handle, string uniqueIdentifier, string displayName)
      : base(handle, uniqueIdentifier, displayName)
    {
    }
  }
}
