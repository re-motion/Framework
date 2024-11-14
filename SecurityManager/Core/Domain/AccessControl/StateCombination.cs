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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class StateCombination : AccessControlObject
  {
    public static StateCombination NewObject ()
    {
      return NewObject<StateCombination>();
    }

    private DomainObjectDeleteHandler? _deleteHandler;

    protected StateCombination ()
    {
    }

    public abstract int Index { get; set; }

    [DBBidirectionalRelation("StateCombination")]
    protected abstract ObjectList<StateUsage> StateUsages { get; }

    [StorageClassNone]
    public SecurableClassDefinition? Class
    {
      get { return AccessControlList != null ? AccessControlList.Class : null; }
    }

    [DBBidirectionalRelation("StateCombinationsInternal")]
    [Mandatory]
    public abstract StatefulAccessControlList? AccessControlList { get; }

    public bool MatchesStates (IList<StateDefinition> states)
    {
      ArgumentUtility.CheckNotNullOrItemsNull("states", states);

      if (StateUsages.Count == 0 && states.Count > 0)
        return false;

      return StateUsages.Select(stateUsage => stateUsage.StateDefinition).All(usedState => states.Contains(usedState));
    }

    public void AttachState (StateDefinition state)
    {
      ArgumentUtility.CheckNotNull("state", state);
      StateUsage stateUsage = StateUsage.NewObject(state);
      StateUsages.Add(stateUsage);
    }

    public void ClearStates ()
    {
      foreach (var stateUsage in StateUsages.ToList())
        stateUsage.Delete();
    }

    public StateDefinition[] GetStates ()
    {
      return StateUsages.Select(stateUsage => stateUsage.StateDefinition).ToArray();
    }

    protected override void OnCommitting (DomainObjectCommittingEventArgs args)
    {
      base.OnCommitting(args);

      if (Class != null)
        Class.RegisterForCommit();
    }

    //TODO: Rewrite with test

    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting(args);

      _deleteHandler = new DomainObjectDeleteHandler(StateUsages);
    }

    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted(args);

      _deleteHandler?.Delete();
    }
  }
}
