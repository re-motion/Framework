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

namespace Remotion.Security
{
  /// <summary>
  /// Provides extension methods for the <see cref="AccessType"/> type.
  /// </summary>
  public static class AccessTypeExtensions
  {
    /// <summary>
    /// Checks if all items in the <paramref name="subSet"/> are contained within the <paramref name="otherSet"/>
    /// </summary>
    /// <returns>
    /// <see langword="true" /> if all items from the <paramref name="subSet"/> are found, otherwise <see langword="false" />. 
    /// An empty <paramref name="subSet"/> always results in <see langword="true" />.
    /// </returns>
    public static bool IsSubsetOf ([NotNull] this IReadOnlyList<AccessType> subSet, [NotNull] IReadOnlyList<AccessType> otherSet)
    {
      ArgumentUtility.CheckNotNull ("subSet", subSet);
      ArgumentUtility.CheckNotNull ("otherSet", otherSet);

      // This section is performance critical. No closure should be created, therefor converting this code to Linq is not possible.
      // return subSet.All (accessType => otherSet.Contains (accessType));
      // ReSharper disable LoopCanBeConvertedToQuery
      // ReSharper disable ForCanBeConvertedToForeach
      for (int i = 0; i < subSet.Count; i++)
      {
        if (!otherSet.Contains (subSet[i]))
          return false;
      }
      return true;
      // ReSharper restore ForCanBeConvertedToForeach
      // ReSharper restore LoopCanBeConvertedToQuery
    }

    /// <summary>
    /// Checks if the <paramref name="item"/> is contained within the <paramref name="set"/>
    /// </summary>
    /// <returns>
    /// <see langword="true" /> if the <paramref name="item"/> was found, otherwise <see langword="false" />. 
    /// </returns>
    public static bool Contains (this IReadOnlyList<AccessType> set, AccessType item)
    {
      ArgumentUtility.CheckNotNull ("set", set);

      // This section is performance critical. No closure should be created, therefor converting this code to Linq is not possible.
      // return set.Any (t => accessType.Equals (t));
      // ReSharper disable LoopCanBeConvertedToQuery
      // ReSharper disable ForCanBeConvertedToForeach
      for (int i = 0; i < set.Count; i++)
      {
        if (item.Equals (set[i]))
          return true;
      }
      return false;
      // ReSharper restore ForCanBeConvertedToForeach
      // ReSharper restore LoopCanBeConvertedToQuery
    }
  }
}