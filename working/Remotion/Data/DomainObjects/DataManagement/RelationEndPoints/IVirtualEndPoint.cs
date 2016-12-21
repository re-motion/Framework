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
  /// Represents the relation property that does not hold the foreign key in a bidirectional relation.
  /// </summary>
  public interface IVirtualEndPoint : IRelationEndPoint
  {
    bool CanBeCollected { get; }
    bool CanBeMarkedIncomplete { get; }

    void MarkDataIncomplete ();

    void RegisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint);
    void UnregisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint);
    void RegisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint);
    void UnregisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint);

    /// <summary>
    /// Synchronizes the opposite end point with this end-point. Must only be called if the oppositeEndPoint is out-of-sync with this end-point.
    /// </summary>
    void SynchronizeOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint);
  }

  /// <summary>
  /// Represents the relation property that does not hold the foreign key in a bidirectional relation. Adds the <see cref="GetData"/> and
  /// <see cref="GetOriginalData"/> members to <see cref="IVirtualEndPoint"/>.
  /// </summary>
  public interface IVirtualEndPoint<TData> : IVirtualEndPoint
  {
    TData GetData ();
    TData GetOriginalData ();
  }
}