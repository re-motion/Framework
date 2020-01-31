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
  /// Represents the result of the accessibility analysis in a readable format.
  /// </summary>
  public class AccessibilityResult
  {
    /// <summary>
    /// Timestamp of analysis.
    /// </summary>
    public DateTime Timestamp { get; }

    /// <summary>
    /// Url of the website that was tested.
    /// </summary>
    public string Url { get; }

    /// <summary>
    /// Version of the Axe test engine.
    /// </summary>
    public string AxeVersion { get; }

    /// <summary>
    /// Width of the window.
    /// </summary>
    public int WindowWidth { get; }

    /// <summary>
    /// Height of the window.
    /// </summary>
    public int WindowHeight { get; }

    /// <summary>
    /// Orientation angle of the screen.
    /// </summary>
    public int OrientationAngle { get; }

    /// <summary>
    /// Orientation type of the screen (eg. landscape, portrait).
    /// </summary>
    public string OrientationType { get; }

    /// <summary>
    /// User agent that was used.
    /// </summary>
    public string UserAgent { get; }

    /// <summary>
    /// Whether iframes were included during the test.
    /// </summary>
    public bool IncludeIFrames { get; }

    /// <summary>
    /// Accessibility tag that was applied for the analysis.
    /// <see cref="IAccessibilityConfiguration.ConformanceLevel" />
    /// </summary>
    public AccessibilityConformanceLevel ConformanceLevel { get; }

    /// <summary>
    /// Elements on the webpage that have been analyzed.
    /// </summary>
    public IReadOnlyCollection<AccessibilityRuleResult> Violations { get; }

    /// <remarks>
    /// Internet Explorer does return null for the orientation type.
    /// </remarks>
    public AccessibilityResult (
        DateTime timestamp,
        [NotNull] string url,
        [NotNull] string axeVersion,
        int windowWidth,
        int windowHeight,
        int orientationAngle,
        [CanBeNull] string orientationType,
        [NotNull] string userAgent,
        bool includeIFrames,
        AccessibilityConformanceLevel conformanceLevel,
        [NotNull] IReadOnlyCollection<AccessibilityRuleResult> violations)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("url", url);
      ArgumentUtility.CheckNotNullOrEmpty ("axeVersion", axeVersion);
      ArgumentUtility.CheckNotEmpty ("orientationType", orientationType); // Is null in IE
      ArgumentUtility.CheckNotNullOrEmpty ("userAgent", userAgent);
      ArgumentUtility.CheckNotNullOrItemsNull ("violations", violations);


      Timestamp = timestamp;
      Url = url;
      AxeVersion = axeVersion;
      WindowWidth = windowWidth;
      WindowHeight = windowHeight;
      OrientationAngle = orientationAngle;
      OrientationType = orientationType;
      UserAgent = userAgent;
      IncludeIFrames = includeIFrames;
      ConformanceLevel = conformanceLevel;
      Violations = violations;
    }

    /// <summary>
    /// Creates a string containing the applied tag and the violations.
    /// </summary>
    public override string ToString ()
    {
      var violations = string.Join (", ", Violations.Select (x => "<" + x.ToString() + ">"));
      return $"Tag: <{ConformanceLevel}>, Violations: <{violations}>";
    }
  }
}