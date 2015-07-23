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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// Various extension methods for <see cref="IEnumerable{T}"/>.
  /// </summary>
  public static class EnumerableExtensions
  {
    /// <summary>
    /// Returns the index of the first item in <paramref name="enumerable"/> matching the given <paramref name="predicate"/>.
    /// </summary>
    /// <remarks>
    /// This method enumerates the <paramref name="enumerable"/>.
    /// </remarks>
    /// <returns>The index of the matching item, or -1 if no item matches.</returns>
    public static int IndexOf<T> ([NotNull] this IEnumerable<T> enumerable, [NotNull] Func<T, bool> predicate)
    {
      ArgumentUtility.CheckNotNull ("enumerable", enumerable);
      ArgumentUtility.CheckNotNull ("predicate", predicate);

      var index = 0;
      foreach (var item in enumerable)
      {
        if (predicate (item))
          return index;

        index++;
      }

      return -1;
    }
  }
}