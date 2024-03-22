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
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Development.Data.UnitTesting.DomainObjects.Configuration;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection.TypeDiscovery;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  public class SchemaGenerationConfiguration
  {
    private static SchemaGenerationConfiguration s_instance;

    public static SchemaGenerationConfiguration Instance
    {
      get
      {
        if (s_instance == null)
        {
          Debugger.Break();
          throw new InvalidOperationException("SchemaGenerationConfiguration has not been Initialized by invoking Initialize()");
        }
        return s_instance;
      }
    }

    public static void Initialize ()
    {
      s_instance = new SchemaGenerationConfiguration();
    }

    private readonly MappingConfiguration _mappingConfiguration;
    private readonly IStorageSettings _storageSettings;

    public SchemaGenerationConfiguration ()
    {
      var fakeStorageObjectFactoryFactory = new FakeStorageObjectFactoryFactory();
      var fakeStorageSettingsFactory = new FakeStorageSettingsFactory();
      var fakeStorageSettingsFactoryResolver = new FakeStorageSettingsFactoryResolver(fakeStorageSettingsFactory);

      var deferredStorageSettings = new DeferredStorageSettings(fakeStorageObjectFactoryFactory, fakeStorageSettingsFactoryResolver);

      var sqlStorageObjectFactory = new SqlStorageObjectFactory(
          deferredStorageSettings,
          SafeServiceLocator.Current.GetInstance<ITypeConversionProvider>(),
          SafeServiceLocator.Current.GetInstance<IDataContainerValidator>());

      var defaultStorageProviderDefinition = new RdbmsProviderDefinition(
          DatabaseTest.SchemaGenerationFirstStorageProviderID,
          sqlStorageObjectFactory,
          DatabaseTest.SchemaGenerationConnectionString1,
          new[] { typeof(FirstStorageGroupAttribute) });

      var storageProviderDefinitionCollection = new List<StorageProviderDefinition>
                                                {
                                                  defaultStorageProviderDefinition,
                                                  new RdbmsProviderDefinition(
                                                      DatabaseTest.SchemaGenerationSecondStorageProviderID,
                                                      sqlStorageObjectFactory,
                                                      DatabaseTest.SchemaGenerationConnectionString2,
                                                      new[] { typeof(SecondStorageGroupAttribute) }),
                                                  new RdbmsProviderDefinition(
                                                      DatabaseTest.SchemaGenerationThirdStorageProviderID,
                                                      sqlStorageObjectFactory,
                                                      DatabaseTest.SchemaGenerationConnectionString3,
                                                      new[] { typeof(ThirdStorageGroupAttribute) }),
                                                  new RdbmsProviderDefinition(
                                                      DatabaseTest.SchemaGenerationInternalStorageProviderID,
                                                      sqlStorageObjectFactory,
                                                      DatabaseTest.SchemaGenerationConnectionString1,
                                                      new[] { typeof(InternalStorageGroupAttribute) })
                                                };

      _storageSettings = new StorageSettings(
          defaultStorageProviderDefinition,
          storageProviderDefinitionCollection);

      fakeStorageObjectFactoryFactory.SetUp(sqlStorageObjectFactory);
      fakeStorageSettingsFactory.SetUp(_storageSettings);

      var typeDiscoveryService = GetTypeDiscoveryService(GetType().Assembly);

      _mappingConfiguration = MappingConfiguration.Create(
            MappingReflectorObjectMother.CreateMappingReflector(typeDiscoveryService),
            new PersistenceModelLoader(_storageSettings));
    }

    public MappingConfiguration GetMappingConfiguration ()
    {
      return _mappingConfiguration;
    }

    public IStorageSettings GetStorageSettings ()
    {
      return _storageSettings;
    }

    public ITypeDiscoveryService GetTypeDiscoveryService (params Assembly[] rootAssemblies)
    {
      var baseTypeDiscoveryService = new FixedTypeDiscoveryService(Assembly.GetExecutingAssembly().GetTypes());

      return FilteringTypeDiscoveryService.CreateFromNamespaceWhitelist(
          baseTypeDiscoveryService,
          "Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain");
    }

  }
}
