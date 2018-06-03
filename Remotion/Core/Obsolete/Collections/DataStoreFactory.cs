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

namespace Remotion.Collections
{
  [Obsolete ("Dummy declaration for DependDB. Moved to Remotion.Collections.DataStore.dll", true)]
  public static class DataStoreFactory
  {
    public static IDataStore<TKey, TValue> Create<TKey, TValue> ()
    {
      throw new NotImplementedException();
    }

    public static IDataStore<TKey, TValue> Create<TKey, TValue> (IEqualityComparer<TKey> comparer)
    {
      throw new NotImplementedException();
    }

    public static IDataStore<TKey, TValue> CreateWithSynchronization<TKey, TValue> ()
    {
      throw new NotImplementedException();
    }

    public static IDataStore<TKey, TValue> CreateWithSynchronization<TKey, TValue> (IEqualityComparer<TKey> comparer)
    {
      throw new NotImplementedException();
    }

    [Obsolete ("Use CreateWithSynchronization(...) instead. (Version: 1.19.3)")]
    public static IDataStore<TKey, TValue> CreateWithLocking<TKey, TValue> ()
    {
      throw new NotImplementedException();
    }

    [Obsolete ("Use CreateWithSynchronization(...) instead. (Version: 1.19.3)")]
    public static IDataStore<TKey, TValue> CreateWithLocking<TKey, TValue> ( IEqualityComparer<TKey> comparer)
    {
      throw new NotImplementedException();
    }

    [Obsolete ("Use CreateWithSynchronization(...) instead. (Version: 1.19.3)")]
    public static IDataStore<TKey, TValue> CreateWithLazyLocking<TKey, TValue> () where TValue: class
    {
      throw new NotImplementedException();
    }

    [Obsolete ("Use CreateWithSynchronization(...) instead. (Version: 1.19.3)")]
    public static IDataStore<TKey, TValue> CreateWithLazyLocking<TKey, TValue> ( IEqualityComparer<TKey> comparer) where TValue: class
    {
      throw new NotImplementedException();
    }
  }
}