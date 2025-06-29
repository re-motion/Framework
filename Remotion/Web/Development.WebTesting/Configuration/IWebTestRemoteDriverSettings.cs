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
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chromium;

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
    /// If enabled, a <see cref="Url"/> to the remote endpoint must be provided.
    /// </summary>
    bool Enabled { get; }

    /// <summary>
    /// The URL to the remote endpoint that should be used for browser automation.
    /// This is usually the Selenium Grid endpoint using port 4444.
    /// </summary>
    string Url { get; }
  }
}
