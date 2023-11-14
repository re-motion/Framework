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
  [Obsolete("Dummy declaration for DependDB. Moved to Remotion.Collections.DataStore.dll", true)]
  public static class ExpiringDataStoreFactory
  {
    public static ExpiringDataStore<TKey, TValue, TExpirationInfo, TScanInfo> Create<TKey, TValue, TExpirationInfo, TScanInfo> (
        IExpirationPolicy<TValue, TExpirationInfo, TScanInfo> policy,
        IEqualityComparer<TKey> comparer)
    {
      throw new NotImplementedException();
    }

    [Obsolete("Presently, there is no synchronized version of the ExpiringDataStore available. (Version: 1.19.3)")]
    public static LockingDataStoreDecorator<TKey, TValue> CreateWithLocking<TKey, TValue, TExpirationInfo, TScanInfo> (
        IExpirationPolicy<TValue, TExpirationInfo, TScanInfo> policy,
        IEqualityComparer<TKey> comparer)
    {
      throw new NotImplementedException();
    }

    [Obsolete("Presently, there is no synchronized version of the ExpiringDataStore available. (Version: 1.19.3)")]
    public static LazyLockingDataStoreAdapter<TKey, TValue> CreateWithLazyLocking<TKey, TValue, TExpirationInfo, TScanInfo> (
        IExpirationPolicy<Lazy<LazyLockingDataStoreAdapter<TKey, TValue>.Wrapper>, TExpirationInfo, TScanInfo> policy,
        IEqualityComparer<TKey> comparer)
        where TValue: class
    {
      throw new NotImplementedException();
    }
  }
}
