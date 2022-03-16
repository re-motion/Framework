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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.MappingReflectionIntegrationTests
{
  public class ReferenceTypeDefinitionBuilderContext
  {
    private readonly IDictionary<Type, IReferenceTypeDefinitionBuilder> _builders;

    private readonly Dictionary<Type, TypeDefinition> _buildTypeDefinitions = new();
    private readonly HashSet<Type> _currentlyResolving = new();

    public ReferenceTypeDefinitionBuilderContext (IDictionary<Type, IReferenceTypeDefinitionBuilder> builders)
    {
      ArgumentUtility.CheckNotNull("builders", builders);

      _builders = builders;
    }

    public ClassDefinition ResolveClassDefinition (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);
      if (!type.IsClass)
        throw new InvalidOperationException("The specified type must be an class.");

      return (ClassDefinition)ResolveTypeDefinition(type);
    }

    public InterfaceDefinition ResolveInterfaceDefinition (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);
      if (!type.IsInterface)
        throw new InvalidOperationException("The specified type must be an interface.");

      return (InterfaceDefinition)ResolveTypeDefinition(type);
    }

    public TypeDefinition ResolveTypeDefinition (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);
      if (_currentlyResolving.Contains(type))
        throw new InvalidOperationException($"Recursive lookup detected while resolving type '{type.GetFullNameSafe()}'");

      if (_buildTypeDefinitions.TryGetValue(type, out var builtTypeDefinition))
        return builtTypeDefinition;

      if (!_builders.TryGetValue(type, out var builder))
        throw new InvalidOperationException($"Cannot resolve type definition for type '{type.GetFullNameSafe()}' as there is no builder registered.");

      TypeDefinition typeDefinition;
      try
      {
        _currentlyResolving.Add(type);

        typeDefinition = builder.BuildTypeDefinition(this);
        _buildTypeDefinitions.Add(type, typeDefinition);
      }
      finally
      {
        _currentlyResolving.Remove(type);
      }

      return typeDefinition;
    }
  }
}
