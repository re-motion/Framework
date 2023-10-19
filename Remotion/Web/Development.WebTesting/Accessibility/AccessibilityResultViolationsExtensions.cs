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
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration;

namespace Remotion.Web.Development.WebTesting.Accessibility
{
  /// <summary>
  /// Provides extension methods to filter <see cref="AccessibilityResult.Violations"/>.
  /// </summary>
  public static class AccessibilityResultViolationsExtensions
  {
    /// <summary>
    /// Removes elements with matching CSS selector from the given violations.
    /// </summary>
    public static IEnumerable<AccessibilityRuleResult> IgnoreByCssSelector (
        [NotNull] this IReadOnlyCollection<AccessibilityRuleResult> violations,
        [NotNull] params string[] cssSelector)
    {
      ArgumentUtility.CheckNotNull("violations", violations);
      ArgumentUtility.CheckNotNullOrItemsNull("cssSelector", cssSelector);

      return violations.Where(x => !ArrayEquals(x.TargetPath.Select(p => p.CssSelector).ToArray(), cssSelector));
    }

    /// <summary>
    /// Removes elements where the rule impact matches the given impact.
    /// </summary>
    public static IEnumerable<AccessibilityRuleResult> IgnoreByImpact (
        [NotNull] this IReadOnlyCollection<AccessibilityRuleResult> violations,
        AccessibilityTestImpact ruleImpact)
    {
      ArgumentUtility.CheckNotNullOrItemsNull("violations", violations);

      return violations.Where(x => ruleImpact != x.Rule.Impact);
    }

    /// <summary>
    /// Removes elements where the rule ID matches the given rule ID.
    /// </summary>
    public static IEnumerable<AccessibilityRuleResult> IgnoreByRuleID (
        [NotNull] this IReadOnlyCollection<AccessibilityRuleResult> violations,
        AccessibilityRuleID ruleID)
    {
      ArgumentUtility.CheckNotNullOrItemsNull("violations", violations);

      return violations.Where(x => ruleID != x.Rule.ID);
    }

    /// <summary>
    /// Removes elements where the success criteria tag matches the given success criteria.
    /// </summary>
    public static IEnumerable<AccessibilityRuleResult> IgnoreBySuccessCriteria (
        [NotNull] this IReadOnlyCollection<AccessibilityRuleResult> violations,
        AccessibilityTestSuccessCriteria successCriteria)
    {
      ArgumentUtility.CheckNotNullOrItemsNull("violations", violations);

      return violations.Where(v => v.Rule.SuccessCriteria.All(s => successCriteria != s));
    }

    /// <summary>
    /// Removes elements with checks that match the given checkIDs.
    /// </summary>
    public static IEnumerable<AccessibilityRuleResult> IgnoreByCheckID (
        [NotNull] this IReadOnlyCollection<AccessibilityRuleResult> violations,
        [NotNull] params AccessibilityRequirementID[] checkIDs)
    {
      ArgumentUtility.CheckNotNullOrItemsNull("violations", violations);
      ArgumentUtility.CheckNotNullOrEmpty("checkIDs", checkIDs);

      return violations.Where(
          x => checkIDs.Except(x.Any.Select(a => a.ID)).Any()
               && checkIDs.Except(x.All.Select(a => a.ID)).Any()
               && checkIDs.Except(x.None.Select(a => a.ID)).Any());
    }

    /// <summary>
    /// Filters violations by filter options.
    /// </summary>
    public static IEnumerable<AccessibilityRuleResult> Filter (
        [NotNull] this IReadOnlyCollection<AccessibilityRuleResult> violations,
        [NotNull] AccessibilityResultFilter filter)
    {
      ArgumentUtility.CheckNotNullOrItemsNull("violations", violations);
      ArgumentUtility.CheckNotNull("filter", filter);

      return violations.Where(
          v => !(v.TargetPath.Select(p => p.CssSelector).Any(s => filter.IgnoreCssSelectors.Contains(s))
                 || filter.IgnoreRuleID.Contains(v.Rule.ID)
                 || v.Rule.SuccessCriteria.All(s => filter.IgnoreSuccessCriteria.Contains(s))
                 || filter.IncludeImpact != null && v.Rule.Impact != filter.IncludeImpact.Value)
          );
    }

    /// <summary>
    /// Removes elements with the given rule ID and xpath.
    /// </summary>
    public static IReadOnlyCollection<AccessibilityRuleResult> IgnoreByRuleIDAndXPath (
        [NotNull] this IReadOnlyCollection<AccessibilityRuleResult> violations,
        AccessibilityRuleID ruleID,
        params string[] xPath)
    {
      ArgumentUtility.CheckNotNullOrItemsNull("violations", violations);
      ArgumentUtility.CheckNotNullOrEmpty("xPath", xPath);

      return violations.Where(v => v.Rule.ID != ruleID || !ArrayEquals(v.TargetPath.Select(p => p.XPath).ToArray(), xPath)).ToArray();
    }

    /// <summary>
    /// Removes elements where known workarounds cause violations.
    /// </summary>
    public static IEnumerable<AccessibilityRuleResult> IgnoreKnownIssues (
        [NotNull] this IEnumerable<AccessibilityRuleResult> violations)
    {
      //RM-8997 This will be removed again when the accessibility problems have been fixed
      var cssRegex = new Regex(@"(\.bocListTableBlock|\.hasMenuBlock|\.hasNavigator) > \.screenReaderText\[aria-label="".+""]\[aria-hidden=""true""]");

      var requiredChildrenRegex = new Regex(@"/div[@id='.*_Boc_ListMenu']/table");
      var xpath = new Regex(@"/a\[@id='.*_MenuItem_/d+_Command'\]");
      violations = violations.Where(r => !(r.Rule.ID == AccessibilityRuleID.AriaRequiredParent)&& cssRegex.IsMatch(r.TargetPath.Last().XPath));
      return violations;
    }

    private static bool ArrayEquals (string[] arrayA, string[] arrayB)
    {
      if (arrayA.Length != arrayB.Length)
        return false;

      for (var i = 0; i < arrayA.Length; i++)
        if (arrayA[i] != arrayB[i])
          return false;

      return true;
    }
  }
}
