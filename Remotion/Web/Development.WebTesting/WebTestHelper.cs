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
using System.Drawing;
using System.Windows.Forms;
using Coypu;
using JetBrains.Annotations;
using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Accessibility;
using Remotion.Web.Development.WebTesting.Accessibility.Implementation;
using Remotion.Web.Development.WebTesting.BrowserSession;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.RequestErrorDetectionStrategies;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration;
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
      return new WebTestHelper(new WebTestConfigurationFactory());
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
      return new WebTestHelper(new TFactory());
    }

    private static readonly ILog s_log = LogManager.GetLogger(typeof(WebTestHelper));

    private readonly IBrowserConfiguration _browserConfiguration;
    private readonly DriverConfiguration _driverConfiguration;
    private readonly ITestInfrastructureConfiguration _testInfrastructureConfiguration;
    private readonly List<IBrowserSession> _browserSessions = new List<IBrowserSession>();
    private IBrowserSession? _mainBrowserSession;
    private readonly IAccessibilityConfiguration _accessibilityConfiguration;

    /// <summary>
    /// Name of the current web test.
    /// </summary>
    private string? _testName;

    [PublicAPI]
    protected WebTestHelper ([NotNull] WebTestConfigurationFactory webTestConfigurationFactory)
    {
      ArgumentUtility.CheckNotNull("webTestConfigurationFactory", webTestConfigurationFactory);

      _browserConfiguration = webTestConfigurationFactory.CreateBrowserConfiguration();
      _driverConfiguration = webTestConfigurationFactory.CreateDriverConfiguration();
      _testInfrastructureConfiguration = webTestConfigurationFactory.CreateTestInfrastructureConfiguration();
      _accessibilityConfiguration = webTestConfigurationFactory.CreateAccessibilityConfiguration();
    }

    public IBrowserConfiguration BrowserConfiguration
    {
      get { return _browserConfiguration; }
    }

    [NotNull]
    public DriverConfiguration DriverConfiguration
    {
      get { return _driverConfiguration; }
    }

    public ITestInfrastructureConfiguration TestInfrastructureConfiguration
    {
      get { return _testInfrastructureConfiguration; }
    }

    /// <summary>
    /// Coypu main browser session for the web test.
    /// </summary>
    public IBrowserSession MainBrowserSession
    {
      get { return Assertion.IsNotNull(_mainBrowserSession, "OnFixtureSetup must be called before accessing MainBrowserSession."); }
    }

    /// <summary>
    /// SetUp method for each web test fixture.
    /// </summary>
    /// <param name="windowSize">Specifies the main browser session's window size. Maximizes the window if left <see langword="null"/>.</param>
    /// <param name="configurationOverride">Specifies additional options applied when creating the browser.</param>
    public void OnFixtureSetUp (WindowSize? windowSize = null, [CanBeNull] DriverConfigurationOverride? configurationOverride = null)
    {
      s_log.InfoFormat("WebTestHelper.OnFixtureSetup() has been called.");
      s_log.InfoFormat("Remotion version: " + typeof(WebTestHelper).Assembly.GetName().Version);
      s_log.InfoFormat("Selenium (WebDriver) version: " + typeof(IWebDriver).Assembly.GetName().Version);
      s_log.InfoFormat("Selenium Support (WebDriver.Support) version: " + typeof(WebDriverWait).Assembly.GetName().Version);
      s_log.InfoFormat("Coypu version: " + typeof(Element).Assembly.GetName().Version);

      // Note: otherwise the Selenium web driver may get confused when searching for windows.
      // Confusion could theoretically happen when calling Coypu.BrowserSession.FindWindow(string locator), as the window gets found per title.
      // This method only gets called when using ExpectNewWindow or ExpectNewPopWindow.
      // See RM-6731.
      EnsureAllBrowserWindowsAreClosed();

      _mainBrowserSession = CreateNewBrowserSession(windowSize, configurationOverride);
      s_log.InfoFormat("Browser: {0}, version {1}", _mainBrowserSession.Driver.GetBrowserName(), _mainBrowserSession.Driver.GetBrowserVersion());
      s_log.InfoFormat("WebDriver version: {0}", _mainBrowserSession.Driver.GetWebDriverVersion());

      // Note: otherwise cursor could interfere with element hovering.
      if (!_mainBrowserSession.Headless)
        EnsureCursorIsOutsideBrowserWindow();
    }

    /// <summary>
    /// SetUp method for each web test.
    /// </summary>
    /// <param name="testName">Name of the test being performed.</param>
    public void OnSetUp ([NotNull] string testName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("testName", testName);

      _testName = testName;
      s_log.InfoFormat("Executing test: {0}.", _testName);

      if (_mainBrowserSession != null)
        s_log.InfoFormat("Current window title: {0}.", _mainBrowserSession.Window.Title);
    }

    /// <summary>
    /// Creates a new browser session using the configured settings from <see cref="WebTestConfigurationFactory"/>.
    /// </summary>
    /// <param name="windowSize">Specifies the main browser session's window size. Maximizes the window if left <see langword="null"/>.</param>
    /// <param name="configurationOverride">Specifies additional options applied when creating the browser.</param>
    /// <returns>The new browser session.</returns>
    public IBrowserSession CreateNewBrowserSession (WindowSize? windowSize = null, [CanBeNull] DriverConfigurationOverride? configurationOverride = null)
    {
      using (new PerformanceTimer(s_log, string.Format("Created new {0} browser session.", _browserConfiguration.BrowserName)))
      {
        var mergedDriverConfiguration = MergeDriverConfiguration(_driverConfiguration, configurationOverride);

        var browserResult = _browserConfiguration.BrowserFactory.CreateBrowser(mergedDriverConfiguration);
        _browserSessions.Add(browserResult);

        windowSize ??= mergedDriverConfiguration.Headless ? new WindowSize(1280, 900) : WindowSize.Maximized;

        if (windowSize.IsMaximized)
          browserResult.Window.MaximiseWindow();
        else
          browserResult.Driver.ResizeTo(new Size(windowSize.Width, windowSize.Height), browserResult.Window);

        return browserResult;
      }
    }

    /// <summary>
    /// Returns a new <typeparamref name="TPageObject"/> for the initial page displayed by <paramref name="browser"/>.
    /// </summary>
    public TPageObject CreateInitialPageObject<TPageObject> ([NotNull] IBrowserSession browser)
        where TPageObject : PageObject
    {
      ArgumentUtility.CheckNotNull("browser", browser);

      return CreateInitialPageObject<TPageObject>(browser, _testInfrastructureConfiguration.RequestErrorDetectionStrategy);
    }

    /// <summary>
    /// Returns a new <typeparamref name="TPageObject"/> for the initial page displayed by <paramref name="browser"/> with a <see cref="NullRequestErrorDetectionStrategy"/>.
    /// </summary>
    public TPageObject CreateInitialPageObjectWithoutRequestErrorDetection<TPageObject> ([NotNull] IBrowserSession browser)
        where TPageObject : PageObject
    {
      ArgumentUtility.CheckNotNull("browser", browser);

      return CreateInitialPageObject<TPageObject>(browser, new NullRequestErrorDetectionStrategy());
    }

    private TPageObject CreateInitialPageObject<TPageObject> (IBrowserSession browser, IRequestErrorDetectionStrategy requestErrorDetectionStrategy)
        where TPageObject : PageObject
    {
      s_log.InfoFormat("WebTestHelper.CreateInitialPageObject<" + typeof(TPageObject).FullName + "> has been called.");
      var context = PageObjectContext.New(browser, requestErrorDetectionStrategy);
      s_log.InfoFormat("New PageObjectContext has been created.");

      requestErrorDetectionStrategy.CheckPageForError(context.Scope);

      var pageObject = (TPageObject)Activator.CreateInstance(typeof(TPageObject), new object[] { context })!;
      s_log.InfoFormat("Initial PageObject has been created.");
      return pageObject;
    }

    /// <summary>
    /// Accepts a, possibly existent, modal dialog.
    /// </summary>
    public void AcceptPossibleModalDialog ()
    {
      try
      {
        Assertion.IsNotNull(_mainBrowserSession, "'_mainBrowserSession' must not be null.");
        _mainBrowserSession.Window.AcceptModalDialog(Options.NoWait);
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
        Assertion.IsNotNull(_testName, "'{0}' should be set by the test infrastructure calling '{1}'", nameof(_testName), nameof(OnSetUp));
        var screenshotRecorder = new TestExecutionScreenshotRecorder(_testInfrastructureConfiguration.ScreenshotDirectory);
        screenshotRecorder.CaptureCursor();
        if (_mainBrowserSession is { Headless: false })
          screenshotRecorder.TakeDesktopScreenshot(_testName);
        screenshotRecorder.TakeBrowserScreenshot(_testName, _browserSessions.ToArray(), BrowserConfiguration.Locator);
      }

      s_log.InfoFormat("Finished test: {0} [has succeeded: {1}].", _testName, hasSucceeded);

      _browserConfiguration.DownloadHelper.DeleteFiles();
    }

    public ScreenshotBuilder CreateDesktopScreenshot ()
    {
      return new ScreenshotBuilder(Screenshot.TakeDesktopScreenshot(), BrowserConfiguration.Locator);
    }

    public ScreenshotBuilder CreateBrowserScreenshot (IBrowserSession? browserSession = null)
    {
      if (browserSession == null)
        browserSession = MainBrowserSession;

      return new ScreenshotBuilder(Screenshot.TakeBrowserScreenshot(browserSession, BrowserConfiguration.Locator), BrowserConfiguration.Locator);
    }

    private bool ShouldTakeScreenshots ()
    {
      return !string.IsNullOrEmpty(_testInfrastructureConfiguration.ScreenshotDirectory);
    }

    /// <summary>
    /// TearDown method for each web test fixture.
    /// </summary>
    public void OnFixtureTearDown ()
    {
      s_log.InfoFormat("WebTestHelper.OnFixtureTearDown() has been called.");

      foreach (var browserSession in _browserSessions)
        browserSession.Dispose();

      s_log.InfoFormat("{0} sessions have been disposed.", _browserSessions.Count);
      _browserSessions.Clear();
    }

    /// <summary>
    /// Uses the configured <see cref="IRequestErrorDetectionStrategy"/> to check if the given <paramref name="context"/> contains a request error.
    /// </summary>
    public void CheckPageForError ([NotNull] PageObjectContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      context.RequestErrorDetectionStrategy.CheckPageForError(context.Scope);
    }

    private void EnsureAllBrowserWindowsAreClosed ()
    {
      if (!_testInfrastructureConfiguration.CloseBrowserWindowsOnSetUpAndTearDown)
        return;

      s_log.InfoFormat("Killing all processes named '{0}'.", _browserConfiguration.BrowserExecutableName);
      var browserProcessName = _browserConfiguration.BrowserExecutableName;
      if (browserProcessName == null)
        return;

      ProcessUtils.KillAllProcessesWithName(browserProcessName);
    }

    private void EnsureCursorIsOutsideBrowserWindow ()
    {
      Cursor.Position = new Point(0, 0);
    }

    private DriverConfiguration MergeDriverConfiguration (DriverConfiguration configuration, DriverConfigurationOverride? configurationOverride)
    {
      if (configurationOverride == null)
        return configuration;

      return new DriverConfiguration(
          configurationOverride.CommandTimeout ?? configuration.CommandTimeout,
          configurationOverride.SearchTimeout ?? configuration.SearchTimeout,
          configurationOverride.RetryInterval ?? configuration.RetryInterval,
          configurationOverride.AsyncJavaScriptTimeout ?? configuration.AsyncJavaScriptTimeout,
          configurationOverride.Headless ?? configuration.Headless);
    }

    /// <summary>
    /// Returns an instance of AccessibilityAnalyzer.
    /// </summary>
    /// <returns>Initialized instance of AccessibilityAnalyzer</returns>
    public AccessibilityAnalyzer CreateAccessibilityAnalyzer ([NotNull] IBrowserSession browserSession)
    {
      ArgumentUtility.CheckNotNull("browserSession", browserSession);

      return AccessibilityAnalyzer.CreateForWebDriver(
          (IWebDriver)browserSession.Driver.Native,
          new AxeResultParser(),
          _accessibilityConfiguration,
          new AxeSourceProvider(),
          new AccessibilityResultMapper(),
          LogManager.GetLogger(typeof(WebTestHelper).Assembly, typeof(AccessibilityAnalyzer)));
    }

    /// <summary>
    /// Returns an instance of AccessibilityAnalyzer.
    /// </summary>
    /// <returns>Initialized instance of AccessibilityAnalyzer</returns>
    public AccessibilityAnalyzer CreateAccessibilityAnalyzer ()
    {
      Assertion.IsNotNull(_mainBrowserSession, "No MainBrowserSession is available.");
      return CreateAccessibilityAnalyzer(_mainBrowserSession);
    }
  }
}
