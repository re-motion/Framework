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
using System.Linq;
using Remotion.FunctionalProgramming;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  /// <summary>
  /// Used for creating the outer product of a <see cref="SecurableClassDefinition"/>'s <see cref="SecurableClassDefinition.StateProperties"/>'
  /// <see cref="StateDefinition"/> values.
  /// </summary>
  public class StateCombinationBuilder : IStateCombinationBuilder
  {
    private readonly SecurableClassDefinition _classDefinition;

    public StateCombinationBuilder (SecurableClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);

      _classDefinition = classDefinition;
    }

    public SecurableClassDefinition ClassDefinition
    {
      get { return _classDefinition; }
    }

    public PropertyStateTuple[][] CreatePropertyProduct ()
    {
      IEnumerable<IEnumerable<PropertyStateTuple>> seed = new PropertyStateTuple[][] { };

      var allStatesByProperty = from property in _classDefinition.StateProperties
                                select from state in property.DefinedStates.DefaultIfEmpty()
                                       select new PropertyStateTuple(property, state);

      var aggregatedStates = allStatesByProperty.Aggregate(seed, CreateOuterProduct);

      return aggregatedStates.Select(innerList => innerList.ToArray()).ToArray();
    }

    private IEnumerable<IEnumerable<PropertyStateTuple>> CreateOuterProduct (
        IEnumerable<IEnumerable<PropertyStateTuple>> previous,
        IEnumerable<PropertyStateTuple> current)
    {
      return from p in previous.DefaultIfEmpty(new PropertyStateTuple[0])
             from c in current
             select p.Concat(c);
    }
  }
}
