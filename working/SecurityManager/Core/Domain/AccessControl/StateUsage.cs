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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [Serializable]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class StateUsage : AccessControlObject
  {
    public static StateUsage NewObject ()
    {
      return NewObject<StateUsage> ();
    }

    protected StateUsage ()
    {
    }

    [Mandatory]
    public abstract StateDefinition StateDefinition { get; set; }

    [DBBidirectionalRelation ("StateUsages")]
    [Mandatory]
    protected abstract StateCombination StateCombination { get; }
    
    [StorageClassNone]
    public SecurableClassDefinition Class
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
      base.OnCommitting (args);

      if (Class != null)
        Class.RegisterForCommit ();
    }
  }
}
