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
  [Obsolete ("Dummy declaration for DependDB. Moved to Remotion.Collections.DataStore.dll", true)]
  public class NullDataStore<TKey, TValue> : IDataStore<TKey, TValue>
  {
    public static readonly NullDataStore<TKey, TValue> Instance = null;

    private NullDataStore()
    {
      throw new NotImplementedException();
    }

    public bool ContainsKey (TKey key)
    {
      throw new NotImplementedException();
    }

    public void Add (TKey key, TValue value)
    {
      throw new NotImplementedException();
    }

    public bool Remove (TKey key)
    {
      throw new NotImplementedException();
    }

    public void Clear ()
    {
      throw new NotImplementedException();
    }

    public TValue this [TKey key]
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public TValue GetValueOrDefault (TKey key)
    {
      throw new NotImplementedException();
    }

    public bool TryGetValue (TKey key, out TValue value)
    {
      throw new NotImplementedException();
    }

    public TValue GetOrCreateValue (TKey key, Func<TKey, TValue> valueFactory)
    {
      throw new NotImplementedException();
    }

    public bool IsNull
    {
      get { throw new NotImplementedException(); }
    }
  }
}
