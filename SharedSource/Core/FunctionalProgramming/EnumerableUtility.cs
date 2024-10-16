// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;
using System.Linq;
#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.FunctionalProgramming
{
  /// <summary>
  /// Provides helper functions for <see cref="IEnumerable{T}"/> objects.
  /// </summary>
  /// <remarks>
  /// Most of these methods will become obsolete with C# 3.0/LINQ.
  /// </remarks>
  static partial class EnumerableUtility
  {
    /// <summary>
    /// Combines the specified <see cref="IEnumerable{T}"/> sequences into a single sequence.
    /// </summary>
    /// <typeparam name="T">The item type of the sequences to combine.</typeparam>
    /// <param name="sources">The source sequences to combine.</param>
    /// <returns>A single sequence yielding the items of each source sequence in the same order in which they were passed via the 
    /// <paramref name="sources"/> parameter.</returns>
    public static IEnumerable<T> Combine<T> (params IEnumerable<T>[] sources)
    {
      for (int i = 0; i < sources.Length; ++i)
      {
        foreach (T item in sources[i])
          yield return item;
      }
    }

    /// <summary>
    /// Creates an <see cref="IEnumerable{T}"/> containing <paramref name="item"/> as its single element.
    /// </summary>
    /// <typeparam name="TItem">The type of the <paramref name="item"/> element.</typeparam>
    /// <param name="item">The object to be added to the sequence. Can be <see langword="null" />.</param>
    /// <returns>A sequence containing only the <paramref name="item"/> element.</returns>
    public static IEnumerable<TItem> Singleton<TItem> (TItem item)
    {
      yield return item;
    }

    /// <summary>
    /// Recursively select items from a tree data structure defined by a start element and a child selector function. 
    /// Iterating the resulting sequence performs a depth-first traversal of the data structure.
    /// </summary>
    /// <typeparam name="T">The item type to select.</typeparam>
    /// <param name="start">The item to start with.</param>
    /// <param name="childrenSelector">A selector function that returns the child items of a given parent item.</param>
    /// <returns>A sequence containing all items in the tree in a depth-first order.</returns>
    public static IEnumerable<T> SelectRecursiveDepthFirst<T> (T start, Func<T, IEnumerable<T>> childrenSelector)
    {
      if (childrenSelector == null)
        throw new ArgumentNullException("childrenSelector");

      return Singleton(start).Concat(childrenSelector(start).SelectMany(child => SelectRecursiveDepthFirst(child, childrenSelector)));
    }
  }
}
