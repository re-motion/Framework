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

namespace Remotion.Collections.Caching
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
        where TKey : notnull
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
        where TKey : notnull
    {
      ArgumentUtility.CheckNotNull("invalidationToken", invalidationToken);

      return new InvalidationTokenBasedCacheDecorator<TKey, TValue>(new Cache<TKey, TValue>(), invalidationToken);
    }

    /// <summary>
    /// Creates a <see cref="Cache{TKey,TValue}"/> instance that is not thread-safe and uses the specified
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <param name="comparer">The comparer to use for comparing keys. Must not be <see langword="null" />.</param>
    /// <returns>
    /// A <see cref="Cache{TKey,TValue}"/> instances for storing keys and values.
    /// </returns>
    public static ICache<TKey, TValue> Create<TKey, TValue> ([NotNull] IEqualityComparer<TKey> comparer)
        where TKey : notnull
    {
      ArgumentUtility.CheckNotNull("comparer", comparer);

      return new Cache<TKey, TValue>(comparer);
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
        [NotNull] IEqualityComparer<TKey> comparer)
        where TKey : notnull
    {
      ArgumentUtility.CheckNotNull("invalidationToken", invalidationToken);
      ArgumentUtility.CheckNotNull("comparer", comparer);

      return new InvalidationTokenBasedCacheDecorator<TKey, TValue>(new Cache<TKey, TValue>(comparer), invalidationToken);
    }

    /// <summary>
    /// Creates a <see cref="ConcurrentCache{TKey,TValue}"/> instance that is thread-safe and uses the <see cref="EqualityComparer{T}.Default"/> 
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <returns>
    /// A <see cref="ConcurrentCache{TKey,TValue}"/> instances for storing keys and values in a thread-safe way.
    /// </returns>
    public static ICache<TKey, TValue> CreateWithSynchronization<TKey, TValue> ()
        where TKey : notnull
    {
      return new ConcurrentCache<TKey, TValue>();
    }

    /// <summary>
    /// Creates a <see cref="ConcurrentCache{TKey,TValue}"/> instance that is thread-safe and uses the <see cref="EqualityComparer{T}.Default"/> 
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <param name="invalidationToken">The <see cref="LockingInvalidationToken"/> that can be used to signal a cache invalidation. Must not be <see langword="null" />.</param>
    /// <returns>
    /// A <see cref="ConcurrentCache{TKey,TValue}"/> instances for storing keys and values in a thread-safe way.
    /// </returns>
    public static ICache<TKey, TValue> CreateWithSynchronization<TKey, TValue> (
        [NotNull] LockingInvalidationToken invalidationToken)
        where TKey : notnull
    {
      ArgumentUtility.CheckNotNull("invalidationToken", invalidationToken);

      return new InvalidationTokenBasedCacheDecorator<TKey, TValue>(new ConcurrentCache<TKey, TValue>(), invalidationToken);
    }

    /// <summary>
    /// Creates a <see cref="ConcurrentCache{TKey,TValue}"/> instance that is thread-safe and uses the specified
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <param name="comparer">The comparer to use for comparing keys. Must not be <see langword="null" />.</param>
    /// <returns>
    /// A <see cref="ConcurrentCache{TKey,TValue}"/> instances for storing keys and values in a thread-safe way.
    /// </returns>
    public static ICache<TKey, TValue> CreateWithSynchronization<TKey, TValue> ([NotNull] IEqualityComparer<TKey> comparer)
        where TKey : notnull
    {
      ArgumentUtility.CheckNotNull("comparer", comparer);

      return new ConcurrentCache<TKey, TValue>(comparer);
    }

    /// <summary>
    /// Creates a <see cref="ConcurrentCache{TKey,TValue}"/> instance that is thread-safe and uses the specified
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <param name="invalidationToken">The <see cref="LockingInvalidationToken"/> that can be used to signal a cache invalidation. Must not be <see langword="null" />.</param>
    /// <param name="comparer">The comparer to use for comparing keys. Must not be <see langword="null" />.</param>
    /// <returns>
    /// A <see cref="ConcurrentCache{TKey,TValue}"/> instances for storing keys and values in a thread-safe way.
    /// </returns>
    public static ICache<TKey, TValue> CreateWithSynchronization<TKey, TValue> (
        [NotNull] LockingInvalidationToken invalidationToken,
        [NotNull] IEqualityComparer<TKey> comparer)
        where TKey: notnull
    {
      ArgumentUtility.CheckNotNull("invalidationToken", invalidationToken);
      ArgumentUtility.CheckNotNull("comparer", comparer);

      return new InvalidationTokenBasedCacheDecorator<TKey, TValue>(new ConcurrentCache<TKey, TValue>(comparer), invalidationToken);
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
    /// The <see cref="ConcurrentCache{TKey,TValue}"/> created with <see cref="CreateWithSynchronization{TKey,TValue}()"/> 
    /// provides better performance and contention behavior. Existing usages of <see cref="CreateWithLocking{TKey,TValue}()"/> 
    /// should therefor be replaced.
    /// </remarks>
    [Obsolete("Use CreateWithSynchronization(...) instead. (Version: 1.19.3)")]
    public static LockingCacheDecorator<TKey, TValue> CreateWithLocking<TKey, TValue> ()
        where TKey : notnull
    {
      return new LockingCacheDecorator<TKey, TValue>(new Cache<TKey, TValue>());
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
    /// The <see cref="ConcurrentCache{TKey,TValue}"/> created with <see cref="CreateWithSynchronization{TKey,TValue}(LockingInvalidationToken)"/> 
    /// provides better performance and contention behavior. Existing usages of <see cref="CreateWithLocking{TKey,TValue}(LockingInvalidationToken)"/> 
    /// should therefor be replaced.
    /// </remarks>
    [Obsolete("Use CreateWithSynchronization(...) instead. (Version: 1.19.3)")]
    public static ICache<TKey, TValue> CreateWithLocking<TKey, TValue> (
        [NotNull] LockingInvalidationToken invalidationToken)
        where TKey : notnull
    {
      ArgumentUtility.CheckNotNull("invalidationToken", invalidationToken);

      return new LockingCacheDecorator<TKey, TValue>(
          new InvalidationTokenBasedCacheDecorator<TKey, TValue>(new Cache<TKey, TValue>(), invalidationToken));
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
    /// The <see cref="ConcurrentCache{TKey,TValue}"/> created with <see cref="CreateWithSynchronization{TKey,TValue}(IEqualityComparer{TKey})"/> 
    /// provides better performance and contention behavior. Existing usages of <see cref="CreateWithLocking{TKey,TValue}(IEqualityComparer{TKey})"/> 
    /// should therefor be replaced.
    /// </remarks>
    [Obsolete("Use CreateWithSynchronization(...) instead. (Version: 1.19.3)")]
    public static ICache<TKey, TValue> CreateWithLocking<TKey, TValue> ([CanBeNull] IEqualityComparer<TKey>? comparer)
        where TKey : notnull
    {
      return new LockingCacheDecorator<TKey, TValue>(new Cache<TKey, TValue>(comparer ?? EqualityComparer<TKey>.Default));
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
    /// The <see cref="ConcurrentCache{TKey,TValue}"/> created with <see cref="CreateWithSynchronization{TKey,TValue}(LockingInvalidationToken, IEqualityComparer{TKey})"/> 
    /// provides better performance and contention behavior. Existing usages of <see cref="CreateWithLocking{TKey,TValue}(LockingInvalidationToken, IEqualityComparer{TKey})"/> 
    /// should therefor be replaced.
    /// </remarks>
    [Obsolete("Use CreateWithSynchronization(...) instead. (Version: 1.19.3)")]
    public static ICache<TKey, TValue> CreateWithLocking<TKey, TValue> (
        [NotNull] LockingInvalidationToken invalidationToken,
        [CanBeNull] IEqualityComparer<TKey>? comparer)
        where TKey : notnull
    {
      ArgumentUtility.CheckNotNull("invalidationToken", invalidationToken);

      return new LockingCacheDecorator<TKey, TValue>(
          new InvalidationTokenBasedCacheDecorator<TKey, TValue>(
              new Cache<TKey, TValue>(comparer ?? EqualityComparer<TKey>.Default),
              invalidationToken));
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
    /// The <see cref="ConcurrentCache{TKey,TValue}"/> created with <see cref="CreateWithSynchronization{TKey,TValue}()"/> 
    /// provides better performance and contention behavior. Existing usages of <see cref="CreateWithLazyLocking{TKey,TValue}()"/> 
    /// should therefor be replaced.
    /// </remarks>
    [Obsolete("Use CreateWithSynchronization(...) instead. (Version: 1.19.3)")]
    public static ICache<TKey, TValue> CreateWithLazyLocking<TKey, TValue> ()
        where TKey : notnull
        where TValue : class?
    {
      return new LazyLockingCachingAdapter<TKey, TValue>(
          new Cache<TKey, Lazy<LazyLockingCachingAdapter<TKey, TValue>.Wrapper>>());
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
    /// The <see cref="ConcurrentCache{TKey,TValue}"/> created with <see cref="CreateWithSynchronization{TKey,TValue}(LockingInvalidationToken)"/> 
    /// provides better performance and contention behavior. Existing usages of <see cref="CreateWithLazyLocking{TKey,TValue}(LockingInvalidationToken)"/> 
    /// should therefor be replaced.
    /// </remarks>
    [Obsolete("Use CreateWithSynchronization(...) instead. (Version: 1.19.3)")]
    public static ICache<TKey, TValue> CreateWithLazyLocking<TKey, TValue> (
        [NotNull] LockingInvalidationToken invalidationToken)
        where TKey : notnull
        where TValue : class?
    {
      ArgumentUtility.CheckNotNull("invalidationToken", invalidationToken);

      return new LazyLockingCachingAdapter<TKey, TValue>(
          new InvalidationTokenBasedCacheDecorator<TKey, Lazy<LazyLockingCachingAdapter<TKey, TValue>.Wrapper>>(
              new Cache<TKey, Lazy<LazyLockingCachingAdapter<TKey, TValue>.Wrapper>>(),
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
    /// The <see cref="ConcurrentCache{TKey,TValue}"/> created with <see cref="CreateWithSynchronization{TKey,TValue}(IEqualityComparer{TKey})"/> 
    /// provides better performance and contention behavior. Existing usages of <see cref="CreateWithLazyLocking{TKey,TValue}(IEqualityComparer{TKey})"/> 
    /// should therefor be replaced.
    /// </remarks>
    [Obsolete("Use CreateWithSynchronization(...) instead. (Version: 1.19.3)")]
    public static ICache<TKey, TValue> CreateWithLazyLocking<TKey, TValue> ([CanBeNull] IEqualityComparer<TKey>? comparer)
        where TKey : notnull
        where TValue : class?
    {
      return new LazyLockingCachingAdapter<TKey, TValue>(
          new Cache<TKey, Lazy<LazyLockingCachingAdapter<TKey, TValue>.Wrapper>>(comparer ?? EqualityComparer<TKey>.Default));
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
    /// The <see cref="ConcurrentCache{TKey,TValue}"/> created with <see cref="CreateWithSynchronization{TKey,TValue}(LockingInvalidationToken, IEqualityComparer{TKey})"/> 
    /// provides better performance and contention behavior. Existing usages of <see cref="CreateWithLazyLocking{TKey,TValue}(LockingInvalidationToken, IEqualityComparer{TKey})"/> 
    /// should therefor be replaced.
    /// </remarks>
    [Obsolete("Use CreateWithSynchronization(...) instead. (Version: 1.19.3)")]
    public static ICache<TKey, TValue> CreateWithLazyLocking<TKey, TValue> (
        [NotNull] LockingInvalidationToken invalidationToken,
        [CanBeNull] IEqualityComparer<TKey>? comparer)
        where TKey : notnull
        where TValue : class?
    {
      ArgumentUtility.CheckNotNull("invalidationToken", invalidationToken);

      return new LazyLockingCachingAdapter<TKey, TValue>(
          new InvalidationTokenBasedCacheDecorator<TKey, Lazy<LazyLockingCachingAdapter<TKey, TValue>.Wrapper>>(
              new Cache<TKey, Lazy<LazyLockingCachingAdapter<TKey, TValue>.Wrapper>>(comparer ?? EqualityComparer<TKey>.Default),
              invalidationToken));
    }
  }
}
