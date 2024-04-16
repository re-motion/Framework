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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Implements a <see cref="ITypeDefinitionVisitor"/> that visits the type definitions in a depth-first manner.
  /// The direction of the walk can be specified using <see cref="TypeDefinitionWalkerDirection"/>.
  /// </summary>
  public abstract class TypeDefinitionWalker : ITypeDefinitionVisitor
  {
    private readonly TypeDefinitionWalkerDirection _direction;

    protected TypeDefinitionWalker (TypeDefinitionWalkerDirection direction)
    {
      _direction = direction;
    }

    /// <inheritdoc />
    public virtual void VisitClassDefinition (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);

      if (_direction == TypeDefinitionWalkerDirection.Descendants)
      {
        foreach (var derivedClass in classDefinition.DerivedClasses)
          derivedClass.Accept(this);
      }
      else
      {
        if (classDefinition.BaseClass != null)
          classDefinition.BaseClass.Accept(this);

        foreach (var implementedInterface in classDefinition.ImplementedInterfaces)
          implementedInterface.Accept(this);
      }
    }

    /// <inheritdoc />
    public virtual void VisitInterfaceDefinition (InterfaceDefinition interfaceDefinition)
    {
      ArgumentUtility.CheckNotNull("interfaceDefinition", interfaceDefinition);

      if (_direction == TypeDefinitionWalkerDirection.Descendants)
      {
        foreach (var implementingClass in interfaceDefinition.ImplementingClasses)
          implementingClass.Accept(this);

        foreach (var extendingInterface in interfaceDefinition.ExtendingInterfaces)
          extendingInterface.Accept(this);
      }
      else
      {
        foreach (var extendedInterface in interfaceDefinition.ExtendedInterfaces)
          extendedInterface.Accept(this);
      }
    }
  }
}
