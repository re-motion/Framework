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
using System.ComponentModel;
using NUnit.Framework;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class TypeConversionProviderTest
  {
    // ReSharper disable EnumUnderlyingTypeIsInt
    public enum Int32Enum : int
    {
      ValueA = 0,
      ValueB = 1
    }
    // ReSharper restore EnumUnderlyingTypeIsInt

    private ITypeConversionProvider _provider;
    private readonly Type _int32 = typeof (int);
    private readonly Type _nullableInt32 = typeof (int?);
    private readonly Type _string = typeof (string);
    private readonly Type _stringArray = typeof (string[]);
    private readonly Type _object = typeof (object);
    private readonly Type _guid = typeof (Guid);
    private readonly Type _int32Enum = typeof (Int32Enum);
    private readonly Type _nullableInt32Enum = typeof (Int32Enum?);

    [SetUp]
    public void SetUp ()
    {
      _provider = new TypeConversionProvider (SafeServiceLocator.Current.GetInstance<ITypeConverterFactory>());
    }

    [Test]
    [Obsolete]
    public void TestCurrent ()
    {
      Assert.That (TypeConversionProvider.Current, Is.SameAs (SafeServiceLocator.Current.GetInstance<ITypeConversionProvider>()));
    }

    [Test]
    public void CanConvert_FromInt32_ToInt32 ()
    {
      Assert.That (_provider.CanConvert (_int32, _int32), Is.True);
    }

    [Test]
    public void CanConvert_FromNullableInt32_ToNullableInt32 ()
    {
      Assert.That (_provider.CanConvert (_nullableInt32, _nullableInt32), Is.True);
    }

    [Test]
    public void CanConvert_FromInt32_ToNullableInt32 ()
    {
      Assert.That (_provider.CanConvert (_int32, _nullableInt32), Is.True);
    }

    [Test]
    public void CanConvert_FromNullableInt32_ToInt32 ()
    {
      Assert.That (_provider.CanConvert (_nullableInt32, _int32), Is.True);
    }

    [Test]
    public void CanConvert_FromInt32_ToString ()
    {
      Assert.That (_provider.CanConvert (_string, _int32), Is.True);
    }

    [Test]
    public void CanConvert_FromString_ToInt32 ()
    {
      Assert.That (_provider.CanConvert (_int32, _string), Is.True);
    }

    [Test]
    public void CanConvert_FromNullableInt32_ToString ()
    {
      Assert.That (_provider.CanConvert (_string, _nullableInt32), Is.True);
    }

    [Test]
    public void CanConvert_FromString_ToNullableInt32 ()
    {
      Assert.That (_provider.CanConvert (_nullableInt32, _string), Is.True);
    }

    [Test]
    public void CanConvert_FromObject_ToNullableInt32 ()
    {
      Assert.That (_provider.CanConvert (_object, _nullableInt32), Is.False);
    }

    [Test]
    public void CanConvert_FromGuid_ToString ()
    {
      Assert.That (_provider.CanConvert (_guid, _string), Is.True);
    }

    [Test]
    public void CanConvert_FromString_ToGuid ()
    {
      Assert.That (_provider.CanConvert (_string, _guid), Is.True);
    }

    [Test]
    public void CanConvert_FromStringArray_ToString ()
    {
      Assert.That (_provider.CanConvert (_stringArray, _string), Is.True);
    }

    [Test]
    public void CanConvert_FromString_ToStringArray ()
    {
      Assert.That (_provider.CanConvert (_string, _stringArray), Is.True);
    }


    [Test]
    public void CanConvert_FromInt32Enum_ToInt32Enum ()
    {
      Assert.That (_provider.CanConvert (_int32Enum, _int32Enum), Is.True);
    }

    [Test]
    public void CanConvert_FromInt32Enum_ToInt32 ()
    {
      Assert.That (_provider.CanConvert (_int32Enum, _int32), Is.True);
    }

    [Test]
    public void CanConvert_FromInt32_ToInt32Enum ()
    {
      Assert.That (_provider.CanConvert (_int32, _int32Enum), Is.True);
    }

    [Test]
    public void CanConvert_FromInt32Enum_ToString ()
    {
      Assert.That (_provider.CanConvert (_int32Enum, _string), Is.True);
    }

    [Test]
    public void CanConvert_FromString_ToInt32Enum ()
    {
      Assert.That (_provider.CanConvert (_string, _int32Enum), Is.True);
    }


    [Test]
    public void CanConvert_FromNullableInt32Enum_ToNullableInt32Enum ()
    {
      Assert.That (_provider.CanConvert (_nullableInt32Enum, _nullableInt32Enum), Is.True);
    }

    [Test]
    public void CanConvert_FromNullableInt32Enum_ToNullableInt32 ()
    {
      Assert.That (_provider.CanConvert (_nullableInt32Enum, _nullableInt32), Is.True);
    }

    [Test]
    public void CanConvert_FromNullableInt32_ToNullableInt32Enum ()
    {
      Assert.That (_provider.CanConvert (_nullableInt32, _nullableInt32Enum), Is.True);
    }

    [Test]
    public void CanConvert_FromInt32Enum_ToNullableInt32 ()
    {
      Assert.That (_provider.CanConvert (_int32Enum, _nullableInt32), Is.True);
    }

    [Test]
    public void CanConvert_FromInt32_ToNullableInt32Enum ()
    {
      Assert.That (_provider.CanConvert (_int32, _nullableInt32Enum), Is.True);
    }

    [Test]
    public void CanConvert_FromNullableInt32Enum_ToString ()
    {
      Assert.That (_provider.CanConvert (_nullableInt32Enum, _string), Is.True);
    }

    [Test]
    public void CanConvert_FromString_ToNullableInt32Enum ()
    {
      Assert.That (_provider.CanConvert (_string, _nullableInt32Enum), Is.True);
    }

    [Test]
    public void CanConvert_FromDBNull_ToNullableInt32 ()
    {
      Assert.That (_provider.CanConvert (typeof (DBNull), _nullableInt32), Is.True);
    }

    [Test]
    public void CanConvert_FromDBNull_ToInt32 ()
    {
      Assert.That (_provider.CanConvert (typeof (DBNull), _int32), Is.False);
    }

    [Test]
    public void Convert_FromInt32_ToInt32 ()
    {
      Assert.That (_provider.Convert (_int32, _int32, 1), Is.EqualTo (1));
    }

    [Test]
    public void Convert_FromNullableInt32_ToNullableInt32 ()
    {
      Assert.That (_provider.Convert (_nullableInt32, _nullableInt32, (int?) 1), Is.EqualTo ((int?) 1));
    }

    [Test]
    public void Convert_FromNullableInt32_ToInt32 ()
    {
      Assert.That (_provider.Convert (_nullableInt32, _int32, 1), Is.EqualTo (1));
    }

    [Test]
    public void Convert_FromInt32_ToNullableInt32 ()
    {
      Assert.That (_provider.Convert (_int32, _nullableInt32, 1), Is.EqualTo (1));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Convert_FromObject_ToNullableInt32 ()
    {
      _provider.Convert (_object, _nullableInt32, new object());
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Convert_FromNullableInt32_ToObject ()
    {
      _provider.Convert (_nullableInt32, _object, 1);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Convert_FromInt32_ToObject ()
    {
      _provider.Convert (_int32, _object, 1);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Convert_FromObject_ToInt32 ()
    {
      _provider.Convert (_object, _int32, new object());
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Convert_FromInt64_ToInt32 ()
    {
      _provider.Convert (_object, _int32, 1L);
    }

    [Test]
    public void Convert_FromNullableInt32_ToString ()
    {
      Assert.That (_provider.Convert (_nullableInt32, _string, 1), Is.EqualTo ("1"));
    }

    [Test]
    public void Convert_FromString_ToNullableInt32 ()
    {
      Assert.That (_provider.Convert (_string, _nullableInt32, "1"), Is.EqualTo (1));
    }

    [Test]
    public void Convert_FromNullableInt32_ToString_WithNull ()
    {
      Assert.That (_provider.Convert (_nullableInt32, _string, null), Is.EqualTo (""));
    }

    [Test]
    public void Convert_FromString_ToNullableInt32_WithEmpty ()
    {
      Assert.That (_provider.Convert (_string, _nullableInt32, ""), Is.EqualTo (null));
    }

    [Test]
    public void Convert_FromInt32_ToNullableInt32_WithNull ()
    {
      Assert.That (_provider.Convert (_int32, _nullableInt32, null), Is.EqualTo (null));
    }

    [Test]
    public void Convert_FromInt32_ToNullableInt32_WithDBNull ()
    {
      Assert.That (_provider.Convert (_int32, _nullableInt32, DBNull.Value), Is.EqualTo (null));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'value' has type 'System.DBNull' when type 'System.Int32' was expected.\r\nParameter name: value")]
    public void Convert_FromInt32_ToInt32_WithDBNull ()
    {
      _provider.Convert (_int32, _int32, DBNull.Value);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'value' has type 'System.String' when type 'System.Int32' was expected.\r\nParameter name: value")]
    public void Convert_WithInvalidValue ()
    {
      _provider.Convert (_int32, _nullableInt32, "pwned!");
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot convert value 'null' to non-nullable type 'System.Int32'.")]
    public void Convert_WithInvalidNullValue ()
    {
      _provider.Convert (_int32, _int32, null);
    }

    [Test]
    public void Convert_WithValidNullValue ()
    {
      _provider.Convert (_int32, _nullableInt32, null);
    }

    [Test]
    public void Convert_FromInt32_ToString ()
    {
      Assert.That (_provider.Convert (_int32, _string, 1), Is.EqualTo ("1"));
    }

    [Test]
    public void Convert_FromString_ToInt32 ()
    {
      Assert.That (_provider.Convert (_string, _int32, "1"), Is.EqualTo (1));
    }

    [Test]
    public void Convert_FromInt32_ToString_WithNull ()
    {
      Assert.That (_provider.Convert (_int32, _string, null), Is.EqualTo (string.Empty));
    }

    [Test]
    public void Convert_FromInt32_ToString_WithDBNull ()
    {
      Assert.That (_provider.Convert (_int32, _string, DBNull.Value), Is.EqualTo (string.Empty));
    }

    [Test]
    [ExpectedException (typeof (ParseException))]
    public void Convert_FromString_ToInt32_WithEmpty ()
    {
      _provider.Convert (_string, _int32, string.Empty);
    }


    [Test]
    public void Convert_FromInt32Enum_ToInt32Enum ()
    {
      Assert.That (_provider.Convert (_int32Enum, _int32Enum, Int32Enum.ValueA), Is.EqualTo (Int32Enum.ValueA));
    }

    [Test]
    public void Convert_FromInt32Enum_ToInt32 ()
    {
      Assert.That (_provider.Convert (_int32Enum, _int32, Int32Enum.ValueA), Is.EqualTo (0));
      Assert.That (_provider.Convert (_int32Enum, _int32, Int32Enum.ValueB), Is.EqualTo (1));
    }

    [Test]
    public void Convert_FromInt32_ToInt32Enum ()
    {
      Assert.That (_provider.Convert (_int32, _int32Enum, 0), Is.EqualTo (Int32Enum.ValueA));
      Assert.That (_provider.Convert (_int32, _int32Enum, 1), Is.EqualTo (Int32Enum.ValueB));
    }

    [Test]
    public void Convert_FromInt32Enum_ToString ()
    {
      Assert.That (_provider.Convert (_int32Enum, _string, Int32Enum.ValueA), Is.EqualTo ("ValueA"));
      Assert.That (_provider.Convert (_int32Enum, _string, Int32Enum.ValueB), Is.EqualTo ("ValueB"));
    }

    [Test]
    public void Convert_FromString_ToInt32Enum ()
    {
      Assert.That (_provider.Convert (_string, _int32Enum, "ValueA"), Is.EqualTo (Int32Enum.ValueA));
      Assert.That (_provider.Convert (_string, _int32Enum, "ValueB"), Is.EqualTo (Int32Enum.ValueB));
    }

    [Test]
    public void Convert_FromInt32Enum_ToString_WithNull ()
    {
      Assert.That (_provider.Convert (_int32Enum, _string, null), Is.EqualTo (string.Empty));
    }

    [Test]
    public void Convert_FromInt32Enum_ToString_WithDBNull ()
    {
      Assert.That (_provider.Convert (_int32Enum, _string, DBNull.Value), Is.EqualTo (string.Empty));
    }

    [Test]
    [ExpectedException (typeof (FormatException), ExpectedMessage = " is not a valid value for Int32Enum.")]
    public void Convert_FromString_ToInt32Enum_WithEmpty ()
    {
      _provider.Convert (_string, _int32Enum, string.Empty);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Convert_FromInt32_ToInt32Enum_WithNull ()
    {
      _provider.Convert (_int32, _int32Enum, null);
    }

    [Test]
    public void Convert_FromNullableInt32Enum_ToNullableInt32Enum ()
    {
      Assert.That (_provider.Convert (_nullableInt32Enum, _nullableInt32Enum, Int32Enum.ValueA), Is.EqualTo (Int32Enum.ValueA));
    }

    [Test]
    public void Convert_FromNullableInt32Enum_ToNullableInt32 ()
    {
      Assert.That (_provider.Convert (_nullableInt32Enum, _nullableInt32, Int32Enum.ValueA), Is.EqualTo (0));
      Assert.That (_provider.Convert (_nullableInt32Enum, _nullableInt32, Int32Enum.ValueB), Is.EqualTo (1));
      Assert.That (_provider.Convert (_nullableInt32Enum, _nullableInt32, null), Is.Null);
    }

    [Test]
    public void Convert_FromNullableInt32_ToNullableInt32Enum ()
    {
      Assert.That (_provider.Convert (_nullableInt32, _nullableInt32Enum, 0), Is.EqualTo (Int32Enum.ValueA));
      Assert.That (_provider.Convert (_nullableInt32, _nullableInt32Enum, 1), Is.EqualTo (Int32Enum.ValueB));
      Assert.That (_provider.Convert (_nullableInt32, _nullableInt32Enum, null), Is.Null);
    }

    [Test]
    public void Convert_FromNullableInt32Enum_ToString ()
    {
      Assert.That (_provider.Convert (_nullableInt32Enum, _string, Int32Enum.ValueA), Is.EqualTo ("ValueA"));
      Assert.That (_provider.Convert (_nullableInt32Enum, _string, Int32Enum.ValueB), Is.EqualTo ("ValueB"));
      Assert.That (_provider.Convert (_nullableInt32Enum, _string, null), Is.EqualTo (string.Empty));
    }

    [Test]
    public void Convert_FromString_ToNullableInt32Enum ()
    {
      Assert.That (_provider.Convert (_string, _nullableInt32Enum, "ValueA"), Is.EqualTo (Int32Enum.ValueA));
      Assert.That (_provider.Convert (_string, _nullableInt32Enum, "ValueB"), Is.EqualTo (Int32Enum.ValueB));
      Assert.That (_provider.Convert (_string, _nullableInt32Enum, null), Is.Null);
      Assert.That (_provider.Convert (_string, _nullableInt32Enum, string.Empty), Is.Null);
    }

    [Test]
    public void Convert_FromNullableInt32Enum_ToString_WithDBNull ()
    {
      Assert.That (_provider.Convert (_nullableInt32Enum, _string, DBNull.Value), Is.EqualTo (string.Empty));
    }


    [Test]
    public void Convert_FromString_ToString ()
    {
      string value = "Hello World!";
      Assert.That (_provider.Convert (_string, _string, value), Is.EqualTo (value));
    }

    [Test]
    public void Convert_FromStringArray_ToString ()
    {
      var value = new[] { "Hello", "World", "!" };
      Assert.That (_provider.Convert (_stringArray, _string, value), Is.EqualTo ("Hello,World,!"));
    }

    [Test]
    public void Convert_FromString_ToStringArray ()
    {
      string value = "Hello,World,!";
      Assert.That (_provider.Convert (_string, _stringArray, value), Is.EqualTo (new[] { "Hello", "World", "!" }));
    }

    [Test]
    public void Convert_FromString_ToString_WithNull ()
    {
      Assert.That (_provider.Convert (_string, _string, null), Is.EqualTo (string.Empty));
    }

    [Test]
    public void GetTypeConverter_FromInt32_ToInt32 ()
    {
      TypeConverterResult converterResult = _provider.GetTypeConverter (_int32, _int32);
      Assert.That (converterResult, Is.EqualTo (TypeConverterResult.Empty), "TypeConverterResult is not empty.");
    }


    [Test]
    public void GetTypeConverter_FromInt32_ToNullableInt32 ()
    {
      TypeConverterResult converterResult = _provider.GetTypeConverter (_int32, _nullableInt32);
      Assert.That (converterResult, Is.EqualTo (TypeConverterResult.Empty), "TypeConverterResult is not empty.");
    }

    [Test]
    public void GetTypeConverter_FromNullableInt32_ToInt32 ()
    {
      TypeConverterResult converterResult = _provider.GetTypeConverter (_nullableInt32, _int32);
      Assert.That (converterResult, Is.EqualTo (TypeConverterResult.Empty), "TypeConverterResult is not empty.");
    }

    [Test]
    public void GetTypeConverter_FromNullableInt32_ToString ()
    {
      TypeConverterResult converterResult = _provider.GetTypeConverter (_nullableInt32, _string);
      Assert.That (converterResult, Is.Not.EqualTo (TypeConverterResult.Empty), "TypeConverterResult is not empty.");
      Assert.That (converterResult.TypeConverter.GetType(), Is.EqualTo (typeof (BidirectionalStringConverter)));
    }

    [Test]
    public void GetTypeConverter_FromString_ToNullableInt32 ()
    {
      TypeConverterResult converterResult = _provider.GetTypeConverter (_string, _nullableInt32);
      Assert.That (converterResult, Is.Not.EqualTo (TypeConverterResult.Empty), "TypeConverterResult is not empty.");
      Assert.That (converterResult.TypeConverter.GetType(), Is.EqualTo (typeof (BidirectionalStringConverter)));
    }

    [Test]
    public void GetTypeConverter_FromObject_ToString ()
    {
      TypeConverterResult converterResult = _provider.GetTypeConverter (_object, _string);
      Assert.That (converterResult, Is.EqualTo (TypeConverterResult.Empty), "TypeConverterResult is not empty.");
    }

    [Test]
    public void GetTypeConverter_FromString_ToObject ()
    {
      TypeConverterResult converterResult = _provider.GetTypeConverter (_string, _object);
      Assert.That (converterResult, Is.EqualTo (TypeConverterResult.Empty), "TypeConverterResult is not empty.");
    }

    [Test]
    public void GetTypeConverter_FromStringArray_ToString ()
    {
      TypeConverterResult converterResult = _provider.GetTypeConverter (_stringArray, _string);
      Assert.That (converterResult, Is.Not.EqualTo (TypeConverterResult.Empty), "TypeConverterResult is empty.");
      Assert.That (converterResult.TypeConverterType, Is.EqualTo (TypeConverterType.DestinationTypeConverter));
      Assert.That (converterResult.TypeConverter.GetType(), Is.EqualTo (typeof (BidirectionalStringConverter)));
    }

    [Test]
    public void GetTypeConverter_FromString_ToArray ()
    {
      TypeConverterResult converterResult = _provider.GetTypeConverter (_string, _stringArray);
      Assert.That (converterResult, Is.Not.EqualTo (TypeConverterResult.Empty), "TypeConverterResult is empty.");
      Assert.That (converterResult.TypeConverterType, Is.EqualTo (TypeConverterType.SourceTypeConverter));
      Assert.That (converterResult.TypeConverter.GetType(), Is.EqualTo (typeof (BidirectionalStringConverter)));
    }

    [Test]
    public void GetTypeConverter_ForNaByte ()
    {
      TypeConverter converter = _provider.GetTypeConverter (typeof (byte?));
      Assert.That (converter, Is.Null, "TypeConverter is not null.");
    }

    [Test]
    public void GetTypeConverter_ForByte ()
    {
      TypeConverter converter = _provider.GetTypeConverter (typeof (byte));
      Assert.That (converter, Is.Null, "TypeConverter is not null.");
    }

    [Test]
    public void GetTypeConverter_ForNaInt16 ()
    {
      TypeConverter converter = _provider.GetTypeConverter (typeof (short?));
      Assert.That (converter, Is.Null, "TypeConverter is not null.");
    }

    [Test]
    public void GetTypeConverter_ForInt16 ()
    {
      TypeConverter converter = _provider.GetTypeConverter (typeof (short));
      Assert.That (converter, Is.Null, "TypeConverter is not null.");
    }

    [Test]
    public void GetTypeConverter_ForInt32 ()
    {
      TypeConverter converter = _provider.GetTypeConverter (_int32);
      Assert.That (converter, Is.Null, "TypeConverter is not null.");
    }

    [Test]
    public void GetTypeConverter_ForNullableInt32 ()
    {
      TypeConverter converter = _provider.GetTypeConverter (_nullableInt32);
      Assert.That (converter, Is.Null, "TypeConverter is not null.");
    }

    [Test]
    public void GetTypeConverter_ForNaInt64 ()
    {
      TypeConverter converter = _provider.GetTypeConverter (typeof (long?));
      Assert.That (converter, Is.Null, "TypeConverter is not null.");
    }

    [Test]
    public void GetTypeConverter_ForInt64 ()
    {
      TypeConverter converter = _provider.GetTypeConverter (typeof (long));
      Assert.That (converter, Is.Null, "TypeConverter is not null.");
    }

    [Test]
    public void GetTypeConverter_ForNaSingle ()
    {
      TypeConverter converter = _provider.GetTypeConverter (typeof (float?));
      Assert.That (converter, Is.Null, "TypeConverter is not null.");
    }

    [Test]
    public void GetTypeConverter_ForSingle ()
    {
      TypeConverter converter = _provider.GetTypeConverter (typeof (float));
      Assert.That (converter, Is.Null, "TypeConverter is not null.");
    }

    [Test]
    public void GetTypeConverter_ForNaDouble ()
    {
      TypeConverter converter = _provider.GetTypeConverter (typeof (double?));
      Assert.That (converter, Is.Null, "TypeConverter is not null.");
    }

    [Test]
    public void GetTypeConverter_ForDouble ()
    {
      TypeConverter converter = _provider.GetTypeConverter (typeof (double));
      Assert.That (converter, Is.Null, "TypeConverter is not null.");
    }

    [Test]
    public void GetTypeConverter_ForNaDateTime ()
    {
      TypeConverter converter = _provider.GetTypeConverter (typeof (DateTime?));
      Assert.That (converter, Is.Null, "TypeConverter is not null.");
    }

    [Test]
    public void GetTypeConverter_ForDateTime ()
    {
      TypeConverter converter = _provider.GetTypeConverter (typeof (DateTime));
      Assert.That (converter, Is.Null, "TypeConverter is not null.");
    }

    [Test]
    public void GetTypeConverter_ForNullableBoolean ()
    {
      TypeConverter converter = _provider.GetTypeConverter (typeof (bool?));
      Assert.That (converter, Is.Null, "TypeConverter is not null.");
    }

    [Test]
    public void GetTypeConverter_ForBoolean ()
    {
      TypeConverter converter = _provider.GetTypeConverter (typeof (bool));
      Assert.That (converter, Is.Null, "TypeConverter is not null.");
    }

    [Test]
    public void GetTypeConverter_ForNaGuid ()
    {
      TypeConverter converter = _provider.GetTypeConverter (typeof (Guid?));
      Assert.That (converter, Is.Null, "TypeConverter is not null.");
    }

    [Test]
    public void GetTypeConverter_ForNaDecimal ()
    {
      TypeConverter converter = _provider.GetTypeConverter (typeof (decimal?));
      Assert.That (converter, Is.Null, "TypeConverter is not null.");
    }

    [Test]
    public void GetTypeConverter_ForDecimal ()
    {
      TypeConverter converter = _provider.GetTypeConverter (typeof (decimal));
      Assert.That (converter, Is.Null, "TypeConverter is not null.");
    }

    [Test]
    public void GetTypeConverter_ForGuid ()
    {
      TypeConverter converter = _provider.GetTypeConverter (typeof (Guid));
      Assert.That (converter, Is.Null, "TypeConverter is not null.");
    }

    [Test]
    public void GetTypeConverter_ForInt32Enum ()
    {
      TypeConverter converterFirstRun = _provider.GetTypeConverter (_int32Enum);
      TypeConverter converterSecondRun = _provider.GetTypeConverter (_int32Enum);
      Assert.That (converterFirstRun, Is.Not.Null, "TypeConverter from first run is null.");
      Assert.That (converterSecondRun, Is.Not.Null, "TypeConverter from second run is null.");
      Assert.That (converterSecondRun, Is.SameAs (converterFirstRun));
      Assert.That (converterFirstRun.GetType(), Is.EqualTo (typeof (AdvancedEnumConverter)));
      Assert.That (((AdvancedEnumConverter)converterFirstRun).EnumType, Is.EqualTo (_int32Enum));
    }

    [Test]
    public void GetTypeConverter_UsesCache ()
    {
      Assert.That (_provider.GetTypeConverter (_nullableInt32), Is.SameAs (_provider.GetTypeConverter (_nullableInt32)));
    }

    [Test]
    public void AddTypeConverter ()
    {
      var converter = new NullableConverter (typeof (Guid?));
      Assert.That (_provider.GetTypeConverter (_guid), Is.Null);
      ((TypeConversionProvider) _provider).AddTypeConverter (_guid, converter);
      Assert.That (_provider.GetTypeConverter (_guid), Is.SameAs (converter));
    }

    [Test]
    public void RemoveTypeConverter ()
    {
      var converter = new NullableConverter (typeof (Guid?));
      ((TypeConversionProvider) _provider).AddTypeConverter (_guid, converter);
      Assert.That (_provider.GetTypeConverter (_guid), Is.SameAs (converter));
      ((TypeConversionProvider) _provider).RemoveTypeConverter (_guid);
      Assert.That (_provider.GetTypeConverter (_guid), Is.Null);
    }
  }
}
