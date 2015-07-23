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
  public class AdvancedEnumConverterTest
  {
    private AdvancedEnumConverter _int32EnumConverter;
    private AdvancedEnumConverter _int16EnumConverter;
    private AdvancedEnumConverter _nullableInt32EnumConverter;

    // Explicitly declare an Int32 enum
    // ReSharper disable EnumUnderlyingTypeIsInt
    public enum Int32Enum: int
    {
      ValueA = 0,
      ValueB = 1
    }
    // ReSharper restore EnumUnderlyingTypeIsInt

    public enum Int16Enum: short
    {
      ValueA = 0,
      ValueB = 1
    }

    [SetUp]
    public void SetUp()
    {
      _int32EnumConverter = new AdvancedEnumConverter (typeof (Int32Enum));
      _int16EnumConverter = new AdvancedEnumConverter (typeof (Int16Enum));
      _nullableInt32EnumConverter = new AdvancedEnumConverter (typeof (Int32Enum?));
    }

    [Test]
    public void EnumType ()
    {
      Assert.That (_int32EnumConverter.EnumType, Is.SameAs (typeof (Int32Enum)));
      Assert.That (_int16EnumConverter.EnumType, Is.SameAs (typeof (Int16Enum)));
      Assert.That (_nullableInt32EnumConverter.EnumType, Is.SameAs (typeof (Int32Enum?)));
    }

    [Test]
    public void UnderlyingEnumType ()
    {
      Assert.That (_int32EnumConverter.UnderlyingEnumType, Is.SameAs (typeof (Int32Enum)));
      Assert.That (_int16EnumConverter.UnderlyingEnumType, Is.SameAs (typeof (Int16Enum)));
      Assert.That (_nullableInt32EnumConverter.UnderlyingEnumType, Is.SameAs (typeof (Int32Enum)));
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
      Assert.That (_int32EnumConverter.ConvertFrom ("ValueA"), Is.EqualTo (Int32Enum.ValueA));
      Assert.That (_int32EnumConverter.ConvertFrom ("ValueB"), Is.EqualTo (Int32Enum.ValueB));
      Assert.That (_nullableInt32EnumConverter.ConvertFrom ("ValueA"), Is.EqualTo (Int32Enum.ValueA));
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

      Assert.That (_int32EnumConverter.ConvertTo (Int32Enum.ValueA, destinationType), Is.EqualTo ("ValueA"));
      Assert.That (_int32EnumConverter.ConvertTo (Int32Enum.ValueB, destinationType), Is.EqualTo ("ValueB"));
      Assert.That (_nullableInt32EnumConverter.ConvertTo (Int32Enum.ValueA, destinationType), Is.EqualTo ("ValueA"));
      Assert.That (_nullableInt32EnumConverter.ConvertTo (null, destinationType), Is.EqualTo (string.Empty));
    }

    [Test]
    public void ConvertFromInt32()
    {
      Assert.That (_int32EnumConverter.ConvertFrom (0), Is.EqualTo (Int32Enum.ValueA));
      Assert.That (_int32EnumConverter.ConvertFrom (1), Is.EqualTo (Int32Enum.ValueB));
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException), ExpectedMessage =
        "The value -1 is not supported for enumeration 'Remotion.UnitTests.Utilities.AdvancedEnumConverterTest+Int32Enum'.")]
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

      Assert.That (_int32EnumConverter.ConvertTo (Int32Enum.ValueA, destinationType), Is.EqualTo (0));
      Assert.That (_int32EnumConverter.ConvertTo (Int32Enum.ValueB, destinationType), Is.EqualTo (1));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ConvertToInt32_WithNullableInt32EnumConverter()
    {
      Assert.That (_nullableInt32EnumConverter.ConvertTo (Int32Enum.ValueB, typeof (Int32)), Is.EqualTo (1));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ConvertToInt32_WithInt16EnumConverter ()
    {
      Assert.That (_int16EnumConverter.ConvertTo (Int32Enum.ValueB, typeof (Int32)), Is.EqualTo (1));
    }

    [Test]
    public void ConvertFromNullableInt32()
    {
      Assert.That (_nullableInt32EnumConverter.ConvertFrom (0), Is.EqualTo (Int32Enum.ValueA));
      Assert.That (_nullableInt32EnumConverter.ConvertFrom (1), Is.EqualTo (Int32Enum.ValueB));
      Assert.That (_nullableInt32EnumConverter.ConvertFrom (null), Is.Null);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ConvertFromNullableInt32_WithInt32EnumConverter ()
    {
      _int32EnumConverter.ConvertFrom (null);
    }

    [Test]
    public void ConvertToNullableInt32()
    {
      Type destinationType = typeof (Int32?);

      Assert.That (_nullableInt32EnumConverter.ConvertTo (Int32Enum.ValueA, destinationType), Is.EqualTo (0));
      Assert.That (_nullableInt32EnumConverter.ConvertTo (Int32Enum.ValueB, destinationType), Is.EqualTo (1));
      Assert.That (_nullableInt32EnumConverter.ConvertTo (null, destinationType), Is.Null);
      Assert.That (_int32EnumConverter.ConvertTo (Int32Enum.ValueA, destinationType), Is.EqualTo (0));
    }

    [Test]
    public void ConvertFromInt16()
    {
      Assert.That (_int16EnumConverter.ConvertFrom ((Int16) 0), Is.EqualTo (Int16Enum.ValueA));
      Assert.That (_int16EnumConverter.ConvertFrom ((Int16) 1), Is.EqualTo (Int16Enum.ValueB));
    }

    [Test]
    public void ConvertToInt16()
    {
      Type destinationType = typeof (Int16);

      Assert.That (_int16EnumConverter.ConvertTo (Int16Enum.ValueA, destinationType), Is.EqualTo ((Int16) 0));
      Assert.That (_int16EnumConverter.ConvertTo (Int16Enum.ValueB, destinationType), Is.EqualTo ((Int16) 1));
    }
  }
}
