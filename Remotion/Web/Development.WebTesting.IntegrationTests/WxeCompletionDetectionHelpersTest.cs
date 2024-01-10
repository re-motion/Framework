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
using Remotion.Web.Development.WebTesting.ExecutionEngine.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class WxeCompletionDetectionHelpersTest : IntegrationTest
  {
    private DiagnosticInformationCollectingRequestErrorDetectionStrategy _requestErrorDetectionStrategy;

    [SetUp]
    public void SetUp ()
    {
      _requestErrorDetectionStrategy = (DiagnosticInformationCollectingRequestErrorDetectionStrategy)Helper.TestInfrastructureConfiguration.RequestErrorDetectionStrategy;
    }

    [Test]
    [Category("LongRunning")]
    public void WxeCompletionDetectionHelpers_GetWxePostBackSequenceNumber_CallsRequestErrorDetectionStrategyWithCorrectScope ()
    {
      var home = Start();
      var currentCallCount = _requestErrorDetectionStrategy.GetCallCounter();
      var rootScope = home.Context.Window.GetRootScope();
      var completionDetection = new WxePostBackCompletionDetectionStrategy(1);

      try
      {
        completionDetection.WaitForCompletion(home.Context, 2);
      }
      catch (WebTestException)
      {
      }

      Assert.That(_requestErrorDetectionStrategy.GetCallCounter(), Is.EqualTo(currentCallCount + 1));
      Assert.That(_requestErrorDetectionStrategy.GetLastPassedScope().InnerHTML, Is.EqualTo(rootScope.InnerHTML));
    }

    [Test]
    [Category("LongRunning")]
    public void WxeCompletionDetectionHelpers_GetWxeFunctionToken_CallsRequestErrorDetectionStrategyWithCorrectScope ()
    {
      var home = Start();
      var currentCallCount = _requestErrorDetectionStrategy.GetCallCounter();
      var rootScope = home.Context.Window.GetRootScope();
      var completionDetection = new WxeResetCompletionDetectionStrategy();

      try
      {
        completionDetection.WaitForCompletion(home.Context, "wxeFunctionToken");
      }
      catch (WebTestException)
      {
      }

      Assert.That(_requestErrorDetectionStrategy.GetCallCounter(), Is.EqualTo(currentCallCount + 1));
      Assert.That(_requestErrorDetectionStrategy.GetLastPassedScope().InnerHTML, Is.EqualTo(rootScope.InnerHTML));
    }

    private WxePageObject Start ()
    {
      // We cannot start the correct page directly as it would throw because of the error detection
      // So instead, we start an empty page, navigate to the actual page and return the empty-page
      // page object to ensure that the error detection is correctly set in the context
      var pageObject = Start<WxePageObject>("Empty.aspx");

      StartWithoutRequestErrorDetection<WxePageObject>("AspNetRequestErrorDetectionParserStaticPages/CustomErrorDefaultErrorPage.html");
      return pageObject;
    }
  }
}
