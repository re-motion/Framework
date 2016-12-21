// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation
{
  public class StatefulAccessControlListData
  {
    private readonly IDomainObjectHandle<StatefulAccessControlList> _handle;
    private readonly ReadOnlyCollectionDecorator<State> _states;

    public StatefulAccessControlListData ([NotNull] IDomainObjectHandle<StatefulAccessControlList> handle, [NotNull] IEnumerable<State> states)
    {
      ArgumentUtility.CheckNotNull ("handle", handle);
      ArgumentUtility.CheckNotNull ("states", states);

      var stateArray = states.ToArray().AsReadOnly();

      if (stateArray.Select (s => s.PropertyHandle).Distinct().Count() != stateArray.Count)
        throw new ArgumentException ("Multiple state values found for a single state property.", "states");

      _handle = handle;
      _states = stateArray;
    }

    [NotNull]
    public IDomainObjectHandle<StatefulAccessControlList> Handle
    {
      get { return _handle; }
    }

    [NotNull]
    public ReadOnlyCollectionDecorator<State> States
    {
      get { return _states; }
    }
  }
}