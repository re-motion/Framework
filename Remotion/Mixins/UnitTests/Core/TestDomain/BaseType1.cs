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

namespace Remotion.Mixins.UnitTests.Core.TestDomain
{
  public class BT1Attribute : Attribute { }

  [BT1]
  public class BaseType1
  {
    public int I;

    [BT1]
    public virtual string VirtualMethod ()
    {
      return "BaseType1.VirtualMethod";
    }

    public virtual string VirtualMethod (string text)
    {
      return "BaseType1.VirtualMethod(" + text + ")";
    }

    private string _backingField = "BaseType1.BackingField";

    [BT1]
    public virtual string VirtualProperty
    {
      get { return _backingField; }
      set { _backingField = value; }
    }

    public object this [int index]
    {
      get { return null; }
    }

    public object this[string index]
    {
      set { }
    }

    [BT1]
    public virtual event EventHandler VirtualEvent;

    public event EventHandler ExplicitEvent
    {
      add { VirtualEvent += value; }
      remove { VirtualEvent -= value; }
    }

    internal Delegate[] GetVirtualEventInvocationList ()
    {
      if (VirtualEvent != null)
        return VirtualEvent.GetInvocationList();
      else
        return null;
    }
  }
}
