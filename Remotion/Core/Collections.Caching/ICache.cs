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
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Remotion.Collections.Caching
{
  /// <summary>
  /// Provides a comnmon interface for caches, which provide efficient storage and retrieval for values that are costly to calculate.
  /// </summary>
  /// <typeparam name="TKey">The key type via which values should be indexed.</typeparam>
  /// <typeparam name="TValue">The type of the values to be stored in the cache.</typeparam>
  /// <remarks>
  /// Caches are only meant for performance improvement, they are not reliable data containers. Do not rely on values being present in the cache;
  /// caches might choose to remove individual items (or all their items) at any time. If a reliable store is needed, use 
  /// <see cref="IDictionary{TKey,TValue}"/> or <see cref="T:Remotion.Collections.DataStore.IDataStore{TKey,TValue}"/>.
  /// </remarks>
  public interface ICache<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, INullObject
      where TKey : notnull
  {
    /// <summary>
    /// Gets the value of the element with the specified key, creating a new one if none exists.
    /// </summary>
    /// <param name="key">The key of the element to be retrieved. Must not be <see langword="null" />.</param>
    /// <param name="valueFactory">A delegate used for creating a new element if none exists. Must not be <see langword="null" />.</param>
    /// <returns>The value of the element that was found or created.</returns>
    /// <exception cref="InvalidOperationException">An attempt is made to call <see cref="GetOrCreateValue"/> from inside the factory using the <paramref name="key"/>.</exception>
    TValue GetOrCreateValue ([JetBrains.Annotations.NotNull] TKey key, [JetBrains.Annotations.NotNull] Func<TKey, TValue> valueFactory);

    /// <summary>
    /// Tries to get the value of the element with the specified key.
    /// </summary>
    /// <param name="key">The key to look up. Must not be <see langword="null" />.</param>
    /// <param name="value">
    /// The value of the element with the specified key, or <typeparamref name="TValue"/>'s default value if no such element exists.</param>
    /// <returns><see langword="true" /> if an element with the specified key was found; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">An attempt is made to call <see cref="GetOrCreateValue"/> from inside the factory using the <paramref name="key"/>.</exception>
    bool TryGetValue ([JetBrains.Annotations.NotNull] TKey key, [CanBeNull, AllowNull, MaybeNullWhen (false)] out TValue value);

    /// <summary>
    /// Removes all elements from the store.
    /// </summary>
    void Clear ();
  }
}
