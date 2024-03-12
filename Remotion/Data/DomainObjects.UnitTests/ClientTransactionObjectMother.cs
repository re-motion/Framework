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
using Moq.Protected;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Development.UnitTesting;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.UnitTests
{
  public static class ClientTransactionObjectMother
  {
    public static Mock<ClientTransaction> CreateStrictMock ()
    {
      var componentFactory = RootClientTransactionComponentFactory.Create(
          SafeServiceLocator.Current.GetInstance<IStorageSettings>(),
          SafeServiceLocator.Current.GetInstance<IPersistenceService>(),
          SafeServiceLocator.Current.GetInstance<IPersistenceExtensionFactory>());
      return new Mock<ClientTransaction>(MockBehavior.Strict, componentFactory);
    }

    public static T CreateTransactionWithPersistenceStrategy<T> (IPersistenceStrategy persistenceStrategy) where T : ClientTransaction
    {
      var componentFactory = new TestComponentFactoryWithSpecificPersistenceStrategy(persistenceStrategy);
      return CreateWithComponents<T>(componentFactory);
    }

    public static T CreateTransactionWithQueryManager<T> (IQueryManager queryManager) where T : ClientTransaction
    {
      var componentFactory = new TestComponentFactoryWithSpecificQueryManager(queryManager);
      return CreateWithComponents<T>(componentFactory);
    }

    public static T CreateTransactionWithObjectLoaderDecorator<T> (TestComponentFactoryWithObjectLoaderDecorator.DecoratorFactory factory)
        where T : ClientTransaction
    {
      var componentFactory = new TestComponentFactoryWithObjectLoaderDecorator(factory);
      return CreateWithComponents<T>(componentFactory);
    }

    public static ClientTransaction Create ()
    {
      return ClientTransaction.CreateRootTransaction();
    }

    public static T CreateWithComponents<T> (IClientTransactionComponentFactory componentFactory) where T : ClientTransaction
    {
      return (T)PrivateInvoke.CreateInstanceNonPublicCtor(typeof(T), componentFactory);
    }

    public static T CreateWithComponents<T> (
        Dictionary<Enum, object> applicationData = null,
        IClientTransactionEventBroker eventBroker = null,
        ITransactionHierarchyManager transactionHierarchyManager = null,
        IEnlistedDomainObjectManager enlistedDomainObjectManager = null,
        IInvalidDomainObjectManager invalidDomainObjectManager = null,
        IPersistenceStrategy persistenceStrategy = null,
        IDataManager dataManager = null,
        IObjectLifetimeAgent objectLifetimeAgent = null,
        IQueryManager queryManager = null,
        ICommitRollbackAgent commitRollbackAgent = null,
        IEnumerable<IClientTransactionExtension> extensions = null)
      where T : ClientTransaction
    {
      var componentFactoryStub = CreateComponentFactory(
          applicationData,
          eventBroker,
          transactionHierarchyManager,
          enlistedDomainObjectManager,
          invalidDomainObjectManager,
          persistenceStrategy,
          dataManager,
          objectLifetimeAgent,
          queryManager,
          commitRollbackAgent,
          extensions);

      return CreateWithComponents<T>(componentFactoryStub);
    }

    public static IClientTransactionComponentFactory CreateComponentFactory (
        Dictionary<Enum, object> applicationData = null,
        IClientTransactionEventBroker eventBroker = null,
        ITransactionHierarchyManager transactionHierarchyManager = null,
        IEnlistedDomainObjectManager enlistedDomainObjectManager = null,
        IInvalidDomainObjectManager invalidDomainObjectManager = null,
        IPersistenceStrategy persistenceStrategy = null,
        IDataManager dataManager = null,
        IObjectLifetimeAgent objectLifetimeAgent = null,
        IQueryManager queryManager = null,
        ICommitRollbackAgent commitRollbackAgent = null,
        IEnumerable<IClientTransactionExtension> extensions = null)
    {
      applicationData = applicationData ?? new Dictionary<Enum, object>();
      transactionHierarchyManager = transactionHierarchyManager ?? Mock.Of<ITransactionHierarchyManager>();
      enlistedDomainObjectManager = enlistedDomainObjectManager ?? Mock.Of<IEnlistedDomainObjectManager>();
      invalidDomainObjectManager = invalidDomainObjectManager ?? Mock.Of<IInvalidDomainObjectManager>();
      persistenceStrategy = persistenceStrategy ?? Mock.Of<IPersistenceStrategy>();
      dataManager = dataManager ?? Mock.Of<IDataManager>();
      objectLifetimeAgent = objectLifetimeAgent ?? Mock.Of<IObjectLifetimeAgent>();
      eventBroker = eventBroker ?? Mock.Of<IClientTransactionEventBroker>();
      queryManager = queryManager ?? Mock.Of<IQueryManager>();
      commitRollbackAgent = commitRollbackAgent ?? Mock.Of<ICommitRollbackAgent>();
      extensions = extensions ?? Enumerable.Empty<IClientTransactionExtension>();

      var componentFactoryStub = new Mock<IClientTransactionComponentFactory>();
      componentFactoryStub.Setup(stub => stub.CreateApplicationData(It.IsAny<ClientTransaction>())).Returns(applicationData);
      componentFactoryStub
          .Setup(stub => stub.CreateEventBroker(It.IsAny<ClientTransaction>()))
          .Returns(eventBroker);
      componentFactoryStub
          .Setup(stub => stub.CreateTransactionHierarchyManager(It.IsAny<ClientTransaction>(), It.IsAny<IClientTransactionEventSink>()))
          .Returns(transactionHierarchyManager);
      componentFactoryStub.Setup(stub => stub.CreateEnlistedObjectManager(It.IsAny<ClientTransaction>())).Returns(enlistedDomainObjectManager);
      componentFactoryStub
          .Setup(stub => stub.CreateInvalidDomainObjectManager(It.IsAny<ClientTransaction>(), It.IsAny<IClientTransactionEventSink>()))
          .Returns(invalidDomainObjectManager);
      componentFactoryStub.Setup(stub => stub.CreatePersistenceStrategy(It.IsAny<ClientTransaction>())).Returns(persistenceStrategy);
      componentFactoryStub
          .Setup(
              stub => stub.CreateDataManager(
                  It.IsAny<ClientTransaction>(),
                  It.IsAny<IClientTransactionEventSink>(),
                  It.IsAny<IInvalidDomainObjectManager>(),
                  It.IsAny<IPersistenceStrategy>(),
                  It.IsAny<ITransactionHierarchyManager>()))
          .Returns(dataManager);
      componentFactoryStub
          .Setup(
              stub => stub.CreateObjectLifetimeAgent(
                  It.IsAny<ClientTransaction>(),
                  It.IsAny<IClientTransactionEventSink>(),
                  It.IsAny<IInvalidDomainObjectManager>(),
                  It.IsAny<IDataManager>(),
                  It.IsAny<IEnlistedDomainObjectManager>(),
                  It.IsAny<IPersistenceStrategy>()))
          .Returns(objectLifetimeAgent);
      componentFactoryStub
          .Setup(
              stub => stub.CreateQueryManager(
                  It.IsAny<ClientTransaction>(),
                  It.IsAny<IClientTransactionEventSink>(),
                  It.IsAny<IInvalidDomainObjectManager>(),
                  It.IsAny<IPersistenceStrategy>(),
                  It.IsAny<IDataManager>(),
                  It.IsAny<ITransactionHierarchyManager>()))
          .Returns(queryManager);
      componentFactoryStub
          .Setup(
              stub => stub.CreateCommitRollbackAgent(
                  It.IsAny<ClientTransaction>(),
                  It.IsAny<IClientTransactionEventSink>(),
                  It.IsAny<IPersistenceStrategy>(),
                  It.IsAny<IDataManager>()))
          .Returns(commitRollbackAgent);
      componentFactoryStub.Setup(stub => stub.CreateExtensions(It.IsAny<ClientTransaction>())).Returns(extensions);
      return componentFactoryStub.Object;
    }

    public static ClientTransaction CreateWithCustomListeners (params IClientTransactionListener[] listeners)
    {
      var componentFactoryPartialMock = new Mock<RootClientTransactionComponentFactory>(Mock.Of<IStorageSettings>(), Mock.Of<IPersistenceService>(), Mock.Of<IPersistenceExtensionFactory>()) { CallBase = true };
      componentFactoryPartialMock
          .Protected()
          .Setup<IEnumerable<IClientTransactionListener>>("CreateListeners", true, ItExpr.IsAny<ClientTransaction>())
          .Returns(listeners);

      return CreateWithComponents<ClientTransaction>(componentFactoryPartialMock.Object);
    }

    public static ClientTransaction CreateWithParent (ClientTransaction parent)
    {
      var hierarchyManagerStub = new Mock<ITransactionHierarchyManager>();
      hierarchyManagerStub.Setup(stub => stub.ParentTransaction).Returns(parent);
      return CreateWithComponents<ClientTransaction>(transactionHierarchyManager: hierarchyManagerStub.Object);
    }

    public static ClientTransaction CreateWithSub (ClientTransaction sub)
    {
      var hierarchyManagerStub = new Mock<ITransactionHierarchyManager>();
      hierarchyManagerStub.Setup(stub => stub.SubTransaction).Returns(sub);
      hierarchyManagerStub.Setup(stub => stub.IsWriteable).Returns(false);
      return CreateWithComponents<ClientTransaction>(transactionHierarchyManager: hierarchyManagerStub.Object);
    }
  }
}
