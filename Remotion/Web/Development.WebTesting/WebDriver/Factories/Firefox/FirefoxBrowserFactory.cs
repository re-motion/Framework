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
using System.IO;
using Coypu;
using Coypu.Drivers;
using JetBrains.Annotations;
using OpenQA.Selenium.Firefox;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.BrowserSession;
using Remotion.Web.Development.WebTesting.BrowserSession.Firefox;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Firefox;

namespace Remotion.Web.Development.WebTesting.WebDriver.Factories.Firefox
{
  /// <summary>
  /// Responsible for creating a new Firefox browser, configured based on <see cref="IFirefoxConfiguration"/> and <see cref="DriverConfiguration"/>.
  /// </summary>
  public class FirefoxBrowserFactory : IBrowserFactory
  {
    private readonly IFirefoxConfiguration _firefoxConfiguration;

    public FirefoxBrowserFactory ([NotNull] IFirefoxConfiguration firefoxConfiguration)
    {
      ArgumentUtility.CheckNotNull ("firefoxConfiguration", firefoxConfiguration);

      _firefoxConfiguration = firefoxConfiguration;
    }

    /// <inheritdoc />
    public IBrowserSession CreateBrowser (DriverConfiguration driverConfiguration)
    {
      ArgumentUtility.CheckNotNull ("driverConfiguration", driverConfiguration);

      var sessionConfiguration = CreateSessionConfiguration (driverConfiguration);
      var commandTimeout = driverConfiguration.CommandTimeout;

      var firefoxDriverService = GetFirefoxDriverService();
      var driver = new FirefoxDriver (firefoxDriverService, _firefoxConfiguration.CreateFirefoxOptions(), commandTimeout);
      var session = new Coypu.BrowserSession (sessionConfiguration, new CustomSeleniumWebDriver (driver, Browser.Firefox));

      return new FirefoxBrowserSession (session, _firefoxConfiguration, firefoxDriverService.ProcessId);
    }

    private FirefoxDriverService GetFirefoxDriverService ()
    {
      var driverDirectory = Path.GetDirectoryName (_firefoxConfiguration.DriverBinaryPath);
      var driverExecutable = Path.GetFileName (_firefoxConfiguration.DriverBinaryPath);

      var firefoxDriverService = FirefoxDriverService.CreateDefaultService (driverDirectory, driverExecutable);
      firefoxDriverService.FirefoxBinaryPath = _firefoxConfiguration.BrowserBinaryPath;

      return firefoxDriverService;
    }

    private SessionConfiguration CreateSessionConfiguration (DriverConfiguration driverConfiguration)
    {
      return new SessionConfiguration
             {
                 Browser = Browser.Firefox,
                 RetryInterval = driverConfiguration.RetryInterval,
                 Timeout = driverConfiguration.SearchTimeout,
                 ConsiderInvisibleElements = WebTestingConstants.ShouldConsiderInvisibleElements,
                 Match = WebTestingConstants.DefaultMatchStrategy,
                 TextPrecision = WebTestingConstants.DefaultTextPrecision,
                 Driver = typeof (CustomSeleniumWebDriver)
             };
    }
  }
}