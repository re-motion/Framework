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
  public interface IBT6Mixin3
  {
    string Mixin3Method ();
  }

  public interface IBT6Mixin3Constraints : ICBT6Mixin1, ICBT6Mixin2 {}

  [Extends (typeof (BaseType6))]
  public class BT6Mixin3<[BindToConstraints] This> : Mixin<This>, IBT6Mixin3
      where This : class, IBT6Mixin3Constraints
  {
    public string Mixin3Method ()
    {
      return "BT6Mixin3.Mixin3Method";
    }
  }

  [ComposedInterface (typeof (BaseType6))]
  public interface ICBT6Mixin3 : IBT6Mixin1, IBT6Mixin2, IBaseType6
  {
  }
}
