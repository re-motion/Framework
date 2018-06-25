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
using Remotion.Web.Development.WebTesting.ExecutionEngine.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class RequestErrorDetectionStrategyTest : IntegrationTest
  {
    private DiagnosticInformationCollectioningRequestErrorDetectionStrategy _requestErrorDetectionStrategy;
    
    [SetUp]
    public void SetUp ()
    {
      _requestErrorDetectionStrategy = (DiagnosticInformationCollectioningRequestErrorDetectionStrategy) Helper.TestInfrastructureConfiguration.RequestErrorDetectionStrategy;
    }

    [Test]
    public void WxeCompletionDetectionHelpers_GetWxePostBackSequenceNumber_CallsRequestErrorDetectionStrategyWithCorrectScope ()
    {
      var home = Start();
      var currentCallCount = _requestErrorDetectionStrategy.GetCallCounter();
      var rootScope = home.Context.Window.GetRootScope();

      var completionDetection = new WxePostBackCompletionDetectionStrategy(1);


      completionDetection.PrepareWaitForCompletion (home.Context);

      Assert.That (_requestErrorDetectionStrategy.GetCallCounter(), Is.EqualTo (currentCallCount + 1));
      Assert.That (_requestErrorDetectionStrategy.GetLastPassedScope().InnerHTML, Is.EqualTo (rootScope.InnerHTML));
    }

    [Test]
    public void WxeCompletionDetectionHelpers_GetWxeFunctionToken_CallsRequestErrorDetectionStrategyWithCorrectScope ()
    {
      var home = Start();
      var currentCallCount = _requestErrorDetectionStrategy.GetCallCounter();
      var rootScope = home.Context.Window.GetRootScope();

      var completionDetection = new WxeResetCompletionDetectionStrategy();


      completionDetection.PrepareWaitForCompletion (home.Context);

      Assert.That (_requestErrorDetectionStrategy.GetCallCounter(), Is.EqualTo (currentCallCount + 1));
      Assert.That (_requestErrorDetectionStrategy.GetLastPassedScope().InnerHTML, Is.EqualTo (rootScope.InnerHTML));
    }

    [Test]
    public void ControlObjectContext_CloneForControl_CallsRequestErrorDetectionStrategyWithCorrectScope ()
    {
      var home = Start();
      var currentCallCount = _requestErrorDetectionStrategy.GetCallCounter();
      var rootScope = home.Context.Window.GetRootScope();

      //Get any control
      var button = home.Anchors().First();

      //Use small Timeout so we dont have to wait for exception
      home.Scope.ElementFinder.Options.Timeout = TimeSpan.Zero;

      try
      {
        button.Context.CloneForControl (home.Scope.FindId ("NotExistent"));
      }
      catch (MissingHtmlException)
      {
      }

      Assert.That (_requestErrorDetectionStrategy.GetCallCounter(), Is.EqualTo (currentCallCount + 1));
      Assert.That (_requestErrorDetectionStrategy.GetLastPassedScope().InnerHTML, Is.EqualTo (rootScope.InnerHTML));
    }

    [Test]
    public void ControlObjectContext_CloneForNewPage_CallsRequestErrorDetectionStrategyWithCorrectScope ()
    {
      var home = Start();
      var currentCallCount = _requestErrorDetectionStrategy.GetCallCounter();
      var rootScope = home.Context.Window.GetRootScope();

      //Get any control
      var button = home.Anchors().First();


      button.Context.CloneForNewPage ();

      Assert.That (_requestErrorDetectionStrategy.GetCallCounter(), Is.EqualTo (currentCallCount + 1));
      Assert.That (_requestErrorDetectionStrategy.GetLastPassedScope().InnerHTML, Is.EqualTo (rootScope.InnerHTML));
    }

    [Test]
    public void ControlObjectContext_CloneForNewWindow_CallsRequestErrorDetectionStrategyWithCorrectScope ()
    {
      var home = Start();
      var currentCallCount = _requestErrorDetectionStrategy.GetCallCounter();
      var rootScope = home.Context.Window.GetRootScope();

      //Get any control
      var button = home.Anchors().First();


      button.Context.CloneForNewWindow (home.Context.Window.Title);

      Assert.That (_requestErrorDetectionStrategy.GetCallCounter(), Is.EqualTo (currentCallCount + 1));
      Assert.That (_requestErrorDetectionStrategy.GetLastPassedScope().InnerHTML, Is.EqualTo (rootScope.InnerHTML));
    }

    [Test]
    public void ControlObjectContext_CloneForPopupWindow_CallsRequestErrorDetectionStrategyWithCorrectScope ()
    {
      var home = Start();
      var currentCallCount = _requestErrorDetectionStrategy.GetCallCounter();
      var rootScope = home.Context.Window.GetRootScope();

      //Get any control
      var button = home.Anchors().First();


      button.Context.CloneForNewPopupWindow (home.Context.Window.Title);

      Assert.That (_requestErrorDetectionStrategy.GetCallCounter(), Is.EqualTo (currentCallCount + 1));
      Assert.That (_requestErrorDetectionStrategy.GetLastPassedScope().InnerHTML, Is.EqualTo (rootScope.InnerHTML));
    }

    [Test]
    public void ControlObjectContext_CloneForControlSelection_CallsRequestErrorDetectionStrategyWithCorrectScope ()
    {
      var home = Start();
      var currentCallCount = _requestErrorDetectionStrategy.GetCallCounter();
      var rootScope = home.Context.Window.GetRootScope();

      //Get any control
      var button = home.Anchors().First();

      //Switch site so button scope is invalid
      Helper.MainBrowserSession.Window.Visit (Helper.TestInfrastructureConfiguration.WebApplicationRoot + "WebButtonTest.wxe");
      
      //Workaround needed, because scope is not correctly handled on window switch
      button.Context.Scope.ExistsWorkaround();

      //Use small Timeout so we dont have to wait for exception
      button.Scope.ElementFinder.Options.Timeout = TimeSpan.Zero;

      try
      {
        button.Context.CloneForControlSelection();
      }
      catch (MissingHtmlException)
      {
      }
      
      Assert.That (_requestErrorDetectionStrategy.GetCallCounter(), Is.EqualTo (currentCallCount + 1));
      Assert.That (_requestErrorDetectionStrategy.GetLastPassedScope().InnerHTML, Is.EqualTo (rootScope.InnerHTML));
    }

    [Test]
    public void PageObjectContext_New_CallsRequestErrorDetectionStrategyWithCorrectScope ()
    {
      var home = Start();
      var currentCallCount = _requestErrorDetectionStrategy.GetCallCounter();
      var rootScope = home.Context.Window.GetRootScope();


      PageObjectContext.New (Helper.MainBrowserSession, _requestErrorDetectionStrategy);

      Assert.That (_requestErrorDetectionStrategy.GetCallCounter(), Is.EqualTo (currentCallCount + 1));
      Assert.That (_requestErrorDetectionStrategy.GetLastPassedScope().InnerHTML, Is.EqualTo (rootScope.InnerHTML));
    }

    [Test]
    public void PageObjectContext_CloneForSession_CallsRequestErrorDetectionStrategyWithCorrectScope ()
    {
      var home = Start();
      var currentCallCount = _requestErrorDetectionStrategy.GetCallCounter();
      var rootScope = home.Context.Window.GetRootScope();


      home.Context.CloneForSession (Helper.MainBrowserSession);

      Assert.That (_requestErrorDetectionStrategy.GetCallCounter(), Is.EqualTo (currentCallCount + 1));
      Assert.That (_requestErrorDetectionStrategy.GetLastPassedScope().InnerHTML, Is.EqualTo (rootScope.InnerHTML));
    }

    [Test]
    public void PageObjectContext_CloneForFrame_CallsRequestErrorDetectionStrategyWithCorrectScope ()
    {
      var home = Start();
      var frameScope = home.Scope.FindFrame ("frame");
      
      var currentCallCount = _requestErrorDetectionStrategy.GetCallCounter();


      home.Context.CloneForFrame (home.Scope.FindFrame ("frame"));

      Assert.That (_requestErrorDetectionStrategy.GetCallCounter(), Is.EqualTo (currentCallCount + 1));
      Assert.That (_requestErrorDetectionStrategy.GetLastPassedScope().InnerHTML, Is.EqualTo (frameScope.FindCss("html").InnerHTML));
    }

    [Test]
    public void PageObjectContext_CloneForControl_CallsRequestErrorDetectionStrategyWithCorrectScope ()
    {
      var home = Start();
      var currentCallCount = _requestErrorDetectionStrategy.GetCallCounter();
      var rootScope = home.Context.Window.GetRootScope();

      //Use small Timeout so we dont have to wait for exception
      home.Scope.ElementFinder.Options.Timeout = TimeSpan.Zero;

      try
      {
        home.Context.CloneForControl (home, home.Scope.FindId ("NotExistent"));
      }
      catch (MissingHtmlException)
      {
      }

      Assert.That (_requestErrorDetectionStrategy.GetCallCounter(), Is.EqualTo (currentCallCount + 1));
      Assert.That (_requestErrorDetectionStrategy.GetLastPassedScope().InnerHTML, Is.EqualTo (rootScope.InnerHTML));
    }

    [Test]
    public void PageObjectContext_CloneForControlSelection_DoesNotCallRequestErrorDetection ()
    {
      var home = Start();
      var currentCallCount = _requestErrorDetectionStrategy.GetCallCounter();
      
      
      home.Context.CloneForControlSelection (home);
      
      // Note: Does not call requestErrorDetection
      Assert.That (_requestErrorDetectionStrategy.GetCallCounter(), Is.EqualTo (currentCallCount));
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject> ("MultiWindowTest/Main.wxe");
    }
  }
}
