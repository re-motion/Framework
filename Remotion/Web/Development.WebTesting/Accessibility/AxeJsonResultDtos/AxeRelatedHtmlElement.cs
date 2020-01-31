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
  /// Represents the <c>relatedNodes</c> JSON object in an AXE analysis result.
  /// </summary>
  /// <remarks>
  /// Represents nodes causally related to a <see cref="AxeRuleCheckResult"/>. In some cases, not only one node is affected by a check, but multiple. Those additional
  /// nodes are referred to as "related nodes".
  /// </remarks>
  [DataContract]
  public class AxeRelatedHtmlElement
  {
    /// <summary>Html snippet of the related node.</summary>
    [DataMember (Name = "html")]
    public string Html { get; set; }

    /// <summary>Target of the related node.</summary>
    [DataMember (Name = "target")]
    public string[] Target { get; set; }

    /// <summary>XPath to the related node.</summary>
    [DataMember (Name = "xpath")]
    public string[] XPath { get; set; }
  }
}