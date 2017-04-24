﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using OpenQA.Selenium.Chrome;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.DownloadInfrastructure;
using Remotion.Web.Development.WebTesting.DownloadInfrastructure.Chrome;
using Remotion.Web.Development.WebTesting.WebDriver.Factories;
using Remotion.Web.Development.WebTesting.WebDriver.Factories.Chrome;

namespace Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chrome
{
  /// <summary>
  /// Implements the <see cref="IBrowserConfiguration"/> interface for Chrome.
  /// </summary>
  public class ChromeConfiguration : BrowserConfigurationBase, IChromeConfiguration
  {
    private readonly string _binaryPath;
    private readonly string _userDirectory;
    private readonly IDownloadHelper _downloadHelper;
    private readonly string _downloadDirectory;

    public ChromeConfiguration ([NotNull] WebTestConfigurationSection webTestConfigurationSection)
        : base (webTestConfigurationSection)
    {
      ArgumentUtility.CheckNotNull ("webTestConfigurationSection", webTestConfigurationSection);
      
      _downloadDirectory = Path.Combine (Path.GetTempPath(), Path.GetRandomFileName());
      
      var downloadStartedGracePeriod = TimeSpan.FromMinutes (1);

      _downloadHelper = new ChromeDownloadHelper (
          _downloadDirectory,
          webTestConfigurationSection.DownloadStartedTimeout,
          webTestConfigurationSection.DownloadUpdatedTimeout,
          downloadStartedGracePeriod,
          webTestConfigurationSection.CleanUpUnmatchedDownloadedFiles);
    }

    public ChromeConfiguration (
        [NotNull] WebTestConfigurationSection webTestConfigurationSection,
        [NotNull] ChromeExecutable chromeExecutable)
        : base (webTestConfigurationSection)
    {
      ArgumentUtility.CheckNotNull ("webTestConfigurationSection", webTestConfigurationSection);
      ArgumentUtility.CheckNotNull ("chromeExecutable", chromeExecutable);

      _binaryPath = chromeExecutable.BinaryPath;
      _userDirectory = chromeExecutable.UserDirectory;

      _downloadDirectory = Path.Combine (Path.GetTempPath(), Path.GetRandomFileName());
      
      var downloadStartedGracePeriod = TimeSpan.FromMinutes (1);

      _downloadHelper = new ChromeDownloadHelper (
          _downloadDirectory,
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

    public string BinaryPath
    {
      get { return _binaryPath; }
    }

    public string UserDirectory
    {
      get { return _userDirectory; }
    }

    public virtual ChromeOptions CreateChromeOptions ()
    {
      var chromeOptions = new ChromeOptions();

      if (!string.IsNullOrEmpty (_binaryPath))
        chromeOptions.BinaryLocation = _binaryPath;

      if (!string.IsNullOrEmpty (_userDirectory))
        chromeOptions.AddArgument (string.Format ("user-data-dir={0}", _userDirectory));

      chromeOptions.AddArgument ("no-first-run");

      chromeOptions.AddUserProfilePreference ("safebrowsing.enabled", true);
      chromeOptions.AddUserProfilePreference ("download.default_directory", _downloadDirectory);

      return chromeOptions;
    }
  }
}