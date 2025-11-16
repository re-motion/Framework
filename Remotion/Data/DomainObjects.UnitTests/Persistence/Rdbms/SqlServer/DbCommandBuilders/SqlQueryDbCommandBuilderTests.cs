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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Parameters;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.DbCommandBuilders;

[TestFixture]
public class SqlQueryDbCommandBuilderTests
{
  [Test]
  public void Create_WithTableValuedParameter_UsesTempTable ()
  {
    var records = new[]
                  {
                      new Guid("280D68FF-BEA9-47CE-BAEC-44569517829D"),
                      new Guid("6AE44A0F-2345-49AA-95C0-ABB8E968E5CA"),
                      new Guid("4F5D0D1E-EF63-4D3A-9519-585FF464477B")
                  };

    var parameterDefinition = GetTvpParameterDefinition(StorageTypeInformationObjectMother.CreateUniqueIdentifierStorageTypeInformation());
    var commandBuilder = new SqlQueryDbCommandBuilder(
        "SELECT * FROM [MyTable] WHERE MyRefID IN (SELECT [Value] FROM @tvp)",
        [new QueryParameterWithDataParameterDefinition(new QueryParameter("@tvp", records), parameterDefinition)],
        new SqlDialect());

    var commandFactoryStub = new Mock<IDbCommandFactory>();
    commandFactoryStub.Setup(stub => stub.CreateDbCommand()).Returns(new SqlCommand());

    var result = commandBuilder.Create(commandFactoryStub.Object);

    Assert.That(
        result.CommandText,
        Is.EqualTo("""
                   SELECT [Value] INTO [#@tvp] FROM @tvp;
                   CREATE NONCLUSTERED INDEX [IX_@tvp] ON [#@tvp] ([Value]);

                   SELECT * FROM [MyTable] WHERE MyRefID IN (SELECT [Value] FROM [#@tvp])

                   DROP TABLE [#@tvp];

                   """.ReplaceLineEndings()));
  }

  [Test]
  public void Create_WithoutTableValuedParameter_DoesNotUseTempTable ()
  {
    var storageTypeInformation = StorageTypeInformationObjectMother.CreateIntStorageTypeInformation();
    var intParameterDefinition = new SimpleDataParameterDefinition(storageTypeInformation);
    var commandBuilder = new SqlQueryDbCommandBuilder(
        "SELECT * FROM [MyTable] WHERE MyNumber = @int",
        [new QueryParameterWithDataParameterDefinition(new QueryParameter("@int", 42), intParameterDefinition)],
        new SqlDialect());

    var commandFactoryStub = new Mock<IDbCommandFactory>();
    commandFactoryStub.Setup(stub => stub.CreateDbCommand()).Returns(new SqlCommand());

    var result = commandBuilder.Create(commandFactoryStub.Object);

    Assert.That(result.CommandText, Is.EqualTo("SELECT * FROM [MyTable] WHERE MyNumber = @int"));
  }

  [Test]
  public void Create_WithSimpleAndTableValuedParameters_UsesSeparateTempTableForEachTVP ()
  {
    var guidRecords = new[]
                  {
                      new Guid("280D68FF-BEA9-47CE-BAEC-44569517829D"),
                      new Guid("6AE44A0F-2345-49AA-95C0-ABB8E968E5CA"),
                      new Guid("4F5D0D1E-EF63-4D3A-9519-585FF464477B")
                  };

    var intRecords = new[] { 8, 15, 42 };

    var tvpGuidParameterDefinition = GetTvpParameterDefinition(StorageTypeInformationObjectMother.CreateUniqueIdentifierStorageTypeInformation());
    var tvpIntParameterDefinition = GetTvpParameterDefinition(StorageTypeInformationObjectMother.CreateIntStorageTypeInformation());
    var dateTimeParameterDefinition = new SimpleDataParameterDefinition(StorageTypeInformationObjectMother.CreateDateTimeStorageTypeInformation());
    var intParameterDefinition = new SimpleDataParameterDefinition(StorageTypeInformationObjectMother.CreateIntStorageTypeInformation());

    var commandBuilder = new SqlQueryDbCommandBuilder(
        """
        SELECT * 
        FROM [MyTable]
        WHERE MyRefID IN (SELECT [Value] FROM @tvpGuid) 
        AND MyDate < @dateTime 
        AND (
            MyNumber = @int OR 
            MyOtherNumber IN (SELECT [Value] FROM @tvpInt)
        )
        """.ReplaceLineEndings(),
        [
            new QueryParameterWithDataParameterDefinition(new QueryParameter("@dateTime", DateTime.Now), dateTimeParameterDefinition),
            new QueryParameterWithDataParameterDefinition(new QueryParameter("@tvpGuid", guidRecords), tvpGuidParameterDefinition),
            new QueryParameterWithDataParameterDefinition(new QueryParameter("@int", 69), intParameterDefinition),
            new QueryParameterWithDataParameterDefinition(new QueryParameter("@tvpInt", intRecords), tvpIntParameterDefinition)
        ],
        new SqlDialect());

    var commandFactoryStub = new Mock<IDbCommandFactory>();
    commandFactoryStub.Setup(stub => stub.CreateDbCommand()).Returns(new SqlCommand());

    var result = commandBuilder.Create(commandFactoryStub.Object);

    Assert.That(
        result.CommandText,
        Is.EqualTo("""
                   SELECT [Value] INTO [#@tvpGuid] FROM @tvpGuid;
                   CREATE NONCLUSTERED INDEX [IX_@tvpGuid] ON [#@tvpGuid] ([Value]);
                   SELECT [Value] INTO [#@tvpInt] FROM @tvpInt;
                   CREATE NONCLUSTERED INDEX [IX_@tvpInt] ON [#@tvpInt] ([Value]);

                   SELECT * 
                   FROM [MyTable]
                   WHERE MyRefID IN (SELECT [Value] FROM [#@tvpGuid]) 
                   AND MyDate < @dateTime 
                   AND (
                       MyNumber = @int OR 
                       MyOtherNumber IN (SELECT [Value] FROM [#@tvpInt])
                   )

                   DROP TABLE [#@tvpGuid];
                   DROP TABLE [#@tvpInt];

                   """.ReplaceLineEndings()));
  }

  private static SqlTableValuedDataParameterDefinition GetTvpParameterDefinition (StorageTypeInformation storageTypeInformation)
  {
    // set up a table type with a single Guid column
    var storagePropertyDefinition = new SimpleStoragePropertyDefinition(typeof(Guid), new ColumnDefinition("Value", storageTypeInformation, false));
    var tableTypeDefinition = new TableTypeDefinition(new EntityNameDefinition(null, "Test"), [ storagePropertyDefinition ], Array.Empty<ITableConstraintDefinition>());

    // set up a record definition with a single property that uses an integer input as index to the records array
    var recordPropertyDefinition = new FakeRecordPropertyDefinition(storagePropertyDefinition, o => o);
    var recordDefinition = new RecordDefinition("Test", tableTypeDefinition, [ recordPropertyDefinition ]);

    var parameterDefinition = new SqlTableValuedDataParameterDefinition(recordDefinition);
    return parameterDefinition;
  }

  private static IEnumerable<(IRdbmsStoragePropertyDefinition storageProperty, ICollection tvpValue, bool hasIndex)>
      GetTestCasesFor_Create_WithTableValuedParameter_CreatesIndex_DependingOnValueType ()
  {
    yield return (SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Value", StorageTypeInformationObjectMother.CreateUniqueIdentifierStorageTypeInformation()),
        new[] { Guid.NewGuid() }, true);
    yield return (SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Value", StorageTypeInformationObjectMother.CreateBitStorageTypeInformation()), new[] { true },
        true);
    yield return (SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Value", StorageTypeInformationObjectMother.CreateIntStorageTypeInformation()), new[] { 42 },
        true);
    yield return (SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Value", StorageTypeInformationObjectMother.CreateDateTimeStorageTypeInformation()),
        new[] { DateTime.Today }, true);
    yield return (SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Value", StorageTypeInformationObjectMother.CreateVarchar100StorageTypeInformation()),
        new[] { "Short string" }, true);
    yield return (
        SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty(
            "Value",
            StorageTypeInformationObjectMother.CreateStorageTypeInformation(typeof(string), "nvarchar(850)", DbType.String, storageTypeLength: 850)),
        new[] { "Maximum indexed string" },
        true);
    yield return (
        SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty(
            "Value",
            StorageTypeInformationObjectMother.CreateStorageTypeInformation(typeof(string), "nvarchar(851)", DbType.String, storageTypeLength: 851)), new[] { "Long string" },
        false);
    yield return (
        SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty(
            "Value",
            StorageTypeInformationObjectMother.CreateStorageTypeInformation(typeof(string), "nvarchar(max)", DbType.String, storageTypeLength: -1)), new[] { "Max string" },
        false);
    yield return (
        SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty(
            "Value",
            StorageTypeInformationObjectMother.CreateStorageTypeInformation(
                typeof(byte[]),
                "varbinary(1700)",
                DbType.Binary,
                storageTypeLength: 1700,
                dotNetTypeConverter: new DefaultConverter(typeof(byte[])))), new byte[][] { [0, 8, 15] },
        true);
    yield return (
        SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty(
            "Value",
            StorageTypeInformationObjectMother.CreateStorageTypeInformation(
                typeof(byte[]),
                "varbinary(1701)",
                DbType.Binary,
                storageTypeLength: 1701,
                dotNetTypeConverter: new DefaultConverter(typeof(byte[])))), new byte[][] { [0, 8, 15] },
        false);
    yield return (
        SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty(
            "Value",
            StorageTypeInformationObjectMother.CreateStorageTypeInformation(
                typeof(byte[]),
                "varbinary(max)",
                DbType.Binary,
                storageTypeLength: -1,
                dotNetTypeConverter: new DefaultConverter(typeof(byte[])))), new byte[][] { [0, 8, 15] },
        false);
  }

  [Test]
  [TestCaseSource(nameof(GetTestCasesFor_Create_WithTableValuedParameter_CreatesIndex_DependingOnValueType))]
  public void Create_WithTableValuedParameter_CreatesIndex_DependingOnValueType ((IRdbmsStoragePropertyDefinition storageProperty, ICollection tvpValue, bool hasIndex) testCase)
  {
    // set up a table type with a single Guid column
    var storagePropertyDefinition = testCase.storageProperty;
    var tableTypeDefinition = new TableTypeDefinition(new EntityNameDefinition(null, "Test"), [ storagePropertyDefinition ], Array.Empty<ITableConstraintDefinition>());

    // set up a record definition with a single property that uses an integer input as index to the records array
    var recordPropertyDefinition = new FakeRecordPropertyDefinition(storagePropertyDefinition, o => o);
    var recordDefinition = new RecordDefinition("Test", tableTypeDefinition, [ recordPropertyDefinition ]);

    var parameterDefinition = new SqlTableValuedDataParameterDefinition(recordDefinition);
    var commandBuilder = new SqlQueryDbCommandBuilder(
        "SELECT * FROM [MyTable] WHERE MyRefID IN (SELECT [Value] FROM @tvp)",
        [new QueryParameterWithDataParameterDefinition(new QueryParameter("@tvp", testCase.tvpValue), parameterDefinition)],
        new SqlDialect());

    var commandFactoryStub = new Mock<IDbCommandFactory>();
    commandFactoryStub.Setup(stub => stub.CreateDbCommand()).Returns(new SqlCommand());

    var result = commandBuilder.Create(commandFactoryStub.Object);

    Assert.That(
        result.CommandText,
        testCase.hasIndex
            ? Does.Contain("CREATE NONCLUSTERED INDEX [IX_@tvp] ON [#@tvp] ([Value]);")
            : Does.Not.Contain("CREATE NONCLUSTERED INDEX [IX_@tvp] ON [#@tvp] ([Value]);"));
  }
}
