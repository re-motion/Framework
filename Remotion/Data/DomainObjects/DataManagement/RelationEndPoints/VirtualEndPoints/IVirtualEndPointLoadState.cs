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

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints
{
  /// <summary>
  /// Represents the lazy-loading state of an <see cref="IVirtualEndPoint"/> and implements accessor methods for that end-point.
  /// </summary>
  /// <typeparam name="TEndPoint">The type of the end point whose state is managed by this instance.</typeparam>
  /// <typeparam name="TData">The type of data held by the <typeparamref name="TDataManager"/>.</typeparam>
  /// <typeparam name="TDataManager">The type of <see cref="IVirtualEndPointDataManager"/> holding the data for the end-point.</typeparam>
  public interface IVirtualEndPointLoadState<TEndPoint, TData, TDataManager>
      where TEndPoint : IVirtualEndPoint<TData>
      where TDataManager : IVirtualEndPointDataManager
  {
    bool IsDataComplete ();

    bool CanEndPointBeCollected (TEndPoint endPoint);

    bool CanDataBeMarkedIncomplete (TEndPoint endPoint);
    void MarkDataIncomplete (TEndPoint endPoint, Action stateSetter);

    TData GetData (TEndPoint endPoint);
    TData GetOriginalData (TEndPoint endPoint);

    void RegisterOriginalOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint);
    void UnregisterOriginalOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint);

    void RegisterCurrentOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint);
    void UnregisterCurrentOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint);

    bool? IsSynchronized (TEndPoint endPoint);
    void Synchronize (TEndPoint endPoint);

    void SynchronizeOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint);

    void SetDataFromSubTransaction (TEndPoint endPoint, IVirtualEndPointLoadState<TEndPoint, TData, TDataManager> sourceLoadState);

    bool HasChanged ();

    void Commit (TEndPoint endPoint);
    void Rollback (TEndPoint endPoint);
  }
}
