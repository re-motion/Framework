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
  public class RdbmsRelationEndPointCombinationIsSupportedValidationRule : RelationEndPointCombinationIsSupportedValidationRule
  {
    public RdbmsRelationEndPointCombinationIsSupportedValidationRule ()
    {
    }

    public override MappingValidationResult Validate (RelationDefinition relationDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationDefinition", relationDefinition);

      var endPointDefinition1 = relationDefinition.EndPointDefinitions[0];
      var endPointDefinition2 = relationDefinition.EndPointDefinitions[1];

      if (IsInvalidEndPointDefinition (endPointDefinition1) || IsInvalidEndPointDefinition (endPointDefinition2))
        return MappingValidationResult.CreateValidResult();

      if (endPointDefinition1.IsAnonymous && endPointDefinition2.IsAnonymous)
      {
        return MappingValidationResult.CreateInvalidResult ("Relation '{0}' cannot have two anonymous end points.", relationDefinition.ID);
      }

      if (endPointDefinition1.IsVirtual && endPointDefinition2.IsVirtual)
      {
        var endPointDefinition1AsVirtualRelationEndPointDefinition = endPointDefinition1 as VirtualRelationEndPointDefinition;
        var endPointDefinition2AsVirtualRelationEndPointDefinition = endPointDefinition2 as VirtualRelationEndPointDefinition;
        if (endPointDefinition1AsVirtualRelationEndPointDefinition != null && endPointDefinition2AsVirtualRelationEndPointDefinition != null)
        {
          return MappingValidationResult.CreateInvalidResultForRelation (
              relationDefinition.ID,
              endPointDefinition1.PropertyInfo,
              "The relation between property '{0}', declared on type '{1}', and property '{2}' declared on type '{3}', contains two virtual end points. "
              + "One of the two properties must set 'ContainsForeignKey' to 'true' on the '{4}'.",
              endPointDefinition1.PropertyInfo.Name,
              endPointDefinition1.ClassDefinition.ClassType.Name,
              endPointDefinition2.PropertyInfo.Name,
              endPointDefinition2.ClassDefinition.ClassType.Name,
              typeof (DBBidirectionalRelationAttribute).Name);
        }
        else
        {
          return MappingValidationResult.CreateInvalidResultForRelation (
              relationDefinition.ID,
              endPointDefinition1.PropertyInfo,
              "Relation '{0}' cannot have two virtual end points.",
              relationDefinition.ID);
        }
      }

      if (!endPointDefinition1.IsVirtual && !endPointDefinition2.IsVirtual)
      {
        return MappingValidationResult.CreateInvalidResultForRelation (
            relationDefinition.ID,
            endPointDefinition1.PropertyInfo,
            "The relation between property '{0}', declared on type '{1}', and property '{2}' declared on type '{3}', contains two non-virtual end points. "
            + "One of the two properties must set 'ContainsForeignKey' to 'false' on the '{4}'.",
            endPointDefinition1.PropertyInfo.Name,
            endPointDefinition1.ClassDefinition.ClassType.Name,
            endPointDefinition2.PropertyInfo.Name,
            endPointDefinition2.ClassDefinition.ClassType.Name,
            typeof (DBBidirectionalRelationAttribute).Name);
      }

      return MappingValidationResult.CreateValidResult();
    }
  }
}