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
using Remotion.Utilities;

namespace Remotion.Collections
{
  /// <summary>
  /// The <see cref="TimeSpanBasedExpirationPolicy{TValue}"/> handles values which can be expire based on <see cref="TimeSpan"/> periods.
  /// </summary>
  public class TimeSpanBasedExpirationPolicy<TValue> : IExpirationPolicy<TValue, DateTime, DateTime>
  {
    private readonly TimeSpan _period;
    private readonly IUtcNowProvider _utcNowProvider;

    public TimeSpanBasedExpirationPolicy (TimeSpan period, IUtcNowProvider utcNowProvider)
    {
      ArgumentUtility.CheckNotNull ("utcNowProvider", utcNowProvider);

      _period = period;
      _utcNowProvider = utcNowProvider;
    }

    public DateTime GetNextScanInfo ()
    {
      return _utcNowProvider.UtcNow + _period;
    }

    public DateTime GetExpirationInfo (TValue value)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      return _utcNowProvider.UtcNow + _period;
    }

    public bool IsExpired (TValue value, DateTime expirationInfo)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckNotNull ("expirationInfo", expirationInfo);

      return expirationInfo <= _utcNowProvider.UtcNow;
    }

    public bool ShouldScanForExpiredItems (DateTime nextScanInfo)
    {
      return nextScanInfo <= _utcNowProvider.UtcNow;
    }
  }
}