// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;

namespace Remotion.Web.Development.WebTesting;

/// <summary>
/// Provides convenience extension methods for <see cref="IReadOnlyWebTestFeatureCollection"/>.
/// </summary>
public static class ReadOnlyWebTestFeatureCollectionExtensions
{
  /// <summary>
  /// Returns if the specified <typeparamref name="TFeature"/> is contained in the collection.
  /// To retrieve the value in one go, use <see cref="IReadOnlyWebTestFeatureCollection.TryGet{TFeature}"/>.
  /// </summary>
  public static bool Contains<TFeature> (this IReadOnlyWebTestFeatureCollection features)
      where TFeature : class
  {
    ArgumentNullException.ThrowIfNull(features);

    return features.TryGet<TFeature>(out _);
  }

  /// <summary>
  /// Retrieves the specified <typeparamref name="TFeature"/> from the collection.
  /// Throws an exception if the specified <typeparamref name="TFeature"/> is not found.
  /// </summary>
  /// <exception cref="KeyNotFoundException">A feature of the specified type <typeparamref name="TFeature"/> was not found.</exception>
  public static TFeature Get<TFeature> (this IReadOnlyWebTestFeatureCollection features)
      where TFeature : class
  {
    ArgumentNullException.ThrowIfNull(features);

    return features.TryGet<TFeature>(out var feature)
        ? feature
        : throw new KeyNotFoundException($"The specified feature '{typeof(TFeature)}' was not found in the feature collection.");
  }

  /// <summary>
  /// Retrieves the specified <typeparamref name="TFeature"/> from the collection.
  /// Returns <see langword="null"/> if feature was not found.
  /// </summary>
  public static TFeature? GetOrDefault<TFeature> (this IReadOnlyWebTestFeatureCollection features)
      where TFeature : class
  {
    ArgumentNullException.ThrowIfNull(features);

    return features.TryGet<TFeature>(out var feature)
        ? feature
        : null;
  }
}
