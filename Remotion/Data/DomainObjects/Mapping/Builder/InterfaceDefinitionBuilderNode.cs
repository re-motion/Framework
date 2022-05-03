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
  [DebuggerDisplay("Builder node for interface '{Type}'")]
  public class InterfaceDefinitionBuilderNode : TypeDefinitionBuilderNode
  {
    public IReadOnlyList<InterfaceDefinitionBuilderNode> ExtendedInterfaces { get; }

    public InterfaceDefinition? InterfaceDefinition { get; private set; }

    public bool IsConstructed { get; private set; }

    private readonly List<ClassDefinitionBuilderNode> _implementingClasses = new();
    private readonly List<InterfaceDefinitionBuilderNode> _extendingInterfaces = new();

    public InterfaceDefinitionBuilderNode (Type type, IEnumerable<InterfaceDefinitionBuilderNode> extendedInterfaces)
        : base(type)
    {
      ArgumentUtility.CheckNotNull("extendedInterfaces", extendedInterfaces);
      if (!type.IsInterface)
        throw new ArgumentException("The specified type must be an interface.", "type");
      if (type == typeof(IDomainObject))
        throw new ArgumentException("Cannot create a builder node for the IDomainObject type.", "type");
      if (!typeof(IDomainObject).IsAssignableFrom(type))
        throw new ArgumentException("Cannot create a builder node for a type that is not a domain object.", "type");

      var extendedInterfacesArray = extendedInterfaces.ToArray();
      if (extendedInterfacesArray.Any(e => e.IsConstructed))
        throw new ArgumentException("The specified extended interfaces must not be constructed.", "extendedInterfaces");

      ExtendedInterfaces = extendedInterfacesArray.Where(e => extendedInterfacesArray.All(f => !f.ImplementsInterface(e))).ToArray();

      foreach (var extendedInterface in ExtendedInterfaces)
        extendedInterface.AddExtendingInterface(this);
    }

    public IReadOnlyList<ClassDefinitionBuilderNode> ImplementingClasses => _implementingClasses;

    public IReadOnlyList<InterfaceDefinitionBuilderNode> ExtendingInterfaces => _extendingInterfaces;

    /// <inheritdoc />
    public override bool IsLeafNode => _implementingClasses.Count == 0 && _extendingInterfaces.Count == 0;

    /// <inheritdoc />
    internal override void BuildTypeDefinition (IMappingObjectFactory mappingObjectFactory)
    {
      ArgumentUtility.CheckNotNull("mappingObjectFactory", mappingObjectFactory);

      if (IsConstructed)
        throw new InvalidOperationException("Cannot build type definition as the builder node is already constructed.");

      if (InterfaceDefinition != null)
        return;

      var extendedInterfaces = new List<InterfaceDefinition>();
      foreach (var extendedInterface in ExtendedInterfaces)
      {
        extendedInterface.BuildTypeDefinition(mappingObjectFactory);
        if (extendedInterface.InterfaceDefinition != null)
          extendedInterfaces.Add(extendedInterface.InterfaceDefinition);
      }

      InterfaceDefinition = mappingObjectFactory.CreateInterfaceDefinition(Type, extendedInterfaces);
    }

    /// <inheritdoc />
    public override void EndBuildTypeDefinition ()
    {
      if (IsConstructed)
        return;

      IsConstructed = true;

      foreach (var extendedInterface in ExtendedInterfaces)
        extendedInterface.EndBuildTypeDefinition();
      foreach (var implementingClass in ImplementingClasses)
        implementingClass.EndBuildTypeDefinition();
      foreach (var extendingInterface in ExtendingInterfaces)
        extendingInterface.EndBuildTypeDefinition();

      if (InterfaceDefinition != null)
      {
        InterfaceDefinition.SetImplementingClasses(ImplementingClasses.Select(e => e.ClassDefinition).Where(e => e != null)!);
        InterfaceDefinition.SetExtendingInterfaces(ExtendingInterfaces.Select(e => e.InterfaceDefinition).Where(e => e != null)!);
      }
    }

    /// <inheritdoc />
    public override TypeDefinition? GetBuiltTypeDefinition ()
    {
      if (!IsConstructed)
        throw new InvalidOperationException("Cannot get the built type definition from an unconstructed builder node.");

      return InterfaceDefinition;
    }

    internal void AddImplementingClass (ClassDefinitionBuilderNode implementingClass)
    {
      ArgumentUtility.CheckNotNull("implementingClass", implementingClass);
      Assertion.DebugAssert(implementingClass.ImplementedInterfaces.Contains(this), "implementingClass.ImplementedInterfaces.Contains(this)");
      Assertion.DebugAssert(!implementingClass.IsConstructed, "!implementingClass.IsConstructed");
      Assertion.DebugAssert(!_implementingClasses.Contains(implementingClass), "!_implementingClasses.Contains(implementingClass)");

      _implementingClasses.Add(implementingClass);
    }

    private void AddExtendingInterface (InterfaceDefinitionBuilderNode extendingInterface)
    {
      Assertion.DebugAssert(extendingInterface.ExtendedInterfaces.Contains(this), "extendingInterface.ExtendedInterfaces.Contains(this)");
      Assertion.DebugAssert(!_extendingInterfaces.Contains(this), "!_extendingInterfaces.Contains(this)");

      _extendingInterfaces.Add(extendingInterface);
    }

    private bool ImplementsInterface (InterfaceDefinitionBuilderNode interfaceDefinitionBuilderNode)
    {
      if (ExtendedInterfaces.Contains(interfaceDefinitionBuilderNode))
        return true;

      return ExtendedInterfaces.Any(e => e.ImplementsInterface(interfaceDefinitionBuilderNode));
    }
  }
}
