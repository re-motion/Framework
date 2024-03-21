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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Validation
{
  /// <summary>
  /// Validates that all concrete (non-abstract) classes used with an RDBMS provider in the mapping have an associated table (either directly or indirectly).
  /// </summary>
  public class ClassAboveTableIsAbstractValidationRule : IPersistenceMappingValidationRule
  {
    public ClassAboveTableIsAbstractValidationRule ()
    {

    }

    public IEnumerable<MappingValidationResult> Validate (TypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      if (typeDefinition is not ClassDefinition classDefinition)
      {
        yield return MappingValidationResult.CreateValidResult();
      }
      else if (!classDefinition.IsTypeResolved)
      {
        yield return MappingValidationResult.CreateValidResult();
      }
      else if (!IsAssociatedWithRdbmsStorageEntityDefinition(classDefinition))
      {
        yield return MappingValidationResult.CreateValidResult();
      }
      else if (classDefinition.IsAbstract)
      {
        yield return MappingValidationResult.CreateValidResult();
      }
      else if (IsAssociatedWithTable(classDefinition))
      {
        yield return MappingValidationResult.CreateValidResult();
      }
      else
      {
        yield return MappingValidationResult.CreateInvalidResultForType(
            classDefinition.Type,
            "Neither class '{0}' nor its base classes are mapped to a table. "
            + "Make class '{0}' abstract or define a table for it or one of its base classes.",
            classDefinition.Type.Name);
      }
    }

    private bool IsAssociatedWithRdbmsStorageEntityDefinition (ClassDefinition classDefinition)
    {
      return classDefinition.HasStorageEntityDefinitionBeenSet && classDefinition.StorageEntityDefinition is IRdbmsStorageEntityDefinition;
    }

    private bool IsAssociatedWithTable (ClassDefinition classDefinition)
    {
      return classDefinition.HasStorageEntityDefinitionBeenSet
             && (classDefinition.StorageEntityDefinition is TableDefinition || classDefinition.StorageEntityDefinition is FilterViewDefinition);
    }
  }
}
