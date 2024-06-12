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
using System.Collections.ObjectModel;
using Remotion.Utilities;

namespace Remotion.Collections
{
  /// <summary>
  /// Read-only wrapper around an <see cref="ICollection{T}"/> which itself explicitely implements <see cref="ICollection{T}"/>.
  /// </summary>
  /// <remarks>
  /// Behaves analogue to <see cref="ReadOnlyCollection{T}"/>, i.e. not supported methods required by <see cref="ICollection{T}"/> 
  /// throw <see cref="NotSupportedException"/>|s.
  /// <para/>
  /// </remarks>
  public class ReadOnlyCollectionDecorator<T> : ICollection<T>, IReadOnlyCollection<T>
  {
    private readonly ICollection<T> _collection;

    public ReadOnlyCollectionDecorator (ICollection<T> collection)
    {
      ArgumentUtility.CheckNotNull("collection", collection);

      _collection = collection;
    }

    public bool Contains (T item)
    {
      return _collection.Contains(item);
    }

    public void CopyTo (T[] array, int arrayIndex)
    {
      _collection.CopyTo(array, arrayIndex);
    }

    public int Count
    {
      get { return _collection.Count; }
    }

    public bool IsReadOnly
    {
      get { return true; }
    }

    public IEnumerator<T> GetEnumerator ()
    {
      return _collection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    void ICollection<T>.Add (T item)
    {
      throw new NotSupportedException("'Add' ist not supported for read-only collections.");
    }

    bool ICollection<T>.Remove (T item)
    {
      throw new NotSupportedException("'Remove' ist not supported for read-only collections.");
    }

    void ICollection<T>.Clear ()
    {
      throw new NotSupportedException("'Clear' ist not supported for read-only collections.");
    }

  }
}
