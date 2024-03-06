﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
#if !NET48
using System;

namespace Remotion.Web.Resources;

/// <summary>
/// Caching options for serving static files, creating a Cache-Control header.
/// Create a subclass to extend/override the Cache-Control header generation.
/// </summary>
public class StaticFileCachingOptions
{
  /// <summary>
  /// The max age of the response before the cache discards it. (Default: 1 week)
  /// Will be rounded down to the nearest second.
  /// </summary>
  public TimeSpan MaxAge { get; init; } = TimeSpan.FromDays(7);

  /// <summary>
  /// Returns a value for the Cache-Control header derived from the current options.
  /// </summary>
  public virtual string GetCacheControlHeader ()
  {
    return $"public, max-age={GetMaxAgeInSeconds()}";
  }

  protected double GetMaxAgeInSeconds ()
  {
    return Math.Floor(MaxAge.TotalSeconds);
  }
}
#endif
