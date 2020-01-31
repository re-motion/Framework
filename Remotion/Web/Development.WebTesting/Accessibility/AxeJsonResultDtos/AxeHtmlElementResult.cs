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
  /// <summary>Represents an HTML element that was targeted by AXE accessibility checks.</summary>
  [DataContract]
  public class AxeHtmlElementResult
  {
    /// <summary>Html snippet of the node.</summary>
    [DataMember (Name = "html")]
    public string Html { get; set; }

    /// <summary>Target of the node.</summary>
    [DataMember (Name = "target")]
    public string[] Target { get; set; }

    /// <summary>XPath to the node.</summary>
    [DataMember (Name = "xpath")]
    public string[] XPaths { get; set; }

    /// <summary>List of checks where at least one has passed.</summary>
    [DataMember (Name = "any")]
    public AxeRuleCheckResult[] Any { get; set; }

    /// <summary>List of checks where none have passed.</summary>
    [DataMember (Name = "none")]
    public AxeRuleCheckResult[] None { get; set; }

    /// <summary>List of checks where all have passed.</summary>
    [DataMember (Name = "all")]
    public AxeRuleCheckResult[] All { get; set; }
  }
}