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
using System.Runtime.Serialization;

namespace Remotion.Web.Development.WebTesting.Accessibility.AxeJsonResultDtos
{
  /// <summary>
  /// Represents the tags or rules that were run during an AXE analysis, if the tags or rules were narrowed down.
  /// </summary>
  [DataContract]
  public class AxeExecutionConstraint
  {
    /// <summary>Either <c>tag</c> or <c>rule</c>.</summary>
    [DataMember(Name = "type")]
    public string Type { get; set; } = null!;

    /// <summary>Defined tag or rule names.</summary>
    [DataMember(Name = "values")]
    public string[] Values { get; set; } = { };
  }
}
