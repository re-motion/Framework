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
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// The <see cref="InterfaceReflector"/> is used to build a <see cref="InterfaceReflector"/>.
  /// </summary>
  public class InterfaceReflector
  {
    private readonly Type _type;
    private readonly IMappingObjectFactory _mappingObjectFactory;
    private readonly IMemberInformationNameResolver _nameResolver;
    private readonly IPropertyMetadataProvider _propertyMetadataProvider;
    private readonly IDomainModelConstraintProvider _domainModelConstraintProvider;
    private readonly ISortExpressionDefinitionProvider _sortExpressionDefinitionProvider;

    public InterfaceReflector (
        Type type,
        IMappingObjectFactory mappingObjectFactory,
        IMemberInformationNameResolver nameResolver,
        IPropertyMetadataProvider propertyMetadataProvider,
        IDomainModelConstraintProvider domainModelConstraintProvider,
        ISortExpressionDefinitionProvider sortExpressionDefinitionProvider)
    {
      ArgumentUtility.CheckNotNull("type", type);
      ArgumentUtility.CheckNotNull("mappingObjectFactory", mappingObjectFactory);
      ArgumentUtility.CheckNotNull("nameResolver", nameResolver);
      ArgumentUtility.CheckNotNull("propertyMetadataProvider", propertyMetadataProvider);
      ArgumentUtility.CheckNotNull("domainModelConstraintProvider", domainModelConstraintProvider);
      ArgumentUtility.CheckNotNull("sortExpressionDefinitionProvider", sortExpressionDefinitionProvider);

      _type = type;
      _mappingObjectFactory = mappingObjectFactory;
      _nameResolver = nameResolver;
      _propertyMetadataProvider = propertyMetadataProvider;
      _domainModelConstraintProvider = domainModelConstraintProvider;
      _sortExpressionDefinitionProvider = sortExpressionDefinitionProvider;
    }

    public InterfaceDefinition GetMetadata (IEnumerable<InterfaceDefinition> extendedInterfaces)
    {
      ArgumentUtility.CheckNotNull("extendedInterfaces", extendedInterfaces);

      var storageGroupAttribute = GetStorageGroupAttribute();
      var storageGroupType = storageGroupAttribute?.GetType();
      var defaultStorageClass = storageGroupAttribute?.DefaultStorageClass ?? DefaultStorageClass.Persistent;
      var interfaceDefinition = new InterfaceDefinition(
          _type,
          extendedInterfaces,
          storageGroupType,
          defaultStorageClass);

      var properties = _mappingObjectFactory.CreatePropertyDefinitionCollection(interfaceDefinition, GetPropertyInfos(interfaceDefinition));
      interfaceDefinition.SetPropertyDefinitions(properties);

      var endPoints = _mappingObjectFactory.CreateRelationEndPointDefinitionCollection(interfaceDefinition);
      interfaceDefinition.SetRelationEndPointDefinitions(endPoints);

      return interfaceDefinition;
    }

    private IEnumerable<IPropertyInformation> GetPropertyInfos (InterfaceDefinition interfaceDefinition)
    {
      var propertyFinder = new PropertyFinder(
          interfaceDefinition.Type,
          interfaceDefinition,
          false,
          false,
          _nameResolver,
          new PersistentMixinFinder(_type, false),
          _propertyMetadataProvider,
          _domainModelConstraintProvider,
          _sortExpressionDefinitionProvider);
      return propertyFinder.FindPropertyInfos();
    }

    private StorageGroupAttribute? GetStorageGroupAttribute ()
    {
      return AttributeUtility.GetCustomAttributes<StorageGroupAttribute>(_type, true).FirstOrDefault();
    }
  }
}
