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

namespace Remotion.Collections
{
  /// <summary>
  ///   A dictionary that automatically creates new value objects when queried for a specific key.
  /// </summary>
  /// <remarks>
  ///		This collection should usually not be modified using <see cref="IDictionary{TKey,TValue}.Add(TKey,TValue)"/>, setting values through 
  ///   the indexer or removing items. Getting values through the indexer will assign a new object to the specified key if none exists.
  /// </remarks>
  [DebuggerDisplay("Count={Count}")]
  public class AutoInitDictionary<TKey, TValue> : IDictionary<TKey, TValue>
      where TKey : notnull
  {
    private Dictionary<TKey, TValue> _dictionary;
    private Func<TValue>? _createMethod;

    public AutoInitDictionary ()
      : this(null, null)
    {
    }

    public AutoInitDictionary (Func<TValue>? createMethod)
      : this(createMethod, null)
    {
    }

    public AutoInitDictionary (IEqualityComparer<TKey>? comparer)
      : this(null, comparer)
    {
    }

    public AutoInitDictionary (Func<TValue>? createMethod, IEqualityComparer<TKey>? comparer)
    {
      _createMethod = createMethod;
      _dictionary = new Dictionary<TKey, TValue>(comparer);
    }

    private TValue CreateValue ()
    {
      if (_createMethod != null)
        return _createMethod();
      else
        return Activator.CreateInstance<TValue>();
    }

    public TValue this[TKey key]
    {
      get
      {
        if (!_dictionary.TryGetValue(key, out var value))
        {
          // TODO RM-7749: return value of CreateValue should be checked for null.
          value = CreateValue();
          _dictionary.Add(key, value);
        }
        return value;
      }
    }

    // ICollection <KeyValuePair<TKey,TValue>> Members

    private ICollection<KeyValuePair<TKey, TValue>> AsCollection
    {
      get { return _dictionary; }
    }

    void IDictionary<TKey, TValue>.Add (TKey key, TValue value)
    {
      _dictionary.Add(key, value);
    }

    bool IDictionary<TKey, TValue>.ContainsKey (TKey key)
    {
      return _dictionary.ContainsKey(key);
    }

    public ICollection<TKey> Keys
    {
      get { return _dictionary.Keys; }
    }

    bool IDictionary<TKey, TValue>.Remove (TKey key)
    {
      return _dictionary.Remove(key);
    }

    bool IDictionary<TKey,TValue>.TryGetValue (TKey key, out TValue value)
    {
      value = this[key];
      return true;
    }

    public ICollection<TValue> Values
    {
      get { return _dictionary.Values; }
    }

    TValue IDictionary<TKey,TValue>.this[TKey key]
    {
      get { return this[key]; }
      set { _dictionary[key] = value; }
    }

    void ICollection<KeyValuePair<TKey,TValue>>.Add (KeyValuePair<TKey, TValue> item)
    {
      AsCollection.Add(item);
    }

    public void Clear ()
    {
      _dictionary.Clear();
    }

    bool ICollection<KeyValuePair<TKey,TValue>>.Contains (KeyValuePair<TKey, TValue> item)
    {
      return AsCollection.Contains(item);
    }

    public bool ContainsKey (TKey key)
    {
      return _dictionary.ContainsKey(key);
    }

    void ICollection<KeyValuePair<TKey,TValue>>.CopyTo (KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      AsCollection.CopyTo(array, arrayIndex);
    }

    public int Count
    {
      get { return _dictionary.Count; }
    }

    bool ICollection<KeyValuePair<TKey,TValue>>.IsReadOnly
    {
      get { return false; }
    }

    public bool Remove (KeyValuePair<TKey, TValue> item)
    {
      return AsCollection.Remove(item);
    }

    // IEnumerable<KeyValuePair<TKey,TValue>> Members

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey,TValue>>.GetEnumerator ()
    {
      return ((IEnumerable<KeyValuePair<TKey, TValue>>)_dictionary).GetEnumerator();
    }

    // IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return ((IEnumerable)_dictionary).GetEnumerator();
    }
  }
}
