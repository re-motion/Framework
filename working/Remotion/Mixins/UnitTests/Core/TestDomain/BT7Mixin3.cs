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
  public interface IBT7Mixin3
  {
    string One<T> (T t);
  }

  [Extends (typeof (BaseType7))]
  public class BT7Mixin3 : Mixin<object, IBT7Mixin1>, IBT7Mixin3
  {
    [OverrideTarget]
    public virtual string One<T> (T t)
    {
      return "BT7Mixin3.One(" + t + ")-" + Next.BT7Mixin1Specific() + "-" + Next.One(t);
    }
  }
}
