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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Collections.DataStore
{
  /// <summary>
  /// Implements the <see cref="IDataStore{TKey,TValue}"/> interface as a thread-safe in-memory data store based on a 
  /// <see cref="ConcurrentDictionary{TKey,TValue}"/>.
  /// </summary>
  /// <typeparam name="TKey">The type of the keys.</typeparam>
  /// <typeparam name="TValue">The type of the values.</typeparam>
  /// <remarks>
  /// It is recommended to use the <see cref="DataStoreFactory"/> type to create instances of the desired data store implementation 
  /// (e.g. thread-safe implementations, implementations with support for data invalidation, etc).
  /// </remarks>
  /// <threadsafety static="true" instance="true" />
  public sealed class ConcurrentDataStore<TKey, TValue> : IDataStore<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>
  {
    private sealed class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
    {
      private readonly IEnumerator<KeyValuePair<TKey, SynchronizedValue>> _inner;

      public Enumerator (IEnumerator<KeyValuePair<TKey, SynchronizedValue>> inner)
      {
        _inner = inner;
      }

      public bool MoveNext ()
      {
        while (true)
        {
          if (!_inner.MoveNext())
            return false;

          var synchronizedValue = _inner.Current.Value;

          if (synchronizedValue.Boxed == null)
          {
            // Double-checked lock
            lock (synchronizedValue)
            {
              // Skip values that are just getting initialized on the current thread.
              if (synchronizedValue.Boxed == null)
                continue;
            }
          }

          // Skip values where the initialization has failed. They can only be observed until the cleanup has completed.
          if (ReferenceEquals (synchronizedValue.Boxed, Boxed.ExceptionSentinel))
            continue;

          return true;
        }
      }

      public void Reset ()
      {
        _inner.Reset();
      }

      public KeyValuePair<TKey, TValue> Current
      {
        get
        {
          var current = _inner.Current;

          return new KeyValuePair<TKey, TValue> (current.Key, current.Value.Boxed.Value);
        }
      }

      object IEnumerator.Current
      {
        get { return Current; }
      }

      public void Dispose ()
      {
        _inner.Dispose();
      }
    }

    /// <summary>
    /// Provides an object that can be used to synchronize the value access based on the one-time initialized <see cref="SynchronizedValue.Boxed"/> field.
    /// </summary>
    private class SynchronizedValue
    {
      public Boxed Boxed;

      public SynchronizedValue ()
      {
      }
    }

    private class Boxed
    {
      public static readonly Boxed ExceptionSentinel = new Boxed (default);

      public readonly TValue Value;

      public Boxed (TValue value)
      {
        Value = value;
      }
    }

    private readonly ConcurrentDictionary<TKey, SynchronizedValue> _innerDictionary;

    public ConcurrentDataStore ()
    {
      _innerDictionary = new ConcurrentDictionary<TKey, SynchronizedValue>();
    }

    public ConcurrentDataStore ([NotNull] IEqualityComparer<TKey> comparer)
    {
      _innerDictionary = new ConcurrentDictionary<TKey, SynchronizedValue> (comparer);
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
      return TryGetValueInternal (key, out _);
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

      if (!_innerDictionary.TryAdd (key, new SynchronizedValue { Boxed = new Boxed (value) }))
      {
        string message =
            string.Format ("The store already contains an element with key '{0}'. (Old value: '{1}', new value: '{2}')", key, this[key], value);
        throw new ArgumentException (message, "key");
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
      if (TryGetValueInternal (key, out _))
        return _innerDictionary.TryRemove (key, out _);

      return false;
    }

    /// <summary>
    /// Gets or sets the <typeparamref name="TValue"/> with the specified key.
    /// </summary>
    public TValue this [TKey key]
    {
      get
      {
        ArgumentUtility.CheckNotNull ("key", key);
        if (TryGetValueInternal (key, out var value))
          return value;

        string message = string.Format ("There is no element with key '{0}' in the store.", key);
        throw new KeyNotFoundException (message);
      }
      set
      {
        ArgumentUtility.CheckNotNull ("key", key);
        _innerDictionary[key] = new SynchronizedValue { Boxed = new Boxed (value) };
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

      TryGetValueInternal (key, out var value);
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

      return TryGetValueInternal (key, out value);
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

      // Implementation of ConcurrentDictionary.GetOrAdd(valueFactory) is already set up with TryGetValue() + GetOrAdd(value) if key-not-found.
      // By splitting the implementation to perform the calls to TryGetValue() and GetOrAdd(value) separately, 
      // we perform the same operations (including the synchronization-behavior) but can defer creating the SynchronizedValue to the key-not-found branch.

      while (true)
      {
        if (TryGetValueInternal (key, out var resultValue))
          return resultValue;

        var synchronizedValue = new SynchronizedValue();
        // By establishing the lock before inserting the SynchronizedValue into the dictionary, we ensure that only the current thread will be allowed 
        // to initialize the value. All other threads that also happen to receive this SynchronizedValue before it is initialized, will have to 
        // synchronize their value access.
        lock (synchronizedValue)
        {
          SynchronizedValue cachedSynchronizedValue = _innerDictionary.GetOrAdd (key, synchronizedValue);
          if (ReferenceEquals (synchronizedValue, cachedSynchronizedValue))
          {
            try
            {
              var value = valueFactory (key);
              synchronizedValue.Boxed = new Boxed (value);
              return value;
            }
            catch
            {
              synchronizedValue.Boxed = Boxed.ExceptionSentinel;

              // ICollection.Remove() performs the remove only if both the Key and the Value are a match, the lookup has O(1) complexity.
              // The specific implementation for ConcurrentDictionary does not have a branch where an exception would be expected, 
              // therefor no separate exception handling for ICollection.Remove() has been added.
              ((ICollection<KeyValuePair<TKey, SynchronizedValue>>) _innerDictionary)
                  .Remove (new KeyValuePair<TKey, SynchronizedValue> (key, synchronizedValue));

              throw;
            }
          }
        }
      }
    }

    /// <summary>
    /// Returns an <see cref="IEnumerator{T}"/> that iterates through the contents of this <see cref="ConcurrentDataStore{TKey,TValue}"/>.
    /// </summary>
    /// <returns>An <see cref="IEnumerator{T}"/> that iterates through the contents of this <see cref="ConcurrentDataStore{TKey,TValue}"/>.</returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator ()
    {
      return new Enumerator (_innerDictionary.GetEnumerator());
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    /// <summary>
    /// Removes all elements from the store.
    /// </summary>
    public void Clear ()
    {
      _innerDictionary.Clear();
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }

    [MethodImpl (MethodImplOptions.AggressiveInlining)]
    private bool TryGetValueInternal (TKey key, out TValue value)
    {
      if (_innerDictionary.TryGetValue (key, out var synchronizedValue))
      {
        if (synchronizedValue.Boxed == null)
        {
          // Double-checked lock
          lock (synchronizedValue)
          {
            if (synchronizedValue.Boxed == null)
            {
              throw new InvalidOperationException (
                  string.Format (
                      "An attempt was detected to access the value for key ('{0}') during the factory operation of GetOrCreateValue(key, factory).",
                      key));
            }
          }
        }

        var boxed = synchronizedValue.Boxed;
        value = boxed.Value;
        return !ReferenceEquals (boxed, Boxed.ExceptionSentinel);
      }
      else
      {
        value = default;
        return false;
      }
    }
  }
}