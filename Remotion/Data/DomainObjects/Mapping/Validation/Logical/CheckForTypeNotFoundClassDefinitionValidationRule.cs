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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation.Logical
{
  /// <summary>
  /// Validates that the given<see cref="ClassDefinition"/> is no <see cref="ClassDefinitionForUnresolvedRelationPropertyType"/>.
  /// </summary>
  public class CheckForTypeNotFoundClassDefinitionValidationRule : IRelationDefinitionValidatorRule
  {
    public MappingValidationResult Validate (RelationDefinition relationDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationDefinition", relationDefinition);

      foreach (var endPointDefinition in relationDefinition.EndPointDefinitions)
      {
        var classDefinitionAsTypeNotFoundClassDefinition = endPointDefinition.ClassDefinition as ClassDefinitionForUnresolvedRelationPropertyType;
        if (classDefinitionAsTypeNotFoundClassDefinition!=null)
        {
          return MappingValidationResult.CreateInvalidResultForProperty (
              classDefinitionAsTypeNotFoundClassDefinition.RelationProperty,
              "The relation property '{0}' has return type '{1}', which is not a part of the mapping. Relation properties must not point to "
              + "classes above the inheritance root.",
              classDefinitionAsTypeNotFoundClassDefinition.RelationProperty.Name,
              classDefinitionAsTypeNotFoundClassDefinition.RelationProperty.PropertyType.Name);
        }
      }
      
      return MappingValidationResult.CreateValidResult();
    }
  }
}