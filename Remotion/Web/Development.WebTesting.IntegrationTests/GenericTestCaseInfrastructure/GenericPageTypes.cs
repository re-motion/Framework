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
using Remotion.Web.Development.WebTesting.TestSite;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.GenericTestCaseInfrastructure
{
  /// <summary>
  /// The type of page that should be displayed when displaying the generic page.
  /// </summary>
  /// <remarks>
  /// This enumeration goes hand in hand with <see cref="GenericTest.GenericPageTypes"/>. 
  /// </remarks>
  [Flags]
  public enum GenericPageTypes
  {
    /// <summary>
    /// Renders only the hidden sections.
    /// </summary>
    Hidden = 1,

    /// <summary>
    /// Renders all sections that are not ambiguous.
    /// </summary>
    NoAmbiguous = 3,

    /// <summary>
    /// Renders all sections.
    /// </summary>
    All = 7,

    /// <summary>
    /// Renders the hidden section.
    /// </summary>
    HiddenSection = 1,

    /// <summary>
    /// Renders the shown section.
    /// </summary>
    ShownSection = 2,

    /// <summary>
    /// Renders the ambiguous section.
    /// </summary>
    AmbiguousSection = 4
  }
}