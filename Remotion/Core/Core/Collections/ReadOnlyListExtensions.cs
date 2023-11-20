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
  /// Provides useful extension methods for <see cref="IReadOnlyList{T}"/>.
  /// </summary>
  public static class ReadOnlyListExtensions
  {
    /// <summary>
    /// Searches for the specified <paramref name="value"/> and returns the index of its
    /// first occurence in <paramref name="list"/>.
    /// </summary>
    /// <param name="list">The list to search <paramref name="value"/> in. Must not be <see langword="null"/>.</param>
    /// <param name="value">The value to search for.</param>
    /// <returns>The index of the first occurence of <paramref name="value"/> in <paramref name="list"/>, or <c>-1</c> if not found.</returns>
    public static int IndexOf<T> (this IReadOnlyList<T> list, T value)
    {
      ArgumentUtility.CheckNotNull("list", list);

      for (var i = 0; i < list.Count; i++)
      {
        var item = list[i];
        if (item == null)
        {
          if (value == null)
            return i;
        }
        else
        {
          if (item.Equals(value))
            return i;
        }
      }

      return -1;
    }
  }
}
