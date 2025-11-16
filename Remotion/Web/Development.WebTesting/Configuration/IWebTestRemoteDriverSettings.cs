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

namespace Remotion.Web.Development.WebTesting.Configuration
{
  /// <summary>
  /// Represents settings for using Selenium RemoteDriver, which connect to a remote browser to automate it, removing the
  /// need to have the browser running locally.
  /// </summary>
  public interface IWebTestRemoteDriverSettings
  {
    /// <summary>
    /// Indicates that remote driver will be used to run the web tests.
    /// If enabled, either a <see cref="Url"/> must be provided or <see cref="HostRemoteDriverInDocker"/> must be set.
    /// </summary>
    bool Enabled { get; }

    /// <summary>
    /// The URL to the remote endpoint that should be used for browser automation.
    /// This is usually the Selenium Grid endpoint using port 4444.
    /// </summary>
    string Url { get; }

    /// <summary>
    /// If set, a docker container for the required browser will be started for the web tests.
    /// A <see cref="Url"/> is still required for the connection.
    /// </summary>
    /// <remarks>
    /// A default docker image using the Selenium standalone images will be used.
    /// If necessary, use <see cref="DockerImageName"/> to override the docker image name.
    /// The docker image will be started without any parameters (e.g. without port forwarding).
    /// Use <see cref="DockerCustomArguments"/> to customize the docker run invocation.
    /// </remarks>
    bool HostRemoteDriverInDocker { get; }

    /// <summary>
    /// Overrides the default docker image name used to host the browser executable.
    /// Only used if <see cref="HostRemoteDriverInDocker"/> is set.
    /// </summary>
    /// <remarks>
    /// As the docker image usually contains the browser name and/or the browser version,
    /// the placeholders are available and will be replaced at runtime:
    /// <list type="table">
    /// <item>
    /// <term>{browsername}</term>
    /// <description>The lowercase browser name (e.g. chrome, edge, firefox)</description>
    /// </item>
    /// <item>
    /// <term>{browsermajor}</term>
    /// <description>The majorversion of the browser used (e.g. 114)</description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <example>
    /// For example, the default value:
    /// <code>
    /// selenium/standalone-{browsername}:{browsermajor}.0
    /// </code>
    /// </example>
    string? DockerImageName { get; }

    /// <summary>
    /// Custom arguments passed when starting the docker image that hosts the browser executable.
    /// Only used if <see cref="HostRemoteDriverInDocker"/> is set.
    /// </summary>
    string? DockerCustomArguments { get; }
  }
}
