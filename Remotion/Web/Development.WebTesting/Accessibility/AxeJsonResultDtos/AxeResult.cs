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
  /// <summary>Represents an AXE accessibility analysis result.</summary>
  [DataContract]
  public class AxeResult
  {
    /// <summary>Timestamp of analysis.</summary>
    [DataMember (Name = "timestamp")]
    public string Timestamp { get; set; }

    /// <summary>Engine the test ran on.</summary>
    [DataMember (Name = "testEngine")]
    public AxeTestEngine TestEngine { get; set; }

    /// <summary>Name of the test runner.</summary>
    [DataMember (Name = "testRunner")]
    public AxeTestRunner TestRunner { get; set; }

    /// <summary>Test environment the test ran in.</summary>
    [DataMember (Name = "testEnvironment")]
    public AxeTestEnvironment TestEnvironment { get; set; }

    /// <summary>Url of the website that was tested.</summary>
    [DataMember (Name = "url")]
    public string Url { get; set; }

    /// <summary>The options the test ran with.</summary>
    [DataMember (Name = "toolOptions")]
    public AxeToolOptions ToolOptions { get; set; }

    /// <summary>Contains elements that violated the rules.</summary>
    [DataMember (Name = "violations")]
    public AxeRuleResult[] Violations { get; set; } = new AxeRuleResult[0];

    /// <summary>Contains elements that passed the rules.</summary>
    [DataMember (Name = "passes")]
    public AxeRuleResult[] Passes { get; set; } = new AxeRuleResult[0];

    /// <summary>Indicates which rules did not run because no matching content was found on the page.</summary>
    [DataMember (Name = "inapplicable")]
    public AxeRuleResult[] Inapplicable { get; set; } = new AxeRuleResult[0];

    /// <summary>Contains elements which could not be checked.</summary>
    [DataMember (Name = "incomplete")]
    public AxeRuleResult[] Incomplete { get; set; } = new AxeRuleResult[0];
  }
}