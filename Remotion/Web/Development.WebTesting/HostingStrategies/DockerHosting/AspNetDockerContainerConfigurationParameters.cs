// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting;

/// <summary>
/// Contains the parameters used by <see cref="AspNetDockerContainerWrapper"/> while preparing and starting the Docker container.
/// </summary>
public class AspNetDockerContainerConfigurationParameters : DockerContainerConfigurationParameters
{
  public string? ProcessPath { get; set; }

  public AspNetDockerContainerConfigurationParameters (
      string absoluteWebApplicationPath,
      int webApplicationPort,
      string dockerImageName,
      string? dockerIsolationMode,
      string? hostname,
      bool is32BitProcess,
      IReadOnlyCollection<string> mounts,
      string? processPath)
      : base(absoluteWebApplicationPath, webApplicationPort, dockerImageName, dockerIsolationMode, hostname, is32BitProcess, mounts)
  {
    ArgumentUtility.CheckNotEmpty("processPath", processPath);

    ProcessPath = processPath;
  }
}
