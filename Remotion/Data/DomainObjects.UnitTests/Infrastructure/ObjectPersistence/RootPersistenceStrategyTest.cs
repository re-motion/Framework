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
using JetBrains.Annotations;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class RootPersistenceStrategyTest : StandardMappingTest
  {
    private class PersistableDataStub : PersistableData
    {
      public PersistableDataStub (DataContainer dataContainer)
          : base(DomainObjectMother.CreateFakeObject(), new DomainObjectState(), dataContainer, new[] { Mock.Of<IRelationEndPoint>() })
      {
      }
    }

    private RootPersistenceStrategy _rootPersistenceStrategy;
    private Mock<IStorageSettings> _storageSettingsMock;
    private Mock<IPersistenceService> _persistenceServiceMock;
    private UnitTestStorageProviderStubDefinition _unitTestStorageProviderStubDefinition;
    private Mock<IRdbmsStorageObjectFactory> _rdbmsStorageObjectFactoryStub;
    private Mock<IStorageProvider> _storageProviderStub;
    private Mock<IStorageAccessResolver> _storageAccessResolverStub;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _storageSettingsMock = new Mock<IStorageSettings>(MockBehavior.Strict);
      _persistenceServiceMock = new Mock<IPersistenceService>(MockBehavior.Strict);
      _rdbmsStorageObjectFactoryStub = new Mock<IRdbmsStorageObjectFactory>();
      _storageProviderStub = new Mock<IStorageProvider>();
      _storageAccessResolverStub = new Mock<IStorageAccessResolver>();

      var transactionID = Guid.NewGuid();
      var persistenceExtensionFactoryMock = new Mock<IPersistenceExtensionFactory>(MockBehavior.Strict);
      persistenceExtensionFactoryMock.Setup(_ => _.CreatePersistenceExtensions(transactionID)).Returns(new IPersistenceExtension[0]);

      _storageAccessResolverStub
          .Setup(_ => _.ResolveStorageAccessForQuery(It.IsAny<Query>()))
          .Returns(StorageAccessType.ReadWrite);
      _storageAccessResolverStub
          .Setup(_ => _.ResolveStorageAccessForLoadingDomainObjectRelation())
          .Returns(StorageAccessType.ReadWrite);
      _storageAccessResolverStub
          .Setup(_ => _.ResolveStorageAccessForLoadingDomainObjectsByObjectID())
          .Returns(StorageAccessType.ReadWrite);

      _rootPersistenceStrategy = new RootPersistenceStrategy(
          transactionID,
          _storageSettingsMock.Object,
          _persistenceServiceMock.Object,
          persistenceExtensionFactoryMock.Object,
          _storageAccessResolverStub.Object);
    }

    [Test]
    public void Initialize ()
    {
      var transactionID = Guid.NewGuid();

      var strategy = new RootPersistenceStrategy(
          transactionID,
          Mock.Of<IStorageSettings>(),
          Mock.Of<IPersistenceService>(),
          Mock.Of<IPersistenceExtensionFactory>(),
          Mock.Of<IStorageAccessResolver>());
      Assert.That(strategy.TransactionID, Is.EqualTo(transactionID));
    }

    [Test]
    public void CreateNewObjectID_UsesIPersistenceServiceToCreateObjectID ()
    {
      var classDefinition = Configuration.GetTypeDefinition(typeof(Order));
      var objectID = new ObjectID(classDefinition, Guid.NewGuid());
      _persistenceServiceMock
          .Setup(_ => _.CreateNewObjectID(It.IsAny<IStorageProviderManager>(), classDefinition))
          .Returns(objectID)
          .Verifiable();

      var result = _rootPersistenceStrategy.CreateNewObjectID(classDefinition);

      Assert.That(result, Is.EqualTo(objectID));
      _persistenceServiceMock.Verify();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void LoadObjectData_SingleObjectID_WithLoadedObject_CreatesFreshlyLoadedObjectData (bool readOnlyStorageAccess)
    {
      var classDefinition = Configuration.GetTypeDefinition(typeof(Order));
      var objectID = new ObjectID(classDefinition, Guid.NewGuid());
      var dataContainer = DataContainer.CreateNew(objectID);
      var lookupResult = new ObjectLookupResult<DataContainer>(objectID, dataContainer);
      _persistenceServiceMock
          .Setup(_ => _.LoadDataContainer(GetStorageProviderManager(readOnlyStorageAccess), objectID))
          .Returns(lookupResult)
          .Verifiable();
      _storageAccessResolverStub
          .Setup(_ => _.ResolveStorageAccessForLoadingDomainObjectsByObjectID())
          .Returns(readOnlyStorageAccess ? StorageAccessType.ReadOnly : StorageAccessType.ReadWrite);

      var result = _rootPersistenceStrategy.LoadObjectData(objectID);

      Assert.That(result, Is.TypeOf<FreshlyLoadedObjectData>());
      Assert.That(result.ObjectID, Is.EqualTo(objectID));
      Assert.That(result.As<FreshlyLoadedObjectData>().FreshlyLoadedDataContainer, Is.EqualTo(dataContainer));
      _persistenceServiceMock.Verify();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void LoadObjectData_SingleObjectID_WithoutLoadedObject_CreatesNotFoundLoadedObjectData (bool readOnlyStorageAccess)
    {
      var classDefinition = Configuration.GetTypeDefinition(typeof(Order));
      var objectID = new ObjectID(classDefinition, Guid.NewGuid());
      var lookupResult = new ObjectLookupResult<DataContainer>(objectID, null);
      _persistenceServiceMock
          .Setup(_ => _.LoadDataContainer(GetStorageProviderManager(readOnlyStorageAccess), objectID))
          .Returns(lookupResult)
          .Verifiable();
      _storageAccessResolverStub
          .Setup(_ => _.ResolveStorageAccessForLoadingDomainObjectsByObjectID())
          .Returns(readOnlyStorageAccess ? StorageAccessType.ReadOnly : StorageAccessType.ReadWrite);

      var result = _rootPersistenceStrategy.LoadObjectData(objectID);

      Assert.That(result, Is.TypeOf<NotFoundLoadedObjectData>());
      Assert.That(result.ObjectID, Is.EqualTo(objectID));
      _persistenceServiceMock.Verify();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void LoadObjectData_MultipleObjectIDs_CreateLoadedObjectData (bool readOnlyStorageAccess)
    {
      var classDefinition1 = Configuration.GetTypeDefinition(typeof(Order));
      var classDefinition2 = Configuration.GetTypeDefinition(typeof(Customer));
      var objectID1 = new ObjectID(classDefinition1, Guid.NewGuid());
      var objectID2 = new ObjectID(classDefinition2, Guid.NewGuid());
      var dataContainer = DataContainer.CreateNew(objectID1);
      var lookupResult1 = new ObjectLookupResult<DataContainer>(objectID1, dataContainer);
      var lookupResult2 = new ObjectLookupResult<DataContainer>(objectID2, null);
      _persistenceServiceMock
          .Setup(_ => _.LoadDataContainers(GetStorageProviderManager(readOnlyStorageAccess), new[] { objectID1, objectID2 }))
          .Returns(new[] { lookupResult1, lookupResult2 })
          .Verifiable();
      _storageAccessResolverStub
          .Setup(_ => _.ResolveStorageAccessForLoadingDomainObjectsByObjectID())
          .Returns(readOnlyStorageAccess ? StorageAccessType.ReadOnly : StorageAccessType.ReadWrite);

      var result = _rootPersistenceStrategy.LoadObjectData(new[] { objectID1, objectID2 }).ToArray();

      Assert.That(result.Length, Is.EqualTo(2));
      Assert.That(result[0], Is.InstanceOf<FreshlyLoadedObjectData>());
      Assert.That(result[0].ObjectID, Is.EqualTo(objectID1));
      Assert.That(result[0].As<FreshlyLoadedObjectData>().FreshlyLoadedDataContainer, Is.EqualTo(dataContainer));
      Assert.That(result[1], Is.InstanceOf<NotFoundLoadedObjectData>());
      Assert.That(result[1].ObjectID, Is.EqualTo(objectID2));
      _persistenceServiceMock.Verify();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void ResolveObjectRelationData_NoRelatedData_ReturnsNullLoadedObjectData (bool readOnlyStorageAccess)
    {
      var loadedObjectDataProviderStub = new Mock<ILoadedObjectDataProvider>();
      var classDefinition = Configuration.GetTypeDefinition(typeof(Order));
      var objectID = new ObjectID(classDefinition, Guid.NewGuid());
      var relationEndPoint = RelationEndPointID.Create(objectID, Mock.Of<IRelationEndPointDefinition>());
      _persistenceServiceMock
          .Setup(_ => _.LoadRelatedDataContainer(GetStorageProviderManager(readOnlyStorageAccess), relationEndPoint))
          .Returns((DataContainer)null)
          .Verifiable();
      _storageAccessResolverStub
          .Setup(_ => _.ResolveStorageAccessForLoadingDomainObjectRelation())
          .Returns(readOnlyStorageAccess ? StorageAccessType.ReadOnly : StorageAccessType.ReadWrite);
      var result = _rootPersistenceStrategy.ResolveObjectRelationData(relationEndPoint, loadedObjectDataProviderStub.Object);

      Assert.That(result, Is.InstanceOf<NullLoadedObjectData>());
      _persistenceServiceMock.Verify();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void ResolveObjectRelationData_RelatedDataWithoutDataObject_ReturnsFreshlyLoadedObjectData (bool readOnlyStorageAccess)
    {
      var classDefinition = Configuration.GetTypeDefinition(typeof(Order));
      var objectID = new ObjectID(classDefinition, Guid.NewGuid());

      var classDefinitionForReturnedDataContainer = Configuration.GetTypeDefinition(typeof(Customer));
      var objectIdForReturnedDataContainer = new ObjectID(classDefinitionForReturnedDataContainer, Guid.NewGuid());
      var returnedDataContainer = DataContainer.CreateNew(objectIdForReturnedDataContainer);

      var loadedObjectDataProviderStub = new Mock<ILoadedObjectDataProvider>();
      loadedObjectDataProviderStub
          .Setup(_ => _.GetLoadedObject(returnedDataContainer.ID))
          .Returns((ILoadedObjectData)null);

      var relationEndPoint = RelationEndPointID.Create(objectID, Mock.Of<IRelationEndPointDefinition>());
      _persistenceServiceMock
          .Setup(_ => _.LoadRelatedDataContainer(GetStorageProviderManager(readOnlyStorageAccess), relationEndPoint))
          .Returns(returnedDataContainer)
          .Verifiable();
      _storageAccessResolverStub
          .Setup(_ => _.ResolveStorageAccessForLoadingDomainObjectRelation())
          .Returns(readOnlyStorageAccess ? StorageAccessType.ReadOnly : StorageAccessType.ReadWrite);
      var result = _rootPersistenceStrategy.ResolveObjectRelationData(relationEndPoint, loadedObjectDataProviderStub.Object);

      Assert.That(result, Is.InstanceOf<FreshlyLoadedObjectData>());
      Assert.That(result.ObjectID, Is.EqualTo(objectIdForReturnedDataContainer));
      Assert.That(result.As<FreshlyLoadedObjectData>().FreshlyLoadedDataContainer, Is.EqualTo(returnedDataContainer));
      _persistenceServiceMock.Verify();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void ResolveObjectRelationData_RelatedData_ReturnsFreshlyLoadedObjectData (bool readOnlyStorageAccess)
    {
      var classDefinition = Configuration.GetTypeDefinition(typeof(Order));
      var objectID = new ObjectID(classDefinition, Guid.NewGuid());

      var classDefinitionForReturnedDataContainer = Configuration.GetTypeDefinition(typeof(Customer));
      var objectIdForReturnedDataContainer = new ObjectID(classDefinitionForReturnedDataContainer, Guid.NewGuid());
      var returnedDataContainer = DataContainer.CreateNew(objectIdForReturnedDataContainer);

      var loadedObjectDataStub = new Mock<ILoadedObjectData>();
      var loadedObjectDataProviderStub = new Mock<ILoadedObjectDataProvider>();
      loadedObjectDataProviderStub
          .Setup(_ => _.GetLoadedObject(returnedDataContainer.ID))
          .Returns(loadedObjectDataStub.Object);

      var relationEndPoint = RelationEndPointID.Create(objectID, Mock.Of<IRelationEndPointDefinition>());
      _persistenceServiceMock
          .Setup(_ => _.LoadRelatedDataContainer(GetStorageProviderManager(readOnlyStorageAccess), relationEndPoint))
          .Returns(returnedDataContainer)
          .Verifiable();
      _storageAccessResolverStub
          .Setup(_ => _.ResolveStorageAccessForLoadingDomainObjectRelation())
          .Returns(readOnlyStorageAccess ? StorageAccessType.ReadOnly : StorageAccessType.ReadWrite);
      var result = _rootPersistenceStrategy.ResolveObjectRelationData(relationEndPoint, loadedObjectDataProviderStub.Object);

      Assert.That(result, Is.SameAs(loadedObjectDataStub.Object));
      _persistenceServiceMock.Verify();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void ResolveCollectionRelationData (bool readOnlyStorageAccess)
    {
      var classDefinition = Configuration.GetTypeDefinition(typeof(Order));
      var objectID = new ObjectID(classDefinition, Guid.NewGuid());

      var classDefinitionForReturnedDataContainer = Configuration.GetTypeDefinition(typeof(Customer));
      var objectIdForReturnedDataContainer = new ObjectID(classDefinitionForReturnedDataContainer, Guid.NewGuid());
      var objectIdForNotYetLoadedDataContainer = new ObjectID(classDefinitionForReturnedDataContainer, Guid.NewGuid());
      var returnedDataContainer = DataContainer.CreateNew(objectIdForReturnedDataContainer);
      var returnedNotYetLoadedDataContainer = DataContainer.CreateNew(objectIdForNotYetLoadedDataContainer);

      var loadedObjectDataStub = new Mock<ILoadedObjectData>();
      var loadedObjectDataProvider = SetupLoadedObjectDataProviderStubWithDataContainers(returnedDataContainer, returnedNotYetLoadedDataContainer, loadedObjectDataStub.Object);

      var dataContainerCollection = new DataContainerCollection(new[] { returnedDataContainer, returnedNotYetLoadedDataContainer }, true);
      var relationEndPoint = RelationEndPointID.Create(objectID, Mock.Of<IRelationEndPointDefinition>());
      _persistenceServiceMock
          .Setup(_ => _.LoadRelatedDataContainers(GetStorageProviderManager(readOnlyStorageAccess), relationEndPoint))
          .Returns(dataContainerCollection)
          .Verifiable();
      _storageAccessResolverStub
          .Setup(_ => _.ResolveStorageAccessForLoadingDomainObjectRelation())
          .Returns(readOnlyStorageAccess ? StorageAccessType.ReadOnly : StorageAccessType.ReadWrite);

      var result = _rootPersistenceStrategy.ResolveCollectionRelationData(relationEndPoint, loadedObjectDataProvider).ToArray();

      Assert.That(result.Length, Is.EqualTo(2));
      Assert.That(result[0], Is.SameAs(loadedObjectDataStub.Object));
      Assert.That(result[1], Is.InstanceOf<FreshlyLoadedObjectData>());
      Assert.That(result[1].As<FreshlyLoadedObjectData>().FreshlyLoadedDataContainer, Is.EqualTo(returnedNotYetLoadedDataContainer));
      Assert.That(result[1].As<FreshlyLoadedObjectData>().ObjectID, Is.EqualTo(objectIdForNotYetLoadedDataContainer));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void ExecuteCollectionQuery (bool readOnlyStorageAccess)
    {
      var classDefinitionForReturnedDataContainer = Configuration.GetTypeDefinition(typeof(Customer));
      var objectIdForReturnedDataContainer = new ObjectID(classDefinitionForReturnedDataContainer, Guid.NewGuid());
      var objectIdForNotYetLoadedDataContainer = new ObjectID(classDefinitionForReturnedDataContainer, Guid.NewGuid());
      var returnedDataContainer = DataContainer.CreateNew(objectIdForReturnedDataContainer);
      var returnedNotYetLoadedDataContainer = DataContainer.CreateNew(objectIdForNotYetLoadedDataContainer);

      var loadedObjectDataStub = SetupLoadedObjectDataStubWithDataContainer(returnedDataContainer);
      var loadedObjectDataProvider = SetupLoadedObjectDataProviderStubWithDataContainers(returnedDataContainer, returnedNotYetLoadedDataContainer, loadedObjectDataStub.Object);

      var query = SetupQueryAndInfrastructureForQueryType(QueryType.CollectionReadOnly, readOnlyStorageAccess);
      SetupRdbmsStorageObjectFactoryStubToCreateStorageProvider(readOnlyStorageAccess);

      var dataContainers = new[] { returnedDataContainer, returnedNotYetLoadedDataContainer };
      _storageProviderStub.Setup(_ => _.ExecuteCollectionQuery(query)).Returns(dataContainers);
      _storageAccessResolverStub
          .Setup(_ => _.ResolveStorageAccessForQuery(query))
          .Returns(readOnlyStorageAccess ? StorageAccessType.ReadOnly : StorageAccessType.ReadWrite);

      var result = _rootPersistenceStrategy.ExecuteCollectionQuery(query, loadedObjectDataProvider).ToArray();

      Assert.That(result.Length, Is.EqualTo(2));
      Assert.That(result[0], Is.SameAs(loadedObjectDataStub.Object));
      Assert.That(result[1], Is.InstanceOf<FreshlyLoadedObjectData>());
      Assert.That(result[1].As<FreshlyLoadedObjectData>().FreshlyLoadedDataContainer, Is.EqualTo(returnedNotYetLoadedDataContainer));
      Assert.That(result[1].As<FreshlyLoadedObjectData>().ObjectID, Is.EqualTo(objectIdForNotYetLoadedDataContainer));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void ExecuteFetchQuery (bool readOnlyStorageAccess)
    {
      var classDefinitionForReturnedDataContainer = Configuration.GetTypeDefinition(typeof(Customer));
      var objectIdForReturnedDataContainer = new ObjectID(classDefinitionForReturnedDataContainer, Guid.NewGuid());
      var objectIdForNotYetLoadedDataContainer = new ObjectID(classDefinitionForReturnedDataContainer, Guid.NewGuid());
      var returnedDataContainer = DataContainer.CreateNew(objectIdForReturnedDataContainer);
      var returnedNotYetLoadedDataContainer = DataContainer.CreateNew(objectIdForNotYetLoadedDataContainer);

      var loadedObjectDataStub = SetupLoadedObjectDataStubWithDataContainer(returnedDataContainer);
      var loadedObjectDataProvider = SetupLoadedObjectDataProviderStubWithDataContainers(returnedDataContainer, returnedNotYetLoadedDataContainer, loadedObjectDataStub.Object);

      var query = SetupQueryAndInfrastructureForQueryType(QueryType.CollectionReadOnly, readOnlyStorageAccess);
      SetupRdbmsStorageObjectFactoryStubToCreateStorageProvider(readOnlyStorageAccess);

      var dataContainers = new[] { returnedDataContainer, returnedNotYetLoadedDataContainer };
      _storageProviderStub.Setup(_ => _.ExecuteCollectionQuery(query)).Returns(dataContainers);
      _storageAccessResolverStub
          .Setup(_ => _.ResolveStorageAccessForQuery(query))
          .Returns(readOnlyStorageAccess ? StorageAccessType.ReadOnly : StorageAccessType.ReadWrite);

      var result = _rootPersistenceStrategy.ExecuteFetchQuery(query, loadedObjectDataProvider).ToArray();

      Assert.That(result.Length, Is.EqualTo(2));
      Assert.That(result[0].LoadedObjectData, Is.SameAs(loadedObjectDataStub.Object));
      Assert.That(result[0].DataSourceData, Is.SameAs(returnedDataContainer));
      Assert.That(result[1].LoadedObjectData, Is.InstanceOf<FreshlyLoadedObjectData>());
      Assert.That(result[1].LoadedObjectData.As<FreshlyLoadedObjectData>().FreshlyLoadedDataContainer, Is.EqualTo(returnedNotYetLoadedDataContainer));
      Assert.That(result[1].LoadedObjectData.As<FreshlyLoadedObjectData>().ObjectID, Is.EqualTo(objectIdForNotYetLoadedDataContainer));
      Assert.That(result[1].DataSourceData, Is.EqualTo(returnedNotYetLoadedDataContainer));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void ExecuteCustomQuery (bool readOnlyStorageAccess)
    {
      var query = SetupQueryAndInfrastructureForQueryType(readOnlyStorageAccess ? QueryType.CustomReadOnly : QueryType.CustomReadWrite, readOnlyStorageAccess);
      SetupRdbmsStorageObjectFactoryStubToCreateStorageProvider(readOnlyStorageAccess);

      var queryResultRow1Fake = new Mock<IQueryResultRow>().Object;
      var queryResultRow2Fake = new Mock<IQueryResultRow>().Object;
      var queryResultRow3Fake = new Mock<IQueryResultRow>().Object;
      _storageProviderStub.Setup(_ => _.ExecuteCustomQuery(query)).Returns(new[] { queryResultRow1Fake, queryResultRow2Fake, queryResultRow3Fake });
      _storageAccessResolverStub
          .Setup(_ => _.ResolveStorageAccessForQuery(query))
          .Returns(readOnlyStorageAccess ? StorageAccessType.ReadOnly : StorageAccessType.ReadWrite);

      var result = _rootPersistenceStrategy.ExecuteCustomQuery(query).ToArray();

      Assert.That(result.Length, Is.EqualTo(3));
      Assert.That(result[0], Is.SameAs(queryResultRow1Fake));
      Assert.That(result[1], Is.SameAs(queryResultRow2Fake));
      Assert.That(result[2], Is.SameAs(queryResultRow3Fake));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void ExecuteScalarQuery (bool readOnlyStorageAccess)
    {
      var query = SetupQueryAndInfrastructureForQueryType(QueryType.ScalarReadOnly, readOnlyStorageAccess);
      SetupRdbmsStorageObjectFactoryStubToCreateStorageProvider(readOnlyStorageAccess);

      _storageProviderStub.Setup(_ => _.ExecuteScalarQuery(query)).Returns("ScalarQueryOutput");
      _storageAccessResolverStub
          .Setup(_ => _.ResolveStorageAccessForQuery(query))
          .Returns(readOnlyStorageAccess ? StorageAccessType.ReadOnly : StorageAccessType.ReadWrite);

      var result = _rootPersistenceStrategy.ExecuteScalarQuery(query);

      Assert.That(result, Is.InstanceOf<string>());
      Assert.That(result!.As<string>(), Is.EqualTo("ScalarQueryOutput"));
    }

    [Test]
    public void PersistData ()
    {
      var classDefinition = Configuration.GetTypeDefinition(typeof(Order));
      var objectID1 = new ObjectID(classDefinition, Guid.NewGuid());
      var objectID2 = new ObjectID(classDefinition, Guid.NewGuid());
      var dataContainer1 = DataContainer.CreateNew(objectID1);
      var dataContainer2 = DataContainer.CreateNew(objectID2);
      var data1Fake = new PersistableDataStub(dataContainer1);
      var data2Fake = new PersistableDataStub(dataContainer2);

      var data = new[] { data1Fake, data2Fake };
      DataContainer[] containerArray = {};

      _persistenceServiceMock
          .Setup(_ => _.Save(It.IsAny<IStorageProviderManager>(), It.IsAny<DataContainerCollection>()))
          .Callback(
              (IStorageProviderManager _, [CanBeNull] DataContainerCollection dataContainers) =>
              {
                containerArray = dataContainers.ToArray();
              })
          .Verifiable();

      _rootPersistenceStrategy.PersistData(data);

      Assert.That(containerArray.Length, Is.EqualTo(2));
      Assert.That(containerArray[0].ID, Is.EqualTo(objectID1));
      Assert.That(containerArray[1].ID, Is.EqualTo(objectID2));
      _persistenceServiceMock.Verify();
    }

    private ILoadedObjectDataProvider SetupLoadedObjectDataProviderStubWithDataContainers (
        DataContainer returnedDataContainer,
        DataContainer returnedNotYetLoadedDataContainer,
        ILoadedObjectData loadedObjectData)
    {
      var loadedObjectDataProviderStub = new Mock<ILoadedObjectDataProvider>();
      loadedObjectDataProviderStub
          .Setup(_ => _.GetLoadedObject(returnedDataContainer.ID))
          .Returns(loadedObjectData);
      loadedObjectDataProviderStub
          .Setup(_ => _.GetLoadedObject(returnedNotYetLoadedDataContainer.ID))
          .Returns((ILoadedObjectData)null);
      return loadedObjectDataProviderStub.Object;
    }

    private Mock<ILoadedObjectData> SetupLoadedObjectDataStubWithDataContainer (DataContainer returnedDataContainer)
    {
      var loadedObjectDataStub = new Mock<ILoadedObjectData>();
      loadedObjectDataStub.Setup(_ => _.ObjectID).Returns(returnedDataContainer.ID);
      return loadedObjectDataStub;
    }

    private IQuery SetupQueryAndInfrastructureForQueryType (QueryType queryType, bool createReadOnlyStorageProvider)
    {
      _unitTestStorageProviderStubDefinition = new UnitTestStorageProviderStubDefinition(
          "storageProviderDefinition",
          _rdbmsStorageObjectFactoryStub.Object,
          new[] { typeof(Customer), typeof(Order) });

      var queryStub = new Mock<IQuery>();
      queryStub.Setup(_ => _.QueryType).Returns(queryType);
      queryStub.Setup(_ => _.StorageProviderDefinition).Returns(_unitTestStorageProviderStubDefinition);

      _storageSettingsMock
          .Setup(_ => _.GetStorageProviderDefinition(_unitTestStorageProviderStubDefinition.Name))
          .Returns(_unitTestStorageProviderStubDefinition);

      return queryStub.Object;
    }

    private void SetupRdbmsStorageObjectFactoryStubToCreateStorageProvider (bool createReadOnlyStorageProvider)
    {
      if (createReadOnlyStorageProvider)
      {
        _rdbmsStorageObjectFactoryStub
            .Setup(_ => _.CreateReadOnlyStorageProvider(_unitTestStorageProviderStubDefinition, It.IsAny<IPersistenceExtension>()))
            .Returns(_storageProviderStub.Object);
      }
      else
      {
        _rdbmsStorageObjectFactoryStub
            .Setup(_ => _.CreateStorageProvider(_unitTestStorageProviderStubDefinition, It.IsAny<IPersistenceExtension>()))
            .Returns(_storageProviderStub.Object);
      }
    }

    private IReadOnlyStorageProviderManager GetStorageProviderManager (bool readOnlyStorageAccess)
    {
      if (readOnlyStorageAccess)
        return It.IsAny<ReadOnlyStorageProviderManager>();
      else
        return It.IsAny<StorageProviderManager>();
    }
  }
}
