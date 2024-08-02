// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.IO;
using Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting;

namespace Remotion.Web.Development.WebTesting.HostingStrategies;

/// <summary>
/// Represents an ASP.NET Core Docker container and manages its lifecycle.
/// </summary>
public class AspNetDockerContainerWrapper : DockerContainerWrapperBase
{
  private readonly string _processPath;

  public AspNetDockerContainerWrapper (IDockerClient docker, AspNetDockerContainerConfigurationParameters configurationParameters)
      : base(docker, configurationParameters)
  {
    if (configurationParameters.ProcessPath == null)
    {
      throw new ArgumentException(
          "The executable's path was not set while trying to host with ASP.NET Core. Please add the 'processPath' attribute to the 'testSiteLayout' configuration.");
    }

    _processPath = configurationParameters.ProcessPath;
  }

  protected override string GetEntryPoint ()
  {
    return Path.GetFullPath(Path.Combine(ConfigurationParameters.AbsoluteWebApplicationPath, _processPath));
  }

  protected override string? GetArguments ()
  {
    return null;
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
