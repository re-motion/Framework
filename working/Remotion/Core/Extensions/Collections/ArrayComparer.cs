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
  /// Provides equality and hash codes for arrays. Use <see cref="GetComparer"/> to get an instance.
  /// </summary>
  public class ArrayComparer<T> : IEqualityComparer<T[]>
  {
    public struct Wrapper: IEquatable<Wrapper>
    {
      private T[] _array;
    
      public Wrapper (T[] array)
      {
        _array = array;
      }

      public T[] Array
      {
        get { return _array; }
      }

      public static implicit operator T[] (Wrapper wrapper)
      {
        return wrapper.Array; 
      }

      public static implicit operator Wrapper (T[] array)
      {
        return new Wrapper (array);
      }

      public override bool Equals (object obj)
      {
        if (! (obj is Wrapper))
          return false;
        return Equals ((Wrapper)obj);
      }

      public bool Equals (Wrapper other)
      {
        return GetComparer().Equals (this._array, other._array);
      }

      public override int GetHashCode ()
      {
        return GetComparer().GetHashCode (_array);
      }
    }

    private static ArrayComparer<T> _instance = new ArrayComparer<T>();

    public static ArrayComparer<T> GetComparer()
    {
      return _instance;
    }

    private ArrayComparer()
    {
    }

    public bool Equals (T[] x, T[] y)
    {
      if (ReferenceEquals (x, y))
        return true;

      if (ReferenceEquals (x, null) || ReferenceEquals (y, null))
        return false;

      if (x.Rank != 1 || y.Rank != 1)
        throw new NotSupportedException ("ArrayComparer does not support multidimensional arrays.");

      if (x.Length != y.Length)
        return false;

      for (int i = 0; i < x.Length; ++i)
      {
        if (!Equals (x[i], y[i]))
          return false;
      }

      return true;
    }

    public int GetHashCode (T[] array)
    {
      if (ReferenceEquals (array, null))
        return 0;

      if (array.Rank != 1)
        throw new NotSupportedException ("ArrayComparer does not support multidimensional arrays.");

      int hc = 0;
      for (int i = 0; i < array.Length; ++i)
      {
        T item = array[i];
        if (item != null)
          hc ^= item.GetHashCode ();
      }
      return hc;
    }
  }
}
