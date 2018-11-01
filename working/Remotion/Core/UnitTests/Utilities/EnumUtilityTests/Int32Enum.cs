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
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.EnumUtilityTests
{
  [TestFixture]
  public class Int32Enum
  {
    public enum TestEnum:int
    {
      Negative = -1,
      Zero = 0,
      Positive = 1
    }

    [Flags]
    public enum TestFlags:int
    {
      Flag1 = 1,
      Flag2 = 2,
      Flag3 = 4,
      Flag4 = 8,
      Flag1and2 = Flag1 | Flag2,
      AllFlags = Flag1and2 | Flag3 | Flag4
    }

    [Test]
    public void IsValidEnumValue_WithEnum_AndValueOutOfRange_Negative ()
    {
      Assert.That (EnumUtility.IsValidEnumValue ((TestEnum) (-3)), Is.False);
    }

    [Test]
    public void IsValidEnumValue_WithEnum_AndValueOutOfRange_Positive ()
    {
      Assert.That (EnumUtility.IsValidEnumValue ((TestEnum) 3), Is.False);
    }

    [Test]
    public void IsValidEnumValue_WithEnum_AndValueDefinedAsNegative ()
    {
      Assert.That (EnumUtility.IsValidEnumValue (TestEnum.Negative), Is.True);
    }

    [Test]
    public void IsValidEnumValue_WithEnum_AndValueDefinedAsZero ()
    {
      Assert.That (EnumUtility.IsValidEnumValue (TestEnum.Zero), Is.True);
    }

    [Test]
    public void IsValidEnumValue_WithEnum_AndValueDefinedAsPositive ()
    {
      Assert.That (EnumUtility.IsValidEnumValue (TestEnum.Positive), Is.True);
    }

    [Test]
    public void IsValidEnumValue_WithTypeAndInt32 ()
    {
      Assert.That (EnumUtility.IsValidEnumValue (typeof (TestEnum), 1), Is.True);
    }

    [Test]
    public void IsValidEnumValue_WithTypeAndEnum()
    {
      Assert.That (EnumUtility.IsValidEnumValue (typeof (TestEnum), TestEnum.Positive), Is.True);
    }

    [Test]
    public void IsValidEnumValue_WithFlag_AndValueOutOfRange_Negative ()
    {
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) (-3)), Is.False);
    }

    [Test]
    public void IsValidEnumValue_WithFlag_AndValueOutOfRange_Zero ()
    {
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) (0)), Is.False);
    }

    [Test]
    public void IsValidEnumValue_WithFlag_AndValueOutOfRange_UndefinedBit ()
    {
      Assert.That (EnumUtility.IsValidEnumValue (TestFlags.Flag1 | ((TestFlags) 16)), Is.False);
    }

    [Test]
    public void IsValidEnumValue_WithFlag_AndFlagCombination ()
    {
      Assert.That (EnumUtility.IsValidEnumValue (TestFlags.Flag1 | TestFlags.Flag3), Is.True);
    }

    [Test]
    public void IsValidEnumValue_WithFlag_AndFlagCombination2 ()
    {
      Assert.That (EnumUtility.IsValidEnumValue (TestFlags.Flag1 | TestFlags.Flag2 | TestFlags.Flag3), Is.True);
    }

    [Test]
    public void IsFlagsEnumValue_NotFlag ()
    {
      Assert.That (EnumUtility.IsFlagsEnumValue (TestEnum.Zero), Is.False);
    }

    [Test]
    public void IsFlagsEnumValue_Flag ()
    {
      Assert.That (EnumUtility.IsFlagsEnumValue (TestFlags.Flag1), Is.True);
    }

    [Test]
    public void IsFlagsEnumType_NotFlag ()
    {
      Assert.That (EnumUtility.IsFlagsEnumType (typeof (TestEnum)), Is.False);
    }

    [Test]
    public void IsFlagsEnumType_Flag ()
    {
      Assert.That (EnumUtility.IsFlagsEnumType (typeof (TestFlags)), Is.True);
    }
  }
}
