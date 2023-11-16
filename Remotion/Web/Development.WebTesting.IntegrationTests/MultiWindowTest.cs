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
using System.Threading;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ExecutionEngine.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.WebDriver;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  [RequiresUserInterface]
  public class MultiWindowTest : IntegrationTest
  {
    [Test]
    public void TestMultiFrameActions ()
    {
      if (Helper.BrowserConfiguration.IsEdge())
        Assert.Ignore("RM-7473 - Flaky test");

      var home = Start();

      var mainLabel = home.Labels().GetByID("MainLabel");
      AssertPostBackSequenceNumber(mainLabel, 1);

      var frameLabel = home.Frame.Labels().GetByID("FrameLabel");
      AssertPostBackSequenceNumber(frameLabel, 1);

      var simplePostBackButton = home.WebButtons().GetByID("SimplePostBack");
      simplePostBackButton.Click();
      AssertPostBackSequenceNumber(frameLabel, 1);
      AssertPostBackSequenceNumber(mainLabel, 2);

      var loadFrameFunctionAsSubInFrameButton = home.WebButtons().GetByID("LoadFrameFunctionAsSubInFrame");
      loadFrameFunctionAsSubInFrameButton.Click(Opt.ContinueWhen(Wxe.PostBackCompletedIn(home.Frame)));
      AssertPostBackSequenceNumber(frameLabel, 2);
      AssertPostBackSequenceNumber(mainLabel, 3);

      var loadFrameFunctionInFrameButton = home.WebButtons().GetByID("LoadFrameFunctionInFrame");
      loadFrameFunctionInFrameButton.Click(Opt.ContinueWhen(Wxe.ResetIn(home.Frame)));
      AssertPostBackSequenceNumber(frameLabel, 1);
      AssertPostBackSequenceNumber(mainLabel, 4);

      var simplePostBackButtonInFrameButton = home.Frame.WebButtons().GetByID("SimplePostBack");
      simplePostBackButtonInFrameButton.Click();
      AssertPostBackSequenceNumber(frameLabel, 2);
      AssertPostBackSequenceNumber(mainLabel, 4);

      var refreshMainUpdatePanelButton = home.Frame.WebButtons().GetByID("RefreshMainUpdatePanel");
      refreshMainUpdatePanelButton.Click(Opt.ContinueWhen(Wxe.PostBackCompletedIn(home)));
      AssertPostBackSequenceNumber(frameLabel, 3);
      AssertPostBackSequenceNumber(mainLabel, 5);

      var loadMainAutoRefreshingFrameFunctionInFrameButton = home.WebButtons().GetByID("LoadMainAutoRefreshingFrameFunctionInFrame");
      loadMainAutoRefreshingFrameFunctionInFrameButton.Click(Opt.ContinueWhen(Wxe.ResetIn(home.Frame)));
      AssertPostBackSequenceNumber(frameLabel, 1);
      AssertPostBackSequenceNumber(mainLabel, 6);

      simplePostBackButtonInFrameButton.Click(Opt.ContinueWhenAll(Wxe.PostBackCompleted, Wxe.PostBackCompletedIn(home)));
      AssertPostBackSequenceNumber(frameLabel, 2);
      AssertPostBackSequenceNumber(mainLabel, 7);
    }

    [Test]
    public void TestMultiWindowActions ()
    {
      var home = Start();

      var mainLabel = home.Labels().GetByID("MainLabel");
      AssertPostBackSequenceNumber(mainLabel, 1);

      var frameLabel = home.Frame.Labels().GetByID("FrameLabel");
      AssertPostBackSequenceNumber(frameLabel, 1);

      var loadWindowFunctionInNewWindowButton = home.WebButtons().GetByID("LoadWindowFunctionInNewWindow");
      var window = loadWindowFunctionInNewWindowButton.Click().ExpectNewPopupWindow<WxePageObject>("MyWindow");
      var windowLabel = window.Labels().GetByID("WindowLabel");
      AssertPostBackSequenceNumber(windowLabel, 1);
      AssertPostBackSequenceNumber(frameLabel, 1);
      AssertPostBackSequenceNumber(mainLabel, 2);

      var simplePostBackButtonInWindowButton = window.WebButtons().GetByID("SimplePostBack");
      simplePostBackButtonInWindowButton.Click();
      AssertPostBackSequenceNumber(windowLabel, 2);
      AssertPostBackSequenceNumber(frameLabel, 1);
      AssertPostBackSequenceNumber(mainLabel, 2);

      var closeButton = FluentControlSelectorExtensionsForIntegrationTests.WebButtons(window).GetByID("Close");
      closeButton.Click(Opt.ContinueWhen(Wxe.PostBackCompletedInContext(window.Context.ParentContext)));
      AssertPostBackSequenceNumber(frameLabel, 1);
      AssertPostBackSequenceNumber(mainLabel, 3);

      var loadWindowFunctionInNewWindowInFrameButton = home.Frame.WebButtons().GetByID("LoadWindowFunctionInNewWindow");
      var window2 = loadWindowFunctionInNewWindowInFrameButton.Click().ExpectNewPopupWindow<WxePageObject>("MyWindow");

      //Workaround until it is possible to configure timeout on ExpectNewPopupWindow (RM-6771)
      Thread.Sleep(TimeSpan.FromSeconds(5));
      var windowLabel2 = window2.Labels().GetByID("WindowLabel");

      AssertPostBackSequenceNumber(windowLabel2, 1);
      AssertPostBackSequenceNumber(frameLabel, 2);
      AssertPostBackSequenceNumber(mainLabel, 3);

      var closeAndRefreshMainAsWellButton = FluentControlSelectorExtensionsForIntegrationTests.WebButtons(window2).GetByID("CloseAndRefreshMainAsWell");
      var options = Opt.ContinueWhenAll(Wxe.PostBackCompletedIn(home.Frame), Wxe.PostBackCompletedInContext(window2.Context.ParentContext));
      closeAndRefreshMainAsWellButton.Click(options);
      AssertPostBackSequenceNumber(frameLabel, 3);
      AssertPostBackSequenceNumber(mainLabel, 4);
    }

    [Test]
    public void TestAcceptModalBrowserDialog ()
    {
      if (Helper.BrowserConfiguration.IsFirefox())
        Assert.Ignore("Firefox does not show a dialog.");

      var home = Start();

      var mainLabel = home.Labels().GetByID("MainLabel");
      AssertPostBackSequenceNumber(mainLabel, 1);

      var frameLabel = home.Frame.Labels().GetByID("FrameLabel");
      AssertPostBackSequenceNumber(frameLabel, 1);

      home.Frame.WebButtons().GetByLocalID("SimplePostBack").Click();
      AssertPostBackSequenceNumber(frameLabel, 2);

      home.Frame.TextBoxes().GetByLocalID("MyTextBox").FillWith("MyText", FinishInput.Promptly);

      // We need to manually setup WXE post back detection since we do Opt.ContinueImmediately() later
      var mainPostBackCompletionDetectionStrategy = new WxePostBackCompletionDetectionStrategy(1, TimeSpan.FromSeconds(15));
      var mainPostBackState = mainPostBackCompletionDetectionStrategy.PrepareWaitForCompletion(home.Context);

      var framePostBackCompletionDetectionStrategy = new WxePostBackCompletionDetectionStrategy(-1, TimeSpan.FromSeconds(15));
      var framePostBackState = framePostBackCompletionDetectionStrategy.PrepareWaitForCompletion(home.Frame.Context);

      var loadFrameFunctionInFrameButton = home.WebButtons().GetByID("LoadFrameFunctionInFrameWithoutPostback");
      loadFrameFunctionInFrameButton.Click(Opt.ContinueImmediately().AcceptModalDialog());

      mainPostBackCompletionDetectionStrategy.WaitForCompletion(home.Context, mainPostBackState);
      framePostBackCompletionDetectionStrategy.WaitForCompletion(home.Frame.Context, framePostBackState);

      AssertPostBackSequenceNumber(frameLabel, 1);
      AssertPostBackSequenceNumber(mainLabel, 2);

      // Ensure that page can still be used
      var navigatieAwayButton = home.WebButtons().GetByID("NavigateAway");
      var defaultPage = navigatieAwayButton.Click().Expect<WxePageObject>();
      Assert.That(defaultPage.GetTitle(), Is.EqualTo("Web.Development.WebTesting.TestSite"));
    }

    [Test]
    public void TestCancelModalBrowserDialog ()
    {
      if (Helper.BrowserConfiguration.IsFirefox())
        Assert.Ignore("Firefox does not show a dialog.");

      var home = Start();

      var mainLabel = home.Labels().GetByID("MainLabel");
      AssertPostBackSequenceNumber(mainLabel, 1);

      var frameLabel = home.Frame.Labels().GetByID("FrameLabel");
      AssertPostBackSequenceNumber(frameLabel, 1);

      home.Frame.WebButtons().GetByLocalID("SimplePostBack").Click();
      AssertPostBackSequenceNumber(frameLabel, 2);

      home.Frame.TextBoxes().GetByLocalID("MyTextBox").FillWith("MyText", FinishInput.Promptly);

      var loadFrameFunctionInFrameButton = home.WebButtons().GetByID("LoadFrameFunctionInFrameWithoutPostback");
      loadFrameFunctionInFrameButton.Click(Opt.ContinueImmediately().CancelModalDialog());
      AssertPostBackSequenceNumber(frameLabel, 2);
      AssertPostBackSequenceNumber(mainLabel, 2);

      // Ensure that page can still be used
      var navigatieAwayButton = home.WebButtons().GetByID("NavigateAway");
      var defaultPage = navigatieAwayButton.Click(Opt.ContinueImmediately().AcceptModalDialog()).Expect<WxePageObject>();
      Assert.That(defaultPage.GetTitle(), Is.EqualTo("Web.Development.WebTesting.TestSite"));
    }

    private void AssertPostBackSequenceNumber (LabelControlObject label, int expectedPostBackSequenceNumber)
    {
      Assert.That(label.GetText(), Does.Contain(string.Format("| {0} |", expectedPostBackSequenceNumber)));
    }

    private MultiWindowTestPageObject Start ()
    {
      return Start<MultiWindowTestPageObject>("MultiWindowTest/Main.wxe");
    }
  }
}
