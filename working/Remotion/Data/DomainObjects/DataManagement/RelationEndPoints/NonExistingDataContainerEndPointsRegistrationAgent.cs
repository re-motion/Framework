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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Handles end-point registration for <see cref="DataContainer"/> instances that do not exist in the underlying data source. Such data containers
  /// own both virtual and non-virtual end-points, and when unloading, these end-points must not reference any related objects (because the 
  /// references cannot be reconstructed and the related end-points would therefore end up as dangling references).
  /// </summary>
  [Serializable]
  public class NonExistingDataContainerEndPointsRegistrationAgent : DataContainerEndPointsRegistrationAgentBase
  {
    public NonExistingDataContainerEndPointsRegistrationAgent (IRelationEndPointFactory endPointFactory, IRelationEndPointRegistrationAgent registrationAgent)
        : base(endPointFactory, registrationAgent)
    {
    }

    protected override IEnumerable<RelationEndPointID> GetOwnedEndPointIDs (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      return dataContainer.AssociatedRelationEndPointIDs;
    }

    protected override string GetUnregisterProblem (IRelationEndPoint endPoint, RelationEndPointMap relationEndPointMap)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("relationEndPointMap", relationEndPointMap);

      var objectEndPoint = endPoint as IObjectEndPoint;
      if (objectEndPoint != null)
      {
        if (objectEndPoint.OppositeObjectID == null && objectEndPoint.OriginalOppositeObjectID == null)
          return null;
      }
      else
      {
        var collectionEndPoint = (ICollectionEndPoint) endPoint;
        if (collectionEndPoint.GetData ().Count == 0 && collectionEndPoint.GetOriginalData ().Count == 0)
          return null;
      }

      return string.Format ("Relation end-point '{0}' would leave a dangling reference.", endPoint.ID);
    }
  }
}