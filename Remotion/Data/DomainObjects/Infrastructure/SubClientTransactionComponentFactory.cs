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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Mixins;
using Remotion.TypePipe;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Creates all parts necessary to construct a <see cref="ClientTransaction"/> with sub-transaction semantics.
  /// </summary>
  public class SubClientTransactionComponentFactory : ClientTransactionComponentFactoryBase
  {
    public static SubClientTransactionComponentFactory Create (
        ClientTransaction parentTransaction,
        IInvalidDomainObjectManager parentInvalidDomainObjectManager,
        IEnlistedDomainObjectManager parentEnlistedDomainObjectManager,
        ITransactionHierarchyManager parentTransactionHierarchyManager,
        IClientTransactionEventSink parentEventSink)
    {
      return ObjectFactory.Create<SubClientTransactionComponentFactory>(
          true,
          ParamList.Create(
              parentTransaction,
              parentInvalidDomainObjectManager,
              parentEnlistedDomainObjectManager,
              parentTransactionHierarchyManager,
              parentEventSink));
    }

    private readonly ClientTransaction _parentTransaction;
    private readonly IInvalidDomainObjectManager _parentInvalidDomainObjectManager;
    private readonly IEnlistedDomainObjectManager _parentEnlistedDomainObjectManager;
    private readonly ITransactionHierarchyManager _parentHierarchyManager;
    private readonly IClientTransactionEventSink _parentEventSink;

    protected SubClientTransactionComponentFactory (
        ClientTransaction parentTransaction,
        IInvalidDomainObjectManager parentInvalidDomainObjectManager,
        IEnlistedDomainObjectManager parentEnlistedDomainObjectManager,
        ITransactionHierarchyManager parentHierarchyManager,
        IClientTransactionEventSink parentEventSink)
    {
      ArgumentUtility.CheckNotNull("parentTransaction", parentTransaction);
      ArgumentUtility.CheckNotNull("parentInvalidDomainObjectManager", parentInvalidDomainObjectManager);
      ArgumentUtility.CheckNotNull("parentEnlistedDomainObjectManager", parentEnlistedDomainObjectManager);
      ArgumentUtility.CheckNotNull("parentHierarchyManager", parentHierarchyManager);
      ArgumentUtility.CheckNotNull("parentEventSink", parentEventSink);

      _parentTransaction = parentTransaction;
      _parentInvalidDomainObjectManager = parentInvalidDomainObjectManager;
      _parentEnlistedDomainObjectManager = parentEnlistedDomainObjectManager;
      _parentHierarchyManager = parentHierarchyManager;
      _parentEventSink = parentEventSink;
    }

    public override ITransactionHierarchyManager CreateTransactionHierarchyManager (ClientTransaction constructedTransaction, IClientTransactionEventSink eventSink)
    {
      ArgumentUtility.CheckNotNull("constructedTransaction", constructedTransaction);
      ArgumentUtility.CheckNotNull("eventSink", eventSink);
      return new TransactionHierarchyManager(constructedTransaction, eventSink, _parentTransaction, _parentHierarchyManager, _parentEventSink);
    }

    public override IDictionary<Enum, object> CreateApplicationData (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull("constructedTransaction", constructedTransaction);
      return _parentTransaction.ApplicationData;
    }

    public override IEnlistedDomainObjectManager CreateEnlistedObjectManager (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull("constructedTransaction", constructedTransaction);
      return _parentEnlistedDomainObjectManager;
    }

    public override IInvalidDomainObjectManager CreateInvalidDomainObjectManager (
        ClientTransaction constructedTransaction, IClientTransactionEventSink eventSink)
    {
      ArgumentUtility.CheckNotNull("constructedTransaction", constructedTransaction);
      ArgumentUtility.CheckNotNull("eventSink", eventSink);

      var invalidObjects =
          _parentInvalidDomainObjectManager.InvalidObjectIDs.Select(id => _parentInvalidDomainObjectManager.GetInvalidObjectReference(id));

      var parentDataManager = _parentTransaction.DataManager;
      var deletedObjects = parentDataManager.DataContainers.Where(dc => dc.State.IsDeleted).Select(dc => dc.DomainObject);

      return new InvalidDomainObjectManager(eventSink, invalidObjects.Concat(deletedObjects));
    }

    public override IPersistenceStrategy CreatePersistenceStrategy (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull("constructedTransaction", constructedTransaction);

      var parentTransactionContext = new ParentTransactionContext(_parentTransaction, _parentInvalidDomainObjectManager);
      return ObjectFactory.Create<SubPersistenceStrategy>(true, ParamList.Create(parentTransactionContext));
    }

    protected override IRelationEndPointManager CreateRelationEndPointManager (
        ClientTransaction clientTransaction,
        IRelationEndPointProvider endPointProvider,
        ILazyLoader lazyLoader,
        IClientTransactionEventSink eventSink,
        IDataContainerMapReadOnlyView dataContainerMap)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull("lazyLoader", lazyLoader);
      ArgumentUtility.CheckNotNull("eventSink", eventSink);
      ArgumentUtility.CheckNotNull("dataContainerMap", dataContainerMap);

      var domainObjectCollectionEndPointChangeDetectionStrategy = new SubDomainObjectCollectionEndPointChangeDetectionStrategy();
      var domainObjectCollectionEndPointDataManagerFactory = new DomainObjectCollectionEndPointDataManagerFactory(domainObjectCollectionEndPointChangeDetectionStrategy);
      var virtualCollectionEndPointDataManagerFactory = new VirtualCollectionEndPointDataManagerFactory(dataContainerMap);
      var virtualObjectEndPointDataManagerFactory = new VirtualObjectEndPointDataManagerFactory();

      var relationEndPointFactory = CreateRelationEndPointFactory(
          clientTransaction,
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

      var relationEndPointRegistrationAgent = new RelationEndPointRegistrationAgent(endPointProvider);
      return new RelationEndPointManager(
          clientTransaction, lazyLoader, eventSink, stateUpdateRaisingRelationEndPointFactory, relationEndPointRegistrationAgent);
    }

    protected virtual RelationEndPointFactory CreateRelationEndPointFactory (
        ClientTransaction clientTransaction,
        IRelationEndPointProvider endPointProvider,
        ILazyLoader lazyLoader,
        IClientTransactionEventSink eventSink,
        IVirtualObjectEndPointDataManagerFactory virtualObjectEndPointDataManagerFactory,
        IDomainObjectCollectionEndPointDataManagerFactory domainObjectCollectionEndPointDataManagerFactory,
        IVirtualCollectionEndPointDataManagerFactory virtualCollectionEndPointDataManagerFactory)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull("lazyLoader", lazyLoader);
      ArgumentUtility.CheckNotNull("eventSink", eventSink);
      ArgumentUtility.CheckNotNull("virtualObjectEndPointDataManagerFactory", virtualObjectEndPointDataManagerFactory);
      ArgumentUtility.CheckNotNull("domainObjectCollectionEndPointDataManagerFactory", domainObjectCollectionEndPointDataManagerFactory);

      var associatedDomainObjectCollectionDataStrategyFactory = new AssociatedDomainObjectCollectionDataStrategyFactory(endPointProvider);
      var domainObjectCollectionEndPointCollectionProvider = new DomainObjectCollectionEndPointCollectionProvider(associatedDomainObjectCollectionDataStrategyFactory);
      var virtualCollectionEndPointCollectionProvider = new VirtualCollectionEndPointCollectionProvider(endPointProvider);

      return new RelationEndPointFactory(
          clientTransaction,
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
      ArgumentUtility.CheckNotNull("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull("invalidDomainObjectManager", invalidDomainObjectManager);
      ArgumentUtility.CheckNotNull("dataManager", dataManager);
      ArgumentUtility.CheckNotNull("hierarchyManager", hierarchyManager);

      return CreateBasicObjectLoader(constructedTransaction, eventSink, persistenceStrategy, invalidDomainObjectManager, dataManager, hierarchyManager);
    }

    protected virtual IObjectLoader CreateBasicObjectLoader (
        ClientTransaction constructedTransaction,
        IClientTransactionEventSink eventSink,
        IPersistenceStrategy persistenceStrategy,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        IDataManager dataManager,
        ITransactionHierarchyManager hierarchyManager)
    {
      ArgumentUtility.CheckNotNull("constructedTransaction", constructedTransaction);
      ArgumentUtility.CheckNotNull("eventSink", eventSink);
      ArgumentUtility.CheckNotNull("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull("invalidDomainObjectManager", invalidDomainObjectManager);
      ArgumentUtility.CheckNotNull("dataManager", dataManager);
      ArgumentUtility.CheckNotNull("hierarchyManager", hierarchyManager);

      var loadedObjectDataProvider = new LoadedObjectDataProvider(dataManager, invalidDomainObjectManager);
      var registrationListener = new LoadedObjectDataRegistrationListener(eventSink, hierarchyManager);
      var loadedObjectDataRegistrationAgent = new LoadedObjectDataRegistrationAgent(constructedTransaction, dataManager, registrationListener);
      return new ObjectLoader(
          persistenceStrategy,
          loadedObjectDataRegistrationAgent,
          loadedObjectDataProvider);
    }
  }
}
