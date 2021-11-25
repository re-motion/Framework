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
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ExecutionEngine.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;
using Remotion.Web.Development.WebTesting.IntegrationTests.TestCaseFactories;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class WebButtonControlObjectTest : IntegrationTest
  {
    [Test]
    [TestCaseSource (typeof(DisabledTestCaseFactory<WebButtonSelector, WebButtonControlObject>))]
    public void GenericTests (GenericSelectorTestAction<WebButtonSelector, WebButtonControlObject> testAction)
    {
      testAction(Helper, e => e.WebButtons(), "webButton");
    }

    [Test]
    [TestCaseSource (typeof(HtmlIDControlSelectorTestCaseFactory<WebButtonSelector, WebButtonControlObject>))]
    [TestCaseSource (typeof(IndexControlSelectorTestCaseFactory<WebButtonSelector, WebButtonControlObject>))]
    [TestCaseSource (typeof(ItemIDControlSelectorTestCaseFactory<WebButtonSelector, WebButtonControlObject>))]
    [TestCaseSource (typeof(LocalIDControlSelectorTestCaseFactory<WebButtonSelector, WebButtonControlObject>))]
    [TestCaseSource (typeof(TextContentControlSelectorTestCaseFactory<WebButtonSelector, WebButtonControlObject>))]
    [TestCaseSource (typeof(FirstControlSelectorTestCaseFactory<WebButtonSelector, WebButtonControlObject>))]
    [TestCaseSource (typeof(SingleControlSelectorTestCaseFactory<WebButtonSelector, WebButtonControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<WebButtonSelector, WebButtonControlObject> testAction)
    {
      testAction(Helper, e => e.WebButtons(), "webButton");
    }

    [Test]
    public void TestIsDisabled_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.WebButtons().GetByLocalID("MyDisabledWebButton");

      Assert.That(control.IsDisabled(), Is.True);
      Assert.That(
          () => control.Click(),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "Click").Message));
    }

    [Test]
    public void TestGetText ()
    {
      var home = Start();

      var webButton = home.WebButtons().GetByLocalID("MyWebButton1Sync");
      Assert.That(webButton.GetText(), Is.EqualTo("SyncButton"));
    }

    [Test]
    public void TestClick ()
    {
      var home = Start();

      {
        var syncWebButton = home.WebButtons().GetByLocalID("MyWebButton1Sync");
        var completionDetection = new CompletionDetectionStrategyTestHelper(syncWebButton);
        home = syncWebButton.Click().Expect<WxePageObject>();
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That(home.Scope.FindId("TestOutputLabel").Text, Is.EqualTo("Sync"));
      }

      {
        var asyncWebButton = home.WebButtons().GetByLocalID("MyWebButton2Async");
        var completionDetection = new CompletionDetectionStrategyTestHelper(asyncWebButton);
        home = asyncWebButton.Click().Expect<WxePageObject>();
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That(home.Scope.FindId("TestOutputLabel").Text, Is.EqualTo("Async"));
      }

      {
        var hrefWebButton = home.WebButtons().GetByTextContent("HrefButton");
        var completionDetection = new CompletionDetectionStrategyTestHelper(hrefWebButton);
        home = hrefWebButton.Click().Expect<WxePageObject>();
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That(home.Scope.FindId("TestOutputLabel").Text, Is.Empty);
      }
    }

    [Test]
    public void TestHasStandardButtonType ()
    {
      var home = Start();

      var webButton = home.WebButtons().GetByID("body_MyWebButton1Sync");
      Assert.That(webButton.GetButtonType(), Is.EqualTo(ButtonType.Standard));
    }

    [Test]
    public void TestHasPrimaryButtonType ()
    {
      var home = Start();

      var webButton = home.WebButtons().GetByID("body_MyWebButtonPrimary1Sync");
      Assert.That(webButton.GetButtonType(), Is.EqualTo(ButtonType.Primary));
    }

    [Test]
    public void TestHasSupplementalButtonType ()
    {
      var home = Start();

      var webButton = home.WebButtons().GetByID("body_MyWebButtonSupplemental1Sync");
      Assert.That(webButton.GetButtonType(), Is.EqualTo(ButtonType.Supplemental));
    }

    [Test]
    public void TestHasClass ()
    {
      var home = Start();

      var webButton = home.WebButtons().GetByID("body_MyWebButton1Sync");
      Assert.That(webButton.StyleInfo.HasCssClass("buttonBody"), Is.True);
      Assert.That(webButton.StyleInfo.HasCssClass("doesNotHaveThisClass"), Is.False);
    }

    [Test]
    public void TestGetBackgroundColor ()
    {
      var home = Start();

      var webButton = home.WebButtons().GetByID("body_MyWebButton1Sync");
      Assert.That(webButton.StyleInfo.GetBackgroundColor(), Is.EqualTo(WebColor.FromRgb(230, 229, 231)));
    }

    [Test]
    public void TestGetTextColor ()
    {
      var home = Start();

      var webButton = home.WebButtons().GetByID("body_MyWebButton1Sync");
      Assert.That(webButton.StyleInfo.GetTextColor(), Is.EqualTo(WebColor.Black));
    }

    [Test]
    public void TestGetTextWithUseLegacyButton ()
    {
      var home = Start();

      var webButton = home.WebButtons().GetByItemID("MyWebButtonWithUseLegacyButton");
      Assert.That(webButton.GetText(), Is.EqualTo("LegacyButton"));
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject>("WebButtonTest.wxe");
    }
  }
}
