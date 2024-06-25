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
using System.Diagnostics.CodeAnalysis;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Represents an <see cref="ObjectEndPoint"/> that does not hold the foreign key in a relation. The <see cref="VirtualObjectEndPoint"/> is
  /// constructed by the <see cref="RelationEndPointManager"/> as an in-memory representation of the opposite of the <see cref="RealObjectEndPoint"/> 
  /// holding the foreign key.
  /// </summary>
  public class VirtualObjectEndPoint : ObjectEndPoint, IVirtualObjectEndPoint
  {
    public class EndPointLoader : IncompleteVirtualObjectEndPointLoadState.IEndPointLoader
    {
      private readonly ILazyLoader _lazyLoader;

      public EndPointLoader (ILazyLoader lazyLoader)
      {
        ArgumentUtility.CheckNotNull("lazyLoader", lazyLoader);
        _lazyLoader = lazyLoader;
      }

      public ILazyLoader LazyLoader
      {
        get { return _lazyLoader; }
      }

      public IVirtualObjectEndPointLoadState LoadEndPointAndGetNewState (IVirtualObjectEndPoint endPoint)
      {
        var virtualObjectEndPoint = ArgumentUtility.CheckNotNullAndType<VirtualObjectEndPoint>("endPoint", endPoint);
        _lazyLoader.LoadLazyVirtualObjectEndPoint(virtualObjectEndPoint.ID);
        return virtualObjectEndPoint._loadState;
      }
    }

    private readonly ILazyLoader _lazyLoader;
    private readonly IRelationEndPointProvider _endPointProvider;
    private readonly IClientTransactionEventSink _transactionEventSink;
    private readonly IVirtualObjectEndPointDataManagerFactory _dataManagerFactory;

    private IVirtualObjectEndPointLoadState _loadState;

    private bool _hasBeenTouched;

    public VirtualObjectEndPoint (
        ClientTransaction clientTransaction,
        RelationEndPointID id,
        ILazyLoader lazyLoader,
        IRelationEndPointProvider endPointProvider,
        IClientTransactionEventSink transactionEventSink,
        IVirtualObjectEndPointDataManagerFactory dataManagerFactory)
        : base(
            ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction),
            ArgumentUtility.CheckNotNull("id", id))
    {
      ArgumentUtility.CheckNotNull("lazyLoader", lazyLoader);
      ArgumentUtility.CheckNotNull("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull("transactionEventSink", transactionEventSink);
      ArgumentUtility.CheckNotNull("dataManagerFactory", dataManagerFactory);

      if (!ID.Definition.IsVirtual)
        throw new ArgumentException("End point ID must refer to a virtual end point.", "id");

      _lazyLoader = lazyLoader;
      _endPointProvider = endPointProvider;
      _transactionEventSink = transactionEventSink;
      _dataManagerFactory = dataManagerFactory;

      SetIncompleteState();

      _hasBeenTouched = false;
    }

    public ILazyLoader LazyLoader
    {
      get { return _lazyLoader; }
    }

    public IRelationEndPointProvider EndPointProvider
    {
      get { return _endPointProvider; }
    }

    public IClientTransactionEventSink TransactionEventSink
    {
      get { return _transactionEventSink; }
    }

    public IVirtualObjectEndPointDataManagerFactory DataManagerFactory
    {
      get { return _dataManagerFactory; }
    }

    public override ObjectID? OppositeObjectID
    {
      get { return GetOppositeObject().GetSafeID(); }
    }

    DomainObject? IVirtualEndPoint<DomainObject?>.GetData ()
    {
      return GetOppositeObject();
    }

    public override ObjectID? OriginalOppositeObjectID
    {
      get { return GetOriginalOppositeObject().GetSafeID(); }
    }

    DomainObject? IVirtualEndPoint<DomainObject?>.GetOriginalData ()
    {
      return GetOriginalOppositeObject();
    }

    public override bool HasChanged
    {
      get { return _loadState.HasChanged(); }
    }

    public override bool HasBeenTouched
    {
      get { return _hasBeenTouched; }
    }

    public override bool IsDataComplete
    {
      get { return _loadState.IsDataComplete(); }
    }

    public override bool? IsSynchronized
    {
      get { return _loadState.IsSynchronized(this); }
    }

    public override DomainObject? GetOppositeObject ()
    {
      return _loadState.GetData(this);
    }

    public override DomainObject? GetOriginalOppositeObject ()
    {
      return _loadState.GetOriginalData(this);
    }

    public override void EnsureDataComplete ()
    {
      _loadState.EnsureDataComplete(this);
    }

    public override void Synchronize ()
    {
      _loadState.Synchronize(this);
    }

    public void SynchronizeOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);

      _loadState.SynchronizeOppositeEndPoint(this, oppositeEndPoint);
    }

    public void MarkDataComplete (DomainObject? item)
    {
      _loadState.MarkDataComplete(this, item, SetCompleteState);
    }

    public bool CanBeCollected
    {
      get { return _loadState.CanEndPointBeCollected(this); }
    }

    public bool CanBeMarkedIncomplete
    {
      get { return _loadState.CanDataBeMarkedIncomplete(this); }
    }

    public void MarkDataIncomplete ()
    {
      _loadState.MarkDataIncomplete(this, SetIncompleteState);
    }

    public void RegisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);
      _loadState.RegisterOriginalOppositeEndPoint(this, oppositeEndPoint);
    }

    public void UnregisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);
      _loadState.UnregisterOriginalOppositeEndPoint(this, oppositeEndPoint);
    }

    public void RegisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);
      _loadState.RegisterCurrentOppositeEndPoint(this, oppositeEndPoint);
    }

    public void UnregisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);
      _loadState.UnregisterCurrentOppositeEndPoint(this, oppositeEndPoint);
    }

    public override IDataManagementCommand CreateSetCommand (DomainObject? newRelatedObject)
    {
      var command = _loadState.CreateSetCommand(this, newRelatedObject);
      return command;
    }

    public override IDataManagementCommand CreateDeleteCommand ()
    {
      var command = _loadState.CreateDeleteCommand(this);
      return command;
    }

    public override void Touch ()
    {
      _hasBeenTouched = true;
    }

    public override void Commit ()
    {
      if (HasChanged)
      {
        _loadState.Commit(this);
      }

      _hasBeenTouched = false;
    }

    public override void Rollback ()
    {
      if (HasChanged)
      {
        _loadState.Rollback(this);
      }

      _hasBeenTouched = false;
    }

    protected override void SetOppositeObjectDataFromSubTransaction (IObjectEndPoint sourceObjectEndPoint)
    {
      var sourceVirtualObjectEndPoint = ArgumentUtility.CheckNotNullAndType<VirtualObjectEndPoint>("sourceObjectEndPoint", sourceObjectEndPoint);
      _loadState.SetDataFromSubTransaction(this, sourceVirtualObjectEndPoint._loadState);
    }

    [MemberNotNull(nameof(_loadState))]
    private void SetIncompleteState ()
    {
      var loader = new EndPointLoader(_lazyLoader);
      _loadState = new IncompleteVirtualObjectEndPointLoadState(loader, _dataManagerFactory);
    }

    [MemberNotNull(nameof(_loadState))]
    private void SetCompleteState (IVirtualObjectEndPointDataManager dataManager)
    {
      _loadState = new CompleteVirtualObjectEndPointLoadState(dataManager, EndPointProvider, _transactionEventSink);
    }
  }
}
