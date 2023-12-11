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
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.HostingStrategies.Configuration;
using Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting;

namespace Remotion.Web.Development.WebTesting.HostingStrategies
{
  /// <summary>
  /// Hosts the WebApplication using a Docker Container.
  /// </summary>
  public class DockerHostingStrategy : IHostingStrategy
  {
    private readonly IisDockerContainerWrapper _iisDockerContainerWrapper;

    public DockerHostingStrategy (
        [NotNull] TestSiteLayoutConfiguration testSiteLayoutConfiguration,
        int port,
        [NotNull] string dockerImageName,
        [CanBeNull] string? dockerIsolationMode,
        TimeSpan dockerPullTimeout,
        [CanBeNull] string? hostname)
    {
      ArgumentUtility.CheckNotNull("testSiteLayoutConfiguration", testSiteLayoutConfiguration);
      ArgumentUtility.CheckNotNullOrEmpty("dockerImageName", dockerImageName);
      ArgumentUtility.CheckNotEmpty("hostname", hostname);

      var absoluteWebApplicationPath = testSiteLayoutConfiguration.RootPath;
      var is32BitProcess = !Environment.Is64BitProcess;

      var docker = new DockerCommandLineClient(dockerPullTimeout);

      var resources = testSiteLayoutConfiguration.Resources.Select(resource => resource.Path);
      var mounts = resources
          .Select(path => GetAbsolutePath(path, absoluteWebApplicationPath))
          .Distinct(StringComparer.OrdinalIgnoreCase)
          .ToArray();

      var configurationParameters = new IisDockerContainerConfigurationParameters(
          absoluteWebApplicationPath,
          port,
          dockerImageName,
          dockerIsolationMode,
          hostname,
          is32BitProcess,
          mounts);

      _iisDockerContainerWrapper = new IisDockerContainerWrapper(docker, configurationParameters);
    }

    /// <summary>
    /// Constructor required for direct usage in <see cref="WebTestConfigurationSection"/>.
    /// </summary>
    /// <param name="testSiteLayoutConfiguration">The configuration of the used test site.</param>
    /// <param name="properties">The configuration properties.</param>
    [UsedImplicitly]
    public DockerHostingStrategy ([NotNull] TestSiteLayoutConfiguration testSiteLayoutConfiguration, [NotNull] NameValueCollection properties)
        : this(
            testSiteLayoutConfiguration,
            int.Parse(ArgumentUtility.CheckNotNull("properties", properties)["port"]!),
            properties["dockerImageName"]!,
            properties["dockerIsolationMode"],
            TimeSpan.Parse(properties["dockerPullTimeout"]!),
            properties["hostname"])
    {
      // TODO RM-8113: Guard used properties against null values.
    }

    /// <inheritdoc />
    public void DeployAndStartWebApplication ()
    {
      _iisDockerContainerWrapper.Run();
    }

    /// <inheritdoc />
    public void StopAndUndeployWebApplication ()
    {
      _iisDockerContainerWrapper.Dispose();
    }

    private string GetAbsolutePath (string path, string workingDirectory)
    {
      if (Path.IsPathRooted(path))
        return Path.GetFullPath(path);

      var absolutePath = Path.Combine(workingDirectory, path);

      return Path.GetFullPath(absolutePath);
    }
  }
}
