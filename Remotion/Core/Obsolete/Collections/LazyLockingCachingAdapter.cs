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

namespace Remotion.Collections
{
  [Obsolete("Dummy declaration for DependDB. Moved to Remotion.Collections.Caching.dll", true)]
  public class LazyLockingCachingAdapter<TKey, TValue> : ICache<TKey, TValue>
      where TValue : class
  {
    public class Wrapper
    {
      public readonly TValue Value;

      public Wrapper (TValue value)
      {
        throw new NotImplementedException();
      }
    }

    public LazyLockingCachingAdapter (ICache<TKey, Lazy<Wrapper>> innerCache)
    {
      throw new NotImplementedException();
    }

    public bool IsNull
    {
      get { throw new NotImplementedException(); }
    }

    public TValue GetOrCreateValue (TKey key, Func<TKey, TValue> valueFactory)
    {
      throw new NotImplementedException();
    }

    private Wrapper GetOrCreateValueWithClosure (TKey key, Func<TKey, TValue> valueFactory)
    {
      throw new NotImplementedException();
    }

    public bool TryGetValue (TKey key, out TValue value)
    {
      throw new NotImplementedException();
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator ()
    {
      throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      throw new NotImplementedException();
    }

    private IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator ()
    {
      throw new NotImplementedException();
    }

    public void Clear ()
    {
      throw new NotImplementedException();
    }
  }
}
