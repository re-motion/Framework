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
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.Web.Development.WebTesting.Accessibility;
using Remotion.Web.Development.WebTesting.Accessibility.AxeJsonResultDtos;
using Remotion.Web.Development.WebTesting.Accessibility.Implementation;

namespace Remotion.Web.Development.WebTesting.UnitTests.Accessibility
{
  [TestFixture]
  public class AccessibilityResultMapperTest
  {
    [Test]
    public void Map_ParseSuccessCriteriaTags ()
    {
      var mapper = new AccessibilityResultMapper();

      var axeResult = CreateAxeResult (tags: new[] { "wcag2aa", "wcag111", "section508.22.a" });

      var finalResult = mapper.Map (axeResult);

      Assert.That (finalResult.ConformanceLevel, Is.EqualTo (AccessibilityConformanceLevel.Wcag20_ConformanceLevelDoubleA));
      Assert.That (finalResult.Violations.Single().Rule.SuccessCriteria.Contains (AccessibilityTestSuccessCriteria.Wcag_1_1_1));
      Assert.That (finalResult.Violations.Single().Rule.SuccessCriteria.Contains (AccessibilityTestSuccessCriteria.Section508_22_a));
    }

    [Test]
    public void Map_InvalidTimeStamp_ThrowsFormatException ()
    {
      var mapper = new AccessibilityResultMapper();
      var testResult = CreateAxeResult (timestamp: "invalidTimestamp");

      Assert.That (() => mapper.Map (testResult), Throws.TypeOf<FormatException>());
    }

    [Test]
    public void Map_WellKnownSuccessCriteriaToIgnore_ThrowsNothing ()
    {
      var mapper = new AccessibilityResultMapper();
      var testResult = CreateAxeResult (tags: new[] { "wcag2aa", "wcag111", "cat.sensory-and-visual-cues" });

      Assert.That (() => mapper.Map (testResult), Throws.Nothing);
    }

    [Test]
    public void Map_ParseUnknownRuleID ()
    {
      var axeResult = CreateAxeResult (ruleId: "SomeUnknownRuleID");
      var mapper = new AccessibilityResultMapper();

      var finalResult = mapper.Map (axeResult);

      Assert.That (finalResult.Violations.Single().Rule.ID, Is.EqualTo (AccessibilityRuleID.Unknown));
    }

    [Test]
    public void Map_ParseUnknownCheckID ()
    {
      var axeResult = CreateAxeResult (ruleId: "SomeUnknownRuleID");
      var mapper = new AccessibilityResultMapper();

      var finalResult = mapper.Map (axeResult);

      Assert.That (finalResult.Violations.Single().Any.Single().ID, Is.EqualTo (AccessibilityRequirementID.Unknown));
    }

    [Test]
    public void Map_StringRepresentation ()
    {
      var mapper = new AccessibilityResultMapper();
      var axeResult = CreateAxeResult (tags: new[] { "wcag2a", "wcag111" }, ruleId: "accesskeys", xPath: new[] { "TargetTest" }, cssTargets: new[] { "TargetTest" });

      var finalResult = mapper.Map (axeResult);

      Assert.That (
          finalResult.ToString(),
          Is.EqualTo (
              "Tag: <Wcag20_ConformanceLevelA>, Violations: <<Rule: AccessKeys, XPath: [\"TargetTest\"]>>"));
    }

    [Test]
    public void Map_ConvertsAll ()
    {
      var mapper = new AccessibilityResultMapper();
      var includeIframes = BooleanObjectMother.GetRandomBoolean();
      const string timestamp = "2018-06-04T07:37:53.055Z";
      var testResult = CreateAxeResult (
          axeVersion: "3.2.2",
          userAgent: "Firefox",
          url: "MyOwnTestingUrl",
          orientationType: "landscape",
          orientationAngle: 1,
          windowWidth: 2,
          windowHeight: 3,
          tags: new[] { "wcag2aa", "wcag123" },
          timestamp: timestamp,
          includeIFrames: includeIframes);

      var finalResult = mapper.Map (testResult);

      Assert.That (finalResult.AxeVersion, Is.EqualTo ("3.2.2"));
      Assert.That (finalResult.UserAgent, Is.EqualTo ("Firefox"));
      Assert.That (finalResult.Url, Is.EqualTo ("MyOwnTestingUrl"));
      Assert.That (finalResult.OrientationType, Is.EqualTo ("landscape"));
      Assert.That (finalResult.OrientationAngle, Is.EqualTo (1));
      Assert.That (finalResult.WindowWidth, Is.EqualTo (2));
      Assert.That (finalResult.WindowHeight, Is.EqualTo (3));
      Assert.That (finalResult.ConformanceLevel, Is.EqualTo (AccessibilityConformanceLevel.Wcag20_ConformanceLevelDoubleA));
      Assert.That (
          finalResult.Timestamp.ToUniversalTime().ToString ("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.CreateSpecificCulture ("de-AT")),
          Is.EqualTo (timestamp));
      Assert.That (finalResult.IncludeIFrames, Is.EqualTo (includeIframes));
    }

    [Test]
    public void Map_DifferentXPathAndTargetLengths_ThrowsFormatException ()
    {
      var mapper = new AccessibilityResultMapper();

      var testResult = CreateAxeResult (xPath: new[] { "XPath1", "XPath2", "XPath3" }, cssTargets: new[] { "Target1", "Target2" });

      Assert.That (() => mapper.Map (testResult), Throws.TypeOf<InvalidOperationException>());
    }

    private AxeResult CreateAxeResult (
        string axeVersion = null,
        string userAgent = null,
        string ruleId = null,
        string[] tags = null,
        string impact = null,
        string[] xPath = null,
        string[] cssTargets = null,
        string url = null,
        string orientationType = null,
        int? orientationAngle = null,
        int? windowWidth = null,
        int? windowHeight = null,
        bool? includeIFrames = null,
        string timestamp = null)
    {
      ruleId = ruleId ?? "blink";
      impact = impact ?? "serious";
      xPath = xPath ?? new[] { "A", "B" };
      cssTargets = cssTargets ?? new[] { "C", "D" };
      tags = tags ?? new[] { "wcag2aa", "wcag111" };
      url = url ?? "TestUrl";
      orientationType = orientationType ?? "portrait";
      orientationAngle = orientationAngle ?? 1337;
      windowWidth = windowWidth ?? 42;
      windowHeight = windowHeight ?? 9001;
      userAgent = userAgent ?? "Chrome";
      axeVersion = axeVersion ?? "1.2.3";
      includeIFrames = includeIFrames ?? true;
      timestamp = timestamp ?? "2019-07-05T07:38:54.066Z";

      var rule = new AxeRuleResult
                 {
                     ID = ruleId,
                     Tags = tags,
                     Description = "Rule Description",
                     Nodes = new[] { CreateRuleNode (ruleId, impact, xPath, cssTargets) },
                     Impact = impact,
                 };

      return new AxeResult
             {
                 Url = url,
                 Timestamp = timestamp,
                 TestEngine = new AxeTestEngine
                              {
                                  Version = axeVersion
                              },
                 TestEnvironment = new AxeTestEnvironment
                                   {
                                       OrientationAngle = orientationAngle.Value,
                                       OrientationType = orientationType,
                                       WindowWidth = windowWidth.Value,
                                       WindowHeight = windowHeight.Value,
                                       UserAgent = userAgent
                                   },
                 TestRunner = new AxeTestRunner(),
                 ToolOptions = new AxeToolOptions
                               {
                                   RunOnly = new AxeExecutionConstraint
                                             {
                                                 Type = "tag",
                                                 Values = new[] { tags.First() }
                                             },
                                   FrameWaitTime = 2000,
                                   IFrames = includeIFrames.Value
                               },
                 Violations = new[] { rule },
             };
    }

    private AxeHtmlElementResult CreateRuleNode (string ruleId, string impact, string[] xPaths, string[] targets)
    {
      return new AxeHtmlElementResult
             {
                 Html = "Gustav",
                 Any = new[]
                       {
                           new AxeRuleCheckResult
                           {
                               ID = ruleId,
                               Impact = impact,
                               Message = "TestMessage"
                           }
                       },
                 All = new AxeRuleCheckResult[0],
                 None = new AxeRuleCheckResult[0],
                 XPaths = xPaths ?? new[] { "A", "B" },
                 Target = targets ?? new[] { "C", "D" },
             };
    }
  }
}