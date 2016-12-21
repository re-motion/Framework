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
using System.Threading;

namespace Remotion.Collections
{
  /// <summary>
  /// The <see cref="ExpiringDataStoreFactory"/> provides factory methods to create new expired data stores.
  /// </summary>
  public class ExpiringDataStoreFactory
  {
    /// <summary>
    /// Creates a <see cref="ExpiringDataStore{TKey,TValue,TExpirationInfo,TScanInfo}"/> instance that is not thread-safe and uses the specified
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <typeparam name="TExpirationInfo">The type of the expiration info used by the <paramref name="policy"/>.</typeparam>
    /// <typeparam name="TScanInfo">The type of the scan info used by the <paramref name="policy"/>.</typeparam>
    /// <param name="policy">The policy that is used to check for expired items.</param>
    /// <param name="comparer">The comparer to use for comparing keys.</param>
    /// <returns>
    /// A <see cref="ExpiringDataStore{TKey,TValue,TExpirationInfo,TScanInfo}"/> instances for storing keys and values.
    /// </returns>
    public static ExpiringDataStore<TKey, TValue, TExpirationInfo, TScanInfo> Create<TKey, TValue, TExpirationInfo, TScanInfo> (
        IExpirationPolicy<TValue, TExpirationInfo, TScanInfo> policy, IEqualityComparer<TKey> comparer)
    {
      return new ExpiringDataStore<TKey, TValue, TExpirationInfo, TScanInfo> (policy, comparer);
    }

    /// <summary>
    /// Creates a <see cref="LockingDataStoreDecorator{TKey,TValue}"/> instance that is thread-safe and uses the specified
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <typeparam name="TExpirationInfo">The type of the expiration info used by the <paramref name="policy"/>.</typeparam>
    /// <typeparam name="TScanInfo">The type of the scan info used by the <paramref name="policy"/>.</typeparam>
    /// <param name="comparer">The comparer to use for comparing keys.</param>
    /// <param name="policy">The policy that is used to check for expired items.</param>
    /// <returns>
    /// A <see cref="LockingDataStoreDecorator{TKey,TValue}"/> instances for storing keys and values in a thread-safe way.
    /// </returns>
    /// <remarks>
    /// The created instance uses a single lock (see <see cref="Monitor"/>) to guard the data store against multi-threaded access. It is well-suited
    /// for data stores in which the factory delegates passed to <see cref="IDataStore{TKey,TValue}.GetOrCreateValue"/> only take a short time to 
    /// complete. When the factory delegates take a long time to execute, consider using <see cref="CreateWithLazyLocking{TKey,TValue,TExpirationInfo,TScanInfo}"/> 
    /// instead to reduce contention.
    /// </remarks>
    public static LockingDataStoreDecorator<TKey, TValue> CreateWithLocking<TKey, TValue, TExpirationInfo, TScanInfo> (
        IExpirationPolicy<TValue, TExpirationInfo, TScanInfo> policy, IEqualityComparer<TKey> comparer)
    {
      return new LockingDataStoreDecorator<TKey, TValue> (new ExpiringDataStore<TKey, TValue, TExpirationInfo, TScanInfo> (policy, comparer));
    }

    /// <summary>
    /// Creates a <see cref="LazyLockingDataStoreAdapter{TKey,TValue}"/> instance that is thread-safe and uses the specified
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <typeparam name="TExpirationInfo">The type of the expiration info used by the <paramref name="policy"/>.</typeparam>
    /// <typeparam name="TScanInfo">The type of the scan info used by the <paramref name="policy"/>.</typeparam>
    /// <param name="comparer">The comparer to use for comparing keys.</param>
    /// <param name="policy">The policy that is used to check for expired items.</param>
    /// <returns>
    /// A <see cref="LazyLockingDataStoreAdapter{TKey,TValue}"/> instances for storing keys and values in a thread-safe way.
    /// </returns>
    /// <remarks>
    /// The created instance uses a single lock (see <see cref="Monitor"/>) to guard the data store against multi-threaded access and additional,
    /// double-checked locks (see <see cref="DoubleCheckedLockingContainer{T}"/>) to protect each single value. It is well-suited for data stores
    /// in which the factory delegates passed to <see cref="IDataStore{TKey,TValue}.GetOrCreateValue"/> take a long time to execute. When the factory
    /// delegates do not take a long time, consider using <see cref="CreateWithLocking{TKey,TValue,TExpirationInfo,TScanInfo}"/> instead to reduce the number of locks used.
    /// </remarks>
    public static LazyLockingDataStoreAdapter<TKey, TValue> CreateWithLazyLocking<TKey, TValue, TExpirationInfo, TScanInfo> (
        IExpirationPolicy<DoubleCheckedLockingContainer<LazyLockingDataStoreAdapter<TKey, TValue>.Wrapper>, TExpirationInfo, TScanInfo> policy,
        IEqualityComparer<TKey> comparer) 
        where TValue: class
    {
      return new LazyLockingDataStoreAdapter<TKey, TValue> (
          new ExpiringDataStore<TKey, DoubleCheckedLockingContainer<LazyLockingDataStoreAdapter<TKey, TValue>.Wrapper>, TExpirationInfo, TScanInfo> (
              policy, 
              comparer));
    }
  }
}