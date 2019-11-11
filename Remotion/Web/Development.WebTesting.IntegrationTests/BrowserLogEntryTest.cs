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
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using Remotion.Web.Development.WebTesting.BrowserSession;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebDriver;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  public class BrowserLogEntryTest : IntegrationTest
  {
    [Ignore ("The Selenium log endpoints for Chrome are currently not working. (RM-7278)")]
    [Test]
    public void BrowserLogEntry_ShouldWrapSeleniumLogEntry ()
    {
      if (!Helper.BrowserConfiguration.IsChrome())
        Assert.Ignore ("Getting the browser log entries is currently only supported by Chrome.");

      var home = Start();

      var errorMessage = Guid.NewGuid().ToString();
      var maxExpectedTestRunTime = TimeSpan.FromSeconds (10);

      var js = JavaScriptExecutor.GetJavaScriptExecutor (home.Context.Browser);
      js.ExecuteScript ($"console.error('{errorMessage}')");

      var browserLogEntries = ((IWebDriver) home.Context.Browser.Driver.Native)
          .Manage().Logs.GetLog (LogType.Browser);

      var errorLogEntry = browserLogEntries.Single (log => log.Message.Contains (errorMessage));
      var wrappedErrorLogEntry = new BrowserLogEntry (errorLogEntry);

      Assert.That (wrappedErrorLogEntry.Level, Is.EqualTo (LogLevel.Severe));
      Assert.That (
          wrappedErrorLogEntry.Timestamp,
          Is.InRange (DateTime.UtcNow.Subtract (maxExpectedTestRunTime), DateTime.UtcNow.Add (maxExpectedTestRunTime)));
      Assert.That (wrappedErrorLogEntry.Message, Is.StringMatching ($@"^console-api \d+:\d+ ""{errorMessage}""$"));

      var logAsStringPattern = $@"^\[\d\d\d\d-\d\d-\d\dT\d\d:\d\d:\d\dZ\] \[Severe\] console-api \d+:\d+ ""{errorMessage}""$";

      Assert.That (
          wrappedErrorLogEntry.ToString(),
          Is.StringMatching (logAsStringPattern));
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject> ("BrowserSessionTest.wxe");
    }
  }
}