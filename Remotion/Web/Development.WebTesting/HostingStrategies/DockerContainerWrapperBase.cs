// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using log4net;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting;

namespace Remotion.Web.Development.WebTesting.HostingStrategies
{
  /// <summary>
  /// Represents a Docker container base which manages the lifecycle of a docker container.
  /// </summary>
  public abstract class DockerContainerWrapperBase : IDisposable
  {
    private static readonly ILog s_log = LogManager.GetLogger(typeof(DockerContainerWrapperBase));

    protected IDockerClient Docker { get; }

    protected DockerContainerConfigurationParameters ConfigurationParameters { get; }

    protected string? ContainerName;

    protected DockerContainerWrapperBase (
        IDockerClient docker,
        DockerContainerConfigurationParameters configurationParameters)
    {
      ArgumentUtility.CheckNotNull("docker", docker);
      ArgumentUtility.CheckNotNull("configurationParameters", configurationParameters);

      Docker = docker;
      ConfigurationParameters = configurationParameters;
    }

    protected abstract string GetEntryPoint ();

    protected abstract string? GetWorkingDirectory ();

    protected abstract string? GetArguments ();

    protected virtual void SetEnvironmentVariables (IDictionary<string, string> environmentVariables)
    {
    }

    /// <summary>
    /// Pulls and starts the configured docker image as a container with the settings specified in the App.config.
    /// </summary>
    public void Run ()
    {
      try
      {
        Docker.Pull(ConfigurationParameters.DockerImageName);
      }
      catch (DockerOperationException ex)
      {
        s_log.Error($"Pulling the docker image '{ConfigurationParameters.DockerImageName}' failed. Trying to proceed with a locally cached image.", ex);
      }

      var mounts = GetMountsWithWebApplicationPath(ConfigurationParameters.Mounts);
      var portMappings = new Dictionary<int, int> { { ConfigurationParameters.WebApplicationPort, ConfigurationParameters.WebApplicationPort } };

      var environmentVariables = new Dictionary<string, string>();
      SetEnvironmentVariables(environmentVariables);

      ContainerName = Docker.Run(
          portMappings,
          mounts,
          environmentVariables,
          ConfigurationParameters.DockerImageName,
          ConfigurationParameters.DockerIsolationMode,
          ConfigurationParameters.Hostname,
          true,
          GetEntryPoint(),
          GetWorkingDirectory(),
          GetArguments());
    }

    public void Dispose ()
    {
      Assertion.DebugIsNotNull(ContainerName, "No container was started.");
      Docker.Stop(ContainerName);
      var isContainerRemovedAfterStop = IsContainerRemoved(retries: 15, interval: TimeSpan.FromMilliseconds(100));

      if (isContainerRemovedAfterStop)
        return;

      Assertion.DebugIsNotNull(ContainerName, "No container was started.");
      Docker.Remove(ContainerName, true);
      var isContainerRemovedAfterForceRemove = IsContainerRemoved(retries: 15, interval: TimeSpan.FromMilliseconds(100));

      if (isContainerRemovedAfterForceRemove)
        return;

      throw new InvalidOperationException($"The container with the id '{ContainerName}' could not be removed.");
    }

    private bool IsContainerRemoved (int retries, TimeSpan interval)
    {
      Assertion.DebugIsNotNull(ContainerName, "No container was started.");

      for (var i = 0; i <= retries; i++)
      {
        if (!Docker.ContainerExists(ContainerName))
          return true;

        Thread.Sleep((int)interval.TotalMilliseconds);
      }

      return false;
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
}
