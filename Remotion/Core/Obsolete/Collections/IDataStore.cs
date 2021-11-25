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

namespace Remotion.Collections
{
  [Obsolete("Dummy declaration for DependDB. Moved to Remotion.Collections.DataStore.dll", true)]
  public interface IDataStore<TKey, TValue> : INullObject
  {
    bool ContainsKey (TKey key);

    void Add (TKey key,  TValue value);

    bool Remove (TKey key);

    void Clear ();

    TValue this [TKey key] { get; set; }

    TValue GetValueOrDefault (TKey key);

    bool TryGetValue (TKey key,  out TValue value);

    TValue GetOrCreateValue (TKey key, Func<TKey, TValue> valueFactory);
  }
}
