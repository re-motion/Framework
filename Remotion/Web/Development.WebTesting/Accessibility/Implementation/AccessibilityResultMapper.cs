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
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Accessibility.AxeJsonResultDtos;

namespace Remotion.Web.Development.WebTesting.Accessibility.Implementation
{
  /// Default implementation of <see cref="IAccessibilityResultMapper" />.
  public class AccessibilityResultMapper : IAccessibilityResultMapper
  {
    private readonly HashSet<string> _wellKnownSuccessCriteriaToIgnore =
        new HashSet<string>
        {
            "cat.keyboard",
            "cat.text-alternatives",
            "cat.sensory-and-visual-cues",
            "cat.time",
            "cat.tables",
            "cat.semantics",
            "cat.language",
            "cat.parsing",
            "cat.aria",
            "cat.name-role-value",
            "cat.time-and-media",
            "cat.forms",
            "cat.color",
            "cat.structure",
            "experimental",
            "review-item",
            "best-practice",
            "ACT",
            "TTv5",
            "EN-301-549"
        };

    private readonly Regex[] _wellKnownSuccessCriteriaMatcherToIgnore =
        {
            new Regex(@"EN-\d\.\d\.\d\.\d"),
            new Regex(@"TT\d+\.[a-z]")
        };

    public AccessibilityResultMapper ()
    {
    }

    /// <inheritdoc />
    public AccessibilityResult Map (AxeResult axeResult)
    {
      ArgumentUtility.CheckNotNull("axeResult", axeResult);

      return new AccessibilityResult(
          axeVersion: axeResult.TestEngine.Version,
          url: axeResult.Url,
          timestamp: ParseTimestamp(axeResult.Timestamp),
          windowWidth: axeResult.TestEnvironment.WindowWidth,
          windowHeight: axeResult.TestEnvironment.WindowHeight,
          orientationAngle: axeResult.TestEnvironment.OrientationAngle,
          orientationType: axeResult.TestEnvironment.OrientationType,
          userAgent: axeResult.TestEnvironment.UserAgent,
          includeIFrames: axeResult.ToolOptions.IFrames,
          conformanceLevel: ParseConformanceLevel(axeResult.ToolOptions.RunOnly.Values),
          violations: ParseViolations(axeResult).ToList());
    }

    private DateTime ParseTimestamp (string timestampAsString)
    {
      return DateTime.ParseExact(
          timestampAsString,
          "yyyy-MM-ddTHH:mm:ss.fffZ",
          CultureInfo.CreateSpecificCulture("en-US"),
          DateTimeStyles.AssumeLocal);
    }

    private AccessibilityConformanceLevel ParseConformanceLevel (string[] tags)
    {
      var conformanceLevels = tags.Where(AccessibilityConformanceLevelConverter.IsValid);
      var highestConformanceLevel = conformanceLevels.OrderByDescending(s => s.Length).First();
      return AccessibilityConformanceLevelConverter.ConvertToEnum(highestConformanceLevel);
    }

    private IEnumerable<AccessibilityRuleResult> ParseViolations (AxeResult deserializedResult)
    {
      foreach (var axeRule in deserializedResult.Violations)
      {
        var accessibilityRule = GetAccessibilityRule(axeRule);
        foreach (var node in axeRule.Nodes)
        {
          yield return GetViolation(node, accessibilityRule);
        }
      }
    }

    private AccessibilityRule GetAccessibilityRule (AxeRuleResult ruleResult)
    {
      return new AccessibilityRule(
          AccessibilityRuleIDConverter.ConvertToEnum(ruleResult.ID),
          ruleResult.Description,
          ToEnum<AccessibilityTestImpact>(ruleResult.Impact),
          GetSuccessCriteria(ruleResult.Tags).ToArray());
    }

    private AccessibilityRuleResult GetViolation (AxeHtmlElementResult htmlElementResult, AccessibilityRule accessibilityRule)
    {
      var anyRequirements = GetAccessibilityRequirements(htmlElementResult.Any).ToArray();
      var allRequirements = GetAccessibilityRequirements(htmlElementResult.All).ToArray();
      var noneRequirements = GetAccessibilityRequirements(htmlElementResult.None).ToArray();

      if (htmlElementResult.XPaths.Length != htmlElementResult.Target.Length)
        throw new InvalidOperationException("The XPath and CSS selectors of the target do not match.");

      var targets = htmlElementResult.XPaths.Zip(htmlElementResult.Target, (xpath, css) => new AccessibilityTestTarget(xpath, css)).ToArray();

      return new AccessibilityRuleResult(accessibilityRule, targets, htmlElementResult.Html, allRequirements, anyRequirements, noneRequirements);
    }

    private IEnumerable<AccessibilityTestSuccessCriteria> GetSuccessCriteria (string[] ruleTags)
    {
      return ruleTags
          .Where(t => !AccessibilityConformanceLevelConverter.IsValid(t))
          .Where(t => !_wellKnownSuccessCriteriaToIgnore.Contains(t))
          .Where(t => !_wellKnownSuccessCriteriaMatcherToIgnore.Any(m => m.IsMatch(t)))
          .Select(ParseAccessibilityTestSuccessCriteria);
    }

    private AccessibilityTestSuccessCriteria ParseAccessibilityTestSuccessCriteria (string rawTag)
    {
      var transformedTag = Regex.Replace(rawTag, @"^wcag(\d)(\d)(\d{1,2})$", "wcag_$1_$2_$3");
      transformedTag = Regex.Replace(transformedTag, @"^section508\.(\d+)\.([a-z])$", "section508_$1_$2");

      return ToEnum<AccessibilityTestSuccessCriteria>(transformedTag);
    }

    private IEnumerable<AccessibilityRequirement> GetAccessibilityRequirements (IEnumerable<AxeRuleCheckResult> ruleCheckResults)
    {
      foreach (var check in ruleCheckResults)
      {
        var checkImpact = ToEnum<AccessibilityTestImpact>(check.Impact);
        var checkId = AccessibilityCheckIDConverter.ConvertToEnum(check.ID);

        yield return new AccessibilityRequirement(checkId, check.Message, checkImpact);
      }
    }

    private T ToEnum<T> (string enumAsString)
    {
      return (T)Enum.Parse(typeof(T), enumAsString, true);
    }
  }
}
