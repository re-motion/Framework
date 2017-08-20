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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Coypu;
using JetBrains.Annotations;
using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration;
using Remotion.Web.Development.WebTesting.WebDriver.Factories;
using Screenshot = Remotion.Web.Development.WebTesting.ScreenshotCreation.Screenshot;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Helper class for web tests which provides various convenience methods.
  /// </summary>
  /// <remarks>
  /// Use the factory methods <see cref="CreateFromConfiguration"/> or <see cref="CreateFromConfiguration{TFactory}"/> 
  /// for instantiating an instance of type <see cref="WebTestHelper"/>.
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
  /// </remarks>
  public class WebTestHelper
  {
    /// <summary>
    /// Creates a new <see cref="WebTestHelper"/> with configuration based on <see cref="WebTestConfigurationFactory"/>.
    /// </summary>
    [PublicAPI]
    public static WebTestHelper CreateFromConfiguration ()
    {
      return new WebTestHelper (new WebTestConfigurationFactory());
    }

    /// <summary>
    /// Creates a new <see cref="WebTestHelper"/> with configuration based on <typeparamref name="TFactory"/>.
    /// </summary>
    /// <remarks>
    /// Use this overload when you have to provide test-project specific configuration settings (e.g. custom chrome.exe) 
    /// via custom <see cref="WebTestConfigurationFactory"/>.
    /// </remarks>
    [PublicAPI]
    public static WebTestHelper CreateFromConfiguration<TFactory> ()
        where TFactory : WebTestConfigurationFactory, new()
    {
      return new WebTestHelper (new TFactory());
    }

    private static readonly ILog s_log = LogManager.GetLogger (typeof (WebTestHelper));

    private readonly IBrowserConfiguration _browserConfiguration;
    private readonly ITestInfrastructureConfiguration _testInfrastructureConfiguration;
    private readonly List<IBrowserSession> _browserSessions = new List<IBrowserSession>();
    private BrowserSession _mainBrowserSession;

    /// <summary>
    /// Name of the current web test.
    /// </summary>
    private string _testName;

    [PublicAPI]
    protected WebTestHelper ([NotNull] WebTestConfigurationFactory webTestConfigurationFactory)
    {
      ArgumentUtility.CheckNotNull ("webTestConfigurationFactory", webTestConfigurationFactory);

      _browserConfiguration = webTestConfigurationFactory.CreateBrowserConfiguration();
      _testInfrastructureConfiguration = webTestConfigurationFactory.CreateTestInfrastructureConfiguration();
    }

    public IBrowserConfiguration BrowserConfiguration
    {
      get { return _browserConfiguration; }
    }

    public ITestInfrastructureConfiguration TestInfrastructureConfiguration
    {
      get { return _testInfrastructureConfiguration; }
    }

    /// <summary>
    /// Coypu main browser session for the web test.
    /// </summary>
    public BrowserSession MainBrowserSession
    {
      get { return _mainBrowserSession; }
    }

    /// <summary>
    /// SetUp method for each web test fixture.
    /// </summary>
    /// <param name="maximizeWindow">Specifies whether the main browser session's window should be maximized.</param>
    public void OnFixtureSetUp (bool maximizeWindow = true)
    {
      s_log.InfoFormat ("WebTestHelper.OnFixtureSetup() has been called.");
      s_log.InfoFormat ("Remotion version: " + typeof (WebTestHelper).Assembly.GetName().Version);
      s_log.InfoFormat ("Selenium (WebDriver) version: " + typeof (IWebDriver).Assembly.GetName().Version);
      s_log.InfoFormat ("Selenium Support (WebDriver.Support) version: " + typeof (WebDriverWait).Assembly.GetName().Version);
      s_log.InfoFormat ("Coypu version: " + typeof (Element).Assembly.GetName().Version);

      // Note: otherwise the Selenium web driver may get confused when searching for windows.
      // Confusion could theoretically happen when calling Coypu.BrowserSession.FindWindow(string locator), as the window gets found per title.
      // This method only gets called when using ExpectNewWindow or ExpectNewPopWindow.
      // See RM-6731.
      EnsureAllBrowserWindowsAreClosed();

      _mainBrowserSession = CreateNewBrowserSession (maximizeWindow);

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

      if (_mainBrowserSession != null)
        s_log.InfoFormat ("Current window title: {0}.", _mainBrowserSession.Title);
    }

    /// <summary>
    /// Creates a new browser session using the configured settings from <see cref="WebTestConfigurationFactory"/>.
    /// </summary>
    /// <param name="maximizeWindow">Specified whether the new browser session's window should be maximized.</param>
    /// <returns>The new browser session.</returns>
    public BrowserSession CreateNewBrowserSession (bool maximizeWindow = true)
    {
      using (new PerformanceTimer (s_log, string.Format ("Created new {0} browser session.", _browserConfiguration.BrowserName)))
      {
        var browserResult = _browserConfiguration.BrowserFactory.CreateBrowser (_testInfrastructureConfiguration);
        _browserSessions.Add (browserResult);

        if (maximizeWindow)
          browserResult.Value.MaximiseWindow();

        return browserResult.Value;
      }
    }

    /// <summary>
    /// Returns a new <typeparamref name="TPageObject"/> for the initial page displayed by <paramref name="browser"/>.
    /// </summary>
    public TPageObject CreateInitialPageObject<TPageObject> ([NotNull] BrowserSession browser)
        where TPageObject : PageObject
    {
      ArgumentUtility.CheckNotNull ("browser", browser);

      s_log.InfoFormat ("WebTestHelper.CreateInitialPageObject<" + typeof (TPageObject).FullName + "> has been called.");
      var context = PageObjectContext.New (browser);
      s_log.InfoFormat ("New PageObjectContext has been created.");

      var pageObject = (TPageObject) Activator.CreateInstance (typeof (TPageObject), new object[] { context });
      s_log.InfoFormat ("Initial PageObject has been created.");
      return pageObject;
    }

    /// <summary>
    /// Accepts a, possibly existent, modal dialog.
    /// </summary>
    public void AcceptPossibleModalDialog ()
    {
      try
      {
        _mainBrowserSession.AcceptModalDialog (Options.NoWait);
      }
      catch (MissingDialogException)
      {
        // It's okay.
      }
    }

    /// <summary>
    /// TearDown method for each web test.
    /// </summary>
    /// <param name="hasSucceeded">Specifies whether the test has been successful.</param>
    public void OnTearDown (bool hasSucceeded)
    {
      if (!hasSucceeded && ShouldTakeScreenshots())
      {
        var screenshotRecorder = new TestExecutionScreenshotRecorder (_testInfrastructureConfiguration.ScreenshotDirectory);
        screenshotRecorder.Capture();
        screenshotRecorder.TakeDesktopScreenshot (_testName);
        screenshotRecorder.TakeBrowserScreenshot (_testName, _browserSessions.Select (s => s.Value).ToArray(), BrowserConfiguration.Locator);
      }

      s_log.InfoFormat ("Finished test: {0} [has succeeded: {1}].", _testName, hasSucceeded);

      _browserConfiguration.DownloadHelper.DeleteFiles();
    }

    public ScreenshotBuilder CreateDesktopScreenshot ()
    {
      return new ScreenshotBuilder (Screenshot.TakeDesktopScreenshot(), BrowserConfiguration.Locator);
    }

    public ScreenshotBuilder CreateBrowserScreenshot (BrowserSession browserSession = null)
    {
      if (browserSession == null)
        browserSession = MainBrowserSession;

      return new ScreenshotBuilder (Screenshot.TakeBrowserScreenshot (browserSession, BrowserConfiguration.Locator), BrowserConfiguration.Locator);
    }

    private bool ShouldTakeScreenshots ()
    {
      return !string.IsNullOrEmpty (_testInfrastructureConfiguration.ScreenshotDirectory);
    }

    /// <summary>
    /// TearDown method for each web test fixture.
    /// </summary>
    public void OnFixtureTearDown ()
    {
      s_log.InfoFormat ("WebTestHelper.OnFixtureTearDown() has been called.");

      foreach (var browserSession in _browserSessions)
        browserSession.Dispose();

      s_log.InfoFormat ("{0} sessions have been disposed.", _browserSessions.Count);
      _browserSessions.Clear();
    }

    private void EnsureAllBrowserWindowsAreClosed ()
    {
      if (!_testInfrastructureConfiguration.CloseBrowserWindowsOnSetUpAndTearDown)
        return;

      s_log.InfoFormat ("Killing all processes named '{0}'.", _browserConfiguration.BrowserExecutableName);
      var browserProcessName = _browserConfiguration.BrowserExecutableName;
      if (browserProcessName == null)
        return;

      ProcessUtils.KillAllProcessesWithName (browserProcessName);
    }

    private void EnsureCursorIsOutsideBrowserWindow ()
    {
      Cursor.Position = new Point (0, 0);
    }
  }
}