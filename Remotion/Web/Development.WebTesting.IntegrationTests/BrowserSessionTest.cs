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
using NUnit.Framework;
using OpenQA.Selenium.Remote;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BrowserSessionTest : IntegrationTest
  {
    [Test]
    public void DeleteAllCookies_DeletesAllCookies ()
    {
      var browserSession = Start();

      var allCookiesBeforeCreation = ((RemoteWebDriver) browserSession.Context.Browser.Driver.Native).Manage().Cookies.AllCookies;

      browserSession.WebButtons().GetByLocalID ("CreateSessionCookie").Click();
      browserSession.WebButtons().GetByLocalID ("CreatePersistentCookie").Click();

      var allCookiesBeforeDelete = ((RemoteWebDriver) browserSession.Context.Browser.Driver.Native).Manage().Cookies.AllCookies;

      Assert.That (allCookiesBeforeDelete.Count, Is.EqualTo (allCookiesBeforeCreation.Count + 2));

      browserSession.Context.Browser.DeleteAllCookies();

      var allCookiesAfterDelete = ((RemoteWebDriver) browserSession.Context.Browser.Driver.Native).Manage().Cookies.AllCookies;
      Assert.That (allCookiesAfterDelete.Count, Is.EqualTo (0));
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject> ("BrowserSessionTest.wxe");
    }
  }


}
