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
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.WebDriver.Configuration.Edge
{
  /// <summary>
  /// Contains the information required for customizing an Edge instance.
  /// </summary>
  public class EdgeExecutable
  {
    /// <summary>
    /// Gets the full path to msedge.exe.
    /// </summary>
    [NotNull]
    public string BrowserBinaryPath { get; }

    /// <summary>
    /// Gets the full path to msedgedriver.exe.
    /// </summary>
    [NotNull]
    public string DriverBinaryPath { get; }

    /// <summary>
    /// Gets the path to the user directory used by Edge for saving custom user data.
    /// </summary>
    [NotNull]
    public string UserDirectory { get; }

    /// <param name="browserBinaryPath">Path to the Microsoft Edge executable</param>
    /// <param name="driverBinaryPath">Path to the MSEdgeDriver executable</param>
    /// <param name="userDirectory">Path to the desired Microsoft Edge user data directory</param>
    public EdgeExecutable ([NotNull] string browserBinaryPath, [NotNull] string driverBinaryPath, [NotNull] string userDirectory)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("browserBinaryPath", browserBinaryPath);
      ArgumentUtility.CheckNotNullOrEmpty ("driverBinaryPath", driverBinaryPath);
      ArgumentUtility.CheckNotNullOrEmpty ("userDirectory", userDirectory);

      BrowserBinaryPath = browserBinaryPath;
      DriverBinaryPath = driverBinaryPath;
      UserDirectory = userDirectory;
    }
  }
}