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
using System.Data.SqlClient;
using System.Linq;
using Microsoft.SqlServer.Server;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Parameters;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.Parameters;

[TestFixture]
public class SqlTableValuedDataParameterDefinitionTest
{
  [Test]
  public void GetParameterValue_NullValue_ReturnsDbNull ()
  {
    var storagePropertyDefinition = Mock.Of<IRdbmsStoragePropertyDefinition>();
    var tableTypeDefinition = new TableTypeDefinition(
        new EntityNameDefinition(null, "Test"),
        [storagePropertyDefinition],
        Array.Empty<ITableConstraintDefinition>());

    var recordProperty = new FakeRecordPropertyDefinition(storagePropertyDefinition);
    var recordDefinition = new RecordDefinition("Test", tableTypeDefinition, [recordProperty]);

    var parameterDefinition = new SqlTableValuedDataParameterDefinition(recordDefinition);

    var result = parameterDefinition.GetParameterValue(null);
    Assert.That(result, Is.EqualTo(DBNull.Value));
  }

  [Test]
  public void GetParameterValue_GetsValuesFromRecordDefinition_AndAddsThemToTvpValue ()
  {
    var records = new[]
                  {
                      new Guid("280D68FF-BEA9-47CE-BAEC-44569517829D"),
                      new Guid("6AE44A0F-2345-49AA-95C0-ABB8E968E5CA"),
                      new Guid("4F5D0D1E-EF63-4D3A-9519-585FF464477B")
                  };

    // set up a table type with a single Guid column
    var storageTypeInformation = StorageTypeInformationObjectMother.CreateUniqueIdentifierStorageTypeInformation();
    var storagePropertyDefinition = new SimpleStoragePropertyDefinition(typeof(Guid), new ColumnDefinition("Value", storageTypeInformation, false));
    var tableTypeDefinition = new TableTypeDefinition(new EntityNameDefinition(null, "Test"), [ storagePropertyDefinition ], Array.Empty<ITableConstraintDefinition>());

    // set up a record definition with a single property that uses an integer input as index to the records array
    var recordPropertyDefinition = new FakeRecordPropertyDefinition(storagePropertyDefinition, o => records[(int)o]);
    var recordDefinition = new RecordDefinition("Test", tableTypeDefinition, [ recordPropertyDefinition ]);

    var parameterDefinition = new SqlTableValuedDataParameterDefinition(recordDefinition);

    // input is a sequence of indices for the records array, so that the TVP value contains the GUIDs of the records array
    var result = parameterDefinition.GetParameterValue(new[] { 0, 1, 2 });

    Assert.That(result, Is.InstanceOf<SqlTableValuedParameterValue>());
    var tvpValue = result.As<SqlTableValuedParameterValue>();
    Assert.That(tvpValue.Count, Is.EqualTo(3));
    Assert.That(tvpValue.ColumnMetaData.Count, Is.EqualTo(1));
    Assert.That(tvpValue.Select(rec => rec.GetValue(0)), Is.EqualTo(records));
  }

  [Test]
  public void CreateDataParameter_WithTableValuedParameter_SetsSqlParameterProperties ()
  {
    var items = new[] { 1, 2, 3, 5, 8, 13, 21 };
    var tvpValue = new SqlTableValuedParameterValue("any", new[] { new SqlMetaData("Value", SqlDbType.Int) });
    foreach (var item in items)
      tvpValue.AddRecord(item);

    var storageTypeInformation = Mock.Of<IStorageTypeInformation>();
    var columnDefinition = new ColumnDefinition("Value", storageTypeInformation, false);
    var tableTypeDefinition = new TableTypeDefinition(
        new EntityNameDefinition(null, $"TestTableType"),
        [ new SimpleStoragePropertyDefinition(typeof(int), columnDefinition) ],
        Array.Empty<ITableConstraintDefinition>());

    var properties = tableTypeDefinition.Properties.Select(RecordPropertyDefinition.ScalarAsValue).ToArray();
    var recordDefinition = new RecordDefinition("Test", tableTypeDefinition, properties);

    var parameterDefinition = new SqlTableValuedDataParameterDefinition(recordDefinition);

    var command = new SqlCommand();
    var dataParameter = parameterDefinition.CreateDataParameter(command, "@dummy", tvpValue);
    Assert.That(dataParameter, Is.InstanceOf<SqlParameter>());

    var sqlParameter = dataParameter.As<SqlParameter>();
    Assert.That(sqlParameter.ParameterName, Is.EqualTo("@dummy"));
    Assert.That(sqlParameter.DbType, Is.EqualTo(DbType.Object));
    Assert.That(sqlParameter.SqlDbType, Is.EqualTo(SqlDbType.Structured));
    Assert.That(sqlParameter.TypeName, Is.EqualTo(tvpValue.TableTypeName));
    Assert.That(sqlParameter.Value, Is.SameAs(tvpValue));
  }

  [Test]
  public void CreateDataParameter_WithEmptyTableValuedParameter_SetsParameterValueToDBNull ()
  {
    var tvpValue = new SqlTableValuedParameterValue("any", new[] { new SqlMetaData("Value", SqlDbType.Int) });

    var storageTypeInformation = Mock.Of<IStorageTypeInformation>();
    var columnDefinition = new ColumnDefinition("Value", storageTypeInformation, false);
    var tableTypeDefinition = new TableTypeDefinition(
        new EntityNameDefinition(null, $"TestTableType"),
        [ new SimpleStoragePropertyDefinition(typeof(int), columnDefinition) ],
        Array.Empty<ITableConstraintDefinition>());

    var properties = tableTypeDefinition.Properties.Select(RecordPropertyDefinition.ScalarAsValue).ToArray();
    var recordDefinition = new RecordDefinition("Test", tableTypeDefinition, properties);

    var parameterDefinition = new SqlTableValuedDataParameterDefinition(recordDefinition);

    var command = new SqlCommand();
    var dataParameter = parameterDefinition.CreateDataParameter(command, "@dummy", tvpValue);
    Assert.That(dataParameter, Is.InstanceOf<SqlParameter>());

    var sqlParameter = dataParameter.As<SqlParameter>();
    Assert.That(sqlParameter.DbType, Is.EqualTo(DbType.Object));
    Assert.That(sqlParameter.SqlDbType, Is.EqualTo(SqlDbType.Structured));
    Assert.That(sqlParameter.TypeName, Is.EqualTo(tvpValue.TableTypeName));
    Assert.That(sqlParameter.Value, Is.Null);
  }
}
