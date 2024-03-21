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
    private readonly IPropertyDefaultValueProvider _propertyDefaultValueProvider;

    public ReflectionBasedMappingObjectFactory (
        IMemberInformationNameResolver nameResolver,
        IClassIDProvider classIDProvider,
        IPropertyMetadataProvider propertyMetadataProvider,
        IDomainModelConstraintProvider domainModelConstraintProvider,
        IPropertyDefaultValueProvider propertyDefaultValueProvider,
        ISortExpressionDefinitionProvider sortExpressionDefinitionProvider,
        IDomainObjectCreator instanceCreator)
    {
      ArgumentUtility.CheckNotNull("nameResolver", nameResolver);
      ArgumentUtility.CheckNotNull("classIDProvider", classIDProvider);
      ArgumentUtility.CheckNotNull("propertyMetadataProvider", propertyMetadataProvider);
      ArgumentUtility.CheckNotNull("domainModelConstraintProvider", domainModelConstraintProvider);
      ArgumentUtility.CheckNotNull("propertyDefaultValueProvider", propertyDefaultValueProvider);
      ArgumentUtility.CheckNotNull("sortExpressionDefinitionProvider", sortExpressionDefinitionProvider);
      ArgumentUtility.CheckNotNull("instanceCreator", instanceCreator);

      _nameResolver = nameResolver;
      _classIDProvider = classIDProvider;
      _propertyMetadataProvider = propertyMetadataProvider;
      _domainModelConstraintProvider = domainModelConstraintProvider;
      _sortExpressionDefinitionProvider = sortExpressionDefinitionProvider;
      _instanceCreator = instanceCreator;
      _propertyDefaultValueProvider = propertyDefaultValueProvider;
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

    public PropertyDefinition CreatePropertyDefinition (TypeDefinition typeDefinition, IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);
      ArgumentUtility.CheckNotNull("propertyInfo", propertyInfo);

      var propertyReflector = new PropertyReflector(
          typeDefinition,
          propertyInfo,
          _nameResolver,
          _propertyMetadataProvider,
          _domainModelConstraintProvider,
          _propertyDefaultValueProvider);
      return propertyReflector.GetMetadata();
    }

    public RelationDefinition CreateRelationDefinition (
        IDictionary<Type, TypeDefinition> typeDefinitions, TypeDefinition typeDefinition, IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull("typeDefinitions", typeDefinitions);
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);
      ArgumentUtility.CheckNotNull("propertyInfo", propertyInfo);

      var relationReflector = new RelationReflector(typeDefinition, propertyInfo, _nameResolver, _propertyMetadataProvider);
      return relationReflector.GetMetadata(typeDefinitions);
    }

    public IRelationEndPointDefinition CreateRelationEndPointDefinition (TypeDefinition typeDefinition, IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);
      ArgumentUtility.CheckNotNull("propertyInfo", propertyInfo);

      var relationEndPointReflector = RelationEndPointReflector.CreateRelationEndPointReflector(
          typeDefinition,
          propertyInfo,
          _nameResolver,
          _propertyMetadataProvider,
          _domainModelConstraintProvider,
          _sortExpressionDefinitionProvider);
      return relationEndPointReflector.GetMetadata();
    }

    public TypeDefinition[] CreateTypeDefinitionCollection (IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull("types", types);

      var classDefinitionCollectionFactory = new ClassDefinitionCollectionFactory(this);
      return classDefinitionCollectionFactory.CreateClassDefinitionCollection(types);
    }

    public PropertyDefinitionCollection CreatePropertyDefinitionCollection (
        TypeDefinition typeDefinition, IEnumerable<IPropertyInformation> propertyInfos)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);
      ArgumentUtility.CheckNotNull("propertyInfos", propertyInfos);

      var factory = new PropertyDefinitionCollectionFactory(this);
      return factory.CreatePropertyDefinitions(typeDefinition, propertyInfos);
    }

    public RelationDefinition[] CreateRelationDefinitionCollection (IDictionary<Type, TypeDefinition> typeDefinitions)
    {
      ArgumentUtility.CheckNotNull("typeDefinitions", typeDefinitions);

      var factory = new RelationDefinitionCollectionFactory(this);
      return factory.CreateRelationDefinitionCollection(typeDefinitions);
    }

    public RelationEndPointDefinitionCollection CreateRelationEndPointDefinitionCollection (TypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      var factory = new RelationEndPointDefinitionCollectionFactory(this, _nameResolver, _propertyMetadataProvider);
      return factory.CreateRelationEndPointDefinitionCollection(typeDefinition);
    }
  }
}
