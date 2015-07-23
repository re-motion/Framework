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
using Remotion.Utilities;

namespace Remotion.Collections
{
  [Serializable]
  public sealed class InvalidationTokenBasedCacheDecorator<TKey, TValue> : ICache<TKey, TValue>
  {
    private readonly ICache<TKey, TValue> _innerCache;
    private readonly InvalidationToken _invalidationToken;
    private InvalidationToken.Revision _revision;

    public InvalidationTokenBasedCacheDecorator (ICache<TKey, TValue> innerCache, InvalidationToken invalidationToken)
    {
      ArgumentUtility.CheckNotNull ("innerCache", innerCache);
      ArgumentUtility.CheckNotNull ("invalidationToken", invalidationToken);

      _innerCache = innerCache;
      _invalidationToken = invalidationToken;
      _revision = _invalidationToken.GetCurrent();
    }

    public InvalidationToken InvalidationToken
    {
      get { return _invalidationToken; }
    }

    public TValue GetOrCreateValue (TKey key, Func<TKey, TValue> valueFactory)
    {
      ArgumentUtility.DebugCheckNotNull ("key", key);
      ArgumentUtility.DebugCheckNotNull ("valueFactory", valueFactory);

      CheckRevision();
      return _innerCache.GetOrCreateValue (key, valueFactory);
    }

    public bool TryGetValue (TKey key, out TValue value)
    {
      ArgumentUtility.DebugCheckNotNull ("key", key);

      CheckRevision();
      return _innerCache.TryGetValue (key, out value);
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
      CheckRevision();
      return _innerCache.GetEnumerator();
    }

    void ICache<TKey, TValue>.Clear ()
    {
      _innerCache.Clear();
      _revision = _invalidationToken.GetCurrent();
    }

    bool INullObject.IsNull
    {
      get { return _innerCache.IsNull; }
    }

    private void CheckRevision ()
    {
      if (!_invalidationToken.IsCurrent (_revision))
      {
        _innerCache.Clear();
        _revision = _invalidationToken.GetCurrent();
      }
    }
  }
}