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
using JetBrains.Annotations;
using OpenQA.Selenium.Chrome;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chromium;

namespace Remotion.Web.Development.WebTesting.WebDriver.Configuration.Edge
{
  /// <summary>
  /// Provides configuration specific to initializing and shutting down a Edge browser.
  /// </summary>
  public interface IEdgeConfiguration : IBrowserConfiguration
  {
    /// <summary>
    /// Gets the full path to msedge.exe.
    /// </summary>
    [NotNull]
    string BrowserBinaryPath { get; }

    /// <summary>
    /// Gets the full path to msedgedriver.exe.
    /// </summary>
    [NotNull]
    string DriverBinaryPath { get; }

    /// <summary>
    /// Gets the path to the user directories, which is used by the started Edge. If the path does not exists, it will be automatically created at startup.
    /// </summary>
    [NotNull]
    string UserDirectoryRoot { get; }

    /// <summary>
    /// Defines the behavior of tests depending on the <c>CommandLineFlagSecurityWarningsEnabled</c> registry flag.
    /// </summary>
    ChromiumDisableSecurityWarningsBehavior DisableSecurityWarningsBehavior { get; }

    /// <summary>
    /// Creates the <see cref="ChromeOptions"/> used when instantiating the Edge browser.
    /// </summary>
    [NotNull]
    ExtendedEdgeOptions CreateEdgeOptions ();
  }
}
