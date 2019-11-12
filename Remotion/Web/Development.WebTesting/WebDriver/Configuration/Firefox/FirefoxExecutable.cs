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

namespace Remotion.Web.Development.WebTesting.WebDriver.Configuration.Firefox
{
  /// <summary>
  /// Contains the information for customizing a Firefox browser instance.
  /// </summary>
  public class FirefoxExecutable
  {
    /// <summary>
    /// Path to the Firefox browser binary.
    /// </summary>
    [NotNull]
    public string BrowserBinaryPath { get; }

    /// <summary>
    /// Path to the Firefox driver binary.
    /// </summary>
    [NotNull]
    public string DriverBinaryPath { get; }

    public FirefoxExecutable ([NotNull] string browserBinaryPath, [NotNull] string driverBinaryPath)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("browserBinaryPath", browserBinaryPath);
      ArgumentUtility.CheckNotNullOrEmpty ("driverBinaryPath", driverBinaryPath);

      BrowserBinaryPath = browserBinaryPath;
      DriverBinaryPath = driverBinaryPath;
    }
  }
}