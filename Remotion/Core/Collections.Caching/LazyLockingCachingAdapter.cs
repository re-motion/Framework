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
using System.Threading;
using Remotion.Utilities;

namespace Remotion.Collections.Caching
{
  /// <summary>
  /// Adapts an implementation of <see cref="ICache{TKey,TValue}"/> that stores <see cref="Lazy{T}"/> holding
  /// lazily constructed values so that users can access those values without indirection, and in a thread-safe way. Use 
  /// <see cref="CacheFactory.CreateWithLazyLocking{TKey,TValue}()"/> to create an instance of this type.
  /// </summary>
  /// <typeparam name="TKey">The type of the keys.</typeparam>
  /// <typeparam name="TValue">The type of the values.</typeparam>
  /// <threadsafety static="true" instance="true" />
  /// <remarks>
  /// This class internally combines a <see cref="LockingCacheDecorator{TKey,TValue}"/> with <see cref="Lazy{T}"/>
  /// instances. This leads to the effect that the lock used for the synchronization of the data store is always held for a very short time only,
  /// even if the factory delegate for a specific value takes a long time to execute.
  /// </remarks>
  [Obsolete("This type is only used in conjunction by obsolete factory method CacheFactory.CreateWithLazyLocking(...). (Version: 1.19.3)")]
  public class LazyLockingCachingAdapter<TKey, TValue> : ICache<TKey, TValue>
      where TKey : notnull
      where TValue : class?
  {
    public class Wrapper
    {
      public readonly TValue Value;

      public Wrapper (TValue value)
      {
        Value = value;
      }
    }

    private readonly LockingCacheDecorator<TKey, Lazy<Wrapper>> _innerCache;

    public LazyLockingCachingAdapter (ICache<TKey, Lazy<Wrapper>> innerCache)
    {
      ArgumentUtility.CheckNotNull("innerCache", innerCache);

      _innerCache = new LockingCacheDecorator<TKey, Lazy<Wrapper>>(innerCache);
    }

    public bool IsNull
    {
      get { return false; }
    }

    public TValue GetOrCreateValue (TKey key, Func<TKey, TValue> valueFactory)
    {
      ArgumentUtility.DebugCheckNotNull("key", key);
      ArgumentUtility.DebugCheckNotNull("valueFactory", valueFactory);

      Wrapper wrapper;
      if (_innerCache.TryGetValue(key, out var value))
        wrapper = value.Value;
      else
        wrapper = GetOrCreateValueWithClosure(key, valueFactory); // Split to prevent closure being created during the TryGetValue-operation

      return wrapper.Value;
    }

    private Wrapper GetOrCreateValueWithClosure (TKey key, Func<TKey, TValue> valueFactory)
    {
      ArgumentUtility.CheckNotNull("valueFactory", valueFactory);
      var result = _innerCache.GetOrCreateValue(
          key,
          k => new Lazy<Wrapper>(() => new Wrapper(valueFactory(k)), LazyThreadSafetyMode.ExecutionAndPublication));
      return result.Value;
    }

    public bool TryGetValue (TKey key, [AllowNull, MaybeNullWhen(false)] out TValue value)
    {
      ArgumentUtility.DebugCheckNotNull("key", key);

      if (_innerCache.TryGetValue(key, out var result))
      {
        value = result.Value.Value;
        return true;
      }
      value = null!;
      return false;
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
      return _innerCache.Select(item => new KeyValuePair<TKey, TValue>(item.Key, item.Value.Value.Value)).GetEnumerator();
    }

    public void Clear ()
    {
      _innerCache.Clear();
    }
  }
}
