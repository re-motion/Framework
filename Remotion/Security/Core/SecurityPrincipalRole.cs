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
  /// The <see cref="SecurityPrincipalRole"/> type defines a role the user can be in.
  /// </summary>
  public sealed class SecurityPrincipalRole : ISecurityPrincipalRole, IEquatable<SecurityPrincipalRole>
  {
    private readonly string _group;
    private readonly string _position;

    /// <summary>
    /// Initializes a new instance of the <see cref="SecurityPrincipalRole"/> type.
    /// </summary>
    /// <param name="group">The group the user is a member of when he is this role. Must not be <see langword="null" /> or empty.</param>
    /// <param name="position">The position in the <paramref name="group"/>. Must not be empty.</param>
    public SecurityPrincipalRole (string group, string position)
    {
      ArgumentUtility.CheckNotNullOrEmpty("group", group);
      ArgumentUtility.CheckNotEmpty("position", position);

      _group = group;
      _position = position;
    }

    public string Group
    {
      get { return _group; }
    }

    public string Position
    {
      get { return _position; }
    }

    bool IEquatable<SecurityPrincipalRole>.Equals (SecurityPrincipalRole? other)
    {
      if (other == null)
        return false;

      if (!string.Equals(this._group, other._group, StringComparison.Ordinal))
        return false;

      if (!string.Equals(this._position, other._position, StringComparison.Ordinal))
        return false;

      return true;
    }

    public override bool Equals (object? obj)
    {
      SecurityPrincipalRole? other = obj as SecurityPrincipalRole;
      if (other == null)
        return false;
      return ((IEquatable<SecurityPrincipalRole>)this).Equals(other);
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode(_group, _position);
    }
  }
}
