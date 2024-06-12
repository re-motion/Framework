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
using System.Collections.ObjectModel;
using System.Linq;

namespace Remotion.Utilities
{
  /// <summary>
  /// Implements <see cref="IComparer{T}"/> as a composite of several individual <see cref="IComparer{T}"/> instances. If the first comparer
  /// compares equal, the second is used, and so on. This is similar to how
  /// <see cref="Enumerable.OrderBy{TSource,TKey}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,TKey})"/> and
  /// <see cref="Enumerable.ThenBy{TSource,TKey}(System.Linq.IOrderedEnumerable{TSource},System.Func{TSource,TKey})"/> work.
  /// </summary>
  public class CompoundComparer<T> : IComparer<T>
  {
    private readonly IComparer<T>[] _comparers;

    public CompoundComparer (params IComparer<T>[] comparers)
        : this((IEnumerable<IComparer<T>>)ArgumentUtility.CheckNotNull("comparers", comparers))
    {
    }

    public CompoundComparer (IEnumerable<IComparer<T>> comparers)
    {
      ArgumentUtility.CheckNotNull("comparers", comparers);

      _comparers = comparers.ToArray();
    }

    public ReadOnlyCollection<IComparer<T>> Comparers
    {
      get { return Array.AsReadOnly(_comparers); }
    }

    public int Compare (T? x, T? y)
    {
      // This is not a LINQ query for performance reasons.

      // ReSharper disable LoopCanBeConvertedToQuery
      for (int i = 0; i < _comparers.Length; ++i)
      {
        var comparisonResult = _comparers[i].Compare(x, y);
        if (comparisonResult != 0)
          return comparisonResult;
      }
      return 0;
      // ReSharper restore LoopCanBeConvertedToQuery
    }
  }
}
