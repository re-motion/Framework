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
  /// <summary>
  /// This class implements a cache that does not actually cache anything.
  /// </summary>
  /// <remarks>
  /// Use NullCache objects if some code expects an <see cref="ICache{TKey,TValue}"/> interface, but you don't actually want to use caching.
  /// </remarks>
  [Serializable]
  public sealed class NullCache<TKey, TValue> : ICache<TKey, TValue>
      where TKey: notnull
  {
    public NullCache ()
    {
    }

    public bool TryGetValue (TKey key, [AllowNull, MaybeNullWhen(false)] out TValue value)
    {
      value = default(TValue)!;
      return false;
    }

    public TValue GetOrCreateValue (TKey key, Func<TKey,TValue> valueFactory)
    {
      ArgumentUtility.CheckNotNull("valueFactory", valueFactory);
      return valueFactory(key);
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator ()
    {
      return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
    }

    public void Clear ()
    {
    }

    bool INullObject.IsNull
    {
      get { return true; }
    }
  }
}
