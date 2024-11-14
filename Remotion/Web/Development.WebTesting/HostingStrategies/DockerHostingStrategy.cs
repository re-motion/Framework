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
using System.Configuration;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
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
    private readonly DockerContainerWrapperBase _dockerContainerWrapper;

    public DockerHostingStrategy (DockerContainerWrapperBase dockerContainerWrapper)
    {
      ArgumentUtility.CheckNotNull("dockerContainerWrapper", dockerContainerWrapper);
      _dockerContainerWrapper = dockerContainerWrapper;
    }

    /// <summary>
    /// Constructor required for direct usage in <see cref="WebTestConfigurationSection"/>.
    /// </summary>
    /// <param name="testSiteLayoutConfiguration">The configuration of the used test site.</param>
    /// <param name="properties">The configuration properties.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> used when creating an <see cref="ILogger"/>.</param>
    [UsedImplicitly]
    public DockerHostingStrategy (
        [NotNull] ITestSiteLayoutConfiguration testSiteLayoutConfiguration,
        [NotNull] IReadOnlyDictionary<string, string> properties,
        [NotNull] ILoggerFactory loggerFactory)
    {
      ArgumentUtility.CheckNotNull(nameof(testSiteLayoutConfiguration), testSiteLayoutConfiguration);
      ArgumentUtility.CheckNotNull(nameof(properties), properties);
      ArgumentUtility.CheckNotNull("loggerFactory", loggerFactory);

      var port = int.Parse(properties["port"]);
      var dockerImageName = properties["dockerImageName"];
      var dockerIsolationMode = properties["dockerIsolationMode"];
      var dockerPullTimeout = TimeSpan.Parse(properties["dockerPullTimeout"]);
      var hostname = properties["hostname"];
      var innerType = properties.GetValueOrDefault("innerType");

      var absoluteWebApplicationPath = testSiteLayoutConfiguration.RootPath;
      var is32BitProcess = !Environment.Is64BitProcess;

      var docker = new DockerCommandLineClient(dockerPullTimeout, loggerFactory);

      var resources = testSiteLayoutConfiguration.Resources.Select(resource => resource.Path);
      var mounts = resources
          .Select(path => GetAbsolutePath(path, absoluteWebApplicationPath))
          .Distinct(StringComparer.OrdinalIgnoreCase)
          .ToArray();

      _dockerContainerWrapper = innerType switch
      {
        null => ThrowNoInnerTypeException(),
        _ when innerType.Equals("iisExpress", StringComparison.OrdinalIgnoreCase) => CreateIisDockerContainer(),
        _ when innerType.Equals("aspnetcore", StringComparison.OrdinalIgnoreCase) => CreateAspNetCoreDockerContainer(),
        _ => ThrowNoInnerTypeException()
      };

      DockerContainerWrapperBase CreateIisDockerContainer ()
      {
        var configurationParameters = new DockerContainerConfigurationParameters(
            absoluteWebApplicationPath,
            port,
            dockerImageName,
            dockerIsolationMode,
            hostname,
            is32BitProcess,
            mounts);

        return new IisDockerContainerWrapper(docker, configurationParameters, loggerFactory);
      }

      DockerContainerWrapperBase CreateAspNetCoreDockerContainer ()
      {
        var configurationParameters = new AspNetDockerContainerConfigurationParameters(
            absoluteWebApplicationPath,
            port,
            dockerImageName,
            dockerIsolationMode,
            hostname,
            is32BitProcess,
            mounts,
            testSiteLayoutConfiguration.ProcessPath);

        return new AspNetDockerContainerWrapper(docker, configurationParameters, loggerFactory);
      }

      DockerContainerWrapperBase ThrowNoInnerTypeException ()
      {
        throw new ConfigurationErrorsException($"Could not resolve inner Type '{innerType}'. (Possible values: 'iisExpress' or 'aspnetcore')");
      }
    }

    /// <inheritdoc />
    public void DeployAndStartWebApplication ()
    {
      _dockerContainerWrapper.Run();
    }

    /// <inheritdoc />
    public void StopAndUndeployWebApplication ()
    {
      _dockerContainerWrapper.Dispose();
    }

    private static string GetAbsolutePath (string path, string workingDirectory)
    {
      if (Path.IsPathRooted(path))
        return Path.GetFullPath(path);

      var absolutePath = Path.Combine(workingDirectory, path);

      return Path.GetFullPath(absolutePath);
    }
  }
}
