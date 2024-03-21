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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Validation
{
  /// <summary>
  /// Validates that a persistent property type is supported by the storage provider.
  /// </summary>
  public class PropertyTypeIsSupportedByStorageProviderValidationRule : IPersistenceMappingValidationRule
  {
    public IEnumerable<MappingValidationResult> Validate (TypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      return from PropertyDefinition propertyDefinition in typeDefinition.MyPropertyDefinitions
             select Validate(propertyDefinition);
    }

    private MappingValidationResult Validate (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull("propertyDefinition", propertyDefinition);

      if (propertyDefinition.StorageClass == StorageClass.Persistent)
      {
        var unsupportedStoragePropertyDefinition = propertyDefinition.HasStoragePropertyDefinitionBeenSet
            ? propertyDefinition.StoragePropertyDefinition as UnsupportedStoragePropertyDefinition
            : null;
        if (unsupportedStoragePropertyDefinition != null)
        {
          return MappingValidationResult.CreateInvalidResultForProperty(
              propertyDefinition.PropertyInfo,
              "The property type '{0}' is not supported by this storage provider. {1}",
              propertyDefinition.PropertyType.Name,
              unsupportedStoragePropertyDefinition.Message);
        }
      }
      return MappingValidationResult.CreateValidResult();
    }
  }
}
