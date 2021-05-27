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
using System.Collections.Generic;
using Coypu;
using JetBrains.Annotations;
using Microsoft.Edge.SeleniumTools;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// Extension methods for Coypu's <see cref="IDriver"/> interface.
  /// </summary>
  public static class DriverExtensions
  {
    private const string c_unknown = "unknown";

    /// <summary>
    /// Gets the name of the browser associated with <paramref name="driver"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> representing the browser version or <c>unknown</c> if the <paramref name="driver"/>'s type is not supported.
    /// </returns>
    public static string GetBrowserName ([NotNull] this IDriver driver)
    {
      ArgumentUtility.CheckNotNull ("driver", driver);

      switch (driver.Native)
      {
        case ChromeDriver _:
          return "Chrome";
        case EdgeDriver _:
          return "Edge";
        case FirefoxDriver _:
          return "Firefox";
        case InternetExplorerDriver _:
          return "InternetExplorer";
        default:
          return c_unknown;
      }
    }

    /// <summary>
    /// Gets the version of the browser associated with <paramref name="driver"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> representing the browser version or <c>unknown</c> if the version cannot be resolved.
    /// </returns>
    public static string GetBrowserVersion ([NotNull] this IDriver driver)
    {
      ArgumentUtility.CheckNotNull ("driver", driver);

      if (!(driver.Native is IHasCapabilities driverWithCapabilities))
        return c_unknown;

      switch (driver.Native)
      {
        case ChromeDriver _:
        case EdgeDriver _:
          return driverWithCapabilities.Capabilities.GetCapability ("version") as string ?? c_unknown;
        case FirefoxDriver _:
          return driverWithCapabilities.Capabilities.GetCapability ("browserVersion") as string ?? c_unknown;
        default:
          return c_unknown;
      }
    }

    /// <summary>
    /// Gets the version of the webdriver associated with <paramref name="driver"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> representing the webdriver version or <c>unknown</c> if the version cannot be resolved.
    /// </returns>
    public static string GetWebDriverVersion ([NotNull] this IDriver driver)
    {
      ArgumentUtility.CheckNotNull ("driver", driver);

      if (!(driver.Native is IHasCapabilities driverWithCapabilities))
        return c_unknown;

      switch (driver.Native)
      {
        case ChromeDriver _ when driverWithCapabilities.Capabilities.GetCapability ("chrome") is Dictionary<string, object> capabilities
                                 && capabilities.TryGetValue ("chromedriverVersion", out var driverVersion):
          return driverVersion as string ?? c_unknown;
        case EdgeDriver _ when driverWithCapabilities.Capabilities.GetCapability ("msedge") is Dictionary<string, object> capabilities
                               && capabilities.TryGetValue ("msedgedriverVersion", out var driverVersion):
          return driverVersion as string ?? c_unknown;
        case FirefoxDriver _:
          return driverWithCapabilities.Capabilities.GetCapability ("moz:geckodriverVersion") as string ?? c_unknown;
        default:
          return c_unknown;
      }
    }
  }
}