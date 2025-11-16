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
    /// <param name="settings">Settings for starting the docker container.</param>
    /// <returns>The container ID of the started container.</returns>
    /// <exception cref="DockerOperationException">The issued docker operation failed.</exception>
    /// <exception cref="TimeoutException">The issued docker command took longer than the set timeout.</exception>
    /// <exception cref="FileNotFoundException">The docker executable was not found.</exception>
    [NotNull]
    string Run (DockerRunSettings settings);

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
