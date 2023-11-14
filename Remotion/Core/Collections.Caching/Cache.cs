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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Remotion.Utilities;

namespace Remotion.Collections.Caching
{
  /// <summary>
  /// Implements the <see cref="ICache{TKey,TValue}"/> interface to provide a simple, dictionary-based cache for values.
  /// </summary>
  /// <typeparam name="TKey">The type of the keys.</typeparam>
  /// <typeparam name="TValue">The type of the values.</typeparam>
  /// <remarks>
  /// It is recommended to use the <see cref="CacheFactory"/> type to create instances of the desired cache implementation 
  /// (e.g. thread-safe implementations, implementations with support for data invalidation, etc).
  /// </remarks>
  /// <threadsafety static="true" instance="false" />
  [Serializable]
  public sealed class Cache<TKey, TValue> : ICache<TKey, TValue>
      where TKey: notnull
  {
    private sealed class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
    {
      private readonly IEnumerator<KeyValuePair<TKey, Data>> _inner;

      public Enumerator (IEnumerator<KeyValuePair<TKey, Data>> inner)
      {
        _inner = inner;
      }

      public bool MoveNext ()
      {
        if (!_inner.MoveNext())
          return false;
        if (_inner.Current.Value.IsInitialized)
          return true;
        return MoveNext();
      }

      public void Reset ()
      {
        _inner.Reset();
      }

      public KeyValuePair<TKey, TValue> Current
      {
        get
        {
          var current = _inner.Current;
          var data = current.Value;
          Assertion.DebugAssert(data.IsInitialized, "Uninitialized values should be skipped during MoveNext().");
          return new KeyValuePair<TKey, TValue>(current.Key, data.Value);
        }
      }

      object IEnumerator.Current
      {
        get { return Current; }
      }

      public void Dispose ()
      {
        _inner.Dispose();
      }
    }

    [Serializable]
    private struct Data
    {
      public readonly TValue Value;
      public readonly bool IsInitialized;

      public Data (TValue value)
      {
        Value = value;
        IsInitialized = true;
      }
    }

    private readonly Dictionary<TKey, Data> _innerDictionary;

    public Cache ()
    {
      _innerDictionary = new Dictionary<TKey, Data>();
    }

    public Cache ([JetBrains.Annotations.NotNull] IEqualityComparer<TKey> comparer)
    {
      ArgumentUtility.CheckNotNull("comparer", comparer);

      _innerDictionary = new Dictionary<TKey, Data>(comparer);
    }

    public IEqualityComparer<TKey> Comparer
    {
      get { return _innerDictionary.Comparer; }
    }

    public bool TryGetValue (TKey key, [AllowNull, MaybeNullWhen(false)] out TValue value)
    {
      ArgumentUtility.DebugCheckNotNull("key", key);

      return TryGetValueInternal(key, out value);
    }

    public TValue GetOrCreateValue (TKey key, Func<TKey,TValue> valueFactory)
    {
      ArgumentUtility.DebugCheckNotNull("key", key);
      ArgumentUtility.DebugCheckNotNull("valueFactory", valueFactory);

      if (!TryGetValueInternal(key, out var value))
      {
        ArgumentUtility.CheckNotNull("valueFactory", valueFactory);

        _innerDictionary.Add(key, new Data());
        try
        {
          value = valueFactory(key);
        }
        catch
        {
          _innerDictionary.Remove(key);
          throw;
        }
        var data = new Data(value);
        if (_innerDictionary.Remove(key))
          _innerDictionary.Add(key, data);
      }

      return value;
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator ()
    {
      return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    private IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator ()
    {
      return new Enumerator(_innerDictionary.GetEnumerator());
    }

    public void Clear ()
    {
      _innerDictionary.Clear();
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryGetValueInternal (TKey key, [AllowNull, MaybeNullWhen(false)] out TValue value)
    {
      Data data;
      var hasData = _innerDictionary.TryGetValue(key, out data);

      value = data.Value;

      if (data.IsInitialized)
        return true;

      if (!hasData)
        return false;

      // Note: JIT32 (x86 platform) will not inline methods that contain throw-statements. This will incur a 10% performance penalty 
      // for this particular method. By moving the throw-statement to the helper method, this penalty could be avoided but the readability 
      // would suffer. Given that a) RyuJIT is expected to take over for JIT32 in a future release and that the x86 platform is not used for 
      // high-scale (web server) applications any longer, this optimization is skipped at this point.
      throw CreateExceptionRecursiveKeyAccess(key);
    }

    /// <remarks>
    /// Method must be extracted into separate, non-inlined method because string-Format would otherwise incur a 20% performance overhead.
    /// </remarks>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static InvalidOperationException CreateExceptionRecursiveKeyAccess (TKey key)
    {
      return new InvalidOperationException(
          string.Format(
              "An attempt was detected to access the value for key ('{0}') during the factory operation of GetOrCreateValue(key, factory).",
              key));
    }
  }
}
