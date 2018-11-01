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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure
{
  [TestFixture]
  public class SubClientTransactionComponentFactoryTest : StandardMappingTest
  {
    private TestableClientTransaction _parentTransaction;
    private IInvalidDomainObjectManager _parentInvalidDomainObjectManagerStub;
    private IEnlistedDomainObjectManager _parentEnlistedDomainObjectManagerStub;
    private ITransactionHierarchyManager _parentTransactionHierarchyManagerStub;
    private IClientTransactionEventSink _parentEventSink;

    private SubClientTransactionComponentFactory _factory;
    
    private TestableClientTransaction _fakeConstructedTransaction;

    public override void SetUp ()
    {
      base.SetUp ();

      _parentTransaction = new TestableClientTransaction ();
      _parentInvalidDomainObjectManagerStub = MockRepository.GenerateStub<IInvalidDomainObjectManager> ();
      _parentEnlistedDomainObjectManagerStub = MockRepository.GenerateStub<IEnlistedDomainObjectManager> ();
      _parentTransactionHierarchyManagerStub = MockRepository.GenerateStub<ITransactionHierarchyManager> ();
      _parentTransactionHierarchyManagerStub
          .Stub (stub => stub.TransactionHierarchy)
          .Return (MockRepository.GenerateStub<IClientTransactionHierarchy>());
      _parentEventSink = MockRepository.GenerateStub<IClientTransactionEventSink>();
    
      _factory = SubClientTransactionComponentFactory.Create (
          _parentTransaction,
          _parentInvalidDomainObjectManagerStub,
          _parentEnlistedDomainObjectManagerStub,
          _parentTransactionHierarchyManagerStub,
          _parentEventSink);
      
      _fakeConstructedTransaction = new TestableClientTransaction ();
    }

    [Test]
    public void CreateTransactionHierarchyManager ()
    {
      var eventSink = MockRepository.GenerateStub<IClientTransactionEventSink>();

      var transactionHierarchyManager = _factory.CreateTransactionHierarchyManager (_fakeConstructedTransaction, eventSink);
      
      Assert.That (transactionHierarchyManager, Is.TypeOf<TransactionHierarchyManager> ());
      Assert.That (((TransactionHierarchyManager) transactionHierarchyManager).ThisTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (((TransactionHierarchyManager) transactionHierarchyManager).ThisEventSink, Is.SameAs (eventSink));
      Assert.That (((TransactionHierarchyManager) transactionHierarchyManager).ParentTransaction, Is.SameAs (_parentTransaction));
      Assert.That (((TransactionHierarchyManager) transactionHierarchyManager).ParentHierarchyManager, Is.SameAs (_parentTransactionHierarchyManagerStub));
      Assert.That (((TransactionHierarchyManager) transactionHierarchyManager).ParentEventSink, Is.SameAs (_parentEventSink));
    }

    [Test]
    public void CreateApplicationData ()
    {
      Assert.That (_factory.CreateApplicationData (_fakeConstructedTransaction), Is.SameAs (_parentTransaction.ApplicationData));
    }

    [Test]
    public void CreateEnlistedObjectManager ()
    {
      var manager = _factory.CreateEnlistedObjectManager (_fakeConstructedTransaction);
      Assert.That (manager, Is.SameAs (_parentEnlistedDomainObjectManagerStub));
    }

    [Test]
    public void CreateInvalidDomainObjectManager ()
    {
      _parentInvalidDomainObjectManagerStub.Stub (stub => stub.InvalidObjectIDs).Return (new ObjectID[0]);
      var eventSink = MockRepository.GenerateStub<IClientTransactionEventSink>();

      var manager = _factory.CreateInvalidDomainObjectManager (_fakeConstructedTransaction, eventSink);
      Assert.That (manager, Is.TypeOf (typeof (InvalidDomainObjectManager)));
      Assert.That (((InvalidDomainObjectManager) manager).TransactionEventSink, Is.SameAs (eventSink));
    }

    [Test]
    public void CreateInvalidDomainObjectManager_AutomaticallyMarksInvalid_ObjectsInvalidOrDeletedInParentTransaction ()
    {
      var objectInvalidInParent = _parentTransaction.ExecuteInScope (() => Order.NewObject ());
      var objectDeletedInParent = _parentTransaction.GetObject (DomainObjectIDs.Order3, false);
      var objectLoadedInParent = _parentTransaction.GetObject (DomainObjectIDs.Order4, false);

      _parentInvalidDomainObjectManagerStub.Stub (stub => stub.InvalidObjectIDs).Return (new[] { objectInvalidInParent.ID });
      _parentInvalidDomainObjectManagerStub.Stub (stub => stub.GetInvalidObjectReference (objectInvalidInParent.ID)).Return (objectInvalidInParent);
      
      _parentTransaction.Delete (objectDeletedInParent);

      var invalidOjectManager = _factory.CreateInvalidDomainObjectManager (
          _fakeConstructedTransaction, MockRepository.GenerateStub<IClientTransactionEventSink>());

      Assert.That (invalidOjectManager.IsInvalid (objectInvalidInParent.ID), Is.True);
      Assert.That (invalidOjectManager.IsInvalid (objectDeletedInParent.ID), Is.True);
      Assert.That (invalidOjectManager.IsInvalid (objectLoadedInParent.ID), Is.False);
    }

    [Test]
    public void CreatePersistenceStrategy ()
    {
      ClientTransactionTestHelper.SetIsWriteable (_parentTransaction, false);

      var result = _factory.CreatePersistenceStrategy (_fakeConstructedTransaction);

      Assert.That (result, Is.TypeOf<SubPersistenceStrategy> ());
      var parentTransactionContext = ((SubPersistenceStrategy) result).ParentTransactionContext;
      Assert.That (parentTransactionContext, Is.TypeOf<ParentTransactionContext>());
      Assert.That (((ParentTransactionContext) parentTransactionContext).ParentTransaction, Is.SameAs (_parentTransaction));
      Assert.That (
          ((ParentTransactionContext) parentTransactionContext).ParentInvalidDomainObjectManager, 
          Is.SameAs (_parentInvalidDomainObjectManagerStub));
    }

    [Test]
    public void CreatePersistenceStrategy_CanBeMixed ()
    {
      ClientTransactionTestHelper.SetIsWriteable (_parentTransaction, false);

      using (MixinConfiguration.BuildNew ().ForClass<SubPersistenceStrategy> ().AddMixin<NullMixin> ().EnterScope ())
      {
        var result = _factory.CreatePersistenceStrategy (_fakeConstructedTransaction);
        Assert.That (Mixin.Get<NullMixin> (result), Is.Not.Null);
      }
    }

    [Test]
    public void CreateRelationEndPointManager ()
    {
      var lazyLoader = MockRepository.GenerateStub<ILazyLoader> ();
      var endPointProvider = MockRepository.GenerateStub<IRelationEndPointProvider> ();
      var eventSink = MockRepository.GenerateStub<IClientTransactionEventSink> ();

      var relationEndPointManager =
          (RelationEndPointManager) PrivateInvoke.InvokeNonPublicMethod (
              _factory,
              "CreateRelationEndPointManager",
              _fakeConstructedTransaction,
              endPointProvider,
              lazyLoader,
              eventSink);

      Assert.That (relationEndPointManager.ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (relationEndPointManager.RegistrationAgent, Is.TypeOf<RelationEndPointRegistrationAgent> ());
      Assert.That (relationEndPointManager.EndPointFactory, Is.TypeOf<StateUpdateRaisingRelationEndPointFactoryDecorator> ());

      Assert.That (RelationEndPointManagerTestHelper.GetMap (relationEndPointManager).TransactionEventSink, Is.SameAs (eventSink));

      var stateUpdateRaisingFactory = (StateUpdateRaisingRelationEndPointFactoryDecorator) relationEndPointManager.EndPointFactory;
      Assert.That (
          stateUpdateRaisingFactory.Listener,
          Is.TypeOf<VirtualEndPointStateUpdateListener> ()
              .With.Property<VirtualEndPointStateUpdateListener> (l => l.TransactionEventSink).SameAs (eventSink));
      Assert.That (stateUpdateRaisingFactory.InnerFactory, Is.TypeOf<RelationEndPointFactory> ());
      var endPointFactory = ((RelationEndPointFactory) stateUpdateRaisingFactory.InnerFactory);
      Assert.That (endPointFactory.ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (endPointFactory.LazyLoader, Is.SameAs (lazyLoader));
      Assert.That (endPointFactory.EndPointProvider, Is.SameAs (endPointProvider));
      Assert.That (endPointFactory.TransactionEventSink, Is.SameAs (eventSink));
      
      Assert.That (endPointFactory.CollectionEndPointDataManagerFactory, Is.TypeOf (typeof (CollectionEndPointDataManagerFactory)));
      var collectionEndPointDataManagerFactory = ((CollectionEndPointDataManagerFactory) endPointFactory.CollectionEndPointDataManagerFactory);
      Assert.That (collectionEndPointDataManagerFactory.ChangeDetectionStrategy, Is.TypeOf<SubCollectionEndPointChangeDetectionStrategy> ());
      Assert.That (endPointFactory.VirtualObjectEndPointDataManagerFactory, Is.TypeOf<VirtualObjectEndPointDataManagerFactory> ());

      Assert.That (endPointFactory.CollectionEndPointCollectionProvider, Is.TypeOf<CollectionEndPointCollectionProvider> ());
      var collectionEndPointCollectionProvider = (CollectionEndPointCollectionProvider) endPointFactory.CollectionEndPointCollectionProvider;
      Assert.That (
          collectionEndPointCollectionProvider.DataStrategyFactory,
          Is.TypeOf<AssociatedCollectionDataStrategyFactory> ()
              .With.Property ((AssociatedCollectionDataStrategyFactory f) => f.VirtualEndPointProvider).SameAs (endPointProvider));
    }

    [Test]
    public void CreateObjectLoader ()
    {
      var persistenceStrategy = MockRepository.GenerateStub<IPersistenceStrategy> ();
      var dataManager = MockRepository.GenerateStub<IDataManager> ();
      var invalidDomainObjectManager = MockRepository.GenerateStub<IInvalidDomainObjectManager> ();
      var eventSink = MockRepository.GenerateStub<IClientTransactionEventSink> ();
      var hierarchyManager = MockRepository.GenerateStub<ITransactionHierarchyManager> ();

      var fakeBasicObjectLoader = MockRepository.GenerateStub<IObjectLoader> ();

      var factoryPartialMock = MockRepository.GeneratePartialMock<SubClientTransactionComponentFactory> (
          _parentTransaction, 
          _parentInvalidDomainObjectManagerStub,
          _parentEnlistedDomainObjectManagerStub,
          _parentTransactionHierarchyManagerStub,
          _parentEventSink);
      factoryPartialMock
          .Expect (
              mock => CallCreateBasicObjectLoader (
                  mock, _fakeConstructedTransaction, eventSink, persistenceStrategy, invalidDomainObjectManager, dataManager, hierarchyManager))
          .Return (fakeBasicObjectLoader);
      factoryPartialMock.Replay ();

      var result = PrivateInvoke.InvokeNonPublicMethod (
          factoryPartialMock,
          "CreateObjectLoader",
          _fakeConstructedTransaction,
          eventSink,
          persistenceStrategy,
          invalidDomainObjectManager,
          dataManager,
          hierarchyManager);

      Assert.That (result, Is.SameAs (fakeBasicObjectLoader));
    }

    [Test]
    public void CreateBasicObjectLoader ()
    {
      var persistenceStrategy = MockRepository.GenerateStub<IPersistenceStrategy> ();
      var dataManager = MockRepository.GenerateStub<IDataManager> ();
      var invalidDomainObjectManager = MockRepository.GenerateStub<IInvalidDomainObjectManager> ();
      var eventSink = MockRepository.GenerateStub<IClientTransactionEventSink> ();
      var hierarchyManager = MockRepository.GenerateStub<ITransactionHierarchyManager> ();

      var result = CallCreateBasicObjectLoader (
          _factory,
          _fakeConstructedTransaction,
          eventSink,
          persistenceStrategy,
          invalidDomainObjectManager,
          dataManager,
          hierarchyManager);

      Assert.That (result, Is.TypeOf (typeof (ObjectLoader)));
      var objectLoader = (ObjectLoader) result;
      Assert.That (objectLoader.PersistenceStrategy, Is.SameAs (persistenceStrategy));
      Assert.That (
          objectLoader.LoadedObjectDataRegistrationAgent,
          Is.TypeOf<LoadedObjectDataRegistrationAgent> ()
              .With.Property ((LoadedObjectDataRegistrationAgent agent) => agent.ClientTransaction).SameAs (_fakeConstructedTransaction)
              .With.Property ((LoadedObjectDataRegistrationAgent agent) => agent.DataManager).SameAs (dataManager));

      var registrationListener = ((LoadedObjectDataRegistrationAgent) objectLoader.LoadedObjectDataRegistrationAgent).RegistrationListener;
      Assert.That (registrationListener, Is.TypeOf<LoadedObjectDataRegistrationListener> ());
      Assert.That (((LoadedObjectDataRegistrationListener) registrationListener).EventSink, Is.SameAs (eventSink));
      Assert.That (((LoadedObjectDataRegistrationListener) registrationListener).HierarchyManager, Is.SameAs (hierarchyManager));

      Assert.That (objectLoader.LoadedObjectDataProvider, Is.TypeOf<LoadedObjectDataProvider> ());
      var loadedObjectDataProvider = (LoadedObjectDataProvider) objectLoader.LoadedObjectDataProvider;
      Assert.That (loadedObjectDataProvider.LoadedDataContainerProvider, Is.SameAs (dataManager));
      Assert.That (loadedObjectDataProvider.InvalidDomainObjectManager, Is.SameAs (invalidDomainObjectManager));
    }

    private object CallCreateBasicObjectLoader (
        SubClientTransactionComponentFactory factory,
        TestableClientTransaction constructedTransaction,
        IClientTransactionEventSink eventSink,
        IPersistenceStrategy persistenceStrategy,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        IDataManager dataManager,
        ITransactionHierarchyManager hierarchyManager)
    {
      return PrivateInvoke.InvokeNonPublicMethod (
          factory,
          "CreateBasicObjectLoader",
          constructedTransaction,
          eventSink,
          persistenceStrategy,
          invalidDomainObjectManager,
          dataManager,
          hierarchyManager);
    }
  }
}