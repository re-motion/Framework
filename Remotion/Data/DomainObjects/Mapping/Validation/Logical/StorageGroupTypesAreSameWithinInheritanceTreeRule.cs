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
  public class StorageGroupTypesAreSameWithinInheritanceTreeRule : IClassDefinitionValidationRule
  {
    public MappingValidationResult Validate (ClassDefinition classDefinition)
    {
      if (classDefinition.BaseClass != null)
      {
        if (classDefinition.StorageGroupType != classDefinition.BaseClass.StorageGroupType)
        {
          var message = "Class '{0}' must have the same storage group type as its base class '{1}'.";
          return MappingValidationResult.CreateInvalidResultForType (
              classDefinition.ClassType, 
              message, 
              classDefinition.ClassType.Name, 
              classDefinition.BaseClass.ClassType.Name);
        }
      }
      return MappingValidationResult.CreateValidResult();
    }
  }
}