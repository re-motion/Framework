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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// Finds a property based on its name. This is used by <see cref="RelationReflectorBase{T}"/> to find the opposite property for a relation end-point.
  /// </summary>
  public class NameBasedPropertyFinder : PropertyFinderBase
  {
    private readonly string _propertyName;

    public NameBasedPropertyFinder (
        string propertyName,
        Type type,
        bool includeBaseProperties,
        bool includeMixinProperties,
        IMemberInformationNameResolver nameResolver,
        IPersistentMixinFinder persistentMixinFinder,
        IPropertyMetadataProvider propertyMetadataProvider)
        : base (type, includeBaseProperties, includeMixinProperties, nameResolver, persistentMixinFinder, propertyMetadataProvider)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      _propertyName = propertyName;
    }

    protected override bool FindPropertiesFilter (IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      if (!base.FindPropertiesFilter (propertyInfo))
        return false;

      return propertyInfo.Name == _propertyName;
    }

    protected override PropertyFinderBase CreateNewFinder (
        Type type,
        bool includeBaseProperties,
        bool includeMixinProperties,
        IMemberInformationNameResolver nameResolver,
        IPersistentMixinFinder persistentMixinFinder,
        IPropertyMetadataProvider propertyMetadataProvider)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("nameResolver", nameResolver);
      ArgumentUtility.CheckNotNull ("persistentMixinFinder", persistentMixinFinder);
      ArgumentUtility.CheckNotNull ("propertyMetadataProvider", propertyMetadataProvider);

      return new NameBasedPropertyFinder (
          _propertyName,
          type,
          includeBaseProperties,
          includeMixinProperties,
          nameResolver,
          persistentMixinFinder,
          propertyMetadataProvider);
    }
  }
}