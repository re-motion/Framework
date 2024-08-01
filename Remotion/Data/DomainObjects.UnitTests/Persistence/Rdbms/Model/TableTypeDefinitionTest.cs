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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;

[TestFixture]
public class TableTypeDefinitionTest
{
  [Test]
  public void Initialize ()
  {
    var typeName = new EntityNameDefinition("test", "MyTableType");
    var properties = new[] { Mock.Of<IRdbmsStoragePropertyDefinition>(), Mock.Of<IRdbmsStoragePropertyDefinition>() };
    var constraints = new List<ITableConstraintDefinition> { Mock.Of<ITableConstraintDefinition>() };

    var tableTypeDefinition = new TableTypeDefinition(typeName, properties, constraints);

    Assert.That(tableTypeDefinition.TypeName, Is.EqualTo(typeName));
    Assert.That(tableTypeDefinition.Properties, Is.EqualTo(properties));
    Assert.That(tableTypeDefinition.Constraints, Is.EqualTo(constraints));
  }

  [Test]
  public void GetAllColumns_ReturnsColumnsOfAllProperties ()
  {
    var name = new EntityNameDefinition(null, "Test");

    var column1 = new ColumnDefinition("Column1", Mock.Of<IStorageTypeInformation>(), false);
    var column2 = new ColumnDefinition("Column2", Mock.Of<IStorageTypeInformation>(), false);
    var column3 = new ColumnDefinition("Column3", Mock.Of<IStorageTypeInformation>(), false);
    var column4 = new ColumnDefinition("Column4", Mock.Of<IStorageTypeInformation>(), false);

    var storagePropertyStub1 = new Mock<IRdbmsStoragePropertyDefinition>();
    storagePropertyStub1.Setup(_ => _.GetColumns()).Returns([column1, column2]);
    var storagePropertyStub2 = new Mock<IRdbmsStoragePropertyDefinition>();
    storagePropertyStub2.Setup(_ => _.GetColumns()).Returns([column3, column4]);

    var tableTypeDefinition = new TableTypeDefinition(name, [storagePropertyStub1.Object, storagePropertyStub2.Object], []);
    var result = tableTypeDefinition.GetAllColumns();
    Assert.That(result, Is.EqualTo(new[] { column1, column2, column3, column4 }));
  }

  [Test]
  public void Accept_Calls_VisitTableTypeDefinition ()
  {
    var typeName = new EntityNameDefinition("test", "MyTableType");
    var properties = new List<IRdbmsStoragePropertyDefinition> { Mock.Of<IRdbmsStoragePropertyDefinition>() };
    var constraints = Array.Empty<ITableConstraintDefinition>();

    var tableTypeDefinition = new TableTypeDefinition(typeName, properties, constraints);

    var visitorMock = new Mock<IRdbmsStructuredTypeDefinitionVisitor>();
    visitorMock.Setup(_ => _.VisitTableTypeDefinition(tableTypeDefinition)).Verifiable();

    ((IRdbmsStructuredTypeDefinition)tableTypeDefinition).Accept(visitorMock.Object);
    visitorMock.Verify();
  }

  public struct RecordPropertyData
  {
    public string Name { get; set; }
    public ColumnData[] Columns { get; set; }
  }

  public struct ColumnData
  {
    public string Name;
    public DbType DbType;
    public int? Length;
    public SqlDbType SqlDbType;
  }

  private static IEnumerable<RecordPropertyData[]> GetSqlMetaDataTestCases ()
  {
    // single property with one column
    yield return [new RecordPropertyData { Columns = [ new ColumnData { Name = "Value", DbType = DbType.String, Length = -1, SqlDbType = SqlDbType.NVarChar } ] }];
    yield return [new RecordPropertyData { Columns = [ new ColumnData { Name = "Value", DbType = DbType.AnsiString, Length = 100, SqlDbType = SqlDbType.VarChar } ] }];
    yield return [new RecordPropertyData { Columns = [ new ColumnData { Name = "Value", DbType = DbType.Binary, Length = 512, SqlDbType = SqlDbType.VarBinary } ] }];

    // multiple properties, one column each
    yield return
    [
        new RecordPropertyData { Columns = [ new ColumnData { Name = "Bool", DbType = DbType.Boolean, Length = null, SqlDbType = SqlDbType.Bit } ] },
        new RecordPropertyData { Columns = [ new ColumnData { Name = "Byte", DbType = DbType.Byte, Length = null, SqlDbType = SqlDbType.TinyInt } ] },
        new RecordPropertyData { Columns = [ new ColumnData { Name = "Short", DbType = DbType.Int16, Length = null, SqlDbType = SqlDbType.SmallInt } ] },
        new RecordPropertyData { Columns = [ new ColumnData { Name = "Int", DbType = DbType.Int32, Length = null, SqlDbType = SqlDbType.Int } ] },
        new RecordPropertyData { Columns = [ new ColumnData { Name = "Long", DbType = DbType.Int64, Length = null, SqlDbType = SqlDbType.BigInt } ] }
    ];
    yield return
    [
        new RecordPropertyData { Columns = [ new ColumnData { Name = "Decimal", DbType = DbType.Decimal, Length = null, SqlDbType = SqlDbType.Decimal } ] },
        new RecordPropertyData { Columns = [ new ColumnData { Name = "Float", DbType = DbType.Single, Length = null, SqlDbType = SqlDbType.Real } ] },
        new RecordPropertyData { Columns = [ new ColumnData { Name = "Double", DbType = DbType.Double, Length = null, SqlDbType = SqlDbType.Float } ] }
    ];
    yield return
    [
        new RecordPropertyData { Columns = [ new ColumnData { Name = "Guid", DbType = DbType.Guid, Length = null, SqlDbType = SqlDbType.UniqueIdentifier } ] },
        new RecordPropertyData { Columns = [ new ColumnData { Name = "DateTime", DbType = DbType.DateTime2, Length = null, SqlDbType = SqlDbType.DateTime2 } ] },
        new RecordPropertyData { Columns = [ new ColumnData { Name = "Date", DbType = DbType.DateTime, Length = null, SqlDbType = SqlDbType.DateTime } ] }
    ];

    // one property with multiple columns
    yield return
    [
        new RecordPropertyData
        {
            Columns =
            [
                new ColumnData { Name = "IDValue", DbType = DbType.Guid, Length = null, SqlDbType = SqlDbType.UniqueIdentifier },
                new ColumnData { Name = "ClassID", DbType = DbType.AnsiString, Length = 100, SqlDbType = SqlDbType.VarChar }
            ]
        }
    ];
  }

  [Test]
  [TestCaseSource(nameof(GetSqlMetaDataTestCases))]
  public void CreateTableValuedParameterValue (RecordPropertyData[] properties)
  {
    var storagePropertyDefinitions = new List<IRdbmsStoragePropertyDefinition>();
    var expectedSqlMetaData = new List<(string name, SqlDbType sqlDbType, int? length)>();

    foreach (var property in properties)
    {
      var columnDefinitions = new List<ColumnDefinition>();
      foreach (var column in property.Columns)
      {
        var storageTypeInfo = new Mock<IStorageTypeInformation>();
        storageTypeInfo.Setup(_ => _.StorageDbType).Returns(column.DbType);
        storageTypeInfo.Setup(_ => _.StorageTypeLength).Returns(column.Length);

        var columnDefinition = new ColumnDefinition(column.Name, storageTypeInfo.Object, false);
        columnDefinitions.Add(columnDefinition);
        expectedSqlMetaData.Add((column.Name, column.SqlDbType, column.Length));
      }

      var storagePropertyDefinition = new Mock<IRdbmsStoragePropertyDefinition>();
      storagePropertyDefinition.Setup(_ => _.GetColumns()).Returns(columnDefinitions);
      storagePropertyDefinitions.Add(storagePropertyDefinition.Object);
    }

    var tableTypeName = new EntityNameDefinition(null, $"TestTableType");
    var tableTypeDefinition = new TableTypeDefinition(
        tableTypeName,
        storagePropertyDefinitions,
        Array.Empty<ITableConstraintDefinition>());

    var result = tableTypeDefinition.CreateTableValuedParameterValue();
    Assert.That(result.IsEmpty, Is.True);
    Assert.That(result.TableTypeName, Is.EqualTo("TestTableType"));
    Assert.That(result.ColumnMetaData.Count, Is.EqualTo(expectedSqlMetaData.Count));
    var expected = expectedSqlMetaData.GetEnumerator();
    foreach (var actual in result.ColumnMetaData)
    {
      expected.MoveNext();
      Assert.That(actual.Name, Is.EqualTo(expected.Current.name));
      Assert.That(actual.SqlDbType, Is.EqualTo(expected.Current.sqlDbType));
      if (expected.Current.length.HasValue)
        Assert.That(actual.MaxLength, Is.EqualTo(expected.Current.length));
    }
  }

  [Test]
  public void CreateTableValuedParameterValue_Decimal ()
  {
    var storagePropertyDefinitions = new List<IRdbmsStoragePropertyDefinition>();
    var columnDefinitions = new List<ColumnDefinition>();

    var storageTypeInfo = new Mock<IStorageTypeInformation>();
    storageTypeInfo.Setup(_ => _.StorageDbType).Returns(DbType.Decimal);
    storageTypeInfo.Setup(_ => _.StorageTypeLength).Returns(default(int?));

    var columnDefinition = new ColumnDefinition("DecimalValue", storageTypeInfo.Object, false);
    columnDefinitions.Add(columnDefinition);

    var storagePropertyDefinition = new Mock<IRdbmsStoragePropertyDefinition>();
    storagePropertyDefinition.Setup(_ => _.GetColumns()).Returns(columnDefinitions);
    storagePropertyDefinitions.Add(storagePropertyDefinition.Object);

    var tableTypeName = new EntityNameDefinition(null, $"TestTableType");
    var tableTypeDefinition = new TableTypeDefinition(
        tableTypeName,
        storagePropertyDefinitions,
        Array.Empty<ITableConstraintDefinition>());

    var result = tableTypeDefinition.CreateTableValuedParameterValue();
    Assert.That(result.IsEmpty, Is.True);
    Assert.That(result.TableTypeName, Is.EqualTo("TestTableType"));
    Assert.That(result.ColumnMetaData.Count, Is.EqualTo(1));

    var sqlMetaData = result.ColumnMetaData.Single();

    Assert.That(sqlMetaData.Name, Is.EqualTo("DecimalValue"));
    Assert.That(sqlMetaData.SqlDbType, Is.EqualTo(SqlDbType.Decimal));
    Assert.That(sqlMetaData.Precision, Is.EqualTo(38));
    Assert.That(sqlMetaData.Scale, Is.EqualTo(3));
  }
}
