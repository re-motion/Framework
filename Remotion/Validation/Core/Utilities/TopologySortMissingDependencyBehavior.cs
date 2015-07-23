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

namespace Remotion.Validation.Utilities
{
  /// <summary>
  /// Defines how <see cref="TopologySortExtensions.TopologySort{TValidatedType}(System.Collections.Generic.IEnumerable{TValidatedType},System.Func{TValidatedType,System.Collections.Generic.IEnumerable{TValidatedType}},System.Func{System.Collections.Generic.IEnumerable{TValidatedType},System.Collections.Generic.IEnumerable{TValidatedType}},TopologySortMissingDependencyBehavior)"/>
  /// handles dependencies that had not already been included in the set of objects to sort.
  /// </summary>
  public enum TopologySortMissingDependencyBehavior
  {
    /// <summary>
    /// The not included dependency will be ignored.
    /// </summary>
    Ignore = 0,

    /// <summary>
    /// The not included dependency will not be added to the sorted result,
    /// but it's dependencies will transitively be respected in the sort.
    /// </summary>
    Respect = 1,

    /// <summary>
    /// The not included dependency will be treated as if it had been already included in set of objects to sort.
    /// </summary>
    Include = 2
  }
}