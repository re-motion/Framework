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
using OpenQA.Selenium.Edge;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.BrowserSession;
using Remotion.Web.Development.WebTesting.BrowserSession.Edge;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chromium;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Edge;

namespace Remotion.Web.Development.WebTesting.WebDriver.Factories.Edge
{
  /// <summary>
  /// Responsible for creating a new Edge browser, configured based on <see cref="IEdgeConfiguration"/> and <see cref="DriverConfiguration"/>.
  /// </summary>
  public class EdgeBrowserFactory : IBrowserFactory
  {
    private readonly IEdgeConfiguration _edgeConfiguration;
    private readonly IBrowserSessionCleanUpStrategy _registryCleanUpStrategy;

    public EdgeBrowserFactory ([NotNull] IEdgeConfiguration edgeConfiguration)
    {
      ArgumentUtility.CheckNotNull("edgeConfiguration", edgeConfiguration);

      _edgeConfiguration = edgeConfiguration;
      _registryCleanUpStrategy = ChromiumSecurityWarningsRegistryCleanUpStrategyFactory.CreateForEdge(_edgeConfiguration.DisableSecurityWarningsBehavior);
    }

    public IBrowserSession CreateBrowser (DriverConfiguration configuration)
    {
      ArgumentUtility.CheckNotNull("configuration", configuration);

      var sessionConfiguration = CreateSessionConfiguration(configuration);
      var commandTimeout = configuration.CommandTimeout;

      var extendedEdgeOptions = _edgeConfiguration.CreateEdgeOptions();
      if (configuration.Headless)
        extendedEdgeOptions.AddArgument("headless=new");

      var driver = CreateEdgeDriver(extendedEdgeOptions, out var driverProcessID, commandTimeout);
      driver.Manage().Timeouts().AsynchronousJavaScript = configuration.AsyncJavaScriptTimeout;

      var session = new Coypu.BrowserSession(sessionConfiguration, new CustomSeleniumWebDriver(driver, Browser.Edge));

      return new EdgeBrowserSession(
          session,
          _edgeConfiguration,
          driverProcessID,
          configuration.Headless,
          new[]
          {
              _registryCleanUpStrategy,
              new ChromiumUserDirectoryCleanUpStrategy(_edgeConfiguration.UserDirectoryRoot, extendedEdgeOptions.UserDirectory!)
          });
    }

    private SessionConfiguration CreateSessionConfiguration (DriverConfiguration configuration)
    {
      return new SessionConfiguration
             {
                 Browser = Browser.Edge,
                 RetryInterval = configuration.RetryInterval,
                 Timeout = configuration.SearchTimeout,
                 ConsiderInvisibleElements = WebTestingConstants.ShouldConsiderInvisibleElements,
                 Match = WebTestingConstants.DefaultMatchStrategy,
                 TextPrecision = WebTestingConstants.DefaultTextPrecision,
                 Driver = typeof(CustomSeleniumWebDriver)
             };
    }

    private EdgeDriver CreateEdgeDriver (ExtendedEdgeOptions extendedEdgeOptions, out int driverProcessID, TimeSpan commandTimeout)
    {
      var driverService = CreateEdgeDriverService();
      var driver = new EdgeDriver(driverService, extendedEdgeOptions, commandTimeout);
      driverProcessID = driverService.ProcessId;

      return driver;
    }

    private EdgeDriverService CreateEdgeDriverService ()
    {
      var driverDirectory = Path.GetDirectoryName(_edgeConfiguration.DriverBinaryPath);
      var driverExecutable = Path.GetFileName(_edgeConfiguration.DriverBinaryPath);

      var driverService = EdgeDriverService.CreateDefaultService(driverDirectory, driverExecutable);

      driverService.UseVerboseLogging = false;
      driverService.LogPath = WebDriverLogUtility.CreateLogFile(_edgeConfiguration.LogsDirectory, _edgeConfiguration.BrowserName);

      return driverService;
    }
  }
}
