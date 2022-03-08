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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Builder
{
  /// <summary>
  /// Represents <see cref="Mapping.ClassDefinition"/> node in a tree constructed by <see cref="TypeDefinitionFactory"/>.
  /// Supports child navigation and correctly setting up child relationships in a resulting <see cref="Mapping.ClassDefinition"/>.
  /// </summary>
  public class ClassDefinitionBuilderNode : TypeDefinitionBuilderNode
  {
    public ClassDefinitionBuilderNode? BaseClass { get; }

    public ClassDefinition? ClassDefinition { get; private set; }

    public bool IsConstructed { get; private set; }

    private readonly List<ClassDefinitionBuilderNode> _derivedClasses = new();

    public ClassDefinitionBuilderNode (Type type, ClassDefinitionBuilderNode? baseClass)
        : base(type)
    {
      if (!type.IsClass)
        throw new ArgumentException("The specified type must be a class.", "type");
      if (baseClass?.IsConstructed == true)
        throw new ArgumentException("The specified base class must not be a constructed node.", "baseClass");

      BaseClass = baseClass;
      BaseClass?.AddDerivedClass(this);
    }

    public IReadOnlyList<ClassDefinitionBuilderNode> DerivedClasses => _derivedClasses;

    /// <inheritdoc />
    public override bool IsLeafNode => DerivedClasses.Count == 0;

    /// <inheritdoc />
    protected override void BuildTypeDefinition (IMappingObjectFactory mappingObjectFactory)
    {
      ArgumentUtility.CheckNotNull("mappingObjectFactory", mappingObjectFactory);

      if (IsConstructed)
        throw new InvalidOperationException("Cannot build type definition as the builder node is already constructed.");

      if (ClassDefinition != null)
        return;

      ClassDefinition? baseClass = null;
      if (BaseClass != null && !ReflectionUtility.IsInheritanceRoot(Type))
      {
        BaseClass.BuildTypeDefinition(mappingObjectFactory);
        baseClass = BaseClass.ClassDefinition;
      }

      ClassDefinition = mappingObjectFactory.CreateClassDefinition(Type, baseClass);
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
  }
}
