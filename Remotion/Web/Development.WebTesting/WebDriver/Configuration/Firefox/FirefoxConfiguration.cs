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
using System.Threading;
using JetBrains.Annotations;
using OpenQA.Selenium.Firefox;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.DownloadInfrastructure;
using Remotion.Web.Development.WebTesting.DownloadInfrastructure.Default;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Annotations;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.BrowserContentLocators;
using Remotion.Web.Development.WebTesting.WebDriver.Factories;
using Remotion.Web.Development.WebTesting.WebDriver.Factories.Firefox;

namespace Remotion.Web.Development.WebTesting.WebDriver.Configuration.Firefox
{
  /// <summary>
  /// Implements the <see cref="IBrowserConfiguration"/> interface for Firefox.
  /// </summary>
  public class FirefoxConfiguration : BrowserConfigurationBase, IFirefoxConfiguration
  {
    private const string c_partialFileDownloadExtension = ".part";

    private static readonly Lazy<FirefoxExecutable> s_firefoxExecutable = new Lazy<FirefoxExecutable>(
        () => new FirefoxBinariesProvider().GetInstalledExecutable(),
        LazyThreadSafetyMode.ExecutionAndPublication);

    /// <inheritdoc />
    public string BrowserBinaryPath { get; }

    /// <inheritdoc />
    public string DriverBinaryPath { get; }

    public string DownloadDirectory { get; }

    /// <inheritdoc />
    public override IDownloadHelper DownloadHelper { get; }

    public FirefoxConfiguration ([NotNull] IWebTestSettings webTestSettings)
        : this(webTestSettings, s_firefoxExecutable.Value)
    {
    }

    public FirefoxConfiguration (
        [NotNull] IWebTestSettings webTestSettings,
        [NotNull] FirefoxExecutable firefoxExecutable)
        : base(webTestSettings)
    {
      ArgumentUtility.CheckNotNull("firefoxExecutable", firefoxExecutable);

      BrowserBinaryPath = firefoxExecutable.BrowserBinaryPath;
      DriverBinaryPath = firefoxExecutable.DriverBinaryPath;

      DownloadDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

      var downloadStartedGracePeriod = TimeSpan.FromMinutes(1);
      DownloadHelper = new DefaultDownloadHelper(
          DownloadDirectory,
          c_partialFileDownloadExtension,
          webTestSettings.DownloadStartedTimeout,
          webTestSettings.DownloadUpdatedTimeout,
          downloadStartedGracePeriod,
          webTestSettings.CleanUpUnmatchedDownloadedFiles,
          webTestSettings.LoggerFactory);
    }

    /// <inheritdoc />
    public override string BrowserExecutableName => "firefox";

    /// <inheritdoc />
    public override string WebDriverExecutableName => "geckodriver";

    /// <inheritdoc />
    public override IBrowserFactory BrowserFactory => new FirefoxBrowserFactory(this);

    /// <inheritdoc />
    public override IBrowserContentLocator Locator => new FirefoxBrowserContentLocator();

    /// <inheritdoc />
    public override ScreenshotTooltipStyle TooltipStyle => ScreenshotTooltipStyle.Firefox;

    /// <inheritdoc />
    public virtual FirefoxOptions CreateFirefoxOptions ()
    {
      const int useCustomDownloadDirectory = 2;
      const string mimeTypesToSkipDownloadDialog = "text/plain, application/x-zip-compressed, text/xml";

      var profile = new FirefoxProfile();

      profile.SetPreference("browser.download.dir", DownloadDirectory);
      profile.SetPreference("browser.download.folderList", useCustomDownloadDirectory);
      profile.SetPreference("browser.helperApps.neverAsk.saveToDisk", mimeTypesToSkipDownloadDialog);
      profile.SetPreference("layout.css.devPixelsPerPx", 1);

      var firefoxOptions =  new FirefoxOptions
             {
                 Profile = profile,
                 BinaryLocation = BrowserBinaryPath,
             };

      // Mirrors Chrome's startup behavior to fulfill some initial test expectations
      firefoxOptions.AddArgument("--width=800");
      firefoxOptions.AddArgument("--height=600");

      return firefoxOptions;
    }
  }
}
