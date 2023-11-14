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
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.NonPersistent.Validation
{
  /// <summary>
  /// Validates that a property's <see cref="PropertyDefinition.StorageClass"/> is supported by the (non-persistent) storage provider.
  /// </summary>
  public class PropertyStorageClassIsSupportedByStorageProviderValidationRule : IPersistenceMappingValidationRule
  {
    public IEnumerable<MappingValidationResult> Validate (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);

      return from PropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions
          select Validate(propertyDefinition);
    }

    private MappingValidationResult Validate (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull("propertyDefinition", propertyDefinition);

      if (propertyDefinition.StorageClass == StorageClass.Persistent && propertyDefinition.ClassDefinition.HasStorageEntityDefinitionBeenSet)
      {
        if (propertyDefinition.ClassDefinition.IsNonPersistent())
        {
          return MappingValidationResult.CreateInvalidResultForProperty(
              propertyDefinition.PropertyInfo,
              "StorageClass.Persistent is not supported for properties of classes that belong to the '{0}'.",
              typeof(NonPersistentProviderDefinition).Name);
        }
      }
      return MappingValidationResult.CreateValidResult();
    }
  }}
