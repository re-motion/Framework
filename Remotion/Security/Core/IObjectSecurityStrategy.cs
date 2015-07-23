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
  /// <summary>Encapsulates the security checks for the business object.</summary>
  /// <remarks>
  /// Typically the <see cref="IObjectSecurityStrategy"/> knows its business object (possibly indirectly) 
  /// and uses the security-relevant object state (as part of the <see cref="ISecurityContext"/>) as 
  /// parameter when evaluating the required permissions.
  /// <note type="inotes">Implementations are free to decide whether they provide caching.</note>
  /// </remarks>
  /// <seealso cref="ObjectSecurityStrategy"/>
  /// <seealso cref="InstanceBasedReEntrancyGuardedObjectSecurityStrategyDecorator"/>
  public interface IObjectSecurityStrategy
  {
    /// <summary>Determines whether the requested access is granted.</summary>
    /// <param name="securityProvider">The <see cref="ISecurityProvider"/> used to determine the permissions. Must not be <see langword="null" />. </param>
    /// <param name="principal">The <see cref="ISecurityPrincipal"/> on whose behalf the permissions are evaluated. Must not be <see langword="null" />.</param>
    /// <param name="requiredAccessTypes">The access rights required for the access to be granted. Must not be <see langword="null" />.</param>
    /// <returns><see langword="true"/> if the <paramref name="requiredAccessTypes"/> are granted. Must not be <see langword="null" /> or empty.</returns>
    /// <remarks>
    /// Typically called via <see cref="SecurityClient.HasAccess(ISecurableObject, AccessType[])"/> of <see cref="SecurityClient"/>.
    /// The strategy incorporates <see cref="ISecurityContext"/> in the permission query.
    /// The <paramref name="requiredAccessTypes"/> are determined by the <see cref="SecurityClient"/>, 
    /// taking the business object instance and the member (property or method) into account.
    /// </remarks>
    bool HasAccess (
        [NotNull] ISecurityProvider securityProvider,
        [NotNull] ISecurityPrincipal principal,
        [NotNull] IReadOnlyList<AccessType> requiredAccessTypes);
  }
}