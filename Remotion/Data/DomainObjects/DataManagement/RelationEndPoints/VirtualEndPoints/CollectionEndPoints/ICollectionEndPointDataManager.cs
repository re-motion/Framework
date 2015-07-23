using System;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Defines an interface for classes storing the data for a <see cref="CollectionEndPoint"/>.
  /// </summary>
  public interface ICollectionEndPointDataManager : IVirtualEndPointDataManager
  {
    bool? HasDataChangedFast ();

    IDomainObjectCollectionData CollectionData { get; }
    ReadOnlyCollectionDataDecorator OriginalCollectionData { get; }

    IRealObjectEndPoint[] OriginalOppositeEndPoints { get; }
    DomainObject[] OriginalItemsWithoutEndPoints { get; }
    IRealObjectEndPoint[] CurrentOppositeEndPoints { get; }

    bool ContainsOriginalItemWithoutEndPoint (DomainObject domainObject);

    void SortCurrentData (Comparison<DomainObject> comparison);
    void SortCurrentAndOriginalData (Comparison<DomainObject> comparison);
    void SetDataFromSubTransaction (ICollectionEndPointDataManager sourceDataManager, IRelationEndPointProvider endPointProvider);
  }
}