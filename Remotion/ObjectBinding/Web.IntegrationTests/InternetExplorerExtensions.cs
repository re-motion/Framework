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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Remotion.Web.Development.WebTesting.Accessibility;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.InternetExplorer;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration;

namespace Remotion.ObjectBinding.Web.IntegrationTests
{
  internal static class InternetExplorerExtensions
  {
    /// <summary>
    /// Removes elements where known workarounds cause violations.
    /// </summary>
    public static IEnumerable<AccessibilityRuleResult> IgnoreKnownWorkarounds (
        this IEnumerable<AccessibilityRuleResult> violations,
        IBrowserConfiguration browserConfiguration)
    {
      if (browserConfiguration.IsInternetExplorer())
      {
        var cssRegex = new Regex (
            @"(\.bocListTableBlock|\.hasMenuBlock|\.hasNavigator) > \.screenReaderText\[aria-label="".+""]\[aria-hidden=""true""]");

        return violations.Where (
            ruleResult => !(ruleResult.Rule.ID == AccessibilityRuleID.AriaHiddenFocus
                            && cssRegex.IsMatch (ruleResult.TargetPath.Last().CssSelector)));
      }

      return violations;
    }
  }
}