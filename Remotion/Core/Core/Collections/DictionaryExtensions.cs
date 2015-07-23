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
using System.Collections.ObjectModel;
using Remotion.Utilities;

namespace Remotion.Collections
{
  /// <summary>
  /// Provides useful extension methods for <see cref="IDictionary{TKey,TValue}"/>.
  /// </summary>
  public static class DictionaryExtensions
  {
    public static TValue GetValueOrDefault<TKey, TValue> (this IDictionary<TKey, TValue> dictionary, TKey key)
    {
      ArgumentUtility.CheckNotNull ("dictionary", dictionary);
      // Implementations of IDictionary<TKey, TValue> are free to allow null keys.

      return GetValueOrDefault (dictionary, key, default (TValue));
    }

    public static TValue GetValueOrDefault<TKey, TValue> (this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
    {
      ArgumentUtility.CheckNotNull ("dictionary", dictionary);
      // Implementations of IDictionary<TKey, TValue> are free to allow null keys.
      // Default value may be null.

      TValue value;
      if (dictionary.TryGetValue (key, out value))
        return value;
      else
        return defaultValue;
    }

    public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue> (this IDictionary<TKey, TValue> dict)
    {
      ArgumentUtility.CheckNotNull ("dict", dict);

      return new ReadOnlyDictionary<TKey, TValue> (dict);
    }
  }
}