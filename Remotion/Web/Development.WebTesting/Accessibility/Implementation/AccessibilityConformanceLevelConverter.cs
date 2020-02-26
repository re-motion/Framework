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

namespace Remotion.Web.Development.WebTesting.Accessibility.Implementation
{
  /// <summary>
  /// Provides conversions from <see cref="AccessibilityConformanceLevel"/> to <see cref="string"/> and vice versa.
  /// </summary>
  public static class AccessibilityConformanceLevelConverter
  {
    private static readonly IReadOnlyDictionary<string, AccessibilityConformanceLevel> s_stringToEnum;
    private static readonly IReadOnlyDictionary<AccessibilityConformanceLevel, string> s_enumToString;

    static AccessibilityConformanceLevelConverter ()
    {
      s_stringToEnum
          = new Dictionary<string, AccessibilityConformanceLevel>
            {
                { "wcag2a", AccessibilityConformanceLevel.Wcag20_ConformanceLevelA },
                { "wcag21a", AccessibilityConformanceLevel.Wcag21_ConformanceLevelA },
                { "wcag2aa", AccessibilityConformanceLevel.Wcag20_ConformanceLevelDoubleA },
                { "wcag21aa", AccessibilityConformanceLevel.Wcag21_ConformanceLevelDoubleA },
                { "section508", AccessibilityConformanceLevel.Section508 },
            };

      s_enumToString = s_stringToEnum.ToDictionary (kvp => kvp.Value, kvp => kvp.Key);
    }

    /// <summary>
    /// Converts a <see cref="string"/> to its <see cref="AccessibilityConformanceLevel"/> representation.
    /// </summary>
    public static AccessibilityConformanceLevel ConvertToEnum ([NotNull] string conformanceLevelAsString)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("conformanceLevelAsString", conformanceLevelAsString);

      if (!s_stringToEnum.TryGetValue (conformanceLevelAsString, out var conformanceLevel))
        throw new InvalidOperationException ($"The conformance level '{conformanceLevelAsString}' is not supported.");

      return conformanceLevel;
    }

    /// <summary>
    /// Converts <see cref="AccessibilityConformanceLevel"/> to its <see cref="string"/> representation.
    /// </summary>
    public static string ConvertToString (AccessibilityConformanceLevel conformanceLevel)
    {
      return s_enumToString[conformanceLevel];
    }

    /// <summary>
    /// Determines whether <param name="conformanceLevelAsString"/> can be converted to <see cref="AccessibilityConformanceLevel"/>.
    /// </summary>
    public static bool IsValid ([NotNull] string conformanceLevelAsString)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("conformanceLevelAsString", conformanceLevelAsString);

      return s_stringToEnum.ContainsKey (conformanceLevelAsString);
    }
  }
}