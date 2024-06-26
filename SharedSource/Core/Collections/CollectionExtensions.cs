// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;
using Remotion.Utilities;

// ReSharper disable once CheckNamespace
namespace Remotion.Collections
{
  /// <summary>
  /// Provides useful extension methods for the <see cref="IReadOnlyCollection{T}"/> interface.
  /// </summary>
  static class CollectionExtensions
  {
    public static ReadOnlyCollectionWrapper<T> AsReadOnly<T> (this IReadOnlyCollection<T> collection)
    {
      ArgumentUtility.CheckNotNull("collection", collection);

      return new ReadOnlyCollectionWrapper<T>(collection);
    }
  }
}
