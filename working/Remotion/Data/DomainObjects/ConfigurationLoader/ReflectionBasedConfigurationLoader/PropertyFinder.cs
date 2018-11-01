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
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// The <see cref="PropertyFinder"/> is used to find all <see cref="PropertyInfo"/> objects that constitute a <see cref="PropertyDefinition"/>.
  /// </summary>
  public class PropertyFinder : PropertyFinderBase
  {
    private readonly ClassDefinition _classDefinition;
    private readonly IDomainModelConstraintProvider _domainModelConstraintProvider;

    public PropertyFinder (
        Type type,
        ClassDefinition classDefinition,
        bool includeBaseProperties,
        bool includeMixinProperties,
        IMemberInformationNameResolver nameResolver,
        IPersistentMixinFinder persistentMixinFinder,
        IPropertyMetadataProvider propertyMetadataProvider,
        IDomainModelConstraintProvider domainModelConstraintProvider)
        : base (type, includeBaseProperties, includeMixinProperties, nameResolver, persistentMixinFinder, propertyMetadataProvider)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      _classDefinition = classDefinition;
      _domainModelConstraintProvider = domainModelConstraintProvider;
    }

    protected override bool FindPropertiesFilter (IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      if (!base.FindPropertiesFilter (propertyInfo))
        return false;

      if (IsVirtualRelationEndPoint (propertyInfo))
        return false;

      return true;
    }

    protected override PropertyFinderBase CreateNewFinder (
        Type type,
        bool includeBaseProperties,
        bool includeMixinProperties,
        IMemberInformationNameResolver nameResolver,
        IPersistentMixinFinder persistentMixinFinder,
        IPropertyMetadataProvider propertyMetadataProvider)
    {
      return new PropertyFinder (
          type,
          _classDefinition,
          includeBaseProperties,
          includeMixinProperties,
          nameResolver,
          persistentMixinFinder,
          propertyMetadataProvider,
          _domainModelConstraintProvider);
    }

    private bool IsVirtualRelationEndPoint (IPropertyInformation propertyInfo)
    {
      if (!ReflectionUtility.IsRelationType (propertyInfo.PropertyType))
        return false;
      var relationEndPointReflector = RelationEndPointReflector.CreateRelationEndPointReflector (
          _classDefinition,
          propertyInfo,
          NameResolver,
          PropertyMetadataProvider,
          _domainModelConstraintProvider);
      return relationEndPointReflector.IsVirtualEndRelationEndpoint();
    }
  }
}