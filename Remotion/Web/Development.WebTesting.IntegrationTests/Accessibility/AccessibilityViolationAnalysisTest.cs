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
using System.Collections.Generic;
using System.Linq;
using log4net;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using OpenQA.Selenium;
using Remotion.Web.Development.WebTesting.Accessibility;
using Remotion.Web.Development.WebTesting.Accessibility.Implementation;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Accessibility
{
  [TestFixture]
  public class AccessibilityViolationAnalysisTest : IntegrationTest
  {
    [Test]
    public void AccessibilityFormElementWithoutLabel ()
    {
      Start<WxePageObject>("Accessibility/FormElementWithoutLabel.html");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = analyzer.Analyze();
      var violation = result.Violations.Single();

      Assert.That(violation.All, Has.Length.EqualTo(0));
      Assert.That(
          violation.Any.Select(x => x.ID),
          Is.EquivalentTo(
              new[]
              {
                  AccessibilityRequirementID.AriaLabel,
                  AccessibilityRequirementID.AriaLabelledBy,
                  AccessibilityRequirementID.ExplicitLabel,
                  AccessibilityRequirementID.ImplicitLabel,
                  AccessibilityRequirementID.NonEmptyPlaceholder,
                  AccessibilityRequirementID.NonEmptyTitle,
                  AccessibilityRequirementID.PresentationalRole
              }));
      Assert.That(violation.None, Has.Length.EqualTo(0));
      Assert.That(violation.TargetPath.Single().CssSelector, Is.EqualTo("html > body > #labels > #input"));
      Assert.That(violation.Html, Contains.Substring("id=\"input\""));
      Assert.That(violation.Html, Contains.Substring("name=\"test\""));
      Assert.That(violation.Html, Contains.Substring("type=\"text\">"));
      Assert.That(violation.Rule.ID, Is.EqualTo(AccessibilityRuleID.Label));
      Assert.That(violation.Rule.Description, Is.EqualTo("Ensures every form element has a label"));
      Assert.That(violation.Rule.Impact, Is.EqualTo(AccessibilityTestImpact.Critical));
      Assert.That(violation.Rule.SuccessCriteria, Has.Length.EqualTo(2));
      Assert.That(violation.Rule.SuccessCriteria, Contains.Item(AccessibilityTestSuccessCriteria.Wcag_4_1_2));
      Assert.That(violation.Rule.SuccessCriteria, Contains.Item(AccessibilityTestSuccessCriteria.Section508_22_n));
      Assert.That(violation.TargetPath.Single().XPath, Is.EqualTo("/input[@id='input']"));
    }

    [Test]
    public void AccessibilityImageWithoutAlt ()
    {
      Start<HtmlPageObject>("Accessibility/ImageWithoutAlt.html");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = analyzer.Analyze();
      var violation = result.Violations.Single();

      Assert.That(violation.All, Has.Length.EqualTo(0));
      Assert.That(
          violation.Any.Select(x => x.ID),
          Is.EquivalentTo(
              new[]
              {
                  AccessibilityRequirementID.HasAlt,
                  AccessibilityRequirementID.AriaLabel,
                  AccessibilityRequirementID.AriaLabelledBy,
                  AccessibilityRequirementID.NonEmptyTitle,
                  AccessibilityRequirementID.PresentationalRole,
              }));
      Assert.That(violation.None, Has.Length.EqualTo(0));
      Assert.That(violation.TargetPath.Single().CssSelector, Is.EqualTo("html > body > #testImage"));
      Assert.That(violation.Html, Is.EqualTo("<img id=\"testImage\" src=\"../Image/SampleIcon.gif\">"));
      Assert.That(violation.Rule.ID, Is.EqualTo(AccessibilityRuleID.ImageAlt));
      Assert.That(violation.Rule.Impact, Is.EqualTo(AccessibilityTestImpact.Critical));
      Assert.That(violation.Rule.Description, Is.EqualTo("Ensures <img> elements have alternate text or a role of none or presentation"));
      Assert.That(violation.Rule.Impact, Is.EqualTo(AccessibilityTestImpact.Critical));
      Assert.That(violation.Rule.SuccessCriteria, Has.Length.EqualTo(2));
      Assert.That(violation.Rule.SuccessCriteria, Contains.Item(AccessibilityTestSuccessCriteria.Wcag_1_1_1));
      Assert.That(violation.Rule.SuccessCriteria, Contains.Item(AccessibilityTestSuccessCriteria.Section508_22_a));
      Assert.That(violation.TargetPath.Single().XPath, Is.EqualTo("/img[@id='testImage']"));
    }

    [Test]
    public void AccessibilityButtonWithoutInnerHTML ()
    {
      Start<HtmlPageObject>("Accessibility/ButtonWithoutInnerHTML.html");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = analyzer.Analyze();
      var violation = result.Violations.Single();

      Assert.That(violation.All, Is.Empty);
      Assert.That(
          violation.Any.Select(x => x.ID),
          Is.EquivalentTo(
              new[]
              {
                  AccessibilityRequirementID.ButtonHasVisibleText,
                  AccessibilityRequirementID.AriaLabel,
                  AccessibilityRequirementID.AriaLabelledBy,
                  AccessibilityRequirementID.PresentationalRole,
                  AccessibilityRequirementID.NonEmptyTitle,
              }));
      Assert.That(violation.None, Has.Length.EqualTo(0));
      Assert.That(violation.TargetPath.Single().CssSelector, Is.EqualTo("html > body > #coolButton"));
      Assert.That(violation.Html, Is.EqualTo("<button id=\"coolButton\"></button>"));
      Assert.That(violation.Rule.ID, Is.EqualTo(AccessibilityRuleID.ButtonName));
      Assert.That(violation.Rule.Impact, Is.EqualTo(AccessibilityTestImpact.Critical));
      Assert.That(violation.Rule.Description, Is.EqualTo("Ensures buttons have discernible text"));
      Assert.That(violation.Rule.Impact, Is.EqualTo(AccessibilityTestImpact.Critical));
      Assert.That(violation.Rule.SuccessCriteria, Has.Length.EqualTo(2));
      Assert.That(violation.Rule.SuccessCriteria, Contains.Item(AccessibilityTestSuccessCriteria.Wcag_4_1_2));
      Assert.That(violation.Rule.SuccessCriteria, Contains.Item(AccessibilityTestSuccessCriteria.Section508_22_a));
      Assert.That(violation.TargetPath.Single().XPath, Is.EqualTo("/button[@id='coolButton']"));
    }

    [Test]
    public void AccessibilityContrast ()
    {
      Start<HtmlPageObject>("Accessibility/Contrast.html");
      var config = new AccessibilityConfiguration(AccessibilityConformanceLevel.Wcag20_ConformanceLevelDoubleA);
      var analyzer = CreateAnalyzer(config);

      var result = analyzer.Analyze();
      var violation = result.Violations.Single();

      Assert.That(violation.All, Has.Length.EqualTo(0));
      Assert.That(violation.Any, Has.Length.EqualTo(1));
      Assert.That(violation.Any.Where(x => x.ID == AccessibilityRequirementID.ColorContrast).ToArray(), Has.Length.EqualTo(1));
      Assert.That(violation.None, Has.Length.EqualTo(0));
      Assert.That(violation.TargetPath.Single().CssSelector, Is.EqualTo("html > body > h1"));
      Assert.That(violation.Html, Is.EqualTo("<h1 style=\"color: #000080\">Hello World!</h1>"));
      Assert.That(violation.Rule.ID, Is.EqualTo(AccessibilityRuleID.ColorContrast));
      Assert.That(violation.Rule.Impact, Is.EqualTo(AccessibilityTestImpact.Serious));
      Assert.That(violation.Rule.Description, Is.EqualTo("Ensures the contrast between foreground and background colors meets WCAG 2 AA minimum contrast ratio thresholds"));
      Assert.That(violation.Rule.Impact, Is.EqualTo(AccessibilityTestImpact.Serious));
      Assert.That(violation.Rule.SuccessCriteria, Has.Length.EqualTo(1));
      Assert.That(violation.Rule.SuccessCriteria, Contains.Item(AccessibilityTestSuccessCriteria.Wcag_1_4_3));
      Assert.That(violation.TargetPath.Single().XPath, Is.EqualTo("/html/body/h1"));
    }

    [Test]
    public void AccessibilityContrastWCAG2A_Pass ()
    {
      Start<HtmlPageObject>("Accessibility/Contrast.html");
      var config = new AccessibilityConfiguration(AccessibilityConformanceLevel.Wcag20_ConformanceLevelA);
      var analyzer = CreateAnalyzer(config);

      var result = analyzer.Analyze();

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void ConformanceLevel_WCAG_20A ()
    {
      Start<HtmlPageObject>("Accessibility/ViolationsWithDifferentConformanceLevels.html");

      var config = new AccessibilityConfiguration(AccessibilityConformanceLevel.Wcag20_ConformanceLevelA);
      var analyzer = CreateAnalyzer(config);

      var result = analyzer.Analyze();

      Assert.That(result.ConformanceLevel, Is.EqualTo(AccessibilityConformanceLevel.Wcag20_ConformanceLevelA));
      Assert.That(result.Violations.Single().Rule.ID, Is.EqualTo(AccessibilityRuleID.ImageAlt));
    }

    [Test]
    public void ConformanceLevel_WCAG_20AA ()
    {
      Start<HtmlPageObject>("Accessibility/ViolationsWithDifferentConformanceLevels.html");

      var config = new AccessibilityConfiguration(AccessibilityConformanceLevel.Wcag20_ConformanceLevelDoubleA);
      var analyzer = CreateAnalyzer(config);

      var result = analyzer.Analyze();

      var expectedRules = new[] { AccessibilityRuleID.ColorContrast, AccessibilityRuleID.ImageAlt };

      Assert.That(result.ConformanceLevel, Is.EqualTo(AccessibilityConformanceLevel.Wcag20_ConformanceLevelDoubleA));
      Assert.That(
          result.Violations.Select(v => v.Rule.ID),
          Is.EquivalentTo(expectedRules));
    }

    [Test]
    public void ConformanceLevel_WCAG_21A ()
    {
      Start<HtmlPageObject>("Accessibility/ViolationsWithDifferentConformanceLevels.html");

      var config = new AccessibilityConfiguration(AccessibilityConformanceLevel.Wcag21_ConformanceLevelA);
      var analyzer = CreateAnalyzer(config);

      var result = analyzer.Analyze();

      Assert.That(result.ConformanceLevel, Is.EqualTo(AccessibilityConformanceLevel.Wcag21_ConformanceLevelA));
      Assert.That(
          result.Violations.Select(v => v.Rule.ID),
          Is.EquivalentTo(new[] { AccessibilityRuleID.ImageAlt, AccessibilityRuleID.LabelContentNameMismatch }));
    }

    [Test]
    public void ConformanceLevel_WCAG_21AA ()
    {
      Start<HtmlPageObject>("Accessibility/ViolationsWithDifferentConformanceLevels.html");

      var config = new AccessibilityConfiguration(AccessibilityConformanceLevel.Wcag21_ConformanceLevelDoubleA);
      var analyzer = CreateAnalyzer(config);

      var result = analyzer.Analyze();

      var expectedRules = new List<AccessibilityRuleID>
                          {
                              AccessibilityRuleID.ColorContrast,
                              AccessibilityRuleID.ImageAlt,
                              AccessibilityRuleID.LabelContentNameMismatch,
                              AccessibilityRuleID.AutocompleteValid
                          };

      Assert.That(result.ConformanceLevel, Is.EqualTo(AccessibilityConformanceLevel.Wcag21_ConformanceLevelDoubleA));
      Assert.That(result.Violations.Select(v => v.Rule.ID), Is.EquivalentTo(expectedRules));
    }

    [Test]
    public void ConformanceLevel_Section508 ()
    {
      Start<HtmlPageObject>("Accessibility/ViolationsWithDifferentConformanceLevels.html");

      var config = new AccessibilityConfiguration(AccessibilityConformanceLevel.Section508);
      var analyzer = CreateAnalyzer(config);

      var result = analyzer.Analyze();

      var expectedRules = new List<AccessibilityRuleID>
                          {
                              AccessibilityRuleID.ImageAlt
                          };

      Assert.That(result.ConformanceLevel, Is.EqualTo(AccessibilityConformanceLevel.Section508));
      Assert.That(result.Violations.Select(v => v.Rule.ID), Is.EquivalentTo(expectedRules));
    }

    private AccessibilityAnalyzer CreateAnalyzer (IAccessibilityConfiguration config)
    {
      return AccessibilityAnalyzer.CreateForWebDriver(
          (IWebDriver)Helper.MainBrowserSession.Driver.Native,
          new AxeResultParser(),
          config,
          new AxeSourceProvider(),
          new AccessibilityResultMapper(),
          NullLogger.Instance);
    }
  }
}
