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
namespace Remotion.Web.Resources
{
  /// <summary>
  /// Defines an API for retrieving the cache key for static resources.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  public interface IStaticResourceCacheKeyProvider : INullObject
  {
    /// <summary>
    /// Returns the cache key for static resources, or <see langword="null"/> if caching is not enabled.
    /// </summary>
    /// <returns>A non-empty cache key consisting of alphanumeric characters or underscores ([a-zA-Z0-9_]+), <see langword="null"/> if caching is not enabled.</returns>
    string? GetStaticResourceCacheKey ();
  }
}
