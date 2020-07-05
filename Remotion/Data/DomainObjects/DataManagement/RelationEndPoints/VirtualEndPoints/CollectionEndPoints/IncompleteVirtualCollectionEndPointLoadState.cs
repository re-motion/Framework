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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Represents the state of a <see cref="VirtualCollectionEndPoint"/> where not all of its data is available (ie., the end-point has not been (lazily) 
  /// loaded, or it has been unloaded).
  /// </summary>
  public class IncompleteVirtualCollectionEndPointLoadState : IVirtualCollectionEndPointLoadState
  {
    public interface IEndPointLoader : IFlattenedSerializable
    {
      IVirtualCollectionEndPointLoadState LoadEndPointAndGetNewState (IVirtualCollectionEndPoint endPoint);
    }

    private static readonly ILog s_log = LogManager.GetLogger (typeof (IncompleteVirtualCollectionEndPointLoadState));

    private readonly IEndPointLoader _endPointLoader;
    private readonly Dictionary<ObjectID, IRealObjectEndPoint> _originalOppositeEndPoints;
    private readonly IVirtualCollectionEndPointDataManagerFactory _dataManagerFactory;

    public IncompleteVirtualCollectionEndPointLoadState (
        IEndPointLoader endPointLoader,
        IVirtualCollectionEndPointDataManagerFactory dataManagerFactory)
    {
      ArgumentUtility.CheckNotNull ("endPointLoader", endPointLoader);
      ArgumentUtility.CheckNotNull ("dataManagerFactory", dataManagerFactory);

      _endPointLoader = endPointLoader;
      _originalOppositeEndPoints = new Dictionary<ObjectID, IRealObjectEndPoint>();
      _dataManagerFactory = dataManagerFactory;
    }

    public IVirtualCollectionEndPointDataManagerFactory DataManagerFactory
    {
      get { return _dataManagerFactory; }
    }

    public bool? HasChangedFast ()
    {
      return false;
    }

    public void EnsureDataComplete (IVirtualCollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      EndPointLoader.LoadEndPointAndGetNewState (endPoint);
    }

    public void MarkDataComplete (
        IVirtualCollectionEndPoint collectionEndPoint,
        IEnumerable<DomainObject> items,
        Action<IVirtualCollectionEndPointDataManager> stateSetter)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("items", items);
      ArgumentUtility.CheckNotNull ("stateSetter", stateSetter);

      if (s_log.IsInfoEnabled())
        s_log.InfoFormat ("Virtual end-point '{0}' is transitioned to complete state.", collectionEndPoint.ID);

      var dataManager = CreateEndPointDataManager(collectionEndPoint);
      
      foreach (var item in items)
      {
        IRealObjectEndPoint oppositeEndPoint;
        if (_originalOppositeEndPoints.TryGetValue (item.ID, out oppositeEndPoint))
        {
          dataManager.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
          oppositeEndPoint.MarkSynchronized ();
          _originalOppositeEndPoints.Remove (item.ID);
        }
        else
        {
          // Virtual end-point contains an item without an opposite end-point. The virtual end-point is out-of-sync. Note that this can temporarily 
          // occur during eager fetching because the end-point contents are set before the related objects' DataContainers are registered.
          // Apart from that case, this indicates that foreign keys in the database have changed between loading the foreign key side and the virtual
          // side of a bidirectional relation.
          dataManager.RegisterOriginalItemWithoutEndPoint (item);
        }
      }

      stateSetter (dataManager);

      foreach (var oppositeEndPointWithoutItem in _originalOppositeEndPoints.Values)
        collectionEndPoint.RegisterOriginalOppositeEndPoint (oppositeEndPointWithoutItem);

      var eventRaiser = collectionEndPoint.GetCollectionEventRaiser();
      eventRaiser.WithinReplaceData();
    }

    public void SortCurrentData (IVirtualCollectionEndPoint collectionEndPoint, Comparison<DomainObject> comparison)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("comparison", comparison);

      // TODO RM-7294: Remove SortCurrentData (...) ?
      throw new NotSupportedException();
      //var completeState = EndPointLoader.LoadEndPointAndGetNewState (collectionEndPoint);
      //completeState.SortCurrentData (collectionEndPoint, comparison);
    }

    public IDataManagementCommand CreateRemoveCommand (IVirtualCollectionEndPoint collectionEndPoint, DomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("removedRelatedObject", removedRelatedObject);

      return new NopCommand();
    }

    public IDataManagementCommand CreateDeleteCommand (IVirtualCollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);

      return new NopCommand();
    }

    public IDataManagementCommand CreateAddCommand (IVirtualCollectionEndPoint collectionEndPoint, DomainObject addedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("addedRelatedObject", addedRelatedObject);

      return new NopCommand();
    }

    protected IVirtualCollectionEndPointDataManager CreateEndPointDataManager (IVirtualCollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      return _dataManagerFactory.CreateEndPointDataManager (endPoint.ID);
    }

    public bool CanEndPointBeCollected (IVirtualCollectionEndPoint endPoint)
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

    public bool CanDataBeMarkedIncomplete (IVirtualCollectionEndPoint endPoint)
    {
      return true;
    }

    public void MarkDataIncomplete (IVirtualCollectionEndPoint endPoint, Action stateSetter)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("stateSetter", stateSetter);

      // Do nothing - data is already incomplete
    }

    public ReadOnlyVirtualCollectionDataDecorator GetData (IVirtualCollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      var completeState = _endPointLoader.LoadEndPointAndGetNewState (endPoint);
      return completeState.GetData (endPoint);
    }

    public ReadOnlyVirtualCollectionDataDecorator GetOriginalData (IVirtualCollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      var completeState = _endPointLoader.LoadEndPointAndGetNewState (endPoint);
      return completeState.GetOriginalData (endPoint);
    }
 
    public void RegisterOriginalOppositeEndPoint (IVirtualCollectionEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _originalOppositeEndPoints.Add (oppositeEndPoint.ObjectID, oppositeEndPoint);
      oppositeEndPoint.ResetSyncState ();
    }

    public void UnregisterOriginalOppositeEndPoint (IVirtualCollectionEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (!_originalOppositeEndPoints.ContainsKey (oppositeEndPoint.ObjectID))
        throw new InvalidOperationException ("The opposite end-point has not been registered.");

      _originalOppositeEndPoints.Remove (oppositeEndPoint.ObjectID);
    }

    public void RegisterCurrentOppositeEndPoint (IVirtualCollectionEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      var completeState = _endPointLoader.LoadEndPointAndGetNewState (endPoint);
      completeState.RegisterCurrentOppositeEndPoint (endPoint, oppositeEndPoint);
    }

    public void UnregisterCurrentOppositeEndPoint (IVirtualCollectionEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      var completeState = _endPointLoader.LoadEndPointAndGetNewState (endPoint);
      completeState.UnregisterCurrentOppositeEndPoint (endPoint, oppositeEndPoint);
    }

    public bool? IsSynchronized (IVirtualCollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      return null;
    }

    public void Synchronize (IVirtualCollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      var completeState = _endPointLoader.LoadEndPointAndGetNewState (endPoint);
      completeState.Synchronize (endPoint);
    }

    public void SynchronizeOppositeEndPoint (IVirtualCollectionEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      throw new InvalidOperationException ("Cannot synchronize an opposite end-point with a virtual end-point in incomplete state.");
    }

    public void SetDataFromSubTransaction (IVirtualCollectionEndPoint endPoint, IVirtualEndPointLoadState<IVirtualCollectionEndPoint, ReadOnlyVirtualCollectionDataDecorator, IVirtualCollectionEndPointDataManager> sourceLoadState)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("sourceLoadState", sourceLoadState);

      throw new InvalidOperationException ("Cannot comit data from a sub-transaction into a virtual end-point in incomplete state.");
    }

    public bool HasChanged ()
    {
      return false; 
    }

    public void Commit (IVirtualCollectionEndPoint endPoint)
    {
      Assertion.IsTrue (!HasChanged());
    }

    public void Rollback (IVirtualCollectionEndPoint endPoint)
    {
      Assertion.IsTrue (!HasChanged ());
    }

    #region Serialization

    public IncompleteVirtualCollectionEndPointLoadState (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      _endPointLoader = info.GetValue<IEndPointLoader> ();

      var realObjectEndPoints = new List<IRealObjectEndPoint>();
      info.FillCollection (realObjectEndPoints);
      _originalOppositeEndPoints = realObjectEndPoints.ToDictionary (ep => ep.ObjectID);
      _dataManagerFactory = info.GetValueForHandle<IVirtualCollectionEndPointDataManagerFactory>();
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      info.AddValue (_endPointLoader);
      info.AddCollection(_originalOppositeEndPoints.Values);
      info.AddHandle (_dataManagerFactory);
    }

    #endregion
  }
}