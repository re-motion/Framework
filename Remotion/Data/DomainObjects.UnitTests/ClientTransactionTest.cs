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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.TypePipe;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class ClientTransactionTest : StandardMappingTest
  {
    private ClientTransaction _transaction;
    private DataManager _dataManager;

    private MockRepository _mockRepository;
    private Dictionary<Enum, object> _fakeApplicationData;
    private IClientTransactionEventBroker _eventBrokerMock;
    private ITransactionHierarchyManager _hierarchyManagerMock;
    private IEnlistedDomainObjectManager _enlistedObjectManagerMock;
    private IInvalidDomainObjectManager _invalidDomainObjectManagerMock;
    private IPersistenceStrategy _persistenceStrategyMock;
    private IDataManager _dataManagerMock;
    private IObjectLifetimeAgent _objectLifetimeAgentMock;
    private IQueryManager _queryManagerMock;
    private ICommitRollbackAgent _commitRollbackAgentMock;

    private TestableClientTransaction _transactionWithMocks;

    private ObjectID _objectID1;
    private DomainObject _fakeDomainObject1;
    private ObjectID _objectID2;
    private DomainObject _fakeDomainObject2;

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = ClientTransaction.CreateRootTransaction();
      _dataManager = ClientTransactionTestHelper.GetDataManager (_transaction);

      _mockRepository = new MockRepository();
      _fakeApplicationData = new Dictionary<Enum, object> ();
      _eventBrokerMock = _mockRepository.StrictMock<IClientTransactionEventBroker> ();
      _hierarchyManagerMock = _mockRepository.StrictMock<ITransactionHierarchyManager> ();
      _enlistedObjectManagerMock = _mockRepository.StrictMock<IEnlistedDomainObjectManager> ();
      _invalidDomainObjectManagerMock = _mockRepository.StrictMock<IInvalidDomainObjectManager> ();
      _persistenceStrategyMock = _mockRepository.StrictMock<IPersistenceStrategy> ();
      _dataManagerMock = _mockRepository.StrictMock<IDataManager> ();
      _objectLifetimeAgentMock = _mockRepository.StrictMock<IObjectLifetimeAgent> ();
      _queryManagerMock = _mockRepository.StrictMock<IQueryManager> ();
      _commitRollbackAgentMock = _mockRepository.StrictMock<ICommitRollbackAgent> ();

      _transactionWithMocks = ClientTransactionObjectMother.CreateWithComponents<TestableClientTransaction> (
          _fakeApplicationData,
          _eventBrokerMock,
          _hierarchyManagerMock,
          _enlistedObjectManagerMock,
          _invalidDomainObjectManagerMock,
          _persistenceStrategyMock,
          _dataManagerMock,
          _objectLifetimeAgentMock,
          _queryManagerMock,
          _commitRollbackAgentMock,
          Enumerable.Empty<IClientTransactionExtension>());
      // Ignore calls made by ctor
      _hierarchyManagerMock.BackToRecord ();
      _eventBrokerMock.BackToRecord ();

      _objectID1 = DomainObjectIDs.Order1;
      _fakeDomainObject1 = DomainObjectMother.CreateFakeObject (_objectID1);
      _objectID2 = DomainObjectIDs.Order2;
      _fakeDomainObject2 = DomainObjectMother.CreateFakeObject (_objectID2);
    }

    [Test]
    public void Initialization_OrderOfFactoryCalls ()
    {
      var componentFactoryMock = _mockRepository.StrictMock<IClientTransactionComponentFactory>();

      var fakeExtension = MockRepository.GenerateStub<IClientTransactionExtension>();
      fakeExtension.Stub (stub => stub.Key).Return ("fake");

      var fakeExtensionCollection = new ClientTransactionExtensionCollection("root");

      ClientTransaction constructedTransaction = null;

      using (_mockRepository.Ordered ())
      {
        componentFactoryMock
            .Expect (mock => mock.CreateApplicationData (Arg<ClientTransaction>.Is.Anything))
            .Return (_fakeApplicationData)
            .WhenCalled (
                mi =>
                {
                  constructedTransaction = (ClientTransaction) mi.Arguments[0];
                  Assert.That (constructedTransaction.ID, Is.Not.EqualTo (Guid.Empty));
                });
        componentFactoryMock
            .Expect (mock => mock.CreateEventBroker (Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction)))
            .Return (_eventBrokerMock)
            .WhenCalled (mi => Assert.That (constructedTransaction.ApplicationData, Is.SameAs (_fakeApplicationData)));
        componentFactoryMock
            .Expect (mock => mock.CreateTransactionHierarchyManager (Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction), Arg.Is (_eventBrokerMock)))
            .Return (_hierarchyManagerMock)
            .WhenCalled (mi => Assert.That (ClientTransactionTestHelper.GetEventBroker (constructedTransaction), Is.SameAs (_eventBrokerMock)));
        _hierarchyManagerMock
            .Expect (mock => mock.InstallListeners (_eventBrokerMock));
        componentFactoryMock
            .Expect (mock => mock.CreateEnlistedObjectManager (Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction)))
            .Return (_enlistedObjectManagerMock)
            .WhenCalled (mi => Assert.That (ClientTransactionTestHelper.GetHierarchyManager (constructedTransaction) == _hierarchyManagerMock));
        componentFactoryMock
            .Expect (mock => mock.CreateInvalidDomainObjectManager (
                Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction), 
                Arg.Is (_eventBrokerMock)))
            .Return (_invalidDomainObjectManagerMock)
            .WhenCalled (
                mi => Assert.That (
                    ClientTransactionTestHelper.GetEnlistedDomainObjectManager (constructedTransaction), Is.SameAs (_enlistedObjectManagerMock)));
        componentFactoryMock
            .Expect (mock => mock.CreatePersistenceStrategy (Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction)))
            .Return (_persistenceStrategyMock)
            .WhenCalled (
                mi => Assert.That (
                    ClientTransactionTestHelper.GetInvalidDomainObjectManager (constructedTransaction), Is.SameAs (_invalidDomainObjectManagerMock)));
        componentFactoryMock
            .Expect (
                mock =>
                mock.CreateDataManager (
                    Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction), 
                    Arg<IClientTransactionEventSink>.Matches (eventSink => eventSink == _eventBrokerMock),
                    Arg.Is (_invalidDomainObjectManagerMock), 
                    Arg.Is (_persistenceStrategyMock),
                    Arg.Is (_hierarchyManagerMock)))
            .Return (_dataManagerMock)
            .WhenCalled (
                mi => Assert.That (ClientTransactionTestHelper.GetPersistenceStrategy (constructedTransaction), Is.SameAs (_persistenceStrategyMock)));
        componentFactoryMock
            .Expect (
                mock =>
                mock.CreateObjectLifetimeAgent (
                    Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction),
                    Arg<IClientTransactionEventSink>.Matches (eventSink => eventSink == _eventBrokerMock),
                    Arg.Is (_invalidDomainObjectManagerMock),
                    Arg.Is (_dataManagerMock),
                    Arg.Is (_enlistedObjectManagerMock), 
                    Arg.Is (_persistenceStrategyMock)))
            .Return (_objectLifetimeAgentMock)
            .WhenCalled (mi => Assert.That (ClientTransactionTestHelper.GetIDataManager (constructedTransaction), Is.SameAs (_dataManagerMock)));
        componentFactoryMock
            .Expect (
                mock =>
                mock.CreateQueryManager (
                    Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction),
                    Arg<IClientTransactionEventSink>.Matches (eventSink => eventSink == _eventBrokerMock), 
                    Arg.Is (_invalidDomainObjectManagerMock), 
                    Arg.Is (_persistenceStrategyMock), 
                    Arg.Is (_dataManagerMock),
                    Arg.Is (_hierarchyManagerMock)))
            .Return (_queryManagerMock)
            .WhenCalled (mi => Assert.That (ClientTransactionTestHelper.GetObjectLifetimeAgent (constructedTransaction), Is.SameAs (_objectLifetimeAgentMock)));
        componentFactoryMock
            .Expect (
                mock =>
                mock.CreateCommitRollbackAgent (
                    Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction),
                    Arg<IClientTransactionEventSink>.Matches (eventSink => eventSink == _eventBrokerMock),
                    Arg.Is (_persistenceStrategyMock),
                    Arg.Is (_dataManagerMock)))
            .Return (_commitRollbackAgentMock)
            .WhenCalled (mi => Assert.That (constructedTransaction.QueryManager, Is.SameAs (_queryManagerMock)));
        componentFactoryMock
            .Expect (mock => mock.CreateExtensions (Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction)))
            .Return (new[] { fakeExtension })
            .WhenCalled (mi => Assert.That (ClientTransactionTestHelper.GetCommitRollbackAgent (constructedTransaction), Is.SameAs (_commitRollbackAgentMock)));
        _eventBrokerMock
            .Stub (mock => mock.Extensions)
            .Return (fakeExtensionCollection);
        
        _hierarchyManagerMock
            .Expect (mock => mock.OnBeforeTransactionInitialize())
            .WhenCalled (mi => Assert.That (fakeExtensionCollection, Has.Member (fakeExtension)));
        _eventBrokerMock
            .Expect (mock => mock.RaiseTransactionInitializeEvent());
      }

      _mockRepository.ReplayAll();

      var result = Activator.CreateInstance (
          typeof (ClientTransaction),
          BindingFlags.NonPublic | BindingFlags.Instance,
          null,
          new[] { componentFactoryMock },
          null);

      _mockRepository.VerifyAll();

      Assert.That (result, Is.SameAs (constructedTransaction));
    }

    [Test]
    public void Extensions ()
    {
      var fakeExtensions = new ClientTransactionExtensionCollection ("root");
      _eventBrokerMock.Stub (mock => mock.Extensions).Return (fakeExtensions);
      _eventBrokerMock.Replay ();

      Assert.That (_transactionWithMocks.Extensions, Is.SameAs (fakeExtensions));
    }

    [Test]
    public void ParentTransaction ()
    {
      var fakeParent = ClientTransactionObjectMother.Create();
      _hierarchyManagerMock.Stub (mock => mock.ParentTransaction).Return (fakeParent);
      _hierarchyManagerMock.Replay ();

      Assert.That (_transactionWithMocks.ParentTransaction, Is.SameAs (fakeParent));
    }

    [Test]
    public void SubTransaction ()
    {
      var fakeSub = ClientTransactionObjectMother.Create ();
      _hierarchyManagerMock.Stub (mock => mock.SubTransaction).Return (fakeSub);
      _hierarchyManagerMock.Replay ();

      Assert.That (_transactionWithMocks.SubTransaction, Is.SameAs (fakeSub));
    }

    [Test]
    public void RootTransaction ()
    {
      var fakeRootTransaction = ClientTransactionObjectMother.Create();
      _hierarchyManagerMock.Stub (mock => mock.TransactionHierarchy.RootTransaction).Return (fakeRootTransaction);
      _hierarchyManagerMock.Replay ();

      Assert.That (_transactionWithMocks.RootTransaction, Is.SameAs (fakeRootTransaction));
    }

    [Test]
    public void LeafTransaction ()
    {
      var fakeLeafTransaction = ClientTransactionObjectMother.Create ();
      _hierarchyManagerMock.Stub (mock => mock.TransactionHierarchy.LeafTransaction).Return (fakeLeafTransaction);
      _hierarchyManagerMock.Replay ();

      Assert.That (_transactionWithMocks.LeafTransaction, Is.SameAs (fakeLeafTransaction));
    }

    [Test]
    public void ActiveTransaction ()
    {
      var fakeActiveTransaction = ClientTransactionObjectMother.Create ();
      _hierarchyManagerMock.Stub (mock => mock.TransactionHierarchy.ActiveTransaction).Return (fakeActiveTransaction);
      _hierarchyManagerMock.Replay ();

      Assert.That (_transactionWithMocks.ActiveTransaction, Is.SameAs (fakeActiveTransaction));
    }

    [Test]
    public void ToString_LeafRootTransaction ()
    {
      var expected = string.Format ("ClientTransaction (root, leaf) {0}", _transaction.ID);
      Assert.That (_transaction.ToString(), Is.EqualTo (expected));
    }

    [Test]
    public void ToString_NonLeafRootTransaction ()
    {
      _transaction.CreateSubTransaction ();
      var expected = string.Format ("ClientTransaction (root, parent) {0}", _transaction.ID);
      Assert.That (_transaction.ToString (), Is.EqualTo (expected));
    }

    [Test]
    public void ToString_LeafSubTransaction ()
    {
      var subTransaction = _transaction.CreateSubTransaction();
      var expected = string.Format ("ClientTransaction (sub, leaf) {0}", subTransaction.ID);
      Assert.That (subTransaction.ToString(), Is.EqualTo (expected));
    }

    [Test]
    public void ToString_NonLeafSubTransaction ()
    {
      var subTransaction = _transaction.CreateSubTransaction ();
      subTransaction.CreateSubTransaction ();
      var expected = string.Format ("ClientTransaction (sub, parent) {0}", subTransaction.ID);
      Assert.That (subTransaction.ToString (), Is.EqualTo (expected));
    }

    [Test]
    public void AddListener ()
    {
      var listenerStub = MockRepository.GenerateStub<IClientTransactionListener>();
      _eventBrokerMock.Expect (mock => mock.AddListener (listenerStub));
      _eventBrokerMock.Replay();

      _transactionWithMocks.AddListener (listenerStub);

      _eventBrokerMock.VerifyAllExpectations();
    }

    [Test]
    public void RemoveListener ()
    {
      var listenerStub = MockRepository.GenerateStub<IClientTransactionListener> ();
      _eventBrokerMock.Expect (mock => mock.RemoveListener (listenerStub));
      _eventBrokerMock.Replay ();

      _transactionWithMocks.RemoveListener (listenerStub);

      _eventBrokerMock.VerifyAllExpectations ();
    }

    [Test]
    public void HasChanged ()
    {
      _commitRollbackAgentMock.Expect (mock => mock.HasDataChanged ()).Return (true).Repeat.Once ();
      _commitRollbackAgentMock.Expect (mock => mock.HasDataChanged ()).Return (false).Repeat.Once ();
      _mockRepository.ReplayAll ();

      var result1 = _transactionWithMocks.HasChanged ();
      var result2 = _transactionWithMocks.HasChanged ();

      _mockRepository.VerifyAll ();

      Assert.That (result1, Is.True);
      Assert.That (result2, Is.False);
    }

    [Test]
    public void Commit ()
    {
      _commitRollbackAgentMock.Expect (mock => mock.CommitData());
      _mockRepository.ReplayAll();

      _transactionWithMocks.Commit();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Rollback ()
    {
      _commitRollbackAgentMock.Expect (mock => mock.RollbackData ());
      _mockRepository.ReplayAll ();

      _transactionWithMocks.Rollback ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetObject ()
    {
      var includeDeleted = BooleanObjectMother.GetRandomBoolean();
      _objectLifetimeAgentMock
          .Expect (mock => mock.GetObject (_objectID1, includeDeleted))
          .Return (_fakeDomainObject1);
      _objectLifetimeAgentMock.Replay();

      var result = ClientTransactionTestHelper.CallGetObject (_transactionWithMocks, _objectID1, includeDeleted);

      _objectLifetimeAgentMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_fakeDomainObject1));
    }

    [Test]
    public void TryGetObject ()
    {
      _objectLifetimeAgentMock
          .Expect (mock => mock.TryGetObject (_objectID1))
          .Return (_fakeDomainObject1);
      _objectLifetimeAgentMock.Replay ();

      var result = ClientTransactionTestHelper.CallTryGetObject (_transactionWithMocks, _objectID1);

      _objectLifetimeAgentMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (_fakeDomainObject1));
    }

    [Test]
    public void GetObjectReference ()
    {
      _objectLifetimeAgentMock
          .Expect (mock => mock.GetObjectReference (_objectID1))
          .Return (_fakeDomainObject1);
      _objectLifetimeAgentMock.Replay ();

      var result = ClientTransactionTestHelper.CallGetObjectReference (_transactionWithMocks, _objectID1);

      _objectLifetimeAgentMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (_fakeDomainObject1));
    }

    [Test]
    public void GetInvalidObjectReference ()
    {
      var invalidObject = DomainObjectMother.CreateObjectInTransaction<Order> (_transaction);
      _transaction.ExecuteInScope (invalidObject.Delete);

      Assert.That (invalidObject.TransactionContext[_transaction].State, Is.EqualTo (StateType.Invalid));

      var invalidObjectReference = ClientTransactionTestHelper.CallGetInvalidObjectReference (_transaction, invalidObject.ID);

      Assert.That (invalidObjectReference, Is.SameAs (invalidObject));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' has "
        + "not been marked invalid.\r\nParameter name: id")]
    public void GetInvalidObjectReference_ThrowsWhenNotInvalid ()
    {
      ClientTransactionTestHelper.CallGetInvalidObjectReference (_transaction, _objectID1);
    }

    [Test]
    public void IsInvalid ()
    {
      var domainObject = DomainObjectMother.CreateObjectInTransaction<Order> (_transaction);
      Assert.That (_transaction.IsInvalid (domainObject.ID), Is.False);

      _transaction.ExecuteInScope (domainObject.Delete);

      Assert.That (_transaction.IsInvalid (domainObject.ID), Is.True);
    }

    [Test]
    public void NewObject ()
    {
      var typeDefinition = GetTypeDefinition (typeof (Order));
      var constructorParameters = ParamList.Create (_fakeDomainObject1);
      _objectLifetimeAgentMock
          .Expect (mock => mock.NewObject (typeDefinition, constructorParameters))
          .Return (_fakeDomainObject1);
      _objectLifetimeAgentMock.Replay ();

      var result = ClientTransactionTestHelper.CallNewObject (_transactionWithMocks, typeof (Order), constructorParameters);

      _objectLifetimeAgentMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (_fakeDomainObject1));
    }

    [Test]
    public void EnsureDataAvailable ()
    {
      _dataManagerMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (_objectID1, true))
          .Return (DataContainerObjectMother.Create());
      _mockRepository.ReplayAll();
      
      _transactionWithMocks.EnsureDataAvailable (_objectID1);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void EnsureDataAvailable_Many ()
    {
      _dataManagerMock
          .Expect (mock => mock.GetDataContainersWithLazyLoad (new[] { _objectID1, _objectID2 }, true))
          .Return (new DataContainer[0]);
      _mockRepository.ReplayAll ();

      _transactionWithMocks.EnsureDataAvailable (new[] { _objectID1, _objectID2 });

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void TryEnsureDataAvailable_True ()
    {
      _dataManagerMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (_objectID1, false))
          .Return (DataContainerObjectMother.Create ());
      _mockRepository.ReplayAll ();

      var result = _transactionWithMocks.TryEnsureDataAvailable (_objectID1);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.True);
    }

    [Test]
    public void TryEnsureDataAvailable_False ()
    {
      _dataManagerMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (_objectID1, false))
          .Return (null);
      _mockRepository.ReplayAll ();

      var result = _transactionWithMocks.TryEnsureDataAvailable (_objectID1);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.False);
    }

    [Test]
    public void TryEnsureDataAvailable_Many_True ()
    {
      _dataManagerMock
          .Expect (mock => mock.GetDataContainersWithLazyLoad (new[] { _objectID1, _objectID2 }, false))
          .Return (new[] { DataContainerObjectMother.Create(), DataContainerObjectMother.Create() });
      _mockRepository.ReplayAll ();

      var result = _transactionWithMocks.TryEnsureDataAvailable (new[] { _objectID1, _objectID2 });

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.True);
    }

    [Test]
    public void TryEnsureDataAvailable_Many_False ()
    {
      _dataManagerMock
          .Expect (mock => mock.GetDataContainersWithLazyLoad (new[] { _objectID1, _objectID2 }, false))
          .Return (new[] { DataContainerObjectMother.Create (), null });
      _mockRepository.ReplayAll ();

      var result = _transactionWithMocks.TryEnsureDataAvailable (new[] { _objectID1, _objectID2 });

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.False);
    }

    [Test]
    public void EnsureDataComplete_EndPoint_Virtual ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Null);

      _transaction.EnsureDataComplete (endPointID);

      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Not.Null);
    }

    [Test]
    public void EnsureDataComplete_EndPoint_Real ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (_objectID1, "Customer");
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Null);

      _transaction.EnsureDataComplete (endPointID);

      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Not.Null);
    }

    [Test]
    public void EnsureDataComplete_EndPoint_Complete ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      _transaction.ExecuteInScope (() => DomainObjectIDs.Customer1.GetObject<Customer> ().Orders);

      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Not.Null);

      _transaction.EnsureDataComplete (endPointID);
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Not.Null);
    }

    [Test]
    public void EnsureDataComplete_EndPoint_Incomplete ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      _transaction.ExecuteInScope (() => DomainObjectIDs.Customer1.GetObject<Customer> ().Orders);
      
      var endPoint = (ICollectionEndPoint) _dataManager.GetRelationEndPointWithoutLoading (endPointID);
      Assert.That (endPoint, Is.Not.Null);
      endPoint.MarkDataIncomplete ();
      Assert.That (endPoint.IsDataComplete, Is.False);

      _transaction.EnsureDataComplete (endPointID);
      Assert.That (endPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void GetEnlistedDomainObjects ()
    {
      var order1 = _transaction.ExecuteInScope(() => Order.NewObject ());
      var order3 = _transaction.ExecuteInScope (() => Order.NewObject ());
      Assert.That (_transaction.GetEnlistedDomainObjects ().ToArray (), Is.EquivalentTo (new[] { order1, order3 }));
    }

    [Test]
    public void EnlistedDomainObjectCount ()
    {
      _transaction.ExecuteInScope (() => Order.NewObject ());
      _transaction.ExecuteInScope (() => Order.NewObject ());
      Assert.That (_transaction.EnlistedDomainObjectCount, Is.EqualTo (2));
    }

    [Test]
    public void IsEnlisted ()
    {
      var order = _transaction.ExecuteInScope (() => Order.NewObject ());
      Assert.That (_transaction.IsEnlisted (order), Is.True);
      Assert.That (ClientTransaction.CreateRootTransaction().IsEnlisted (order), Is.False);
    }

    [Test]
    public void GetEnlistedDomainObject ()
    {
      var order = _transaction.ExecuteInScope (() => Order.NewObject ());
      Assert.That (_transaction.GetEnlistedDomainObject (order.ID), Is.SameAs (order));
    }

    [Test]
    [Obsolete ("TODO 2072 - Remove")]
    public void CopyCollectionEventHandlers ()
    {
      var order = _transaction.ExecuteInScope (() => _objectID1.GetObject<Order> ());
      
      bool orderItemAdded = false;
      _transaction.ExecuteInScope (() => order.OrderItems.Added += delegate { orderItemAdded = true; });

      var otherTransaction = ClientTransaction.CreateRootTransaction ();
      Assert.That (orderItemAdded, Is.False);

      var orderInOtherTransaction = order.GetHandle().GetObject (otherTransaction);
      otherTransaction.CopyCollectionEventHandlers (order, _transaction);

      otherTransaction.ExecuteInScope (() => orderInOtherTransaction.OrderItems.Add (OrderItem.NewObject ()));
      Assert.That (orderItemAdded, Is.True);
    }

    [Test]
    public void CreateSubTransaction_WithDefaultFactory ()
    {
      Assert.That (_transaction.IsWriteable, Is.True);
      
      var subTransaction = _transaction.CreateSubTransaction ();
      Assert.That (subTransaction, Is.TypeOf (typeof (ClientTransaction)));
      Assert.That (subTransaction.ParentTransaction, Is.SameAs (_transaction));
      Assert.That (_transaction.IsWriteable, Is.False);
      Assert.That (_transaction.SubTransaction, Is.SameAs (subTransaction));

      Assert.That (subTransaction.Extensions, Is.Empty);
      Assert.That (subTransaction.ApplicationData, Is.SameAs (_transaction.ApplicationData));
      
      var enlistedObjectManager = ClientTransactionTestHelper.GetEnlistedDomainObjectManager (subTransaction);
      Assert.That (enlistedObjectManager, Is.SameAs (ClientTransactionTestHelper.GetEnlistedDomainObjectManager (_transaction)));

      var invalidDomainObjectManager = ClientTransactionTestHelper.GetInvalidDomainObjectManager (subTransaction);
      Assert.That (invalidDomainObjectManager, Is.TypeOf (typeof (InvalidDomainObjectManager)));
      var persistenceStrategy = ClientTransactionTestHelper.GetPersistenceStrategy (subTransaction);
      Assert.That (persistenceStrategy, Is.TypeOf (typeof (SubPersistenceStrategy)));
    }

    [Test]
    public void CreateSubTransaction_WithCustomFactory ()
    {
      ClientTransaction fakeSubTransaction = ClientTransactionObjectMother.Create();
      Func<ClientTransaction, ClientTransaction> actualFactoryFunc = null;
      _hierarchyManagerMock
          .Expect (mock => mock.CreateSubTransaction (Arg<Func<ClientTransaction, ClientTransaction>>.Is.Anything))
          .WhenCalled (mi => actualFactoryFunc = (Func<ClientTransaction, ClientTransaction>) mi.Arguments[0])
          .Return (fakeSubTransaction);

      ClientTransaction fakeSubTransaction2 = ClientTransactionObjectMother.Create ();
      Func<ClientTransaction, IInvalidDomainObjectManager, IEnlistedDomainObjectManager, ITransactionHierarchyManager, IClientTransactionEventSink, ClientTransaction> factoryMock =
        (tx, invalidDomainObjectManager, enlistedDomainObjectManager, hierarchyManager, eventSink) =>
        {
          Assert.That (tx, Is.SameAs (_transactionWithMocks));
          Assert.That (invalidDomainObjectManager, Is.SameAs (_invalidDomainObjectManagerMock));
          Assert.That (enlistedDomainObjectManager, Is.SameAs (_enlistedObjectManagerMock));
          Assert.That (hierarchyManager, Is.SameAs (_hierarchyManagerMock));
          Assert.That (eventSink, Is.SameAs (_eventBrokerMock));
          return fakeSubTransaction2;
        };

      _mockRepository.ReplayAll();
      
      var result = _transactionWithMocks.CreateSubTransaction (factoryMock);

      _hierarchyManagerMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeSubTransaction));

      // Check the actualfactoryFunc that was passed from CreateSubTransaction to _hierarchyManagerMock by invoking it.
      var actualFactoryFuncResult = actualFactoryFunc (_transactionWithMocks);
      Assert.That (actualFactoryFuncResult, Is.SameAs (fakeSubTransaction2));
    }

    [Test]
    public void Discard ()
    {
      using (_mockRepository.Ordered ())
      {
        _eventBrokerMock.RaiseTransactionDiscardEvent();
        _hierarchyManagerMock.Expect (mock => mock.OnTransactionDiscard());
        _eventBrokerMock.Expect (mock => mock.AddListener (Arg<InvalidatedTransactionListener>.Is.TypeOf));
      }
      _mockRepository.ReplayAll();

      Assert.That (_transactionWithMocks.IsDiscarded, Is.False);

      _transactionWithMocks.Discard();

      _mockRepository.VerifyAll();
      Assert.That (_transactionWithMocks.IsDiscarded, Is.True);
    }

    [Test]
    public void Discard_Twice ()
    {
      var parentTransaction = ClientTransaction.CreateRootTransaction ();
      ClientTransactionTestHelper.SetIsWriteable (parentTransaction, false);
      ClientTransactionTestHelper.SetSubTransaction (parentTransaction, _transactionWithMocks);

      _eventBrokerMock.Stub (stub => stub.RaiseTransactionDiscardEvent());
      _hierarchyManagerMock.Stub (mock => mock.OnTransactionDiscard ());
      
      _transactionWithMocks.Discard();

      _mockRepository.BackToRecordAll();
      _mockRepository.ReplayAll ();

      _transactionWithMocks.Discard ();

      _eventBrokerMock.AssertWasNotCalled (mock => mock.RaiseTransactionDiscardEvent());
      _hierarchyManagerMock.AssertWasNotCalled (mock => mock.OnTransactionDiscard ());
    }

    [Test]
    public void EnterScope_ActiveTransaction_ActivatesTransaction ()
    {
      var hierarchyMock = MockRepository.GenerateStrictMock<IClientTransactionHierarchy> ();
      _hierarchyManagerMock.Stub (stub => stub.TransactionHierarchy).Return (hierarchyMock);
      _hierarchyManagerMock.Replay ();

      hierarchyMock.Stub (stub => stub.ActiveTransaction).Return (_transactionWithMocks);

      var activatedScopeStub = MockRepository.GenerateStub<IDisposable> ();
      hierarchyMock.Expect (mock => mock.ActivateTransaction (_transactionWithMocks)).Return (activatedScopeStub);

      _transactionWithMocks.EnterScope (AutoRollbackBehavior.Rollback);

      hierarchyMock.VerifyAllExpectations ();
    }

    [Test]
    public void EnterScope_InactiveTransaction_SetsTransactionActiveAndReturnsAScope_ThatUndoesActivation ()
    {
      var hierarchyMock = MockRepository.GenerateStrictMock<IClientTransactionHierarchy> ();
      _hierarchyManagerMock.Stub (stub => stub.TransactionHierarchy).Return (hierarchyMock);
      _hierarchyManagerMock.Replay();

      var fakeSub = ClientTransactionObjectMother.Create ();
      hierarchyMock.Stub (stub => stub.ActiveTransaction).Return (fakeSub);
      
      var activatedScopeMock = MockRepository.GenerateStrictMock<IDisposable>();
      hierarchyMock.Expect (mock => mock.ActivateTransaction (_transactionWithMocks)).Return (activatedScopeMock);

      var result = _transactionWithMocks.EnterScope (AutoRollbackBehavior.None);

      hierarchyMock.VerifyAllExpectations();

      // Check that result scope integrates and disposes activation scope for undo.

      activatedScopeMock.Expect (mock => mock.Dispose());

      result.Leave();

      activatedScopeMock.VerifyAllExpectations();
    }

    [Test]
    public void EnterNonDiscardingScope_SetsTransactionCurrent ()
    {
      Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));

      var scope = _transaction.EnterNonDiscardingScope ();

      Assert.That (ClientTransaction.Current, Is.SameAs (_transaction));

      scope.Leave();

      Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));
      Assert.That (_transaction.IsDiscarded, Is.False);
    }

    [Test]
    public void EnterNonDiscardingScope_ActivatesInactiveTransaction ()
    {
      Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));
      using (ClientTransactionTestHelper.MakeInactive (_transaction))
      {
        var scope = _transaction.EnterNonDiscardingScope();

        Assert.That (ClientTransaction.Current, Is.SameAs (_transaction));
        Assert.That (_transaction.ActiveTransaction, Is.SameAs (_transaction));

        scope.Leave();

        Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));
        Assert.That (_transaction.ActiveTransaction, Is.Not.SameAs (_transaction));
      }
    }

    [Test]
    public void EnterDiscardingScope_SetsTransactionCurrent_AndDiscardsTransactionUponLeave ()
    {
      Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));

      var scope = _transaction.EnterDiscardingScope ();

      Assert.That (ClientTransaction.Current, Is.SameAs (_transaction));

      scope.Leave ();

      Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));
      Assert.That (_transaction.IsDiscarded, Is.True);
    }

    [Test]
    public void EnterDiscardingScope_ActivatesInactiveTransaction ()
    {
      Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));
      using (ClientTransactionTestHelper.MakeInactive (_transaction))
      {
        var scope = _transaction.EnterDiscardingScope();

        Assert.That (ClientTransaction.Current, Is.SameAs (_transaction));
        Assert.That (_transaction.ActiveTransaction, Is.SameAs (_transaction));

        scope.Leave();

        Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));
        Assert.That (_transaction.ActiveTransaction, Is.Not.SameAs (_transaction));
      }
    }
    
    [Test]
    public void GetRelatedObject ()
    {
      Order order = _transaction.ExecuteInScope (() => _objectID1.GetObject<Order> ());

      var endPointID = RelationEndPointID.Resolve (order, o => o.OrderTicket);
      _transaction.ExecuteInScope (() => order.OrderTicket = DomainObjectIDs.OrderTicket2.GetObject<OrderTicket> ());
      
      DomainObject orderTicket = ClientTransactionTestHelper.CallGetRelatedObject (_transaction, endPointID);

      Assert.That (orderTicket, Is.Not.Null);
      Assert.That (orderTicket, Is.SameAs (_transaction.ExecuteInScope (() => DomainObjectIDs.OrderTicket2.GetObject<OrderTicket> ())));
    }

    [Test]
    public void GetRelatedObject_LoadsOriginatingObject ()
    {
      Order order = _transaction.ExecuteInScope (() => _objectID1.GetObjectReference<Order> ());
      var endPointID = RelationEndPointID.Resolve (order, o => o.OrderTicket);

      ClientTransactionTestHelper.CallGetRelatedObject (_transaction, endPointID);

      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void GetRelatedObject_Deleted ()
    {
      Location location = _transaction.ExecuteInScope (() => DomainObjectIDs.Location1.GetObject<Location>());

      var client = _transaction.ExecuteInScope (() => location.Client);
      _transaction.ExecuteInScope (() => location.Client.Delete());

      var endPointID = RelationEndPointID.Create (location.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client");
      var result = ClientTransactionTestHelper.CallGetRelatedObject (_transaction, endPointID);

      Assert.That (result, Is.SameAs (client));
      Assert.That (_transaction.ExecuteInScope (() => result.State), Is.EqualTo (StateType.Deleted));
    }

    [Test]
    public void GetRelatedObject_Invalid ()
    {
      Location location = _transaction.ExecuteInScope (() => DomainObjectIDs.Location1.GetObject<Location>());
      Client newClient = _transaction.ExecuteInScope (() => Client.NewObject ());
      _transaction.ExecuteInScope (() => location.Client = newClient);
      _transaction.ExecuteInScope (() => location.Client.Delete ());

      var endPointID = RelationEndPointID.Create (location.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client");
      var result = ClientTransactionTestHelper.CallGetRelatedObject (_transaction, endPointID);
      Assert.That (result, Is.SameAs (newClient));
      Assert.That (_transaction.ExecuteInScope (() => result.State), Is.EqualTo (StateType.Invalid));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The given end-point ID does not denote a related object (cardinality one).\r\nParameter name: relationEndPointID")]
    public void GetRelatedObject_WrongCardinality ()
    {
      Order order = _transaction.ExecuteInScope (() => _objectID1.GetObject<Order> ());

      ClientTransactionTestHelper.CallGetRelatedObject (
          _transaction, 
          RelationEndPointID.Create (order.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"));
    }

    [Test]
    public void GetOriginalRelatedObject ()
    {
      Order order = _transaction.ExecuteInScope (() => _objectID1.GetObject<Order> ());
      var endPointID = RelationEndPointID.Create (order.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");

      _transaction.ExecuteInScope (() => order.OrderTicket = DomainObjectIDs.OrderTicket2.GetObject<OrderTicket> ());

      DomainObject orderTicket = ClientTransactionTestHelper.CallGetOriginalRelatedObject (_transaction, endPointID);

      Assert.That (orderTicket, Is.Not.Null);
      Assert.That (orderTicket, Is.SameAs (_transaction.ExecuteInScope (() => DomainObjectIDs.OrderTicket1.GetObject<OrderTicket> ())));
    }

    [Test]
    public void GetOriginalRelatedObject_LoadsOriginatingObject ()
    {
      Order order = _transaction.ExecuteInScope (() => _objectID1.GetObjectReference<Order> ());
      var endPointID = RelationEndPointID.Resolve (order, o => o.OrderTicket);

      ClientTransactionTestHelper.CallGetOriginalRelatedObject (_transaction, endPointID);

      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The given end-point ID does not denote a related object (cardinality one).\r\nParameter name: relationEndPointID")]
    public void GetOriginalRelatedObject_WrongCardinality ()
    {
      Order order = _transaction.ExecuteInScope (() => _objectID1.GetObject<Order> ());

      ClientTransactionTestHelper.CallGetOriginalRelatedObject (
          _transaction,
          RelationEndPointID.Create (order.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"));
    }

    [Test]
    public void GetRelatedObjects ()
    {
      Order order = _transaction.ExecuteInScope (() => _objectID1.GetObject<Order> ());

      var endPointID = RelationEndPointID.Resolve (order, o => o.OrderItems);
      var endPoint = ((ICollectionEndPoint) ClientTransactionTestHelper.GetDataManager (_transaction).GetRelationEndPointWithLazyLoad (endPointID));
      endPoint.CreateAddCommand (_transaction.ExecuteInScope (() => DomainObjectIDs.OrderItem3.GetObject<OrderItem>())).ExpandToAllRelatedObjects ().Perform ();

      var orderItems = ClientTransactionTestHelper.CallGetRelatedObjects (_transaction, endPointID);

      Assert.That (orderItems, Is.TypeOf<ObjectList<OrderItem>>());
      Assert.That (
          orderItems,
          Is.EquivalentTo (
              new[]
              {
                  _transaction.ExecuteInScope (() => DomainObjectIDs.OrderItem1.GetObject<OrderItem>()),
                  _transaction.ExecuteInScope (() => DomainObjectIDs.OrderItem2.GetObject<OrderItem>()),
                  _transaction.ExecuteInScope (() => DomainObjectIDs.OrderItem3.GetObject<OrderItem>())
              }));
    }

    [Test]
    public void GetRelatedObjects_LoadsOriginatingObject ()
    {
      Order order = _transaction.ExecuteInScope (() => _objectID1.GetObjectReference<Order> ());
      var endPointID = RelationEndPointID.Resolve (order, o => o.OrderItems);

      ClientTransactionTestHelper.CallGetRelatedObjects (_transaction, endPointID);

      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The given end-point ID does not denote a related object collection (cardinality many).\r\nParameter name: relationEndPointID")]
    public void GetRelatedObjects_WrongCardinality ()
    {
      Order order = _transaction.ExecuteInScope (() => _objectID1.GetObject<Order> ());

      ClientTransactionTestHelper.CallGetRelatedObjects (
          _transaction,
          RelationEndPointID.Create (order.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));
    }

    [Test]
    public void GetOriginalRelatedObjects ()
    {
      Order order = _transaction.ExecuteInScope (() => _objectID1.GetObject<Order> ());

      var endPointID = RelationEndPointID.Create (order.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");
      var endPoint = ((ICollectionEndPoint) ClientTransactionTestHelper.GetDataManager (_transaction).GetRelationEndPointWithLazyLoad (endPointID));
      endPoint.CreateAddCommand (_transaction.ExecuteInScope (() => DomainObjectIDs.OrderItem3.GetObject<OrderItem>())).ExpandToAllRelatedObjects ().Perform ();

      var orderItems = ClientTransactionTestHelper.CallGetOriginalRelatedObjects (_transaction, endPointID);

      Assert.That (orderItems, Is.TypeOf<ObjectList<OrderItem>> ());
      Assert.That (
        orderItems,
        Is.EquivalentTo (
            new[]
              {
                  _transaction.ExecuteInScope (() => DomainObjectIDs.OrderItem1.GetObject<OrderItem>()),
                  _transaction.ExecuteInScope (() => DomainObjectIDs.OrderItem2.GetObject<OrderItem>())
              }));
    }

    public void GetOriginalRelatedObjects_LoadsOriginatingObject ()
    {
      Order order = _transaction.ExecuteInScope (() => _objectID1.GetObjectReference<Order> ());
      var endPointID = RelationEndPointID.Resolve (order, o => o.OrderItems);

      ClientTransactionTestHelper.CallGetOriginalRelatedObjects (_transaction, endPointID);

      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The given end-point ID does not denote a related object collection (cardinality many).\r\nParameter name: relationEndPointID")]
    public void GetOriginalRelatedObjects_WrongCardinality ()
    {
      Order order = _transaction.ExecuteInScope (() => _objectID1.GetObject<Order> ());

      ClientTransactionTestHelper.CallGetOriginalRelatedObjects (
          _transaction,
          RelationEndPointID.Create (order.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));
    }

    [Test]
    public void GetObjects ()
    {
      _objectLifetimeAgentMock
          .Expect (mock => mock.GetObjects<DomainObject> (new[] { _objectID1, _objectID2 }))
          .Return (new[] { _fakeDomainObject1, _fakeDomainObject2 });
      _objectLifetimeAgentMock.Replay ();

      var result = ClientTransactionTestHelper.CallGetObjects<DomainObject> (_transactionWithMocks, _objectID1, _objectID2);

      _objectLifetimeAgentMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (new[] { _fakeDomainObject1, _fakeDomainObject2 }));
    }

    [Test]
    public void TryGetObjects ()
    {
      _objectLifetimeAgentMock
          .Expect (mock => mock.TryGetObjects<DomainObject> (new[] { _objectID1, _objectID2 }))
          .Return (new[] { _fakeDomainObject1, _fakeDomainObject2 });
      _objectLifetimeAgentMock.Replay ();

      var result = ClientTransactionTestHelper.CallTryGetObjects<DomainObject> (_transactionWithMocks, _objectID1, _objectID2);

      _objectLifetimeAgentMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (new[] { _fakeDomainObject1, _fakeDomainObject2 }));
    }
    
    [Test]
    public void Serialization ()
    {
      var clientTransaction = ClientTransaction.CreateRootTransaction();
      var subTransaction = clientTransaction.CreateSubTransaction ();

      var deserializedClientTransaction = Serializer.SerializeAndDeserialize (clientTransaction);

      Assert.That (deserializedClientTransaction, Is.Not.Null);
      Assert.That (deserializedClientTransaction.ParentTransaction, Is.Null);
      Assert.That (deserializedClientTransaction.ApplicationData, Is.Not.Null);
      Assert.That (deserializedClientTransaction.Extensions, Is.Not.Null);
      Assert.That (ClientTransactionTestHelper.GetEventBroker (deserializedClientTransaction), Is.Not.Null);
      Assert.That (ClientTransactionTestHelper.GetEnlistedDomainObjectManager (deserializedClientTransaction), Is.Not.Null);
      Assert.That (ClientTransactionTestHelper.GetInvalidDomainObjectManager (deserializedClientTransaction), Is.Not.Null);
      Assert.That (ClientTransactionTestHelper.GetIDataManager (deserializedClientTransaction), Is.Not.Null);
      Assert.That (ClientTransactionTestHelper.GetPersistenceStrategy (deserializedClientTransaction), Is.Not.Null);
      Assert.That (deserializedClientTransaction.QueryManager, Is.Not.Null);
      Assert.That (ClientTransactionTestHelper.GetCommitRollbackAgent (deserializedClientTransaction), Is.Not.Null);
      Assert.That (deserializedClientTransaction.SubTransaction, Is.Not.Null);
      Assert.That (deserializedClientTransaction.IsDiscarded, Is.False);
      Assert.That (deserializedClientTransaction.ID, Is.EqualTo (clientTransaction.ID));

      var deserializedSubTransaction = Serializer.SerializeAndDeserialize (subTransaction);

      Assert.That (deserializedSubTransaction.ParentTransaction, Is.Not.Null);
      Assert.That (deserializedSubTransaction.SubTransaction, Is.Null);

      clientTransaction.Discard();

      var deserializedDiscardedTransaction = Serializer.SerializeAndDeserialize (clientTransaction);

      Assert.That (deserializedDiscardedTransaction.IsDiscarded, Is.True);
    }
  }
}