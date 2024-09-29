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
using Microsoft.Extensions.Logging;
using OpenQA.Selenium.Chrome;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.BrowserSession;
using Remotion.Web.Development.WebTesting.BrowserSession.Chrome;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chrome;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chromium;

namespace Remotion.Web.Development.WebTesting.WebDriver.Factories.Chrome
{
  /// <summary>
  /// Responsible for creating a new Chrome browser, configured based on <see cref="IChromeConfiguration"/> and <see cref="DriverConfiguration"/>.
  /// </summary>
  public class ChromeBrowserFactory : IBrowserFactory
  {
    private readonly IChromeConfiguration _chromeConfiguration;
    private readonly IBrowserSessionCleanUpStrategy _registryCleanUpStrategy;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger _logger;

    public ChromeBrowserFactory ([NotNull] IChromeConfiguration chromeConfiguration, [NotNull] ILoggerFactory loggerFactory)
    {
      ArgumentUtility.CheckNotNull("chromeConfiguration", chromeConfiguration);
      ArgumentUtility.CheckNotNull("loggerFactory", loggerFactory);

      _loggerFactory = loggerFactory;
      _logger = _loggerFactory.CreateLogger(typeof(ChromeBrowserFactory));
      _chromeConfiguration = chromeConfiguration;
      _registryCleanUpStrategy = ChromiumSecurityWarningsRegistryCleanUpStrategyFactory.CreateForChrome(_chromeConfiguration.DisableSecurityWarningsBehavior);
    }

    public IBrowserSession CreateBrowser (DriverConfiguration configuration)
    {
      ArgumentUtility.CheckNotNull("configuration", configuration);

      var sessionConfiguration = CreateSessionConfiguration(configuration);
      var commandTimeout = configuration.CommandTimeout;

      var extendedChromeOptions = _chromeConfiguration.CreateChromeOptions();
      if (configuration.Headless)
        extendedChromeOptions.AddArgument("headless=new");

      var driver = CreateChromeDriver(extendedChromeOptions, out var driverProcessID, commandTimeout);
      driver.Manage().Timeouts().AsynchronousJavaScript = configuration.AsyncJavaScriptTimeout;

      var session = new Coypu.BrowserSession(sessionConfiguration, new CustomSeleniumWebDriver(driver, Browser.Chrome));

      return new ChromeBrowserSession(
          session,
          _chromeConfiguration,
          driverProcessID,
          configuration.Headless,
          new[]
          {
              _registryCleanUpStrategy,
              new ChromiumUserDirectoryCleanUpStrategy(
                  _chromeConfiguration.UserDirectoryRoot,
                  extendedChromeOptions.UserDirectory!,
                  _loggerFactory.CreateLogger<ChromeBrowserSession>())
          });
    }

    private SessionConfiguration CreateSessionConfiguration (DriverConfiguration configuration)
    {
      return new SessionConfiguration
             {
                 Browser = Browser.Chrome,
                 RetryInterval = configuration.RetryInterval,
                 Timeout = configuration.SearchTimeout,
                 ConsiderInvisibleElements = WebTestingConstants.ShouldConsiderInvisibleElements,
                 Match = WebTestingConstants.DefaultMatchStrategy,
                 TextPrecision = WebTestingConstants.DefaultTextPrecision,
                 Driver = typeof(CustomSeleniumWebDriver)
             };
    }

    private ChromeDriver CreateChromeDriver (ExtendedChromeOptions extendedChromeOptions, out int driverProcessID, TimeSpan commandTimeout)
    {
      var driverService = CreateChromeDriverService();

      var driver = new ChromeDriver(driverService, extendedChromeOptions, commandTimeout);
      driverProcessID = driverService.ProcessId;

      return driver;
    }

    private ChromeDriverService CreateChromeDriverService ()
    {
      var driverDirectory = Path.GetDirectoryName(_chromeConfiguration.DriverBinaryPath);
      var driverExecutable = Path.GetFileName(_chromeConfiguration.DriverBinaryPath);

      var driverService = ChromeDriverService.CreateDefaultService(driverDirectory, driverExecutable);

      driverService.EnableVerboseLogging = false;
      driverService.LogPath = WebDriverLogUtility.CreateLogFile(_chromeConfiguration.LogsDirectory, _chromeConfiguration.BrowserName, _logger);

      return driverService;
    }
  }
}
