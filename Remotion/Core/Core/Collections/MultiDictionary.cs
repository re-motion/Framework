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

namespace Remotion.Collections
{
  /// <summary>
  /// A dictionary that contains a <see cref="List{T}"/> of values for every key.
  /// </summary>
  public class MultiDictionary<TKey, TValue> : AutoInitDictionary<TKey, List<TValue>>
      where TKey : notnull
  {
    public MultiDictionary ()
    {
    }

    public MultiDictionary (IEqualityComparer<TKey> comparer)
      : base(comparer)
    {
    }

    public int KeyCount
    {
      get { return base.Count; }
    }

    public int CountValues ()
    {
      int count = 0;
      foreach (TKey key in Keys)
        count += this[key].Count;
      return count;
    }

    /// <summary>
    /// Adds a value to the key's value list.
    /// </summary>
    public void Add (TKey key, TValue value)
    {
      this[key].Add(value);
    }
  }
}
