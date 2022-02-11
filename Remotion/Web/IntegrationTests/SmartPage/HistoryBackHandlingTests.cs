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
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.IntegrationTests.SmartPage
{
  [TestFixture]
  public class HistoryBackHandlingTests : IntegrationTest
  {
    [Test]
    public void PostBacks_IncreaseSessionStorageEntries ()
    {
      var home = StartWithCleanSessionStorage("WebTabStripTest.wxe");
      var jsExecutor = JavaScriptExecutor.GetJavaScriptExecutor(home.Scope);
      var initialSessionStorageEntryCount = GetSessionStorageEntryCount(jsExecutor);

      home.WebTabStrips().First().SwitchTo("Tab2");
      home.WebTabStrips().First().SwitchTo("Tab1");

      var sessionStorageEntryCount = GetSessionStorageEntryCount(jsExecutor);
      var sessionStorageEntryKeyValue = GetSessionStorageEntry(jsExecutor, "pageTokenEntryCount");
      var sessionStorageEntryCountDifference = sessionStorageEntryCount - initialSessionStorageEntryCount;
      Assert.That(sessionStorageEntryCountDifference, Is.EqualTo(2));
      Assert.That(sessionStorageEntryKeyValue, Is.EqualTo((sessionStorageEntryCount - 1).ToString()));
    }

    [Test]
    public void NavigateToSamePage_IncreasesSessionStorageEntries ()
    {
      var initialPage = StartWithCleanSessionStorage("WebTabStripTest.wxe");
      var jsExecutor = JavaScriptExecutor.GetJavaScriptExecutor(initialPage.Scope);
      var initialSessionStorageEntryCount = GetSessionStorageEntryCount(jsExecutor);

      Start<WxePageObject>("WebTabStripTest.wxe");

      var sessionStorageEntryCount = GetSessionStorageEntryCount(jsExecutor);
      var sessionStorageEntryKeyValue = GetSessionStorageEntry(jsExecutor, "pageTokenEntryCount");
      var sessionStorageEntryCountDifference = sessionStorageEntryCount - initialSessionStorageEntryCount;
      Assert.That(sessionStorageEntryCountDifference, Is.EqualTo(1));
      Assert.That(sessionStorageEntryKeyValue, Is.EqualTo((sessionStorageEntryCount - 1).ToString()));
    }

    [Test]
    public void NavigateToDifferentPage_IncreasesSessionStorageEntries ()
    {
      var initialPage = StartWithCleanSessionStorage("WebButtonTest.wxe");
      var jsExecutor = JavaScriptExecutor.GetJavaScriptExecutor(initialPage.Scope);
      var initialSessionStorageEntryCount = GetSessionStorageEntryCount(jsExecutor);

      Start<WxePageObject>("WebTabStripTest.wxe");

      var sessionStorageEntryCount = GetSessionStorageEntryCount(jsExecutor);
      var sessionStorageEntryKeyValue = GetSessionStorageEntry(jsExecutor, "pageTokenEntryCount");
      var sessionStorageEntryCountDifference = sessionStorageEntryCount - initialSessionStorageEntryCount;
      Assert.That(sessionStorageEntryCountDifference, Is.EqualTo(1));
      Assert.That(sessionStorageEntryKeyValue, Is.EqualTo((sessionStorageEntryCount - 1).ToString()));
    }

    [Test]
    public void NewSessionStorageEntry_OverThreshold_ClearsOldEntries ()
    {
      var initialPage = StartWithCleanSessionStorage("WebButtonTest.wxe");
      var jsExecutor = JavaScriptExecutor.GetJavaScriptExecutor(initialPage.Scope);
      var firstPageToken = initialPage.Scope.FindCss("#SmartPage_TokenField").Value;
      Func<string> getFirstSessionKeyValue = () => GetSessionStorageEntry(jsExecutor, "pt_" + firstPageToken);
      Assert.That(getFirstSessionKeyValue(), Is.Not.Null);

      var pageTokensThatShouldNotBeRemoved = new List<string>();
      foreach (var i in Enumerable.Range(0, 149))
      {
        var page = Start<WxePageObject>("WebTabStripTest.wxe");
        if (i > 49)
        {
          var pageTokenValue = GetPageTokenValue(page);
          pageTokensThatShouldNotBeRemoved.Add(pageTokenValue);
          Assert.That(GetSessionStorageEntry(jsExecutor, "pt_" + pageTokenValue), Is.Not.Null);
        }
      }

      Assert.That(pageTokensThatShouldNotBeRemoved.Count, Is.EqualTo(99));
      Assert.That(GetSessionStorageEntryCount(jsExecutor), Is.EqualTo(151));
      Assert.That(GetSessionStorageEntry(jsExecutor, "pageTokenEntryCount"), Is.EqualTo(150.ToString()));

      Start<WxePageObject>("WebTabStripTest.wxe");

      var sessionStorageEntryCount = GetSessionStorageEntryCount(jsExecutor);
      var sessionStorageEntryKeyValue = GetSessionStorageEntry(jsExecutor, "pageTokenEntryCount");
      Assert.That(sessionStorageEntryCount, Is.EqualTo(101));
      Assert.That(sessionStorageEntryKeyValue, Is.EqualTo((sessionStorageEntryCount - 1).ToString()));
      Assert.That(getFirstSessionKeyValue(), Is.Null);
      foreach (var pageToken in pageTokensThatShouldNotBeRemoved)
        Assert.That(GetSessionStorageEntry(jsExecutor, "pt_" + pageToken), Is.Not.Null);
    }

    [Test]
    public void NavigatingBack_WithLastPageLoaded_ShowsOverlay ()
    {
      var home = StartWithCleanSessionStorage("WebButtonTest.wxe");
      Start<WxePageObject>("WebTabStripTest.wxe");

      home.Driver.GoBack(home.Scope);

      _ = home.Scope.OuterHTML; // Ensure up to date html
      var overlayScope = home.Scope.FindCss("#WxeStatusIsCachedMessage");
      Assert.That(overlayScope.Exists(), Is.True);
    }

    [Test]
    public void NavigatingBack_WithLastPageSubmitted_ShowsOverlay ()
    {
      var home = StartWithCleanSessionStorage("WebTabStripTest.wxe");
      home.WebTabStrips().First().SwitchTo("Tab2");
      home.WebTabStrips().First().SwitchTo("Tab1");

      home.Driver.GoBack(home.Scope);

      _ = home.Scope.OuterHTML; // Ensure up to date html
      var overlayScope = home.Scope.FindCss("#WxeStatusIsCachedMessage");
      Assert.That(overlayScope.Exists(), Is.True);
    }

    [Test]
    public void NavigateForward_IsNotPossible ()
    {
      var home = StartWithCleanSessionStorage("WebButtonTest.wxe");
      Start<WxePageObject>("WebTabStripTest.wxe");

      home.Driver.GoBack(home.Scope);
      var initialLocation = home.Driver.Location(home.Scope);
      home.Driver.GoForward(home.Scope);
      var location = home.Driver.Location(home.Scope);

      Assert.That(initialLocation.ToString(), Does.Contain("WebButtonTest.wxe"));
      Assert.That(location, Is.EqualTo(initialLocation));
    }

    private string GetPageTokenValue (PageObject home) =>
        home.Scope.FindCss("#SmartPage_TokenField").Value;

    private long GetSessionStorageEntryCount (IJavaScriptExecutor executor) =>
        JavaScriptExecutor.ExecuteStatement<long>(executor, "return window.sessionStorage.length;");

    private void ClearSessionStorage (IJavaScriptExecutor executor) =>
        JavaScriptExecutor.ExecuteVoidStatement(executor, "window.sessionStorage.clear();");

    private string GetSessionStorageEntry (IJavaScriptExecutor executor, string key) =>
        JavaScriptExecutor.ExecuteStatement<string>(executor, $"return window.sessionStorage['{key}'];");

    private WxePageObject StartWithCleanSessionStorage (string page)
    {
      var home = Start<WxePageObject>(page);
      var jsExecutor = JavaScriptExecutor.GetJavaScriptExecutor(home.Scope);
      ClearSessionStorage(jsExecutor);
      return Start<WxePageObject>(page);
    }
  }
}
