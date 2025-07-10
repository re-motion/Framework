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
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.DownloadInfrastructure;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Annotations;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebDriver.Factories;

namespace Remotion.Web.Development.WebTesting.WebDriver.Configuration
{
  /// <summary>
  /// Provides a default implementation of <see cref="IBrowserConfiguration"/>, acting as a base class for browser specific implementations.
  /// </summary>
  public abstract class BrowserConfigurationBase : IBrowserConfiguration
  {
    public static void ApplyCommonWebTestFeatureDefaults (
        WebTestFeatureCollection features,
        IBrowserConfiguration browserConfiguration)
    {
      ArgumentNullException.ThrowIfNull(features);
      ArgumentNullException.ThrowIfNull(browserConfiguration);

      features.Set(new BrowserAnnotateHelper(browserConfiguration));
      features.Set(new BrowserHelper(browserConfiguration));
      features.Set(new LocatorHelper(browserConfiguration));
    }

    private readonly ILoggerFactory _loggerFactory;
    private readonly string _browserName;
    private readonly TimeSpan _searchTimeout;
    private readonly TimeSpan _retryInterval;
    private readonly string _logsDirectory;

    private readonly WebTestFeatureCollection _features = new WebTestFeatureCollection();

    protected BrowserConfigurationBase ([NotNull] IWebTestSettings webTestSettings)
    {
      ArgumentNullException.ThrowIfNull(webTestSettings);

      _loggerFactory = webTestSettings.LoggerFactory;
      _browserName = webTestSettings.BrowserName;
      _searchTimeout = webTestSettings.SearchTimeout;
      _retryInterval = webTestSettings.RetryInterval;
      _logsDirectory = webTestSettings.LogsDirectory;

      ApplyCommonWebTestFeatureDefaults(FeaturesMutable, this);
    }

    public abstract string BrowserExecutableName { get; }

    public abstract string WebDriverExecutableName { get; }

    public ILoggerFactory LoggerFactory
    {
      get { return _loggerFactory; }
    }

    public BrowserAnnotateHelper BrowserAnnotateHelper => Features.Get<BrowserAnnotateHelper>();

    public abstract IBrowserFactory BrowserFactory { get; }

    public BrowserHelper BrowserHelper => Features.Get<BrowserHelper>();

    public abstract IDownloadHelper DownloadHelper { get; }

    public LocatorHelper LocatorHelper => Features.Get<LocatorHelper>();

    public abstract IBrowserContentLocator Locator { get; }

    public abstract ScreenshotTooltipStyle TooltipStyle { get; }

    public IReadOnlyWebTestFeatureCollection Features => _features;

    /// <summary>
    /// Mutable features collection intended to allow mutation during the construction of the browser configuration.
    /// Manipulating the collection after construction is not supported.
    /// </summary>
    protected WebTestFeatureCollection FeaturesMutable => _features;

    public string BrowserName
    {
      get { return _browserName; }
    }

    public TimeSpan SearchTimeout
    {
      get { return _searchTimeout; }
    }

    public TimeSpan RetryInterval
    {
      get { return _retryInterval; }
    }

    public string LogsDirectory
    {
      get { return _logsDirectory; }
    }
  }
}
