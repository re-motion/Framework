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
  /// Validates that the virtual property definition is derived from DomainObject, DomainObjectValidation or DomainObjectCollection.
  /// </summary>
  public class VirtualRelationEndPointPropertyTypeIsSupportedValidationRule : IRelationDefinitionValidatorRule
  {
    public VirtualRelationEndPointPropertyTypeIsSupportedValidationRule ()
    {

    }

    public MappingValidationResult Validate (RelationDefinition relationDefinition)
    {
      ArgumentUtility.CheckNotNull("relationDefinition", relationDefinition);

      foreach (var endPointDefinition in relationDefinition.EndPointDefinitions)
      {
        var validationResult = Validate(endPointDefinition);
        if (!validationResult.IsValid)
          return validationResult;
      }

      return MappingValidationResult.CreateValidResult();
    }

    private MappingValidationResult Validate (IRelationEndPointDefinition relationEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull("relationEndPointDefinition", relationEndPointDefinition);

      if (relationEndPointDefinition.IsVirtual
          && relationEndPointDefinition.PropertyInfo != null
          && !ReflectionUtility.IsRelationType(relationEndPointDefinition.PropertyInfo.PropertyType))
      {
        return MappingValidationResult.CreateInvalidResultForProperty(
            relationEndPointDefinition.PropertyInfo,
            "Virtual property '{0}' of type '{1}' is of type '{2}', but must be assignable to '{3}' or '{4}' or be of type '{5}'.",
            relationEndPointDefinition.PropertyInfo.Name,
            relationEndPointDefinition.TypeDefinition.Type.Name,
            relationEndPointDefinition.PropertyInfo.PropertyType.Name,
            typeof(DomainObject).Name,
            typeof(ObjectList<>).Name,
            typeof(IObjectList<>).Name);
      }
      return MappingValidationResult.CreateValidResult();
    }
  }
}
