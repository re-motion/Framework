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
using Remotion.Utilities;

namespace Remotion.Collections
{
  /// <summary>
  /// Implements the <see cref="IDataStore{TKey,TValue}"/> interface as a simple, not thread-safe in-memory data store based on a 
  /// <see cref="Dictionary{TKey,TValue}"/>.
  /// </summary>
  /// <typeparam name="TKey">The type of the keys.</typeparam>
  /// <typeparam name="TValue">The type of the values.</typeparam>
  [Serializable]
  public class SimpleDataStore<TKey, TValue> : IDataStore<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>
  {
    private readonly Dictionary<TKey, TValue> _innerDictionary;

    public SimpleDataStore ()
      :this (null)
    {
    }

    public SimpleDataStore (IEqualityComparer<TKey> comparer)
    {
      _innerDictionary = new Dictionary<TKey, TValue> (comparer);
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }

    public IEqualityComparer<TKey> Comparer
    {
      get { return _innerDictionary.Comparer; }
    }

    /// <summary>
    /// Determines whether the store contains an element with the specified <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key to look up.</param>
    /// <returns>
    /// true if the store contains the specified key; otherwise, false.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    public bool ContainsKey (TKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      return _innerDictionary.ContainsKey (key);
    }

    /// <summary>
    /// Adds a new element to the store.
    /// </summary>
    /// <param name="key">The key of the new element.</param>
    /// <param name="value">The value of the new element.</param>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">An item with an equal key already exists in the store.</exception>
    public void Add (TKey key, TValue value)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      // value can be null

      try
      {
        _innerDictionary.Add (key, value);
      }
      catch (ArgumentException ex)
      {
        string message =
            string.Format ("The store already contains an element with key '{0}'. (Old value: '{1}', new value: '{2}')", key, this[key], value);
        throw new ArgumentException (message, "key", ex);
      }
    }

    /// <summary>
    /// Removes the element with the specified key from the store, if any.
    /// </summary>
    /// <param name="key">The key of the element to be removed.</param>
    /// <returns>
    /// true if the item was found in the store; otherwise, false.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    public bool Remove (TKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      return _innerDictionary.Remove (key);
    }

    /// <summary>
    /// Removes all elements from the store.
    /// </summary>
    public void Clear ()
    {
      _innerDictionary.Clear();
    }

    /// <summary>
    /// Gets or sets the <typeparamref name="TValue"/> with the specified key.
    /// </summary>
    /// <value></value>
    public TValue this[TKey key]
    {
      get
      {
        ArgumentUtility.CheckNotNull ("key", key);
        try
        {
          return _innerDictionary[key];
        }
        catch (KeyNotFoundException ex)
        {
          string message = string.Format ("There is no element with key '{0}' in the store.", key);
          throw new KeyNotFoundException (message, ex);
        }
      }
      set 
      {
        ArgumentUtility.CheckNotNull ("key", key);
        _innerDictionary[key] = value; 
      }
    }

    /// <summary>
    /// Gets the value of the element with the specified key, or <typeparamref name="TValue"/>'s default value if no such element exists.
    /// </summary>
    /// <param name="key">The key to look up.</param>
    /// <returns>
    /// The value of the element, or the default value if no such element exists.
    /// </returns>
    public TValue GetValueOrDefault (TKey key)
    {
      ArgumentUtility.DebugCheckNotNull ("key", key);

      TValue value;
      TryGetValue (key, out value);
      return value;
    }

    /// <summary>
    /// Tries to get the value of the element with the specified key.
    /// </summary>
    /// <param name="key">The key to look up.</param>
    /// <param name="value">The value of the element with the specified key, or <typeparamref name="TValue"/>'s default value if no such element
    /// exists.</param>
    /// <returns>
    /// true if an element with the specified key was found; otherwise, false.
    /// </returns>
    public bool TryGetValue (TKey key, out TValue value)
    {
      ArgumentUtility.DebugCheckNotNull ("key", key);

      return _innerDictionary.TryGetValue (key, out value);
    }

    /// <summary>
    /// Gets the value of the element with the specified key, creating a new one if none exists.
    /// </summary>
    /// <param name="key">The key of the element to be retrieved.</param>
    /// <param name="valueFactory">A delegate used for creating a new element if none exists.</param>
    /// <returns>
    /// The value of the element that was found or created.
    /// </returns>
    public TValue GetOrCreateValue (TKey key, Func<TKey, TValue> valueFactory)
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

    /// <summary>
    /// Returns an <see cref="IEnumerator{T}"/> that iterates through the contents of this <see cref="SimpleDataStore{TKey,TValue}"/>.
    /// </summary>
    /// <returns>An <see cref="IEnumerator{T}"/> that iterates through the contents of this <see cref="SimpleDataStore{TKey,TValue}"/>.</returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator ()
    {
      return _innerDictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }
  }
}
