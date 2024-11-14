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
using Remotion.Security;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.Domain
{
  /// <summary>
  /// The <see cref="ISecurityManagerPrincipal"/> interface is represent the current <see cref="Tenant"/>, <see cref="User"/>, 
  /// and optional <see cref="Substitution"/>. 
  /// See <see cref="SecurityManagerPrincipal"/> for the <see cref="SecurityManagerPrincipal.Current"/> instance.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Implementations of the interface must be threadsafe.
  /// </para>
  /// <para>
  /// To update the <see cref="ISecurityManagerPrincipal"/> with fresh data, use the <see cref="GetRefreshedInstance"/> method.
  /// </para>
  /// </remarks>
  /// <threadsafety static="true" instance="true"/>
  public interface ISecurityManagerPrincipal : INullObject
  {
    [CanBeNull]
    TenantProxy? Tenant { get; }

    [CanBeNull]
    UserProxy? User { get; }

    [CanBeNull]
    IReadOnlyList<RoleProxy>? Roles { get; }

    [CanBeNull]
    SubstitutionProxy? Substitution { get; }

    [NotNull]
    [MustUseReturnValue]
    ISecurityManagerPrincipal GetRefreshedInstance ();

    [NotNull]
    ISecurityPrincipal GetSecurityPrincipal ();

    [NotNull]
    TenantProxy[] GetTenants (bool includeAbstractTenants);

    [NotNull]
    SubstitutionProxy[] GetActiveSubstitutions ();
  }
}
