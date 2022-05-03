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
using System.Linq;
using Remotion.Data.DomainObjects.Mapping.Builder;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// The <see cref="TypeDefinitionFactory"/> is used to get the list of <see cref="TypeDefinition"/>s for a set of types.
  /// It automatically sets the relationships between the different <see cref="TypeDefinition"/>s.
  /// </summary>
  public class TypeDefinitionFactory
  {
    private readonly IMappingObjectFactory _mappingObjectFactory;

    public TypeDefinitionFactory (IMappingObjectFactory mappingObjectFactory)
    {
      ArgumentUtility.CheckNotNull("mappingObjectFactory", mappingObjectFactory);

      _mappingObjectFactory = mappingObjectFactory;
    }

    public TypeDefinition[] CreateTypeDefinitions (IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull("types", types);

      var builderNodesLookup = new Dictionary<Type, TypeDefinitionBuilderNode>();

      foreach (var type in types)
        AddOrCreateBuilderNode(builderNodesLookup, type);

      foreach (var node in builderNodesLookup.Values)
        node.BeginBuildTypeDefinition(_mappingObjectFactory);

      foreach (var builderNode in builderNodesLookup.Values)
        builderNode.EndBuildTypeDefinition();

      return builderNodesLookup.Values
          .Select(e => e.GetBuiltTypeDefinition())
          .Where(e => e != null)
          .Select(e => e!)
          .OrderBy(e => e.Type.AssemblyQualifiedName)
          .ToArray();
    }

    private TypeDefinitionBuilderNode? AddOrCreateBuilderNode (
        Dictionary<Type, TypeDefinitionBuilderNode> builderNodesLookup,
        Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      if (builderNodesLookup.TryGetValue(type, out var existingBuilderNode))
        return existingBuilderNode;

      var newBuilderNode = CreateBuilderNode(builderNodesLookup, type);
      if (newBuilderNode != null)
        builderNodesLookup.Add(type, newBuilderNode);

      return newBuilderNode;
    }

    private TypeDefinitionBuilderNode? CreateBuilderNode (
        Dictionary<Type, TypeDefinitionBuilderNode> builderNodesLookup,
        Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      if (type.IsClass)
      {
        if (!ReflectionUtility.IsDomainObject(type) || type == typeof(DomainObject))
          return null;

        ClassDefinitionBuilderNode? baseClass = null;
        if (type.BaseType != null)
        {
          var baseType = type.BaseType.IsGenericType
              ? type.BaseType.GetGenericTypeDefinition()
              : type.BaseType;

          baseClass = (ClassDefinitionBuilderNode?)AddOrCreateBuilderNode(builderNodesLookup, baseType);
        }

        var implementedInterfaces = GetBuilderNodesForImplementedInterfaces(builderNodesLookup, type);
        return new ClassDefinitionBuilderNode(type, baseClass, implementedInterfaces);
      }
      else if (type.IsInterface)
      {
        if (!ReflectionUtility.IsDomainObject(type) || type == typeof(IDomainObject))
          return null;

        var implementedInterfaces = GetBuilderNodesForImplementedInterfaces(builderNodesLookup, type);
        return new InterfaceDefinitionBuilderNode(type, implementedInterfaces);
      }
      else
      {
        throw new InvalidOperationException($"Cannot create a builder node for type '{type}' because it is not a class-type.");
      }
    }

    private IEnumerable<InterfaceDefinitionBuilderNode> GetBuilderNodesForImplementedInterfaces (Dictionary<Type, TypeDefinitionBuilderNode> builderNodesLookup, Type type)
    {
      return type.GetInterfaces()
          .Select(e => AddOrCreateBuilderNode(builderNodesLookup, e))
          .Where(e => e != null)
          .Cast<InterfaceDefinitionBuilderNode>();
    }
  }
}
