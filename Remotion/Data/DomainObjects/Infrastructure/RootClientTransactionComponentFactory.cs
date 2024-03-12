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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.FunctionalProgramming;
using Remotion.Mixins;
using Remotion.TypePipe;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Creates all parts necessary to construct a <see cref="ClientTransaction"/> with root-transaction semantics.
  /// </summary>
  [Serializable]
  public class RootClientTransactionComponentFactory : ClientTransactionComponentFactoryBase
  {
    private readonly IStorageSettings _storageSettings;
    private readonly IPersistenceManager _persistenceManager;
    private readonly IPersistenceExtensionFactory _persistenceExtensionFactory;

    public static RootClientTransactionComponentFactory Create (
            IStorageSettings storageSettings,
            IPersistenceManager persistenceManager,
            IPersistenceExtensionFactory persistenceExtensionFactory)
    {
      ArgumentUtility.CheckNotNull("storageSettings", storageSettings);
      ArgumentUtility.CheckNotNull("persistenceManager", persistenceManager);
      ArgumentUtility.CheckNotNull("persistenceExtensionFactory", persistenceExtensionFactory);

      return ObjectFactory.Create<RootClientTransactionComponentFactory>(
          true,
          ParamList.Create(
              storageSettings,
              persistenceManager,
              persistenceExtensionFactory));
    }

    protected RootClientTransactionComponentFactory (
        IStorageSettings storageSettings,
        IPersistenceManager persistenceManager,
        IPersistenceExtensionFactory persistenceExtensionFactory)
    {
      ArgumentUtility.CheckNotNull("storageSettings", storageSettings);
      ArgumentUtility.CheckNotNull("persistenceManager", persistenceManager);
      ArgumentUtility.CheckNotNull("persistenceExtensionFactory", persistenceExtensionFactory);

      _storageSettings = storageSettings;
      _persistenceManager = persistenceManager;
      _persistenceExtensionFactory = persistenceExtensionFactory;
    }

    public override ITransactionHierarchyManager CreateTransactionHierarchyManager (
        ClientTransaction constructedTransaction,
        IClientTransactionEventSink eventSink)
    {
      ArgumentUtility.CheckNotNull("constructedTransaction", constructedTransaction);
      return new TransactionHierarchyManager(constructedTransaction, eventSink);
    }

    public override IDictionary<Enum, object> CreateApplicationData (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull("constructedTransaction", constructedTransaction);
      return new Dictionary<Enum, object>();
    }

    public override IEnlistedDomainObjectManager CreateEnlistedObjectManager (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull("constructedTransaction", constructedTransaction);
      return new DictionaryBasedEnlistedDomainObjectManager();
    }

    public override IInvalidDomainObjectManager CreateInvalidDomainObjectManager (
        ClientTransaction constructedTransaction, IClientTransactionEventSink eventSink)
    {
      ArgumentUtility.CheckNotNull("constructedTransaction", constructedTransaction);
      ArgumentUtility.CheckNotNull("eventSink", eventSink);
      return new InvalidDomainObjectManager(eventSink);
    }

    public override IPersistenceStrategy CreatePersistenceStrategy (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull("constructedTransaction", constructedTransaction);
      return ObjectFactory.Create<RootPersistenceStrategy>(
          true,
          ParamList.Create(
              constructedTransaction.ID,
              _storageSettings,
              _persistenceManager,
              _persistenceExtensionFactory)
          );
    }

    protected override IEnumerable<IClientTransactionListener> CreateListeners (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull("constructedTransaction", constructedTransaction);
      return base.CreateListeners(constructedTransaction)
          .Concat(new NotFoundObjectsClientTransactionListener());
    }

    protected override IRelationEndPointManager CreateRelationEndPointManager (
        ClientTransaction constructedTransaction,
        IRelationEndPointProvider endPointProvider,
        ILazyLoader lazyLoader,
        IClientTransactionEventSink eventSink,
        IDataContainerMapReadOnlyView dataContainerMap)
    {
      ArgumentUtility.CheckNotNull("constructedTransaction", constructedTransaction);
      ArgumentUtility.CheckNotNull("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull("lazyLoader", lazyLoader);
      ArgumentUtility.CheckNotNull("eventSink", eventSink);
      ArgumentUtility.CheckNotNull("dataContainerMap", dataContainerMap);

      var domainObjectCollectionEndPointChangeDetectionStrategy = new RootDomainObjectCollectionEndPointChangeDetectionStrategy();
      var domainObjectCollectionEndPointDataManagerFactory = new DomainObjectCollectionEndPointDataManagerFactory(domainObjectCollectionEndPointChangeDetectionStrategy);
      var virtualCollectionEndPointDataManagerFactory = new VirtualCollectionEndPointDataManagerFactory(dataContainerMap);
      var virtualObjectEndPointDataManagerFactory = new VirtualObjectEndPointDataManagerFactory();

      var relationEndPointFactory = CreateRelationEndPointFactory(
          constructedTransaction,
          endPointProvider,
          lazyLoader,
          eventSink,
          virtualObjectEndPointDataManagerFactory,
          domainObjectCollectionEndPointDataManagerFactory,
          virtualCollectionEndPointDataManagerFactory);
      var virtualEndPointStateUpdateListener = new VirtualEndPointStateUpdateListener(eventSink);
      var stateUpdateRaisingRelationEndPointFactory = new StateUpdateRaisingRelationEndPointFactoryDecorator(
          relationEndPointFactory,
          virtualEndPointStateUpdateListener);

      var relationEndPointRegistrationAgent = new RootRelationEndPointRegistrationAgent(endPointProvider);
      return new RelationEndPointManager(
          constructedTransaction, lazyLoader, eventSink, stateUpdateRaisingRelationEndPointFactory, relationEndPointRegistrationAgent);
    }

    protected virtual RelationEndPointFactory CreateRelationEndPointFactory (
        ClientTransaction constructedTransaction,
        IRelationEndPointProvider endPointProvider,
        ILazyLoader lazyLoader,
        IClientTransactionEventSink eventSink,
        IVirtualObjectEndPointDataManagerFactory virtualObjectEndPointDataManagerFactory,
        IDomainObjectCollectionEndPointDataManagerFactory domainObjectCollectionEndPointDataManagerFactory,
        IVirtualCollectionEndPointDataManagerFactory virtualCollectionEndPointDataManagerFactory)
    {
      ArgumentUtility.CheckNotNull("constructedTransaction", constructedTransaction);
      ArgumentUtility.CheckNotNull("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull("lazyLoader", lazyLoader);
      ArgumentUtility.CheckNotNull("eventSink", eventSink);
      ArgumentUtility.CheckNotNull("virtualObjectEndPointDataManagerFactory", virtualObjectEndPointDataManagerFactory);
      ArgumentUtility.CheckNotNull("domainObjectCollectionEndPointDataManagerFactory", domainObjectCollectionEndPointDataManagerFactory);

      var associatedDomainObjectCollectionDataStrategyFactory = new AssociatedDomainObjectCollectionDataStrategyFactory(endPointProvider);
      var domainObjectCollectionEndPointCollectionProvider = new DomainObjectCollectionEndPointCollectionProvider(associatedDomainObjectCollectionDataStrategyFactory);
      var virtualCollectionEndPointCollectionProvider = new VirtualCollectionEndPointCollectionProvider(endPointProvider);

      return new RelationEndPointFactory(
          constructedTransaction,
          endPointProvider,
          lazyLoader,
          eventSink,
          virtualObjectEndPointDataManagerFactory,
          domainObjectCollectionEndPointDataManagerFactory,
          domainObjectCollectionEndPointCollectionProvider,
          associatedDomainObjectCollectionDataStrategyFactory,
          virtualCollectionEndPointCollectionProvider,
          virtualCollectionEndPointDataManagerFactory);
    }

    protected override IObjectLoader CreateObjectLoader (
        ClientTransaction constructedTransaction,
        IClientTransactionEventSink eventSink,
        IPersistenceStrategy persistenceStrategy,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        IDataManager dataManager,
        ITransactionHierarchyManager hierarchyManager)
    {
      ArgumentUtility.CheckNotNull("constructedTransaction", constructedTransaction);
      ArgumentUtility.CheckNotNull("eventSink", eventSink);
      var fetchEnabledPersistenceStrategy =
          ArgumentUtility.CheckNotNullAndType<IFetchEnabledPersistenceStrategy>("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull("invalidDomainObjectManager", invalidDomainObjectManager);
      ArgumentUtility.CheckNotNull("dataManager", dataManager);
      ArgumentUtility.CheckNotNull("hierarchyManager", hierarchyManager);

      var loadedObjectDataProvider = new LoadedObjectDataProvider(dataManager, invalidDomainObjectManager);
      var registrationListener = new LoadedObjectDataRegistrationListener(eventSink, hierarchyManager);
      var loadedObjectDataRegistrationAgent = new LoadedObjectDataRegistrationAgent(constructedTransaction, dataManager, registrationListener);

      IFetchedRelationDataRegistrationAgent registrationAgent =
          new DelegatingFetchedRelationDataRegistrationAgent(
              new FetchedRealObjectRelationDataRegistrationAgent(),
              new FetchedVirtualObjectRelationDataRegistrationAgent(dataManager),
              new FetchedCollectionRelationDataRegistrationAgent(dataManager));
      var eagerFetcher = new EagerFetcher(registrationAgent);

      return new FetchEnabledObjectLoader(
          fetchEnabledPersistenceStrategy,
          loadedObjectDataRegistrationAgent,
          loadedObjectDataProvider,
          eagerFetcher);
    }
  }
}
