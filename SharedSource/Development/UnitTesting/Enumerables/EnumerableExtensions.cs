// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Utilities;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTesting.Enumerables
{
  /// <summary>
  /// Provides extensions methods for <see cref="IEnumerable{T}"/>.
  /// </summary>
  public static partial class EnumerableExtensions
  {
    /// <summary>
    /// Wraps an <see cref="IEnumerable{T}"/> to ensure that it is iterated only once.
    /// </summary>
    /// <typeparam name="T">The element type of the <see cref="IEnumerable{T}"/>.</typeparam>
    /// <param name="source">The source <see cref="IEnumerable{T}"/> to be wrapped.</param>
    /// <returns>An instance of <see cref="OneTimeEnumerable{T}"/> decorating the <paramref name="source"/>.</returns>
    public static OneTimeEnumerable<T> AsOneTime<T> (this IEnumerable<T> source)
    {
      ArgumentUtility.CheckNotNull("source", source);

      return new OneTimeEnumerable<T>(source);
    }

    /// <summary>
    /// Forces the enumeration of the <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The element type of the <see cref="IEnumerable{T}"/>.</typeparam>
    /// <param name="source">The source <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An array containing all values computed by <paramref name="source"/>.</returns>
    public static T[] ForceEnumeration<T> (this IEnumerable<T> source)
    {
      ArgumentUtility.CheckNotNull("source", source);

      return source.ToArray();
    }
  }
}
