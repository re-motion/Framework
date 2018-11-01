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

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain
{
  [DBTable ("TableWithAllDataTypes")]
  [FirstStorageGroup]
  [Instantiable]
  public abstract class ClassWithAllDataTypes : DomainObject
  {
    public enum EnumType
    {
      Value0 = 0,
      Value1 = 1,
      Value2 = 2
    }

    public static ClassWithAllDataTypes NewObject()
    {
      return NewObject<ClassWithAllDataTypes> ();
    }

    [DBColumn ("Boolean")]
    public abstract bool BooleanProperty { get; set; }

    [DBColumn ("Byte")]
    public abstract byte ByteProperty { get; set; }

    [DBColumn ("Date")]
    public abstract DateTime DateProperty { get; set; }

    [DBColumn ("DateTime")]
    public abstract DateTime DateTimeProperty { get; set; }

    [DBColumn ("Decimal")]
    public abstract decimal DecimalProperty { get; set; }

    [DBColumn ("Double")]
    public abstract double DoubleProperty { get; set; }

    [DBColumn ("Enum")]
    public abstract EnumType EnumProperty { get; set; }

    [ExtensibleEnumProperty (IsNullable = false)]
    [DBColumn ("ExtensibleEnum")]
    public virtual Color ExtensibleEnumProperty { get; set; }

    [DBColumn ("Guid")]
    public abstract Guid GuidProperty { get; set; }

    [DBColumn ("Int16")]
    public abstract short Int16Property { get; set; }

    [DBColumn ("Int32")]
    public abstract int Int32Property { get; set; }

    [DBColumn ("Int64")]
    public abstract long Int64Property { get; set; }

    [DBColumn ("Single")]
    public abstract float SingleProperty { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    [DBColumn ("String")]
    public abstract string StringProperty { get; set; }

    [StringProperty (IsNullable = false)]
    [DBColumn ("StringWithoutMaxLength")]
    public abstract string StringPropertyWithoutMaxLength { get; set; }

    [BinaryProperty (IsNullable = false, MaximumLength = 100)]
    [DBColumn ("Binary")]
    public abstract byte[] BinaryProperty { get; set; }

    [BinaryProperty (IsNullable = false)]
    [DBColumn ("BinaryWithoutMaxLength")]
    public abstract byte[] BinaryWithoutMaxLengthProperty { get; set; }

    [DBColumn ("NaBoolean")]
    public abstract bool? NaBooleanProperty { get; set; }

    [DBColumn ("NaByte")]
    public abstract byte? NaByteProperty { get; set; }

    [DBColumn ("NaDate")]
    public abstract DateTime? NaDateProperty { get; set; }

    [DBColumn ("NaDateTime")]
    public abstract DateTime? NaDateTimeProperty { get; set; }

    [DBColumn ("NaDecimal")]
    public abstract Decimal? NaDecimalProperty { get; set; }

    [DBColumn ("NaDouble")]
    public abstract double? NaDoubleProperty { get; set; }

    [DBColumn ("NaEnum")]
    public abstract EnumType? NaEnumProperty { get; set; }

    [DBColumn ("NaGuid")]
    public abstract Guid? NaGuidProperty { get; set; }

    [DBColumn ("NaInt16")]
    public abstract short? NaInt16Property { get; set; }

    [DBColumn ("NaInt32")]
    public abstract int? NaInt32Property { get; set; }

    [DBColumn ("NaInt64")]
    public abstract long? NaInt64Property { get; set; }

    [DBColumn ("NaSingle")]
    public abstract float? NaSingleProperty { get; set; }

    [StringProperty (MaximumLength = 100)]
    [DBColumn ("StringWithNullValue")]
    public abstract string StringWithNullValueProperty { get; set; }

    [DBColumn ("ExtensibleEnumWithNullValue")]
    public virtual Color ExtensibleEnumWithNullValueProperty { get; set; }

    [DBColumn ("NaBooleanWithNullValue")]
    public abstract bool? NaBooleanWithNullValueProperty { get; set; }

    [DBColumn ("NaByteWithNullValue")]
    public abstract byte? NaByteWithNullValueProperty { get; set; }

    [DBColumn ("NaDateWithNullValue")]
    public abstract DateTime? NaDateWithNullValueProperty { get; set; }

    [DBColumn ("NaDateTimeWithNullValue")]
    public abstract DateTime? NaDateTimeWithNullValueProperty { get; set; }

    [DBColumn ("NaDecimalWithNullValue")]
    public abstract Decimal? NaDecimalWithNullValueProperty { get; set; }

    [DBColumn ("NaDoubleWithNullValue")]
    public abstract double? NaDoubleWithNullValueProperty { get; set; }

    [DBColumn ("NaEnumWithNullValue")]
    public abstract EnumType? NaEnumWithNullValueProperty { get; set; }

    [DBColumn ("NaGuidWithNullValue")]
    public abstract Guid? NaGuidWithNullValueProperty { get; set; }

    [DBColumn ("NaInt16WithNullValue")]
    public abstract short? NaInt16WithNullValueProperty { get; set; }

    [DBColumn ("NaInt32WithNullValue")]
    public abstract int? NaInt32WithNullValueProperty { get; set; }

    [DBColumn ("NaInt64WithNullValue")]
    public abstract long? NaInt64WithNullValueProperty { get; set; }

    [DBColumn ("NaSingleWithNullValue")]
    public abstract float? NaSingleWithNullValueProperty { get; set; }

    [BinaryProperty (MaximumLength = 100)]
    [DBColumn ("NullableBinary")]
    public abstract byte[] NullableBinaryProperty { get; set; }

    [DBColumn ("NullableBinaryWithoutMaxLength")]
    public abstract byte[] NullableBinaryWithoutMaxLengthProperty { get; set; }
  }
}
