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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Keeps the data of a <see cref="IDomainObjectCollectionEndPoint"/>.
  /// </summary>
  public class DomainObjectCollectionEndPointDataManager : IDomainObjectCollectionEndPointDataManager
  {
    private readonly RelationEndPointID _endPointID;
    private readonly IDomainObjectCollectionEndPointChangeDetectionStrategy _changeDetectionStrategy;

    private readonly ChangeCachingDomainObjectCollectionDataDecorator _changeCachingDomainObjectCollectionData;

    private readonly HashSet<IRealObjectEndPoint> _originalOppositeEndPoints;
    private readonly HashSet<DomainObject> _originalItemsWithoutEndPoint;
    private Dictionary<ObjectID, IRealObjectEndPoint> _currentOppositeEndPoints;

    public DomainObjectCollectionEndPointDataManager (RelationEndPointID endPointID, IDomainObjectCollectionEndPointChangeDetectionStrategy changeDetectionStrategy)
    {
      ArgumentUtility.CheckNotNull("endPointID", endPointID);
      ArgumentUtility.CheckNotNull("changeDetectionStrategy", changeDetectionStrategy);

      _endPointID = endPointID;
      _changeDetectionStrategy = changeDetectionStrategy;

      var wrappedData = new DomainObjectCollectionData();
      _changeCachingDomainObjectCollectionData = new ChangeCachingDomainObjectCollectionDataDecorator(wrappedData);

      _originalOppositeEndPoints = new HashSet<IRealObjectEndPoint>();
      _originalItemsWithoutEndPoint = new HashSet<DomainObject>();
      _currentOppositeEndPoints = new Dictionary<ObjectID, IRealObjectEndPoint>();
    }

    public RelationEndPointID EndPointID
    {
      get { return _endPointID; }
    }

    public IDomainObjectCollectionEndPointChangeDetectionStrategy ChangeDetectionStrategy
    {
      get { return _changeDetectionStrategy; }
    }

    public bool? HasDataChangedFast ()
    {
      return _changeCachingDomainObjectCollectionData.IsCacheUpToDate ? _changeCachingDomainObjectCollectionData.HasChanged(_changeDetectionStrategy) : (bool?)null;
    }

    public IDomainObjectCollectionData CollectionData
    {
      get { return _changeCachingDomainObjectCollectionData; }
    }

    public ReadOnlyDomainObjectCollectionDataDecorator OriginalCollectionData
    {
      get { return _changeCachingDomainObjectCollectionData.OriginalData; }
    }

    public IRealObjectEndPoint[] OriginalOppositeEndPoints
    {
      get { return _originalOppositeEndPoints.ToArray(); }
    }

    public IRealObjectEndPoint[] CurrentOppositeEndPoints
    {
      get { return _currentOppositeEndPoints.Values.ToArray(); }
    }

    public DomainObject[] OriginalItemsWithoutEndPoints
    {
      get { return _originalItemsWithoutEndPoint.ToArray(); }
    }

    public bool ContainsOriginalObjectID (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);

      return OriginalCollectionData.ContainsObjectID(objectID);
    }

    public bool ContainsOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);

      return _originalOppositeEndPoints.Contains(oppositeEndPoint);
    }

    public void RegisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);
      if (oppositeEndPoint.IsNull)
        throw new ArgumentException("End point must not be a null object.", "oppositeEndPoint");

      Assertion.DebugIsNotNull(oppositeEndPoint.ObjectID, "oppositeEndPoint.ObjectID != null when oppositeEndPoint.IsNull == false");

      if (ContainsOriginalOppositeEndPoint(oppositeEndPoint))
        throw new InvalidOperationException("The opposite end-point has already been registered.");

      var item = oppositeEndPoint.GetDomainObjectReference();
      Assertion.DebugIsNotNull(item, "oppositeEndPoint.GetDomainObjectReference() != null when oppositeEndPoint.IsNull == false");

      if (_originalItemsWithoutEndPoint.Contains(item))
        _originalItemsWithoutEndPoint.Remove(item);
      else
        _changeCachingDomainObjectCollectionData.RegisterOriginalItem(item);

      // RegisterOriginalItem adds item to both original and current collection, so we must add end-points for both
      _originalOppositeEndPoints.Add(oppositeEndPoint);
      _currentOppositeEndPoints.Add(oppositeEndPoint.ObjectID, oppositeEndPoint);

    }

    public void UnregisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);
      if (oppositeEndPoint.IsNull)
        throw new ArgumentException("End point must not be a null object.", "oppositeEndPoint");

      if (!ContainsOriginalOppositeEndPoint(oppositeEndPoint))
        throw new InvalidOperationException("The opposite end-point has not been registered.");

      var itemID = oppositeEndPoint.ObjectID;
      Assertion.DebugIsNotNull(itemID, "oppositeEndPoint.ObjectID != null when oppositeEndPoint.IsNull == false");
      _changeCachingDomainObjectCollectionData.UnregisterOriginalItem(itemID);

      // UnregisterOriginalItem removes item from both original and current collection, so we must remove end-points for both
      _originalOppositeEndPoints.Remove(oppositeEndPoint);
      _currentOppositeEndPoints.Remove(itemID);
    }

    public bool ContainsCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);
      if (oppositeEndPoint.IsNull)
        throw new ArgumentException("End point must not be a null object.", "oppositeEndPoint");

      Assertion.DebugIsNotNull(oppositeEndPoint.ObjectID, "oppositeEndPoint.ObjectID != null when oppositeEndPoint.IsNull == false");

      return _currentOppositeEndPoints.ContainsKey(oppositeEndPoint.ObjectID);
    }

    public void RegisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);
      if (oppositeEndPoint.IsNull)
        throw new ArgumentException("End point must not be a null object.", "oppositeEndPoint");

      if (ContainsCurrentOppositeEndPoint(oppositeEndPoint))
        throw new InvalidOperationException("The opposite end-point has already been registered.");

      Assertion.DebugIsNotNull(oppositeEndPoint.ObjectID, "oppositeEndPoint.ObjectID != null when oppositeEndPoint.IsNull == false");

      _currentOppositeEndPoints.Add(oppositeEndPoint.ObjectID, oppositeEndPoint);
    }

    public void UnregisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);
      if (oppositeEndPoint.IsNull)
        throw new ArgumentException("End point must not be a null object.", "oppositeEndPoint");

      if (!ContainsCurrentOppositeEndPoint(oppositeEndPoint))
        throw new InvalidOperationException("The opposite end-point has not been registered.");

      Assertion.DebugIsNotNull(oppositeEndPoint.ObjectID, "oppositeEndPoint.ObjectID != null when oppositeEndPoint.IsNull == false");

      _currentOppositeEndPoints.Remove(oppositeEndPoint.ObjectID);
    }

    public bool ContainsOriginalItemWithoutEndPoint (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      return _originalItemsWithoutEndPoint.Contains(domainObject);
    }

    public void RegisterOriginalItemWithoutEndPoint (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      _changeCachingDomainObjectCollectionData.RegisterOriginalItem(domainObject);
      _originalItemsWithoutEndPoint.Add(domainObject);
    }

    public void UnregisterOriginalItemWithoutEndPoint (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      if (!_originalItemsWithoutEndPoint.Contains(domainObject))
      {
        var message = string.Format("The domain object with ID '{0}' has not been registered as an item without end-point.", domainObject.ID);
        throw new InvalidOperationException(message);
      }

      _changeCachingDomainObjectCollectionData.UnregisterOriginalItem(domainObject.ID);
      _originalItemsWithoutEndPoint.Remove(domainObject);
    }

    public bool HasDataChanged ()
    {
      return _changeCachingDomainObjectCollectionData.HasChanged(_changeDetectionStrategy);
    }

    public void SortCurrentData (Comparison<DomainObject> comparison)
    {
      ArgumentUtility.CheckNotNull("comparison", comparison);

      _changeCachingDomainObjectCollectionData.Sort(comparison);
    }

    public void SortCurrentAndOriginalData (Comparison<DomainObject> comparison)
    {
      ArgumentUtility.CheckNotNull("comparison", comparison);

      _changeCachingDomainObjectCollectionData.SortOriginalAndCurrent(comparison);
    }

    public void Commit ()
    {
      _changeCachingDomainObjectCollectionData.Commit();

      _originalOppositeEndPoints.Clear();
      _originalItemsWithoutEndPoint.Clear();

      foreach (var item in OriginalCollectionData)
      {
        var oppositeEndPoint = _currentOppositeEndPoints.GetValueOrDefault(item.ID);
        if (oppositeEndPoint != null)
          _originalOppositeEndPoints.Add(oppositeEndPoint);
        else
          _originalItemsWithoutEndPoint.Add(item);
      }

      Assertion.IsTrue(OriginalCollectionData.Count == _originalOppositeEndPoints.Count + _originalItemsWithoutEndPoint.Count);
    }

    public void Rollback ()
    {
      _changeCachingDomainObjectCollectionData.Rollback();

      _currentOppositeEndPoints = _originalOppositeEndPoints.ToDictionary(
          ep =>
          {
            Assertion.IsFalse(ep.IsNull, "ep.IsNull");
            Assertion.DebugIsNotNull(ep.ObjectID, "ep.ObjectID != null when ep.IsNull == false");

            return ep.ObjectID;
          });
    }

    public void SetDataFromSubTransaction (IDomainObjectCollectionEndPointDataManager sourceDataManager, IRelationEndPointProvider endPointProvider)
    {
      ArgumentUtility.CheckNotNull("sourceDataManager", sourceDataManager);
      ArgumentUtility.CheckNotNull("endPointProvider", endPointProvider);

      _changeCachingDomainObjectCollectionData.ReplaceContents(sourceDataManager.CollectionData);
      _currentOppositeEndPoints = sourceDataManager.CurrentOppositeEndPoints
          .Select(ep => Assertion.IsNotNull(
              (IRealObjectEndPoint?)endPointProvider.GetRelationEndPointWithoutLoading(ep.ID),
              "When committing a current virtual relation value from a sub-transaction, the opposite end-point is guaranteed to exist."))
          .ToDictionary(
              ep =>
              {
                Assertion.IsFalse(ep.IsNull, "ep.IsNull");
                Assertion.DebugIsNotNull(ep.ObjectID, "ep.ObjectID != null when ep.IsNull == false");

                return ep.ObjectID;
              });
    }
  }
}
