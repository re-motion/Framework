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

namespace Remotion.Web.Development.WebTesting.Accessibility
{
  /// Default implementation of <see cref="IAccessibilityConfiguration" />.
  public class AccessibilityConfiguration : IAccessibilityConfiguration
  {
    public AccessibilityConformanceLevel ConformanceLevel { get; }
    public bool IncludeIFrames { get; }
    public TimeSpan IFrameTimeout { get; }
    public bool EnableScrollToInitialPosition { get; }
    public bool EnableAbsolutePaths { get; }
    public bool EnableXPath { get; }

    public AccessibilityConfiguration (
        AccessibilityConformanceLevel conformanceLevel = AccessibilityConformanceLevel.Wcag20_ConformanceLevelDoubleA,
        bool? includeIframes = null,
        TimeSpan? iframeTimeout = null,
        bool? scrollToInitialPosition = null,
        bool? absolutePaths = null)
    {
      if (iframeTimeout < TimeSpan.Zero)
        throw new ArgumentOutOfRangeException("iframeTimeout", "The iframe timeout must be greater than or equal to zero.");

      ConformanceLevel = conformanceLevel;
      IncludeIFrames = includeIframes ?? true;
      IFrameTimeout = iframeTimeout ?? TimeSpan.FromSeconds(5);
      EnableScrollToInitialPosition = scrollToInitialPosition ?? true;
      EnableAbsolutePaths = absolutePaths ?? true;
      EnableXPath = true;
    }
  }
}
