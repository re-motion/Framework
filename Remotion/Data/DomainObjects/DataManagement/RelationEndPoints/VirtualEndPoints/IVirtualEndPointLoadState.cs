using System;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints
{
  /// <summary>
  /// Represents the lazy-loading state of an <see cref="IVirtualEndPoint"/> and implements accessor methods for that end-point.
  /// </summary>
  /// <typeparam name="TEndPoint">The type of the end point whose state is managed by this instance.</typeparam>
  /// <typeparam name="TData">The type of data held by the <typeparamref name="TDataManager"/>.</typeparam>
  /// <typeparam name="TDataManager">The type of <see cref="IVirtualEndPointDataManager"/> holding the data for the end-point.</typeparam>
  public interface IVirtualEndPointLoadState<TEndPoint, TData, TDataManager> : IFlattenedSerializable
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