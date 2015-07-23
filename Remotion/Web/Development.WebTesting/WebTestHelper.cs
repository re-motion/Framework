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
using System.Drawing;
using System.Windows.Forms;
using Coypu;
using JetBrains.Annotations;
using log4net;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Helper class for web tests which provides various convinience methods:
  /// <list type="table">
  ///   <listheader>
  ///     <term>Step</term>
  ///     <description>Actions</description>
  ///   </listheader>
  ///   <item>
  ///     <term>FixtureSetUp</term>
  ///     <description>
  ///       <list type="bullet">
  ///         <item>Ensures that all browser windows are closed.</item>
  ///         <item>Initializes a new browser session.</item>
  ///         <item>Ensures that the cursor is outside the browser window.</item>
  ///       </list>
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term>SetUp</term>
  ///     <description>
  ///       <list type="bullet">
  ///         <item>Log output.</item>
  ///       </list>
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term>TearDown</term>
  ///     <description>
  ///       <list type="bullet">
  ///         <item>Takes screenshots if the test failed.</item>
  ///         <item>Log output.</item>
  ///       </list>
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term>FixtureTearDown</term>
  ///     <description>
  ///       <list type="bullet">
  ///         <item>Disposes the browser session.</item>
  ///         <item>Ensures that all browser windows are closed.</item>
  ///       </list>
  ///     </description>
  ///   </item>
  /// </list>
  /// </summary>
  public class WebTestHelper
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (WebTestHelper));

    /// <summary>
    /// Browser configuration.
    /// </summary>
    private readonly IBrowserConfiguration _browserConfiguration;

    /// <summary>
    /// Coypu configuration.
    /// </summary>
    private readonly ICoypuConfiguration _coypuConfiguration;

    /// <summary>
    /// Web test configuration.
    /// </summary>
    private readonly IWebTestConfiguration _webTestConfiguration;

    /// <summary>
    /// Name of the current web test.
    /// </summary>
    private string _testName;

    /// <summary>
    /// Coypu main browser session for the web test.
    /// </summary>
    public BrowserSession MainBrowserSession { get; private set; }

    public WebTestHelper (
        [NotNull] IBrowserConfiguration browserConfiguration,
        [NotNull] ICoypuConfiguration coypuConfiguration,
        [NotNull] IWebTestConfiguration webTestConfiguration)
    {
      ArgumentUtility.CheckNotNull ("browserConfiguration", browserConfiguration);
      ArgumentUtility.CheckNotNull ("coypuConfiguration", coypuConfiguration);
      ArgumentUtility.CheckNotNull ("webTestConfiguration", webTestConfiguration);

      _browserConfiguration = browserConfiguration;
      _coypuConfiguration = coypuConfiguration;
      _webTestConfiguration = webTestConfiguration;
    }

    /// <summary>
    /// Creates a new <see cref="WebTestHelper"/> from <see cref="WebTestingConfiguration.Current"/>.
    /// </summary>
    public static WebTestHelper CreateFromConfiguration ()
    {
      return new WebTestHelper (WebTestingConfiguration.Current, WebTestingConfiguration.Current, WebTestingConfiguration.Current);
    }

    /// <summary>
    /// SetUp method for each web test fixture.
    /// </summary>
    public void OnFixtureSetUp ()
    {
      // Note: otherwise the Selenium web driver may get confused when searching for windows.
      EnsureAllBrowserWindowsAreClosed();

      MainBrowserSession = CreateNewBrowserSession();

      // Note: otherwise cursor could interfere with element hovering.
      EnsureCursorIsOutsideBrowserWindow();
    }

    /// <summary>
    /// SetUp method for each web test.
    /// </summary>
    /// <param name="testName">Name of the test being performed.</param>
    public void OnSetUp ([NotNull] string testName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("testName", testName);

      _testName = testName;
      s_log.InfoFormat ("Executing test: {0}.", _testName);
    }

    /// <summary>
    /// Creates a new browser session using the configured settings from the App.config file.
    /// </summary>
    /// <param name="maximiseWindow">Specified whether the new browser session's window should be maximized.</param>
    /// <returns>The new browser session.</returns>
    public BrowserSession CreateNewBrowserSession (bool maximiseWindow = true)
    {
      using (new PerformanceTimer (s_log, string.Format ("Created new {0} browser session.", _browserConfiguration.BrowserName)))
      {
        var browser = BrowserFactory.CreateBrowser (_browserConfiguration, _coypuConfiguration);
        if (maximiseWindow)
          browser.MaximiseWindow();
        return browser;
      }
    }

    /// <summary>
    /// Returns a new <typeparamref name="TPageObject"/> for the initial page displayed by <paramref name="browser"/>.
    /// </summary>
    public TPageObject CreateInitialPageObject<TPageObject> ([NotNull] BrowserSession browser)
        where TPageObject : PageObject
    {
      ArgumentUtility.CheckNotNull ("browser", browser);

      var context = PageObjectContext.New (browser);
      return (TPageObject) Activator.CreateInstance (typeof (TPageObject), new object[] { context });
    }

    /// <summary>
    /// Accepts a, possibly existent, modal dialog.
    /// </summary>
    public void AcceptPossibleModalDialog ()
    {
      try
      {
        MainBrowserSession.AcceptModalDialogImmediatelyFixed (MainBrowserSession);
      }
      catch (MissingDialogException)
      {
        // It's okay.
      }
    }

    /// <summary>
    /// Returns a new <see cref="DownloadHelper"/> for the given <paramref name="fileName"/>.
    /// </summary>
    /// <param name="fileName">The name of the downloaded file (not the path).</param>
    public DownloadHelper NewDownloadHelper ([NotNull] string fileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("fileName", fileName);

      return new DownloadHelper (_browserConfiguration, fileName, _coypuConfiguration.SearchTimeout);
    }

    /// <summary>
    /// TearDown method for each web test.
    /// </summary>
    /// <param name="hasSucceeded">Specifies whether the test has been successful.</param>
    public void OnTearDown (bool hasSucceeded)
    {
      if (!hasSucceeded && ShouldTakeScreenshots())
      {
        var screenshotCapturer = new ScreenshotCapturer (_webTestConfiguration.ScreenshotDirectory);
        screenshotCapturer.TakeDesktopScreenshot (_testName);
        screenshotCapturer.TakeBrowserScreenshot (_testName, MainBrowserSession);
      }

      s_log.InfoFormat ("Finished test: {0} [has succeeded: {1}].", _testName, hasSucceeded);
    }

    private bool ShouldTakeScreenshots ()
    {
      return !string.IsNullOrEmpty (_webTestConfiguration.ScreenshotDirectory);
    }

    /// <summary>
    /// TearDown method for each web test fixture.
    /// </summary>
    public void OnFixtureTearDown ()
    {
      if (MainBrowserSession != null)
        MainBrowserSession.Dispose();

      // Note: otherwise the sytem may get clogged, if the Selenium web driver implementation does not properly close all windows in all situations.
      EnsureAllBrowserWindowsAreClosed();
      EnsureAllWebDriverInstancesAreClosed();
    }

    private void EnsureAllBrowserWindowsAreClosed ()
    {
      if (!_browserConfiguration.CloseBrowserWindowsOnSetUpAndTearDown)
        return;

      var browserProcessName = _browserConfiguration.GetBrowserExecutableName();
      if (browserProcessName == null)
        return;

      ProcessUtils.KillAllProcessesWithName (browserProcessName);
    }


    private void EnsureAllWebDriverInstancesAreClosed ()
    {
      var webDriverProcessName = _browserConfiguration.GetWebDriverExecutableName();
      if (webDriverProcessName == null)
        return;

      ProcessUtils.KillAllProcessesWithName (webDriverProcessName);
    }

    private void EnsureCursorIsOutsideBrowserWindow ()
    {
      Cursor.Position = new Point (0, 0);
    }
  }
}