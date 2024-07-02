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

namespace Remotion.Mixins.MixerTools.UnitTests.TestDomain
{
  public interface IBT1Mixin1
  {
    string IntroducedMethod ();
    string IntroducedProperty { get; }
    event EventHandler IntroducedEvent;
  }

  public class BT1M1Attribute : Attribute {}

  [Extends(typeof(BaseType1))]
  [BT1M1]
  [AcceptsAlphabeticOrdering]
  public class BT1Mixin1 : IBT1Mixin1
  {
    [OverrideTarget]
    [BT1M1]
    public string VirtualMethod ()
    {
      return "BT1Mixin1.VirtualMethod";
    }

    public string BackingField = "BT1Mixin1.BackingField";

    [OverrideTarget]
    [BT1M1]
    public virtual string VirtualProperty
    {
      set { BackingField = value; } // no getter
    }

    public bool VirtualEventAddCalled = false;
    public bool VirtualEventRemoveCalled = false;

    [OverrideTarget]
    [BT1M1]
    public virtual event EventHandler VirtualEvent
    {
      add { VirtualEventAddCalled = true; }
      remove { VirtualEventRemoveCalled = true; }
    }


    [BT1M1]
    public string IntroducedMethod ()
    {
      return "BT1Mixin1.IntroducedMethod";
    }

    [BT1M1]
    public string IntroducedProperty
    {
      get { return "BT1Mixin1.IntroducedProperty"; }
    }

    [BT1M1]
    public event EventHandler IntroducedEvent;
  }
}
