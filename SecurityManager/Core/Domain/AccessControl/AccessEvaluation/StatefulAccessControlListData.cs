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
using JetBrains.Annotations;
using Remotion.Data.DomainObjects;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation
{
  public class StatefulAccessControlListData
  {
    private readonly IDomainObjectHandle<StatefulAccessControlList> _handle;
    private readonly IReadOnlyCollection<State> _states;

    public StatefulAccessControlListData ([NotNull] IDomainObjectHandle<StatefulAccessControlList> handle, [NotNull] IEnumerable<State> states)
    {
      ArgumentUtility.CheckNotNull("handle", handle);
      ArgumentUtility.CheckNotNull("states", states);

      var stateArray = Array.AsReadOnly(states.ToArray());

      if (stateArray.Select(s => s.PropertyHandle).Distinct().Count() != stateArray.Count)
        throw new ArgumentException("Multiple state values found for a single state property.", "states");

      _handle = handle;
      _states = stateArray;
    }

    [NotNull]
    public IDomainObjectHandle<StatefulAccessControlList> Handle
    {
      get { return _handle; }
    }

    [NotNull]
    public IReadOnlyCollection<State> States
    {
      get { return _states; }
    }
  }
}
