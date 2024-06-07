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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.TypePipe;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [Serializable]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class StateUsage : AccessControlObject
  {
    public static StateUsage NewObject (StateDefinition stateDefinition)
    {
      ArgumentUtility.CheckNotNull("stateDefinition", stateDefinition);

      return NewObject<StateUsage>(ParamList.Create(stateDefinition));
    }

    protected StateUsage (StateDefinition stateDefinition)
    {
      // ReSharper disable once VirtualMemberCallInConstructor
      StateDefinition = stateDefinition;
    }

    [Mandatory]
    public abstract StateDefinition StateDefinition { get; protected set; }

    [DBBidirectionalRelation("StateUsages")]
    [Mandatory]
    protected abstract StateCombination? StateCombination { get; }

    [StorageClassNone]
    public SecurableClassDefinition? Class
    {
      get
      {
        if (StateCombination == null)
          return null;
        return StateCombination.Class;
      }
    }

    protected override void OnCommitting (DomainObjectCommittingEventArgs args)
    {
      base.OnCommitting(args);

      if (Class != null)
        Class.RegisterForCommit();
    }
  }
}
