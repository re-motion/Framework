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

namespace Remotion.Security
{
  /// <summary>
  /// The <see cref="ISecurityPrincipal"/> interface represents a user, and optionally his active role and the user for whom he's acting as a substitue.
  /// </summary>
  public interface ISecurityPrincipal : INullObject
  {
    /// <summary>
    /// Gets the name of the user. The <see cref="User"/> must always be specified.
    /// </summary>
    [CanBeNull]
    string? User { get; }

    /// <summary>
    /// Gets the collection of active roles of the user. The <see cref="Roles"/> collection is optional and can be empty.
    /// </summary>
    [CanBeNull]
    IReadOnlyList<ISecurityPrincipalRole>? Roles { get; }

    /// <summary>
    /// Gets the name of the user that is being substitued. 
    /// The <see cref="SubstitutedUser"/> must be specified if a <see cref="SubstitutedRoles"/> are specified.
    /// </summary>
    [CanBeNull]
    string? SubstitutedUser { get; }

    /// <summary>
    /// Gets the collection of active roles of the user. The <see cref="Roles"/> collection is optional and can be empty.
    /// </summary>
    [CanBeNull]
    IReadOnlyList<ISecurityPrincipalRole>? SubstitutedRoles { get; }
  }
}