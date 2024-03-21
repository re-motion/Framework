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
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.NonPersistent.Validation
{
  /// <summary>
  /// Validates that a persistent relation property does not reference non-persistent classes.
  /// </summary>
  public class RelationPropertyStorageClassMatchesReferencedTypeDefinitionStorageClassValidationRule : IPersistenceMappingValidationRule
  {
    public RelationPropertyStorageClassMatchesReferencedTypeDefinitionStorageClassValidationRule ()
    {
    }

    public IEnumerable<MappingValidationResult> Validate (TypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      return from IRelationEndPointDefinition endPointDefinition in typeDefinition.MyRelationEndPointDefinitions
          select Validate(endPointDefinition);
    }

    private MappingValidationResult Validate (IRelationEndPointDefinition endPointDefinition)
    {
      if (endPointDefinition is RelationEndPointDefinition relationEndPointDefinition && relationEndPointDefinition.HasRelationDefinitionBeenSet)
      {
        var oppositeEndPointDefinition = endPointDefinition.GetOppositeEndPointDefinition();

        if (relationEndPointDefinition.PropertyDefinition.StorageClass == StorageClass.Persistent
            && oppositeEndPointDefinition.TypeDefinition.HasStorageEntityDefinitionBeenSet
            && oppositeEndPointDefinition.TypeDefinition.IsNonPersistent())
        {
          return MappingValidationResult.CreateInvalidResultForProperty(
              relationEndPointDefinition.PropertyInfo,
              "The relation property is defined as persistent but the referenced type '{0}' is non-persistent. "
              + "Persistent relation properties may only reference persistent types.",
              oppositeEndPointDefinition.TypeDefinition.Type.GetFullNameSafe());
        }
      }

      return MappingValidationResult.CreateValidResult();
    }
  }
}
