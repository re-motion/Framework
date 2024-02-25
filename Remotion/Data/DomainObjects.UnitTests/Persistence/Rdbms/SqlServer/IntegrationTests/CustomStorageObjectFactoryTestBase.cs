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
using System.Linq;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;
using Remotion.Development.Data.UnitTesting.DomainObjects.Configuration;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Data.SqlClient;
using Remotion.Development.UnitTesting.Reflection.TypeDiscovery;
using Remotion.Reflection;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests
{
  public abstract class CustomStorageObjectFactoryTestBase : DatabaseTest
  {
    public const string CreateEmptyTestDataFileName = "Database\\DataDomainObjects_CreateEmptyTestData.sql";

    private SqlStorageObjectFactory _storageObjectFactory;
    private RdbmsProviderDefinition _storageProviderDefinition;
    private MappingConfiguration _mappingConfiguration;
    private ServiceLocatorScope _serviceLocatorScope;

    protected CustomStorageObjectFactoryTestBase (string createTestDataFileName)
        : base(new DatabaseAgent(TestDomainConnectionString), createTestDataFileName)
    {
    }

    public override void SetUp ()
    {
      base.SetUp();

      var storageSettings = FakeStorageSettings.CreateForSqlServer(TestDomainConnectionString, CreateSqlStorageObjectFactory);
      _storageProviderDefinition = (RdbmsProviderDefinition)storageSettings.GetDefaultStorageProviderDefinition();
      _storageObjectFactory = (SqlStorageObjectFactory)_storageProviderDefinition.Factory;

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle<IStorageSettings>(() => storageSettings);
      SetupServiceLocator(serviceLocator);
      _serviceLocatorScope = new ServiceLocatorScope(serviceLocator);

      var mappingLoader = MappingReflector.Create(
          new FixedTypeDiscoveryService(GetReflectedTypes()),
          new ClassIDProvider(),
          new ReflectionBasedMemberInformationNameResolver(),
          new PropertyMetadataReflector(),
          new DomainModelConstraintProvider(),
          new PropertyDefaultValueProvider(),
          new SortExpressionDefinitionProvider(),
          SafeServiceLocator.Current.GetInstance<IDomainObjectCreator>());
      var persistenceModelLoader = _storageObjectFactory.CreatePersistenceModelLoader(_storageProviderDefinition);
      _mappingConfiguration = MappingConfiguration.Create(mappingLoader, persistenceModelLoader);
      MappingConfiguration.SetCurrent(_mappingConfiguration);
    }

    protected virtual void SetupServiceLocator (DefaultServiceLocator serviceLocator)
    {
    }

    public override void TearDown ()
    {
      MappingConfiguration.SetCurrent(null);
      _serviceLocatorScope.Dispose();

      base.TearDown();
    }

    protected MappingConfiguration MappingConfiguration
    {
      get { return _mappingConfiguration; }
    }

    protected IRdbmsStorageObjectFactory StorageObjectFactory
    {
      get { return _storageObjectFactory; }
    }

    protected RdbmsProviderDefinition StorageProviderDefinition
    {
      get { return _storageProviderDefinition; }
    }

    protected abstract SqlStorageObjectFactory CreateSqlStorageObjectFactory (IStorageSettings storageSettings);

    protected virtual Type[] GetReflectedTypes ()
    {
      var thisType = GetType();
      var testDomainNamespace = thisType.Namespace + ".TestDomain";
      return thisType.Assembly.GetTypes().Where(t => t.Namespace != null && t.Namespace.StartsWith(testDomainNamespace)).ToArray();
    }
  }
}
