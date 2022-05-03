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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Builder
{
  /// <summary>
  /// Represents <see cref="Mapping.ClassDefinition"/> node in a tree constructed by <see cref="TypeDefinitionFactory"/>.
  /// Supports child navigation and correctly setting up child relationships in a resulting <see cref="Mapping.ClassDefinition"/>.
  /// </summary>
  [DebuggerDisplay("Builder node for class '{Type}'")]
  public class ClassDefinitionBuilderNode : TypeDefinitionBuilderNode
  {
    public ClassDefinitionBuilderNode? BaseClassNode { get; }

    public IReadOnlyList<InterfaceDefinitionBuilderNode> ImplementedInterfaceNodes { get; }

    [MemberNotNullWhen(false, nameof(BaseClassNode))]
    public bool IsInheritanceRoot { get; }

    public ClassDefinition? ClassDefinition { get; private set; }

    public bool IsConstructed { get; private set; }

    private readonly List<ClassDefinitionBuilderNode> _derivedClassNodes = new();

    public ClassDefinitionBuilderNode (Type type, ClassDefinitionBuilderNode? baseClassNode, IEnumerable<InterfaceDefinitionBuilderNode> implementedInterfaceNodes)
        : base(type)
    {
      ArgumentUtility.CheckNotNull("implementedInterfaceNodes", implementedInterfaceNodes);
      if (!type.IsClass)
        throw new ArgumentException("The specified type must be a class.", "type");
      if (type == typeof(DomainObject))
        throw new ArgumentException($"The specified type must not be the '{nameof(DomainObject)}' class.", "type");
      if (!ReflectionUtility.IsDomainObject(type))
        throw new ArgumentException($"The specified type must be derived from '{nameof(DomainObject)}'.", "type");
      if (baseClassNode?.IsConstructed == true)
        throw new ArgumentException("The specified base class node must not be a constructed.", "baseClassNode");

      var implementedInterfaceNodeArray = implementedInterfaceNodes.ToArray();
      if (implementedInterfaceNodeArray.Any(e => e.IsConstructed))
        throw new ArgumentException("The specified implemented interface nodes must not be constructed.", "implementedInterfaceNodes");

      IsInheritanceRoot = ReflectionUtility.IsInheritanceRoot(type) || baseClassNode == null;

      BaseClassNode = baseClassNode;
      ImplementedInterfaceNodes = IsInheritanceRoot
          ? implementedInterfaceNodeArray
          : implementedInterfaceNodeArray.Where(e => !BaseClassNode.ImplementsInterface(e)).ToArray();

      BaseClassNode?.AddDerivedClass(this);
      foreach (var implementedInterface in ImplementedInterfaceNodes)
        implementedInterface.AddImplementingClass(this);
    }

    public IReadOnlyList<ClassDefinitionBuilderNode> DerivedClassNodes => _derivedClassNodes;

    /// <inheritdoc />
    public override bool IsLeafNode => DerivedClassNodes.Count == 0;

    /// <inheritdoc />
    protected override void BuildTypeDefinition (IMappingObjectFactory mappingObjectFactory)
    {
      ArgumentUtility.CheckNotNull("mappingObjectFactory", mappingObjectFactory);

      if (IsConstructed)
        throw new InvalidOperationException("Cannot build type definition as the builder node is already constructed.");

      if (ClassDefinition != null)
        return;

      ClassDefinition? baseClassNode = null;
      if (!IsInheritanceRoot)
      {
        BaseClassNode.BuildTypeDefinition(mappingObjectFactory);
        baseClassNode = BaseClassNode.ClassDefinition;
      }

      var implementedInterfaceNodes = new List<InterfaceDefinition>();
      foreach (var implementedInterfaceNode in ImplementedInterfaceNodes)
      {
        BuildTypeDefinition(implementedInterfaceNode, mappingObjectFactory);
        if (implementedInterfaceNode.InterfaceDefinition != null)
          implementedInterfaceNodes.Add(implementedInterfaceNode.InterfaceDefinition);
      }

      ClassDefinition = mappingObjectFactory.CreateClassDefinition(Type, baseClassNode, implementedInterfaceNodes);
    }

    /// <inheritdoc />
    public override void EndBuildTypeDefinition ()
    {
      if (IsConstructed)
        return;

      IsConstructed = true;

      BaseClassNode?.EndBuildTypeDefinition();
      foreach (var derivedClassNode in DerivedClassNodes)
        derivedClassNode.EndBuildTypeDefinition();
      foreach (var implementedInterfaceNode in ImplementedInterfaceNodes)
        implementedInterfaceNode.EndBuildTypeDefinition();

      if (ClassDefinition != null)
        ClassDefinition.SetDerivedClasses(DerivedClassNodes.Select(e => e.ClassDefinition).Where(e => e?.BaseClass != null)!);
    }

    /// <inheritdoc />
    public override TypeDefinition? GetBuiltTypeDefinition ()
    {
      if (!IsConstructed)
        throw new InvalidOperationException("Cannot get the built type definition from an unconstructed builder node.");

      return ClassDefinition;
    }

    private void AddDerivedClass (ClassDefinitionBuilderNode derivedClassNode)
    {
      Assertion.DebugAssert(derivedClassNode.BaseClassNode == this, "derivedClassNode.BaseClass == this");
      Assertion.DebugAssert(!_derivedClassNodes.Contains(derivedClassNode), "!_derivedClassesNodes.Contains(derivedClass)");

      _derivedClassNodes.Add(derivedClassNode);
    }

    private bool ImplementsInterface (InterfaceDefinitionBuilderNode interfaceNode)
    {
      if (ImplementedInterfaceNodes.Contains(interfaceNode))
        return true;

      if (IsInheritanceRoot)
        return false;

      return BaseClassNode.ImplementsInterface(interfaceNode);
    }
  }
}
