// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Coypu;
using Coypu.Drivers;
using OpenQA.Selenium.BiDi.Modules.Session;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.BrowserSession;
using Remotion.Web.Development.WebTesting.BrowserSession.Remote;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Remote;

namespace Remotion.Web.Development.WebTesting.WebDriver.Factories.Remote;

public class RemoteBrowserFactory : IBrowserFactory
{
  private readonly IRemoteBrowserConfiguration _remoteConfiguration;

  public RemoteBrowserFactory (IRemoteBrowserConfiguration remoteConfiguration)
  {
    ArgumentUtility.CheckNotNull(nameof(remoteConfiguration), remoteConfiguration);

    _remoteConfiguration = remoteConfiguration;
  }

  public IBrowserSession CreateBrowser (DriverConfiguration driverConfiguration)
  {
    ArgumentUtility.CheckNotNull(nameof(driverConfiguration), driverConfiguration);

    var sessionConfiguration = CreateSessionConfiguration(driverConfiguration);

    var firefoxOptions = new FirefoxOptions()
                         {
                             UseWebSocketUrl = true
                         };
    var remoteWebDriver = new RemoteWebDriver(new Uri(_remoteConfiguration.RemotingSettings.Url), firefoxOptions);
    remoteWebDriver.Manage().Timeouts().AsynchronousJavaScript = driverConfiguration.AsyncJavaScriptTimeout;
    var session = new Coypu.BrowserSession(sessionConfiguration, new CustomSeleniumWebDriver(remoteWebDriver, Browser.Firefox));

    return new RemoteBrowserSession(session, _remoteConfiguration, 1_333_337, driverConfiguration.Headless);
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
               Driver = typeof(CustomSeleniumWebDriver)
           };
  }
}
