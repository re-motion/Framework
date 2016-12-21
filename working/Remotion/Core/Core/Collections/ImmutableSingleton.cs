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
using JetBrains.Annotations;

namespace Remotion.Collections
{
  /// <summary>
  /// Defines factory methods for <see cref="ImmutableSingleton{T}"/>.
  /// </summary>
  public static class ImmutableSingleton
  {
    public static ImmutableSingleton<T> Create<T> ([CanBeNull]T item)
    {
      return new ImmutableSingleton<T> (item);
    }
  }

  /// <summary>
  /// The <see cref="ImmutableSingleton{T}"/> is a single-item implementation of the <see cref="IReadOnlyList{T}"/> interface.
  /// </summary>
  /// <seealso cref="ImmutableSingleton"/>
  public sealed class ImmutableSingleton<T> : IReadOnlyList<T>
  {
    private sealed class Enumerator : IEnumerator<T>
    {
      private readonly T _item;
      private sbyte _position = -1;

      public Enumerator (T item)
      {
        _item = item;
      }

      public void Dispose ()
      {
      }

      public bool MoveNext ()
      {
        if (_position < 1)
          _position++;
        return _position < 1;
      }

      public void Reset ()
      {
        _position = -1;
      }

      public T Current
      {
        get
        {
          switch (_position)
          {
            case -1:
              throw new InvalidOperationException ("Enumeration has not started. Call MoveNext.");
            case 0:
              return _item;
            default:
              throw new InvalidOperationException ("Enumeration already finished.");
          }
        }
      }

      object IEnumerator.Current
      {
        get { return Current; }
      }
    }

    private readonly T _item;

    public ImmutableSingleton ([CanBeNull]T item)
    {
      _item = item;
    }

    public IEnumerator<T> GetEnumerator ()
    {
      return new Enumerator (_item);
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public int Count
    {
      get { return 1; }
    }

    public T this [int index]
    {
      get
      {
        if (index != 0)
          throw new ArgumentOutOfRangeException ("index", index, "The list contains only a single item.");

        return _item;
      }
    }
  }
}