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
using Remotion.Utilities;
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
    private readonly string _browserName;
    private readonly TimeSpan _searchTimeout;
    private readonly TimeSpan _retryInterval;
    private readonly string _logsDirectory;
    private readonly AnnotateHelper _annotateHelper;
    private readonly BrowserHelper _browserHelper;
    private readonly LocatorHelper _locatorHelper;
    private readonly MouseHelper _mouseHelper;
	private readonly string _logPrefix;

    protected BrowserConfigurationBase ([NotNull] WebTestConfigurationSection webTestConfigurationSection)
    {
      ArgumentUtility.CheckNotNull ("webTestConfigurationSection", webTestConfigurationSection);

      _browserName = webTestConfigurationSection.BrowserName;
      _searchTimeout = webTestConfigurationSection.SearchTimeout;
      _retryInterval = webTestConfigurationSection.RetryInterval;
      _logsDirectory = webTestConfigurationSection.LogsDirectory;
      _logPrefix = webTestConfigurationSection.LogPrefix;
      _annotateHelper = new AnnotateHelper (this);
      _browserHelper = new BrowserHelper (this);
      _locatorHelper = new LocatorHelper (this);
      _mouseHelper = new MouseHelper (this);
    }

    public abstract string BrowserExecutableName { get; }

    public abstract string WebDriverExecutableName { get; }

    public AnnotateHelper AnnotateHelper
    {
      get { return _annotateHelper; }
    }

    public abstract IBrowserFactory BrowserFactory { get; }

    public BrowserHelper BrowserHelper
    {
      get { return _browserHelper; }
    }

    public abstract IDownloadHelper DownloadHelper { get; }

    public LocatorHelper LocatorHelper
    {
      get { return _locatorHelper; }
    }

    public MouseHelper MouseHelper
    {
      get { return _mouseHelper; }
    }

    public abstract IBrowserContentLocator Locator { get; }

    public abstract ScreenshotTooltipStyle TooltipStyle { get; }

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
    
    public string LogPrefix
    {
      get { return _logPrefix; }
    }
  }
}