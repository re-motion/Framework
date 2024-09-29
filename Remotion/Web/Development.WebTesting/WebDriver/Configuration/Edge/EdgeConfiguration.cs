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
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chromium;
using Remotion.Web.Development.WebTesting.WebDriver.Factories;
using Remotion.Web.Development.WebTesting.WebDriver.Factories.Edge;

namespace Remotion.Web.Development.WebTesting.WebDriver.Configuration.Edge
{
  /// <summary>
  /// Implements the <see cref="IBrowserConfiguration"/> interface for Edge.
  /// </summary>
  public class EdgeConfiguration : BrowserConfigurationBase, IEdgeConfiguration
  {
    private const string c_userDataFolderPrefix = "userdata";

    private static readonly Lazy<EdgeExecutable> s_edgeExecutable =
        new Lazy<EdgeExecutable>(() => new EdgeBinariesProvider().GetInstalledExecutable(), LazyThreadSafetyMode.ExecutionAndPublication);

    private static readonly Lazy<FieldInfo> s_knownCapabilityNamesField = new Lazy<FieldInfo>(
        () =>
        {
          var knownCapabilityNamesField = typeof(DriverOptions).GetField("knownCapabilityNames", BindingFlags.Instance | BindingFlags.NonPublic);
          Assertion.IsNotNull(knownCapabilityNamesField, "Selenium has changed, please update s_knownCapabilityNamesField field.");
          return knownCapabilityNamesField;
        },
        LazyThreadSafetyMode.ExecutionAndPublication);

    public override string BrowserExecutableName { get; } = "msedge";
    public override string WebDriverExecutableName { get; } = "msedgedriver";
    public override IDownloadHelper DownloadHelper { get; }
    public override IBrowserContentLocator Locator { get; } = new EdgeBrowserContentLocator();
    public override ScreenshotTooltipStyle TooltipStyle { get; } = ScreenshotTooltipStyle.Edge;

    public string BrowserBinaryPath { get; }
    public string DriverBinaryPath { get; }
    public string UserDirectoryRoot { get; }
    public string DownloadDirectory { get; }
    public ChromiumDisableSecurityWarningsBehavior DisableSecurityWarningsBehavior { get; }

    public EdgeConfiguration (
        [NotNull] IWebTestSettings webTestSettings)
        : this(webTestSettings, s_edgeExecutable.Value)
    {
    }

    public EdgeConfiguration (
        [NotNull] IWebTestSettings webTestSettings,
        [NotNull] EdgeExecutable edgeExecutable)
        : base(webTestSettings)
    {
      ArgumentUtility.CheckNotNull("webTestSettings", webTestSettings);
      ArgumentUtility.CheckNotNull("edgeExecutable", edgeExecutable);

      BrowserBinaryPath = edgeExecutable.BrowserBinaryPath;
      DriverBinaryPath = edgeExecutable.DriverBinaryPath;
      UserDirectoryRoot = edgeExecutable.UserDirectory;

      DownloadDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

      var downloadStartedGracePeriod = TimeSpan.FromMinutes(1);
      DownloadHelper = new DefaultDownloadHelper(
          DownloadDirectory,
          ".crdownload",
          webTestSettings.DownloadStartedTimeout,
          webTestSettings.DownloadUpdatedTimeout,
          downloadStartedGracePeriod,
          webTestSettings.CleanUpUnmatchedDownloadedFiles,
          webTestSettings.LoggerFactory);

      DisableSecurityWarningsBehavior = webTestSettings.Edge.DisableSecurityWarningsBehavior;
    }

    public virtual ExtendedEdgeOptions CreateEdgeOptions ()
    {
      var userDirectory = CreateUnusedUserDirectoryPath();
      var edgeOptions = new ExtendedEdgeOptions
                          {
                              BinaryLocation = BrowserBinaryPath,
                              UserDirectory = userDirectory
                          };

      DisableSpecCompliance(edgeOptions);

      edgeOptions.AddArgument($"user-data-dir={userDirectory}");

      edgeOptions.AddArgument("no-first-run");
      edgeOptions.AddArgument("force-device-scale-factor=1");

      edgeOptions.AddUserProfilePreference("safebrowsing.enabled", true);
      edgeOptions.AddUserProfilePreference("download.default_directory", DownloadDirectory);

      return edgeOptions;
    }

    private void DisableSpecCompliance (ExtendedEdgeOptions edgeOptions)
    {
      var knownCapabilityNames = (Dictionary<string, string>)s_knownCapabilityNamesField.Value.GetValue(edgeOptions)!;
      knownCapabilityNames.Remove("w3c");

      edgeOptions.AddAdditionalOption("w3c", false);
    }

    private string CreateUnusedUserDirectoryPath ()
    {
      string userDirectory;
      var userDirectoryID = 0;
      do
      {
        userDirectory = Path.Combine(UserDirectoryRoot, string.Join(c_userDataFolderPrefix, userDirectoryID));
        userDirectoryID++;
      } while (Directory.Exists(userDirectory));

      return userDirectory;
    }

    public override IBrowserFactory BrowserFactory => new EdgeBrowserFactory(this, LoggerFactory);
  }
}
