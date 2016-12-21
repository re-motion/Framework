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
  public class ComplexAttributeTarget
  {
    [Complex ()]
    public void DefaultCtor ()
    {
    }

    [Complex (S = "foo")]
    public void DefaultCtorWithProperty ()
    {
    }

    [Complex (T = typeof (object))]
    public void DefaultCtorWithField ()
    {
    }

    [Complex (typeof (void), S = "string")]
    public void CtorWithTypeAndProperty ()
    {
    }

    [Complex ("s", 1, 2, 3, "4")]
    public void CtorWithStringAndParamsArray ()
    {
    }

    [Complex (typeof (double), typeof (int), typeof (string))]
    public void CtorWithStringAndTypeParamsArray ()
    {
    }

    [Complex (new int[] {1, 2, 3})]
    public void CtorWithIntArray ()
    {
    }

    [Complex (new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday })]
    public void CtorWithEnumArray ()
    {
    }
  }
}
