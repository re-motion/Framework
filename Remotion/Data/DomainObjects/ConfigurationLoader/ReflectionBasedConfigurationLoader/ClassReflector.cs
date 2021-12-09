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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// The <see cref="ClassReflector"/> is used to build a <see cref="ClassDefinition"/>.
  /// </summary>
  /// <remarks>Derived classes must have a cosntructor with a matching the <see cref="ClassReflector"/>'s constructor signature. </remarks>
  public class ClassReflector
  {
    private readonly Type _type;
    private readonly IMappingObjectFactory _mappingObjectFactory;
    private readonly IMemberInformationNameResolver _nameResolver;
    private readonly IClassIDProvider _classIDProvider;
    private readonly IPropertyMetadataProvider _propertyMetadataProvider;
    private readonly IDomainModelConstraintProvider _domainModelConstraintProvider;
    private readonly ISortExpressionDefinitionProvider _sortExpressionDefinitionProvider;
    private readonly IDomainObjectCreator _instanceCreator;

    public ClassReflector (
        Type type,
        IMappingObjectFactory mappingObjectFactory,
        IMemberInformationNameResolver nameResolver,
        IClassIDProvider classIDProvider,
        IPropertyMetadataProvider propertyMetadataProvider,
        IDomainModelConstraintProvider domainModelConstraintProvider,
        ISortExpressionDefinitionProvider sortExpressionDefinitionProvider,
        IDomainObjectCreator instanceCreator)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("type", type, typeof(DomainObject));
      ArgumentUtility.CheckNotNull("mappingObjectFactory", mappingObjectFactory);
      ArgumentUtility.CheckNotNull("nameResolver", nameResolver);
      ArgumentUtility.CheckNotNull("classIDProvider", classIDProvider);
      ArgumentUtility.CheckNotNull("propertyMetadataProvider", propertyMetadataProvider);
      ArgumentUtility.CheckNotNull("domainModelConstraintProvider", domainModelConstraintProvider);
      ArgumentUtility.CheckNotNull("sortExpressionDefinitionProvider", sortExpressionDefinitionProvider);
      ArgumentUtility.CheckNotNull("instanceCreator", instanceCreator);

      _type = type;
      _mappingObjectFactory = mappingObjectFactory;
      _nameResolver = nameResolver;
      _classIDProvider = classIDProvider;
      _propertyMetadataProvider = propertyMetadataProvider;
      _domainModelConstraintProvider = domainModelConstraintProvider;
      _sortExpressionDefinitionProvider = sortExpressionDefinitionProvider;
      _instanceCreator = instanceCreator;
    }

    public Type Type
    {
      get { return _type; }
    }

    public IMappingObjectFactory MappingObjectFactory
    {
      get { return _mappingObjectFactory; }
    }

    public IMemberInformationNameResolver NameResolver
    {
      get { return _nameResolver; }
    }

    public ClassDefinition GetMetadata (ClassDefinition? baseClassDefinition)
    {
      var persistentMixinFinder = new PersistentMixinFinder(Type, baseClassDefinition == null);
      var storageGroupAttribute = GetStorageGroupAttribute();
      var storageGroupType = storageGroupAttribute?.GetType();
      var defaultStorageClass = storageGroupAttribute?.DefaultStorageClass ?? DefaultStorageClass.Persistent;
      var classDefinition = new ClassDefinition(
              _classIDProvider.GetClassID(Type),
              Type,
              IsAbstract(),
              baseClassDefinition,
              storageGroupType,
              defaultStorageClass,
              persistentMixinFinder,
              _instanceCreator);

      var properties = MappingObjectFactory.CreatePropertyDefinitionCollection(classDefinition, GetPropertyInfos(classDefinition));
      classDefinition.SetPropertyDefinitions(properties);
      var endPoints = MappingObjectFactory.CreateRelationEndPointDefinitionCollection(classDefinition);
      classDefinition.SetRelationEndPointDefinitions(endPoints);

      return classDefinition;
    }

    private StorageGroupAttribute? GetStorageGroupAttribute ()
    {
      return AttributeUtility.GetCustomAttributes<StorageGroupAttribute>(Type, true).FirstOrDefault();
    }

    private bool IsAbstract ()
    {
      if (Type.IsAbstract)
        return !Attribute.IsDefined(Type, typeof(InstantiableAttribute), false);

      return false;
    }

    private IEnumerable<IPropertyInformation> GetPropertyInfos (ClassDefinition classDefinition)
    {
      PropertyFinder propertyFinder = new PropertyFinder(
          classDefinition.Type,
          classDefinition,
          classDefinition.BaseClass == null,
          true,
          _nameResolver,
          classDefinition.PersistentMixinFinder,
          _propertyMetadataProvider,
          _domainModelConstraintProvider,
          _sortExpressionDefinitionProvider);
      return propertyFinder.FindPropertyInfos();
    }
  }
}
