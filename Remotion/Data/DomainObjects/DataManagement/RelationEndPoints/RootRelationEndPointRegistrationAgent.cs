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

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Extends <see cref="RelationEndPointRegistrationAgent"/> with functionality specific to root transactions. Specifically, it optimizes
  /// end-point registration for 1:1 relations to avoid unnecessary queries.
  /// </summary>
  public class RootRelationEndPointRegistrationAgent : RelationEndPointRegistrationAgent
  {
    public RootRelationEndPointRegistrationAgent (IVirtualEndPointProvider endPointProvider)
        : base(endPointProvider)
    {
    }

    protected override IVirtualEndPoint? RegisterOppositeForRealObjectEndPoint (IRealObjectEndPoint realObjectEndPoint)
    {
      var oppositeVirtualEndPoint = base.RegisterOppositeForRealObjectEndPoint(realObjectEndPoint);

      // Optimization for 1:1 relations: to avoid a database query, we'll mark the virtual end-point complete when the first opposite foreign key
      // is registered with it. We can only do this in root transactions; in sub-transactions we need the query to occur so that we get the same
      // relation state in the sub-transaction as in the root transaction even in the case of unsynchronized end-points.

      var oppositeVirtualObjectEndPoint = oppositeVirtualEndPoint as IVirtualObjectEndPoint;
      if (oppositeVirtualObjectEndPoint != null && !oppositeVirtualObjectEndPoint.IsDataComplete)
        oppositeVirtualObjectEndPoint.MarkDataComplete(realObjectEndPoint.GetDomainObjectReference());

      return oppositeVirtualEndPoint;
    }
  }
}
