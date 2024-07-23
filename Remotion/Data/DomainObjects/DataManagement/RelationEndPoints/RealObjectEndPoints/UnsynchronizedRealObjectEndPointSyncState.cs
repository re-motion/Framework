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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.RealObjectEndPoints
{
  /// <summary>
  /// Represents the state of an <see cref="IObjectEndPoint"/> that is not synchronized with the opposite <see cref="IRelationEndPoint"/>.
  /// </summary>
  public class UnsynchronizedRealObjectEndPointSyncState : IRealObjectEndPointSyncState
  {
    public UnsynchronizedRealObjectEndPointSyncState ()
    {
    }

    public bool? IsSynchronized (IRealObjectEndPoint endPoint)
    {
      return false;
    }

    public void Synchronize (IRealObjectEndPoint endPoint, IVirtualEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);
      ArgumentUtility.CheckNotNull("oppositeEndPoint", oppositeEndPoint);

      oppositeEndPoint.SynchronizeOppositeEndPoint(endPoint);
    }

    public IDataManagementCommand CreateDeleteCommand (IRealObjectEndPoint endPoint, Action oppositeObjectNullSetter)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);
      ArgumentUtility.CheckNotNull("oppositeObjectNullSetter", oppositeObjectNullSetter);

      throw new InvalidOperationException(
          string.Format(
              "The domain object '{0}' cannot be deleted because its relation property '{1}' is out of sync with the opposite property '{2}'. "
              + "To make this change, synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method "
              + "on the '{1}' property.",
              endPoint.ObjectID,
              endPoint.Definition.PropertyName,
              endPoint.Definition.GetOppositeEndPointDefinition().PropertyName));
    }

    public IDataManagementCommand CreateSetCommand (IRealObjectEndPoint endPoint, DomainObject? newRelatedObject, Action<DomainObject> oppositeObjectIDSetter)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);
      ArgumentUtility.CheckNotNull("oppositeObjectIDSetter", oppositeObjectIDSetter);

      throw new InvalidOperationException(
          string.Format(
              "The relation property '{0}' of object '{1}' cannot be changed because it is out of sync with the opposite property '{2}'. "
              + "To make this change, synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method "
              + "on the '{0}' property.",
              endPoint.Definition.PropertyName,
              endPoint.ObjectID,
              endPoint.Definition.GetOppositeEndPointDefinition().PropertyName));
    }
  }
}
