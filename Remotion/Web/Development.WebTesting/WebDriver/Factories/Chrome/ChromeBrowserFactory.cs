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
using Coypu;
using Coypu.Drivers;
using JetBrains.Annotations;
using OpenQA.Selenium.Chrome;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.BrowserSession;
using Remotion.Web.Development.WebTesting.BrowserSession.Chrome;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chrome;

namespace Remotion.Web.Development.WebTesting.WebDriver.Factories.Chrome
{
  /// <summary>
  /// Responsible for creating a new Chrome browser, configured based on <see cref="IChromeConfiguration"/> and <see cref="ITestInfrastructureConfiguration"/>.
  /// </summary>
  public class ChromeBrowserFactory : IBrowserFactory
  {
    private readonly IChromeConfiguration _chromeConfiguration;

    public ChromeBrowserFactory ([NotNull] IChromeConfiguration chromeConfiguration)
    {
      ArgumentUtility.CheckNotNull ("chromeConfiguration", chromeConfiguration);

      _chromeConfiguration = chromeConfiguration;
    }

    public IBrowserSession CreateBrowser (ITestInfrastructureConfiguration testInfrastructureConfiguration)
    {
      ArgumentUtility.CheckNotNull ("testInfrastructureConfiguration", testInfrastructureConfiguration);

      var sessionConfiguration = CreateSessionConfiguration (testInfrastructureConfiguration);
      int driverProcessID;
      string userDirectory;
      var driver = CreateChromeDriver (out driverProcessID, out userDirectory);

      var session = new Coypu.BrowserSession (sessionConfiguration, new CustomSeleniumWebDriver (driver, Browser.Chrome));

      return new ChromeBrowserSession (session, _chromeConfiguration, driverProcessID, userDirectory);
    }

    private SessionConfiguration CreateSessionConfiguration (ITestInfrastructureConfiguration testInfrastructureConfiguration)
    {
      return new SessionConfiguration
             {
                 Browser = Browser.Chrome,
                 RetryInterval = testInfrastructureConfiguration.RetryInterval,
                 Timeout = testInfrastructureConfiguration.SearchTimeout,
                 ConsiderInvisibleElements = WebTestingConstants.ShouldConsiderInvisibleElements,
                 Match = WebTestingConstants.DefaultMatchStrategy,
                 TextPrecision = WebTestingConstants.DefaultTextPrecision,
                 Driver = typeof (CustomSeleniumWebDriver)
             };
    }

    private ChromeDriver CreateChromeDriver (out int driverProcessID, [CanBeNull] out string userDirectory)
    {
      var driverService = CreateChromeDriverService();
      var chromeOptions = _chromeConfiguration.CreateChromeOptions();

      var driver = new ChromeDriver (driverService, chromeOptions);
      driverProcessID = driverService.ProcessId;
      userDirectory = chromeOptions.UserDirectory;
      return driver;
    }

    private ChromeDriverService CreateChromeDriverService ()
    {
      var driverService = ChromeDriverService.CreateDefaultService();

      driverService.EnableVerboseLogging = false;

#pragma warning disable 0612
      driverService.LogPath = WebDriverLogUtility.CreateLogFile (_chromeConfiguration.LogsDirectory, _chromeConfiguration.LogPrefix, _chromeConfiguration.BrowserName);
#pragma warning restore 0612

      return driverService;
    }
  }
}