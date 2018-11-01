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
  [AttributeWithPropertyParams (
      1,
      "1",
      null,
      typeof (object),
      new int[] { 2, 3 },
      new string[] { "2", "3" },
      new object[] { null, "foo", typeof (object) }, new Type[] { typeof (string), typeof (int), typeof (double) },
      INamed = 5,
      SNamed = "P5",
      ONamed = "Pbla",
      TNamed = typeof (float),
      INamedArray = new int[] { 1, 2, 3 },
      SNamedArray = new string[] { "P1", null, "P2" },
      ONamedArray = new object[] { 1, 2, null },
      TNamedArray = new Type[] { typeof (Random), null }
      )]
  public class TestAttributeApplicationWithCtorArgumentsAndNamedProperties
  {
  }
}
