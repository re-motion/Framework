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
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
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
      return Enumerable.Empty<Exception>();
    }

    public void Begin ()
    {
      _dataContainersForUnloading = DataContainerMap
          .Where(dc => DomainObjectFilter(dc.DomainObject))
          .ToList()
          .AsReadOnly();

      var relationEndPoints = GetRelationEndPointsForUnloading(_dataContainersForUnloading);
      _realObjectEndPointsForUnloading = relationEndPoints.RealObjectEndPoints;
      _virtualEndPointsForUnloading = relationEndPoints.VirtualEndPoints;
      if (_dataContainersForUnloading.Count > 0)
        TransactionEventSink.RaiseObjectsUnloadingEvent(_dataContainersForUnloading.Select(dc => dc.DomainObject).ToList());
    }

    public void Perform ()
    {
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
      if (_dataContainersForUnloading.Count > 0)
        TransactionEventSink.RaiseObjectsUnloadingEvent(_dataContainersForUnloading.Select(dc=>dc.DomainObject).ToList());
    }

    public ExpandedCommand ExpandToAllRelatedObjects ()
    {
      return new ExpandedCommand(this);
    }

    private (IReadOnlyCollection<IRealObjectEndPoint> RealObjectEndPoints, IReadOnlyCollection<IVirtualEndPoint> VirtualEndPoints) GetRelationEndPointsForUnloading (
        IReadOnlyCollection<DataContainer> dataContainers)
    {
      var realObjectEndPoints = new HashSet<IRealObjectEndPoint>();
      var virtualObjectEndPoints = new HashSet<IVirtualEndPoint>();

      var endPoints = dataContainers
          .SelectMany(dc => dc.AssociatedRelationEndPointIDs.Select(endPointID => RelationEndPointManager.GetRelationEndPointWithoutLoading(endPointID)))
          .Where(ep => ep != null)
          .Select(ep => ep!)
          .Where(ep => !ep.IsNull);

      foreach (var endPoint in endPoints)
      {
        if (endPoint is IRealObjectEndPoint realObjectEndPoint)
        {
          realObjectEndPoints.Add(realObjectEndPoint);
        }
        else
        {
          var virtualEndPoint = (IVirtualEndPoint)endPoint;
          virtualObjectEndPoints.Add(virtualEndPoint);
        }
      }

      //          if (virtualEndPoint.CanBeCollected)

      return (RealObjectEndPoints: realObjectEndPoints, VirtualEndPoints: virtualObjectEndPoints);
    }
  }
}
