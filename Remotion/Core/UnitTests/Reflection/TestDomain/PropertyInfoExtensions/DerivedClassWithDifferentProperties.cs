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

namespace Remotion.UnitTests.Reflection.TestDomain.PropertyInfoExtensions
{
  public abstract class DerivedClassWithDifferentProperties : ClassWithDifferentProperties
  {
    public override int Int32
    {
      get { return 0; }
      set { }
    }

    public abstract string OtherString { get; set; }
    public new abstract string String { get; set; }

    public override object Object1 { get { return 1; } }
    public override object Object2 { set { } }

    public override int this [object p]
    {
      get { return 2; }
    }
  }
}
