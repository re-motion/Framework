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
using Remotion.Collections;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.AclTools.Expansion.Infrastructure
{
  /// <summary>
  /// A tuple of (<see cref="User"/>, <see cref="Role"/>, <see cref="AccessControlList"/>, <see cref="AccessControlEntry"/>).
  /// Returned by the enumerator of <see cref="UserRoleAclAceCombinationFinder"/>.
  /// </summary>
  public class UserRoleAclAceCombination : IEquatable<UserRoleAclAceCombination>
  {
    private static readonly CompoundValueEqualityComparer<UserRoleAclAceCombination> _userRoleAclAceCombinationsComparer =
      new CompoundValueEqualityComparer<UserRoleAclAceCombination> (x => new object[] { x.Role, x.Ace });


    public UserRoleAclAceCombination (Role role, AccessControlEntry ace)
    {
      Role = role;
      Ace = ace;
    }

    public Role Role { get; private set; }
    public User User { get { return Role.User; } }
    public AccessControlEntry Ace { get; private set; }
    public AccessControlList Acl { get { return Ace.AccessControlList; } }

    public static CompoundValueEqualityComparer<UserRoleAclAceCombination> Comparer
    {
      get { return _userRoleAclAceCombinationsComparer; }
    }

    public override int GetHashCode ()
    {
      return _userRoleAclAceCombinationsComparer.GetHashCode(this);
    }

    public override bool Equals (object obj)
    {
      return _userRoleAclAceCombinationsComparer.Equals (this, obj);
    }

    public bool Equals (UserRoleAclAceCombination other)
    {
      return _userRoleAclAceCombinationsComparer.Equals (this, other);
    }
  }
}
