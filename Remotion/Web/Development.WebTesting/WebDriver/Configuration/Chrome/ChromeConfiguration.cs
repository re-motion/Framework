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
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.DownloadInfrastructure;
using Remotion.Web.Development.WebTesting.DownloadInfrastructure.Default;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Annotations;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.BrowserContentLocators;
using Remotion.Web.Development.WebTesting.WebDriver.Factories;
using Remotion.Web.Development.WebTesting.WebDriver.Factories.Chrome;

namespace Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chrome
{
  /// <summary>
  /// Implements the <see cref="IBrowserConfiguration"/> interface for Chrome.
  /// </summary>
  public class ChromeConfiguration : BrowserConfigurationBase, IChromeConfiguration
  {
    private const string c_userDataFolderPrefix = "userdata";
    private const string c_partialFileDownloadExtension = ".crdownload";

    private readonly IBrowserContentLocator _locator = new ChromeBrowserContentLocator();

    private readonly string _binaryPath;
    private readonly string _userDirectoryRoot;
    private readonly IDownloadHelper _downloadHelper;
    private readonly string _downloadDirectory;
    private readonly bool _disableInfoBars;
    private readonly bool _deleteUserDirectoryRoot;

    public ChromeConfiguration (
        [NotNull] WebTestConfigurationSection webTestConfigurationSection,
        [NotNull] AdvancedChromeOptions advancedChromeOptions)
        : base (webTestConfigurationSection)
    {
      ArgumentUtility.CheckNotNull ("webTestConfigurationSection", webTestConfigurationSection);
      ArgumentUtility.CheckNotNull ("advancedChromeOptions", advancedChromeOptions);

      _downloadDirectory = Path.Combine (Path.GetTempPath(), Path.GetRandomFileName());

      var downloadStartedGracePeriod = TimeSpan.FromMinutes (1);

      _downloadHelper = new DefaultDownloadHelper (
          _downloadDirectory,
          c_partialFileDownloadExtension,
          webTestConfigurationSection.DownloadStartedTimeout,
          webTestConfigurationSection.DownloadUpdatedTimeout,
          downloadStartedGracePeriod,
          webTestConfigurationSection.CleanUpUnmatchedDownloadedFiles);

      _disableInfoBars = advancedChromeOptions.DisableInfoBars;
    }

    public ChromeConfiguration (
        [NotNull] WebTestConfigurationSection webTestConfigurationSection,
        [NotNull] ChromeExecutable chromeExecutable,
        [NotNull] AdvancedChromeOptions advancedChromeOptions)
        : base (webTestConfigurationSection)
    {
      ArgumentUtility.CheckNotNull ("webTestConfigurationSection", webTestConfigurationSection);
      ArgumentUtility.CheckNotNull ("chromeExecutable", chromeExecutable);
      ArgumentUtility.CheckNotNull ("advancedChromeOptions", advancedChromeOptions);

      _binaryPath = chromeExecutable.BinaryPath;
      _userDirectoryRoot = chromeExecutable.UserDirectory;

      _deleteUserDirectoryRoot = advancedChromeOptions.DeleteUserDirectoryRoot;
      _disableInfoBars = advancedChromeOptions.DisableInfoBars;

      _downloadDirectory = Path.Combine (Path.GetTempPath(), Path.GetRandomFileName());

      var downloadStartedGracePeriod = TimeSpan.FromMinutes (1);

      _downloadHelper = new DefaultDownloadHelper (
          _downloadDirectory,
          c_partialFileDownloadExtension,
          webTestConfigurationSection.DownloadStartedTimeout,
          webTestConfigurationSection.DownloadUpdatedTimeout,
          downloadStartedGracePeriod,
          webTestConfigurationSection.CleanUpUnmatchedDownloadedFiles);
    }

    public override string BrowserExecutableName
    {
      get { return "chrome"; }
    }

    public override string WebDriverExecutableName
    {
      get { return "chromedriver"; }
    }

    public override IBrowserFactory BrowserFactory
    {
      get { return new ChromeBrowserFactory (this); }
    }

    public override IDownloadHelper DownloadHelper
    {
      get { return _downloadHelper; }
    }

    public override IBrowserContentLocator Locator
    {
      get { return _locator; }
    }

    public override ScreenshotTooltipStyle TooltipStyle
    {
      get { return ScreenshotTooltipStyle.Chrome; }
    }

    public string BinaryPath
    {
      get { return _binaryPath; }
    }

    public string UserDirectoryRoot
    {
      get { return _userDirectoryRoot; }
    }

    public bool EnableUserDirectoryRootCleanup
    {
      get { return _deleteUserDirectoryRoot; }
    }

    public virtual ExtendedChromeOptions CreateChromeOptions ()
    {
      var chromeOptions = new ExtendedChromeOptions();

      if (!string.IsNullOrEmpty (_binaryPath))
        chromeOptions.BinaryLocation = _binaryPath;

      var userDirectory = CreateUnusedUserDirectoryPath();
      chromeOptions.UserDirectory = userDirectory;

      if (!string.IsNullOrEmpty (userDirectory))
        chromeOptions.AddArgument (string.Format ("user-data-dir={0}", userDirectory));

      if (_disableInfoBars)
        chromeOptions.AddArgument ("disable-infobars");

      chromeOptions.AddArgument ("no-first-run");

      chromeOptions.AddUserProfilePreference ("safebrowsing.enabled", true);
      chromeOptions.AddUserProfilePreference ("download.default_directory", _downloadDirectory);

      return chromeOptions;
    }

    [CanBeNull]
    private string CreateUnusedUserDirectoryPath ()
    {
      if (_userDirectoryRoot == null)
        return null;

      string userDirectory;
      var userDirectoryID = 0;
      do
      {
        userDirectory = Path.Combine (_userDirectoryRoot, string.Join (c_userDataFolderPrefix, userDirectoryID));
        userDirectoryID++;
      } while (Directory.Exists (userDirectory));

      return userDirectory;
    }
  }
}