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

// ReSharper disable once CheckNamespace
namespace Remotion.Collections
{
  /// <summary>
  /// Read-only wrapper around an <see cref="IReadOnlyCollection{T}"/> to prevent casting an <see cref="IReadOnlyCollection{T}"/> back to mutable type.
  /// </summary>
  [Serializable]
  sealed class ReadOnlyCollectionWrapper<T> : IReadOnlyCollection<T>
  {
    private readonly IReadOnlyCollection<T> _collection;

    public ReadOnlyCollectionWrapper (IReadOnlyCollection<T> collection)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);

      _collection = collection;
    }

    public int Count
    {
      get { return _collection.Count; }
    }

    public IEnumerator<T> GetEnumerator ()
    {
      return _collection.GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator ();
    }
  }
}