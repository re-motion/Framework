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
  /// <summary>Represents an AXE accessibility rule.</summary>
  [DataContract]
  public class AxeRuleResult
  {
    /// <summary>Description of the rule.</summary>
    [DataMember (Name = "description")]
    public string Description { get; set; }

    /// <summary>ID of the rule.</summary>
    [DataMember (Name = "id")]
    public string ID { get; set; }

    /// <summary>Impact of the rule.</summary>
    [DataMember (Name = "impact")]
    public string Impact { get; set; }

    /// <summary>Tags of the rule.</summary>
    [DataMember (Name = "tags")]
    public string[] Tags { get; set; }

    /// <summary>Help information about the rule.</summary>
    [DataMember (Name = "help")]
    public string Help { get; set; }

    /// <summary>Help url for the rule.</summary>
    [DataMember (Name = "helpUrl")]
    public string HelpUrl { get; set; }

    /// <summary>Tested nodes of the rule.</summary>
    [DataMember (Name = "nodes")]
    public AxeHtmlElementResult[] Nodes { get; set; } = new AxeHtmlElementResult[0];
  }
}