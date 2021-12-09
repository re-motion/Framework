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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation.Logical
{
  /// <summary>
  /// Validates that the given storage class is supported.
  /// </summary>
  public class StorageClassIsSupportedValidationRule : IPropertyDefinitionValidationRule
  {
    public StorageClassIsSupportedValidationRule ()
    {
    }

    public IEnumerable<MappingValidationResult> Validate (TypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      return from PropertyDefinition propertyDefinition in typeDefinition.MyPropertyDefinitions
             select Validate(propertyDefinition.PropertyInfo);
    }

    private MappingValidationResult Validate (IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull("propertyInfo", propertyInfo);

      var storageClassAttribute = propertyInfo.GetCustomAttribute<StorageClassAttribute>(true);

      if (storageClassAttribute != null && storageClassAttribute.StorageClass != StorageClass.Persistent
          && storageClassAttribute.StorageClass != StorageClass.Transaction)
      {
        return MappingValidationResult.CreateInvalidResultForProperty(
            propertyInfo,
            "Only StorageClass.Persistent and StorageClass.Transaction are supported for property '{0}' of type '{1}'.",
            propertyInfo.Name,
            propertyInfo.DeclaringType!.Name);
      }
      return MappingValidationResult.CreateValidResult();
    }
  }
}
