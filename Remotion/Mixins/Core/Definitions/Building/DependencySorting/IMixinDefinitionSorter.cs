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
using System.Collections.Generic;

namespace Remotion.Mixins.Definitions.Building.DependencySorting
{
  /// <summary>
  /// Defines an interface for classes sorting a sequence of <see cref="MixinDefinition"/> objects based on the dependencies between the mixins and 
  /// other ordering-relevant information.
  /// </summary>
 public interface IMixinDefinitionSorter
  {
    /// <summary>
    /// Sorts the given mixins.
    /// </summary>
    /// <param name="mixinDefinitions">The <see cref="MixinDefinition"/> objects to sort relative to each other.</param>
    /// <returns>A sequence with the given mixins, but in the correct order.</returns>
    /// <exception cref="InvalidOperationException">The <paramref name="mixinDefinitions"/> cannot be sorted.</exception>
    IEnumerable<MixinDefinition> SortMixins (IEnumerable<MixinDefinition> mixinDefinitions);
  }
}
