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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Accessibility
{
  /// <summary>Contains information about a rule.</summary>
  public class AccessibilityRule
  {
    /// <summary>ID of the rule.</summary>
    public AccessibilityRuleID ID { get; }

    /// <summary>Description of the rule.</summary>
    public string Description { get; }

    /// <summary>Impact of the tested rule.</summary>
    public AccessibilityTestImpact Impact { get; }

    /// <summary>Success criteria of the rule.</summary>
    public IReadOnlyCollection<AccessibilityTestSuccessCriteria> SuccessCriteria { get; }

    public AccessibilityRule (
        AccessibilityRuleID id,
        [NotNull] string description,
        AccessibilityTestImpact impact,
        [NotNull] IReadOnlyCollection<AccessibilityTestSuccessCriteria> successCriteria)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("description", description);
      ArgumentUtility.CheckNotNullOrEmpty ("successCriteria", successCriteria);

      ID = id;
      Description = description;
      Impact = impact;
      SuccessCriteria = successCriteria;
    }
  }
}