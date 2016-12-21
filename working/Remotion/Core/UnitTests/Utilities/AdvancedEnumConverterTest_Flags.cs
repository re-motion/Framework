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

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class AdvancedEnumConverterTest_Flags
  {
    private AdvancedEnumConverter _int32EnumConverter;
    private AdvancedEnumConverter _int16EnumConverter;
    private AdvancedEnumConverter _nullableInt32EnumConverter;

    [Flags]
    public enum Int32Enum: int
    {
      Value0 = 0,
      Value1 = 1,
      Value2 = 2,
      Value4 = 4,
      Value5 = Value1 | Value4,
    }

    [Flags]
    public enum Int16Enum : short
    {
      Value0 = 0,
      Value1 = 1
    }

    [SetUp]
    public void SetUp()
    {
      _int32EnumConverter = new AdvancedEnumConverter (typeof (Int32Enum));
      _int16EnumConverter = new AdvancedEnumConverter (typeof (Int16Enum));
      _nullableInt32EnumConverter = new AdvancedEnumConverter (typeof (Int32Enum?));
    }

    [Test]
    public void CanConvertFromString()
    {
      Assert.That (_int32EnumConverter.CanConvertFrom (typeof (string)), Is.True);
      Assert.That (_nullableInt32EnumConverter.CanConvertFrom (typeof (string)), Is.True);
    }

    [Test]
    public void CanConvertToString()
    {
      Assert.That (_int32EnumConverter.CanConvertTo (typeof (string)), Is.True);
      Assert.That (_nullableInt32EnumConverter.CanConvertTo (typeof (string)), Is.True);
    }

    [Test]
    public void CanConvertFromNumeric()
    {
      Assert.That (_int32EnumConverter.CanConvertFrom (typeof (Int32)), Is.True);
      Assert.That (_int16EnumConverter.CanConvertFrom (typeof (Int16)), Is.True);
      Assert.That (_nullableInt32EnumConverter.CanConvertFrom (typeof (Int32?)), Is.True);
      Assert.That (_nullableInt32EnumConverter.CanConvertFrom (typeof (Int32)), Is.True);

      Assert.That (_int32EnumConverter.CanConvertFrom (typeof (Int16)), Is.False);
      Assert.That (_int16EnumConverter.CanConvertFrom (typeof (Int32)), Is.False);
      Assert.That (_int32EnumConverter.CanConvertFrom (typeof (Int32?)), Is.False);
      Assert.That (_nullableInt32EnumConverter.CanConvertFrom (typeof (Int16?)), Is.False);
    }

    [Test]
    public void CanConvertToNumeric()
    {
      Assert.That (_int32EnumConverter.CanConvertTo (typeof (Int32)), Is.True);
      Assert.That (_int16EnumConverter.CanConvertTo (typeof (Int16)), Is.True);
      Assert.That (_nullableInt32EnumConverter.CanConvertTo (typeof (Int32?)), Is.True);
      Assert.That (_int32EnumConverter.CanConvertTo (typeof (Int32?)), Is.True);

      Assert.That (_int32EnumConverter.CanConvertTo (typeof (Int16)), Is.False);
      Assert.That (_int16EnumConverter.CanConvertTo (typeof (Int32)), Is.False);
      Assert.That (_nullableInt32EnumConverter.CanConvertTo (typeof (Int32)), Is.False);
      Assert.That (_nullableInt32EnumConverter.CanConvertTo (typeof (Int16?)), Is.False);
    }

    [Test]
    public void ConvertFromString()
    {
      Assert.That (_int32EnumConverter.ConvertFrom ("Value0"), Is.EqualTo (Int32Enum.Value0));
      Assert.That (_int32EnumConverter.ConvertFrom ("Value1"), Is.EqualTo (Int32Enum.Value1));
      Assert.That (_int32EnumConverter.ConvertFrom ("Value2"), Is.EqualTo (Int32Enum.Value2));
      Assert.That (_int32EnumConverter.ConvertFrom ("Value5"), Is.EqualTo (Int32Enum.Value5));
      Assert.That (_int32EnumConverter.ConvertFrom ("Value1, Value2"), Is.EqualTo (Int32Enum.Value1 | Int32Enum.Value2));

      Assert.That (_nullableInt32EnumConverter.ConvertFrom ("Value1"), Is.EqualTo (Int32Enum.Value1));
      Assert.That (_nullableInt32EnumConverter.ConvertFrom ("Value5"), Is.EqualTo (Int32Enum.Value5));
      Assert.That (_nullableInt32EnumConverter.ConvertFrom ("Value1, Value2"), Is.EqualTo (Int32Enum.Value1 | Int32Enum.Value2));
      Assert.That (_nullableInt32EnumConverter.ConvertFrom (null), Is.EqualTo (null));
      Assert.That (_nullableInt32EnumConverter.ConvertFrom (string.Empty), Is.EqualTo (null));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ConvertFromString_WithNullAndInt32EnumConverter ()
    {
      _int32EnumConverter.ConvertFrom (null);
    }

    [Test]
    [ExpectedException (typeof (FormatException))]
    public void ConvertFromString_WithEmptyStringAndInt32EnumConverter ()
    {
      _int32EnumConverter.ConvertFrom (string.Empty);
    }

    [Test]
    public void ConvertToString()
    {
      Type destinationType = typeof (string);

      Assert.That (_int32EnumConverter.ConvertTo (Int32Enum.Value0, destinationType), Is.EqualTo ("Value0"));
      Assert.That (_int32EnumConverter.ConvertTo (Int32Enum.Value1, destinationType), Is.EqualTo ("Value1"));
      Assert.That (_int32EnumConverter.ConvertTo (Int32Enum.Value2, destinationType), Is.EqualTo ("Value2"));
      Assert.That (_int32EnumConverter.ConvertTo (Int32Enum.Value5, destinationType), Is.EqualTo ("Value5"));
      Assert.That (_int32EnumConverter.ConvertTo (Int32Enum.Value1 | Int32Enum.Value2, destinationType), Is.EqualTo ("Value1, Value2"));

      Assert.That (_nullableInt32EnumConverter.ConvertTo (Int32Enum.Value1, destinationType), Is.EqualTo ("Value1"));
      Assert.That (_nullableInt32EnumConverter.ConvertTo (Int32Enum.Value5, destinationType), Is.EqualTo ("Value5"));
      Assert.That (_nullableInt32EnumConverter.ConvertTo (Int32Enum.Value1 | Int32Enum.Value2, destinationType), Is.EqualTo ("Value1, Value2"));
      Assert.That (_nullableInt32EnumConverter.ConvertTo (null, destinationType), Is.EqualTo (string.Empty));
    }

    [Test]
    public void ConvertFromInt32()
    {
      Assert.That (_int32EnumConverter.ConvertFrom (0), Is.EqualTo (Int32Enum.Value0));
      Assert.That (_int32EnumConverter.ConvertFrom (1), Is.EqualTo (Int32Enum.Value1));
      Assert.That (_int32EnumConverter.ConvertFrom (2), Is.EqualTo (Int32Enum.Value2));
      Assert.That (_int32EnumConverter.ConvertFrom (5), Is.EqualTo (Int32Enum.Value5));
      Assert.That (_int32EnumConverter.ConvertFrom (3), Is.EqualTo (Int32Enum.Value1 | Int32Enum.Value2));
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException), ExpectedMessage =
        "The value -1 is not supported for enumeration 'Remotion.UnitTests.Utilities.AdvancedEnumConverterTest_Flags+Int32Enum'.")]
    public void ConvertFromInt32_WithUndefinedValue()
    {
      _int32EnumConverter.ConvertFrom (-1);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ConvertFromInt32_WithInvalidDataType()
    {
      _int32EnumConverter.ConvertFrom ((short) -1);
    }

    [Test]
    public void ConvertToInt32()
    {
      Type destinationType = typeof (Int32);

      Assert.That (_int32EnumConverter.ConvertTo (Int32Enum.Value0, destinationType), Is.EqualTo (0));
      Assert.That (_int32EnumConverter.ConvertTo (Int32Enum.Value1, destinationType), Is.EqualTo (1));
      Assert.That (_int32EnumConverter.ConvertTo (Int32Enum.Value2, destinationType), Is.EqualTo (2));
      Assert.That (_int32EnumConverter.ConvertTo (Int32Enum.Value5, destinationType), Is.EqualTo (5));
      Assert.That (_int32EnumConverter.ConvertTo (Int32Enum.Value1|Int32Enum.Value2, destinationType), Is.EqualTo (3));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ConvertToInt32_WithNullableInt32EnumConverter()
    {
      Assert.That (_nullableInt32EnumConverter.ConvertTo (Int32Enum.Value1, typeof (Int32)), Is.EqualTo (1));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ConvertToInt32_WithInt16EnumConverter ()
    {
      Assert.That (_int16EnumConverter.ConvertTo (Int32Enum.Value1, typeof (Int32)), Is.EqualTo (1));
    }

    [Test]
    public void ConvertFromNullableInt32()
    {
      Assert.That (_nullableInt32EnumConverter.ConvertFrom (0), Is.EqualTo (Int32Enum.Value0));
      Assert.That (_nullableInt32EnumConverter.ConvertFrom (1), Is.EqualTo (Int32Enum.Value1));
      Assert.That (_nullableInt32EnumConverter.ConvertFrom (2), Is.EqualTo (Int32Enum.Value2));
      Assert.That (_nullableInt32EnumConverter.ConvertFrom (5), Is.EqualTo (Int32Enum.Value5));
      Assert.That (_nullableInt32EnumConverter.ConvertFrom (3), Is.EqualTo (Int32Enum.Value1|Int32Enum.Value2));
      Assert.That (_nullableInt32EnumConverter.ConvertFrom (null), Is.Null);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ConvertFromNullableInt32_WithInt32EnumConverter ()
    {
      _int32EnumConverter.ConvertFrom ((int?) null);
    }

    [Test]
    public void ConvertToNullableInt32()
    {
      Type destinationType = typeof (Int32?);

      Assert.That (_nullableInt32EnumConverter.ConvertTo (Int32Enum.Value0, destinationType), Is.EqualTo (0));
      Assert.That (_nullableInt32EnumConverter.ConvertTo (Int32Enum.Value1, destinationType), Is.EqualTo (1));
      Assert.That (_nullableInt32EnumConverter.ConvertTo (Int32Enum.Value2, destinationType), Is.EqualTo (2));
      Assert.That (_nullableInt32EnumConverter.ConvertTo (Int32Enum.Value5, destinationType), Is.EqualTo (5));
      Assert.That (_nullableInt32EnumConverter.ConvertTo (Int32Enum.Value1 | Int32Enum.Value2, destinationType), Is.EqualTo (3));
      Assert.That (_nullableInt32EnumConverter.ConvertTo (null, destinationType), Is.Null);
      Assert.That (_int32EnumConverter.ConvertTo (Int32Enum.Value0, destinationType), Is.EqualTo (0));
    }

    [Test]
    public void ConvertFromInt16()
    {
      Assert.That (_int16EnumConverter.ConvertFrom ((Int16) 0), Is.EqualTo (Int16Enum.Value0));
      Assert.That (_int16EnumConverter.ConvertFrom ((Int16) 1), Is.EqualTo (Int16Enum.Value1));
    }

    [Test]
    public void ConvertToInt16()
    {
      Type destinationType = typeof (Int16);

      Assert.That (_int16EnumConverter.ConvertTo (Int16Enum.Value0, destinationType), Is.EqualTo ((Int16) 0));
      Assert.That (_int16EnumConverter.ConvertTo (Int16Enum.Value1, destinationType), Is.EqualTo ((Int16) 1));
    }
  }
}
