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
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [Instantiable]
  public abstract class StatefulAccessControlList : AccessControlList
  {
    public static Expression<Func<StatefulAccessControlList, IEnumerable<StateCombination>>> SelectStateCombinations ()
    {
      return acl => acl.StateCombinationsInternal;
    }

    public static StatefulAccessControlList NewObject ()
    {
      return NewObject<StatefulAccessControlList>();
    }

    private DomainObjectDeleteHandler? _deleteHandler;

    protected StatefulAccessControlList ()
    {
    }

    protected override void OnRelationChanged (RelationChangedEventArgs args)
    {
      base.OnRelationChanged(args);
      if (args.IsRelation(this, nameof(StateCombinationsInternal)))
        HandleStateCombinationsChanged();
    }

    private void HandleStateCombinationsChanged ()
    {
      var stateCombinations = StateCombinationsInternal;
      for (int i = 0; i < stateCombinations.Count; i++)
        stateCombinations[i].Index = i;
    }

    public abstract int Index { get; set; }

    [DBBidirectionalRelation("StatefulAccessControlLists")]
    [DBColumn("StatefulAcl_ClassID")]
    [Mandatory]
    protected abstract SecurableClassDefinition? MyClass { get; }

    [DBBidirectionalRelation("AccessControlList", SortExpression = "Index ASC")]
    [Mandatory]
    protected abstract ObjectList<StateCombination> StateCombinationsInternal { get; }

    [StorageClassNone]
    public ReadOnlyCollection<StateCombination> StateCombinations
    {
      get { return StateCombinationsInternal.AsReadOnlyCollection(); }
    }

    public override SecurableClassDefinition? Class
    {
      get { return MyClass; }
    }

    //TODO: Rewrite with test
    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting(args);

      _deleteHandler = new DomainObjectDeleteHandler(StateCombinationsInternal);
    }

    //TODO: Rewrite with test
    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted(args);

      _deleteHandler?.Delete();
    }

    public StateCombination CreateStateCombination ()
    {
      if (Class == null)
      {
        throw new InvalidOperationException(
            "Cannot create StateCombination if no SecurableClassDefinition is assigned to this StatefulAccessControlList.");
      }

      var stateCombination = StateCombination.NewObject();
      StateCombinationsInternal.Add(stateCombination);

      return stateCombination;
    }
  }
}
