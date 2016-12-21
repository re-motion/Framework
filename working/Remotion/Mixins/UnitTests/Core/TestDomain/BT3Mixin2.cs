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
  [Extends (typeof (BaseType3))]
  [Serializable]
  public class BT3Mixin2 : Mixin<IBaseType32>
  {
    public int I;

    public BT3Mixin2()
    {
    }

    protected BT3Mixin2 (int i)
    {
      I = i;
    }

    public new IBaseType32 Target
    {
      get { return base.Target; }
    }
  }

  [Serializable]
  public class BT3Mixin2B : Mixin<IBaseType32>
  {
    public new IBaseType32 Target
    {
      get { return base.Target; }
    }
  }
}
