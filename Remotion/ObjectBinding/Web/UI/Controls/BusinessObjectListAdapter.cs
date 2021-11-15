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
using System.Linq;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  /// Wraps an implementation of <see cref="IList"/> into a type compatible with the <see cref="IReadOnlyList{T}"/> interface.
  /// </summary>
  public class BusinessObjectListAdapter<T> : IReadOnlyList<T>, IList
      where T : IBusinessObject
  {
    private readonly IList _list;

    public BusinessObjectListAdapter (IList list)
    {
      ArgumentUtility.CheckNotNull ("list", list);

      _list = list;
    }

    public IList WrappedList
    {
      get { return _list; }
    }

    public int Count
    {
      get { return _list.Count; }
    }

    public T this [int index]
    {
      get { return (T) _list[index]; }
    }

    public IEnumerator<T> GetEnumerator ()
    {
      return _list.Cast<T>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return _list.GetEnumerator();
    }

    void ICollection.CopyTo (Array array, int index)
    {
      _list.CopyTo (array, index);
    }

    bool ICollection.IsSynchronized
    {
      get { return _list.IsSynchronized; }
    }

    object ICollection.SyncRoot
    {
      get { return _list.SyncRoot; }
    }

    int IList.Add (object value)
    {
      return _list.Add (value);
    }

    void IList.Clear ()
    {
      _list.Clear();
    }

    bool IList.Contains (object value)
    {
      return _list.Contains (value);
    }

    int IList.IndexOf (object value)
    {
      return _list.IndexOf (value);
    }

    void IList.Insert (int index, object value)
    {
      _list.Insert (index, value);
    }

    void IList.Remove (object value)
    {
      _list.Remove (value);
    }

    void IList.RemoveAt (int index)
    {
      _list.RemoveAt (index);
    }

    bool IList.IsFixedSize
    {
      get { return _list.IsFixedSize; }
    }

    bool IList.IsReadOnly
    {
      get { return _list.IsReadOnly; }
    }

    object IList.this [int index]
    {
      get { return _list[index]; }
      set { _list[index] = value; }
    }
  }
}