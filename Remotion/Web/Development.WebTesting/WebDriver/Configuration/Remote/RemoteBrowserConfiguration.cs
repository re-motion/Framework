// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.DownloadInfrastructure;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Annotations;
using Remotion.Web.Development.WebTesting.WebDriver.Factories;
using Remotion.Web.Development.WebTesting.WebDriver.Factories.Remote;

namespace Remotion.Web.Development.WebTesting.WebDriver.Configuration.Remote;

public class RemoteBrowserConfiguration : BrowserConfigurationBase, IRemoteBrowserConfiguration
{
  public IWebTestRemotingSettings RemotingSettings { get; }

  public RemoteBrowserConfiguration (IWebTestSettings webTestSettings)
      : base(webTestSettings)
  {
    ArgumentUtility.CheckNotNull(nameof(webTestSettings), webTestSettings);

    RemotingSettings = webTestSettings.Remoting;
  }

  public override string BrowserExecutableName => "nonexistent-remote-browser-executable";

  public override string WebDriverExecutableName => "nonexistent-remote-driver-executable";

  public override IBrowserFactory BrowserFactory => new RemoteBrowserFactory(this);

  public override IDownloadHelper DownloadHelper => NullDownloadHelper.Instance;

  public override IBrowserContentLocator Locator { get; } = DefaultBrowserContentLocator.Instance;

  public override ScreenshotTooltipStyle TooltipStyle { get; } = ScreenshotTooltipStyle.Chrome; // TODO this should be browser specific
}
