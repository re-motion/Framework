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
using System.Runtime.Serialization;

namespace Remotion.Web.Development.WebTesting.Accessibility.AxeJsonResultDtos
{
  /// <summary>
  /// Represents the AXE configuration.
  /// </summary>
  [DataContract]
  public class AxeToolOptions
  {
    /// <summary>Limit which rules are executed, based on names or tags.</summary>
    [DataMember (Name = "runOnly")]
    public AxeExecutionConstraint RunOnly { get; set; }

    /// <summary>Whether <see cref="IAccessibilityConfiguration.IncludeIFrames" /> were allowed during the test.</summary>
    [DataMember (Name = "iframes")]
    public bool IFrames { get; set; }

    /// <summary>Whether XPaths were allowed during the test.</summary>
    [DataMember (Name = "xpath")]
    public bool XPath { get; set; }

    /// <summary>The set IFrameTimeout in milliseconds.</summary>
    [DataMember (Name = "frameWaitTime")]
    public int FrameWaitTime { get; set; }

    /// <summary>Whether Absolute Paths were allowed during the test.</summary>
    [DataMember (Name = "absolutePaths")]
    public bool AbsolutePaths { get; set; }

    /// <summary>Whether RestoreScroll was enabled during the test.</summary>
    [DataMember (Name = "restoreScroll")]
    public bool RestoreScroll { get; set; }
  }
}