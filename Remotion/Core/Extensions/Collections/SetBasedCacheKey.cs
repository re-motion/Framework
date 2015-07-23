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
using Remotion.Utilities;

namespace Remotion.Collections
{
  /// <summary>
  /// Takes a set of objects and provides value-type semantics to the set, so that it can be used as a cache key.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This type implements <see cref="GetHashCode"/> by calculating the hash codes of all elements in the set wrapped by this collection
  /// and combining them in an (order-independent) way. Therefore, two instances of this type give the same hash codes if the elements wrapped
  /// by them give the same hash codes. It is not guaranteed that two instances of this type give different hash codes if the elements give
  /// different hash codes.
  /// </para>
  /// <para>
  /// Similarly, <see cref="Equals(object)"/> is implemented by comparing the elements of the sets with each other. Two instances of this type
  /// compare equal if and only if they contain equal objects (without considering order).
  /// </para>
  /// <para>
  /// For efficiency, the hash codes of instances of this type are cached. Changes to the objects wrapped by an instance do not
  /// influance the hash code and should therefore be avoided.
  /// </para>
  /// <para>
  /// Note that, because this type is based on <see cref="HashSet{T}"/>, equal elements are regarded as one. This is, a set (1, 1, 2, 2) is equivalent to 
  /// a set (1, 2).
  /// </para>
  /// </remarks>
  public struct SetBasedCacheKey<T>
  {
    private static int CalculateHashCode (IEnumerable<T> items)
    {
      return EqualityUtility.GetXorHashCode (items);
    }

    private readonly HashSet<T> _items;
    private readonly int _cachedHashCode;

    public SetBasedCacheKey(params T[] items) : this((IEnumerable<T>) items)
    {
    }

    public SetBasedCacheKey (IEnumerable<T> items)
    {
      ArgumentUtility.CheckNotNull ("items", items);
      _items = new HashSet<T> (items);
      _cachedHashCode = CalculateHashCode (_items);
    }

    public int Count
    {
      get { return _items != null ? _items.Count : 0; }
    }

    public override int GetHashCode ()
    {
      return _cachedHashCode;
    }

    public override bool Equals (object obj)
    {
      if (!(obj is SetBasedCacheKey<T>))
        return false;

      var other = (SetBasedCacheKey<T>) obj;
      if (other.Count != Count)
        return false;

      if (Count > 0)
      {
        foreach (T item in _items)
        {
          if (!other._items.Contains (item))
            return false;
        }
      }

      return true;
    }
  }
}
