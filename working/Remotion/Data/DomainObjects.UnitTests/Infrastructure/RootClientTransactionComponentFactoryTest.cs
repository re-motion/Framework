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
using Microsoft.Practices.ServiceLocation;
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
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Data.UnitTests.UnitTesting;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure
{
  [TestFixture]
  public class RootClientTransactionComponentFactoryTest
  {
    private RootClientTransactionComponentFactory _factory;
    private TestableClientTransaction _fakeConstructedTransaction;

    [SetUp]
    public void SetUp ()
    {
      _factory = RootClientTransactionComponentFactory.Create ();
      _fakeConstructedTransaction = new TestableClientTransaction ();
    }

    [Test]
    public void CreateTransactionHierarchyManager ()
    {
      var eventSink = MockRepository.GenerateStub<IClientTransactionEventSink> ();

      var transactionHierarchyManager = _factory.CreateTransactionHierarchyManager (_fakeConstructedTransaction, eventSink);
      
      Assert.That (transactionHierarchyManager, Is.TypeOf<TransactionHierarchyManager> ());
      Assert.That (((TransactionHierarchyManager) transactionHierarchyManager).ThisTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (((TransactionHierarchyManager) transactionHierarchyManager).ThisEventSink, Is.SameAs (eventSink));
      Assert.That (((TransactionHierarchyManager) transactionHierarchyManager).ParentTransaction, Is.Null);
      Assert.That (((TransactionHierarchyManager) transactionHierarchyManager).ParentHierarchyManager, Is.Null);
      Assert.That (((TransactionHierarchyManager) transactionHierarchyManager).ParentEventSink, Is.Null);
    }

    [Test]
    public void CreateApplicationData ()
    {
      var applicationData = _factory.CreateApplicationData (_fakeConstructedTransaction);

      Assert.That (applicationData, Is.Not.Null);
      Assert.That (applicationData.Count, Is.EqualTo (0));
    }

    [Test]
    public void CreateEnlistedDomainObjectManager ()
    {
      var manager = _factory.CreateEnlistedObjectManager (_fakeConstructedTransaction);
      Assert.That (manager, Is.TypeOf<DictionaryBasedEnlistedDomainObjectManager>());
    }

    [Test]
    public void CreateInvalidDomainObjectManager ()
    {
      var eventSink = MockRepository.GenerateStub<IClientTransactionEventSink>();
      var manager = _factory.CreateInvalidDomainObjectManager (_fakeConstructedTransaction, eventSink);
      Assert.That (manager, Is.TypeOf (typeof (InvalidDomainObjectManager)));
      Assert.That (((InvalidDomainObjectManager) manager).InvalidObjectCount, Is.EqualTo (0));
      Assert.That (((InvalidDomainObjectManager) manager).TransactionEventSink, Is.SameAs (eventSink));
    }

    [Test]
    public void CreatePersistenceStrategy ()
    {
      var result = _factory.CreatePersistenceStrategy (_fakeConstructedTransaction);

      Assert.That (result, Is.TypeOf<RootPersistenceStrategy> ());
      Assert.That (((RootPersistenceStrategy) result).TransactionID, Is.EqualTo (_fakeConstructedTransaction.ID));
    }

    [Test]
    public void CreatePersistenceStrategy_CanBeMixed ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<RootPersistenceStrategy> ().AddMixin<NullMixin> ().EnterScope ())
      {
        var result = _factory.CreatePersistenceStrategy (_fakeConstructedTransaction);
        Assert.That (Mixin.Get<NullMixin> (result), Is.Not.Null);
      }
    }

    [Test]
    public void CreateExtensions ()
    {
      var extensionFactoryMock = MockRepository.GenerateStrictMock<IClientTransactionExtensionFactory> ();
      var extensionStub = MockRepository.GenerateStub<IClientTransactionExtension> ();
      extensionStub.Stub (stub => stub.Key).Return ("stub1");

      extensionFactoryMock.Expect (mock => mock.CreateClientTransactionExtensions (_fakeConstructedTransaction)).Return (new[] { extensionStub });
      extensionFactoryMock.Replay ();

      var serviceLocatorMock = MockRepository.GenerateStrictMock<IServiceLocator> ();
      serviceLocatorMock.Expect (mock => mock.GetInstance<IClientTransactionExtensionFactory> ()).Return (extensionFactoryMock);
      serviceLocatorMock.Replay ();

      IClientTransactionExtension[] extensions;
      using (new ServiceLocatorScope (serviceLocatorMock))
      {
        extensions = _factory.CreateExtensions (_fakeConstructedTransaction).ToArray();
      }

      serviceLocatorMock.VerifyAllExpectations ();
      extensionFactoryMock.VerifyAllExpectations ();

      Assert.That (extensions, Is.EqualTo (new[] { extensionStub }));
    }

    [Test]
    public void CreateListeners ()
    {
      var listeners =
          ((IEnumerable<IClientTransactionListener>) PrivateInvoke.InvokeNonPublicMethod (_factory, "CreateListeners", _fakeConstructedTransaction))
          .ToArray();
      Assert.That (
          listeners,
          Has
              .Length.EqualTo (2)
              .And.Some.TypeOf<LoggingClientTransactionListener> ()
              .And.Some.TypeOf<NotFoundObjectsClientTransactionListener> ());
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
      Assert.That (relationEndPointManager.RegistrationAgent, Is.TypeOf<RootRelationEndPointRegistrationAgent> ());
      Assert.That (relationEndPointManager.EndPointFactory, Is.TypeOf<StateUpdateRaisingRelationEndPointFactoryDecorator> ());

      Assert.That (RelationEndPointManagerTestHelper.GetMap (relationEndPointManager).TransactionEventSink, Is.SameAs (eventSink));
      
      var stateUpdateRaisingFactory = (StateUpdateRaisingRelationEndPointFactoryDecorator) relationEndPointManager.EndPointFactory;
      Assert.That (
          stateUpdateRaisingFactory.Listener,
          Is.TypeOf<VirtualEndPointStateUpdateListener>()
              .With.Property<VirtualEndPointStateUpdateListener> (l => l.TransactionEventSink).SameAs (eventSink));
      Assert.That (stateUpdateRaisingFactory.InnerFactory, Is.TypeOf<RelationEndPointFactory> ());
      var endPointFactory = ((RelationEndPointFactory) stateUpdateRaisingFactory.InnerFactory);
      Assert.That (endPointFactory.ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (endPointFactory.LazyLoader, Is.SameAs (lazyLoader));
      Assert.That (endPointFactory.EndPointProvider, Is.SameAs (endPointProvider));
      Assert.That (endPointFactory.TransactionEventSink, Is.SameAs (eventSink));

      Assert.That (endPointFactory.CollectionEndPointDataManagerFactory, Is.TypeOf (typeof (CollectionEndPointDataManagerFactory)));
      var collectionEndPointDataManagerFactory = ((CollectionEndPointDataManagerFactory) endPointFactory.CollectionEndPointDataManagerFactory);
      Assert.That (collectionEndPointDataManagerFactory.ChangeDetectionStrategy, Is.TypeOf<RootCollectionEndPointChangeDetectionStrategy> ());
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
      var persistenceStrategy = MockRepository.GenerateStub<IFetchEnabledPersistenceStrategy> ();
      var dataManager = MockRepository.GenerateStub<IDataManager> ();
      var invalidDomainObjectManager = MockRepository.GenerateStub<IInvalidDomainObjectManager> ();
      var eventSink = MockRepository.GenerateStub<IClientTransactionEventSink> ();
      var hierarchyManager = MockRepository.GenerateStub<ITransactionHierarchyManager> ();

      var result = PrivateInvoke.InvokeNonPublicMethod (
          _factory,
          "CreateObjectLoader",
          _fakeConstructedTransaction,
          eventSink,
          persistenceStrategy,
          invalidDomainObjectManager,
          dataManager,
          hierarchyManager);

      Assert.That (result, Is.TypeOf (typeof (FetchEnabledObjectLoader)));
      var objectLoader = (FetchEnabledObjectLoader) result;
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

      Assert.That (objectLoader.EagerFetcher, Is.TypeOf<EagerFetcher> ());

      var eagerFetcher = (EagerFetcher) objectLoader.EagerFetcher;
      Assert.That (eagerFetcher.RegistrationAgent, Is.TypeOf<DelegatingFetchedRelationDataRegistrationAgent> ());
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) eagerFetcher.RegistrationAgent).RealObjectDataRegistrationAgent,
          Is.TypeOf<FetchedRealObjectRelationDataRegistrationAgent> ());
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) eagerFetcher.RegistrationAgent).VirtualObjectDataRegistrationAgent,
          Is.TypeOf<FetchedVirtualObjectRelationDataRegistrationAgent> ()
              .With.Property<FetchedCollectionRelationDataRegistrationAgent> (a => a.VirtualEndPointProvider).SameAs (dataManager));
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) eagerFetcher.RegistrationAgent).CollectionDataRegistrationAgent,
          Is.TypeOf<FetchedCollectionRelationDataRegistrationAgent> ()
              .With.Property<FetchedCollectionRelationDataRegistrationAgent> (a => a.VirtualEndPointProvider).SameAs (dataManager));
    }
  }
}