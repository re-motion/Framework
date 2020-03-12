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
using System.Text;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Accessibility.Implementation
{
  public static class AccessibilityConfigurationJsonSerializer
  {
    [NotNull]
    public static string Serialize ([NotNull] IAccessibilityConfiguration configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      var sb = new StringBuilder();
      sb.Append ('{');
      sb.Append ($"iframes: {configuration.IncludeIFrames.ToString().ToLower()}");
      sb.Append ($", xpath: {configuration.EnableXPath.ToString().ToLower()}");
      sb.Append ($", frameWaitTime: {configuration.IFrameTimeout.TotalMilliseconds}");
      sb.Append ($", absolutePaths: {configuration.EnableAbsolutePaths.ToString().ToLower()}");
      sb.Append ($", restoreScroll: {configuration.EnableScrollToInitialPosition.ToString().ToLower()}");
      sb.Append (", runOnly: {type:\"tag\",values:[");
      sb.Append ($"\"{AccessibilityConformanceLevelConverter.ConvertToString (configuration.ConformanceLevel)}\"");

      if (configuration.ConformanceLevel == AccessibilityConformanceLevel.Wcag20_ConformanceLevelDoubleA)
        AppendWcag2A (sb);

      //axe version 3.2.2 requires the experimental tag for WCAG 2.1 A and WCAG 2.1 AA
      if (configuration.ConformanceLevel == AccessibilityConformanceLevel.Wcag21_ConformanceLevelA)
      {
        AppendWcag2A (sb);
        AppendExperimental (sb);
      }

      if (configuration.ConformanceLevel == AccessibilityConformanceLevel.Wcag21_ConformanceLevelDoubleA)
      {
        AppendWcag2A (sb);
        AppendWcag21A (sb);
        AppendWcag2DoubleA (sb);
        AppendExperimental (sb);
      }

      sb.Append ("]}}");

      return sb.ToString();

      void AppendWcag2A (StringBuilder stringBuilder) => stringBuilder.Append (",\"wcag2a\"");
      void AppendWcag21A (StringBuilder stringBuilder) => stringBuilder.Append (",\"wcag21a\"");
      void AppendWcag2DoubleA (StringBuilder stringBuilder) => stringBuilder.Append (",\"wcag2aa\"");
      void AppendExperimental (StringBuilder stringBuilder) => stringBuilder.Append (",\"experimental\"");
    }
  }
}