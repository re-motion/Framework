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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests
{
  public static class ClientTransactionObjectMother
  {
    public static ClientTransaction CreateStrictMock ()
    {
      return CreateStrictMock(new MockRepository ());
    }

    public static ClientTransaction CreateStrictMock (MockRepository mockRepository)
    {
      var componentFactory = RootClientTransactionComponentFactory.Create();
      return mockRepository.StrictMock<ClientTransaction> (componentFactory);
    }

    public static T CreateTransactionWithPersistenceStrategy<T> (IPersistenceStrategy persistenceStrategy) where T : ClientTransaction
    {
      var componentFactory = new TestComponentFactoryWithSpecificPersistenceStrategy (persistenceStrategy);
      return CreateWithComponents<T> (componentFactory);
    }

    public static T CreateTransactionWithQueryManager<T> (IQueryManager queryManager) where T : ClientTransaction
    {
      var componentFactory = new TestComponentFactoryWithSpecificQueryManager (queryManager);
      return CreateWithComponents<T> (componentFactory);
    }

    public static T CreateTransactionWithObjectLoaderDecorator<T> (TestComponentFactoryWithObjectLoaderDecorator.DecoratorFactory factory) 
        where T : ClientTransaction
    {
      var componentFactory = new TestComponentFactoryWithObjectLoaderDecorator (factory);
      return CreateWithComponents<T> (componentFactory);
    }

    public static ClientTransaction Create ()
    {
      return ClientTransaction.CreateRootTransaction();
    }

    public static T CreateWithComponents<T> (IClientTransactionComponentFactory componentFactory) where T : ClientTransaction
    {
      return (T) PrivateInvoke.CreateInstanceNonPublicCtor (typeof (T), componentFactory);
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
      var componentFactoryStub = CreateComponentFactory (
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

      return CreateWithComponents<T> (componentFactoryStub);
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
      transactionHierarchyManager = transactionHierarchyManager ?? MockRepository.GenerateStub<ITransactionHierarchyManager>();
      enlistedDomainObjectManager = enlistedDomainObjectManager ?? MockRepository.GenerateStub<IEnlistedDomainObjectManager>();
      invalidDomainObjectManager = invalidDomainObjectManager ?? MockRepository.GenerateStub<IInvalidDomainObjectManager>();
      persistenceStrategy = persistenceStrategy ?? MockRepository.GenerateStub<IPersistenceStrategy>();
      dataManager = dataManager ?? MockRepository.GenerateStub<IDataManager>();
      objectLifetimeAgent = objectLifetimeAgent ?? MockRepository.GenerateStub<IObjectLifetimeAgent>();
      eventBroker = eventBroker ?? MockRepository.GenerateStub<IClientTransactionEventBroker>();
      queryManager = queryManager ?? MockRepository.GenerateStub<IQueryManager>();
      commitRollbackAgent = commitRollbackAgent ?? MockRepository.GenerateStub<ICommitRollbackAgent>();
      extensions = extensions ?? Enumerable.Empty<IClientTransactionExtension>();
      
      var componentFactoryStub = MockRepository.GenerateStub<IClientTransactionComponentFactory>();
      componentFactoryStub.Stub (stub => stub.CreateApplicationData (Arg<ClientTransaction>.Is.Anything)).Return (applicationData);
      componentFactoryStub
          .Stub (stub => stub.CreateEventBroker (Arg<ClientTransaction>.Is.Anything))
          .Return (eventBroker);
      componentFactoryStub
          .Stub (stub => stub.CreateTransactionHierarchyManager (Arg<ClientTransaction>.Is.Anything, Arg<IClientTransactionEventSink>.Is.Anything))
          .Return (transactionHierarchyManager);
      componentFactoryStub.Stub (stub => stub.CreateEnlistedObjectManager (Arg<ClientTransaction>.Is.Anything)).Return (enlistedDomainObjectManager);
      componentFactoryStub
          .Stub (stub => stub.CreateInvalidDomainObjectManager (Arg<ClientTransaction>.Is.Anything, Arg<IClientTransactionEventSink>.Is.Anything))
          .Return (invalidDomainObjectManager);
      componentFactoryStub.Stub (stub => stub.CreatePersistenceStrategy (Arg<ClientTransaction>.Is.Anything)).Return (persistenceStrategy);
      componentFactoryStub
          .Stub (stub => stub.CreateDataManager(
              Arg<ClientTransaction>.Is.Anything,
              Arg<IClientTransactionEventSink>.Is.Anything, 
              Arg<IInvalidDomainObjectManager>.Is.Anything, 
              Arg<IPersistenceStrategy>.Is.Anything, 
              Arg<ITransactionHierarchyManager>.Is.Anything))
          .Return (dataManager);
      componentFactoryStub
          .Stub (
              stub =>
              stub.CreateObjectLifetimeAgent (
                  Arg<ClientTransaction>.Is.Anything,
                  Arg<IClientTransactionEventSink>.Is.Anything,
                  Arg<IInvalidDomainObjectManager>.Is.Anything,
                  Arg<IDataManager>.Is.Anything,
                  Arg<IEnlistedDomainObjectManager>.Is.Anything, 
                  Arg<IPersistenceStrategy>.Is.Anything))
          .Return (objectLifetimeAgent);
      componentFactoryStub
          .Stub (stub => stub.CreateQueryManager (
              Arg<ClientTransaction>.Is.Anything,
              Arg<IClientTransactionEventSink>.Is.Anything,
              Arg<IInvalidDomainObjectManager>.Is.Anything,
              Arg<IPersistenceStrategy>.Is.Anything,
              Arg<IDataManager>.Is.Anything,
              Arg<ITransactionHierarchyManager>.Is.Anything))
          .Return (queryManager);
      componentFactoryStub
          .Stub (stub => stub.CreateCommitRollbackAgent (
              Arg<ClientTransaction>.Is.Anything,
              Arg<IClientTransactionEventSink>.Is.Anything,
              Arg<IPersistenceStrategy>.Is.Anything,
              Arg<IDataManager>.Is.Anything))
          .Return (commitRollbackAgent);
      componentFactoryStub.Stub (stub => stub.CreateExtensions (Arg<ClientTransaction>.Is.Anything)).Return (extensions);
      return componentFactoryStub;
    }

    public static ClientTransaction CreateWithCustomListeners (params IClientTransactionListener[] listeners)
    {
      var componentFactoryPartialMock = MockRepository.GeneratePartialMock<RootClientTransactionComponentFactory>();
      componentFactoryPartialMock
          .Stub (stub => PrivateInvoke.InvokeNonPublicMethod (stub, "CreateListeners", Arg<ClientTransaction>.Is.Anything))
          .Return (listeners);
      componentFactoryPartialMock.Replay ();

      return CreateWithComponents<ClientTransaction> (componentFactoryPartialMock);
    }

    public static ClientTransaction CreateWithParent (ClientTransaction parent)
    {
      var hierarchyManagerStub = MockRepository.GenerateStub<ITransactionHierarchyManager>();
      hierarchyManagerStub.Stub (stub => stub.ParentTransaction).Return (parent);
      return CreateWithComponents<ClientTransaction> (transactionHierarchyManager: hierarchyManagerStub);
    }

    public static ClientTransaction CreateWithSub (ClientTransaction sub)
    {
      var hierarchyManagerStub = MockRepository.GenerateStub<ITransactionHierarchyManager>();
      hierarchyManagerStub.Stub (stub => stub.SubTransaction).Return (sub);
      hierarchyManagerStub.Stub (stub => stub.IsWriteable).Return (false);
      return CreateWithComponents<ClientTransaction> (transactionHierarchyManager: hierarchyManagerStub);
    }
  }
}