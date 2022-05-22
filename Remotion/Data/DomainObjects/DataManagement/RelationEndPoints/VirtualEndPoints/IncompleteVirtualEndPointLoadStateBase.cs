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
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints
{
  /// <summary>
  /// Defines common logic for <see cref="IVirtualEndPoint"/> implementations in incomplete state, ie., before lazy loading has completed.
  /// </summary>
  /// <typeparam name="TEndPoint">The type of the end point whose state is managed by this class.</typeparam>
  /// <typeparam name="TData">The type of data held by the <typeparamref name="TDataManager"/>.</typeparam>
  /// <typeparam name="TDataManager">The type of <see cref="IVirtualEndPointDataManager"/> holding the data for the end-point.</typeparam>
  /// <typeparam name="TLoadStateInterface">The type of the load state interface used by <typeparamref name="TEndPoint"/>.</typeparam>
  public abstract class IncompleteVirtualEndPointLoadStateBase<TEndPoint, TData, TDataManager, TLoadStateInterface>
      : IVirtualEndPointLoadState<TEndPoint, TData, TDataManager>
      where TEndPoint : IVirtualEndPoint<TData>
      where TDataManager : IVirtualEndPointDataManager
      where TLoadStateInterface : IVirtualEndPointLoadState<TEndPoint, TData, TDataManager>
  {
    public interface IEndPointLoader : IFlattenedSerializable
    {
      TLoadStateInterface LoadEndPointAndGetNewState (TEndPoint endPoint);
    }

    private static readonly ILog s_log = LogManager.GetLogger(typeof(IncompleteVirtualEndPointLoadStateBase<TEndPoint, TData, TDataManager, TLoadStateInterface>));

    protected static ILog Log
    {
      get { return s_log; }
    }

    private readonly IEndPointLoader _endPointLoader;
    private readonly Dictionary<ObjectID, IRealObjectEndPoint> _originalOppositeEndPoints;

    protected IncompleteVirtualEndPointLoadStateBase (IEndPointLoader endPointLoader)
    {
      ArgumentUtility.CheckNotNull("endPointLoader", endPointLoader);

      _endPointLoader = endPointLoader;
      _originalOppositeEndPoints = new Dictionary<ObjectID, IRealObjectEndPoint>();
    }

    protected abstract TDataManager CreateEndPointDataManager (TEndPoint endPoint);

    public bool CanEndPointBeCollected (TEndPoint endPoint)
    {
      return _originalOppositeEndPoints.Count == 0;
    }

    public ICollection<IRealObjectEndPoint> OriginalOppositeEndPoints
    {
      get { return _originalOppositeEndPoints.Values; }
    }

    public IEndPointLoader EndPointLoader
    {
      get { return _endPointLoader; }
    }

    public bool IsDataComplete ()
    {
      return false;
    }

    public bool CanDataBeMarkedIncomplete (TEndPoint endPoint)
    {
      return true;
    }

    public void MarkDataIncomplete (TEndPoint endPoint, Action stateSetter)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);
      ArgumentUtility.CheckNotNull("stateSetter", stateSetter);

      // Do nothing - data is already incomplete
    }

    public virtual TData GetData (TEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);

      var completeState = _endPointLoader.LoadEndPointAndGetNewState(endPoint);
      return completeState.GetData(endPoint);
    }

    public virtual TData GetOriginalData (TEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);

      var completeState = _endPointLoader.LoadEndPointAndGetNewState(endPoint);
      return completeState.GetOriginalData(endPoint);
    }

    public virtual void RegisterOriginalOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);
      if (oppositeEndPoint.IsNull)
        throw new ArgumentException("End point must not be a null object.", "oppositeEndPoint");

      Assertion.DebugIsNotNull(oppositeEndPoint.ObjectID, "oppositeEndPoint.ObjectID != null when oppositeEndPoint.IsNull == false");

      _originalOppositeEndPoints.Add(oppositeEndPoint.ObjectID, oppositeEndPoint);
      oppositeEndPoint.ResetSyncState();
    }

    public virtual void UnregisterOriginalOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);
      if (oppositeEndPoint.IsNull)
        throw new ArgumentException("End point must not be a null object.", "oppositeEndPoint");

      Assertion.DebugIsNotNull(oppositeEndPoint.ObjectID, "oppositeEndPoint.ObjectID != null when oppositeEndPoint.IsNull == false");

      if (!_originalOppositeEndPoints.ContainsKey(oppositeEndPoint.ObjectID))
        throw new InvalidOperationException("The opposite end-point has not been registered.");

      _originalOppositeEndPoints.Remove(oppositeEndPoint.ObjectID);
    }

    public void RegisterCurrentOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);

      var completeState = _endPointLoader.LoadEndPointAndGetNewState(endPoint);
      completeState.RegisterCurrentOppositeEndPoint(endPoint, oppositeEndPoint);
    }

    public void UnregisterCurrentOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);

      var completeState = _endPointLoader.LoadEndPointAndGetNewState(endPoint);
      completeState.UnregisterCurrentOppositeEndPoint(endPoint, oppositeEndPoint);
    }

    public bool? IsSynchronized (TEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);

      return null;
    }

    public void Synchronize (TEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);

      var completeState = _endPointLoader.LoadEndPointAndGetNewState(endPoint);
      completeState.Synchronize(endPoint);
    }

    public void SynchronizeOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);

      throw new InvalidOperationException("Cannot synchronize an opposite end-point with a virtual end-point in incomplete state.");
    }

    public void SetDataFromSubTransaction (TEndPoint endPoint, IVirtualEndPointLoadState<TEndPoint, TData, TDataManager> sourceLoadState)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);
      ArgumentUtility.CheckNotNull("sourceLoadState", sourceLoadState);

      throw new InvalidOperationException("Cannot comit data from a sub-transaction into a virtual end-point in incomplete state.");
    }

    public bool HasChanged ()
    {
      return false;
    }

    public void Commit (TEndPoint endPoint)
    {
      Assertion.IsTrue(!HasChanged());
    }

    public void Rollback (TEndPoint endPoint)
    {
      Assertion.IsTrue(!HasChanged());
    }

    protected void MarkDataComplete (TEndPoint endPoint, IEnumerable<IDomainObject> items, Action<TDataManager> stateSetter)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);
      ArgumentUtility.CheckNotNull("items", items);
      ArgumentUtility.CheckNotNull("stateSetter", stateSetter);

      if (s_log.IsInfoEnabled())
        s_log.InfoFormat("Virtual end-point '{0}' is transitioned to complete state.", endPoint.ID);

      var dataManager = CreateEndPointDataManager(endPoint);

      foreach (var item in items)
      {
        if (_originalOppositeEndPoints.TryGetValue(item.ID, out var oppositeEndPoint))
        {
          dataManager.RegisterOriginalOppositeEndPoint(oppositeEndPoint);
          oppositeEndPoint.MarkSynchronized();
          _originalOppositeEndPoints.Remove(item.ID);
        }
        else
        {
          // Virtual end-point contains an item without an opposite end-point. The virtual end-point is out-of-sync. Note that this can temporarily 
          // occur during eager fetching because the end-point contents are set before the related objects' DataContainers are registered.
          // Apart from that case, this indicates that foreign keys in the database have changed between loading the foreign key side and the virtual
          // side of a bidirectional relation.
          dataManager.RegisterOriginalItemWithoutEndPoint(item);
        }
      }

      stateSetter(dataManager);

      foreach (var oppositeEndPointWithoutItem in _originalOppositeEndPoints.Values)
        endPoint.RegisterOriginalOppositeEndPoint(oppositeEndPointWithoutItem);
    }

    #region Serialization

    protected IncompleteVirtualEndPointLoadStateBase (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull("info", info);
      _endPointLoader = info.GetValue<IEndPointLoader>();

      var realObjectEndPoints = new List<IRealObjectEndPoint>();
      info.FillCollection(realObjectEndPoints);
      _originalOppositeEndPoints = realObjectEndPoints.ToDictionary(
          ep =>
          {
            Assertion.IsFalse(ep.IsNull, "ep.IsNull");
            Assertion.DebugIsNotNull(ep.ObjectID, "ep.ObjectID != null when ep.IsNull == false");

            return ep.ObjectID;
          });
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull("info", info);
      info.AddValue(_endPointLoader);
      info.AddCollection(_originalOppositeEndPoints.Values);

      SerializeSubclassData(info);
    }

    protected virtual void SerializeSubclassData (FlattenedSerializationInfo info)
    {
    }

    #endregion
  }
}
