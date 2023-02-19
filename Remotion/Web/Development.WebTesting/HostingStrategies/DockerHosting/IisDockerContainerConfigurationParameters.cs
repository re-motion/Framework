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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting
{
  /// <summary>
  /// Contains the parameters used by <see cref="IisDockerContainerWrapper"/> while preparing and starting the Docker container.
  /// </summary>
  public class IisDockerContainerConfigurationParameters
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

    public IisDockerContainerConfigurationParameters (
        [NotNull] string absoluteWebApplicationPath,
        int webApplicationPort,
        [NotNull] string dockerImageName,
        [CanBeNull] string? dockerIsolationMode,
        [CanBeNull] string? hostname,
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
}
