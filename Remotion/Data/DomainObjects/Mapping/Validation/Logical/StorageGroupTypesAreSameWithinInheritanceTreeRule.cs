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

namespace Remotion.Data.DomainObjects.Mapping.Validation.Logical
{
  /// <summary>
  /// Validates that the class has the same storage group type as its base class.
  /// </summary>
  public class StorageGroupTypesAreSameWithinInheritanceTreeRule : ITypeDefinitionValidationRule
  {
    public MappingValidationResult Validate (TypeDefinition typeDefinition)
    {
      if (typeDefinition is ClassDefinition classDefinition)
      {
        return ValidateClassDefinition(classDefinition);
      }
      else if (typeDefinition is InterfaceDefinition interfaceDefinition)
      {
        return ValidateInterfaceDefinition(interfaceDefinition);
      }
      else
      {
        throw new InvalidOperationException("Only class definitions are supported");
      }
    }

    private static MappingValidationResult ValidateClassDefinition (ClassDefinition classDefinition)
    {
      if (classDefinition.BaseClass != null && classDefinition.StorageGroupType != classDefinition.BaseClass.StorageGroupType)
      {
        return MappingValidationResult.CreateInvalidResultForType(
            classDefinition.Type,
            "Class '{0}' must have the same storage group type as its base class '{1}'.",
            classDefinition.Type.Name,
            classDefinition.BaseClass.Type.Name);
      }

      foreach (var implementedInterface in classDefinition.ImplementedInterfaces)
      {
        if (classDefinition.StorageGroupType != implementedInterface.StorageGroupType)
        {
          return MappingValidationResult.CreateInvalidResultForType(
              classDefinition.Type,
              "Class '{0}' must have the same storage group type as its implemented interface '{1}'.",
              classDefinition.Type.Name,
              implementedInterface.Type.Name);
        }
      }

      return MappingValidationResult.CreateValidResult();
    }

    private static MappingValidationResult ValidateInterfaceDefinition (InterfaceDefinition interfaceDefinition)
    {
      foreach (var extendedInterfaces in interfaceDefinition.ExtendedInterfaces)
      {
        if (interfaceDefinition.StorageGroupType != extendedInterfaces.StorageGroupType)
        {
          return MappingValidationResult.CreateInvalidResultForType(
              interfaceDefinition.Type,
              "Interface '{0}' must have the same storage group type as its extended interface '{1}'.",
              interfaceDefinition.Type.Name,
              extendedInterfaces.Type.Name);
        }
      }

      return MappingValidationResult.CreateValidResult();
    }
  }
}
