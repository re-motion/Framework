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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting.Enumerables;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class SubPersistenceStrategyTest : ClientTransactionBaseTest
  {
    private Mock<IParentTransactionContext> _parentTransactionContextMock;
    private Mock<IUnlockedParentTransactionContext> _unlockedParentTransactionContextMock;
    private SubPersistenceStrategy _persistenceStrategy;

    private Mock<IQuery> _queryStub;

    private PropertyDefinition _orderNumberPropertyDefinition;
    private PropertyDefinition _fileNamePropertyDefinition;
    private PropertyDefinition _productPropertyDefinition;

    private RelationEndPointID _virtualObjectRelationEndPointID;
    private RelationEndPointID _collectionEndPointID;
    private RelationEndPointID _nonVirtualEndPointID;

    private Mock<ILoadedObjectDataProvider> _alreadyLoadedObjectDataProviderMock;

    public override void SetUp ()
    {
      base.SetUp();

      _parentTransactionContextMock = new Mock<IParentTransactionContext>(MockBehavior.Strict);
      _unlockedParentTransactionContextMock = new Mock<IUnlockedParentTransactionContext>(MockBehavior.Strict);
      _persistenceStrategy = new SubPersistenceStrategy(_parentTransactionContextMock.Object);

      _queryStub = new Mock<IQuery>();

      _orderNumberPropertyDefinition = GetPropertyDefinition(typeof(Order), "OrderNumber");
      _fileNamePropertyDefinition = GetPropertyDefinition(typeof(OrderTicket), "FileName");
      _productPropertyDefinition = GetPropertyDefinition(typeof(OrderItem), "Product");

      _virtualObjectRelationEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, GetEndPointDefinition(typeof(Order), "OrderTicket"));
      _collectionEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, GetEndPointDefinition(typeof(Order), "OrderItems"));
      _nonVirtualEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, GetEndPointDefinition(typeof(Order), "Customer"));

      _alreadyLoadedObjectDataProviderMock = new Mock<ILoadedObjectDataProvider>(MockBehavior.Strict);
    }

    [Test]
    public void CreateNewObjectID ()
    {
      var classDefinition = GetClassDefinition(typeof(Order));
      var fakeResult = DomainObjectIDs.Order1;

      _parentTransactionContextMock
          .Setup(mock => mock.CreateNewObjectID(classDefinition))
          .Returns(fakeResult)
          .Verifiable();

      var result = _persistenceStrategy.CreateNewObjectID(classDefinition);

      _parentTransactionContextMock.Verify();
      Assert.That(result, Is.SameAs(fakeResult));
    }

    [Test]
    public void LoadObjectData_Single ()
    {
      var objectID = DomainObjectIDs.Order1;
      var parentObject = DomainObjectMother.CreateFakeObject<Order>(objectID);
      var parentDataContainer = CreateChangedDataContainer(objectID, 4711, _orderNumberPropertyDefinition, 17);
      CheckDataContainer(parentDataContainer, objectID, 4711, state => state.IsChanged && !state.IsNewInHierarchy, _orderNumberPropertyDefinition, 17, 0, true);

      var parentEventListenerMock = new Mock<IDataContainerEventListener>(MockBehavior.Strict);
      parentDataContainer.SetEventListener(parentEventListenerMock.Object);

      _parentTransactionContextMock
          .Setup(mock => mock.TryGetObject(objectID))
          .Returns(parentObject)
          .Verifiable();
      _parentTransactionContextMock
          .Setup(mock => mock.GetDataContainerWithLazyLoad(objectID, true))
          .Returns(parentDataContainer)
          .Verifiable();

      var result = _persistenceStrategy.LoadObjectData(objectID);

      _parentTransactionContextMock.Verify();
      parentEventListenerMock.Verify(mock => mock.PropertyValueReading(It.IsAny<DataContainer>(), It.IsAny<PropertyDefinition>(), It.IsAny<ValueAccess>()), Times.Never());

      Assert.That(result, Is.TypeOf<FreshlyLoadedObjectData>());
      Assert.That(result.ObjectID, Is.EqualTo(objectID));
      var dataContainer = ((FreshlyLoadedObjectData)result).FreshlyLoadedDataContainer;
      CheckDataContainer(dataContainer, objectID, 4711, state => state.IsUnchanged && !state.IsNewInHierarchy, _orderNumberPropertyDefinition, 17, 17, false);
    }

    [Test]
    public void LoadObjectData_Single_PropagatesExceptionsFromGetDataContainer ()
    {
      var objectID = DomainObjectIDs.Order1;
      var parentObject = DomainObjectMother.CreateFakeObject<Order>(objectID);
      var exception = new Exception("E.g., object is invalid.");

      _parentTransactionContextMock
          .Setup(mock => mock.TryGetObject(objectID))
          .Returns(parentObject)
          .Verifiable();
      _parentTransactionContextMock
          .Setup(mock => mock.GetDataContainerWithLazyLoad(objectID, true))
          .Throws(exception)
          .Verifiable();

      Assert.That(() => _persistenceStrategy.LoadObjectData(objectID), Throws.Exception.SameAs(exception));

      _parentTransactionContextMock.Verify();
    }

    [Test]
    public void LoadObjectData_Single_ParentObjectIsNewInHierarchy ()
    {
      var objectID = DomainObjectIDs.Order1;
      var parentObject = DomainObjectMother.CreateFakeObject<Order>(objectID);
      var parentDataContainer = CreateChangedDataContainer(objectID, 4711, _orderNumberPropertyDefinition, 17);
      parentDataContainer.SetNewInHierarchy();
      CheckDataContainer(parentDataContainer, objectID, 4711, state => state.IsChanged && state.IsNewInHierarchy, _orderNumberPropertyDefinition, 17, 0, true);

      var parentEventListenerMock = new Mock<IDataContainerEventListener>(MockBehavior.Strict);
      parentDataContainer.SetEventListener(parentEventListenerMock.Object);

      _parentTransactionContextMock
          .Setup(mock => mock.TryGetObject(objectID))
          .Returns(parentObject)
          .Verifiable();
      _parentTransactionContextMock
          .Setup(mock => mock.GetDataContainerWithLazyLoad(objectID, true))
          .Returns(parentDataContainer)
          .Verifiable();

      var result = _persistenceStrategy.LoadObjectData(objectID);

      _parentTransactionContextMock.Verify();
      parentEventListenerMock.Verify(mock => mock.PropertyValueReading(It.IsAny<DataContainer>(), It.IsAny<PropertyDefinition>(), It.IsAny<ValueAccess>()), Times.Never());

      Assert.That(result, Is.TypeOf<FreshlyLoadedObjectData>());
      Assert.That(result.ObjectID, Is.EqualTo(objectID));
      var dataContainer = ((FreshlyLoadedObjectData)result).FreshlyLoadedDataContainer;
      CheckDataContainer(dataContainer, objectID, 4711, state => state.IsUnchanged && state.IsNewInHierarchy, _orderNumberPropertyDefinition, 17, 17, false);
    }

    [Test]
    public void LoadObjectData_Single_ParentObjectIsDeleted ()
    {
      var objectID = DomainObjectIDs.Order1;
      var parentObject = DomainObjectMother.CreateFakeObject<Order>(objectID);
      var deletedParentDataContainer = DataContainerObjectMother.CreateDeleted(objectID);

      _parentTransactionContextMock
          .Setup(mock => mock.TryGetObject(objectID))
          .Returns(parentObject)
          .Verifiable();
      _parentTransactionContextMock
          .Setup(mock => mock.GetDataContainerWithLazyLoad(objectID, true))
          .Returns(deletedParentDataContainer)
          .Verifiable();

      Assert.That(
          () => _persistenceStrategy.LoadObjectData(objectID),
          Throws.TypeOf<ObjectDeletedException>().With.Message.EqualTo(
              "Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' is already deleted in the parent transaction."));

      _parentTransactionContextMock.Verify();
    }

    [Test]
    public void LoadObjectData_Single_NoParentObjectFound ()
    {
      var objectID = DomainObjectIDs.Order1;

      _parentTransactionContextMock
          .Setup(mock => mock.TryGetObject(objectID))
          .Returns((DomainObject)null)
          .Verifiable();

      var result = _persistenceStrategy.LoadObjectData(objectID);

      _parentTransactionContextMock.Verify();
      Assert.That(result, Is.TypeOf<NotFoundLoadedObjectData>());
      Assert.That(result.ObjectID, Is.EqualTo(objectID));
    }

    [Test]
    public void LoadObjectData_Multiple ()
    {
      var objectID1 = DomainObjectIDs.Order1;
      var objectID2 = DomainObjectIDs.Order3;
      var objectID3 = DomainObjectIDs.Order4;

      var parentObject1 = DomainObjectMother.CreateFakeObject<Order>(objectID1);
      var parentDataContainer1 = CreateChangedDataContainer(objectID1, 4711, _orderNumberPropertyDefinition, 17);
      CheckDataContainer(parentDataContainer1, objectID1, 4711, state => state.IsChanged && !state.IsNewInHierarchy, _orderNumberPropertyDefinition, 17, 0, true);

      var parentObject3 = DomainObjectMother.CreateFakeObject<Order>(objectID3);
      var parentDataContainer3 = DataContainerObjectMother.CreateExisting(objectID3);

      var parentEventListenerMock = new Mock<IDataContainerEventListener>(MockBehavior.Strict);
      parentDataContainer1.SetEventListener(parentEventListenerMock.Object);
      parentDataContainer3.SetEventListener(parentEventListenerMock.Object);

      // Use a strict mock because the parameter should not be enumerated, it should only be passed on TryGetObjects

      _parentTransactionContextMock
          .Setup(mock => mock.TryGetObjects(new[] { objectID1, objectID2, objectID3 }))
          .Returns(new DomainObject[] { parentObject1, null, parentObject3 })
          .Verifiable();
      _parentTransactionContextMock
          .Setup(mock => mock.GetDataContainerWithLazyLoad(objectID1, true))
          .Returns(parentDataContainer1)
          .Verifiable();
      _parentTransactionContextMock
          .Setup(mock => mock.GetDataContainerWithLazyLoad(objectID3, true))
          .Returns(parentDataContainer3)
          .Verifiable();

      var result = _persistenceStrategy.LoadObjectData(new[] { objectID1, objectID2, objectID3 }.AsOneTime()).ToList();

      _parentTransactionContextMock.Verify();
      parentEventListenerMock
          .Verify(
              mock => mock.PropertyValueReading(It.IsAny<DataContainer>(), It.IsAny<PropertyDefinition>(), It.IsAny<ValueAccess>()),
              Times.Never());

      Assert.That(result[0], Is.TypeOf<FreshlyLoadedObjectData>());
      Assert.That(result[0].ObjectID, Is.EqualTo(objectID1));
      CheckDataContainer(
          ((FreshlyLoadedObjectData)result[0]).FreshlyLoadedDataContainer,
          objectID1,
          4711,
          state => state.IsUnchanged && !state.IsNewInHierarchy,
          _orderNumberPropertyDefinition,
          17,
          17,
          false);

      Assert.That(result[1], Is.TypeOf<NotFoundLoadedObjectData>());
      Assert.That(result[1].ObjectID, Is.EqualTo(objectID2));

      Assert.That(result[2], Is.TypeOf<FreshlyLoadedObjectData>());
      Assert.That(result[2].ObjectID, Is.EqualTo(objectID3));
    }

    [Test]
    public void LoadObjectData_Multiple_PropagatesExceptionsFromGetDataContainer ()
    {
      var objectID = DomainObjectIDs.Order1;
      var parentObject = DomainObjectMother.CreateFakeObject<Order>();
      var exception = new Exception("E.g., object is invalid.");

      _parentTransactionContextMock
          .Setup(mock => mock.TryGetObject(objectID))
          .Returns(parentObject)
          .Verifiable();
      _parentTransactionContextMock
          .Setup(mock => mock.GetDataContainerWithLazyLoad(objectID, true))
          .Throws(exception)
          .Verifiable();

      Assert.That(() => _persistenceStrategy.LoadObjectData(objectID), Throws.Exception.SameAs(exception));

      _parentTransactionContextMock.Verify();
    }

    [Test]
    public void LoadObjectData_Multiple_ParentObjectIsNewInHierarchy ()
    {
      var objectID1 = DomainObjectIDs.Order1;
      var objectID2 = DomainObjectIDs.Order4;

      var parentObject1 = DomainObjectMother.CreateFakeObject<Order>(objectID1);
      var parentDataContainer1 = CreateChangedDataContainer(objectID1, 4711, _orderNumberPropertyDefinition, 17);
      parentDataContainer1.SetNewInHierarchy();
      CheckDataContainer(parentDataContainer1, objectID1, 4711, state => state.IsChanged && state.IsNewInHierarchy, _orderNumberPropertyDefinition, 17, 0, true);

      var parentObject2 = DomainObjectMother.CreateFakeObject<Order>(objectID2);
      var parentDataContainer2 = DataContainerObjectMother.CreateExisting(objectID2);

      var parentEventListenerMock = new Mock<IDataContainerEventListener>(MockBehavior.Strict);
      parentDataContainer1.SetEventListener(parentEventListenerMock.Object);
      parentDataContainer2.SetEventListener(parentEventListenerMock.Object);

      // Use a strict mock because the parameter should not be enumerated, it should only be passed on TryGetObjects

      _parentTransactionContextMock
          .Setup(mock => mock.TryGetObjects(new[] { objectID1, objectID2 }))
          .Returns(new DomainObject[] { parentObject1, parentObject2 })
          .Verifiable();
      _parentTransactionContextMock
          .Setup(mock => mock.GetDataContainerWithLazyLoad(objectID1, true))
          .Returns(parentDataContainer1)
          .Verifiable();
      _parentTransactionContextMock
          .Setup(mock => mock.GetDataContainerWithLazyLoad(objectID2, true))
          .Returns(parentDataContainer2)
          .Verifiable();

      var result = _persistenceStrategy.LoadObjectData(new[] { objectID1, objectID2 }.AsOneTime()).ToList();

      _parentTransactionContextMock.Verify();
      parentEventListenerMock
          .Verify(
              mock => mock.PropertyValueReading(It.IsAny<DataContainer>(), It.IsAny<PropertyDefinition>(), It.IsAny<ValueAccess>()),
              Times.Never());

      Assert.That(result[0], Is.TypeOf<FreshlyLoadedObjectData>());
      Assert.That(result[0].ObjectID, Is.EqualTo(objectID1));
      Assert.That(((FreshlyLoadedObjectData)result[0]).FreshlyLoadedDataContainer.State.IsUnchanged, Is.True);
      Assert.That(((FreshlyLoadedObjectData)result[0]).FreshlyLoadedDataContainer.State.IsNewInHierarchy, Is.True);

      Assert.That(result[1], Is.TypeOf<FreshlyLoadedObjectData>());
      Assert.That(result[1].ObjectID, Is.EqualTo(objectID2));
      Assert.That(((FreshlyLoadedObjectData)result[1]).FreshlyLoadedDataContainer.State.IsUnchanged, Is.True);
      Assert.That(((FreshlyLoadedObjectData)result[1]).FreshlyLoadedDataContainer.State.IsNewInHierarchy, Is.False);
    }

    [Test]
    public void LoadObjectData_Multiple_ParentObjectIsDeleted ()
    {
      var objectID = DomainObjectIDs.Order1;
      var parentObject = DomainObjectMother.CreateFakeObject<Order>();
      var deletedParentDataContainer = DataContainerObjectMother.CreateDeleted(objectID);

      _parentTransactionContextMock
          .Setup(mock => mock.TryGetObject(objectID))
          .Returns(parentObject)
          .Verifiable();
      _parentTransactionContextMock
          .Setup(mock => mock.GetDataContainerWithLazyLoad(objectID, true))
          .Returns(deletedParentDataContainer)
          .Verifiable();

      Assert.That(
          () => _persistenceStrategy.LoadObjectData(objectID),
          Throws.TypeOf<ObjectDeletedException>().With.Message.EqualTo(
              "Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' is already deleted in the parent transaction."));

      _parentTransactionContextMock.Verify();
    }

    [Test]
    public void LoadObjectData_Multiple_NoParentObjectFound ()
    {
      var objectID = DomainObjectIDs.Order1;

      _parentTransactionContextMock
          .Setup(mock => mock.TryGetObject(objectID))
          .Returns((DomainObject)null)
          .Verifiable();

      var result = _persistenceStrategy.LoadObjectData(objectID);

      _parentTransactionContextMock.Verify();
      Assert.That(result, Is.TypeOf<NotFoundLoadedObjectData>());
      Assert.That(result.ObjectID, Is.EqualTo(objectID));
    }

    [Test]
    public void ResolveObjectRelationData_ResolvesToAlreadyLoadedObject ()
    {
      var parentObject = DomainObjectMother.CreateFakeObject<OrderTicket>();
      _parentTransactionContextMock
          .Setup(mock => mock.ResolveRelatedObject(_virtualObjectRelationEndPointID))
          .Returns(parentObject)
          .Verifiable();

      var alreadyLoadedObjectData = new Mock<ILoadedObjectData>();
      _alreadyLoadedObjectDataProviderMock
          .Setup(mock => mock.GetLoadedObject(parentObject.ID))
          .Returns(alreadyLoadedObjectData.Object)
          .Verifiable();

      var result = _persistenceStrategy.ResolveObjectRelationData(_virtualObjectRelationEndPointID, _alreadyLoadedObjectDataProviderMock.Object);

      _parentTransactionContextMock.Verify();
      _alreadyLoadedObjectDataProviderMock.Verify();
      Assert.That(result, Is.SameAs(alreadyLoadedObjectData.Object));
    }

    [Test]
    public void ResolveObjectRelationData_ResolvesToNotYetLoadedObject ()
    {
      var parentObject = DomainObjectMother.CreateFakeObject<OrderTicket>();
      _parentTransactionContextMock
          .Setup(mock => mock.ResolveRelatedObject(_virtualObjectRelationEndPointID))
          .Returns(parentObject)
          .Verifiable();
      _alreadyLoadedObjectDataProviderMock
          .Setup(mock => mock.GetLoadedObject(parentObject.ID))
          .Returns((ILoadedObjectData)null)
          .Verifiable();

      var parentDataContainer = CreateChangedDataContainer(parentObject.ID, 4711, _fileNamePropertyDefinition, "Hugo");
      CheckDataContainer(parentDataContainer, parentObject.ID, 4711, state => state.IsChanged, _fileNamePropertyDefinition, "Hugo", null, true);
      _parentTransactionContextMock
          .Setup(mock => mock.GetDataContainerWithLazyLoad(parentObject.ID, true))
          .Returns(parentDataContainer)
          .Verifiable();

      var result = _persistenceStrategy.ResolveObjectRelationData(_virtualObjectRelationEndPointID, _alreadyLoadedObjectDataProviderMock.Object);

      _parentTransactionContextMock.Verify();
      _alreadyLoadedObjectDataProviderMock.Verify();
      Assert.That(result, Is.TypeOf<FreshlyLoadedObjectData>());
      Assert.That(result.ObjectID, Is.EqualTo(parentObject.ID));

      var dataContainer = ((FreshlyLoadedObjectData)result).FreshlyLoadedDataContainer;
      CheckDataContainer(dataContainer, parentObject.ID, 4711, state => state.IsUnchanged, _fileNamePropertyDefinition, "Hugo", "Hugo", false);
    }

    [Test]
    public void ResolveObjectRelationData_ResolvesToNull ()
    {
      _parentTransactionContextMock
          .Setup(mock => mock.ResolveRelatedObject(_virtualObjectRelationEndPointID))
          .Returns((DomainObject)null)
          .Verifiable();

      var result = _persistenceStrategy.ResolveObjectRelationData(_virtualObjectRelationEndPointID, _alreadyLoadedObjectDataProviderMock.Object);

      _parentTransactionContextMock.Verify();
      Assert.That(result, Is.TypeOf<NullLoadedObjectData>());
    }

    [Test]
    public void ResolveObjectRelationData_NonVirtualObjectEndPoint ()
    {
      Assert.That(
          () => _persistenceStrategy.ResolveObjectRelationData(_collectionEndPointID, _alreadyLoadedObjectDataProviderMock.Object),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "ResolveObjectRelationData can only be called for virtual object end points.",
              "relationEndPointID"));
      Assert.That(
          () => _persistenceStrategy.ResolveObjectRelationData(_nonVirtualEndPointID, _alreadyLoadedObjectDataProviderMock.Object),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "ResolveObjectRelationData can only be called for virtual object end points.",
              "relationEndPointID"));
    }

    [Test]
    public void ResolveCollectionRelationData ()
    {
      var parentObjects =
          new DomainObject[]
          {
              DomainObjectMother.CreateFakeObject<OrderItem>(),
              DomainObjectMother.CreateFakeObject<OrderItem>()
          };
      _parentTransactionContextMock
          .Setup(mock => mock.ResolveRelatedObjects(_collectionEndPointID))
          .Returns(parentObjects.AsOneTime())
          .Verifiable();

      var alreadyLoadedObjectData = new Mock<ILoadedObjectData>();
      _alreadyLoadedObjectDataProviderMock
          .Setup(mock => mock.GetLoadedObject(parentObjects[0].ID))
          .Returns(alreadyLoadedObjectData.Object)
          .Verifiable();
      _alreadyLoadedObjectDataProviderMock
          .Setup(mock => mock.GetLoadedObject(parentObjects[1].ID))
          .Returns((ILoadedObjectData)null)
          .Verifiable();

      var parentDataContainer = CreateChangedDataContainer(parentObjects[1].ID, 4711, _productPropertyDefinition, "Keyboard");
      CheckDataContainer(parentDataContainer, parentObjects[1].ID, 4711, state => state.IsChanged, _productPropertyDefinition, "Keyboard", null, true);
      _parentTransactionContextMock
          .Setup(mock => mock.GetDataContainerWithLazyLoad(parentObjects[1].ID, true))
          .Returns(parentDataContainer)
          .Verifiable();

      var result = _persistenceStrategy.ResolveCollectionRelationData(_collectionEndPointID, _alreadyLoadedObjectDataProviderMock.Object).ToList();

      _parentTransactionContextMock.Verify();
      _alreadyLoadedObjectDataProviderMock.Verify();
      Assert.That(result[0], Is.SameAs(alreadyLoadedObjectData.Object));
      Assert.That(result[1], Is.TypeOf<FreshlyLoadedObjectData>());
      var dataContainer = ((FreshlyLoadedObjectData)result[1]).FreshlyLoadedDataContainer;
      CheckDataContainer(dataContainer, parentDataContainer.ID, 4711, state => state.IsUnchanged, _productPropertyDefinition, "Keyboard", "Keyboard", false);
    }

    [Test]
    public void ResolveCollectionRelationData_NonCollectionEndPoint ()
    {
      Assert.That(
          () => _persistenceStrategy.ResolveCollectionRelationData(_virtualObjectRelationEndPointID, _alreadyLoadedObjectDataProviderMock.Object),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "ResolveCollectionRelationData can only be called for CollectionEndPoints.",
              "relationEndPointID"));
      Assert.That(
          () => _persistenceStrategy.ResolveCollectionRelationData(_nonVirtualEndPointID, _alreadyLoadedObjectDataProviderMock.Object),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "ResolveCollectionRelationData can only be called for CollectionEndPoints.",
              "relationEndPointID"));
    }

    [Test]
    public void ExecuteCustomQuery ()
    {
      var fakeResult = new IQueryResultRow[0];

      _parentTransactionContextMock
          .Setup(mock => mock.ExecuteCustomQuery(_queryStub.Object))
          .Returns(fakeResult)
          .Verifiable();

      var result = _persistenceStrategy.ExecuteCustomQuery(_queryStub.Object);

      _parentTransactionContextMock.Verify();
      Assert.That(result, Is.SameAs(fakeResult));
    }

    [Test]
    public void ExecuteScalarQuery ()
    {
      var fakeResult = new object();

      _parentTransactionContextMock
          .Setup(mock => mock.ExecuteScalarQuery(_queryStub.Object))
          .Returns(fakeResult)
          .Verifiable();

      var result = _persistenceStrategy.ExecuteScalarQuery(_queryStub.Object);

      _parentTransactionContextMock.Verify();
      Assert.That(result, Is.SameAs(fakeResult));
    }

    [Test]
    public void PersistData_UnchangedDataContainer ()
    {
      var persistableData = CreatePersistableDataForExistingOrder();
      Assert.That(persistableData.DataContainer.State.IsUnchanged, Is.True);

      _parentTransactionContextMock.Setup(mock => mock.UnlockParentTransaction()).Returns(_unlockedParentTransactionContextMock.Object).Verifiable();
      _unlockedParentTransactionContextMock.Setup(mock => mock.Dispose()).Verifiable();

      _persistenceStrategy.PersistData(new[] { persistableData }.AsOneTime());

      _parentTransactionContextMock.Verify();
      _unlockedParentTransactionContextMock.Verify();
    }

    [Test]
    public void PersistData_ChangedDataContainer ()
    {
      var persistableData = CreatePersistableDataForExistingOrder();
      persistableData.DataContainer.SetTimestamp(1676);
      SetPropertyValue(persistableData.DataContainer, typeof(Order), "OrderNumber", 12);
      Assert.That(persistableData.DataContainer.State.IsChanged, Is.True);

      _parentTransactionContextMock.Setup(mock => mock.UnlockParentTransaction()).Returns(_unlockedParentTransactionContextMock.Object).Verifiable();

      var parentDataContainer = DataContainerObjectMother.CreateExisting(persistableData.DomainObject);
      parentDataContainer.SetTimestamp(4711);
      Assert.That(parentDataContainer.State.IsUnchanged, Is.True);
      _parentTransactionContextMock
          .Setup(mock => mock.GetDataContainerWithoutLoading(persistableData.DomainObject.ID))
          .Returns(parentDataContainer)
          .Verifiable();

      _unlockedParentTransactionContextMock
          .Setup(mock => mock.Dispose())
          .Callback(
              () =>
              {
                Assert.That(parentDataContainer.Timestamp, Is.EqualTo(1676), "ParentDataContainer must be changed prior to Dispose.");
                Assert.That(GetPropertyValue(parentDataContainer, typeof(Order), "OrderNumber"), Is.EqualTo(12));
                Assert.That(parentDataContainer.State.IsChanged, Is.True);
                Assert.That(parentDataContainer.HasBeenMarkedChanged, Is.False);
              })
          .Verifiable();

      _persistenceStrategy.PersistData(new[] { persistableData }.AsOneTime());

      _parentTransactionContextMock.Verify();
      _unlockedParentTransactionContextMock.Verify();
    }

    [Test]
    public void PersistData_MarkedAsChangedDataContainer ()
    {
      var persistableData1 = CreateMarkAsChangedPersistableDataForOrder();
      var persistableData2 = CreateMarkAsChangedPersistableDataForOrder();
      var persistableData3 = CreateMarkAsChangedPersistableDataForOrder();

      _parentTransactionContextMock.Setup(mock => mock.UnlockParentTransaction()).Returns(_unlockedParentTransactionContextMock.Object).Verifiable();

      var unchangedParentDataContainer = DataContainerObjectMother.CreateExisting(persistableData1.DomainObject);
      _parentTransactionContextMock
          .Setup(mock => mock.GetDataContainerWithoutLoading(persistableData1.DomainObject.ID))
          .Returns(unchangedParentDataContainer)
          .Verifiable();

      var changedParentDataContainer = DataContainerObjectMother.CreateExisting(persistableData2.DomainObject);
      SetPropertyValue(changedParentDataContainer, typeof(Order), "OrderNumber", 23);
      _parentTransactionContextMock
          .Setup(mock => mock.GetDataContainerWithoutLoading(persistableData2.DomainObject.ID))
          .Returns(changedParentDataContainer)
          .Verifiable();

      var newParentDataContainer = DataContainerObjectMother.CreateNew(persistableData3.DomainObject);
      _parentTransactionContextMock
          .Setup(mock => mock.GetDataContainerWithoutLoading(persistableData3.DomainObject.ID))
          .Returns(newParentDataContainer)
          .Verifiable();

      _unlockedParentTransactionContextMock
          .Setup(mock => mock.Dispose())
          .Callback(
              () =>
              {
                Assert.That(unchangedParentDataContainer.HasBeenMarkedChanged, Is.True);
                Assert.That(changedParentDataContainer.HasBeenMarkedChanged, Is.True);
                Assert.That(newParentDataContainer.HasBeenMarkedChanged, Is.False);
              })
          .Verifiable();

      _persistenceStrategy.PersistData(new[] { persistableData1, persistableData2, persistableData3 }.AsOneTime());

      _parentTransactionContextMock.Verify();
      _unlockedParentTransactionContextMock.Verify();
    }

    [Test]
    public void PersistData_EndPoints ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      var persistedDataContainer = DataContainerObjectMother.CreateExisting(domainObject);
      var persistedEndPoint1 = RelationEndPointObjectMother.CreateStub();
      persistedEndPoint1.Setup(stub => stub.HasChanged).Returns(true);
      var persistedEndPoint2 = RelationEndPointObjectMother.CreateStub();
      persistedEndPoint2.Setup(stub => stub.HasChanged).Returns(false);
      var persistedEndPoint3 = RelationEndPointObjectMother.CreateStub();
      persistedEndPoint3.Setup(stub => stub.HasChanged).Returns(true);
      var persistableData = new PersistableData(
          domainObject,
          new DomainObjectState.Builder().SetChanged().Value,
          persistedDataContainer,
          new[] { persistedEndPoint1.Object, persistedEndPoint2.Object, persistedEndPoint3.Object });

      var sequence = new VerifiableSequence();

      _parentTransactionContextMock.Setup(mock => mock.UnlockParentTransaction()).Returns(_unlockedParentTransactionContextMock.Object).Verifiable();

      var parentEndPointMock1 = new Mock<IRelationEndPoint>(MockBehavior.Strict);
      _parentTransactionContextMock
          .Setup(mock => mock.GetRelationEndPointWithoutLoading(persistedEndPoint1.Object.ID))
          .Returns(parentEndPointMock1.Object)
          .Verifiable();
      parentEndPointMock1
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.SetDataFromSubTransaction(persistedEndPoint1.Object))
          .Verifiable("SetDataFromSubTransaction must occur prior to Dispose.");

      var parentEndPointMock3 = new Mock<IRelationEndPoint>(MockBehavior.Strict);
      _parentTransactionContextMock
          .Setup(mock => mock.GetRelationEndPointWithoutLoading(persistedEndPoint3.Object.ID))
          .Returns(parentEndPointMock3.Object)
          .Verifiable();
      parentEndPointMock3
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.SetDataFromSubTransaction(persistedEndPoint3.Object))
          .Verifiable("SetDataFromSubTransaction must occur prior to Dispose.");
      _unlockedParentTransactionContextMock
          .Setup(mock => mock.Dispose())
          .Verifiable("Dispose should come at the end.");

      _persistenceStrategy.PersistData(new[] { persistableData }.AsOneTime());

      _parentTransactionContextMock.Verify();
      _unlockedParentTransactionContextMock.Verify();
      parentEndPointMock1.Verify();
      parentEndPointMock3.Verify();
      sequence.Verify();
    }

    [Test]
    public void PersistData_NewDataContainer_WithEndPoint ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      var persistedDataContainer = DataContainerObjectMother.CreateNew(domainObject);
      SetPropertyValue(persistedDataContainer, typeof(Order), "OrderNumber", 12);
      var persistedEndPoint = RelationEndPointObjectMother.CreateStub();
      persistedEndPoint.Setup(stub => stub.HasChanged).Returns(true);
      var persistableData = new PersistableData(
          domainObject,
          new DomainObjectState.Builder().SetNew().Value,
          persistedDataContainer,
          new[] { persistedEndPoint.Object });

      var sequence = new VerifiableSequence();

      _parentTransactionContextMock.Setup(mock => mock.UnlockParentTransaction()).Returns(_unlockedParentTransactionContextMock.Object).Verifiable();

      _parentTransactionContextMock.Setup(stub => stub.IsInvalid(domainObject.ID)).Returns(true);
      _parentTransactionContextMock.Setup(stub => stub.GetDataContainerWithoutLoading(domainObject.ID)).Returns((DataContainer)null);
      _unlockedParentTransactionContextMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.MarkNotInvalid(domainObject.ID))
          .Verifiable();
      _unlockedParentTransactionContextMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RegisterDataContainer(It.IsAny<DataContainer>()))
          .Callback(
              (DataContainer dataContainer) =>
                  CheckDataContainer(dataContainer, domainObject, null, state => state.IsNew, _orderNumberPropertyDefinition, 12, 0, true)
              )
          .Verifiable("New DataContainers must be registered before the parent relation end-points are retrieved for persistence (and prior to Dispose).");

      var parentEndPointMock = new Mock<IRelationEndPoint>(MockBehavior.Strict);
      _parentTransactionContextMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.GetRelationEndPointWithoutLoading(persistedEndPoint.Object.ID))
          .Returns(parentEndPointMock.Object)
          .Verifiable("New DataContainers must be registered before the parent relation end-points are retrieved for persistence.");
      parentEndPointMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.SetDataFromSubTransaction(persistedEndPoint.Object))
          .Verifiable("SetDataFromSubTransaction must occur prior to Dispose.");
      _unlockedParentTransactionContextMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Dispose())
          .Verifiable("Dispose should come at the end.");

      _persistenceStrategy.PersistData(new[] { persistableData }.AsOneTime());

      _parentTransactionContextMock.Verify();
      _unlockedParentTransactionContextMock.Verify();
      parentEndPointMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void PersistData_NewDataContainer_CopiesOriginalValue ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      var persistedDataContainer = DataContainerObjectMother.CreateNew(domainObject);
      SetPropertyValue(persistedDataContainer, typeof(Order), "OrderNumber", 12);
      persistedDataContainer.CommitValue(GetPropertyDefinition(typeof(Order), "OrderNumber"));
      SetPropertyValue(persistedDataContainer, typeof(Order), "OrderNumber", 13);
      var persistableData = new PersistableData(
          domainObject,
          new DomainObjectState.Builder().SetNew().Value,
          persistedDataContainer,
          Array.Empty<IRelationEndPoint>());

      _parentTransactionContextMock.Setup(mock => mock.UnlockParentTransaction()).Returns(_unlockedParentTransactionContextMock.Object).Verifiable();
      _parentTransactionContextMock.Setup(stub => stub.IsInvalid(domainObject.ID)).Returns(true);
      _parentTransactionContextMock.Setup(stub => stub.GetDataContainerWithoutLoading(domainObject.ID)).Returns((DataContainer)null);

      _unlockedParentTransactionContextMock.Setup(mock => mock.MarkNotInvalid(domainObject.ID));
      _unlockedParentTransactionContextMock
          .Setup(mock => mock.RegisterDataContainer(It.IsAny<DataContainer>()))
          .Callback(
              (DataContainer dataContainer) =>
                  CheckDataContainer(
                      dataContainer,
                      domainObject,
                      null,
                      state => state.IsNew,
                      _orderNumberPropertyDefinition,
                      expectedCurrentPropertyValue: 13,
                      expectedOriginalPropertyValue: 12,
                      expectedHasPropertyValueBeenTouched: true)
              )
          .Verifiable();
      _unlockedParentTransactionContextMock.Setup(mock => mock.Dispose());

      _persistenceStrategy.PersistData(new[] { persistableData }.AsOneTime());

      _unlockedParentTransactionContextMock.Verify();
    }

    [Test]
    public void PersistData_DeletedDataContainer_WithEndPoint_NewInParent ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      var persistedDataContainer = DataContainerObjectMother.CreateDeleted(domainObject);
      var persistedEndPoint = RelationEndPointObjectMother.CreateStub();
      persistedEndPoint.Setup(stub => stub.HasChanged).Returns(true);
      var persistableData = new PersistableData(
          domainObject,
          new DomainObjectState.Builder().SetDeleted().Value,
          persistedDataContainer,
          new[] { persistedEndPoint.Object });

      var sequence = new VerifiableSequence();

      _parentTransactionContextMock.Setup(mock => mock.UnlockParentTransaction()).Returns(_unlockedParentTransactionContextMock.Object).Verifiable();

      var parentEndPointMock = new Mock<IRelationEndPoint>(MockBehavior.Strict);
      _parentTransactionContextMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.GetRelationEndPointWithoutLoading(persistedEndPoint.Object.ID))
          .Returns(parentEndPointMock.Object)
          .Verifiable("Deleted DataContainers must be persisted after the parent relation end-points are retrieved for persistence.");
      parentEndPointMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.SetDataFromSubTransaction(persistedEndPoint.Object))
          .Verifiable("SetDataFromSubTransaction must occur prior to Dispose.");
      var parentDataContainer = DataContainerObjectMother.CreateNew(persistedDataContainer.ID);
      parentDataContainer.SetDomainObject(domainObject);
      _parentTransactionContextMock
          .Setup(stub => stub.GetDataContainerWithoutLoading(domainObject.ID))
          .Returns(parentDataContainer);
      _unlockedParentTransactionContextMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Discard(parentDataContainer))
          .Verifiable("Deleted DataContainers must be persisted after the parent relation end-points are retrieved for persistence.");
      _unlockedParentTransactionContextMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Dispose())
          .Verifiable("Dispose should come at the end.");

      _persistenceStrategy.PersistData(new[] { persistableData }.AsOneTime());

      _parentTransactionContextMock.Verify();
      _unlockedParentTransactionContextMock.Verify();
      parentEndPointMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void PersistData_NewDataContainer_PreservesNewInHierarchyFlag ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      var persistedDataContainer = DataContainerObjectMother.CreateNew(domainObject);
      var persistableData = new PersistableData(
          domainObject,
          new DomainObjectState.Builder().SetNew().Value,
          persistedDataContainer,
          Array.Empty<IRelationEndPoint>());

      _parentTransactionContextMock.Setup(mock => mock.UnlockParentTransaction()).Returns(_unlockedParentTransactionContextMock.Object).Verifiable();
      _parentTransactionContextMock.Setup(stub => stub.IsInvalid(domainObject.ID)).Returns(true);
      _parentTransactionContextMock.Setup(stub => stub.GetDataContainerWithoutLoading(domainObject.ID)).Returns((DataContainer)null);

      _unlockedParentTransactionContextMock.Setup(mock => mock.MarkNotInvalid(domainObject.ID));
      _unlockedParentTransactionContextMock.Setup(mock => mock.RegisterDataContainer(It.IsAny<DataContainer>()));
      _unlockedParentTransactionContextMock.Setup(mock => mock.Dispose());

      Assert.That(persistedDataContainer.State.IsNew, Is.True);
      Assert.That(persistedDataContainer.State.IsNewInHierarchy, Is.True);

      _persistenceStrategy.PersistData(new[] { persistableData }.AsOneTime());

      Assert.That(persistedDataContainer.State.IsNew, Is.True);
      Assert.That(persistedDataContainer.State.IsNewInHierarchy, Is.True);
    }

    [Test]
    public void PersistData_DeletedDataContainer_WithEndPoint_ExistingInParent ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      var persistedDataContainer = DataContainerObjectMother.CreateDeleted(domainObject);
      var persistedEndPoint = RelationEndPointObjectMother.CreateStub();
      persistedEndPoint.Setup(stub => stub.HasChanged).Returns(true);
      var persistableData = new PersistableData(
          domainObject,
          new DomainObjectState.Builder().SetDeleted().Value,
          persistedDataContainer,
          new[] { persistedEndPoint.Object });

      _parentTransactionContextMock.Setup(mock => mock.UnlockParentTransaction()).Returns(_unlockedParentTransactionContextMock.Object).Verifiable();

      var sequence = new VerifiableSequence();

      var parentEndPointMock = new Mock<IRelationEndPoint>(MockBehavior.Strict);
      _parentTransactionContextMock
          .Setup(mock => mock.GetRelationEndPointWithoutLoading(persistedEndPoint.Object.ID))
          .Returns(parentEndPointMock.Object)
          .Verifiable();
      parentEndPointMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.SetDataFromSubTransaction(persistedEndPoint.Object))
          .Verifiable("SetDataFromSubTransaction must occur prior to Dispose.");

      var parentDataContainer = DataContainerObjectMother.CreateExisting(persistedDataContainer.ID);
      parentDataContainer.SetDomainObject(domainObject);
      _parentTransactionContextMock
          .Setup(stub => stub.GetDataContainerWithoutLoading(domainObject.ID))
          .Returns(parentDataContainer);
      _unlockedParentTransactionContextMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Dispose())
          .Callback(
              () => Assert.That(
                  parentDataContainer.State.IsDeleted,
                  Is.True,
                  "Parent DataContainer must be deleted before parent transaction is locked again."))
          .Verifiable("Dispose should come at the end.");

      _persistenceStrategy.PersistData(new[] { persistableData }.AsOneTime());

      _parentTransactionContextMock.Verify();
      _unlockedParentTransactionContextMock.Verify();
      parentEndPointMock.Verify();
      sequence.Verify();
    }

    private void CheckDataContainer (
        DataContainer dataContainer,
        DomainObject expectedDomainObject,
        object expectedTimestamp,
        Func<DataContainerState, bool> expectedStatePredicate,
        PropertyDefinition propertyDefinition,
        object expectedCurrentPropertyValue,
        object expectedOriginalPropertyValue,
        bool expectedHasPropertyValueBeenTouched)
    {
      Assert.That(dataContainer.HasDomainObject, Is.True);
      Assert.That(dataContainer.DomainObject, Is.SameAs(expectedDomainObject));

      CheckDataContainer(
          dataContainer,
          expectedDomainObject.ID,
          expectedTimestamp,
          expectedStatePredicate,
          propertyDefinition,
          expectedCurrentPropertyValue,
          expectedOriginalPropertyValue,
          expectedHasPropertyValueBeenTouched);
    }

    private void CheckDataContainer (
        DataContainer dataContainer,
        ObjectID expectedID,
        object expectedTimestamp,
        Func<DataContainerState, bool> expectedStatePredicate,
        PropertyDefinition propertyDefinition,
        object expectedCurrentPropertyValue,
        object expectedOriginalPropertyValue,
        bool expectedHasPropertyValueBeenTouched)
    {
      Assert.That(dataContainer.ID, Is.EqualTo(expectedID));
      Assert.That(dataContainer.Timestamp, Is.EqualTo(expectedTimestamp));
      Assert.That(expectedStatePredicate(dataContainer.State), Is.True);
      Assert.That(dataContainer.GetValue(propertyDefinition, ValueAccess.Current), Is.EqualTo(expectedCurrentPropertyValue));
      Assert.That(dataContainer.GetValue(propertyDefinition, ValueAccess.Original), Is.EqualTo(expectedOriginalPropertyValue));
      Assert.That(dataContainer.HasValueBeenTouched(propertyDefinition), Is.EqualTo(expectedHasPropertyValueBeenTouched));
    }

    private DataContainer CreateChangedDataContainer (
        ObjectID objectID,
        int timestamp,
        PropertyDefinition propertyDefinition,
        object currentPropertyValue)
    {
      var parentDataContainer = DataContainerObjectMother.CreateExisting(objectID);
      parentDataContainer.SetTimestamp(timestamp);
      parentDataContainer.SetValue(propertyDefinition, currentPropertyValue);
      return parentDataContainer;
    }

    private PersistableData CreatePersistableDataForExistingOrder ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      var dataContainer = DataContainerObjectMother.CreateExisting(domainObject.ID);
      dataContainer.SetDomainObject(domainObject);
      return new PersistableData(
          domainObject,
          new DomainObjectState.Builder().SetChanged().Value,
          dataContainer,
          new IRelationEndPoint[0]);
    }

    private PersistableData CreateMarkAsChangedPersistableDataForOrder ()
    {
      var persistableData = CreatePersistableDataForExistingOrder();
      persistableData.DataContainer.MarkAsChanged();
      return persistableData;
    }
  }
}
