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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Accessibility
{
  /// <summary>
  /// Accessibility check of an element.
  /// </summary>
  public class AccessibilityRuleResult
  {
    /// <summary>List of checks where all have passed.</summary>
    public IReadOnlyCollection<AccessibilityRequirement> All { get; }

    /// <summary>List of checks where at least one has passed.</summary>
    public IReadOnlyCollection<AccessibilityRequirement> Any { get; }

    /// <summary>List of checks where none have passed.</summary>
    public IReadOnlyCollection<AccessibilityRequirement> None { get; }

    /// <summary>
    /// Target path of the tested element.
    /// In case the element is in an iframe, there are multiple targets, one for
    /// the element itself and one for every iframe level.
    /// </summary>
    public IReadOnlyList<AccessibilityTestTarget> TargetPath { get; }

    /// <summary>Html snippet of the element.</summary>
    public string Html { get; }

    /// <summary>Rule which this element is part of.</summary>
    public AccessibilityRule Rule { get; }

    public AccessibilityRuleResult (
        [NotNull] AccessibilityRule rule,
        [NotNull] IReadOnlyList<AccessibilityTestTarget> targetPath,
        [NotNull] string html,
        [NotNull] IReadOnlyCollection<AccessibilityRequirement> all,
        [NotNull] IReadOnlyCollection<AccessibilityRequirement> any,
        [NotNull] IReadOnlyCollection<AccessibilityRequirement> none)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("all", all);
      ArgumentUtility.CheckNotNullOrItemsNull ("any", any);
      ArgumentUtility.CheckNotNullOrItemsNull ("none", none);
      ArgumentUtility.CheckNotNull ("rule", rule);
      ArgumentUtility.CheckNotNullOrEmpty ("html", html);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("targetPath", targetPath);

      Html = html;
      Rule = rule;
      All = all;
      Any = any;
      None = none;
      TargetPath = targetPath;
    }

    /// <summary>
    /// Creates a string containing the rule ID, the CSS selector and the checks.
    /// </summary>
    public override string ToString ()
    {
      var xPaths = string.Join (",", TargetPath.Select (p => p.XPath).Select (p => $"\"{p}\""));

      return $"Rule: {Rule.ID}, XPath: [{xPaths}]";
    }
  }
}