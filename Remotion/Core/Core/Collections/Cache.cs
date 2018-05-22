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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Collections
{
  /// <summary>
  /// Implements the <see cref="ICache{TKey,TValue}"/> interface to provide a simple, dictionary-based cache for values.
  /// </summary>
  /// <typeparam name="TKey">The type of the keys.</typeparam>
  /// <typeparam name="TValue">The type of the values.</typeparam>
  /// <remarks>
  /// It is recommended to use the <see cref="CacheFactory"/> type to create instances of the desired cache implementation 
  /// (e.g. thread-safe implementations, implementations with support for data invalidation, etc).
  /// </remarks>
  /// <threadsafety static="true" instance="false" />
  [Serializable]
  public sealed class Cache<TKey, TValue> : ICache<TKey, TValue> 
  {
    private readonly Dictionary<TKey, TValue> _innerDictionary;

    public Cache ()
      : this (null)
    {
    }

    public Cache ([CanBeNull] IEqualityComparer<TKey> comparer)
    {
      _innerDictionary = new Dictionary<TKey, TValue> (comparer);
    }

    public IEqualityComparer<TKey> Comparer
    {
      get { return _innerDictionary.Comparer; }
    }

    public void Add (TKey key, TValue value)
    {
      ArgumentUtility.DebugCheckNotNull ("key", key);
      // value can be null

      _innerDictionary[key] = value;
    }

    public bool TryGetValue (TKey key, out TValue value)
    {
      ArgumentUtility.DebugCheckNotNull ("key", key);
      
      return _innerDictionary.TryGetValue (key, out value);
    }

    public TValue GetOrCreateValue (TKey key, Func<TKey,TValue> valueFactory)
    {
      ArgumentUtility.DebugCheckNotNull ("key", key);
      ArgumentUtility.DebugCheckNotNull ("valueFactory", valueFactory);

      TValue value;
      if (!TryGetValue (key, out value))
      {
        ArgumentUtility.CheckNotNull ("valueFactory", valueFactory);

        value = valueFactory (key);
        Add (key, value);
      }
      return value;
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator ()
    {
      return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    private IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator ()
    {
      return _innerDictionary.GetEnumerator();
    }

    public void Clear ()
    {
      _innerDictionary.Clear();
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
