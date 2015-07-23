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

namespace Remotion.Security
{
  /// <summary>Encapsulates the security checks.</summary>
  /// <remarks><note type="inotes">Implementations are free to decide whether they provide caching.</note></remarks>
  public interface ISecurityStrategy
  {
    /// <summary>Determines whether the requested access is granted.</summary>
    /// <param name="factory">The <see cref="ISecurityContextFactory"/> to be used.</param>
    /// <param name="securityProvider">The <see cref="ISecurityProvider"/> used to determine the permissions.</param>
    /// <param name="principal">The <see cref="ISecurityPrincipal"/> on whose behalf the permissions are evaluated.</param>
    /// <param name="requiredAccessTypes">The access rights required for the access to be granted.</param>
    /// <returns><see langword="true"/> if the <paramref name="requiredAccessTypes"/> are granted.</returns>
    /// <remarks>
    /// <note type="inotes">
    /// When caching is provided by the implementation, <see cref="ISecurityContextFactory.CreateSecurityContext"/> of the <paramref name="factory"/>
    /// shall only be called when the local cache does not already have a reference to a <see cref="ISecurityContext"/>.
    /// </note>
    /// </remarks>
    bool HasAccess (
        ISecurityContextFactory factory, ISecurityProvider securityProvider, ISecurityPrincipal principal, params AccessType[] requiredAccessTypes);

    /// <summary>Clears the cached access types of the <see cref="ISecurableObject"/> associated with this <see cref="ISecurityStrategy"/>.</summary>
    /// <remarks>Called by application code when <see cref="ISecurableObject"/> properties that are relevant for the <see cref="ISecurityContext"/> change.</remarks>
    void InvalidateLocalCache ();
  }
}
