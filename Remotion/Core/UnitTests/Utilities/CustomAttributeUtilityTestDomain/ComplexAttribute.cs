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

namespace Remotion.UnitTests.Utilities.CustomAttributeUtilityTestDomain
{
  public class ComplexAttribute : Attribute
  {
    public Type T;
    public object[] Os;
    public Type[] Ts;
    public int[] Is;
    public DayOfWeek[] Es;

    private string s;

    public ComplexAttribute ()
    {
    }

    public ComplexAttribute (Type t)
    {
      T = t;
    }

    public ComplexAttribute (string s)
    {
      this.s = s;
    }

    public ComplexAttribute (string s, params object[] os)
    {
      S = s;
      Os = os;
    }

    public ComplexAttribute (Type t, params Type[] ts)
    {
      T = t;
      Ts = ts;
    }

    public ComplexAttribute (int[] ints)
    {
      Is = ints;
    }

    public ComplexAttribute (DayOfWeek[] es)
    {
      Es = es;
    }

    public string S
    {
      get { return s; }
      set { s = value; }
    }
  }
}
