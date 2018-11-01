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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Remotion.Development.UnitTesting.Enumerables;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class SubPersistenceStrategyTest : ClientTransactionBaseTest
  {
    private IParentTransactionContext _parentTransactionContextMock;
    private IUnlockedParentTransactionContext _unlockedParentTransactionContextMock;
    private SubPersistenceStrategy _persistenceStrategy;

    private IQuery _queryStub;
    
    private PropertyDefinition _orderNumberPropertyDefinition;
    private PropertyDefinition _fileNamePropertyDefinition;
    private PropertyDefinition _productPropertyDefinition;

    private RelationEndPointID _virtualObjectRelationEndPointID;
    private RelationEndPointID _collectionEndPointID;
    private RelationEndPointID _nonVirtualEndPointID;

    private ILoadedObjectDataProvider _alreadyLoadedObjectDataProviderMock;

    public override void SetUp ()
    {
      base.SetUp ();

      _parentTransactionContextMock = MockRepository.GenerateStrictMock<IParentTransactionContext> ();
      _unlockedParentTransactionContextMock = MockRepository.GenerateStrictMock<IUnlockedParentTransactionContext> ();
      _persistenceStrategy = new SubPersistenceStrategy (_parentTransactionContextMock);

      _queryStub = MockRepository.GenerateStub<IQuery>();
    
      _orderNumberPropertyDefinition = GetPropertyDefinition (typeof (Order), "OrderNumber");
      _fileNamePropertyDefinition = GetPropertyDefinition (typeof (OrderTicket), "FileName");
      _productPropertyDefinition = GetPropertyDefinition (typeof (OrderItem), "Product");

      _virtualObjectRelationEndPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, GetEndPointDefinition (typeof (Order), "OrderTicket"));
      _collectionEndPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, GetEndPointDefinition (typeof (Order), "OrderItems"));
      _nonVirtualEndPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, GetEndPointDefinition (typeof (Order), "Customer"));
      
      _alreadyLoadedObjectDataProviderMock = MockRepository.GenerateStrictMock<ILoadedObjectDataProvider>();
    }

    [Test]
    public void CreateNewObjectID ()
    {
      var classDefinition = GetTypeDefinition (typeof (Order));
      var fakeResult = DomainObjectIDs.Order1;

      _parentTransactionContextMock
          .Expect (mock => mock.CreateNewObjectID (classDefinition))
          .Return (fakeResult);

      var result = _persistenceStrategy.CreateNewObjectID (classDefinition);

      _parentTransactionContextMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void LoadObjectData_Single ()
    {
      var objectID = DomainObjectIDs.Order1;
      var parentObject = DomainObjectMother.CreateFakeObject<Order> (objectID);
      var parentDataContainer = CreateChangedDataContainer (objectID, 4711, _orderNumberPropertyDefinition, 17);
      CheckDataContainer (parentDataContainer, objectID, 4711, StateType.Changed, _orderNumberPropertyDefinition, 17, 0, true);

      var parentEventListenerMock = MockRepository.GenerateStrictMock<IDataContainerEventListener>();
      parentDataContainer.SetEventListener (parentEventListenerMock);

      _parentTransactionContextMock
          .Expect (mock => mock.TryGetObject (objectID))
          .Return (parentObject);
      _parentTransactionContextMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (objectID, true))
          .Return (parentDataContainer);

      var result = _persistenceStrategy.LoadObjectData (objectID);

      _parentTransactionContextMock.VerifyAllExpectations();
      parentEventListenerMock.AssertWasNotCalled (
          mock => mock.PropertyValueReading (Arg<DataContainer>.Is.Anything, Arg<PropertyDefinition>.Is.Anything, Arg<ValueAccess>.Is.Anything));

      Assert.That (result, Is.TypeOf<FreshlyLoadedObjectData> ());
      Assert.That (result.ObjectID, Is.EqualTo (objectID));
      var dataContainer = ((FreshlyLoadedObjectData) result).FreshlyLoadedDataContainer;
      CheckDataContainer (dataContainer, objectID, 4711, StateType.Unchanged, _orderNumberPropertyDefinition, 17, 17, false);
    }

    [Test]
    public void LoadObjectData_Single_PropagatesExceptionsFromGetDataContainer ()
    {
      var objectID = DomainObjectIDs.Order1;
      var parentObject = DomainObjectMother.CreateFakeObject<Order> (objectID);
      var exception = new Exception ("E.g., object is invalid.");

      _parentTransactionContextMock
          .Expect (mock => mock.TryGetObject (objectID))
          .Return (parentObject);
      _parentTransactionContextMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (objectID, true))
          .Throw (exception);

      Assert.That (() => _persistenceStrategy.LoadObjectData (objectID), Throws.Exception.SameAs (exception));

      _parentTransactionContextMock.VerifyAllExpectations ();
    }

    [Test]
    public void LoadObjectData_Single_ParentObjectIsDeleted ()
    {
      var objectID = DomainObjectIDs.Order1;
      var parentObject = DomainObjectMother.CreateFakeObject<Order> (objectID);
      var deletedParentDataContainer = DataContainerObjectMother.CreateDeleted (objectID);

      _parentTransactionContextMock
          .Expect (mock => mock.TryGetObject (objectID))
          .Return (parentObject);
      _parentTransactionContextMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (objectID, true))
          .Return (deletedParentDataContainer);

      Assert.That (
          () => _persistenceStrategy.LoadObjectData (objectID),
          Throws.TypeOf<ObjectDeletedException> ().With.Message.EqualTo (
            "Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' is already deleted in the parent transaction."));

      _parentTransactionContextMock.VerifyAllExpectations ();
    }

    [Test]
    public void LoadObjectData_Single_NoParentObjectFound ()
    {
      var objectID = DomainObjectIDs.Order1;

      _parentTransactionContextMock
          .Expect (mock => mock.TryGetObject (objectID))
          .Return (null);

      var result = _persistenceStrategy.LoadObjectData (objectID);

      _parentTransactionContextMock.VerifyAllExpectations();
      Assert.That (result, Is.TypeOf<NotFoundLoadedObjectData>());
      Assert.That (result.ObjectID, Is.EqualTo (objectID));
    }

    [Test]
    public void LoadObjectData_Multiple ()
    {
      var objectID1 = DomainObjectIDs.Order1;
      var objectID2 = DomainObjectIDs.Order3;
      var objectID3 = DomainObjectIDs.Order4;

      var parentObject1 = DomainObjectMother.CreateFakeObject<Order> (objectID1);
      var parentDataContainer1 = CreateChangedDataContainer (objectID1, 4711, _orderNumberPropertyDefinition, 17);
      CheckDataContainer (parentDataContainer1, objectID1, 4711, StateType.Changed, _orderNumberPropertyDefinition, 17, 0, true);

      var parentObject3 = DomainObjectMother.CreateFakeObject<Order> (objectID3);
      var parentDataContainer3 = DataContainerObjectMother.CreateExisting (objectID3);

      var parentEventListenerMock = MockRepository.GenerateStrictMock<IDataContainerEventListener> ();
      parentDataContainer1.SetEventListener (parentEventListenerMock);
      parentDataContainer3.SetEventListener (parentEventListenerMock);

      // Use a strict mock because the parameter should not be enumerated, it should only be passed on TryGetObjects

      _parentTransactionContextMock
          .Expect (mock => mock.TryGetObjects (Arg<IEnumerable<ObjectID>>.List.Equal (new[] { objectID1, objectID2, objectID3 })))
          .Return (new[] { parentObject1, null, parentObject3 });
      _parentTransactionContextMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (objectID1, true))
          .Return (parentDataContainer1);
      _parentTransactionContextMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (objectID3, true))
          .Return (parentDataContainer3);

      var result = _persistenceStrategy.LoadObjectData (new[] { objectID1, objectID2, objectID3 }.AsOneTime()).ToList();

      _parentTransactionContextMock.VerifyAllExpectations ();
      parentEventListenerMock.AssertWasNotCalled (
          mock => mock.PropertyValueReading (Arg<DataContainer>.Is.Anything, Arg<PropertyDefinition>.Is.Anything, Arg<ValueAccess>.Is.Anything));

      Assert.That (result[0], Is.TypeOf<FreshlyLoadedObjectData> ());
      Assert.That (result[0].ObjectID, Is.EqualTo (objectID1));
      CheckDataContainer (((FreshlyLoadedObjectData) result[0]).FreshlyLoadedDataContainer, objectID1, 4711, StateType.Unchanged, _orderNumberPropertyDefinition, 17, 17, false);

      Assert.That (result[1], Is.TypeOf<NotFoundLoadedObjectData> ());
      Assert.That (result[1].ObjectID, Is.EqualTo (objectID2));

      Assert.That (result[2], Is.TypeOf<FreshlyLoadedObjectData> ());
      Assert.That (result[2].ObjectID, Is.EqualTo (objectID3));
    }

    [Test]
    public void LoadObjectData_Multiple_PropagatesExceptionsFromGetDataContainer ()
    {
      var objectID = DomainObjectIDs.Order1;
      var parentObject = DomainObjectMother.CreateFakeObject<Order> ();
      var exception = new Exception ("E.g., object is invalid.");

      _parentTransactionContextMock
          .Expect (mock => mock.TryGetObject (objectID))
          .Return (parentObject);
      _parentTransactionContextMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (objectID, true))
          .Throw (exception);

      Assert.That (() => _persistenceStrategy.LoadObjectData (objectID), Throws.Exception.SameAs (exception));

      _parentTransactionContextMock.VerifyAllExpectations ();
    }

    [Test]
    public void LoadObjectData_Multiple_ParentObjectIsDeleted ()
    {
      var objectID = DomainObjectIDs.Order1;
      var parentObject = DomainObjectMother.CreateFakeObject<Order> ();
      var deletedParentDataContainer = DataContainerObjectMother.CreateDeleted (objectID);

      _parentTransactionContextMock
          .Expect (mock => mock.TryGetObject (objectID))
          .Return (parentObject);
      _parentTransactionContextMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (objectID, true))
          .Return (deletedParentDataContainer);

      Assert.That (
          () => _persistenceStrategy.LoadObjectData (objectID),
          Throws.TypeOf<ObjectDeletedException> ().With.Message.EqualTo (
            "Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' is already deleted in the parent transaction."));

      _parentTransactionContextMock.VerifyAllExpectations ();
    }

    [Test]
    public void LoadObjectData_Multiple_NoParentObjectFound ()
    {
      var objectID = DomainObjectIDs.Order1;

      _parentTransactionContextMock
          .Expect (mock => mock.TryGetObject (objectID))
          .Return (null);

      var result = _persistenceStrategy.LoadObjectData (objectID);

      _parentTransactionContextMock.VerifyAllExpectations ();
      Assert.That (result, Is.TypeOf<NotFoundLoadedObjectData> ());
      Assert.That (result.ObjectID, Is.EqualTo (objectID));
    }

    [Test]
    public void ResolveObjectRelationData_ResolvesToAlreadyLoadedObject ()
    {
      var parentObject = DomainObjectMother.CreateFakeObject<OrderTicket>();
      _parentTransactionContextMock
          .Expect (mock => mock.ResolveRelatedObject (_virtualObjectRelationEndPointID))
          .Return (parentObject);

      var alreadyLoadedObjectData = MockRepository.GenerateStub<ILoadedObjectData>();
      _alreadyLoadedObjectDataProviderMock
          .Expect (mock => mock.GetLoadedObject (parentObject.ID))
          .Return (alreadyLoadedObjectData);
      
      var result = _persistenceStrategy.ResolveObjectRelationData (_virtualObjectRelationEndPointID, _alreadyLoadedObjectDataProviderMock);

      _parentTransactionContextMock.VerifyAllExpectations ();
      _alreadyLoadedObjectDataProviderMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (alreadyLoadedObjectData));
    }

    [Test]
    public void ResolveObjectRelationData_ResolvesToNotYetLoadedObject ()
    {
      var parentObject = DomainObjectMother.CreateFakeObject<OrderTicket> ();
      _parentTransactionContextMock
          .Expect (mock => mock.ResolveRelatedObject (_virtualObjectRelationEndPointID))
          .Return (parentObject);
      _alreadyLoadedObjectDataProviderMock
          .Expect (mock => mock.GetLoadedObject (parentObject.ID))
          .Return (null);

      var parentDataContainer = CreateChangedDataContainer (parentObject.ID, 4711, _fileNamePropertyDefinition, "Hugo");
      CheckDataContainer (parentDataContainer, parentObject.ID, 4711, StateType.Changed, _fileNamePropertyDefinition, "Hugo", "", true);
      _parentTransactionContextMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (parentObject.ID, true))
          .Return (parentDataContainer);

      var result = _persistenceStrategy.ResolveObjectRelationData (_virtualObjectRelationEndPointID, _alreadyLoadedObjectDataProviderMock);

      _parentTransactionContextMock.VerifyAllExpectations ();
      _alreadyLoadedObjectDataProviderMock.VerifyAllExpectations ();
      Assert.That (result, Is.TypeOf<FreshlyLoadedObjectData> ());
      Assert.That (result.ObjectID, Is.EqualTo (parentObject.ID));
      
      var dataContainer = ((FreshlyLoadedObjectData) result).FreshlyLoadedDataContainer;
      CheckDataContainer (dataContainer, parentObject.ID, 4711, StateType.Unchanged, _fileNamePropertyDefinition, "Hugo", "Hugo", false);
    }

    [Test]
    public void ResolveObjectRelationData_ResolvesToNull ()
    {
      _parentTransactionContextMock
          .Expect (mock => mock.ResolveRelatedObject (_virtualObjectRelationEndPointID))
          .Return (null);

      var result = _persistenceStrategy.ResolveObjectRelationData (_virtualObjectRelationEndPointID, _alreadyLoadedObjectDataProviderMock);

      _parentTransactionContextMock.VerifyAllExpectations ();
      Assert.That (result, Is.TypeOf<NullLoadedObjectData> ());
    }

    [Test]
    public void ResolveObjectRelationData_NonVirtualObjectEndPoint ()
    {
      Assert.That (
          () => _persistenceStrategy.ResolveObjectRelationData (_collectionEndPointID, _alreadyLoadedObjectDataProviderMock),
          Throws.ArgumentException.With.Message.EqualTo (
            "ResolveObjectRelationData can only be called for virtual object end points.\r\nParameter name: relationEndPointID"));
      Assert.That (
          () => _persistenceStrategy.ResolveObjectRelationData (_nonVirtualEndPointID, _alreadyLoadedObjectDataProviderMock),
          Throws.ArgumentException.With.Message.EqualTo (
            "ResolveObjectRelationData can only be called for virtual object end points.\r\nParameter name: relationEndPointID"));
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
          .Expect (mock => mock.ResolveRelatedObjects (_collectionEndPointID))
          .Return (parentObjects.AsOneTime());

      var alreadyLoadedObjectData = MockRepository.GenerateStub<ILoadedObjectData> ();
      _alreadyLoadedObjectDataProviderMock
          .Expect (mock => mock.GetLoadedObject (parentObjects[0].ID))
          .Return (alreadyLoadedObjectData);
      _alreadyLoadedObjectDataProviderMock
          .Expect (mock => mock.GetLoadedObject (parentObjects[1].ID))
          .Return (null);

      var parentDataContainer = CreateChangedDataContainer (parentObjects[1].ID, 4711, _productPropertyDefinition, "Keyboard");
      CheckDataContainer (parentDataContainer, parentObjects[1].ID, 4711, StateType.Changed, _productPropertyDefinition, "Keyboard", "", true);
      _parentTransactionContextMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (parentObjects[1].ID, true))
          .Return (parentDataContainer);

      var result = _persistenceStrategy.ResolveCollectionRelationData (_collectionEndPointID, _alreadyLoadedObjectDataProviderMock).ToList();

      _parentTransactionContextMock.VerifyAllExpectations ();
      _alreadyLoadedObjectDataProviderMock.VerifyAllExpectations ();
      Assert.That (result[0], Is.SameAs (alreadyLoadedObjectData));
      Assert.That (result[1], Is.TypeOf<FreshlyLoadedObjectData> ());
      var dataContainer = ((FreshlyLoadedObjectData) result[1]).FreshlyLoadedDataContainer;
      CheckDataContainer (dataContainer, parentDataContainer.ID, 4711, StateType.Unchanged, _productPropertyDefinition, "Keyboard", "Keyboard", false);
    }

    [Test]
    public void ResolveCollectionRelationData_NonCollectionEndPoint ()
    {
      Assert.That (
          () => _persistenceStrategy.ResolveCollectionRelationData (_virtualObjectRelationEndPointID, _alreadyLoadedObjectDataProviderMock),
          Throws.ArgumentException.With.Message.EqualTo (
            "ResolveCollectionRelationData can only be called for CollectionEndPoints.\r\nParameter name: relationEndPointID"));
      Assert.That (
          () => _persistenceStrategy.ResolveCollectionRelationData (_nonVirtualEndPointID, _alreadyLoadedObjectDataProviderMock),
          Throws.ArgumentException.With.Message.EqualTo (
            "ResolveCollectionRelationData can only be called for CollectionEndPoints.\r\nParameter name: relationEndPointID"));
    }

    [Test]
    public void ExecuteCustomQuery ()
    {
      var fakeResult = new IQueryResultRow[0];
      
      _parentTransactionContextMock
          .Expect (mock => mock.ExecuteCustomQuery (_queryStub))
          .Return (fakeResult);

      var result = _persistenceStrategy.ExecuteCustomQuery (_queryStub);

      _parentTransactionContextMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void ExecuteScalarQuery ()
    {
      var fakeResult = new object();

      _parentTransactionContextMock
          .Expect (mock => mock.ExecuteScalarQuery (_queryStub))
          .Return (fakeResult);

      var result = _persistenceStrategy.ExecuteScalarQuery (_queryStub);

      _parentTransactionContextMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void PersistData_UnchangedDataContainer ()
    {
      var persistableData = CreatePersistableDataForExistingOrder ();
      Assert.That (persistableData.DataContainer.State, Is.EqualTo (StateType.Unchanged));

      _parentTransactionContextMock.Expect (mock => mock.UnlockParentTransaction ()).Return (_unlockedParentTransactionContextMock);
      _unlockedParentTransactionContextMock.Expect (mock => mock.Dispose ());

      _persistenceStrategy.PersistData (new[] { persistableData }.AsOneTime ());

      _parentTransactionContextMock.VerifyAllExpectations ();
      _unlockedParentTransactionContextMock.VerifyAllExpectations ();
    }

    [Test]
    public void PersistData_ChangedDataContainer ()
    {
      var persistableData = CreatePersistableDataForExistingOrder ();
      persistableData.DataContainer.SetTimestamp (1676);
      SetPropertyValue (persistableData.DataContainer, typeof (Order), "OrderNumber", 12);
      Assert.That (persistableData.DataContainer.State, Is.EqualTo (StateType.Changed));

      _parentTransactionContextMock.Expect (mock => mock.UnlockParentTransaction ()).Return (_unlockedParentTransactionContextMock);

      var parentDataContainer = DataContainerObjectMother.CreateExisting (persistableData.DomainObject);
      parentDataContainer.SetTimestamp (4711);
      Assert.That (parentDataContainer.State, Is.EqualTo (StateType.Unchanged));
      _parentTransactionContextMock
          .Expect (mock => mock.GetDataContainerWithoutLoading (persistableData.DomainObject.ID))
          .Return (parentDataContainer);

      _unlockedParentTransactionContextMock
          .Expect (mock => mock.Dispose ())
          .WhenCalled (mi => 
          {
            Assert.That (parentDataContainer.Timestamp, Is.EqualTo (1676), "ParentDataContainer must be changed prior to Dispose.");
            Assert.That (GetPropertyValue (parentDataContainer, typeof (Order), "OrderNumber"), Is.EqualTo (12));
            Assert.That (parentDataContainer.State, Is.EqualTo (StateType.Changed));
            Assert.That (parentDataContainer.HasBeenMarkedChanged, Is.False); 
          });

      _persistenceStrategy.PersistData (new[] { persistableData }.AsOneTime());

      _parentTransactionContextMock.VerifyAllExpectations ();
      _unlockedParentTransactionContextMock.VerifyAllExpectations ();
    }

    [Test]
    public void PersistData_MarkedAsChangedDataContainer ()
    {
      var persistableData1 = CreateMarkAsChangedPersistableDataForOrder ();
      var persistableData2 = CreateMarkAsChangedPersistableDataForOrder ();
      var persistableData3 = CreateMarkAsChangedPersistableDataForOrder ();

      _parentTransactionContextMock.Expect (mock => mock.UnlockParentTransaction ()).Return (_unlockedParentTransactionContextMock);

      var unchangedParentDataContainer = DataContainerObjectMother.CreateExisting (persistableData1.DomainObject);
      _parentTransactionContextMock
          .Expect (mock => mock.GetDataContainerWithoutLoading (persistableData1.DomainObject.ID))
          .Return (unchangedParentDataContainer);

      var changedParentDataContainer = DataContainerObjectMother.CreateExisting (persistableData2.DomainObject);
      SetPropertyValue (changedParentDataContainer, typeof (Order), "OrderNumber", 23);
      _parentTransactionContextMock
          .Expect (mock => mock.GetDataContainerWithoutLoading (persistableData2.DomainObject.ID))
          .Return (changedParentDataContainer);

      var newParentDataContainer = DataContainerObjectMother.CreateNew (persistableData3.DomainObject);
      _parentTransactionContextMock
          .Expect (mock => mock.GetDataContainerWithoutLoading (persistableData3.DomainObject.ID))
          .Return (newParentDataContainer);

      _unlockedParentTransactionContextMock
          .Expect (mock => mock.Dispose ())
          .WhenCalled (mi =>
          {
            Assert.That (unchangedParentDataContainer.HasBeenMarkedChanged, Is.True);
            Assert.That (changedParentDataContainer.HasBeenMarkedChanged, Is.True);
            Assert.That (newParentDataContainer.HasBeenMarkedChanged, Is.False);
          });

      _persistenceStrategy.PersistData (new[] { persistableData1, persistableData2, persistableData3 }.AsOneTime());

      _parentTransactionContextMock.VerifyAllExpectations ();
      _unlockedParentTransactionContextMock.VerifyAllExpectations ();
    }

    [Test]
    public void PersistData_EndPoints ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();
      var persistedDataContainer = DataContainerObjectMother.CreateExisting (domainObject);
      var persistedEndPoint1 = RelationEndPointObjectMother.CreateStub ();
      persistedEndPoint1.Stub (stub => stub.HasChanged).Return (true);
      var persistedEndPoint2 = RelationEndPointObjectMother.CreateStub ();
      persistedEndPoint2.Stub (stub => stub.HasChanged).Return (false);
      var persistedEndPoint3 = RelationEndPointObjectMother.CreateStub ();
      persistedEndPoint3.Stub (stub => stub.HasChanged).Return (true);
      var persistableData = new PersistableData (
          domainObject, StateType.Changed, persistedDataContainer, new[] { persistedEndPoint1, persistedEndPoint2, persistedEndPoint3 });

      var counter = new OrderedExpectationCounter ();

      _parentTransactionContextMock.Expect (mock => mock.UnlockParentTransaction ()).Return (_unlockedParentTransactionContextMock);

      var parentEndPointMock1 = MockRepository.GenerateStrictMock<IRelationEndPoint> ();
      _parentTransactionContextMock
          .Expect (mock => mock.GetRelationEndPointWithoutLoading (persistedEndPoint1.ID))
          .Return (parentEndPointMock1);
      parentEndPointMock1
          .Expect (mock => mock.SetDataFromSubTransaction (persistedEndPoint1))
          .Ordered (counter, "SetDataFromSubTransaction must occur prior to Dispose.");

      var parentEndPointMock3 = MockRepository.GenerateStrictMock<IRelationEndPoint> ();
      _parentTransactionContextMock
          .Expect (mock => mock.GetRelationEndPointWithoutLoading (persistedEndPoint3.ID))
          .Return (parentEndPointMock3);
      parentEndPointMock3
          .Expect (mock => mock.SetDataFromSubTransaction (persistedEndPoint3))
          .Ordered (counter, "SetDataFromSubTransaction must occur prior to Dispose.");

      _unlockedParentTransactionContextMock.Expect (mock => mock.Dispose ()).Ordered (counter, "Dispose should come at the end.");

      _persistenceStrategy.PersistData (new[] { persistableData }.AsOneTime ());

      _parentTransactionContextMock.VerifyAllExpectations ();
      _unlockedParentTransactionContextMock.VerifyAllExpectations ();
      parentEndPointMock1.VerifyAllExpectations ();
      parentEndPointMock3.VerifyAllExpectations ();
    }

    [Test]
    public void PersistData_NewDataContainer_WithEndPoint ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();
      var persistedDataContainer = DataContainerObjectMother.CreateNew (domainObject);
      SetPropertyValue (persistedDataContainer, typeof (Order), "OrderNumber", 12);
      var persistedEndPoint = RelationEndPointObjectMother.CreateStub();
      persistedEndPoint.Stub (stub => stub.HasChanged).Return (true);
      var persistableData = new PersistableData (domainObject, StateType.New, persistedDataContainer, new[] { persistedEndPoint });

      var counter = new OrderedExpectationCounter ();

      _parentTransactionContextMock.Expect (mock => mock.UnlockParentTransaction ()).Return (_unlockedParentTransactionContextMock);

      _parentTransactionContextMock.Stub (stub => stub.IsInvalid (domainObject.ID)).Return (true);
      _parentTransactionContextMock.Stub (stub => stub.GetDataContainerWithoutLoading (domainObject.ID)).Return (null);
      _unlockedParentTransactionContextMock
          .Expect (mock => mock.MarkNotInvalid (domainObject.ID))
          .Ordered (counter);
      _unlockedParentTransactionContextMock
          .Expect (mock => mock.RegisterDataContainer (Arg<DataContainer>.Is.Anything))
          .WhenCalledOrdered (
              counter,
              mi => CheckDataContainer ((DataContainer) mi.Arguments[0], domainObject, null, StateType.New, _orderNumberPropertyDefinition, 12, 0, true),
              "New DataContainers must be registered before the parent relation end-points are retrieved for persistence (and prior to Dispose)."
          );

      var parentEndPointMock = MockRepository.GenerateStrictMock<IRelationEndPoint>();
      _parentTransactionContextMock
          .Expect (mock => mock.GetRelationEndPointWithoutLoading (persistedEndPoint.ID))
          .Return (parentEndPointMock)
          .Ordered (counter, "New DataContainers must be registered before the parent relation end-points are retrieved for persistence.");
      parentEndPointMock
          .Expect (mock => mock.SetDataFromSubTransaction (persistedEndPoint))
          .Ordered (counter, "SetDataFromSubTransaction must occur prior to Dispose.");

      _unlockedParentTransactionContextMock.Expect (mock => mock.Dispose ()).Ordered (counter, "Dispose should come at the end.");

      _persistenceStrategy.PersistData (new[] { persistableData }.AsOneTime());

      _parentTransactionContextMock.VerifyAllExpectations ();
      _unlockedParentTransactionContextMock.VerifyAllExpectations ();
      parentEndPointMock.VerifyAllExpectations();
    }

    [Test]
    public void PersistData_DeletedDataContainer_WithEndPoint_NewInParent ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();
      var persistedDataContainer = DataContainerObjectMother.CreateDeleted (domainObject);
      var persistedEndPoint = RelationEndPointObjectMother.CreateStub ();
      persistedEndPoint.Stub (stub => stub.HasChanged).Return (true);
      var persistableData = new PersistableData (domainObject, StateType.Deleted, persistedDataContainer, new[] { persistedEndPoint });

      var counter = new OrderedExpectationCounter ();

      _parentTransactionContextMock.Expect (mock => mock.UnlockParentTransaction ()).Return (_unlockedParentTransactionContextMock);

      var parentEndPointMock = MockRepository.GenerateStrictMock<IRelationEndPoint> ();
      _parentTransactionContextMock
          .Expect (mock => mock.GetRelationEndPointWithoutLoading (persistedEndPoint.ID))
          .Return (parentEndPointMock)
          .Ordered (counter, "Deleted DataContainers must be persisted after the parent relation end-points are retrieved for persistence.");
      parentEndPointMock
          .Expect (mock => mock.SetDataFromSubTransaction (persistedEndPoint))
          .Ordered (counter, "SetDataFromSubTransaction must occur prior to Dispose.");

      var parentDataContainer = DataContainerObjectMother.CreateNew (persistedDataContainer.ID);
      parentDataContainer.SetDomainObject (domainObject);
      _parentTransactionContextMock
          .Stub (stub => stub.GetDataContainerWithoutLoading (domainObject.ID))
          .Return (parentDataContainer);
      
      _unlockedParentTransactionContextMock
          .Expect (mock => mock.Discard (parentDataContainer))
          .Ordered (counter, "Deleted DataContainers must be persisted after the parent relation end-points are retrieved for persistence.");

      _unlockedParentTransactionContextMock
          .Expect (mock => mock.Dispose ())
          .Ordered (counter, "Dispose should come at the end.");

      _persistenceStrategy.PersistData (new[] { persistableData }.AsOneTime());

      _parentTransactionContextMock.VerifyAllExpectations ();
      _unlockedParentTransactionContextMock.VerifyAllExpectations ();
      parentEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void PersistData_DeletedDataContainer_WithEndPoint_ExistingInParent ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();
      var persistedDataContainer = DataContainerObjectMother.CreateDeleted (domainObject);
      var persistedEndPoint = RelationEndPointObjectMother.CreateStub ();
      persistedEndPoint.Stub (stub => stub.HasChanged).Return (true);
      var persistableData = new PersistableData (domainObject, StateType.Deleted, persistedDataContainer, new[] { persistedEndPoint });

      _parentTransactionContextMock.Expect (mock => mock.UnlockParentTransaction ()).Return (_unlockedParentTransactionContextMock);

      var counter = new OrderedExpectationCounter ();

      var parentEndPointMock = MockRepository.GenerateStrictMock<IRelationEndPoint> ();
      _parentTransactionContextMock
          .Expect (mock => mock.GetRelationEndPointWithoutLoading (persistedEndPoint.ID))
          .Return (parentEndPointMock);
      parentEndPointMock
          .Expect (mock => mock.SetDataFromSubTransaction (persistedEndPoint))
          .Ordered (counter, "SetDataFromSubTransaction must occur prior to Dispose.");

      var parentDataContainer = DataContainerObjectMother.CreateExisting (persistedDataContainer.ID);
      parentDataContainer.SetDomainObject (domainObject);
      _parentTransactionContextMock
          .Stub (stub => stub.GetDataContainerWithoutLoading (domainObject.ID))
          .Return (parentDataContainer);

      _unlockedParentTransactionContextMock
          .Expect (mock => mock.Dispose ())
          .WhenCalledOrdered (
              counter,
              mi => Assert.That (
                  parentDataContainer.State, 
                  Is.EqualTo (StateType.Deleted), 
                  "Parent DataContainer must be deleted before parent transaction is locked again."),
              "Dispose should come at the end.");

      _persistenceStrategy.PersistData (new[] { persistableData }.AsOneTime());

      _parentTransactionContextMock.VerifyAllExpectations ();
      _unlockedParentTransactionContextMock.VerifyAllExpectations ();
      parentEndPointMock.VerifyAllExpectations ();
    }

    private void CheckDataContainer (
        DataContainer dataContainer,
        DomainObject expectedDomainObject,
        object expectedTimestamp,
        StateType expectedState,
        PropertyDefinition propertyDefinition,
        object expectedCurrentPropertyValue,
        object expectedOriginalPropertyValue, 
      bool expectedHasPropertyValueBeenTouched)
    {
      Assert.That (dataContainer.HasDomainObject, Is.True);
      Assert.That (dataContainer.DomainObject, Is.SameAs (expectedDomainObject));

      CheckDataContainer (
          dataContainer,
          expectedDomainObject.ID,
          expectedTimestamp,
          expectedState,
          propertyDefinition,
          expectedCurrentPropertyValue,
          expectedOriginalPropertyValue,
          expectedHasPropertyValueBeenTouched);
    }

    private void CheckDataContainer (
        DataContainer dataContainer,
        ObjectID expectedID,
        object expectedTimestamp,
        StateType expectedState,
        PropertyDefinition propertyDefinition,
        object expectedCurrentPropertyValue,
        object expectedOriginalPropertyValue,
        bool expectedHasPropertyValueBeenTouched)
    {
      Assert.That (dataContainer.ID, Is.EqualTo (expectedID));
      Assert.That (dataContainer.Timestamp, Is.EqualTo (expectedTimestamp));
      Assert.That (dataContainer.State, Is.EqualTo (expectedState));
      Assert.That (dataContainer.GetValue (propertyDefinition, ValueAccess.Current), Is.EqualTo (expectedCurrentPropertyValue));
      Assert.That (dataContainer.GetValue (propertyDefinition, ValueAccess.Original), Is.EqualTo (expectedOriginalPropertyValue));
      Assert.That (dataContainer.HasValueBeenTouched (propertyDefinition), Is.EqualTo (expectedHasPropertyValueBeenTouched));
    }

    private DataContainer CreateChangedDataContainer (
        ObjectID objectID, int timestamp, PropertyDefinition propertyDefinition, object currentPropertyValue)
    {
      var parentDataContainer = DataContainerObjectMother.CreateExisting (objectID);
      parentDataContainer.SetTimestamp (timestamp);
      parentDataContainer.SetValue (propertyDefinition, currentPropertyValue);
      return parentDataContainer;
    }

    private PersistableData CreatePersistableDataForExistingOrder ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();
      var dataContainer = DataContainerObjectMother.CreateExisting (domainObject.ID);
      dataContainer.SetDomainObject (domainObject);
      return new PersistableData (domainObject, StateType.Changed, dataContainer, new IRelationEndPoint[0]);
    }

    private PersistableData CreateMarkAsChangedPersistableDataForOrder ()
    {
      var persistableData = CreatePersistableDataForExistingOrder ();
      persistableData.DataContainer.MarkAsChanged ();
      return persistableData;
    }
  }
}