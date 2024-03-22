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
using System.Diagnostics;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.NonPersistent;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.TableInheritance;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Development.Data.UnitTesting.DomainObjects.Configuration;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  public class TestMappingConfiguration
  {
    private static TestMappingConfiguration s_instance;

    private readonly IStorageSettings _storageSettings;
    private readonly MappingConfiguration _mappingConfiguration;
    private readonly DomainObjectIDs _domainObjectIDs;

    public static TestMappingConfiguration Instance
    {
      get
      {
        if (s_instance == null)
        {
          Debugger.Break();
          throw new InvalidOperationException($"TestMappingConfiguration has not been Initialized by invoking {nameof(EnsureInitialized)}()");
        }
        return s_instance;
      }
    }

    public static void EnsureInitialized ()
    {
      if (s_instance != null)
        return;

      s_instance = new TestMappingConfiguration();
    }

    protected TestMappingConfiguration ()
    {
      _storageSettings = CreateStorageSettings();

      var typeDiscoveryService = GetTypeDiscoveryService();

      _mappingConfiguration = MappingConfiguration.Create(
          MappingReflectorObjectMother.CreateMappingReflector(typeDiscoveryService),
          new PersistenceModelLoader(_storageSettings));
      MappingConfiguration.SetCurrent(_mappingConfiguration);

      _domainObjectIDs = new DomainObjectIDs();
    }

    private static IStorageSettings CreateStorageSettings ()
    {
      var fakeStorageObjectFactoryFactory = new FakeStorageObjectFactoryFactory();
      var fakeStorageSettingsFactory = new FakeStorageSettingsFactory();
      var fakeStorageSettingsFactoryResolver = new FakeStorageSettingsFactoryResolver(fakeStorageSettingsFactory);

      var deferredStorageSettings = new DeferredStorageSettings(fakeStorageObjectFactoryFactory, fakeStorageSettingsFactoryResolver);

      var sqlStorageObjectFactory = new SqlStorageObjectFactory(
          deferredStorageSettings,
          SafeServiceLocator.Current.GetInstance<ITypeConversionProvider>(),
          SafeServiceLocator.Current.GetInstance<IDataContainerValidator>());

      var nonPersistentStorageObjectFactory = new NonPersistentStorageObjectFactory();

      var storageProviderDefinitionCollection = new List<StorageProviderDefinition>();

      var defaultStorageProvider = new RdbmsProviderDefinition(
          MappingReflectionTestBase.DefaultStorageProviderID,
          sqlStorageObjectFactory,
          connectionString: "Mapping TestDomain ConnectionString");
      storageProviderDefinitionCollection.Add(defaultStorageProvider);

      storageProviderDefinitionCollection.Add(
          new RdbmsProviderDefinition(
              MappingReflectionTestBase.c_testDomainProviderID,
              sqlStorageObjectFactory,
              connectionString: "Mapping TestDomain ConnectionString",
              assignedStorageGroups: new[] { typeof(TestDomainAttribute) }));

      storageProviderDefinitionCollection.Add(
          new UnitTestStorageProviderStubDefinition(
              MappingReflectionTestBase.c_unitTestStorageProviderStubID,
              assignedStorageGroups: new[] { typeof(StorageProviderStubAttribute) }));

      storageProviderDefinitionCollection.Add(
          new RdbmsProviderDefinition(
              TableInheritanceMappingTest.TableInheritanceTestDomainProviderID,
              sqlStorageObjectFactory,
              connectionString: "Mapping TestDomain ConnectionString",
              assignedStorageGroups: new[] { typeof(TableInheritanceTestDomainAttribute) }));

      storageProviderDefinitionCollection.Add(
          new NonPersistentProviderDefinition(
              MappingReflectionTestBase.c_nonPersistentTestDomainProviderID,
              nonPersistentStorageObjectFactory,
              assignedStorageGroups: new[] { typeof(NonPersistentTestDomainAttribute) }));

      var storageSettings = new StorageSettings(
          defaultStorageProvider,
          storageProviderDefinitionCollection);

      fakeStorageObjectFactoryFactory.SetUp(sqlStorageObjectFactory);
      fakeStorageSettingsFactory.SetUp(storageSettings);

      return storageSettings;
    }

    public static ITypeDiscoveryService GetTypeDiscoveryService ()
    {
      var mappingRootNamespace = typeof(TestMappingConfiguration).Namespace;
      var testMappingNamespace = mappingRootNamespace + ".TestDomain.Integration";

      var rootAssemlbies = new[] { new RootAssembly(typeof(TestMappingConfiguration).Assembly, true) };
      var rootAssemblyFinder = new FixedRootAssemblyFinder(rootAssemlbies);
      var assemblyLoader = new FilteringAssemblyLoader(ApplicationAssemblyLoaderFilter.Instance);
      var assemblyFinder = new CachingAssemblyFinderDecorator(new AssemblyFinder(rootAssemblyFinder, assemblyLoader));
      var typeDiscoveryService = (ITypeDiscoveryService)new AssemblyFinderTypeDiscoveryService(assemblyFinder);
      typeDiscoveryService = FilteringTypeDiscoveryService.CreateFromNamespaceWhitelist(typeDiscoveryService, testMappingNamespace);

      return typeDiscoveryService;
    }

    public MappingConfiguration GetMappingConfiguration ()
    {
      return _mappingConfiguration;
    }

    public IStorageSettings GetStorageSettings ()
    {
      return _storageSettings;
    }

    public DomainObjectIDs GetDomainObjectIDs ()
    {
      return _domainObjectIDs;
    }
  }
}
