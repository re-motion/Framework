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
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// The <see cref="AllMappingPropertiesFinder"/> is used to find all <see cref="IPropertyInformation"/> objects that have a mapping attribute applied.
  /// </summary>
  public class AllMappingPropertiesFinder : PropertyFinderBase
  {
    public AllMappingPropertiesFinder (
        Type type,
        bool includeBaseProperties,
        bool includeMixinProperties,
        IMemberInformationNameResolver nameResolver,
        IPersistentMixinFinder persistentMixinFinder,
        IPropertyMetadataProvider propertyMetadataProvider)
        : base(type, includeBaseProperties, includeMixinProperties, nameResolver, persistentMixinFinder, propertyMetadataProvider)
    {

    }

    protected override bool FindPropertiesFilter (IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull("propertyInfo", propertyInfo);
      return propertyInfo.GetCustomAttributes<IMappingAttribute>(false).Any();
    }

    protected override PropertyFinderBase CreateNewFinder (
        Type type,
        bool includeBaseProperties,
        bool includeMixinProperties,
        IMemberInformationNameResolver nameResolver,
        IPersistentMixinFinder persistentMixinFinder,
        IPropertyMetadataProvider propertyMetadataProvider)
    {
      ArgumentUtility.CheckNotNull("type", type);
      ArgumentUtility.CheckNotNull("nameResolver", nameResolver);
      ArgumentUtility.CheckNotNull("persistentMixinFinder", persistentMixinFinder);
      ArgumentUtility.CheckNotNull("propertyMetadataProvider", propertyMetadataProvider);

      return new AllMappingPropertiesFinder(
          type,
          includeBaseProperties,
          includeMixinProperties,
          nameResolver,
          persistentMixinFinder,
          propertyMetadataProvider);
    }
  }
}
