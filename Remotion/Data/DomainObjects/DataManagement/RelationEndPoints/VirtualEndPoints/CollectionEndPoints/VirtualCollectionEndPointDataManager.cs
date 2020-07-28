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
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Keeps the data of a <see cref="IVirtualCollectionEndPoint"/>.
  /// </summary>
  public class VirtualCollectionEndPointDataManager : IVirtualCollectionEndPointDataManager
  {
    public RelationEndPointID EndPointID { get; }

    public IVirtualCollectionEndPointChangeDetectionStrategy ChangeDetectionStrategy { get; }

    public IDataContainerMapReadOnlyView DataContainerMap { get; }

    private readonly ChangeCachingVirtualCollectionDataDecorator _changeCachingVirtualCollectionData;

    //private readonly HashSet<IRealObjectEndPoint> _originalOppositeEndPoints;
    //private readonly HashSet<DomainObject> _originalItemsWithoutEndPoint;
    //private Dictionary<ObjectID, IRealObjectEndPoint> _currentOppositeEndPoints;

    public VirtualCollectionEndPointDataManager (
        RelationEndPointID endPointID,
        IVirtualCollectionEndPointChangeDetectionStrategy changeDetectionStrategy,
        IDataContainerMapReadOnlyView dataContainerMap)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      ArgumentUtility.CheckNotNull ("changeDetectionStrategy", changeDetectionStrategy);
      ArgumentUtility.CheckNotNull ("dataContainerMap", dataContainerMap);

      EndPointID = endPointID;
      ChangeDetectionStrategy = changeDetectionStrategy;
      DataContainerMap = dataContainerMap;

      //TODO: RM-7294: unify VirtualCollectionData and ChangeCachingVirtualCollectionDataDecorator
      var wrappedData = new VirtualCollectionData (endPointID, dataContainerMap, ValueAccess.Current);
      _changeCachingVirtualCollectionData = new ChangeCachingVirtualCollectionDataDecorator (wrappedData);

      //_originalOppositeEndPoints = new HashSet<IRealObjectEndPoint>();
      //_originalItemsWithoutEndPoint = new HashSet<DomainObject>();
      //_currentOppositeEndPoints = new Dictionary<ObjectID, IRealObjectEndPoint>();
    }

    public IVirtualCollectionData CollectionData
    {
      get { return _changeCachingVirtualCollectionData; }
    }

    public ReadOnlyVirtualCollectionDataDecorator GetOriginalCollectionData ()
    {
      return _changeCachingVirtualCollectionData.GetOriginalData();
    }

    //public IRealObjectEndPoint[] OriginalOppositeEndPoints // TODO: RM-7294: Only public usage of OriginalOppositeEndPoints
    //{
    //  get { return _originalOppositeEndPoints.ToArray(); }
    //}

    //public IRealObjectEndPoint[] CurrentOppositeEndPoints // TODO: RM-7294: Only public usage of CurrentOppositeEndPoints
    //{
    //  get { return _currentOppositeEndPoints.Values.ToArray(); } // only used for copying during subtransaction commit
    //}

    //public DomainObject[] OriginalItemsWithoutEndPoints // TODO: RM-7294: Only public usage of OriginalItemsOppositeEndPoints
    //{
    //  get { return _originalItemsWithoutEndPoint.ToArray(); } // could return empty list, makes UnregisterOriginalItemWithoutEndPoint() obsolete
    //}

    //public bool ContainsOriginalObjectID (ObjectID objectID)
    //{
    //  ArgumentUtility.CheckNotNull ("objectID", objectID);

    //  return GetOriginalCollectionData().ContainsObjectID (objectID);
    //}

    //public bool ContainsOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    //{
    //  ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

    //  return _originalOppositeEndPoints.Contains (oppositeEndPoint);
    //}

    public void RegisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      //if (_originalOppositeEndPoints.Contains (oppositeEndPoint))
      //  throw new InvalidOperationException ("The opposite end-point has already been registered.");

      //var item = oppositeEndPoint.GetDomainObjectReference();

      //if (_originalItemsWithoutEndPoint.Contains (item))
      //{
      //  _originalItemsWithoutEndPoint.Remove (item);
      //}
      //else
      //{
        _changeCachingVirtualCollectionData.ResetCachedDomainObjects();
        //_changeCachingVirtualCollectionData.RegisterOriginalItem (item);
      //}

      //// RegisterOriginalItem adds item to both original and current collection, so we must add end-points for both
      //_originalOppositeEndPoints.Add (oppositeEndPoint);
      //_currentOppositeEndPoints.Add (oppositeEndPoint.ObjectID, oppositeEndPoint);
    }

    //public void UnregisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    //{
    //  //TODO: RM-7294: API is only implemented because of the interface. Interface member probably be dropped since there is no usage
    //  throw new NotSupportedException();
      //ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      //if (!ContainsOriginalOppositeEndPoint (oppositeEndPoint))
      //  throw new InvalidOperationException ("The opposite end-point has not been registered.");

      //var itemID = oppositeEndPoint.ObjectID;
      //_changeCachingVirtualCollectionData.UnregisterOriginalItem (itemID);

      //// UnregisterOriginalItem removes item from both original and current collection, so we must remove end-points for both
      //_originalOppositeEndPoints.Remove (oppositeEndPoint);
      //_currentOppositeEndPoints.Remove (oppositeEndPoint.ObjectID);
    //}

    //public bool ContainsCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    //{
    //  ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

    //  return _currentOppositeEndPoints.ContainsKey (oppositeEndPoint.ObjectID);
    //}

    public void RegisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      //if (_currentOppositeEndPoints.ContainsKey (oppositeEndPoint.ObjectID))
      //  throw new InvalidOperationException ("The opposite end-point has already been registered.");

      //_currentOppositeEndPoints.Add (oppositeEndPoint.ObjectID, oppositeEndPoint);
    }

    public void UnregisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      //if (!_currentOppositeEndPoints.ContainsKey (oppositeEndPoint.ObjectID))
      //  throw new InvalidOperationException ("The opposite end-point has not been registered.");

      //_currentOppositeEndPoints.Remove (oppositeEndPoint.ObjectID);
    }

    //bool IVirtualCollectionEndPointDataManager.ContainsOriginalItemWithoutEndPoint (DomainObject domainObject)
    //{
    //  ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    //  return _originalItemsWithoutEndPoint.Contains (domainObject);
    //}

    //public void RegisterOriginalItemWithoutEndPoint (DomainObject domainObject)
    //{
    //  ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    //  _changeCachingVirtualCollectionData.ResetCachedDomainObjects();
    //  //_changeCachingVirtualCollectionData.RegisterOriginalItem (domainObject);
    //  //_originalItemsWithoutEndPoint.Add (domainObject);
    //}

    //public void UnregisterOriginalItemWithoutEndPoint (DomainObject domainObject)
    //{
    //  ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      //if (!_originalItemsWithoutEndPoint.Contains (domainObject))
      //{
      //  var message = string.Format ("The domain object with ID '{0}' has not been registered as an item without end-point.", domainObject.ID);
      //  throw new InvalidOperationException (message);
      //}

      //_changeCachingVirtualCollectionData.UnregisterOriginalItem (domainObject.ID);
      //_changeCachingVirtualCollectionData.ResetCachedHasChangedState();
      //_originalItemsWithoutEndPoint.Remove (domainObject);
    //}

    //bool? IVirtualCollectionEndPointDataManager.HasDataChangedFast ()
    //{
    //  return _changeCachingVirtualCollectionData.IsCacheUpToDate ? _changeCachingVirtualCollectionData.HasChanged (ChangeDetectionStrategy) : (bool?) null;
    //}

    //bool IVirtualCollectionEndPointDataManager.HasDataChanged ()
    //{
    //  return _changeCachingVirtualCollectionData.HasChanged (ChangeDetectionStrategy);
    //}

    //void IVirtualCollectionEndPointDataManager.SortCurrentData (Comparison<DomainObject> comparison)
    //{
    //  ArgumentUtility.CheckNotNull ("comparison", comparison);

    //  _changeCachingVirtualCollectionData.Sort (comparison);
    //}

    //void IVirtualCollectionEndPointDataManager.SortCurrentAndOriginalData (Comparison<DomainObject> comparison)
    //{
    //  ArgumentUtility.CheckNotNull ("comparison", comparison);

    //  _changeCachingVirtualCollectionData.SortOriginalAndCurrent (comparison);
    //}

    public void Commit ()
    {
      _changeCachingVirtualCollectionData.Commit();

      //_originalOppositeEndPoints.Clear();
      //_originalItemsWithoutEndPoint.Clear();

      //var originalCollectionData = GetOriginalCollectionData();

      //foreach (var item in originalCollectionData)
      //{
        //var oppositeEndPoint = _currentOppositeEndPoints.GetValueOrDefault (item.ID);
        //if (oppositeEndPoint != null)
        //  _originalOppositeEndPoints.Add (oppositeEndPoint);
        //else
        //  _originalItemsWithoutEndPoint.Add (item);
      //}
      
      //Assertion.IsTrue (originalCollectionData.Count == _originalOppositeEndPoints.Count + _originalItemsWithoutEndPoint.Count);
    }

    public void Rollback ()
    {
      _changeCachingVirtualCollectionData.Rollback();

      //_currentOppositeEndPoints = _originalOppositeEndPoints.ToDictionary (ep => ep.ObjectID);
    }

    public void SetDataFromSubTransaction (IVirtualCollectionEndPointDataManager sourceDataManager, IRelationEndPointProvider endPointProvider)
    {
      ArgumentUtility.CheckNotNull ("sourceDataManager", sourceDataManager);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);

      _changeCachingVirtualCollectionData.ReplaceContents (sourceDataManager.CollectionData);
      //_currentOppositeEndPoints = sourceDataManager.CurrentOppositeEndPoints
      //    .Select (ep => Assertion.IsNotNull (
      //        (IRealObjectEndPoint) endPointProvider.GetRelationEndPointWithoutLoading (ep.ID), 
      //        "When committing a current virtual relation value from a sub-transaction, the opposite end-point is guaranteed to exist."))
      //    .ToDictionary (ep => ep.ObjectID);
    }

    #region Serialization

    // ReSharper disable UnusedMember.Local
    private VirtualCollectionEndPointDataManager (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      EndPointID = info.GetValueForHandle<RelationEndPointID>();
      ChangeDetectionStrategy = info.GetValueForHandle<IVirtualCollectionEndPointChangeDetectionStrategy>();

      _changeCachingVirtualCollectionData = info.GetValueForHandle<ChangeCachingVirtualCollectionDataDecorator>();

      //_originalOppositeEndPoints = new HashSet<IRealObjectEndPoint>();
      //info.FillCollection (_originalOppositeEndPoints);

      //_originalItemsWithoutEndPoint = new HashSet<DomainObject>();
      //info.FillCollection (_originalItemsWithoutEndPoint);

      //var currentOppositeEndPoints = new List<IRealObjectEndPoint>();
      //info.FillCollection (currentOppositeEndPoints);
      //_currentOppositeEndPoints = currentOppositeEndPoints.ToDictionary (ep => ep.ObjectID);
    }

    // ReSharper restore UnusedMember.Local

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      info.AddHandle (EndPointID);
      info.AddHandle (ChangeDetectionStrategy);
      info.AddHandle (_changeCachingVirtualCollectionData);

      //info.AddCollection (_originalOppositeEndPoints);
     // info.AddCollection (_originalItemsWithoutEndPoint);

      //info.AddCollection (_currentOppositeEndPoints.Values);
    }

    #endregion
  }
}