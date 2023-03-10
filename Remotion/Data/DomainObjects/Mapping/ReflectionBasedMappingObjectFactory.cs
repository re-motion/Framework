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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// The <see cref="ReflectionBasedMappingObjectFactory"/> is used to create new mapping objects.
  /// </summary>
  public class ReflectionBasedMappingObjectFactory : IMappingObjectFactory
  {
    private readonly IMemberInformationNameResolver _nameResolver;
    private readonly IClassIDProvider _classIDProvider;
    private readonly IPropertyMetadataProvider _propertyMetadataProvider;
    private readonly IDomainModelConstraintProvider _domainModelConstraintProvider;
    private readonly IDomainObjectCreator _instanceCreator;
    private readonly ISortExpressionDefinitionProvider _sortExpressionDefinitionProvider;

    public ReflectionBasedMappingObjectFactory (
        IMemberInformationNameResolver nameResolver,
        IClassIDProvider classIDProvider,
        IPropertyMetadataProvider propertyMetadataProvider,
        IDomainModelConstraintProvider domainModelConstraintProvider,
        ISortExpressionDefinitionProvider sortExpressionDefinitionProvider,
        IDomainObjectCreator instanceCreator)
    {
      ArgumentUtility.CheckNotNull("nameResolver", nameResolver);
      ArgumentUtility.CheckNotNull("classIDProvider", classIDProvider);
      ArgumentUtility.CheckNotNull("propertyMetadataProvider", propertyMetadataProvider);
      ArgumentUtility.CheckNotNull("domainModelConstraintProvider", domainModelConstraintProvider);
      ArgumentUtility.CheckNotNull("sortExpressionDefinitionProvider", sortExpressionDefinitionProvider);
      ArgumentUtility.CheckNotNull("instanceCreator", instanceCreator);

      _nameResolver = nameResolver;
      _classIDProvider = classIDProvider;
      _propertyMetadataProvider = propertyMetadataProvider;
      _domainModelConstraintProvider = domainModelConstraintProvider;
      _sortExpressionDefinitionProvider = sortExpressionDefinitionProvider;
      _instanceCreator = instanceCreator;
    }

    public ClassDefinition CreateClassDefinition (Type type, ClassDefinition? baseClass)
    {
      ArgumentUtility.CheckNotNull("type", type);

      var classReflector = new ClassReflector(
          type,
          this,
          _nameResolver,
          _classIDProvider,
          _propertyMetadataProvider,
          _domainModelConstraintProvider,
          _sortExpressionDefinitionProvider,
          _instanceCreator);
      return classReflector.GetMetadata(baseClass);
    }

    public PropertyDefinition CreatePropertyDefinition (ClassDefinition classDefinition, IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull("propertyInfo", propertyInfo);

      var propertyReflector = new PropertyReflector(
          classDefinition,
          propertyInfo,
          _nameResolver,
          _propertyMetadataProvider,
          _domainModelConstraintProvider,
          new LegacyPropertyDefaultValueProvider());
      return propertyReflector.GetMetadata();
    }

    public RelationDefinition CreateRelationDefinition (
        IDictionary<Type, ClassDefinition> classDefinitions, ClassDefinition classDefinition, IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull("classDefinitions", classDefinitions);
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull("propertyInfo", propertyInfo);

      var relationReflector = new RelationReflector(classDefinition, propertyInfo, _nameResolver, _propertyMetadataProvider);
      return relationReflector.GetMetadata(classDefinitions);
    }

    public IRelationEndPointDefinition CreateRelationEndPointDefinition (ClassDefinition classDefinition, IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull("propertyInfo", propertyInfo);

      var relationEndPointReflector = RelationEndPointReflector.CreateRelationEndPointReflector(
          classDefinition,
          propertyInfo,
          _nameResolver,
          _propertyMetadataProvider,
          _domainModelConstraintProvider,
          _sortExpressionDefinitionProvider);
      return relationEndPointReflector.GetMetadata();
    }

    public ClassDefinition[] CreateClassDefinitionCollection (IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull("types", types);

      var classDefinitionCollectionFactory = new ClassDefinitionCollectionFactory(this);
      return classDefinitionCollectionFactory.CreateClassDefinitionCollection(types);
    }

    public PropertyDefinitionCollection CreatePropertyDefinitionCollection (
        ClassDefinition classDefinition, IEnumerable<IPropertyInformation> propertyInfos)
    {
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull("propertyInfos", propertyInfos);

      var factory = new PropertyDefinitionCollectionFactory(this);
      return factory.CreatePropertyDefinitions(classDefinition, propertyInfos);
    }

    public RelationDefinition[] CreateRelationDefinitionCollection (IDictionary<Type, ClassDefinition> classDefinitions)
    {
      ArgumentUtility.CheckNotNull("classDefinitions", classDefinitions);

      var factory = new RelationDefinitionCollectionFactory(this);
      return factory.CreateRelationDefinitionCollection(classDefinitions);
    }

    public RelationEndPointDefinitionCollection CreateRelationEndPointDefinitionCollection (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);

      var factory = new RelationEndPointDefinitionCollectionFactory(this, _nameResolver, _propertyMetadataProvider);
      return factory.CreateRelationEndPointDefinitionCollection(classDefinition);
    }
  }
}
