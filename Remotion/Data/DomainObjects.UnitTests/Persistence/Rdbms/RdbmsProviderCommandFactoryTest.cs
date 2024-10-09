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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.Validation;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  [TestFixture]
  public class RdbmsProviderCommandFactoryTest : StandardMappingTest
  {
    private RdbmsProviderCommandFactory _factory;

    private ObjectID _objectID1;
    private ObjectID _objectID2;
    private ObjectID _objectID3;

    public override void SetUp ()
    {
      base.SetUp();

      var rdbmsPersistenceModelProvider = new RdbmsPersistenceModelProvider();
      var storageTypeInformationProvider = new SqlStorageTypeInformationProvider(new DateTime2DefaultStorageTypeProvider());
      var dataContainerValidator = new CompoundDataContainerValidator(Enumerable.Empty<IDataContainerValidator>());

      var storageNameProvider = new ReflectionBasedStorageNameProvider();
      var infrastructureStoragePropertyDefinitionProvider = new InfrastructureStoragePropertyDefinitionProvider(storageTypeInformationProvider, storageNameProvider);
      var dataStoragePropertyDefinitionFactory = new DataStoragePropertyDefinitionFactory(
          new ValueStoragePropertyDefinitionFactory(storageTypeInformationProvider, storageNameProvider),
          new RelationStoragePropertyDefinitionFactory(
              TestDomainStorageProviderDefinition, false, storageNameProvider, storageTypeInformationProvider, StorageSettings));

      var dataParameterDefinitionFactoryChain =
          new ObjectIDDataParameterDefinitionFactory(TestDomainStorageProviderDefinition, storageTypeInformationProvider, StorageSettings,
              new SimpleDataParameterDefinitionFactory(storageTypeInformationProvider));

      var singleScalarTableTypeDefinitionProvider = new SingleScalarSqlTableTypeDefinitionProvider(storageTypeInformationProvider);

      _factory = new RdbmsProviderCommandFactory(
          TestDomainStorageProviderDefinition,
          new SqlDbCommandBuilderFactory(singleScalarTableTypeDefinitionProvider, new SqlDialect()),
          rdbmsPersistenceModelProvider,
          new ObjectReaderFactory(
              rdbmsPersistenceModelProvider, infrastructureStoragePropertyDefinitionProvider, storageTypeInformationProvider, dataContainerValidator),
          new TableDefinitionFinder(rdbmsPersistenceModelProvider),
          dataStoragePropertyDefinitionFactory,
          dataParameterDefinitionFactoryChain);

      _objectID1 = DomainObjectIDs.Order1;
      _objectID2 = DomainObjectIDs.Order3;
      _objectID3 = DomainObjectIDs.Order4;
    }

    [Test]
    public void CreateForSingleIDLookup ()
    {
      var result = _factory.CreateForSingleIDLookup(_objectID1);

      Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void CreateForSortedMultiIDLookup ()
    {
      var result = _factory.CreateForSortedMultiIDLookup(new[] { _objectID1 });

      Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void CreateForRelationLookup ()
    {
      var relationEndPointDefinition = (RelationEndPointDefinition)GetEndPointDefinition(typeof(OrderItem), "Order");

      var result = _factory.CreateForRelationLookup(relationEndPointDefinition, DomainObjectIDs.Order1, null);

      Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void CreateForDataContainerQuery ()
    {
      var queryStub = new Mock<IQuery>();
      queryStub.Setup(stub => stub.Statement).Returns("Statement");
      queryStub.Setup(stub => stub.Parameters).Returns(new QueryParameterCollection());

      var result = _factory.CreateForDataContainerQuery(queryStub.Object);

      Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void CreateForCustomQuery ()
    {
      var queryStub = new Mock<IQuery>();
      queryStub.Setup(stub => stub.Statement).Returns("Statement");
      queryStub.Setup(stub => stub.Parameters).Returns(new QueryParameterCollection());

      var result = _factory.CreateForCustomQuery(queryStub.Object);

      Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void CreateForScalarQuery ()
    {
      var queryStub = new Mock<IQuery>();
      queryStub.Setup(stub => stub.Statement).Returns("Statement");
      queryStub.Setup(stub => stub.Parameters).Returns(new QueryParameterCollection());

      var result = _factory.CreateForScalarQuery(queryStub.Object);

      Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void CreateForMultiTimestampLookup ()
    {
      var result = _factory.CreateForMultiTimestampLookup(new[] { _objectID1, _objectID2, _objectID3 });

      Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void CreateForSave ()
    {
      var dataContainer = DataContainer.CreateNew(DomainObjectIDs.Computer1);
      SetPropertyValue(dataContainer, typeof(Computer), "SerialNumber", "123456");
      var result = _factory.CreateForSave(new[] { dataContainer });

      Assert.That(result, Is.Not.Null);
    }
  }
}
