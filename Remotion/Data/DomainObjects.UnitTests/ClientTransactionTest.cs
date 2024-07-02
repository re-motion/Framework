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
using Moq;
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
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class ClientTransactionTest : StandardMappingTest
  {
    private ClientTransaction _transaction;
    private DataManager _dataManager;
    private Dictionary<Enum, object> _fakeApplicationData;
    private Mock<IClientTransactionEventBroker> _eventBrokerMock;
    private Mock<ITransactionHierarchyManager> _hierarchyManagerMock;
    private Mock<IEnlistedDomainObjectManager> _enlistedObjectManagerMock;
    private Mock<IInvalidDomainObjectManager> _invalidDomainObjectManagerMock;
    private Mock<IPersistenceStrategy> _persistenceStrategyMock;
    private Mock<IDataManager> _dataManagerMock;
    private Mock<IObjectLifetimeAgent> _objectLifetimeAgentMock;
    private Mock<IQueryManager> _queryManagerMock;
    private Mock<ICommitRollbackAgent> _commitRollbackAgentMock;

    private TestableClientTransaction _transactionWithMocks;

    private ObjectID _objectID1;
    private DomainObject _fakeDomainObject1;
    private ObjectID _objectID2;
    private DomainObject _fakeDomainObject2;

    public override void SetUp ()
    {
      base.SetUp();

      _transaction = ClientTransaction.CreateRootTransaction();
      _dataManager = ClientTransactionTestHelper.GetDataManager(_transaction);

      _fakeApplicationData = new Dictionary<Enum, object>();
      _eventBrokerMock = new Mock<IClientTransactionEventBroker>(MockBehavior.Strict);
      _hierarchyManagerMock = new Mock<ITransactionHierarchyManager>(MockBehavior.Strict);
      _enlistedObjectManagerMock = new Mock<IEnlistedDomainObjectManager>(MockBehavior.Strict);
      _invalidDomainObjectManagerMock = new Mock<IInvalidDomainObjectManager>(MockBehavior.Strict);
      _persistenceStrategyMock = new Mock<IPersistenceStrategy>(MockBehavior.Strict);
      _dataManagerMock = new Mock<IDataManager>(MockBehavior.Strict);
      _objectLifetimeAgentMock = new Mock<IObjectLifetimeAgent>(MockBehavior.Strict);
      _queryManagerMock = new Mock<IQueryManager>(MockBehavior.Strict);
      _commitRollbackAgentMock = new Mock<ICommitRollbackAgent>(MockBehavior.Strict);

      _hierarchyManagerMock.Setup(_ => _.InstallListeners(It.IsAny<IClientTransactionEventBroker>()));
      _hierarchyManagerMock.Setup(_ => _.OnBeforeTransactionInitialize());
      _eventBrokerMock.Setup(_ => _.RaiseTransactionInitializeEvent());

      _transactionWithMocks = ClientTransactionObjectMother.CreateWithComponents<TestableClientTransaction>(
          _fakeApplicationData,
          _eventBrokerMock.Object,
          _hierarchyManagerMock.Object,
          _enlistedObjectManagerMock.Object,
          _invalidDomainObjectManagerMock.Object,
          _persistenceStrategyMock.Object,
          _dataManagerMock.Object,
          _objectLifetimeAgentMock.Object,
          _queryManagerMock.Object,
          _commitRollbackAgentMock.Object,
          Enumerable.Empty<IClientTransactionExtension>());

      // Ignore calls made by ctor
      _eventBrokerMock.Reset();
      _hierarchyManagerMock.Reset();
      _enlistedObjectManagerMock.Reset();
      _invalidDomainObjectManagerMock.Reset();
      _persistenceStrategyMock.Reset();
      _dataManagerMock.Reset();
      _objectLifetimeAgentMock.Reset();
      _queryManagerMock.Reset();
      _commitRollbackAgentMock.Reset();

      _objectID1 = DomainObjectIDs.Order1;
      _fakeDomainObject1 = DomainObjectMother.CreateFakeObject(_objectID1);
      _objectID2 = DomainObjectIDs.Order2;
      _fakeDomainObject2 = DomainObjectMother.CreateFakeObject(_objectID2);
    }

    [Test]
    public void Initialization_OrderOfFactoryCalls ()
    {
      var componentFactoryMock = new Mock<IClientTransactionComponentFactory>(MockBehavior.Strict);

      var fakeExtension = new Mock<IClientTransactionExtension>();
      fakeExtension.Setup(stub => stub.Key).Returns("fake");

      var fakeExtensionCollection = new ClientTransactionExtensionCollection("root");

      ClientTransaction actualConstructedTransaction = null;

      var sequence = new VerifiableSequence();
      componentFactoryMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CreateApplicationData(It.IsAny<ClientTransaction>()))
          .Returns(_fakeApplicationData)
          .Callback(
              (ClientTransaction constructedTransaction) =>
              {
                actualConstructedTransaction = constructedTransaction;
                Assert.That(actualConstructedTransaction.ID, Is.Not.EqualTo(Guid.Empty));
              })
          .Verifiable();
      componentFactoryMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CreateEventBroker(It.Is<ClientTransaction>(tx => tx == actualConstructedTransaction)))
          .Returns(_eventBrokerMock.Object)
          .Callback(
              (ClientTransaction constructedTransaction) =>
                  Assert.That(constructedTransaction.ApplicationData, Is.SameAs(_fakeApplicationData)))
          .Verifiable();

      componentFactoryMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.CreateTransactionHierarchyManager(
                  It.Is<ClientTransaction>(tx => tx == actualConstructedTransaction),
                  _eventBrokerMock.Object))
          .Returns(_hierarchyManagerMock.Object)
          .Callback(
              (ClientTransaction constructedTransaction, IClientTransactionEventSink _) =>
                  Assert.That(ClientTransactionTestHelper.GetEventBroker(constructedTransaction), Is.SameAs(_eventBrokerMock.Object)))
          .Verifiable();

      _hierarchyManagerMock
          .InVerifiableSequence(sequence)
            .Setup(mock => mock.InstallListeners(_eventBrokerMock.Object))
            .Verifiable();
      componentFactoryMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CreateEnlistedObjectManager(It.Is<ClientTransaction>(tx => tx == actualConstructedTransaction)))
          .Returns(_enlistedObjectManagerMock.Object)
          .Callback(
              (ClientTransaction constructedTransaction) =>
                  Assert.That(ClientTransactionTestHelper.GetHierarchyManager(constructedTransaction), Is.SameAs(_hierarchyManagerMock.Object)))
          .Verifiable();

      componentFactoryMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.CreateInvalidDomainObjectManager(
                  It.Is<ClientTransaction>(tx => tx == actualConstructedTransaction),
                  _eventBrokerMock.Object))
          .Returns(_invalidDomainObjectManagerMock.Object)
          .Callback(
              (ClientTransaction constructedTransaction, IClientTransactionEventSink _) =>
                  Assert.That(ClientTransactionTestHelper.GetEnlistedDomainObjectManager(constructedTransaction), Is.SameAs(_enlistedObjectManagerMock.Object)))
          .Verifiable();
      componentFactoryMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CreatePersistenceStrategy(It.Is<ClientTransaction>(tx => tx == actualConstructedTransaction)))
          .Returns(_persistenceStrategyMock.Object)
          .Callback(
              (ClientTransaction constructedTransaction) =>
                  Assert.That(ClientTransactionTestHelper.GetInvalidDomainObjectManager(constructedTransaction), Is.SameAs(_invalidDomainObjectManagerMock.Object)))
          .Verifiable();

      componentFactoryMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.CreateDataManager(
                  It.Is<ClientTransaction>(tx => tx == actualConstructedTransaction),
                  It.Is<IClientTransactionEventSink>(eventSink => eventSink == _eventBrokerMock.Object),
                  _invalidDomainObjectManagerMock.Object,
                  _persistenceStrategyMock.Object,
                  _hierarchyManagerMock.Object))
          .Returns(_dataManagerMock.Object)
          .Callback(
              (
                  ClientTransaction constructedTransaction,
                  IClientTransactionEventSink _,
                  IInvalidDomainObjectManager _,
                  IPersistenceStrategy _,
                  ITransactionHierarchyManager _) =>
                  Assert.That(ClientTransactionTestHelper.GetPersistenceStrategy(constructedTransaction), Is.SameAs(_persistenceStrategyMock.Object)))
          .Verifiable();

      componentFactoryMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.CreateObjectLifetimeAgent(
                  It.Is<ClientTransaction>(tx => tx == actualConstructedTransaction),
                  It.Is<IClientTransactionEventSink>(eventSink => eventSink == _eventBrokerMock.Object),
                  _invalidDomainObjectManagerMock.Object,
                  _dataManagerMock.Object,
                  _enlistedObjectManagerMock.Object,
                  _persistenceStrategyMock.Object))
          .Returns(_objectLifetimeAgentMock.Object)
          .Callback(
              (
                  ClientTransaction constructedTransaction,
                  IClientTransactionEventSink _,
                  IInvalidDomainObjectManager _,
                  IDataManager _,
                  IEnlistedDomainObjectManager _,
                  IPersistenceStrategy _) =>
                  Assert.That(ClientTransactionTestHelper.GetIDataManager(constructedTransaction), Is.SameAs(_dataManagerMock.Object)))
          .Verifiable();

      componentFactoryMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.CreateQueryManager(
                  It.Is<ClientTransaction>(tx => tx == actualConstructedTransaction),
                  It.Is<IClientTransactionEventSink>(eventSink => eventSink == _eventBrokerMock.Object),
                  _invalidDomainObjectManagerMock.Object,
                  _persistenceStrategyMock.Object,
                  _dataManagerMock.Object,
                  _hierarchyManagerMock.Object))
          .Returns(_queryManagerMock.Object)
          .Callback(
              (
                  ClientTransaction constructedTransaction,
                  IClientTransactionEventSink _,
                  IInvalidDomainObjectManager _,
                  IPersistenceStrategy _,
                  IDataManager _,
                  ITransactionHierarchyManager _) =>
                  Assert.That(ClientTransactionTestHelper.GetObjectLifetimeAgent(constructedTransaction), Is.SameAs(_objectLifetimeAgentMock.Object)))
          .Verifiable();

      componentFactoryMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.CreateCommitRollbackAgent(
                  It.Is<ClientTransaction>(tx => tx == actualConstructedTransaction),
                  It.Is<IClientTransactionEventSink>(eventSink => eventSink == _eventBrokerMock.Object),
                  _persistenceStrategyMock.Object,
                  _dataManagerMock.Object))
          .Returns(_commitRollbackAgentMock.Object)
          .Callback(
              (
                  ClientTransaction constructedTransaction,
                  IClientTransactionEventSink _,
                  IPersistenceStrategy _,
                  IDataManager _) =>
                  Assert.That(constructedTransaction.QueryManager, Is.SameAs(_queryManagerMock.Object)))
          .Verifiable();
      componentFactoryMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CreateExtensions(It.Is<ClientTransaction>(tx => tx == actualConstructedTransaction)))
          .Returns(new[] { fakeExtension.Object })
          .Callback(
              (ClientTransaction constructedTransaction) =>
                  Assert.That(ClientTransactionTestHelper.GetCommitRollbackAgent(constructedTransaction), Is.SameAs(_commitRollbackAgentMock.Object)))
          .Verifiable();
      _hierarchyManagerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.OnBeforeTransactionInitialize())
          .Callback(() => Assert.That(fakeExtensionCollection, Has.Member(fakeExtension.Object)))
          .Verifiable();
      _eventBrokerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RaiseTransactionInitializeEvent())
          .Verifiable();
      _eventBrokerMock
          .Setup(mock => mock.Extensions)
          .Returns(fakeExtensionCollection);
      _hierarchyManagerMock
          .SetupGet(mock => mock.ParentTransaction)
          .Returns((ClientTransaction)null);
      _hierarchyManagerMock
          .SetupGet(mock => mock.SubTransaction)
          .Returns((ClientTransaction)null);

      var result = Activator.CreateInstance(
          typeof(ClientTransaction),
          BindingFlags.NonPublic | BindingFlags.Instance,
          null,
          new[] { componentFactoryMock.Object },
          null);

      _eventBrokerMock.Verify();
      _hierarchyManagerMock.Verify();
      _enlistedObjectManagerMock.Verify();
      _invalidDomainObjectManagerMock.Verify();
      _persistenceStrategyMock.Verify();
      _dataManagerMock.Verify();
      _objectLifetimeAgentMock.Verify();
      _queryManagerMock.Verify();
      _commitRollbackAgentMock.Verify();
      componentFactoryMock.Verify();
      sequence.Verify();

      Assert.That(result, Is.SameAs(actualConstructedTransaction));
    }

    [Test]
    public void Extensions ()
    {
      var fakeExtensions = new ClientTransactionExtensionCollection("root");
      _eventBrokerMock.Setup(mock => mock.Extensions).Returns(fakeExtensions);

      Assert.That(_transactionWithMocks.Extensions, Is.SameAs(fakeExtensions));
    }

    [Test]
    public void ParentTransaction ()
    {
      var fakeParent = ClientTransactionObjectMother.Create();
      _hierarchyManagerMock.Setup(mock => mock.ParentTransaction).Returns(fakeParent);

      Assert.That(_transactionWithMocks.ParentTransaction, Is.SameAs(fakeParent));
    }

    [Test]
    public void SubTransaction ()
    {
      var fakeSub = ClientTransactionObjectMother.Create();
      _hierarchyManagerMock.Setup(mock => mock.SubTransaction).Returns(fakeSub);

      Assert.That(_transactionWithMocks.SubTransaction, Is.SameAs(fakeSub));
    }

    [Test]
    public void RootTransaction ()
    {
      var fakeRootTransaction = ClientTransactionObjectMother.Create();
      _hierarchyManagerMock.Setup(mock => mock.TransactionHierarchy.RootTransaction).Returns(fakeRootTransaction);

      Assert.That(_transactionWithMocks.RootTransaction, Is.SameAs(fakeRootTransaction));
    }

    [Test]
    public void LeafTransaction ()
    {
      var fakeLeafTransaction = ClientTransactionObjectMother.Create();
      _hierarchyManagerMock.Setup(mock => mock.TransactionHierarchy.LeafTransaction).Returns(fakeLeafTransaction);

      Assert.That(_transactionWithMocks.LeafTransaction, Is.SameAs(fakeLeafTransaction));
    }

    [Test]
    public void ActiveTransaction ()
    {
      var fakeActiveTransaction = ClientTransactionObjectMother.Create();
      _hierarchyManagerMock.Setup(mock => mock.TransactionHierarchy.ActiveTransaction).Returns(fakeActiveTransaction);

      Assert.That(_transactionWithMocks.ActiveTransaction, Is.SameAs(fakeActiveTransaction));
    }

    [Test]
    public void ToString_LeafRootTransaction ()
    {
      var expected = string.Format("ClientTransaction (root, leaf) {0}", _transaction.ID);
      Assert.That(_transaction.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public void ToString_NonLeafRootTransaction ()
    {
      _transaction.CreateSubTransaction();
      var expected = string.Format("ClientTransaction (root, parent) {0}", _transaction.ID);
      Assert.That(_transaction.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public void ToString_LeafSubTransaction ()
    {
      var subTransaction = _transaction.CreateSubTransaction();
      var expected = string.Format("ClientTransaction (sub, leaf) {0}", subTransaction.ID);
      Assert.That(subTransaction.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public void ToString_NonLeafSubTransaction ()
    {
      var subTransaction = _transaction.CreateSubTransaction();
      subTransaction.CreateSubTransaction();
      var expected = string.Format("ClientTransaction (sub, parent) {0}", subTransaction.ID);
      Assert.That(subTransaction.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public void AddListener ()
    {
      var listenerStub = new Mock<IClientTransactionListener>();
      _eventBrokerMock.Setup(mock => mock.AddListener(listenerStub.Object)).Verifiable();

      _transactionWithMocks.AddListener(listenerStub.Object);

      _eventBrokerMock.Verify();
    }

    [Test]
    public void RemoveListener ()
    {
      var listenerStub = new Mock<IClientTransactionListener>();
      _eventBrokerMock.Setup(mock => mock.RemoveListener(listenerStub.Object)).Verifiable();

      _transactionWithMocks.RemoveListener(listenerStub.Object);

      _eventBrokerMock.Verify();
    }

    [Test]
    public void HasObjectsWithState ()
    {
      var expectedResult = BooleanObjectMother.GetRandomBoolean();
      Predicate<DomainObjectState> predicate = _ => true;
      _commitRollbackAgentMock
          .Setup(mock => mock.HasData(It.Is<Predicate<DomainObjectState>>(v=> object.ReferenceEquals(v, predicate))))
          .Returns(expectedResult);

      var result = _transactionWithMocks.HasObjectsWithState(predicate);
      Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void Commit ()
    {
      _commitRollbackAgentMock.Setup(mock => mock.CommitData()).Verifiable();

      _transactionWithMocks.Commit();

      _eventBrokerMock.Verify();
      _hierarchyManagerMock.Verify();
      _enlistedObjectManagerMock.Verify();
      _invalidDomainObjectManagerMock.Verify();
      _persistenceStrategyMock.Verify();
      _dataManagerMock.Verify();
      _objectLifetimeAgentMock.Verify();
      _queryManagerMock.Verify();
      _commitRollbackAgentMock.Verify();
    }

    [Test]
    public void Rollback ()
    {
      _commitRollbackAgentMock.Setup(mock => mock.RollbackData()).Verifiable();

      _transactionWithMocks.Rollback();

      _eventBrokerMock.Verify();
      _hierarchyManagerMock.Verify();
      _enlistedObjectManagerMock.Verify();
      _invalidDomainObjectManagerMock.Verify();
      _persistenceStrategyMock.Verify();
      _dataManagerMock.Verify();
      _objectLifetimeAgentMock.Verify();
      _queryManagerMock.Verify();
      _commitRollbackAgentMock.Verify();
    }

    [Test]
    public void GetObject ()
    {
      var includeDeleted = BooleanObjectMother.GetRandomBoolean();
      _objectLifetimeAgentMock
          .Setup(mock => mock.GetObject(_objectID1, includeDeleted))
          .Returns(_fakeDomainObject1)
          .Verifiable();

      var result = ClientTransactionTestHelper.CallGetObject(_transactionWithMocks, _objectID1, includeDeleted);

      _objectLifetimeAgentMock.Verify();
      Assert.That(result, Is.SameAs(_fakeDomainObject1));
    }

    [Test]
    public void TryGetObject ()
    {
      _objectLifetimeAgentMock
          .Setup(mock => mock.TryGetObject(_objectID1))
          .Returns(_fakeDomainObject1)
          .Verifiable();

      var result = ClientTransactionTestHelper.CallTryGetObject(_transactionWithMocks, _objectID1);

      _objectLifetimeAgentMock.Verify();
      Assert.That(result, Is.SameAs(_fakeDomainObject1));
    }

    [Test]
    public void GetObjectReference ()
    {
      _objectLifetimeAgentMock
          .Setup(mock => mock.GetObjectReference(_objectID1))
          .Returns(_fakeDomainObject1)
          .Verifiable();

      var result = ClientTransactionTestHelper.CallGetObjectReference(_transactionWithMocks, _objectID1);

      _objectLifetimeAgentMock.Verify();
      Assert.That(result, Is.SameAs(_fakeDomainObject1));
    }

    [Test]
    public void GetInvalidObjectReference ()
    {
      var invalidObject = DomainObjectMother.CreateObjectInTransaction<Order>(_transaction);
      _transaction.ExecuteInScope(invalidObject.Delete);

      Assert.That(invalidObject.TransactionContext[_transaction].State.IsInvalid, Is.True);

      var invalidObjectReference = ClientTransactionTestHelper.CallGetInvalidObjectReference(_transaction, invalidObject.ID);

      Assert.That(invalidObjectReference, Is.SameAs(invalidObject));
    }

    [Test]
    public void GetInvalidObjectReference_ThrowsWhenNotInvalid ()
    {
      Assert.That(
          () => ClientTransactionTestHelper.CallGetInvalidObjectReference(_transaction, _objectID1),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' has "
                  + "not been marked invalid.",
                  "id"));
    }

    [Test]
    public void IsInvalid ()
    {
      var domainObject = DomainObjectMother.CreateObjectInTransaction<Order>(_transaction);
      Assert.That(_transaction.IsInvalid(domainObject.ID), Is.False);

      _transaction.ExecuteInScope(domainObject.Delete);

      Assert.That(_transaction.IsInvalid(domainObject.ID), Is.True);
    }

    [Test]
    public void NewObject ()
    {
      var typeDefinition = GetTypeDefinition(typeof(Order));
      var constructorParameters = ParamList.Create(_fakeDomainObject1);
      _objectLifetimeAgentMock
          .Setup(mock => mock.NewObject(typeDefinition, constructorParameters))
          .Returns(_fakeDomainObject1)
          .Verifiable();

      var result = ClientTransactionTestHelper.CallNewObject(_transactionWithMocks, typeof(Order), constructorParameters);

      _objectLifetimeAgentMock.Verify();
      Assert.That(result, Is.SameAs(_fakeDomainObject1));
    }

    [Test]
    public void EnsureDataAvailable ()
    {
      _dataManagerMock
          .Setup(mock => mock.GetDataContainerWithLazyLoad(_objectID1, true))
          .Returns(DataContainerObjectMother.Create())
          .Verifiable();

      _transactionWithMocks.EnsureDataAvailable(_objectID1);

      _eventBrokerMock.Verify();
      _hierarchyManagerMock.Verify();
      _enlistedObjectManagerMock.Verify();
      _invalidDomainObjectManagerMock.Verify();
      _persistenceStrategyMock.Verify();
      _dataManagerMock.Verify();
      _objectLifetimeAgentMock.Verify();
      _queryManagerMock.Verify();
      _commitRollbackAgentMock.Verify();
    }

    [Test]
    public void EnsureDataAvailable_Many ()
    {
      _dataManagerMock
          .Setup(mock => mock.GetDataContainersWithLazyLoad(new[] { _objectID1, _objectID2 }, true))
          .Returns(new DataContainer[0])
          .Verifiable();

      _transactionWithMocks.EnsureDataAvailable(new[] { _objectID1, _objectID2 });

      _eventBrokerMock.Verify();
      _hierarchyManagerMock.Verify();
      _enlistedObjectManagerMock.Verify();
      _invalidDomainObjectManagerMock.Verify();
      _persistenceStrategyMock.Verify();
      _dataManagerMock.Verify();
      _objectLifetimeAgentMock.Verify();
      _queryManagerMock.Verify();
      _commitRollbackAgentMock.Verify();
    }

    [Test]
    public void TryEnsureDataAvailable_True ()
    {
      _dataManagerMock
          .Setup(mock => mock.GetDataContainerWithLazyLoad(_objectID1, false))
          .Returns(DataContainerObjectMother.Create())
          .Verifiable();

      var result = _transactionWithMocks.TryEnsureDataAvailable(_objectID1);

      _eventBrokerMock.Verify();
      _hierarchyManagerMock.Verify();
      _enlistedObjectManagerMock.Verify();
      _invalidDomainObjectManagerMock.Verify();
      _persistenceStrategyMock.Verify();
      _dataManagerMock.Verify();
      _objectLifetimeAgentMock.Verify();
      _queryManagerMock.Verify();
      _commitRollbackAgentMock.Verify();
      Assert.That(result, Is.True);
    }

    [Test]
    public void TryEnsureDataAvailable_False ()
    {
      _dataManagerMock
          .Setup(mock => mock.GetDataContainerWithLazyLoad(_objectID1, false))
          .Returns((DataContainer)null)
          .Verifiable();

      var result = _transactionWithMocks.TryEnsureDataAvailable(_objectID1);

      _eventBrokerMock.Verify();
      _hierarchyManagerMock.Verify();
      _enlistedObjectManagerMock.Verify();
      _invalidDomainObjectManagerMock.Verify();
      _persistenceStrategyMock.Verify();
      _dataManagerMock.Verify();
      _objectLifetimeAgentMock.Verify();
      _queryManagerMock.Verify();
      _commitRollbackAgentMock.Verify();
      Assert.That(result, Is.False);
    }

    [Test]
    public void TryEnsureDataAvailable_Many_True ()
    {
      _dataManagerMock
          .Setup(mock => mock.GetDataContainersWithLazyLoad(new[] { _objectID1, _objectID2 }, false))
          .Returns(new[] { DataContainerObjectMother.Create(), DataContainerObjectMother.Create() })
          .Verifiable();

      var result = _transactionWithMocks.TryEnsureDataAvailable(new[] { _objectID1, _objectID2 });

      _eventBrokerMock.Verify();
      _hierarchyManagerMock.Verify();
      _enlistedObjectManagerMock.Verify();
      _invalidDomainObjectManagerMock.Verify();
      _persistenceStrategyMock.Verify();
      _dataManagerMock.Verify();
      _objectLifetimeAgentMock.Verify();
      _queryManagerMock.Verify();
      _commitRollbackAgentMock.Verify();
      Assert.That(result, Is.True);
    }

    [Test]
    public void TryEnsureDataAvailable_Many_False ()
    {
      _dataManagerMock
          .Setup(mock => mock.GetDataContainersWithLazyLoad(new[] { _objectID1, _objectID2 }, false))
          .Returns(new[] { DataContainerObjectMother.Create(), null })
          .Verifiable();

      var result = _transactionWithMocks.TryEnsureDataAvailable(new[] { _objectID1, _objectID2 });

      _eventBrokerMock.Verify();
      _hierarchyManagerMock.Verify();
      _enlistedObjectManagerMock.Verify();
      _invalidDomainObjectManagerMock.Verify();
      _persistenceStrategyMock.Verify();
      _dataManagerMock.Verify();
      _objectLifetimeAgentMock.Verify();
      _queryManagerMock.Verify();
      _commitRollbackAgentMock.Verify();
      Assert.That(result, Is.False);
    }

    [Test]
    public void EnsureDataComplete_EndPoint_Virtual ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Customer1, "Orders");
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(endPointID), Is.Null);

      _transaction.EnsureDataComplete(endPointID);

      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(endPointID), Is.Not.Null);
    }

    [Test]
    public void EnsureDataComplete_EndPoint_Real ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(_objectID1, "Customer");
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(endPointID), Is.Null);

      _transaction.EnsureDataComplete(endPointID);

      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(endPointID), Is.Not.Null);
    }

    [Test]
    public void EnsureDataComplete_EndPoint_Complete ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Customer1, "Orders");
      _transaction.ExecuteInScope(() => DomainObjectIDs.Customer1.GetObject<Customer>().Orders);

      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(endPointID), Is.Not.Null);

      _transaction.EnsureDataComplete(endPointID);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(endPointID), Is.Not.Null);
    }

    [Test]
    public void EnsureDataComplete_EndPoint_Incomplete ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Customer1, "Orders");
      _transaction.ExecuteInScope(() => DomainObjectIDs.Customer1.GetObject<Customer>().Orders);

      var endPoint = (ICollectionEndPoint<ICollectionEndPointData>)_dataManager.GetRelationEndPointWithoutLoading(endPointID);
      Assert.That(endPoint, Is.Not.Null);
      endPoint.MarkDataIncomplete();
      Assert.That(endPoint.IsDataComplete, Is.False);

      _transaction.EnsureDataComplete(endPointID);
      Assert.That(endPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void GetEnlistedDomainObjects ()
    {
      var order1 = _transaction.ExecuteInScope(() => Order.NewObject());
      var order3 = _transaction.ExecuteInScope(() => Order.NewObject());
      Assert.That(_transaction.GetEnlistedDomainObjects().ToArray(), Is.EquivalentTo(new[] { order1, order3 }));
    }

    [Test]
    public void EnlistedDomainObjectCount ()
    {
      _transaction.ExecuteInScope(() => Order.NewObject());
      _transaction.ExecuteInScope(() => Order.NewObject());
      Assert.That(_transaction.EnlistedDomainObjectCount, Is.EqualTo(2));
    }

    [Test]
    public void IsEnlisted ()
    {
      var order = _transaction.ExecuteInScope(() => Order.NewObject());
      Assert.That(_transaction.IsEnlisted(order), Is.True);
      Assert.That(ClientTransaction.CreateRootTransaction().IsEnlisted(order), Is.False);
    }

    [Test]
    public void GetEnlistedDomainObject ()
    {
      var order = _transaction.ExecuteInScope(() => Order.NewObject());
      Assert.That(_transaction.GetEnlistedDomainObject(order.ID), Is.SameAs(order));
    }

    [Test]
    public void CreateSubTransaction_WithDefaultFactory ()
    {
      Assert.That(_transaction.IsWriteable, Is.True);

      var subTransaction = _transaction.CreateSubTransaction();
      Assert.That(subTransaction, Is.TypeOf(typeof(ClientTransaction)));
      Assert.That(subTransaction.ParentTransaction, Is.SameAs(_transaction));
      Assert.That(_transaction.IsWriteable, Is.False);
      Assert.That(_transaction.SubTransaction, Is.SameAs(subTransaction));

      Assert.That(subTransaction.Extensions, Is.Empty);
      Assert.That(subTransaction.ApplicationData, Is.SameAs(_transaction.ApplicationData));

      var enlistedObjectManager = ClientTransactionTestHelper.GetEnlistedDomainObjectManager(subTransaction);
      Assert.That(enlistedObjectManager, Is.SameAs(ClientTransactionTestHelper.GetEnlistedDomainObjectManager(_transaction)));

      var invalidDomainObjectManager = ClientTransactionTestHelper.GetInvalidDomainObjectManager(subTransaction);
      Assert.That(invalidDomainObjectManager, Is.TypeOf(typeof(InvalidDomainObjectManager)));
      var persistenceStrategy = ClientTransactionTestHelper.GetPersistenceStrategy(subTransaction);
      Assert.That(persistenceStrategy, Is.TypeOf(typeof(SubPersistenceStrategy)));
    }

    [Test]
    public void CreateSubTransaction_WithCustomFactory ()
    {
      ClientTransaction fakeSubTransaction = ClientTransactionObjectMother.Create();
      Func<ClientTransaction, ClientTransaction> actualFactoryFunc = null;
      _hierarchyManagerMock
          .Setup(mock => mock.CreateSubTransaction(It.IsAny<Func<ClientTransaction, ClientTransaction>>()))
          .Callback((Func<ClientTransaction, ClientTransaction> subTransactionFactory) => actualFactoryFunc = subTransactionFactory)
          .Returns(fakeSubTransaction)
          .Verifiable();

      ClientTransaction fakeSubTransaction2 = ClientTransactionObjectMother.Create();
      Func<ClientTransaction, IInvalidDomainObjectManager, IEnlistedDomainObjectManager, ITransactionHierarchyManager, IClientTransactionEventSink, ClientTransaction> factoryMock =
          (tx, invalidDomainObjectManager, enlistedDomainObjectManager, hierarchyManager, eventSink) =>
          {
            Assert.That(tx, Is.SameAs(_transactionWithMocks));
            Assert.That(invalidDomainObjectManager, Is.SameAs(_invalidDomainObjectManagerMock.Object));
            Assert.That(enlistedDomainObjectManager, Is.SameAs(_enlistedObjectManagerMock.Object));
            Assert.That(hierarchyManager, Is.SameAs(_hierarchyManagerMock.Object));
            Assert.That(eventSink, Is.SameAs(_eventBrokerMock.Object));
            return fakeSubTransaction2;
          };

      var result = _transactionWithMocks.CreateSubTransaction(factoryMock);

      _hierarchyManagerMock.Verify();
      Assert.That(result, Is.SameAs(fakeSubTransaction));

      // Check the actualfactoryFunc that was passed from CreateSubTransaction to _hierarchyManagerMock by invoking it.
      var actualFactoryFuncResult = actualFactoryFunc(_transactionWithMocks);
      Assert.That(actualFactoryFuncResult, Is.SameAs(fakeSubTransaction2));
    }

    [Test]
    public void Discard ()
    {
      var sequence = new VerifiableSequence();
      _eventBrokerMock.InVerifiableSequence(sequence).Setup(mock => mock.RaiseTransactionDiscardEvent()).Verifiable();
      _hierarchyManagerMock.InVerifiableSequence(sequence).Setup(mock => mock.OnTransactionDiscard()).Verifiable();
      _eventBrokerMock.InVerifiableSequence(sequence).Setup(mock => mock.AddListener(It.IsNotNull<InvalidatedTransactionListener>())).Verifiable();

      Assert.That(_transactionWithMocks.IsDiscarded, Is.False);

      _transactionWithMocks.Discard();

      _eventBrokerMock.Verify();
      _hierarchyManagerMock.Verify();
      _enlistedObjectManagerMock.Verify();
      _invalidDomainObjectManagerMock.Verify();
      _persistenceStrategyMock.Verify();
      _dataManagerMock.Verify();
      _objectLifetimeAgentMock.Verify();
      _queryManagerMock.Verify();
      _commitRollbackAgentMock.Verify();
      sequence.Verify();
      Assert.That(_transactionWithMocks.IsDiscarded, Is.True);
    }

    [Test]
    public void Discard_Twice ()
    {
      var parentTransaction = ClientTransaction.CreateRootTransaction();
      ClientTransactionTestHelper.SetIsWriteable(parentTransaction, false);
      ClientTransactionTestHelper.SetSubTransaction(parentTransaction, _transactionWithMocks);

      _eventBrokerMock.Setup(stub => stub.AddListener(It.IsAny<InvalidatedTransactionListener>()));
      _eventBrokerMock.Setup(stub => stub.RaiseTransactionDiscardEvent());
      _hierarchyManagerMock.Setup(stub => stub.OnTransactionDiscard());

      _transactionWithMocks.Discard();

      _eventBrokerMock.Reset();
      _hierarchyManagerMock.Reset();

      _transactionWithMocks.Discard();

      _eventBrokerMock.Verify(mock => mock.RaiseTransactionDiscardEvent(), Times.Never());
      _hierarchyManagerMock.Verify(mock => mock.OnTransactionDiscard(), Times.Never());
    }

    [Test]
    public void EnterScope_ActiveTransaction_ActivatesTransaction ()
    {
      var hierarchyMock = new Mock<IClientTransactionHierarchy>(MockBehavior.Strict);
      _hierarchyManagerMock.Setup(stub => stub.TransactionHierarchy).Returns(hierarchyMock.Object);

      hierarchyMock.Setup(stub => stub.ActiveTransaction).Returns(_transactionWithMocks);

      var activatedScopeStub = new Mock<IDisposable>();
      hierarchyMock.Setup(mock => mock.ActivateTransaction(_transactionWithMocks)).Returns(activatedScopeStub.Object).Verifiable();

      _transactionWithMocks.EnterScope(AutoRollbackBehavior.Rollback);

      hierarchyMock.Verify();
    }

    [Test]
    public void EnterScope_InactiveTransaction_SetsTransactionActiveAndReturnsAScope_ThatUndoesActivation ()
    {
      var hierarchyMock = new Mock<IClientTransactionHierarchy>(MockBehavior.Strict);
      _hierarchyManagerMock.Setup(stub => stub.TransactionHierarchy).Returns(hierarchyMock.Object);

      var fakeSub = ClientTransactionObjectMother.Create();
      hierarchyMock.Setup(stub => stub.ActiveTransaction).Returns(fakeSub);

      var activatedScopeMock = new Mock<IDisposable>(MockBehavior.Strict);
      hierarchyMock.Setup(mock => mock.ActivateTransaction(_transactionWithMocks)).Returns(activatedScopeMock.Object).Verifiable();

      var result = _transactionWithMocks.EnterScope(AutoRollbackBehavior.None);

      hierarchyMock.Verify();

      // Check that result scope integrates and disposes activation scope for undo.

      activatedScopeMock.Setup(mock => mock.Dispose()).Verifiable();

      result.Leave();

      activatedScopeMock.Verify();
    }

    [Test]
    public void EnterNonDiscardingScope_SetsTransactionCurrent ()
    {
      Assert.That(ClientTransaction.Current, Is.Not.SameAs(_transaction));

      var scope = _transaction.EnterNonDiscardingScope();

      Assert.That(ClientTransaction.Current, Is.SameAs(_transaction));

      scope.Leave();

      Assert.That(ClientTransaction.Current, Is.Not.SameAs(_transaction));
      Assert.That(_transaction.IsDiscarded, Is.False);
    }

    [Test]
    public void EnterNonDiscardingScope_ActivatesInactiveTransaction ()
    {
      Assert.That(ClientTransaction.Current, Is.Not.SameAs(_transaction));
      using (ClientTransactionTestHelper.MakeInactive(_transaction))
      {
        var scope = _transaction.EnterNonDiscardingScope();

        Assert.That(ClientTransaction.Current, Is.SameAs(_transaction));
        Assert.That(_transaction.ActiveTransaction, Is.SameAs(_transaction));

        scope.Leave();

        Assert.That(ClientTransaction.Current, Is.Not.SameAs(_transaction));
        Assert.That(_transaction.ActiveTransaction, Is.Not.SameAs(_transaction));
      }
    }

    [Test]
    public void EnterDiscardingScope_SetsTransactionCurrent_AndDiscardsTransactionUponLeave ()
    {
      Assert.That(ClientTransaction.Current, Is.Not.SameAs(_transaction));

      var scope = _transaction.EnterDiscardingScope();

      Assert.That(ClientTransaction.Current, Is.SameAs(_transaction));

      scope.Leave();

      Assert.That(ClientTransaction.Current, Is.Not.SameAs(_transaction));
      Assert.That(_transaction.IsDiscarded, Is.True);
    }

    [Test]
    public void EnterDiscardingScope_ActivatesInactiveTransaction ()
    {
      Assert.That(ClientTransaction.Current, Is.Not.SameAs(_transaction));
      using (ClientTransactionTestHelper.MakeInactive(_transaction))
      {
        var scope = _transaction.EnterDiscardingScope();

        Assert.That(ClientTransaction.Current, Is.SameAs(_transaction));
        Assert.That(_transaction.ActiveTransaction, Is.SameAs(_transaction));

        scope.Leave();

        Assert.That(ClientTransaction.Current, Is.Not.SameAs(_transaction));
        Assert.That(_transaction.ActiveTransaction, Is.Not.SameAs(_transaction));
      }
    }

    [Test]
    public void GetRelatedObject ()
    {
      Order order = _transaction.ExecuteInScope(() => _objectID1.GetObject<Order>());

      var endPointID = RelationEndPointID.Resolve(order, o => o.OrderTicket);
      _transaction.ExecuteInScope(() => order.OrderTicket = DomainObjectIDs.OrderTicket2.GetObject<OrderTicket>());

      DomainObject orderTicket = ClientTransactionTestHelper.CallGetRelatedObject(_transaction, endPointID);

      Assert.That(orderTicket, Is.Not.Null);
      Assert.That(orderTicket, Is.SameAs(_transaction.ExecuteInScope(() => DomainObjectIDs.OrderTicket2.GetObject<OrderTicket>())));
    }

    [Test]
    public void GetRelatedObject_LoadsOriginatingObject ()
    {
      Order order = _transaction.ExecuteInScope(() => _objectID1.GetObjectReference<Order>());
      var endPointID = RelationEndPointID.Resolve(order, o => o.OrderTicket);

      ClientTransactionTestHelper.CallGetRelatedObject(_transaction, endPointID);

      Assert.That(order.State.IsUnchanged, Is.True);
    }

    [Test]
    public void GetRelatedObject_Deleted ()
    {
      Location location = _transaction.ExecuteInScope(() => DomainObjectIDs.Location1.GetObject<Location>());

      var client = _transaction.ExecuteInScope(() => location.Client);
      _transaction.ExecuteInScope(() => location.Client.Delete());

      var endPointID = RelationEndPointID.Create(location.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client");
      var result = ClientTransactionTestHelper.CallGetRelatedObject(_transaction, endPointID);

      Assert.That(result, Is.SameAs(client));
      Assert.That(_transaction.ExecuteInScope(() => result.State).IsDeleted, Is.True);
    }

    [Test]
    public void GetRelatedObject_Invalid ()
    {
      Location location = _transaction.ExecuteInScope(() => DomainObjectIDs.Location1.GetObject<Location>());
      Client newClient = _transaction.ExecuteInScope(() => Client.NewObject());
      _transaction.ExecuteInScope(() => location.Client = newClient);
      _transaction.ExecuteInScope(() => location.Client.Delete());

      var endPointID = RelationEndPointID.Create(location.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client");
      var result = ClientTransactionTestHelper.CallGetRelatedObject(_transaction, endPointID);
      Assert.That(result, Is.SameAs(newClient));
      Assert.That(_transaction.ExecuteInScope(() => result.State).IsInvalid, Is.True);
    }

    [Test]
    public void GetRelatedObject_WrongCardinality ()
    {
      Order order = _transaction.ExecuteInScope(() => _objectID1.GetObject<Order>());
      Assert.That(
          () => ClientTransactionTestHelper.CallGetRelatedObject(
              _transaction,
              RelationEndPointID.Create(order.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems")),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The given end-point ID does not denote a related object (cardinality one).",
                  "relationEndPointID"));
    }

    [Test]
    public void GetOriginalRelatedObject ()
    {
      Order order = _transaction.ExecuteInScope(() => _objectID1.GetObject<Order>());
      var endPointID = RelationEndPointID.Create(order.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");

      _transaction.ExecuteInScope(() => order.OrderTicket = DomainObjectIDs.OrderTicket2.GetObject<OrderTicket>());

      DomainObject orderTicket = ClientTransactionTestHelper.CallGetOriginalRelatedObject(_transaction, endPointID);

      Assert.That(orderTicket, Is.Not.Null);
      Assert.That(orderTicket, Is.SameAs(_transaction.ExecuteInScope(() => DomainObjectIDs.OrderTicket1.GetObject<OrderTicket>())));
    }

    [Test]
    public void GetOriginalRelatedObject_LoadsOriginatingObject ()
    {
      Order order = _transaction.ExecuteInScope(() => _objectID1.GetObjectReference<Order>());
      var endPointID = RelationEndPointID.Resolve(order, o => o.OrderTicket);

      ClientTransactionTestHelper.CallGetOriginalRelatedObject(_transaction, endPointID);

      Assert.That(order.State.IsUnchanged, Is.True);
    }

    [Test]
    public void GetOriginalRelatedObject_WrongCardinality ()
    {
      Order order = _transaction.ExecuteInScope(() => _objectID1.GetObject<Order>());
      Assert.That(
          () => ClientTransactionTestHelper.CallGetOriginalRelatedObject(
              _transaction,
              RelationEndPointID.Create(order.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems")),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The given end-point ID does not denote a related object (cardinality one).",
                  "relationEndPointID"));
    }

    [Test]
    public void GetRelatedObjects ()
    {
      Order order = _transaction.ExecuteInScope(() => _objectID1.GetObject<Order>());

      var endPointID = RelationEndPointID.Resolve(order, o => o.OrderItems);
      var endPoint = ((ICollectionEndPoint<ICollectionEndPointData>)ClientTransactionTestHelper.GetDataManager(_transaction).GetRelationEndPointWithLazyLoad(endPointID));
      endPoint.CreateAddCommand(_transaction.ExecuteInScope(() => DomainObjectIDs.OrderItem3.GetObject<OrderItem>())).ExpandToAllRelatedObjects().Perform();

      var orderItems = ClientTransactionTestHelper.CallGetRelatedObjects(_transaction, endPointID);

      Assert.That(orderItems, Is.TypeOf<ObjectList<OrderItem>>());
      Assert.That(
          orderItems,
          Is.EquivalentTo(
              new[]
              {
                  _transaction.ExecuteInScope(() => DomainObjectIDs.OrderItem1.GetObject<OrderItem>()),
                  _transaction.ExecuteInScope(() => DomainObjectIDs.OrderItem2.GetObject<OrderItem>()),
                  _transaction.ExecuteInScope(() => DomainObjectIDs.OrderItem3.GetObject<OrderItem>())
              }));
    }

    [Test]
    public void GetRelatedObjects_LoadsOriginatingObject ()
    {
      Order order = _transaction.ExecuteInScope(() => _objectID1.GetObjectReference<Order>());
      var endPointID = RelationEndPointID.Resolve(order, o => o.OrderItems);

      ClientTransactionTestHelper.CallGetRelatedObjects(_transaction, endPointID);

      Assert.That(order.State.IsUnchanged, Is.True);
    }

    [Test]
    public void GetRelatedObjects_WrongCardinality ()
    {
      Order order = _transaction.ExecuteInScope(() => _objectID1.GetObject<Order>());
      Assert.That(
          () => ClientTransactionTestHelper.CallGetRelatedObjects(
              _transaction,
              RelationEndPointID.Create(order.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket")),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The given end-point ID does not denote a related object collection (cardinality many).",
                  "relationEndPointID"));
    }

    [Test]
    public void GetOriginalRelatedObjects ()
    {
      Order order = _transaction.ExecuteInScope(() => _objectID1.GetObject<Order>());

      var endPointID = RelationEndPointID.Create(order.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");
      var endPoint = ((ICollectionEndPoint<ICollectionEndPointData>)ClientTransactionTestHelper.GetDataManager(_transaction).GetRelationEndPointWithLazyLoad(endPointID));
      endPoint.CreateAddCommand(_transaction.ExecuteInScope(() => DomainObjectIDs.OrderItem3.GetObject<OrderItem>())).ExpandToAllRelatedObjects().Perform();

      var orderItems = ClientTransactionTestHelper.CallGetOriginalRelatedObjects(_transaction, endPointID);

      Assert.That(orderItems, Is.TypeOf<ObjectList<OrderItem>>());
      Assert.That(
          orderItems,
          Is.EquivalentTo(
              new[]
              {
                  _transaction.ExecuteInScope(() => DomainObjectIDs.OrderItem1.GetObject<OrderItem>()),
                  _transaction.ExecuteInScope(() => DomainObjectIDs.OrderItem2.GetObject<OrderItem>())
              }));
    }

    public void GetOriginalRelatedObjects_LoadsOriginatingObject ()
    {
      Order order = _transaction.ExecuteInScope(() => _objectID1.GetObjectReference<Order>());
      var endPointID = RelationEndPointID.Resolve(order, o => o.OrderItems);

      ClientTransactionTestHelper.CallGetOriginalRelatedObjects(_transaction, endPointID);

      Assert.That(order.State.IsUnchanged, Is.True);
    }

    [Test]
    public void GetOriginalRelatedObjects_WrongCardinality ()
    {
      Order order = _transaction.ExecuteInScope(() => _objectID1.GetObject<Order>());
      Assert.That(
          () => ClientTransactionTestHelper.CallGetOriginalRelatedObjects(
              _transaction,
              RelationEndPointID.Create(order.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket")),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The given end-point ID does not denote a related object collection (cardinality many).",
                  "relationEndPointID"));
    }

    [Test]
    public void GetObjects ()
    {
      _objectLifetimeAgentMock
          .Setup(mock => mock.GetObjects<DomainObject>(new[] { _objectID1, _objectID2 }))
          .Returns(new[] { _fakeDomainObject1, _fakeDomainObject2 })
          .Verifiable();

      var result = ClientTransactionTestHelper.CallGetObjects<DomainObject>(_transactionWithMocks, _objectID1, _objectID2);

      _objectLifetimeAgentMock.Verify();
      Assert.That(result, Is.EqualTo(new[] { _fakeDomainObject1, _fakeDomainObject2 }));
    }

    [Test]
    public void TryGetObjects ()
    {
      _objectLifetimeAgentMock
          .Setup(mock => mock.TryGetObjects<DomainObject>(new[] { _objectID1, _objectID2 }))
          .Returns(new[] { _fakeDomainObject1, _fakeDomainObject2 })
          .Verifiable();

      var result = ClientTransactionTestHelper.CallTryGetObjects<DomainObject>(_transactionWithMocks, _objectID1, _objectID2);

      _objectLifetimeAgentMock.Verify();
      Assert.That(result, Is.EqualTo(new[] { _fakeDomainObject1, _fakeDomainObject2 }));
    }
  }
}
