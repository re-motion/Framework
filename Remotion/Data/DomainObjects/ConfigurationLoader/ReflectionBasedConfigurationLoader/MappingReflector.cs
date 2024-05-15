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
using System.ComponentModel.Design;
using System.Linq;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Data.DomainObjects.Mapping.Validation.Reflection;
using Remotion.Logging;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  [ImplementationFor(typeof(IMappingLoader), Lifetime = LifetimeKind.Singleton)]
  public class MappingReflector : IMappingLoader
  {
    public static MappingReflector Create (
        ITypeDiscoveryService typeDiscoveryService,
        IClassIDProvider classIDProvider,
        IMemberInformationNameResolver nameResolver,
        IPropertyMetadataProvider propertyMetadataProvider,
        IDomainModelConstraintProvider domainModelConstraintProvider,
        IPropertyDefaultValueProvider propertyDefaultValueProvider,
        ISortExpressionDefinitionProvider sortExpressionDefinitionProvider,
        IDomainObjectCreator domainObjectCreator)
    {
      ArgumentUtility.CheckNotNull("typeDiscoveryService", typeDiscoveryService);
      ArgumentUtility.CheckNotNull("classIDProvider", classIDProvider);
      ArgumentUtility.CheckNotNull("propertyMetadataProvider", propertyMetadataProvider);
      ArgumentUtility.CheckNotNull("domainModelConstraintProvider", domainModelConstraintProvider);
      ArgumentUtility.CheckNotNull("propertyDefaultValueProvider", propertyDefaultValueProvider);
      ArgumentUtility.CheckNotNull("sortExpressionDefinitionProvider", sortExpressionDefinitionProvider);
      ArgumentUtility.CheckNotNull("nameResolver", nameResolver);
      ArgumentUtility.CheckNotNull("domainObjectCreator", domainObjectCreator);

      return new MappingReflector(
          typeDiscoveryService,
          classIDProvider,
          nameResolver,
          propertyMetadataProvider,
          domainModelConstraintProvider,
          propertyDefaultValueProvider,
          sortExpressionDefinitionProvider,
          domainObjectCreator);
    }

    private static readonly ILog s_log = LogManager.GetLogger(typeof(MappingReflector));
    private readonly IMemberInformationNameResolver _nameResolver;
    private readonly IMappingObjectFactory _mappingObjectFactory;
    private readonly ITypeDiscoveryService _typeDiscoveryService;
    private readonly IClassIDProvider _classIDProvider;
    private readonly IPropertyMetadataProvider _propertyMetadataProvider;
    private readonly IDomainModelConstraintProvider _domainModelConstraintProvider;
    private readonly ISortExpressionDefinitionProvider _sortExpressionDefinitionProvider;
    private readonly IPropertyDefaultValueProvider _propertyDefaultValueProvider;

    public MappingReflector (
        IClassIDProvider classIDProvider,
        IMemberInformationNameResolver nameResolver,
        IPropertyMetadataProvider propertyMetadataProvider,
        IDomainModelConstraintProvider domainModelConstraintProvider,
        IPropertyDefaultValueProvider propertyDefaultValueProvider,
        ISortExpressionDefinitionProvider sortExpressionDefinitionProvider,
        IDomainObjectCreator domainObjectCreator)
        : this(
            ContextAwareTypeUtility.GetTypeDiscoveryService(),
            classIDProvider,
            nameResolver,
            propertyMetadataProvider,
            domainModelConstraintProvider,
            propertyDefaultValueProvider,
            sortExpressionDefinitionProvider,
            domainObjectCreator)
    {
    }

    private MappingReflector (
        ITypeDiscoveryService typeDiscoveryService,
        IClassIDProvider classIDProvider,
        IMemberInformationNameResolver nameResolver,
        IPropertyMetadataProvider propertyMetadataProvider,
        IDomainModelConstraintProvider domainModelConstraintProvider,
        IPropertyDefaultValueProvider propertyDefaultValueProvider,
        ISortExpressionDefinitionProvider sortExpressionDefinitionProvider,
        IDomainObjectCreator domainObjectCreator)
    {
      ArgumentUtility.CheckNotNull("typeDiscoveryService", typeDiscoveryService);
      ArgumentUtility.CheckNotNull("classIDProvider", classIDProvider);
      ArgumentUtility.CheckNotNull("propertyMetadataProvider", propertyMetadataProvider);
      ArgumentUtility.CheckNotNull("domainModelConstraintProvider", domainModelConstraintProvider);
      ArgumentUtility.CheckNotNull("propertyDefaultValueProvider", propertyDefaultValueProvider);
      ArgumentUtility.CheckNotNull("sortExpressionDefinitionProvider", sortExpressionDefinitionProvider);
      ArgumentUtility.CheckNotNull("nameResolver", nameResolver);
      ArgumentUtility.CheckNotNull("domainObjectCreator", domainObjectCreator);

      _typeDiscoveryService = typeDiscoveryService;
      _classIDProvider = classIDProvider;
      _propertyMetadataProvider = propertyMetadataProvider;
      _domainModelConstraintProvider = domainModelConstraintProvider;
      _sortExpressionDefinitionProvider = sortExpressionDefinitionProvider;
      _nameResolver = nameResolver;
      _propertyDefaultValueProvider = propertyDefaultValueProvider;
      _mappingObjectFactory = new ReflectionBasedMappingObjectFactory(
          _nameResolver,
          _classIDProvider,
          _propertyMetadataProvider,
          _domainModelConstraintProvider,
          _propertyDefaultValueProvider,
          _sortExpressionDefinitionProvider,
          domainObjectCreator);
    }

    public ITypeDiscoveryService TypeDiscoveryService
    {
      get { return _typeDiscoveryService; }
    }

    public ClassDefinition[] GetClassDefinitions ()
    {
      s_log.Info("Reflecting class definitions...");

      using (StopwatchScope.CreateScope(s_log, LogLevel.Info, "Time needed to reflect class definitions: {elapsed}."))
      {
        var types = GetDomainObjectTypesSorted();
        var classDefinitions = MappingObjectFactory.CreateClassDefinitionCollection(types);

        return classDefinitions
            .LogAndReturnValue(s_log, LogLevel.Info, result => string.Format("Generated {0} class definitions.", result.Length));
      }
    }

    public RelationDefinition[] GetRelationDefinitions (IDictionary<Type, ClassDefinition> classDefinitions)
    {
      ArgumentUtility.CheckNotNull("classDefinitions", classDefinitions);
      s_log.InfoFormat("Reflecting relation definitions of {0} class definitions...", classDefinitions.Count);

      using (StopwatchScope.CreateScope(s_log, LogLevel.Info, "Time needed to reflect relation definitions: {elapsed}."))
      {
        var relationDefinitions = MappingObjectFactory.CreateRelationDefinitionCollection(classDefinitions);
        return relationDefinitions
            .LogAndReturnValue(s_log, LogLevel.Info, result => string.Format("Generated {0} relation definitions.", result.Length));
      }
    }

    public TupleDefinition[] GetTupleDefinitions ()
    {
      s_log.Info("Reflecting tuple definitions...");

      using (StopwatchScope.CreateScope(s_log, LogLevel.Info, "Time needed to reflect tuple definitions: {elapsed}."))
      {
        var tupleDefinitions = new List<TupleDefinition>();
        foreach (var (type, maxLength) in GetSimpleTypes())
        {
          var tupleDefinition = new TupleDefinition(type.Name, type, null);
          tupleDefinition.SetTuplePropertyDefinitions(new[] { new TuplePropertyDefinition(tupleDefinition, "Value", new ValuePropertyInformation(type), true, maxLength) });
          tupleDefinitions.Add(tupleDefinition);
        }

        var types = GetTupleTypesSorted();
        tupleDefinitions.AddRange(MappingObjectFactory.CreateTupleDefinitionCollection(types));

        return tupleDefinitions.ToArray()
            .LogAndReturnValue(s_log, LogLevel.Info, result => $"Generated {result.Length} tuple definitions.");
      }
    }

    private IEnumerable<(Type, int)> GetSimpleTypes ()
    {
      yield return (typeof(string), -1);
      yield return (typeof(int), 0);
    }

    private IEnumerable<Type> GetDomainObjectTypes ()
    {
      return (from type in _typeDiscoveryService.GetTypes(typeof(DomainObject), excludeGlobalTypes: false).Cast<Type>()
              where !ReflectionUtility.IsTypeIgnoredForMappingConfiguration(type)
              select type).Distinct();
    }

    private IEnumerable<Type> GetTupleTypes ()
    {
      return (from type in _typeDiscoveryService.GetTypes(typeof(IStructuredType), excludeGlobalTypes: false).Cast<Type>()
              where !ReflectionUtility.IsTypeIgnoredForMappingConfiguration(type)
              select type).Distinct();
    }

    private Type[] GetDomainObjectTypesSorted ()
    {
      return GetDomainObjectTypes().OrderBy(t => t.GetFullNameChecked(), StringComparer.OrdinalIgnoreCase).ToArray();
    }

    private Type[] GetTupleTypesSorted ()
    {
      return GetTupleTypes().OrderBy(t => t.GetFullNameChecked(), StringComparer.OrdinalIgnoreCase).ToArray();
    }

    bool IMappingLoader.ResolveTypes
    {
      get { return true; }
    }

    public IMemberInformationNameResolver NameResolver
    {
      get { return _nameResolver; }
    }

    public IMappingObjectFactory MappingObjectFactory
    {
      get { return _mappingObjectFactory; }
    }

    public IClassDefinitionValidator CreateClassDefinitionValidator ()
    {
      return new ClassDefinitionValidator(
          new DomainObjectTypeDoesNotHaveLegacyInfrastructureConstructorValidationRule(),
          new DomainObjectTypeIsNotGenericValidationRule(),
          new InheritanceHierarchyFollowsClassHierarchyValidationRule(),
          new StorageGroupAttributeIsOnlyDefinedOncePerInheritanceHierarchyValidationRule(),
          new ClassDefinitionTypeIsSubclassOfDomainObjectValidationRule(),
          new StorageGroupTypesAreSameWithinInheritanceTreeRule(),
          new CheckForClassIDIsValidValidationRule());
    }

    public IPropertyDefinitionValidator CreatePropertyDefinitionValidator ()
    {
      return new PropertyDefinitionValidator(
          new MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule(_nameResolver, _propertyMetadataProvider),
          new MappingAttributesAreSupportedForPropertyTypeValidationRule(),
          new StorageClassIsSupportedValidationRule(),
          new PropertyTypeIsSupportedValidationRule(),
          new MandatoryNetEnumTypeHasValuesDefinedValidationRule(),
          new MandatoryExtensibleEnumTypeHasValuesDefinedValidationRule());
    }

    public IRelationDefinitionValidator CreateRelationDefinitionValidator ()
    {
      return new RelationDefinitionValidator(
          new RdbmsRelationEndPointCombinationIsSupportedValidationRule(),
          new SortExpressionIsSupportedForCardinalityOfRelationPropertyValidationRule(),
          new VirtualRelationEndPointCardinalityMatchesPropertyTypeValidationRule(),
          new VirtualRelationEndPointPropertyTypeIsSupportedValidationRule(),
          new ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule(),
          new RelationEndPointPropertyTypeIsSupportedValidationRule(),
          new RelationEndPointNamesAreConsistentValidationRule(),
          new RelationEndPointTypesAreConsistentValidationRule(),
          new CheckForInvalidRelationEndPointsValidationRule(),
          new CheckForTypeNotFoundClassDefinitionValidationRule());
    }

    public ISortExpressionValidator CreateSortExpressionValidator ()
    {
      return new SortExpressionValidator(new SortExpressionIsValidValidationRule());
    }
  }
}
