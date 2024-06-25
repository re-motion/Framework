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

namespace Remotion.Collections.DataStore
{
  /// <summary>
  /// Implements the <see cref="IDataStore{TKey,TValue}"/> interface as a simple, not thread-safe in-memory data store based on a 
  /// <see cref="Dictionary{TKey,TValue}"/>.
  /// </summary>
  /// <typeparam name="TKey">The type of the keys.</typeparam>
  /// <typeparam name="TValue">The type of the values.</typeparam>
  public sealed class SimpleDataStore<TKey, TValue> : IDataStore<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>
      where TKey : notnull
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

    public SimpleDataStore ()
    {
      _innerDictionary = new Dictionary<TKey, Data>();
    }

    public SimpleDataStore ([JetBrains.Annotations.NotNull] IEqualityComparer<TKey> comparer)
    {
      ArgumentUtility.CheckNotNull("comparer", comparer);

      _innerDictionary = new Dictionary<TKey, Data>(comparer);
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }

    public IEqualityComparer<TKey> Comparer
    {
      get { return _innerDictionary.Comparer; }
    }

    /// <inheritdoc />
    public bool ContainsKey (TKey key)
    {
      ArgumentUtility.CheckNotNull("key", key);

      return TryGetValueInternal(key, out _);
    }

    /// <inheritdoc />
    public void Add (TKey key, TValue value)
    {
      ArgumentUtility.CheckNotNull("key", key);
      // value can be null

      if (TryGetValueInternal(key, out _))
        throw new ArgumentException(string.Format("The store already contains an element with key '{0}'.", key), "key");

      _innerDictionary.Add(key, new Data(value));
    }

    /// <inheritdoc />
    public bool Remove (TKey key)
    {
      ArgumentUtility.CheckNotNull("key", key);

      if (TryGetValueInternal(key, out _))
        return _innerDictionary.Remove(key);

      return false;
    }

    /// <inheritdoc />
    public void Clear ()
    {
      _innerDictionary.Clear();
    }

    /// <inheritdoc />
    public TValue this[TKey key]
    {
      get
      {
        ArgumentUtility.CheckNotNull("key", key);

        if (TryGetValueInternal(key, out var value))
          return value;

        string message = string.Format("There is no element with key '{0}' in the store.", key);
        throw new KeyNotFoundException(message);
      }
      set
      {
        ArgumentUtility.CheckNotNull("key", key);

        if (TryGetValueInternal(key, out _))
          _innerDictionary.Remove(key);
        _innerDictionary.Add(key, new Data(value));
      }
    }

    /// <inheritdoc />
    [return: MaybeNull]
    public TValue GetValueOrDefault (TKey key)
    {
      ArgumentUtility.DebugCheckNotNull("key", key);

      TryGetValueInternal(key, out var value);
      return value;
    }

    /// <inheritdoc />
    public bool TryGetValue (TKey key, [AllowNull, MaybeNullWhen(false)] out TValue value)
    {
      ArgumentUtility.DebugCheckNotNull("key", key);

      return TryGetValueInternal(key, out value);
    }

    /// <inheritdoc />
    public TValue GetOrCreateValue (TKey key, Func<TKey, TValue> valueFactory)
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

    /// <summary>
    /// Returns an <see cref="IEnumerator{T}"/> that iterates through the contents of this <see cref="SimpleDataStore{TKey,TValue}"/>.
    /// </summary>
    /// <returns>An <see cref="IEnumerator{T}"/> that iterates through the contents of this <see cref="SimpleDataStore{TKey,TValue}"/>.</returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator ()
    {
      return new Enumerator(_innerDictionary.GetEnumerator());
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
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
