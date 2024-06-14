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
using System.Collections;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  public class PrincipalRoleComparer : IEqualityComparer
  {
    private static readonly PrincipalRoleComparer s_principalRoleComparer = new PrincipalRoleComparer();

    public static PrincipalRoleComparer Instance
    {
      get { return s_principalRoleComparer; }
    }

    private PrincipalRoleComparer ()
    {
    }

    public new bool Equals (object x, object y)
    {
      if (x == null && y == null)
        return true;

      if (x == null || y == null)
        return false;

      if (x is Role)
        return Equals((Role)x, (PrincipalRole)y);
      else
        return Equals((Role)y, (PrincipalRole)x);
    }

    private bool Equals (Role role, PrincipalRole principalRole)
    {
      return DomainObjectHandleComparer.Instance.Equals(role.Position, principalRole.Position)
             && DomainObjectHandleComparer.Instance.Equals(role.Group, principalRole.Group);
    }

    public int GetHashCode (object obj)
    {
      return obj.GetHashCode();
    }
  }
}
