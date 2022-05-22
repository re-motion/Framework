using System;
using System.Collections;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionData
{
  /// <summary>
  /// This class acts as a read-only decorator for another <see cref="IVirtualCollectionData"/> object. Every modifying method 
  /// of the <see cref="IVirtualCollectionData"/> interface will throw an <see cref="NotSupportedException"/> when invoked on this class.
  /// Modifications are still possible via the <see cref="IVirtualCollectionData"/> passed into the <see cref="ReadOnlyDomainObjectCollectionDataDecorator"/>'s
  /// constructor.
  /// </summary>
  public class ReadOnlyVirtualCollectionDataDecorator : IVirtualCollectionData, ICollectionEndPointData
  {
    private readonly IVirtualCollectionData _collectionData;

    public ReadOnlyVirtualCollectionDataDecorator (IVirtualCollectionData collectionData)
    {
      ArgumentUtility.CheckNotNull("collectionData", collectionData);

      _collectionData = collectionData;
    }

    public IEnumerator<IDomainObject> GetEnumerator () => _collectionData.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator () => _collectionData.GetEnumerator();

    public int Count => _collectionData.Count;

    public Type RequiredItemType => _collectionData.RequiredItemType;

    public bool IsReadOnly => true;

    public RelationEndPointID AssociatedEndPointID => _collectionData.AssociatedEndPointID;

    public bool IsDataComplete => _collectionData.IsDataComplete;

    public void EnsureDataComplete () => _collectionData.EnsureDataComplete();

    public bool ContainsObjectID (ObjectID objectID) => _collectionData.ContainsObjectID(objectID);

    public IDomainObject GetObject (int index) => _collectionData.GetObject(index);

    public IDomainObject? GetObject (ObjectID objectID) => _collectionData.GetObject(objectID);

    public int IndexOf (ObjectID objectID) => _collectionData.IndexOf(objectID);

    void IVirtualCollectionData.Clear () => throw new NotSupportedException("Cannot clear a read-only collection.");

    void IVirtualCollectionData.Add (IDomainObject domainObject) => throw new NotSupportedException("Cannot add an item to a read-only collection.");

    bool IVirtualCollectionData.Remove (IDomainObject domainObject) => throw new NotSupportedException("Cannot remove an item from a read-only collection.");
  }
}
