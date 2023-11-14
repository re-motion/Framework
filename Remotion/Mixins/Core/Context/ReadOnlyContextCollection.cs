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
using Remotion.Utilities;

namespace Remotion.Mixins.Context
{
  public class ReadOnlyContextCollection<TKey, TValue> : ICollection<TValue>, ICollection
      where TKey : notnull
      where TValue : notnull
  {
    private readonly Func<TValue, TKey> _keyGenerator;
    private readonly IDictionary<TKey, TValue> _internalCollection;

    public ReadOnlyContextCollection (Func<TValue, TKey> keyGenerator, IEnumerable<TValue> values)
    {
      ArgumentUtility.CheckNotNull("keyGenerator", keyGenerator);
      _internalCollection = new Dictionary<TKey, TValue>();
      _keyGenerator = keyGenerator;

      // Workaround: For some reason, NCover and Visual Studio Coverage will produce invalid programs when the content of Initialize is inlined 
      // within this ctor.
      Initialize(values);
    }

    private void Initialize (IEnumerable<TValue> values)
    {
      foreach (TValue value in values)
      {
        ArgumentUtility.CheckNotNull("values[" + _internalCollection.Count + "]", value);

        TKey key = _keyGenerator(value);
        if (_internalCollection.TryGetValue(key, out var existingValue))
        {
          if (!value.Equals(existingValue))
          {
            string message = string.Format(
                "The items {0} and {1} are identified by the same key {2} and cannot both be added to the collection.",
                existingValue,
                value,
                key);
            throw new ArgumentException(message, "values");
          }
        }
        else
          _internalCollection.Add(key, value);
      }
    }

    public virtual int Count
    {
      get { return _internalCollection.Count; }
    }

    public virtual bool ContainsKey (TKey key)
    {
      ArgumentUtility.CheckNotNull("key", key);
      return _internalCollection.ContainsKey(key);
    }

    public virtual bool Contains (TValue value)
    {
      ArgumentUtility.CheckNotNull("value", value);
      TKey key = _keyGenerator(value);
      if (!_internalCollection.TryGetValue(key, out var foundValue))
        return false;
      else
        return value.Equals(foundValue);
    }

    // TODO RM-7703 evaluate if the indexer should be used for checking if a value exists.
    public virtual TValue? this[TKey key]
    {
      get
      {
        if (!_internalCollection.TryGetValue(key, out var value))
          return default(TValue);
        else
          return value;
      }
    }

    public virtual IEnumerator<TValue> GetEnumerator ()
    {
      return _internalCollection.Values.GetEnumerator();
    }

    public virtual void CopyTo (TValue[] array, int arrayIndex)
    {
      _internalCollection.Values.CopyTo(array, arrayIndex);
    }

    void ICollection<TValue>.Add (TValue item)
    {
      throw new NotSupportedException("This list cannot be changed.");
    }

    void ICollection<TValue>.Clear ()
    {
      throw new NotSupportedException("This list cannot be changed.");
    }

    bool ICollection<TValue>.Remove (TValue item)
    {
      throw new NotSupportedException("This list cannot be changed.");
    }

    bool ICollection<TValue>.IsReadOnly
    {
      get { return true; }
    }

    void ICollection.CopyTo (Array array, int index)
    {
      ((ICollection)_internalCollection.Values).CopyTo(array, index);
    }

    object ICollection.SyncRoot
    {
      get { return ((ICollection)_internalCollection).SyncRoot; }
    }

    bool ICollection.IsSynchronized
    {
      get { return false; }
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }
  }
}
