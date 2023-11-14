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
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chrome;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Edge;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Firefox;

namespace Remotion.Web.Development.WebTesting.WebDriver
{
  /// <summary>
  /// Various extension methods for the <see cref="IBrowserConfiguration"/> interface and Coypu's <see cref="Browser"/> class.
  /// </summary>
  public static class BrowserConfigurationExtensions
  {
    /// <summary>
    /// Gets a flag indicating if <paramref name="browserConfiguration"/> represents <b>Chrome</b> by testing if the passed object 
    /// implements <see cref="IChromeConfiguration"/>.
    /// </summary>
    public static bool IsChrome ([NotNull] this IBrowserConfiguration browserConfiguration)
    {
      ArgumentUtility.CheckNotNull("browserConfiguration", browserConfiguration);

      return browserConfiguration is IChromeConfiguration;
    }

    /// <summary>
    /// Gets a flag indicating if <paramref name="browserConfiguration"/> represents <b>Edge</b> by testing if the passed object
    /// implements <see cref="IEdgeConfiguration"/>.
    /// </summary>
    public static bool IsEdge ([NotNull] this IBrowserConfiguration browserConfiguration)
    {
      ArgumentUtility.CheckNotNull("browserConfiguration", browserConfiguration);

      return browserConfiguration is IEdgeConfiguration;
    }

    /// <summary>
    /// Gets a flag indicating if <paramref name="browserConfiguration"/> represents a <b>Chromium browser</b> by testing if the passed object 
    /// implements either <see cref="IChromeConfiguration"/> or <see cref="IEdgeConfiguration"/>.
    /// </summary>
    public static bool IsChromium ([NotNull] this IBrowserConfiguration browserConfiguration)
    {
      ArgumentUtility.CheckNotNull("browserConfiguration", browserConfiguration);

      return browserConfiguration is IChromeConfiguration || browserConfiguration is IEdgeConfiguration;
    }

    /// <summary>
    /// Gets a flag indicating if <paramref name="browserConfiguration"/> represents <b>Firefox</b> by testing if the passed object 
    /// implements <see cref="IFirefoxConfiguration"/>.
    /// </summary>
    public static bool IsFirefox ([NotNull] this IBrowserConfiguration browserConfiguration)
    {
      ArgumentUtility.CheckNotNull("browserConfiguration", browserConfiguration);

      return browserConfiguration is IFirefoxConfiguration;
    }

    /// <summary>
    /// Gets a flag indicating if <paramref name="browser"/> represents <b>Chrome</b> by testing if the passed instance 
    /// is <see cref="Browser.Chrome"/>.
    /// </summary>
    public static bool IsChrome ([NotNull] this Browser browser)
    {
      ArgumentUtility.CheckNotNull("browser", browser);

      return browser == Browser.Chrome;
    }

    /// <summary>
    /// Gets a flag indicating if <paramref name="browser"/> represents <b>Edge</b> by testing if the passed instance
    /// is <see cref="Browser.Edge"/>.
    /// </summary>
    public static bool IsEdge ([NotNull] this Browser browser)
    {
      ArgumentUtility.CheckNotNull("browser", browser);

      return browser == Browser.Edge;
    }

    /// <summary>
    /// Gets a flag indicating if <paramref name="browser"/> represents a <b>Chromium browser</b> by testing if the passed instance
    /// is <see cref="Browser.Chrome"/> or <seealso cref="Browser.Edge"/>.
    /// </summary>
    public static bool IsChromium ([NotNull] this Browser browser)
    {
      ArgumentUtility.CheckNotNull("browser", browser);

      return browser == Browser.Chrome || browser == Browser.Edge;
    }

    /// <summary>
    /// Gets a flag indicating if <paramref name="browser"/> represents <b>Firefox</b> by testing if the passed instance 
    /// is <see cref="Browser.Firefox"/>.
    /// </summary>
    public static bool IsFirefox ([NotNull] this Browser browser)
    {
      ArgumentUtility.CheckNotNull("browser", browser);

      return browser == Browser.Firefox;
    }
  }
}
