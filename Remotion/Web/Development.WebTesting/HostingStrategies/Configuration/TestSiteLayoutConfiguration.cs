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
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Configuration;

namespace Remotion.Web.Development.WebTesting.HostingStrategies.Configuration
{
  /// <inheritdoc />
  public class TestSiteLayoutConfiguration : ITestSiteLayoutConfiguration
  {
    /// <inheritdoc />
    public string RootPath { get; }

    /// <inheritdoc />
    public IReadOnlyList<ITestSiteResource> Resources { get; }

    public TestSiteLayoutConfiguration ([NotNull] IWebTestConfiguration configSettings)
    {
      ArgumentUtility.CheckNotNull("configSettings", configSettings);

      RootPath = GetRootedRootPath(configSettings.WebTestSiteLayoutConfiguration.RootPath);
      Resources = configSettings.WebTestSiteLayoutConfiguration.Resources
          .Select(resourceElement => EnsureRootedPath(RootPath, resourceElement))
          .Select(rootedPath => new TestSiteResource(rootedPath)).ToArray();
    }

    private string GetRootedRootPath ([NotNull] string path)
    {
      return EnsureRootedPath(AppContext.BaseDirectory, path);
    }

    private string EnsureRootedPath ([NotNull] string rootPath, [NotNull] string path)
    {
      if (Path.IsPathRooted(path))
      {
        return Path.GetFullPath(path);
      }

      var combinedPath = Path.Combine(rootPath, path);

      return Path.GetFullPath(combinedPath);
    }
  }
}
