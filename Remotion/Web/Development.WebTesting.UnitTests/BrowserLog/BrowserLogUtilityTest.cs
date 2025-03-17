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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Coypu;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Remotion.Web.Development.WebTesting.BrowserLog;
using Remotion.Web.Development.WebTesting.BrowserSession;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Firefox;

namespace Remotion.Web.Development.WebTesting.UnitTests.BrowserLog;

[TestFixture]
public class BrowserLogUtilityTest
{
  [SetUp]
  public void Setup ()
  {
  }

  [TearDown]
  public void TearDown ()
  {
  }

  [Test]
  public void IsBrowserLogCheckActive_Default_False ()
  {
    var testContextMock = new Mock<ITestContext>();
    testContextMock.Setup(_ => _.Properties).Returns(new Dictionary<string, object>());

    var result = BrowserLogUtility.IsBrowserLogCheckActive(testContextMock.Object);
    Assert.That(result, Is.EqualTo(false));
  }

  [Test]
  [PerformBrowserLogCheck(true)]
  public void IsBrowserLogCheckActive_IsSetFromTestContext ()
  {
    var testContextMock = new Mock<ITestContext>();
    testContextMock.Setup(_ => _.Properties).Returns(
        new Dictionary<string, object>
        {
            { PerformBrowserLogCheckAttribute.PropertyKey, true }
        });

    var result = BrowserLogUtility.IsBrowserLogCheckActive(testContextMock.Object);
    Assert.That(result, Is.EqualTo(true));
  }

  [Test]
  public void GetMinimumLogLevel_Default_Off ()
  {
    var testContextMock = new Mock<ITestContext>();
    testContextMock.Setup(_ => _.Properties).Returns(new Dictionary<string, object>());

    var result = BrowserLogUtility.GetMinimumLogLevel(testContextMock.Object);
    Assert.That(result, Is.EqualTo(LogLevel.Off));
  }

  [Test]
  public void GetMinimumLogLevel_IsSetFromTestContext ()
  {
    var testContextMock = new Mock<ITestContext>();
    testContextMock.Setup(_ => _.Properties).Returns(
        new Dictionary<string, object>()
        {
            { BrowserLogMinimumLevelAttribute.PropertyKey, LogLevel.Info }
        });

    var result = BrowserLogUtility.GetMinimumLogLevel(testContextMock.Object);
    Assert.That(result, Is.EqualTo(LogLevel.Info));
  }

  [Test]
  public void GetBrowserLogIgnoredEntriesPredicates_Default_Empty ()
  {
    var testContextMock = new Mock<ITestContext>();
    testContextMock.Setup(_ => _.Properties).Returns(new Dictionary<string, object>());

    var result = BrowserLogUtility.GetBrowserLogIgnoredEntryRegexes(testContextMock.Object);
    Assert.That(result, Is.Empty);
  }

  [Test]
  public void GetBrowserLogIgnoredEntriesPredicates_IsSetFromTestContext ()
  {
    var testContextMock = new Mock<ITestContext>();
    var regexFilters = new[] { new Regex("ignored"), new Regex("disregarded"), new Regex("omitted") };
    testContextMock.Setup(_ => _.Properties).Returns(
        new Dictionary<string, object>
        {
            { IgnoreBrowserLogMessageAttribute.PropertyKey, regexFilters }
        });

    var result = BrowserLogUtility.GetBrowserLogIgnoredEntryRegexes(testContextMock.Object);
    Assert.That(result, Is.EquivalentTo(regexFilters));
  }

  [Test]
  public void GetUnexpectedBrowserLogEntries_FiltersLogEntries_ByMinimumLevel_AndIgnoreDelegate ()
  {
    var browserLogEntries = new[]
                            {
                                new BrowserLogEntry(LogLevel.Info, "too low", DateTime.Now),
                                new BrowserLogEntry(LogLevel.Severe, "ignored", DateTime.Now),
                                new BrowserLogEntry(LogLevel.Warning, "also ignored", DateTime.Now),
                                new BrowserLogEntry(LogLevel.Warning, "included", DateTime.Now),
                                new BrowserLogEntry(LogLevel.Severe, "also included", DateTime.Now),
                            };

    var browserSession = new Mock<IBrowserSession>(MockBehavior.Strict);
    browserSession.Setup(_ => _.GetBrowserLogs()).Returns(browserLogEntries);

    var result = BrowserLogUtility.GetUnexpectedBrowserLogEntries(
        browserSession.Object,
        Mock.Of<IBrowserConfiguration>(),
        LogLevel.Warning,
        [new Regex(".*ignored")]);
    Assert.That(result, Is.EquivalentTo(new[] { browserLogEntries[3], browserLogEntries[4] }));
  }

  [Test]
  public void GetUnexpectedBrowserLogEntries_WithBidiButChromeUrl_DoesNotExecuteBidiLogic ()
  {
    var browserLogEntries = new[] { new BrowserLogEntry(LogLevel.Warning, "my message", DateTime.Now) };

    var fakeBrowserWindow = CreateFakeBrowserWindow(new Uri("chrome://blank"));

    var browserSessionMock = new Mock<IBrowserSession>(MockBehavior.Strict);
    browserSessionMock.Setup(_ => _.GetBrowserLogs()).Returns(browserLogEntries);
    browserSessionMock.Setup(_ => _.Window).Returns(fakeBrowserWindow);

    var browserConfigurationMock = new Mock<IBrowserConfiguration>(MockBehavior.Strict);
    browserConfigurationMock.As<IFirefoxConfiguration>();

    var result = BrowserLogUtility.GetUnexpectedBrowserLogEntries(
        browserSessionMock.Object,
        browserConfigurationMock.Object,
        LogLevel.Warning,
        []);
    Assert.That(result, Is.EquivalentTo(browserLogEntries));
  }

  [Test]
  public void GetUnexpectedBrowserLogEntries_WithBidi_DoesNotReturnMarkerLogEntry ()
  {
    var browserLogEntries = new[] { new BrowserLogEntry(LogLevel.Warning, "my message", DateTime.Now) };

    var fakeBrowserWindow = CreateFakeBrowserWindow(new Uri("http://mywebpage"));

    var webDriverMock = new Mock<IWebDriver>(MockBehavior.Strict);
    var javaScriptExecutorMock = webDriverMock.As<IJavaScriptExecutor>();
    javaScriptExecutorMock.Setup(_ => _.ExecuteScript("console.error('REMOTION_FINAL_BROWSER_LOG_MARKER');", It.Is<object[]>(e => e.Length == 0))).Returns(null);

    var getBrowserLogsCallTimes = 0;
    var browserSessionMock = new Mock<IBrowserSession>(MockBehavior.Strict);
    browserSessionMock.Setup(_ => _.Driver.Native).Returns(webDriverMock.Object);
    browserSessionMock.Setup(_ => _.Window).Returns(fakeBrowserWindow);
    browserSessionMock
            .Setup(_ => _.GetBrowserLogs())
            .Returns(
                    () =>
                    {
                        getBrowserLogsCallTimes += 1;
                        return getBrowserLogsCallTimes > 3
                                ? [..browserLogEntries, new BrowserLogEntry(LogLevel.Warning, "REMOTION_FINAL_BROWSER_LOG_MARKER", DateTime.Now)]
                                : browserLogEntries;
                    });

    var browserConfigurationMock = new Mock<IBrowserConfiguration>(MockBehavior.Strict);
    browserConfigurationMock.As<IFirefoxConfiguration>();

    var result = BrowserLogUtility.GetUnexpectedBrowserLogEntries(
        browserSessionMock.Object,
        browserConfigurationMock.Object,
        LogLevel.Warning,
        []);
    Assert.That(result, Is.EquivalentTo(browserLogEntries));

    javaScriptExecutorMock.Verify(_ => _.ExecuteScript(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
    browserSessionMock.Verify(_ => _.GetBrowserLogs(), Times.Exactly(4));
  }

  [Test]
  public void GetUnexpectedBrowserLogEntries_WithBidi_FailsAfterRetries ()
  {
    var browserLogEntries = new[] { new BrowserLogEntry(LogLevel.Warning, "my message", DateTime.Now) };

    var fakeBrowserWindow = CreateFakeBrowserWindow(new Uri("http://mywebpage"));

    var webDriverMock = new Mock<IWebDriver>();
    webDriverMock.As<IJavaScriptExecutor>();

    var browserSessionMock = new Mock<IBrowserSession>(MockBehavior.Strict);
    browserSessionMock.Setup(_ => _.Driver.Native).Returns(webDriverMock.Object);
    browserSessionMock.Setup(_ => _.Window).Returns(fakeBrowserWindow);
    browserSessionMock.Setup(_ => _.GetBrowserLogs()).Returns(() => browserLogEntries);

    var browserConfigurationMock = new Mock<IBrowserConfiguration>(MockBehavior.Strict);
    browserConfigurationMock.As<IFirefoxConfiguration>();

    Assert.That(
        () => BrowserLogUtility.GetUnexpectedBrowserLogEntries(
            browserSessionMock.Object,
            browserConfigurationMock.Object,
            LogLevel.Warning,
            []),
        Throws.InvalidOperationException.With.Message.StartsWith("Waiting for unexpected browser"));
  }

  [Test]
  public void CheckBrowserLog_WhenCheckInactive_DoesNothing ()
  {
    var testContext = new Mock<ITestContext>(MockBehavior.Strict);
    testContext.Setup(_ => _.Properties).Returns(new Dictionary<string, object> { { PerformBrowserLogCheckAttribute.PropertyKey, false } });

    var browserSession = new Mock<IBrowserSession>(MockBehavior.Strict);

    BrowserLogUtility.IsBrowserLogOkay(browserSession.Object, Mock.Of<IBrowserConfiguration>(), testContext.Object);
  }

  [Test]
  public void CheckBrowserLog_WithoutUnexpectedLogEntries_DoesNothing ()
  {
    var testContext = new Mock<ITestContext>(MockBehavior.Strict);
    testContext.Setup(_ => _.Properties).Returns(
        new Dictionary<string, object>
        {
            { PerformBrowserLogCheckAttribute.PropertyKey, true },
            { BrowserLogMinimumLevelAttribute.PropertyKey, LogLevel.Warning },
            { IgnoreBrowserLogMessageAttribute.PropertyKey, Array.Empty<Regex>() }
        });

    var browserSession = new Mock<IBrowserSession>(MockBehavior.Strict);
    browserSession.Setup(_ => _.GetBrowserLogs()).Returns(Array.Empty<BrowserLogEntry>());

    BrowserLogUtility.IsBrowserLogOkay(browserSession.Object, Mock.Of<IBrowserConfiguration>(), testContext.Object);

    testContext.Verify(_ => _.SetFailure(It.IsAny<string>()), Times.Never);
  }

  [Test]
  public void CheckBrowserLog_WithUnexpectedLogEntries_SetsFailure ()
  {
    var testContext = new Mock<ITestContext>(MockBehavior.Strict);
    testContext.Setup(_ => _.Properties).Returns(
        new Dictionary<string, object>
        {
            { PerformBrowserLogCheckAttribute.PropertyKey, true },
            { BrowserLogMinimumLevelAttribute.PropertyKey, LogLevel.Warning },
            { IgnoreBrowserLogMessageAttribute.PropertyKey, new[] { new Regex("ignored") } }
        });

    testContext.Setup(_ => _.SetFailure($"There are unexpected browser log entries at the end of the test:{Environment.NewLine}error{Environment.NewLine}warning"));

    var browserSession = new Mock<IBrowserSession>(MockBehavior.Strict);
    browserSession.Setup(_ => _.GetBrowserLogs())
        .Returns(
        [
            new BrowserLogEntry(LogLevel.Severe, "error", DateTime.Now),
            new BrowserLogEntry(LogLevel.Info, "info", DateTime.Now),
            new BrowserLogEntry(LogLevel.Severe, "ignored", DateTime.Now),
            new BrowserLogEntry(LogLevel.Warning, "warning", DateTime.Now),
        ]);

    BrowserLogUtility.IsBrowserLogOkay(browserSession.Object, Mock.Of<IBrowserConfiguration>(), testContext.Object);

    testContext.Verify(_ => _.SetFailure(It.IsAny<string>()), Times.Once);
  }

  [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_driver")]
  private static extern ref IDriver GetBrowserWindowDriverRef (DriverScope @this);

  private static BrowserWindow CreateFakeBrowserWindow (Uri uri)
  {
    // I am sorry... but there is no good way to mock BrowserWindow.
    // As such, we do some trickery just to get window.Location to work which we need for the test.
    var driverStub = new Mock<IDriver>(MockBehavior.Strict);
    driverStub.Setup(_ => _.Location(It.IsAny<Scope>())).Returns(uri);

    var browserWindow = (BrowserWindow)RuntimeHelpers.GetUninitializedObject(typeof(BrowserWindow));
    GetBrowserWindowDriverRef(browserWindow) = driverStub.Object;

    return browserWindow;
  }
}
