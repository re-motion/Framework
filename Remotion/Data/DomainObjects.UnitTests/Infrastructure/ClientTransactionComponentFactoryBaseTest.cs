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
using CommonServiceLocator;
using Moq;
using Moq.Protected;
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
using Remotion.Data.UnitTests.UnitTesting;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure
{
  [TestFixture]
  public class ClientTransactionComponentFactoryBaseTest
  {
    private TestableClientTransactionComponentFactoryBase _factory;
    private ClientTransaction _fakeConstructedTransaction;

    [SetUp]
    public void SetUp ()
    {
      _factory = new TestableClientTransactionComponentFactoryBase();
      _fakeConstructedTransaction = ClientTransaction.CreateRootTransaction();
    }

    [Test]
    public void CreateEventBroker ()
    {
      var fakeListener = new Mock<IClientTransactionListener>();

      var factoryPartialMock = new Mock<TestableClientTransactionComponentFactoryBase>() { CallBase = true };
      factoryPartialMock.Setup (stub => stub.CallCreateListeners (_fakeConstructedTransaction)).Returns (new[] { fakeListener.Object });

      var result = factoryPartialMock.Object.CreateEventBroker(_fakeConstructedTransaction);

      Assert.That(
          result,
          Is.TypeOf<ClientTransactionEventBroker>()
              .With.Property<ClientTransactionEventBroker>(m => m.ClientTransaction).SameAs(_fakeConstructedTransaction)
              .And.Property<ClientTransactionEventBroker>(m => m.Listeners).EqualTo(new[] { fakeListener.Object }));
    }

    [Test]
    public void CreateListeners ()
    {
      IEnumerable<IClientTransactionListener> listeners = _factory.CallCreateListeners(_fakeConstructedTransaction).ToArray();
      Assert.That(
          listeners,
          Has
              .Length.EqualTo(1)
              .And.Some.TypeOf<LoggingClientTransactionListener>());
    }

    [Test]
    public void CreateDataManager ()
    {
      var fakeEventSink = new Mock<IClientTransactionEventSink>();
      var fakeInvalidDomainObjectManager = new Mock<IInvalidDomainObjectManager>();
      var fakePersistenceStrategy = new Mock<IPersistenceStrategy>();

      var fakeDataContainerEventListener = new Mock<IDataContainerEventListener>();
      var fakeEndPointProvider = new Mock<IRelationEndPointProvider>();
      var fakeLazyLoader = new Mock<ILazyLoader>();
      var fakeRelationEndPointManager = new Mock<IRelationEndPointManager>();
      var fakeObjectLoader = new Mock<IObjectLoader>();
      var fakeHierarchyManager = new Mock<ITransactionHierarchyManager>();

      DelegatingDataManager endPointProviderDataManager = null;
      DelegatingDataManager lazyLoaderDataManager = null;
      DelegatingDataManager objectLoaderDataManager = null;
      DelegatingDataContainerMap relationEndPointManagerDataContainerMap = null;

      var factoryPartialMock = new Mock<TestableClientTransactionComponentFactoryBase>() { CallBase = true };
      factoryPartialMock
          .Setup(mock => mock.CallCreateDataContainerEventListener(fakeEventSink.Object))
          .Returns(fakeDataContainerEventListener.Object)
          .Verifiable();
      factoryPartialMock
          .Setup(mock => mock.CallGetEndPointProvider(Arg<DelegatingDataManager>.Is.TypeOf))
          .Returns(fakeEndPointProvider.Object)
          .Callback((IDataManager dataManager) => endPointProviderDataManager = dataManager)
          .Verifiable();
      factoryPartialMock
          .Setup(mock => mock.CallGetLazyLoader(Arg<DelegatingDataManager>.Is.TypeOf))
          .Returns(fakeLazyLoader.Object)
          .Callback((IDataManager dataManager) => lazyLoaderDataManager = dataManager)
          .Verifiable();
      factoryPartialMock
          .Setup(
              mock => mock.CallCreateRelationEndPointManager(
                  _fakeConstructedTransaction,
                  fakeEndPointProvider.Object,
                  fakeLazyLoader.Object,
                  fakeEventSink.Object,
                  Arg<DelegatingDataContainerMap>.Is.TypeOf))
          .Returns(fakeRelationEndPointManager.Object)
          .Callback((ClientTransaction constructedTransaction, IRelationEndPointProvider endPointProvider, ILazyLoader lazyLoader, IClientTransactionEventSink eventSink, IDataContainerMapReadOnlyView dataContainerMap) => relationEndPointManagerDataContainerMap = dataContainerMap)
          .Verifiable();
      factoryPartialMock
          .Setup(
              mock => mock.CallCreateObjectLoader(
                  _fakeConstructedTransaction,
                  fakeEventSink.Object,
                  fakePersistenceStrategy.Object,
                  fakeInvalidDomainObjectManager.Object,
                  Arg<DelegatingDataManager>.Is.TypeOf,
                  fakeHierarchyManager.Object))
          .Returns(fakeObjectLoader.Object)
          .Callback((ClientTransaction constructedTransaction, IClientTransactionEventSink eventSink, IPersistenceStrategy persistenceStrategy, IInvalidDomainObjectManager invalidDomainObjectManager, IDataManager dataManager, ITransactionHierarchyManager hierarchyManager) => objectLoaderDataManager = dataManager)
          .Verifiable();

      var dataManager = (DataManager)factoryPartialMock.Object.CreateDataManager(
          _fakeConstructedTransaction,
          fakeEventSink.Object,
          fakeInvalidDomainObjectManager.Object,
          fakePersistenceStrategy.Object,
          fakeHierarchyManager.Object);

      factoryPartialMock.Verify();
      Assert.That(endPointProviderDataManager.InnerDataManager, Is.SameAs(dataManager));
      Assert.That(lazyLoaderDataManager.InnerDataManager, Is.SameAs(dataManager));
      Assert.That(objectLoaderDataManager.InnerDataManager, Is.SameAs(dataManager));
      Assert.That(relationEndPointManagerDataContainerMap.InnerDataContainerMap, Is.SameAs(dataManager.DataContainers));

      Assert.That(dataManager.ClientTransaction, Is.SameAs(_fakeConstructedTransaction));
      Assert.That(dataManager.TransactionEventSink, Is.SameAs(fakeEventSink.Object));
      Assert.That(dataManager.DataContainerEventListener, Is.SameAs(fakeDataContainerEventListener.Object));
      Assert.That(DataManagerTestHelper.GetInvalidDomainObjectManager(dataManager), Is.SameAs(fakeInvalidDomainObjectManager.Object));
      Assert.That(DataManagerTestHelper.GetObjectLoader(dataManager), Is.SameAs(fakeObjectLoader.Object));
      Assert.That(DataManagerTestHelper.GetRelationEndPointManager(dataManager), Is.SameAs(fakeRelationEndPointManager.Object));
    }

    [Test]
    public void CreateObjectLifetimeAgent ()
    {
      var dataManager = new Mock<IDataManager>();
      var invalidDomainObjectManager = new Mock<IInvalidDomainObjectManager>();
      var eventSink = new Mock<IClientTransactionEventSink>();
      var enlistedDomainObjectManager = new Mock<IEnlistedDomainObjectManager>();
      var persistenceStrategy = new Mock<IPersistenceStrategy>();

      var result = _factory.CreateObjectLifetimeAgent(
          _fakeConstructedTransaction, eventSink.Object, invalidDomainObjectManager.Object, dataManager.Object, enlistedDomainObjectManager.Object, persistenceStrategy.Object);

      Assert.That(result, Is.TypeOf(typeof(ObjectLifetimeAgent)));

      var objectLifetimeAgent = ((ObjectLifetimeAgent)result);
      Assert.That(objectLifetimeAgent.ClientTransaction, Is.SameAs(_fakeConstructedTransaction));
      Assert.That(objectLifetimeAgent.EventSink, Is.SameAs(eventSink.Object));
      Assert.That(objectLifetimeAgent.InvalidDomainObjectManager, Is.SameAs(invalidDomainObjectManager.Object));
      Assert.That(objectLifetimeAgent.DataManager, Is.SameAs(dataManager.Object));
      Assert.That(objectLifetimeAgent.EnlistedDomainObjectManager, Is.SameAs(enlistedDomainObjectManager.Object));
      Assert.That(objectLifetimeAgent.PersistenceStrategy, Is.SameAs(persistenceStrategy.Object));
    }

    [Test]
    public void CreateQueryManager ()
    {
      var persistenceStrategy = new Mock<IPersistenceStrategy>();
      var dataManager = new Mock<IDataManager>();
      var invalidDomainObjectManager = new Mock<IInvalidDomainObjectManager>();
      var eventSink = new Mock<IClientTransactionEventSink>();
      var hierarchyManager = new Mock<ITransactionHierarchyManager>();

      var fakeObjectLoader = new Mock<IObjectLoader>();

      var factoryPartialMock = new Mock<TestableClientTransactionComponentFactoryBase>() { CallBase = true };
      factoryPartialMock
          .Setup(
              mock => mock.CallCreateObjectLoader(
                  _fakeConstructedTransaction, eventSink.Object, persistenceStrategy.Object, invalidDomainObjectManager.Object, dataManager.Object, hierarchyManager.Object))
          .Returns(fakeObjectLoader.Object)
          .Verifiable();

      var result = factoryPartialMock.Object.CreateQueryManager(
          _fakeConstructedTransaction, eventSink.Object, invalidDomainObjectManager.Object, persistenceStrategy.Object, dataManager.Object, hierarchyManager.Object);

      factoryPartialMock.Verify();

      Assert.That(result, Is.TypeOf(typeof(QueryManager)));
      Assert.That(((QueryManager)result).PersistenceStrategy, Is.SameAs(persistenceStrategy.Object));
      Assert.That(((QueryManager)result).TransactionEventSink, Is.SameAs(eventSink.Object));
      Assert.That(((QueryManager)result).ObjectLoader, Is.SameAs(fakeObjectLoader.Object));
    }

    [Test]
    public void CreateCommitRollbackAgent ()
    {
      var eventSink = new Mock<IClientTransactionEventSink>();
      var persistenceStrategy = new Mock<IPersistenceStrategy>();
      var dataManager = new Mock<IDataManager>();

      var result = _factory.CreateCommitRollbackAgent(_fakeConstructedTransaction, eventSink.Object, persistenceStrategy.Object, dataManager.Object);

      Assert.That(result, Is.TypeOf<CommitRollbackAgent>());
      Assert.That(((CommitRollbackAgent)result).ClientTransaction, Is.SameAs(_fakeConstructedTransaction));
      Assert.That(((CommitRollbackAgent)result).EventSink, Is.SameAs(eventSink.Object));
      Assert.That(((CommitRollbackAgent)result).PersistenceStrategy, Is.SameAs(persistenceStrategy.Object));
      Assert.That(((CommitRollbackAgent)result).DataManager, Is.SameAs(dataManager.Object));
    }

    [Test]
    public void CreateExtensions ()
    {
      var extensionFactoryMock = new Mock<IClientTransactionExtensionFactory> (MockBehavior.Strict);
      var extensionStub = new Mock<IClientTransactionExtension>();
      extensionStub.Setup (stub => stub.Key).Returns ("stub1");

      extensionFactoryMock.Setup (mock => mock.CreateClientTransactionExtensions (_fakeConstructedTransaction)).Returns (new[] { extensionStub.Object }).Verifiable();

      var serviceLocatorMock = new Mock<IServiceLocator> (MockBehavior.Strict);
      serviceLocatorMock.Setup (mock => mock.GetInstance<IClientTransactionExtensionFactory>()).Returns (extensionFactoryMock.Object).Verifiable();

      IClientTransactionExtension[] extensions;
      using (new ServiceLocatorScope(serviceLocatorMock.Object))
      {
        extensions = _factory.CreateExtensions(_fakeConstructedTransaction).ToArray();
      }

      serviceLocatorMock.Verify();
      extensionFactoryMock.Verify();

      Assert.That(extensions, Is.EqualTo(new[] { extensionStub.Object }));
    }

    [Test]
    public void GetLazyLoader ()
    {
      var dataManager = ClientTransactionTestHelper.GetDataManager(_fakeConstructedTransaction);

      var result = _factory.CallGetLazyLoader(dataManager);

      Assert.That(result, Is.SameAs(result));
    }

    [Test]
    public void GetEndPointProvider ()
    {
      var dataManager = new Mock<IDataManager>();

      var result = _factory.CallGetEndPointProvider(dataManager.Object);

      Assert.That(result, Is.SameAs(dataManager.Object));
    }
  }
}
