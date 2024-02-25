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
using System.Linq;
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.NonPersistent;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
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

namespace Remotion.Data.DomainObjects.UnitTests.Factories
{
  public sealed class BaseConfiguration
  {
    private const string c_databaseDisabledConnectionString = "DATABASE_DISABLED_IN_UNITEST";

    private static BaseConfiguration s_instance;

    public static BaseConfiguration Instance
    {
      get
      {
        if (s_instance == null)
        {
          Debugger.Break();
          throw new InvalidOperationException("BaseConfiguration has not been Initialized by invoking Initialize()");
        }
        return s_instance;
      }
    }

    public static void EnsureInitialized ()
    {
      if (s_instance != null)
        return;

      s_instance = new BaseConfiguration();
      s_instance.DisableDatabaseAccess();
    }

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

    private readonly IStorageSettings _storageSettings;
    private readonly MappingConfiguration _mappingConfiguration;
    private readonly FakeStorageSettingsFactory _storageSettingsFactory;

    private readonly Dictionary<RdbmsProviderDefinition, string> _originalConnectionStrings = new();

    private BaseConfiguration ()
    {
      _storageSettingsFactory = CreateStorageSettings();
      _storageSettings = _storageSettingsFactory.StorageSettings;

      var typeDiscoveryService = GetTypeDiscoveryService(GetType().Assembly);

      _mappingConfiguration = MappingConfiguration.Create(
            MappingReflectorObjectMother.CreateMappingReflector(typeDiscoveryService),
            new PersistenceModelLoader(_storageSettings));
    }

    private FakeStorageSettingsFactory CreateStorageSettings ()
    {
      var storageProviderDefinitionCollection = new List<StorageProviderDefinition>();

      var fakeStorageObjectFactoryFactory = new FakeStorageObjectFactoryFactory();
      var fakeStorageSettingsFactory = new FakeStorageSettingsFactory();
      var fakeStorageSettingsFactoryResolver = new FakeStorageSettingsFactoryResolver(fakeStorageSettingsFactory);

      var deferredStorageSettings = new DeferredStorageSettings(fakeStorageObjectFactoryFactory, fakeStorageSettingsFactoryResolver);

      var sqlStorageObjectFactory = new SqlStorageObjectFactory(
          deferredStorageSettings,
          SafeServiceLocator.Current.GetInstance<ITypeConversionProvider>(),
          SafeServiceLocator.Current.GetInstance<IDataContainerValidator>());

      var nonPersistentStorageObjectFactory = new NonPersistentStorageObjectFactory();

      var defaultStorageProvider = new RdbmsProviderDefinition(
          DatabaseTest.DefaultStorageProviderID,
          sqlStorageObjectFactory,
          DatabaseTest.TestDomainConnectionString);
      storageProviderDefinitionCollection.Add(defaultStorageProvider);

      storageProviderDefinitionCollection.Add(
          new RdbmsProviderDefinition(
              DatabaseTest.c_testDomainProviderID,
              sqlStorageObjectFactory,
              DatabaseTest.TestDomainConnectionString,
              assignedStorageGroups: new[] { typeof(TestDomainAttribute) }));

      storageProviderDefinitionCollection.Add(
          new NonPersistentProviderDefinition(
              DatabaseTest.c_nonPersistentTestDomainProviderID,
              nonPersistentStorageObjectFactory,
              assignedStorageGroups: new[] { typeof(NonPersistentTestDomainAttribute) }));

      storageProviderDefinitionCollection.Add(
          new UnitTestStorageProviderStubDefinition(
              DatabaseTest.c_unitTestStorageProviderStubID,
              assignedStorageGroups: new[] { typeof(StorageProviderStubAttribute) }));

      storageProviderDefinitionCollection.Add(
          new RdbmsProviderDefinition(
              TableInheritanceMappingTest.TableInheritanceTestDomainProviderID,
              sqlStorageObjectFactory,
              DatabaseTest.TestDomainConnectionString,
              assignedStorageGroups: new[] { typeof(TableInheritanceTestDomainAttribute) }));

      var storageSettings = new StorageSettings(
          defaultStorageProvider,
          storageProviderDefinitionCollection);

      fakeStorageObjectFactoryFactory.SetUp(sqlStorageObjectFactory);
      fakeStorageSettingsFactory.SetUp(storageSettings);

      return fakeStorageSettingsFactory;
    }

    public void Register (DefaultServiceLocator defaultServiceLocator)
    {
      Assertion.IsNotNull(_storageSettingsFactory);

      defaultServiceLocator.RegisterSingle<IStorageSettingsFactory>(() => _storageSettingsFactory);
    }

    public MappingConfiguration GetMappingConfiguration ()
    {
      return _mappingConfiguration;
    }

    public IStorageSettings GetStorageSettings ()
    {
      return _storageSettings;
    }

    public void DisableDatabaseAccess ()
    {
      foreach (var rdbmsProviderDefinition in _storageSettings.GetStorageProviderDefinitions().OfType<RdbmsProviderDefinition>())
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
      foreach (var rdbmsProviderDefinition in _storageSettings.GetStorageProviderDefinitions().OfType<RdbmsProviderDefinition>())
      {
        if (_originalConnectionStrings.TryGetValue(rdbmsProviderDefinition, out var originalConnectionString))
          PrivateInvoke.SetNonPublicField(rdbmsProviderDefinition, "_connectionString", originalConnectionString);
      }
    }
  }
}
