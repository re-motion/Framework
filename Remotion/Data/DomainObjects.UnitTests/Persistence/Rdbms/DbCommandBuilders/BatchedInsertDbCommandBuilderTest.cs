// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Data.Common;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DbCommandBuilders;

[TestFixture]
public class BatchedInsertDbCommandBuilderTest : StandardMappingTest
{
  private Mock<ISqlDialect> _sqlDialectStub;
  private Mock<DbParameter> _dbDataParameterStub;
  private Mock<DbParameterCollection> _dataParameterCollectionMock;
  private Mock<DbCommand> _dbCommandStub;
  private Mock<IDbCommandFactory> _dbCommandFactoryStub;

  public override void SetUp ()
  {
    base.SetUp();

    _sqlDialectStub = new Mock<ISqlDialect>();
    _sqlDialectStub.Setup(stub => stub.StatementDelimiter).Returns(";");
    _sqlDialectStub.Setup(stub => stub.DelimitIdentifier(It.IsAny<string>())).Returns<string>((x) => $"[{x}]");
    _sqlDialectStub.Setup(stub => stub.GetParameterName(It.IsAny<string>())).Returns<string>((x) => $"@{x}");

    _dbDataParameterStub = new Mock<DbParameter>();
    _dataParameterCollectionMock = new Mock<DbParameterCollection>(MockBehavior.Strict);

    _dbCommandStub = new Mock<DbCommand>();
    _dbCommandStub.Protected().Setup<DbParameter>("CreateDbParameter").Returns(_dbDataParameterStub.Object);
    _dbCommandStub.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(_dataParameterCollectionMock.Object);
    _dbCommandStub.SetupProperty(stub => stub.CommandText);

    _dbCommandFactoryStub = new Mock<IDbCommandFactory>();
    _dbCommandFactoryStub.Setup(stub => stub.CreateDbCommand()).Returns(_dbCommandStub.Object);
  }

  [Test]
  public void Create_WithOneTableAndDefaultSchema ()
  {
    _dataParameterCollectionMock.Setup(stub => stub.Add(_dbDataParameterStub.Object)).Returns(1);

    var tableDefinition = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition(null, "Table"));

    var commandSpecification = new Mock<IBatchedCommandSpecification>(MockBehavior.Strict);
    commandSpecification.Setup(stub => stub.Columns).Returns(["col1", "col2"]);
    commandSpecification.Setup(stub => stub.TableDefinition).Returns(tableDefinition);
    commandSpecification.Setup(stub => stub.CreateDbParameter(It.IsAny<DbCommand>(), "@TVP_Insert_Table")).Returns(_dbDataParameterStub.Object);

    var builder = new BatchedInsertDbCommandBuilder(_sqlDialectStub.Object, [commandSpecification.Object]);
    var result = builder.Create(_dbCommandFactoryStub.Object);
    Assert.That(
        result.CommandText,
        Is.EqualTo(
            """
            INSERT INTO [Table] ([col1], [col2])
            SELECT [col1], [col2] FROM @TVP_Insert_Table;
            """));
  }

  [Test]
  public void Create_WithOneTableCustomSchema ()
  {
    _sqlDialectStub.Setup(stub => stub.DelimitIdentifier(It.IsAny<string>())).Returns<string>((x) => $"[{x}]");
    _dataParameterCollectionMock.Setup(stub => stub.Add(_dbDataParameterStub.Object)).Returns(1);

    var tableDefinition = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition("customscheme", "Table"));

    var commandSpecification = new Mock<IBatchedCommandSpecification>(MockBehavior.Strict);
    commandSpecification.Setup(stub => stub.Columns).Returns(["col1", "col2"]);
    commandSpecification.Setup(stub => stub.TableDefinition).Returns(tableDefinition);
    commandSpecification.Setup(stub => stub.CreateDbParameter(It.IsAny<DbCommand>(), "@TVP_Insert_Table")).Returns(_dbDataParameterStub.Object);

    var builder = new BatchedInsertDbCommandBuilder(_sqlDialectStub.Object, [commandSpecification.Object]);
    var result = builder.Create(_dbCommandFactoryStub.Object);
    Assert.That(
        result.CommandText,
        Is.EqualTo(
            """
            INSERT INTO [customscheme].[Table] ([col1], [col2])
            SELECT [col1], [col2] FROM @TVP_Insert_Table;
            """));
  }

  [Test]
  public void Create_WithMultipleTablesDefaultSchema ()
  {
    _sqlDialectStub.Setup(stub => stub.DelimitIdentifier(It.IsAny<string>())).Returns<string>((x) => $"[{x}]");
    _dataParameterCollectionMock.Setup(stub => stub.Add(_dbDataParameterStub.Object)).Returns(1);

    var tableDefinition1 = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition(null, "Table1"));
    var tableDefinition2 = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition(null, "Table2"));
    var tableDefinition3 = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition(null, "Table3"));

    var commandSpecifications = new List<IBatchedCommandSpecification>();
    foreach (var td in new List<TableDefinition> { tableDefinition1, tableDefinition2, tableDefinition3 })
    {
      var commandSpecification = new Mock<IBatchedCommandSpecification>(MockBehavior.Strict);
      commandSpecification.Setup(stub => stub.Columns).Returns(["col1", "col2"]);
      commandSpecification.Setup(stub => stub.TableDefinition).Returns(td);
      commandSpecification.Setup(stub => stub.CreateDbParameter(It.IsAny<DbCommand>(), _sqlDialectStub.Object.GetParameterName("TVP_Insert_" + td.TableName.EntityName)))
          .Returns(_dbDataParameterStub.Object);
      commandSpecifications.Add(commandSpecification.Object);
    }

    var builder = new BatchedInsertDbCommandBuilder(_sqlDialectStub.Object, commandSpecifications.ToArray());
    var result = builder.Create(_dbCommandFactoryStub.Object);
    Assert.That(
        result.CommandText,
        Is.EqualTo(
            """
            INSERT INTO [Table1] ([col1], [col2])
            SELECT [col1], [col2] FROM @TVP_Insert_Table1;
            INSERT INTO [Table2] ([col1], [col2])
            SELECT [col1], [col2] FROM @TVP_Insert_Table2;
            INSERT INTO [Table3] ([col1], [col2])
            SELECT [col1], [col2] FROM @TVP_Insert_Table3;
            """));
  }

  [Test]
  public void Create_WithMultipleTablesCustomSchema ()
  {
    _sqlDialectStub.Setup(stub => stub.DelimitIdentifier(It.IsAny<string>())).Returns<string>((x) => $"[{x}]");
    _dataParameterCollectionMock.Setup(stub => stub.Add(_dbDataParameterStub.Object)).Returns(1);

    var tableDefinition1 = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition("customscheme", "Table1"));
    var tableDefinition2 = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition("customscheme", "Table2"));
    var tableDefinition3 = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition("customscheme", "Table3"));

    var commandSpecifications = new List<IBatchedCommandSpecification>();
    foreach (var td in new List<TableDefinition> { tableDefinition1, tableDefinition2, tableDefinition3 })
    {
      var commandSpecification = new Mock<IBatchedCommandSpecification>(MockBehavior.Strict);
      commandSpecification.Setup(stub => stub.Columns).Returns(["col1", "col2"]);
      commandSpecification.Setup(stub => stub.TableDefinition).Returns(td);
      commandSpecification.Setup(stub => stub.CreateDbParameter(It.IsAny<DbCommand>(), _sqlDialectStub.Object.GetParameterName("TVP_Insert_" + td.TableName.EntityName)))
          .Returns(_dbDataParameterStub.Object);
      commandSpecifications.Add(commandSpecification.Object);
    }

    var builder = new BatchedInsertDbCommandBuilder(_sqlDialectStub.Object, commandSpecifications.ToArray());
    var result = builder.Create(_dbCommandFactoryStub.Object);
    Assert.That(
        result.CommandText,
        Is.EqualTo(
            """
            INSERT INTO [customscheme].[Table1] ([col1], [col2])
            SELECT [col1], [col2] FROM @TVP_Insert_Table1;
            INSERT INTO [customscheme].[Table2] ([col1], [col2])
            SELECT [col1], [col2] FROM @TVP_Insert_Table2;
            INSERT INTO [customscheme].[Table3] ([col1], [col2])
            SELECT [col1], [col2] FROM @TVP_Insert_Table3;
            """));
  }
}
