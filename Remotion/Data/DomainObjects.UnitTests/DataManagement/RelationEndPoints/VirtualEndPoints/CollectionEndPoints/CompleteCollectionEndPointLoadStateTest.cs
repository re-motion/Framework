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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.Serialization;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  [TestFixture]
  public class CompleteCollectionEndPointLoadStateTest : StandardMappingTest
  {
    private ICollectionEndPoint _collectionEndPointMock;
    private ICollectionEndPointDataManager _dataManagerMock;
    private IRelationEndPointProvider _endPointProviderStub;
    private IClientTransactionEventSink _transactionEventSinkStub;
    private IDomainObjectCollectionEventRaiser _eventRaiserMock;
    
    private CompleteCollectionEndPointLoadState _loadState;

    private IRelationEndPointDefinition _definition;
    private Order _relatedObject;
    private IRealObjectEndPoint _relatedEndPointStub;
    private Customer _owningObject;
    private ICollectionEndPointCollectionManager _collectionManagerStub;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _definition = Configuration.GetTypeDefinition (typeof (Customer)).GetRelationEndPointDefinition (typeof (Customer).FullName + ".Orders");

      _collectionEndPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint>();
      _dataManagerMock = MockRepository.GenerateStrictMock<ICollectionEndPointDataManager>();
      _dataManagerMock.Stub (stub => stub.EndPointID).Return (RelationEndPointID.Create (DomainObjectIDs.Customer1, _definition));
      _endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider> ();
      _transactionEventSinkStub = MockRepository.GenerateStub<IClientTransactionEventSink> ();
      _eventRaiserMock = MockRepository.GenerateStrictMock<IDomainObjectCollectionEventRaiser>();

      _loadState = new CompleteCollectionEndPointLoadState (_dataManagerMock, _endPointProviderStub, _transactionEventSinkStub);

      _relatedObject = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);
      _relatedEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _relatedEndPointStub.Stub (stub => stub.GetDomainObjectReference ()).Return (_relatedObject);
      _relatedEndPointStub.Stub (stub => stub.ObjectID).Return (_relatedObject.ID);
      _owningObject = DomainObjectMother.CreateFakeObject<Customer>();
      _collectionManagerStub = MockRepository.GenerateStub<ICollectionEndPointCollectionManager> ();
    }

    [Test]
    public void HasChangedFast ()
    {
      _dataManagerMock.Stub (stub => stub.HasDataChangedFast ()).Return (true).Repeat.Once ();
      _dataManagerMock.Stub (stub => stub.HasDataChangedFast ()).Return (false).Repeat.Once ();
      _dataManagerMock.Stub (stub => stub.HasDataChangedFast ()).Return (null).Repeat.Once ();

      Assert.That (_loadState.HasChangedFast (), Is.True);
      Assert.That (_loadState.HasChangedFast (), Is.False);
      Assert.That (_loadState.HasChangedFast (), Is.Null);
    }

    [Test]
    public void GetData ()
    {
      var collectionDataStub = MockRepository.GenerateStub<IDomainObjectCollectionData> ();
      _dataManagerMock.Stub (stub => stub.CollectionData).Return (collectionDataStub);

      var result = _loadState.GetData (_collectionEndPointMock);

      Assert.That (result, Is.TypeOf (typeof (ReadOnlyCollectionDataDecorator)));
      var wrappedData = DomainObjectCollectionDataTestHelper.GetWrappedData (result);
      Assert.That (wrappedData, Is.SameAs (collectionDataStub));
    }

    [Test]
    public void GetCollectionData ()
    {
      var collectionDataStub = new ReadOnlyCollectionDataDecorator (MockRepository.GenerateStub<IDomainObjectCollectionData> ());
      _dataManagerMock.Stub (stub => stub.OriginalCollectionData).Return (collectionDataStub);

      var result = _loadState.GetOriginalData (_collectionEndPointMock);

      Assert.That (result, Is.SameAs (collectionDataStub));
    }

    [Test]
    public void SetDataFromSubTransaction ()
    {
      var counter = new OrderedExpectationCounter ();

      _collectionEndPointMock.Stub (stub => stub.GetCollectionEventRaiser ()).Return (_eventRaiserMock);

      var sourceDataManager = MockRepository.GenerateStub<ICollectionEndPointDataManager> ();
      var sourceLoadState = new CompleteCollectionEndPointLoadState (sourceDataManager, _endPointProviderStub, _transactionEventSinkStub);
      _dataManagerMock.Expect (mock => mock.SetDataFromSubTransaction (sourceDataManager, _endPointProviderStub)).Ordered (counter);
      _dataManagerMock.Replay ();

      _eventRaiserMock.Expect (mock => mock.WithinReplaceData ()).Ordered (counter);
      _eventRaiserMock.Replay ();

      _loadState.SetDataFromSubTransaction (_collectionEndPointMock, sourceLoadState);

      _dataManagerMock.VerifyAllExpectations();
      _eventRaiserMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The data is already complete.")]
    public void MarkDataComplete_ThrowsException ()
    {
      var items = new DomainObject[] { _relatedObject };
      _loadState.MarkDataComplete (_collectionEndPointMock, items, dataManager => Assert.Fail ("Must not be called"));
    }

    [Test]
    public void SortCurrentData_RaisesEvent ()
    {
      var counter = new OrderedExpectationCounter ();

      _collectionEndPointMock.Stub (stub => stub.GetCollectionEventRaiser ()).Return (_eventRaiserMock);

      Comparison<DomainObject> comparison = (one, two) => 0;
      _dataManagerMock.Expect (mock => mock.SortCurrentData (comparison)).Ordered (counter);
      _dataManagerMock.Replay ();

      _eventRaiserMock.Expect (mock => mock.WithinReplaceData ()).Ordered (counter);
      _eventRaiserMock.Replay ();

      _loadState.SortCurrentData (_collectionEndPointMock, comparison);

      _dataManagerMock.VerifyAllExpectations ();
      _eventRaiserMock.VerifyAllExpectations ();
    }

    [Test]
    public void Rollback_RaisesEvent ()
    {
      var counter = new OrderedExpectationCounter ();

      _collectionEndPointMock.Stub (stub => stub.GetCollectionEventRaiser ()).Return (_eventRaiserMock);

      _dataManagerMock.Expect (mock => mock.Rollback ()).Ordered (counter);
      _dataManagerMock.Replay ();

      _eventRaiserMock.Expect (mock => mock.WithinReplaceData()).Ordered (counter);
      _eventRaiserMock.Replay ();

      _loadState.Rollback (_collectionEndPointMock);

      _dataManagerMock.VerifyAllExpectations ();
      _eventRaiserMock.VerifyAllExpectations ();
    }

    [Test]
    public void Synchronize ()
    {
      var counter = new OrderedExpectationCounter ();

      _collectionEndPointMock.Stub (stub => stub.GetCollectionEventRaiser ()).Return (_eventRaiserMock);

      _dataManagerMock.Stub (stub => stub.OriginalItemsWithoutEndPoints).Return (new[] { _relatedObject });

      _dataManagerMock.Expect (mock => mock.UnregisterOriginalItemWithoutEndPoint (_relatedObject)).Ordered (counter);
      _dataManagerMock.Replay ();

      _eventRaiserMock.Expect (mock => mock.WithinReplaceData ()).Ordered (counter);
      _eventRaiserMock.Replay();

      _loadState.Synchronize (_collectionEndPointMock);

      _dataManagerMock.VerifyAllExpectations ();
      _eventRaiserMock.VerifyAllExpectations();
    }

    [Test]
    public void SynchronizeOppositeEndPoint ()
    {
      var counter = new OrderedExpectationCounter ();

      _collectionEndPointMock.Stub (stub => stub.GetCollectionEventRaiser ()).Return (_eventRaiserMock);

      _dataManagerMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (_relatedEndPointStub)).Ordered (counter);
      _dataManagerMock.Replay ();

      _eventRaiserMock.Expect (mock => mock.WithinReplaceData ()).Ordered (counter);
      _eventRaiserMock.Replay ();

      AddUnsynchronizedOppositeEndPoint (_loadState, _relatedEndPointStub);

      _loadState.SynchronizeOppositeEndPoint (_collectionEndPointMock, _relatedEndPointStub);

      _dataManagerMock.VerifyAllExpectations ();
      _eventRaiserMock.VerifyAllExpectations ();
    }

    [Test]
    public void CreateSetCollectionCommand ()
    {
      var fakeCollectionData = new DomainObjectCollectionData ();
      _dataManagerMock.Stub (stub => stub.CollectionData).Return (fakeCollectionData);
      _dataManagerMock.Stub (stub => stub.OriginalItemsWithoutEndPoints).Return (new DomainObject[0]);

      var fakeCollection = new DomainObjectCollection ();
      _collectionEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _collectionEndPointMock.Stub (mock => mock.Collection).Return (fakeCollection);
      _collectionEndPointMock.Stub (mock => mock.GetDomainObject ()).Return (_owningObject);
      _collectionEndPointMock.Replay ();

      var newCollection = new OrderCollection ();

      var command = (RelationEndPointModificationCommand) _loadState.CreateSetCollectionCommand (_collectionEndPointMock, newCollection, _collectionManagerStub);

      Assert.That (command, Is.TypeOf (typeof (CollectionEndPointSetCollectionCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_collectionEndPointMock));
      Assert.That (command.TransactionEventSink, Is.SameAs (_transactionEventSinkStub));
      Assert.That (((CollectionEndPointSetCollectionCommand) command).NewCollection, Is.SameAs (newCollection));
      Assert.That (((CollectionEndPointSetCollectionCommand) command).CollectionEndPointCollectionManager, Is.SameAs (_collectionManagerStub));
      Assert.That (((CollectionEndPointSetCollectionCommand) command).ModifiedCollectionData, Is.SameAs (fakeCollectionData));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The collection of relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of domain object "
        + "'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' cannot be replaced because the opposite object property "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' of domain object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' is out of sync. To make this change, synchronize the two properties by calling the "
        + "'BidirectionalRelationSyncService.Synchronize' method on the 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' property.")]
    public void CreateSetCollectionCommand_WithUnsyncedOpposites ()
    {
      AddUnsynchronizedOppositeEndPoint (_loadState, _relatedEndPointStub);

      var newCollection = new OrderCollection ();

      _loadState.CreateSetCollectionCommand (_collectionEndPointMock, newCollection, _collectionManagerStub);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The collection of relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of domain object "
        + "'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' cannot be replaced because the relation property is out of sync with the "
        + "opposite object property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' of domain object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'. To make this change, synchronize the two properties by calling the "
        + "'BidirectionalRelationSyncService.Synchronize' method on the 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' property.")]
    public void CreateSetCollectionCommand_WithItemsWithoutEndPoints ()
    {
      _dataManagerMock.Stub (stub => stub.OriginalItemsWithoutEndPoints).Return (new[] { _relatedObject });

      var newCollection = new OrderCollection ();

      _loadState.CreateSetCollectionCommand (_collectionEndPointMock, newCollection, _collectionManagerStub);
    }

    [Test]
    public void CreateRemoveCommand ()
    {
      var fakeCollectionData = new DomainObjectCollectionData ();
      _dataManagerMock.Stub (stub => stub.CollectionData).Return (fakeCollectionData);
      _dataManagerMock.Stub (stub => stub.ContainsOriginalItemWithoutEndPoint (_relatedObject)).Return (false);

      var fakeCollection = new DomainObjectCollection ();
      _collectionEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _collectionEndPointMock.Stub (mock => mock.Collection).Return (fakeCollection);
      _collectionEndPointMock.Stub (mock => mock.GetDomainObject ()).Return (_owningObject);

      var command = (RelationEndPointModificationCommand) _loadState.CreateRemoveCommand (_collectionEndPointMock, _relatedObject);
      Assert.That (command, Is.InstanceOf (typeof (CollectionEndPointRemoveCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_collectionEndPointMock));
      Assert.That (command.DomainObject, Is.SameAs (_owningObject));
      Assert.That (command.OldRelatedObject, Is.SameAs (_relatedObject));
      Assert.That (command.TransactionEventSink, Is.SameAs (_transactionEventSinkStub));

      Assert.That (((CollectionEndPointRemoveCommand) command).ModifiedCollection, Is.SameAs (fakeCollection));
      Assert.That (((CollectionEndPointRemoveCommand) command).ModifiedCollectionData, Is.SameAs (fakeCollectionData));
      Assert.That (((CollectionEndPointRemoveCommand) command).EndPointProvider, Is.SameAs (_endPointProviderStub));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The domain object with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be replaced or removed from collection property "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of object 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' "
        + "because its object property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' is out of sync with the collection property. "
        + "To make this change, synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' property.")]
    public void CreateRemoveCommand_RemoveItemWithUnsyncedOpposite ()
    {
      AddUnsynchronizedOppositeEndPoint (_loadState, _relatedEndPointStub);

      _loadState.CreateRemoveCommand (_collectionEndPointMock, _relatedObject);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The domain object with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be replaced or removed from collection property "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of object 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' "
        + "because the property is out of sync with the opposite object property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer'. "
        + "To make this change, synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' property.")]
    public void CreateRemoveCommand_RemoveItemWithoutEndPoint ()
    {
      _dataManagerMock.Stub (stub => stub.ContainsOriginalItemWithoutEndPoint (_relatedObject)).Return (true);

      _loadState.CreateRemoveCommand (_collectionEndPointMock, _relatedObject);
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      var fakeCollectionData = new DomainObjectCollectionData ();
      _dataManagerMock.Stub (stub => stub.CollectionData).Return (fakeCollectionData);
      _dataManagerMock.Stub (stub => stub.OriginalItemsWithoutEndPoints).Return (new DomainObject[0]);

      var fakeCollection = new DomainObjectCollection ();
      _collectionEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _collectionEndPointMock.Stub (mock => mock.Collection).Return (fakeCollection);
      _collectionEndPointMock.Stub (mock => mock.GetDomainObject ()).Return (_owningObject);

      var command = (RelationEndPointModificationCommand) _loadState.CreateDeleteCommand (_collectionEndPointMock);
      Assert.That (command, Is.InstanceOf (typeof (CollectionEndPointDeleteCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_collectionEndPointMock));
      Assert.That (command.TransactionEventSink, Is.SameAs (_transactionEventSinkStub));

      Assert.That (((CollectionEndPointDeleteCommand) command).ModifiedCollectionData, Is.SameAs (fakeCollectionData));
      Assert.That (((CollectionEndPointDeleteCommand) command).ModifiedCollection, Is.SameAs (fakeCollection));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The domain object 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' cannot be deleted because the opposite object property "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' of domain object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' is out of sync with the collection property "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders'. To make this change, synchronize the two properties by calling the "
        + "'BidirectionalRelationSyncService.Synchronize' method on the 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' property.")]
    public void CreateDeleteCommand_WithUnsyncedOpposites ()
    {
      AddUnsynchronizedOppositeEndPoint (_loadState, _relatedEndPointStub);

      _loadState.CreateDeleteCommand (_collectionEndPointMock);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The domain object 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' cannot be deleted because its collection property "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' is out of sync with the opposite object property "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' of domain object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'. To make this change, synchronize the two properties by calling the "
        + "'BidirectionalRelationSyncService.Synchronize' method on the 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' property.")]
    public void CreateDeleteCommand_WithItemsWithoutEndPoints ()
    {
      _dataManagerMock.Stub (stub => stub.OriginalItemsWithoutEndPoints).Return (new[] { _relatedObject });

      _loadState.CreateDeleteCommand (_collectionEndPointMock);
    }

    [Test]
    public void CreateInsertCommand ()
    {
      var fakeCollectionData = new DomainObjectCollectionData ();
      _dataManagerMock.Stub (stub => stub.CollectionData).Return (fakeCollectionData);
      _dataManagerMock.Stub (stub => stub.ContainsOriginalItemWithoutEndPoint (_relatedObject)).Return (false);

      var fakeCollection = new DomainObjectCollection ();
      _collectionEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _collectionEndPointMock.Stub (mock => mock.Collection).Return (fakeCollection);
      _collectionEndPointMock.Stub (mock => mock.GetDomainObject ()).Return (_owningObject);

      var command = (RelationEndPointModificationCommand) _loadState.CreateInsertCommand (_collectionEndPointMock, _relatedObject, 12);

      Assert.That (command, Is.InstanceOf (typeof (CollectionEndPointInsertCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_collectionEndPointMock));
      Assert.That (command.NewRelatedObject, Is.SameAs (_relatedObject));
      Assert.That (command.TransactionEventSink, Is.SameAs (_transactionEventSinkStub));

      Assert.That (((CollectionEndPointInsertCommand) command).Index, Is.EqualTo (12));
      Assert.That (((CollectionEndPointInsertCommand) command).ModifiedCollectionData, Is.SameAs (fakeCollectionData));
      Assert.That (((CollectionEndPointInsertCommand) command).ModifiedCollection, Is.SameAs (fakeCollection));
      Assert.That (((CollectionEndPointInsertCommand) command).EndPointProvider, Is.SameAs (_endPointProviderStub));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The domain object with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be added to collection property "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of object 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' "
        + "because its object property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' is out of sync with the collection property. "
        + "To make this change, synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' property.")]
    public void CreateInsertCommand_ItemWithUnsyncedOpposite ()
    {
      AddUnsynchronizedOppositeEndPoint (_loadState, _relatedEndPointStub);

      _loadState.CreateInsertCommand (_collectionEndPointMock, _relatedObject, 0);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The domain object with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be added to collection property "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of object 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' "
        + "because the property is out of sync with the opposite object property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer'. "
        + "To make this change, synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' property.")]
    public void CreateInsertCommand_ItemWithoutEndPoint ()
    {
      _dataManagerMock.Stub (stub => stub.ContainsOriginalItemWithoutEndPoint (_relatedObject)).Return (true);

      _loadState.CreateInsertCommand (_collectionEndPointMock, _relatedObject, 0);
    }

    [Test]
    public void CreateAddCommand ()
    {
      var fakeCollectionData =
          new DomainObjectCollectionData (new[] { DomainObjectMother.CreateFakeObject<Order> (), DomainObjectMother.CreateFakeObject<Order> () });
      _dataManagerMock.Stub (stub => stub.CollectionData).Return (fakeCollectionData);
      _dataManagerMock.Stub (stub => stub.ContainsOriginalItemWithoutEndPoint (_relatedObject)).Return (false);

      var fakeCollection = new DomainObjectCollection ();
      _collectionEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _collectionEndPointMock.Stub (mock => mock.Collection).Return (fakeCollection);
      _collectionEndPointMock.Stub (mock => mock.GetDomainObject ()).Return (_owningObject);

      var command = (RelationEndPointModificationCommand) _loadState.CreateAddCommand (_collectionEndPointMock, _relatedObject);
      Assert.That (command, Is.InstanceOf (typeof (CollectionEndPointInsertCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_collectionEndPointMock));
      Assert.That (command.NewRelatedObject, Is.SameAs (_relatedObject));
      Assert.That (((CollectionEndPointInsertCommand) command).Index, Is.EqualTo (2));

      Assert.That (((CollectionEndPointInsertCommand) command).ModifiedCollectionData, Is.SameAs (fakeCollectionData));
      Assert.That (((CollectionEndPointInsertCommand) command).ModifiedCollection, Is.SameAs (fakeCollection));
      Assert.That (((CollectionEndPointInsertCommand) command).EndPointProvider, Is.SameAs (_endPointProviderStub));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The domain object with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be added to collection property "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of object 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' "
        + "because its object property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' is out of sync with the collection property. "
        + "To make this change, synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' property.")]
    public void CreateAddCommand_ItemWithUnsyncedOpposite ()
    {
      AddUnsynchronizedOppositeEndPoint (_loadState, _relatedEndPointStub);
      _dataManagerMock.Stub (stub => stub.CollectionData).Return (new DomainObjectCollectionData ());
      _loadState.CreateAddCommand (_collectionEndPointMock, _relatedObject);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The domain object with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be added to collection property "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of object 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' "
        + "because the property is out of sync with the opposite object property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer'. "
        + "To make this change, synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' property.")]
    public void CreateAddCommand_ItemWithoutEndPoint ()
    {
      _dataManagerMock.Stub (stub => stub.ContainsOriginalItemWithoutEndPoint (_relatedObject)).Return (true);

      _dataManagerMock.Stub (stub => stub.CollectionData).Return (new DomainObjectCollectionData ());
      _loadState.CreateAddCommand (_collectionEndPointMock, _relatedObject);
    }

    [Test]
    public void CreateReplaceCommand ()
    {
      var oldRelatedObject = DomainObjectMother.CreateFakeObject<Order> ();
      var fakeCollectionData = new DomainObjectCollectionData (new[] { oldRelatedObject });
      _dataManagerMock.Stub (stub => stub.CollectionData).Return (fakeCollectionData);
      _dataManagerMock.Stub (stub => stub.ContainsOriginalItemWithoutEndPoint (_relatedObject)).Return (false);
      _dataManagerMock.Stub (stub => stub.ContainsOriginalItemWithoutEndPoint (oldRelatedObject)).Return (false);

      var fakeCollection = new DomainObjectCollection ();
      _collectionEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _collectionEndPointMock.Stub (mock => mock.Collection).Return (fakeCollection);
      _collectionEndPointMock.Stub (mock => mock.GetDomainObject ()).Return (_owningObject);

      var command = (RelationEndPointModificationCommand) _loadState.CreateReplaceCommand (_collectionEndPointMock, 0, _relatedObject);
      Assert.That (command, Is.InstanceOf (typeof (CollectionEndPointReplaceCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_collectionEndPointMock));
      Assert.That (command.OldRelatedObject, Is.SameAs (oldRelatedObject));
      Assert.That (command.NewRelatedObject, Is.SameAs (_relatedObject));
      Assert.That (command.TransactionEventSink, Is.SameAs (_transactionEventSinkStub));

      Assert.That (((CollectionEndPointReplaceCommand) command).ModifiedCollectionData, Is.SameAs (fakeCollectionData));
      Assert.That (((CollectionEndPointReplaceCommand) command).ModifiedCollection, Is.SameAs (fakeCollection));
    }

    [Test]
    public void CreateReplaceCommand_SelfReplace ()
    {
      var fakeCollectionData = new DomainObjectCollectionData (new[] { _relatedObject });
      _dataManagerMock.Stub (stub => stub.CollectionData).Return (fakeCollectionData);
      _dataManagerMock.Stub (stub => stub.ContainsOriginalItemWithoutEndPoint (_relatedObject)).Return (false);

      var fakeCollection = new DomainObjectCollection ();
      _collectionEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _collectionEndPointMock.Stub (mock => mock.Collection).Return (fakeCollection);
      _collectionEndPointMock.Stub (mock => mock.GetDomainObject ()).Return (_owningObject);

      var command = (RelationEndPointModificationCommand) _loadState.CreateReplaceCommand (_collectionEndPointMock, 0, _relatedObject);
      Assert.That (command, Is.InstanceOf (typeof (CollectionEndPointReplaceSameCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_collectionEndPointMock));
      Assert.That (command.OldRelatedObject, Is.SameAs (_relatedObject));
      Assert.That (command.NewRelatedObject, Is.SameAs (_relatedObject));
      Assert.That (command.TransactionEventSink, Is.SameAs (_transactionEventSinkStub));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The domain object with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be replaced or removed from collection property "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of object 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' "
        + "because its object property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' is out of sync with the collection property. "
        + "To make this change, synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' property.")]
    public void CreateReplaceCommand_ItemWithUnsyncedOpposite ()
    {
      AddUnsynchronizedOppositeEndPoint (_loadState, _relatedEndPointStub);

      var newRelatedObject = DomainObjectMother.CreateFakeObject<Order> ();
      _dataManagerMock.Stub (stub => stub.ContainsOriginalItemWithoutEndPoint (newRelatedObject)).Return (false);
      _dataManagerMock
        .Stub (stub => stub.CollectionData)
        .Return (new DomainObjectCollectionData (new[] { _relatedObject }));

      _loadState.CreateReplaceCommand (_collectionEndPointMock, 0, newRelatedObject);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The domain object with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be replaced or removed from collection property "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of object 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' "
        + "because the property is out of sync with the opposite object property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer'. "
        + "To make this change, synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' property.")]
    public void CreateReplaceCommand_ItemWithoutEndPoint ()
    {
      var newRelatedObject = DomainObjectMother.CreateFakeObject<Order> ();
      _dataManagerMock.Stub (stub => stub.ContainsOriginalItemWithoutEndPoint (_relatedObject)).Return (true);
      _dataManagerMock.Stub (stub => stub.ContainsOriginalItemWithoutEndPoint (newRelatedObject)).Return (false);
      _dataManagerMock
        .Stub (stub => stub.CollectionData)
        .Return (new DomainObjectCollectionData (new[] { _relatedObject }));

      _loadState.CreateReplaceCommand (_collectionEndPointMock, 0, newRelatedObject);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The domain object with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be added to collection property "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of object 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' "
        + "because its object property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' is out of sync with the collection property. "
        + "To make this change, synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' property.")]
    public void CreateReplaceCommand_WithItemWithUnsyncedOpposite ()
    {
      AddUnsynchronizedOppositeEndPoint (_loadState, _relatedEndPointStub);

      _dataManagerMock
        .Stub (stub => stub.CollectionData)
        .Return (new DomainObjectCollectionData (new[] { DomainObjectMother.CreateFakeObject<Order> () }));
      _loadState.CreateReplaceCommand (_collectionEndPointMock, 0, _relatedObject);
    }
    
    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The domain object with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be added to collection property "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of object 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' "
        + "because the property is out of sync with the opposite object property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer'. "
        + "To make this change, synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' property.")]
    public void CreateReplaceCommand_WithItemWithoutEndPoint ()
    {
      _dataManagerMock.Stub (stub => stub.ContainsOriginalItemWithoutEndPoint (_relatedObject)).Return (true);

      _dataManagerMock
        .Stub (stub => stub.CollectionData)
        .Return (new DomainObjectCollectionData (new[] { DomainObjectMother.CreateFakeObject<Order> () }));
      _loadState.CreateReplaceCommand (_collectionEndPointMock, 0, _relatedObject);
    }

    [Test]
    public void GetOriginalOppositeEndPoints ()
    {
      _dataManagerMock.Stub (mock => mock.OriginalOppositeEndPoints).Return (new[] { _relatedEndPointStub });
      _dataManagerMock.Replay ();

      var result = (IEnumerable<IRealObjectEndPoint>) PrivateInvoke.InvokeNonPublicMethod (_loadState, "GetOriginalOppositeEndPoints");

      Assert.That (result, Is.EqualTo (new[] { _relatedEndPointStub }));
    }

    [Test]
    public void GetOriginalItemsWithoutEndPoints ()
    {
      _dataManagerMock.Stub (mock => mock.OriginalItemsWithoutEndPoints).Return (new[] { _relatedObject });
      _dataManagerMock.Replay ();

      var result = (IEnumerable<DomainObject>) PrivateInvoke.InvokeNonPublicMethod (_loadState, "GetOriginalItemsWithoutEndPoints");

      Assert.That (result, Is.EqualTo (new[] { _relatedObject }));
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var state = new CompleteCollectionEndPointLoadState (
          new SerializableCollectionEndPointDataManagerFake(),
          new SerializableRelationEndPointProviderFake(),
          new SerializableClientTransactionEventSinkFake());

      var oppositeEndPoint = new SerializableRealObjectEndPointFake (null, _relatedObject);
      AddUnsynchronizedOppositeEndPoint (state, oppositeEndPoint);

      var result = FlattenedSerializer.SerializeAndDeserialize (state);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.DataManager, Is.Not.Null);
      Assert.That (result.TransactionEventSink, Is.Not.Null);
      Assert.That (result.EndPointProvider, Is.Not.Null);
      Assert.That (result.UnsynchronizedOppositeEndPoints.Count, Is.EqualTo (1));
    }

    private void AddUnsynchronizedOppositeEndPoint (CompleteCollectionEndPointLoadState loadState, IRealObjectEndPoint oppositeEndPoint)
    {
      var dictionary = (Dictionary<ObjectID, IRealObjectEndPoint>) PrivateInvoke.GetNonPublicField (loadState, "_unsynchronizedOppositeEndPoints");
      dictionary.Add (oppositeEndPoint.ObjectID, oppositeEndPoint);
    }
  }
}