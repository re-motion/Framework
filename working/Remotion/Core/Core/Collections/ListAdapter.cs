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
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Collections
{
  /// <summary>
  /// Implements a different version of <see cref="IList{T}"/> for an object that already implements <see cref="IList{T}"/>. Delegates are used
  /// to convert items from the first to the second implementation.
  /// </summary>
  /// <remarks>
  /// Use this class if your only have an implementation of <c>IList&lt;T&gt;</c>, but need <c>IList&lt;U&gt;</c> instead (if, of course, T and U
  /// can be converted to each other).
  /// </remarks>
  public class ListAdapter<TSource, TDest> : IList<TDest>
  {
    private readonly IList<TSource> _adaptedList;
    private readonly Func<TSource, TDest> _sourceToDest;
    private readonly Func<TDest, TSource> _destToSource;

    public ListAdapter (IList<TSource> adaptedList, Func<TSource, TDest> sourceToDest, Func<TDest, TSource> destToSource)
    {
      ArgumentUtility.CheckNotNull ("adaptedList", adaptedList);
      ArgumentUtility.CheckNotNull ("sourceToDest", sourceToDest);
      ArgumentUtility.CheckNotNull ("destToSource", destToSource);

      _adaptedList = adaptedList;
      _sourceToDest = sourceToDest;
      _destToSource = destToSource;
    }

    public int Count
    {
      get { return _adaptedList.Count; }
    }

    public bool IsReadOnly
    {
      get { return _adaptedList.IsReadOnly; }
    }

    public TDest this [int index]
    {
      get { return _sourceToDest (_adaptedList[index]); }
      set { _adaptedList[index] = _destToSource (value); }
    }

    public IEnumerator<TDest> GetEnumerator ()
    {
      return _adaptedList.Select (item => _sourceToDest (item)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public void Insert (int index, TDest item)
    {
      _adaptedList.Insert (index, _destToSource (item));
    }

    public void Add (TDest item)
    {
      _adaptedList.Add (_destToSource (item));
    }

    public bool Remove (TDest item)
    {
      return _adaptedList.Remove (_destToSource (item));
    }

    public void RemoveAt (int index)
    {
      _adaptedList.RemoveAt (index);
    }

    public void Clear ()
    {
      _adaptedList.Clear();
    }

    public bool Contains (TDest item)
    {
      var equalityComparer = EqualityComparer<TDest>.Default;
      return this.Any (element => equalityComparer.Equals (element, item));
    }

    public int IndexOf (TDest item)
    {
      var equalityComparer = EqualityComparer<TDest>.Default;
      var result = this.Select ((element, i) => new { element, i }).FirstOrDefault (tuple => equalityComparer.Equals (item, tuple.element));
      if (result == null)
        return -1;
      else
        return result.i;
    }

    public void CopyTo (TDest[] array, int arrayIndex)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("array", array);

      if (arrayIndex < 0)
        throw new ArgumentOutOfRangeException ("arrayIndex", "Index must not be negative.");
      if (arrayIndex >= array.Length)
        throw new ArgumentException ("Index must be less than the length of the array.", "arrayIndex");
      if (arrayIndex + Count > array.Length)
      {
        throw new ArgumentException (
            "There must be enough space to copy all items into the destination array starting at the given index.", "arrayIndex");
      }

      for (int i = 0; i < Count; ++i)
        array[arrayIndex + i] = this[i];
    }
  }

  /// <summary>
  /// Provides factory methods to create instances of <see cref="ListAdapter{TSource,TDest}"/>.
  /// </summary>
  public static class ListAdapter
  {
    public static ListAdapter<TSource, TDest> Adapt<TSource, TDest> (
        IList<TSource> adaptedList, 
        Func<TSource, TDest> sourceToDest, 
        Func<TDest, TSource> destToSource)
    {
      return new ListAdapter<TSource, TDest> (adaptedList, sourceToDest, destToSource);
    }

    public static ListAdapter<TSource, TDest> AdaptOneWay<TSource, TDest> (IList<TSource> adaptedList, Func<TSource, TDest> sourceToDest)
    {
      return new ListAdapter<TSource, TDest> (
          adaptedList,
          sourceToDest,
          delegate
          {
            var message = string.Format ("This list does not support setting of '{0}' values.", typeof (TDest).Name);
            throw new NotSupportedException (message);
          });
    }

    public static ReadOnlyCollection<TDest> AdaptReadOnly<TSource, TDest> (IList<TSource> adaptedList, Func<TSource, TDest> sourceToDest)
    {
      return new ReadOnlyCollection<TDest> (AdaptOneWay (adaptedList, sourceToDest));
    }
  }
}