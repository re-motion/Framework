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
using JetBrains.Annotations;

namespace Remotion.Security
{
  /// <summary>Provides access to the permission management functionality.</summary>
  /// <remarks>This service interface enables a plugable security system architecture, acting as single point of access to the permission management functionality.</remarks>
  public interface ISecurityProvider : INullObject
  {
    /// <summary>Determines permission for a user.</summary>
    /// <param name="context">The <see cref="ISecurityContext"/> gouping all object-specific security information of the current permission check.</param>
    /// <param name="principal">The <see cref="ISecurityPrincipal"/> on whose behalf the permissions are evaluated.</param>
    /// <returns>An array of <see cref="AccessType"/>s.</returns>
    [NotNull]
    AccessType[] GetAccess (ISecurityContext context, ISecurityPrincipal principal);
  }
}
