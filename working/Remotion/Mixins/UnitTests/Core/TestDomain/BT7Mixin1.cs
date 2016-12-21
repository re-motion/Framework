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
  public interface IBT7Mixin1
  {
    string One<T> (T t);
    string BT7Mixin1Specific ();
  }

  [Extends (typeof (BaseType7))]
  public class BT7Mixin1 : Mixin<BaseType7, IBaseType7> , IBT7Mixin1
  {
    [OverrideTarget]
    public virtual string One<T> (T t)
    {
      return "BT7Mixin1.One(" + t + ")-" + Next.One(t);
    }

    public string BT7Mixin1Specific ()
    {
      return "BT7Mixin1.BT7Mixin1Specific-" + Next.Three() + "-" + Target.Three();
    }
  }
}
