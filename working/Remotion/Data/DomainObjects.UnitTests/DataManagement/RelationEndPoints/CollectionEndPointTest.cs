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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.Serialization;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Data.UnitTests.UnitTesting;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class CollectionEndPointTest : ClientTransactionBaseTest
  {
    private RelationEndPointID _customerEndPointID;
    private Order _order1; // Customer1
    private Order _order3; // Customer3

    private OrderCollection _fakeCollection;
    private ICollectionEndPointCollectionManager _collectionManagerMock;
    private ILazyLoader _lazyLoaderMock;
    private IRelationEndPointProvider _endPointProviderStub;
    private IClientTransactionEventSink _transactionEventSinkStub;
    private ICollectionEndPointDataManagerFactory _dataManagerFactoryStub;
    private ICollectionEndPointLoadState _loadStateMock;

    private CollectionEndPoint _endPoint;
    
    private IRealObjectEndPoint _relatedEndPointStub;

    public override void SetUp ()
    {
      base.SetUp ();

      _customerEndPointID = RelationEndPointID.Create(DomainObjectIDs.Customer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");
      _order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      _order3 = DomainObjectIDs.Order3.GetObject<Order> ();

      _fakeCollection = new OrderCollection ();
      _collectionManagerMock = MockRepository.GenerateStrictMock<ICollectionEndPointCollectionManager> ();
      _lazyLoaderMock = MockRepository.GenerateMock<ILazyLoader> ();
      _endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider> ();
      _transactionEventSinkStub = MockRepository.GenerateStub<IClientTransactionEventSink> ();
      _dataManagerFactoryStub = MockRepository.GenerateStub<ICollectionEndPointDataManagerFactory> ();
      _loadStateMock = MockRepository.GenerateStrictMock<ICollectionEndPointLoadState> ();

      _endPoint = new CollectionEndPoint (
          TestableClientTransaction,
          _customerEndPointID,
          _collectionManagerMock,
          _lazyLoaderMock,
          _endPointProviderStub,
          _transactionEventSinkStub,
          _dataManagerFactoryStub);
      PrivateInvoke.SetNonPublicField (_endPoint, "_loadState", _loadStateMock);
      _endPointProviderStub.Stub (stub => stub.GetOrCreateVirtualEndPoint (_customerEndPointID)).Return (_endPoint);

      _relatedEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
    }

    [Test]
    public void Initialize ()
    {
      var endPoint = new CollectionEndPoint (
          TestableClientTransaction, 
          _customerEndPointID, 
          _collectionManagerMock,
          _lazyLoaderMock, 
          _endPointProviderStub,
          _transactionEventSinkStub,
          _dataManagerFactoryStub);

      Assert.That (endPoint.ID, Is.EqualTo (_customerEndPointID));
      Assert.That (endPoint.CollectionManager, Is.SameAs (_collectionManagerMock));
      Assert.That (endPoint.LazyLoader, Is.SameAs (_lazyLoaderMock));
      Assert.That (endPoint.EndPointProvider, Is.SameAs (_endPointProviderStub));
      Assert.That (endPoint.TransactionEventSink, Is.SameAs (_transactionEventSinkStub));
      Assert.That (endPoint.DataManagerFactory, Is.SameAs (_dataManagerFactoryStub));

      var loadState = CollectionEndPointTestHelper.GetLoadState (endPoint);
      Assert.That (loadState, Is.TypeOf (typeof (IncompleteCollectionEndPointLoadState)));
      Assert.That (((IncompleteCollectionEndPointLoadState) loadState).DataManagerFactory, Is.SameAs (_dataManagerFactoryStub));
      Assert.That (
          ((IncompleteCollectionEndPointLoadState) loadState).EndPointLoader, 
          Is.TypeOf<CollectionEndPoint.EndPointLoader>().With.Property<CollectionEndPoint.EndPointLoader> (l => l.LazyLoader).SameAs (_lazyLoaderMock));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "End point ID must refer to an end point with cardinality 'Many'.\r\nParameter name: id")]
    public void Initialize_WithNonManyEndPointID_Throws ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderTicket");
      new CollectionEndPoint (
          TestableClientTransaction,
          endPointID,
          _collectionManagerMock,
          _lazyLoaderMock,
          _endPointProviderStub,
          _transactionEventSinkStub,
          _dataManagerFactoryStub);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "End point ID must not refer to an anonymous end point.\r\nParameter name: id")]
    public void Initialize_WithAnonymousEndPointID_Throws ()
    {
      var endPointID = RelationEndPointObjectMother.CreateAnonymousEndPointID ();
      new CollectionEndPoint (
          TestableClientTransaction,
          endPointID,
          _collectionManagerMock,
          _lazyLoaderMock,
          _endPointProviderStub,
          _transactionEventSinkStub,
          _dataManagerFactoryStub);
    }

    [Test]
    public void Collection ()
    {
      _collectionManagerMock.Stub (stub => stub.GetCurrentCollectionReference ()).Return (_fakeCollection);

      Assert.That (_endPoint.Collection, Is.SameAs (_fakeCollection));
    }

    [Test]
    public void OriginalCollection ()
    {
      _collectionManagerMock.Stub (stub => stub.GetOriginalCollectionReference ()).Return (_fakeCollection);

      Assert.That (_endPoint.OriginalCollection, Is.SameAs (_fakeCollection));
    }

    [Test]
    public void GetCollectionEventRaiser ()
    {
      _collectionManagerMock.Stub (stub => stub.GetCurrentCollectionReference ()).Return (_fakeCollection);

      var result = _endPoint.GetCollectionEventRaiser ();

      Assert.That (result, Is.SameAs (_fakeCollection));
    }

    [Test]
    public void GetCollectionWithOriginalData ()
    {
      var collectionDataStub = MockRepository.GenerateStub<IDomainObjectCollectionData> ();
      collectionDataStub.Stub (stub => stub.RequiredItemType).Return (typeof (Order));
      var readOnlyCollectionDataDecorator = new ReadOnlyCollectionDataDecorator (collectionDataStub);

      _loadStateMock.Stub (stub => stub.GetOriginalData (_endPoint)).Return (readOnlyCollectionDataDecorator);
      _loadStateMock.Replay ();

      var result = _endPoint.GetCollectionWithOriginalData ();

      Assert.That (result, Is.TypeOf (typeof (OrderCollection)));
      var actualCollectionData = DomainObjectCollectionDataTestHelper.GetDataStrategy (result);
      Assert.That (actualCollectionData, Is.SameAs (readOnlyCollectionDataDecorator));
    }

    [Test]
    public void GetData ()
    {
      var fakeResult = new ReadOnlyCollectionDataDecorator (new DomainObjectCollectionData ());
      _loadStateMock.Expect (mock => mock.GetData (_endPoint)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPoint.GetData ();

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void GetOriginalData ()
    {
      var fakeResult = new ReadOnlyCollectionDataDecorator (new DomainObjectCollectionData ());
      _loadStateMock.Expect (mock => mock.GetOriginalData (_endPoint)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPoint.GetOriginalData ();

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void CanBeCollected ()
    {
      _collectionManagerMock.Stub (stub => stub.HasCollectionReferenceChanged ()).Return (false);
      _loadStateMock.Stub (stub => stub.CanEndPointBeCollected (_endPoint)).Return (true).Repeat.Once ();
      _loadStateMock.Stub (stub => stub.CanEndPointBeCollected (_endPoint)).Return (false).Repeat.Once ();

      var result1 = _endPoint.CanBeCollected;
      var result2 = _endPoint.CanBeCollected;

      Assert.That (result1, Is.True);
      Assert.That (result2, Is.False);
    }

    [Test]
    public void CanBeCollected_WithChangedCollectionReference ()
    {
      _collectionManagerMock.Stub (stub => stub.HasCollectionReferenceChanged ()).Return (true);
      _loadStateMock.Stub (stub => stub.CanEndPointBeCollected (_endPoint)).Return (true);

      var result = _endPoint.CanBeCollected;

      Assert.That (result, Is.False);
    }

    [Test]
    public void CanBeMarkedIncomplete ()
    {
      _collectionManagerMock.Stub (stub => stub.HasCollectionReferenceChanged ()).Return (false);
      _loadStateMock.Stub (stub => stub.CanDataBeMarkedIncomplete (_endPoint)).Return (true).Repeat.Once ();
      _loadStateMock.Stub (stub => stub.CanDataBeMarkedIncomplete (_endPoint)).Return (false).Repeat.Once ();

      var result1 = _endPoint.CanBeMarkedIncomplete;
      var result2 = _endPoint.CanBeMarkedIncomplete;

      Assert.That (result1, Is.True);
      Assert.That (result2, Is.False);
    }

    [Test]
    public void CanBeMarkedIncomplete_WithChangedCollectionReference ()
    {
      _collectionManagerMock.Stub (mock => mock.HasCollectionReferenceChanged ()).Return (true);
      _loadStateMock.Stub (mock => mock.CanDataBeMarkedIncomplete (_endPoint)).Return (true);

      var result = _endPoint.CanBeMarkedIncomplete;

      Assert.That (result, Is.False);
    }

    [Test]
    public void HasChanged_True_CollectionChanged ()
    {
      _loadStateMock.Replay();
      _collectionManagerMock.Stub (stub => stub.HasCollectionReferenceChanged ()).Return (true);

      var result = _endPoint.HasChanged;

      _loadStateMock.AssertWasNotCalled (mock => mock.HasChanged ());
      Assert.That (result, Is.True);
    }

    [Test]
    public void HasChanged_True_LoadState ()
    {
      _collectionManagerMock.Stub (stub => stub.HasCollectionReferenceChanged ()).Return (false);
      _loadStateMock.Stub (stub => stub.HasChanged ()).Return (true);

      var result = _endPoint.HasChanged;

      Assert.That (result, Is.True);
    }

    [Test]
    public void HasChanged_False ()
    {
      _collectionManagerMock.Stub (stub => stub.HasCollectionReferenceChanged ()).Return (false);
      _loadStateMock.Stub (stub => stub.HasChanged ()).Return (false);

      var result = _endPoint.HasChanged;

      Assert.That (result, Is.False);
    }

    [Test]
    public void HasChangedFast_True_CollectionChanged ()
    {
      _loadStateMock.Replay ();
      _collectionManagerMock.Stub (stub => stub.HasCollectionReferenceChanged ()).Return (true);

      var result = _endPoint.HasChangedFast;

      _loadStateMock.AssertWasNotCalled (mock => mock.HasChangedFast ());
      Assert.That (result, Is.True);
    }

    [Test]
    public void HasChangedFast_True_LoadState ()
    {
      _collectionManagerMock.Stub (stub => stub.HasCollectionReferenceChanged ()).Return (false);
      _loadStateMock.Stub (stub => stub.HasChangedFast ()).Return (true);

      var result = _endPoint.HasChangedFast;

      Assert.That (result, Is.True);
    }

    [Test]
    public void HasChangedFast_False ()
    {
      _collectionManagerMock.Stub (stub => stub.HasCollectionReferenceChanged ()).Return (false);
      _loadStateMock.Stub (stub => stub.HasChangedFast ()).Return (false);

      var result = _endPoint.HasChangedFast;

      Assert.That (result, Is.False);
    }
    
    [Test]
    public void EnsureDataComplete ()
    {
      _loadStateMock.Expect (mock => mock.EnsureDataComplete (_endPoint));
      _loadStateMock.Replay ();

      _endPoint.EnsureDataComplete();

      _loadStateMock.VerifyAllExpectations ();
    }
    
    [Test]
    public void MarkDataComplete ()
    {
      Action<ICollectionEndPointDataManager> stateSetter = null;

      var items = new DomainObject[0];

      _loadStateMock
          .Expect (mock => mock.MarkDataComplete (Arg.Is (_endPoint), Arg.Is (items), Arg<Action<ICollectionEndPointDataManager>>.Is.Anything))
          .WhenCalled (mi => { stateSetter = (Action<ICollectionEndPointDataManager>) mi.Arguments[2]; });
      _loadStateMock.Replay ();

      _endPoint.MarkDataComplete (items);

      _loadStateMock.VerifyAllExpectations ();

      Assert.That (CollectionEndPointTestHelper.GetLoadState (_endPoint), Is.SameAs (_loadStateMock));

      var dataManagerStub = MockRepository.GenerateStub<ICollectionEndPointDataManager>();
      stateSetter (dataManagerStub);
      
      var newLoadState = CollectionEndPointTestHelper.GetLoadState (_endPoint);
      Assert.That (newLoadState, Is.TypeOf (typeof (CompleteCollectionEndPointLoadState)));

      Assert.That (((CompleteCollectionEndPointLoadState) newLoadState).DataManager, Is.SameAs (dataManagerStub));
      Assert.That (((CompleteCollectionEndPointLoadState) newLoadState).TransactionEventSink, Is.SameAs (_transactionEventSinkStub));
      Assert.That (((CompleteCollectionEndPointLoadState) newLoadState).EndPointProvider, Is.SameAs (_endPointProviderStub));
    }

    [Test]
    public void MarkDataIncomplete ()
    {
      Action stateSetter = null;

      _loadStateMock
          .Expect (mock => mock.MarkDataIncomplete (Arg.Is (_endPoint), Arg<Action>.Is.Anything))
          .WhenCalled (mi => { stateSetter = (Action) mi.Arguments[1]; });
      _loadStateMock.Replay ();

      _endPoint.MarkDataIncomplete ();

      _loadStateMock.VerifyAllExpectations ();

      Assert.That (CollectionEndPointTestHelper.GetLoadState (_endPoint), Is.SameAs (_loadStateMock));

      stateSetter ();
      
      var newLoadState = CollectionEndPointTestHelper.GetLoadState (_endPoint);
      Assert.That (newLoadState, Is.TypeOf (typeof (IncompleteCollectionEndPointLoadState)));
      Assert.That (
          ((IncompleteCollectionEndPointLoadState) newLoadState).EndPointLoader,
          Is.TypeOf<CollectionEndPoint.EndPointLoader> ().With.Property<CollectionEndPoint.EndPointLoader> (l => l.LazyLoader).SameAs (_lazyLoaderMock));
    }

    [Test]
    public void Touch ()
    {
      Assert.That (_endPoint.HasBeenTouched, Is.False);
      _endPoint.Touch ();
      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void Commit_TouchedUnchanged ()
    {
      _collectionManagerMock.Stub (stub => stub.HasCollectionReferenceChanged ()).Return (false);
      _collectionManagerMock.Replay ();

      _loadStateMock.Stub (stub => stub.HasChanged ()).Return (false);
      _loadStateMock.Replay ();

      _endPoint.Touch ();
      Assert.That (_endPoint.HasBeenTouched, Is.True);
      Assert.That (_endPoint.HasChanged, Is.False);

      _endPoint.Commit();

      _collectionManagerMock.AssertWasNotCalled (mock => mock.CommitCollectionReference ());
      _loadStateMock.AssertWasNotCalled (mock => mock.Commit (_endPoint));
      Assert.That (_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void Commit_ChangedCollectionReference ()
    {
      _collectionManagerMock.Stub (stub => stub.HasCollectionReferenceChanged ()).Return (true);
      _collectionManagerMock.Expect (mock => mock.CommitCollectionReference ());
      _collectionManagerMock.Replay ();

      _loadStateMock.Stub (stub => stub.HasChanged ()).Return (false);
      _loadStateMock.Expect (mock => mock.Commit (_endPoint));
      _loadStateMock.Replay ();

      _endPoint.Touch ();

      Assert.That (_endPoint.HasBeenTouched, Is.True);
      Assert.That (_endPoint.HasChanged, Is.True);

      _endPoint.Commit ();

      _collectionManagerMock.VerifyAllExpectations ();
      _loadStateMock.VerifyAllExpectations ();
      Assert.That (_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void Commit_ChangedLoadState ()
    {
      _collectionManagerMock.Stub (stub => stub.HasCollectionReferenceChanged ()).Return (false);
      _collectionManagerMock.Expect (mock => mock.CommitCollectionReference ());
      _collectionManagerMock.Replay ();

      _loadStateMock.Stub (stub => stub.HasChanged ()).Return (true);
      _loadStateMock.Expect (mock => mock.Commit (_endPoint));
      _loadStateMock.Replay ();

      _endPoint.Touch ();

      Assert.That (_endPoint.HasBeenTouched, Is.True);
      Assert.That (_endPoint.HasChanged, Is.True);

      _endPoint.Commit ();

      _collectionManagerMock.VerifyAllExpectations ();
      _loadStateMock.VerifyAllExpectations ();
      Assert.That (_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void Rollback_ChangedCollectionReference ()
    {
      _collectionManagerMock.Stub (stub => stub.HasCollectionReferenceChanged ()).Return (true);
      _collectionManagerMock.Expect (mock => mock.RollbackCollectionReference ());
      _collectionManagerMock.Replay();

      _loadStateMock.Stub (stub => stub.HasChanged ()).Return (false);
      _loadStateMock.Expect (mock => mock.Rollback (_endPoint));
      _loadStateMock.Replay ();

      _endPoint.Touch();
      Assert.That (_endPoint.HasBeenTouched, Is.True);

      _endPoint.Rollback ();

      _collectionManagerMock.VerifyAllExpectations ();
      _loadStateMock.VerifyAllExpectations ();
      Assert.That (_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void Rollback_ChangedLoadState ()
    {
      _collectionManagerMock.Stub (stub => stub.HasCollectionReferenceChanged ()).Return (false);
      _collectionManagerMock.Expect (mock => mock.RollbackCollectionReference ());
      _collectionManagerMock.Replay ();

      _loadStateMock.Stub (stub => stub.HasChanged ()).Return (true);
      _loadStateMock.Expect (mock => mock.Rollback (_endPoint));
      _loadStateMock.Replay ();

      _endPoint.Touch ();
      Assert.That (_endPoint.HasBeenTouched, Is.True);

      _endPoint.Rollback ();

      _collectionManagerMock.VerifyAllExpectations ();
      _loadStateMock.VerifyAllExpectations ();
      Assert.That (_endPoint.HasBeenTouched, Is.False);
    }
    
    [Test]
    public void Rollback_TouchedUnchanged ()
    {
      _endPoint.Touch();

      _loadStateMock.Stub (stub => stub.HasChanged()).Return (false);
      _collectionManagerMock.Stub (stub => stub.HasCollectionReferenceChanged ()).Return (false);

      Assert.That (_endPoint.HasChanged, Is.False);
      Assert.That (_endPoint.HasBeenTouched, Is.True);

      _endPoint.Rollback ();

      _collectionManagerMock.AssertWasNotCalled (mock => mock.RollbackCollectionReference ());
      _loadStateMock.AssertWasNotCalled (mock => mock.Rollback (_endPoint));
      Assert.That (_endPoint.HasChanged, Is.False);
      Assert.That (_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void ValidateMandatory_WithItems_Succeeds ()
    {
      var domainObjectCollectionData = new DomainObjectCollectionData (new[] { DomainObjectMother.CreateFakeObject<Order> () });
      _loadStateMock
          .Stub (stub => stub.GetData (_endPoint))
          .Return (new ReadOnlyCollectionDataDecorator (domainObjectCollectionData));
      _loadStateMock.Replay ();

      _endPoint.ValidateMandatory ();
    }

    [Test]
    [ExpectedException (typeof (MandatoryRelationNotSetException), ExpectedMessage =
        "Mandatory relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of domain object "
        + "'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' contains no items.")]
    public void ValidateMandatory_WithNoItems_Throws ()
    {
      var domainObjectCollectionData = new DomainObjectCollectionData ();
      _loadStateMock
          .Stub (stub => stub.GetData (_endPoint))
          .Return (new ReadOnlyCollectionDataDecorator (domainObjectCollectionData));
      _loadStateMock.Replay ();

      _endPoint.ValidateMandatory ();
    }

    [Test]
    public void SortCurrentData ()
    {
      Comparison<DomainObject> comparison = (one, two) => 0;
      _loadStateMock.Expect (mock => mock.SortCurrentData (_endPoint, comparison));
      _loadStateMock.Replay ();

      _endPoint.SortCurrentData (comparison);

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint ()
    {
      _loadStateMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (_endPoint, _relatedEndPointStub));
      _loadStateMock.Replay ();

      _endPoint.RegisterOriginalOppositeEndPoint(_relatedEndPointStub);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint ()
    {
      _loadStateMock.Expect (mock => mock.UnregisterOriginalOppositeEndPoint (_endPoint, _relatedEndPointStub));
      _loadStateMock.Replay ();

      _endPoint.UnregisterOriginalOppositeEndPoint (_relatedEndPointStub);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void RegisterCurrentOppositeEndPoint ()
    {
      _loadStateMock.Expect (mock => mock.RegisterCurrentOppositeEndPoint (_endPoint, _relatedEndPointStub));
      _loadStateMock.Replay ();

      _endPoint.RegisterCurrentOppositeEndPoint (_relatedEndPointStub);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint ()
    {
      _loadStateMock.Expect (mock => mock.UnregisterCurrentOppositeEndPoint (_endPoint, _relatedEndPointStub));
      _loadStateMock.Replay ();

      _endPoint.UnregisterCurrentOppositeEndPoint (_relatedEndPointStub);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void IsSynchronized ()
    {
      _loadStateMock.Expect (mock => mock.IsSynchronized (_endPoint)).Return (true);
      _loadStateMock.Replay ();

      var result = _endPoint.IsSynchronized;

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.True);
    }

    [Test]
    public void Synchronize ()
    {
      _loadStateMock.Expect (mock => mock.Synchronize (_endPoint));
      _loadStateMock.Replay ();

      _endPoint.Synchronize();

      _loadStateMock.VerifyAllExpectations ();
    }
    
    [Test]
    public void SynchronizeOppositeEndPoint ()
    {
      var fakeEndPoint = _relatedEndPointStub;
      _loadStateMock.Expect (mock => mock.SynchronizeOppositeEndPoint (_endPoint, fakeEndPoint));
      _loadStateMock.Replay ();

      _endPoint.SynchronizeOppositeEndPoint (fakeEndPoint);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void CreateSetCollectionCommand ()
    {
      var oppositeDomainObjects = new OrderCollection ();
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand>();

      _loadStateMock
          .Expect (
              mock => mock.CreateSetCollectionCommand (
                  Arg.Is (_endPoint),
                  Arg.Is (oppositeDomainObjects),
                  Arg.Is (_collectionManagerMock)))
          .Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPoint.CreateSetCollectionCommand (oppositeDomainObjects);

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void CreateRemoveCommand ()
    {
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand> ();

      _loadStateMock.Expect (mock => mock.CreateRemoveCommand (_endPoint, _order1)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPoint.CreateRemoveCommand (_order1);

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand> ();

      _loadStateMock.Expect (mock => mock.CreateDeleteCommand (_endPoint)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPoint.CreateDeleteCommand ();

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void CreateInsertCommand ()
    {
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand> ();

      _loadStateMock.Expect (mock => mock.CreateInsertCommand (_endPoint, _order1, 0)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPoint.CreateInsertCommand (_order1, 0);

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void CreateAddCommand ()
    {
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand> ();

      _loadStateMock.Expect (mock => mock.CreateAddCommand (_endPoint, _order1)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPoint.CreateAddCommand (_order1);

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void CreateReplaceCommand ()
    {
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand> ();

      _loadStateMock.Expect (mock => mock.CreateReplaceCommand (_endPoint, 0, _order1)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPoint.CreateReplaceCommand (0, _order1);

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void GetOppositeRelationEndPointIDs ()
    {
      var relatedObject1 = DomainObjectMother.CreateFakeObject<Order> ();
      var relatedObject2 = DomainObjectMother.CreateFakeObject<Order> ();
      var collectionData = new DomainObjectCollectionData (new[] { relatedObject1, relatedObject2 });

      _loadStateMock.Stub (stub => stub.GetData (_endPoint)).Return (new ReadOnlyCollectionDataDecorator (collectionData));
      _loadStateMock.Replay();

      var oppositeEndPoints = _endPoint.GetOppositeRelationEndPointIDs ().ToArray ();

      var expectedOppositeEndPointID1 = RelationEndPointID.Create (relatedObject1.ID, typeof (Order).FullName + ".Customer");
      var expectedOppositeEndPointID2 = RelationEndPointID.Create (relatedObject2.ID, typeof (Order).FullName + ".Customer");
      Assert.That (oppositeEndPoints, Is.EqualTo (new[] { expectedOppositeEndPointID1, expectedOppositeEndPointID2 }));
    }

    [Test]
    public void SetDataFromSubTransaction ()
    {
      var source = RelationEndPointObjectMother.CreateCollectionEndPoint (_customerEndPointID, new[] { _order3 });

      _loadStateMock.Stub (stub => stub.HasChanged ()).Return (false);
      _loadStateMock.Expect (mock => mock.SetDataFromSubTransaction (_endPoint, CollectionEndPointTestHelper.GetLoadState (source)));
      _loadStateMock.Replay ();

      _collectionManagerMock.Stub (stub => stub.HasCollectionReferenceChanged ()).Return (false);

      _endPoint.SetDataFromSubTransaction (source);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void SetDataFromSubTransaction_TouchesEndPoint_WhenSourceHasBeenTouched ()
    {
      var source = RelationEndPointObjectMother.CreateCollectionEndPoint (_customerEndPointID, new[] { _order3 });
      source.Touch ();

      _loadStateMock.Stub (stub => stub.HasChanged ()).Return (false);
      _loadStateMock.Stub (stub => stub.SetDataFromSubTransaction (_endPoint, CollectionEndPointTestHelper.GetLoadState (source)));
      _loadStateMock.Replay ();

      _collectionManagerMock.Stub (stub => stub.HasCollectionReferenceChanged ()).Return (false);

      Assert.That (_endPoint.HasBeenTouched, Is.False);

      _endPoint.SetDataFromSubTransaction (source);

      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetDataFromSubTransaction_TouchesEndPoint_WhenTargetLoadStateHasChanged ()
    {
      var source = RelationEndPointObjectMother.CreateCollectionEndPoint (_customerEndPointID, new[] { _order3 });

      _loadStateMock.Stub (stub => stub.HasChanged ()).Return (true);
      _loadStateMock.Stub (stub => stub.SetDataFromSubTransaction (_endPoint, CollectionEndPointTestHelper.GetLoadState (source)));
      _loadStateMock.Replay ();

      _collectionManagerMock.Stub (stub => stub.HasCollectionReferenceChanged ()).Return (false);

      Assert.That (_endPoint.HasBeenTouched, Is.False);

      _endPoint.SetDataFromSubTransaction (source);

      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetDataFromSubTransaction_TouchesEndPoint_WhenTargetCollectionReferenceHasChanged ()
    {
      var source = RelationEndPointObjectMother.CreateCollectionEndPoint (_customerEndPointID, new[] { _order3 });

      _loadStateMock.Stub (stub => stub.HasChanged ()).Return (false);
      _loadStateMock.Stub (stub => stub.SetDataFromSubTransaction (_endPoint, CollectionEndPointTestHelper.GetLoadState (source)));
      _loadStateMock.Replay ();

      _collectionManagerMock.Stub (stub => stub.HasCollectionReferenceChanged ()).Return (true);

      Assert.That (_endPoint.HasBeenTouched, Is.False);

      _endPoint.SetDataFromSubTransaction (source);

      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetDataFromSubTransaction_DoesNotTouchEndPoint_WhenSourceUntouched_AndTargetUnchanged ()
    {
      var source = RelationEndPointObjectMother.CreateCollectionEndPoint (_customerEndPointID, new[] { _order3 });

      _loadStateMock.Stub (stub => stub.HasChanged ()).Return (false);
      _loadStateMock.Stub (stub => stub.SetDataFromSubTransaction (_endPoint, CollectionEndPointTestHelper.GetLoadState (source)));
      _loadStateMock.Replay ();

      _collectionManagerMock.Stub (stub => stub.HasCollectionReferenceChanged ()).Return (false);

      Assert.That (_endPoint.HasBeenTouched, Is.False);

      _endPoint.SetDataFromSubTransaction (source);

      Assert.That (_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Cannot set this end point's value from "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems'; the end points "
        + "do not have the same end point definition.\r\nParameter name: source")]
    public void SetDataFromSubTransaction_InvalidDefinition ()
    {
      var otherID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var source = RelationEndPointObjectMother.CreateCollectionEndPoint (otherID, new DomainObject[0]);

      _endPoint.SetDataFromSubTransaction (source);
    }

    [Test]
    public void EndPointLoader_LoadEndPointAndGetNewState ()
    {
      var endPointLoader = new CollectionEndPoint.EndPointLoader (_lazyLoaderMock);
      var loadStateFake = MockRepository.GenerateStub<ICollectionEndPointLoadState> ();
      _lazyLoaderMock
          .Expect (mock => mock.LoadLazyCollectionEndPoint (_customerEndPointID))
          .WhenCalled (mi => CollectionEndPointTestHelper.SetLoadState (_endPoint, loadStateFake));

      _lazyLoaderMock.Replay ();

      var result = endPointLoader.LoadEndPointAndGetNewState (_endPoint);

      _lazyLoaderMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (loadStateFake));
    }

    [Test]
    public void EndPointLoader_Serializable ()
    {
      var endPointLoader = new CollectionEndPoint.EndPointLoader (new SerializableLazyLoaderFake());

      var deserializedInstance = FlattenedSerializer.SerializeAndDeserialize (endPointLoader);

      Assert.That (deserializedInstance.LazyLoader, Is.Not.Null);
    }
  }
}
