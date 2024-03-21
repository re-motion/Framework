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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.TypePipe
{
  /// <summary>
  /// Implements <see cref="IClassDefinitionProvider"/> by retrieving the <see cref="ClassDefinition"/> of the domain object type from the current 
  /// mapping configuration.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  [ImplementationFor(typeof(IClassDefinitionProvider), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Single)]
  public class ClassDefinitionProvider : IClassDefinitionProvider
  {
    public ClassDefinition? GetClassDefinition (Type domainObjectType)
    {
      ArgumentUtility.CheckNotNull("domainObjectType", domainObjectType);

      var mappingConfiguration = MappingConfiguration.Current;
      if (!mappingConfiguration.ContainsClassDefinition(domainObjectType))
        return null;

      return mappingConfiguration.GetClassDefinition(domainObjectType);
    }
  }
}
