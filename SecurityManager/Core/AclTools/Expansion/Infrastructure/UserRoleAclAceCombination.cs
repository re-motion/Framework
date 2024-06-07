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
using Remotion.Collections;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion.Infrastructure
{
  /// <summary>
  /// A tuple of (<see cref="User"/>, <see cref="Role"/>, <see cref="AccessControlList"/>, <see cref="AccessControlEntry"/>).
  /// Returned by the enumerator of <see cref="UserRoleAclAceCombinationFinder"/>.
  /// </summary>
  public class UserRoleAclAceCombination : IEquatable<UserRoleAclAceCombination>
  {
    private static readonly CompoundValueEqualityComparer<UserRoleAclAceCombination> _userRoleAclAceCombinationsComparer =
      new CompoundValueEqualityComparer<UserRoleAclAceCombination>(x => new object[] { x.Role, x.Ace });


    public UserRoleAclAceCombination (Role role, AccessControlEntry ace)
    {
      ArgumentUtility.CheckNotNull("role", role);
      ArgumentUtility.CheckNotNull("ace", ace);
      if (role.User == null)
        throw new ArgumentException("Role must have a User set.", "role");
      if (ace.AccessControlList == null)
        throw new ArgumentException("AccessControlEntry must have an AccessControlList set.", "ace");

      Role = role;
      Ace = ace;
    }

    public Role Role { get; }

    public User User
    {
      get
      {
        Assertion.DebugIsNotNull(Role.User, "Role.User != null");
        return Role.User;
      }
    }

    public AccessControlEntry Ace { get; }

    public AccessControlList Acl
    {
      get
      {
        Assertion.DebugIsNotNull(Ace.AccessControlList, "Ace.AccessControlList != null");
        return Ace.AccessControlList;
      }
    }

    public static CompoundValueEqualityComparer<UserRoleAclAceCombination> Comparer
    {
      get { return _userRoleAclAceCombinationsComparer; }
    }

    public override int GetHashCode ()
    {
      return _userRoleAclAceCombinationsComparer.GetHashCode(this);
    }

    public override bool Equals (object? obj)
    {
      return _userRoleAclAceCombinationsComparer.Equals(this, obj);
    }

    public bool Equals (UserRoleAclAceCombination? other)
    {
      return _userRoleAclAceCombinationsComparer.Equals(this, other);
    }
  }
}
