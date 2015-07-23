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

//

using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Collections
{
  /// <summary>
  /// The <see cref="CacheFactory"/> provides factory methods to create new caches.
  /// </summary>
  public static class CacheFactory
  {
    /// <summary>
    /// Creates a <see cref="Cache{TKey,TValue}"/> instance that is not thread-safe and uses the <see cref="EqualityComparer{T}.Default"/> 
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <returns>
    /// A <see cref="Cache{TKey,TValue}"/> instance for storing keys and values.
    /// </returns>
    public static ICache<TKey, TValue> Create<TKey, TValue> ()
    {
      return new Cache<TKey, TValue>();
    }

    /// <summary>
    /// Creates a <see cref="Cache{TKey,TValue}"/> instance that is not thread-safe and uses the <see cref="EqualityComparer{T}.Default"/> 
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <param name="invalidationToken">The <see cref="InvalidationToken"/> that can be used to signal a cache invalidation. Must not be <see langword="null" />.</param>
    /// <returns>
    /// A <see cref="Cache{TKey,TValue}"/> instance for storing keys and values.
    /// </returns>
    public static ICache<TKey, TValue> Create<TKey, TValue> ([NotNull] InvalidationToken invalidationToken)
    {
      ArgumentUtility.CheckNotNull ("invalidationToken", invalidationToken);

      return new InvalidationTokenBasedCacheDecorator<TKey, TValue> (new Cache<TKey, TValue>(), invalidationToken);
    }

    /// <summary>
    /// Creates a <see cref="Cache{TKey,TValue}"/> instance that is not thread-safe and uses the specified
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <param name="comparer">The comparer to use for comparing keys.</param>
    /// <returns>
    /// A <see cref="Cache{TKey,TValue}"/> instances for storing keys and values.
    /// </returns>
    public static ICache<TKey, TValue> Create<TKey, TValue> (IEqualityComparer<TKey> comparer)
    {
      return new Cache<TKey, TValue> (comparer);
    }

    /// <summary>
    /// Creates a <see cref="Cache{TKey,TValue}"/> instance that is not thread-safe and uses the specified
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <param name="invalidationToken">The <see cref="InvalidationToken"/> that can be used to signal a cache invalidation. Must not be <see langword="null" />.</param>
    /// <param name="comparer">The comparer to use for comparing keys. Can be <see langword="null" />.</param>
    /// <returns>
    /// A <see cref="Cache{TKey,TValue}"/> instances for storing keys and values.
    /// </returns>
    public static ICache<TKey, TValue> Create<TKey, TValue> (
        [NotNull] InvalidationToken invalidationToken,
        [CanBeNull] IEqualityComparer<TKey> comparer)
    {
      ArgumentUtility.CheckNotNull ("invalidationToken", invalidationToken);

      return new InvalidationTokenBasedCacheDecorator<TKey, TValue> (new Cache<TKey, TValue> (comparer), invalidationToken);
    }

    /// <summary>
    /// Creates a <see cref="LockingCacheDecorator{TKey,TValue}"/> instance that is thread-safe and uses the <see cref="EqualityComparer{T}.Default"/> 
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <returns>
    /// A <see cref="LockingCacheDecorator{TKey,TValue}"/> instances for storing keys and values in a thread-safe way.
    /// </returns>
    /// <remarks>
    /// The created instance uses a single lock (see <see cref="Monitor"/>) to guard the data store against multi-threaded access. It is well-suited
    /// for caches in which the factory delegates passed to <see cref="ICache{TKey,TValue}.GetOrCreateValue"/> only take a short time to 
    /// complete. When the factory delegates take a long time to execute, consider using <see cref="CreateWithLazyLocking{TKey,TValue}()"/> instead 
    /// to reduce contention.
    /// </remarks>
    public static LockingCacheDecorator<TKey, TValue> CreateWithLocking<TKey, TValue> ()
    {
      return new LockingCacheDecorator<TKey, TValue> (new Cache<TKey, TValue>());
    }

    /// <summary>
    /// Creates a <see cref="LockingCacheDecorator{TKey,TValue}"/> instance that is thread-safe and uses the <see cref="EqualityComparer{T}.Default"/> 
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <param name="invalidationToken">The <see cref="LockingInvalidationToken"/> that can be used to signal a cache invalidation. Must not be <see langword="null" />.</param>
    /// <returns>
    /// A <see cref="LockingCacheDecorator{TKey,TValue}"/> instances for storing keys and values in a thread-safe way.
    /// </returns>
    /// <remarks>
    /// The created instance uses a single lock (see <see cref="Monitor"/>) to guard the data store against multi-threaded access. It is well-suited
    /// for caches in which the factory delegates passed to <see cref="ICache{TKey,TValue}.GetOrCreateValue"/> only take a short time to 
    /// complete. When the factory delegates take a long time to execute, consider using <see cref="CreateWithLazyLocking{TKey,TValue}()"/> instead 
    /// to reduce contention.
    /// </remarks>
    public static ICache<TKey, TValue> CreateWithLocking<TKey, TValue> (
        [NotNull] LockingInvalidationToken invalidationToken)
    {
      ArgumentUtility.CheckNotNull ("invalidationToken", invalidationToken);

      return new LockingCacheDecorator<TKey, TValue> (
          new InvalidationTokenBasedCacheDecorator<TKey, TValue> (new Cache<TKey, TValue>(), invalidationToken));
    }

    /// <summary>
    /// Creates a <see cref="LockingCacheDecorator{TKey,TValue}"/> instance that is thread-safe and uses the specified
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <param name="comparer">The comparer to use for comparing keys.</param>
    /// <returns>
    /// A <see cref="LockingCacheDecorator{TKey,TValue}"/> instances for storing keys and values in a thread-safe way.
    /// </returns>
    /// <remarks>
    /// The created instance uses a single lock (see <see cref="Monitor"/>) to guard the data store against multi-threaded access. It is well-suited
    /// for caches in which the factory delegates passed to <see cref="ICache{TKey,TValue}.GetOrCreateValue"/> only take a short time to 
    /// complete. When the factory delegates take a long time to execute, consider using 
    /// <see cref="CreateWithLazyLocking{TKey,TValue}(System.Collections.Generic.IEqualityComparer{TKey})"/> instead to reduce contention.
    /// </remarks>
    public static ICache<TKey, TValue> CreateWithLocking<TKey, TValue> ([CanBeNull] IEqualityComparer<TKey> comparer)
    {
      return new LockingCacheDecorator<TKey, TValue> (new Cache<TKey, TValue> (comparer));
    }

    /// <summary>
    /// Creates a <see cref="LockingCacheDecorator{TKey,TValue}"/> instance that is thread-safe and uses the specified
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <param name="invalidationToken">The <see cref="LockingInvalidationToken"/> that can be used to signal a cache invalidation. Must not be <see langword="null" />.</param>
    /// <param name="comparer">The comparer to use for comparing keys.</param>
    /// <returns>
    /// A <see cref="LockingCacheDecorator{TKey,TValue}"/> instances for storing keys and values in a thread-safe way.
    /// </returns>
    /// <remarks>
    /// The created instance uses a single lock (see <see cref="Monitor"/>) to guard the data store against multi-threaded access. It is well-suited
    /// for caches in which the factory delegates passed to <see cref="ICache{TKey,TValue}.GetOrCreateValue"/> only take a short time to 
    /// complete. When the factory delegates take a long time to execute, consider using 
    /// <see cref="CreateWithLazyLocking{TKey,TValue}(System.Collections.Generic.IEqualityComparer{TKey})"/> instead to reduce contention.
    /// </remarks>
    public static ICache<TKey, TValue> CreateWithLocking<TKey, TValue> (
        [NotNull] LockingInvalidationToken invalidationToken,
        [CanBeNull] IEqualityComparer<TKey> comparer)
    {
      ArgumentUtility.CheckNotNull ("invalidationToken", invalidationToken);

      return new LockingCacheDecorator<TKey, TValue> (
          new InvalidationTokenBasedCacheDecorator<TKey, TValue> (new Cache<TKey, TValue> (comparer), invalidationToken));
    }

    /// <summary>
    /// Creates a <see cref="LazyLockingCachingAdapter{TKey,TValue}"/> instance that is thread-safe and uses the <see cref="EqualityComparer{T}.Default"/> 
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <returns>
    /// A <see cref="LazyLockingCachingAdapter{TKey,TValue}"/> instances for storing keys and values in a thread-safe way.
    /// </returns>
    /// <remarks>
    /// The created instance uses a single lock (see <see cref="Monitor"/>) to guard the data store against multi-threaded access and additional, 
    /// double-checked locks (see <see cref="DoubleCheckedLockingContainer{T}"/>) to protect each single value. It is well-suited for caches
    /// in which the factory delegates passed to <see cref="ICache{TKey,TValue}.GetOrCreateValue"/> take a long time to execute. When the factory
    /// delegates do not take a long time, consider using <see cref="CreateWithLocking{TKey,TValue}()"/> instead to reduce the number of locks used.
    /// </remarks>
    public static ICache<TKey, TValue> CreateWithLazyLocking<TKey, TValue> () where TValue : class
    {
      return new LazyLockingCachingAdapter<TKey, TValue> (
          new Cache<TKey, DoubleCheckedLockingContainer<LazyLockingCachingAdapter<TKey, TValue>.Wrapper>>());
    }

    /// <summary>
    /// Creates a <see cref="LazyLockingCachingAdapter{TKey,TValue}"/> instance that is thread-safe and uses the <see cref="EqualityComparer{T}.Default"/> 
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <param name="invalidationToken">The <see cref="LockingInvalidationToken"/> that can be used to signal a cache invalidation. Must not be <see langword="null" />.</param>
    /// <returns>
    /// A <see cref="LazyLockingCachingAdapter{TKey,TValue}"/> instances for storing keys and values in a thread-safe way.
    /// </returns>
    /// <remarks>
    /// The created instance uses a single lock (see <see cref="Monitor"/>) to guard the data store against multi-threaded access and additional, 
    /// double-checked locks (see <see cref="DoubleCheckedLockingContainer{T}"/>) to protect each single value. It is well-suited for caches
    /// in which the factory delegates passed to <see cref="ICache{TKey,TValue}.GetOrCreateValue"/> take a long time to execute. When the factory
    /// delegates do not take a long time, consider using <see cref="CreateWithLocking{TKey,TValue}()"/> instead to reduce the number of locks used.
    /// </remarks>
    public static ICache<TKey, TValue> CreateWithLazyLocking<TKey, TValue> (
        [NotNull] LockingInvalidationToken invalidationToken)
        where TValue : class
    {
      ArgumentUtility.CheckNotNull ("invalidationToken", invalidationToken);

      return new LazyLockingCachingAdapter<TKey, TValue> (
          new InvalidationTokenBasedCacheDecorator<TKey, DoubleCheckedLockingContainer<LazyLockingCachingAdapter<TKey, TValue>.Wrapper>> (
              new Cache<TKey, DoubleCheckedLockingContainer<LazyLockingCachingAdapter<TKey, TValue>.Wrapper>>(),
              invalidationToken));
    }

    /// <summary>
    /// Creates a <see cref="LazyLockingCachingAdapter{TKey,TValue}"/> instance that is thread-safe and uses the specified
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <param name="comparer">The comparer to use for comparing keys.</param>
    /// <returns>
    /// A <see cref="LazyLockingCachingAdapter{TKey,TValue}"/> instances for storing keys and values in a thread-safe way.
    /// </returns>
    /// <remarks>
    /// The created instance uses a single lock (see <see cref="Monitor"/>) to guard the data store against multi-threaded access and additional,
    /// double-checked locks (see <see cref="DoubleCheckedLockingContainer{T}"/>) to protect each single value. It is well-suited for caches
    /// in which the factory delegates passed to <see cref="ICache{TKey,TValue}.GetOrCreateValue"/> take a long time to execute. When the factory
    /// delegates do not take a long time, consider using 
    /// <see cref="CreateWithLocking{TKey,TValue}(System.Collections.Generic.IEqualityComparer{TKey})"/> instead to reduce the number of locks used.
    /// </remarks>
    public static ICache<TKey, TValue> CreateWithLazyLocking<TKey, TValue> ([CanBeNull] IEqualityComparer<TKey> comparer)
        where TValue : class
    {
      return new LazyLockingCachingAdapter<TKey, TValue> (
          new Cache<TKey, DoubleCheckedLockingContainer<LazyLockingCachingAdapter<TKey, TValue>.Wrapper>> (comparer));
    }

    /// <summary>
    /// Creates a <see cref="LazyLockingCachingAdapter{TKey,TValue}"/> instance that is thread-safe and uses the specified
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <param name="invalidationToken">The <see cref="LockingInvalidationToken"/> that can be used to signal a cache invalidation. Must not be <see langword="null" />.</param>
    /// <param name="comparer">The comparer to use for comparing keys.</param>
    /// <returns>
    /// A <see cref="LazyLockingCachingAdapter{TKey,TValue}"/> instances for storing keys and values in a thread-safe way.
    /// </returns>
    /// <remarks>
    /// The created instance uses a single lock (see <see cref="Monitor"/>) to guard the data store against multi-threaded access and additional,
    /// double-checked locks (see <see cref="DoubleCheckedLockingContainer{T}"/>) to protect each single value. It is well-suited for caches
    /// in which the factory delegates passed to <see cref="ICache{TKey,TValue}.GetOrCreateValue"/> take a long time to execute. When the factory
    /// delegates do not take a long time, consider using 
    /// <see cref="CreateWithLocking{TKey,TValue}(System.Collections.Generic.IEqualityComparer{TKey})"/> instead to reduce the number of locks used.
    /// </remarks>
    public static ICache<TKey, TValue> CreateWithLazyLocking<TKey, TValue> (
        [NotNull] LockingInvalidationToken invalidationToken,
        [CanBeNull] IEqualityComparer<TKey> comparer)
        where TValue : class
    {
      ArgumentUtility.CheckNotNull ("invalidationToken", invalidationToken);

      return new LazyLockingCachingAdapter<TKey, TValue> (
          new InvalidationTokenBasedCacheDecorator<TKey, DoubleCheckedLockingContainer<LazyLockingCachingAdapter<TKey, TValue>.Wrapper>> (
              new Cache<TKey, DoubleCheckedLockingContainer<LazyLockingCachingAdapter<TKey, TValue>.Wrapper>> (comparer),
              invalidationToken));
    }
  }
}