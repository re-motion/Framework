// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using Coypu.Drivers;
using Remotion.Web.Development.WebTesting.Configuration;

namespace Remotion.Web.Development.WebTesting.WebDriver.Configuration.Remote;

/// <summary>
/// Represents configuration for remote browser sessions.
/// </summary>
public interface IRemoteBrowserConfiguration : IBrowserConfiguration
{
  /// <summary>
  /// Retrieves settings related to the remote driver hosting and connection.
  /// </summary>
  IWebTestRemoteDriverSettings RemoteDriverSettings { get; }

  /// <summary>
  /// The browser that the remote driver connects to and automates.
  /// </summary>
  Browser Browser { get; }
}
