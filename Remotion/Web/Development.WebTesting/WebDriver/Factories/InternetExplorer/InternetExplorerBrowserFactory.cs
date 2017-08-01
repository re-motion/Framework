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
using Coypu;
using Coypu.Drivers;
using JetBrains.Annotations;
using OpenQA.Selenium.IE;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.BrowserSession;
using Remotion.Web.Development.WebTesting.BrowserSession.InternetExplorer;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.InternetExplorer;

namespace Remotion.Web.Development.WebTesting.WebDriver.Factories.InternetExplorer
{
  /// <summary>
  /// Responsible for creating a new InternetExplorer browser, configured based on <see cref="IInternetExplorerConfiguration"/> and <see cref="ITestInfrastructureConfiguration"/>.
  /// </summary>
  public class InternetExplorerBrowserFactory : IBrowserFactory
  {
    private readonly IInternetExplorerConfiguration _internetExplorerConfiguration;

    public InternetExplorerBrowserFactory ([NotNull] IInternetExplorerConfiguration internetExplorerConfiguration)
    {
      ArgumentUtility.CheckNotNull ("internetExplorerConfiguration", internetExplorerConfiguration);

      _internetExplorerConfiguration = internetExplorerConfiguration;
    }

    public IBrowserSession CreateBrowser (ITestInfrastructureConfiguration testInfrastructureConfiguration)
    {
      ArgumentUtility.CheckNotNull ("testInfrastructureConfiguration", testInfrastructureConfiguration);

      var sessionConfiguration = CreateSessionConfiguration (testInfrastructureConfiguration);
      int driverProcessId;
      var driver = CreateInternetExplorerDriver (out driverProcessId);

      var session = new Coypu.BrowserSession (sessionConfiguration, new CustomSeleniumWebDriver (driver, Browser.InternetExplorer));

      return new InternetExplorerBrowserSession (session, _internetExplorerConfiguration, driverProcessId);
    }

    private SessionConfiguration CreateSessionConfiguration (ITestInfrastructureConfiguration testInfrastructureConfiguration)
    {
      return new SessionConfiguration
             {
                 Browser = Browser.InternetExplorer,
                 RetryInterval = testInfrastructureConfiguration.RetryInterval,
                 Timeout = testInfrastructureConfiguration.SearchTimeout,
                 ConsiderInvisibleElements = WebTestingConstants.ShouldConsiderInvisibleElements,
                 Match = WebTestingConstants.DefaultMatchStrategy,
                 TextPrecision = WebTestingConstants.DefaultTextPrecision,
                 Driver = typeof (CustomSeleniumWebDriver)
             };
    }

    private InternetExplorerDriver CreateInternetExplorerDriver (out int driverProcessId)
    {
      var driverService = CreateInternetExplorerDriverService();
      var options = _internetExplorerConfiguration.CreateInternetExplorerOptions();

      var driver = new InternetExplorerDriver (driverService, options);

      driverProcessId = driverService.ProcessId;

      return driver;
    }

    private InternetExplorerDriverService CreateInternetExplorerDriverService ()
    {
      var driverService = InternetExplorerDriverService.CreateDefaultService();
      driverService.LogFile = WebDriverLogUtility.CreateLogFile (
          _internetExplorerConfiguration.LogsDirectory,
          _internetExplorerConfiguration.BrowserName);
      driverService.LoggingLevel = InternetExplorerDriverLogLevel.Info;
      return driverService;
    }
  }
}