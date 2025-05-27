// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting;

namespace Remotion.Web.Development.WebTesting.HostingStrategies;

/// <summary>
/// Represents an ASP.NET Core Docker container and manages its lifecycle.
/// </summary>
public class AspNetDockerContainerWrapper : DockerContainerWrapperBase
{
  private readonly string _processPath;
  private readonly string? _arguments;

  public AspNetDockerContainerWrapper (IDockerClient docker, AspNetDockerContainerConfigurationParameters configurationParameters, ILoggerFactory loggerFactory)
      : base(docker, configurationParameters, loggerFactory)
  {
    if (configurationParameters.ProcessPath == null)
    {
      throw new ArgumentException(
          "The executable's path was not set while trying to host with ASP.NET Core. Please add the 'processPath' attribute to the 'testSiteLayout' configuration.");
    }

    _processPath = configurationParameters.ProcessPath;
    _arguments = configurationParameters.Arguments;
  }

  protected override string GetEntryPoint ()
  {
    return _processPath;
  }

  protected override string? GetArguments ()
  {
    return _arguments;
  }

  protected override string? GetWorkingDirectory ()
  {
    return Path.GetFullPath(ConfigurationParameters.AbsoluteWebApplicationPath);
  }

  protected override void SetEnvironmentVariables (IDictionary<string, string> environmentVariables)
  {
    environmentVariables.Add("ASPNETCORE_URLS", $"http://{ConfigurationParameters.Hostname}.local:{ConfigurationParameters.WebApplicationPort}");
  }
}
