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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Holds common code required to implement the <see cref="IClientTransactionComponentFactory"/> interface.
  /// </summary>
  [Serializable]
  public abstract class ClientTransactionComponentFactoryBase : IClientTransactionComponentFactory
  {
    public abstract ITransactionHierarchyManager CreateTransactionHierarchyManager (
        ClientTransaction constructedTransaction, IClientTransactionEventSink eventSink);

    public abstract IDictionary<Enum, object> CreateApplicationData (ClientTransaction constructedTransaction);
    public abstract IEnlistedDomainObjectManager CreateEnlistedObjectManager (ClientTransaction constructedTransaction);

    public abstract IInvalidDomainObjectManager CreateInvalidDomainObjectManager (
        ClientTransaction constructedTransaction, IClientTransactionEventSink eventSink);

    public abstract IPersistenceStrategy CreatePersistenceStrategy (ClientTransaction constructedTransaction);

    protected abstract IRelationEndPointManager CreateRelationEndPointManager (
        ClientTransaction constructedTransaction,
        IRelationEndPointProvider endPointProvider,
        ILazyLoader lazyLoader,
        IClientTransactionEventSink eventSink,
        IDataContainerMapReadOnlyView dataContainerMap);

    protected abstract IObjectLoader CreateObjectLoader (
        ClientTransaction constructedTransaction,
        IClientTransactionEventSink eventSink,
        IPersistenceStrategy persistenceStrategy,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        IDataManager dataManager,
        ITransactionHierarchyManager hierarchyManager);

    public virtual IClientTransactionEventBroker CreateEventBroker (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull("constructedTransaction", constructedTransaction);

      var listenerManager = new ClientTransactionEventBroker(constructedTransaction);
      foreach (var listener in CreateListeners(constructedTransaction))
        listenerManager.AddListener(listener);
      return listenerManager;
    }

    protected virtual IEnumerable<IClientTransactionListener> CreateListeners (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull("constructedTransaction", constructedTransaction);
      yield return new LoggingClientTransactionListener();
    }

    public virtual IDataManager CreateDataManager (
        ClientTransaction constructedTransaction,
        IClientTransactionEventSink eventSink,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        IPersistenceStrategy persistenceStrategy,
        ITransactionHierarchyManager hierarchyManager)
    {
      ArgumentUtility.CheckNotNull("constructedTransaction", constructedTransaction);
      ArgumentUtility.CheckNotNull("eventSink", eventSink);
      ArgumentUtility.CheckNotNull("invalidDomainObjectManager", invalidDomainObjectManager);
      ArgumentUtility.CheckNotNull("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull("hierarchyManager", hierarchyManager);

      var dataContainerEventListener = CreateDataContainerEventListener(eventSink);

      var delegatingDataManager = new DelegatingDataManager();
      var delegatingDataContainerMap = new DelegatingDataContainerMap();
      var objectLoader = CreateObjectLoader(
          constructedTransaction, eventSink, persistenceStrategy, invalidDomainObjectManager, delegatingDataManager, hierarchyManager);

      var relationEndPointProvider = GetEndPointProvider(delegatingDataManager);
      var lazyLoader = GetLazyLoader(delegatingDataManager);

      var endPointManager = CreateRelationEndPointManager(
          constructedTransaction,
          relationEndPointProvider,
          lazyLoader,
          eventSink,
          delegatingDataContainerMap);

      var dataManager = new DataManager(
          constructedTransaction, eventSink, dataContainerEventListener, invalidDomainObjectManager, objectLoader, endPointManager);
      delegatingDataManager.InnerDataManager = dataManager;
      delegatingDataContainerMap.InnerDataContainerMap = dataManager.DataContainers;
      return dataManager;
    }

    public virtual IObjectLifetimeAgent CreateObjectLifetimeAgent (
        ClientTransaction constructedTransaction,
        IClientTransactionEventSink eventSink,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        IDataManager dataManager,
        IEnlistedDomainObjectManager enlistedDomainObjectManager,
        IPersistenceStrategy persistenceStrategy)
    {
      ArgumentUtility.CheckNotNull("constructedTransaction", constructedTransaction);
      ArgumentUtility.CheckNotNull("eventSink", eventSink);
      ArgumentUtility.CheckNotNull("invalidDomainObjectManager", invalidDomainObjectManager);
      ArgumentUtility.CheckNotNull("dataManager", dataManager);
      ArgumentUtility.CheckNotNull("enlistedDomainObjectManager", enlistedDomainObjectManager);
      ArgumentUtility.CheckNotNull("persistenceStrategy", persistenceStrategy);

      return new ObjectLifetimeAgent(
          constructedTransaction, eventSink, invalidDomainObjectManager, dataManager, enlistedDomainObjectManager, persistenceStrategy);
    }

    public virtual IQueryManager CreateQueryManager (
        ClientTransaction constructedTransaction,
        IClientTransactionEventSink eventSink,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        IPersistenceStrategy persistenceStrategy,
        IDataManager dataManager,
        ITransactionHierarchyManager hierarchyManager)
    {
      ArgumentUtility.CheckNotNull("constructedTransaction", constructedTransaction);
      ArgumentUtility.CheckNotNull("eventSink", eventSink);
      ArgumentUtility.CheckNotNull("invalidDomainObjectManager", invalidDomainObjectManager);
      ArgumentUtility.CheckNotNull("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull("dataManager", dataManager);

      var objectLoader = CreateObjectLoader(
          constructedTransaction, eventSink, persistenceStrategy, invalidDomainObjectManager, dataManager, hierarchyManager);
      return new QueryManager(persistenceStrategy, objectLoader, eventSink);
    }

    public virtual ICommitRollbackAgent CreateCommitRollbackAgent (
        ClientTransaction constructedTransaction,
        IClientTransactionEventSink eventSink,
        IPersistenceStrategy persistenceStrategy,
        IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull("constructedTransaction", constructedTransaction);
      ArgumentUtility.CheckNotNull("eventSink", eventSink);
      ArgumentUtility.CheckNotNull("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull("dataManager", dataManager);

      return new CommitRollbackAgent(constructedTransaction, eventSink, persistenceStrategy, dataManager);
    }

    public virtual IEnumerable<IClientTransactionExtension> CreateExtensions (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull("constructedTransaction", constructedTransaction);

      //TODO: Serialize
      var extensionFactories = SafeServiceLocator.Current.GetInstance<IClientTransactionExtensionFactory>();
      return extensionFactories.CreateClientTransactionExtensions(constructedTransaction);
    }

    protected virtual IDataContainerEventListener CreateDataContainerEventListener (IClientTransactionEventSink eventSink)
    {
      ArgumentUtility.CheckNotNull("eventSink", eventSink);
      return new DataContainerEventListener(eventSink);
    }

    protected virtual ILazyLoader GetLazyLoader (IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull("dataManager", dataManager);
      return dataManager;
    }

    protected virtual IRelationEndPointProvider GetEndPointProvider (IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull("dataManager", dataManager);
      return dataManager;
    }
  }
}
