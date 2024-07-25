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

    public string? ProcessPath { get; }

    public TestSiteLayoutConfiguration ([NotNull] IWebTestSettings webTestSettings)
    {
      ArgumentUtility.CheckNotNull("webTestSettings", webTestSettings);

      RootPath = GetRootedRootPath(webTestSettings.TestSiteLayout.RootPath);
      Resources = webTestSettings.TestSiteLayout.Resources
          .Select(resourceElement => EnsureRootedPath(RootPath, resourceElement))
          .Select(rootedPath => new TestSiteResource(rootedPath)).ToArray();

      ProcessPath = GetRootedProcessPathOrNull(RootPath, webTestSettings.TestSiteLayout.ProcessPath);
    }

    private string? GetRootedProcessPathOrNull (string rootPath, string? processPath)
    {
      if (processPath == null)
        return null;
      if (!processPath.EndsWith(".exe"))
        throw new ArgumentException("The 'processPath' defined in the 'testSiteLayout' did not end with '.exe'. The path must lead to an executable.");

      return EnsureRootedPath(rootPath, processPath);
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
