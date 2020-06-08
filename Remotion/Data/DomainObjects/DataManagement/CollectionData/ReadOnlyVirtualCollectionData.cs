using System;
using System.Collections;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionData
{
  public class ReadOnlyVirtualCollectionData : IVirtualCollectionData, ICollectionEndPointData
  {
    public ReadOnlyVirtualCollectionData ()
    {
      // TODO RM-7294 implement
    }

    public IEnumerator<DomainObject> GetEnumerator ()
    {
      throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public int Count => throw new NotImplementedException();

    public Type RequiredItemType => throw new NotImplementedException();

    public bool IsReadOnly => throw new NotImplementedException();

    public RelationEndPointID AssociatedEndPointID => throw new NotImplementedException();

    public bool IsDataComplete => throw new NotImplementedException();

    public void EnsureDataComplete ()
    {
      throw new NotImplementedException();
    }

    public bool ContainsObjectID (ObjectID objectID)
    {
      throw new NotImplementedException();
    }

    public DomainObject GetObject (int index)
    {
      throw new NotImplementedException();
    }

    public DomainObject GetObject (ObjectID objectID)
    {
      throw new NotImplementedException();
    }

    public int IndexOf (ObjectID objectID)
    {
      throw new NotImplementedException();
    }

    public void Clear ()
    {
      throw new NotImplementedException();
    }

    public void Add (DomainObject domainObject)
    {
      throw new NotImplementedException();
    }

    public bool Remove (DomainObject domainObject)
    {
      throw new NotImplementedException();
    }

    public bool Remove (ObjectID objectID)
    {
      throw new NotImplementedException();
    }

    public void Sort (Comparison<DomainObject> comparison)
    {
      throw new NotImplementedException();
    }
  }
}