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
using System.Diagnostics;

namespace Remotion.Collections
{
  /// <summary>
  /// Case-sensitive name/object dictionary.
  /// </summary>
  [DebuggerDisplay("Count={Count}")]
  public class NameObjectCollection : ICollection, IDictionary, ICloneable
  {
    private Hashtable _hashtable;

    public NameObjectCollection ()
    {
      _hashtable = new Hashtable();
    }

    public object? this [string name]
    {
      get { return _hashtable[name]; }
      set { _hashtable[name] = value; }
    }

    public void Clear ()
    {
      _hashtable.Clear();
    }

    public bool IsSynchronized
    {
      get { return false; }
    }

    public int Count
    {
      get { return _hashtable.Count; }
    }

    public void CopyTo (Array array, int index)
    {
      _hashtable.CopyTo(array, index);
    }

    public object SyncRoot
    {
      get { return this; }
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return ((IEnumerable)_hashtable).GetEnumerator();
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    IDictionaryEnumerator IDictionary.GetEnumerator ()
    {
      return ((IDictionary)_hashtable).GetEnumerator();
    }

    object? IDictionary.this [object key]
    {
      get { return this[(string)key]; }
      set { this[(string)key] = value; }
    }

    public void Remove (string name)
    {
      _hashtable.Remove(name);
    }

    void IDictionary.Remove (object key)
    {
      Remove((string)key);
    }

    public bool Contains (object key)
    {
      return _hashtable.Contains(key);
    }

    bool IDictionary.Contains (object key)
    {
      return _hashtable.Contains(key);
    }

    public ICollection Values
    {
      get { return _hashtable.Values; }
    }

    public void Add (string key, object? value)
    {
      _hashtable.Add(key, value);
    }

    void IDictionary.Add (object key, object? value)
    {
      Add((string)key, value);
    }

    public ICollection Keys
    {
      get { return _hashtable.Keys; }
    }

    public bool IsFixedSize
    {
      get { return false; }
    }

    /// <summary>
    /// Merges two collections. If a key occurs in both collections, the value of the second collection is taken.
    /// </summary>
    public static NameObjectCollection? Merge (NameObjectCollection? first, NameObjectCollection? second)
    {
      if (first == null && second == null)
        return null;
      else if (first == null)
        return second!.Clone();
      if (second == null)
        return first.Clone();

      NameObjectCollection result = first.Clone();

      foreach (DictionaryEntry entry in second)
        result[(string)entry.Key] = entry.Value;

      return result;
    }

    public NameObjectCollection Clone ()
    {
      NameObjectCollection result = new NameObjectCollection();
      foreach (DictionaryEntry entry in this)
        result.Add((string)entry.Key, entry.Value);

      return result;
    }

    object ICloneable.Clone ()
    {
      return Clone();
    }
  }
}
