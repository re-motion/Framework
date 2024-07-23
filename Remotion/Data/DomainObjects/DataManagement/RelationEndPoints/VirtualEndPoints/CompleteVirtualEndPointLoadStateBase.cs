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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints
{
  /// <summary>
  /// Defines common logic for <see cref="IVirtualEndPoint"/> implementations in complete state, ie., when lazy loading has completed.
  /// </summary>
  /// <typeparam name="TEndPoint">The type of the end point whose state is managed by this class.</typeparam>
  /// <typeparam name="TData">The type of data held by the <typeparamref name="TDataManager"/>.</typeparam>
  /// <typeparam name="TDataManager">The type of <see cref="IVirtualEndPointDataManager"/> holding the data for the end-point.</typeparam>
  public abstract class CompleteVirtualEndPointLoadStateBase<TEndPoint, TData, TDataManager>
      : IVirtualEndPointLoadState<TEndPoint, TData, TDataManager>
      where TEndPoint : IVirtualEndPoint<TData>
      where TDataManager : class, IVirtualEndPointDataManager
  {
    private static readonly ILog s_log = LogManager.GetLogger(typeof(CompleteVirtualEndPointLoadStateBase<TEndPoint, TData, TDataManager>));

    private readonly TDataManager _dataManager;
    private readonly IRelationEndPointProvider _endPointProvider;
    private readonly IClientTransactionEventSink _transactionEventSink;

    private readonly Dictionary<ObjectID, IRealObjectEndPoint> _unsynchronizedOppositeEndPoints;

    protected CompleteVirtualEndPointLoadStateBase (
        TDataManager dataManager,
        IRelationEndPointProvider endPointProvider,
        IClientTransactionEventSink transactionEventSink)
    {
      ArgumentUtility.CheckNotNull("dataManager", dataManager);
      ArgumentUtility.CheckNotNull("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull("transactionEventSink", transactionEventSink);

      _dataManager = dataManager;
      _endPointProvider = endPointProvider;
      _transactionEventSink = transactionEventSink;

      _unsynchronizedOppositeEndPoints = new Dictionary<ObjectID, IRealObjectEndPoint>();
    }

    public abstract TData GetData (TEndPoint endPoint);
    public abstract TData GetOriginalData (TEndPoint endPoint);
    public abstract void SetDataFromSubTransaction (TEndPoint endPoint, IVirtualEndPointLoadState<TEndPoint, TData, TDataManager> sourceLoadState);

    protected abstract IEnumerable<IRealObjectEndPoint> GetOriginalOppositeEndPoints ();
    protected abstract IEnumerable<DomainObject> GetOriginalItemsWithoutEndPoints ();

    public static ILog Log
    {
      get { return s_log; }
    }

    public TDataManager DataManager
    {
      get { return _dataManager; }
    }

    public IRelationEndPointProvider EndPointProvider
    {
      get { return _endPointProvider; }
    }

    public IClientTransactionEventSink TransactionEventSink
    {
      get { return _transactionEventSink; }
    }

    public ICollection<IRealObjectEndPoint> UnsynchronizedOppositeEndPoints
    {
      get { return _unsynchronizedOppositeEndPoints.Values; }
    }

    public bool IsDataComplete ()
    {
      return true;
    }

    public void EnsureDataComplete (TEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);
      // Data is already complete
    }

    public bool CanDataBeMarkedIncomplete (TEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);
      return !HasChanged();
    }

    public virtual void MarkDataIncomplete (TEndPoint endPoint, Action stateSetter)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);
      ArgumentUtility.CheckNotNull("stateSetter", stateSetter);

      if (HasChanged())
      {
        var message = string.Format("Cannot mark virtual end-point '{0}' incomplete because it has been changed.", endPoint.ID);
        throw new InvalidOperationException(message);
      }

      _transactionEventSink.RaiseRelationEndPointBecomingIncompleteEvent(endPoint.ID);

      stateSetter();

      var allOppositeEndPoints = UnsynchronizedOppositeEndPoints.Concat(GetOriginalOppositeEndPoints());
      foreach (var oppositeEndPoint in allOppositeEndPoints)
        endPoint.RegisterOriginalOppositeEndPoint(oppositeEndPoint);
    }

    public bool CanEndPointBeCollected (TEndPoint endPoint)
    {
      return false;
    }

    public void RegisterOriginalOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);
      if (oppositeEndPoint.IsNull)
        throw new ArgumentException("End point must not be a null object.", "oppositeEndPoint");

      Assertion.DebugIsNotNull(oppositeEndPoint.ObjectID, "oppositeEndPoint.ObjectID != null when oppositeEndPoint.IsNull == false");

      if (_dataManager.ContainsOriginalObjectID(oppositeEndPoint.ObjectID))
      {
        // RealObjectEndPoint is registered for an already loaded virtual end-point. The query result contained the item, so the ObjectEndPoint is 
        // marked as synchronzed.

        _dataManager.RegisterOriginalOppositeEndPoint(oppositeEndPoint);
        oppositeEndPoint.MarkSynchronized();
      }
      else
      {
        // ObjectEndPoint is registered for an already loaded virtual end-point. The query result did not contain the item, so the ObjectEndPoint is 
        // out-of-sync.

        _unsynchronizedOppositeEndPoints.Add(oppositeEndPoint.ObjectID, oppositeEndPoint);
        oppositeEndPoint.MarkUnsynchronized();
      }
    }

    public void UnregisterOriginalOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);
      if (oppositeEndPoint.IsNull)
        throw new ArgumentException("End point must not be a null object.", "oppositeEndPoint");

      Assertion.DebugIsNotNull(oppositeEndPoint.ObjectID, "oppositeEndPoint.ObjectID != null when oppositeEndPoint.IsNull == false");

      if (_unsynchronizedOppositeEndPoints.ContainsKey(oppositeEndPoint.ObjectID))
      {
        if (s_log.IsDebugEnabled())
        {
          s_log.DebugFormat(
              "Unsynchronized ObjectEndPoint '{0}' is unregistered from virtual end-point '{1}'.",
              oppositeEndPoint.ID,
              endPoint.ID);
        }

        _unsynchronizedOppositeEndPoints.Remove(oppositeEndPoint.ObjectID);
      }
      else
      {
        if (s_log.IsInfoEnabled())
        {
          s_log.InfoFormat(
              "ObjectEndPoint '{0}' is unregistered from virtual end-point '{1}'. The virtual end-point is transitioned to incomplete state.",
              oppositeEndPoint.ID,
              endPoint.ID);
        }

        endPoint.MarkDataIncomplete();
        endPoint.UnregisterOriginalOppositeEndPoint(oppositeEndPoint);
      }
    }

    public void RegisterCurrentOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);

      _dataManager.RegisterCurrentOppositeEndPoint(oppositeEndPoint);
    }

    public void UnregisterCurrentOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);

      _dataManager.UnregisterCurrentOppositeEndPoint(oppositeEndPoint);
    }

    public bool IsSynchronized (TEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);

      return !GetOriginalItemsWithoutEndPoints().Any();
    }

    bool? IVirtualEndPointLoadState<TEndPoint, TData, TDataManager>.IsSynchronized (TEndPoint endPoint)
    {
      return IsSynchronized(endPoint);
    }

    public virtual void Synchronize (TEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);

      if (Log.IsDebugEnabled())
        Log.DebugFormat("End-point '{0}' is being synchronized.", endPoint.ID);

      foreach (var item in GetOriginalItemsWithoutEndPoints())
        DataManager.UnregisterOriginalItemWithoutEndPoint(item);
    }

    public virtual void SynchronizeOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);
      if (oppositeEndPoint.IsNull)
        throw new ArgumentException("End point must not be a null object.", "oppositeEndPoint");

      Assertion.DebugIsNotNull(oppositeEndPoint.ObjectID, "oppositeEndPoint.ObjectID != null when oppositeEndPoint.IsNull == false");

      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("ObjectEndPoint '{0}' is being marked as synchronized.", oppositeEndPoint.ID);

      if (!_unsynchronizedOppositeEndPoints.Remove(oppositeEndPoint.ObjectID))
      {
        var message = string.Format(
            "Cannot synchronize opposite end-point '{0}' - the end-point is not in the list of unsynchronized end-points.",
            oppositeEndPoint.ID);
        throw new InvalidOperationException(message);
      }

      _dataManager.RegisterOriginalOppositeEndPoint(oppositeEndPoint);
      oppositeEndPoint.MarkSynchronized();
    }

    public bool HasChanged ()
    {
      return _dataManager.HasDataChanged();
    }

    public virtual void Commit (TEndPoint endPoint)
    {
      _dataManager.Commit();
    }

    public virtual void Rollback (TEndPoint endPoint)
    {
      _dataManager.Rollback();
    }

    protected void MarkDataComplete (TEndPoint endPoint, IEnumerable<DomainObject> data, Action<TDataManager> stateSetter)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);
      ArgumentUtility.CheckNotNull("data", data);
      ArgumentUtility.CheckNotNull("stateSetter", stateSetter);

      throw new InvalidOperationException("The data is already complete.");
    }

    protected bool ContainsUnsynchronizedOppositeEndPoint (ObjectID objectID)
    {
      return _unsynchronizedOppositeEndPoints.ContainsKey(objectID);
    }
  }
}
