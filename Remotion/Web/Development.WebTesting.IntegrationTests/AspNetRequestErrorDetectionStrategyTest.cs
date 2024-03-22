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
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class AspNetRequestErrorDetectionStrategyTest : IntegrationTest
  {
    [Test]
    public void Parse_PostbackError ()
    {
      var aspNetRequestErrorDetectionParser = new AspNetRequestErrorDetectionStrategy();
      var home = StartToErrorPage("SyncPostbackError");

      var exception = Assert.Throws<WebTestException>(() => aspNetRequestErrorDetectionParser.CheckPageForError(home.Scope));

      Assert.That(exception.Message, Is.EqualTo("Request has failed due to a server error"));
      Assert.That(exception.InnerException, Is.TypeOf(typeof(ServerErrorException)));
      Assert.That(exception.InnerException.Message, Is.EqualTo("SyncPostbackError"));
      Assert.That(exception.InnerException.StackTrace, Does.StartWith("\r\n[Exception: SyncPostbackError]\r\n"));
    }

    [Test]
    public void Parse_WithoutError ()
    {
      var aspNetRequestErrorDetectionParser = new AspNetRequestErrorDetectionParser();

      var home = Start();

      Assert.That(
          () => aspNetRequestErrorDetectionParser.Parse(home.Scope),
          Throws.Nothing);

      var result = aspNetRequestErrorDetectionParser.Parse(home.Scope);

      Assert.That(result.HasError, Is.EqualTo(false));
    }


    private WxePageObject Start ()
    {
      return Start<WxePageObject>("RequestErrorDetectionStrategyTest.wxe");
    }

    private WxePageObject StartToErrorPage (string localButtonId)
    {
      var home = Start( );
      var anchor = home.Anchors().GetByLocalID(localButtonId);

      //Note: Normale completion detection does not work because of the Error Page
      anchor.Click(new WebTestActionOptions() { CompletionDetectionStrategy = new NullCompletionDetectionStrategy() });

      //Call Exists workaround because scope is not updated properly
      home.Scope.FindCss("body > span > h1").ExistsWorkaround();

      //Wait for Message header to exist
      home.Scope.FindCss("body > span > h1").Exists();

      return home;
    }
  }
}
