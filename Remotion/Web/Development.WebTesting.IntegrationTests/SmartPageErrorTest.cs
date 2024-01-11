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
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.RequestErrorDetectionStrategies;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class SmartPageErrorTest : IntegrationTest
  {
    [Test]
    public void AsynchronousPageRequestError_ShowsYellowPageOfDeath ()
    {
      var home = Start();

      var statusCodeTextBox = home.TextBoxes().GetByID("statusCode");
      statusCodeTextBox.FillWith("418", new WebTestActionOptions() { CompletionDetectionStrategy = new NullCompletionDetectionStrategy()});

      home.Scope.FindId("TriggerAsyncResponseWithPartialRenderingErrorButton").Click();

      home.Scope.FindId("SmartPageServerErrorMessage").Exists();

      var result = new AspNetRequestErrorDetectionParser().Parse(home.Scope);

      Assert.That(result.Message, Is.EqualTo("Some random error."));
      Assert.That(result.Stacktrace, Is.Not.Empty);
    }

    [Test]
    public void SynchronousPageRequestError_ShowsResponseBodyInIFrame ()
    {
      var home = Start();

      var statusCodeTextBox = home.TextBoxes().GetByID("statusCode");
      statusCodeTextBox.FillWith("418", new WebTestActionOptions() { CompletionDetectionStrategy = new NullCompletionDetectionStrategy()});

      home.WebButtons().GetByID("TriggerAsyncResponseWithFullPageRenderingErrorButton").Click();

      home.Scope.FindId("SmartPageServerErrorMessage").Exists();

      //Note: the custom error module message is due to the ASP.Net error handling only showing basic error information for remote connections.
      Assert.That(
          home.Scope.FindCss("#SmartPageServerErrorMessage iframe").InnerHTML,
          Is.EqualTo("<h1>Test error response body</h1><p>This is a test error response body</p>")
              .Or.EqualTo("The custom error module does not recognize this error."));
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject>("SmartPageErrorTest.wxe");
    }
  }
}
