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

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Provides extension methods for <see cref="IRelationEndPoint"/>.
  /// </summary>
  public static class RelationEndPointExtensions
  {
    public static T GetEndPointWithOppositeDefinition<T> (this IRelationEndPoint endPoint, IDomainObject? oppositeObject) where T : IRelationEndPoint
    {
      return endPoint.GetEndPointWithOppositeDefinition<T>(oppositeObject.GetSafeID());
    }

    public static T GetEndPointWithOppositeDefinition<T> (this IRelationEndPoint endPoint, ObjectID? oppositeObjectID) where T : IRelationEndPoint
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);

      var oppositeDefinition = endPoint.Definition.GetOppositeEndPointDefinition();

      var id = RelationEndPointID.Create(oppositeObjectID, oppositeDefinition);
      var oppositeEndPoint = endPoint.ClientTransaction.DataManager.GetRelationEndPointWithLazyLoad(id);

      if (!(oppositeEndPoint is T))
      {
        var message = String.Format(
            "The opposite end point '{0}' is of type '{1}', not of type '{2}'.",
            oppositeEndPoint.ID,
            oppositeEndPoint.GetType(),
            typeof(T));
        throw new InvalidOperationException(message);
      }

      return (T)oppositeEndPoint;
    }
  }
}
