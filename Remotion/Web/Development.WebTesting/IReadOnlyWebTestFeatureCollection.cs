// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Remotion.Web.Development.WebTesting;

/// <summary>
/// Represents a read-only collection of web testing features that can be accessed by type.
/// Features can be any reference type, but are usually services that provide functionality.
/// </summary>
public interface IReadOnlyWebTestFeatureCollection : IEnumerable<KeyValuePair<Type, object>>
{
  /// <summary>
  /// Retrieves the feature of type <typeparamref name="TFeature"/> from the collection.
  /// The return value indicates whether the feature was found in the collection.
  /// </summary>
  bool TryGet<TFeature> ([NotNullWhen(true)] out TFeature? result)
      where TFeature : class;

  /// <summary>
  /// Iterates through the collection and invokes the initialization logic for each initializable feature.
  /// </summary>
  /// <exception cref="AggregateException">Thrown if one or more features fail to initialize.</exception>
  void InitializeFeatures ();
}
