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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation.Reflection
{
  /// <summary>
  /// Validates that a matching <see cref="BidirectionalRelationAttribute"/> is defined on both end points and that the corresponding defined opposite
  /// property names do match.
  /// </summary>
  public class RelationEndPointNamesAreConsistentValidationRule : IRelationDefinitionValidatorRule
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

      if (!relationEndPointDefinition.IsAnonymous)
      {
        var relationAttribute = relationEndPointDefinition.PropertyInfo.GetCustomAttribute<BidirectionalRelationAttribute> (true);
        if (relationAttribute != null)
        {
          var oppositeEndPointDefinition = relationEndPointDefinition.GetOppositeEndPointDefinition ();
          if (!oppositeEndPointDefinition.IsAnonymous)
          {
            var oppositeProperty = oppositeEndPointDefinition.PropertyInfo;
            var oppositeRelationAttribute = oppositeProperty.GetCustomAttribute<BidirectionalRelationAttribute> (true);

            if (oppositeRelationAttribute == null)
            {
              return MappingValidationResult.CreateInvalidResultForProperty (
                  relationEndPointDefinition.PropertyInfo,
                  "Opposite relation property '{0}' declared on type '{1}' does not define a matching '{2}'.",
                  relationAttribute.OppositeProperty,
                  oppositeProperty.DeclaringType.Name,
                  relationAttribute.GetType().Name);
            }

            if (!relationEndPointDefinition.PropertyInfo.Name.Equals (oppositeRelationAttribute.OppositeProperty, StringComparison.Ordinal))
            {
              return MappingValidationResult.CreateInvalidResultForProperty (
                  relationEndPointDefinition.PropertyInfo,
                  "Opposite relation property '{0}' declared on type '{1}' defines a '{2}' whose opposite property does not match.",
                  relationAttribute.OppositeProperty,
                  oppositeProperty.DeclaringType.Name,
                  relationAttribute.GetType().Name);
            }
          }
        }
      }

      return MappingValidationResult.CreateValidResult();
    }
   
  }
}