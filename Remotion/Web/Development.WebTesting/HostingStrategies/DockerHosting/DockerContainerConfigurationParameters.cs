// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System.Collections.Generic;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting;

/// <summary>
/// Contains the parameters used by <see cref="DockerContainerWrapperBase"/> while preparing and starting the Docker container.
/// </summary>
public class DockerContainerConfigurationParameters
{
  /// <summary>
  /// Gets the path to the web application project that should be hosted.
  /// </summary>
  [NotNull]
  public string AbsoluteWebApplicationPath { get; }

  /// <summary>
  /// Gets the port by which the container can be accessed.
  /// </summary>
  public int WebApplicationPort { get; }

  /// <summary>
  /// Gets the name of the image used for starting the container.
  /// </summary>
  [NotNull]
  public string DockerImageName { get; }

  /// <summary>
  /// Gets the isolation mode for the container, or <see langword="null"/> if the default isolation mode should be used.
  /// </summary>
  public string? DockerIsolationMode { get; }

  /// <summary>
  /// Gets the hostname by which the container can be accessed.
  /// </summary>
  [CanBeNull]
  public string? Hostname { get; }

  /// <summary>
  /// Gets a flag indicating whether the application should be hosted in 32-bit mode.
  /// </summary>
  public bool Is32BitProcess { get; }

  /// <summary>
  /// Gets the directories that should be mounted in the container.
  /// </summary>
  [NotNull]
  public IReadOnlyCollection<string> Mounts { get; }

  public DockerContainerConfigurationParameters (
      string absoluteWebApplicationPath,
      int webApplicationPort,
      string dockerImageName,
      string? dockerIsolationMode,
      string? hostname,
      bool is32BitProcess,
      IReadOnlyCollection<string> mounts)
  {
    ArgumentUtility.CheckNotNullOrEmpty("absoluteWebApplicationPath", absoluteWebApplicationPath);
    ArgumentUtility.CheckNotNullOrEmpty("dockerImageName", dockerImageName);
    ArgumentUtility.CheckNotEmpty("hostname", hostname);
    ArgumentUtility.CheckNotNull("mounts", mounts);

    AbsoluteWebApplicationPath = absoluteWebApplicationPath;
    WebApplicationPort = webApplicationPort;
    DockerImageName = dockerImageName;
    DockerIsolationMode = dockerIsolationMode;
    Hostname = hostname;
    Is32BitProcess = is32BitProcess;
    Mounts = mounts;
  }
}
