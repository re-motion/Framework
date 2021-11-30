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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Collections.DataStore
{
  /// <summary>
  /// The <see cref="ExpiringDataStore{TKey,TValue,TExpirationInfo,TScanInfo}"/> stores values that can be expire.
  /// </summary>
  public class ExpiringDataStore<TKey, TValue, TExpirationInfo, TScanInfo> : IDataStore<TKey, TValue>
      where TKey: notnull
      where TValue: notnull
  {
    private readonly SimpleDataStore<TKey, Tuple<TValue, TExpirationInfo>> _innerDataStore;
    private readonly IExpirationPolicy<TValue, TExpirationInfo, TScanInfo> _expirationPolicy;

    private TScanInfo _nextScanInfo;

    public ExpiringDataStore (
        [JetBrains.Annotations.NotNull] IExpirationPolicy<TValue, TExpirationInfo, TScanInfo> expirationPolicy,
        [JetBrains.Annotations.NotNull] IEqualityComparer<TKey> equalityComparer)
    {
      ArgumentUtility.CheckNotNull("expirationPolicy", expirationPolicy);
      ArgumentUtility.CheckNotNull("equalityComparer", equalityComparer);

      _innerDataStore = new SimpleDataStore<TKey, Tuple<TValue, TExpirationInfo>>(equalityComparer);
      _expirationPolicy = expirationPolicy;
      _nextScanInfo = _expirationPolicy.GetNextScanInfo();
    }

    public TScanInfo NextScanInfo
    {
      get { return _nextScanInfo; }
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }

    public bool ContainsKey (TKey key)
    {
      ArgumentUtility.CheckNotNull("key", key);

      return TryGetValue(key, out var dummy);
    }

    public void Add (TKey key, TValue value)
    {
      ArgumentUtility.CheckNotNull("key", key);
      ArgumentUtility.CheckNotNull("value", value);

      RemoveExpiredItems();
      AddWithoutScanning(key, value);
    }

    public bool Remove (TKey key)
    {
      ArgumentUtility.CheckNotNull("key", key);
      RemoveExpiredItems();
      return RemoveWithoutScanning(key);
    }

    public void Clear ()
    {
      _innerDataStore.Clear();
    }

    public TValue this [TKey key]
    {
      get
      {
        if (!TryGetValue(key, out var result))
          throw new KeyNotFoundException("Key not found.");
        return result;
      }
      set
      {
        RemoveExpiredItems();
        _innerDataStore[key] = Tuple.Create(value, _expirationPolicy.GetExpirationInfo(value));
      }
    }

    [return: MaybeNull]
    public TValue GetValueOrDefault (TKey key)
    {
      ArgumentUtility.CheckNotNull("key", key);

      TryGetValue(key, out var value);
      return value;
    }

    public bool TryGetValue (TKey key, [AllowNull, MaybeNullWhen(false)] out TValue value)
    {
      ArgumentUtility.CheckNotNull("key", key);

      RemoveExpiredItems();

      if (_innerDataStore.TryGetValue(key, out var valueResult))
      {
        if (!_expirationPolicy.IsExpired(valueResult.Item1, valueResult.Item2))
        {
          value = valueResult.Item1;
          return true;
        }

        RemoveWithoutScanning(key);
      }

      value = default(TValue)!;
      return false;
    }

    public TValue GetOrCreateValue (TKey key, Func<TKey, TValue> valueFactory)
    {
      ArgumentUtility.CheckNotNull("key", key);
      ArgumentUtility.CheckNotNull("valueFactory", valueFactory);

      if (!TryGetValue(key, out var value))
      {
        value = valueFactory(key);
        AddWithoutScanning(key, value);
      }

      return value;
    }

    private void AddWithoutScanning (TKey key, TValue value)
    {
      _innerDataStore.Add(key, Tuple.Create(value, _expirationPolicy.GetExpirationInfo(value)));
    }

    private bool RemoveWithoutScanning (TKey key)
    {
      return _innerDataStore.Remove(key);
    }

    private void RemoveExpiredItems ()
    {
      if (_expirationPolicy.ShouldScanForExpiredItems(_nextScanInfo))
      {
        var expiredKeys = (
            from kvp in _innerDataStore
            where _expirationPolicy.IsExpired(kvp.Value.Item1, kvp.Value.Item2)
            select kvp.Key).ToList();

        foreach (var key in expiredKeys)
          RemoveWithoutScanning(key);

        _nextScanInfo = _expirationPolicy.GetNextScanInfo();
      }
    }
  }
}
