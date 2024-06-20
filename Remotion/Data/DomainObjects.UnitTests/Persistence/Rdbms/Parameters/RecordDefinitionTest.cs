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
using System;
using System.Data;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.ExtensibleEnums;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Parameters;

[TestFixture]
public class RecordDefinitionTest
{
  [Test]
  public void Initialize ()
  {
    var storagePropertyStub1 = new Mock<IRdbmsStoragePropertyDefinition>();

    var propertyDefinition1 = new FakeRecordPropertyDefinition(storagePropertyStub1.Object);

    var structTypeDefinitionStub = new Mock<IRdbmsStructuredTypeDefinition>();
    structTypeDefinitionStub.Setup(_ => _.Properties).Returns(new[] { storagePropertyStub1.Object });

    RecordDefinition recordDefinition = null!;
    Assert.That(() => recordDefinition = new RecordDefinition("Test", structTypeDefinitionStub.Object, [propertyDefinition1]), Throws.Nothing);
    Assert.That(recordDefinition.RecordName, Is.EqualTo("Test"));
    Assert.That(recordDefinition.PropertyDefinitions, Is.EqualTo(new[] { propertyDefinition1 }));
    Assert.That(recordDefinition.StructuredTypeDefinition, Is.SameAs(structTypeDefinitionStub.Object));
  }

  [Test]
  public void Initialize_WithMismatchedColumns_ThrowsArgumentException ()
  {
    var storagePropertyStub1 = new Mock<IRdbmsStoragePropertyDefinition>();
    var storagePropertyStub2 = new Mock<IRdbmsStoragePropertyDefinition>();

    var propertyDefinition1 = new FakeRecordPropertyDefinition(storagePropertyStub1.Object);

    var structTypeDefinitionStub = new Mock<IRdbmsStructuredTypeDefinition>();
    structTypeDefinitionStub.Setup(_ => _.Properties).Returns(new[] { storagePropertyStub2.Object });

    Assert.That(
        () => new RecordDefinition("Fail", structTypeDefinitionStub.Object, [propertyDefinition1]),
        Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("The given structuredTypeDefinition's properties do not match those of the given propertyDefinitions."));
  }

  [Test]
  public void GetColumnValues_ExtractsPropertyValuesFromItem_AndSplitsIntoColumnValues ()
  {
    var column1 = new ColumnDefinition("Int", StorageTypeInformationObjectMother.CreateIntStorageTypeInformation(), false);
    var column2 = new ColumnDefinition("Bit", StorageTypeInformationObjectMother.CreateBitStorageTypeInformation(), false);
    var column3 = new ColumnDefinition("AnsiString", StorageTypeInformationObjectMother.CreateVarchar100StorageTypeInformation(), false);

    var storageProperty1 = new Mock<IRdbmsStoragePropertyDefinition>();
    storageProperty1.Setup(_ => _.SplitValue("PropertyValue1")).Returns([ new ColumnValue(column1, 42), new ColumnValue(column2, true) ]);

    var storageProperty2 = new Mock<IRdbmsStoragePropertyDefinition>();
    storageProperty2.Setup(_ => _.SplitValue("PropertyValue2")).Returns([ new ColumnValue(column3, "Text") ]);

    var tableTypeDefinition = new TableTypeDefinition(
        new EntityNameDefinition(null, "Test"),
        [ storageProperty1.Object, storageProperty2.Object ],
        Array.Empty<ITableConstraintDefinition>());

    var property1 = new FakeRecordPropertyDefinition(storageProperty1.Object, "PropertyValue1");
    var property2 = new FakeRecordPropertyDefinition(storageProperty2.Object, "PropertyValue2");

    var recordDefinition = new RecordDefinition("Test", tableTypeDefinition, [property1, property2]);

    var result = recordDefinition.GetColumnValues("Item");
    var expected = new object[] { 42, true, "Text"}; // values of all "SplitValue" results concatenated
    Assert.That(result, Is.EqualTo(expected));
  }

  [Test]
  public void GetColumnValues_ConvertsColumnValues ()
  {
    var enumStorageTypeInfo = StorageTypeInformationObjectMother.CreateStorageTypeInformation(
        typeof(int?),
        "int",
        DbType.Int32,
        true,
        null,
        typeof(ClassWithAllDataTypes.EnumType),
        new AdvancedEnumConverter(typeof(ClassWithAllDataTypes.EnumType)));

    var extensibleEnumStorageTypeInfo = StorageTypeInformationObjectMother.CreateStorageTypeInformation(
        typeof(string),
        "varchar(100)",
        DbType.AnsiString,
        true,
        100,
        typeof(IExtensibleEnum),
        new ExtensibleEnumConverter(typeof(Color)));

    var column1 = new ColumnDefinition("Int", enumStorageTypeInfo, false);
    var column2 = new ColumnDefinition("AnsiString", extensibleEnumStorageTypeInfo, false);

    var storageProperty1 = new Mock<IRdbmsStoragePropertyDefinition>();
    storageProperty1.Setup(_ => _.SplitValue("PropertyValue1")).Returns([new ColumnValue(column1, ClassWithAllDataTypes.EnumType.Value1)]);

    var storageProperty2 = new Mock<IRdbmsStoragePropertyDefinition>();
    storageProperty2.Setup(_ => _.SplitValue("PropertyValue2")).Returns([new ColumnValue(column2, Color.Values.Green())]);

    var tableTypeDefinition = new TableTypeDefinition(
        new EntityNameDefinition(null, "Test"),
        [ storageProperty1.Object, storageProperty2.Object ],
        Array.Empty<ITableConstraintDefinition>());

    var property1 = new FakeRecordPropertyDefinition(storageProperty1.Object, "PropertyValue1");
    var property2 = new FakeRecordPropertyDefinition(storageProperty2.Object, "PropertyValue2");

    var recordDefinition = new RecordDefinition("Test", tableTypeDefinition, [property1, property2]);

    var result = recordDefinition.GetColumnValues("Item");
    var expected = new object[] { (int)ClassWithAllDataTypes.EnumType.Value1, Color.Values.Green().ID }; // values of all "SplitValue" results converted and concatenated
    Assert.That(result, Is.EqualTo(expected));
  }
}
