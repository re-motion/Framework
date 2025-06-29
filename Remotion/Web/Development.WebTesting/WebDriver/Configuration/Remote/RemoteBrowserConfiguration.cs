// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Coypu.Drivers;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.DownloadInfrastructure;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Annotations;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chrome;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Edge;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Firefox;
using Remotion.Web.Development.WebTesting.WebDriver.Factories;
using Remotion.Web.Development.WebTesting.WebDriver.Factories.Remote;

namespace Remotion.Web.Development.WebTesting.WebDriver.Configuration.Remote;

/// <inheritdoc cref="IRemoteBrowserConfiguration" />
public class RemoteBrowserConfiguration : BrowserConfigurationBase, IRemoteBrowserConfiguration
{
  public IWebTestRemoteDriverSettings RemoteDriverSettings { get; }

  public Browser Browser { get; }

  public RemoteBrowserConfiguration (IWebTestSettings webTestSettings, Browser browser)
      : base(webTestSettings)
  {
    ArgumentNullException.ThrowIfNull(webTestSettings);
    ArgumentNullException.ThrowIfNull(browser);

    RemoteDriverSettings = webTestSettings.RemoteDriver;
    Browser = browser;

    if (browser == Browser.Chrome)
    {
      TooltipStyle = ScreenshotTooltipStyle.Chrome;

      ChromeConfiguration.ApplyDefaultWebTestFeatures(FeaturesMutable, this);
    }
    else if (browser == Browser.Edge)
    {
      TooltipStyle = ScreenshotTooltipStyle.Edge;

      EdgeConfiguration.ApplyDefaultWebTestFeatures(FeaturesMutable, this);
    }
    else if (browser == Browser.Firefox)
    {
      TooltipStyle = ScreenshotTooltipStyle.Firefox;

      FirefoxConfiguration.ApplyDefaultWebTestFeatures(FeaturesMutable, this);
    }
    else
    {
      throw new NotSupportedException("RemoteDriver is only supported with Chrome, Edge, or Firefox.");
    }
  }

  public override string BrowserExecutableName => "nonexistent-remote-browser-executable";

  public override string WebDriverExecutableName => "nonexistent-remote-driver-executable";

  public override IBrowserFactory BrowserFactory => new RemoteBrowserFactory(this);

  public override IDownloadHelper DownloadHelper => NullDownloadHelper.Instance;

  public override IBrowserContentLocator Locator { get; } = DefaultBrowserContentLocator.Instance;

  public override ScreenshotTooltipStyle TooltipStyle { get; }
}
