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
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class AnchorControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var webLinkButton = home.GetAnchor().ByID ("body_MyWebLinkButton");
      Assert.That (webLinkButton.Scope.Id, Is.EqualTo ("body_MyWebLinkButton"));

      var smartHyperLink = home.GetAnchor().ByID ("body_MySmartHyperLink");
      Assert.That (smartHyperLink.Scope.Id, Is.EqualTo ("body_MySmartHyperLink"));

      var aspLinkButton = home.GetAnchor().ByID ("body_MyAspLinkButton");
      Assert.That (aspLinkButton.Scope.Id, Is.EqualTo ("body_MyAspLinkButton"));

      var aspHyperLink = home.GetAnchor().ByID ("body_MyAspHyperLink");
      Assert.That (aspHyperLink.Scope.Id, Is.EqualTo ("body_MyAspHyperLink"));

      var htmlAnchor = home.GetAnchor().ByID ("body_MyHtmlAnchor");
      Assert.That (htmlAnchor.Scope.Id, Is.EqualTo ("body_MyHtmlAnchor"));

      var htmlAnchorWithJavaScriptLink = home.GetAnchor().ByID ("body_MyHtmlAnchorWithJavaScriptLink");
      Assert.That (htmlAnchorWithJavaScriptLink.Scope.Id, Is.EqualTo ("body_MyHtmlAnchorWithJavaScriptLink"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var htmlAnchor = home.GetAnchor().ByIndex (2);
      Assert.That (htmlAnchor.Scope.Id, Is.EqualTo ("body_MySmartHyperLink"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var webLinkButton = home.GetAnchor().ByLocalID ("MyWebLinkButton");
      Assert.That (webLinkButton.Scope.Id, Is.EqualTo ("body_MyWebLinkButton"));

      var smartHyperLink = home.GetAnchor().ByLocalID ("MySmartHyperLink");
      Assert.That (smartHyperLink.Scope.Id, Is.EqualTo ("body_MySmartHyperLink"));

      var aspLinkButton = home.GetAnchor().ByLocalID ("MyAspLinkButton");
      Assert.That (aspLinkButton.Scope.Id, Is.EqualTo ("body_MyAspLinkButton"));

      var aspHyperLink = home.GetAnchor().ByLocalID ("MyAspHyperLink");
      Assert.That (aspHyperLink.Scope.Id, Is.EqualTo ("body_MyAspHyperLink"));

      var htmlAnchor = home.GetAnchor().ByLocalID ("MyHtmlAnchor");
      Assert.That (htmlAnchor.Scope.Id, Is.EqualTo ("body_MyHtmlAnchor"));

      var htmlAnchorWithJavaScriptLink = home.GetAnchor().ByLocalID ("MyHtmlAnchorWithJavaScriptLink");
      Assert.That (htmlAnchorWithJavaScriptLink.Scope.Id, Is.EqualTo ("body_MyHtmlAnchorWithJavaScriptLink"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var htmlAnchor = home.GetAnchor().First();
      Assert.That (htmlAnchor.Scope.Id, Is.EqualTo ("body_MyWebLinkButton"));
    }

    [Test]
    [Category ("LongRunning")]
    public void TestSelection_Single ()
    {
      var home = Start();
      var scope = home.GetScope().ByID ("scope");

      var htmlAnchor = scope.GetAnchor().Single();
      Assert.That (htmlAnchor.Scope.Id, Is.EqualTo ("body_MyHtmlAnchor"));

      try
      {
        home.GetAnchor().Single();
        Assert.Fail ("Should not be able to unambigously find an HTML anchor.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestSelection_Text ()
    {
      var home = Start();

      var htmlAnchor = home.GetAnchor().ByTextContent ("MyHtmlAnchor");
      Assert.That (htmlAnchor.Scope.Id, Is.EqualTo ("body_MyHtmlAnchor"));
    }

    [Test]
    public void TestGetText ()
    {
      var home = Start();

      var webLinkButton = home.GetAnchor().ByLocalID ("MyWebLinkButton");
      Assert.That (webLinkButton.GetText(), Is.EqualTo ("MyWebLinkButton"));

      var smartHyperLink = home.GetAnchor().ByLocalID ("MySmartHyperLink");
      Assert.That (smartHyperLink.GetText(), Is.EqualTo("MySmartHyperLink"));

      var aspLinkButton = home.GetAnchor().ByLocalID ("MyAspLinkButton");
      Assert.That (aspLinkButton.GetText(), Is.EqualTo ("MyAspLinkButton"));

      var aspHyperLink = home.GetAnchor().ByLocalID ("MyAspHyperLink");
      Assert.That (aspHyperLink.GetText(), Is.EqualTo("MyAspHyperLink"));

      var htmlAnchor = home.GetAnchor().ByLocalID ("MyHtmlAnchor");
      Assert.That (htmlAnchor.GetText(), Is.EqualTo ("MyHtmlAnchor"));

      var htmlAnchorWithJavaScriptLink = home.GetAnchor().ByLocalID ("MyHtmlAnchorWithJavaScriptLink");
      Assert.That (htmlAnchorWithJavaScriptLink.GetText(), Is.EqualTo ("MyHtmlAnchorWithJavaScriptLink"));
    }

    [Test]
    public void TestClick ()
    {
      var home = Start();

      var webLinkButton = home.GetAnchor().ByLocalID ("MyWebLinkButton");
      home = webLinkButton.Click().Expect<WxePageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("MyWebLinkButton|MyWebLinkButtonCommand"));

      var smartHyperLink = home.GetAnchor().ByLocalID ("MySmartHyperLink");
      home = smartHyperLink.Click().Expect<WxePageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);

      var aspLinkButton = home.GetAnchor().ByLocalID ("MyAspLinkButton");
      home = aspLinkButton.Click().Expect<WxePageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("MyAspLinkButton|MyAspLinkButtonCommand"));

      var aspHyperLink = home.GetAnchor().ByLocalID ("MyAspHyperLink");
      home = aspHyperLink.Click().Expect<WxePageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);

      var htmlAnchor = home.GetAnchor().ByLocalID ("MyHtmlAnchor");
      home = htmlAnchor.Click().Expect<WxePageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("MyHtmlAnchor"));

      var htmlAnchorWithJavaScriptLink = home.GetAnchor().ByLocalID ("MyHtmlAnchorWithJavaScriptLink");
      home = htmlAnchorWithJavaScriptLink.Click().Expect<WxePageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("MyHtmlAnchorWithJavaScriptLink"));
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject> ("AnchorTest.wxe");
    }
  }
}