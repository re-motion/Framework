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

namespace Remotion.Data.DomainObjects.Mapping.Validation.Reflection
{
  /// <summary>
  /// Validates that a foreign key is not defined for a virtual relation end point.
  /// </summary>
  public class 
    ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule : IRelationDefinitionValidatorRule
  {
    public MappingValidationResult Validate (RelationDefinition relationDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationDefinition", relationDefinition);

      foreach (var endPointDefinition in relationDefinition.EndPointDefinitions)
      {
        var validationResult = Validate (endPointDefinition);
        if (!validationResult.IsValid)
          return validationResult;
      }

      return MappingValidationResult.CreateValidResult();
    }

    private MappingValidationResult Validate (IRelationEndPointDefinition relationEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

      if (relationEndPointDefinition.IsAnonymous)
        return MappingValidationResult.CreateValidResult();

      var propertyInfo = relationEndPointDefinition.PropertyInfo;
      if (propertyInfo == null)
        return MappingValidationResult.CreateValidResult();

      var relationAttribute = propertyInfo.GetCustomAttribute<DBBidirectionalRelationAttribute> (true);
      if (relationAttribute != null && relationAttribute.ContainsForeignKey && ReflectionUtility.IsObjectList (propertyInfo.PropertyType))
      {
        return MappingValidationResult.CreateInvalidResultForProperty (
            propertyInfo,
            "Only relation end points with a property type of '{0}' can contain the foreign key.",
            typeof (DomainObject).Name);
      }

      return MappingValidationResult.CreateValidResult();
    }
  }
}