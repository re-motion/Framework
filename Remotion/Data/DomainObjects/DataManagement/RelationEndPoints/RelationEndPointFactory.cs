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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Creates <see cref="IRelationEndPoint"/> instances.
  /// </summary>
  [Serializable]
  public class RelationEndPointFactory : IRelationEndPointFactory
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly IRelationEndPointProvider _endPointProvider;
    private readonly ILazyLoader _lazyLoader;
    private readonly IClientTransactionEventSink _transactionEventSink;
    private readonly IVirtualObjectEndPointDataManagerFactory _virtualObjectEndPointDataManagerFactory;
    private readonly ICollectionEndPointDataManagerFactory _collectionEndPointDataManagerFactory;
    private readonly ICollectionEndPointCollectionProvider _collectionEndPointCollectionProvider;
    private readonly IAssociatedCollectionDataStrategyFactory _associatedCollectionDataStrategyFactory;

    public RelationEndPointFactory (
        ClientTransaction clientTransaction,
        IRelationEndPointProvider endPointProvider,
        ILazyLoader lazyLoader,
        IClientTransactionEventSink transactionEventSink,
        IVirtualObjectEndPointDataManagerFactory virtualObjectEndPointDataManagerFactory,
        ICollectionEndPointDataManagerFactory collectionEndPointDataManagerFactory, 
        ICollectionEndPointCollectionProvider collectionEndPointCollectionProvider, 
        IAssociatedCollectionDataStrategyFactory associatedCollectionDataStrategyFactory)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader);
      ArgumentUtility.CheckNotNull ("transactionEventSink", transactionEventSink);
      ArgumentUtility.CheckNotNull ("virtualObjectEndPointDataManagerFactory", virtualObjectEndPointDataManagerFactory);
      ArgumentUtility.CheckNotNull ("collectionEndPointDataManagerFactory", collectionEndPointDataManagerFactory);
      ArgumentUtility.CheckNotNull ("collectionEndPointCollectionProvider", collectionEndPointCollectionProvider);
      ArgumentUtility.CheckNotNull ("associatedCollectionDataStrategyFactory", associatedCollectionDataStrategyFactory);

      _clientTransaction = clientTransaction;
      _endPointProvider = endPointProvider;
      _lazyLoader = lazyLoader;
      _transactionEventSink = transactionEventSink;
      _virtualObjectEndPointDataManagerFactory = virtualObjectEndPointDataManagerFactory;
      _collectionEndPointDataManagerFactory = collectionEndPointDataManagerFactory;
      _collectionEndPointCollectionProvider = collectionEndPointCollectionProvider;
      _associatedCollectionDataStrategyFactory = associatedCollectionDataStrategyFactory;
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public IRelationEndPointProvider EndPointProvider
    {
      get { return _endPointProvider; }
    }

    public ILazyLoader LazyLoader
    {
      get { return _lazyLoader; }
    }

    public IClientTransactionEventSink TransactionEventSink
    {
      get { return _transactionEventSink; }
    }

    public IVirtualObjectEndPointDataManagerFactory VirtualObjectEndPointDataManagerFactory
    {
      get { return _virtualObjectEndPointDataManagerFactory; }
    }

    public ICollectionEndPointDataManagerFactory CollectionEndPointDataManagerFactory
    {
      get { return _collectionEndPointDataManagerFactory; }
    }

    public ICollectionEndPointCollectionProvider CollectionEndPointCollectionProvider
    {
      get { return _collectionEndPointCollectionProvider; }
    }

    public IAssociatedCollectionDataStrategyFactory AssociatedCollectionDataStrategyFactory
    {
      get { return _associatedCollectionDataStrategyFactory; }
    }

    public IRealObjectEndPoint CreateRealObjectEndPoint (RelationEndPointID endPointID, DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      return new RealObjectEndPoint (_clientTransaction, endPointID, dataContainer, _endPointProvider, _transactionEventSink);
    }

    public IVirtualObjectEndPoint CreateVirtualObjectEndPoint (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      var virtualObjectEndPoint = new VirtualObjectEndPoint (
          _clientTransaction,
          endPointID,
          _lazyLoader,
          _endPointProvider,
          _transactionEventSink,
          _virtualObjectEndPointDataManagerFactory);
      return virtualObjectEndPoint;
    }

    public ICollectionEndPoint CreateCollectionEndPoint (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      var collectionEndPoint = new CollectionEndPoint (
          _clientTransaction,
          endPointID,
          new CollectionEndPointCollectionManager (endPointID, _collectionEndPointCollectionProvider, _associatedCollectionDataStrategyFactory),
          _lazyLoader,
          _endPointProvider,
          _transactionEventSink,
          _collectionEndPointDataManagerFactory);
      return collectionEndPoint;
    }
  }
}