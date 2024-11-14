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
using Microsoft.Extensions.Logging;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.RealObjectEndPoints
{
  /// <summary>
  /// Represents the synchronization state of an <see cref="ObjectEndPoint"/> whose opposite end-point is not loaded/complete yet.
  /// In this case, the synchronization state is unknown until the opposite end-point is loaded. Any access to the sync state will cause the
  /// opposite end-point to be loaded.
  /// </summary>
  public class UnknownRealObjectEndPointSyncState : IRealObjectEndPointSyncState
  {
    private static readonly ILogger s_logger = LazyLoggerFactory.CreateLogger<UnknownRealObjectEndPointSyncState>();

    private readonly IVirtualEndPointProvider _virtualEndPointProvider;

    public UnknownRealObjectEndPointSyncState (IVirtualEndPointProvider virtualEndPointProvider)
    {
      ArgumentUtility.CheckNotNull("virtualEndPointProvider", virtualEndPointProvider);
      _virtualEndPointProvider = virtualEndPointProvider;
    }

    public IVirtualEndPointProvider VirtualEndPointProvider
    {
      get { return _virtualEndPointProvider; }
    }

    public bool? IsSynchronized (IRealObjectEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);

      return null;
    }

    public void Synchronize (IRealObjectEndPoint endPoint, IVirtualEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);

      LoadOppositeEndPoint(endPoint);

      endPoint.Synchronize();
    }

    public IDataManagementCommand CreateDeleteCommand (IRealObjectEndPoint endPoint, Action oppositeObjectNullSetter)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);
      ArgumentUtility.CheckNotNull("oppositeObjectNullSetter", oppositeObjectNullSetter);

      LoadOppositeEndPoint(endPoint);

      return endPoint.CreateDeleteCommand();
    }

    public IDataManagementCommand CreateSetCommand (IRealObjectEndPoint endPoint, DomainObject? newRelatedObject, Action<DomainObject> oppositeObjectIDSetter)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);
      ArgumentUtility.CheckNotNull("oppositeObjectIDSetter", oppositeObjectIDSetter);

      LoadOppositeEndPoint(endPoint);

      return endPoint.CreateSetCommand(newRelatedObject);
    }

    private void LoadOppositeEndPoint (IRealObjectEndPoint endPoint)
    {
      var oppositeID = RelationEndPointID.CreateOpposite(endPoint.Definition, endPoint.OppositeObjectID);
      Assertion.IsFalse(oppositeID.Definition.IsAnonymous, "Unidirectional end-points don't get used in unknown state.");

      var oppositeEndPoint = _virtualEndPointProvider.GetOrCreateVirtualEndPoint(oppositeID);
      oppositeEndPoint.EnsureDataComplete();
    }
  }
}
