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
using System.ComponentModel.Design;
using System.Diagnostics;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.TableInheritance;
using Remotion.Development.UnitTesting.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  public class TestMappingConfiguration
  {
    private static TestMappingConfiguration s_instance;

    private readonly StorageConfiguration _storageConfiguration;
    private readonly QueryConfiguration _queryConfiguration;
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
      ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = StorageProviderDefinitionObjectMother.CreateTestDomainStorageProviders();
      _storageConfiguration = new StorageConfiguration(
          storageProviderDefinitionCollection, storageProviderDefinitionCollection[MappingReflectionTestBase.DefaultStorageProviderID]);
      _storageConfiguration.StorageGroups.Add(new StorageGroupElement(new TestDomainAttribute(), MappingReflectionTestBase.c_testDomainProviderID));
      _storageConfiguration.StorageGroups.Add(
          new StorageGroupElement(new StorageProviderStubAttribute(), MappingReflectionTestBase.c_unitTestStorageProviderStubID));
      _storageConfiguration.StorageGroups.Add(
          new StorageGroupElement(new TableInheritanceTestDomainAttribute(), TableInheritanceMappingTest.TableInheritanceTestDomainProviderID));
      _storageConfiguration.StorageGroups.Add(
          new StorageGroupElement(new NonPersistentTestDomainAttribute(), MappingReflectionTestBase.c_nonPersistentTestDomainProviderID));

      _queryConfiguration = new QueryConfiguration("QueriesForStandardMapping.xml");
      DomainObjectsConfiguration.SetCurrent(
          new FakeDomainObjectsConfiguration(_storageConfiguration, _queryConfiguration));

      var typeDiscoveryService = GetTypeDiscoveryService();

      _mappingConfiguration = new MappingConfiguration(
          MappingReflectorObjectMother.CreateMappingReflector(typeDiscoveryService),
          new PersistenceModelLoader(new StorageGroupBasedStorageProviderDefinitionFinder(DomainObjectsConfiguration.Current.Storage)));
      MappingConfiguration.SetCurrent(_mappingConfiguration);

      _domainObjectIDs = new DomainObjectIDs();
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

    public StorageConfiguration GetPersistenceConfiguration ()
    {
      return _storageConfiguration;
    }

    public FakeDomainObjectsConfiguration GetDomainObjectsConfiguration ()
    {
      return new FakeDomainObjectsConfiguration(_storageConfiguration, _queryConfiguration);
    }

    public DomainObjectIDs GetDomainObjectIDs ()
    {
      return _domainObjectIDs;
    }
  }
}
