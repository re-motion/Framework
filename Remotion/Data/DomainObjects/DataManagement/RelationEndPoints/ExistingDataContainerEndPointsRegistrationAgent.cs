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
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Handles end-point registration for <see cref="DataContainer"/> instances that exist in the underlying data source. Such data containers
  /// only own their non-virtual end-points, and when unloading, these end-points must not be changed (because the changes would be lost and might
  /// leave other end-points in an inconsistent state).
  /// </summary>
  [Serializable]
  public class ExistingDataContainerEndPointsRegistrationAgent : DataContainerEndPointsRegistrationAgentBase
  {
    public ExistingDataContainerEndPointsRegistrationAgent (
        IRelationEndPointFactory endPointFactory, 
        IRelationEndPointRegistrationAgent registrationAgent)
        : base(endPointFactory, registrationAgent)
    {
    }

    protected override IEnumerable<RelationEndPointID> GetOwnedEndPointIDs (DataContainer dataContainer)
    {
      return dataContainer.AssociatedRelationEndPointIDs.Where (id => !id.Definition.IsVirtual);
    }

    protected override string GetUnregisterProblem (IRelationEndPoint endPoint, RelationEndPointMap relationEndPointMap)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("relationEndPointMap", relationEndPointMap);

      // An end-point must be unchanged to be unregisterable.
      if (endPoint.HasChanged)
      {
        return string.Format (
            "Relation end-point '{0}' has changed. Only unchanged relation end-points can be unregistered.",
            endPoint.ID);
      }

      // If it is a real object end-point pointing to a non-null object, and the opposite end-point is loaded, the opposite (virtual) end-point 
      // must be unchanged. Virtual end-points cannot exist in changed state without their opposite real end-points.
      // (This only affects 1:n relations: for those, the opposite virtual end-point can be changed although the (one of many) real end-point is 
      // unchanged. For 1:1 relations, the real and virtual end-points always have an equal HasChanged flag.)

      var maybeOppositeEndPoint =
          Maybe
              .ForValue (endPoint as IRealObjectEndPoint)
              .Select (ep => RelationEndPointID.CreateOpposite (ep.Definition, ep.OppositeObjectID))
              .Select (oppositeEndPointID => relationEndPointMap[oppositeEndPointID]);
      if (maybeOppositeEndPoint.Where (ep => ep.HasChanged).HasValue)
      {
        return string.Format (
            "The opposite relation property '{0}' of relation end-point '{1}' has changed. Non-virtual end-points that are part of changed relations "
            + "cannot be unloaded.",
            maybeOppositeEndPoint.Value().Definition.PropertyName,
            endPoint.ID);
      }

      return null;
    }
  }
}