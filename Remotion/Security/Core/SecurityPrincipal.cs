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

namespace Remotion.Security
{
  /// <summary>
  /// The <see cref="SecurityPrincipal"/> type represents a user, and optionally his active role and the user for whom he's acting as a substitue.
  /// </summary>
  [Serializable]
  public sealed class SecurityPrincipal : ISecurityPrincipal, IEquatable<SecurityPrincipal>
  {
    private readonly string _user;
    private readonly ISecurityPrincipalRole _role;
    private readonly string _substitutedUser;
    private readonly ISecurityPrincipalRole _substitutedRole;

    public SecurityPrincipal (string user, ISecurityPrincipalRole role, string substitutedUser, ISecurityPrincipalRole substitutedRole)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("user", user);
      ArgumentUtility.CheckNotEmpty ("substitutedUser", substitutedUser);
      if (substitutedRole != null && substitutedUser == null)
        throw new ArgumentException ("The substituted user must be specified if a substituted role is also specified.", "substitutedUser");

      _user = user;
      _substitutedRole = substitutedRole;
      _substitutedUser = substitutedUser;
      _role = role;
    }

    public string User
    {
      get { return _user; }
    }

    public ISecurityPrincipalRole Role
    {
      get { return _role; }
    }

    public string SubstitutedUser
    {
      get { return _substitutedUser; }
    }

    public ISecurityPrincipalRole SubstitutedRole
    {
      get { return _substitutedRole; }
    }

    public bool Equals (SecurityPrincipal other)
    {
      if (ReferenceEquals (this, other))
        return true;

      if (other == null)
        return false;

      if (!string.Equals (this._user, other._user, StringComparison.Ordinal))
        return false;

      if ((this._role == null && other._role != null) || (this._role != null && !this._role.Equals (other._role)))
        return false;

      if (!string.Equals (this._substitutedUser, other._substitutedUser, StringComparison.Ordinal))
        return false;

      if ((this._substitutedRole == null && other._substitutedRole != null) || (this._substitutedRole != null && !this._substitutedRole.Equals (other._substitutedRole)))
        return false;

      return true;
    }

    public override bool Equals (object obj)
    {
      SecurityPrincipal other = obj as SecurityPrincipal;
      if (other == null)
        return false;
      return ((IEquatable<SecurityPrincipal>) this).Equals (other);
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (_user, _role, _substitutedUser, _substitutedRole);
    }

    public bool IsNull
    {
      get { return false; }
    }
  }
}
