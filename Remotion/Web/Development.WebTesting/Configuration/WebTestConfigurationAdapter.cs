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

namespace Remotion.Web.Development.WebTesting.Configuration
{
  public class WebTestConfigurationAdapter : IWebTestConfiguration
  {
    public string Browser => WebTestConfiguration.Current.Browser;

    public TimeSpan SearchTimeout => WebTestConfigurationSection.Current.SearchTimeout;

    public TimeSpan DownloadStartedTimeout => WebTestConfigurationSection.Current.DownloadStartedTimeout;

    public TimeSpan DownloadUpdatedTimeout => WebTestConfigurationSection.Current.DownloadUpdatedTimeout;

    public TimeSpan VerifyWebApplicationStartedTimeout => WebTestConfigurationSection.Current.VerifyWebApplicationStartedTimeout;

    public TimeSpan RetryInterval => WebTestConfigurationSection.Current.RetryInterval;

    public TimeSpan AsyncJavaScriptTimeout => WebTestConfigurationSection.Current.AsyncJavaScriptTimeout;

    public bool Headless => WebTestConfigurationSection.Current.Headless;

    public string WebApplicationRoot => WebTestConfigurationSection.Current.WebApplicationRoot;

    public string ScreenshotDirectory => WebTestConfigurationSection.Current.ScreenshotDirectory;

    public string LogsDirectory => WebTestConfigurationSection.Current.LogsDirectory;

    public bool CloseBrowserWindowsOnSetUpAndTearDown => WebTestConfigurationSection.Current.CloseBrowserWindowsOnSetUpAndTearDown;

    public bool CleanUpUnmatchedDownloadedFiles => WebTestConfigurationSection.Current.CleanUpUnmatchedDownloadedFiles;

    public string RequestErrorDetectionStrategy => WebTestConfigurationSection.Current.RequestErrorDetectionStrategyTypeName;

    public IWebTestChromiumConfiguration Chrome { get; } = new WebTestChromeConfigurationAdapter();

    public IWebTestChromiumConfiguration Edge { get; } = new WebTestEdgeConfigurationAdapter();

    public IWebTestHostingConfiguration WebTestHostingConfiguration { get; } = new WebTestHostingConfigurationAdapter();

    public IWebTestSiteLayoutConfiguration WebTestSiteLayoutConfiguration { get; } = new WebWebTestWebTestSiteLayoutConfigurationConfigurationAdapter();
  }
}
