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
using System.Diagnostics;
using Remotion.Utilities;

namespace Remotion.Collections
{
  /// <summary>
  /// Implements a set collection data type.
  /// </summary>
  /// <typeparam name="T">The type of items to be stored by this collection.</typeparam>
  /// <remarks>
  /// 	<para>
  /// A <see cref="Set{T}"/> is an unordered collection of items, where each item added is held exactly once. When an item is added more than once
  /// (determined by the given <see cref="IEqualityComparer{T}"/> or the default comparer <see cref="EqualityComparer{T}.Default"/>), the set
  /// will automatically ignore the second add operation.
  /// </para>
  /// 	<para>
  /// The <see cref="Set{T}"/> data type is internally based on a <see cref="Dictionary{T,T}"/>, so checking whether a set contains an item is
  /// very fast, but depends on the quality of the hashing algorithm used for <typeparamref name="T"/>.
  /// </para>
  /// <para>This collection type does not support items being <see langword="null"/>, is not automatically safe for multi-threading, and cannot be
  /// set to be read-only.</para>
  /// </remarks>
  [DebuggerDisplay ("Set - Count = {Count}")]
  [Serializable]
  [Obsolete ("Use a standard .NET System.Collections.Generic.HashSet<T> instead. (1.13.185.0)", true)]
  public class Set<T> : IEnumerable<T>, ICollection<T>, ICollection
  {
    private Dictionary<T, T> _items;

    /// <summary>
    /// Initializes a new empty instance of the <see cref="Set&lt;T&gt;"/> class.
    /// </summary>
    public Set ()
    {
      _items = new Dictionary<T, T> ();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Set&lt;T&gt;"/> class, adding a range of initial items.
    /// </summary>
    /// <param name="initialItems">The initial items to be held by the set. If this contains duplicates, they will be filtered out while being
    /// added to the set.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="initialItems"/> parameter is <see langword="null"/> or contains a
    /// <see langword="null"/> reference.</exception>
    public Set (IEnumerable<T> initialItems)
      : this ()
    {
      ArgumentUtility.CheckNotNull ("initialItems", initialItems);
      AddRange (initialItems);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Set&lt;T&gt;"/> class, adding a range of initial items.
    /// </summary>
    /// <param name="initialItems">The initial items to be held by the set. If this contains duplicates, they will be filtered out while being
    /// added to the set.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="initialItems"/> parameter is <see langword="null"/> or contains a
    /// <see langword="null"/> reference.</exception>
    public Set (params T[] initialItems)
      : this ((IEnumerable<T>) initialItems)
    {
    }

    /// <summary>
    /// Initializes a new empty instance of the <see cref="Set&lt;T&gt;"/> class with a given <see cref="EqualityComparer{T}"/>.
    /// </summary>
    /// <param name="equalityComparer">The comparer to be used to determine whether two items are equal to each other.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="equalityComparer"/> parameter is <see langword="null"/>.</exception>
    public Set (IEqualityComparer<T> equalityComparer)
    {
      _items = new Dictionary<T, T> (equalityComparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Set&lt;T&gt;"/> class, adding a range of initial items.
    /// </summary>
    /// <param name="initialItems">The initial items to be held by the set. If this contains duplicates, they will be filtered out in the adding
    /// process.</param>
    /// <param name="equalityComparer">The comparer to be used to determine whether two items are equal to each other.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="initialItems"/> parameter is <see langword="null"/> or contains a
    /// <see langword="null"/> reference, or the <paramref name="equalityComparer"/> parameter is <see langword="null"/>.</exception>
    public Set (IEnumerable<T> initialItems, IEqualityComparer<T> equalityComparer)
      : this (equalityComparer)
    {
      ArgumentUtility.CheckNotNull ("initialItems", initialItems);
      AddRange (initialItems);
    }

    /// <summary>
    /// Adds a range of items to the set.
    /// </summary>
    /// <param name="items">The items to be added to the set. If this contains duplicates, they will be filtered out in the adding
    /// process.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="items"/> parameter is <see langword="null"/> or contains a
    /// <see langword="null"/> reference.</exception>
    public void AddRange (IEnumerable<T> items)
    {
      ArgumentUtility.CheckNotNull ("items", items);
      foreach (T item in items)
        Add (item);
    }

    /// <summary>
    /// Adds an item to the set.
    /// </summary>
    /// <param name="item">The item to add to the set. If the set already contains this item, the operation is ignored.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="item"/> parameter is <see langword="null"/>.</exception>
    public void Add (T item)
    {
      ArgumentUtility.CheckNotNull ("item", item);
      if (!Contains (item))
        _items.Add (item, item);
    }

    /// <summary>
    /// Removes all items from the set.
    /// </summary>
    public void Clear ()
    {
      _items.Clear ();
    }

    /// <summary>
    /// Determines whether the set contains a specific value.
    /// </summary>
    /// <param name="item">The object to locate in the set.</param>
    /// <returns>
    /// True if item is found in the set; otherwise, false.
    /// </returns>
    public bool Contains (T item)
    {
      ArgumentUtility.CheckNotNull ("item", item);
      return _items.ContainsKey (item);
    }

    /// <summary>
    /// Copies the elements of the set to an <see cref="T:System.Array"/> starting at a particular <see cref="T:System.Array"></see> index.
    /// </summary>
    /// <param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from
    /// the set. The <see cref="T:System.Array"></see> must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception>
    /// <exception cref="T:System.ArgumentException"><para><paramref name="array"/> is multidimensional.</para>
    /// <para>-or-</para>
    /// <para><paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.</para>
    /// <para>-or-</para>
    /// <para>The number of elements in the set is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination
    /// array.</para>
    /// </exception>
    /// <remarks>The elements are copied to the <see cref="T:System.Array"/> in the same order in which the enumerator iterates through the
    /// set.</remarks>
    public void CopyTo (T[] array, int arrayIndex)
    {
      ArgumentUtility.CheckNotNull ("array", array);

      _items.Keys.CopyTo (array, arrayIndex);
    }

    ///<summary>
    ///Copies the elements of the <see cref="Set{T}"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
    ///</summary>
    ///
    ///<param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied.
    ///The <see cref="T:System.Array"></see> must have zero-based indexing. </param>
    ///<param name="index">The zero-based index in array at which copying begins. </param>
    ///<exception cref="T:System.ArgumentNullException">array is null. </exception>
    ///<exception cref="T:System.ArgumentOutOfRangeException">index is less than zero. </exception>
    ///<exception cref="T:System.ArgumentException">array is multidimensional. -or- index is equal to or greater than the length of array. -or-
    /// The number of elements in the <see cref="Set{T}"/> is greater than the available space from index to the end of the destination array. </exception>
    ///<exception cref="T:System.InvalidCastException">The type of the <see cref="Set{T}"/> elements cannot be cast automatically to the type of the
    /// destination array. </exception><filterpriority>2</filterpriority>
    void ICollection.CopyTo (Array array, int index)
    {
      ArgumentUtility.CheckNotNull ("array", array);
      ((ICollection) _items.Keys).CopyTo (array, index);
    }

    /// <summary>
    /// Returns an array holding the items currently stored in the set.
    /// </summary>
    /// <returns>An array holding the same items as the set</returns>
    /// <remarks>The elements are copied to the <see cref="T:System.Array"/> in the same order in which the enumerator iterates through the
    /// set.</remarks>
    public T[] ToArray ()
    {
      T[] array = new T[Count];
      CopyTo (array, 0);
      return array;
    }

    /// <summary>
    /// Removes the given item from the set, if it is contained.
    /// </summary>
    /// <param name="item">The object to remove from the set.</param>
    /// <returns>
    /// True if the set contained the item; otherwise, false.
    /// </returns>
    public bool Remove (T item)
    {
      ArgumentUtility.CheckNotNull ("item", item);
      return _items.Remove (item);
    }

    /// <summary>
    /// Gets the number of elements contained in the set.
    /// </summary>
    /// <returns>The number of elements contained in the set.</returns>
    public int Count
    {
      get { return _items.Count; }
    }

    ///<summary>
    ///This method is not supported and always throws an exception.
    ///</summary>
    ///<exception cref="NotSupportedException">The <see cref="Set{T}"/> collection type does not support SyncRoots.</exception>
    object ICollection.SyncRoot
    {
      get { throw new NotSupportedException ("The Set collection type does not support SyncRoots."); }
    }

    ///<summary>
    ///Gets a value indicating whether access to the collection is synchronized (thread safe). This always returns false.
    ///</summary>
    ///<returns>
    ///Always false.
    ///</returns>
    bool ICollection.IsSynchronized
    {
      get { return false; }
    }

    bool ICollection<T>.IsReadOnly
    {
      get { return false; }
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator ();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
    /// </returns>
    /// <remarks>The order in which items are returned by the enumerator is undefined and may change between consecutive calls of
    /// <see cref="GetEnumerator"/> if items are added or removed between the calls.</remarks>
    public IEnumerator<T> GetEnumerator ()
    {
      return _items.Keys.GetEnumerator ();
    }

    /// <summary>
    /// Returns an arbitrary element contained in the set without removing it.
    /// </summary>
    /// <returns>An arbitrary element contained in the set.</returns>
    /// <exception cref="InvalidOperationException">The set is empty.</exception>
    public T GetAny ()
    {
      if (Count == 0)
        throw new InvalidOperationException ("The set is empty.");
      using (IEnumerator<T> enumerator = GetEnumerator())
      {
        enumerator.MoveNext();
        return enumerator.Current;
      }
    }
  }
}
