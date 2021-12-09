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
using Remotion.Collections;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Validation
{
  /// <summary>
  /// Validates that each defined persistent property storage specific name is not already defined in a class in the same inheritance hierarchy.
  /// </summary>
  public class ColumnNamesAreUniqueWithinInheritanceTreeValidationRule : IPersistenceMappingValidationRule
  {
    private readonly IRdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;

    public ColumnNamesAreUniqueWithinInheritanceTreeValidationRule (IRdbmsPersistenceModelProvider rdbmsPersistenceModelProvider)
    {
      ArgumentUtility.CheckNotNull("rdbmsPersistenceModelProvider", rdbmsPersistenceModelProvider);

      _rdbmsPersistenceModelProvider = rdbmsPersistenceModelProvider;
    }

    public IEnumerable<MappingValidationResult> Validate (TypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      var validationResults = new List<MappingValidationResult>();
      if (typeDefinition is not ClassDefinition classDefinition) // TODO R2I Persistence: Support TypeDefinition
        return validationResults;

      if (classDefinition.BaseClass == null) //if class definition is inheritance root class
      {
        var derivedPropertyDefinitions = classDefinition.GetAllDerivedClasses()
            .SelectMany(cd => cd.MyPropertyDefinitions);
        var allPropertyDefinitions =
            classDefinition.MyPropertyDefinitions.Concat(derivedPropertyDefinitions).Where(pd => pd.StorageClass == StorageClass.Persistent);

        var  propertyDefinitionsByName = new MultiDictionary<string, PropertyDefinition>();
        foreach (var propertyDefinition in allPropertyDefinitions)
        {
          foreach (var simpleColumnDefinition in _rdbmsPersistenceModelProvider.GetStoragePropertyDefinition(propertyDefinition).GetColumns())
            propertyDefinitionsByName[simpleColumnDefinition.Name].Add(propertyDefinition);
        }
        foreach (var keyValuePair in propertyDefinitionsByName)
          validationResults.AddRange(ValidatePropertyGroup(keyValuePair.Key, keyValuePair.Value));
      }
      return validationResults;
    }

    private IEnumerable<MappingValidationResult> ValidatePropertyGroup (string columnName, IList<PropertyDefinition> propertyDefinitions)
    {
      if (propertyDefinitions.Count > 1)
      {
        var referenceProperty = propertyDefinitions[0];
        var differentProperties = propertyDefinitions.Where(pd => !pd.PropertyInfo.Equals(referenceProperty.PropertyInfo));
        if (differentProperties.Any())
        {
          foreach (var differentProperty in differentProperties)
          {
            yield return MappingValidationResult.CreateInvalidResultForProperty(
                referenceProperty.PropertyInfo,
                "Property '{0}' of class '{1}' must not define storage specific name '{2}',"
                + " because class '{3}' in same inheritance hierarchy already defines property '{4}' with the same storage specific name.",
                differentProperty.PropertyInfo.Name,
                differentProperty.TypeDefinition.Type.Name,
                columnName,
                referenceProperty.TypeDefinition.Type.Name,
                referenceProperty.PropertyInfo.Name);
          }
        }
        else
          yield return MappingValidationResult.CreateValidResult();
      }
    }
  }
}
