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
  /// Validates that all tables within a concrete table inheritance hierarchy have unique names.
  /// </summary>
  public class TableNamesAreDistinctWithinConcreteTableInheritanceHierarchyValidationRule : IPersistenceMappingValidationRule
  {
    public TableNamesAreDistinctWithinConcreteTableInheritanceHierarchyValidationRule ()
    {

    }

    public IEnumerable<MappingValidationResult> Validate (TypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      if (typeDefinition is not ClassDefinition classDefinition)
        yield break;

      if (classDefinition.BaseClass == null) //if class definition is inheritance root class
      {
        var allDistinctTableNames = new HashSet<string>();
        foreach (var tableName in FindAllTableDefinitions(classDefinition).Select(td => td.TableName.EntityName))
        {
          if (allDistinctTableNames.Contains(tableName))
          {
            yield return MappingValidationResult.CreateInvalidResultForType(
                classDefinition.Type,
                "At least two classes in different inheritance branches derived from abstract class '{0}'"
                + " specify the same entity name '{1}', which is not allowed.",
                classDefinition.Type.Name,
                tableName);
          }
          else
          {
            yield return MappingValidationResult.CreateValidResult();
          }

          allDistinctTableNames.Add(tableName);
        }
      }
    }

    private IEnumerable<TableDefinition> FindAllTableDefinitions (ClassDefinition classDefinition)
    {
      var tableDefinition = classDefinition.HasStorageEntityDefinitionBeenSet ? classDefinition.StorageEntityDefinition as TableDefinition : null;
      if (tableDefinition != null)
        yield return tableDefinition;

      foreach (ClassDefinition derivedClass in classDefinition.DerivedClasses)
      {
        var tableDefinitionsInDerivedClass = FindAllTableDefinitions(derivedClass);
        foreach (var tableDefinitionInDerivedClass in tableDefinitionsInDerivedClass)
          yield return tableDefinitionInDerivedClass;
      }
    }
  }
}
