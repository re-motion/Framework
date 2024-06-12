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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Collections.Caching
{
  /// <summary>
  /// Provides a synchronization wrapper around an implementation of <see cref="ICache{TKey,TValue}"/>. Use 
  /// <see cref="CacheFactory.CreateWithLocking{TKey,TValue}()"/> to create an instance of this type.
  /// </summary>
  /// <typeparam name="TKey">The type of the keys.</typeparam>
  /// <typeparam name="TValue">The type of the values.</typeparam>
  /// <remarks>
  /// Instances of this object delegate every method call to an inner <see cref="ICache{TKey,TValue}"/> implementation,
  /// locking on a private synchronization object while the method is executed. This provides a convenient way to make an 
  /// <see cref="ICache{TKey,TValue}"/> thread-safe, as long as the cache is only accessed through this wrapper.
  /// </remarks>
  /// <threadsafety static="true" instance="true" />
  [Obsolete("Use ConcurrentCache<TKey, TValue> instead. (Version: 1.19.3)")]
  public sealed class LockingCacheDecorator<TKey, TValue> : ICache<TKey, TValue>
      where TKey: notnull
  {
    private readonly ICache<TKey, TValue> _innerCache;
    private readonly object _lock = new object();

    public LockingCacheDecorator (ICache<TKey, TValue> innerCache)
    {
      ArgumentUtility.CheckNotNull("innerCache", innerCache);

      _innerCache = innerCache;
    }

    public TValue GetOrCreateValue (TKey key, Func<TKey, TValue> valueFactory)
    {
      ArgumentUtility.DebugCheckNotNull("key", key);
      ArgumentUtility.DebugCheckNotNull("valueFactory", valueFactory);

      lock (_lock)
        return _innerCache.GetOrCreateValue(key, valueFactory);
    }

    public bool TryGetValue (TKey key, [AllowNull, MaybeNullWhen(false)] out TValue value)
    {
      ArgumentUtility.DebugCheckNotNull("key", key);

      lock (_lock)
        return _innerCache.TryGetValue(key, out value);
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
      // Sequence must be evaluated while inside the lock
      lock (_lock)
      {
        return ((IEnumerable<KeyValuePair<TKey, TValue>>)_innerCache.ToArray()).GetEnumerator();
      }
    }

    public void Clear ()
    {
      lock (_lock)
        _innerCache.Clear();
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
