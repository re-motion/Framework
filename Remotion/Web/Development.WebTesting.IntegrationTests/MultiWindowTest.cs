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
using Remotion.Web.Development.WebTesting.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ExecutionEngine.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class MultiWindowTest : IntegrationTest
  {
    [Test]
    public void TestMultiFrameActions ()
    {
      var home = Start();

      var mainLabel = home.GetLabel().ByID ("MainLabel");
      AssertPostBackSequenceNumber (mainLabel, 1);

      var frameLabel = home.Frame.GetLabel().ByID ("FrameLabel");
      AssertPostBackSequenceNumber (frameLabel, 1);

      var simplePostBackButton = home.GetWebButton().ByID ("SimplePostBack");
      simplePostBackButton.Click();
      AssertPostBackSequenceNumber (frameLabel, 1);
      AssertPostBackSequenceNumber (mainLabel, 2);

      var loadFrameFunctionAsSubInFrameButton = home.GetWebButton().ByID ("LoadFrameFunctionAsSubInFrame");
      loadFrameFunctionAsSubInFrameButton.Click (Opt.ContinueWhen(Wxe.PostBackCompletedIn (home.Frame)));
      AssertPostBackSequenceNumber (frameLabel, 2);
      AssertPostBackSequenceNumber (mainLabel, 3);

      var loadFrameFunctionInFrameButton = home.GetWebButton().ByID ("LoadFrameFunctionInFrame");
      loadFrameFunctionInFrameButton.Click (Opt.ContinueWhen (Wxe.ResetIn (home.Frame)));
      AssertPostBackSequenceNumber (frameLabel, 1);
      AssertPostBackSequenceNumber (mainLabel, 4);

      var simplePostBackButtonInFrameButton = home.Frame.GetWebButton().ByID ("SimplePostBack");
      simplePostBackButtonInFrameButton.Click();
      AssertPostBackSequenceNumber (frameLabel, 2);
      AssertPostBackSequenceNumber (mainLabel, 4);

      var refreshMainUpdatePanelButton = home.Frame.GetWebButton().ByID ("RefreshMainUpdatePanel");
      refreshMainUpdatePanelButton.Click (Opt.ContinueWhen (Wxe.PostBackCompletedIn (home)));
      AssertPostBackSequenceNumber (frameLabel, 3);
      AssertPostBackSequenceNumber (mainLabel, 5);

      var loadMainAutoRefreshingFrameFunctionInFrameButton = home.GetWebButton().ByID ("LoadMainAutoRefreshingFrameFunctionInFrame");
      loadMainAutoRefreshingFrameFunctionInFrameButton.Click (Opt.ContinueWhen (Wxe.ResetIn (home.Frame)));
      AssertPostBackSequenceNumber (frameLabel, 1);
      AssertPostBackSequenceNumber (mainLabel, 6);

      simplePostBackButtonInFrameButton.Click (Opt.ContinueWhenAll (Wxe.PostBackCompleted, Wxe.PostBackCompletedIn (home)));
      AssertPostBackSequenceNumber (frameLabel, 2);
      AssertPostBackSequenceNumber (mainLabel, 7);
    }

    [Test]
    public void TestMultiWindowActions ()
    {
      var home = Start();

      var mainLabel = home.GetLabel().ByID ("MainLabel");
      AssertPostBackSequenceNumber (mainLabel, 1);

      var frameLabel = home.Frame.GetLabel().ByID ("FrameLabel");
      AssertPostBackSequenceNumber (frameLabel, 1);

      var loadWindowFunctionInNewWindowButton = home.GetWebButton().ByID ("LoadWindowFunctionInNewWindow");
      var window = loadWindowFunctionInNewWindowButton.Click().ExpectNewPopupWindow<WxePageObject> ("MyWindow");
      var windowLabel = window.GetLabel().ByID ("WindowLabel");
      AssertPostBackSequenceNumber (windowLabel, 1);
      AssertPostBackSequenceNumber (frameLabel, 1);
      AssertPostBackSequenceNumber (mainLabel, 2);

      var simplePostBackButtonInWindowButton = window.GetWebButton().ByID ("SimplePostBack");
      simplePostBackButtonInWindowButton.Click();
      AssertPostBackSequenceNumber (windowLabel, 2);
      AssertPostBackSequenceNumber (frameLabel, 1);
      AssertPostBackSequenceNumber (mainLabel, 2);

      var closeButton = window.GetWebButton().ByID ("Close");
      closeButton.Click (Opt.ContinueWhen (Wxe.PostBackCompletedInContext (window.Context.ParentContext)));
      AssertPostBackSequenceNumber (frameLabel, 1);
      AssertPostBackSequenceNumber (mainLabel, 3);

      var loadWindowFunctionInNewWindowInFrameButton = home.Frame.GetWebButton().ByID ("LoadWindowFunctionInNewWindow");
      loadWindowFunctionInNewWindowInFrameButton.Click().ExpectNewPopupWindow<WxePageObject> ("MyWindow");
      AssertPostBackSequenceNumber (windowLabel, 1);
      AssertPostBackSequenceNumber (frameLabel, 2);
      AssertPostBackSequenceNumber (mainLabel, 3);

      var closeAndRefreshMainAsWellButton = window.GetWebButton().ByID ("CloseAndRefreshMainAsWell");
      var options = Opt.ContinueWhenAll (Wxe.PostBackCompletedIn (home.Frame), Wxe.PostBackCompletedInContext (window.Context.ParentContext));
      closeAndRefreshMainAsWellButton.Click (options);
      AssertPostBackSequenceNumber (frameLabel, 3);
      AssertPostBackSequenceNumber (mainLabel, 4);
    }

    [Test]
    public void TestAcceptModalBrowserDialog ()
    {
      var home = Start();

      var mainLabel = home.GetLabel().ByID ("MainLabel");
      AssertPostBackSequenceNumber (mainLabel, 1);

      var frameLabel = home.Frame.GetLabel().ByID ("FrameLabel");
      AssertPostBackSequenceNumber (frameLabel, 1);

      home.Frame.GetTextBox().ByLocalID ("MyTextBox").FillWith ("MyText", FinishInput.Promptly);

      var loadFrameFunctionInFrameButton = home.GetWebButton().ByID ("LoadFrameFunctionInFrame");
      loadFrameFunctionInFrameButton.Click (Opt.ContinueWhen (Wxe.ResetIn (home.Frame)).AcceptModalDialog());
      AssertPostBackSequenceNumber (frameLabel, 1);
      AssertPostBackSequenceNumber (mainLabel, 2);

      // Ensure that page can still be used
      var navigatieAwayButton = home.GetWebButton().ByID ("NavigateAway");
      var defaultPage = navigatieAwayButton.Click().Expect<WxePageObject>();
      Assert.That (defaultPage.GetTitle(), Is.EqualTo ("Web.Development.WebTesting.TestSite"));
    }

    [Test]
    public void TestCancelModalBrowserDialog ()
    {
      var home = Start();

      var mainLabel = home.GetLabel().ByID ("MainLabel");
      AssertPostBackSequenceNumber (mainLabel, 1);

      var frameLabel = home.Frame.GetLabel().ByID ("FrameLabel");
      AssertPostBackSequenceNumber (frameLabel, 1);

      home.Frame.GetTextBox().ByLocalID ("MyTextBox").FillWith ("MyText", FinishInput.Promptly);

      var loadFrameFunctionInFrameButton = home.GetWebButton().ByID ("LoadFrameFunctionInFrame");
      loadFrameFunctionInFrameButton.Click (Opt.ContinueWhen (Wxe.PostBackCompletedIn (home.Frame)).CancelModalDialog());
      AssertPostBackSequenceNumber (frameLabel, 2);
      AssertPostBackSequenceNumber (mainLabel, 2);

      // Ensure that page can still be used
      var navigatieAwayButton = home.GetWebButton().ByID ("NavigateAway");
      var defaultPage = navigatieAwayButton.Click (Opt.ContinueImmediately().AcceptModalDialog()).Expect<WxePageObject>();
      Assert.That (defaultPage.GetTitle(), Is.EqualTo ("Web.Development.WebTesting.TestSite"));
    }

    private void AssertPostBackSequenceNumber (LabelControlObject label, int expectedPostBackSequenceNumber)
    {
      Assert.That (label.GetText(), Is.StringContaining (string.Format ("| {0} |", expectedPostBackSequenceNumber)));
    }

    private MultiWindowTestPageObject Start ()
    {
      return Start<MultiWindowTestPageObject> ("MultiWindowTest/Main.wxe");
    }
  }
}