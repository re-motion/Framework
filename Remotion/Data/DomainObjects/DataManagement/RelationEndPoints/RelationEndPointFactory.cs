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
  public class RelationEndPointFactory : IRelationEndPointFactory
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly IRelationEndPointProvider _endPointProvider;
    private readonly ILazyLoader _lazyLoader;
    private readonly IClientTransactionEventSink _transactionEventSink;
    private readonly IVirtualObjectEndPointDataManagerFactory _virtualObjectEndPointDataManagerFactory;
    private readonly IDomainObjectCollectionEndPointDataManagerFactory _domainObjectCollectionEndPointDataManagerFactory;
    private readonly IDomainObjectCollectionEndPointCollectionProvider _domainObjectCollectionEndPointCollectionProvider;
    private readonly IAssociatedDomainObjectCollectionDataStrategyFactory _associatedDomainObjectCollectionDataStrategyFactory;
    private readonly IVirtualCollectionEndPointCollectionProvider _virtualCollectionEndPointCollectionProvider;
    private readonly IVirtualCollectionEndPointDataManagerFactory _virtualCollectionEndPointDataManagerFactory;

    public RelationEndPointFactory (
        ClientTransaction clientTransaction,
        IRelationEndPointProvider endPointProvider,
        ILazyLoader lazyLoader,
        IClientTransactionEventSink transactionEventSink,
        IVirtualObjectEndPointDataManagerFactory virtualObjectEndPointDataManagerFactory,
        IDomainObjectCollectionEndPointDataManagerFactory domainObjectCollectionEndPointDataManagerFactory,
        IDomainObjectCollectionEndPointCollectionProvider domainObjectCollectionEndPointCollectionProvider,
        IAssociatedDomainObjectCollectionDataStrategyFactory associatedDomainObjectCollectionDataStrategyFactory,
        IVirtualCollectionEndPointCollectionProvider virtualCollectionEndPointCollectionProvider,
        IVirtualCollectionEndPointDataManagerFactory virtualCollectionEndPointDataManagerFactory)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull("lazyLoader", lazyLoader);
      ArgumentUtility.CheckNotNull("transactionEventSink", transactionEventSink);
      ArgumentUtility.CheckNotNull("virtualObjectEndPointDataManagerFactory", virtualObjectEndPointDataManagerFactory);
      ArgumentUtility.CheckNotNull("domainObjectCollectionEndPointDataManagerFactory", domainObjectCollectionEndPointDataManagerFactory);
      ArgumentUtility.CheckNotNull("domainObjectCollectionEndPointCollectionProvider", domainObjectCollectionEndPointCollectionProvider);
      ArgumentUtility.CheckNotNull("associatedDomainObjectCollectionDataStrategyFactory", associatedDomainObjectCollectionDataStrategyFactory);
      ArgumentUtility.CheckNotNull("virtualCollectionEndPointCollectionProvider", virtualCollectionEndPointCollectionProvider);
      ArgumentUtility.CheckNotNull("virtualObjectEndPointDataManagerFactory", virtualObjectEndPointDataManagerFactory);

      _clientTransaction = clientTransaction;
      _endPointProvider = endPointProvider;
      _lazyLoader = lazyLoader;
      _transactionEventSink = transactionEventSink;
      _virtualObjectEndPointDataManagerFactory = virtualObjectEndPointDataManagerFactory;
      _domainObjectCollectionEndPointDataManagerFactory = domainObjectCollectionEndPointDataManagerFactory;
      _domainObjectCollectionEndPointCollectionProvider = domainObjectCollectionEndPointCollectionProvider;
      _associatedDomainObjectCollectionDataStrategyFactory = associatedDomainObjectCollectionDataStrategyFactory;
      _virtualCollectionEndPointCollectionProvider = virtualCollectionEndPointCollectionProvider;
      _virtualCollectionEndPointDataManagerFactory = virtualCollectionEndPointDataManagerFactory;
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

    public IDomainObjectCollectionEndPointDataManagerFactory DomainObjectCollectionEndPointDataManagerFactory
    {
      get { return _domainObjectCollectionEndPointDataManagerFactory; }
    }

    public IDomainObjectCollectionEndPointCollectionProvider DomainObjectCollectionEndPointCollectionProvider
    {
      get { return _domainObjectCollectionEndPointCollectionProvider; }
    }

    public IAssociatedDomainObjectCollectionDataStrategyFactory AssociatedDomainObjectCollectionDataStrategyFactory
    {
      get { return _associatedDomainObjectCollectionDataStrategyFactory; }
    }

    public IVirtualCollectionEndPointCollectionProvider VirtualCollectionEndPointCollectionProvider
    {
      get { return _virtualCollectionEndPointCollectionProvider; }
    }

    public IVirtualCollectionEndPointDataManagerFactory VirtualCollectionEndPointDataManagerFactory
    {
      get { return _virtualCollectionEndPointDataManagerFactory; }
    }

    public IRealObjectEndPoint CreateRealObjectEndPoint (RelationEndPointID endPointID, DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull("endPointID", endPointID);
      ArgumentUtility.CheckNotNull("dataContainer", dataContainer);
      if (endPointID.ObjectID == null)
        throw new ArgumentException("End point ID must have a non-null ObjectID.", "endPointID");

      return new RealObjectEndPoint(_clientTransaction, endPointID, dataContainer, _endPointProvider, _transactionEventSink);
    }

    public IVirtualObjectEndPoint CreateVirtualObjectEndPoint (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull("endPointID", endPointID);

      var virtualObjectEndPoint = new VirtualObjectEndPoint(
          _clientTransaction,
          endPointID,
          _lazyLoader,
          _endPointProvider,
          _transactionEventSink,
          _virtualObjectEndPointDataManagerFactory);
      return virtualObjectEndPoint;
    }

    public IVirtualCollectionEndPoint CreateVirtualCollectionEndPoint (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull("endPointID", endPointID);

      var collectionEndPoint = new VirtualCollectionEndPoint(
          _clientTransaction,
          endPointID,
          new VirtualCollectionEndPointCollectionManager(endPointID, _virtualCollectionEndPointCollectionProvider),
          _lazyLoader,
          _endPointProvider,
          _transactionEventSink,
          _virtualCollectionEndPointDataManagerFactory);
      return collectionEndPoint;
    }

    public IDomainObjectCollectionEndPoint CreateDomainObjectCollectionEndPoint (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull("endPointID", endPointID);

      var collectionEndPoint = new DomainObjectCollectionEndPoint(
          _clientTransaction,
          endPointID,
          new DomainObjectCollectionEndPointCollectionManager(endPointID, _domainObjectCollectionEndPointCollectionProvider, _associatedDomainObjectCollectionDataStrategyFactory),
          _lazyLoader,
          _endPointProvider,
          _transactionEventSink,
          _domainObjectCollectionEndPointDataManagerFactory);
      return collectionEndPoint;
    }
  }
}
