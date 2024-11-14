using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.NonPersistent;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.NonPersistent
{
  [TestFixture]
  public class NonPersistentProviderTest : StandardMappingTest
  {
    private IStorageProvider _provider;

    public override void SetUp ()
    {
      base.SetUp();

      _provider = new NonPersistentProvider(NonPersistentStorageProviderDefinition);
    }

    [Test]
    public void CreateNewObjectID ()
    {
      var classDefinition = DomainObjectIDs.OrderViewModel1.ClassDefinition;

      var result = _provider.CreateNewObjectID(classDefinition);
      Assert.That(result.Value, Is.Not.Null);
      Assert.That(result.Value, Is.Not.SameAs(_provider.CreateNewObjectID(classDefinition).Value));
      Assert.That(result.ClassDefinition, Is.SameAs(classDefinition));
    }

    [Test]
    public void CreateNewObjectID_Disposed ()
    {
      _provider.Dispose();

      Assert.That(
          () => _provider.CreateNewObjectID(DomainObjectIDs.OrderViewModel1.ClassDefinition),
          Throws.Exception.TypeOf<ObjectDisposedException>().With.Message.EqualTo(
              "A disposed NonPersistentProvider cannot be accessed.\r\nObject name: 'NonPersistentProvider'."));
    }

    [Test]
    public void CreateNewObjectID_ClassDefinitionWithDifferentStorageProviderDefinition ()
    {
      var providerWithDifferentID = new NonPersistentProvider(new NonPersistentProviderDefinition("Test", new NonPersistentStorageObjectFactory()));

      Assert.That(
          () => providerWithDifferentID.CreateNewObjectID(DomainObjectIDs.OrderViewModel1.ClassDefinition),
          Throws.Exception.TypeOf<ArgumentException>().With.ArgumentExceptionMessageEqualTo(
              "The StorageProviderID 'NonPersistentTestDomain' of the provided ClassDefinition does not match with this StorageProvider's ID 'Test'.", "classDefinition"));
    }

    [Test]
    public void LoadDataContainer ()
    {
      var objectID = DomainObjectIDs.OrderViewModel1;
      var fakeResult = new ObjectLookupResult<DataContainer>(objectID, null);

      var result = _provider.LoadDataContainer(objectID);

      Assert.That(result, Is.EqualTo(fakeResult));
    }

    [Test]
    public void LoadDataContainer_InvalidID ()
    {
      var objectID = DomainObjectIDs.Official1;

      Assert.That(
          () => _provider.LoadDataContainer(objectID),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "The StorageProviderID 'UnitTestStorageProviderStub' of the provided ObjectID 'Official|1|System.Int32' does not match with this "
              + "StorageProvider's ID 'NonPersistentTestDomain'.", "id"));
    }

    [Test]
    public void LoadDataContainer_Disposed ()
    {
      var objectID = DomainObjectIDs.OrderViewModel1;

      _provider.Dispose();

      Assert.That(
          () => _provider.LoadDataContainer(objectID),
          Throws.Exception.TypeOf<ObjectDisposedException>().With.Message.EqualTo(
              "A disposed NonPersistentProvider cannot be accessed.\r\nObject name: 'NonPersistentProvider'."));
    }

    [Test]
    public void LoadDataContainers ()
    {
      var objectID1 = DomainObjectIDs.OrderViewModel1;
      var objectID2 = DomainObjectIDs.OrderViewModel2;

      var lookupResult1 = new ObjectLookupResult<DataContainer>(objectID1, null);
      var lookupResult2 = new ObjectLookupResult<DataContainer>(objectID2, null);

      var result = _provider.LoadDataContainers(new[] { objectID1, objectID2 }).ToArray();

      Assert.That(result, Is.EqualTo(new[] { lookupResult1, lookupResult2 }));
    }

    [Test]
    public void LoadDataContainers_Disposed ()
    {
      var objectID = DomainObjectIDs.OrderViewModel1;

      _provider.Dispose();

      Assert.That(
          () => _provider.LoadDataContainers(new[] { objectID }),
          Throws.Exception.TypeOf<ObjectDisposedException>().With.Message.EqualTo(
              "A disposed NonPersistentProvider cannot be accessed.\r\nObject name: 'NonPersistentProvider'."));
    }

    [Test]
    public void LoadDataContainers_InvalidID ()
    {
      var objectID = DomainObjectIDs.Official1;

      Assert.That(
          () => _provider.LoadDataContainers(new[] { objectID }),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "The StorageProviderID 'UnitTestStorageProviderStub' of the provided ObjectID 'Official|1|System.Int32' does not match with this "
              + "StorageProvider's ID 'NonPersistentTestDomain'.", "ids"));
    }

    [Test]
    public void LoadDataContainersByRelatedID ()
    {
      var objectID = DomainObjectIDs.OrderViewModel1;
      var relationEndPointDefinition = (RelationEndPointDefinition)GetEndPointDefinition(typeof(OrderViewModel), "Object");

      var result = _provider.LoadDataContainersByRelatedID(relationEndPointDefinition, null, objectID);

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void LoadDataContainersByRelatedID_Disposed ()
    {
      var objectID = DomainObjectIDs.OrderViewModel1;
      var relationEndPointDefinition = (RelationEndPointDefinition)GetEndPointDefinition(typeof(OrderViewModel), "Object");

      _provider.Dispose();

      Assert.That(
          () => _provider.LoadDataContainersByRelatedID(relationEndPointDefinition, null, objectID),
          Throws.Exception.TypeOf<ObjectDisposedException>().With.Message.EqualTo(
              "A disposed NonPersistentProvider cannot be accessed.\r\nObject name: 'NonPersistentProvider'."));
    }

    [Test]
    public void LoadDataContainersByRelatedID_ClassDefinitionWithDifferentStorageProviderDefinition ()
    {
      var providerWithDifferentID = new NonPersistentProvider(new NonPersistentProviderDefinition("Test", new NonPersistentStorageObjectFactory()));
      var objectID = DomainObjectIDs.OrderViewModel1;
      var relationEndPointDefinition = (RelationEndPointDefinition)GetEndPointDefinition(typeof(OrderViewModel), "Object");

      Assert.That(
          () => providerWithDifferentID.LoadDataContainersByRelatedID(relationEndPointDefinition, null, objectID),
          Throws.Exception.TypeOf<ArgumentException>().With.ArgumentExceptionMessageEqualTo(
              "The StorageProviderID 'NonPersistentTestDomain' of the provided ClassDefinition does not match with this StorageProvider's ID 'Test'.", "classDefinition"));
    }

    [Test]
    public void Save ()
    {
      var dataContainer1 = DataContainer.CreateNew(DomainObjectIDs.OrderViewModel1);
      var dataContainer2 = DataContainer.CreateNew(DomainObjectIDs.OrderViewModel2);

      _provider.Save(new DataContainerCollection(new[] { dataContainer1, dataContainer2 }, true));
    }

    [Test]
    public void Save_Disposed ()
    {
      _provider.Dispose();

      Assert.That(
          () => _provider.Save(new DataContainerCollection()),
          Throws.Exception.TypeOf<ObjectDisposedException>().With.Message.EqualTo(
              "A disposed NonPersistentProvider cannot be accessed.\r\nObject name: 'NonPersistentProvider'."));
    }

    [Test]
    public void UpdateTimestamps ()
    {
      var dataContainer1 = DataContainer.CreateNew(DomainObjectIDs.OrderViewModel1);
      var dataContainer2 = DataContainer.CreateNew(DomainObjectIDs.OrderViewModel2);

      _provider.UpdateTimestamps(new DataContainerCollection(new[] { dataContainer1, dataContainer2 }, true));

      Assert.That(dataContainer1.Timestamp, Is.Not.Null);
      Assert.That(dataContainer2.Timestamp, Is.Not.Null);
      Assert.That(dataContainer1.Timestamp, Is.Not.EqualTo(dataContainer2.Timestamp));
    }

    [Test]
    public void UpdateTimestamps_Disposed ()
    {
      _provider.Dispose();

      Assert.That(
          () => _provider.UpdateTimestamps(new DataContainerCollection()),
          Throws.Exception.TypeOf<ObjectDisposedException>().With.Message.EqualTo(
              "A disposed NonPersistentProvider cannot be accessed.\r\nObject name: 'NonPersistentProvider'."));
    }

    [Test]
    public void ExecuteCollectionQuery ()
    {
      var queryStub = new Mock<IQuery>();

      var result = _provider.ExecuteCollectionQuery(queryStub.Object);

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void ExecuteCollectionQuery_Disposed ()
    {
      var queryStub = new Mock<IQuery>();
      _provider.Dispose();

      Assert.That(
          () => _provider.ExecuteCollectionQuery(queryStub.Object),
          Throws.Exception.TypeOf<ObjectDisposedException>().With.Message.EqualTo(
              "A disposed NonPersistentProvider cannot be accessed.\r\nObject name: 'NonPersistentProvider'."));
    }


    [Test]
    public void ExecuteCustomQuery ()
    {
      var queryStub = new Mock<IQuery>();

      var result = _provider.ExecuteCustomQuery(queryStub.Object);

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void ExecuteCustomQuery_Disposed ()
    {
      var queryStub = new Mock<IQuery>();
      _provider.Dispose();

      Assert.That(
          () => _provider.ExecuteCustomQuery(queryStub.Object),
          Throws.Exception.TypeOf<ObjectDisposedException>().With.Message.EqualTo(
              "A disposed NonPersistentProvider cannot be accessed.\r\nObject name: 'NonPersistentProvider'."));
    }

    [Test]
    public void ExecuteScalarQuery ()
    {
      var queryStub = new Mock<IQuery>();

      var result = _provider.ExecuteScalarQuery(queryStub.Object);

      Assert.That(result, Is.Null);
    }

    [Test]
    public void ExecuteScalarQuery_Disposed ()
    {
      var queryStub = new Mock<IQuery>();
      _provider.Dispose();

      Assert.That(
          () => _provider.ExecuteScalarQuery(queryStub.Object),
          Throws.Exception.TypeOf<ObjectDisposedException>().With.Message.EqualTo(
              "A disposed NonPersistentProvider cannot be accessed.\r\nObject name: 'NonPersistentProvider'."));
    }

    [Test]
    public void BeginTransaction ()
    {
      _provider.BeginTransaction();
    }

    [Test]
    public void BeginTransaction_Disposed ()
    {
      _provider.Dispose();

      Assert.That(
          () => _provider.BeginTransaction(),
          Throws.Exception.TypeOf<ObjectDisposedException>().With.Message.EqualTo(
              "A disposed NonPersistentProvider cannot be accessed.\r\nObject name: 'NonPersistentProvider'."));
    }

    [Test]
    public void Commit ()
    {
      _provider.Commit();
    }

    [Test]
    public void Commit_Disposed ()
    {
      _provider.Dispose();

      Assert.That(
          () => _provider.Commit(),
          Throws.Exception.TypeOf<ObjectDisposedException>().With.Message.EqualTo(
              "A disposed NonPersistentProvider cannot be accessed.\r\nObject name: 'NonPersistentProvider'."));
    }

    [Test]
    public void Rollback ()
    {
      _provider.Rollback();
    }

    [Test]
    public void Rollback_Disposed ()
    {
      _provider.Dispose();

      Assert.That(
          () => _provider.Rollback(),
          Throws.Exception.TypeOf<ObjectDisposedException>().With.Message.EqualTo(
              "A disposed NonPersistentProvider cannot be accessed.\r\nObject name: 'NonPersistentProvider'."));
    }
  }
}
