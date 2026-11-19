// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Remotion.Web.Development.WebTesting;

/// <summary>
/// Represents a collection of web testing features that can be access by type.
/// Features can be any reference type, but are usually services that provide functionality.
/// </summary>
/// <threadsafety static="true" instance="true" />
/// <remarks>
/// Items in the collection are owned by the collection.
/// Thus, any items in the collection that implement <see cref="IDisposable"/> will be disposed when the collection is disposed.
/// </remarks>
public class WebTestFeatureCollection : IReadOnlyWebTestFeatureCollection, IDisposable
{
  private readonly ConcurrentDictionary<Type, object> _features;

  public WebTestFeatureCollection ()
  {
    _features = new ConcurrentDictionary<Type, object>();
  }

  public WebTestFeatureCollection (IReadOnlyWebTestFeatureCollection features)
  {
    ArgumentNullException.ThrowIfNull(features);

    _features = new ConcurrentDictionary<Type, object>(features);
  }

  /// <summary>
  /// Constructor for the unit tests. Does not check that type and instances match
  /// </summary>
  internal WebTestFeatureCollection (IEnumerable<(Type, object)> items)
  {
    _features = new ConcurrentDictionary<Type, object>(items.Select(e => new KeyValuePair<Type, object>(e.Item1, e.Item2)));
  }

  /// <summary>
  /// Removes the feature of type <typeparamref name="TFeature"/> from the collection.
  /// </summary>
  public void Remove<TFeature> ()
      where TFeature : class
  {
    _features.Remove(typeof(TFeature), out _);
  }

  /// <summary>
  /// Adds the specified <paramref name="feature"/> for the feature type <typeparamref name="TFeature"/> to the collection.
  /// Overrides an already existing feature in the collection.
  /// </summary>
  public void Set<TFeature> (TFeature feature)
      where TFeature : class
  {
    ArgumentNullException.ThrowIfNull(feature);

    _features[typeof(TFeature)] = feature;
  }

  /// <summary>
  /// Tries to add the specified <paramref name="feature"/> for the feature type <typeparamref name="TFeature"/> to the collection.
  /// Returns if the add operation has succeeded and the wasn't a feature for type <typeparamref name="TFeature"/> registered.
  /// </summary>
  public bool TryAdd<TFeature> (TFeature feature)
      where TFeature : class
  {
    ArgumentNullException.ThrowIfNull(feature);

    return _features.TryAdd(typeof(TFeature), feature);
  }

  /// <inheritdoc />
  public bool TryGet<TFeature> ([NotNullWhen(true)] out TFeature? result)
      where TFeature : class
  {
    if (_features.TryGetValue(typeof(TFeature), out var value))
    {
      result = (TFeature)value;
      return true;
    }

    result = null;
    return false;
  }

  public void InitializeFeatures ()
  {
    var exceptions = new List<Exception>();
    foreach (var feature in _features.Values.OfType<ILifecycleWebTestFeature>())
    {
      try
      {
        feature.Initialize();
      }
      catch (Exception ex)
      {
        exceptions.Add(ex);
      }
    }

    if (exceptions.Count != 0)
      throw new AggregateException("One or more features failed to initialize.", exceptions);
  }

  public void Dispose ()
  {
    foreach (var feature in _features.Values)
    {
      if (feature is IDisposable disposableFeature)
        disposableFeature.Dispose();
    }
  }

  public IEnumerator<KeyValuePair<Type, object>> GetEnumerator () => _features.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
}
