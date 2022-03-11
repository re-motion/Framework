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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands
{
  public class UnloadFilteredDomainObjectsCommand : IDataManagementCommand
  {
    public IRelationEndPointManager RelationEndPointManager { get; }
    public DataContainerMap DataContainerMap { get; }
    public IInvalidDomainObjectManager InvalidDomainObjectManager { get; }
    public RelationEndPointMap RelationEndPointMap { get; }
    public IRelationEndPointRegistrationAgent RelationEndPointRegistrationAgent { get; }
    public IClientTransactionEventSink TransactionEventSink { get; }
    public Predicate<DomainObject> DomainObjectFilter { get; }
    private IReadOnlyCollection<DataContainer> _dataContainersForUnloading = Array.Empty<DataContainer>();
    private IReadOnlyCollection<IVirtualEndPoint> _virtualEndPointsForUnloading = Array.Empty<IVirtualEndPoint>();
    private IReadOnlyCollection<IRealObjectEndPoint> _realObjectEndPointsForUnloading = Array.Empty<IRealObjectEndPoint>();
    private List<Exception> _exceptions = new List<Exception>();

    public UnloadFilteredDomainObjectsCommand (
        IRelationEndPointManager relationEndPointManager,
        DataContainerMap dataContainerMap,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        RelationEndPointMap relationRelationEndPointMap,
        IRelationEndPointRegistrationAgent relationEndPointRegistrationAgent,
        IClientTransactionEventSink transactionEventSink,
        Predicate<DomainObject> domainObjectFilter)
    {
      ArgumentUtility.CheckNotNull("relationEndPointManager", relationEndPointManager);
      ArgumentUtility.CheckNotNull("dataContainerMap", dataContainerMap);
      ArgumentUtility.CheckNotNull("invalidDomainObjectManager", invalidDomainObjectManager);
      ArgumentUtility.CheckNotNull("relationRelationEndPointMap", relationRelationEndPointMap);
      ArgumentUtility.CheckNotNull("relationEndPointRegistrationAgent", relationEndPointRegistrationAgent);
      ArgumentUtility.CheckNotNull("transactionEventSink", transactionEventSink);
      ArgumentUtility.CheckNotNull("domainObjectFilter", domainObjectFilter);

      RelationEndPointManager = relationEndPointManager;
      DataContainerMap = dataContainerMap;
      InvalidDomainObjectManager = invalidDomainObjectManager;
      RelationEndPointMap = relationRelationEndPointMap;
      RelationEndPointRegistrationAgent = relationEndPointRegistrationAgent;
      TransactionEventSink = transactionEventSink;
      DomainObjectFilter = domainObjectFilter;
    }

    public IEnumerable<Exception> GetAllExceptions ()
    {
      return _exceptions.AsReadOnly();
    }

    public void Begin ()
    {
      _dataContainersForUnloading = DataContainerMap
          .Where(dc => DomainObjectFilter(dc.DomainObject))
          .ToList()
          .AsReadOnly();

      var relationEndPoints = GetRelationEndPointsForUnloading(_dataContainersForUnloading);
      if (relationEndPoints.RealObjectEndPointsNotPartOfUnloadSet.Any() || relationEndPoints.VirtualEndPointsNotPartOfUnloadSet.Any())
      {
        _exceptions.Add(new InvalidOperationException("Transitive closure violated"));
        throw _exceptions.First();
      }
      else
      {
        _realObjectEndPointsForUnloading = relationEndPoints.RealObjectEndPoints;
        _virtualEndPointsForUnloading = relationEndPoints.VirtualEndPoints;
        if (_dataContainersForUnloading.Count > 0)
          TransactionEventSink.RaiseObjectsUnloadingEvent(_dataContainersForUnloading.Select(dc => dc.DomainObject).ToList());
      }
    }

    public void Perform ()
    {
      if (_exceptions.Count > 0)
        throw _exceptions.First();

      // Unregistering a real-object-endpoint also unregisters the associated virtual endpoint.
      // If is therefor best to first handle all real-object-endpoints in case both sides belong to the same object.

      foreach (var relationEndPoint in _realObjectEndPointsForUnloading)
      {
        RelationEndPointMap.RemoveEndPoint(relationEndPoint.ID);
        RelationEndPointRegistrationAgent.UnregisterEndPoint(relationEndPoint, RelationEndPointMap);
      }

      foreach (var relationEndPoint in _virtualEndPointsForUnloading)
      {
        RelationEndPointMap.RemoveEndPoint(relationEndPoint.ID);
        relationEndPoint.MarkDataIncomplete();
        Assertion.IsTrue(relationEndPoint.CanBeCollected, "VirtualEndPoint '{0}' cannot be collected.", relationEndPoint.ID);
        RelationEndPointRegistrationAgent.UnregisterEndPoint(relationEndPoint, RelationEndPointMap);
      }

      foreach (var dataContainer in _dataContainersForUnloading)
      {
        DataContainerMap.Remove(dataContainer.ID);

        var dataContainerState = dataContainer.State;
        dataContainer.Discard();
        if (dataContainerState.IsNew)
          InvalidDomainObjectManager.MarkInvalid(dataContainer.DomainObject);
      }
    }

    public void End ()
    {
      if (_exceptions.Count > 0)
        throw _exceptions.First();

      if (_dataContainersForUnloading.Count > 0)
        TransactionEventSink.RaiseObjectsUnloadingEvent(_dataContainersForUnloading.Select(dc => dc.DomainObject).ToList());
    }

    public ExpandedCommand ExpandToAllRelatedObjects ()
    {
      return new ExpandedCommand(this);
    }

    private
        (HashSet<IRealObjectEndPoint> RealObjectEndPoints,
        HashSet<IVirtualEndPoint> VirtualEndPoints,
        HashSet<IRealObjectEndPoint> RealObjectEndPointsNotPartOfUnloadSet,
        HashSet<IVirtualEndPoint> VirtualEndPointsNotPartOfUnloadSet)
        GetRelationEndPointsForUnloading (IReadOnlyCollection<DataContainer> dataContainers)
    {
      var realObjectEndPoints = new HashSet<IRealObjectEndPoint>();
      var virtualObjectEndPoints = new HashSet<IVirtualEndPoint>();
      var realObjectEndPointsNotPartOfUnloadSet = new HashSet<IRealObjectEndPoint>();
      var virtualEndPointsNotPartOfUnloadSet = new HashSet<IVirtualEndPoint>();
      var objectIDs = dataContainers.ToDictionary(dc => dc.ID);

      var endPoints = RelationEndPointManager.RelationEndPoints
          .ApplySideEffect(ep => Assertion.IsFalse(ep.IsNull, "RelationEndPoint '{0}' is a null end-point.", ep.ID))
          .Where(ep => ep.ObjectID != null);

      foreach (var endPoint in endPoints)
      {
        Assertion.DebugAssert(endPoint.Definition.IsAnonymous == false, "endPoint.Definition.IsAnonymous == false");
        Assertion.DebugIsNotNull(endPoint.ObjectID, "endPoint.ObjectID != null");
        var isPartOfUnloadedSet = objectIDs.ContainsKey(endPoint.ObjectID);

        if (endPoint is IRealObjectEndPoint realObjectEndPoint)
        {
          if (isPartOfUnloadedSet)
          {
            if (realObjectEndPoint.IsNull)
            {
              realObjectEndPoints.Add(realObjectEndPoint);
            }
            else if ((realObjectEndPoint.OppositeObjectID == null || objectIDs.ContainsKey(realObjectEndPoint.OppositeObjectID))
                     && (realObjectEndPoint.OriginalOppositeObjectID == null || objectIDs.ContainsKey(realObjectEndPoint.OriginalOppositeObjectID)))
            {
              realObjectEndPoints.Add(realObjectEndPoint);
            }
            else
            {
              realObjectEndPointsNotPartOfUnloadSet.Add(realObjectEndPoint);
              var virtualEndPointID = realObjectEndPoint.GetOppositeRelationEndPointID();
              if (virtualEndPointID != null)
              {
                var virtualEndPoint = (IVirtualEndPoint?)RelationEndPointManager.GetRelationEndPointWithoutLoading(virtualEndPointID);
                if (virtualEndPoint != null)
                  virtualEndPointsNotPartOfUnloadSet.Add(virtualEndPoint);
              }
            }
          }
        }
        else
        {
          var virtualEndPoint = (IVirtualEndPoint)endPoint;
          if (isPartOfUnloadedSet)
          {
            if (virtualEndPoint.IsNull)
            {
              virtualObjectEndPoints.Add(virtualEndPoint);
            }
            else if (virtualEndPoint.CanBeCollected)
            {
              virtualObjectEndPoints.Add(virtualEndPoint);
            }
            else if (virtualEndPoint.Definition.Cardinality == CardinalityType.One)
            {
              var virtualObjectEndPoint = (IVirtualObjectEndPoint)virtualEndPoint;
              if ((virtualObjectEndPoint.OppositeObjectID == null || objectIDs.ContainsKey(virtualObjectEndPoint.OppositeObjectID))
                  && (virtualObjectEndPoint.OriginalOppositeObjectID == null || objectIDs.ContainsKey(virtualObjectEndPoint.OriginalOppositeObjectID)))
              {
                virtualObjectEndPoints.Add(virtualEndPoint);
              }
              else
              {
                virtualEndPointsNotPartOfUnloadSet.Add(virtualEndPoint);
                var realObjectEndPointID = virtualObjectEndPoint.GetOppositeRelationEndPointID();
                if (realObjectEndPointID != null)
                {
                  var oppositeRealObjectEndPoint = (IRealObjectEndPoint?)RelationEndPointManager.GetRelationEndPointWithoutLoading(realObjectEndPointID);
                  if (oppositeRealObjectEndPoint != null)
                    realObjectEndPointsNotPartOfUnloadSet.Add(oppositeRealObjectEndPoint);
                }
              }
            }
            else
            {
              var collectionEndPoint = (ICollectionEndPoint<ICollectionEndPointData>)virtualEndPoint;
              if (collectionEndPoint.GetData().All(obj => objectIDs.ContainsKey(obj.ID))
                  && collectionEndPoint.GetOriginalData().All(obj => objectIDs.ContainsKey(obj.ID)))
              {
                virtualObjectEndPoints.Add(virtualEndPoint);
              }
              else
              {
                virtualEndPointsNotPartOfUnloadSet.Add(virtualEndPoint);
                var realObjectEndPointIDs = collectionEndPoint.GetOppositeRelationEndPointIDs().Where(epID => epID.ObjectID != null && !objectIDs.ContainsKey(epID.ObjectID));
                foreach (var realObjectEndPointID in realObjectEndPointIDs)
                {
                  var oppositeRealObjectEndPoint = (IRealObjectEndPoint?)RelationEndPointManager.GetRelationEndPointWithoutLoading(realObjectEndPointID);
                  if (oppositeRealObjectEndPoint != null)
                    realObjectEndPointsNotPartOfUnloadSet.Add(oppositeRealObjectEndPoint);
                }
              }
            }
          }
        }
      }

      return (
          RealObjectEndPoints: realObjectEndPoints,
          VirtualEndPoints: virtualObjectEndPoints,
          RealObjectEndPointsNotPartOfUnloadSet: realObjectEndPointsNotPartOfUnloadSet,
          VirtualEndPointsNotPartOfUnloadSet: virtualEndPointsNotPartOfUnloadSet
          );
    }
  }
}
