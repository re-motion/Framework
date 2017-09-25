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
using Coypu;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;
using Remotion.Web.Development.WebTesting.IntegrationTests.TestCaseFactories;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class WebButtonControlObjectTest : IntegrationTest
  {
    [Test]
    [RemotionTestCaseSource (typeof (GeneralTestCaseFactory<WebButtonSelector, WebButtonControlObject>))]
    public void GenericTests (GenericSelectorTestSetupAction<WebButtonSelector, WebButtonControlObject> testSetupAction)
    {
      testSetupAction (Helper, e => e.WebButtons(), "webButton");
    }

    [Test]
    [RemotionTestCaseSource (typeof (HtmlIDControlSelectorTestCaseFactory<WebButtonSelector, WebButtonControlObject>))]
    [RemotionTestCaseSource (typeof (IndexControlSelectorTestCaseFactory<WebButtonSelector, WebButtonControlObject>))]
    [RemotionTestCaseSource (typeof (ItemIDControlSelectorTestCaseFactory<WebButtonSelector, WebButtonControlObject>))]
    [RemotionTestCaseSource (typeof (LocalIDControlSelectorTestCaseFactory<WebButtonSelector, WebButtonControlObject>))]
    [RemotionTestCaseSource (typeof (TextContentControlSelectorTestCaseFactory<WebButtonSelector, WebButtonControlObject>))]
    [RemotionTestCaseSource (typeof (FirstControlSelectorTestCaseFactory<WebButtonSelector, WebButtonControlObject>))]
    [RemotionTestCaseSource (typeof (SingleControlSelectorTestCaseFactory<WebButtonSelector, WebButtonControlObject>))]
    public void TestControlSelectors (GenericSelectorTestSetupAction<WebButtonSelector, WebButtonControlObject> testSetupAction)
    {
      testSetupAction (Helper, e => e.WebButtons(), "webButton");
    }

    [Test]
    public void TestIsDisabled_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.WebButtons().GetByLocalID ("MyDisabledWebButton");

      Assert.That (control.IsDisabled(), Is.True);
      Assert.That (() => control.Click(), Throws.InstanceOf<MissingHtmlException>());
    }

    [Test]
    public void TestGetText ()
    {
      var home = Start();
      
      var webButton = home.WebButtons().GetByLocalID ("MyWebButton1Sync");
      Assert.That (webButton.GetText(), Is.EqualTo ("SyncButton"));
    }

    [Test]
    public void TestClick ()
    {
      var home = Start();

      var syncWebButton = home.WebButtons().GetByLocalID ("MyWebButton1Sync");
      home = syncWebButton.Click().Expect<WxePageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("Sync"));

      var asyncWebButton = home.WebButtons().GetByLocalID ("MyWebButton2Async");
      home = asyncWebButton.Click().Expect<WxePageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("Async"));

      var hrefWebButton = home.WebButtons().GetByTextContent ("HrefButton");
      home = hrefWebButton.Click().Expect<WxePageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);
    }

    [Test]
    public void TestHasClass ()
    {
      var home = Start();

      var webButton = home.WebButtons().GetByID ("body_MyWebButton1Sync");
      Assert.That (webButton.StyleInfo.HasCssClass ("buttonBody"), Is.True);
      Assert.That (webButton.StyleInfo.HasCssClass ("doesNotHaveThisClass"), Is.False);
    }

    [Test]
    public void TestGetBackgroundColor ()
    {
      var home = Start();

      var webButton = home.WebButtons().GetByID ("body_MyWebButton1Sync");
      Assert.That (webButton.StyleInfo.GetBackgroundColor(), Is.EqualTo (WebColor.FromRgb (230, 229, 231)));
    }

    [Test]
    public void TestGetTextColor ()
    {
      var home = Start();

      var webButton = home.WebButtons().GetByID ("body_MyWebButton1Sync");
      Assert.That (webButton.StyleInfo.GetTextColor(), Is.EqualTo (WebColor.Black));
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject> ("WebButtonTest.wxe");
    }
  }
}