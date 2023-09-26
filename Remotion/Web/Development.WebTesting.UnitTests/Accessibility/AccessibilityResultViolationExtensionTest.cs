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
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.Accessibility;

namespace Remotion.Web.Development.WebTesting.UnitTests.Accessibility
{
  [TestFixture]
  public class AccessibilityResultViolationExtensionTest
  {
    private AccessibilityRuleResult _defaultViolation;

    [SetUp]
    public void SetUp ()
    {
      _defaultViolation = new AccessibilityRuleResult(
          new AccessibilityRule(
              AccessibilityRuleID.AriaAllowedAttr,
              "Description",
              AccessibilityTestImpact.Moderate,
              new[] { AccessibilityTestSuccessCriteria.Wcag_4_1_2 }),
          new[] { new AccessibilityTestTarget("xPath", "CSS") },
          "Html",
          new[] { new AccessibilityRequirement(AccessibilityRequirementID.MetaViewport, "Message", AccessibilityTestImpact.Moderate) },
          new AccessibilityRequirement[0],
          new AccessibilityRequirement[0]);
    }

    [Test]
    public void Filter_ByCssSelectorAndImpact ()
    {
      var filter = new AccessibilityResultFilter
                   {
                       IgnoreCssSelectors = { "test" },
                       IncludeImpact = AccessibilityTestImpact.Moderate
                   };
      var violation = CreateViolation(impact: AccessibilityTestImpact.Moderate, targets: new[] { CreateTargetWithXPathAndCss("test") });
      var violations = new[] { violation, _defaultViolation };

      var filteredViolations = violations.Filter(filter);

      Assert.That(filteredViolations, Is.EquivalentTo(new[] { _defaultViolation }));
    }

    [Test]
    public void Filter_ByMultipleRuleIDs ()
    {
      var violation1 = CreateViolation(AccessibilityRuleID.AreaAlt);
      var violation2 = CreateViolation(AccessibilityRuleID.FrameTitleUnique);
      var violations = new[] { violation1, violation2, _defaultViolation };

      var filter = new AccessibilityResultFilter();
      filter.IgnoreRuleID.Add(AccessibilityRuleID.AreaAlt);
      filter.IgnoreRuleID.Add(AccessibilityRuleID.FrameTitleUnique);
      var filteredViolations = violations.Filter(filter);

      Assert.That(filteredViolations, Is.EquivalentTo(new[] { _defaultViolation }));
    }

    [Test]
    public void Filter_ByRuleIDAndSuccessCriteria ()
    {
      var filter = new AccessibilityResultFilter();
      filter.IgnoreSuccessCriteria.Add(AccessibilityTestSuccessCriteria.Wcag_1_2_4);
      filter.IgnoreRuleID.Add(AccessibilityRuleID.AreaAlt);

      var violation = CreateViolation(AccessibilityRuleID.AreaAlt, successCriteria: new[] { AccessibilityTestSuccessCriteria.Wcag_1_2_4 });
      var violations = new[] { violation, _defaultViolation };
      var filteredViolations = violations.Filter(filter);

      Assert.That(filteredViolations, Is.EquivalentTo(new[] { _defaultViolation }));
    }

    [Test]
    public void Ignore_ByCheckID ()
    {
      var violation = CreateViolation(checkIDs: new[] { AccessibilityRequirementID.TdHasHeader });
      var violations = new[] { violation, _defaultViolation };

      var filteredViolations = violations.IgnoreByCheckID(AccessibilityRequirementID.TdHasHeader);

      Assert.That(filteredViolations, Is.EquivalentTo(new[] { _defaultViolation }));
    }

    [Test]
    public void Ignore_ByCssSelector ()
    {
      var violation = CreateViolation(targets: new[] { CreateTargetWithXPathAndCss("test1") });
      var violations = new[] { violation, _defaultViolation };

      var filteredViolations = violations.IgnoreByCssSelector("test1");

      Assert.That(filteredViolations, Is.EquivalentTo(new[] { _defaultViolation }));
    }

    [Test]
    public void Ignore_ByMultipleCheckIDs ()
    {
      var violation = CreateViolation(checkIDs: new[] { AccessibilityRequirementID.ThHasDataCells, AccessibilityRequirementID.DuplicateId });
      var violations = new[] { violation, _defaultViolation };

      var filteredViolations = violations.IgnoreByCheckID(AccessibilityRequirementID.ThHasDataCells, AccessibilityRequirementID.DuplicateId);

      Assert.That(filteredViolations, Is.EquivalentTo(new[] { _defaultViolation }));
    }

    [Test]
    public void Ignore_ByRuleIDAndCssSelector ()
    {
      var violation = CreateViolation(AccessibilityRuleID.AreaAlt, targets: new[] { CreateTargetWithXPathAndCss("testFrame") });
      var violations = new[] { violation, _defaultViolation };

      var filteredViolations = violations.IgnoreByRuleIDAndXPath(AccessibilityRuleID.AreaAlt, "testFrame");

      Assert.That(filteredViolations, Is.EquivalentTo(new[] { _defaultViolation }));
    }

    [Test]
    public void Ignore_ByRuleIDAndMultipleCssSelectors ()
    {
      var violation = CreateViolation(AccessibilityRuleID.AreaAlt, targets: new[] { CreateTargetWithXPathAndCss("testFrame"), CreateTargetWithXPathAndCss("test") });
      var violations = new[] { violation, _defaultViolation };

      var filteredViolations = violations.IgnoreByRuleIDAndXPath(AccessibilityRuleID.AreaAlt, "testFrame", "test");

      Assert.That(filteredViolations, Is.EquivalentTo(new[] { _defaultViolation }));
    }

    [Test]
    public void Ignore_ByRuleIDXPath_OnlyOnePathMatching_DoesNotIgnore ()
    {
      var violation = CreateViolation(
          AccessibilityRuleID.AreaAlt,
          targets: new[] { CreateTargetWithXPathAndCss("testFrame1"), CreateTargetWithXPathAndCss("testFrame2") });
      var violations = new[] { violation, _defaultViolation };

      var filteredViolations = violations.IgnoreByRuleIDAndXPath(AccessibilityRuleID.AreaAlt, "testFrame1");

      Assert.That(filteredViolations, Is.EquivalentTo(new[] { violation, _defaultViolation }));
    }

    [Test]
    public void Ignore_BySuccessCriteria ()
    {
      var violation = CreateViolation(successCriteria: new[] { AccessibilityTestSuccessCriteria.Wcag_1_2_9 });
      var violations = new[] { violation, _defaultViolation };

      var filteredViolations = violations.IgnoreBySuccessCriteria(AccessibilityTestSuccessCriteria.Wcag_1_2_9);

      Assert.That(filteredViolations, Is.EquivalentTo(new[] { _defaultViolation }));
    }

    [Test]
    public void Ignore_RuleByRuleID ()
    {
      var violation = CreateViolation(AccessibilityRuleID.AreaAlt);
      var violations = new[] { violation, _defaultViolation };

      var filteredViolations = violations.IgnoreByRuleID(AccessibilityRuleID.AreaAlt);

      Assert.That(filteredViolations, Is.EquivalentTo(new[] { _defaultViolation }));
    }

    private AccessibilityRuleResult CreateViolation (
        AccessibilityRuleID? ruleID = null,
        AccessibilityTestImpact? impact = null,
        AccessibilityRequirementID[] checkIDs = null,
        AccessibilityTestSuccessCriteria[] successCriteria = null,
        AccessibilityTestTarget[] targets = null)
    {
      return new AccessibilityRuleResult(
          new AccessibilityRule(
              ruleID ?? AccessibilityRuleID.AccessKeys,
              "test",
              impact ?? AccessibilityTestImpact.Minor,
              successCriteria ?? new[] { AccessibilityTestSuccessCriteria.Wcag_1_2_6 }
              ),
          targets ?? new[] { new AccessibilityTestTarget("A", "B") },
          "test",
          new List<AccessibilityRequirement>(),
          checkIDs == null ? CreateRequirements(new[] { AccessibilityRequirementID.AriaLabelledBy }).ToArray() : CreateRequirements(checkIDs).ToArray(),
          new List<AccessibilityRequirement>()
          );
    }

    private IEnumerable<AccessibilityRequirement> CreateRequirements (AccessibilityRequirementID[] checkIDs)
    {
      foreach (var accessibilityCheckID in checkIDs)
        yield return new AccessibilityRequirement(accessibilityCheckID, "message", AccessibilityTestImpact.Minor);
    }

    private AccessibilityTestTarget CreateTargetWithXPathAndCss (string xpathAndCss)
    {
      return new AccessibilityTestTarget(xpathAndCss, xpathAndCss);
    }
  }
}
