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
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class WebTabStripControlObjectTest : IntegrationTest
  {
    [Test]
    [TestCaseSource(typeof(HtmlIDControlSelectorTestCaseFactory<WebTabStripSelector, WebTabStripControlObject>))]
    [TestCaseSource(typeof(IndexControlSelectorTestCaseFactory<WebTabStripSelector, WebTabStripControlObject>))]
    [TestCaseSource(typeof(LocalIDControlSelectorTestCaseFactory<WebTabStripSelector, WebTabStripControlObject>))]
    [TestCaseSource(typeof(FirstControlSelectorTestCaseFactory<WebTabStripSelector, WebTabStripControlObject>))]
    [TestCaseSource(typeof(SingleControlSelectorTestCaseFactory<WebTabStripSelector, WebTabStripControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<WebTabStripSelector, WebTabStripControlObject> testAction)
    {
      testAction(Helper, e => e.WebTabStrips(), "webTabStrip");
    }

    [Test]
    public void TestIsDisabled_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.WebTabStrips().GetByLocalID("MyTabStrip1");

      Assert.That(
          () => control.SwitchTo("Tab3"),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateCommandDisabledException(Driver, "SwitchTo(itemID)").Message));
      Assert.That(
          () => control.SwitchTo().WithHtmlID("body_MyTabStrip1_Tab3"),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateCommandDisabledException(Driver, "SwitchTo.WithHtmlID").Message));
      Assert.That(
          () => control.SwitchTo().WithIndex(3),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateCommandDisabledException(Driver, "SwitchTo.WithIndex").Message));
      Assert.That(
          () => control.SwitchTo().WithItemID("Tab3"),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateCommandDisabledException(Driver, "SwitchTo.WithItemID").Message));
      Assert.That(
          () => control.SwitchTo().WithDisplayText("Tab3 disabled"),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateCommandDisabledException(Driver, "SwitchTo.WithDisplayText").Message));
      Assert.That(
          () => control.SwitchTo().WithDisplayTextContains("Tab3"),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateCommandDisabledException(Driver, "SwitchTo.WithDisplayTextContains").Message));
    }

    [Test]
    public void TestGetSelectedTab ()
    {
      var home = Start();

      var tabStrip = home.WebTabStrips().First();
      Assert.That(tabStrip.GetSelectedTab().ItemID, Is.EqualTo("Tab1"));
      Assert.That(tabStrip.GetSelectedTab().Index, Is.EqualTo(-1));
      Assert.That(tabStrip.GetSelectedTab().Title, Is.EqualTo("Tab1Label"));

      tabStrip.SwitchTo("Tab2");
      Assert.That(tabStrip.GetSelectedTab().ItemID, Is.EqualTo("Tab2"));
      Assert.That(tabStrip.GetSelectedTab().Index, Is.EqualTo(-1));
      Assert.That(tabStrip.GetSelectedTab().Title, Is.EqualTo("Tab2Label"));
    }

    [Test]
    public void TestGetTabDefinitions ()
    {
      var home = Start();

      var tabStrip = home.WebTabStrips().First();
      var tabs = tabStrip.GetTabDefinitions();
      Assert.That(tabs.Count, Is.EqualTo(3));
      Assert.That(tabs[1].ItemID, Is.EqualTo("Tab2"));
      Assert.That(tabs[1].Index, Is.EqualTo(2));
      Assert.That(tabs[1].Title, Is.EqualTo("Tab2Label"));
      Assert.That(tabs[1].IsDisabled, Is.False);
      Assert.That(tabs[2].IsDisabled, Is.True);
    }

    [Test]
    public void TestGetAccessKey_EmptyAccessKey ()
    {
      var home = Start();

      var tabStrip = home.WebTabStrips().First();
      var tab = tabStrip.GetSelectedTab();
      Assert.That(tab.AccessKey,Is.Empty);
    }

    [Test]
    public void Test_TabWithAccessKey ()
    {
      var home = Start();

      var tabStrip = home.WebTabStrips().GetByLocalID("MyTabStripWithAccessKeys");
      var tab = tabStrip.GetTabDefinitions().Single(t => t.ItemID == "TabWithAccessKey");
      Assert.That(tab.Title, Is.EqualTo("Tab with access key"));
      Assert.That(tab.AccessKey, Is.EqualTo("A"));
    }

    [Test]
    public void Test_TabWithImplicitAccessKey ()
    {
      var home = Start();

      var tabStrip = home.WebTabStrips().GetByLocalID("MyTabStripWithAccessKeys");
      var tab = tabStrip.GetTabDefinitions().Single(t => t.ItemID == "TabWithImplicitAccessKey");
      Assert.That(tab.Title, Is.EqualTo("Tab with implicit access key"));
      Assert.That(tab.AccessKey, Is.EqualTo("K"));
    }

    [Test]
    public void Test_TabDisabledWithAccessKey ()
    {
      var home = Start();

      var tabStrip = home.WebTabStrips().GetByLocalID("MyTabStripWithAccessKeys");
      var tab = tabStrip.GetTabDefinitions().Single(t => t.ItemID == "TabDisabledWithAccessKey");
      Assert.That(tab.AccessKey, Is.Empty);
    }

    [Test]
    public void TestSwitchTo ()
    {
      var home = Start();

      var tabStrip1 = home.WebTabStrips().First();
      var tabStrip2 = home.WebTabStrips().GetByIndex(2);

      home = tabStrip1.SwitchTo("Tab2").Expect<WxePageObject>();
      Assert.That(home.Scope.FindId("TestOutputLabel").Text, Is.EqualTo("MyTabStrip1/Tab2"));

      home = tabStrip1.SwitchTo().WithDisplayText("Tab1Label").Expect<WxePageObject>();
      Assert.That(home.Scope.FindId("TestOutputLabel").Text, Is.EqualTo("MyTabStrip1/Tab1"));

      home = tabStrip1.SwitchTo().WithDisplayTextContains("b2L").Expect<WxePageObject>();
      Assert.That(home.Scope.FindId("TestOutputLabel").Text, Is.EqualTo("MyTabStrip1/Tab2"));

      home = tabStrip2.SwitchTo().WithIndex(2).Expect<WxePageObject>();
      Assert.That(home.Scope.FindId("TestOutputLabel").Text, Is.EqualTo("MyTabStrip2/Tab2"));

      home = tabStrip2.SwitchTo().WithHtmlID("body_MyTabStrip2_Tab1").Expect<WxePageObject>();
      Assert.That(home.Scope.FindId("TestOutputLabel").Text, Is.EqualTo("MyTabStrip2/Tab1"));
    }

    [Test]
    public void Test_TabWithUmlaut ()
    {
      var home = Start();

      var tabStrip = home.WebTabStrips().GetByLocalID("MyTabStripWithUmlaut");

      Assert.That(tabStrip.GetTabDefinitions().Single(t => t.Title == "UmlautÖ"), Is.Not.Null);
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject>("WebTabStripTest.wxe");
    }
  }
}
