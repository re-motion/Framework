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
using Moq;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2014;
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

    protected CustomStorageObjectFactoryTestBase (string createTestDataFileName)
        : base(new DatabaseAgent(TestDomainConnectionString), createTestDataFileName)
    {
    }

    public override void SetUp ()
    {
      base.SetUp();
      var mappingLoader = new MappingReflector(
          new FixedTypeDiscoveryService(GetReflectedTypes()),
          new ClassIDProvider(),
          new ReflectionBasedMemberInformationNameResolver(),
          new PropertyMetadataReflector(),
          new DomainModelConstraintProvider(),
          SafeServiceLocator.Current.GetInstance<IPropertyDefaultValueProvider>(),
          new SortExpressionDefinitionProvider(),
          SafeServiceLocator.Current.GetInstance<IDomainObjectCreator>());
      _storageObjectFactory = CreateSqlStorageObjectFactory();
      var storageProviderDefinitionFinderStub = new Mock<IStorageProviderDefinitionFinder>();
      _storageProviderDefinition = new RdbmsProviderDefinition("test", _storageObjectFactory, DatabaseTest.TestDomainConnectionString);
      storageProviderDefinitionFinderStub
          .Setup(stub => stub.GetStorageProviderDefinition(It.IsAny<ClassDefinition>(), It.IsAny<string>()))
          .Returns(_storageProviderDefinition);
      var persistenceModelLoader = _storageObjectFactory.CreatePersistenceModelLoader(_storageProviderDefinition, storageProviderDefinitionFinderStub.Object);
      _mappingConfiguration = new MappingConfiguration(mappingLoader, persistenceModelLoader);

      MappingConfiguration.SetCurrent(_mappingConfiguration);

      DomainObjectsConfiguration.SetCurrent(
          new FakeDomainObjectsConfiguration(
              null,
              new StorageConfiguration(
                  new ProviderCollection<StorageProviderDefinition> { _storageProviderDefinition },
                  _storageProviderDefinition),
              null));
    }

    public override void TearDown ()
    {
      MappingConfiguration.SetCurrent(null);
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

    protected abstract SqlStorageObjectFactory CreateSqlStorageObjectFactory ();

    protected virtual Type[] GetReflectedTypes ()
    {
      var thisType = GetType();
      var testDomainNamespace = thisType.Namespace + ".TestDomain";
      return thisType.Assembly.GetTypes().Where(t => t.Namespace != null && t.Namespace.StartsWith(testDomainNamespace)).ToArray();
    }
  }
}
