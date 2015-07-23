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
using Coypu.Drivers;

namespace Remotion.Web.Development.WebTesting.Configuration
{
  /// <summary>
  /// Provides all the necessary information to initialize an shut down a browser session.
  /// </summary>
  public interface IBrowserConfiguration
  {
    /// <summary>
    /// Browser (as Coypu Browser object).
    /// </summary>
    Browser Browser { get; }

    /// <summary>
    /// Browser in which the web tests are run.
    /// </summary>
    string BrowserName { get; }

    /// <summary>
    /// Absolute or relative path to the logs directory. Some web driver implementations write log files for debugging reasons.
    /// </summary>
    string LogsDirectory { get; }

    /// <summary>
    /// Some Selenium web driver implementations may become confused when searching for windows if there are other browser windows present. Typically
    /// you want to turn this auto-close option on when running web tests, on developer machines, however, this may unexpectedly close important
    /// browser windows, which is why the default value is set to <see langword="false" />.
    /// </summary>
    bool CloseBrowserWindowsOnSetUpAndTearDown { get; }

    /// <summary>
    /// Returns whether the <see cref="Browser"/> is set to <see cref="Coypu.Drivers.Browser.InternetExplorer"/>.
    /// </summary>
    bool BrowserIsInternetExplorer ();

    /// <summary>
    /// Returns the process name of the configured browser.
    /// </summary>
    /// <returns>The process name without the file extension.</returns>
    string GetBrowserExecutableName ();

    /// <summary>
    /// Returns the process name of the configured browser's web driver implementation.
    /// </summary>
    /// <returns>The process name without the file extension.</returns>
    string GetWebDriverExecutableName ();
  }
}