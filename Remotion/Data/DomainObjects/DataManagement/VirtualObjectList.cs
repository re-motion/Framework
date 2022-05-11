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

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Implementation of the <see cref="IObjectList{TDomainObject}"/> interface for use in relations (i.e. <see cref="IVirtualCollectionEndPoint"/>).
  /// </summary>
  /// <typeparam name="T"></typeparam>
  [Serializable]
  public class VirtualObjectList<T> : IObjectList<T>, IReadOnlyCollectionData<T>
      where T : IDomainObject
  {
    private readonly IVirtualCollectionData _dataStrategy;
    private readonly object _syncRoot = new object();

    public VirtualObjectList (IVirtualCollectionData dataStrategy)
    {
      ArgumentUtility.CheckNotNull("dataStrategy", dataStrategy);

      _dataStrategy = dataStrategy;
    }

    public RelationEndPointID AssociatedEndPointID => _dataStrategy.AssociatedEndPointID;


    public bool IsDataComplete => _dataStrategy.IsDataComplete;

    public void EnsureDataComplete () => _dataStrategy.EnsureDataComplete();

    public IEnumerator<T> GetEnumerator () => _dataStrategy.Cast<T>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();

    public int Count => _dataStrategy.Count;

    public T this [int index] => (T)_dataStrategy.GetObject(index);

    public bool Contains (ObjectID objectID) => _dataStrategy.ContainsObjectID(objectID);

    public T? GetObject (ObjectID objectID) => (T?)_dataStrategy.GetObject(objectID);

    bool IList.IsReadOnly => true;

    bool IList.IsFixedSize => false;

    object ICollection.SyncRoot => _syncRoot;

    bool ICollection.IsSynchronized => false;

    void ICollection.CopyTo (Array array, int index)
    {
      ArgumentUtility.CheckNotNull("array", array);

      _dataStrategy.ToArray().CopyTo(array, index);
    }

    bool IList.Contains (object? value)
    {
      if (value is DomainObject domainObject)
        return ReferenceEquals(domainObject, GetObject(domainObject.ID));
      return false;
    }

    int IList.IndexOf (object? value)
    {
      return _dataStrategy
          .Select((obj, index) => new KeyValuePair<DomainObject, int>(obj, index))
          .Where(kvp => ReferenceEquals(kvp.Key, value))
          .Select(kvp => (int?)kvp.Value).FirstOrDefault() ?? -1;
    }

    object? IList.this [int index]
    {
      get { return this[index]; }
      set
      {
        throw new NotSupportedException(
            "The collection does not support updating the data explicitly. Instead, modify the opposite endpoint of this bidirectional relation.");
      }
    }

    int IObjectList<T>.Add (object value)
    {
      throw new NotSupportedException(
          "The collection does not support updating the data explicitly. Instead, modify the opposite endpoint of this bidirectional relation.");
    }

    int IList.Add (object? value)
    {
      throw new NotSupportedException(
          "The collection does not support updating the data explicitly. Instead, modify the opposite endpoint of this bidirectional relation.");
    }

    void IObjectList<T>.Clear ()
    {
      throw new NotSupportedException(
          "The collection does not support updating the data explicitly. Instead, modify the opposite endpoint of this bidirectional relation.");
    }

    void IList.Clear ()
    {
      throw new NotSupportedException(
          "The collection does not support updating the data explicitly. Instead, modify the opposite endpoint of this bidirectional relation.");
    }

    void IObjectList<T>.Insert (int index, object value)
    {
      throw new NotSupportedException(
          "The collection does not support updating the data explicitly. Instead, modify the opposite endpoint of this bidirectional relation.");
    }

    void IList.Insert (int index, object? value)
    {
      throw new NotSupportedException(
          "The collection does not support updating the data explicitly. Instead, modify the opposite endpoint of this bidirectional relation.");
    }

    void IObjectList<T>.Remove (object value)
    {
      throw new NotSupportedException(
          "The collection does not support updating the data explicitly. Instead, modify the opposite endpoint of this bidirectional relation.");
    }

    void IList.Remove (object? value)
    {
      throw new NotSupportedException(
          "The collection does not support updating the data explicitly. Instead, modify the opposite endpoint of this bidirectional relation.");
    }

    void IObjectList<T>.RemoveAt (int index)
    {
      throw new NotSupportedException(
          "The collection does not support updating the data explicitly. Instead, modify the opposite endpoint of this bidirectional relation.");
    }

    void IList.RemoveAt (int index)
    {
      throw new NotSupportedException(
          "The collection does not support updating the data explicitly. Instead, modify the opposite endpoint of this bidirectional relation.");
    }
  }
}
