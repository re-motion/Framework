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
  /// Validates that the given <see cref="RelationDefinition"/> has no a <see cref="PropertyNotFoundRelationEndPointDefinition"/>
  /// </summary>
  public class CheckForInvalidRelationEndPointsValidationRule : IRelationDefinitionValidatorRule
  {
    public MappingValidationResult Validate (RelationDefinition relationDefinition)
    {
      ArgumentUtility.CheckNotNull("relationDefinition", relationDefinition);

      foreach (var endPointDefinition in relationDefinition.EndPointDefinitions)
      {
        if (endPointDefinition is PropertyNotFoundRelationEndPointDefinition propertyNotFoundRelationEndPointDefinition)
        {
          return MappingValidationResult.CreateInvalidResultForType(
              propertyNotFoundRelationEndPointDefinition.TypeDefinition.Type,
              "Property '{0}' on type '{1}' could not be found.",
              propertyNotFoundRelationEndPointDefinition.PropertyName,
              propertyNotFoundRelationEndPointDefinition.TypeDefinition.Type.Name);
        }
        else if (endPointDefinition is TypeNotObjectIDRelationEndPointDefinition typeNotObjectIDRelationEndPointDefinition)
        {
          return MappingValidationResult.CreateInvalidResultForType(
              typeNotObjectIDRelationEndPointDefinition.TypeDefinition.Type,
            "Relation property '{0}' on type '{1}' is of type '{2}', but non-virtual relation properties must be of type '{3}'.",
            typeNotObjectIDRelationEndPointDefinition.PropertyName,
            typeNotObjectIDRelationEndPointDefinition.TypeDefinition.Type.Name,
            typeNotObjectIDRelationEndPointDefinition.PropertyInfo.PropertyType.Name,
            typeof(ObjectID).Name);
        }
        else if (endPointDefinition is TypeNotCompatibleWithVirtualRelationEndPointDefinition)
        {
          Assertion.DebugAssert(endPointDefinition.IsAnonymous == false, "endPointDefinition.IsAnonymous == false");
          Assertion.DebugIsNotNull(endPointDefinition.PropertyInfo, "endPointDefinition.PropertyInfo != null when endPointDefinition.IsAnonymous == false");

          return MappingValidationResult.CreateInvalidResultForType(
              endPointDefinition.TypeDefinition.Type,
              "Relation property '{0}' on type '{1}' is of type '{2}', but virtual relation properties must be of type '{3}', '{4}', or '{5}'.",
              endPointDefinition.PropertyName,
              endPointDefinition.TypeDefinition.Type.Name,
              endPointDefinition.PropertyInfo.PropertyType.Name,
              typeof(DomainObject).Name,
              typeof(ObjectList<>).Name,
              typeof(IObjectList<>).Name);
        }
      }
      return MappingValidationResult.CreateValidResult();
    }
  }
}
