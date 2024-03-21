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
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Provides methods for getting the hierarchy of a <see cref="TypeDefinition"/>.
  /// </summary>
  public static class TypeDefinitionHierarchy
  {
    /// <summary>
    /// Checks if the two <see cref="TypeDefinition"/>s are part of the same hierarchy, sharing a common ascendant.
    /// </summary>
    public static bool ArePartOfSameHierarchy (TypeDefinition left, TypeDefinition right)
    {
      ArgumentUtility.CheckNotNull("left", left);
      ArgumentUtility.CheckNotNull("right", right);

      if (ReferenceEquals(left, right))
        return true;

      var typeHierarchyCollector = new TypeHierarchyCollector(TypeDefinitionWalkerDirection.Ascendants);
      left.Accept(typeHierarchyCollector);

      var leftTypeHierarchy = typeHierarchyCollector.ToHashSet();
      return InlineTypeDefinitionWalker.WalkAncestors(
          right,
          classDefinition => leftTypeHierarchy.Contains(classDefinition),
          interfaceDefinition => leftTypeHierarchy.Contains(interfaceDefinition),
          match: result => result);
    }

    /// <summary>
    /// Returns the hierarchy roots of the specified <paramref name="typeDefinitions"/>, using a depth-first approach.
    /// </summary>
    public static IReadOnlyList<TypeDefinition> GetHierarchyRoots (IEnumerable<TypeDefinition> typeDefinitions)
    {
      ArgumentUtility.CheckNotNull("typeDefinitions", typeDefinitions);

      var inheritanceRootsCollector = new InheritanceRootsCollector();
      foreach (var typeDefinition in typeDefinitions)
      {
        typeDefinition.Accept(inheritanceRootsCollector);
      }

      return inheritanceRootsCollector.ToArray();
    }

    /// <summary>
    /// Returns the hierarchy (self and descendants) of the specified <paramref name="typeDefinition"/>, using a depth-first approach.
    /// </summary>
    /// <remarks>
    /// The hierarchy includes the specified <paramref name="typeDefinition"/> as the first element.
    /// </remarks>
    public static IReadOnlyList<TypeDefinition> GetDescendantsAndSelf (TypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      var typeHierarchyCollector = new TypeHierarchyCollector(TypeDefinitionWalkerDirection.Descendants);
      typeDefinition.Accept(typeHierarchyCollector);

      return typeHierarchyCollector.ToArray();
    }

    private class InheritanceRootsCollector : TypeDefinitionWalker
    {
      private readonly HashSet<TypeDefinition> _visitedTypeDefinitions = new();
      private readonly List<TypeDefinition> _collectedTypeDefinitions = new();

      public IReadOnlyList<TypeDefinition> ToArray () => _collectedTypeDefinitions.ToArray();

      public InheritanceRootsCollector ()
          : base(TypeDefinitionWalkerDirection.Ascendants)
      {
      }

      /// <inheritdoc />
      public override void VisitClassDefinition (ClassDefinition classDefinition)
      {
        if (!_visitedTypeDefinitions.Add(classDefinition))
          return;

        if (classDefinition.BaseClass == null)
          _collectedTypeDefinitions.Add(classDefinition);

        base.VisitClassDefinition(classDefinition);
      }

      /// <inheritdoc />
      public override void VisitInterfaceDefinition (InterfaceDefinition interfaceDefinition)
      {
        if (!_visitedTypeDefinitions.Add(interfaceDefinition))
          return;

        if (interfaceDefinition.ExtendedInterfaces.Count == 0)
          _collectedTypeDefinitions.Add(interfaceDefinition);

        base.VisitInterfaceDefinition(interfaceDefinition);
      }
    }

    private class TypeHierarchyCollector : TypeDefinitionWalker
    {
      private readonly HashSet<TypeDefinition> _visitedTypeDefinitions = new();
      private readonly List<TypeDefinition> _collectedTypeDefinitions = new();

      public IReadOnlyList<TypeDefinition> ToArray () => _collectedTypeDefinitions.ToArray();

      public HashSet<TypeDefinition> ToHashSet () => new(_collectedTypeDefinitions);

      public TypeHierarchyCollector (TypeDefinitionWalkerDirection direction)
          : base(direction)
      {
      }

      /// <inheritdoc />
      public override void VisitClassDefinition (ClassDefinition classDefinition)
      {
        if (!_visitedTypeDefinitions.Add(classDefinition))
          return;

        _collectedTypeDefinitions.Add(classDefinition);
        base.VisitClassDefinition(classDefinition);
      }

      /// <inheritdoc />
      public override void VisitInterfaceDefinition (InterfaceDefinition interfaceDefinition)
      {
        if (!_visitedTypeDefinitions.Add(interfaceDefinition))
          return;

        _collectedTypeDefinitions.Add(interfaceDefinition);
        base.VisitInterfaceDefinition(interfaceDefinition);
      }
    }
  }
}
