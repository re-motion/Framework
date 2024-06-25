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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// This class acts as a read-only adapter for an <see cref="IDomainObjectCollectionData"/> object.
  /// </summary>
  public class ReadOnlyDomainObjectCollectionAdapter<T> : IReadOnlyCollectionData<T>, IList<T> where T : DomainObject
  {
    private readonly DomainObjectCollection _wrappedData;

    public ReadOnlyDomainObjectCollectionAdapter (DomainObjectCollection wrappedData)
    {
      ArgumentUtility.CheckNotNull("wrappedData", wrappedData);
      _wrappedData = wrappedData;
    }

    public Type? RequiredItemType
    {
      get { return _wrappedData.RequiredItemType; }
    }

    public RelationEndPointID? AssociatedEndPointID
    {
      get { return _wrappedData.AssociatedEndPointID; }
    }

    public bool IsDataComplete
    {
      get { return _wrappedData.IsDataComplete; }
    }

    public int Count
    {
      get { return _wrappedData.Count; }
    }

    public IEnumerator<T> GetEnumerator ()
    {
      return _wrappedData.Cast<T>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public bool Contains (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);
      return _wrappedData.Contains(objectID);
    }

    public bool ContainsObject (T domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      return _wrappedData.ContainsObject(domainObject);
    }

    public T this[int index]
    {
      get { return (T)_wrappedData[index]; }
    }

    public T? this[ObjectID objectID]
    {
      get
      {
        ArgumentUtility.CheckNotNull("objectID", objectID);
        return GetObject(objectID);
      }
    }

    public T? GetObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);
      return (T?)_wrappedData[objectID];
    }

    public int IndexOf (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);
      return _wrappedData.IndexOf(objectID);
    }

    public void CopyTo (T[] array, int arrayIndex)
    {
      _wrappedData.CopyTo(array, arrayIndex);
    }

    public int IndexOf (T item)
    {
      ArgumentUtility.CheckNotNull("item", item);
      return _wrappedData.IndexOf(item);
    }

    void ICollection<T>.Add (T item)
    {
      throw new NotSupportedException("This collection does not support modifications.");
    }

    void ICollection<T>.Clear ()
    {
      throw new NotSupportedException("This collection does not support modifications.");
    }

    bool ICollection<T>.Contains (T item)
    {
      return ContainsObject(item);
    }

    bool ICollection<T>.Remove (T item)
    {
      throw new NotSupportedException("This collection does not support modifications.");
    }

    bool ICollection<T>.IsReadOnly
    {
      get { return true; }
    }

    void IList<T>.Insert (int index, T item)
    {
      throw new NotSupportedException("This collection does not support modifications.");
    }

    void IList<T>.RemoveAt (int index)
    {
      throw new NotSupportedException("This collection does not support modifications.");
    }

    T IList<T>.this [int index]
    {
      get { return this[index]; }
      set { throw new NotSupportedException("This collection does not support modifications."); }
    }
  }
}
