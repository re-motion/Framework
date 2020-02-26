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
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.Accessibility.Implementation;

namespace Remotion.Web.Development.WebTesting.UnitTests.Accessibility
{
  [TestFixture]
  public class AxeResultParserTest
  {
    [Test]
    public void ParseResults_MinimalValidRawJson ()
    {
      var jsonResponse =
          "{\"url\":\"TestURL\",\"violations\":[{\"description\":\"TestDescription\",\"help\":\"TestHelp\",\"helpUrl\":\"TestHelpUrl\",\"id\":\"TestId\",\"impact\":\"critical\",\"nodes\":[{\"all\":[],\"any\":[{\"data\":[\"aria-expanded\"],\"id\":\"aria-required-attr\",\"impact\":\"critical\",\"message\":\"TestMessage\",\"relatedNodes\":[]}],\"failureSummary\":\"TestFailureSummary\",\"html\":\"TestHTML\",\"impact\":\"critical\",\"none\":[],\"target\":[\"TestTarget\"],\"xpath\":[\"XPathTest\"]}],\"tags\":[\"wcag2a\",\"wcag412\"]},{\"description\":\"TestDescription2\",\"help\":\"TestHelp2\",\"helpUrl\":\"TestHelpUrl2\",\"id\":\"TestId2\",\"impact\":\"serious\",\"nodes\":[{\"all\":[],\"any\":[{\"data\":null,\"id\":\"internal-link-present\",\"impact\":\"serious\",\"message\":\"TestMessage2\",\"relatedNodes\":[]}],\"failureSummary\":\"TestFailureSummary2\",\"html\":\"TestHTML2\",\"impact\":\"serious\",\"none\":[],\"target\":[\"html\"],\"xpath\":[\"XPathTest\"]}],\"tags\":[\"cat.keyboard\"]}]}";

      var parser = new AxeResultParser();

      var result = parser.Parse (jsonResponse);

      var violationsCount = result.Violations.Length;
      Assert.That (violationsCount, Is.EqualTo (2));
      Assert.That (result.Violations[0].ID, Is.EqualTo ("TestId"));
      Assert.That (result.Violations[0].Nodes[0].Any[0].ID, Is.EqualTo ("aria-required-attr"));
      Assert.That (result.Violations[0].Description, Is.EqualTo ("TestDescription"));
    }

    [Test]
    [TestCase ("<violations>test</violations>")]
    [TestCase ("{[[}")]
    public void Parse_WithInvalidFormat_ThrowsException (string jsonResponse)
    {
      var parser = new AxeResultParser();

      Assert.That (() => parser.Parse (jsonResponse), Throws.InstanceOf<SerializationException>());
    }

    [Test]
    public void Parse_WithTestResultsContainsViolations ()
    {
      var jsonResponse =
          "{\"inapplicable\":[],\"incomplete\":[],\"passes\":[],\"testEngine\":{\"name\":\"engineTest\",\"version\":\"3.2.2\"},\"testEnvironment\":{\"orientationAngle\":0,\"orientationType\":\"type\",\"userAgent\":\"Mozilla\",\"windowHeight\":1008,\"windowWidth\":929},\"testRunner\":{\"name\":\"axe\"},\"timestamp\":\"2019-07-05T07:38:54.066Z\",\"toolOptions\":{\"iframes\":true,\"reporter\":\"v1\",\"runOnly\":{\"type\":\"tag\",\"values\":[\"wcag2a\"]}},\"url\":\"url\",\"violations\":[{\"description\":\"testDescription\",\"help\":\"testHelp\",\"helpUrl\":\"helpUrl\",\"id\":\"testId\",\"impact\":\"serious\",\"nodes\":[{\"all\":[],\"any\":[{\"data\":null,\"id\":\"landmark\",\"impact\":\"serious\",\"message\":\"testMessage\",\"relatedNodes\":[{\"html\":\"RelatedHtml\",\"target\":[\"RelatedTarget\"],\"xpath\":[\"RelatedXPath\"]}]}],\"failureSummary\":\"FailureSummary\",\"html\":\"testHtml\",\"impact\":\"serious\",\"none\":[],\"target\":[\"html\"],\"xpath\":[\"XPathTest\"]}],\"tags\":[\"cat.keyboard\",\"wcag2a\"]}]}";
      var parser = new AxeResultParser();

      var result = parser.Parse (jsonResponse);

      Assert.That (result.Inapplicable.Length, Is.EqualTo (0));
      Assert.That (result.Incomplete.Length, Is.EqualTo (0));
      Assert.That (result.Passes.Length, Is.EqualTo (0));
      Assert.That (result.TestEngine.Version, Is.EqualTo ("3.2.2"));
      Assert.That (result.TestEngine.Name, Is.EqualTo ("engineTest"));
      Assert.That (result.TestEnvironment.OrientationAngle, Is.EqualTo (0));
      Assert.That (result.TestEnvironment.OrientationType, Is.EqualTo ("type"));
      Assert.That (result.TestEnvironment.UserAgent, Is.EqualTo ("Mozilla"));
      Assert.That (result.TestEnvironment.WindowHeight, Is.EqualTo (1008));
      Assert.That (result.TestEnvironment.WindowWidth, Is.EqualTo (929));
      Assert.That (result.TestRunner.Name, Is.EqualTo ("axe"));
      Assert.That (result.Timestamp.ToString, Is.EqualTo ("2019-07-05T07:38:54.066Z"));
      Assert.That (result.ToolOptions.IFrames, Is.True);
      Assert.That (result.ToolOptions.FrameWaitTime, Is.EqualTo (0));
      Assert.That (result.ToolOptions.AbsolutePaths, Is.False);
      Assert.That (result.ToolOptions.RestoreScroll, Is.False);
      Assert.That (result.ToolOptions.XPath, Is.False);
      Assert.That (result.ToolOptions.RunOnly.Type, Is.EqualTo ("tag"));
      Assert.That (result.ToolOptions.RunOnly.Values.Length, Is.EqualTo (1));
      Assert.That (result.Url, Is.EqualTo ("url"));
      Assert.That (result.Violations.Length, Is.EqualTo (1));
      Assert.That (result.Violations[0].Description, Is.EqualTo ("testDescription"));
      Assert.That (result.Violations[0].Help, Is.EqualTo ("testHelp"));
      Assert.That (result.Violations[0].HelpUrl, Is.EqualTo ("helpUrl"));
      Assert.That (result.Violations[0].ID, Is.EqualTo ("testId"));
      Assert.That (result.Violations[0].Impact, Is.EqualTo ("serious"));
      Assert.That (result.Violations[0].Nodes.Length, Is.EqualTo (1));
      Assert.That (result.Violations[0].Nodes[0].Html, Is.EqualTo ("testHtml"));
      Assert.That (result.Violations[0].Nodes[0].XPaths.Length, Is.EqualTo (1));
      Assert.That (result.Violations[0].Nodes[0].XPaths[0], Is.EqualTo ("XPathTest"));
      Assert.That (result.Violations[0].Nodes[0].None.Length, Is.EqualTo (0));
      Assert.That (result.Violations[0].Nodes[0].Any.Length, Is.EqualTo (1));
      Assert.That (result.Violations[0].Nodes[0].Any[0].Data, Is.Null);
      Assert.That (result.Violations[0].Nodes[0].Any[0].ID, Is.EqualTo ("landmark"));
      Assert.That (result.Violations[0].Nodes[0].Any[0].Impact, Is.EqualTo ("serious"));
      Assert.That (result.Violations[0].Nodes[0].Any[0].Message, Is.EqualTo ("testMessage"));
      Assert.That (result.Violations[0].Nodes[0].Any[0].RelatedNodes[0].Html, Is.EqualTo ("RelatedHtml"));
      Assert.That (result.Violations[0].Nodes[0].Any[0].RelatedNodes[0].Target[0], Is.EqualTo ("RelatedTarget"));
      Assert.That (result.Violations[0].Nodes[0].Any[0].RelatedNodes[0].XPath[0], Is.EqualTo ("RelatedXPath"));
      Assert.That (result.Violations[0].Nodes[0].Target.Length, Is.EqualTo (1));
      Assert.That (result.Violations[0].Nodes[0].Target[0], Is.EqualTo ("html"));
      Assert.That (result.Violations[0].Tags.Length, Is.EqualTo (2));
      Assert.That (result.Violations[0].Tags[0], Is.EqualTo ("cat.keyboard"));
      Assert.That (result.Violations[0].Tags[1], Is.EqualTo ("wcag2a"));
    }

    [Test]
    public void Parse_WithTestResultsContainsPasses ()
    {
      var jsonResponse =
          "{\"inapplicable\":[],\"incomplete\":[],\"passes\":[{\"description\":\"testDescription\",\"help\":\"testHelp\",\"helpUrl\":\"helpUrl\",\"id\":\"testId\",\"impact\":\"serious\",\"nodes\":[{\"all\":[],\"any\":[{\"data\":null,\"id\":\"landmark\",\"impact\":\"serious\",\"message\":\"testMessage\",\"relatedNodes\":[]}],\"failureSummary\":\"FailureSummary\",\"html\":\"testHtml\",\"impact\":\"serious\",\"none\":[],\"target\":[\"html\"],\"xpath\":[\"XPathTest\"]}],\"tags\":[\"cat.keyboard\",\"wcag2a\"]}],\"testEngine\":{\"name\":\"engineTest\",\"version\":\"3.2.2\"},\"testEnvironment\":{\"orientationAngle\":0,\"orientationType\":\"type\",\"userAgent\":\"Mozilla\",\"windowHeight\":1008,\"windowWidth\":929},\"testRunner\":{\"name\":\"axe\"},\"timestamp\":\"2019-07-05T07:38:54.066Z\",\"toolOptions\":{\"iframes\":true,\"reporter\":\"v1\",\"runOnly\":{\"type\":\"tag\",\"values\":[\"wcag2a\"]}},\"url\":\"url\",\"violations\":[]}";
      var parser = new AxeResultParser();

      var result = parser.Parse (jsonResponse);

      Assert.That (result.Inapplicable.Length, Is.EqualTo (0));
      Assert.That (result.Incomplete.Length, Is.EqualTo (0));
      Assert.That (result.Passes.Length, Is.EqualTo (1));
      Assert.That (result.Violations.Length, Is.EqualTo (0));
      Assert.That (result.Passes.Length, Is.EqualTo (1));
      Assert.That (result.Passes[0].Description, Is.EqualTo ("testDescription"));
      Assert.That (result.Passes[0].Help, Is.EqualTo ("testHelp"));
      Assert.That (result.Passes[0].HelpUrl, Is.EqualTo ("helpUrl"));
      Assert.That (result.Passes[0].ID, Is.EqualTo ("testId"));
      Assert.That (result.Passes[0].Impact, Is.EqualTo ("serious"));
      Assert.That (result.Passes[0].Nodes.Length, Is.EqualTo (1));
      Assert.That (result.Passes[0].Nodes[0].Html, Is.EqualTo ("testHtml"));
      Assert.That (result.Passes[0].Nodes[0].XPaths.Length, Is.EqualTo (1));
      Assert.That (result.Passes[0].Nodes[0].XPaths[0], Is.EqualTo ("XPathTest"));
      Assert.That (result.Passes[0].Nodes[0].None.Length, Is.EqualTo (0));
      Assert.That (result.Passes[0].Nodes[0].Any.Length, Is.EqualTo (1));
      Assert.That (result.Passes[0].Nodes[0].Any[0].Data, Is.Null);
      Assert.That (result.Passes[0].Nodes[0].Any[0].ID, Is.EqualTo ("landmark"));
      Assert.That (result.Passes[0].Nodes[0].Any[0].Impact, Is.EqualTo ("serious"));
      Assert.That (result.Passes[0].Nodes[0].Any[0].Message, Is.EqualTo ("testMessage"));
      Assert.That (result.Passes[0].Nodes[0].Target.Length, Is.EqualTo (1));
      Assert.That (result.Passes[0].Nodes[0].Target[0], Is.EqualTo ("html"));
      Assert.That (result.Passes[0].Tags.Length, Is.EqualTo (2));
      Assert.That (result.Passes[0].Tags[0], Is.EqualTo ("cat.keyboard"));
      Assert.That (result.Passes[0].Tags[1], Is.EqualTo ("wcag2a"));
    }

    [Test]
    public void Parse_WithTestResultsContainsIncompletes ()
    {
      var jsonResponse =
          "{\"inapplicable\":[],\"incomplete\":[{\"description\":\"testDescription\",\"help\":\"testHelp\",\"helpUrl\":\"helpUrl\",\"id\":\"testId\",\"impact\":\"serious\",\"nodes\":[{\"all\":[],\"any\":[{\"data\":null,\"id\":\"landmark\",\"impact\":\"serious\",\"message\":\"testMessage\",\"relatedNodes\":[]}],\"failureSummary\":\"FailureSummary\",\"html\":\"testHtml\",\"impact\":\"serious\",\"none\":[],\"target\":[\"html\"],\"xpath\":[\"XPathTest\"]}],\"tags\":[\"cat.keyboard\",\"wcag2a\"]}],\"passes\":[],\"testEngine\":{\"name\":\"engineTest\",\"version\":\"3.2.2\"},\"testEnvironment\":{\"orientationAngle\":0,\"orientationType\":\"type\",\"userAgent\":\"Mozilla\",\"windowHeight\":1008,\"windowWidth\":929},\"testRunner\":{\"name\":\"axe\"},\"timestamp\":\"2019-07-05T07:38:54.066Z\",\"toolOptions\":{\"iframes\":true,\"reporter\":\"v1\",\"runOnly\":{\"type\":\"tag\",\"values\":[\"wcag2a\"]}},\"url\":\"url\",\"violations\":[]}";
      var parser = new AxeResultParser();

      var result = parser.Parse (jsonResponse);

      Assert.That (result.Inapplicable.Length, Is.EqualTo (0));
      Assert.That (result.Incomplete.Length, Is.EqualTo (1));
      Assert.That (result.Passes.Length, Is.EqualTo (0));
      Assert.That (result.Violations.Length, Is.EqualTo (0));
      Assert.That (result.Incomplete.Length, Is.EqualTo (1));
      Assert.That (result.Incomplete[0].Description, Is.EqualTo ("testDescription"));
      Assert.That (result.Incomplete[0].Help, Is.EqualTo ("testHelp"));
      Assert.That (result.Incomplete[0].HelpUrl, Is.EqualTo ("helpUrl"));
      Assert.That (result.Incomplete[0].ID, Is.EqualTo ("testId"));
      Assert.That (result.Incomplete[0].Impact, Is.EqualTo ("serious"));
      Assert.That (result.Incomplete[0].Nodes.Length, Is.EqualTo (1));
      Assert.That (result.Incomplete[0].Nodes[0].Html, Is.EqualTo ("testHtml"));
      Assert.That (result.Incomplete[0].Nodes[0].XPaths.Length, Is.EqualTo (1));
      Assert.That (result.Incomplete[0].Nodes[0].XPaths[0], Is.EqualTo ("XPathTest"));
      Assert.That (result.Incomplete[0].Nodes[0].None.Length, Is.EqualTo (0));
      Assert.That (result.Incomplete[0].Nodes[0].Any.Length, Is.EqualTo (1));
      Assert.That (result.Incomplete[0].Nodes[0].Any[0].Data, Is.Null);
      Assert.That (result.Incomplete[0].Nodes[0].Any[0].ID, Is.EqualTo ("landmark"));
      Assert.That (result.Incomplete[0].Nodes[0].Any[0].Impact, Is.EqualTo ("serious"));
      Assert.That (result.Incomplete[0].Nodes[0].Any[0].Message, Is.EqualTo ("testMessage"));
      Assert.That (result.Incomplete[0].Nodes[0].Target.Length, Is.EqualTo (1));
      Assert.That (result.Incomplete[0].Nodes[0].Target[0], Is.EqualTo ("html"));
      Assert.That (result.Incomplete[0].Tags.Length, Is.EqualTo (2));
      Assert.That (result.Incomplete[0].Tags[0], Is.EqualTo ("cat.keyboard"));
      Assert.That (result.Incomplete[0].Tags[1], Is.EqualTo ("wcag2a"));
    }

    [Test]
    public void Parse_WithTestResultsContainsInapplicables ()
    {
      var jsonResponse =
          "{\"inapplicable\":[{\"description\":\"testDescription\",\"help\":\"testHelp\",\"helpUrl\":\"helpUrl\",\"id\":\"testId\",\"impact\":\"serious\",\"nodes\":[{\"all\":[],\"any\":[{\"data\":null,\"id\":\"landmark\",\"impact\":\"serious\",\"message\":\"testMessage\",\"relatedNodes\":[]}],\"failureSummary\":\"FailureSummary\",\"html\":\"testHtml\",\"impact\":\"serious\",\"none\":[],\"target\":[\"html\"],\"xpath\":[\"XPathTest\"]}],\"tags\":[\"cat.keyboard\",\"wcag2a\"]}],\"incomplete\":[],\"passes\":[],\"testEngine\":{\"name\":\"engineTest\",\"version\":\"3.2.2\"},\"testEnvironment\":{\"orientationAngle\":0,\"orientationType\":\"type\",\"userAgent\":\"Mozilla\",\"windowHeight\":1008,\"windowWidth\":929},\"testRunner\":{\"name\":\"axe\"},\"timestamp\":\"2019-07-05T07:38:54.066Z\",\"toolOptions\":{\"iframes\":true,\"reporter\":\"v1\",\"runOnly\":{\"type\":\"tag\",\"values\":[\"wcag2a\"]}},\"url\":\"url\",\"violations\":[]}";

      var parser = new AxeResultParser();

      var result = parser.Parse (jsonResponse);

      Assert.That (result.Inapplicable.Length, Is.EqualTo (1));
      Assert.That (result.Incomplete.Length, Is.EqualTo (0));
      Assert.That (result.Passes.Length, Is.EqualTo (0));
      Assert.That (result.Violations.Length, Is.EqualTo (0));
      Assert.That (result.Inapplicable[0].Description, Is.EqualTo ("testDescription"));
      Assert.That (result.Inapplicable[0].Help, Is.EqualTo ("testHelp"));
      Assert.That (result.Inapplicable[0].HelpUrl, Is.EqualTo ("helpUrl"));
      Assert.That (result.Inapplicable[0].ID, Is.EqualTo ("testId"));
      Assert.That (result.Inapplicable[0].Impact, Is.EqualTo ("serious"));
      Assert.That (result.Inapplicable[0].Nodes.Length, Is.EqualTo (1));
      Assert.That (result.Inapplicable[0].Nodes[0].Html, Is.EqualTo ("testHtml"));
      Assert.That (result.Inapplicable[0].Nodes[0].XPaths.Length, Is.EqualTo (1));
      Assert.That (result.Inapplicable[0].Nodes[0].XPaths[0], Is.EqualTo ("XPathTest"));
      Assert.That (result.Inapplicable[0].Nodes[0].None.Length, Is.EqualTo (0));
      Assert.That (result.Inapplicable[0].Nodes[0].Any.Length, Is.EqualTo (1));
      Assert.That (result.Inapplicable[0].Nodes[0].Any[0].Data, Is.Null);
      Assert.That (result.Inapplicable[0].Nodes[0].Any[0].ID, Is.EqualTo ("landmark"));
      Assert.That (result.Inapplicable[0].Nodes[0].Any[0].Impact, Is.EqualTo ("serious"));
      Assert.That (result.Inapplicable[0].Nodes[0].Any[0].Message, Is.EqualTo ("testMessage"));
      Assert.That (result.Inapplicable[0].Nodes[0].Target.Length, Is.EqualTo (1));
      Assert.That (result.Inapplicable[0].Nodes[0].Target[0], Is.EqualTo ("html"));
      Assert.That (result.Inapplicable[0].Tags.Length, Is.EqualTo (2));
      Assert.That (result.Inapplicable[0].Tags[0], Is.EqualTo ("cat.keyboard"));
      Assert.That (result.Inapplicable[0].Tags[1], Is.EqualTo ("wcag2a"));
    }

    [Test]
    public void Parse_WithEmptyViolations_AccessibilityResultWithEmptyViolations ()
    {
      var jsonResponse =
          "{\"inapplicable\":[],\"incomplete\":[],\"passes\":[],\"testEngine\":{\"name\":\"axe-core\",\"version\":\"3.2.2\"},\"testEnvironment\":{\"orientationAngle\":0,\"orientationType\":\"landscape-primary\",\"userAgent\":\"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36\",\"windowHeight\":1008,\"windowWidth\":929},\"testRunner\":{\"name\":\"axe\"},\"timestamp\":\"2019-07-03T13:10:42.964Z\",\"toolOptions\":{\"iframes\":true,\"reporter\":\"v1\",\"runOnly\":{\"type\":\"tag\",\"values\":[\"wcag2a\"]}},\"url\":\"https://www.google.com/\",\"violations\":[]}";
      var parser = new AxeResultParser();

      var axeResult = parser.Parse (jsonResponse);

      Assert.That (axeResult.Violations.Length, Is.EqualTo (0));
    }

    [Test]
    [TestCase (null, typeof (ArgumentNullException))]
    [TestCase ("", typeof (ArgumentException))]
    public void Parse_WithNullOrEmptyJson_ThrowsException (string rawJson, Type exceptionType)
    {
      var parser = new AxeResultParser();

      Assert.That (() => parser.Parse (rawJson), Throws.InstanceOf (exceptionType));
    }

    [Test]
    public void Parse_WithMinimalValidRawJsonWithNoneNodes ()
    {
      var jsonResponse =
          "{\"url\":\"TestURL\",\"violations\":[{\"description\":\"TestDescription\",\"help\":\"TestHelp\",\"helpUrl\":\"TestHelpUrl\",\"id\":\"TestId\",\"impact\":\"critical\",\"nodes\":[{\"all\":[],\"any\":[{\"data\":[\"aria-expanded\"],\"id\":\"aria-required-attr\",\"impact\":\"critical\",\"message\":\"TestMessage\",\"relatedNodes\":[]}],\"failureSummary\":\"TestFailureSummary\",\"html\":\"TestHTML\",\"impact\":\"critical\",\"none\":[],\"target\":[\"TestTarget\"],\"xpath\":[\"XPathTest\"]}],\"tags\":[\"wcag2a\",\"wcag412\"]},{\"description\":\"TestDescription2\",\"help\":\"TestHelp2\",\"helpUrl\":\"TestHelpUrl2\",\"id\":\"TestId2\",\"impact\":\"serious\",\"nodes\":[{\"all\":[],\"any\":[{\"data\":null,\"id\":\"internal-link-present\",\"impact\":\"serious\",\"message\":\"TestMessage2\",\"relatedNodes\":[]}],\"failureSummary\":\"TestFailureSummary2\",\"html\":\"TestHTML2\",\"impact\":\"serious\",\"none\":[],\"target\":[\"html\"],\"xpath\":[\"XPathTest\"]}],\"tags\":[\"cat.keyboard\"]}]}";

      var parser = new AxeResultParser();

      var result = parser.Parse (jsonResponse);

      var violationsCount = result.Violations.Length;
      Assert.That (violationsCount, Is.EqualTo (2));
      Assert.That (result.Violations[0].ID, Is.EqualTo ("TestId"));
      Assert.That (result.Violations[0].Nodes[0].Any[0].ID, Is.EqualTo ("aria-required-attr"));
    }
  }
}