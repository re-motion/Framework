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
using System.Threading;
using Remotion.Utilities;

namespace Remotion.Collections.DataStore
{
  /// <summary>
  /// Adapts an implementation of <see cref="IDataStore{TKey,TValue}"/> that stores <see cref="Lazy{T}"/> holding
  /// lazily constructed values so that users can access those values without indirection, and in a thread-safe way. Use 
  /// <see cref="DataStoreFactory.CreateWithLazyLocking{TKey,TValue}()"/> to create an instance of this type.
  /// </summary>
  /// <typeparam name="TKey">The type of the keys.</typeparam>
  /// <typeparam name="TValue">The type of the values.</typeparam>
  /// <threadsafety static="true" instance="true" />
  /// <remarks>
  /// This class internally combines a <see cref="LockingDataStoreDecorator{TKey,TValue}"/> with <see cref="Lazy{T}"/>
  /// instances. This leads to the effect that the lock used for the synchronization of the data store is always held for a very short time only,
  /// even if the factory delegate for a specific value takes a long time to execute.
  /// </remarks>
  [Obsolete("This type is only used in conjunction by obsolete factory method DataStoreFactory.CreateWithLazyLocking(...). (Version: 1.19.3)")]
  public class LazyLockingDataStoreAdapter<TKey, TValue> : IDataStore<TKey, TValue>
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

    private readonly LockingDataStoreDecorator<TKey, Lazy<Wrapper>> _innerDataStore;

    public LazyLockingDataStoreAdapter (IDataStore<TKey, Lazy<Wrapper>> innerDataStore)
    {
      ArgumentUtility.CheckNotNull("innerDataStore", innerDataStore);
      _innerDataStore = new LockingDataStoreDecorator<TKey, Lazy<Wrapper>>(innerDataStore);
    }

    public bool IsNull
    {
      get { return false; }
    }

    public bool ContainsKey (TKey key)
    {
      ArgumentUtility.DebugCheckNotNull("key", key);
      return _innerDataStore.ContainsKey(key);
    }

    public void Add (TKey key, TValue value)
    {
      ArgumentUtility.DebugCheckNotNull("key", key);
      _innerDataStore.Add(key, new Lazy<Wrapper>(() => new Wrapper(value), LazyThreadSafetyMode.PublicationOnly));
    }

    public bool Remove (TKey key)
    {
      ArgumentUtility.DebugCheckNotNull("key", key);
      return _innerDataStore.Remove(key);
    }

    public void Clear ()
    {
      _innerDataStore.Clear();
    }

    public TValue this [TKey key]
    {
      get
      {
        ArgumentUtility.DebugCheckNotNull("key", key);
        return _innerDataStore[key].Value.Value;
      }
      set
      {
        ArgumentUtility.DebugCheckNotNull("key", key);
        _innerDataStore[key] = new Lazy<Wrapper>(() => new Wrapper(value), LazyThreadSafetyMode.PublicationOnly);
      }
    }

    [return: MaybeNull]
    public TValue GetValueOrDefault (TKey key)
    {
      ArgumentUtility.DebugCheckNotNull("key", key);

      var result = _innerDataStore.GetValueOrDefault(key);
      return result != null ? result.Value.Value : null!;
    }

    public bool TryGetValue (TKey key, [AllowNull, MaybeNullWhen(false)] out TValue value)
    {
      ArgumentUtility.DebugCheckNotNull("key", key);


      if (_innerDataStore.TryGetValue(key, out var result))
      {
        value = result.Value.Value;
        return true;
      }
      value = null;
      return false;
    }

    public TValue GetOrCreateValue (TKey key, Func<TKey, TValue> valueFactory)
    {
      ArgumentUtility.DebugCheckNotNull("key", key);
      ArgumentUtility.DebugCheckNotNull("valueFactory", valueFactory);

      Wrapper wrapper;
      if (_innerDataStore.TryGetValue(key, out var value))
        wrapper = value.Value;
      else
        wrapper = GetOrCreateValueWithClosure(key, valueFactory); // Split to prevent closure being created during the TryGetValue-operation

      return wrapper.Value;
    }

    private Wrapper GetOrCreateValueWithClosure (TKey key, Func<TKey, TValue> valueFactory)
    {
      ArgumentUtility.CheckNotNull("valueFactory", valueFactory);
      var result = _innerDataStore.GetOrCreateValue(
          key,
          k => new Lazy<Wrapper>(() => new Wrapper(valueFactory(k)), LazyThreadSafetyMode.ExecutionAndPublication));
      return result.Value;
    }
  }
}
