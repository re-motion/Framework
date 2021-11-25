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
using OpenQA.Selenium.Remote;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebDriver;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BrowserSessionTest : IntegrationTest
  {
    [Test]
    public void DeleteAllCookies_DeletesAllCookies ()
    {
      var browserSession = Start();

      var allCookiesBeforeCreation = ((RemoteWebDriver)browserSession.Context.Browser.Driver.Native).Manage().Cookies.AllCookies;

      browserSession.WebButtons().GetByLocalID("CreateSessionCookie").Click();
      browserSession.WebButtons().GetByLocalID("CreatePersistentCookie").Click();

      var allCookiesBeforeDelete = ((RemoteWebDriver)browserSession.Context.Browser.Driver.Native).Manage().Cookies.AllCookies;

      Assert.That(allCookiesBeforeDelete.Count, Is.EqualTo(allCookiesBeforeCreation.Count + 2));

      browserSession.Context.Browser.DeleteAllCookies();

      var allCookiesAfterDelete = ((RemoteWebDriver)browserSession.Context.Browser.Driver.Native).Manage().Cookies.AllCookies;
      Assert.That(allCookiesAfterDelete.Count, Is.EqualTo(0));
    }

    [Test]
    public void ChromeDriver_SupportsBrowserLogs ()
    {
      if (!Helper.BrowserConfiguration.IsChrome())
        Assert.Ignore("Tests if ChromeDriver behaves as expected and hence, only concerns Chrome");

      TestDriverSupportsBrowserLogs();
    }

    [Test]
    public void MSEdgeDriver_SupportsBrowserLogs ()
    {
      if (!Helper.BrowserConfiguration.IsEdge())
        Assert.Ignore("Tests if MSEdgeDriver behaves as expected and hence, only concerns Microsoft Edge");

      TestDriverSupportsBrowserLogs();
    }

    [Test]
    public void GeckoDriver_DoesNotSupportBrowserLogs ()
    {
      if (!Helper.BrowserConfiguration.IsFirefox())
        Assert.Ignore("Tests if GeckoDriver behaves as expected and hence, only concerns Firefox");

      var home = Start();

      var selenium = (IWebDriver)home.Context.Browser.Driver.Native;

      Assert.That(
          () => selenium.Manage().Logs.GetLog(LogType.Browser),
          Throws.InstanceOf<NullReferenceException>().With.Message
              .EqualTo("Object reference not set to an instance of an object."));
    }

    private void TestDriverSupportsBrowserLogs ()
    {
      var home = Start();

      const string errorMessage = "777d20c6-58ac-4d51-bf22-625d1ab9e856";
      const string warningMessage = "f312fad3-ad79-4f68-aee3-c741dd0e7083";

      var js = JavaScriptExecutor.GetJavaScriptExecutor(home.Context.Browser);
      js.ExecuteScript($"console.error('{errorMessage}')");
      js.ExecuteScript($"console.warn('{warningMessage}')");

      var logs = home.Context.Browser.GetBrowserLogs();

      Assert.That(logs.Count(l => l.Message.Contains(errorMessage)), Is.EqualTo(1));
      Assert.That(logs.Count(l => l.Message.Contains(warningMessage)), Is.EqualTo(1));
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject>("BrowserSessionTest.wxe");
    }
  }
}
