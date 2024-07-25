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
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.HostingStrategies.Configuration
{
  /// <summary>
  /// Represents a test site.
  /// </summary>
  public interface ITestSiteLayoutConfiguration
  {
    /// <summary>
    /// Gets the absolute path to the test site hosted when running web tests.
    /// </summary>
    [NotNull]
    string RootPath { get; }

    /// <summary>
    /// Gets the runtime dependencies of the test site.
    /// </summary>
    [NotNull]
    IReadOnlyList<ITestSiteResource> Resources { get; }

    /// <summary>
    /// Gets the path to the test site's executable. The path may be relative to the <see cref="RootPath"/>. Used with Kestrel-based hosting.
    /// </summary>
    string? ProcessPath { get; }
  }
}
