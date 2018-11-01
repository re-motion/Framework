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
using Remotion.Web.Development.WebTesting.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.RequestErrorDetectionStrategies;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class AspNetRequestErrorDetectionParserTest : IntegrationTest
  {
    [Test]
    public void Parse_SyncPostbackError ()
    {
      var aspNetRequestErrorDetectionParser = new AspNetRequestErrorDetectionParser();
      var home = StartToErrorPageSync ("SyncPostbackError");

      var result = aspNetRequestErrorDetectionParser.Parse (home.Scope);

      Assert.That (result.HasError, Is.EqualTo (true));
      Assert.That (result.Message, Is.EqualTo ("SyncPostbackError"));
      Assert.That (result.Stacktrace, Is.StringStarting ("\r\n[Exception: SyncPostbackError]\r\n"));
    }

    [Test]
    public void Parse_SyncPostbackError_WithSpecialCharacters ()
    {
      var aspNetRequestErrorDetectionParser = new AspNetRequestErrorDetectionParser();
      var home = StartToErrorPageSync ("SyncPostbackWithSpecialCharactersInErrorMessage");

      var result = aspNetRequestErrorDetectionParser.Parse (home.Scope);
      
      Assert.That (result.HasError, Is.EqualTo (true));
      Assert.That (result.Message, Is.EqualTo ("ä&<\r\n'\""));
      Assert.That (result.Stacktrace, Is.StringStarting ("\r\n[Exception: ä&<\r\n'\"]\r\n"));
    }

    [Test]
    public void Parse_AsyncPostbackError ()
    {
      var aspNetRequestErrorDetectionParser = new AspNetRequestErrorDetectionParser();
      var home = StartToErrorPageAsync ("AsyncPostbackError");

      var result = aspNetRequestErrorDetectionParser.Parse (home.Scope);
      
      Assert.That (result.HasError, Is.EqualTo (true));
      Assert.That (result.Message, Is.EqualTo ("AsyncPostbackError"));
      Assert.That (result.Stacktrace, Is.StringStarting ("\r\n[Exception: AsyncPostbackError]\r\n"));
    }

    [Test]
    public void Parse_WithoutError ()
    {
      var aspNetRequestErrorDetectionParser = new AspNetRequestErrorDetectionParser();

      var home = Start();
      
      Assert.That (
          () => aspNetRequestErrorDetectionParser.Parse (home.Scope),
          Throws.Nothing);

      var result = aspNetRequestErrorDetectionParser.Parse (home.Scope);

      Assert.That (result.HasError, Is.EqualTo (false));
    }

    [Test]
    public void Parse_CustomError ()
    {
      var aspNetRequestErrorDetectionParser = new AspNetRequestErrorDetectionParser();
      var home = StartToStaticPage ("AspNetRequestErrorDetectionParserStaticPages/CustomErrorDefaultErrorPage.html");

      var result = aspNetRequestErrorDetectionParser.Parse (home);

      Assert.That (result.HasError, Is.EqualTo (true));
      Assert.That (result.Message, Is.EqualTo ("Runtime Error"));
      Assert.That (result.Stacktrace, Is.EqualTo ("TestStacktrace"));
    }

    [Test]
    public void Parse_InvalidErrorPageWithStacktraceMissing ()
    {
      var aspNetRequestErrorDetectionParser = new AspNetRequestErrorDetectionParser();
      var home = StartToStaticPage ("AspNetRequestErrorDetectionParserStaticPages/InvalidErrorPageWithStacktraceMissing.html");

      var result = aspNetRequestErrorDetectionParser.Parse (home);

      Assert.That (result.HasError, Is.EqualTo (false));
      Assert.That (result.Message, Is.EqualTo (""));
      Assert.That (result.Stacktrace, Is.EqualTo (""));
    }

    [Test]
    public void Parse_InvalidErrorPageWithMessageMissing ()
    {
      var aspNetRequestErrorDetectionParser = new AspNetRequestErrorDetectionParser();
      var home = StartToStaticPage ("AspNetRequestErrorDetectionParserStaticPages/InvalidErrorPageWithMessageMissing.html");

      var result = aspNetRequestErrorDetectionParser.Parse (home);

      Assert.That (result.HasError, Is.EqualTo (false));
      Assert.That (result.Message, Is.EqualTo (""));
      Assert.That (result.Stacktrace, Is.EqualTo (""));
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject> ("RequestErrorDetectionStrategyTest.wxe");
    }

    private WxePageObject StartToErrorPageSync (string localButtonId)
    {
      return StartToErrorPage (localButtonId, "body");
    }

    private WxePageObject StartToErrorPageAsync (string localButtonId)
    {
      return StartToErrorPage (localButtonId, "div.SmartPageErrorBody > div");
    }
    
    private WxePageObject StartToErrorPage (string localButtonId, string startSelector)
    {
      var home = Start();
      var anchor = home.Anchors().GetByLocalID (localButtonId);

      //Note: Normale completion detection does not work because of the Error Page
      anchor.Click (new WebTestActionOptions() { CompletionDetectionStrategy = new NullCompletionDetectionStrategy() });

      //Call Exists workaround because scope is not updated properly
      home.Scope.FindCss (startSelector + " > span > h1").ExistsWorkaround();
      
      //Wait for Message header to exist
      home.Scope.FindCss (startSelector + " > span > h1").Exists();

      return home;
    }

    private ElementScope StartToStaticPage (string page)
    {
      var url = Helper.TestInfrastructureConfiguration.WebApplicationRoot + page;
      Helper.MainBrowserSession.Window.Visit (url);

      return Helper.MainBrowserSession.Window.GetRootScope();
    }
  }
}