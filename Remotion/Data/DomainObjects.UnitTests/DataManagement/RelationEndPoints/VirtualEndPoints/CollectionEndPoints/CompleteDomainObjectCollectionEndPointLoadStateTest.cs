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
using Moq;
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
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  [TestFixture]
  public class CompleteDomainObjectCollectionEndPointLoadStateTest : StandardMappingTest
  {
    private Mock<IDomainObjectCollectionEndPoint> _collectionEndPointMock;
    private Mock<IDomainObjectCollectionEndPointDataManager> _dataManagerMock;
    private Mock<IRelationEndPointProvider> _endPointProviderStub;
    private Mock<IClientTransactionEventSink> _transactionEventSinkStub;
    private Mock<IDomainObjectCollectionEventRaiser> _eventRaiserMock;

    private CompleteDomainObjectCollectionEndPointLoadState _loadState;

    private IRelationEndPointDefinition _definition;
    private Order _relatedObject;
    private Mock<IRealObjectEndPoint> _relatedEndPointStub;
    private Customer _owningObject;
    private Mock<IDomainObjectCollectionEndPointCollectionManager> _collectionManagerStub;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _definition = Configuration.GetTypeDefinition(typeof(Customer)).GetRelationEndPointDefinition(typeof(Customer).FullName + ".Orders");

      _collectionEndPointMock = new Mock<IDomainObjectCollectionEndPoint>(MockBehavior.Strict);
      _dataManagerMock = new Mock<IDomainObjectCollectionEndPointDataManager>(MockBehavior.Strict);
      _dataManagerMock.Setup(stub => stub.EndPointID).Returns(RelationEndPointID.Create(DomainObjectIDs.Customer1, _definition));
      _endPointProviderStub = new Mock<IRelationEndPointProvider>();
      _transactionEventSinkStub = new Mock<IClientTransactionEventSink>();
      _eventRaiserMock = new Mock<IDomainObjectCollectionEventRaiser>(MockBehavior.Strict);

      _loadState = new CompleteDomainObjectCollectionEndPointLoadState(_dataManagerMock.Object, _endPointProviderStub.Object, _transactionEventSinkStub.Object);

      _relatedObject = DomainObjectMother.CreateFakeObject<Order>(DomainObjectIDs.Order1);
      _relatedEndPointStub = new Mock<IRealObjectEndPoint>();
      _relatedEndPointStub.Setup(stub => stub.GetDomainObjectReference()).Returns(_relatedObject);
      _relatedEndPointStub.Setup(stub => stub.ObjectID).Returns(_relatedObject.ID);
      _owningObject = DomainObjectMother.CreateFakeObject<Customer>();
      _collectionManagerStub = new Mock<IDomainObjectCollectionEndPointCollectionManager>();
    }

    [Test]
    public void HasChangedFast_WithHasDataChangedFastIsTrue_ReturnsTrue ()
    {
      _dataManagerMock.Setup(stub => stub.HasDataChangedFast()).Returns(true);

      Assert.That(_loadState.HasChangedFast(), Is.True);
    }

    [Test]
    public void HasChangedFast_WithHasDataChangedFastIsFalse_ReturnsFalse ()
    {
      _dataManagerMock.Setup(stub => stub.HasDataChangedFast()).Returns(false);

      Assert.That(_loadState.HasChangedFast(), Is.False);
    }

    [Test]
    public void HasChangedFast_WithHasDataChangedFastIsNull_ReturnsNull ()
    {
      _dataManagerMock.Setup(stub => stub.HasDataChangedFast()).Returns((bool?)null);

      Assert.That(_loadState.HasChangedFast(), Is.Null);
    }

    [Test]
    public void GetData ()
    {
      var collectionDataStub = new Mock<IDomainObjectCollectionData>();
      _dataManagerMock.Setup(stub => stub.CollectionData).Returns(collectionDataStub.Object);

      var result = _loadState.GetData(_collectionEndPointMock.Object);

      Assert.That(result, Is.TypeOf(typeof(ReadOnlyDomainObjectCollectionDataDecorator)));
      var wrappedData = DomainObjectCollectionDataTestHelper.GetWrappedData(result);
      Assert.That(wrappedData, Is.SameAs(collectionDataStub.Object));
    }

    [Test]
    public void GetCollectionData ()
    {
      var collectionDataStub = new ReadOnlyDomainObjectCollectionDataDecorator(new Mock<IDomainObjectCollectionData>().Object);
      _dataManagerMock.Setup(stub => stub.OriginalCollectionData).Returns(collectionDataStub);

      var result = _loadState.GetOriginalData(_collectionEndPointMock.Object);

      Assert.That(result, Is.SameAs(collectionDataStub));
    }

    [Test]
    public void SetDataFromSubTransaction ()
    {
      var sequence = new VerifiableSequence();

      _collectionEndPointMock.Setup(stub => stub.GetCollectionEventRaiser()).Returns(_eventRaiserMock.Object);

      var sourceDataManager = new Mock<IDomainObjectCollectionEndPointDataManager>();
      var sourceLoadState = new CompleteDomainObjectCollectionEndPointLoadState(sourceDataManager.Object, _endPointProviderStub.Object, _transactionEventSinkStub.Object);
      _dataManagerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.SetDataFromSubTransaction(sourceDataManager.Object, _endPointProviderStub.Object))
          .Verifiable();
      _eventRaiserMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.WithinReplaceData())
          .Verifiable();

      _loadState.SetDataFromSubTransaction(_collectionEndPointMock.Object, sourceLoadState);

      _dataManagerMock.Verify();
      _eventRaiserMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void MarkDataComplete_ThrowsException ()
    {
      var items = new DomainObject[] { _relatedObject };
      Assert.That(
          () => _loadState.MarkDataComplete(_collectionEndPointMock.Object, items, dataManager => Assert.Fail("Must not be called")),
          Throws.InvalidOperationException
              .With.Message.EqualTo("The data is already complete."));
    }

    [Test]
    public void SortCurrentData_RaisesEvent ()
    {
      var sequence = new VerifiableSequence();

      _collectionEndPointMock.Setup(stub => stub.GetCollectionEventRaiser()).Returns(_eventRaiserMock.Object);

      Comparison<IDomainObject> comparison = (one, two) => 0;
      _dataManagerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.SortCurrentData(comparison))
          .Verifiable();
      _eventRaiserMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.WithinReplaceData())
          .Verifiable();

      _loadState.SortCurrentData(_collectionEndPointMock.Object, comparison);

      _dataManagerMock.Verify();
      _eventRaiserMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Rollback_RaisesEvent ()
    {
      var sequence = new VerifiableSequence();

      _collectionEndPointMock.Setup(stub => stub.GetCollectionEventRaiser()).Returns(_eventRaiserMock.Object);
      _dataManagerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Rollback())
          .Verifiable();
      _eventRaiserMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.WithinReplaceData())
          .Verifiable();

      _loadState.Rollback(_collectionEndPointMock.Object);

      _dataManagerMock.Verify();
      _eventRaiserMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Synchronize ()
    {
      var sequence = new VerifiableSequence();

      _collectionEndPointMock.Setup(stub => stub.GetCollectionEventRaiser()).Returns(_eventRaiserMock.Object);

      _dataManagerMock.Setup(stub => stub.OriginalItemsWithoutEndPoints).Returns(new[] { _relatedObject });
      _dataManagerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.UnregisterOriginalItemWithoutEndPoint(_relatedObject))
          .Verifiable();
      _eventRaiserMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.WithinReplaceData())
          .Verifiable();

      _loadState.Synchronize(_collectionEndPointMock.Object);

      _dataManagerMock.Verify();
      _eventRaiserMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void SynchronizeOppositeEndPoint ()
    {
      var sequence = new VerifiableSequence();

      _collectionEndPointMock.Setup(stub => stub.GetCollectionEventRaiser()).Returns(_eventRaiserMock.Object);
      _dataManagerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RegisterOriginalOppositeEndPoint(_relatedEndPointStub.Object))
          .Verifiable();
      _eventRaiserMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.WithinReplaceData())
          .Verifiable();

      AddUnsynchronizedOppositeEndPoint(_loadState, _relatedEndPointStub.Object);

      _loadState.SynchronizeOppositeEndPoint(_collectionEndPointMock.Object, _relatedEndPointStub.Object);

      _dataManagerMock.Verify();
      _eventRaiserMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void CreateSetCollectionCommand ()
    {
      var fakeCollectionData = new DomainObjectCollectionData();
      _dataManagerMock.Setup(stub => stub.CollectionData).Returns(fakeCollectionData);
      _dataManagerMock.Setup(stub => stub.OriginalItemsWithoutEndPoints).Returns(new DomainObject[0]);

      var fakeCollection = new DomainObjectCollection();
      _collectionEndPointMock.Setup(mock => mock.IsNull).Returns(false);
      _collectionEndPointMock.Setup(mock => mock.Collection).Returns(fakeCollection);
      _collectionEndPointMock.Setup(mock => mock.GetDomainObject()).Returns(_owningObject);

      var newCollection = new OrderCollection();

      var command = (RelationEndPointModificationCommand)_loadState.CreateSetCollectionCommand(_collectionEndPointMock.Object, newCollection, _collectionManagerStub.Object);

      Assert.That(command, Is.TypeOf(typeof(DomainObjectCollectionEndPointSetCollectionCommand)));
      Assert.That(command.ModifiedEndPoint, Is.SameAs(_collectionEndPointMock.Object));
      Assert.That(command.TransactionEventSink, Is.SameAs(_transactionEventSinkStub.Object));
      Assert.That(((DomainObjectCollectionEndPointSetCollectionCommand)command).NewCollection, Is.SameAs(newCollection));
      Assert.That(((DomainObjectCollectionEndPointSetCollectionCommand)command).CollectionEndPointCollectionManager, Is.SameAs(_collectionManagerStub.Object));
      Assert.That(((DomainObjectCollectionEndPointSetCollectionCommand)command).ModifiedCollectionData, Is.SameAs(fakeCollectionData));
    }

    [Test]
    public void CreateSetCollectionCommand_WithUnsyncedOpposites ()
    {
      AddUnsynchronizedOppositeEndPoint(_loadState, _relatedEndPointStub.Object);

      var newCollection = new OrderCollection();
      Assert.That(
          () => _loadState.CreateSetCollectionCommand(_collectionEndPointMock.Object, newCollection, _collectionManagerStub.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The collection of relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of domain object "
                  + "'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' cannot be replaced because the opposite object property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' of domain object "
                  + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' is out of sync. To make this change, synchronize the two properties by calling the "
                  + "'BidirectionalRelationSyncService.Synchronize' method on the 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' property."));
    }

    [Test]
    public void CreateSetCollectionCommand_WithItemsWithoutEndPoints ()
    {
      _dataManagerMock.Setup(stub => stub.OriginalItemsWithoutEndPoints).Returns(new[] { _relatedObject });

      var newCollection = new OrderCollection();
      Assert.That(
          () => _loadState.CreateSetCollectionCommand(_collectionEndPointMock.Object, newCollection, _collectionManagerStub.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The collection of relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of domain object "
                  + "'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' cannot be replaced because the relation property is out of sync with the "
                  + "opposite object property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' of domain object "
                  + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'. To make this change, synchronize the two properties by calling the "
                  + "'BidirectionalRelationSyncService.Synchronize' method on the 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' property."));
    }

    [Test]
    public void CreateRemoveCommand ()
    {
      var fakeCollectionData = new DomainObjectCollectionData();
      _dataManagerMock.Setup(stub => stub.CollectionData).Returns(fakeCollectionData);
      _dataManagerMock.Setup(stub => stub.ContainsOriginalItemWithoutEndPoint(_relatedObject)).Returns(false);

      var fakeEventRaiser = new Mock<IDomainObjectCollectionEventRaiser>();
      _collectionEndPointMock.Setup(mock => mock.IsNull).Returns(false);
      _collectionEndPointMock.Setup(mock => mock.GetCollectionEventRaiser()).Returns(fakeEventRaiser.Object);
      _collectionEndPointMock.Setup(mock => mock.GetDomainObject()).Returns(_owningObject);
      _collectionEndPointMock.Setup(mock => mock.GetData()).Returns(new ReadOnlyDomainObjectCollectionDataDecorator(fakeCollectionData));

      var command = (RelationEndPointModificationCommand)_loadState.CreateRemoveCommand(_collectionEndPointMock.Object, _relatedObject);
      Assert.That(command, Is.InstanceOf(typeof(DomainObjectCollectionEndPointRemoveCommand)));
      Assert.That(command.ModifiedEndPoint, Is.SameAs(_collectionEndPointMock.Object));
      Assert.That(command.DomainObject, Is.SameAs(_owningObject));
      Assert.That(command.OldRelatedObject, Is.SameAs(_relatedObject));
      Assert.That(command.TransactionEventSink, Is.SameAs(_transactionEventSinkStub.Object));

      Assert.That(((DomainObjectCollectionEndPointRemoveCommand)command).ModifiedCollectionEventRaiser, Is.SameAs(fakeEventRaiser.Object));
      Assert.That(((DomainObjectCollectionEndPointRemoveCommand)command).ModifiedCollectionData, Is.SameAs(fakeCollectionData));
      Assert.That(((DomainObjectCollectionEndPointRemoveCommand)command).EndPointProvider, Is.SameAs(_endPointProviderStub.Object));
    }

    [Test]
    public void CreateRemoveCommand_RemoveItemWithUnsyncedOpposite ()
    {
      AddUnsynchronizedOppositeEndPoint(_loadState, _relatedEndPointStub.Object);
      Assert.That(
          () => _loadState.CreateRemoveCommand(_collectionEndPointMock.Object, _relatedObject),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The domain object with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be replaced or removed from collection property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of object 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' "
                  + "because its object property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' is out of sync with the collection property. "
                  + "To make this change, synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' property."));
    }

    [Test]
    public void CreateRemoveCommand_RemoveItemWithoutEndPoint ()
    {
      _dataManagerMock.Setup(stub => stub.ContainsOriginalItemWithoutEndPoint(_relatedObject)).Returns(true);
      Assert.That(
          () => _loadState.CreateRemoveCommand(_collectionEndPointMock.Object, _relatedObject),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The domain object with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be replaced or removed from collection property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of object 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' "
                  + "because the property is out of sync with the opposite object property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer'. "
                  + "To make this change, synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' property."));
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      var fakeCollectionData = new DomainObjectCollectionData();
      _dataManagerMock.Setup(stub => stub.CollectionData).Returns(fakeCollectionData);
      _dataManagerMock.Setup(stub => stub.OriginalItemsWithoutEndPoints).Returns(new DomainObject[0]);

      var fakeEventRaiser = new Mock<IDomainObjectCollectionEventRaiser>();
      _collectionEndPointMock.Setup(mock => mock.IsNull).Returns(false);
      _collectionEndPointMock.Setup(mock => mock.GetCollectionEventRaiser()).Returns(fakeEventRaiser.Object);
      _collectionEndPointMock.Setup(mock => mock.GetDomainObject()).Returns(_owningObject);

      var command = (RelationEndPointModificationCommand)_loadState.CreateDeleteCommand(_collectionEndPointMock.Object);
      Assert.That(command, Is.InstanceOf(typeof(DomainObjectCollectionEndPointDeleteCommand)));
      Assert.That(command.ModifiedEndPoint, Is.SameAs(_collectionEndPointMock.Object));
      Assert.That(command.TransactionEventSink, Is.SameAs(_transactionEventSinkStub.Object));

      Assert.That(((DomainObjectCollectionEndPointDeleteCommand)command).ModifiedCollectionData, Is.SameAs(fakeCollectionData));
      Assert.That(((DomainObjectCollectionEndPointDeleteCommand)command).ModifiedCollectionEventRaiser, Is.SameAs(fakeEventRaiser.Object));
    }

    [Test]
    public void CreateDeleteCommand_WithUnsyncedOpposites ()
    {
      AddUnsynchronizedOppositeEndPoint(_loadState, _relatedEndPointStub.Object);
      Assert.That(
          () => _loadState.CreateDeleteCommand(_collectionEndPointMock.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The domain object 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' cannot be deleted because the opposite object property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' of domain object "
                  + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' is out of sync with the collection property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders'. To make this change, synchronize the two properties by calling the "
                  + "'BidirectionalRelationSyncService.Synchronize' method on the 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' property."));
    }

    [Test]
    public void CreateDeleteCommand_WithItemsWithoutEndPoints ()
    {
      _dataManagerMock.Setup(stub => stub.OriginalItemsWithoutEndPoints).Returns(new[] { _relatedObject });
      Assert.That(
          () => _loadState.CreateDeleteCommand(_collectionEndPointMock.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The domain object 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' cannot be deleted because its collection property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' is out of sync with the opposite object property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' of domain object "
                  + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'. To make this change, synchronize the two properties by calling the "
                  + "'BidirectionalRelationSyncService.Synchronize' method on the 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' property."));
    }

    [Test]
    public void CreateInsertCommand ()
    {
      var fakeCollectionData = new DomainObjectCollectionData();
      _dataManagerMock.Setup(stub => stub.CollectionData).Returns(fakeCollectionData);
      _dataManagerMock.Setup(stub => stub.ContainsOriginalItemWithoutEndPoint(_relatedObject)).Returns(false);

      var fakeEventRaiser = new Mock<IDomainObjectCollectionEventRaiser>();
      _collectionEndPointMock.Setup(mock => mock.IsNull).Returns(false);
      _collectionEndPointMock.Setup(mock => mock.GetCollectionEventRaiser()).Returns(fakeEventRaiser.Object);
      _collectionEndPointMock.Setup(mock => mock.GetDomainObject()).Returns(_owningObject);

      var command = (RelationEndPointModificationCommand)_loadState.CreateInsertCommand(_collectionEndPointMock.Object, _relatedObject, 12);

      Assert.That(command, Is.InstanceOf(typeof(DomainObjectCollectionEndPointInsertCommand)));
      Assert.That(command.ModifiedEndPoint, Is.SameAs(_collectionEndPointMock.Object));
      Assert.That(command.NewRelatedObject, Is.SameAs(_relatedObject));
      Assert.That(command.TransactionEventSink, Is.SameAs(_transactionEventSinkStub.Object));

      Assert.That(((DomainObjectCollectionEndPointInsertCommand)command).Index, Is.EqualTo(12));
      Assert.That(((DomainObjectCollectionEndPointInsertCommand)command).ModifiedCollectionData, Is.SameAs(fakeCollectionData));
      Assert.That(((DomainObjectCollectionEndPointInsertCommand)command).ModifiedCollectionEventRaiser, Is.SameAs(fakeEventRaiser.Object));
      Assert.That(((DomainObjectCollectionEndPointInsertCommand)command).EndPointProvider, Is.SameAs(_endPointProviderStub.Object));
    }

    [Test]
    public void CreateInsertCommand_ItemWithUnsyncedOpposite ()
    {
      AddUnsynchronizedOppositeEndPoint(_loadState, _relatedEndPointStub.Object);
      Assert.That(
          () => _loadState.CreateInsertCommand(_collectionEndPointMock.Object, _relatedObject, 0),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The domain object with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be added to collection property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of object 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' "
                  + "because its object property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' is out of sync with the collection property. "
                  + "To make this change, synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' property."));
    }

    [Test]
    public void CreateInsertCommand_ItemWithoutEndPoint ()
    {
      _dataManagerMock.Setup(stub => stub.ContainsOriginalItemWithoutEndPoint(_relatedObject)).Returns(true);
      Assert.That(
          () => _loadState.CreateInsertCommand(_collectionEndPointMock.Object, _relatedObject, 0),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The domain object with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be added to collection property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of object 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' "
                  + "because the property is out of sync with the opposite object property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer'. "
                  + "To make this change, synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' property."));
    }

    [Test]
    public void CreateAddCommand ()
    {
      var fakeCollectionData =
          new DomainObjectCollectionData(new[] { DomainObjectMother.CreateFakeObject<Order>(), DomainObjectMother.CreateFakeObject<Order>() });
      _dataManagerMock.Setup(stub => stub.CollectionData).Returns(fakeCollectionData);
      _dataManagerMock.Setup(stub => stub.ContainsOriginalItemWithoutEndPoint(_relatedObject)).Returns(false);

      var fakeEventRaiser = new Mock<IDomainObjectCollectionEventRaiser>();
      _collectionEndPointMock.Setup(mock => mock.IsNull).Returns(false);
      _collectionEndPointMock.Setup(mock => mock.GetCollectionEventRaiser()).Returns(fakeEventRaiser.Object);
      _collectionEndPointMock.Setup(mock => mock.GetDomainObject()).Returns(_owningObject);

      var command = (RelationEndPointModificationCommand)_loadState.CreateAddCommand(_collectionEndPointMock.Object, _relatedObject);
      Assert.That(command, Is.InstanceOf(typeof(DomainObjectCollectionEndPointInsertCommand)));
      Assert.That(command.ModifiedEndPoint, Is.SameAs(_collectionEndPointMock.Object));
      Assert.That(command.NewRelatedObject, Is.SameAs(_relatedObject));
      Assert.That(((DomainObjectCollectionEndPointInsertCommand)command).Index, Is.EqualTo(2));

      Assert.That(((DomainObjectCollectionEndPointInsertCommand)command).ModifiedCollectionData, Is.SameAs(fakeCollectionData));
      Assert.That(((DomainObjectCollectionEndPointInsertCommand)command).ModifiedCollectionEventRaiser, Is.SameAs(fakeEventRaiser.Object));
      Assert.That(((DomainObjectCollectionEndPointInsertCommand)command).EndPointProvider, Is.SameAs(_endPointProviderStub.Object));
    }

    [Test]
    public void CreateAddCommand_ItemWithUnsyncedOpposite ()
    {
      AddUnsynchronizedOppositeEndPoint(_loadState, _relatedEndPointStub.Object);
      _dataManagerMock.Setup(stub => stub.CollectionData).Returns(new DomainObjectCollectionData());
      Assert.That(
          () => _loadState.CreateAddCommand(_collectionEndPointMock.Object, _relatedObject),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The domain object with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be added to collection property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of object 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' "
                  + "because its object property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' is out of sync with the collection property. "
                  + "To make this change, synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' property."));
    }

    [Test]
    public void CreateAddCommand_ItemWithoutEndPoint ()
    {
      _dataManagerMock.Setup(stub => stub.ContainsOriginalItemWithoutEndPoint(_relatedObject)).Returns(true);

      _dataManagerMock.Setup(stub => stub.CollectionData).Returns(new DomainObjectCollectionData());
      Assert.That(
          () => _loadState.CreateAddCommand(_collectionEndPointMock.Object, _relatedObject),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The domain object with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be added to collection property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of object 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' "
                  + "because the property is out of sync with the opposite object property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer'. "
                  + "To make this change, synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' property."));
    }

    [Test]
    public void CreateReplaceCommand ()
    {
      var oldRelatedObject = DomainObjectMother.CreateFakeObject<Order>();
      var fakeCollectionData = new DomainObjectCollectionData(new[] { oldRelatedObject });
      _dataManagerMock.Setup(stub => stub.CollectionData).Returns(fakeCollectionData);
      _dataManagerMock.Setup(stub => stub.ContainsOriginalItemWithoutEndPoint(_relatedObject)).Returns(false);
      _dataManagerMock.Setup(stub => stub.ContainsOriginalItemWithoutEndPoint(oldRelatedObject)).Returns(false);

      var fakeEventRaiser = new Mock<IDomainObjectCollectionEventRaiser>();
      _collectionEndPointMock.Setup(mock => mock.IsNull).Returns(false);
      _collectionEndPointMock.Setup(mock => mock.GetCollectionEventRaiser()).Returns(fakeEventRaiser.Object);
      _collectionEndPointMock.Setup(mock => mock.GetDomainObject()).Returns(_owningObject);

      var command = (RelationEndPointModificationCommand)_loadState.CreateReplaceCommand(_collectionEndPointMock.Object, 0, _relatedObject);
      Assert.That(command, Is.InstanceOf(typeof(DomainObjectCollectionEndPointReplaceCommand)));
      Assert.That(command.ModifiedEndPoint, Is.SameAs(_collectionEndPointMock.Object));
      Assert.That(command.OldRelatedObject, Is.SameAs(oldRelatedObject));
      Assert.That(command.NewRelatedObject, Is.SameAs(_relatedObject));
      Assert.That(command.TransactionEventSink, Is.SameAs(_transactionEventSinkStub.Object));

      Assert.That(((DomainObjectCollectionEndPointReplaceCommand)command).ModifiedCollectionData, Is.SameAs(fakeCollectionData));
      Assert.That(((DomainObjectCollectionEndPointReplaceCommand)command).ModifiedCollectionEventRaiser, Is.SameAs(fakeEventRaiser.Object));
    }

    [Test]
    public void CreateReplaceCommand_SelfReplace ()
    {
      var fakeCollectionData = new DomainObjectCollectionData(new[] { _relatedObject });
      _dataManagerMock.Setup(stub => stub.CollectionData).Returns(fakeCollectionData);
      _dataManagerMock.Setup(stub => stub.ContainsOriginalItemWithoutEndPoint(_relatedObject)).Returns(false);

      var fakeCollection = new DomainObjectCollection();
      _collectionEndPointMock.Setup(mock => mock.IsNull).Returns(false);
      _collectionEndPointMock.Setup(mock => mock.Collection).Returns(fakeCollection);
      _collectionEndPointMock.Setup(mock => mock.GetDomainObject()).Returns(_owningObject);

      var command = (RelationEndPointModificationCommand)_loadState.CreateReplaceCommand(_collectionEndPointMock.Object, 0, _relatedObject);
      Assert.That(command, Is.InstanceOf(typeof(DomainObjectCollectionEndPointReplaceSameCommand)));
      Assert.That(command.ModifiedEndPoint, Is.SameAs(_collectionEndPointMock.Object));
      Assert.That(command.OldRelatedObject, Is.SameAs(_relatedObject));
      Assert.That(command.NewRelatedObject, Is.SameAs(_relatedObject));
      Assert.That(command.TransactionEventSink, Is.SameAs(_transactionEventSinkStub.Object));
    }

    [Test]
    public void CreateReplaceCommand_ItemWithUnsyncedOpposite ()
    {
      AddUnsynchronizedOppositeEndPoint(_loadState, _relatedEndPointStub.Object);

      var newRelatedObject = DomainObjectMother.CreateFakeObject<Order>();
      _dataManagerMock.Setup(stub => stub.ContainsOriginalItemWithoutEndPoint(newRelatedObject)).Returns(false);
      _dataManagerMock
          .Setup(stub => stub.CollectionData)
          .Returns(new DomainObjectCollectionData(new[] { _relatedObject }));
      Assert.That(
          () => _loadState.CreateReplaceCommand(_collectionEndPointMock.Object, 0, newRelatedObject),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The domain object with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be replaced or removed from collection property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of object 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' "
                  + "because its object property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' is out of sync with the collection property. "
                  + "To make this change, synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' property."));
    }

    [Test]
    public void CreateReplaceCommand_ItemWithoutEndPoint ()
    {
      var newRelatedObject = DomainObjectMother.CreateFakeObject<Order>();
      _dataManagerMock.Setup(stub => stub.ContainsOriginalItemWithoutEndPoint(_relatedObject)).Returns(true);
      _dataManagerMock.Setup(stub => stub.ContainsOriginalItemWithoutEndPoint(newRelatedObject)).Returns(false);
      _dataManagerMock
          .Setup(stub => stub.CollectionData)
          .Returns(new DomainObjectCollectionData(new[] { _relatedObject }));
      Assert.That(
          () => _loadState.CreateReplaceCommand(_collectionEndPointMock.Object, 0, newRelatedObject),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The domain object with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be replaced or removed from collection property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of object 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' "
                  + "because the property is out of sync with the opposite object property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer'. "
                  + "To make this change, synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' property."));
    }

    [Test]
    public void CreateReplaceCommand_WithItemWithUnsyncedOpposite ()
    {
      AddUnsynchronizedOppositeEndPoint(_loadState, _relatedEndPointStub.Object);

      _dataManagerMock
          .Setup(stub => stub.CollectionData)
          .Returns(new DomainObjectCollectionData(new[] { DomainObjectMother.CreateFakeObject<Order>() }));
      Assert.That(
          () => _loadState.CreateReplaceCommand(_collectionEndPointMock.Object, 0, _relatedObject),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The domain object with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be added to collection property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of object 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' "
                  + "because its object property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' is out of sync with the collection property. "
                  + "To make this change, synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' property."));
    }

    [Test]
    public void CreateReplaceCommand_WithItemWithoutEndPoint ()
    {
      _dataManagerMock.Setup(stub => stub.ContainsOriginalItemWithoutEndPoint(_relatedObject)).Returns(true);

      _dataManagerMock
          .Setup(stub => stub.CollectionData)
          .Returns(new DomainObjectCollectionData(new[] { DomainObjectMother.CreateFakeObject<Order>() }));
      Assert.That(
          () => _loadState.CreateReplaceCommand(_collectionEndPointMock.Object, 0, _relatedObject),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The domain object with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be added to collection property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of object 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' "
                  + "because the property is out of sync with the opposite object property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer'. "
                  + "To make this change, synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' property."));
    }

    [Test]
    public void GetOriginalOppositeEndPoints ()
    {
      _dataManagerMock.Setup(mock => mock.OriginalOppositeEndPoints).Returns(new[] { _relatedEndPointStub.Object });

      var result = (IEnumerable<IRealObjectEndPoint>)PrivateInvoke.InvokeNonPublicMethod(_loadState, "GetOriginalOppositeEndPoints");

      Assert.That(result, Is.EqualTo(new[] { _relatedEndPointStub.Object }));
    }

    [Test]
    public void GetOriginalItemsWithoutEndPoints ()
    {
      _dataManagerMock.Setup(mock => mock.OriginalItemsWithoutEndPoints).Returns(new[] { _relatedObject });

      var result = (IEnumerable<DomainObject>)PrivateInvoke.InvokeNonPublicMethod(_loadState, "GetOriginalItemsWithoutEndPoints");

      Assert.That(result, Is.EqualTo(new[] { _relatedObject }));
    }

    [Test]
    public void FlattenedSerializable ()
    {
      Assert2.IgnoreIfFeatureSerializationIsDisabled();

      var state = new CompleteDomainObjectCollectionEndPointLoadState(
          new SerializableDomainObjectCollectionEndPointDataManagerFake(),
          new SerializableRelationEndPointProviderFake(),
          new SerializableClientTransactionEventSinkFake());

      var oppositeEndPoint = new SerializableRealObjectEndPointFake(null, _relatedObject);
      AddUnsynchronizedOppositeEndPoint(state, oppositeEndPoint);

      var result = FlattenedSerializer.SerializeAndDeserialize(state);

      Assert.That(result, Is.Not.Null);
      Assert.That(result.DataManager, Is.Not.Null);
      Assert.That(result.TransactionEventSink, Is.Not.Null);
      Assert.That(result.EndPointProvider, Is.Not.Null);
      Assert.That(result.UnsynchronizedOppositeEndPoints.Count, Is.EqualTo(1));
    }

    private void AddUnsynchronizedOppositeEndPoint (CompleteDomainObjectCollectionEndPointLoadState loadState, IRealObjectEndPoint oppositeEndPoint)
    {
      var dictionary = (Dictionary<ObjectID, IRealObjectEndPoint>)PrivateInvoke.GetNonPublicField(loadState, "_unsynchronizedOppositeEndPoints");
      dictionary.Add(oppositeEndPoint.ObjectID, oppositeEndPoint);
    }
  }
}
