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
using System.Diagnostics;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
/// <summary>
/// Base class for all collections of Remotion.Data.DomainObjects.
/// </summary>
[DebuggerDisplay("Collection: {GetType().Name} - Count: {Count}")]
public class CommonCollection : ICollection
{
  // types

  private class CollectionEnumerator : IEnumerator, IDisposable
  {
    private int _index;
    private long _collectionVersion;
    private CommonCollection _collection;

    public CollectionEnumerator (CommonCollection collection)
    {
      ArgumentUtility.CheckNotNull("collection", collection);
      _collection = collection;
      _collectionVersion = _collection._version;
      _index = -1;
    }

    #region IEnumerator Members

    public void Reset ()
    {
      CheckVersion();
      _index = -1;
    }

    public object Current
    {
      get { return _collection.BaseGetObject(_index); }
    }

    public bool MoveNext ()
    {
      CheckVersion();
      _index++;
      return (_index < _collection.Count);
    }

    #endregion

    #region IDisposable Members

    public void Dispose ()
    {
      _collection = null!;
    }

    #endregion

    private void CheckVersion ()
    {
      if (!_collection.IsReadOnly && _collectionVersion != _collection._version)
        throw new InvalidOperationException("Collection was modified during enumeration.");
    }
  }

  // static members and constants

  // member fields

  private Hashtable _collectionData;
  private ArrayList _collectionKeys;
  private bool _isReadOnly;
  private long _version;

  // construction and disposing

  /// <summary>
  /// Initializes a new <b>CommonCollection</b>.
  /// </summary>
  protected CommonCollection ()
  {
    _collectionData = new Hashtable();
    _collectionKeys = new ArrayList();
  }

  // methods and properties

  /// <summary>
  /// Gets a value indicating whether the collection is read-only.
  /// </summary>
  public virtual bool IsReadOnly
  {
    get { return _isReadOnly; }
  }

  #region IEnumerable Members

  /// <summary>
  /// Returns an enumerator that can iterate through the <see cref="CommonCollection"/>.
  /// </summary>
  /// <returns>An <see cref="System.Collections.IEnumerator"/> for the entire <see cref="CommonCollection"/>.</returns>
  public virtual IEnumerator GetEnumerator ()
  {
    return new CollectionEnumerator(this);
  }

  #endregion

  #region ICollection Members

  /// <summary>
  /// Gets a value indicating whether access to the <see cref="CommonCollection"/> is synchronized (thread-safe).
  /// </summary>
  public virtual bool IsSynchronized
  {
    get { return false; }
  }

  /// <summary>
  /// Gets the number of items contained in the collection.
  /// </summary>
  public virtual int Count
  {
    get { return _collectionData.Count; }
  }

  /// <summary>
  /// Copies the items of the <see cref="CommonCollection"/> to an Array, starting at a particular Array index.
  /// </summary>
  /// <param name="array">
  /// The one-dimensional array that is the destination of the items copied from <see cref="CommonCollection"/>. The array must have zero-based indexing.
  /// Must not be <see langword="null"/>.
  /// </param>
  /// <param name="index">The zero-based index in array at which copying begins.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> is smaller than 0.</exception>
  /// <exception cref="System.ArgumentException">
  ///   <paramref name="array"/> is not a one-dimensional array.<br /> -or- <br />
  ///   <paramref name="index"/> is greater than the current length of the array.<br /> -or- <br />
  ///   The number of items is greater than the available space from <paramref name="index"/> to the end of <paramref name="array"/>.
  /// </exception>
  public virtual void CopyTo (Array array, int index)
  {
    ArgumentUtility.CheckNotNull("array", array);
    if (index < 0) throw new ArgumentOutOfRangeException("index", index, "Index must be greater than or equal to zero.");
    if (array.Rank != 1) throw new ArgumentException("CopyTo can only operate on one-dimensional arrays.", "array");
    if (Count > 0 && index >= array.Length)
      throw new ArgumentException("Index cannot be equal to or greater than the length of the array.", "index");
    if ((array.Length - index) < Count)
      throw new ArgumentException("The number of items in the source collection is greater than the available space from index to the end of the destination array.", "index");

    for (int i = 0; i < Count; i++)
      array.SetValue(this.BaseGetObject(i), index + i);
  }

  /// <summary>
  /// Gets an object that can be used to synchronize access to the <see cref="CommonCollection"/>.
  /// </summary>
  public virtual object SyncRoot
  {
    get { return this; }
  }

  #endregion


  /// <summary>
  /// Returns the object with a given index from the collection 
  /// </summary>
  /// <param name="index">The index of the object to return.</param>
  /// <returns>The object with the given index.</returns>
  /// <exception cref="System.ArgumentOutOfRangeException">
  ///   <paramref name="index"/> is less than zero.<br /> -or- <br />
  ///   <paramref name="index"/> is equal to or greater than <see cref="Count"/>.
  /// </exception>
  protected object BaseGetObject (int index)
  {
    var collectionKey = _collectionKeys[index];
    Assertion.DebugIsNotNull(collectionKey, "{0} must not contain null entries.", nameof(_collectionKeys));
    var value = _collectionData[collectionKey];
    Assertion.DebugIsNotNull(value, "{0} is out-of-sync with {1} or contains a null entry for index {2}.", nameof(_collectionData), nameof(_collectionKeys), index);
    return value;
  }

  /// <summary>
  /// Returns the object with a given key from the collection.
  /// </summary>
  /// <param name="key">The key of the object to return. Must not be <see langword="null"/>.</param>
  /// <returns>The object with the given key, if the object is found; otherwise, <see langword="null"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
  protected object? BaseGetObject (object key)
  {
    ArgumentUtility.CheckNotNull("key", key);

    return _collectionData[key];
  }

  /// <summary>
  /// Determines whether the <see cref="CommonCollection"/> contains a specific key.
  /// </summary>
  /// <param name="key">The key to locate in the <see cref="CommonCollection"/>. Must not be <see langword="null"/>.</param>
  /// <returns><see langword="true"/> if the <see cref="CommonCollection"/> contains the key; otherwise <see langword="false"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
  protected bool BaseContainsKey (object key)
  {
    ArgumentUtility.CheckNotNull("key", key);

    return _collectionData.ContainsKey(key);
  }

  /// <summary>
  /// Determines whether the <see cref="CommonCollection"/> contains a specified value for a key.
  /// </summary>
  /// <param name="key">The key to located in the <see cref="CommonCollection"/>. Must not be <see langword="null"/>.</param>
  /// <param name="value">The object that is expected as value for the specified key.</param>
  /// <returns><see langword="true"/> if the <see cref="CommonCollection"/> contains the key and the object found for the key is equal to <paramref name="value"/>; 
  /// otherwise <see langword="false"/>.</returns>
  /// <remarks>This method locates the object with the given <paramref name="key"/> in the collection and compares it with the given <paramref name="value"/>. 
  /// <see cref="object.Equals(object,object)"/> is used for the comparison.</remarks>
  protected bool BaseContains (object key, object value)
  {
    ArgumentUtility.CheckNotNull("key", key);

    if (!BaseContainsKey(key))
      return false;

    object? objectInCollection = BaseGetObject(key);
    return object.Equals(objectInCollection, value);
  }

  /// <summary>
  /// Adds an item with the specified key and value.
  /// </summary>
  /// <param name="key">A key of the item to add. The key must not be <see langword="null"/>. Must not be <see langword="null"/>.</param>
  /// <param name="value">The value of the item to add. The value must not be <see langword="null"/>. Must not be <see langword="null"/>.</param>
  /// <returns>The position into which the new item was inserted.</returns>
  /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
  /// <exception cref="System.ArgumentNullException">
  ///   <paramref name="key"/> is <see langword="null"/>.<br /> -or- <br />
  ///   <paramref name="value"/> is <see langword="null"/>.
  /// </exception>
  protected int BaseAdd (object key, object value)
  {
    ArgumentUtility.CheckNotNull("key", key);
    ArgumentUtility.CheckNotNull("value", value);
    if (_isReadOnly) throw new NotSupportedException("Cannot add an item to a read-only collection.");

    _collectionData.Add(key, value);
    _collectionKeys.Add(key);
    _version++;

    return _collectionKeys.Count - 1;
  }

  /// <summary>
  /// Removes the item with the specified key.
  /// </summary>
  /// <param name="key">The key of the item to remove. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
  protected void BaseRemove (object key)
  {
    ArgumentUtility.CheckNotNull("key", key);
    if (_isReadOnly) throw new NotSupportedException("Cannot remove an item from a read-only collection.");

    _collectionData.Remove(key);
    _collectionKeys.Remove(key);
    _version++;
  }

  /// <summary>
  /// Removes all objects from the collection.
  /// </summary>
  /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
  protected void BaseClear ()
  {
    if (_isReadOnly) throw new NotSupportedException("Cannot clear a read-only collection.");

    _collectionData.Clear();
    _collectionKeys.Clear();
    _version++;
  }

  /// <summary>
  /// Returns the zero-based index of the item with a given key in the collection.
  /// </summary>
  /// <param name="key">The <paramref name="key"/> to locate in the collection.</param>
  /// <returns>The zero-based index of the item with the given <paramref name="key"/>, if found; otherwise, -1.</returns>
  protected int BaseIndexOfKey (object key)
  {
    return _collectionKeys.IndexOf(key);
  }

  /// <summary>
  /// Inserts an item into the collection at the specified index.
  /// </summary>
  /// <param name="index">The zero-based <paramref name="index"/> at which the item should be inserted.</param>
  /// <param name="key">The key of the item to insert. Must not be <see langword="null"/>.</param>
  /// <param name="value">The <paramref name="value"/> of the item to add. The <paramref name="value"/> can be <see langword="null"/>.</param>
  /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
  /// <exception cref="System.ArgumentOutOfRangeException">
  ///   <paramref name="index"/> is less than zero.<br /> -or- <br />
  ///   <paramref name="index"/> is greater than <see cref="Count"/>.
  /// </exception>
  /// <exception cref="System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException">An item with the same <paramref name="key"/> already exists in the collection.</exception>
  protected void BaseInsert (int index, object key, object value)
  {
    if (_isReadOnly) throw new NotSupportedException("Cannot insert an item into a read-only collection.");
    CheckIndexForInsert("index", index);

    _collectionData.Add(key, value);
    _collectionKeys.Insert(index, key);
    _version++;
  }

  /// <summary>
  /// Checks the <paramref name="index"/> for an insert operation and throws an exception if it is invalid.
  /// </summary>
  /// <param name="argumentName">The <paramref name="argumentName"/> for throwing the exception.</param>
  /// <param name="index">The <paramref name="index"/> to check.</param>
  /// <exception cref="System.ArgumentOutOfRangeException">
  ///   <paramref name="index"/> is less than zero.<br /> -or- <br />
  ///   <paramref name="index"/> is greater than <see cref="Count"/>.
  /// </exception>
  protected void CheckIndexForInsert (string argumentName, int index)
  {
    if (index < 0 || index > Count)
    {
      throw new ArgumentOutOfRangeException(
          argumentName,
          index,
          "Index is out of range. Must be non-negative and less than or equal to the size of the collection.");
    }
  }

  /// <summary>
  /// Checks the <paramref name="index"/> for access via the indexer and throws an exception if it is invalid.
  /// </summary>
  /// <param name="argumentName">The <paramref name="argumentName"/> for throwing the exception.</param>
  /// <param name="index">The <paramref name="index"/> to check.</param>
  /// <exception cref="System.ArgumentOutOfRangeException">
  ///   <paramref name="index"/> is less than zero.<br /> -or- <br />
  ///   <paramref name="index"/> is equal to or greater than <see cref="Count"/>.
  /// </exception>
  protected void CheckIndexForIndexer (string argumentName, int index)
  {
    if (index < 0 || index >= Count)
    {
      throw new ArgumentOutOfRangeException(
          argumentName,
          index,
          "Index is out of range. Must be non-negative and less than the size of the collection.");
    }
  }

  /// <summary>
  /// Sets the <see cref="IsReadOnly"/> property of the collection.
  /// </summary>
  /// <param name="isReadOnly">The new value for the <see cref="IsReadOnly"/> property of the collection.</param>
  protected void SetIsReadOnly (bool isReadOnly)
  {
    _isReadOnly = isReadOnly;
  }
}
}
