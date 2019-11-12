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
using System.Configuration;
using System.IO;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.DownloadInfrastructure;

namespace Remotion.Web.Development.WebTesting.Configuration
{
  /// <summary>
  /// Loads the app.config and validates the parameter. Serves as DTO Object to transfer the app.config settings to the finer grained config classes.
  /// </summary>
  public class WebTestConfigurationSection : ConfigurationSection
  {
    private static readonly Lazy<WebTestConfigurationSection> s_current;

    private readonly ConfigurationPropertyCollection _properties;
    private readonly ConfigurationProperty _browserProperty;
    private readonly ConfigurationProperty _searchTimeoutProperty;
    private readonly ConfigurationProperty _commandTimeoutProperty;
    private readonly ConfigurationProperty _downloadStartedTimeoutProperty;
    private readonly ConfigurationProperty _downloadUpdatedTimeoutProperty;
    private readonly ConfigurationProperty _retryIntervalProperty;
    private readonly ConfigurationProperty _webApplicationRootProperty;
    private readonly ConfigurationProperty _screenshotDirectoryProperty;
    private readonly ConfigurationProperty _logsDirectoryProperty;
    private readonly ConfigurationProperty _closeBrowserWindowsOnSetUpAndTearDownProperty;
    private readonly ConfigurationProperty _cleanUpUnmatchedDownloadedFiles;
    private readonly ConfigurationProperty _requestErrorDetectionStrategyProperty;
    private readonly ConfigurationProperty _hostingProperty;

    static WebTestConfigurationSection ()
    {
      s_current = new Lazy<WebTestConfigurationSection> (
          () =>
          {
            var configuration = (WebTestConfigurationSection) ConfigurationManager.GetSection ("remotion.webTesting");
            Assertion.IsNotNull (configuration, "Configuration section 'remotion.webTesting' missing.");
            return configuration;
          });
    }

    private WebTestConfigurationSection ()
    {
      var xmlnsProperty = new ConfigurationProperty ("xmlns", typeof (string), null, ConfigurationPropertyOptions.None);
      _browserProperty = new ConfigurationProperty (
          "browser",
          typeof (string),
          null,
          null,
          new RegexStringValidator ("(InternetExplorer|Chrome|Firefox)"),
          ConfigurationPropertyOptions.IsRequired);
      _searchTimeoutProperty = new ConfigurationProperty ("searchTimeout", typeof (TimeSpan), null, ConfigurationPropertyOptions.IsRequired);
      _commandTimeoutProperty = new ConfigurationProperty ("commandTimeout", typeof (TimeSpan), TimeSpan.FromMinutes (1));
      _downloadStartedTimeoutProperty = new ConfigurationProperty ("downloadStartedTimeout", typeof (TimeSpan), TimeSpan.FromSeconds (10));
      _downloadUpdatedTimeoutProperty = new ConfigurationProperty ("downloadUpdatedTimeout", typeof (TimeSpan), TimeSpan.FromSeconds (5));
      _retryIntervalProperty = new ConfigurationProperty ("retryInterval", typeof (TimeSpan), null, ConfigurationPropertyOptions.IsRequired);
      _webApplicationRootProperty = new ConfigurationProperty (
          "webApplicationRoot",
          typeof (string),
          null,
          null,
          new StringValidator (minLength: 1),
          ConfigurationPropertyOptions.IsRequired);
      _screenshotDirectoryProperty = new ConfigurationProperty ("screenshotDirectory", typeof (string));
      _logsDirectoryProperty = new ConfigurationProperty ("logsDirectory", typeof (string), ".");
      _closeBrowserWindowsOnSetUpAndTearDownProperty = new ConfigurationProperty ("closeBrowserWindowsOnSetUpAndTearDown", typeof (bool), false);
      _cleanUpUnmatchedDownloadedFiles = new ConfigurationProperty ("cleanUpUnmatchedDownloadedFiles", typeof (bool), false);
      _requestErrorDetectionStrategyProperty = new ConfigurationProperty ("requestErrorDetectionStrategy", typeof (string), "None");
      _hostingProperty = new ConfigurationProperty ("hosting", typeof (ProviderSettings));
      
      _properties = new ConfigurationPropertyCollection
                    {
                        xmlnsProperty,
                        _browserProperty,
                        _searchTimeoutProperty,
                        _commandTimeoutProperty,
                        _downloadStartedTimeoutProperty,
                        _downloadUpdatedTimeoutProperty,
                        _retryIntervalProperty,
                        _webApplicationRootProperty,
                        _screenshotDirectoryProperty,
                        _logsDirectoryProperty,
                        _closeBrowserWindowsOnSetUpAndTearDownProperty,
                        _cleanUpUnmatchedDownloadedFiles,
                        _requestErrorDetectionStrategyProperty,
                        _hostingProperty
                    };
    }

    /// <summary>
    /// Internal method to access app.config webtesting section values. External projects needing to access configuration should use <see cref="WebTestConfigurationFactory"/>
    /// </summary>
    internal static WebTestConfigurationSection Current
    {
      get { return s_current.Value; }
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    /// <summary>
    /// Browser in which the web tests are run.
    /// </summary>
    public string BrowserName
    {
      get { return (string) this [_browserProperty]; }
    }

    /// <summary>
    /// Specifies how long the Coypu engine should maximally search for a web element or try to interact with a web element before it fails.
    /// </summary>
    public TimeSpan SearchTimeout
    {
      get { return (TimeSpan) this [_searchTimeoutProperty]; }
    }

    /// <summary>
    /// Specifies how long Selenium should maximally wait for issued commands before failing.
    /// </summary>
    public TimeSpan CommandTimeout
    {
      get { return (TimeSpan) this [_commandTimeoutProperty]; }
    }

    /// <summary>
    /// Specifies how long the <see cref="DownloadHelperBase"/> should wait before looking for the downloaded file. Default is 10 seconds.
    /// </summary>
    public TimeSpan DownloadStartedTimeout
    {
      get { return (TimeSpan) this [_downloadStartedTimeoutProperty]; }
    }

    /// <summary>
    /// Specifies how long the <see cref="DownloadHelperBase"/> should wait for a downloaded partial file to update. Default is 5 seconds.
    /// </summary>
    public TimeSpan DownloadUpdatedTimeout
    {
      get { return (TimeSpan) this [_downloadUpdatedTimeoutProperty]; }
    }

    /// <summary>
    /// Whenever the element to be interacted with is not ready, visible or otherwise not present, the Coypu engine automatically retries the action
    /// after the given <see cref="RetryInterval"/> until the <see cref="SearchTimeout"/> has been reached.
    /// </summary>
    public TimeSpan RetryInterval
    {
      get { return (TimeSpan) this [_retryIntervalProperty]; }
    }

    /// <summary>
    /// URL to which the web application under test has been published.
    /// </summary>
    public string WebApplicationRoot
    {
      get { return (string) this [_webApplicationRootProperty]; }
    }

    /// <summary>
    /// Absolute or relative path to the screenshot directory. The web testing framework automatically takes two screenshots (one of the whole desktop
    /// and one of the browser window) in case a web test failed.
    /// </summary>
    public string ScreenshotDirectory
    {
      get { return Path.GetFullPath ((string) this [_screenshotDirectoryProperty]); }
    }

    /// <summary>
    /// Absolute or relative path to the logs directory. Some web driver implementations write log files for debugging reasons.
    /// </summary>
    public string LogsDirectory
    {
      get { return (string) this [_logsDirectoryProperty]; }
    }

    /// <summary>
    /// Some Selenium web driver implementations may become confused when searching for windows if there are other browser windows present. Typically
    /// you want to turn this auto-close option on when running web tests, on developer machines, however, this may unexpectedly close important
    /// browser windows, which is why the default value is set to <see langword="false" />.
    /// </summary>
    public bool CloseBrowserWindowsOnSetUpAndTearDown
    {
      get { return (bool) this [_closeBrowserWindowsOnSetUpAndTearDownProperty]; }
    }
    
    /// <summary>
    /// Clean up the download folder on error.
    /// </summary>
    /// <remarks>
    /// When handling downloaded files, there are possible error conditions, where we cannot match the files in the download folder 
    /// (e.g. when the given filename (<see cref="IDownloadHelper.HandleDownloadWithExpectedFileName"/>) does not match the name of the downloaded file).
    /// Typically, you want to turn this on when running web tests, to ensure the clean up of all new downloaded files in case of an error.
    /// However, in edge cases it is possible that files downloaded simultaneously with the test run will be deleted. This is only a concern when running the web test on a developer system.
    /// (e.g. when the developer starts a download and then runs the web tests at the same time), which is why the default value is set to <see langword="false" />.
    /// On your build servers you want to set this to <see langword="true" />, as normally, there is no download run simultaneously to the webtests.
    /// </remarks>
    public bool CleanUpUnmatchedDownloadedFiles
    {
      get { return (bool) this [_cleanUpUnmatchedDownloadedFiles]; }
    }

    ///<summary>
    /// Either a well-known identifier or the assembly-qualified type name of a <see cref="IRequestErrorDetectionStrategy"/> implementation.
    /// </summary>
    /// <remarks>
    /// Use <b>None</b> to disable the request error detection (default) or <b>AspNet</b> to select error handling for the ASP.NET standard error page ("Yellow Page").
    /// </remarks>
    public string RequestErrorDetectionStrategyTypeName
    {
      get { return (string) this [_requestErrorDetectionStrategyProperty]; }
    }

    public ProviderSettings HostingProviderSettings
    {
      get { return (ProviderSettings) this [_hostingProperty]; }
    }
  }
}