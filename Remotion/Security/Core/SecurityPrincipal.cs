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
using System.Collections.Generic;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Security
{
  /// <summary>
  /// The <see cref="SecurityPrincipal"/> type represents a user, and optionally his active role and the user for whom he's acting as a substitue.
  /// </summary>
  public sealed class SecurityPrincipal : ISecurityPrincipal, IEquatable<SecurityPrincipal>
  {
    private readonly string _user;
    private readonly IReadOnlyList<ISecurityPrincipalRole>? _roles;
    private readonly string? _substitutedUser;
    private readonly IReadOnlyList<ISecurityPrincipalRole>? _substitutedRoles;

    public SecurityPrincipal (
        [NotNull] string user,
        [CanBeNull] IReadOnlyList<ISecurityPrincipalRole>? roles,
        [CanBeNull] string? substitutedUser,
        [CanBeNull] IReadOnlyList<ISecurityPrincipalRole>? substitutedRoles)
    {
      ArgumentUtility.CheckNotNullOrEmpty("user", user);
      ArgumentUtility.CheckNotEmpty("substitutedUser", substitutedUser);

      _user = user;
      _substitutedRoles = substitutedRoles;
      _substitutedUser = substitutedUser;
      _roles = roles;
    }

    public string User
    {
      get { return _user; }
    }

    public IReadOnlyList<ISecurityPrincipalRole>? Roles
    {
      get { return _roles; }
    }

    public string? SubstitutedUser
    {
      get { return _substitutedUser; }
    }

    public IReadOnlyList<ISecurityPrincipalRole>? SubstitutedRoles
    {
      get { return _substitutedRoles; }
    }

    public bool Equals (SecurityPrincipal? other)
    {
      if (ReferenceEquals(this, other))
        return true;

      if (other == null)
        return false;

      if (!string.Equals(this._user, other._user, StringComparison.Ordinal))
        return false;

      if (this._roles == null && other._roles != null)
        return false;

      if (this._roles != null && other._roles == null)
        return false;

      if (this._roles != null && other._roles != null)
      {
        if (this._roles.Count != other._roles.Count)
          return false;

        // ReSharper disable once LoopCanBeConvertedToQuery
        for (int i = 0; i < this._roles.Count; i++)
        {
          if (!IsRoleInList(this._roles[i], i, other._roles))
            return false;
        }
      }

      if (!string.Equals(this._substitutedUser, other._substitutedUser, StringComparison.Ordinal))
        return false;

      if (this._substitutedRoles == null && other._substitutedRoles != null)
        return false;

      if (this._substitutedRoles != null && other._substitutedRoles == null)
        return false;

      if (this._substitutedRoles != null && other._substitutedRoles != null)
      {
        if (this._substitutedRoles.Count != other._substitutedRoles.Count)
          return false;

        // ReSharper disable once LoopCanBeConvertedToQuery
        for (int i = 0; i < this._substitutedRoles.Count; i++)
        {
          if (!IsRoleInList(this._substitutedRoles[i], i, other._substitutedRoles))
            return false;
        }
      }

      return true;
    }

    private static bool IsRoleInList (ISecurityPrincipalRole roleToLookUp, int currentIndex, IReadOnlyList<ISecurityPrincipalRole> roles)
    {
      if (roleToLookUp.Equals(roles[currentIndex]))
        return true;

      // ReSharper disable once LoopCanBeConvertedToQuery
      // ReSharper disable once ForCanBeConvertedToForeach
      for (int j = 0; j < roles.Count; j++)
      {
        if (roleToLookUp.Equals(roles[j]))
          return true;
      }

      return false;
    }

    public override bool Equals (object? obj)
    {
      SecurityPrincipal? other = obj as SecurityPrincipal;
      if (other == null)
        return false;
      return ((IEquatable<SecurityPrincipal>)this).Equals(other);
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode(
          _user,
          // Skip roles, they don't do much for the hash code but require complex implemenetation.
          _substitutedUser
          // Skip roles, they don't do much for the hash code but require complex implemenetation.
          );
    }

    public bool IsNull
    {
      get { return false; }
    }
  }
}
