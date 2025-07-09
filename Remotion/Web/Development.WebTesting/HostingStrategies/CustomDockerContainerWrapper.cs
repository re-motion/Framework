// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Microsoft.Extensions.Logging;
using Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting;

namespace Remotion.Web.Development.WebTesting.HostingStrategies;

/// <summary>
/// Represents a custom docker container and manages its lifetime.
/// The container is started according to the specified <see cref="DockerRunSettings"/>.
/// </summary>
public class CustomDockerContainerWrapper : DockerContainerWrapperBase
{
  public DockerRunSettings Settings { get; }

  public CustomDockerContainerWrapper (
      IDockerClient docker,
      ILoggerFactory loggerFactory,
      DockerRunSettings settings)
      : base(docker, loggerFactory)
  {
    ArgumentNullException.ThrowIfNull(docker);
    ArgumentNullException.ThrowIfNull(loggerFactory);
    ArgumentNullException.ThrowIfNull(settings);

    Settings = settings;
  }

  /// <inheritdoc />
  public override DockerRunSettings CreateDockerRunSettings ()
  {
    return Settings;
  }
}
