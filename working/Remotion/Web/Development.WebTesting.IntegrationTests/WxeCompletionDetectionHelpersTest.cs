﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Diagnostics;
using System.Threading;
using Coypu;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ExecutionEngine.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.RequestErrorDetectionStrategies;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class WxeCompletionDetectionHelpersTest : IntegrationTest
  {
    private DiagnosticInformationCollectioningRequestErrorDetectionStrategy _requestErrorDetectionStrategy;
    
    [SetUp]
    public void SetUp ()
    {
      _requestErrorDetectionStrategy = (DiagnosticInformationCollectioningRequestErrorDetectionStrategy) Helper.TestInfrastructureConfiguration.RequestErrorDetectionStrategy;
    }

    [Test]
    [Category ("LongRunning")]
    public void WxeCompletionDetectionHelpers_GetWxePostBackSequenceNumber_CallsRequestErrorDetectionStrategyWithCorrectScope ()
    {
      var home = Start();
      var currentCallCount = _requestErrorDetectionStrategy.GetCallCounter();
      var rootScope = home.Context.Window.GetRootScope();
      var completionDetection = new WxePostBackCompletionDetectionStrategy(1);
 
      try
      {
        completionDetection.WaitForCompletion (home.Context, 2);
      }
      catch (MissingHtmlException)
      {
      }
 
      Assert.That (_requestErrorDetectionStrategy.GetCallCounter(), Is.EqualTo (currentCallCount + 1));
      Assert.That (_requestErrorDetectionStrategy.GetLastPassedScope().InnerHTML, Is.EqualTo (rootScope.InnerHTML));
    }

    [Test]
    [Category ("LongRunning")]
    public void WxeCompletionDetectionHelpers_GetWxeFunctionToken_CallsRequestErrorDetectionStrategyWithCorrectScope ()
    {
      var home = Start();
      var currentCallCount = _requestErrorDetectionStrategy.GetCallCounter();
      var rootScope = home.Context.Window.GetRootScope();
      var completionDetection = new WxeResetCompletionDetectionStrategy();
 
      try
      {
        completionDetection.WaitForCompletion (home.Context, "wxeFunctionToken");
      }
      catch (MissingHtmlException)
      {
      }
 
      Assert.That (_requestErrorDetectionStrategy.GetCallCounter(), Is.EqualTo (currentCallCount + 1));
      Assert.That (_requestErrorDetectionStrategy.GetLastPassedScope().InnerHTML, Is.EqualTo (rootScope.InnerHTML));
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject> ("AspNetRequestErrorDetectionParserStaticPages/CustomErrorDefaultErrorPage.html");
    }
  }
}