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

namespace Remotion.Data.DomainObjects.Mapping.Validation.Reflection
{
  /// <summary>
  /// Validates that the property mapping attributes are applied at the original property declaration.
  /// </summary>
  public class MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule : IPropertyDefinitionValidationRule
  {
    private readonly IMemberInformationNameResolver _nameResolver;
    private readonly IPropertyMetadataProvider _propertyMetadataProvider;

    public MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule (
        IMemberInformationNameResolver nameResolver,
        IPropertyMetadataProvider propertyMetadataProvider)
    {
      ArgumentUtility.CheckNotNull("nameResolver", nameResolver);
      ArgumentUtility.CheckNotNull("propertyMetadataProvider", propertyMetadataProvider);

      _nameResolver = nameResolver;
      _propertyMetadataProvider = propertyMetadataProvider;
    }

    public IEnumerable<MappingValidationResult> Validate (TypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      if (!typeDefinition.IsTypeResolved)
        throw new InvalidOperationException("Type '" + typeDefinition.Type.GetFullNameSafe() + "' is not resolved.");

      AllMappingPropertiesFinder propertyFinder;
      if (typeDefinition is ClassDefinition classDefinition)
      {
        var isInheritanceRoot = classDefinition.BaseClass == null;
        propertyFinder = new AllMappingPropertiesFinder(
            classDefinition.Type,
            isInheritanceRoot,
            true,
            _nameResolver,
            classDefinition.PersistentMixinFinder,
            _propertyMetadataProvider);
      }
      else
      {
        throw new NotSupportedException("Only class definitions are supported."); // TODO R2I Mapping: property finder support for interfaces
      }
      var propertyInfos = propertyFinder.FindPropertyInfos();

      return from IPropertyInformation propertyInfo in propertyInfos
             select Validate(propertyInfo);
    }

    private MappingValidationResult Validate (IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull("propertyInfo", propertyInfo);

      if (! propertyInfo.IsOriginalDeclaration())
      {
        var mappingAttributes = propertyInfo.GetCustomAttributes<IMappingAttribute>(false);
        if (mappingAttributes.Any())
        {
          return MappingValidationResult.CreateInvalidResultForProperty(
              propertyInfo,
              "The '{0}' is a mapping attribute and may only be applied at the property's base definition.",
              mappingAttributes[0].GetType().Name);
        }
      }
      return MappingValidationResult.CreateValidResult();
    }
  }
}
