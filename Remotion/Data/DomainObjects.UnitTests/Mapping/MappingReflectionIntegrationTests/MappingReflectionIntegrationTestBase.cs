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
using System.Linq.Expressions;
using System.Reflection;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.MappingReflectionIntegrationTests
{
  public abstract class MappingReflectionIntegrationTestBase
  {
    private static readonly IEnumerable<Type> s_domainObjectTypes =
        typeof(MappingReflectionTestBase).Assembly.GetTypes().Where(t => typeof(DomainObject).IsAssignableFrom(t)).ToArray();

    private IDictionary<Type, TypeDefinition> _typeDefinitions;

    [SetUp]
    public virtual void SetUp ()
    {
      var reflectedTypes = ForTheseReflectedTypes().ToArray();
      var mappingReflector = CreateMappingReflector(reflectedTypes);

      var typeDefinitions = GetTypeDefinitionsAndValidateMapping(mappingReflector).ToDictionary(cd => cd.Type);
      _typeDefinitions = typeDefinitions;
    }

    [OneTimeSetUp]
    public virtual void OneTimeSetUp ()
    {
      TestMappingConfiguration.EnsureInitialized();
      StandardConfiguration.EnsureInitialized();
    }

    protected IDictionary<Type, TypeDefinition> TypeDefinitions
    {
      get { return _typeDefinitions; }
    }

    protected ClassDefinition GetClassDefinition (Type type)
    {
      return (ClassDefinition)TypeDefinitions[type];
    }

    protected virtual IEnumerable<Type> ForTheseReflectedTypes ()
    {
      return AllDomainObjectTypesFromThisNamespace();
    }

    protected IRelationEndPointDefinition GetRelationEndPointDefinition (
        TypeDefinition typeDefinition,
        Type declaringType,
        string shortPropertyName)
    {
      return typeDefinition.GetRelationEndPointDefinition(declaringType.FullName + "." + shortPropertyName);
    }

    private IEnumerable<Type> AllDomainObjectTypesFromThisNamespace ()
    {
      return s_domainObjectTypes.Where(t => t.Namespace == GetType().Namespace);
    }

    private MappingReflector CreateMappingReflector (Type[] reflectedTypes)
    {
      var typeDiscoveryServiceStub = new Mock<ITypeDiscoveryService>();
      typeDiscoveryServiceStub
          .Setup(stub => stub.GetTypes(It.IsAny<Type>(), It.IsAny<bool>()))
          .Returns(reflectedTypes);

      return MappingReflectorObjectMother.CreateMappingReflector(typeDiscoveryServiceStub.Object);
    }

    private IEnumerable<TypeDefinition> GetTypeDefinitionsAndValidateMapping (MappingReflector mappingReflector)
    {
      var storageGroupBasedStorageProviderDefinitionFinder = new StorageGroupBasedStorageProviderDefinitionFinder(
          StandardConfiguration.Instance.GetPersistenceConfiguration());
      var persistenceModelLoader = new PersistenceModelLoader(storageGroupBasedStorageProviderDefinitionFinder);
      return new MappingConfiguration(mappingReflector, persistenceModelLoader).GetTypeDefinitions();
    }

    protected PropertyInfoAdapter GetPropertyInformation (Type declaringType, string propertyName)
    {
      var propertyInfo = declaringType.GetProperty(
          propertyName,
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      Assert.That(propertyInfo, Is.Not.Null, "Property not found: '{0}.{1}'", declaringType, propertyName);
      return PropertyInfoAdapter.Create(propertyInfo);
    }

    protected PropertyInfoAdapter GetPropertyInformation<T, TR> (Expression<Func<T, TR>> propertyAccessExpression)
    {
      var memberInfo = NormalizingMemberInfoFromExpressionUtility.GetProperty(propertyAccessExpression);
      if (memberInfo.DeclaringType != typeof(T))
      {
        var message = string.Format("Property must be declared on type '{0}', but it is declared on '{1}'.", typeof(T), memberInfo.DeclaringType);
        throw new InvalidOperationException(message);
      }

      return PropertyInfoAdapter.Create(memberInfo);
    }
  }
}
