using System;
using System.Collections;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionData
{
  public class ReadOnlyVirtualCollectionDataDecorator : IVirtualCollectionData, ICollectionEndPointData
  {
    //TODO: RM-7294: drop IVirtualCollectionData interface from type

    private readonly IVirtualCollectionData _collectionData;

    public ReadOnlyVirtualCollectionDataDecorator (IVirtualCollectionData collectionData)
    {
      ArgumentUtility.CheckNotNull ("collectionData", collectionData);

      _collectionData = collectionData;
    }

    public IEnumerator<DomainObject> GetEnumerator () => _collectionData.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator () => _collectionData.GetEnumerator();

    public int Count => _collectionData.Count;

    public Type RequiredItemType => _collectionData.RequiredItemType;

    public bool IsReadOnly => true;

    public RelationEndPointID AssociatedEndPointID => _collectionData.AssociatedEndPointID;

    public bool IsDataComplete => _collectionData.IsDataComplete;

    public void EnsureDataComplete () => _collectionData.EnsureDataComplete();

    public bool ContainsObjectID (ObjectID objectID) => _collectionData.ContainsObjectID (objectID);

    public DomainObject GetObject (int index) => _collectionData.GetObject (index);

    public DomainObject GetObject (ObjectID objectID) => _collectionData.GetObject (objectID);

    public int IndexOf (ObjectID objectID) => _collectionData.IndexOf (objectID);

    void IVirtualCollectionData.Clear () => throw new NotSupportedException ("Cannot clear a read-only collection.");

    void IVirtualCollectionData.Add (DomainObject domainObject) => throw new NotSupportedException ("Cannot add an item to a read-only collection.");

    bool IVirtualCollectionData.Remove (DomainObject domainObject) => throw new NotSupportedException ("Cannot remove an item from a read-only collection.");

    bool IVirtualCollectionData.Remove (ObjectID objectID) => throw new NotSupportedException ("Cannot remove an item from a read-only collection.");

    void IVirtualCollectionData.Sort (Comparison<DomainObject> comparison) => throw new NotSupportedException ("Cannot sort a read-only collection."); 
  }
}