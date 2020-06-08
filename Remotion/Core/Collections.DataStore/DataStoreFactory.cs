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
using System.Collections.Generic;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Collections.DataStore
{
  /// <summary>
  /// The <see cref="DataStoreFactory"/> provides factory methods to create new data stores.
  /// </summary>
  public static class DataStoreFactory
  {
    /// <summary>
    /// Creates a <see cref="SimpleDataStore{TKey,TValue}"/> instance that is not thread-safe and uses the <see cref="EqualityComparer{T}.Default"/> 
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <returns>
    /// A <see cref="SimpleDataStore{TKey,TValue}"/> instances for storing keys and values.
    /// </returns>
    public static IDataStore<TKey, TValue> Create<TKey, TValue> ()
        where TKey : notnull
    {
      return new SimpleDataStore<TKey, TValue>();
    }

    /// <summary>
    /// Creates a <see cref="SimpleDataStore{TKey,TValue}"/> instance that is not thread-safe and uses the specifiede
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <param name="comparer">The comparer to use for comparing keys. Must not be <see langword="null" />.</param>
    /// <returns>
    /// A <see cref="SimpleDataStore{TKey,TValue}"/> instances for storing keys and values.
    /// </returns>
    public static IDataStore<TKey, TValue> Create<TKey, TValue> ([NotNull] IEqualityComparer<TKey> comparer)
        where TKey : notnull
    {
      ArgumentUtility.CheckNotNull ("comparer", comparer);

      return new SimpleDataStore<TKey, TValue> (comparer);
    }

    /// <summary>
    /// Creates a <see cref="ConcurrentDataStore{TKey,TValue}"/> instance that is thread-safe and uses the <see cref="EqualityComparer{T}.Default"/> 
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <returns>
    /// A <see cref="ConcurrentDataStore{TKey,TValue}"/> instance for storing keys and values in a thread-safe way.
    /// </returns>
    public static IDataStore<TKey, TValue> CreateWithSynchronization<TKey, TValue> ()
        where TKey : notnull
    {
      return new ConcurrentDataStore<TKey, TValue>();
    }

    /// <summary>
    /// Creates a <see cref="ConcurrentDataStore{TKey,TValue}"/> instance that is thread-safe and uses the specified
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <param name="comparer">The comparer to use for comparing keys. Must not be <see langword="null" />.</param>
    /// <returns>
    /// A <see cref="ConcurrentDataStore{TKey,TValue}"/> instance for storing keys and values in a thread-safe way.
    /// </returns>
    public static IDataStore<TKey, TValue> CreateWithSynchronization<TKey, TValue> ([NotNull] IEqualityComparer<TKey> comparer)
        where TKey : notnull
    {
      ArgumentUtility.CheckNotNull ("comparer", comparer);

      return new ConcurrentDataStore<TKey, TValue> (comparer);
    }

    /// <summary>
    /// Creates a <see cref="LockingDataStoreDecorator{TKey,TValue}"/> instance that is thread-safe and uses the <see cref="EqualityComparer{T}.Default"/> 
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <returns>
    /// A <see cref="LockingDataStoreDecorator{TKey,TValue}"/> instances for storing keys and values in a thread-safe way.
    /// </returns>
    /// <remarks>
    /// The <see cref="ConcurrentDataStore{TKey,TValue}"/> created with <see cref="CreateWithSynchronization{TKey,TValue}()"/> 
    /// provides better performance and contention behavior. Existing usages of <see cref="CreateWithLocking{TKey,TValue}()"/> 
    /// should therefor be replaced.
    /// </remarks>
    [Obsolete ("Use CreateWithSynchronization(...) instead. (Version: 1.19.3)")]
    public static IDataStore<TKey, TValue> CreateWithLocking<TKey, TValue> ()
        where TKey : notnull
    {
      return new LockingDataStoreDecorator<TKey, TValue> (new SimpleDataStore<TKey, TValue>());
    }

    /// <summary>
    /// Creates a <see cref="LockingDataStoreDecorator{TKey,TValue}"/> instance that is thread-safe and uses the specified
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <param name="comparer">The comparer to use for comparing keys.</param>
    /// <returns>
    /// A <see cref="LockingDataStoreDecorator{TKey,TValue}"/> instances for storing keys and values in a thread-safe way.
    /// </returns>
    /// <remarks>
    /// The <see cref="ConcurrentDataStore{TKey,TValue}"/> created with <see cref="CreateWithSynchronization{TKey,TValue}()"/> 
    /// provides better performance and contention behavior. Existing usages of <see cref="CreateWithLocking{TKey,TValue}()"/> 
    /// should therefor be replaced.
    /// </remarks>
    [Obsolete ("Use CreateWithSynchronization(...) instead. (Version: 1.19.3)")]
    public static IDataStore<TKey, TValue> CreateWithLocking<TKey, TValue> ([CanBeNull] IEqualityComparer<TKey>? comparer)
        where TKey : notnull
    {
      return new LockingDataStoreDecorator<TKey, TValue> (new SimpleDataStore<TKey, TValue> (comparer ?? EqualityComparer<TKey>.Default));
    }

    /// <summary>
    /// Creates a <see cref="LazyLockingDataStoreAdapter{TKey,TValue}"/> instance that is thread-safe and uses the <see cref="EqualityComparer{T}.Default"/> 
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <returns>
    /// A <see cref="LazyLockingDataStoreAdapter{TKey,TValue}"/> instances for storing keys and values in a thread-safe way.
    /// </returns>
    /// <remarks>
    /// The <see cref="ConcurrentDataStore{TKey,TValue}"/> created with <see cref="CreateWithSynchronization{TKey,TValue}()"/> 
    /// provides better performance and contention behavior. Existing usages of <see cref="CreateWithLocking{TKey,TValue}()"/> 
    /// should therefor be replaced.
    /// </remarks>
    [Obsolete ("Use CreateWithSynchronization(...) instead. (Version: 1.19.3)")]
    public static IDataStore<TKey, TValue> CreateWithLazyLocking<TKey, TValue> () 
        where TKey : notnull
        where TValue: class?
    {
      return new LazyLockingDataStoreAdapter<TKey, TValue> (
          new SimpleDataStore<TKey, Lazy<LazyLockingDataStoreAdapter<TKey, TValue>.Wrapper>>());
    }

    /// <summary>
    /// Creates a <see cref="LazyLockingDataStoreAdapter{TKey,TValue}"/> instance that is thread-safe and uses the specified
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <param name="comparer">The comparer to use for comparing keys.</param>
    /// <returns>
    /// A <see cref="LazyLockingDataStoreAdapter{TKey,TValue}"/> instances for storing keys and values in a thread-safe way.
    /// </returns>
    /// <remarks>
    /// The <see cref="ConcurrentDataStore{TKey,TValue}"/> created with <see cref="CreateWithSynchronization{TKey,TValue}()"/> 
    /// provides better performance and contention behavior. Existing usages of <see cref="CreateWithLocking{TKey,TValue}()"/> 
    /// should therefor be replaced.
    /// </remarks>
    [Obsolete ("Use CreateWithSynchronization(...) instead. (Version: 1.19.3)")]
    public static IDataStore<TKey, TValue> CreateWithLazyLocking<TKey, TValue> ([CanBeNull] IEqualityComparer<TKey>? comparer) 
        where TKey : notnull
        where TValue: class?
    {
      return new LazyLockingDataStoreAdapter<TKey, TValue> (
          new SimpleDataStore<TKey, Lazy<LazyLockingDataStoreAdapter<TKey, TValue>.Wrapper>> (comparer ?? EqualityComparer<TKey>.Default));
    }
  }
}