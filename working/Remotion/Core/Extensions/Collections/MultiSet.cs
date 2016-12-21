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
using System.Diagnostics;
using Remotion.Collections;

namespace Remotion.Mixins.Utilities
{
  [DebuggerDisplay ("Count = {_items.Count}")]
  internal class MultiSet<T> : IEnumerable<T>
  {
    private MultiDictionary<T, T> _items;

    public MultiSet ()
    {
      _items = new MultiDictionary<T, T>();
    }

    public MultiSet (IEqualityComparer<T> comparer)
    {
      _items = new MultiDictionary<T, T> (comparer);
    }

    public void Add(T item)
    {
      _items.Add (item, item);
    }

    public void AddRange (IEnumerable<T> items)
    {
      foreach (T t in items)
        Add (t);
    }

    public IEnumerable<T> this [T item]
    {
      get { return _items[item]; }
    }

    public int GetItemCount (T item)
    {
      return _items[item].Count;
    }

    public IEnumerable<T> GetUniqueItems()
    {
      foreach (T firstItem in _items.Keys)
        yield return firstItem;
    }

    public IEnumerator<T> GetEnumerator()
    {
      foreach (T firstItem in _items.Keys)
        foreach (T item in _items[firstItem])
          yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
