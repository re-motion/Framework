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
using Remotion.Development.UnitTesting;

namespace Remotion.UnitTests.Reflection.TestDomain.PropertyInfoExtensions
{
  public abstract class ClassWithDifferentProperties
  {
    public static int StaticInt32 { get; set; }
    private static int PrivateStaticInt32 { get; set; }

    public abstract int Int32 { get; set; }
    protected abstract int ProtectedInt32 { get; set; }

    public abstract int this [int p] { get; set; }
    public abstract int this [string p] { get; set; }

    public virtual int this [object p]
    {
      get { return 1; }
      set { Dev.Null = value; }
    }

    public virtual string String
    {
      get { return ""; }
      set { Dev.Null = value; }
    }

    public virtual object Object1
    {
      get { return new object(); }
      set { Dev.Null = value; }
    }

    public virtual object Object2
    {
      get { return new object(); }
      set { Dev.Null = value; }
    }
  }
}