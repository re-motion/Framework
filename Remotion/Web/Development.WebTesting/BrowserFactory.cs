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
using System.Configuration;
using Coypu;
using Coypu.Drivers;
using Coypu.Drivers.Selenium;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Configuration;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Factory to create Coypu <see cref="BrowserSession"/> objects from a given <see cref="WebTestingConfiguration"/>.
  /// </summary>
  public static class BrowserFactory
  {
    /// <summary>
    /// Creates a Coypu <see cref="BrowserSession"/> using the configuration given by <paramref name="browserConfiguration"/>.
    /// </summary>
    /// <param name="browserConfiguration">The browser configuration to use.</param>
    /// /// <param name="coypuConfiguration">The Coypu configuration to use.</param>
    /// <returns>A new Coypu <see cref="BrowserSession"/>.</returns>
    public static BrowserSession CreateBrowser (
        [NotNull] IBrowserConfiguration browserConfiguration,
        [NotNull] ICoypuConfiguration coypuConfiguration)
    {
      ArgumentUtility.CheckNotNull ("browserConfiguration", browserConfiguration);
      ArgumentUtility.CheckNotNull ("coypuConfiguration", coypuConfiguration);

      var sessionConfiguration = new SessionConfiguration
                                 {
                                     Browser = browserConfiguration.Browser,
                                     RetryInterval = coypuConfiguration.RetryInterval,
                                     Timeout = coypuConfiguration.SearchTimeout,
                                     ConsiderInvisibleElements = WebTestingConstants.ShouldConsiderInvisibleElements,
                                     Match = WebTestingConstants.DefaultMatchStrategy,
                                     TextPrecision = WebTestingConstants.DefaultTextPrecision
                                 };

      if (sessionConfiguration.Browser == Browser.Chrome)
      {
        // Todo RM-6337: Switch back to always using the default Chrome driver as soon as the ActaNova language problem has been solved.
        if (ConfigurationManager.AppSettings["CustomChromeUserDataDir"] != null)
        {
          sessionConfiguration.Driver = typeof (CustomChromeDriver);
          return new BrowserSession (sessionConfiguration, new CustomChromeDriver());
        }

        sessionConfiguration.Driver = typeof (SeleniumWebDriver);
        return new BrowserSession (sessionConfiguration);
      }

      if (sessionConfiguration.Browser == Browser.InternetExplorer)
      {
        sessionConfiguration.Driver = typeof (CustomInternetExplorerDriver);
        return new BrowserSession (sessionConfiguration, new CustomInternetExplorerDriver());
      }

      throw new NotSupportedException (string.Format ("Only browsers '{0}' and '{1}' are supported.", Browser.Chrome, Browser.InternetExplorer));
    }
  }
}