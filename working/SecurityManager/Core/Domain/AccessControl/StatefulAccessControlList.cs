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
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [Serializable]
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

    private DomainObjectDeleteHandler _deleteHandler;

    protected StatefulAccessControlList ()
    {
    }

    protected override void OnRelationChanged (RelationChangedEventArgs args)
    {
      base.OnRelationChanged (args);
      if (args.IsRelation (this, "StateCombinationsInternal"))
        HandleStateCombinationsChanged ((StateCombination) args.NewRelatedObject);
    }

    private void HandleStateCombinationsChanged (StateCombination stateCombination)
    {
      if (stateCombination != null)
        stateCombination.Index = StateCombinationsInternal.IndexOf (stateCombination);
    }

    public abstract int Index { get; set; }

    [DBBidirectionalRelation ("StatefulAccessControlLists")]
    [DBColumn ("StatefulAcl_ClassID")]
    [Mandatory]
    protected abstract SecurableClassDefinition MyClass { get; }

    [DBBidirectionalRelation ("AccessControlList", SortExpression = "Index ASC")]
    [Mandatory]
    protected abstract ObjectList<StateCombination> StateCombinationsInternal { get; }

    [StorageClassNone]
    public ReadOnlyCollection<StateCombination> StateCombinations
    {
      get { return StateCombinationsInternal.AsReadOnlyCollection(); }
    }

    public override SecurableClassDefinition Class
    {
      get { return MyClass; }
    }

    //TODO: Rewrite with test
    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting (args);

      _deleteHandler = new DomainObjectDeleteHandler (StateCombinationsInternal);
    }

    //TODO: Rewrite with test
    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted (args);

      _deleteHandler.Delete();
    }

    public StateCombination CreateStateCombination ()
    {
      if (Class == null)
      {
        throw new InvalidOperationException (
            "Cannot create StateCombination if no SecurableClassDefinition is assigned to this StatefulAccessControlList.");
      }

      var stateCombination = StateCombination.NewObject();
      StateCombinationsInternal.Add (stateCombination);

      return stateCombination;
    }
  }
}