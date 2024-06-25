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
using System.Diagnostics.CodeAnalysis;
using Remotion.Utilities;

namespace Remotion.Collections.DataStore
{
  /// <summary>
  /// Provides a synchronization wrapper around an implementation of <see cref="IDataStore{TKey,TValue}"/>. Use 
  /// <see cref="DataStoreFactory.CreateWithLocking{TKey,TValue}()"/> to create an instance of this type.
  /// </summary>
  /// <typeparam name="TKey">The type of the keys.</typeparam>
  /// <typeparam name="TValue">The type of the values.</typeparam>
  /// <remarks>
  /// Instances of this object delegate every method call to an inner <see cref="IDataStore{TKey,TValue}"/> implementation,
  /// locking on a private synchronization object while the method is executed. This provides a convenient way to make an 
  /// <see cref="IDataStore{TKey,TValue}"/> thread-safe, as long as the store is only accessed through this wrapper.
  /// </remarks>
  /// <threadsafety static="true" instance="true" />
  [Obsolete("Use ConcurrentDataStore<TKey, TValue> instead. (Version: 1.19.3)")]
  public class LockingDataStoreDecorator<TKey, TValue> : IDataStore<TKey, TValue>
      where TKey : notnull
  {
    private readonly IDataStore<TKey, TValue> _innerStore;
    private readonly object _lock = new object();

    /// <summary>
    /// Initializes a new instance of the <see cref="LockingDataStoreDecorator{TKey,TValue}"/> class.
    /// </summary>
    /// <param name="innerStore">The inner store which is wrapped.</param>
    public LockingDataStoreDecorator (IDataStore<TKey, TValue> innerStore)
    {
      ArgumentUtility.CheckNotNull("innerStore", innerStore);
      _innerStore = innerStore;
    }

    bool INullObject.IsNull
    {
      get { return false; }
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
      ArgumentUtility.DebugCheckNotNull("key", key);

      lock (_lock)
      {
        return _innerStore.ContainsKey(key);
      }
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
      ArgumentUtility.DebugCheckNotNull("key", key);
      // value can be null

      lock (_lock)
      {
        _innerStore.Add(key, value);
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
      ArgumentUtility.DebugCheckNotNull("key", key);

      lock (_lock)
      {
        return _innerStore.Remove(key);
      }
    }

    /// <summary>
    /// Removes all elements from the store.
    /// </summary>
    public void Clear ()
    {
      lock (_lock)
      {
        _innerStore.Clear();
      }
    }

    /// <summary>
    /// Gets or sets the <typeparamref name="TValue"/> with the specified key.
    /// </summary>
    /// <value></value>
    public TValue this[TKey key]
    {
      get
      {
        ArgumentUtility.DebugCheckNotNull("key", key);

        lock (_lock)
        {
          return _innerStore[key];
        }
      }
      set
      {
        ArgumentUtility.DebugCheckNotNull("key", key);

        lock (_lock)
        {
          _innerStore[key] = value;
        }
      }
    }

    /// <summary>
    /// Gets the value of the element with the specified key, or <typeparamref name="TValue"/>'s default value if no such element exists.
    /// </summary>
    /// <param name="key">The key to look up.</param>
    /// <returns>
    /// The value of the element, or the default value if no such element exists.
    /// </returns>
    [return: MaybeNull]
    public TValue GetValueOrDefault (TKey key)
    {
      ArgumentUtility.DebugCheckNotNull("key", key);

      lock (_lock)
      {
        return _innerStore.GetValueOrDefault(key);
      }
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
    public bool TryGetValue (TKey key, [AllowNull, MaybeNullWhen(false)] out TValue value)
    {
      ArgumentUtility.DebugCheckNotNull("key", key);

      lock (_lock)
      {
        return _innerStore.TryGetValue(key, out value);
      }
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
      ArgumentUtility.DebugCheckNotNull("key", key);
      ArgumentUtility.DebugCheckNotNull("valueFactory", valueFactory);

      lock (_lock)
      {
        return _innerStore.GetOrCreateValue(key, valueFactory);
      }
    }
  }
}
