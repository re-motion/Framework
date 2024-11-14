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
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting
{
  /// <summary>
  /// Provides an interface for communicating with a Docker client.
  /// </summary>
  public interface IDockerClient
  {
    /// <summary>
    /// Pulls the given docker image.
    /// </summary>
    /// <exception cref="DockerOperationException">The issued docker operation failed.</exception>
    /// <exception cref="TimeoutException">The issued docker command took longer than the set timeout.</exception>
    /// <exception cref="FileNotFoundException">The docker executable was not found.</exception>
    void Pull ([NotNull] string imageName);

    /// <summary>
    /// Runs the docker image with the given settings.
    /// </summary>
    /// <param name="ports">The ports to publish, the keys represent the ports on the host system, the values represent the ports in the container.</param>
    /// <param name="mounts">The directories to mount, the keys represent the paths on the host system, the values represent the path in the container.</param>
    /// <param name="environmentVariables">The environment variables that should be used when running the docker image.</param>
    /// <param name="imageName">The name and tag of the image to use.</param>
    /// <param name="isolationMode">The isolation mode that should be used when running the docker image. Can be <see langword="null" />.</param>
    /// <param name="hostname">The hostname to associate with the container. Can be <see langword="null" />.</param>
    /// <param name="remove">Indicates whether the container should be removed upon stopping.</param>
    /// <param name="entryPoint">Overrides the default entry point of the image. Can be <see langword="null" />.</param>
    /// <param name="workingDirectory">Overrides the default working directory. Can be <see langword="null" />.</param>
    /// <param name="args">Overrides the default arguments (CMD) of the image. Can be <see langword="null" />.</param>
    /// <returns>The container ID of the started container.</returns>
    /// <exception cref="DockerOperationException">The issued docker operation failed.</exception>
    /// <exception cref="TimeoutException">The issued docker command took longer than the set timeout.</exception>
    /// <exception cref="FileNotFoundException">The docker executable was not found.</exception>
    [NotNull]
    string Run (
        [NotNull] IDictionary<int, int> ports,
        [NotNull] IDictionary<string, string> mounts,
        [NotNull] IDictionary<string, string> environmentVariables,
        [NotNull] string imageName,
        [CanBeNull] string? isolationMode,
        [CanBeNull] string? hostname,
        bool remove,
        [CanBeNull] string? entryPoint,
        [CanBeNull] string? workingDirectory,
        [CanBeNull] string? args);

    /// <summary>
    /// Checks if a container with the specified ID exists.
    /// </summary>
    /// <returns>True if the container exists, false otherwise.</returns>
    bool ContainerExists ([NotNull] string containerName);

    /// <summary>
    /// Removes a container with the given ID.
    /// </summary>
    /// <exception cref="DockerOperationException">The issued docker operation failed.</exception>
    /// <exception cref="TimeoutException">The issued docker command took longer than the set timeout.</exception>
    /// <exception cref="FileNotFoundException">The docker executable was not found.</exception>
    void Remove ([NotNull] string containerName, bool force = false);

    /// <summary>
    /// Stops a container with the given ID.
    /// </summary>
    /// <exception cref="DockerOperationException">The issued docker operation failed.</exception>
    /// <exception cref="TimeoutException">The issued docker command took longer than the set timeout.</exception>
    /// <exception cref="FileNotFoundException">The docker executable was not found.</exception>
    void Stop ([NotNull] string containerName);
  }
}
