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
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.Mapping.Validation.Reflection
{
  /// <summary>
  /// Validates that the type of a class defintion is derived from base type.
  /// </summary>
  public class InheritanceHierarchyFollowsClassHierarchyValidationRule : ITypeDefinitionValidationRule
  {
    public InheritanceHierarchyFollowsClassHierarchyValidationRule ()
    {

    }

    public MappingValidationResult Validate (TypeDefinition typeDefinition)
    {
      if (typeDefinition is not ClassDefinition classDefinition) // TODO R2I Valdiation: Support for interface support
        throw new InvalidOperationException("Only class definitions are supported.");

      if (classDefinition.BaseClass == null)
        return MappingValidationResult.CreateValidResult();

      if (!classDefinition.Type.IsSubclassOf(classDefinition.BaseClass.Type))
      {
        return MappingValidationResult.CreateInvalidResultForType(
            classDefinition.BaseClass.Type,
            "Type '{0}' of class '{1}' is not derived from type '{2}' of base class '{3}'.",
            classDefinition.Type.GetAssemblyQualifiedNameSafe(),
            classDefinition.ID,
            classDefinition.BaseClass.Type.GetAssemblyQualifiedNameSafe(),
            classDefinition.BaseClass.ID);
      }
      return MappingValidationResult.CreateValidResult();
    }
  }
}
