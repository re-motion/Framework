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

namespace Remotion.Collections
{
  /// <summary>
  /// This class implements a data store that doesn't actually store anything. It's part of the null object pattern.
  /// </summary>
  /// <typeparam name="TKey">The type of the keys.</typeparam>
  /// <typeparam name="TValue">The type of the values.</typeparam>
  public class NullDataStore<TKey, TValue> : IDataStore<TKey, TValue>
  {
    public static readonly NullDataStore<TKey, TValue> Instance = new NullDataStore<TKey, TValue> ();

    private NullDataStore()
    {
    }

    public bool ContainsKey (TKey key)
    {
      return false;
    }

    public void Add (TKey key, TValue value)
    {
    }

    public bool Remove (TKey key)
    {
      return false;
    }

    public void Clear ()
    {
    }

    public TValue this [TKey key]
    {
      get { throw new InvalidOperationException ("No values can be retrieved from this cache."); }
      set { }
    }

    public TValue GetValueOrDefault (TKey key)
    {
      return default (TValue);
    }

    public bool TryGetValue (TKey key, out TValue value)
    {
      value = default (TValue);
      return false;
    }

    public TValue GetOrCreateValue (TKey key, Func<TKey, TValue> valueFactory)
    {
      throw new NotImplementedException();
    }

    public bool IsNull
    {
      get { return true; }
    }
  }
}
