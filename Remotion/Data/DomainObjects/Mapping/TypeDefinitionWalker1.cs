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
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Implements a <see cref="ITypeDefinitionVisitor{T}"/> that visits the type definitions in a depth-first manner,
  /// aborting the walk and returning the first value of <typeparamref name="T"/> that fulfills a specified predicate.
  /// </summary>
  public abstract class TypeDefinitionWalker<T> : ITypeDefinitionVisitor<T>
  {
    protected readonly TypeDefinitionWalkerDirection Direction;
    protected readonly Func<T?, bool> Match;

    protected TypeDefinitionWalker (TypeDefinitionWalkerDirection direction, Predicate<T?> match)
    {
      ArgumentUtility.CheckNotNull("match", match);

      Direction = direction;
      Match = new Func<T?, bool>(match);
    }

    /// <inheritdoc />
    public virtual T? VisitClassDefinition (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);

      if (Direction == TypeDefinitionWalkerDirection.Descendants)
      {
        return classDefinition.DerivedClasses
            .Select(cd => cd.Accept(this))
            .FirstOrDefault(Match);
      }
      else
      {
        if (classDefinition.BaseClass != null)
        {
          var result = classDefinition.BaseClass.Accept(this);
          if (Match(result))
            return result;
        }

        return classDefinition.ImplementedInterfaces
            .Select(id => id.Accept(this))
            .FirstOrDefault(Match);
      }
    }

    /// <inheritdoc />
    public virtual T? VisitInterfaceDefinition (InterfaceDefinition interfaceDefinition)
    {
      ArgumentUtility.CheckNotNull("interfaceDefinition", interfaceDefinition);

      if (Direction == TypeDefinitionWalkerDirection.Descendants)
      {
        return interfaceDefinition.ImplementingClasses
            .Cast<TypeDefinition>()
            .Concat(interfaceDefinition.ExtendingInterfaces)
            .Select(td => td.Accept(this))
            .FirstOrDefault(Match);
      }
      else
      {
        return interfaceDefinition.ExtendedInterfaces
            .Select(id => id.Accept(this))
            .FirstOrDefault(Match);
      }
    }
  }
}
