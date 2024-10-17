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
using System.Data;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.ExtensibleEnums;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.Model.Building
{
  [TestFixture]
  public class SqlStorageTypeInformationProviderTest
  {
    private IStorageTypeInformationProvider _storageTypeInformationProvider;

    // We explicitly want an _int_ enum
    // ReSharper disable EnumUnderlyingTypeIsInt
    private enum Int32Enum : int
    {
    }

    // ReSharper restore EnumUnderlyingTypeIsInt

    private enum Int16Enum : short
    {
    }

    private enum UnsupportedEnum : ulong
    {
    }

    [DateTimeStorageType(DateTimeStorageType.DateTime)]
    public DateTime? NullableDateTime { get; set; }

    [DateTimeStorageType(DateTimeStorageType.DateTime)]
    public DateTime DateTime { get; set; }

    [DateTimeStorageType(DateTimeStorageType.DateTime2)]
    public DateTime DateTime2 { get; set; }

    public DateTime DefaultDateTime2 { get; set; }

    [SetUp]
    public void SetUp ()
    {
      _storageTypeInformationProvider = new SqlStorageTypeInformationProvider();
    }

    [Test]
    public void GetStorageType_ForProperty_SimpleValueTypes ()
    {
      CheckGetStorageType_ForProperty(
          typeof(Boolean),
          null,
          false,
          false,
          typeof(bool),
          "bit",
          DbType.Boolean,
          false,
          null,
          typeof(bool),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(bool)));
      CheckGetStorageType_ForProperty(
          typeof(Byte),
          null,
          false,
          false,
          typeof(Byte),
          "tinyint",
          DbType.Byte,
          false,
          null,
          typeof(byte),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(byte)));
      CheckGetStorageType_ForProperty(
          typeof(DateTime),
          null,
          false,
          false,
          typeof(DateTime),
          "datetime2",
          DbType.DateTime2,
          false,
          null,
          typeof(DateTime),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(DateTime)));
      CheckGetStorageType_ForProperty(
          typeof(DateOnly),
          null,
          false,
          false,
          typeof(DateTime),
          "date",
          DbType.Date,
          false,
          null,
          typeof(DateOnly),
          Is.TypeOf(typeof(DateOnlyConverter)));
      CheckGetStorageType_ForProperty(
          typeof(Decimal),
          null,
          false,
          false,
          typeof(Decimal),
          "decimal (38, 3)",
          DbType.Decimal,
          false,
          null,
          typeof(decimal),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(Decimal)));
      CheckGetStorageType_ForProperty(
          typeof(Double),
          null,
          false,
          false,
          typeof(Double),
          "float",
          DbType.Double,
          false,
          null,
          typeof(double),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(Double)));
      CheckGetStorageType_ForProperty(
          typeof(Guid),
          null,
          false,
          false,
          typeof(Guid),
          "uniqueidentifier",
          DbType.Guid,
          false,
          null,
          typeof(Guid),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(Guid)));
      CheckGetStorageType_ForProperty(
          typeof(Int16),
          null,
          false,
          false,
          typeof(Int16),
          "smallint",
          DbType.Int16,
          false,
          null,
          typeof(short),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(Int16)));
      CheckGetStorageType_ForProperty(
          typeof(Int32),
          null,
          false,
          false,
          typeof(Int32),
          "int",
          DbType.Int32,
          false,
          null,
          typeof(int),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(Int32)));
      CheckGetStorageType_ForProperty(
          typeof(Int64),
          null,
          false,
          false,
          typeof(Int64),
          "bigint",
          DbType.Int64,
          false,
          null,
          typeof(long),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(Int64)));
      CheckGetStorageType_ForProperty(
          typeof(Single),
          null,
          false,
          false,
          typeof(Single),
          "real",
          DbType.Single,
          false,
          null,
          typeof(float),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(Single)));
    }

    [Test]
    public void GetStorageType_ForProperty_Enums ()
    {
      CheckGetStorageType_ForProperty(
          typeof(Int32Enum),
          null,
          false,
          false,
          typeof(Int32),
          "int",
          DbType.Int32,
          false,
          null,
          typeof(Int32Enum),
          Is.TypeOf(typeof(AdvancedEnumConverter)).With.Property("EnumType").EqualTo(typeof(Int32Enum)));
      CheckGetStorageType_ForProperty(
          typeof(Int16Enum),
          null,
          false,
          false,
          typeof(Int16),
          "smallint",
          DbType.Int16,
          false,
          null,
          typeof(Int16Enum),
          Is.TypeOf(typeof(AdvancedEnumConverter)).With.Property("EnumType").EqualTo(typeof(Int16Enum)));
    }

    [Test]
    public void GetStorageType_ForProperty_ExtensibleEnums ()
    {
      var maxColorIDLength = Color.Values.Green().ID.Length;
      CheckGetStorageType_ForProperty(
          typeof(Color),
          null,
          false,
          false,
          typeof(string),
          "varchar (" + maxColorIDLength + ")",
          DbType.AnsiString,
          false,
          maxColorIDLength,
          typeof(Color),
          Is.TypeOf(typeof(ExtensibleEnumConverter)).With.Property("ExtensibleEnumType").EqualTo(typeof(Color)));
      CheckGetStorageType_ForProperty(
          typeof(Color),
          null,
          true,
          false,
          typeof(string),
          "varchar (" + maxColorIDLength + ")",
          DbType.AnsiString,
          true,
          maxColorIDLength,
          typeof(Color),
          Is.TypeOf(typeof(ExtensibleEnumConverter)).With.Property("ExtensibleEnumType").EqualTo(typeof(Color)));
      CheckGetStorageType_ForProperty(
          typeof(Color),
          null,
          false,
          true,
          typeof(string),
          "varchar (" + maxColorIDLength + ")",
          DbType.AnsiString,
          true,
          maxColorIDLength,
          typeof(Color),
          Is.TypeOf(typeof(ExtensibleEnumConverter)).With.Property("ExtensibleEnumType").EqualTo(typeof(Color)));
      CheckGetStorageType_ForProperty(
          typeof(ExtensibleEnumNotDefiningAnyValues),
          null,
          false,
          true,
          typeof(string),
          "varchar (max)",
          DbType.AnsiString,
          true,
          -1,
          typeof(ExtensibleEnumNotDefiningAnyValues),
          Is.TypeOf(typeof(ExtensibleEnumConverter)).With.Property("ExtensibleEnumType").EqualTo(typeof(ExtensibleEnumNotDefiningAnyValues)));
    }

    [Test]
    public void GetStorageType_ForProperty_String ()
    {
      CheckGetStorageType_ForProperty(
          typeof(String),
          200,
          false,
          false,
          typeof(string),
          "nvarchar (200)",
          DbType.String,
          false,
          200,
          typeof(string),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(string)));
      CheckGetStorageType_ForProperty(
          typeof(String),
          200,
          false,
          true,
          typeof(string),
          "nvarchar (200)",
          DbType.String,
          true,
          200,
          typeof(string),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(string)));
      CheckGetStorageType_ForProperty(
          typeof(String),
          200,
          true,
          false,
          typeof(string),
          "nvarchar (200)",
          DbType.String,
          true,
          200,
          typeof(string),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(string)));
      CheckGetStorageType_ForProperty(
          typeof(String),
          null,
          false,
          false,
          typeof(string),
          "nvarchar (max)",
          DbType.String,
          false,
          -1,
          typeof(string),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(string)));
    }

    [Test]
    public void GetStorageType_ForProperty_ByteArray ()
    {
      CheckGetStorageType_ForProperty(
          typeof(Byte[]),
          200,
          false,
          false,
          typeof(Byte[]),
          "varbinary (200)",
          DbType.Binary,
          false,
          200,
          typeof(byte[]),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(byte[])));
      CheckGetStorageType_ForProperty(
          typeof(Byte[]),
          200,
          true,
          false,
          typeof(Byte[]),
          "varbinary (200)",
          DbType.Binary,
          true,
          200,
          typeof(byte[]),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(byte[])));
      CheckGetStorageType_ForProperty(
          typeof(Byte[]),
          200,
          false,
          true,
          typeof(Byte[]),
          "varbinary (200)",
          DbType.Binary,
          true,
          200,
          typeof(byte[]),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(byte[])));
      CheckGetStorageType_ForProperty(
          typeof(Byte[]),
          null,
          false,
          false,
          typeof(Byte[]),
          "varbinary (max)",
          DbType.Binary,
          false,
          -1,
          typeof(byte[]),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(byte[])));
    }

    [Test]
    public void GetStorageType_PropertyDefinition_ForNullableValueTypes ()
    {
      CheckGetStorageType_ForProperty(
          typeof(bool?),
          null,
          false,
          false,
          typeof(bool?),
          "bit",
          DbType.Boolean,
          false,
          null,
          typeof(bool?),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").SameAs(typeof(bool?)));

      CheckGetStorageType_ForProperty(
          typeof(bool?),
          null,
          true,
          false,
          typeof(bool?),
          "bit",
          DbType.Boolean,
          true,
          null,
          typeof(bool?),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").SameAs(typeof(bool?)));
      CheckGetStorageType_ForProperty(
          typeof(bool?),
          null,
          false,
          true,
          typeof(bool?),
          "bit",
          DbType.Boolean,
          true,
          null,
          typeof(bool?),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").SameAs(typeof(bool?)));

      CheckGetStorageType_ForProperty(
          typeof(Int16Enum?),
          null,
          false,
          false,
          typeof(Int16?),
          "smallint",
          DbType.Int16,
          false,
          null,
          typeof(Int16Enum?),
          Is.TypeOf(typeof(AdvancedEnumConverter)));
      CheckGetStorageType_ForProperty(
          typeof(Int16Enum?),
          null,
          false,
          true,
          typeof(Int16?),
          "smallint",
          DbType.Int16,
          true,
          null,
          typeof(Int16Enum?),
          Is.TypeOf(typeof(AdvancedEnumConverter)));
      CheckGetStorageType_ForProperty(
          typeof(Int16Enum?),
          null,
          true,
          false,
          typeof(Int16?),
          "smallint",
          DbType.Int16,
          true,
          null,
          typeof(Int16Enum?),
          Is.TypeOf(typeof(AdvancedEnumConverter)));
    }

    [Test]
    [TestCase(nameof(DateTime), "datetime", DbType.DateTime)]
    [TestCase(nameof(DateTime2), "datetime2", DbType.DateTime2)]
    [TestCase(nameof(DefaultDateTime2), "datetime2", DbType.DateTime2)]
    public void GetStorageType_ForDateTimeProperties (string propertyName, string expectedStorageTypeName, DbType expectedDbType)
    {
        var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo(
                ClassDefinitionObjectMother.CreateClassDefinition(),
                typeof(SqlStorageTypeInformationProviderTest),
                propertyName);
        var storageTypeInformation = (StorageTypeInformation)_storageTypeInformationProvider.GetStorageType(propertyDefinition, false);

        CheckStorageTypeInformation(
                storageTypeInformation,
                typeof(DateTime),
                expectedStorageTypeName,
                expectedDbType,
                false,
                null,
                typeof(DateTime),
                Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(DateTime))
        );
    }

    [Test]
    public void GetStorageType_ForNullableDateTimeProperty_WithDateTimeStorageTypeAttribute ()
    {
        var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo(
                ClassDefinitionObjectMother.CreateClassDefinition(),
                typeof(SqlStorageTypeInformationProviderTest),
                nameof(NullableDateTime));
        var storageTypeInformation = (StorageTypeInformation)_storageTypeInformationProvider.GetStorageType(propertyDefinition, false);

        CheckStorageTypeInformation(
                storageTypeInformation,
                typeof(DateTime?),
                "datetime",
                DbType.DateTime,
                true,
                null,
                typeof(DateTime?),
                Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(DateTime?))
        );
    }

    [Test]
    public void GetStorageTypeForID ()
    {
      var storageTypeForObjectID = (StorageTypeInformation)_storageTypeInformationProvider.GetStorageTypeForID(true);
      CheckStorageTypeInformation(
          storageTypeForObjectID,
          typeof(Guid?),
          "uniqueidentifier",
          DbType.Guid,
          true,
          null,
          typeof(Guid?),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(Guid?)));

      var storageTypeForObjectIDNotNullable = (StorageTypeInformation)_storageTypeInformationProvider.GetStorageTypeForID(false);
      CheckStorageTypeInformation(
          storageTypeForObjectIDNotNullable,
          typeof(Guid?),
          "uniqueidentifier",
          DbType.Guid,
          false,
          null,
          typeof(Guid?),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(Guid?)));
    }

    [Test]
    public void GetStorageTypeForSerializedObjectID ()
    {
      var storageTypeForSerializedObjectID = (StorageTypeInformation)_storageTypeInformationProvider.GetStorageTypeForSerializedObjectID(true);
      CheckStorageTypeInformation(
          storageTypeForSerializedObjectID,
          typeof(string),
          "varchar (255)",
          DbType.AnsiString,
          true,
          255,
          typeof(string),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(string)));

      var storageTypeForSerializedObjectIDNotNullable =
          (StorageTypeInformation)_storageTypeInformationProvider.GetStorageTypeForSerializedObjectID(false);
      CheckStorageTypeInformation(
          storageTypeForSerializedObjectIDNotNullable,
          typeof(string),
          "varchar (255)",
          DbType.AnsiString,
          false,
          255,
          typeof(string),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(string)));
    }

    [Test]
    public void GetStorageTypeForClassID ()
    {
      var storageTypeForClassID = (StorageTypeInformation)_storageTypeInformationProvider.GetStorageTypeForClassID(true);
      CheckStorageTypeInformation(
          storageTypeForClassID,
          typeof(string),
          "varchar (100)",
          DbType.AnsiString,
          true,
          100,
          typeof(string),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(string)));

      var storageTypeForClassIDNotNullable = (StorageTypeInformation)_storageTypeInformationProvider.GetStorageTypeForClassID(false);
      CheckStorageTypeInformation(
          storageTypeForClassIDNotNullable,
          typeof(string),
          "varchar (100)",
          DbType.AnsiString,
          false,
          100,
          typeof(string),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(string)));
    }

    [Test]
    public void GetStorageTypeForTimestamp ()
    {
      var storageTypeForTimestamp = (StorageTypeInformation)_storageTypeInformationProvider.GetStorageTypeForTimestamp(true);
      CheckStorageTypeInformation(
          storageTypeForTimestamp,
          typeof(byte[]),
          "rowversion",
          DbType.Binary,
          true,
          null,
          typeof(Byte[]),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(byte[])));

      var storageTypeForTimestampNotNullable = (StorageTypeInformation)_storageTypeInformationProvider.GetStorageTypeForTimestamp(false);
      CheckStorageTypeInformation(
          storageTypeForTimestampNotNullable,
          typeof(byte[]),
          "rowversion",
          DbType.Binary,
          false,
          null,
          typeof(Byte[]),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(byte[])));
    }

    [Test]
    public void GetStorageType_PropertyDefinition_WithNotSupportedType ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("Name", typeof(Char));
      Assert.That(
          () => _storageTypeInformationProvider.GetStorageType(propertyDefinition, false),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "Type 'System.Char' is not supported by this storage provider."));
    }

    [Test]
    public void GetStorageType_Type_SupportedType_Nullable ()
    {
      var result = (StorageTypeInformation)_storageTypeInformationProvider.GetStorageType(typeof(int?));

      CheckStorageTypeInformation(
          result,
          typeof(int?),
          "int",
          DbType.Int32,
          true,
          null,
          typeof(int?),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(int?)));
    }

    [Test]
    public void GetStorageType_Type_SupportedType_NotNullable ()
    {
      var result = (StorageTypeInformation)_storageTypeInformationProvider.GetStorageType(typeof(int));

      CheckStorageTypeInformation(
          result,
          typeof(int),
          "int",
          DbType.Int32,
          false,
          null,
          typeof(int),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(int)));
    }

    [Test]
    public void GetStorageType_Type_UnsupportedType_NotNullable ()
    {
      Assert.That(
          () => _storageTypeInformationProvider.GetStorageType(typeof(Char)),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "Type 'System.Char' is not supported by this storage provider."));
    }

    [Test]
    public void GetStorageType_Type_UnsupportedType_Nullable ()
    {
      Assert.That(
          () => _storageTypeInformationProvider.GetStorageType(typeof(Char?)),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo("Type 'System.Nullable`1[System.Char]' is not supported by this storage provider."));
    }

    [Test]
    public void GetStorageType_Type_UnsupportedType_EnumWithUnsupportedUnderlyingType ()
    {
      Assert.That(
          () => _storageTypeInformationProvider.GetStorageType(typeof(UnsupportedEnum)),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "Type 'Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.Model.Building.SqlStorageTypeInformationProviderTest+UnsupportedEnum' "
                  + "is not supported by this storage provider."));
    }

    [Test]
    public void GetStorageTypeInformation_ValueNull ()
    {
      var result = (StorageTypeInformation)_storageTypeInformationProvider.GetStorageType((object)null);

      CheckStorageTypeInformation(
          result,
          typeof(object),
          "nvarchar (max)",
          DbType.String,
          true,
          -1,
          typeof(object),
          Is.TypeOf(typeof(NullValueConverter)));
    }

    [Test]
    public void GetStorageTypeInformation_ValueNotNull_NonNullableType ()
    {
      var result = (StorageTypeInformation)_storageTypeInformationProvider.GetStorageType(14);

      CheckStorageTypeInformation(
          result,
          typeof(int),
          "int",
          DbType.Int32,
          false,
          null,
          typeof(int),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(int)));
    }

    [Test]
    public void GetStorageTypeInformation_ValueNotNull_NullableType ()
    {
      var result = (StorageTypeInformation)_storageTypeInformationProvider.GetStorageType("");

      CheckStorageTypeInformation(
          result,
          typeof(string),
          "nvarchar (max)",
          DbType.String,
          true,
          -1,
          typeof(string),
          Is.TypeOf(typeof(DefaultConverter)).With.Property("Type").EqualTo(typeof(string)));
    }

    private void CheckGetStorageType_ForProperty (
        Type propertyType,
        int? maxLength,
        bool isPropertyNullable,
        bool forceNullable,
        Type expectedStorageType,
        string expectedStorageTypeName,
        DbType expectedStorageDbType,
        bool expectedIsNullable,
        int? expectedStorageTypeLength,
        Type expectedDotNetType,
        IResolveConstraint expectedDotNetTypeConverterConstraint)
    {
      var propertyDefinition = CreatePropertyDefinition(propertyType, isPropertyNullable, maxLength);
      var info = (StorageTypeInformation)_storageTypeInformationProvider.GetStorageType(propertyDefinition, forceNullable);
      CheckStorageTypeInformation(
          info,
          expectedStorageType,
          expectedStorageTypeName,
          expectedStorageDbType,
          expectedIsNullable,
          expectedStorageTypeLength,
          expectedDotNetType,
          expectedDotNetTypeConverterConstraint);
    }

    private PropertyDefinition CreatePropertyDefinition (Type propertyType, bool isNullable, int? maxLength = null)
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      return PropertyDefinitionObjectMother.CreateForFakePropertyInfo(
          classDefinition,
          "Name",
          false,
          propertyType,
          isNullable,
          maxLength,
          StorageClass.Persistent);
    }

    private void CheckStorageTypeInformation (
        StorageTypeInformation storageTypeInformation,
        Type expectedStorageType,
        string expectedStorageTypeName,
        DbType expectedStorageDbType,
        bool expectedIsNullable,
        int? expectedStorageTypeLength,
        Type expectedDotNetType,
        IResolveConstraint dotNetTypeConverterConstraint)
    {
      Assert.That(storageTypeInformation.StorageType, Is.SameAs(expectedStorageType));
      Assert.That(storageTypeInformation.StorageTypeName, Is.EqualTo(expectedStorageTypeName));
      Assert.That(storageTypeInformation.StorageDbType, Is.EqualTo(expectedStorageDbType));
      Assert.That(storageTypeInformation.IsStorageTypeNullable, Is.EqualTo(expectedIsNullable));
      Assert.That(storageTypeInformation.StorageTypeLength, Is.EqualTo(expectedStorageTypeLength));
      Assert.That(storageTypeInformation.DotNetType, Is.SameAs(expectedDotNetType));
      Assert.That(storageTypeInformation.DotNetTypeConverter, dotNetTypeConverterConstraint);
    }
  }
}
