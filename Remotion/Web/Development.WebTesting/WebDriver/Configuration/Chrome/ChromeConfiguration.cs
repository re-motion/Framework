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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;
using OpenQA.Selenium;
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

    private static readonly Lazy<ChromeExecutable> s_chromeExecutable =
        new Lazy<ChromeExecutable> (() => new ChromeBinariesProvider().GetInstalledExecutable(), LazyThreadSafetyMode.ExecutionAndPublication);

    private static readonly Lazy<FieldInfo> s_knownCapabilityNamesField = new Lazy<FieldInfo> (
        () =>
        {
          var knownCapabilityNamesField = typeof (DriverOptions).GetField ("knownCapabilityNames", BindingFlags.Instance | BindingFlags.NonPublic);
          Assertion.IsNotNull (knownCapabilityNamesField, "Selenium has changed, please update s_knownCapabilityNamesField field.");
          return knownCapabilityNamesField;
        },
        LazyThreadSafetyMode.ExecutionAndPublication);

    public override string BrowserExecutableName { get; } = "chrome";
    public override string WebDriverExecutableName { get; } = "chromedriver";
    public override IBrowserContentLocator Locator { get; } = new ChromeBrowserContentLocator();
    public override ScreenshotTooltipStyle TooltipStyle { get; } = ScreenshotTooltipStyle.Chrome;
    public override IDownloadHelper DownloadHelper { get; }
    public string BrowserBinaryPath { get; }
    public string DriverBinaryPath { get; }
    public string UserDirectoryRoot { get; }
    public bool EnableUserDirectoryRootCleanup { get; }
    public string DownloadDirectory { get; }
    public bool DisableInfoBars { get; }

    public ChromeConfiguration (
        [NotNull] WebTestConfigurationSection webTestConfigurationSection,
        [NotNull] AdvancedChromeOptions advancedChromeOptions)
        : this (webTestConfigurationSection, s_chromeExecutable.Value, advancedChromeOptions)
    {
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

      BrowserBinaryPath = chromeExecutable.BrowserBinaryPath;
      DriverBinaryPath = chromeExecutable.DriverBinaryPath;
      UserDirectoryRoot = chromeExecutable.UserDirectory;

      EnableUserDirectoryRootCleanup = advancedChromeOptions.DeleteUserDirectoryRoot;
      DisableInfoBars = advancedChromeOptions.DisableInfoBars;

      DownloadDirectory = Path.Combine (Path.GetTempPath(), Path.GetRandomFileName());

      var downloadStartedGracePeriod = TimeSpan.FromMinutes (1);

      DownloadHelper = new DefaultDownloadHelper (
          DownloadDirectory,
          c_partialFileDownloadExtension,
          webTestConfigurationSection.DownloadStartedTimeout,
          webTestConfigurationSection.DownloadUpdatedTimeout,
          downloadStartedGracePeriod,
          webTestConfigurationSection.CleanUpUnmatchedDownloadedFiles);
    }

    public override IBrowserFactory BrowserFactory => new ChromeBrowserFactory (this);

    public virtual ExtendedChromeOptions CreateChromeOptions ()
    {
      var chromeOptions = new ExtendedChromeOptions();

      DisableSpecCompliance (chromeOptions);

      if (!string.IsNullOrEmpty (BrowserBinaryPath))
        chromeOptions.BinaryLocation = BrowserBinaryPath;

      var userDirectory = CreateUnusedUserDirectoryPath();
      chromeOptions.UserDirectory = userDirectory;

      if (!string.IsNullOrEmpty (userDirectory))
        chromeOptions.AddArgument (string.Format ("user-data-dir={0}", userDirectory));

      if (DisableInfoBars)
        chromeOptions.AddArgument ("disable-infobars");

      chromeOptions.AddArgument ("no-first-run");

      chromeOptions.AddUserProfilePreference ("safebrowsing.enabled", true);
      chromeOptions.AddUserProfilePreference ("download.default_directory", DownloadDirectory);

      return chromeOptions;
    }

    private void DisableSpecCompliance (ExtendedChromeOptions chromeOptions)
    {
      var knownCapabilityNames = (Dictionary<string, string>) s_knownCapabilityNamesField.Value.GetValue (chromeOptions);
      knownCapabilityNames.Remove ("w3c");

      chromeOptions.AddAdditionalCapability ("w3c", false);
    }

    [CanBeNull]
    private string CreateUnusedUserDirectoryPath ()
    {
      if (UserDirectoryRoot == null)
        return null;

      string userDirectory;
      var userDirectoryID = 0;
      do
      {
        userDirectory = Path.Combine (UserDirectoryRoot, string.Join (c_userDataFolderPrefix, userDirectoryID));
        userDirectoryID++;
      } while (Directory.Exists (userDirectory));

      return userDirectory;
    }
  }
}