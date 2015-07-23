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

namespace Remotion.Collections
{
  /// <summary>
  /// Provides a common interface for data structures used for storing and retrieving key/value pairs.
  /// </summary>
  /// <typeparam name="TKey">The type of the keys.</typeparam>
  /// <typeparam name="TValue">The type of the values.</typeparam>
  /// <remarks>
  /// <para>
  /// This interface is basically a simplified version of the <see cref="IDictionary{TKey,TValue}"/> interface. In contrast to 
  /// <see cref="IDictionary{TKey,TValue}"/>, it does not require implementers to support <see cref="IEnumerable{T}"/>, <see cref="ICollection{T}"/>,
  /// etc, so it is much simpler to implement.
  /// </para>
  /// <para>
  /// Use this in place of <see cref="ICache{TKey,TValue}"/> if you need a reliable data store which guarantees to keep values once inserted until
  /// they are removed.
  /// </para>
  /// </remarks>
  public interface IDataStore<TKey, TValue> : INullObject
  {
    /// <summary>
    /// Determines whether the store contains an element with the specified <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key to look up.</param>
    /// <returns>
    /// <see langword="true" /> if the store contains the specified key; otherwise, <see langword="false" />.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    bool ContainsKey ([NotNull] TKey key);

    /// <summary>
    /// Adds a new element to the store.
    /// </summary>
    /// <param name="key">The key of the new element. Must not be <see langword="null" />.</param>
    /// <param name="value">The value of the new element. Can be <see langword="null" /> null.</param>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">An item with an equal key already exists in the store.</exception>
    void Add ([NotNull] TKey key, [CanBeNull] TValue value);

    /// <summary>
    /// Removes the element with the specified key from the store, if any.
    /// </summary>
    /// <param name="key">The key of the element to be removed. Must not be <see langword="null" />.</param>
    /// <returns><see langword="true" /> if the item was found in the store; otherwise, <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    bool Remove ([NotNull] TKey key);

    /// <summary>
    /// Removes all elements from the store.
    /// </summary>
    void Clear ();

    /// <summary>
    /// Gets or sets the value of the element with the specified key.
    /// </summary>
    /// <value>The value of the element.</value>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">The element whose value should be retrieved could not be found.</exception>
    TValue this [[NotNull] TKey key] { get; set; }

    /// <summary>
    /// Gets the value of the element with the specified key, or <typeparamref name="TValue"/>'s default value if no such element exists.
    /// </summary>
    /// <param name="key">The key to look up. Must not be <see langword="null" />.</param>
    /// <returns>The value of the element, or the default value if no such element exists.</returns>
    TValue GetValueOrDefault ([NotNull] TKey key);

    /// <summary>
    /// Tries to get the value of the element with the specified key.
    /// </summary>
    /// <param name="key">The key to look up. Must not be <see langword="null" />.</param>
    /// <param name="value">
    /// The value of the element with the specified key, or <typeparamref name="TValue"/>'s default value if no such element exists.
    /// Can be <see langword="null" />.
    /// </param>
    /// <returns><see langword="true" /> if an element with the specified key was found; otherwise, <see langword="false" />.</returns>
    bool TryGetValue ([NotNull] TKey key, [CanBeNull] out TValue value);

    /// <summary>
    /// Gets the value of the element with the specified key, creating a new one if none exists.
    /// </summary>
    /// <param name="key">The key of the element to be retrieved. Must not be <see langword="null" />.</param>
    /// <param name="valueFactory">A delegate used for creating a new element if none exists. Must not be <see langword="null" />.</param>
    /// <returns>The value of the element that was found or created.</returns>
    TValue GetOrCreateValue ([NotNull] TKey key, [NotNull] Func<TKey, TValue> valueFactory);
  }
}