// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Coypu;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using Remotion.Web.Development.WebTesting.BrowserSession;
using Remotion.Web.Development.WebTesting.BrowserSession.Remote;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chrome;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Edge;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Firefox;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Remote;

namespace Remotion.Web.Development.WebTesting.WebDriver.Factories.Remote;

/// <summary>
/// Factory to create a browser session using Selenium remote driver.
/// </summary>
public class RemoteBrowserFactory : IBrowserFactory
{
  // We need to provide a process id as our design requires it and changing that fact would
  // cause breaking changes. So we need a process id that is (hopefully) never valid.
  private const int c_invalidProcessId = 1_333_337;

  private readonly IRemoteBrowserConfiguration _remoteConfiguration;

  public RemoteBrowserFactory (IRemoteBrowserConfiguration remoteConfiguration)
  {
    ArgumentNullException.ThrowIfNull(remoteConfiguration);

    _remoteConfiguration = remoteConfiguration;
  }

  public IBrowserSession CreateBrowser (DriverConfiguration driverConfiguration)
  {
    ArgumentNullException.ThrowIfNull(driverConfiguration);

    var browser = _remoteConfiguration.Browser;
    var sessionConfiguration = new SessionConfiguration
                               {
                                   Browser = browser,
                                   RetryInterval = driverConfiguration.RetryInterval,
                                   Timeout = driverConfiguration.SearchTimeout,
                                   ConsiderInvisibleElements = WebTestingConstants.ShouldConsiderInvisibleElements,
                                   Match = WebTestingConstants.DefaultMatchStrategy,
                                   TextPrecision = WebTestingConstants.DefaultTextPrecision,
                                   Driver = typeof(CustomSeleniumWebDriver)
                               };

    DriverOptions driverOptions;
    if (browser.IsChrome())
    {
      driverOptions = CreateChromeOptions(driverConfiguration);
    }
    else if (browser.IsEdge())
    {
      driverOptions = CreateEdgeOptions(driverConfiguration);
    }
    else if (browser.IsFirefox())
    {
      driverOptions = CreateFirefoxOptions(driverConfiguration);
    }
    else
    {
      throw new NotSupportedException("RemoteBrowser can only be used with Chrome, Edge, or Firefox.");
    }

    var remoteWebDriver = new RemoteWebDriver(new Uri(_remoteConfiguration.RemoteDriverSettings.Url), driverOptions);
    remoteWebDriver.Manage().Timeouts().AsynchronousJavaScript = driverConfiguration.AsyncJavaScriptTimeout;
    var session = new Coypu.BrowserSession(sessionConfiguration, new CustomSeleniumWebDriver(remoteWebDriver, browser));

    return new RemoteBrowserSession(session, _remoteConfiguration, c_invalidProcessId, driverConfiguration.Headless);
  }

  /// <summary>
  /// Creates the <see cref="ChromeOptions"/> used when instantiating the Chrome browser.
  /// </summary>
  /// <remarks>
  /// Changes made here might also need to be made in <see cref="ChromeConfiguration"/>.<see cref="ChromeConfiguration.CreateChromeOptions"/>.
  /// </remarks>
  protected virtual ChromeOptions CreateChromeOptions (DriverConfiguration driverConfiguration)
  {
    var chromeOptions = new ChromeOptions();

    chromeOptions.AddArgument("no-first-run");
    chromeOptions.AddArgument("disable-features=ChromeWhatsNewUI");
    chromeOptions.AddArgument("force-device-scale-factor=1");
    chromeOptions.AddArgument("disable-search-engine-choice-screen");

    chromeOptions.AddUserProfilePreference("safebrowsing.enabled", true);

    return chromeOptions;
  }

  /// <summary>
  /// Creates the <see cref="EdgeOptions"/> used when instantiating the Edge browser.
  /// </summary>
  /// <remarks>
  /// Changes made here might also need to be made in <see cref="EdgeConfiguration"/>.<see cref="EdgeConfiguration.CreateEdgeOptions"/>.
  /// </remarks>
  protected virtual EdgeOptions CreateEdgeOptions (DriverConfiguration driverConfiguration)
  {
    var edgeOptions = new EdgeOptions();

    edgeOptions.AddArgument("no-first-run");
    edgeOptions.AddArgument("force-device-scale-factor=1");

    edgeOptions.AddUserProfilePreference("safebrowsing.enabled", true);

    return edgeOptions;
  }

  /// <summary>
  /// Creates Firefox options to prevent using default options.
  /// </summary>
  /// <remarks>
  /// Changes made here might also need to be made in <see cref="FirefoxConfiguration"/>.<see cref="FirefoxConfiguration.CreateFirefoxOptions"/>.
  /// </remarks>
  protected virtual FirefoxOptions CreateFirefoxOptions (DriverConfiguration driverConfiguration)
  {
    const string mimeTypesToSkipDownloadDialog = "text/plain, application/x-zip-compressed, text/xml";

    var profile = new FirefoxProfile();

    profile.SetPreference("browser.helperApps.neverAsk.saveToDisk", mimeTypesToSkipDownloadDialog);
    profile.SetPreference("layout.css.devPixelsPerPx", 1);

    var firefoxOptions = new FirefoxOptions
                         {
                             Profile = profile,
                             UseWebSocketUrl = true
                         };

    // Mirrors Chrome's startup behavior to fulfill some initial test expectations
    firefoxOptions.AddArgument("--width=800");
    firefoxOptions.AddArgument("--height=600");

    if (driverConfiguration.Headless)
      firefoxOptions.AddArgument("-headless");

    return firefoxOptions;
  }
}
