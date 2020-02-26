﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Accessibility
{
  /// <summary>
  /// Contains information about the accessibility requirement.
  /// </summary>
  /// <remarks>
  /// An accessibility requirement translates to an AXE accessibility check.
  /// </remarks>
  public class AccessibilityRequirement
  {
    /// <summary>ID of the check.</summary>
    public AccessibilityRequirementID ID { get; }

    /// <summary>Message of the requirement.</summary>
    public string Message { get; }

    /// <summary>Impact of the requirement.</summary>
    public AccessibilityTestImpact Impact { get; }

    public AccessibilityRequirement (AccessibilityRequirementID id, string message, AccessibilityTestImpact impact)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("message", message);

      ID = id;
      Message = message;
      Impact = impact;
    }
  }
}