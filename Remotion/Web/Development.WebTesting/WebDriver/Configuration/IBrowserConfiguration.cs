﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting.DownloadInfrastructure;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chrome;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.InternetExplorer;
using Remotion.Web.Development.WebTesting.WebDriver.Factories;

namespace Remotion.Web.Development.WebTesting.WebDriver.Configuration
{
  /// <summary>
  /// Provides configuration needed to initialize and shut down the browser
  /// </summary>
  /// <seealso cref="ChromeConfiguration"/>
  /// <seealso cref="InternetExplorerConfiguration"/>
  public interface IBrowserConfiguration
  {
    /// <summary>
    /// Gets the name of the browser in which the web tests are run.
    /// </summary>
    string BrowserName { [NotNull] get; }

    /// <summary>
    /// Gets the process name of the configured browser.
    /// </summary>
    /// <returns>The process name without the file extension.</returns>
    string BrowserExecutableName { [NotNull] get; }

    /// <summary>
    /// Gets the process name of the configured browser's web driver implementation.
    /// </summary>
    /// <returns>The process name without the file extension.</returns>
    string WebDriverExecutableName { [NotNull] get; }

    /// <summary>
    /// Gets a <see cref="BrowserFactory"/> responsible for creating the browser instance.
    /// </summary>
    IBrowserFactory BrowserFactory { [NotNull] get; }

    /// <summary>
    /// Gets a <see cref="DownloadHelper" />, which provides an API to handle a file download and subsequently delete the downloaded files. 
    /// </summary>
    IDownloadHelper DownloadHelper { [NotNull] get; }

    /// <summary>
    /// Gets an absolute or relative path to the logs directory. Some web driver implementations write log files for debugging reasons.
    /// </summary>
    string LogsDirectory { [NotNull] get; }
  }
}