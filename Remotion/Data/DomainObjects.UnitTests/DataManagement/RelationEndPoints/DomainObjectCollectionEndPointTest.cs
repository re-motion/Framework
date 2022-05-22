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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.Serialization;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.UnitTesting;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class DomainObjectCollectionEndPointTest : ClientTransactionBaseTest
  {
    private RelationEndPointID _customerEndPointID;
    private Order _order1; // Customer1
    private Order _order3; // Customer3

    private OrderCollection _fakeCollection;
    private Mock<IDomainObjectCollectionEndPointCollectionManager> _collectionManagerMock;
    private Mock<ILazyLoader> _lazyLoaderMock;
    private Mock<IRelationEndPointProvider> _endPointProviderStub;
    private Mock<IClientTransactionEventSink> _transactionEventSinkStub;
    private Mock<IDomainObjectCollectionEndPointDataManagerFactory> _dataManagerFactoryStub;
    private Mock<IDomainObjectCollectionEndPointLoadState> _loadStateMock;

    private DomainObjectCollectionEndPoint _endPoint;

    private Mock<IRealObjectEndPoint> _relatedEndPointStub;

    public override void SetUp ()
    {
      base.SetUp();

      _customerEndPointID = RelationEndPointID.Create(DomainObjectIDs.Customer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");
      _order1 = DomainObjectIDs.Order1.GetObject<Order>();
      _order3 = DomainObjectIDs.Order3.GetObject<Order>();

      _fakeCollection = new OrderCollection();
      _collectionManagerMock = new Mock<IDomainObjectCollectionEndPointCollectionManager>(MockBehavior.Strict);
      _lazyLoaderMock = new Mock<ILazyLoader>();
      _endPointProviderStub = new Mock<IRelationEndPointProvider>();
      _transactionEventSinkStub = new Mock<IClientTransactionEventSink>();
      _dataManagerFactoryStub = new Mock<IDomainObjectCollectionEndPointDataManagerFactory>();
      _loadStateMock = new Mock<IDomainObjectCollectionEndPointLoadState>(MockBehavior.Strict);

      _endPoint = new DomainObjectCollectionEndPoint(
          TestableClientTransaction,
          _customerEndPointID,
          _collectionManagerMock.Object,
          _lazyLoaderMock.Object,
          _endPointProviderStub.Object,
          _transactionEventSinkStub.Object,
          _dataManagerFactoryStub.Object);
      PrivateInvoke.SetNonPublicField(_endPoint, "_loadState", _loadStateMock.Object);
      _endPointProviderStub.Setup(stub => stub.GetOrCreateVirtualEndPoint(_customerEndPointID)).Returns(_endPoint);

      _relatedEndPointStub = new Mock<IRealObjectEndPoint>();
    }

    [Test]
    public void Initialize ()
    {
      var endPoint = new DomainObjectCollectionEndPoint(
          TestableClientTransaction,
          _customerEndPointID,
          _collectionManagerMock.Object,
          _lazyLoaderMock.Object,
          _endPointProviderStub.Object,
          _transactionEventSinkStub.Object,
          _dataManagerFactoryStub.Object);

      Assert.That(endPoint.ID, Is.EqualTo(_customerEndPointID));
      Assert.That(endPoint.CollectionManager, Is.SameAs(_collectionManagerMock.Object));
      Assert.That(endPoint.LazyLoader, Is.SameAs(_lazyLoaderMock.Object));
      Assert.That(endPoint.EndPointProvider, Is.SameAs(_endPointProviderStub.Object));
      Assert.That(endPoint.TransactionEventSink, Is.SameAs(_transactionEventSinkStub.Object));
      Assert.That(endPoint.DataManagerFactory, Is.SameAs(_dataManagerFactoryStub.Object));

      var loadState = DomainObjectCollectionEndPointTestHelper.GetLoadState(endPoint);
      Assert.That(loadState, Is.TypeOf(typeof(IncompleteDomainObjectCollectionEndPointLoadState)));
      Assert.That(((IncompleteDomainObjectCollectionEndPointLoadState)loadState).DataManagerFactory, Is.SameAs(_dataManagerFactoryStub.Object));
      Assert.That(
          ((IncompleteDomainObjectCollectionEndPointLoadState)loadState).EndPointLoader,
          Is.TypeOf<DomainObjectCollectionEndPoint.EndPointLoader>()
              .With.Property<DomainObjectCollectionEndPoint.EndPointLoader>(l => l.LazyLoader).SameAs(_lazyLoaderMock.Object));
    }

    [Test]
    public void Initialize_WithNonManyEndPointID_Throws ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderTicket");
      Assert.That(
          () => new DomainObjectCollectionEndPoint(
          TestableClientTransaction,
          endPointID,
          _collectionManagerMock.Object,
          _lazyLoaderMock.Object,
          _endPointProviderStub.Object,
          _transactionEventSinkStub.Object,
          _dataManagerFactoryStub.Object),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("End point ID must refer to an end point with cardinality 'Many'.", "id"));
    }

    [Test]
    public void Initialize_WithAnonymousEndPointID_Throws ()
    {
      var endPointID = RelationEndPointObjectMother.CreateAnonymousEndPointID();
      Assert.That(
          () => new DomainObjectCollectionEndPoint(
          TestableClientTransaction,
          endPointID,
          _collectionManagerMock.Object,
          _lazyLoaderMock.Object,
          _endPointProviderStub.Object,
          _transactionEventSinkStub.Object,
          _dataManagerFactoryStub.Object),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("End point ID must not refer to an anonymous end point.", "id"));
    }

    [Test]
    public void Collection ()
    {
      _collectionManagerMock.Setup(stub => stub.GetCurrentCollectionReference()).Returns(_fakeCollection);

      Assert.That(_endPoint.Collection, Is.SameAs(_fakeCollection));
    }

    [Test]
    public void OriginalCollection ()
    {
      _collectionManagerMock.Setup(stub => stub.GetOriginalCollectionReference()).Returns(_fakeCollection);

      Assert.That(_endPoint.OriginalCollection, Is.SameAs(_fakeCollection));
    }

    [Test]
    public void GetCollectionEventRaiser ()
    {
      _collectionManagerMock.Setup(stub => stub.GetCurrentCollectionReference()).Returns(_fakeCollection);

      var result = _endPoint.GetCollectionEventRaiser();

      Assert.That(result, Is.SameAs(_fakeCollection));
    }

    [Test]
    public void GetCollectionWithOriginalData ()
    {
      var collectionDataStub = new Mock<IDomainObjectCollectionData>();
      collectionDataStub.Setup(stub => stub.RequiredItemType).Returns(typeof(Order));
      var readOnlyCollectionDataDecorator = new ReadOnlyDomainObjectCollectionDataDecorator(collectionDataStub.Object);

      _loadStateMock.Setup(stub => stub.GetOriginalData(_endPoint)).Returns(readOnlyCollectionDataDecorator);

      var result = _endPoint.GetCollectionWithOriginalData();

      Assert.That(result, Is.TypeOf(typeof(OrderCollection)));
      var actualCollectionData = DomainObjectCollectionDataTestHelper.GetDataStrategy(result);
      Assert.That(actualCollectionData, Is.SameAs(readOnlyCollectionDataDecorator));
    }

    [Test]
    public void GetData ()
    {
      var fakeResult = new ReadOnlyDomainObjectCollectionDataDecorator(new DomainObjectCollectionData());
      _loadStateMock.Setup(mock => mock.GetData(_endPoint)).Returns(fakeResult).Verifiable();

      var result = _endPoint.GetData();

      _loadStateMock.Verify();
      Assert.That(result, Is.SameAs(fakeResult));
    }

    [Test]
    public void GetOriginalData ()
    {
      var fakeResult = new ReadOnlyDomainObjectCollectionDataDecorator(new DomainObjectCollectionData());
      _loadStateMock.Setup(mock => mock.GetOriginalData(_endPoint)).Returns(fakeResult).Verifiable();

      var result = _endPoint.GetOriginalData();

      _loadStateMock.Verify();
      Assert.That(result, Is.SameAs(fakeResult));
    }

    [Test]
    public void CanBeCollected_WithoutChangedCollectionReference_WithCanEndPointBeCollectedIsTrue_ReturnsTrue ()
    {
      _collectionManagerMock.Setup(stub => stub.HasCollectionReferenceChanged()).Returns(false);
      _loadStateMock.Setup(stub => stub.CanEndPointBeCollected(_endPoint)).Returns(true);

      var result = _endPoint.CanBeCollected;

      Assert.That(result, Is.True);
    }

    [Test]
    public void CanBeCollected_WithoutChangedCollectionReference_WithCanEndPointBeCollectedIsFalse_ReturnsFalse ()
    {
      _collectionManagerMock.Setup(stub => stub.HasCollectionReferenceChanged()).Returns(false);
      _loadStateMock.Setup(stub => stub.CanEndPointBeCollected(_endPoint)).Returns(false);

      var result = _endPoint.CanBeCollected;

      Assert.That(result, Is.False);
    }

    [Test]
    public void CanBeCollected_WithChangedCollectionReference ()
    {
      _collectionManagerMock.Setup(stub => stub.HasCollectionReferenceChanged()).Returns(true);
      _loadStateMock.Setup(stub => stub.CanEndPointBeCollected(_endPoint)).Returns(true);

      var result = _endPoint.CanBeCollected;

      Assert.That(result, Is.False);
    }

    [Test]
    public void CanBeMarkedIncomplete_WithoutChangedCollectionReference_WithCanDataBeMarkedIncompleteIsTrue_ReturnsTrue ()
    {
      _collectionManagerMock.Setup(stub => stub.HasCollectionReferenceChanged()).Returns(false);
      _loadStateMock.Setup(stub => stub.CanDataBeMarkedIncomplete(_endPoint)).Returns(true);

      var result = _endPoint.CanBeMarkedIncomplete;

      Assert.That(result, Is.True);
    }

    [Test]
    public void CanBeMarkedIncomplete_WithoutChangedCollectionReference_WithCanDataBeMarkedIncompleteIsFalse_ReturnsFalse ()
    {
      _collectionManagerMock.Setup(stub => stub.HasCollectionReferenceChanged()).Returns(false);
      _loadStateMock.Setup(stub => stub.CanDataBeMarkedIncomplete(_endPoint)).Returns(false);

      var result = _endPoint.CanBeMarkedIncomplete;

      Assert.That(result, Is.False);
    }

    [Test]
    public void CanBeMarkedIncomplete_WithChangedCollectionReference ()
    {
      _collectionManagerMock.Setup(mock => mock.HasCollectionReferenceChanged()).Returns(true);
      _loadStateMock.Setup(mock => mock.CanDataBeMarkedIncomplete(_endPoint)).Returns(true);

      var result = _endPoint.CanBeMarkedIncomplete;

      Assert.That(result, Is.False);
    }

    [Test]
    public void HasChanged_True_CollectionChanged ()
    {
      _collectionManagerMock.Setup(stub => stub.HasCollectionReferenceChanged()).Returns(true);

      var result = _endPoint.HasChanged;

      _loadStateMock.Verify(mock => mock.HasChanged(), Times.Never());
      Assert.That(result, Is.True);
    }

    [Test]
    public void HasChanged_True_LoadState ()
    {
      _collectionManagerMock.Setup(stub => stub.HasCollectionReferenceChanged()).Returns(false);
      _loadStateMock.Setup(stub => stub.HasChanged()).Returns(true);

      var result = _endPoint.HasChanged;

      Assert.That(result, Is.True);
    }

    [Test]
    public void HasChanged_False ()
    {
      _collectionManagerMock.Setup(stub => stub.HasCollectionReferenceChanged()).Returns(false);
      _loadStateMock.Setup(stub => stub.HasChanged()).Returns(false);

      var result = _endPoint.HasChanged;

      Assert.That(result, Is.False);
    }

    [Test]
    public void HasChangedFast_True_CollectionChanged ()
    {
      _collectionManagerMock.Setup(stub => stub.HasCollectionReferenceChanged()).Returns(true);

      var result = _endPoint.HasChangedFast;

      _loadStateMock.Verify(mock => mock.HasChangedFast(), Times.Never());
      Assert.That(result, Is.True);
    }

    [Test]
    public void HasChangedFast_True_LoadState ()
    {
      _collectionManagerMock.Setup(stub => stub.HasCollectionReferenceChanged()).Returns(false);
      _loadStateMock.Setup(stub => stub.HasChangedFast()).Returns(true);

      var result = _endPoint.HasChangedFast;

      Assert.That(result, Is.True);
    }

    [Test]
    public void HasChangedFast_False ()
    {
      _collectionManagerMock.Setup(stub => stub.HasCollectionReferenceChanged()).Returns(false);
      _loadStateMock.Setup(stub => stub.HasChangedFast()).Returns(false);

      var result = _endPoint.HasChangedFast;

      Assert.That(result, Is.False);
    }

    [Test]
    public void EnsureDataComplete ()
    {
      _loadStateMock.Setup(mock => mock.EnsureDataComplete(_endPoint)).Verifiable();

      _endPoint.EnsureDataComplete();

      _loadStateMock.Verify();
    }

    [Test]
    public void MarkDataComplete ()
    {
      Action<IDomainObjectCollectionEndPointDataManager> actualStateSetter = null;

      var items = new DomainObject[0];

      _loadStateMock
          .Setup(mock => mock.MarkDataComplete(_endPoint, items, It.IsAny<Action<IDomainObjectCollectionEndPointDataManager>>()))
          .Callback(
              (IDomainObjectCollectionEndPoint _, IEnumerable<DomainObject> _, Action<IDomainObjectCollectionEndPointDataManager> stateSetter) =>
              {
                actualStateSetter = stateSetter;
              })
          .Verifiable();

      _endPoint.MarkDataComplete(items);

      _loadStateMock.Verify();

      Assert.That(DomainObjectCollectionEndPointTestHelper.GetLoadState(_endPoint), Is.SameAs(_loadStateMock.Object));

      var dataManagerStub = new Mock<IDomainObjectCollectionEndPointDataManager>();
      actualStateSetter(dataManagerStub.Object);

      var newLoadState = DomainObjectCollectionEndPointTestHelper.GetLoadState(_endPoint);
      Assert.That(newLoadState, Is.TypeOf(typeof(CompleteDomainObjectCollectionEndPointLoadState)));

      Assert.That(((CompleteDomainObjectCollectionEndPointLoadState)newLoadState).DataManager, Is.SameAs(dataManagerStub.Object));
      Assert.That(((CompleteDomainObjectCollectionEndPointLoadState)newLoadState).TransactionEventSink, Is.SameAs(_transactionEventSinkStub.Object));
      Assert.That(((CompleteDomainObjectCollectionEndPointLoadState)newLoadState).EndPointProvider, Is.SameAs(_endPointProviderStub.Object));
    }

    [Test]
    public void MarkDataIncomplete ()
    {
      Action actualStateSetter = null;

      _loadStateMock
          .Setup(mock => mock.MarkDataIncomplete(_endPoint, It.IsAny<Action>()))
          .Callback((IDomainObjectCollectionEndPoint _, Action stateSetter) => { actualStateSetter = stateSetter; })
          .Verifiable();

      _endPoint.MarkDataIncomplete();

      _loadStateMock.Verify();

      Assert.That(DomainObjectCollectionEndPointTestHelper.GetLoadState(_endPoint), Is.SameAs(_loadStateMock.Object));

      actualStateSetter();

      var newLoadState = DomainObjectCollectionEndPointTestHelper.GetLoadState(_endPoint);
      Assert.That(newLoadState, Is.TypeOf(typeof(IncompleteDomainObjectCollectionEndPointLoadState)));
      Assert.That(
          ((IncompleteDomainObjectCollectionEndPointLoadState)newLoadState).EndPointLoader,
          Is.TypeOf<DomainObjectCollectionEndPoint.EndPointLoader>()
              .With.Property<DomainObjectCollectionEndPoint.EndPointLoader>(l => l.LazyLoader).SameAs(_lazyLoaderMock.Object));
    }

    [Test]
    public void Touch ()
    {
      Assert.That(_endPoint.HasBeenTouched, Is.False);
      _endPoint.Touch();
      Assert.That(_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void Commit_TouchedUnchanged ()
    {
      _collectionManagerMock.Setup(stub => stub.HasCollectionReferenceChanged()).Returns(false);

      _loadStateMock.Setup(stub => stub.HasChanged()).Returns(false);

      _endPoint.Touch();
      Assert.That(_endPoint.HasBeenTouched, Is.True);
      Assert.That(_endPoint.HasChanged, Is.False);

      _endPoint.Commit();

      _collectionManagerMock.Verify(mock => mock.CommitCollectionReference(), Times.Never());
      _loadStateMock.Verify(mock => mock.Commit(_endPoint), Times.Never());
      Assert.That(_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void Commit_ChangedCollectionReference ()
    {
      _collectionManagerMock.Setup(stub => stub.HasCollectionReferenceChanged()).Returns(true);
      _collectionManagerMock.Setup(mock => mock.CommitCollectionReference()).Verifiable();

      _loadStateMock.Setup(stub => stub.HasChanged()).Returns(false);
      _loadStateMock.Setup(mock => mock.Commit(_endPoint)).Verifiable();

      _endPoint.Touch();

      Assert.That(_endPoint.HasBeenTouched, Is.True);
      Assert.That(_endPoint.HasChanged, Is.True);

      _endPoint.Commit();

      _collectionManagerMock.Verify();
      _loadStateMock.Verify();
      Assert.That(_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void Commit_ChangedLoadState ()
    {
      _collectionManagerMock.Setup(stub => stub.HasCollectionReferenceChanged()).Returns(false);
      _collectionManagerMock.Setup(mock => mock.CommitCollectionReference()).Verifiable();

      _loadStateMock.Setup(stub => stub.HasChanged()).Returns(true);
      _loadStateMock.Setup(mock => mock.Commit(_endPoint)).Verifiable();

      _endPoint.Touch();

      Assert.That(_endPoint.HasBeenTouched, Is.True);
      Assert.That(_endPoint.HasChanged, Is.True);

      _endPoint.Commit();

      _collectionManagerMock.Verify();
      _loadStateMock.Verify();
      Assert.That(_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void Rollback_ChangedCollectionReference ()
    {
      _collectionManagerMock.Setup(stub => stub.HasCollectionReferenceChanged()).Returns(true);
      _collectionManagerMock.Setup(mock => mock.RollbackCollectionReference()).Verifiable();

      _loadStateMock.Setup(stub => stub.HasChanged()).Returns(false);
      _loadStateMock.Setup(mock => mock.Rollback(_endPoint)).Verifiable();

      _endPoint.Touch();
      Assert.That(_endPoint.HasBeenTouched, Is.True);

      _endPoint.Rollback();

      _collectionManagerMock.Verify();
      _loadStateMock.Verify();
      Assert.That(_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void Rollback_ChangedLoadState ()
    {
      _collectionManagerMock.Setup(stub => stub.HasCollectionReferenceChanged()).Returns(false);
      _collectionManagerMock.Setup(mock => mock.RollbackCollectionReference()).Verifiable();

      _loadStateMock.Setup(stub => stub.HasChanged()).Returns(true);
      _loadStateMock.Setup(mock => mock.Rollback(_endPoint)).Verifiable();

      _endPoint.Touch();
      Assert.That(_endPoint.HasBeenTouched, Is.True);

      _endPoint.Rollback();

      _collectionManagerMock.Verify();
      _loadStateMock.Verify();
      Assert.That(_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void Rollback_TouchedUnchanged ()
    {
      _endPoint.Touch();

      _loadStateMock.Setup(stub => stub.HasChanged()).Returns(false);
      _collectionManagerMock.Setup(stub => stub.HasCollectionReferenceChanged()).Returns(false);

      Assert.That(_endPoint.HasChanged, Is.False);
      Assert.That(_endPoint.HasBeenTouched, Is.True);

      _endPoint.Rollback();

      _collectionManagerMock.Verify(mock => mock.RollbackCollectionReference(), Times.Never());
      _loadStateMock.Verify(mock => mock.Rollback(_endPoint), Times.Never());
      Assert.That(_endPoint.HasChanged, Is.False);
      Assert.That(_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void ValidateMandatory_WithItems_Succeeds ()
    {
      var domainObjectCollectionData = new DomainObjectCollectionData(new[] { DomainObjectMother.CreateFakeObject<Order>() });
      _loadStateMock
          .Setup(stub => stub.GetData(_endPoint))
          .Returns(new ReadOnlyDomainObjectCollectionDataDecorator(domainObjectCollectionData));

      _endPoint.ValidateMandatory();
    }

    [Test]
    public void ValidateMandatory_WithNoItems_Throws ()
    {
      var domainObjectCollectionData = new DomainObjectCollectionData();
      _loadStateMock
          .Setup(stub => stub.GetData(_endPoint))
          .Returns(new ReadOnlyDomainObjectCollectionDataDecorator(domainObjectCollectionData));
      Assert.That(
          () => _endPoint.ValidateMandatory(),
          Throws.InstanceOf<MandatoryRelationNotSetException>()
              .With.Message.EqualTo(
                  "Mandatory relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of domain object "
                  + "'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' contains no items."));
    }

    [Test]
    public void SortCurrentData ()
    {
      Comparison<IDomainObject> comparison = (one, two) => 0;
      _loadStateMock.Setup(mock => mock.SortCurrentData(_endPoint, comparison)).Verifiable();

      _endPoint.SortCurrentData(comparison);

      _loadStateMock.Verify();
      Assert.That(_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint ()
    {
      _loadStateMock.Setup(mock => mock.RegisterOriginalOppositeEndPoint(_endPoint, _relatedEndPointStub.Object)).Verifiable();

      _endPoint.RegisterOriginalOppositeEndPoint(_relatedEndPointStub.Object);

      _loadStateMock.Verify();
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint ()
    {
      _loadStateMock.Setup(mock => mock.UnregisterOriginalOppositeEndPoint(_endPoint, _relatedEndPointStub.Object)).Verifiable();

      _endPoint.UnregisterOriginalOppositeEndPoint(_relatedEndPointStub.Object);

      _loadStateMock.Verify();
    }

    [Test]
    public void RegisterCurrentOppositeEndPoint ()
    {
      _loadStateMock.Setup(mock => mock.RegisterCurrentOppositeEndPoint(_endPoint, _relatedEndPointStub.Object)).Verifiable();

      _endPoint.RegisterCurrentOppositeEndPoint(_relatedEndPointStub.Object);

      _loadStateMock.Verify();
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint ()
    {
      _loadStateMock.Setup(mock => mock.UnregisterCurrentOppositeEndPoint(_endPoint, _relatedEndPointStub.Object)).Verifiable();

      _endPoint.UnregisterCurrentOppositeEndPoint(_relatedEndPointStub.Object);

      _loadStateMock.Verify();
    }

    [Test]
    public void IsSynchronized ()
    {
      _loadStateMock.Setup(mock => mock.IsSynchronized(_endPoint)).Returns(true).Verifiable();

      var result = _endPoint.IsSynchronized;

      _loadStateMock.Verify();
      Assert.That(result, Is.True);
    }

    [Test]
    public void Synchronize ()
    {
      _loadStateMock.Setup(mock => mock.Synchronize(_endPoint)).Verifiable();

      _endPoint.Synchronize();

      _loadStateMock.Verify();
    }

    [Test]
    public void SynchronizeOppositeEndPoint ()
    {
      var fakeEndPoint = _relatedEndPointStub;
      _loadStateMock.Setup(mock => mock.SynchronizeOppositeEndPoint(_endPoint, fakeEndPoint.Object)).Verifiable();

      _endPoint.SynchronizeOppositeEndPoint(fakeEndPoint.Object);

      _loadStateMock.Verify();
    }

    [Test]
    public void CreateSetCollectionCommand ()
    {
      var oppositeDomainObjects = new OrderCollection();
      var fakeResult = new Mock<IDataManagementCommand>();

      _loadStateMock
          .Setup(
              mock => mock.CreateSetCollectionCommand(
                  _endPoint,
                  oppositeDomainObjects,
                  _collectionManagerMock.Object))
          .Returns(fakeResult.Object)
          .Verifiable();

      var result = _endPoint.CreateSetCollectionCommand(oppositeDomainObjects);

      _loadStateMock.Verify();
      Assert.That(result, Is.SameAs(fakeResult.Object));
    }

    [Test]
    public void CreateRemoveCommand ()
    {
      var fakeResult = new Mock<IDataManagementCommand>();

      _loadStateMock.Setup(mock => mock.CreateRemoveCommand(_endPoint, _order1)).Returns(fakeResult.Object).Verifiable();

      var result = _endPoint.CreateRemoveCommand(_order1);

      _loadStateMock.Verify();
      Assert.That(result, Is.SameAs(fakeResult.Object));
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      var fakeResult = new Mock<IDataManagementCommand>();

      _loadStateMock.Setup(mock => mock.CreateDeleteCommand(_endPoint)).Returns(fakeResult.Object).Verifiable();

      var result = _endPoint.CreateDeleteCommand();

      _loadStateMock.Verify();
      Assert.That(result, Is.SameAs(fakeResult.Object));
    }

    [Test]
    public void CreateInsertCommand ()
    {
      var fakeResult = new Mock<IDataManagementCommand>();

      _loadStateMock.Setup(mock => mock.CreateInsertCommand(_endPoint, _order1, 0)).Returns(fakeResult.Object).Verifiable();

      var result = _endPoint.CreateInsertCommand(_order1, 0);

      _loadStateMock.Verify();
      Assert.That(result, Is.SameAs(fakeResult.Object));
    }

    [Test]
    public void CreateAddCommand ()
    {
      var fakeResult = new Mock<IDataManagementCommand>();

      _loadStateMock.Setup(mock => mock.CreateAddCommand(_endPoint, _order1)).Returns(fakeResult.Object).Verifiable();

      var result = _endPoint.CreateAddCommand(_order1);

      _loadStateMock.Verify();
      Assert.That(result, Is.SameAs(fakeResult.Object));
    }

    [Test]
    public void CreateReplaceCommand ()
    {
      var fakeResult = new Mock<IDataManagementCommand>();

      _loadStateMock.Setup(mock => mock.CreateReplaceCommand(_endPoint, 0, _order1)).Returns(fakeResult.Object).Verifiable();

      var result = _endPoint.CreateReplaceCommand(0, _order1);

      _loadStateMock.Verify();
      Assert.That(result, Is.SameAs(fakeResult.Object));
    }

    [Test]
    public void GetOppositeRelationEndPointIDs ()
    {
      var relatedObject1 = DomainObjectMother.CreateFakeObject<Order>();
      var relatedObject2 = DomainObjectMother.CreateFakeObject<Order>();
      var collectionData = new DomainObjectCollectionData(new[] { relatedObject1, relatedObject2 });

      _loadStateMock.Setup(stub => stub.GetData(_endPoint)).Returns(new ReadOnlyDomainObjectCollectionDataDecorator(collectionData));

      var oppositeEndPoints = _endPoint.GetOppositeRelationEndPointIDs().ToArray();

      var expectedOppositeEndPointID1 = RelationEndPointID.Create(relatedObject1.ID, typeof(Order).FullName + ".Customer");
      var expectedOppositeEndPointID2 = RelationEndPointID.Create(relatedObject2.ID, typeof(Order).FullName + ".Customer");
      Assert.That(oppositeEndPoints, Is.EqualTo(new[] { expectedOppositeEndPointID1, expectedOppositeEndPointID2 }));
    }

    [Test]
    public void SetDataFromSubTransaction ()
    {
      var source = RelationEndPointObjectMother.CreateDomainObjectCollectionEndPoint(_customerEndPointID, new[] { _order3 });

      _loadStateMock.Setup(stub => stub.HasChanged()).Returns(false);
      _loadStateMock.Setup(mock => mock.SetDataFromSubTransaction(_endPoint, DomainObjectCollectionEndPointTestHelper.GetLoadState(source))).Verifiable();

      _collectionManagerMock.Setup(stub => stub.HasCollectionReferenceChanged()).Returns(false);

      _endPoint.SetDataFromSubTransaction(source);

      _loadStateMock.Verify();
    }

    [Test]
    public void SetDataFromSubTransaction_TouchesEndPoint_WhenSourceHasBeenTouched ()
    {
      var source = RelationEndPointObjectMother.CreateDomainObjectCollectionEndPoint(_customerEndPointID, new[] { _order3 });
      source.Touch();

      _loadStateMock.Setup(stub => stub.HasChanged()).Returns(false);
      _loadStateMock.Setup(stub => stub.SetDataFromSubTransaction(_endPoint, DomainObjectCollectionEndPointTestHelper.GetLoadState(source)));

      _collectionManagerMock.Setup(stub => stub.HasCollectionReferenceChanged()).Returns(false);

      Assert.That(_endPoint.HasBeenTouched, Is.False);

      _endPoint.SetDataFromSubTransaction(source);

      Assert.That(_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetDataFromSubTransaction_TouchesEndPoint_WhenTargetLoadStateHasChanged ()
    {
      var source = RelationEndPointObjectMother.CreateDomainObjectCollectionEndPoint(_customerEndPointID, new[] { _order3 });

      _loadStateMock.Setup(stub => stub.HasChanged()).Returns(true);
      _loadStateMock.Setup(stub => stub.SetDataFromSubTransaction(_endPoint, DomainObjectCollectionEndPointTestHelper.GetLoadState(source)));

      _collectionManagerMock.Setup(stub => stub.HasCollectionReferenceChanged()).Returns(false);

      Assert.That(_endPoint.HasBeenTouched, Is.False);

      _endPoint.SetDataFromSubTransaction(source);

      Assert.That(_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetDataFromSubTransaction_TouchesEndPoint_WhenTargetCollectionReferenceHasChanged ()
    {
      var source = RelationEndPointObjectMother.CreateDomainObjectCollectionEndPoint(_customerEndPointID, new[] { _order3 });

      _loadStateMock.Setup(stub => stub.HasChanged()).Returns(false);
      _loadStateMock.Setup(stub => stub.SetDataFromSubTransaction(_endPoint, DomainObjectCollectionEndPointTestHelper.GetLoadState(source)));

      _collectionManagerMock.Setup(stub => stub.HasCollectionReferenceChanged()).Returns(true);

      Assert.That(_endPoint.HasBeenTouched, Is.False);

      _endPoint.SetDataFromSubTransaction(source);

      Assert.That(_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetDataFromSubTransaction_DoesNotTouchEndPoint_WhenSourceUntouched_AndTargetUnchanged ()
    {
      var source = RelationEndPointObjectMother.CreateDomainObjectCollectionEndPoint(_customerEndPointID, new[] { _order3 });

      _loadStateMock.Setup(stub => stub.HasChanged()).Returns(false);
      _loadStateMock.Setup(stub => stub.SetDataFromSubTransaction(_endPoint, DomainObjectCollectionEndPointTestHelper.GetLoadState(source)));

      _collectionManagerMock.Setup(stub => stub.HasCollectionReferenceChanged()).Returns(false);

      Assert.That(_endPoint.HasBeenTouched, Is.False);

      _endPoint.SetDataFromSubTransaction(source);

      Assert.That(_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void SetDataFromSubTransaction_InvalidDefinition ()
    {
      var otherID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "OrderItems");
      var source = RelationEndPointObjectMother.CreateDomainObjectCollectionEndPoint(otherID, new DomainObject[0]);
      Assert.That(
          () => _endPoint.SetDataFromSubTransaction(source),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Cannot set this end point's value from "
                  + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems'; the end points "
                  + "do not have the same end point definition.", "source"));
    }

    [Test]
    public void EndPointLoader_LoadEndPointAndGetNewState ()
    {
      var endPointLoader = new DomainObjectCollectionEndPoint.EndPointLoader(_lazyLoaderMock.Object);
      var loadStateFake = new Mock<IDomainObjectCollectionEndPointLoadState>();
      _lazyLoaderMock
          .Setup(mock => mock.LoadLazyCollectionEndPoint(_customerEndPointID))
          .Callback((RelationEndPointID endPointID) => DomainObjectCollectionEndPointTestHelper.SetLoadState(_endPoint, loadStateFake.Object))
          .Verifiable();

      var result = endPointLoader.LoadEndPointAndGetNewState(_endPoint);

      _lazyLoaderMock.Verify();
      Assert.That(result, Is.SameAs(loadStateFake.Object));
    }

    [Test]
    public void EndPointLoader_Serializable ()
    {
      var endPointLoader = new DomainObjectCollectionEndPoint.EndPointLoader(new SerializableLazyLoaderFake());

      var deserializedInstance = FlattenedSerializer.SerializeAndDeserialize(endPointLoader);

      Assert.That(deserializedInstance.LazyLoader, Is.Not.Null);
    }
  }
}
