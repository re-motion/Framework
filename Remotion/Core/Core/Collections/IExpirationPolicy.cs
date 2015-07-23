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

using System;

namespace Remotion.Collections
{
  /// <summary>
  /// <see cref="IExpirationPolicy{TValue,TExpirationInfo,TScanInfo}"/> defines the API for implementations that handle value expiration. This is
  /// used by <see cref="ExpiringDataStore{TKey,TValue,TExpirationInfo,TScanInfo}"/>. 
  /// </summary>
  /// <typeparam name="TValue">The type of the values that can expire.</typeparam>
  /// <typeparam name="TExpirationInfo">The type of expiration metadata required by the concrete implementation. Implementations use expiration
  /// metadata to decide whether a value is expired.</typeparam>
  /// <typeparam name="TScanInfo">The type of scan metadata required by the concrete implementation. Implementations use scan metadata to decide
  /// whether all values should be rescanned for expiration.</typeparam>
  public interface IExpirationPolicy<TValue, TExpirationInfo, TScanInfo>
  {
    TExpirationInfo GetExpirationInfo (TValue value);
    TScanInfo GetNextScanInfo ();

    bool IsExpired (TValue value, TExpirationInfo expirationInfo);
    bool ShouldScanForExpiredItems (TScanInfo nextScanInfo);
  }
}