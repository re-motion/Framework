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
using System.Reflection;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.TableInheritance;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;

namespace Remotion.Data.DomainObjects.UnitTests.Factories
{
  public abstract class BaseConfiguration
  {
    private const string c_databaseDisabledConnectionString = "DATABASE_DISABLED_IN_UNITEST";

    public static ITypeDiscoveryService GetTypeDiscoveryService (params Assembly[] rootAssemblies)
    {
      var rootAssemblyFinder = new FixedRootAssemblyFinder(rootAssemblies.Select(asm => new RootAssembly(asm, true)).ToArray());
      var assemblyLoader = new FilteringAssemblyLoader(ApplicationAssemblyLoaderFilter.Instance);
      var assemblyFinder = new CachingAssemblyFinderDecorator(new AssemblyFinder(rootAssemblyFinder, assemblyLoader));
      ITypeDiscoveryService typeDiscoveryService = new AssemblyFinderTypeDiscoveryService(assemblyFinder);

      var whitelistedNamespaces = new[]
                                  {
                                      "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.ReflectionBasedPropertyResolver",
                                      "Remotion.Data.DomainObjects.UnitTests.TestDomain",
                                      "Remotion.Data.DomainObjects.UnitTests.DataManagement.TestDomain",
                                      "Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain",
                                      "Remotion.Data.DomainObjects.UnitTests.Linq.TestDomain"
                                  };
      return new FilteringTypeDiscoveryService(
          typeDiscoveryService,
          type =>
          {
            var @namespace = type.Namespace ?? string.Empty;
            if (whitelistedNamespaces.Any(t => @namespace.StartsWith(t)))
              return true;

            return Attribute.IsDefined(type, typeof(IncludeInMappingTestDomainAttribute));
          });
    }

    private readonly StorageConfiguration _storageConfiguration;
    private readonly MappingLoaderConfiguration _mappingLoaderConfiguration;
    private readonly QueryConfiguration _queryConfiguration;
    private readonly MappingConfiguration _mappingConfiguration;

    private readonly Dictionary<RdbmsProviderDefinition, string> _originalConnectionStrings = new();

    protected BaseConfiguration ()
    {
      ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = StorageProviderDefinitionObjectMother.CreateTestDomainStorageProviders();

      _storageConfiguration = new StorageConfiguration(
          storageProviderDefinitionCollection,
          storageProviderDefinitionCollection[DatabaseTest.DefaultStorageProviderID]);

      _storageConfiguration.StorageGroups.Add(
          new StorageGroupElement(
              new TestDomainAttribute(),
              DatabaseTest.c_testDomainProviderID));
      _storageConfiguration.StorageGroups.Add(
          new StorageGroupElement(
              new NonPersistentTestDomainAttribute(),
              DatabaseTest.c_nonPersistentTestDomainProviderID));
      _storageConfiguration.StorageGroups.Add(
          new StorageGroupElement(
              new StorageProviderStubAttribute(),
              DatabaseTest.c_unitTestStorageProviderStubID));
      _storageConfiguration.StorageGroups.Add(
          new StorageGroupElement(
              new TableInheritanceTestDomainAttribute(),
              TableInheritanceMappingTest.TableInheritanceTestDomainProviderID));

      _mappingLoaderConfiguration = new MappingLoaderConfiguration();
      _queryConfiguration = new QueryConfiguration("QueriesForStandardMapping.xml");

      var typeDiscoveryService = GetTypeDiscoveryService(GetType().Assembly);

      _mappingConfiguration = new MappingConfiguration(
          MappingReflectorObjectMother.CreateMappingReflector(typeDiscoveryService),
          new PersistenceModelLoader(new StorageGroupBasedStorageProviderDefinitionFinder(_storageConfiguration)));
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
      return new FakeDomainObjectsConfiguration(_mappingLoaderConfiguration, _storageConfiguration, _queryConfiguration);
    }

    public void DisableDatabaseAccess ()
    {
      foreach (var rdbmsProviderDefinition in _storageConfiguration.StorageProviderDefinitions.OfType<RdbmsProviderDefinition>())
      {
        var connectionString = rdbmsProviderDefinition.ConnectionString;
        var isConnectionStringSet = connectionString != c_databaseDisabledConnectionString;
        if (isConnectionStringSet)
        {
          _originalConnectionStrings[rdbmsProviderDefinition] = connectionString;
          PrivateInvoke.SetNonPublicField(rdbmsProviderDefinition, "_connectionString", c_databaseDisabledConnectionString);
        }
      }
    }

    public void EnableDatabaseAccess ()
    {
      foreach (var rdbmsProviderDefinition in _storageConfiguration.StorageProviderDefinitions.OfType<RdbmsProviderDefinition>())
      {
        if (_originalConnectionStrings.TryGetValue(rdbmsProviderDefinition, out var originalConnectionString))
          PrivateInvoke.SetNonPublicField(rdbmsProviderDefinition, "_connectionString", originalConnectionString);
      }
    }
  }
}
