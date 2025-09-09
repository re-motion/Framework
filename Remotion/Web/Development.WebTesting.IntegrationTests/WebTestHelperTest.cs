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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Remotion.Web.Development.WebTesting.BrowserLog;
using Remotion.Web.Development.WebTesting.BrowserSession;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebDriver;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chrome;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Edge;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class WebTestHelperTest
  {
    private static readonly HashSet<string> s_relevantProcessNames = new()
                                                                     {
                                                                         "chromedriver",
                                                                         "chrome",
                                                                         "msedgedriver",
                                                                         "edge",
                                                                         "geckodriver",
                                                                         "firefox"
                                                                     };

    /// <summary>
    /// Filter for the driver process names that might be left open if something went wrong.
    /// Used to close/kill leftover processes after the execution of each test.
    /// </summary>
    private static bool RelevantProcessFilter (Process process)
    {
      return s_relevantProcessNames.Contains(process.ProcessName);
    }

    private ProcessSnapshot _beforeTestProcessSnapshot;
    private Mock<ITestContext> _testContext;
    private Dictionary<string,object> _testContextProperties;

    [OneTimeSetUp]
    public void OneTimeSetUp ()
    {
      var webTestHelper = WebTestHelper.CreateFromConfiguration<CustomWebTestConfigurationFactory>();
      //TODO: RM-9595 fix and reenable WebTestHelperTests for firefox
      //WebTestHelperTests currently cause firefox with Bidi active to hang
      if (webTestHelper.BrowserConfiguration.IsFirefox())
      {
        Assert.Ignore();
      }
    }

    [SetUp]
    public void SetUp ()
    {
      _beforeTestProcessSnapshot = ProcessSnapshot.CreateWithFilter(RelevantProcessFilter);
    }

    [TearDown]
    public void TearDown ()
    {
      var afterTestsProcessSnapshot = ProcessSnapshot.CreateWithFilter(RelevantProcessFilter);
      var processesStillOpenSnapshot = _beforeTestProcessSnapshot.Difference(afterTestsProcessSnapshot);
      ProcessUtils.GracefulProcessShutdown(processesStillOpenSnapshot.Processes, TimeSpan.FromSeconds(10));
      _testContext = null;
    }

    [Test]
    public void WebTestHelper_OnTestSetUp_ResetsBrowserLog ()
    {
      var webTestHelper = WebTestHelper.CreateFromConfiguration<CustomWebTestConfigurationFactory>();
      SetupWebTestHelper(webTestHelper);
      if (webTestHelper.BrowserConfiguration.UseBidiLog())
        webTestHelper.MainBrowserSession.Window.Visit(webTestHelper.TestInfrastructureConfiguration.WebApplicationRoot + "Empty.wxe");

      webTestHelper.MainBrowserSession.ResetBrowserLogs();
      var js = JavaScriptExecutor.GetJavaScriptExecutor(webTestHelper.MainBrowserSession);
      js.ExecuteScript("console.error('any error message')");

      IReadOnlyCollection<BrowserLogEntry> browserLogEntries = null;
      var logger = webTestHelper.LoggerFactory.CreateLogger("BrowserLogEntryTest");
      RetryUntilTimeout.Run(
          logger,
          () =>
          {
            browserLogEntries = webTestHelper.MainBrowserSession.GetBrowserLogs();
            if (browserLogEntries.Count == 0)
              throw new AssertionException("No browser logs found");
          });

      Assert.That(browserLogEntries.Count, Is.EqualTo(1));
      Assert.That(browserLogEntries.Single().Message, Does.Contain("any error message"));

      webTestHelper.OnSetUp(_testContext.Object);
      browserLogEntries = webTestHelper.MainBrowserSession.GetBrowserLogs();
      Assert.That(browserLogEntries, Is.Empty);
    }

    [Test]
    public void WebTestHelper_OnTestTearDown_WhenBrowserLogEntriesExist_SetsFailure ()
    {
      var webTestHelper = WebTestHelper.CreateFromConfiguration<CustomWebTestConfigurationFactory>();
      SetupWebTestHelper(webTestHelper);
      if (webTestHelper.BrowserConfiguration.UseBidiLog())
        webTestHelper.MainBrowserSession.Window.Visit(webTestHelper.TestInfrastructureConfiguration.WebApplicationRoot + "Empty.wxe");

      var testContext = new Mock<ITestContext>();
      testContext.Setup(_ => _.TestName).Returns("Test");
      testContext.Setup(_ => _.IsSuccessful).Returns(true);
      testContext.Setup(_ => _.Properties).Returns(
          new Dictionary<string, object>
          {
              { PerformBrowserLogCheckAttribute.PropertyKey, true },
              { BrowserLogMinimumLevelAttribute.PropertyKey, LogLevel.All },
              { IgnoreBrowserLogMessageAttribute.PropertyKey, Array.Empty<Regex>() }
          });

      testContext.Setup(_ => _.SetFailure(It.IsAny<string>())).Verifiable();
      webTestHelper.OnSetUp(testContext.Object);

      var js = JavaScriptExecutor.GetJavaScriptExecutor(webTestHelper.MainBrowserSession);
      js.ExecuteScript("console.error('any error message')");

      IReadOnlyCollection<BrowserLogEntry> browserLogEntries;
      var logger = webTestHelper.LoggerFactory.CreateLogger("BrowserLogEntryTest");
      RetryUntilTimeout.Run(
          logger,
          () =>
          {
            browserLogEntries = webTestHelper.MainBrowserSession.GetBrowserLogs();
            if (browserLogEntries.Count == 0)
              throw new AssertionException("No browser logs found");
          });

      webTestHelper.OnTearDown();
      testContext.Verify(
          _ => _.SetFailure(It.IsRegex($"There are unexpected browser log entries at the end of the test:{Environment.NewLine}.*\"?any error message\"?")),
          Times.Once());
    }

    [Test]
    public void WebTestHelper_OnTestFixtureTearDown_ClosesMainBrowserSessionProcesses ([Values(true, false)] bool testSuccess)
    {
      var webTestHelper = WebTestHelper.CreateFromConfiguration<CustomWebTestConfigurationFactory>();

      var configuration = webTestHelper.BrowserConfiguration;

      // Filter used when taking process snapshots
      var filter = CreateFilterForConfiguration(configuration);

      // Start and stop the web helper while taking process snapshots
      var beforeStart = ProcessSnapshot.CreateWithFilter(filter);
      SetupWebTestHelper(webTestHelper);
      var afterStart = ProcessSnapshot.CreateWithFilter(filter);
      ShutdownWebTestHelper(webTestHelper, testSuccess);
      var afterExit = ProcessSnapshot.CreateWithFilter(filter);

      var startedDiff = beforeStart.Difference(afterStart);
      var stoppedDiff = afterExit.Difference(afterStart);

      // Check if driver and browser are started
      Assert.That(
          startedDiff.GetProcessCount(configuration.WebDriverExecutableName),
          Is.EqualTo(1),
          "The web driver has not been started.");
      Assert.That(
          startedDiff.GetProcessCount(configuration.BrowserExecutableName),
          Is.GreaterThan(0),
          "The browser has not been started.");

      // Check if driver and browser are closed
      Assert.That(
          stoppedDiff.GetProcessCount(configuration.WebDriverExecutableName),
          Is.EqualTo(1),
          "The web driver has not been closed.");
      Assert.That(
          stoppedDiff.GetProcessCount(configuration.BrowserExecutableName),
          Is.EqualTo(startedDiff.GetProcessCount(configuration.BrowserExecutableName)),
          "The browser has not been closed.");
    }

    [Test]
    public void WebTestHelper_OnTestFixtureTearDown_ClosesAllBrowserSessionsProcesses ([Values(true, false)] bool testSuccess)
    {
      // Create a uninitialized WebTestHelper
      var webTestHelper = WebTestHelper.CreateFromConfiguration<CustomWebTestConfigurationFactory>();

      var configuration = webTestHelper.BrowserConfiguration;

      // Filter used when taking process snapshots
      var filter = CreateFilterForConfiguration(configuration);

      // The amount of browser sessions to open
      const int browserSessions = 3;

      // Start and stop the web helper while taking process snapshots
      var beforeStart = ProcessSnapshot.CreateWithFilter(filter);
      SetupWebTestHelper(webTestHelper);

      for (var i = 0; i < browserSessions; i++)
        webTestHelper.CreateNewBrowserSession(new WindowSize(500, 500));

      var afterStart = ProcessSnapshot.CreateWithFilter(filter);
      ShutdownWebTestHelper(webTestHelper, testSuccess);
      var afterExit = ProcessSnapshot.CreateWithFilter(filter);

      // All processes that started during Setup
      var startedDiff = beforeStart.Difference(afterStart);
      var stoppedDiff = afterExit.Difference(afterStart);

      // Check if driver and browser are started
      Assert.That(
          startedDiff.GetProcessCount(configuration.WebDriverExecutableName),
          Is.EqualTo(browserSessions + 1),
          "The web drivers have not been started.");
      Assert.That(
          startedDiff.GetProcessCount(configuration.BrowserExecutableName),
          Is.GreaterThan(0),
          "The browsers have not been started.");

      // Check if driver and browser are closed
      Assert.That(
          stoppedDiff.GetProcessCount(configuration.WebDriverExecutableName),
          Is.EqualTo(browserSessions + 1),
          "The web drivers have not been closed.");
      Assert.That(
          stoppedDiff.GetProcessCount(configuration.BrowserExecutableName),
          Is.EqualTo(startedDiff.GetProcessCount(configuration.BrowserExecutableName)),
          "The browsers have not been closed.");
    }

    [Test]
    public void WebTestHelper_OnTestFixtureTearDown_CleansUserDirectory_Chromium ([Values(true, false)] bool testSuccess)
    {
      var webTestHelper = WebTestHelper.CreateFromConfiguration<CustomWebTestConfigurationFactory>();

      if (!webTestHelper.BrowserConfiguration.IsChromium())
        Assert.Ignore("This test is specific for Chromium browsers.");

      var userDirectoryRoot = (webTestHelper.BrowserConfiguration as IChromeConfiguration)?.UserDirectoryRoot
                              ?? (webTestHelper.BrowserConfiguration as IEdgeConfiguration)?.UserDirectoryRoot;

      // Simulate test
      SetupWebTestHelper(webTestHelper);
      ShutdownWebTestHelper(webTestHelper, testSuccess);

      // The user directory root should no longer exist
      Assert.That(
          Directory.Exists(userDirectoryRoot),
          Is.False,
          string.Format("User directory root '{0}' is not cleaned up.", userDirectoryRoot));
    }

    private Func<Process, bool> CreateFilterForConfiguration (IBrowserConfiguration configuration) =>
        p => (string.Equals(p.ProcessName, configuration.BrowserExecutableName, StringComparison.OrdinalIgnoreCase)
              || string.Equals(p.ProcessName, configuration.WebDriverExecutableName, StringComparison.OrdinalIgnoreCase))
             && !p.HasExited;

    /// <summary>
    /// Calls <see cref="WebTestHelper.OnFixtureSetUp"/> and <see cref="WebTestHelper.OnSetUp"/> on the specified <see cref="WebTestHelper"/>.
    /// </summary>
    private void SetupWebTestHelper (WebTestHelper webTestHelper)
    {
      webTestHelper.OnFixtureSetUp();

      _testContextProperties = new Dictionary<string, object>();

      _testContext = new Mock<ITestContext>();
      _testContext.Setup(_ => _.TestName).Returns(GetType().Name + "_" + TestContext.CurrentContext.Test.Name);
      _testContext.Setup(_ => _.Properties).Returns(_testContextProperties);

      webTestHelper.OnSetUp(_testContext.Object);
    }

    /// <summary>
    /// Calls <see cref="WebTestHelper.OnTearDown"/> and <see cref="WebTestHelper.OnFixtureTearDown"/> on the specified <see cref="WebTestHelper"/>.
    /// Cleans up screenshots created by the <see cref="WebTestHelper.OnTearDown"/> method call.
    /// </summary>
    private void ShutdownWebTestHelper (WebTestHelper webTestHelper, bool success)
    {
      var screenshotDirectory = webTestHelper.TestInfrastructureConfiguration.ScreenshotDirectory;
      var screenshotDirectoryBeforeShutdown = Directory.GetFiles(screenshotDirectory);

      _testContext.Setup(_=>_.IsSuccessful).Returns(success);
      webTestHelper.OnTearDown();

      var screenshotsCreatedByWebTestHelperShutDown = Directory.GetFiles(screenshotDirectory).Except(screenshotDirectoryBeforeShutdown);

      foreach (var screenshotFileName in screenshotsCreatedByWebTestHelperShutDown)
      {
        File.Delete(Path.Combine(screenshotDirectory, screenshotFileName));
      }

      webTestHelper.OnFixtureTearDown();
    }


    /// <summary>
    /// Represents a list of running processes at a specific point in time.
    /// </summary>
    private class ProcessSnapshot
    {
      /// <summary>
      /// Creates a new <see cref="ProcessSnapshot"/> from the currently running processes filtered by <paramref name="filter"/>.
      /// </summary>
      public static ProcessSnapshot CreateWithFilter (Func<Process, bool> filter)
      {
        return new ProcessSnapshot(Process.GetProcesses().Where(filter).ToArray());
      }

      private readonly Process[] _processes;
      private readonly Dictionary<string, int> _processCount;

      private ProcessSnapshot (Process[] processes)
      {
        _processes = processes;
        _processCount = _processes.GroupBy(p => p.ProcessName).ToDictionary(g => g.Key, g => g.Count());
      }

      public IReadOnlyList<Process> Processes => _processes;

      /// <summary>
      /// Returns a new <see cref="ProcessSnapshot"/> containing all processes that are in <paramref name="snapshot"/> but not in <c>this</c>.
      /// </summary>
      /// <param name="snapshot">The snapshot to compare itself to.</param>
      /// <returns>The difference of <c>this</c> to the specified snapshot.</returns>
      public ProcessSnapshot Difference (ProcessSnapshot snapshot)
      {
        return new ProcessSnapshot(snapshot._processes.Where(process => this._processes.All(p => p.Id != process.Id)).ToArray());
      }

      public int GetProcessCount (string processName)
      {
        int count;
        if (_processCount.TryGetValue(processName, out count))
        {
          return count;
        }
        return 0;
      }
    }
  }
}
