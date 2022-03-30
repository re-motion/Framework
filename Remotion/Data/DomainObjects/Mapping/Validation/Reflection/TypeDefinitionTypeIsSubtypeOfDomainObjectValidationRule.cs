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
  /// Validates that a type definition type is a sub-type of domain object. 
  /// </summary>
  public class TypeDefinitionTypeIsSubtypeOfDomainObjectValidationRule : ITypeDefinitionValidationRule
  {
    public MappingValidationResult Validate (TypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      if (!ReflectionUtility.IsDomainObject(typeDefinition.Type))
      {
        return MappingValidationResult.CreateInvalidResultForType(
            typeDefinition.Type,
            "Type '{0}' is not assignable to '{1}'.",
            typeDefinition.Type.Name,
            nameof(IDomainObject));
      }
      return MappingValidationResult.CreateValidResult();
    }
  }
}
