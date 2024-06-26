// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
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
