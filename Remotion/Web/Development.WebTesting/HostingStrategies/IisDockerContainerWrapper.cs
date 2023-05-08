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
using System.Threading;
using JetBrains.Annotations;
using log4net;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting;

namespace Remotion.Web.Development.WebTesting.HostingStrategies
{
  /// <summary>
  /// Represents an IIS Docker container and manages its lifecycle.
  /// </summary>
  public class IisDockerContainerWrapper : IDisposable
  {
    private static readonly ILog s_log = LogManager.GetLogger(typeof(IisDockerContainerWrapper));

    private readonly IDockerClient _docker;
    private readonly IisDockerContainerConfigurationParameters _configurationParameters;
    private string? _containerName;

    public IisDockerContainerWrapper (
        [NotNull] IDockerClient docker,
        [NotNull] IisDockerContainerConfigurationParameters configurationParameters)
    {
      ArgumentUtility.CheckNotNull("docker", docker);
      ArgumentUtility.CheckNotNull("configurationParameters", configurationParameters);

      _docker = docker;
      _configurationParameters = configurationParameters;
    }

    /// <summary>
    /// Pulls and starts the configured docker image as a container with the settings specified in the App.config.
    /// </summary>
    public void Run ()
    {
      try
      {
        _docker.Pull(_configurationParameters.DockerImageName);
      }
      catch (DockerOperationException ex)
      {
        s_log.Error($"Pulling the docker image '{_configurationParameters.DockerImageName}' failed. Trying to proceed with a locally cached image.", ex);
      }

      var mounts = GetMountsWithWebApplicationPath(_configurationParameters.Mounts);
      var portMappings = new Dictionary<int, int> { { _configurationParameters.WebApplicationPort, _configurationParameters.WebApplicationPort } };

      _containerName = _docker.Run(
          portMappings,
          mounts,
          _configurationParameters.DockerImageName,
          _configurationParameters.DockerIsolationMode,
          _configurationParameters.Hostname,
          true,
          "powershell",
          $@"-Command ""
C:\Windows\System32\inetsrv\appcmd.exe set apppool /apppool.name:DefaultAppPool /enable32bitapponwin64:{_configurationParameters.Is32BitProcess};
C:\Windows\System32\inetsrv\appcmd.exe add site /name:WebTestingSite /physicalPath:""{_configurationParameters.AbsoluteWebApplicationPath}"" /bindings:""http://*:{_configurationParameters.WebApplicationPort}"";
C:\ServiceMonitor.exe w3svc;
""");
    }

    /// <inheritdoc />
    public void Dispose ()
    {
      Assertion.DebugIsNotNull(_containerName, "No container was started.");
      _docker.Stop(_containerName);
      var isContainerRemovedAfterStop = IsContainerRemoved(retries: 15, interval: TimeSpan.FromMilliseconds(100));

      if (isContainerRemovedAfterStop)
        return;

      Assertion.DebugIsNotNull(_containerName, "No container was started.");
      _docker.Remove(_containerName, true);
      var isContainerRemovedAfterForceRemove = IsContainerRemoved(retries: 15, interval: TimeSpan.FromMilliseconds(100));

      if (isContainerRemovedAfterForceRemove)
        return;

      throw new InvalidOperationException($"The container with the id '{_containerName}' could not be removed.");
    }

    private bool IsContainerRemoved (int retries, TimeSpan interval)
    {
      Assertion.DebugIsNotNull(_containerName, "No container was started.");

      for (var i = 0; i <= retries; i++)
      {
        if (!_docker.ContainerExists(_containerName))
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
                           _configurationParameters.AbsoluteWebApplicationPath,
                           _configurationParameters.AbsoluteWebApplicationPath
                       }
                   };

      foreach (var mount in additionalMounts)
      {
        var absoluteMountPath = Path.Combine(_configurationParameters.AbsoluteWebApplicationPath, mount);
        mounts.Add(absoluteMountPath, absoluteMountPath);
      }

      return mounts;
    }
  }
}
