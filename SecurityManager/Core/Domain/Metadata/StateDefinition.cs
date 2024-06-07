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
using Remotion.TypePipe;

namespace Remotion.SecurityManager.Domain.Metadata
{
  [Serializable]
  [Instantiable]
  public abstract class StateDefinition : EnumValueDefinition
  {
    // types

    // static members and constants

    public static StateDefinition NewObject ()
    {
      return NewObject<StateDefinition>();
    }

    public static StateDefinition NewObject (string name, int value)
    {
      return NewObject<StateDefinition>(ParamList.Create(name, value));
    }

    // member fields

    // construction and disposing

    protected StateDefinition ()
    {
    }

    protected StateDefinition (string name, int value)
    {
      Name = name;
      Value = value;
    }

    // methods and properties

    [DBBidirectionalRelation("DefinedStatesInternal")]
    [Mandatory]
    public abstract StatePropertyDefinition StateProperty { get; }

    public override sealed Guid MetadataItemID
    {
      get { throw new NotSupportedException("States do not support MetadataItemID"); }
      set { throw new NotSupportedException("States do not support MetadataItemID"); }
    }

    protected override void OnDeleting (EventArgs args)
    {
      if (StateProperty != null)
      {
        throw new InvalidOperationException(
            string.Format("State '{0}' cannot be deleted because it is associated with state property '{1}'.", Name, StateProperty.Name));
      }
      base.OnDeleting(args);
    }
  }
}
