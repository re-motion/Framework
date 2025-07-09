// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System.Collections.Immutable;

namespace Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting;

/// <summary>
/// Represents typed settings for running docker containers.
/// Use <see cref="CustomArguments"/> to pass custom arguments if they are not supported as property.
/// </summary>
public record DockerRunSettings
{
  /// <summary>
  /// Overrides the default arguments (CMD) of the image. Can be <see langword="null" />.
  /// </summary>
  public string? Args { get; init; }

  /// <summary>
  /// The custom arguments passed directly to the docker executable. Can be <see langword="null" />.
  /// </summary>
  public string? CustomArguments { get; init; }

  /// <summary>
  /// The environment variables that should be used when running the docker image.
  /// </summary>
  public ImmutableDictionary<string, string> EnvironmentVariables { get; init; } = ImmutableDictionary<string, string>.Empty;

  /// <summary>
  /// Overrides the default entry point of the image. Can be <see langword="null" />.
  /// </summary>
  public string? EntryPoint { get; init; }

  /// <summary>
  /// The hostname to associate with the container. Can be <see langword="null" />.
  /// </summary>
  public string? Hostname { get; init; }

  /// <summary>
  /// The name and tag of the image to use.
  /// </summary>
  public required string ImageName { get; init; }

  /// <summary>
  /// The isolation mode that should be used when running the docker image. Can be <see langword="null" />.
  /// </summary>
  public string? IsolationMode { get; init; }

  /// <summary>
  /// The directories to mount, the keys represent the paths on the host system, the values represent the path in the container.
  /// </summary>
  public ImmutableDictionary<string, string> Mounts { get; init; } = ImmutableDictionary<string, string>.Empty;

  /// <summary>
  /// The ports to publish, the keys represent the ports on the host system, the values represent the ports in the container.
  /// </summary>
  public ImmutableDictionary<int, int> Ports { get; init; } = ImmutableDictionary<int, int>.Empty;

  /// <summary>
  /// Indicates whether the container should be removed upon stopping.
  /// </summary>
  public bool Remove { get; init; }

  /// <summary>
  /// Overrides the default working directory. Can be <see langword="null" />.
  /// </summary>
  public string? WorkingDirectory { get; init; }
}
