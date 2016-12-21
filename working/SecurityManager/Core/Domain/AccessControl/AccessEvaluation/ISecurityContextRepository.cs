// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 

using System;
using JetBrains.Annotations;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.ServiceLocation;

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
    ReadOnlyCollectionDecorator<string> GetStatePropertyValues (IDomainObjectHandle<StatePropertyDefinition> stateProperty);
  }
}