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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Implements the <see cref="IList{T}"/> interface around a <see cref="DomainObjectCollection"/>.
  /// </summary>
  /// <typeparam name="T">The item type of the <see cref="IList{T}"/>. This must be assignable from the <see cref="DomainObjectCollection"/>'s
  /// <see cref="DomainObjectCollection.RequiredItemType"/>. If it is more general than the item type, the <see cref="DomainObjectCollection"/>'s
  /// runtime checks will ensure that only compatible items are inserted into the list.</typeparam>
  /// <remarks><see cref="DomainObjectCollection"/> is a non-generic collection type. For use with LINQ or other strongly-typed APIs, this
  /// adapter can be used. Use <see cref="DomainObjectCollectionExtensions.AsList{T}"/> to conveniently construct an implementation of this
  /// class from a <see cref="DomainObjectCollection"/>.</remarks>
  public class DomainObjectCollectionWrapper<T> : IList<T>
      where T : DomainObject
  {
    private readonly DomainObjectCollection _wrappedCollection;

    public DomainObjectCollectionWrapper (DomainObjectCollection wrappedCollection)
    {
      ArgumentUtility.CheckNotNull ("wrappedCollection", wrappedCollection);

      var requiredItemType = wrappedCollection.RequiredItemType ?? typeof (DomainObject);
      if (!typeof (T).IsAssignableFrom (requiredItemType))
      {
        var message = string.Format (
            "Cannot implement 'IList<{0}>' for a DomainObjectCollection with required item type '{1}'. The IList<T>'s item type must be assignable "
            + "from the required item type.", 
            typeof (T), 
            requiredItemType);
        throw new ArgumentException (message, "wrappedCollection");
      }

      _wrappedCollection = wrappedCollection;
    }

    public DomainObjectCollection WrappedCollection
    {
      get { return _wrappedCollection; }
    }

    public int Count
    {
      get { return _wrappedCollection.Count; }
    }

    public bool IsReadOnly
    {
      get { return _wrappedCollection.IsReadOnly; }
    }

    public T this[int index]
    {
      get { return (T) _wrappedCollection[index]; }
      set { _wrappedCollection[index] = value; }
    }

    public IEnumerator<T> GetEnumerator ()
    {
      return _wrappedCollection.Cast<T> ().GetEnumerator();
    }

    public bool Contains (T item)
    {
      return _wrappedCollection.ContainsObject (item);
    }

    public int IndexOf (T item)
    {
      return _wrappedCollection.IndexOf (item);
    }

    public void Insert (int index, T item)
    {
      _wrappedCollection.Insert (index, item);
    }

    public void RemoveAt (int index)
    {
      _wrappedCollection.RemoveAt (index);
    }

    public void Add (T item)
    {
      _wrappedCollection.Add (item);
    }

    public void Clear ()
    {
      _wrappedCollection.Clear ();
    }

    public bool Remove (T item)
    {
      return _wrappedCollection.Remove (item);
    }
    
    public void CopyTo (T[] array, int arrayIndex)
    {
      _wrappedCollection.CopyTo (array, arrayIndex);
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }
  }
}