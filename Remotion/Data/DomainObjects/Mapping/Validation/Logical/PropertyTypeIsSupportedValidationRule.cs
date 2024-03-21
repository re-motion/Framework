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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation.Logical
{
  /// <summary>
  /// Validates that a property type is supported.
  /// </summary>
  public class PropertyTypeIsSupportedValidationRule : IPropertyDefinitionValidationRule
  {
    public PropertyTypeIsSupportedValidationRule ()
    {
    }

    public IEnumerable<MappingValidationResult> Validate (TypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      return from PropertyDefinition propertyDefinition in typeDefinition.MyPropertyDefinitions
             select Validate(propertyDefinition);
    }

    private MappingValidationResult Validate (PropertyDefinition propertyDefinition)
    {
      var nativePropertyType = propertyDefinition.PropertyType;
      if (!PropertyValue.IsTypeSupported(nativePropertyType))
      {
        return MappingValidationResult.CreateInvalidResultForProperty(
            propertyDefinition.PropertyInfo,
            "The property type '{0}' is not supported. If you meant to declare a relation, '{0}' must be derived from '{1}'. "
            + "For non-mapped properties, use the '{2}'.",
            nativePropertyType.Name,
            typeof(DomainObject).Name,
            typeof(StorageClassNoneAttribute).Name);
      }

      return MappingValidationResult.CreateValidResult();
    }
  }
}
