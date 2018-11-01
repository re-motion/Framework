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
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.EnumUtilityTests
{
  [TestFixture]
  public class Common
  {
    public enum TestEnumWithDuplicateValues
    {
      ValueA = 1,
      ValueB = 1
   }

    [Flags]
    public enum TestFlagsWithoutZero
    {
      Flag1 = 1
    }

    [Flags]
    public enum TestFlags
    {
      Flag0 = 0,
      Flag1 = 1,
      Flag4 = 4,
      Flag8 = 8,
      Flag1AndFlag8 = Flag1 | Flag8, //01001
      Flag4AndFlag8 = Flag4 | Flag8, //01100
      Flag17 = 17, // 10001
      Flag18 = 18, // 10010
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Argument was a type representing 'System.Int32' but only enum-types are supported.\r\nParameter name: enumType")]
    public void IsFlagsEnum_WithOtherType ()
    {
      EnumUtility.IsFlagsEnumType (typeof (int));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Object must be the same type as the enum. The type passed in was 'Remotion.UnitTests.Utilities.EnumUtilityTests.Int16Enum+TestEnum'; "
        + "the enum type was 'Remotion.UnitTests.Utilities.EnumUtilityTests.Common+TestFlags'."
        + "\r\nParameter name: value")]
    public void IsValidEnumValue_WithValueNotMatchingType ()
    {
      EnumUtility.IsValidEnumValue (typeof (TestFlags), (Int16Enum.TestEnum) (short) 1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Enum underlying type and the object must be same type. The type passed in was 'System.Int16'; the enum underlying type was 'System.Int32'."
        + "\r\nParameter name: value")]
    public void IsValidEnumValue_WithValueNotMatchingUnderlyingType ()
    {
      EnumUtility.IsValidEnumValue (typeof (TestFlags), (short) 1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Argument was of type 'System.Int16' but only enum-types are supported with this overload.\r\nParameter name: enumValue")]
    public void IsValidEnumValue_WithValueOfOtherType ()
    {
      EnumUtility.IsValidEnumValue ((short) 1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Argument was a type representing 'System.Int32' but only enum-types are supported.\r\nParameter name: enumType")]
    public void IsValidEnumValue_WithOtherType ()
    {
      EnumUtility.IsValidEnumValue (typeof (Int32), (short) 1);
    }

    [Test]
    public void IsValidEnumValue_DuplicateValue ()
    {
      Assert.That (EnumUtility.IsValidEnumValue (TestEnumWithDuplicateValues.ValueA), Is.True);
      Assert.That (EnumUtility.IsValidEnumValue (TestEnumWithDuplicateValues.ValueB), Is.True);
    }

    [Test]
    public void IsValidEnumValue_FlagCombinations()
    {
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlagsWithoutZero) 0), Is.False);
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlagsWithoutZero) 1), Is.True);
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlagsWithoutZero) 2), Is.False);

      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 00), Is.True,  "00000");
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 01), Is.True,  "00001");
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 02), Is.False, "00010");
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 03), Is.False, "00011");

      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 04), Is.True,  "00100");
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 05), Is.True,  "00101");
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 06), Is.False, "00110");
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 07), Is.False, "00111");

      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 08), Is.True,  "01000");
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 09), Is.True,  "01001");
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 10), Is.False, "01010");
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 11), Is.False, "01010");

      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 12), Is.True,  "01100");
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 13), Is.True,  "01101");
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 14), Is.False, "01110");
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 15), Is.False, "01111");

      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 16), Is.False, "10000");
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 17), Is.True,  "10001");
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 18), Is.True,  "10010");
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 19), Is.True,  "10011");

      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 20), Is.False, "10100");
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 21), Is.True,  "10101");
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 22), Is.True,  "10110");
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 23), Is.True,  "10111");

      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 24), Is.False, "11000");
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 25), Is.True,  "11001");
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 26), Is.True,  "11010");
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 27), Is.True,  "11011");

      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 28), Is.False, "11100");
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 29), Is.True,  "11101");
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 30), Is.True,  "11110");
      Assert.That (EnumUtility.IsValidEnumValue ((TestFlags) 31), Is.True,  "11111");
    }

    [Test]
    [Explicit]
    public void PerformanceTest ()
    {
      int loops = 10000;
      var enumValues =
          new Enum[]
          {
              ByteEnum.TestEnum.Zero, Int16Enum.TestEnum.Positive, Int32Enum.TestEnum.Negative, Int64Enum.TestEnum.Negative,
              SByteEnum.TestEnum.Zero, UInt16Enum.TestEnum.Positive, UInt32Enum.TestEnum.Zero, UInt64Enum.TestEnum.Positive
          };

      var flagEnumValues =
          new Enum[]
          {
              ByteEnum.TestFlags.Flag1, Int16Enum.TestFlags.Flag2, Int32Enum.TestFlags.Flag3, Int64Enum.TestFlags.Flag1and2,
              SByteEnum.TestFlags.Flag3, UInt16Enum.TestFlags.AllFlags, UInt32Enum.TestFlags.Flag1, UInt64Enum.TestFlags.Flag1
          };

      foreach (var enumValue in enumValues.Concat (flagEnumValues))
        EnumUtility.IsValidEnumValue (enumValue);

      Stopwatch byStringFromEnumStopWatch = new Stopwatch();
      byStringFromEnumStopWatch.Start();
      for (int i = 0; i < loops; i++)
      {
        foreach (var enumValue in enumValues)
          IsValidEnumValueByString (enumValue);
      }
      byStringFromEnumStopWatch.Stop();

      Stopwatch byStringFromFlagStopWatch = new Stopwatch();
      byStringFromFlagStopWatch.Start();
      for (int i = 0; i < loops; i++)
      {
        foreach (var enumValue in flagEnumValues)
          IsValidEnumValueByString (enumValue);
      }
      byStringFromFlagStopWatch.Stop();

      Stopwatch byValueFromEnumStopWatch = new Stopwatch();
      byValueFromEnumStopWatch.Start();
      for (int i = 0; i < loops; i++)
      {
        foreach (var enumValue in enumValues)
          EnumUtility.IsValidEnumValue (enumValue);
      }
      byValueFromEnumStopWatch.Stop();

      Stopwatch byValueFromFlagStopWatch = new Stopwatch();
      byValueFromFlagStopWatch.Start();
      for (int i = 0; i < loops; i++)
      {
        foreach (var enumValue in flagEnumValues)
          EnumUtility.IsValidEnumValue (enumValue);
      }
      byValueFromFlagStopWatch.Stop();

      Console.WriteLine ("ByString Enum: {0}", byStringFromEnumStopWatch.ElapsedMilliseconds);
      Console.WriteLine ("ByString Flag: {0}", byStringFromFlagStopWatch.ElapsedMilliseconds);
      Console.WriteLine ("ByValue Enum: {0}", byValueFromEnumStopWatch.ElapsedMilliseconds);
      Console.WriteLine ("ByValue Flag: {0}", byValueFromEnumStopWatch.ElapsedMilliseconds);
    }

    private bool IsValidEnumValueByString (object enumValue)
    {
      if (enumValue == null)
        throw new ArgumentNullException ("enumValue");

      string stringRepresentation = enumValue.ToString();
      if (string.IsNullOrEmpty (stringRepresentation))
        return false;
      char firstChar = stringRepresentation[0];
      return !(firstChar == '-' || char.IsDigit (firstChar));
    }
  }
}
