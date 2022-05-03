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
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Builder
{
  /// <summary>
  /// Represents <see cref="Mapping.InterfaceDefinition"/> node in a tree constructed by <see cref="TypeDefinitionFactory"/>.
  /// Supports child navigation and correctly setting up child relationships in a resulting <see cref="Mapping.InterfaceDefinition"/>.
  /// </summary>
  [DebuggerDisplay("Builder node for interface '{Type}'")]
  public class InterfaceDefinitionBuilderNode : TypeDefinitionBuilderNode
  {
    public IReadOnlyList<InterfaceDefinitionBuilderNode> ExtendedInterfaceNodes { get; }

    public InterfaceDefinition? InterfaceDefinition { get; private set; }

    public bool IsConstructed { get; private set; }

    private readonly List<ClassDefinitionBuilderNode> _implementingClassNodes = new();
    private readonly List<InterfaceDefinitionBuilderNode> _extendingInterfaceNodes = new();

    public InterfaceDefinitionBuilderNode (Type type, IEnumerable<InterfaceDefinitionBuilderNode> extendedInterfaceNodes)
        : base(type)
    {
      ArgumentUtility.CheckNotNull("extendedInterfaceNodes", extendedInterfaceNodes);
      if (!type.IsInterface)
        throw new ArgumentException("The specified type must be an interface.", "type");
      if (type == typeof(IDomainObject))
        throw new ArgumentException($"The specified type must not be the '{nameof(IDomainObject)}' interface.", "type");
      if (!typeof(IDomainObject).IsAssignableFrom(type))
        throw new ArgumentException($"The specified type must implement '{nameof(IDomainObject)}'.", "type");

      var extendedInterfaceNodeArray = extendedInterfaceNodes.ToArray();
      if (extendedInterfaceNodeArray.Any(e => e.IsConstructed))
        throw new ArgumentException("The specified extended interfaces must not be constructed.", "extendedInterfaceNodes");

      ExtendedInterfaceNodes = extendedInterfaceNodeArray.Where(e => extendedInterfaceNodeArray.All(f => !f.ImplementsInterface(e))).ToArray();

      foreach (var extendedInterfaceNode in ExtendedInterfaceNodes)
        extendedInterfaceNode.AddExtendingInterface(this);
    }

    public IReadOnlyList<ClassDefinitionBuilderNode> ImplementingClassNodes => _implementingClassNodes;

    public IReadOnlyList<InterfaceDefinitionBuilderNode> ExtendingInterfaceNodes => _extendingInterfaceNodes;

    /// <inheritdoc />
    public override bool IsLeafNode => _implementingClassNodes.Count == 0 && _extendingInterfaceNodes.Count == 0;

    /// <inheritdoc />
    protected override void BuildTypeDefinition (IMappingObjectFactory mappingObjectFactory)
    {
      ArgumentUtility.CheckNotNull("mappingObjectFactory", mappingObjectFactory);

      if (IsConstructed)
        throw new InvalidOperationException("Cannot build type definition as the builder node is already constructed.");

      if (InterfaceDefinition != null)
        return;

      var extendedInterfaceNodes = new List<InterfaceDefinition>();
      foreach (var extendedInterfaceNode in ExtendedInterfaceNodes)
      {
        extendedInterfaceNode.BuildTypeDefinition(mappingObjectFactory);
        if (extendedInterfaceNode.InterfaceDefinition != null)
          extendedInterfaceNodes.Add(extendedInterfaceNode.InterfaceDefinition);
      }

      InterfaceDefinition = mappingObjectFactory.CreateInterfaceDefinition(Type, extendedInterfaceNodes);
    }

    /// <inheritdoc />
    public override void EndBuildTypeDefinition ()
    {
      if (IsConstructed)
        return;

      IsConstructed = true;

      foreach (var extendedInterfaceNode in ExtendedInterfaceNodes)
        extendedInterfaceNode.EndBuildTypeDefinition();
      foreach (var implementingClassNode in ImplementingClassNodes)
        implementingClassNode.EndBuildTypeDefinition();
      foreach (var extendingInterfaceNode in ExtendingInterfaceNodes)
        extendingInterfaceNode.EndBuildTypeDefinition();

      if (InterfaceDefinition != null)
      {
        InterfaceDefinition.SetImplementingClasses(ImplementingClassNodes.Select(e => e.ClassDefinition).Where(e => e != null)!);
        InterfaceDefinition.SetExtendingInterfaces(ExtendingInterfaceNodes.Select(e => e.InterfaceDefinition).Where(e => e != null)!);
      }
    }

    /// <inheritdoc />
    public override TypeDefinition? GetBuiltTypeDefinition ()
    {
      if (!IsConstructed)
        throw new InvalidOperationException("Cannot get the built type definition from an unconstructed builder node.");

      return InterfaceDefinition;
    }

    internal void AddImplementingClass (ClassDefinitionBuilderNode implementingClassNode)
    {
      ArgumentUtility.CheckNotNull("implementingClassNode", implementingClassNode);
      Assertion.DebugAssert(implementingClassNode.ImplementedInterfaceNodes.Contains(this), "implementingClassNode.ImplementedInterfaces.Contains(this)");
      Assertion.DebugAssert(!implementingClassNode.IsConstructed, "!implementingClassNode.IsConstructed");
      Assertion.DebugAssert(!_implementingClassNodes.Contains(implementingClassNode), "!_implementingClassesNodes.Contains(implementingClass)");

      _implementingClassNodes.Add(implementingClassNode);
    }

    private void AddExtendingInterface (InterfaceDefinitionBuilderNode extendingInterfaceNode)
    {
      Assertion.DebugAssert(extendingInterfaceNode.ExtendedInterfaceNodes.Contains(this), "extendingInterfaceNode.ExtendedInterfaces.Contains(this)");
      Assertion.DebugAssert(!_extendingInterfaceNodes.Contains(this), "!_extendingInterfacesNodes.Contains(this)");

      _extendingInterfaceNodes.Add(extendingInterfaceNode);
    }

    private bool ImplementsInterface (InterfaceDefinitionBuilderNode interfaceNode)
    {
      if (ExtendedInterfaceNodes.Contains(interfaceNode))
        return true;

      return ExtendedInterfaceNodes.Any(e => e.ImplementsInterface(interfaceNode));
    }
  }
}
