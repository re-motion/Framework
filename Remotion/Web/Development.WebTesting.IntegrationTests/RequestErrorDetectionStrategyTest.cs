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
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class RequestErrorDetectionStrategyTest : IntegrationTest
  {
    private DiagnosticInformationCollectingRequestErrorDetectionStrategy _requestErrorDetectionStrategy;

    [SetUp]
    public void SetUp ()
    {
      _requestErrorDetectionStrategy = (DiagnosticInformationCollectingRequestErrorDetectionStrategy)Helper.TestInfrastructureConfiguration.RequestErrorDetectionStrategy;
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
        button.Context.CloneForControl(home.Scope.FindId("NotExistent"));
      }
      catch (WebTestException)
      {
      }

      Assert.That(_requestErrorDetectionStrategy.GetCallCounter(), Is.EqualTo(currentCallCount + 1));
      Assert.That(_requestErrorDetectionStrategy.GetLastPassedScope().InnerHTML, Is.EqualTo(rootScope.InnerHTML));
    }

    [Test]
    public void ControlObjectContext_CloneForNewPage_DoesNotCallRequestErrorDetection ()
    {
      var home = Start();
      var currentCallCount = _requestErrorDetectionStrategy.GetCallCounter();

      //Get any control
      var button = home.Anchors().First();


      button.Context.CloneForNewPage();

      // Note: Does not call requestErrorDetection
      Assert.That(_requestErrorDetectionStrategy.GetCallCounter(), Is.EqualTo(currentCallCount));
    }

    [Test]
    public void ControlObjectContext_CloneForNewWindow_DoesNotCallRequestErrorDetection ()
    {
      var home = Start();
      var currentCallCount = _requestErrorDetectionStrategy.GetCallCounter();

      //Get any control
      var button = home.Anchors().First();


      button.Context.CloneForNewWindow(home.Context.Window.Title);

      // Note: Does not call requestErrorDetection
      Assert.That(_requestErrorDetectionStrategy.GetCallCounter(), Is.EqualTo(currentCallCount));
    }

    [Test]
    public void ControlObjectContext_CloneForPopupWindow_DoesNotCallRequestErrorDetection ()
    {
      var home = Start();
      var currentCallCount = _requestErrorDetectionStrategy.GetCallCounter();

      //Get any control
      var button = home.Anchors().First();


      button.Context.CloneForNewPopupWindow(home.Context.Window.Title);

      // Note: Does not call requestErrorDetection
      Assert.That(_requestErrorDetectionStrategy.GetCallCounter(), Is.EqualTo(currentCallCount));
    }

    [Test]
    public void ControlObjectContext_CloneForControlSelection_DoesNotCallRequestErrorDetection ()
    {
      var home = Start();
      var currentCallCount = _requestErrorDetectionStrategy.GetCallCounter();

      //Get any control
      var button = home.Anchors().First();

      button.Context.CloneForControlSelection();

      // Note: Does not call requestErrorDetection
      Assert.That(_requestErrorDetectionStrategy.GetCallCounter(), Is.EqualTo(currentCallCount));
    }

    [Test]
    public void PageObjectContext_New_DoesNotCallRequestErrorDetection ()
    {
      var currentCallCount = _requestErrorDetectionStrategy.GetCallCounter();


      PageObjectContext.New(Helper.MainBrowserSession, _requestErrorDetectionStrategy, NullLoggerFactory.Instance);

      // Note: Does not call requestErrorDetection
      Assert.That(_requestErrorDetectionStrategy.GetCallCounter(), Is.EqualTo(currentCallCount));
    }

    [Test]
    public void PageObjectContext_CloneForSession_DoesNotCallRequestErrorDetection ()
    {
      var home = Start();
      var currentCallCount = _requestErrorDetectionStrategy.GetCallCounter();


      home.Context.CloneForSession(Helper.MainBrowserSession);

      // Note: Does not call requestErrorDetection
      Assert.That(_requestErrorDetectionStrategy.GetCallCounter(), Is.EqualTo(currentCallCount));
    }

    [Test]
    public void PageObjectContext_CloneForFrame_CallsRequestErrorDetectionStrategyWithCorrectScope ()
    {
      var home = Start();
      var frameScope = home.Scope.FindFrame("frame");

      var currentCallCount = _requestErrorDetectionStrategy.GetCallCounter();

      //Navigate to error page
      var url = Helper.TestInfrastructureConfiguration.WebApplicationRoot + "AspNetRequestErrorDetectionParserStaticPages/CustomErrorDefaultErrorPage.html";
      Helper.MainBrowserSession.Window.Visit(url);

      //Use small Timeout so we dont have to wait for exception. With TimeSpan.Zero, an ElementStaleException gets triggered.
      frameScope.ElementFinder.Options.Timeout = TimeSpan.FromSeconds(1);

      try
      {
        home.Context.CloneForFrame(frameScope);
      }
      catch (WebTestException)
      {
      }

      Assert.That(_requestErrorDetectionStrategy.GetCallCounter(), Is.EqualTo(currentCallCount + 1));
      Assert.That(_requestErrorDetectionStrategy.GetLastPassedScope().InnerHTML, Is.EqualTo(home.Context.Window.GetRootScope().InnerHTML));
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
        home.Context.CloneForControl(home, home.Scope.FindId("NotExistent"));
      }
      catch (WebTestException)
      {
      }

      Assert.That(_requestErrorDetectionStrategy.GetCallCounter(), Is.EqualTo(currentCallCount + 1));
      Assert.That(_requestErrorDetectionStrategy.GetLastPassedScope().InnerHTML, Is.EqualTo(rootScope.InnerHTML));
    }

    [Test]
    public void PageObjectContext_CloneForControlSelection_DoesNotCallRequestErrorDetection ()
    {
      var home = Start();
      var currentCallCount = _requestErrorDetectionStrategy.GetCallCounter();


      home.Context.CloneForControlSelection(home);

      // Note: Does not call requestErrorDetection
      Assert.That(_requestErrorDetectionStrategy.GetCallCounter(), Is.EqualTo(currentCallCount));
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject>("MultiWindowTest/Main.wxe");
    }
  }
}
