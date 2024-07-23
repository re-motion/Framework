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
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Collections.Caching
{
  public sealed class InvalidationTokenBasedCacheDecorator<TKey, TValue> : ICache<TKey, TValue>
      where TKey : notnull
  {
    private readonly ICache<TKey, TValue> _innerCache;
    private readonly InvalidationToken _invalidationToken;
    private InvalidationToken.Revision _revision;
    private readonly object _syncObject = new object();

    public InvalidationTokenBasedCacheDecorator (ICache<TKey, TValue> innerCache, InvalidationToken invalidationToken)
    {
      ArgumentUtility.CheckNotNull("innerCache", innerCache);
      ArgumentUtility.CheckNotNull("invalidationToken", invalidationToken);

      _innerCache = innerCache;
      _invalidationToken = invalidationToken;
      _revision = _invalidationToken.GetCurrent();
    }

    public InvalidationToken InvalidationToken
    {
      // ReSharper disable once InconsistentlySynchronizedField
      get { return _invalidationToken; }
    }

    public TValue GetOrCreateValue (TKey key, Func<TKey, TValue> valueFactory)
    {
      ArgumentUtility.DebugCheckNotNull("key", key);
      ArgumentUtility.DebugCheckNotNull("valueFactory", valueFactory);

      CheckRevision();
      return _innerCache.GetOrCreateValue(key, valueFactory);
    }

    public bool TryGetValue (TKey key, [AllowNull, MaybeNullWhen(false)] out TValue value)
    {
      ArgumentUtility.DebugCheckNotNull("key", key);

      if (CheckRevision())
      {
        return _innerCache.TryGetValue(key, out value);
      }
      else
      {
        value = default!;
        return false;
      }
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
      if (CheckRevision())
        return _innerCache.GetEnumerator();
      else
        return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
    }

    void ICache<TKey, TValue>.Clear ()
    {
      lock (_syncObject)
      {
        ClearInternal();
      }
    }

    bool INullObject.IsNull
    {
      get { return _innerCache.IsNull; }
    }

    private bool CheckRevision ()
    {
      // ReSharper disable InconsistentlySynchronizedField
      if (_invalidationToken.IsCurrent(_revision))
        return true;
      // ReSharper restore InconsistentlySynchronizedField

      lock (_syncObject)
      {
        // If the code enters the lock, a different thread may already have cleared the cache. 
        // After the cache was successfully cleared, the revision will be up-to-date unless there has been another change in the meantime.
        // Therefor, we can skip the cache-clear if the revision is current.
        if (_invalidationToken.IsCurrent(_revision))
          return true;

        ClearInternal();
      }

      return false;
    }

    private void ClearInternal ()
    {
      do
      {
        _revision = _invalidationToken.GetCurrent();
        _innerCache.Clear();
      } while (!_invalidationToken.IsCurrent(_revision));
    }
  }
}
