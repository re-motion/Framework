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
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects.Selectors;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class AnchorControlObjectTest : IntegrationTest
  {
    [Test]
    [TestCaseSource(typeof(HtmlIDControlSelectorTestCaseFactory<AnchorSelector, AnchorControlObject>))]
    [TestCaseSource(typeof(IndexControlSelectorTestCaseFactory<AnchorSelector, AnchorControlObject>))]
    [TestCaseSource(typeof(LocalIDControlSelectorTestCaseFactory<AnchorSelector, AnchorControlObject>))]
    [TestCaseSource(typeof(TextContentControlSelectorTestCaseFactory<AnchorSelector, AnchorControlObject>))]
    [TestCaseSource(typeof(FirstControlSelectorTestCaseFactory<AnchorSelector, AnchorControlObject>))]
    [TestCaseSource(typeof(SingleControlSelectorTestCaseFactory<AnchorSelector, AnchorControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<AnchorSelector, AnchorControlObject> testAction)
    {
      testAction(Helper, e => e.Anchors(), "anchor");
    }

    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var webLinkButton = home.Anchors().GetByID("body_MyWebLinkButton");
      Assert.That(webLinkButton.Scope.Id, Is.EqualTo("body_MyWebLinkButton"));

      var smartHyperLink = home.Anchors().GetByID("body_MySmartHyperLink");
      Assert.That(smartHyperLink.Scope.Id, Is.EqualTo("body_MySmartHyperLink"));

      var aspLinkButton = home.Anchors().GetByID("body_MyAspLinkButton");
      Assert.That(aspLinkButton.Scope.Id, Is.EqualTo("body_MyAspLinkButton"));

      var aspHyperLink = home.Anchors().GetByID("body_MyAspHyperLink");
      Assert.That(aspHyperLink.Scope.Id, Is.EqualTo("body_MyAspHyperLink"));

      var htmlAnchor = home.Anchors().GetByID("body_MyHtmlAnchor");
      Assert.That(htmlAnchor.Scope.Id, Is.EqualTo("body_MyHtmlAnchor"));

      var htmlAnchorWithJavaScriptLink = home.Anchors().GetByID("body_MyHtmlAnchorWithJavaScriptLink");
      Assert.That(htmlAnchorWithJavaScriptLink.Scope.Id, Is.EqualTo("body_MyHtmlAnchorWithJavaScriptLink"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var webLinkButton = home.Anchors().GetByLocalID("MyWebLinkButton");
      Assert.That(webLinkButton.Scope.Id, Is.EqualTo("body_MyWebLinkButton"));

      var smartHyperLink = home.Anchors().GetByLocalID("MySmartHyperLink");
      Assert.That(smartHyperLink.Scope.Id, Is.EqualTo("body_MySmartHyperLink"));

      var aspLinkButton = home.Anchors().GetByLocalID("MyAspLinkButton");
      Assert.That(aspLinkButton.Scope.Id, Is.EqualTo("body_MyAspLinkButton"));

      var aspHyperLink = home.Anchors().GetByLocalID("MyAspHyperLink");
      Assert.That(aspHyperLink.Scope.Id, Is.EqualTo("body_MyAspHyperLink"));

      var htmlAnchor = home.Anchors().GetByLocalID("MyHtmlAnchor");
      Assert.That(htmlAnchor.Scope.Id, Is.EqualTo("body_MyHtmlAnchor"));

      var htmlAnchorWithJavaScriptLink = home.Anchors().GetByLocalID("MyHtmlAnchorWithJavaScriptLink");
      Assert.That(htmlAnchorWithJavaScriptLink.Scope.Id, Is.EqualTo("body_MyHtmlAnchorWithJavaScriptLink"));
    }

    [Test]
    public void TestGetText ()
    {
      var home = Start();

      var webLinkButton = home.Anchors().GetByLocalID("MyWebLinkButton");
      Assert.That(webLinkButton.GetText(), Is.EqualTo("MyWebLinkButton"));

      var smartHyperLink = home.Anchors().GetByLocalID("MySmartHyperLink");
      Assert.That(smartHyperLink.GetText(), Is.EqualTo("MySmartHyperLink"));

      var aspLinkButton = home.Anchors().GetByLocalID("MyAspLinkButton");
      Assert.That(aspLinkButton.GetText(), Is.EqualTo("MyAspLinkButton"));

      var aspHyperLink = home.Anchors().GetByLocalID("MyAspHyperLink");
      Assert.That(aspHyperLink.GetText(), Is.EqualTo("MyAspHyperLink"));

      var htmlAnchor = home.Anchors().GetByLocalID("MyHtmlAnchor");
      Assert.That(htmlAnchor.GetText(), Is.EqualTo("MyHtmlAnchor"));

      var htmlAnchorWithJavaScriptLink = home.Anchors().GetByLocalID("MyHtmlAnchorWithJavaScriptLink");
      Assert.That(htmlAnchorWithJavaScriptLink.GetText(), Is.EqualTo("MyHtmlAnchorWithJavaScriptLink"));
    }

    [Test]
    public void TestClick ()
    {
      var home = Start();

      var webLinkButton = home.Anchors().GetByLocalID("MyWebLinkButton");
      var completionDetection = new CompletionDetectionStrategyTestHelper(webLinkButton);
      home = webLinkButton.Click().Expect<WxePageObject>();
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(home.Scope.FindId("TestOutputLabel").Text, Is.EqualTo("MyWebLinkButton|MyWebLinkButtonCommand"));

      var smartHyperLink = home.Anchors().GetByLocalID("MySmartHyperLink");
      home = smartHyperLink.Click().Expect<WxePageObject>();
      Assert.That(home.Scope.FindId("TestOutputLabel").Text, Is.Empty);

      var aspLinkButton = home.Anchors().GetByLocalID("MyAspLinkButton");
      home = aspLinkButton.Click().Expect<WxePageObject>();
      Assert.That(home.Scope.FindId("TestOutputLabel").Text, Is.EqualTo("MyAspLinkButton|MyAspLinkButtonCommand"));

      var aspHyperLink = home.Anchors().GetByLocalID("MyAspHyperLink");
      home = aspHyperLink.Click().Expect<WxePageObject>();
      Assert.That(home.Scope.FindId("TestOutputLabel").Text, Is.Empty);

      var htmlAnchor = home.Anchors().GetByLocalID("MyHtmlAnchor");
      home = htmlAnchor.Click().Expect<WxePageObject>();
      Assert.That(home.Scope.FindId("TestOutputLabel").Text, Is.EqualTo("MyHtmlAnchor"));

      var htmlAnchorWithJavaScriptLink = home.Anchors().GetByLocalID("MyHtmlAnchorWithJavaScriptLink");
      home = htmlAnchorWithJavaScriptLink.Click().Expect<WxePageObject>();
      Assert.That(home.Scope.FindId("TestOutputLabel").Text, Is.EqualTo("MyHtmlAnchorWithJavaScriptLink"));
    }

    [Test]
    public void TestGetByTextContent_WithSingleQuote ()
    {
      var home = Start();

      var anchor = home.Anchors().GetByTextContent("With'SingleQuote");

      Assert.That(anchor.GetText(), Is.EqualTo("With'SingleQuote"));
    }

    [Test]
    public void TestGetByTextContentOrNull_WithSingleQuote ()
    {
      var home = Start();

      var anchor = home.Anchors().GetByTextContentOrNull("With'SingleQuote");

      Assert.That(anchor.GetText(), Is.EqualTo("With'SingleQuote"));
    }

    [Test]
    public void TestExistsByTextContent_WithSingleQuote ()
    {
      var home = Start();

      var anchorExists = home.Anchors().ExistsByTextContent("With'SingleQuote");

      Assert.That(anchorExists, Is.True);
    }

    [Test]
    public void TestGetByTextContent_WithSingleQuoteAndDoubleQuote ()
    {
      var home = Start();

      var anchor = home.Anchors().GetByTextContent("With'SingleQuoteAnd\"DoubleQuote");

      Assert.That(anchor.GetText(), Is.EqualTo("With'SingleQuoteAnd\"DoubleQuote"));
    }

    [Test]
    public void TestGetByTextContentOrNull_WithSingleQuoteAndDoubleQuote ()
    {
      var home = Start();

      var anchor = home.Anchors().GetByTextContentOrNull("With'SingleQuoteAnd\"DoubleQuote");

      Assert.That(anchor.GetText(), Is.EqualTo("With'SingleQuoteAnd\"DoubleQuote"));
    }

    [Test]
    public void TestExistsByTextContent_WithSingleQuoteAndDoubleQuote ()
    {
      var home = Start();

      var anchorExists = home.Anchors().ExistsByTextContent("With'SingleQuoteAnd\"DoubleQuote");

      Assert.That(anchorExists, Is.True);
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject>("AnchorTest.wxe");
    }
  }
}
