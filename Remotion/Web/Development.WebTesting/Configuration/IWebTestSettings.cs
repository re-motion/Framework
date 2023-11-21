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
using OpenQA.Selenium;
using Remotion.Web.Development.WebTesting.DownloadInfrastructure;

namespace Remotion.Web.Development.WebTesting.Configuration
{
  /// <summary>
  /// Represents WebTesting specific settings.
  /// </summary>
  public interface IWebTestSettings
  {
    /// <summary>
    /// Browser in which the web tests are run.
    /// </summary>
    string BrowserName { get; }

    /// <summary>
    /// Specifies how long the Coypu engine should maximally search for a web element or try to interact with a web element before it fails.
    /// </summary>
    TimeSpan SearchTimeout { get; }

    /// <summary>
    /// Specifies how long Selenium should maximally wait for issued commands before failing. Default is 1 minute.
    /// </summary>
    TimeSpan CommandTimeout { get; }

    /// <summary>
    /// Specifies how long the <see cref="DownloadHelperBase"/> should wait before looking for the downloaded file. Default is 10 seconds.
    /// </summary>
    TimeSpan DownloadStartedTimeout { get; }

    /// <summary>
    /// Specifies how long the <see cref="DownloadHelperBase"/> should wait for a downloaded partial file to update. Default is 10 seconds.
    /// </summary>
    TimeSpan DownloadUpdatedTimeout { get; }

    /// <summary>
    /// Specifies how long the <see cref="WebTestSetUpFixtureHelper"/> should wait for the WebApplication to return a 200 on <see cref="WebApplicationRoot"/>. Default is 1 minute.
    /// </summary>
    TimeSpan VerifyWebApplicationStartedTimeout { get; }

    /// <summary>
    /// Whenever the element to be interacted with is not ready, visible or otherwise not present, the Coypu engine automatically retries the action
    /// after the given <see cref="RetryInterval"/> until the <see cref="SearchTimeout"/> has been reached.
    /// </summary>
    TimeSpan RetryInterval { get; }

    /// <summary>
    /// Specifies how long Selenium should maximally wait for asynchronous callbacks after calling <see cref="IJavaScriptExecutor.ExecuteAsyncScript" /> before failing. Default is 10 seconds.
    /// </summary>
    TimeSpan AsyncJavaScriptTimeout { get; }

    /// <summary>
    /// Run the web browser without a user interface (headless mode). Default is false.
    /// </summary>
    bool Headless { get; }

    /// <summary>
    /// URL to which the web application under test has been published.
    /// </summary>
    string WebApplicationRoot { get; }

    /// <summary>
    /// Absolute or relative path to the screenshot directory. The web testing framework automatically takes two screenshots (one of the whole desktop
    /// and one of the browser window) in case a web test failed.
    /// </summary>
    string ScreenshotDirectory { get; }

    /// <summary>
    /// Absolute or relative path to the logs directory. Some web driver implementations write log files for debugging reasons. Default is ".".
    /// </summary>
    string LogsDirectory { get; }

    /// <summary>
    /// Some Selenium web driver implementations may become confused when searching for windows if there are other browser windows present. Typically
    /// you want to turn this auto-close option on when running web tests, on developer machines, however, this may unexpectedly close important
    /// browser windows, which is why the default value is set to <see langword="false" />.
    /// </summary>
    bool CloseBrowserWindowsOnSetUpAndTearDown { get; }

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
    bool CleanUpUnmatchedDownloadedFiles { get; }

    ///<summary>
    /// Either a well-known identifier or the assembly-qualified type name of a <see cref="IRequestErrorDetectionStrategy"/> implementation. Default is "None".
    /// </summary>
    /// <remarks>
    /// Use <b>None</b> to disable the request error detection (default) or <b>AspNet</b> to select error handling for the ASP.NET standard error page ("Yellow Page").
    /// </remarks>
    string RequestErrorDetectionStrategyTypeName { get; }

    /// <summary>
    /// Contains Chrome specific settings.
    /// </summary>
    IWebTestChromiumSettings Chrome { get; }

    /// <summary>
    /// Contains Edge specific settings.
    /// </summary>
    IWebTestChromiumSettings Edge { get; }

    /// <summary>
    /// Contains Hosting settings.
    /// </summary>
    IWebTestHostingSettings Hosting { get; }

    /// <summary>
    /// Gets the test site layout configuration.
    /// </summary>
    IWebTestTestSiteLayoutSettings TestSiteLayout { get; }
  }
}
