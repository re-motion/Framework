// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using Microsoft.Extensions.Logging;
using Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting;

namespace Remotion.Web.Development.WebTesting.HostingStrategies;

/// <summary>
/// Represents a Docker container base for web applications which manages the lifecycle of a docker container.
/// </summary>
public abstract class WebApplicationDockerContainerWrapperBase : DockerContainerWrapperBase
{
  protected DockerContainerConfigurationParameters ConfigurationParameters { get; }

  protected WebApplicationDockerContainerWrapperBase (
      IDockerClient docker,
      DockerContainerConfigurationParameters configurationParameters,
      ILoggerFactory loggerFactory)
      : base(docker, loggerFactory)
  {
    ArgumentNullException.ThrowIfNull(docker);
    ArgumentNullException.ThrowIfNull(configurationParameters);
    ArgumentNullException.ThrowIfNull(loggerFactory);

    ConfigurationParameters = configurationParameters;
  }

  protected abstract string GetEntryPoint ();

  protected abstract string? GetWorkingDirectory ();

  protected abstract string? GetArguments ();

  protected virtual void SetEnvironmentVariables (IDictionary<string, string> environmentVariables)
  {
  }

  public override DockerRunSettings CreateDockerRunSettings ()
  {
    var mounts = GetMountsWithWebApplicationPath(ConfigurationParameters.Mounts);
    var portMappings = new Dictionary<int, int> { { ConfigurationParameters.WebApplicationPort, ConfigurationParameters.WebApplicationPort } };

    var environmentVariables = new Dictionary<string, string>();
    SetEnvironmentVariables(environmentVariables);

    return new DockerRunSettings
           {
               Args = GetArguments(),
               CustomArguments = ConfigurationParameters.DockerCustomArguments,
               EnvironmentVariables = environmentVariables.ToImmutableDictionary(),
               EntryPoint = GetEntryPoint(),
               Hostname = ConfigurationParameters.Hostname,
               ImageName = ConfigurationParameters.DockerImageName,
               IsolationMode = ConfigurationParameters.DockerIsolationMode,
               Mounts = mounts.ToImmutableDictionary(),
               Ports = portMappings.ToImmutableDictionary(),
               Remove = true,
               WorkingDirectory = GetWorkingDirectory(),
           };
  }

  private Dictionary<string, string> GetMountsWithWebApplicationPath (IEnumerable<string> additionalMounts)
  {
    var mounts = new Dictionary<string, string>
                 {
                     {
                         ConfigurationParameters.AbsoluteWebApplicationPath,
                         ConfigurationParameters.AbsoluteWebApplicationPath
                     }
                 };

    foreach (var mount in additionalMounts)
    {
      var absoluteMountPath = Path.Combine(ConfigurationParameters.AbsoluteWebApplicationPath, mount);
      mounts.Add(absoluteMountPath, absoluteMountPath);
    }

    return mounts;
  }
}
