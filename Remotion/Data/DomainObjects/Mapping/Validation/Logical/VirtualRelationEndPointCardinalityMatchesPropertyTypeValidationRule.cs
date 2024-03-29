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
  /// Validates that the virtual relation end point cardinality matches the property type.
  /// </summary>
  public class VirtualRelationEndPointCardinalityMatchesPropertyTypeValidationRule : IRelationDefinitionValidatorRule
  {
    public VirtualRelationEndPointCardinalityMatchesPropertyTypeValidationRule ()
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

      if (relationEndPointDefinition is VirtualObjectRelationEndPointDefinition virtualObjectRelationEndPointDefinition
          && !ReflectionUtility.IsDomainObject(virtualObjectRelationEndPointDefinition.PropertyInfo.PropertyType))
      {
        return MappingValidationResult.CreateInvalidResultForProperty(
            virtualObjectRelationEndPointDefinition.PropertyInfo,
            "The property type of a virtual end point of a one-to-one relation must be assignable to '{0}'.",
            typeof(DomainObject).Name);
      }

      if (relationEndPointDefinition is DomainObjectCollectionRelationEndPointDefinition domainObjectCollectionRelationEndPointDefinition
          && !ReflectionUtility.IsObjectList(domainObjectCollectionRelationEndPointDefinition.PropertyInfo.PropertyType))
      {
        return MappingValidationResult.CreateInvalidResultForProperty(
            domainObjectCollectionRelationEndPointDefinition.PropertyInfo,
            "The property type of a virtual end point of a one-to-many relation must be assignable to '{0}'.",
            typeof(ObjectList<>).Name);
      }

      if (relationEndPointDefinition is VirtualCollectionRelationEndPointDefinition virtualCollectionRelationEndPointDefinition
          && !ReflectionUtility.IsIObjectList(virtualCollectionRelationEndPointDefinition.PropertyInfo.PropertyType))
      {
        return MappingValidationResult.CreateInvalidResultForProperty(
            virtualCollectionRelationEndPointDefinition.PropertyInfo,
            "The property type of a virtual end point of a one-to-many relation must be '{0}'.",
            typeof(IObjectList<>).Name);
      }

      return MappingValidationResult.CreateValidResult();
    }
  }
}
