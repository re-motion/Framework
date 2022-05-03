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
using Remotion.FunctionalProgramming;
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
    public ClassDefinitionBuilderNode? BaseClass { get; }

    public IReadOnlyList<InterfaceDefinitionBuilderNode> ImplementedInterfaces { get; }

    public bool IsInheritanceRoot { get; }

    public ClassDefinition? ClassDefinition { get; private set; }

    public bool IsConstructed { get; private set; }

    private readonly List<ClassDefinitionBuilderNode> _derivedClasses = new();

    public ClassDefinitionBuilderNode (Type type, ClassDefinitionBuilderNode? baseClass, IEnumerable<InterfaceDefinitionBuilderNode> implementedInterfaces)
        : base(type)
    {
      ArgumentUtility.CheckNotNull("implementedInterfaces", implementedInterfaces);
      if (!type.IsClass)
        throw new ArgumentException("The specified type must be a class.", "type");
      if (type == typeof(DomainObject))
        throw new ArgumentException("Cannot create a builder node for the DomainObject type.", "type");
      if (!ReflectionUtility.IsDomainObject(type))
        throw new ArgumentException("Cannot create a builder node for a type that is not a domain object.", "type");
      if (baseClass?.IsConstructed == true)
        throw new ArgumentException("The specified base class must not be a constructed node.", "baseClass");

      var implementedInterfacesArray = implementedInterfaces.ToArray();
      if (implementedInterfacesArray.Any(e => e.IsConstructed))
        throw new ArgumentException("The specified implemented interfaces must not be constructed.", "implementedInterfaces");

      IsInheritanceRoot = ReflectionUtility.IsInheritanceRoot(type);

      BaseClass = baseClass;
      ImplementedInterfaces = IsInheritanceRoot || baseClass == null
          ? implementedInterfacesArray
          : implementedInterfacesArray.Where(e => !baseClass.ImplementsInterface(e)).ToArray();

      BaseClass?.AddDerivedClass(this);
      foreach (var implementedInterface in ImplementedInterfaces)
        implementedInterface.AddImplementingClass(this);
    }

    public IReadOnlyList<ClassDefinitionBuilderNode> DerivedClasses => _derivedClasses;

    /// <inheritdoc />
    public override bool IsLeafNode => DerivedClasses.Count == 0;

    /// <inheritdoc />
    internal override void BuildTypeDefinition (IMappingObjectFactory mappingObjectFactory)
    {
      ArgumentUtility.CheckNotNull("mappingObjectFactory", mappingObjectFactory);

      if (IsConstructed)
        throw new InvalidOperationException("Cannot build type definition as the builder node is already constructed.");

      if (ClassDefinition != null)
        return;

      ClassDefinition? baseClass = null;
      if (BaseClass != null && !IsInheritanceRoot)
      {
        BaseClass.BuildTypeDefinition(mappingObjectFactory);
        baseClass = BaseClass.ClassDefinition;
      }

      var implementedInterfaces = new List<InterfaceDefinition>();
      foreach (var implementedInterface in ImplementedInterfaces)
      {
        implementedInterface.BuildTypeDefinition(mappingObjectFactory);
        if (implementedInterface.InterfaceDefinition != null)
          implementedInterfaces.Add(implementedInterface.InterfaceDefinition);
      }

      ClassDefinition = mappingObjectFactory.CreateClassDefinition(Type, baseClass, implementedInterfaces);
    }

    /// <inheritdoc />
    public override void EndBuildTypeDefinition ()
    {
      if (IsConstructed)
        return;

      IsConstructed = true;

      BaseClass?.EndBuildTypeDefinition();
      foreach (var derivedClass in DerivedClasses)
        derivedClass.EndBuildTypeDefinition();
      foreach (var implementedInterface in ImplementedInterfaces)
        implementedInterface.EndBuildTypeDefinition();

      if (ClassDefinition != null)
        ClassDefinition.SetDerivedClasses(DerivedClasses.Select(e => e.ClassDefinition).Where(e => e?.BaseClass != null)!);
    }

    /// <inheritdoc />
    public override TypeDefinition? GetBuiltTypeDefinition ()
    {
      if (!IsConstructed)
        throw new InvalidOperationException("Cannot get the built type definition from an unconstructed builder node.");

      return ClassDefinition;
    }

    private void AddDerivedClass (ClassDefinitionBuilderNode derivedClass)
    {
      Assertion.DebugAssert(derivedClass.BaseClass == this, "derivedClass.BaseClass == this");
      Assertion.DebugAssert(!_derivedClasses.Contains(derivedClass), "!_derivedClasses.Contains(derivedClass)");

      _derivedClasses.Add(derivedClass);
    }

    private bool ImplementsInterface (InterfaceDefinitionBuilderNode interfaceDefinitionBuilderNode)
    {
      if (ImplementedInterfaces.Contains(interfaceDefinitionBuilderNode))
        return true;

      if (IsInheritanceRoot)
        return false;

      if (BaseClass != null)
        return BaseClass.ImplementsInterface(interfaceDefinitionBuilderNode);

      return false;
    }
  }
}
