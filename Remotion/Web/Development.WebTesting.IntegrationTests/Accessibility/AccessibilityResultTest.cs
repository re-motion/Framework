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
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Accessibility
{
  [TestFixture]
  public class AccessibilityResultTest : IntegrationTest
  {
    [Test]
    public void IgnoreByCssSelector ()
    {
      Start();
      var analyzer = Helper.CreateAccessibilityAnalyzer();
      var result = analyzer.Analyze();

      var violations = result.Violations.IgnoreByCssSelector ("html");

      Assert.That (result.Violations.Where (x => x.TargetPath.Select (p => p.CssSelector).Contains ("html")), Is.Not.Empty);
      Assert.That (violations.Where (x => x.TargetPath.Select (p => p.CssSelector).Contains ("html")), Is.Empty);
    }

    [Test]
    public void IgnoreByRuleImpact ()
    {
      Start();
      var analyzer = Helper.CreateAccessibilityAnalyzer();
      var result = analyzer.Analyze();

      var violations = result.Violations.IgnoreByImpact (AccessibilityTestImpact.Serious);

      Assert.That (result.Violations.Where (x => x.Rule.Impact == AccessibilityTestImpact.Serious), Is.Not.Empty);
      Assert.That (violations.Where (x => x.Rule.Impact == AccessibilityTestImpact.Serious), Is.Empty);
    }

    [Test]
    public void IgnoreByRuleID ()
    {
      Start();
      var analyzer = Helper.CreateAccessibilityAnalyzer();
      var result = analyzer.Analyze();

      var filter = new AccessibilityResultFilter();
      filter.IgnoreRuleID.Add (AccessibilityRuleID.HtmlHasLang);
      var violations = result.Violations.Filter (filter);

      Assert.That (result.Violations.Where (x => x.Rule.ID == AccessibilityRuleID.HtmlHasLang), Is.Not.Empty);
      Assert.That (violations.Where (x => x.Rule.ID == AccessibilityRuleID.HtmlHasLang), Is.Empty);
    }

    [Test]
    public void IgnoreBySuccessCriteria ()
    {
      Start();
      var analyzer = Helper.CreateAccessibilityAnalyzer();
      var result = analyzer.Analyze();

      var violations = result.Violations.IgnoreBySuccessCriteria (AccessibilityTestSuccessCriteria.Wcag_4_1_2);

      Assert.That (result.Violations.Where (x => x.Rule.SuccessCriteria.Contains (AccessibilityTestSuccessCriteria.Wcag_4_1_2)), Is.Not.Empty);
      Assert.That (violations.Where (x => x.Rule.SuccessCriteria.Contains (AccessibilityTestSuccessCriteria.Wcag_4_1_2)), Is.Empty);
    }

    [Test]
    public void IgnoreAndIncludeMultiple ()
    {
      Start();
      var analyzer = Helper.CreateAccessibilityAnalyzer();
      var result = analyzer.Analyze();

      var filter = new AccessibilityResultFilter();
      filter.IgnoreSuccessCriteria.Add (AccessibilityTestSuccessCriteria.Wcag_4_1_2);
      filter.IncludeImpact = AccessibilityTestImpact.Serious;

      var violations = result.Violations.Filter (filter);

      Assert.That (result.Violations.Where (x => x.Rule.SuccessCriteria.Contains (AccessibilityTestSuccessCriteria.Wcag_4_1_2)), Is.Not.Empty);
      Assert.That (result.Violations.Where (x => x.Rule.Impact == AccessibilityTestImpact.Critical), Is.Not.Empty);
      Assert.That (
          violations.Where (
              x => x.Rule.Impact == AccessibilityTestImpact.Critical ||
                   x.Rule.SuccessCriteria.Contains (AccessibilityTestSuccessCriteria.Wcag_4_1_2)),
          Is.Empty);
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject> ("Accessibility/IncludeExclude.html");
    }
  }
}