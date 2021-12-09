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
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Validation
{
  /// <summary>
  /// Validates that in a hierarchy of classes, only one can be mapped to a table.
  /// </summary>
  public class OnlyOneTablePerHierarchyValidationRule : IPersistenceMappingValidationRule
  {
    public OnlyOneTablePerHierarchyValidationRule ()
    {
    }

    public IEnumerable<MappingValidationResult> Validate (TypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      if (typeDefinition is not ClassDefinition classDefinition)
      {
        yield return MappingValidationResult.CreateValidResult();
      }
      else if (classDefinition.BaseClass != null && classDefinition.HasStorageEntityDefinitionBeenSet && classDefinition.StorageEntityDefinition is TableDefinition)
      {
        var baseClasses = classDefinition.BaseClass.CreateSequence(cd => cd.BaseClass);
        foreach (ClassDefinition baseClass in baseClasses)
        {
          Assertion.DebugAssert(baseClass.HasStorageEntityDefinitionBeenSet, "baseClass.HasStorageEntityDefinitionBeenSet == true");
          if (baseClass.StorageEntityDefinition is TableDefinition)
          {
            yield return MappingValidationResult.CreateInvalidResultForType(
                classDefinition.Type,
                "Class '{0}' must not define a table when its base class '{1}' also defines one.",
                classDefinition.Type.Name,
                baseClass.Type.Name);
          }
        }
      }
      else
      {
        yield return MappingValidationResult.CreateValidResult();
      }
    }
  }
}
