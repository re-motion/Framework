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
using System.Linq;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.Accessibility;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects.Selectors;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Accessibility
{
  /// <remarks>
  /// If these tests fail after an upgrade of aXe core, new tags might have been added to the rules.
  /// This information can be found at <see href="https://github.com/dequelabs/axe-core/blob/master/doc/rule-descriptions.md"/>.
  /// </remarks>
  [TestFixture]
  public class AccessibilityAnalyzerTest : IntegrationTest
  {
    [Test]
    public void Analyze_PageObject ()
    {
      var home = Start<HtmlPageObject>("Accessibility/FormElementWithoutLabel.html");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = home.Analyze(analyzer);
      var violation = result.Violations.Single();

      Assert.That(violation.Rule.ID, Is.EqualTo(AccessibilityRuleID.Label));
    }

    [Test]
    public void Analyze_IgnoreRule ()
    {
      Start<HtmlPageObject>("Accessibility/FormElementWithoutLabelAndImageWithoutAltAndWithoutDocumentTitle.html");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      analyzer.IgnoreRule(AccessibilityRuleID.Label);
      var result = analyzer.Analyze();

      Assert.That(result.Violations.Count, Is.EqualTo(2));
      Assert.That(result.Violations.First().Rule.ID, Is.EqualTo(AccessibilityRuleID.DocumentTitle));
      Assert.That(result.Violations.ElementAt(1).Rule.ID, Is.EqualTo(AccessibilityRuleID.ImageAlt));
    }

    [Test]
    public void Analyze_IgnoreMultipleRules ()
    {
      Start<HtmlPageObject>("Accessibility/FormElementWithoutLabelAndImageWithoutAltAndWithoutDocumentTitle.html");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      analyzer.IgnoreRule(AccessibilityRuleID.Label);
      analyzer.IgnoreRule(AccessibilityRuleID.ImageAlt);
      var result = analyzer.Analyze();

      Assert.That(result.Violations.Count, Is.EqualTo(1));
      Assert.That(result.Violations.First().Rule.ID, Is.EqualTo(AccessibilityRuleID.DocumentTitle));
    }

    [Test]
    public void Analyze_IgnoreCssSelector ()
    {
      Start<HtmlPageObject>("Accessibility/MultipleFormElementsWithoutLabels.html");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      analyzer.IgnoreCssSelector("#first-input");
      var result = analyzer.Analyze();

      Assert.That(result.Violations.Count, Is.EqualTo(2));
      Assert.That(result.Violations.First().TargetPath.Count, Is.EqualTo(1));
      Assert.That(result.Violations.First().TargetPath[0].XPath, Is.EqualTo("/input[@id='second-input']"));
      Assert.That(result.Violations.ElementAt(1).TargetPath.Count, Is.EqualTo(1));
      Assert.That(result.Violations.ElementAt(1).TargetPath[0].XPath, Is.EqualTo("/input[@id='third-input']"));
    }

    [Test]
    public void Analyze_IgnoreMultipleCssSelectors ()
    {
      Start<HtmlPageObject>("Accessibility/MultipleFormElementsWithoutLabels.html");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      analyzer.IgnoreCssSelector("#first-input");
      analyzer.IgnoreCssSelector("#second-input");
      var result = analyzer.Analyze();

      Assert.That(result.Violations.Count, Is.EqualTo(1));
      Assert.That(result.Violations.First().TargetPath.Count, Is.EqualTo(1));
      Assert.That(result.Violations.First().TargetPath[0].XPath, Is.EqualTo("/input[@id='third-input']"));
    }

    [Test]
    public void Analyze_IgnoreControlObject ()
    {
      var home = Start<HtmlPageObject>("Accessibility/MultipleFormElementsWithoutLabels.html");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var textBoxWithViolation = GetTextBoxControlObject(home, "first-input");
      analyzer.IgnoreControlObject(textBoxWithViolation);
      var result = analyzer.Analyze();

      Assert.That(result.Violations.Count, Is.EqualTo(2));
      Assert.That(result.Violations.First().TargetPath.Count, Is.EqualTo(1));
      Assert.That(result.Violations.First().TargetPath[0].XPath, Is.EqualTo("/input[@id='second-input']"));
      Assert.That(result.Violations.ElementAt(1).TargetPath.Count, Is.EqualTo(1));
      Assert.That(result.Violations.ElementAt(1).TargetPath[0].XPath, Is.EqualTo("/input[@id='third-input']"));
    }

    private TextBoxControlObject GetTextBoxControlObject (PageObject page, string htmlID)
    {
      var textBoxSelector = new TextBoxSelector();
      var textBoxSelectionCommand = new HtmlIDControlSelectionCommand<TextBoxControlObject>(textBoxSelector, htmlID);
      return page.GetControl(textBoxSelectionCommand);
    }
  }
}
