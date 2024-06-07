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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.Domain
{
  /// <summary>
  /// Declares a factory method for creating an instance for the <see cref="ISecurityManagerPrincipal"/> interface.
  /// </summary>
  /// <seealso cref="SecurityManagerPrincipalFactory"/>
 public interface ISecurityManagerPrincipalFactory
  {
    /// <summary>
    /// Instantiates a threadsafe implementation of the <see cref="ISecurityManagerPrincipal"/> interface.
    /// </summary>
    /// <param name="tenantHandle">An <see cref="IDomainObjectHandle{T}"/> for the <see cref="Tenant"/>. Must not be <see langword="null" />.</param>
    /// <param name="userHandle">An <see cref="IDomainObjectHandle{T}"/> for the <see cref="User"/>. Must not be <see langword="null" />.</param>
    /// <param name="substitutionHandle">An <see cref="IDomainObjectHandle{T}"/> for the <see cref="Substitution"/>.</param>
    /// <returns>A threadsafe implementation of the <see cref="ISecurityManagerPrincipal"/> interface.</returns>
    ISecurityManagerPrincipal Create (
        IDomainObjectHandle<Tenant> tenantHandle,
        IDomainObjectHandle<User> userHandle,
        IDomainObjectHandle<Substitution>? substitutionHandle);
  }
}
