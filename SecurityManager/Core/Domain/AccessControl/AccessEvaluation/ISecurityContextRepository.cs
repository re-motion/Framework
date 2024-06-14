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
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation
{
  /// <summary>
  /// Defines the API required for resolving the information provided via the <see cref="ISecurityContext"/>.
  /// </summary>
  /// <seealso cref="SecurityContextRepository"/>
  /// <threadsafety static="true" instance="true"/>
  public interface ISecurityContextRepository
  {
    [NotNull]
    IDomainObjectHandle<Tenant> GetTenant (string uniqueIdentifier);

    [NotNull]
    IDomainObjectHandle<Group> GetGroup (string uniqueIdentifier);

    [NotNull]
    IDomainObjectHandle<User> GetUser (string userName);

    [NotNull]
    IDomainObjectHandle<Position> GetPosition (string uniqueIdentifier);

    [NotNull]
    IDomainObjectHandle<AbstractRoleDefinition> GetAbstractRole (EnumWrapper name);

    [NotNull]
    SecurableClassDefinitionData GetClass (string name);

    [NotNull]
    IReadOnlyCollection<string> GetStatePropertyValues (IDomainObjectHandle<StatePropertyDefinition> stateProperty);
  }
}
